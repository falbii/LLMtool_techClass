"""PDF text extraction and prompt assembly helpers."""

from __future__ import annotations

import re
from pathlib import Path


def extract_text(file_path: Path) -> str:
    """Extract page-marked text from a PDF file."""

    from pypdf import PdfReader

    if not file_path.is_file():
        raise FileNotFoundError(f"PDF file not found: {file_path}")
    if file_path.stat().st_size == 0:
        raise ValueError("PDF file is empty")

    try:
        reader = PdfReader(file_path)
    except Exception as exc:
        raise RuntimeError(f"Failed to open PDF: {exc}") from exc
    if not reader.pages:
        raise ValueError("PDF contains no pages")

    output: list[str] = []
    for page_number, page in enumerate(reader.pages, 1):
        output.append(f"[PAGE {page_number}]")
        try:
            output.extend(_with_table_markers(page.extract_text() or ""))
        except Exception as exc:
            output.append(f"[ERROR extracting page {page_number}: {exc}]")
        output.extend(["[END OF PAGE]", ""])
    return "\n".join(output)


def _with_table_markers(content: str) -> list[str]:
    """Mark likely table-like regions so condensation prompts preserve them."""

    result: list[str] = []
    buffer: list[str] = []
    table_type = ""
    for line in content.splitlines():
        numeric_count = len(re.findall(r"\d+[\d.,]*", line))
        structured = "|" in line or "\t" in line or (numeric_count >= 3 and len(line) > 20)
        stripped = line.lstrip()
        list_like = bool(re.match(r"^(\d+[.)]|-|\u2022|\*|[A-Z]{2,}[\s_]|[A-Z][a-z]+\s[A-Z])", stripped))
        capitalized = len(re.findall(r"\b[A-Z][A-Za-z]{2,}", line)) >= 2
        tech_name = (list_like or capitalized) and len(line.strip()) > 10
        is_table = (structured and numeric_count >= 2) or tech_name
        if is_table:
            if not buffer:
                table_type = "DATA" if numeric_count >= 2 else "TECH_LIST"
            buffer.append(line)
        else:
            _flush_table(result, buffer, table_type)
            buffer, table_type = [], ""
            result.append(line)
    _flush_table(result, buffer, table_type)
    return result


def _flush_table(output: list[str], buffer: list[str], table_type: str) -> None:
    if len(buffer) >= 3:
        marker = (
            "[TABLE REGION - IMPORTANT NUMERICAL DATA]"
            if table_type == "DATA"
            else "[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]"
        )
        output.extend([marker, *buffer, "[END TABLE]"])
    else:
        output.extend(buffer)


def split_into_chunks(content: str, max_chars: int = 30_000, overlap_lines: int = 3) -> list[str]:
    """Split long document text into overlapping chunks for model context limits."""

    chunks: list[str] = []
    current: list[str] = []
    current_length = 0
    overlap_lines = max(0, overlap_lines)
    for line in content.splitlines():
        if current and current_length + len(line) > max_chars:
            chunks.append("\n".join(current) + "\n")
            current = current[-overlap_lines:] if overlap_lines else []
            current_length = sum(len(item) + 1 for item in current)
        current.append(line)
        current_length += len(line) + 1
    if current:
        chunks.append("\n".join(current) + "\n")
    return chunks


def build_single_document_prompt(chunks: list[str], question: str) -> str:
    """Build a question prompt for one selected document."""

    if len(chunks) == 1:
        return f"Here's a PDF document I need you to analyze:\n\n{chunks[0]}\n\nQuestion: {question}"
    parts = [f"I have a multi-page PDF document split into {len(chunks)} parts.\n"]
    for index, chunk in enumerate(chunks, 1):
        parts.extend([f"**Part {index}:**", chunk, ""])
    parts.append(f"Question: {question}")
    return "\n".join(parts)


def build_multi_document_prompt(documents: dict[Path, list[str]], question: str) -> str:
    """Build a question prompt that compares or searches across many documents."""

    parts = [f"I have {len(documents)} PDF documents to analyze:\n"]
    for doc_number, (path, chunks) in enumerate(documents.items(), 1):
        parts.append(f"**Document {doc_number}: {path.name}**")
        for index, chunk in enumerate(chunks, 1):
            if len(chunks) > 1:
                parts.append(f"[Part {index}/{len(chunks)}]")
            parts.append(chunk)
        parts.append("\n---\n")
    parts.append(f"Question: {question}")
    return "\n".join(parts)
