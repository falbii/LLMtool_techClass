<!-- condensed from Allgoewer_2024.pdf by claude-sonnet-5 on 2026-07-24 14:36:38 -->

# Cost-Effective Locations for Producing Fuels and Chemicals from CO2 and Low-Carbon Hydrogen

Allgoewer, Becattini, Patt, Grandjean, Wiegner, Gazzani, Moretti. *Ind. Eng. Chem. Res.* 2024, 63, 13660–13676. Received 2024-04-10, Revised 2024-07-15, Accepted 2024-07-16, Published 2024-07-29.

## Abstract
Investigates cost and climate change mitigation potentials of fuels/chemicals from CO2 and low-carbon H2 produced at 4 sites (Iceland, The Netherlands, Spain, Chile). 8 chemicals: Fischer–Tropsch (FT) fuels, methanol, methane, dimethyl ether (DME), ammonia, urea, olefins, aromatics. Two horizons: near future (by 2035), long-term future (post-2035). H2 production: alkaline water electrolysis (AEC), proton exchange membrane (PEM), solid oxide electrolyzer cells (SOEC). CO2 feedstock: low-temperature solid adsorption direct air capture (LT DAC); also high-temperature aqueous absorption DAC (HT DAC) and point-source capture (PSC). Long-term future cost ratios vs. current fossil counterparts: 1–6.5×. Cost per ton CO2eq avoided: 150–750 €/t.

## Introduction (key figures)
- CCU technologies could reduce ~10% of current global CO2 emissions (direct+indirect).
- Chile aims to be world's lowest-cost green H2 producer by 2030, top H2 exporter by 2040.

## Methods

**Scope:** Full supply chain from CO2/H2 production to transport to Basel, Switzerland. CO2 sourcing: LT DAC, HT DAC, or point-source (e.g., cement plant). Electrolysis: AEC, PEM, SOEC. Horizons: near future (2035), long-term (post-2035, by 2050). Reference unit: 1 metric ton of final product (or product mix for multifunctional processes, using mass allocation).

**MSP (Minimum Selling Price) calculation (Eq 1):**
MSP = Σ[(Cy+Iy+Ty+Ly+Oy+Fy)/(1+R)^y] / Σ[Ey/(1+R)^y], summed over plant lifetime T.
- Investment lifetime T = 25 years
- Operation: 8000 h/year (except year 1 = 30% capacity, year 2 = 70% capacity)
- Capital allocation: 30% at year -2, 50% at year -1, 20% at year 1 (production start)
- Debt-to-equity ratio = 1.5
- Near-term scenario uses commercial-scale efficiencies (excludes first-of-a-kind plant premium); long-term reflects well-established technology post-2035.
- All monetary values in 2020 EUR.

**Capacity/CAPEX scaling:**
- Fixed annual CO2 input basis: 360 kt CO2/y
- Scaling (Eq 2): C = C0 × (S/S0)^k, k = 0.7 (cost curve method) for most units; no scaling (k effectively n/a) applied to DAC and H2 electrolysis.
- Reference: Climeworks Mammoth DAC plant nominal capacity = 36 kt CO2/year; company targets multimegaton-scale capture by 2030.

**LCA:** Combustion emissions of CO2-based fuels considered carbon neutral until 2040 (per EU RED, for CO2 from hard-to-abate sectors). Nonfuel products: no credit for temporary biogenic/atmospheric carbon storage (per EU product environmental footprint guidelines). Infrastructure construction/decommissioning GHG emissions neglected (per EU well-to-wheel methodology).

**Cost of CO2 Avoided (CCA, Eq 3):**
CCA = (MSP_CCU − price_fossil) / (Em_fossil − Em_CCU) [€/t CO2eq avoided]
- Fossil fuel comparators (EU RED): liquid/gaseous fuels = 94 kg CO2eq/GJ_LHV and 80 kg CO2eq/GJ_LHV
- Aromatics/olefins fossil benchmark: EU fossil PET cradle-to-grave GHG (EoL mix: 60% recycling, 20% incineration, 20% landfilling)
- Ammonia/urea fossil benchmark: cradle-to-gate GHG (ecoinvent 3.9.1 cutoff: "ammonia, anhydrous, liquid RER" via steam reforming; "urea RER market for urea")

