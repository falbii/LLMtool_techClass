"""FastAPI server for the local browser interface."""

from __future__ import annotations

import asyncio
import json
import shutil
from contextlib import asynccontextmanager
from pathlib import Path
from typing import Any

from ..helpers.benchmark import SELECTION_COUNT, run_benchmark
from ..core.pdf import build_single_document_prompt, split_into_chunks
from ..core.pipeline import check_condensation, classify, condense, extract_all, find_technologies, invalidate_cached_artifacts, summarize
from ..workspace import Workspace


class WebState:
    """Mutable session state for a single local web server process."""

    def __init__(self, ws: Workspace) -> None:
        self.ws = ws
        self.session: Any = None
        self.selected_pdf: Path | None = None
        self.pdf_context_sent = False
        self.busy = False
        self.lock = asyncio.Lock()
        self.listeners: set[asyncio.Queue[dict[str, str]]] = set()

    async def log(self, level: str, text: str) -> None:
        print(text)
        for queue in tuple(self.listeners):
            queue.put_nowait({"level": level, "text": text})


def create_app(ws: Workspace):
    """Create the FastAPI application around a prepared workspace."""

    try:
        from fastapi import FastAPI, File, HTTPException, UploadFile
        from fastapi.responses import FileResponse, JSONResponse, PlainTextResponse, StreamingResponse
        from fastapi.staticfiles import StaticFiles
    except ImportError as exc:
        raise RuntimeError("Web dependencies are missing. Run: pip install -e .[web]") from exc

    state = WebState(ws)

    @asynccontextmanager
    async def lifespan(_: Any):
        yield
        if state.session:
            await state.session.close()

    app = FastAPI(lifespan=lifespan)

    @app.get("/api/models")
    async def models():
        return [{"id": item.id, "supportsReasoning": item.supports_reasoning} for item in await ws.client.list_models()]

    @app.post("/api/session")
    async def start_session(request: dict[str, str]):
        model = request.get("model", "").strip()
        if not model:
            raise HTTPException(400, "Model is required")
        if state.session:
            await state.session.close()
        ws.model = model
        state.session = await ws.client.create_session(model)
        state.pdf_context_sent = False
        return {"sessionId": state.session.session_id, "model": model}

    @app.get("/api/state")
    async def get_state():
        return {"hasSession": state.session is not None, "model": ws.model if state.session else None,
                "selectedPdf": state.selected_pdf.name if state.selected_pdf else None, "busy": state.busy}

    @app.get("/api/pdfs")
    async def pdfs():
        return [item.name for item in sorted(ws.pdf_dir.glob("*.pdf"), key=lambda p: p.name.lower())]

    @app.post("/api/pdfs/upload")
    async def upload(file: Any = File(...)):
        name = Path(file.filename or "").name
        if not name.lower().endswith(".pdf"):
            raise HTTPException(400, "Only PDF files are accepted")
        target = ws.pdf_dir / name
        invalidate_cached_artifacts(ws, target)
        with target.open("wb") as output:
            shutil.copyfileobj(file.file, output)
        state.selected_pdf, state.pdf_context_sent = target, False
        return {"name": name}

    @app.post("/api/select")
    async def select(request: dict[str, str]):
        target = ws.pdf_dir / Path(request.get("name", "")).name
        if not target.is_file():
            raise HTTPException(404, "PDF not found")
        state.selected_pdf, state.pdf_context_sent = target, False
        return {"name": target.name}

    @app.post("/api/deselect")
    async def deselect():
        state.selected_pdf, state.pdf_context_sent = None, False
        return {"ok": True}

    @app.post("/api/chat")
    async def chat(request: dict[str, str]):
        if state.session is None:
            raise HTTPException(409, "Start a session first")
        text = request.get("text", "").strip()

        async def events():
            try:
                prompt = text
                if state.selected_pdf and not state.pdf_context_sent:
                    path = await condense(ws, state.selected_pdf)
                    prompt = build_single_document_prompt(split_into_chunks(path.read_text(encoding="utf-8-sig")), text)
                    state.pdf_context_sent = True
                queue: asyncio.Queue[tuple[str, str] | None] = asyncio.Queue()
                task = asyncio.create_task(state.session.send(
                    prompt,
                    on_reasoning=lambda value: queue.put_nowait(("reasoning", value)),
                    on_content=lambda value: queue.put_nowait(("token", value)),
                ))
                task.add_done_callback(lambda _: queue.put_nowait(None))
                while (item := await queue.get()) is not None:
                    event, value = item
                    yield f"event: {event}\ndata: {json.dumps({'text': value})}\n\n"
                await task
                yield "event: done\ndata: {}\n\n"
            except Exception as exc:
                yield f"event: error\ndata: {json.dumps({'text': str(exc)})}\n\n"
        return StreamingResponse(events(), media_type="text/event-stream")

    async def gated(operation):
        """Serialize long-running pipeline operations for the selected PDF."""

        if state.session is None:
            raise HTTPException(409, "Start a session first")
        if state.selected_pdf is None:
            raise HTTPException(409, "Select a PDF first")
        if state.lock.locked():
            raise HTTPException(409, "Another operation is running")
        async with state.lock:
            state.busy = True
            try:
                output = await operation(ws, state.selected_pdf)
                return {"output": str(output) if output else None}
            finally:
                state.busy = False

    @app.post("/api/run/extraction")
    async def run_extraction(): return await gated(extract_all)

    @app.post("/api/run/condense")
    async def run_condense(): return await gated(condense)

    @app.post("/api/run/summarize")
    async def run_summarize(): return await gated(summarize)

    @app.post("/api/run/classify")
    async def run_classify(): return await gated(classify)

    @app.post("/api/run/condense-check")
    async def run_condense_check(): return await gated(check_condensation)

    @app.post("/api/benchmark/technologies")
    async def benchmark_technologies():
        if state.session is None or state.selected_pdf is None:
            raise HTTPException(409, "Start a session and select a PDF first")
        if state.lock.locked():
            raise HTTPException(409, "Another operation is running")
        async with state.lock:
            state.busy = True
            try:
                names, _ = await find_technologies(ws, state.selected_pdf)
                return {"technologies": names}
            finally:
                state.busy = False

    @app.post("/api/benchmark/run")
    async def benchmark_run(request: dict[str, list[str]]):
        selected = request.get("technologies", [])
        if len(selected) != SELECTION_COUNT:
            raise HTTPException(400, f"Select exactly {SELECTION_COUNT} technologies")
        return await gated(lambda local_ws, pdf: run_benchmark(local_ws, pdf, selected))

    @app.get("/api/output")
    async def output(path: str):
        target = Path(path).resolve()
        root = ws.root.resolve()
        if root not in target.parents or not target.is_file():
            raise HTTPException(404, "Output not found")
        return PlainTextResponse(target.read_text(encoding="utf-8-sig"))

    @app.get("/api/progress")
    async def progress():
        queue: asyncio.Queue[dict[str, str]] = asyncio.Queue()
        state.listeners.add(queue)
        async def events():
            try:
                while True:
                    try:
                        item = await asyncio.wait_for(queue.get(), 15)
                        yield f"event: log\ndata: {json.dumps(item)}\n\n"
                    except TimeoutError:
                        yield ": keepalive\n\n"
            finally:
                state.listeners.discard(queue)
        return StreamingResponse(events(), media_type="text/event-stream")

    @app.post("/api/shutdown")
    async def shutdown():
        server = getattr(app.state, "server", None)
        if server is not None:
            server.should_exit = True
        return {"ok": True}

    static = ws.root / "web" / "wwwroot"
    app.mount("/", StaticFiles(directory=static, html=True), name="static")
    return app


async def serve(ws: Workspace) -> None:
    """Run the local web interface on localhost."""

    try:
        import uvicorn
    except ImportError as exc:
        raise RuntimeError("Web dependencies are missing. Run: pip install -e .[web]") from exc
    app = create_app(ws)
    config = uvicorn.Config(app, host="127.0.0.1", port=5050, log_level="info")
    print("Web interface: http://127.0.0.1:5050")
    server = uvicorn.Server(config)
    app.state.server = server
    await server.serve()
