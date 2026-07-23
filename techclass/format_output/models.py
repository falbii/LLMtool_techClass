"""Shared data models for classification records and chat model metadata."""

from __future__ import annotations

from dataclasses import dataclass, field
from decimal import Decimal


@dataclass(slots=True)
class TechnologyRecord:
    """Normalized technology classification row used for CSV output."""

    tech_id: str | None = None
    process_type: str | None = None
    description: str | None = None
    unit_operation: str | None = None
    summary: str | None = None
    main_sector: str | None = None
    main_category: str | None = None
    category_spec: str | None = None
    tech_type: str | None = None
    reference_unit_size: float | None = None
    reference_unit_size_unit: str | None = None
    year: int | None = None
    location: str | None = None
    currency: str | None = None
    ref_year: int | None = None
    trl: int | None = None
    tech_maturity: str | None = None
    efficiency: float | None = None
    efficiency_unit: str | None = None
    carriers_in: list[str] = field(default_factory=list)
    main_input: str | None = None
    ratios_in: list[float] = field(default_factory=list)
    units_in: list[str] = field(default_factory=list)
    carriers_out: list[str] = field(default_factory=list)
    main_out: str | None = None
    ratios_out: list[float] = field(default_factory=list)
    units_out: list[str] = field(default_factory=list)
    lifetime: float | None = None
    lifetime_unit: str | None = None
    capex: Decimal | None = None
    capex_unit: str | None = None
    opex: Decimal | None = None
    opex_unit: str | None = None


@dataclass(frozen=True, slots=True)
class ChatModelInfo:
    """Model metadata displayed by the CLI and web model picker."""

    id: str
    supports_reasoning: bool = False

