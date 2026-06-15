// All the page logic: talk to the local .NET server (same address the page
// came from) and update the page with the results. No frameworks — just
// fetch() for HTTP requests and DOM updates.

// --- small helpers -----------------------------------------------------------

const $ = (id) => document.getElementById(id);

function addChatBubble(cssClass) {
  const div = document.createElement("div");
  div.className = `bubble ${cssClass}`;
  $("chat").appendChild(div);
  scrollChatDown();
  return div;
}

function scrollChatDown() {
  const wrap = $("chat-wrap");
  wrap.scrollTop = wrap.scrollHeight;
}

function addProgressLine(level, text) {
  const div = document.createElement("div");
  div.className = `log ${level}`;
  div.textContent = text;
  $("progress").appendChild(div);
  $("progress").scrollTop = $("progress").scrollHeight;
  // While a pipeline command runs, mirror its progress live into the chat bubble.
  if (cmdBubble) appendCmdStep(cmdBubble, level, text);
}

function setBusy(busy) {
  document.querySelectorAll(".cmd").forEach((b) => (b.disabled = busy));
  $("chat-send").disabled = busy || !hasSession;
  $("chat-input").disabled = busy || !hasSession;
  $("pdf-attach").disabled = busy; // attaching works before a session, but not mid-operation
  $("pdf-clear").disabled = busy;  // don't let the PDF be detached mid-operation
}

let hasSession = false;
let selectedPdfName = null;
// Non-null while a pipeline command (summarize/classify/…) runs; its presence
// makes addProgressLine mirror each step into this chat bubble.
let cmdBubble = null;

// --- startup -----------------------------------------------------------------

async function init() {
  // Live progress lines from the server (mirrors the terminal output).
  // Started first so startup errors below are visible in the panel too.
  const progress = new EventSource("/api/progress");
  progress.addEventListener("log", (e) => {
    const { level, text } = JSON.parse(e.data);
    addProgressLine(level, text);
  });

  // Fill the model dropdown.
  try {
    const models = await (await fetch("/api/models")).json();
    $("model-select").replaceChildren(
      ...models.map((m) => {
        const o = document.createElement("option");
        o.value = m.id;
        o.textContent = m.supportsReasoning ? `${m.id}  💭` : m.id;
        return o;
      })
    );
  } catch (err) {
    $("model-select").replaceChildren(new Option("⚠ models failed to load", ""));
    addProgressLine("error", `Could not load models: ${err.message} — reload the page to retry.`);
  }

  await refreshPdfList();

  // Restore server-side state, so a page reload doesn't look like a lost
  // session: the server keeps the session, selected PDF and busy flag.
  let busy = false;
  try {
    const st = await (await fetch("/api/state")).json();
    if (st.hasSession) {
      hasSession = true;
      $("session-status").textContent = `session: ${st.model}`;
      if (st.model) $("model-select").value = st.model;
      setSessionButtonStarted(true);
      collapseHeaderForSession();
    }
    if (st.selectedPdf) showSelectedPdf(st.selectedPdf);
    busy = st.busy;
  } catch {
    // State restore is best-effort; the page still works without it.
  }

  if (busy) {
    addProgressLine("info", "An operation is still running on the server…");
    openProgressPanel();
    await waitUntilIdle();
  }
  setBusy(!hasSession); // with a session everything unlocks; without one only setup works
}

// Polls /api/state until the server-side busy flag clears (used after a page
// reload that lands in the middle of a long pipeline run).
async function waitUntilIdle() {
  while (true) {
    await new Promise((r) => setTimeout(r, 2000));
    try {
      const st = await (await fetch("/api/state")).json();
      if (!st.busy) return;
    } catch {
      return; // server gone — nothing left to wait for
    }
  }
}

async function refreshPdfList() {
  const pdfs = await (await fetch("/api/pdfs")).json();
  const list = $("pdf-list");
  list.replaceChildren();
  if (pdfs.length === 0) {
    const empty = document.createElement("div");
    empty.className = "pdf-empty";
    empty.textContent = "No PDFs yet — upload one below.";
    list.appendChild(empty);
    return;
  }
  pdfs.forEach((name) => {
    const item = document.createElement("button");
    item.type = "button";
    item.className = "pdf-item" + (name === selectedPdfName ? " selected" : "");
    item.textContent = name;
    item.addEventListener("click", () => selectPdf(name));
    list.appendChild(item);
  });
}

