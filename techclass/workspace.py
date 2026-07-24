"""Workspace directory model shared by console, web, and pipeline code."""

from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from .chat import ChatClient


@dataclass(slots=True)
class Workspace:
    """Resolved project root plus derived input/output locations."""

    root: Path
    client: ChatClient
    model: str

    @classmethod
    def create(cls, root: Path, client: ChatClient, model: str) -> "Workspace":
        return cls(root.resolve(), client, model)

    @property
    def pdf_dir(self) -> Path:
        return self.root / "01_input" / "11_pdf_to_analyze"

    @property
    def cache_dir(self) -> Path:
        return self.root / "01_input" / "12_condensed_md"

    @property
    def tech_list_dir(self) -> Path:
        return self.root / "01_input" / "13_technology_list_md"

    @property
    def md_dir(self) -> Path:
        return self.root / "02_output" / "21_tech_summary_md"

    @property
    def csv_dir(self) -> Path:
        return self.root / "02_output" / "22_tech_classification_csv"

    @property
    def validation_dir(self) -> Path:
        return self.root / "02_output" / "23_validation"

    @property
    def benchmark_dir(self) -> Path:
        return self.validation_dir / "benchmark"

    @property
    def check_dir(self) -> Path:
        return self.validation_dir / "condensed_md_check"

    @property
    def classify_check_dir(self) -> Path:
        return self.validation_dir / "classification_csv_check"

    def ensure_directories(self) -> None:
        """Create all runtime input, output, and validation directories."""

        for path in (
            self.pdf_dir, self.cache_dir, self.tech_list_dir, self.md_dir,
            self.csv_dir, self.benchmark_dir, self.check_dir, self.classify_check_dir,
        ):
            path.mkdir(parents=True, exist_ok=True)
