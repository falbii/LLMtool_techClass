from techclass.models import TechnologyRecord
from techclass.verifier import contains, extract_numbers, verify


def test_extract_numbers_handles_separator_conventions():
    numbers = extract_numbers("Costs are 1,234.5 EUR and 2.345,6 EUR; efficiency is 0.65.")
    assert 1234.5 in numbers and 2345.6 in numbers and 0.65 in numbers


def test_extract_numbers_preserves_whitespace_values():
    assert extract_numbers("range: 500 1400") == [500.0, 1400.0]


def test_contains_uses_tight_tolerance():
    assert contains([0.65, 2035], 0.65)
    assert not contains([0.65, 2035], 0.651)


def test_verify_accepts_percent_equivalent():
    records = [TechnologyRecord(tech_id="AEC_2035", year=2035, efficiency=0.65, lifetime=25)]
    report = verify(records, "In 2035, efficiency reaches 65 percent.")
    assert report.total_values == 3 and report.verified_values == 2
    assert report.unverified[0].field == "lifetime"


def test_verify_empty_numeric_record_is_one_hundred_percent():
    report = verify([TechnologyRecord()], "No numeric data")
    assert report.total_values == 0 and report.verified_percent == 100

