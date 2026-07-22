from __future__ import annotations

import argparse
import asyncio
import shutil
import sys
from pathlib import Path

from .chat import ChatClient, CopilotChatClient, OllamaChatClient
from .benchmark import SELECTION_COUNT, run_benchmark
from .pdf import build_multi_document_prompt, build_single_document_prompt, split_into_chunks
from .pipeline import (
    check_condensation, classify, condense, extract_all, find_technologies,
    invalidate_cached_artifacts, summarize,
)
from .workspace import Workspace


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Extract structured technology data from PDFs")
    parser.add_argument("--local", "--ollama", action="store_true", dest="local", help="use local Ollama")
    parser.add_argument("--model", help="model id (prompts for selection when omitted)")
    parser.add_argument("--web", action="store_true", help="run the browser interface")
    parser.add_argument("--root", type=Path, default=Path.cwd(), help="repository/workspace root")
    return parser


async def select_model(client: ChatClient, requested: str | None) -> str:
    models = await client.list_models()
    if requested:
        if models and requested not in {model.id for model in models}:
            print(f"Warning: '{requested}' was not returned by {client.provider_name}; trying it anyway.")
        return requested
    if not models:
        raise RuntimeError(f"No {client.provider_name} models are available")
    print("Available models:")
    for index, model in enumerate(models, 1):
        suffix = " (reasoning)" if model.supports_reasoning else ""
        print(f"  {index}. {model.id}{suffix}")
    while True:
        choice = input(f"Choose a model [1-{len(models)}]: ").strip()
        try:
            return models[int(choice) - 1].id
        except (ValueError, IndexError):
            print("Please enter one of the listed numbers.")


def list_pdfs(ws: Workspace) -> list[Path]:
    return sorted(ws.pdf_dir.glob("*.pdf"), key=lambda item: item.name.lower())


def choose_pdf(ws: Workspace) -> Path | None:
    pdfs = list_pdfs(ws)
    if not pdfs:
        print(f"No PDFs found in {ws.pdf_dir}")
        return None
    for index, pdf in enumerate(pdfs, 1):
        print(f"  {index}. {pdf.name}")
    value = input("Select PDF (Enter to cancel): ").strip()
    if not value:
        return None
    try:
        return pdfs[int(value) - 1]
    except (ValueError, IndexError):
        print("Invalid selection")
        return None


async def run_console(ws: Workspace) -> None:
    current_pdf: Path | None = None
    session = await ws.client.create_session(ws.model)
    pdf_context_sent = False
    print("\nTechClass Python is ready. Type /commands for help.\n")
    try:
        while True:
            try:
                value = input("techclass> ").strip()
            except EOFError:
                break
            if not value:
                continue
            command, _, argument = value.partition(" ")
            command = command.lower()
            known = {"list", "upload", "current", "extraction", "condense", "summarize",
                     "classify", "condense-check", "batch-analyze", "benchmark", "commands",
                     "help", "exit", "quit"}
            if command in known:
                command = f"/{command}"
            if command in {"/exit", "/quit"}:
                break
            if command in {"/commands", "/help"}:
                print("/list /upload PATH /current /extraction /condense /summarize /classify")
                print("/condense-check /batch-analyze QUESTION /benchmark /exit")
                continue
            if command == "/list":
                selected = choose_pdf(ws)
                if selected:
                    current_pdf, pdf_context_sent = selected, False
                    print(f"Selected: {current_pdf.name}")
                continue
            if command == "/upload":
                source = Path(argument.strip().strip('"'))
                if not source.is_file() or source.suffix.lower() != ".pdf":
                    print("Usage: /upload PATH_TO_PDF")
                    continue
                target = ws.pdf_dir / source.name
                invalidate_cached_artifacts(ws, target)
                shutil.copy2(source, target)
                current_pdf, pdf_context_sent = target, False
                print(f"Uploaded and selected: {target.name}")
                continue
            if command == "/current":
                print(current_pdf.name if current_pdf else "No PDF selected")
                continue
            if command in {"/extraction", "/condense", "/summarize", "/classify", "/condense-check"}:
                if current_pdf is None:
                    current_pdf = choose_pdf(ws)
                if current_pdf is None:
                    continue
                operation = {
                    "/extraction": extract_all, "/condense": condense,
                    "/summarize": summarize, "/classify": classify, "/condense-check": check_condensation,
                }[command]
                await operation(ws, current_pdf)
                continue
            if command == "/batch-analyze":
                question = argument.strip()
                if not question:
                    print("Usage: /batch-analyze QUESTION")
                    continue
                documents: dict[Path, list[str]] = {}
                for pdf in list_pdfs(ws):
                    path = await condense(ws, pdf)
                    documents[pdf] = split_into_chunks(path.read_text(encoding="utf-8-sig"))
                await session.send(build_multi_document_prompt(documents, question),
                                   on_content=lambda text: print(text, end="", flush=True))
                print()
                continue
            if command == "/benchmark":
                if current_pdf is None:
                    current_pdf = choose_pdf(ws)
                if current_pdf is None:
                    continue
                names, _ = await find_technologies(ws, current_pdf)
                for index, name in enumerate(names, 1):
                    print(f"  {index}. {name}")
                raw = input(f"Pick exactly {SELECTION_COUNT} numbers, comma-separated: ")
                try:
                    indexes = list(dict.fromkeys(int(item.strip()) for item in raw.split(",")))
                    if len(indexes) != SELECTION_COUNT or any(index < 1 for index in indexes):
                        raise ValueError
                    selected = [names[index - 1] for index in indexes]
                except (ValueError, IndexError):
                    print(f"Please select exactly {SELECTION_COUNT} valid technologies")
                    continue
                output = await run_benchmark(ws, current_pdf, selected)
                print(f"Benchmark saved to {output}" if output else "Benchmark produced no output")
                continue
            if command.startswith("/"):
                print(f"Unknown command: {command}. Type /commands for help.")
                continue
            prompt = value
            if current_pdf and not pdf_context_sent:
                condensed_path = await condense(ws, current_pdf)
                chunks = split_into_chunks(condensed_path.read_text(encoding="utf-8-sig"))
                prompt = build_single_document_prompt(chunks, value)
                pdf_context_sent = True
            await session.send(
                prompt,
                on_reasoning=lambda text: print(text, end="", flush=True),
                on_content=lambda text: print(text, end="", flush=True),
            )
            print()
    finally:
        await session.close()


async def async_main(args: argparse.Namespace) -> None:
    client = await (OllamaChatClient.connect() if args.local else CopilotChatClient.connect())
    async with client:
        if args.web and not args.model:
            models = await client.list_models()
            if not models:
                raise RuntimeError(f"No {client.provider_name} models are available")
            model = models[0].id
        else:
            model = await select_model(client, args.model)
        ws = Workspace.create(args.root, client, model)
        ws.ensure_directories()
        if args.web:
            from .web import serve
            await serve(ws)
        else:
            await run_console(ws)


def main() -> None:
    args = build_parser().parse_args()
    try:
        asyncio.run(async_main(args))
    except KeyboardInterrupt:
        print("\nStopped.")
    except Exception as exc:
        print(f"Error: {exc}", file=sys.stderr)
        raise SystemExit(1) from exc


if __name__ == "__main__":
    main()
