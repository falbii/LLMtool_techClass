from decimal import Decimal

import pytest

from techclass.helpers.classifier import (
    extract_abbreviation, extract_json, generate_tech_id, has_meaningful_data,
    is_valid_json_array, merge_by_technology_and_year, parse_record, parse_rows_from_json,
)
from techclass.format_output.models import TechnologyRecord


@pytest.mark.parametrize(("value", "expected"), [
    ("[]", True), ('[{"name":"AEC"}]', True), ("{}", False), ("[", False), ("", False),
])
def test_is_valid_json_array(value, expected):
    assert is_valid_json_array(value) is expected


def test_extract_json_finds_wrapped_array():
    response = 'Here are records:\n```json\n[{"tech_type":"AEC"}]\n```'
    assert extract_json(response) == '[{"tech_type":"AEC"}]'


def test_extract_json_skips_incomplete_attempt():
    response = '[{"broken": true}\n```json\n[{"tech_type":"PEM"}]'
    assert extract_json(response) == '[{"tech_type":"PEM"}]'


def test_parse_rows_converts_values():
    rows = parse_rows_from_json('[{"year":2035,"active":true,"carriers":["H2","O2"],"missing":null}]')
    assert rows == [{"year": "2035", "active": "true", "carriers": "H2, O2"}]


def test_parse_record_normalizes_headers_and_european_numbers():
    record, errors = parse_record({
        "Tech Type": "Alkaline electrolysis", "base_year": "2,035",
        "Reference Unit Size": "1.500 MW", "overall_efficiency": "0,650",
        "capex": "1.234,5 EUR/kW", "currency": "EUR",
    })
    assert not errors
    assert record.tech_type == "Alkaline electrolysis"
    assert record.year == 2035
    assert record.reference_unit_size == 1500
    assert record.efficiency == 0.65
    assert record.capex == Decimal("1234.5")


def test_parse_record_reports_bad_numbers():
    record, errors = parse_record({"year": "unknown", "capex": "not available"})
    assert record.year is None and record.capex is None
    assert len(errors) == 2


@pytest.mark.parametrize(("value", "expected"), [
    ("Electrolysis", "ELE"), ("Fischer-Tropsch synthesis", "FTS"), ("", "UNK"),
])
def test_abbreviation(value, expected):
    assert extract_abbreviation(value) == expected


def test_generated_id_avoids_collision():
    record = TechnologyRecord(main_input="Carbon dioxide", unit_operation="Fischer-Tropsch synthesis",
                              process_type="Conversion", main_out="Synthetic fuel")
    assert generate_tech_id(record, {"CD_FTS_CON_SF"}) == "CD_FTS_CON_SF_2"


def test_meaningful_data_requires_two_fields():
    assert not has_meaningful_data(TechnologyRecord(year=2050))
    assert has_meaningful_data(TechnologyRecord(year=2050, main_sector="Fuels"))


def test_merge_fills_missing_fields():
    rows = [TechnologyRecord(tech_type="AEC", year=2035, description="First"),
            TechnologyRecord(tech_type="AEC", year=2035, main_sector="Hydrogen")]
    merged = merge_by_technology_and_year(rows)
    assert len(merged) == 1
    assert merged[0].description == "First" and merged[0].main_sector == "Hydrogen"


def test_merge_keeps_rows_without_year_separate():
    rows = [TechnologyRecord(tech_type="AEC", description="First"),
            TechnologyRecord(tech_type="AEC", description="Second")]
    assert len(merge_by_technology_and_year(rows)) == 2

