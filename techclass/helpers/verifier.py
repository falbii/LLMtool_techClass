from __future__ import annotations

import re
from dataclasses import dataclass, fields
from decimal import Decimal

from .classifier import normalize_number
from ..format_output.models import TechnologyRecord


@dataclass(frozen=True, slots=True)
class Finding:
    tech_id: str
    field: str
    value: float


@dataclass(frozen=True, slots=True)
class VerificationReport:
    total_values: int
    verified_values: int
    unverified: list[Finding]

    @property
    def unverified_count(self) -> int:
        return len(self.unverified)

    @property
    def verified_percent(self) -> float:
        return 100.0 if self.total_values == 0 else self.verified_values * 100.0 / self.total_values


def extract_numbers(text: str) -> list[float]:
    numbers: list[float] = []
    for match in re.finditer(r"[-+]?\d[\d.,]*", text):
        raw = match.group().rstrip(".,")
        # Values with both separators are unambiguous; a lone three-digit
        # suffix is treated as decimal for ratios and also indexed as grouping.
        normalized = normalize_number(raw)
        if normalized is not None:
            numbers.append(float(normalized))
        if ("," in raw) ^ ("." in raw):
            separator = "," if "," in raw else "."
            if raw.count(separator) == 1 and len(raw.rsplit(separator, 1)[1]) == 3:
                grouped = normalize_number(raw, magnitude=True)
                if grouped is not None:
                    numbers.append(float(grouped))
    return numbers


def contains(index: list[float], value: float) -> bool:
    tolerance = max(1e-9, abs(value) * 1e-6)
    return any(abs(candidate - value) <= tolerance for candidate in index)


def verify(records: list[TechnologyRecord], source_text: str) -> VerificationReport:
    index = extract_numbers(source_text)
    total = verified = 0
    findings: list[Finding] = []
    numeric_fields = {
        "reference_unit_size", "year", "ref_year", "trl", "efficiency", "ratios_in",
        "ratios_out", "lifetime", "capex", "opex",
    }
    for record in records:
        for item in fields(record):
            if item.name not in numeric_fields:
                continue
            raw = getattr(record, item.name)
            values = raw if isinstance(raw, list) else ([] if raw is None else [raw])
            for raw_value in values:
                value = float(raw_value)
                total += 1
                equivalent = contains(index, value)
                if item.name == "efficiency" and value <= 1:
                    equivalent = equivalent or contains(index, value * 100)
                if equivalent:
                    verified += 1
                else:
                    findings.append(Finding(record.tech_id or "(unknown)", item.name, value))
    return VerificationReport(total, verified, findings)


def format_report(source_name: str, report: VerificationReport, notes: list[str] | None = None) -> str:
    lines = [
        f"Classification verification - {source_name}",
        f"Verified: {report.verified_values}/{report.total_values} ({report.verified_percent:.1f}%)",
        "",
        "Unverified numeric values:",
    ]
    if report.unverified:
        lines.extend(f"- [{item.tech_id}] {item.field} = {item.value:g}" for item in report.unverified)
    else:
        lines.append("- none")
    lines.extend(["", "Parsing notes:"])
    lines.extend(f"- {note}" for note in (notes or []))
    if not notes:
        lines.append("- none")
    return "\n".join(lines) + "\n"


def verify_condensation(raw_text: str, condensed_text: str) -> VerificationReport:
    """Report numeric values present in the raw PDF extraction but absent after condensation."""
    condensed_numbers = extract_numbers(condensed_text)
    source_numbers = extract_numbers(raw_text)
    findings: list[Finding] = []
    verified = 0
    for value in source_numbers:
        equivalent = contains(condensed_numbers, value)
        if 0 < value <= 1:
            equivalent = equivalent or contains(condensed_numbers, value * 100)
        if equivalent:
            verified += 1
        else:
            findings.append(Finding("(document)", "number", value))
    return VerificationReport(len(source_numbers), verified, findings)
