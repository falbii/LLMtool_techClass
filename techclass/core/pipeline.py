"""High-level extraction pipeline from PDF input to validated CSV output."""

from __future__ import annotations

import asyncio
import re
from datetime import datetime
from pathlib import Path

from ..helpers.classifier import (
    extract_json, has_meaningful_data, is_valid_json_array, merge_by_technology_and_year,
    parse_record, parse_rows_from_json, validation_notes,
)
from ..format_output.formats import read_csv, read_summary_sections, write_csv, write_summary_md
from .pdf import extract_text, split_into_chunks
from ..helpers.verifier import format_report, verify, verify_condensation
from ..workspace import Workspace

MISSING_SECTION = "[NO PARSEABLE DATA - extraction marker missing for this technology]"


def load_prompt(ws: Workspace, name: str) -> str:
    """Load a prompt template from the workspace prompt directory."""

    path = ws.root / "prompt" / name
    if not path.is_file():
        raise FileNotFoundError(f"Prompt template not found: {path}")
    return path.read_text(encoding="utf-8-sig")


def cache_path(ws: Workspace, pdf: Path) -> Path:
    return ws.cache_dir / f"{pdf.stem}_condensed.md"


def tech_list_path(ws: Workspace, pdf: Path) -> Path:
    return ws.tech_list_dir / f"{pdf.stem}_technology_list.md"


def invalidate_cached_artifacts(ws: Workspace, pdf: Path) -> None:
    """Remove derived files that become stale when a source PDF changes."""

    cache_path(ws, pdf).unlink(missing_ok=True)
    tech_list_path(ws, pdf).unlink(missing_ok=True)


def read_tech_list(ws: Workspace, pdf: Path) -> list[str] | None:
    """Return the editable cached technology list when it is newer than condensation."""

    path, condensed = tech_list_path(ws, pdf), cache_path(ws, pdf)
    if not path.is_file() or not condensed.is_file() or condensed.stat().st_mtime > path.stat().st_mtime:
        return None
    names = [line.strip() for line in path.read_text(encoding="utf-8-sig").splitlines()
             if line.strip() and not line.lstrip().startswith(("#", "<!--"))]
    return names or None


def write_tech_list(ws: Workspace, pdf: Path, names: list[str]) -> None:
    path = tech_list_path(ws, pdf)
    path.parent.mkdir(parents=True, exist_ok=True)
    header = f"<!-- technologies found in {pdf.name} on {datetime.now():%Y-%m-%d %H:%M:%S} -->\n"
    path.write_text(header + "<!-- one technology per line; edit to control downstream rows -->\n\n" +
                    "\n".join(names) + "\n", encoding="utf-8")


async def condense(ws: Workspace, pdf: Path) -> Path:
    """Create or reuse the condensed Markdown representation of a PDF."""

    output = cache_path(ws, pdf)
    output.parent.mkdir(parents=True, exist_ok=True)
    if output.is_file() and pdf.stat().st_mtime <= output.stat().st_mtime:
        print(f"Using cached condensed PDF: {output.name}")
        return output
    raw = await asyncio.to_thread(extract_text, pdf)
    template = load_prompt(ws, "condense_pdf.md")
    parts: list[str] = []
    chunks = split_into_chunks(raw)
    for index, chunk in enumerate(chunks, 1):
        print(f"Condensing part {index}/{len(chunks)}")
        async with await ws.client.create_session(ws.model) as session:
            response = await session.send(template.replace("{{PDF_CONTENT}}", chunk))
        response = re.sub(r"\[(?:TABLE REGION|TECHNOLOGY LIST TABLE|END TABLE)[^\]\r\n]*\]", "", response)
        parts.append(response.strip())
    result = "\n\n".join(parts)
    header = f"<!-- condensed from {pdf.name} by {ws.model} on {datetime.now():%Y-%m-%d %H:%M:%S} -->\n\n"
    output.write_text(header + result, encoding="utf-8")
    print(f"Condensed PDF saved to {output}")
    return output


def build_pdf_content(chunks: list[str]) -> str:
    if len(chunks) == 1:
        return chunks[0]
    return "\n\n".join(f"[PART {index}/{len(chunks)}]\n{chunk}" for index, chunk in enumerate(chunks, 1))


