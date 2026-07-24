"""CSV and Markdown serialization for TechClass artifacts."""

from __future__ import annotations

import csv
import re
from dataclasses import fields
from datetime import datetime
from pathlib import Path

from .models import TechnologyRecord

HEADER_ORDER = [
    "tech_id", "process_type", "description", "unit_operation", "main_sector",
    "main_category", "category_spec", "tech_type", "year", "reference_unit_size",
    "reference_unit_size_unit", "location", "currency", "trl", "tech_maturity",
    "efficiency", "efficiency_unit", "carriers_in", "main_input", "ratios_in",
    "units_in", "carriers_out", "main_out", "ratios_out", "units_out", "capex",
    "capex_unit", "opex", "opex_unit", "lifetime", "lifetime_unit", "ref_year", "summary",
]

LIST_FIELDS = {"carriers_in", "ratios_in", "units_in", "carriers_out", "ratios_out", "units_out"}


def write_csv(path: Path, rows: list[TechnologyRecord], model: str | None = None) -> None:
    """Write normalized classification records in the canonical column order."""

    path.parent.mkdir(parents=True, exist_ok=True)
    header = [*HEADER_ORDER, *( ["model"] if model else [])]
    with path.open("w", encoding="utf-8-sig", newline="") as stream:
        writer = csv.DictWriter(stream, fieldnames=header)
        writer.writeheader()
        for record in rows:
            row: dict[str, object] = {}
            for name in HEADER_ORDER:
                value = getattr(record, name)
                if name in LIST_FIELDS:
                    value = "; ".join(_format_number(item) for item in value)
                elif value is not None:
                    value = _format_number(value)
                else:
                    value = ""
                row[name] = value
            if model:
                row["model"] = model
            writer.writerow(row)


def read_csv(path: Path) -> list[TechnologyRecord]:
    """Read classification CSV rows back into normalized records."""

    from ..helpers.classifier import parse_record

    if not path.is_file():
        return []
    with path.open(encoding="utf-8-sig", newline="") as stream:
        return [parse_record(row)[0] for row in csv.DictReader(stream)]


def _format_number(value: object) -> str:
    if isinstance(value, float):
        return format(value, ".15g")
    return str(value)


SECTION_PATTERN = re.compile(
    r"^#{2,3}\s*TECHNOLOGY\s+\d+:\s*(?P<name>[^\r\n]+?)\s*$",
    re.IGNORECASE | re.MULTILINE,
)


def write_summary_md(
    path: Path,
    source_pdf: Path,
    model: str,
    started_at: datetime,
    finished_at: datetime | None,
    names: list[str],
    details: list[str],
) -> None:
    """Write reviewable Markdown summaries for downstream classification."""

    lines = [
        f"# Technology Extraction Data - {source_pdf.name}", "", f"- **Model:** {model}",
        f"- **Started:** {started_at:%Y-%m-%d %H:%M:%S}",
    ]
    if finished_at:
        lines.append(f"- **Finished:** {finished_at:%Y-%m-%d %H:%M:%S} (duration {finished_at - started_at})")
    else:
        lines.append("- **Finished:** (in progress)")
    lines.extend([f"- **Total Technologies:** {len(names)}", "", "---", ""])
    for index, (name, detail) in enumerate(zip(names, details), 1):
        lines.extend([f"## TECHNOLOGY {index}: {name}", "", detail, "", "---", ""])
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("\n".join(lines), encoding="utf-8")


def read_summary_sections(path: Path) -> list[tuple[str, str]]:
    """Parse technology sections from the generated summary Markdown."""

    if not path.is_file():
        return []
    content = path.read_text(encoding="utf-8-sig")
    matches = list(SECTION_PATTERN.finditer(content))
    sections: list[tuple[str, str]] = []
    for index, match in enumerate(matches):
        end = matches[index + 1].start() if index + 1 < len(matches) else len(content)
        body = content[match.end():end].strip()
        body = re.sub(r"\s*---\s*$", "", body).strip()
        sections.append((match.group("name").strip(), body))
    return sections

