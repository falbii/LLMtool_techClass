from __future__ import annotations

import json
import re
from dataclasses import replace
from decimal import Decimal, InvalidOperation
from typing import Any

from .models import TechnologyRecord


def normalize_header(value: str) -> str:
    value = re.sub(r"(?<=[a-z0-9])(?=[A-Z])", "_", value)
    return re.sub(r"[^a-z0-9]+", "_", value.lower()).strip("_")


def normalize_number(raw: str | None, magnitude: bool = False) -> str | None:
    if not raw or not raw.strip():
        return None
    match = re.search(r"[-+]?\d[\d.,]*", raw)
    if not match:
        return None
    token = match.group().rstrip(".,")
    sign = ""
    if token[:1] in "+-":
        sign, token = ("-" if token[0] == "-" else ""), token[1:]
    dot, comma = token.rfind("."), token.rfind(",")
    if dot >= 0 and comma >= 0:
        decimal_sep, group_sep = (".", ",") if dot > comma else (",", ".")
        token = token.replace(group_sep, "").replace(decimal_sep, ".")
    elif dot >= 0 or comma >= 0:
        separator = "," if comma >= 0 else "."
        count = token.count(separator)
        trailing = len(token) - token.rfind(separator) - 1
        if count > 1 or (trailing == 3 and magnitude):
            token = token.replace(separator, "")
        else:
            token = token.replace(separator, ".")
    candidate = sign + token
    try:
        float(candidate)
        return candidate
    except ValueError:
        return None


def parse_record(row: dict[str, Any]) -> tuple[TechnologyRecord, list[str]]:
    lookup = {normalize_header(key): value for key, value in row.items()}
    errors: list[str] = []

    def get(*aliases: str) -> str | None:
        for alias in aliases:
            value = lookup.get(normalize_header(alias))
            if value is not None and str(value).strip():
                return str(value).strip()
        return None

    def number(name: str, *aliases: str, magnitude: bool = False, integer: bool = False):
        raw = get(name, *aliases)
        if raw is None:
            return None
        normalized = normalize_number(raw, magnitude or integer)
        if normalized is None:
            errors.append(f"{name}: could not parse '{raw}'")
            return None
        try:
            return int(float(normalized)) if integer else float(normalized)
        except ValueError:
            errors.append(f"{name}: could not parse '{raw}'")
            return None

    def decimal(name: str, *aliases: str):
        raw = get(name, *aliases)
        if raw is None:
            return None
        normalized = normalize_number(raw, True)
        if normalized is None:
            errors.append(f"{name}: could not parse '{raw}'")
            return None
        try:
            return Decimal(normalized)
        except InvalidOperation:
            errors.append(f"{name}: could not parse '{raw}'")
            return None

    def strings(name: str) -> list[str]:
        raw = get(name)
        return [part.strip() for part in re.split(r"[;,|]", raw) if part.strip()] if raw else []

    def numbers(name: str) -> list[float]:
        values: list[float] = []
        for item in strings(name):
            normalized = normalize_number(item)
            if normalized is None:
                errors.append(f"{name}: could not parse '{item}'")
            else:
                values.append(float(normalized))
        return values

    record = TechnologyRecord(
        tech_id=get("tech_id", "Datapaper Tech ID"), process_type=get("process_type"),
        description=get("description"), unit_operation=get("unit_operation"), summary=get("summary"),
        main_sector=get("main_sector"), main_category=get("main_category"), category_spec=get("category_spec"),
        tech_type=get("tech_type"), reference_unit_size=number("reference_unit_size", magnitude=True),
        reference_unit_size_unit=get("reference_unit_size_unit"), year=number("year", "base_year", integer=True),
        location=get("location"), currency=get("currency"), ref_year=number("ref_year", "Data Reference Year", integer=True),
        trl=number("trl", "trl_(1-9)", integer=True), tech_maturity=get("tech_maturity"),
        efficiency=number("efficiency", "lhv_efficiency", "overall_efficiency"),
        efficiency_unit=get("efficiency_unit"), carriers_in=strings("carriers_in"), main_input=get("main_input"),
        ratios_in=numbers("ratios_in"), units_in=strings("units_in"), carriers_out=strings("carriers_out"),
        main_out=get("main_out"), ratios_out=numbers("ratios_out"), units_out=strings("units_out"),
        lifetime=number("lifetime", "lifetime_yr", magnitude=True), lifetime_unit=get("lifetime_unit"),
        capex=decimal("capex"), capex_unit=get("capex_unit"), opex=decimal("opex", "opex_fix"),
        opex_unit=get("opex_unit", "opex_fix_unit"),
    )
    return record, errors


def is_valid_json_array(value: str | None) -> bool:
    try:
        return isinstance(json.loads(value or ""), list)
    except json.JSONDecodeError:
        return False


def extract_json(response: str) -> str:
    for start, char in enumerate(response):
        if char != "[":
            continue
        depth, in_string, escaped = 0, False, False
        for index in range(start, len(response)):
            char = response[index]
            if in_string:
                if escaped:
                    escaped = False
                elif char == "\\":
                    escaped = True
                elif char == '"':
                    in_string = False
            elif char == '"':
                in_string = True
            elif char == "[":
                depth += 1
            elif char == "]":
                depth -= 1
                if depth == 0:
                    candidate = response[start:index + 1].strip()
                    if is_valid_json_array(candidate):
                        return candidate
                    break
    return ""


