You are an expert energy systems data extractor specialised in techno-economic analysis. Extract data precisely.

TASK: Extract ALL data for these {{TECHNOLOGY_COUNT}} technologies from this PDF:

{{TECHNOLOGY_LIST}}

For EACH technology listed above, extract and report the following data in this order, separated by year:
- Process description including operating conditions
- ALL inputs (materials, energy, consumables) with quantities and units
- ALL outputs (products, byproducts) with quantities and units
- CAPEX (capital costs) in any format mentioned
- OPEX (operating costs) as % or absolute values
- Efficiency values and units
- Technology readiness level (TRL) or maturity
- Lifetime, reference capacity, or scale information
- Year/time horizon the data applies to
- Location or region if specified
- LCA or environmental impact data if mentioned, like GHG emissions
- Any other technical or economic data you find relevant, like unit size, energy requirements, etc.

BE COMPREHENSIVE:
- Include ALL numeric values you find (even if scattered across pages)
- Report units exactly as stated (MWh/t, kg/t, EUR/kW, etc.)
- If data varies by location, report baseline + variations
- Do NOT create separate technology sections for different years or scenarios
- Within each technology section, organise data into sub-sections by year/time horizon

OUTPUT FORMAT (the section bodies are saved into a Markdown file — format them as markdown):
=== TECHNOLOGY 1: [Name] ===

### Year: 2025 (current/baseline)
- **Description:** [process, operating conditions]
- **Inputs:** [carrier: quantity unit, one bullet item per carrier]
- **Outputs:** [carrier: quantity unit, one bullet item per carrier]
- **CAPEX:** [value unit] | **OPEX:** [value unit]
- **Efficiency:** [value unit]
- **TRL / maturity:** ...
- [any further data as bullets]

### Year: 2035 (near future)
[same bullet structure]

=== TECHNOLOGY 2: [Name] ===
[same sub-section structure by year]
etc.

IMPORTANT OUTPUT RULES:
- Start your response directly with '=== TECHNOLOGY 1: ...' - no preamble or introduction
- Keep the '=== TECHNOLOGY N: [Name] ===' section markers EXACTLY in that form (do not turn them into markdown headings)
- Inside sections use only '###' or smaller headings and bullet lists — never '#' or '##'
- Stop after the last technology section - NO summary, notes, overview, or comparison table at the end
- AVOID any comparison between technologies or ranking statements like 'X is more mature than Y' or research gaps and recommendations
- just report the data for each technology as objectively as possible
- Be concise: report data values and units only; skip large verbose explanations

{{PDF_CONTENT}}

Return detailed summaries for ALL {{TECHNOLOGY_COUNT}} technologies listed above.