## Locations & Energy Supply
- **Iceland:** geothermal (electricity + LT heat <100°C); colocated with Climeworks DAC plant, Hellisheidi.
- **The Netherlands:** offshore wind (near Rotterdam port) + LT heat from industrial residual heat.
- **Spain (Tabernas):** PV + LT heat from industrial processes or solar thermal collectors (high direct normal irradiance).
- **Chile (Sierra Gorda):** PV + LT heat from industrial processes or solar thermal collectors (high direct normal irradiance).
- HT heat (>100°C) at all locations: NG-fed boilers.
- Chile storage options analyzed: PV+BESS (battery energy storage), CSP+TES (concentrated solar power + thermal energy storage); LCOEs include storage costs for 8000 h/y full-load operation.

**Cooling energy (WCT – wet cooling tower), used e.g. for electrochemical CO2 reduction, methanol-to-aromatics:**
- Baseline CAPEX: 24.5 k€/MW_cooling (range 22–27 k€/MW)
- Baseline electricity use: 33.0 Wh_el/kWh_cooling (range 5.0–60.0 Wh_el/kWh_cooling)
- WCT size range: 8.0–284.2 MW; no scaling factor (modular).

## Table 1. CAPEX for Production Capacities (fixed input 360 kt CO2/y)

| Process | CAPEX baseline [M€] | CAPEX min [M€] | CAPEX max [M€] | Capacity |
|---|---|---|---|---|
| RWGS | 28.4 | 14.4 | 42.5 | 229.3 kt_CO2/y |
| ER of CO2 | 95.5 | 68.9 | 122.2 | 225.3 kt_CO2/y |
| FT synthesis (via RWGS) | 100.1 | 44.9 | 195.8 | 116.4 kt_FT-fuel/y |
| FT synthesis (via ER) | 98.8 | 44.4 | 193.4 | 114.4 kt_FT-fuel/y |
| MeOH synthesis (direct route) | 120.2 | 59.0 | 181.9 | 246.6 kt_methanol/y |
| Methanation | 36.8 | 27.8 | 41.7 | 124.6 kt_methane/y |
| DME synthesis | 85.7 | 53.6 | 135.7 | 186.7 kt_DME/y |
| Haber–Bosch process | 132.2 | 107.9 | 165.8 | 277.4 kt_ammonia/y |
| Urea synthesis | 50.1 | 24.6 | 74.2 | 491.8 kt_urea/y |
| MTO synthesis | 92.1 | 77.0 | 107.1 | 93.8 kt_olefins/y |
| MTA synthesis | 16.4 | 14.1 | 18.6 | 97.2 kt_aromatics/y |

CAPEX includes all necessary subunits (compressors, heat exchangers, N2 separation unit for Haber-Bosch, etc.).

## Table 2. Fossil-Based Products Benchmark Data

| Parameter | Unit | Jet fuel/diesel | Methanol | Methane | DME | Ammonia | Urea | Olefins | Aromatics |
|---|---|---|---|---|---|---|---|---|---|
| 2020 avg price | €/t_product | 456 | 261 | 166 | 356 | 193 | 202 | 782 | 605 |
| GHG emissions | kg CO2eq/t_product | 4164 | 1870 | 3984 | 2304 | 2660 | 1406 | 3200 | 3200 |
| LHV | MJ/kg_product | 44.3 | 19.9 | 49.8 | 28.8 | — | — | — | — |

## Table 3. Cost and Life-Cycle GHG Emissions of Energy Supply (Near vs. Long-Term Future)