// Shows the attached file's name on the attach button and ticks it in the menu.
function showSelectedPdf(name) {
  selectedPdfName = name;
  $("pdf-attach-label").textContent = name ?? "PDF";
  $("pdf-attach").classList.toggle("attached", !!name);
  $("pdf-clear").hidden = !name;
  document.querySelectorAll(".pdf-item").forEach(
    (i) => i.classList.toggle("selected", i.textContent === name));
}

// Detaches the current PDF (clears it server-side too, so a reload stays clear).
$("pdf-clear").addEventListener("click", async (e) => {
  e.stopPropagation(); // don't let the click bubble up and open the menu
  try {
    await fetch("/api/deselect", { method: "POST" });
  } catch (err) {
    addProgressLine("error", `Could not remove the PDF: ${err.message}`);
  }
  showSelectedPdf(null);
  addProgressLine("info", "Removed the attached PDF.");
});

async function selectPdf(name) {
  try {
    const resp = await fetch("/api/select", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name }),
    });
    const data = await resp.json();
    if (!resp.ok) {
      addProgressLine("error", data.error ?? "Could not select PDF.");
      return;
    }
    showSelectedPdf(data.name);
    addProgressLine("info", `Selected PDF: ${data.name}`);
  } catch (err) {
    addProgressLine("error", `Could not select PDF: ${err.message}`);
  } finally {
    closePdfMenu();
  }
}

function closePdfMenu() {
  $("pdf-menu").classList.remove("open");
}

// --- setup bar actions ---------------------------------------------------------

// Reflects whether a session is live on the start button: relabels to
// "Session started" and fades it (it stays clickable, to start a fresh session).
function setSessionButtonStarted(started) {
  const btn = $("start-session");
  btn.classList.toggle("started", started);
  btn.textContent = started ? "Session started" : "Start session";
  // No point re-starting the same model: lock the button until the model changes
  // (the model-select change handler calls this with false to re-enable it).
  btn.disabled = started;
}

// Records the header's height in a CSS var so the toggle tab can sit flush under
// the bar when it's open. Only measures while the header is visible (its height
// is 0 when collapsed), keeping the last known value otherwise.
function syncHeaderHeight() {
  const h = document.querySelector("header").offsetHeight;
  if (h) document.body.style.setProperty("--header-h", `${h}px`);
}

// Reveals the collapse tab and folds the top bar away once a session is live,
// leaving the small arrow as the way to bring it back.
function collapseHeaderForSession() {
  syncHeaderHeight(); // capture the height before hiding the bar
  document.body.classList.add("session-active", "header-collapsed");
}

$("header-toggle").addEventListener("click", () => {
  document.body.classList.toggle("header-collapsed");
  if (!document.body.classList.contains("header-collapsed")) syncHeaderHeight();
});

// Re-measure when the bar may have reflowed to a different height.
window.addEventListener("resize", () => {
  if (!document.body.classList.contains("header-collapsed")) syncHeaderHeight();
});

$("start-session").addEventListener("click", async () => {
  const model = $("model-select").value;
  if (!model) return;
  $("session-status").textContent = "starting…";
  try {
    const resp = await fetch("/api/session", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ model }),
    });
    const data = await resp.json();
    if (!resp.ok) {
      $("session-status").textContent = "failed";
      addProgressLine("error", data.error ?? "Could not start session.");
      return;
    }
    hasSession = true;
    setBusy(false);
    setSessionButtonStarted(true);
    collapseHeaderForSession();
    $("session-status").textContent = `session: ${data.model}`;
    $("chat-input").focus();
  } catch (err) {
    $("session-status").textContent = "failed";
    addProgressLine("error", `Could not start session: ${err.message}`);
  }
});

// Picking a different model means the next click starts a new session, so
// restore the button to its default look as a cue.
$("model-select").addEventListener("change", () => setSessionButtonStarted(false));

// Blunt "Ctrl+C": shut the whole server down. Anything running is terminated and
// the app must be relaunched — so confirm through the dialog first.
$("stop-server").addEventListener("click", () => $("confirm-overlay").hidden = false);
$("confirm-cancel").addEventListener("click", () => $("confirm-overlay").hidden = true);
// Clicking the dimmed backdrop (but not the box) also cancels.
$("confirm-overlay").addEventListener("click", (e) => {
  if (e.target === $("confirm-overlay")) $("confirm-overlay").hidden = true;
});