def build_find_prompt(ws: Workspace, chunks: list[str]) -> str:
    return load_prompt(ws, "find_technologies.md").replace("{{PDF_CONTENT}}", build_pdf_content(chunks))


def build_summary_prompt(ws: Workspace, chunks: list[str], names: list[str]) -> str:
    listing = "\n".join(f"{index}. {name}" for index, name in enumerate(names, 1))
    return (load_prompt(ws, "summary_technology.md")
            .replace("{{TECHNOLOGY_COUNT}}", str(len(names)))
            .replace("{{TECHNOLOGY_LIST}}", listing)
            .replace("{{PDF_CONTENT}}", build_pdf_content(chunks)))


def parse_technology_names(response: str) -> list[str]:
    """Extract technology names from a simple model-generated list."""

    names: list[str] = []
    for line in response.splitlines():
        line = line.strip()
        if line.startswith("#") or len(line) < 3:
            continue
        lower = line.lower()
        if (
            ("technology" in lower or "technologies" in lower or "list" in lower)
            and (lower.startswith(("technology", "technologies", "list")) or lower.endswith(":"))
        ):
            continue
        cleaned = re.sub(r"^\d+\.\s*", "", line)
        cleaned = re.sub(r"^[-*\u2022]\s*", "", cleaned).strip()
        if re.search(r"[A-Za-z]{2,}", cleaned):
            names.append(cleaned)
    return names


def parse_batched_response(response: str, expected_count: int) -> list[str]:
    """Split a batched summary response by explicit technology markers."""

    pattern = re.compile(r"===\s*TECHNOLOGY\s+(?P<number>\d+)\s*:.*?===", re.IGNORECASE)
    matches = list(pattern.finditer(response))
    details: list[str | None] = [None] * expected_count
    for index, match in enumerate(matches):
        end = matches[index + 1].start() if index + 1 < len(matches) else len(response)
        content = response[match.end():end].strip()
        number = int(match.group("number"))
        if 1 <= number <= expected_count and content and details[number - 1] is None:
            details[number - 1] = content
    return [detail or MISSING_SECTION for detail in details]


async def find_technologies(ws: Workspace, pdf: Path) -> tuple[list[str], list[str]]:
    """Find technology names and return them with the condensed text chunks."""

    condensed = await condense(ws, pdf)
    chunks = split_into_chunks(condensed.read_text(encoding="utf-8-sig"))
    cached = read_tech_list(ws, pdf)
    if cached:
        return cached, chunks
    async with await ws.client.create_session(ws.model) as session:
        response = await session.send(build_find_prompt(ws, chunks))
    names = parse_technology_names(response)
    if names:
        write_tech_list(ws, pdf, names)
    return names, chunks


async def summarize(ws: Workspace, pdf: Path, delay: float = 3.0) -> Path | None:
    """Generate technology-level Markdown summaries for one PDF."""

    names, chunks = await find_technologies(ws, pdf)
    if not names:
        print("No technologies found")
        return None
    output = ws.md_dir / f"{pdf.stem}_summary.md"
    details: list[str] = []
    started = datetime.now()
    batch_size = 10
    for start in range(0, len(names), batch_size):
        if start and delay:
            await asyncio.sleep(delay)
        batch = names[start:start + batch_size]
        print(f"Summarizing technologies {start + 1}-{start + len(batch)}")
        try:
            async with await ws.client.create_session(ws.model) as session:
                response = await session.send(build_summary_prompt(ws, chunks, batch), on_content=lambda text: print(text, end="", flush=True))
            print()
            details.extend(parse_batched_response(response, len(batch)))
        except Exception as exc:
            details.extend(f"Extraction failed for {name}: {exc}" for name in batch)
        write_summary_md(output, pdf, ws.model, started, None, names, details)
    write_summary_md(output, pdf, ws.model, started, datetime.now(), names, details)
    print(f"Summary saved to {output}")
    return output


def build_classification_prompt(ws: Workspace, sections: list[tuple[str, str]]) -> str:
    content = "\n\n".join(f"=== TECHNOLOGY {index}: {name} ===\n{body}"
                            for index, (name, body) in enumerate(sections, 1))
    return (load_prompt(ws, "classification_from_summary.md")
            .replace("{{TECHNOLOGY_COUNT}}", str(len(sections)))
            .replace("{{SOURCE_LABEL}}", "summary")
            .replace("{{TECHNOLOGY_SECTIONS}}", content))


