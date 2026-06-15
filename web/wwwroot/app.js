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
}

function setBusy(busy) {
  document.querySelectorAll(".cmd").forEach((b) => (b.disabled = busy));
  $("chat-send").disabled = busy || !hasSession;
  $("chat-input").disabled = busy || !hasSession;
  $("pdf-attach").disabled = busy; // attaching works before a session, but not mid-operation
}

let hasSession = false;
let selectedPdfName = null;

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
  document.querySelectorAll(".pdf-item").forEach(
    (i) => i.classList.toggle("selected", i.textContent === name));
}

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
    $("session-status").textContent = `session: ${data.model}`;
    $("chat-input").focus();
  } catch (err) {
    $("session-status").textContent = "failed";
    addProgressLine("error", `Could not start session: ${err.message}`);
  }
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

$("chat-form").addEventListener("submit", async (e) => {
  e.preventDefault(); // a form submit normally reloads the page — we handle it ourselves
  const text = $("chat-input").value.trim();
  if (!text || !hasSession) return;
  $("chat-input").value = "";
  setBusy(true);

  addChatBubble("user").textContent = text;

  // Everything from here can fail mid-flight (network blip, server restart);
  // the finally block guarantees the input is re-enabled no matter what.
  try {
    const resp = await fetch("/api/chat", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ text }),
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
          reasoningBubble ??= addChatBubble("reasoning");
          reasoningBubble.textContent += payload.text;
        } else if (evt === "token") {
          answerBubble ??= addChatBubble("assistant");
          answerBubble.textContent += payload.text;
        } else if (evt === "error") {
          addChatBubble("error").textContent = `❌ ${payload.text}`;
        }
        scrollChatDown();
      }
    }
  } catch (err) {
    addChatBubble("error").textContent = `❌ ${err.message}`;
  } finally {
    setBusy(false);
    $("chat-input").focus();
  }
});

// --- progress panel ----------------------------------------------------------

$("progress-toggle").addEventListener("click", () => {
  $("progress-panel").classList.toggle("open");
});

function openProgressPanel() {
  $("progress-panel").classList.add("open");
}

// --- pipeline commands -----------------------------------------------------------

document.querySelectorAll(".cmd").forEach((button) => {
  button.addEventListener("click", async () => {
    const cmd = button.dataset.cmd;
    setBusy(true);
    openProgressPanel(); // commands report through the progress log — show it
    addProgressLine("info", `▶ Running ${cmd}…`);
    try {
      const resp = await fetch(`/api/run/${cmd}`, { method: "POST" });
      const data = await resp.json();
      if (!resp.ok) addProgressLine("error", data.error ?? `${cmd} failed.`);
      else if (data.output) addProgressLine("success", `✓ ${cmd} done → ${data.output}`);
      else addProgressLine("success", `✓ ${cmd} done`);
    } catch (err) {
      addProgressLine("error", `${cmd}: ${err.message}`);
    }
    setBusy(false);
  });
});

setBusy(true); // everything stays disabled until init() has restored the server state
init().catch((err) => addProgressLine("error", `Startup failed: ${err.message}`));