| Technology | Unit | Near baseline | Near min | Near max | Long-term baseline | Long-term min | Long-term max |
|---|---|---|---|---|---|---|---|
| **Cost — Geothermal** | €/MWh_el | 25.6 | 16.5 | 35.0 | — | — | — |
| Offshore wind | €/MWh_el | 63.9 | 50.0 | 80.0 | 40.0 | 25.0 | 55.0 |
| PV (Spain) | €/MWh_el | 34.7 | 23.0 | 45.0 | 18.1 | 14.0 | 20.4 |
| PV (Chile) | €/MWh_el | 26.4 | 18.4 | 38.9 | 13.7 | 11.0 | 15.0 |
| PV-BESS | €/MWh_el | 68.2 | 50.0 | 88.0 | 45.2 | 30.0 | 66.0 |
| CSP-TES | €/MWh_el | 82.1 | 79.0 | 88.0 | 63.1 | 47.0 | 88.0 |
| Natural gas | €/MWh_th | 25.0 | 15.0 | 40.0 | — | — | — |
| Waste heat | €/MWh_th | 7.5 | 5.0 | 10.0 | — | — | — |
| **Emissions — Geothermal** | kg CO2eq/MWh_el | 17.0 | 8.5 | 32.0 | 13.9 | 5.8 | 21.9 |
| Offshore wind | kg CO2eq/MWh_el | 12.6 | 4.6 | 19.0 | 11.3 | 4.4 | 18.2 |
| PV | kg CO2eq/MWh_el | 66.0 | 20.0 | 123.8 | 57.2 | 15.9 | 98.4 |
| PV-BESS | kg CO2eq/MWh_el | 124.3 | 95.0 | 153.0 | — | — | — |
| CSP-TES | kg CO2eq/MWh_el | 16.0 | 9.8 | 24.3 | — | — | — |
| Natural gas | kg CO2eq/MWh_th | 232.5 | — | — | — | — | — |

Future emission reduction potentials by 2050: geothermal 63%, offshore wind 9%, PV 41% (linearly interpolated to 2035). NG cost min/max reflect avg EU price 2020 vs. April 2023 EU price.

## Table 4. LT DAC — Energy, Costs, Emissions

| Parameter | Unit | Near baseline | Near min | Near max | Long-term baseline | Long-term min | Long-term max |
|---|---|---|---|---|---|---|---|
| Electricity | MWh_el/t_CO2 | 0.48 | 0.25 | 0.70 | 0.38 | 0.25 | 0.50 |
| Heat | MWh_th/t_CO2 | 2.53 | 1.75 | 3.30 | 1.50 | — | — |
| Sorbent consumption | kg/t_CO2 | 7.50 | — | — | 3.00 | — | — |
| CAPEX | €/(t_CO2·y) | 730.0 | — | — | 168.5 | 100.0 | 237.0 |
| Sorbent cost | €/kg_sorbent | 21.5 | 13.0 | 30.0 | 7.4 | 1.8 | 13.0 |
| Sorbent emissions | kgCO2eq/t_CO2 | 28.0 | 46.0(?) | 10.0/15.0 | 10.0 | — | 20.0 |

(Note: Table 4 sorbent emissions row lists near future 28.0 baseline, 46.0 as alt value, 10.0 and 15.0 as min/max; long-term baseline 10.0, min not stated, max 20.0 — long-term assumed lower range of near-future values.)

## Other CO2 Feedstock Data
- Point-source capture (PSC) cost: typically €30–70 per ton CO2 (postcombustion amine capture); baseline assumed = €50/t CO2.
- PSC carbon footprint: 20–300 kg CO2eq/t CO2 (EU RED carbon recycling approach); baseline assumed = 100 kg CO2eq/t CO2 (typical for hard-to-abate industries, e.g., cement plants).
- HT DAC and PSC: ambient condition effects on CAPEX/energy neglected (lower granularity than LT DAC); noted that ambient conditions can still affect HT DAC (per An et al.).
- LT DAC weather basis: 2017 hourly air temperature/humidity data; Rotterdam via Dutch Meteorological Institute, other locations via Meteoblue. Adjustment factors normalized relative to Iceland conditions (Table 5 — not fully shown).

