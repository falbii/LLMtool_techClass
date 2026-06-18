<!-- condensed from Allgoewer_2024.pdf on 2026-06-10 10:31:36 -->

**Source:** Allgoewer et al., *Ind. Eng. Chem. Res.* 2024, 63, 13660–13676. https://doi.org/10.1021/acs.iecr.4c01287

---

## Scope

- **Products:** Fischer–Tropsch (FT) fuels, methanol, methane, dimethyl ether (DME), ammonia, urea, olefins, aromatics
- **CO₂ sources:** LT DAC (low-temperature solid adsorption), HT DAC (high-temperature aqueous absorption), point-source capture (PSC)
- **Electrolysis types:** AEC (alkaline), PEM (proton exchange membrane), SOEC (solid oxide)
- **Locations:** Iceland, The Netherlands (Rotterdam), Spain (Tabernas), Chile (Sierra Gorda)
- **Time horizons:** near future (by 2035), long-term future (post-2035, by 2050)
- **Reference unit:** 1 metric ton final product (or product mix for FT fuels, olefins, aromatics)
- **Delivery point:** Basel, Switzerland
- **Annual CO₂ input reference capacity:** 360 kt_CO₂/y

---

## Key Findings

- Long-term future cost ratios vs. fossil counterparts: **1–6.5×**
- Cost per ton CO₂eq avoided: **150–750 €/t**

---

## Economic Assessment: MSP

$$\text{MSP} = \frac{\sum_{y=0}^{T} \frac{C_y + I_y + T_y + L_y + O_y + F_y}{(1+R)^y}}{\sum_{y=0}^{T} \frac{E_y}{(1+R)^y}}$$

- Plant lifetime (T): **25 years**
- Operating hours: **8000 h/y** (year 1: 30% capacity; year 2: 70% capacity)
- Investment allocation: 30% at −2 y, 50% at −1 y, 20% at year 0
- Debt-to-equity ratio: **1.5**
- All monetary values: **2020 euros**
- CAPEX scaling factor (eq 2): **k = 0.7**; no scaling applied to DAC and H₂ electrolysis
- DAC: no scaling applied (conservative assumption)

$$C = C_0 \times \left(\frac{S}{S_0}\right)^k$$

---

## Cost of CO₂ Avoided (CCA)

$$\text{CCA} = \frac{\text{MSP}_{\text{CCU}} - \text{price}_{\text{fossil}}}{\frac{E_m}{\text{fossil}} - \frac{E_m}{\text{CCU}}} \quad [\text{€/t}_{\text{CO}_2\text{eq,av}}]$$

---

## LCA Carbon Accounting

- CO₂-based fuels: combustion emissions considered carbon neutral until **2040** (for CO₂ from hard-to-decarbonize sectors)
- Fossil fuel comparator: **94 kg CO₂eq/GJ_LHV** and **80 kg CO₂eq/GJ_LHV** (EU RED)
- Fossil PET benchmark (aromatics/olefins): cradle-to-grave, EU mix end-of-life = **60% recycling, 20% incineration, 20% landfilling**
- Ammonia/urea: cradle-to-gate GHG (ecoinvent 3.9.1 cutoff): "ammonia, anhydrous, liquid RER without RU", "urea RER–market for urea"
- Infrastructure construction/decommissioning GHG: neglected (EU well-to-wheel methodology)
- No GHG attributed to LT waste heat

---

## Table 1. CAPEX for Production Units at 360 kt_CO₂/y Input

| Product | Process | CAPEX baseline [M€] | CAPEX min [M€] | CAPEX max [M€] | Capacity |
|---|---|---|---|---|---|
| FT fuels | RWGS | 28.4 (RWGS unit) + 100.1 (FT synth) | 14.4 / 44.9 | 42.5 / 195.8 | RWGS: 229.3 kt_CO/y; FT: 116.4 kt_FT-fuel/y |
| FT fuels | ER of CO₂ | 95.5 (ER unit) + 98.8 (FT synth) | 68.9 / 44.4 | 122.2 / 193.4 | ER: 225.3 kt_CO/y; FT: 114.4 kt_FT-fuel/y |
| Methanol | MeOH synthesis (direct route) | 120.2 | 59.0 | 181.9 | 246.6 kt_methanol/y |
| Methane | Methanation | 36.8 | 27.8 | 41.7 | 124.6 kt_methane/y |
| DME | DME synthesis | 85.7 | 53.6 | 135.7 | 186.7 kt_DME/y |
| Ammonia | Haber–Bosch | 132.2 | 107.9 | 165.8 | 277.4 kt_ammonia/y |
| Urea | Urea synthesis | 50.1 | 24.6 | 74.2 | 491.8 kt_urea/y |
| Olefins | MTO synthesis | 92.1 | 77.0 | 107.1 | 93.8 kt_olefins/y |
| Aromatics | MTA synthesis | 16.4 | 14.1 | 18.6 | 97.2 kt_aromatics/y |

