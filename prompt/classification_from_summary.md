TASK: Convert the following {{TECHNOLOGY_COUNT}} technology summaries into a JSON array.
The summaries were previously extracted from a PDF and are organised by technology and year.

You are an expert of energy systems data extractor specialised in techno-economic analysis. Extract data precisely and return only valid JSON.

JSON Schema:
[{
  "Datapaper Tech ID": "(abbrevation_year) unique id",
  "description": "1-2 sentences",
  "summary": "paragraph",
  "unit_operation": "name",
  "ProcessType": "e.g. Conversion, Storage, Capture, Transport, EndUse, etc (what it does)",
  "main_sector": "e.g. Electricity, Heat, Chemicals, Fuels, Industry, Buildings, etc (broadest)",
  "main_category": "e.g. Electrolysis, CO2 Capture, Syngas Production, etc (field)",
  "category_spec": "e.g. Alkaline, PEM, Solid sorbent, Aqueous, etc (type)",
  "tech_type": "specific name found in source (most specific)",
  "carriers_in": "c1,c2,c3 (any carriers)",
  "main_input": "primary carrier",
  "ratios_in": "r1,r2,r3",
  "units_in": "u1,u2,u3",
  "carriers_out": "c1,c2,c3 (any carriers)",
  "main_out": "primary carrier",
  "ratios_out": "r1,r2,r3",
  "units_out": "u1,u2,u3",
  "reference_unit_size": <num|null>,
  "reference_unit_size_unit": "e.g. MW, t/yr, kg/s (any unit found)",
  "efficiency": <0-1 decimal|null> (prefer LHV if available),
  "efficiency_unit": "e.g. %, kWh/kg, J/mol (any unit found)",
  "trl_(1-9)": <1-9|null>,
  "tech_maturity": "e.g. Mature, Developing, Emerging (use source terminology)",
  "base_year": <year|null>,
  "location": "e.g. Germany, Europe, Chile, Iceland (any location)",
  "Currency": "e.g. EUR, USD, GBP (any currency found)",
  "capex": <num|null>,
  "capex_unit": "e.g. EUR, EUR/kW, EUR/t (any unit found)",
  "opex_fix": <num|null>,
  "opex_fix_unit": "e.g. EUR/year, % of Capex, EUR/kW/year (any unit)",
  "lifetime_yr": <num|null>,
  "Data Reference Year": <year|null>
}]

HIERARCHY (General -> Specific):
- ProcessType, main_sector, main_category, category_spec, tech_type: classify the technology into a hierarchy

Rules:
- One object per technology; multiple years -> separate objects
- Use null where {{SOURCE_LABEL}} has no data
- efficiency: 0-1 decimal (65% -> 0.65, prefer LHV)
- ratios: one per carrier, same order as carriers
- costs: convert to single currency number (€28.4M -> 28400000)
- Return ONLY JSON array, no markdown or commentary

TECHNOLOGY SUMMARIES:

{{TECHNOLOGY_SECTIONS}}

OUTPUT INSTRUCTIONS (mandatory):
- Your entire response MUST be a single raw JSON array: [ ... ]
- Start your response with [ and end it with ]
- Do NOT write any explanation, preamble, summary, or markdown
- Do NOT say what you are doing - just output the JSON