## Electrolyzer Technologies
- AEC and PEM: operate at 60–90 °C; MW-scale, flexibly operated plants already realized.
- SOEC: operates at 700–850 °C; considered below thermoneutral conditions; HT steam assumed generated via NG (heat integration possible but not assumed).
- Key parameters (efficiency, CAPEX, stack lifetime) summarized in Table 6 (not shown in extracted text).

## Transport
- Final destination: Basel, Switzerland.
- Transport cost depends on method and distance (km).
- Netherlands → Switzerland: freight trains assumed.

# Table 5. Cost and Energy Factors for LT DAC by Location
(Normalized to Iceland; CAPEX includes sorbent; electricity/heat factors apply to DAC energy consumption)

| factor | Iceland | Netherlands | Spain | Chile |
|---|---|---|---|---|
| CAPEX (including sorbent) | 1.00 | 1.32 | 1.69 | 1.38 |
| electricity | 1.00 | 0.99 | 1.29 | 1.29 |
| heat | 1.00 | 1.11 | 1.33 | 1.30 |

Delivery assumed to Basel (Switzerland). Netherlands delivery via freight trains. Spain/Chile/Iceland: product transported to nearest industrial port by train, then by freighter to Genoa (Italy) for Spain/Chile or Rotterdam (Netherlands) for Iceland, remaining route to Basel by train.

# Table 6. Energy Consumption and Cost Parameters for Electrolyser Technologies
(Values inter-/extrapolated to 2035 where needed; SOEC long-term energy requirements assumed same as near future)

| parameter | unit | Near future baseline | Near future min | Near future max | Long-term baseline | Long-term min | Long-term max |
|---|---|---|---|---|---|---|---|
| **AEC** | | | | | | | |
| CAPEX (system) | €/kW | 790.5 | 437.8 | 1110.3 | 527.2 | 306.5 | 774.5 |
| OPEX (stack) | % of system | 48.9 | 45.1 | 54.0 | 48.8 | 46.5 | 52.9 |
| lifetime (stack) | h | 75532 | 60000 | 94444 | 87500 | 80000 | 100000 |
| electric efficiency | kWh_el/kg_H2 | 55.1 | 47.5 | 75.0 | 48.9 | 45.47 | 52.0 |
| **PEM** | | | | | | | |
| CAPEX (system) | €/kW | 1047.9 | 613.0 | 1225.9 | 473.5 | 257.9 | 700.5 |
| OPEX (stack) | % of system | 42.0 | 28.6 | 60.0 | 39.4 | 27.8 | 55.6 |
| lifetime (stack) | h | 64026 | 40000 | 90000 | 85420 | 50000 | 100000 |
| electric efficiency | kWh_el/kg_H2 | 57.9 | 48.8 | 83.0 | 53.1 | 47.0 | 64.0 |
| **SOEC** | | | | | | | |
| CAPEX (system) | €/kW | 1739.5 | 593.0 | 2770.0 | 958.3 | 566.5 | 1723.3 |
| OPEX (stack) | % of system | 27.0 | 23.5 | 30.0 | 13.8 | 12.5 | 15.0 |
| lifetime (stack) | h | 30308 | 20000 | 50924 | 71991 | 53750 | 102222 |
| electric efficiency | kWh_el/kg_H2 | 34.1 | 26.6 | 38.1 | (same as near future) | | |
| thermal energy | kWh_th/kg_H2 | 8.3 | 6.7 | 11.0 | (same as near future) | | |

# Results

**Long-Term Future Minimum Selling Prices (MSP)**
Baseline MSP ranges (long-term future):
- FT fuels (RWGS): 1150–1750 €/t
- FT fuels (ER): 1200–1900 €/t
- Methanol: 580–840 €/t
- Methane: 1050–1730 €/t
- DME: 695–1060 €/t
- Ammonia: 410–670 €/t
- Urea: 360–510 €/t
- Olefins: 1515–2240 €/t
- Aromatics: 1360–2050 €/t