async def _try_classify(ws: Workspace, sections: list[tuple[str, str]], attempts: int = 2) -> list[dict[str, str]] | None:
    """Ask the model for JSON rows and retry when no valid array is returned."""

    prompt = build_classification_prompt(ws, sections)
    for attempt in range(attempts):
        async with await ws.client.create_session(ws.model) as session:
            response = await session.send(prompt, on_content=(lambda text: print(text, end="", flush=True)) if attempt == 0 else None)
        candidate = extract_json(response)
        if is_valid_json_array(candidate):
            rows = parse_rows_from_json(candidate)
            if rows:
                return rows
        print("No usable JSON rows returned; retrying")
    return None


def extract_source_year(ws: Workspace, pdf: Path) -> int | None:
    """Infer a source/reference year from the filename or condensed document header."""

    match = re.search(r"(?<!\d)(?:19|20)\d{2}(?!\d)", pdf.stem)
    if match:
        return int(match.group())
    path = cache_path(ws, pdf)
    if path.is_file():
        text = re.sub(r"^\s*<!--.*?-->", "", path.read_text(encoding="utf-8-sig"), flags=re.DOTALL)[:2000]
        match = re.search(r"(?<!\d)(?:19|20)\d{2}(?!\d)", text)
        if match:
            return int(match.group())
    return None


async def classify(ws: Workspace, pdf: Path, delay: float = 3.0) -> Path | None:
    """Convert summary Markdown into CSV rows and write a numeric verification report."""

    summary_path = ws.md_dir / f"{pdf.stem}_summary.md"
    sections = read_summary_sections(summary_path)
    if not sections:
        print(f"No summary sections found. Run summarize first: {summary_path}")
        return None
    output = ws.csv_dir / f"{pdf.stem}_classification.csv"
    records = []
    notes: list[str] = []
    source_year = extract_source_year(ws, pdf)
    for start in range(0, len(sections), 5):
        if start and delay:
            await asyncio.sleep(delay)
        batch = sections[start:start + 5]
        rows = await _try_classify(ws, batch)
        if rows is None:
            rows = []
            for section in batch:
                rows.extend(await _try_classify(ws, [section], attempts=1) or [])
        for row in rows:
            record, row_notes = parse_record(row)
            record.ref_year = source_year
            records.append(record)
            notes.extend(f"Row {len(records)}: {note}" for note in [*row_notes, *validation_notes(record)])
        meaningful = [record for record in records if has_meaningful_data(record)]
        if meaningful:
            write_csv(output, merge_by_technology_and_year(meaningful), ws.model)
    if not output.is_file():
        return None
    source = cache_path(ws, pdf).read_text(encoding="utf-8-sig") if cache_path(ws, pdf).is_file() else await asyncio.to_thread(extract_text, pdf)
    report = verify(read_csv(output), source)
    report_path = ws.classify_check_dir / f"{pdf.stem}_check_classification_with_pdf.txt"
    report_path.write_text(format_report(pdf.name, report, notes), encoding="utf-8")
    print(f"Classification saved to {output}")
    print(f"Verification: {report.verified_values}/{report.total_values} numeric values found in source")
    return output


async def extract_all(ws: Workspace, pdf: Path) -> Path | None:
    """Run condense, summarize, and classify in dependency order."""

    await condense(ws, pdf)
    if await summarize(ws, pdf) is None:
        return None
    return await classify(ws, pdf)


async def check_condensation(ws: Workspace, pdf: Path) -> Path:
    condensed_path = await condense(ws, pdf)
    raw = await asyncio.to_thread(extract_text, pdf)
    condensed = condensed_path.read_text(encoding="utf-8-sig")
    report = verify_condensation(raw, condensed)
    output = ws.check_dir / f"{pdf.stem}_check_condensed_with_pdf.txt"
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(format_report(pdf.name, report), encoding="utf-8")
    print(f"Condensation check: {report.verified_values}/{report.total_values} numeric values retained")
    print(f"Report saved to {output}")
    return output
