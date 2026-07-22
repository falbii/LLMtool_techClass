from techclass.pipeline import MISSING_SECTION, parse_batched_response, parse_technology_names


def test_batch_parser_uses_declared_numbers():
    response = "=== TECHNOLOGY 2: B ===\nsecond\n=== TECHNOLOGY 1: A ===\nfirst"
    assert parse_batched_response(response, 3) == ["first", "second", MISSING_SECTION]


def test_technology_name_parser_handles_numbered_list():
    assert parse_technology_names("Technologies:\n1. Alkaline electrolysis\n2. PEM electrolysis") == [
        "Alkaline electrolysis", "PEM electrolysis",
    ]