Lowest long-term MSPs: Chile < Spain < Iceland < Netherlands. On average, Chile production costs ~5% lower than Spain, ~15% lower than Iceland, ~45% lower than Netherlands. Electricity (via electrolysis) is major cost contributor. Only for ammonia/urea, Spain long-term MSPs ≈ Chile (due to lower electricity consumption per ton and lower interest rate in Spain).

Under baseline for Chile, MSPs of all products except methane are <2.5× fossil counterparts. European Refining Association: long-term FT fuel cost <1.9 €/l (2200 €/t) in Chile, <2.0 €/l (2350 €/t) in South Europe (baseline discount rate 8% vs. 6% used here); 4% discount rate → 20–30% cost reduction.

SOEC vs AEC/PEM: SOEC reduces MSP by 42–218 €/t in Netherlands; in Chile benefit negligible (3–11 €/t_product).

**Long-Term Future Life-Cycle GHG Emissions**
GHG reductions vs fossil counterparts (long-term):
- FT fuels (RWGS): 55–88% lower
- FT fuels (ER): 54–92% lower
- Methanol: 65–91% lower
- Methane: 62–94% lower
- DME: 77–96% lower
- Ammonia: 72–96% lower
- Urea: 68–89% lower
- Olefins: 50–87% lower
- Aromatics: 55–87% lower

Netherlands (AEC) has lowest cradle-to-gate GHG emissions: ~6%, 50%, 55% lower than Iceland, Spain, Chile respectively. Example: DME in Chile (AEC) → 3.6× higher life cycle GHG than Netherlands. AEC only slightly better than PEM (GHG); SOEC can be >2× life cycle GHG of AEC/PEM (due to natural gas for high-T steam). Methane production with SOEC: assuming 90% methane boiler efficiency, production capacity 124.6 kt_CH4/y, LHV 49.9 MJ/kg_CH4 → ~1/3 (41.6 kt_CH4) of CCU methane produced must be reused to reduce SOEC life cycle GHG emissions.

**Near Future Scenario**
Iceland production (avg across products/electrolyzers) is 45%, 29%, 17% cheaper than Netherlands, Spain, Chile respectively. In Iceland+AEC (near future, cheapest case): ~20% of MSP is LT DAC sorbent cost. For FT fuels (RWGS): sorbent cost/consumption decline = 33.4% of total reduction potential between near/long-term scenarios (405 €/t_product absolute). Electricity cost decrease + electrolyzer efficiency improvements = ~1/5 of total MSP reduction potential. Overall MSP decline estimated 54.2% (near→long-term), projected annual decrease 5.1% (15-year horizon).

Life cycle GHG emissions (near future, avg across products/electrolyzers/locations): 17.1% higher than long-term (1319 kg_CO2eq/t_product). Netherlands lowest emissions; AEC most favorable electrolyzer. Electricity accounts for 144.5 kg_CO2eq/t_product more emissions in near future vs. long-term.

**Effect of Carbon Source**
PSC (point-source capture) gives lowest MSPs. Near-term: PSC-based products ~50% cheaper than LT DAC on average. Long-term: LT DAC vs PSC cost difference marginal (electricity dominates). HT DAC (absorption): no advantage over PSC near-term; no cost benefit vs LT DAC long-term (still needs natural gas).

Example (Iceland, AEC, long-term): PSC → avg 7.6% cheaper MSPs than LT DAC but emits 10.9% more GHG than LT DAC. HT DAC vs LT DAC: MSP avg 21.5% higher; life cycle GHG 1.5× higher (energy-intensive calcination requiring natural gas).

**Cost per Ton of CO2 Avoided (CCA)**
Near-future baseline CCA ranges:
- FT fuels, methanol, methane, DME: 543–1969 €/t_CO2eq,av
- Ammonia, urea: 203–1087 €/t_CO2eq,av
- Olefins/aromatics: highly variable; in Spain/Chile (PV, SOEC): 5062–26,739 €/t_CO2eq,av; in Iceland/Netherlands: 986–2561 €/t_CO2eq,av
(Fossil counterpart emissions assumed aligned with European plastics lifecycle; only 20% of European plastics assumed incinerated; no credits for temporary carbon storage from air/biological sources.)