CAPEX includes all subunits (compressors, heat exchangers, separation units, ASU for Haber–Bosch).

---

## Table 2. Fossil-Based Product Benchmarks

| Parameter | Unit | Jet fuel/diesel | Methanol | Methane | DME | Ammonia | Urea | Olefins | Aromatics |
|---|---|---|---|---|---|---|---|---|---|
| 2020 avg price | €/t_product | 456 | 261 | 166 | 356 | 193 | 202 | 782 | 605 |
| GHG emissions | kg_CO₂eq/t_product | 4164 | 1870 | 3984 | 2304 | 2660 | 1406 | 3200 | 3200 |
| LHV | MJ/kg_product | 44.3 | 19.9 | 49.8 | 28.8 | — | — | — | — |

---

## Table 3. Energy Supply Costs and Life-Cycle GHG Emissions

| Technology | Unit | Near future baseline | Near future min | Near future max | Long-term baseline | Long-term min | Long-term max |
|---|---|---|---|---|---|---|---|
| Geothermal | €/MWh_el | 25.6 | 16.5 | 35.0 | — | — | — |
| Offshore wind | €/MWh_el | 63.9 | 50.0 | 80.0 | 40.0 | 25.0 | 55.0 |
| PV (Spain) | €/MWh_el | 34.7 | 23.0 | 45.0 | 18.1 | 14.0 | 20.4 |
| PV (Chile) | €/MWh_el | 26.4 | 18.4 | 38.9 | 13.7 | 11.0 | 15.0 |
| PV-BESS | €/MWh_el | 68.2 | 50.0 | 88.0 | 45.2 | 30.0 | 66.0 |
| CSP-TES | €/MWh_el | 82.1 | 79.0 | 88.0 | 63.1 | 47.0 | 88.0 |
| Natural gas | €/MWh_th | 25.0 | 15.0 | 40.0 | — | — | — |
| Waste heat | €/MWh_th | 7.5 | 5.0 | 10.0 | — | — | — |
| Geothermal | kg_CO₂eq/MWh_el | 17.0 | 8.5 | 32.0 | 13.9 | 5.8 | 21.9 |
| Offshore wind | kg_CO₂eq/MWh_el | 12.6 | 4.6 | 19.0 | 11.3 | 4.4 | 18.2 |
| PV | kg_CO₂eq/MWh_el | 66.0 | 20.0 | 123.8 | 57.2 | 15.9 | 98.4 |
| PV-BESS | kg_CO₂eq/MWh_el | 124.3 | 95.0 | 153.0 | — | — | — |
| CSP-TES | kg_CO₂eq/MWh_el | 16.0 | 9.8 | 24.3 | — | — | — |
| Natural gas | kg_CO₂eq/MWh_th | 232.5 | — | — | — | — | — |

Long-term GHG reductions vs. near-future baseline: geothermal **63%**, offshore wind **9%**, PV **41%** (interpolated linearly to 2035). NG min/max = EU avg price 2020 / April 2023 EU price.

---

## Table 4. LT DAC (Solid Sorbent, Temperature–Vacuum Swing, Amine-Functionalized) Parameters

| Parameter | Unit | Near future baseline | Near future min | Near future max | Long-term baseline | Long-term min | Long-term max |
|---|---|---|---|---|---|---|---|
| Electricity | MWh_el/t_CO₂ | 0.48 | 0.25 | 0.70 | 0.38 | 0.25 | 0.50 |
| Heat | MWh_th/t_CO₂ | 2.53 | 1.75 | 3.30 | 1.50 | — | — |
| Sorbent consumption | kg/t_CO₂ | 7.50 | — | — | 3.00 | — | — |
| CAPEX | €/(t_CO₂·y) | 730.0 | 168.5 | — | 100.0 | — | 237.0 |
| Sorbent cost | €/kg_sorbent | 21.5 | 13.0 | 30.0 | 7.4 | 1.8 | 13.0 |
| Sorbent GHG emissions | kg_CO₂eq/t_CO₂ | 28.0 | 10.0 | 46.0 | 15.0 | 10.0 | 20.0 |

