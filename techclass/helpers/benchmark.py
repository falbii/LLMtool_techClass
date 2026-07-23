"""Benchmark the same selected technologies across every available model."""

from __future__ import annotations

import csv
import time
from dataclasses import replace
from datetime import datetime
from pathlib import Path

from .classifier import has_meaningful_data, merge_by_technology_and_year, parse_record
from ..format_output.formats import HEADER_ORDER
from ..core.pipeline import (
    _try_classify, build_summary_prompt, extract_source_year, find_technologies,
    parse_batched_response,
)
from ..workspace import Workspace

SELECTION_COUNT = 3


async def run_benchmark(ws: Workspace, pdf: Path, selected: list[str]) -> Path | None:
    """Run summary and classification for selected technologies on each model."""

    if not selected:
        return None
    _, chunks = await find_technologies(ws, pdf)
    models = await ws.client.list_models()
    if not models:
        return None
    date = datetime.now().strftime("%Y-%m-%d")
    prefix = f"{pdf.stem}_{ws.client.provider_name}_benchmark"
    ws.benchmark_dir.mkdir(parents=True, exist_ok=True)
    summary_path = ws.benchmark_dir / f"{prefix}_summary_{date}.md"
    classification_path = ws.benchmark_dir / f"{prefix}_classification_{date}.csv"
    overview_path = ws.benchmark_dir / f"{prefix}_overview_{date}.csv"
    summary_lines = [f"# Benchmark Summaries - {pdf.name}", ""]
    overview: list[dict[str, object]] = []
    classified: list[tuple[str, object]] = []
    ref_year = extract_source_year(ws, pdf)

    for model in models:
        # Reuse the same workspace directories and provider, changing only the model id.
        model_ws = replace(ws, model=model.id)
        started = time.perf_counter()
        status = "OK"
        details: list[str] = []
        rows = []
        try:
            async with await ws.client.create_session(model.id) as session:
                response = await session.send(build_summary_prompt(model_ws, chunks, selected))
            details = parse_batched_response(response, len(selected))
            summary_ms = round((time.perf_counter() - started) * 1000)
            sections = list(zip(selected, details))
            classify_started = time.perf_counter()
            raw_rows = await _try_classify(model_ws, sections) or []
            for raw in raw_rows:
                record, _ = parse_record(raw)
                record.ref_year = ref_year
                rows.append(record)
            rows = merge_by_technology_and_year([row for row in rows if has_meaningful_data(row)])
            classify_ms = round((time.perf_counter() - classify_started) * 1000)
            classified.extend((model.id, row) for row in rows)
        except Exception as exc:
            status = f"ERROR: {exc}"
            summary_ms, classify_ms = 0, 0
        words = sum(len(detail.split()) for detail in details)
        overview.append({"Model": model.id, "SummaryWords": words, "SummaryMs": summary_ms,
                         "SummaryStatus": status, "ClassifiedRows": len(rows),
                         "ClassifyMs": classify_ms, "ClassifyStatus": status})
        summary_lines.extend([f"# MODEL: {model.id}", "", f"- **Status:** {status}", ""])
        for index, (name, detail) in enumerate(zip(selected, details), 1):
            summary_lines.extend([f"## TECHNOLOGY {index}: {name}", "", detail, ""])
        summary_lines.extend(["---", ""])

    summary_path.write_text("\n".join(summary_lines), encoding="utf-8")
    with overview_path.open("w", encoding="utf-8-sig", newline="") as stream:
        writer = csv.DictWriter(stream, fieldnames=list(overview[0]))
        writer.writeheader()
        writer.writerows(overview)
    if classified:
        from ..format_output.formats import _format_number, LIST_FIELDS
        with classification_path.open("w", encoding="utf-8-sig", newline="") as stream:
            writer = csv.DictWriter(stream, fieldnames=["Model", *HEADER_ORDER])
            writer.writeheader()
            for model, record in classified:
                data = {"Model": model}
                for name in HEADER_ORDER:
                    value = getattr(record, name)
                    data[name] = "; ".join(_format_number(v) for v in value) if name in LIST_FIELDS else (_format_number(value) if value is not None else "")
                writer.writerow(data)
        return classification_path
    return overview_path