Long-term future CCA ranges:
- FT fuels, methane, methanol, DME: 225–537 €/t_CO2eq,av
- Ammonia, urea: 110–266 €/t_CO2eq,av
- Aromatics, olefins: 395–1730 €/t_CO2eq,av

Comparison: European Refining Association baseline long-term CCA: 400–650 €/t_CO2eq,av (e-methane, e-methanol), 500–800 €/t_CO2eq,av (e-kerosene).

AEC = best CCA option (both scenarios); PEM–AEC gap narrows long-term. Iceland lowest CCA (near-term, and long-term for most products). Exception: ammonia — Chile/Spain CCA slightly lower than Iceland, approaching 100 €/t_CO2eq,av (close to 2023 EU ETS CO2 price). CCA calculations based on 2020 fossil counterpart prices.

# Discussion

**Energy Storage Impact (8000 full load hours, standalone system)**
Long-term future, FT fuels (RWGS, AEC, LT DAC, Chile):
- PV-BESS: MSP +57.9% (reaching 1830 €/t) vs. PV LCOE without storage (avg +51.2% across all products)
- CSP-TES: MSP for FT fuel (RWGS) → 2245 €/t (+93.7% vs. no storage)

GHG: CSP-TES ≈ wind energy in Netherlands (~10% higher emissions). PV-BESS: ~2 t CO2eq/t more than CSP-TES. CCA: CSP-TES 1.5× higher than no-storage case; PV-BESS 4.4× higher.

**Other Key Parameters**
Long-term MSP most sensitive to electricity prices and operating hours. Reducing operating hours 8000→4000 can be offset by cheaper electricity. Central European countries: long-term electricity prices expected below 25–50 €/MWh most hours March–October. Near-term MSP also highly sensitive to sorbent costs and discount rates.

# Conclusions
Long-term future baseline MSPs projected at 2.2–3.0× fossil counterparts for methanol, DME, ammonia, urea, aromatics. Olefins/FT fuels: 2.5–3.3× fossil counterparts. Methane: 6.3–8.9× higher. Optimistic scenario: methanol, DME, ammonia, urea, olefins, aromatics could reach <1.5× fossil cost.

Long-term CCA baseline: 150–750 €/t_CO2eq,av across all products; DME, ammonia, urea estimated <230 €/t_CO2eq,av.

Netherlands = most favorable for life cycle GHG emissions (offshore wind). Iceland = lowest MSP near-term; Chile/Spain = lowest MSP long-term future.

Qualitative findings: (1) electrolyzer choice has minor economic impact vs. location/electricity source; (2) SOEC hydrogen production has much higher life cycle GHG emissions (if natural gas used) than other electrolyzers; (3) product transportation has minor cost/climate impact vs. production energy/technology.

This section is a references/acknowledgments list containing no extractable technical, numeric, or LCA data relevant to preservation — only citation metadata (authors, journals, years, DOI/URLs). Condensed below with all substantive content retained (funding grant numbers, which are the only factual data present).

## Contact
Email: christian.moretti@psi.ch

## Notes
Paul Grandjean currently employed by de Pury Pictet Turrettini; contribution made prior to appointment, no company role in study. No competing financial interests declared.

## Acknowledgments
- Sponsored by Swiss Federal Office of Energy's 'SWEET' program, PATHFNDR project (Grant Number SI/502259)
- Additional funding: SWEET reFuel.ch project (Grant Number SI/502717)
- ETH Future Mobility program, MI-SUNFUELS project (Grant Number 2021-HS-216, MI-05-21)

