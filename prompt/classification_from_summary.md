TASK: Convert the following {{TECHNOLOGY_COUNT}} technology summaries into a JSON array.
The summaries were previously extracted from a PDF and are organised by technology and year.

You are an expert energy-systems data extractor specialised in techno-economic analysis. Extract data precisely and return only valid JSON.

JSON Schema (field meanings and examples are defined below):
[{
  "tech_id": "<string>",
  "description": "<string>",
  "summary": "<string>",
  "unit_operation": "<string>",
  "process_type": "<string>",
  "main_sector": "<string>",
  "main_category": "<string>",
  "category_spec": "<string>",
  "tech_type": "<string>",
  "carriers_in": "<c1,c2,c3>",
  "main_input": "<string>",
  "ratios_in": "<r1,r2,r3>",
  "units_in": "<u1,u2,u3>",
  "carriers_out": "<c1,c2,c3>",
  "main_out": "<string>",
  "ratios_out": "<r1,r2,r3>",
  "units_out": "<u1,u2,u3>",
  "reference_unit_size": <num|null>,
  "reference_unit_size_unit": "<string>",
  "efficiency": <0-1 decimal|null>,
  "efficiency_unit": "<string>",
  "trl": <1-9|null>,
  "tech_maturity": "<string>",
  "year": <year|null>,
  "location": "<string>",
  "currency": "<string>",
  "capex": <num|null>,
  "capex_unit": "<string>",
  "opex": <num|null>,
  "opex_unit": "<string>",
  "lifetime": <num|null>,
  "lifetime_unit": "<string>"
}]

FIELD DEFINITIONS:

General:
- tech_id: short unique id, abbreviation_year (e.g. AEL_2030).
- description: 1-2 sentences on what the technology is and does.
- summary: short paragraph condensing the section's key data.
- location: country or region the data refers to (Germany, Europe, Chile, ...).

Classification hierarchy (general -> specific):
- process_type: the ROLE of the technology in the energy system — what it does
  (Conversion, Storage, Capture, Transport, EndUse).
- main_sector: the broadest sector the technology serves (Electricity, Heat, Chemicals, Fuels, Industry, Buildings).
- main_category: the technology family / field (Electrolysis, CO2 Capture, Syngas Production).
- category_spec: the variant within that family (Alkaline, PEM, Solid sorbent, Aqueous).
- tech_type: the MOST SPECIFIC name the source uses for this exact technology.
- unit_operation: the core process step performed (Electrolysis, Gasification, Fischer-Tropsch synthesis).

Carriers and ratios:
- carriers_in / carriers_out: ALL energy and material carriers entering / leaving the process
  (electricity, hydrogen, CO2, water, heat, ...), comma-separated.
- main_input / main_out: the single primary carrier among them.
- ratios_in / ratios_out: the input/output quantity of each carrier, in the SAME ORDER as the
  carriers list, exactly as the source states them ("9 kg water per kg H2" -> ratio 9, unit kg/kg).
- units_in / units_out: one unit per ratio, same order, exactly as the source writes them.

Performance:
- efficiency: overall conversion efficiency as a 0-1 decimal (65% -> 0.65). Prefer LHV efficiency
  when both LHV and HHV are given; note the basis in efficiency_unit ("% LHV", kWh/kg, ...).
- trl: Technology Readiness Level, integer 1-9, only if the source states one.
- tech_maturity: the source's own qualitative wording (Mature, Developing, Emerging, ...).
- reference_unit_size: the capacity/size of the reference plant or unit the data refers to,
  with its unit in reference_unit_size_unit (MW, t/yr, kg/s, ...).
- lifetime: technical or economic lifetime, with its unit in lifetime_unit exactly as the source
  states it (years for plants, but e.g. operating hours or cycles for stacks/batteries).

Costs:
- capex: the one-time capital investment cost (equipment + installation), as a number.
- opex: operating costs, INCLUDING both fixed (maintenance, labor, insurance) and variable
  (feedstock, electricity, fuel) components — report whatever the source states, with the
  exact unit in opex_unit (EUR/yr, % of CAPEX/yr, EUR/MWh, ...) so fixed vs variable stays
  distinguishable.
- If OPEX is given as a percentage of CAPEX, report the percentage number AS-IS:
  "2% of CAPEX/yr" -> opex: 2, opex_unit: "% of CAPEX/yr". Do NOT compute the absolute value.
- Copy cost numbers exactly as the source states them — do NOT convert currencies, do NOT expand
  magnitudes ("28.4 MEUR" -> capex: 28.4, capex_unit: "MEUR", NOT 28400000).
- currency: the currency the costs are stated in (EUR, USD, ...).

Years:
- year: the year or time horizon this data point DESCRIBES (e.g. a 2050 cost projection -> 2050,
  current/baseline data -> the present-day year the source uses), not the publication year.

Rules:
- One object per technology; multiple years -> separate objects
- Use null where {{SOURCE_LABEL}} has no data — NEVER guess or invent a value

TECHNOLOGY SUMMARIES:

{{TECHNOLOGY_SECTIONS}}

OUTPUT INSTRUCTIONS (mandatory):
- Your entire response MUST be a single raw JSON array: [ ... ]
- Start your response with [ and end it with ]
- Do NOT write any explanation, preamble, summary, or markdown
- Do NOT say what you are doing - just output the JSON
