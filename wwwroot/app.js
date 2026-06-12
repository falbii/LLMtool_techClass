// All the page logic: talk to the local .NET server (same address the page
// came from) and update the page with the results. No frameworks — just
// fetch() for HTTP requests and DOM updates.

// --- small helpers -----------------------------------------------------------

const $ = (id) => document.getElementById(id);

function addChatBubble(cssClass) {
  const div = document.createElement("div");
  div.className = `bubble ${cssClass}`;
  $("chat").appendChild(div);
  $("chat").scrollTop = $("chat").scrollHeight;
  return div;
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
}

let hasSession = false;

// --- startup -----------------------------------------------------------------

async function init() {
  // Fill the model dropdown.
  const models = await (await fetch("/api/models")).json();
  $("model-select").replaceChildren(
    ...models.map((m) => {
      const o = document.createElement("option");
      o.value = m.id;
      o.textContent = m.supportsReasoning ? `${m.id}  💭` : m.id;
      return o;
    })
  );

  await refreshPdfList();

  // Live progress lines from the server (mirrors the terminal output).
  const progress = new EventSource("/api/progress");
  progress.addEventListener("log", (e) => {
    const { level, text } = JSON.parse(e.data);
    addProgressLine(level, text);
  });
}

async function refreshPdfList(selectName) {
  const pdfs = await (await fetch("/api/pdfs")).json();
  const select = $("pdf-select");
  select.replaceChildren(new Option("— none —", ""));
  pdfs.forEach((name) => select.appendChild(new Option(name, name)));
  if (selectName) select.value = selectName;
}

// --- setup bar actions ---------------------------------------------------------

$("start-session").addEventListener("click", async () => {
  const model = $("model-select").value;
  $("session-status").textContent = "starting…";
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
});

$("pdf-select").addEventListener("change", async () => {
  const name = $("pdf-select").value;
  if (!name) return;
  await fetch("/api/select", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name }),
  });
  addProgressLine("info", `Selected PDF: ${name}`);
});

$("pdf-file").addEventListener("change", async () => {
  const file = $("pdf-file").files[0];
  if (!file) return;
  const form = new FormData();
  form.append("file", file);
  const resp = await fetch("/api/pdfs/upload", { method: "POST", body: form });
  const data = await resp.json();
  if (!resp.ok) {
    addProgressLine("error", data.error ?? "Upload failed.");
    return;
  }
  await refreshPdfList(data.name);
  $("pdf-file").value = "";
});

// --- chat ----------------------------------------------------------------------

$("chat-form").addEventListener("submit", async (e) => {
  e.preventDefault(); // a form submit normally reloads the page — we handle it ourselves
  const text = $("chat-input").value.trim();
  if (!text || !hasSession) return;
  $("chat-input").value = "";
  setBusy(true);

  addChatBubble("user").textContent = text;

  const resp = await fetch("/api/chat", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ text }),
  });

  if (!resp.ok) {
    const data = await resp.json().catch(() => ({}));
    addChatBubble("error").textContent = data.error ?? "Request failed.";
    setBusy(false);
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
      $("chat").scrollTop = $("chat").scrollHeight;
    }
  }
  setBusy(false);
  $("chat-input").focus();
});

// --- pipeline commands -----------------------------------------------------------

document.querySelectorAll(".cmd").forEach((button) => {
  button.addEventListener("click", async () => {
    const cmd = button.dataset.cmd;
    setBusy(true);
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

init();
setBusy(true); // chat and commands stay disabled until a session is started