## References
(118 references cited, numbered 1–118, covering topics: CO2 utilization product selection, industrial carbon management, methanol CCU techno-economics, aviation net-zero pathways, direct air capture (DAC), hydrogen economy, power-to-methanol LCA, electrolyser costs (alkaline/PEM), energy storage systems, Climeworks DAC capacity, Swiss energy system 2050, solar thermochemical fuels, climate policy 1.5°C scenarios, building energy retrofits, captured carbon feedstock environmental impact, TEA/LCA guidelines for CO2 utilization, electrofuels review, carbon footprint of CO2 feedstock, EU RFNBO methodology, Product Environmental Footprint method, green hydrogen agreements (Chile-Germany, Chile-World Bank), solid sorbent DAC process design, regional climate DAC deployment, biomass-to-jet fuel, microalgae hydrothermal liquefaction, chemical engineering design economics, solar thermochemical fuel pathway assessment, DAC plant TEA, SCENT costing methodology, synthetic aviation fuel (Netherlands), methanol/Fischer-Tropsch fuel production (US), CO electrolysis TEA, formic acid LCA, natural gas-to-liquids Fischer-Tropsch, electrofuels production cost review, methanol synthesis from captured CO2, liquid/gaseous fuel synthesis via electrolysis, light-duty transport fuels from renewable H2/CO2, synthetic methane production costs (2030/2050), CO2 as chemical feedstock, dimethyl ether TEA/GHG, ammonia synthesis TEA (Germany), hydrogen/ammonia synthesis (Paraguay, Itaipu 14 GW plant), green ammonia production (wind/solar), urea production TEA, DMC production from CO2/ammonia/methanol, methanol-to-propylene TEA, natural gas liquids/methanol to olefins, aromatics production TEA/LCA, GHG default emissions for biofuels (EU), refinery GHG allocation methods, bio-based/petrochemical PET bottle LCA, jet fuel price monitoring (IATA, EIA), gasoline/diesel price trends (FuelsEurope), power-to-liquid synthesis review, methanol pricing (Methanex), natural gas prices (IEA), DME production review, ammonia/urea commodity pricing (USGS, Indexmundi), methanol-to-olefins plant design, aromatics/syngas from shale gas/CO2, Ecoinvent database, electrofuels LCA (corn ethanol CO byproduct), combustion fundamentals, German energy system LCA, enhanced geothermal systems LCA (Reykjanes, Vendenheim), renewable power generation costs 2020/2021 (IRENA), geothermal policy (Andean region), offshore wind TEA/LCOE, PV/wind CAPEX and LCOE projections to 2050, wind/solar learning curves and cost declines (37–49% by 2050), utility-scale PV LCOE (WACC/CAPEX factors), European solar cost assessment, PV integration costs, PV/CSP hybrid competitiveness (copper mining), solar H2 production (Chile/Atacama-Japan export), CSP-PV integration, solar tech district (Diego de Almagro, Chile), Chile renewable energy pathway, flexible green H2/ammonia production (Chile/Argentina), US electricity sector outlook (NREL), CSP with storage optimization, molten salts TES for CSP (2050 LCOE), CSP+TES thermo-economic assessment (Chile), geothermal LCA (Iceland/France), geothermal electricity GHG emissions review, offshore wind GHG mapping (Guangdong, China), offshore vs onshore wind environmental/social footprint, wind energy LCA, utility-scale solar GHG/energy footprint, thin-film PV GHG emissions, crystalline silicon PV GHG emissions, lithium-ion/vanadium redox flow battery LCA, battery storage LCA comparison, battery storage techno-environmental analysis, CSP tower LCA with/without TES, CSP tower LCA with varying storage capacity, CSP sustainability assessment (LCA/LCC/LCWE), CSP sustainability in Europe (value chains), DAC process (Joule/Keith et al.), air-to-water heat rejection TEA, industrial DAC LCA (temperature-vacuum swing adsorption), CO2 removal from air, DAC technical performance, net-zero CO2 emission framework for synthetic fuels, wind-to-methanol production, CO2 capture adsorbent stability, DAC process TEA)

- Journal abbreviation throughout: *Ind. Eng. Chem. Res.* 2024, 63, 13660−13676 (page markers 13672–13675 across PAGE 13–17)
- DOI: https://doi.org/10.1021/acs.iecr.4c01287

[PAGE 17]
Note added after ASAP publication: Originally published ASAP July 29, 2024. Corrections made to unit of CAPEX in Tables 4 and S5. Revised version reposted August 7, 2024.