- Sorbent replacement included as OPEX for the specific year; electrolyzer stack replacement also as OPEX.
- LT DAC CAPEX and energy requirements adjusted per location using surrogate model (Wiegner et al.) normalized to Iceland conditions (Table 5).
- Weather data: Rotterdam = Dutch Meteorological Institute; other locations = Meteoblue (2017).

---

## Table 5. Cost and Energy Factors for LT DAC by Location

*(normalized to Iceland)*

*(values not yet reproduced — table continues on page 6)*

---

## Point-Source Capture (PSC)

- Cost range: **€30–70/t_CO₂** (postcombustion, amine absorbers)
- Assumed baseline cost: **€50/t_CO₂**
- Carbon footprint range: **20–300 kg_CO₂eq/t_CO₂**
- Assumed baseline carbon footprint: **100 kg_CO₂eq/t_CO₂** (typical for cement plants)

---

## HT DAC

- Ambient condition effects on CAPEX/energy neglected (lower granularity than LT DAC); see Section S2.2.

---

## Electrolysis

- AEC and PEM: operate at **60–90 °C**; MW-scale commercially available
- SOEC: operates at **700–850 °C**

---

## Cooling System (Wet Cooling Tower, WCT)

- Baseline CAPEX: **24.5 k€/MW_cooling** (range: 22–27 k€/MW_cooling)
- Electricity: **33.0 Wh_el/kWh_cooling** (range: 5.0–60.0 Wh_el/kWh_cooling)
- WCT capacity range in model: **8.0–284.2 MW**
- No scaling factor (modular assumption)

---

## HT Heat Supply

- All locations: HT heat (>100 °C) from natural gas (NG)-fed boilers
- Iceland: geothermal provides electricity + LT heat (<100 °C); colocated with Climeworks Hellisheidi DAC plant
- Netherlands: offshore wind (near Rotterdam); LT heat from industrial residual heat
- Spain/Chile: PV electricity; LT heat from industrial processes or solar thermal collectors
- PV-BESS and CSP-TES: LCOEs include storage costs for **8000 h/y** full-load operation

---

## Climeworks Reference

- Mammoth DAC plant nominal capacity: **36 kt_CO₂/y**
- Company target: multimegaton-scale capture by **2030**

**DAC Location Cost Factors (normalized to Iceland)**

| factor | Iceland | Netherlands | Spain | Chile |
|---|---|---|---|---|
| CAPEX (incl. sorbent) | 1.00 | 1.32 | 1.69 | 1.38 |
| electricity | 1.00 | 0.99 | 1.29 | 1.29 |
| heat | 1.00 | 1.11 | 1.33 | 1.30 |

CAPEX factor applied to LT DAC plant; electricity/heat factors to DAC energy consumption. Values from Wiegner et al. optimization model.

---

**Table 6. Electrolyser Energy Consumption and Cost Parameters**

*Values inter/extrapolated to 2035 where needed. SOEC long-term energy requirements assumed same as near-future.*

**AEC**

| parameter | unit | near future baseline | near future min | near future max | long-term baseline | long-term min | long-term max |
|---|---|---|---|---|---|---|---|
| CAPEX (system) | €/kW | 790.5 | 437.8 | 1110.3 | 527.2 | 306.5 | 774.5 |
| OPEX (stack) | % of system | 48.9 | 45.1 | 54.0 | 48.8 | 46.5 | 52.9 |
| lifetime (stack) | h | 75532 | 60000 | 94444 | 87500 | 80000 | 100000 |
| electric efficiency | kWh_el/kg_H₂ | 55.1 | 47.5 | 75.0 | 48.9 | 45.47 | 52.0 |

**PEM**