def parse_rows_from_json(value: str) -> list[dict[str, str]]:
    try:
        source = json.loads(value)
    except json.JSONDecodeError:
        return []
    rows: list[dict[str, str]] = []
    for item in source if isinstance(source, list) else []:
        if not isinstance(item, dict):
            continue
        row: dict[str, str] = {}
        for key, value in item.items():
            if value is None:
                continue
            if isinstance(value, list):
                row[key] = ", ".join(str(part) for part in value)
            elif isinstance(value, bool):
                row[key] = str(value).lower()
            else:
                row[key] = str(value)
        rows.append(row)
    return rows


def extract_abbreviation(text: str | None) -> str:
    words = [word for word in re.split(r"\W+", (text or "").strip()) if word]
    if not words:
        return "UNK"
    if len(words) == 1:
        return words[0][:3].upper()
    return "".join(word[0] for word in words).upper()


def generate_tech_id(record: TechnologyRecord, used_ids: set[str]) -> str:
    parts: list[str] = []
    candidates = [record.main_input or (record.carriers_in[0] if record.carriers_in else None),
                  record.unit_operation, record.process_type, record.main_out]
    for value in candidates:
        if value and len(parts) < 4:
            abbreviation = extract_abbreviation(value)
            if abbreviation not in parts:
                parts.append(abbreviation)
    base = "_".join(parts).upper()[:20] or "UNK"
    candidate, counter = base, 2
    lowered = {item.lower() for item in used_ids}
    while candidate.lower() in lowered:
        candidate = f"{base}_{counter}"
        counter += 1
    return candidate


def has_meaningful_data(record: TechnologyRecord) -> bool:
    count = sum(bool(value) for value in (
        record.description, record.unit_operation, record.summary, record.process_type,
        record.main_sector, record.main_category, record.category_spec, record.tech_type,
        record.year, record.trl, record.tech_maturity, record.efficiency,
        record.carriers_in or record.carriers_out, record.main_input or record.main_out,
        record.lifetime or record.capex or record.opex,
    ))
    return count >= 2


def merge_by_technology_and_year(rows: list[TechnologyRecord]) -> list[TechnologyRecord]:
    merged: list[TechnologyRecord] = []
    indexes: dict[str, int] = {}
    for row_index, row in enumerate(rows):
        name = next((value for value in (row.tech_type, row.description, row.unit_operation,
                    row.main_category, row.process_type, row.tech_id) if value), "unknown")
        normalized = re.sub(r"[^a-z0-9]", "", name.lower())
        key = f"{normalized}|Y{row.year}|{normalize_header(row.category_spec or '')}" if row.year and row.year >= 1900 else f"{normalized}|UNMERGEABLE|{row_index}"
        if key not in indexes:
            indexes[key] = len(merged)
            merged.append(replace(row, carriers_in=list(row.carriers_in), ratios_in=list(row.ratios_in),
                           units_in=list(row.units_in), carriers_out=list(row.carriers_out),
                           ratios_out=list(row.ratios_out), units_out=list(row.units_out)))
            continue
        target = merged[indexes[key]]
        for field_name in TechnologyRecord.__dataclass_fields__:
            current, incoming = getattr(target, field_name), getattr(row, field_name)
            if (current is None or current == [] or current == "") and incoming not in (None, [], ""):
                setattr(target, field_name, list(incoming) if isinstance(incoming, list) else incoming)

    used: set[str] = set()
    for record in merged:
        base = record.tech_id or generate_tech_id(record, used)
        candidate, counter = base, 2
        lowered = {item.lower() for item in used}
        while candidate.lower() in lowered:
            candidate = f"{base}_{counter}"
            counter += 1
        record.tech_id = candidate
        used.add(record.tech_id)
    return merged


def validation_notes(record: TechnologyRecord) -> list[str]:
    notes: list[str] = []
    if record.efficiency is not None and not 0 < record.efficiency <= 1:
        notes.append(f"efficiency: {record.efficiency:g} is outside the expected 0-1 range (left as-is)")
    if record.trl is not None and not 1 <= record.trl <= 9:
        notes.append(f"trl: {record.trl} is outside the valid 1-9 range")
    for name in ("year", "ref_year"):
        value = getattr(record, name)
        if value is not None and not 1900 <= value <= 2100:
            notes.append(f"{name}: {value} is implausible (expected 1900-2100)")
    if record.capex is not None and not record.currency:
        notes.append("capex present but currency is empty")
    if record.opex is not None and not record.currency:
        notes.append("opex present but currency is empty")
    for a, b in (("ratios_in", "carriers_in"), ("units_in", "ratios_in"),
                 ("ratios_out", "carriers_out"), ("units_out", "ratios_out")):
        left, right = getattr(record, a), getattr(record, b)
        if left and right and len(left) != len(right):
            notes.append(f"{a} ({len(left)}) and {b} ({len(right)}) length mismatch")
    return notes