$("confirm-stop").addEventListener("click", async () => {
  $("confirm-overlay").hidden = true;
  try {
    await fetch("/api/shutdown", { method: "POST" });
  } catch {
    // The server may drop the connection as it stops — that's expected.
  }
  document.body.classList.add("server-stopped");
  $("session-status").textContent = "server stopped";
  addProgressLine("warn", "🛑 Server stopped — relaunch the app to continue.");
  openProgressPanel();
});

// --- paperclip menu (PDF select / upload) ---------------------------------

$("pdf-attach").addEventListener("click", async () => {
  const menu = $("pdf-menu");
  if (menu.classList.contains("open")) {
    closePdfMenu();
    return;
  }
  await refreshPdfList(); // pick up files dropped into the folder meanwhile
  menu.classList.add("open");
});

// Clicking anywhere outside the paperclip area closes the menu.
document.addEventListener("click", (e) => {
  if (!$("pdf-attach-wrap").contains(e.target)) closePdfMenu();
});

$("pdf-upload-item").addEventListener("click", () => $("pdf-file").click());

$("pdf-file").addEventListener("change", async () => {
  const file = $("pdf-file").files[0];
  if (!file) return;
  try {
    const form = new FormData();
    form.append("file", file);
    const resp = await fetch("/api/pdfs/upload", { method: "POST", body: form });
    const data = await resp.json();
    if (!resp.ok) {
      addProgressLine("error", data.error ?? "Upload failed.");
      return;
    }
    await refreshPdfList();
    showSelectedPdf(data.name); // the server selects an uploaded PDF automatically
    addProgressLine("success", `Uploaded: ${data.name}`);
  } catch (err) {
    addProgressLine("error", `Upload failed: ${err.message}`);
  } finally {
    $("pdf-file").value = "";
    closePdfMenu();
  }
});

// --- chat ----------------------------------------------------------------------

// Non-null only while a chat answer is streaming; its presence is what turns the
// send button into a Stop button (see the click handler and setChatGenerating).
let chatAbort = null;

// Appends the animated "thinking" droplet on the assistant side. Removed as soon
// as the first reasoning/answer token arrives (or the request ends).
function addThinkingIndicator() {
  if ($("thinking")) return;
  const wrap = document.createElement("div");
  wrap.id = "thinking";
  wrap.className = "thinking";
  wrap.appendChild(Object.assign(document.createElement("div"), { className: "blob" }));
  $("chat").appendChild(wrap);
  scrollChatDown();
}

function removeThinkingIndicator() {
  $("thinking")?.remove();
}

// Toggles the page into/out of "answer streaming" mode. Unlike setBusy, this
// keeps the send button enabled — as a Stop button — so the user can abort.
function setChatGenerating(generating) {
  document.querySelectorAll(".cmd").forEach((b) => (b.disabled = generating));
  $("pdf-attach").disabled = generating;
  $("pdf-clear").disabled = generating; // can't detach while the model is answering
  $("chat-input").disabled = generating || !hasSession;
  const send = $("chat-send");
  send.classList.toggle("stop", generating);
  send.title = generating ? "Stop generating" : "Send";
  send.disabled = generating ? false : !hasSession;
}

// While generating, the send button aborts the request instead of submitting a
// new one. preventDefault stops the click from also submitting the form.
$("chat-send").addEventListener("click", (e) => {
  if (chatAbort) {
    e.preventDefault();
    chatAbort.abort();
  }
});