| parameter | unit | near future baseline | near future min | near future max | long-term baseline | long-term min | long-term max |
|---|---|---|---|---|---|---|---|
| CAPEX (system) | €/kW | 1047.9 | 613.0 | 1225.9 | 473.5 | 257.9 | 700.5 |
| OPEX (stack) | % of system | 42.0 | 28.6 | 60.0 | 39.4 | 27.8 | 55.6 |
| lifetime (stack) | h | 64026 | 40000 | 90000 | 85420 | 50000 | 100000 |
| electric efficiency | kWh_el/kg_H₂ | 57.9 | 48.8 | 83.0 | 53.1 | 47.0 | 64.0 |

**SOEC**

| parameter | unit | near future baseline | near future min | near future max | long-term baseline | long-term min | long-term max |
|---|---|---|---|---|---|---|---|
| CAPEX (system) | €/kW | 1739.5 | 593.0 | 2770.0 | 958.3 | 566.5 | 1723.3 |
| OPEX (stack) | % of system | 27.0 | 23.5 | 30.0 | 13.8 | 12.5 | 15.0 |
| lifetime (stack) | h | 30308 | 20000 | 50924 | 71991 | 53750 | 102222 |
| electric efficiency | kWh_el/kg_H₂ | 34.1 | 26.6 | 38.1 | — | — | — |
| thermal energy | kWh_th/kg_H₂ | 8.3 | 6.7 | 11.0 | — | — | — |

*(SOEC near-future values used for long-term scenario)*

Transport destination: Basel, Switzerland. Netherlands → freight train. Spain/Chile/Iceland → train to nearest industrial port → freighter to Genoa (Spain/Chile) or Rotterdam (Iceland) → train to Basel.

---

[PAGE 8]

**Long-Term Future MSP (post-2035), LT DAC, €/t_product — baseline ranges:**

- FT fuels (RWGS): 1150–1750 €/t
- FT fuels (ER): 1200–1900 €/t
- methanol: 580–840 €/t
- methane: 1050–1730 €/t
- DME: 695–1060 €/t
- ammonia: 410–670 €/t
- urea: 360–510 €/t
- olefins: 1515–2240 €/t
- aromatics: 1360–2050 €/t

- Lowest MSPs: Chile < Spain < Iceland < Netherlands
- Chile avg ~5% lower than Spain, ~15% lower than Iceland, ~45% lower than Netherlands
- SOEC can reduce MSP by 42–218 €/t in Netherlands; cost benefit in Chile negligible (3–11 €/t_product)
- Under baseline assumptions (Chile), all products except methane < 2.5× fossil counterpart prices
- European Refining Association long-term future production cost: <1.9 €/l (2200 €/t) Chile, <2.0 €/l (2350 €/t) South Europe; discount rate baseline 8%; 20–30% cost reduction at 4% discount rate

**Long-Term Future Life-Cycle GHG Emissions — reduction vs. fossil counterparts:**

- FT fuels (RWGS): 55–88% lower
- FT fuels (ER): 54–92% lower
- methanol: 65–91% lower
- methane: 62–94% lower
- DME: 77–96% lower
- ammonia: 72–96% lower
- urea: 68–89% lower
- olefins: 50–87% lower
- aromatics: 55–87% lower

- Netherlands products avg ~6% lower GHG than Iceland, ~50% lower than Spain, ~55% lower than Chile
- DME in Chile with AEC → 3.6× higher life cycle GHG than Netherlands
- AEC slight advantage over PEM (GHG); SOEC can result in >2× life cycle GHG vs. AEC/PEM depending on location (cause: NG for HT steam)
- SOEC methane case: assuming 90% methane boiler efficiency, production capacity 124.6 kt_CH₄/y, LHV 49.9 MJ/kg_CH₄ → ~1/3 (41.6 kt_CH₄) of produced CCU methane must be reused to reduce SOEC life cycle GHG

---

[PAGE 10]

**Near-Future Scenario:**
- Averaging all products and electrolyzers: Iceland 45% cheaper than Netherlands, 29% cheaper than Spain, 17% cheaper than Chile
- Near-future life cycle GHG avg over all products/electrolyzers/locations: 17.1% higher (1319 kg_CO₂eq/t_product) than long-term future
- Netherlands emits least; AEC most favorable electrolyzer
- Electricity accounts for avg 144.5 kg_CO₂eq/t_product more GHG in near future vs. long-term future

