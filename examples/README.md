# Examples

This repository keeps one reviewer-friendly example dataset and its generated
artifacts. The `Allgoewer_2024` name identifies the source article; its authors
are not authors of this software. The article is cited in `paper.bib` as
`allgoewer2024` (https://doi.org/10.1021/acs.iecr.4c01287).

## Included Example

- `01_input/11_pdf_to_analyze/Allgoewer_2024.pdf`
- `01_input/12_condensed_md/Allgoewer_2024_condensed.md`
- `01_input/13_technology_list_md/Allgoewer_2024_technology_list.md`
- `02_output/21_tech_summary_md/Allgoewer_2024_summary.md`
- `02_output/22_tech_classification_csv/Allgoewer_2024_classification.csv`
- `02_output/23_validation/...`

## Why These Files Are Tracked

They provide a concrete example for:

- reviewer installation and functional checking
- inspecting intermediate artifacts
- understanding the expected output structure
- validating that the repository documentation matches the actual workflow

## Recommendation

Future large or private datasets should stay out of version control unless they
are intentionally curated as public examples.
