import pytest

from techclass.console import info, subscribe_to_messages, unsubscribe_from_messages
from techclass.web import server
from techclass.workspace import Workspace


@pytest.mark.asyncio
async def test_browser_opens_only_after_server_is_ready(monkeypatch):
    opened = []

    class FakeServer:
        started = False
        should_exit = False

    fake_server = FakeServer()
    monkeypatch.setattr(server.webbrowser, "open", lambda url: opened.append(url) or True)

    task = server.asyncio.create_task(server._open_browser_when_ready(fake_server))
    await server.asyncio.sleep(0)
    assert opened == []

    fake_server.started = True
    await task

    assert opened == [server.WEB_URL]


@pytest.mark.asyncio
async def test_console_messages_are_forwarded_to_web_progress():
    state = server.WebState(object())
    state.loop = server.asyncio.get_running_loop()
    queue = server.asyncio.Queue()
    state.listeners.add(queue)
    subscribe_to_messages(state.forward_console_message)

    try:
        info("Model selected: test-model")
        message = await server.asyncio.wait_for(queue.get(), timeout=1)
    finally:
        unsubscribe_from_messages(state.forward_console_message)

    assert message == {"level": "info", "text": "Model selected: test-model"}


def test_model_progress_distinguishes_selection_and_switch():
    assert server._model_progress_message(None, "gpt-a", "s1") == (
        "🌐 Model selected: gpt-a (session: s1)"
    )
    assert server._model_progress_message("gpt-a", "gpt-b", "s2") == (
        "🔄 Model switched: gpt-a → gpt-b (session: s2)"
    )


def test_python_web_api_exposes_the_legacy_csharp_routes(tmp_path):
    static = tmp_path / "web" / "wwwroot"
    static.mkdir(parents=True)
    (static / "index.html").touch()
    app = server.create_app(Workspace.create(tmp_path, object(), "test-model"))
    paths = {route.path for route in app.routes}

    assert {
        "/api/models",
        "/api/session",
        "/api/state",
        "/api/pdfs",
        "/api/pdfs/upload",
        "/api/select",
        "/api/deselect",
        "/api/chat",
        "/api/run/extraction",
        "/api/run/condense",
        "/api/run/summarize",
        "/api/run/classify",
        "/api/run/condense-check",
        "/api/benchmark/technologies",
        "/api/benchmark/run",
        "/api/output",
        "/api/progress",
        "/api/shutdown",
    } <= paths


def test_first_pdf_question_streams_condensing_status_to_main_chat(monkeypatch, tmp_path):
    from fastapi.testclient import TestClient

    class FakeSession:
        session_id = "web-session"

        async def send(self, _prompt, on_reasoning=None, on_content=None):
            on_content("Answer")
            return "Answer"

        async def close(self):
            pass

    class FakeClient:
        async def create_session(self, _model):
            return FakeSession()

    static = tmp_path / "web" / "wwwroot"
    static.mkdir(parents=True)
    (static / "index.html").touch()
    ws = Workspace.create(tmp_path, FakeClient(), "test-model")
    ws.ensure_directories()
    pdf = ws.pdf_dir / "paper.pdf"
    pdf.touch()

    async def fake_condense(_ws, _pdf):
        condensed = tmp_path / "condensed.md"
        condensed.write_text("Condensed PDF text", encoding="utf-8")
        return condensed

    monkeypatch.setattr(server, "condense", fake_condense)
    app = server.create_app(ws)

    with TestClient(app) as client:
        assert client.post("/api/session", json={"model": "test-model"}).is_success
        assert client.post("/api/select", json={"name": pdf.name}).is_success

        first = client.post("/api/chat", json={"text": "What is this paper about?"}).text
        second = client.post("/api/chat", json={"text": "Tell me more."}).text

    assert "event: status" in first
    assert "Preparing paper.pdf: condensing PDF for context" in first
    assert "PDF context ready" in first
    assert "event: token" in first
    assert "event: status" not in second