**MSP breakdown (near-future, Iceland, AEC):**
- ~20% of MSPs = LT DAC sorbent cost
- FT fuels (RWGS): future decline in sorbent cost/consumption = 33.4% of total MSP reduction potential between scenarios (405 €/t_product absolute)
- Electricity + electrolyzer efficiency improvements together ≈ 1/5 of total MSP reduction potential
- Overall MSPs decline avg 54.2% from near to long-term future; projected annual decrease 5.1% (15-year horizon)

**Effect of Carbon Source (FT fuels RWGS, Iceland, AEC, long-term):**
- PSC vs. LT DAC: avg 7.6% cheaper MSP; emits 10.9% more GHG than LT DAC
- Near-term: PSC products avg 50% cheaper than LT DAC
- HT DAC vs. LT DAC (long-term, Iceland, AEC): MSP avg 21.5% higher; GHG 1.5× higher (due to NG-intensive calcination)

**Cost per Ton of CO₂ Avoided (CCA):**

*Near-future, all locations/electrolyzers, LT DAC:*
- FT fuels, methanol, methane, DME: 543–1969 €/t_CO₂eq,av
- Ammonia and urea: 203–1087 €/t_CO₂eq,av
- Olefins/aromatics (Spain/Chile, SOEC): 5062–26739 €/t_CO₂eq,av
- Iceland and Netherlands: 986–2561 €/t_CO₂eq,av

*Long-term future, LT DAC:*
- FT fuels, methane, methanol, DME: 225–537 €/t_CO₂eq,av
- Ammonia and urea: 110–266 €/t_CO₂eq,av
- Aromatics and olefins: 395–1730 €/t_CO₂eq,av
- European Refining Association baseline long-term CCA: 400–650 €/t_CO₂eq,av (e-methane, e-methanol); 500–800 €/t_CO₂eq,av (e-kerosene)
- Ammonia CCA Chile/Spain approaches ~100 €/t_CO₂eq,av (≈ 2023 EU ETS CO₂ price)
- AEC best CCA both scenarios; PEM–AEC CCA difference narrows long-term
- Iceland lowest CCA near-future; also lowest long-term for most products

---

[PAGE 12]

**Energy Storage Sensitivity (long-term future, FT fuels RWGS, AEC, LT DAC, Chile):**
- PV-BESS vs. PV (no storage): MSP increases 57.9% → 1830 €/t; avg increase all products 51.2%
- CSP-TES vs. no storage: MSP for FT fuel (RWGS) → 2245 €/t (+93.7%)
- CSP-TES GHG ≈ offshore wind Netherlands (~10% higher)
- PV-BESS GHG avg ~2 t_CO₂eq/t more than CSP-TES
- CCA with CSP-TES: 1.5× higher than without storage
- CCA with PV-BESS: 4.4× higher than without storage

**Other Key Parameters (long-term, FT fuels RWGS, Iceland, AEC, Fig. 8):**
- Sensitivity factors: discount rate 5–8% (baseline 6%), scaling rate 0.6–0.9 (baseline 0.7), operating hours 4000–8000 h (baseline 8000 h)
- No scaling factor applied to DAC, electrolysis, cooling units
- Long-term: MSP sensitive mainly to electricity prices and operating hours
- Reduction in operating hours 8000→4000 h can be compensated by cheaper electricity
- Long-term future: even Central European electricity expected <25–50 €/MWh during most hours March–October
- Near-term: MSP highly sensitive also to sorbent costs and discount rates

---

[PAGE 13]

**Conclusions — Key Quantitative Findings:**

*Long-term future, LT DAC, baseline:*
- MSP range vs. fossil: 2.2–3.0× for methanol, DME, ammonia, urea, aromatics
- MSP range vs. fossil: 2.5–3.3× for olefins and FT fuels
- MSP vs. fossil: 6.3–8.9× for methane
- Optimistic scenario: methanol, DME, ammonia, urea, olefins, aromatics potentially <1.5× fossil cost
- Long-term CCA baseline: 150–750 €/t_CO₂eq,av for all products
- DME, ammonia, urea CCA: <230 €/t_CO₂eq,av

[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TECHNOLOGY LIST TABLE - EXTRACT ALL TECHNOLOGIES]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]
[TABLE REGION - IMPORTANT NUMERICAL DATA]
[END TABLE]

**NOTE ADDED AFTER ASAP PUBLICATION**

- Originally published ASAP: July 29, 2024.
- Corrections made to the unit of CAPEX in Tables 4 and S5.
- Revised version reposted: August 7, 2024.