$("chat-form").addEventListener("submit", async (e) => {
  e.preventDefault(); // a form submit normally reloads the page — we handle it ourselves
  if (chatAbort) return; // already streaming; the button is acting as Stop
  const text = $("chat-input").value.trim();
  if (!text || !hasSession) return;
  $("chat-input").value = "";

  chatAbort = new AbortController();
  setChatGenerating(true);
  addChatBubble("user").textContent = text;
  addThinkingIndicator();

  // Everything from here can fail mid-flight (network blip, server restart, or
  // the user hitting Stop); the finally block guarantees the UI is reset.
  try {
    const resp = await fetch("/api/chat", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ text }),
      signal: chatAbort.signal,
    });

    if (!resp.ok) {
      const data = await resp.json().catch(() => ({}));
      addChatBubble("error").textContent = data.error ?? "Request failed.";
      return;
    }

    // The answer streams down as SSE frames: "event: <name>\ndata: <json>\n\n".
    // Read the body chunk by chunk and append tokens as they arrive.
    let reasoningBubble = null;
    let answerBubble = null;
    const reader = resp.body.getReader();
    const decoder = new TextDecoder();
    let buffer = "";

    while (true) {
      const { value, done } = await reader.read();
      if (done) break;
      buffer += decoder.decode(value, { stream: true });

      let sep;
      while ((sep = buffer.indexOf("\n\n")) >= 0) {
        const frame = buffer.slice(0, sep);
        buffer = buffer.slice(sep + 2);

        let evt = "message";
        let data = "";
        for (const line of frame.split("\n")) {
          if (line.startsWith("event: ")) evt = line.slice(7);
          else if (line.startsWith("data: ")) data = line.slice(6);
        }
        const payload = data ? JSON.parse(data) : {};

        if (evt === "reasoning") {
          removeThinkingIndicator(); // real output is arriving now
          reasoningBubble ??= addChatBubble("reasoning");
          reasoningBubble.textContent += payload.text;
        } else if (evt === "token") {
          removeThinkingIndicator();
          answerBubble ??= addChatBubble("assistant");
          answerBubble.textContent += payload.text;
        } else if (evt === "error") {
          removeThinkingIndicator();
          addChatBubble("error").textContent = `❌ ${payload.text}`;
        }
        scrollChatDown();
      }
    }
  } catch (err) {
    if (err.name === "AbortError")
      addChatBubble("note").textContent = "⏹ Stopped.";
    else
      addChatBubble("error").textContent = `❌ ${err.message}`;
  } finally {
    removeThinkingIndicator();
    chatAbort = null;
    setChatGenerating(false);
    $("chat-input").focus();
  }
});

// --- progress panel ----------------------------------------------------------

$("progress-toggle").addEventListener("click", () => {
  $("progress-panel").classList.toggle("open");
});

$("progress-close").addEventListener("click", () => {
  $("progress-panel").classList.remove("open");
});

function openProgressPanel() {
  $("progress-panel").classList.add("open");
}

// --- pipeline commands -----------------------------------------------------------

// A chat bubble that shows a pipeline run live: a title with a spinner, plus the
// progress steps streamed in as they happen (fed by addProgressLine while this
// is the active cmdBubble).
function addCmdBubble(label) {
  const bubble = addChatBubble("pipeline running");
  const title = document.createElement("div");
  title.className = "pipeline-title";
  title.append(
    // morphing droplet (same "thinking" look), tinted to the command's glow colour
    Object.assign(document.createElement("span"), { className: "pipeline-blob" }),
    Object.assign(document.createElement("span"), { textContent: label }));
  const steps = document.createElement("div");
  steps.className = "pipeline-steps";
  bubble.append(title, steps);
  bubble._steps = steps;
  return bubble;
}

function appendCmdStep(bubble, level, text) {
  const line = document.createElement("div");
  line.className = `pipeline-step ${level}`;
  line.textContent = text;
  bubble._steps.appendChild(line);
  scrollChatDown();
}

// Stops the spinner and marks the title with a ✓ / ✗ once the run ends.
function finishCmdBubble(bubble, ok) {
  bubble.classList.remove("running");
  bubble.classList.add(ok ? "ok" : "failed");
}

document.querySelectorAll(".cmd").forEach((button) => {
  button.addEventListener("click", async () => {
    const cmd = button.dataset.cmd;
    setBusy(true);
    document.body.classList.add(`glow-${cmd}`); // tints the corner glow per command
    cmdBubble = addCmdBubble(button.textContent.trim());
    addProgressLine("info", `▶ Running ${cmd}…`);
    try {
      const resp = await fetch(`/api/run/${cmd}`, { method: "POST" });
      const data = await resp.json();
      if (!resp.ok) {
        addProgressLine("error", data.error ?? `${cmd} failed.`);
        finishCmdBubble(cmdBubble, false);
      } else {
        addProgressLine("success", data.output ? `✓ ${cmd} done → ${data.output}` : `✓ ${cmd} done`);
        finishCmdBubble(cmdBubble, true);
      }
    } catch (err) {
      addProgressLine("error", `${cmd}: ${err.message}`);
      finishCmdBubble(cmdBubble, false);
    } finally {
      cmdBubble = null;
      document.body.classList.remove(`glow-${cmd}`);
      setBusy(false);
    }
  });
});

setBusy(true); // everything stays disabled until init() has restored the server state
init().catch((err) => addProgressLine("error", `Startup failed: ${err.message}`));
