# Benchmark Summaries — Allgoewer_2024.pdf

- **Generated:** 2026-07-08 15:19:42
- **Models:** 18
- **Technologies:** Alkaline water electrolysis; Proton exchange membrane electrolysis; Solid oxide electrolysis

---

# MODEL: auto

- **Status:** OK  |  **Words:** 800  |  **Duration:** 40367 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: ~2035 (near future baseline)

- **Description:** Alkaline water electrolysis; dominates market alongside PEM; available at MW-scale; operates at 60–90 °C
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (baseline); min 47.5, max 75.0 kWh_el/kg_H2
  - Water (implied by electrolysis process)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
- **CAPEX:** 790.5 €/kW (system baseline); min 437.8, max 1110.3 €/kW
- **OPEX:** 48.9 % of system CAPEX (stack); min 45.1%, max 54.0%
- **Efficiency:** 55.1 kWh_el/kg_H2 (electric, baseline)
- **TRL / maturity:** Commercial / market-dominant; MW-scale available
- **Lifetime (stack):** 75,532 h (baseline); min 60,000, max 94,444 h
- **Energy source:** Renewable electricity exclusively
- **Operating hours:** 8,000 h/y (plant level)
- **Monetary base year:** 2020 euros

### Year: post-2035 (long-term future baseline)

- **Description:** Same technology; projected cost and efficiency improvements; still operates at 60–90 °C
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (baseline); min 45.47, max 52.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
- **CAPEX:** 527.2 €/kW (system baseline); min 306.5, max 774.5 €/kW
- **OPEX:** 48.8 % of system CAPEX (stack); min 46.5%, max 52.9%
- **Efficiency:** 48.9 kWh_el/kg_H2 (electric, baseline)
- **TRL / maturity:** Well-established commercial technology
- **Lifetime (stack):** 87,500 h (baseline); min 80,000, max 100,000 h
- **LCA / GHG context:** AEC most favorable electrolyzer in terms of cost per ton of CO2 avoided (CCA) for both near and long-term scenarios; near-future life-cycle GHG emissions averaged over all products/locations: 1,319 kg_CO2eq/t_product (17.1% higher than long-term future)
- **CCA performance:** Best option among the three electrolyzers for CCA in both scenarios; PEM–AEC differences reduce long-term

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: ~2035 (near future baseline)

- **Description:** Proton exchange membrane electrolysis; dominates market alongside AEC; available at MW-scale; operates at 60–90 °C
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (baseline); min 48.8, max 83.0 kWh_el/kg_H2
  - Water (implied by electrolysis process)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
- **CAPEX:** 1,047.9 €/kW (system baseline); min 613.0, max 1,225.9 €/kW
- **OPEX:** 42.0 % of system CAPEX (stack); min 28.6%, max 60.0%
- **Efficiency:** 57.9 kWh_el/kg_H2 (electric, baseline)
- **TRL / maturity:** Commercial / market-dominant; MW-scale available
- **Lifetime (stack):** 64,026 h (baseline); min 40,000, max 90,000 h
- **Energy source:** Renewable electricity exclusively
- **Operating hours:** 8,000 h/y (plant level)
- **Monetary base year:** 2020 euros

### Year: post-2035 (long-term future baseline)

- **Description:** Same technology; projected cost and efficiency improvements; still operates at 60–90 °C
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (baseline); min 47.0, max 64.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
- **CAPEX:** 473.5 €/kW (system baseline); min 257.9, max 700.5 €/kW
- **OPEX:** 39.4 % of system CAPEX (stack); min 27.8%, max 55.6%
- **Efficiency:** 53.1 kWh_el/kg_H2 (electric, baseline)
- **TRL / maturity:** Well-established commercial technology
- **Lifetime (stack):** 85,420 h (baseline); min 50,000, max 100,000 h
- **LCA / GHG context:** PEM–AEC CCA differences reduced in long-term future; electricity accounts on average for 144.5 kg_CO2eq/t_product more GHG emissions in near future vs long-term future

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: ~2035 (near future baseline)

- **Description:** Solid oxide electrolysis cells; operates at 700–850 °C; operates below thermoneutral conditions; high-temperature (HT) steam assumed from natural gas-fed boilers; not yet at same commercial maturity as AEC/PEM
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline); min 26.6, max 38.1 kWh_el/kg_H2
  - Thermal energy: 8.3 kWh_th/kg_H2 (baseline); min 6.7, max 11.0 kWh_th/kg_H2
  - HT steam: from natural gas-fed boilers (assumed)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
- **CAPEX:** 1,739.5 €/kW (system baseline); min 593.0, max 2,770.0 €/kW
- **OPEX:** 27.0 % of system CAPEX (stack); min 23.5%, max 30.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 (electric, baseline); total energy input also includes 8.3 kWh_th/kg_H2
- **TRL / maturity:** Pre-commercial / emerging; not listed as MW-scale market-dominant
- **Lifetime (stack):** 30,308 h (baseline); min 20,000, max 50,924 h
- **Energy source:** Renewable electricity; HT heat from natural gas
- **Operating hours:** 8,000 h/y (plant level)
- **Monetary base year:** 2020 euros
- **LCA / GHG note:** SOEC can result in more than double life-cycle GHG emissions vs AEC and PEM due to natural gas use for HT steam; for methane production with SOEC: ~41.6 kt_CH4/y (of 124.6 kt_CH4/y total) reused to reduce SOEC life-cycle GHG emissions; SOEC in Spain/Chile with olefins/aromatics yields CCA of €5,062–26,739/t_CO2eq,av in near future

### Year: post-2035 (long-term future baseline)

- **Description:** Same technology with projected CAPEX and lifetime improvements; energy requirements (electricity and thermal) assumed same as near future
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (same as near future)
  - Thermal energy: 8.3 kWh_th/kg_H2 (same as near future)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
- **CAPEX:** 958.3 €/kW (system baseline); min 566.5, max 1,723.3 €/kW
- **OPEX:** 13.8 % of system CAPEX (stack); min 12.5%, max 15.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 (electric, same as near future)
- **TRL / maturity:** Expected to become well-established by post-2035
- **Lifetime (stack):** 71,991 h (baseline); min 53,750, max 102,222 h
- **MSP benefit:** SOEC can reduce MSP by 42–218 €/t_product in The Netherlands; cost benefits in Chile only 3–11 €/t_product
- **LCA / GHG note:** SOEC much higher life-cycle GHG emissions if natural gas used for HT heat vs other electrolyzers; emissions vary significantly by location due to NG dependency for thermal input

---

# MODEL: claude-sonnet-4.6

- **Status:** OK  |  **Words:** 1179  |  **Duration:** 71922 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future, by 2035)
- **Description:** Alkaline water electrolysis; commercially available at MW-scale; dominates the market alongside PEM; operating temperature 60–90 °C; powered exclusively by renewable electricity
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (baseline); min 47.5, max 75.0 kWh_el/kg_H2
  - Water: not quantified separately in source
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
  - Oxygen: byproduct (not quantified)
- **CAPEX:** 790.5 €/kW (system, baseline) | min 437.8, max 1110.3 €/kW | **OPEX (stack):** 48.9% of system CAPEX (baseline); min 45.1%, max 54.0%
- **Efficiency:** 55.1 kWh_el/kg_H2 (baseline electric efficiency)
- **TRL / maturity:** Commercially available; dominates market; MW-scale deployment
- **Stack lifetime:** 75,532 h (baseline); min 60,000 h, max 94,444 h
- **Plant operation:** 8,000 h/y (except year 1 at 30%, year 2 at 70% of capacity)
- **Investment lifetime (MSP calc):** 25 years
- **Monetary basis:** 2020 euros
- **No CAPEX scaling factor applied** to electrolysis units
- **LCA / GHG:** AEC most favourable electrolyser in terms of cost per tonne of CO₂ avoided (CCA); lowest life-cycle GHG emissions among the three electrolyser types; near-future average life-cycle GHG emissions across all products and locations: 1,319 kg_CO₂eq/t_product (17.1% higher than long-term future); electricity accounts on average for 144.5 kg_CO₂eq/t_product more GHG in near future vs long-term future

### Year: post-2035 (long-term future, by 2050)
- **Description:** Alkaline water electrolysis at commercial scale using well-established technology; operating temperature 60–90 °C; exclusively renewable electricity input
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (baseline); min 45.47, max 52.0 kWh_el/kg_H2
  - Water: not quantified separately in source
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
  - Oxygen: byproduct (not quantified)
- **CAPEX:** 527.2 €/kW (system, baseline); min 306.5, max 774.5 €/kW | **OPEX (stack):** 48.8% of system CAPEX (baseline); min 46.5%, max 52.9%
- **Efficiency:** 48.9 kWh_el/kg_H2 (baseline electric efficiency)
- **TRL / maturity:** Well-established commercial technology
- **Stack lifetime:** 87,500 h (baseline); min 80,000 h, max 100,000 h
- **Plant operation:** 8,000 h/y
- **Investment lifetime (MSP calc):** 25 years
- **Monetary basis:** 2020 euros
- **LCA / GHG:** AEC remains most favourable electrolyser for CCA and life-cycle GHG; PEM–AEC differences in CCA reduced in long-term vs near future; near-future-to-long-term MSP decline averages 54.2% across all products; projected annual MSP decrease ~5.1% over the 15-year horizon
- **MSP impact (product-level, LT DAC + AEC, Iceland):** renewable electricity cost decrease plus electrolyser efficiency improvements together account for ~20% of total MSP reduction potential

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future, by 2035)
- **Description:** Proton exchange membrane electrolysis; commercially available at MW-scale; dominates market alongside AEC; operating temperature 60–90 °C; powered exclusively by renewable electricity
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (baseline); min 48.8, max 83.0 kWh_el/kg_H2
  - Water: not quantified separately in source
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
  - Oxygen: byproduct (not quantified)
- **CAPEX:** 1,047.9 €/kW (system, baseline); min 613.0, max 1,225.9 €/kW | **OPEX (stack):** 42.0% of system CAPEX (baseline); min 28.6%, max 60.0%
- **Efficiency:** 57.9 kWh_el/kg_H2 (baseline electric efficiency)
- **TRL / maturity:** Commercially available; dominates market; MW-scale deployment
- **Stack lifetime:** 64,026 h (baseline); min 40,000 h, max 90,000 h
- **Plant operation:** 8,000 h/y (except year 1 at 30%, year 2 at 70% of capacity)
- **Investment lifetime (MSP calc):** 25 years
- **Monetary basis:** 2020 euros
- **No CAPEX scaling factor applied** to electrolysis units
- **LCA / GHG:** PEM–AEC differences in CCA noted but reduced in long-term future; AEC slightly more favourable than PEM on CCA metric in near future

### Year: post-2035 (long-term future, by 2050)
- **Description:** Proton exchange membrane electrolysis at commercial scale; well-established technology; operating temperature 60–90 °C; exclusively renewable electricity input
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (baseline); min 47.0, max 64.0 kWh_el/kg_H2
  - Water: not quantified separately in source
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
  - Oxygen: byproduct (not quantified)
- **CAPEX:** 473.5 €/kW (system, baseline); min 257.9, max 700.5 €/kW | **OPEX (stack):** 39.4% of system CAPEX (baseline); min 27.8%, max 55.6%
- **Efficiency:** 53.1 kWh_el/kg_H2 (baseline electric efficiency)
- **TRL / maturity:** Well-established commercial technology
- **Stack lifetime:** 85,420 h (baseline); min 50,000 h, max 100,000 h
- **Plant operation:** 8,000 h/y
- **Investment lifetime (MSP calc):** 25 years
- **Monetary basis:** 2020 euros
- **LCA / GHG:** PEM–AEC CCA differences reduced long-term; both perform substantially better than SOEC (when SOEC uses natural gas for steam)

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future, by 2035)
- **Description:** Solid oxide electrolysis; high-temperature operation at 700–850 °C; operates below thermoneutral conditions; requires high-temperature (HT) steam assumed from natural gas (NG)-fed boilers; less commercially mature than AEC and PEM; powered by renewable electricity plus NG-sourced heat
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline); min 26.6, max 38.1 kWh_el/kg_H2
  - Thermal energy (HT steam from NG): 8.3 kWh_th/kg_H2 (baseline); min 6.7, max 11.0 kWh_th/kg_H2
  - Water (as steam): not quantified separately in source
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
  - Oxygen: byproduct (not quantified)
- **CAPEX:** 1,739.5 €/kW (system, baseline); min 593.0, max 2,770.0 €/kW | **OPEX (stack):** 27.0% of system CAPEX (baseline); min 23.5%, max 30.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 electric (baseline); total energy input = electric + thermal
- **TRL / maturity:** Less commercially mature than AEC and PEM; not yet at MW-scale dominance; high CAPEX spread (min–max ratio ~4.7×) reflects uncertainty
- **Stack lifetime:** 30,308 h (baseline); min 20,000 h, max 50,924 h
- **Plant operation:** 8,000 h/y (except year 1 at 30%, year 2 at 70% of capacity)
- **Investment lifetime (MSP calc):** 25 years
- **Monetary basis:** 2020 euros
- **No CAPEX scaling factor applied** to electrolysis units
- **LCA / GHG:** SOEC can result in more than double life-cycle GHG emissions vs AEC and PEM when natural gas supplies the HT steam; for methane production with SOEC, ~1/3 of produced CCU methane (41.6 kt_CH4/y from 124.6 kt_CH4/y capacity, LHV 49.9 MJ/kg_CH4) reused internally to reduce SOEC life-cycle GHG impact; SOEC gives ~6× higher GHG for DME in Chile vs AEC in The Netherlands
- **MSP impact:** SOEC can reduce product MSP by 42–218 €/t_product in The Netherlands (low-carbon grid); benefit reduced to 3–11 €/t_product in Chile (high-solar PV share)

### Year: post-2035 (long-term future, by 2050)
- **Description:** Solid oxide electrolysis; high-temperature operation at 700–850 °C; below thermoneutral conditions; HT steam from NG; well-established technology post-2035; energy requirements assumed unchanged from near future
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline, same as near future); min 26.6, max 38.1 kWh_el/kg_H2
  - Thermal energy (HT steam from NG): 8.3 kWh_th/kg_H2 (baseline, same as near future); min 6.7, max 11.0 kWh_th/kg_H2
  - Water (as steam): not quantified separately in source
- **Outputs:**
  - Hydrogen: 1 kg_H2 (reference unit)
  - Oxygen: byproduct (not quantified)
- **CAPEX:** 958.3 €/kW (system, baseline); min 566.5, max 1,723.3 €/kW | **OPEX (stack):** 13.8% of system CAPEX (baseline); min 12.5%, max 15.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 electric (baseline, same as near future); thermal 8.3 kWh_th/kg_H2 (same as near future)
- **TRL / maturity:** Well-established technology (post-2035 scenario assumes commercial-scale operation)
- **Stack lifetime:** 71,991 h (baseline); min 53,750 h, max 102,222 h
- **Plant operation:** 8,000 h/y
- **Investment lifetime (MSP calc):** 25 years
- **Monetary basis:** 2020 euros
- **LCA / GHG:** Use of natural gas for HT steam remains primary driver of elevated life-cycle GHG relative to AEC/PEM; SOEC hydrogen production has much higher life-cycle GHG if natural gas is used; for methane production, internal reuse of ~1/3 of CCU methane output partially offsets NG-derived emissions; SOEC GHG disadvantage especially pronounced in high-solar locations (Spain, Chile) vs wind/geothermal locations

---

# MODEL: claude-sonnet-4.5

- **Status:** OK  |  **Words:** 783  |  **Duration:** 54277 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)

- **Description:** Alkaline water electrolysis operating at 60–90 °C. Dominates market and available at MW-scale. Uses exclusively renewable energy.
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (baseline); range 47.5–75.0 kWh_el/kg_H2
  - Water
- **Outputs:**
  - Hydrogen: 1 kg_H2 per process cycle
  - Oxygen (byproduct)
- **CAPEX:** 790.5 €/kW (baseline); range 437.8–1110.3 €/kW (system cost) | **OPEX:** 48.9% of system CAPEX (stack replacement); range 45.1–54.0%
- **Efficiency:** 55.1 kWh_el/kg_H2 (baseline electric consumption); range 47.5–75.0 kWh_el/kg_H2
- **Stack lifetime:** 75,532 hours (baseline); range 60,000–94,444 hours
- **TRL / maturity:** Commercial technology; dominates market with PEM; available at MW-scale
- **Operating hours:** 8,000 h/year assumed for plant operation
- **No scaling factor applied to electrolysis units** (CAPEX scaling)

### Year: post-2035 (long-term future)

- **Description:** Well-established alkaline water electrolysis technology at commercial scale, operating at 60–90 °C.
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (baseline); range 45.47–52.0 kWh_el/kg_H2
  - Water
- **Outputs:**
  - Hydrogen: 1 kg_H2 per process cycle
  - Oxygen (byproduct)
- **CAPEX:** 527.2 €/kW (baseline); range 306.5–774.5 €/kW (system cost) | **OPEX:** 48.8% of system CAPEX (stack replacement); range 46.5–52.9%
- **Efficiency:** 48.9 kWh_el/kg_H2 (baseline electric consumption); range 45.47–52.0 kWh_el/kg_H2
- **Stack lifetime:** 87,500 hours (baseline); range 80,000–100,000 hours
- **GHG emissions performance:** AEC most favorable electrolyzer option in terms of cost per ton CO2 avoided for both near and long-term scenarios
- **Economic impact:** Choice of electrolyzer has minor economic impacts compared to location and electricity source

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)

- **Description:** Proton exchange membrane electrolysis operating at 60–90 °C. Dominates market and available at MW-scale. Uses exclusively renewable energy.
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (baseline); range 48.8–83.0 kWh_el/kg_H2
  - Water
- **Outputs:**
  - Hydrogen: 1 kg_H2 per process cycle
  - Oxygen (byproduct)
- **CAPEX:** 1,047.9 €/kW (baseline); range 613.0–1,225.9 €/kW (system cost) | **OPEX:** 42.0% of system CAPEX (stack replacement); range 28.6–60.0%
- **Efficiency:** 57.9 kWh_el/kg_H2 (baseline electric consumption); range 48.8–83.0 kWh_el/kg_H2
- **Stack lifetime:** 64,026 hours (baseline); range 40,000–90,000 hours
- **TRL / maturity:** Commercial technology; dominates market with AEC; available at MW-scale
- **Operating hours:** 8,000 h/year assumed for plant operation
- **No scaling factor applied to electrolysis units** (CAPEX scaling)

### Year: post-2035 (long-term future)

- **Description:** Well-established proton exchange membrane electrolysis technology at commercial scale, operating at 60–90 °C.
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (baseline); range 47.0–64.0 kWh_el/kg_H2
  - Water
- **Outputs:**
  - Hydrogen: 1 kg_H2 per process cycle
  - Oxygen (byproduct)
- **CAPEX:** 473.5 €/kW (baseline); range 257.9–700.5 €/kW (system cost) | **OPEX:** 39.4% of system CAPEX (stack replacement); range 27.8–55.6%
- **Efficiency:** 53.1 kWh_el/kg_H2 (baseline electric consumption); range 47.0–64.0 kWh_el/kg_H2
- **Stack lifetime:** 85,420 hours (baseline); range 50,000–100,000 hours
- **Economic performance:** PEM–AEC cost differences reduced in long-term future
- **GHG emissions performance:** Similar to AEC; much lower life cycle GHG emissions than SOEC when natural gas heat is used

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)

- **Description:** Solid oxide electrolyzer cells operating at 700–850 °C, below thermoneutral conditions. High-temperature steam assumed from natural gas-fed boilers.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2
  - Thermal energy: 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2
  - High-temperature steam (>100 °C) from natural gas
  - Water
- **Outputs:**
  - Hydrogen: 1 kg_H2 per process cycle
  - Oxygen (byproduct)
- **CAPEX:** 1,739.5 €/kW (baseline); range 593.0–2,770.0 €/kW (system cost) | **OPEX:** 27.0% of system CAPEX (stack replacement); range 23.5–30.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 electrical consumption (baseline); range 26.6–38.1 kWh_el/kg_H2
- **Stack lifetime:** 30,308 hours (baseline); range 20,000–50,924 hours
- **Operating temperature:** 700–850 °C
- **Operating hours:** 8,000 h/year assumed for plant operation
- **No scaling factor applied to electrolysis units** (CAPEX scaling)
- **GHG emissions:** Can result in more than double life cycle GHG emissions vs AEC and PEM due to natural gas use for high-temperature heat

### Year: post-2035 (long-term future)

- **Description:** Well-established solid oxide electrolyzer technology at commercial scale, operating at 700–850 °C, below thermoneutral conditions. High-temperature steam from natural gas.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2 (same as near future)
  - Thermal energy: 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2 (same as near future)
  - High-temperature steam (>100 °C) from natural gas
  - Water
- **Outputs:**
  - Hydrogen: 1 kg_H2 per process cycle
  - Oxygen (byproduct)
- **CAPEX:** 958.3 €/kW (baseline); range 566.5–1,723.3 €/kW (system cost) | **OPEX:** 13.8% of system CAPEX (stack replacement); range 12.5–15.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 electrical consumption (baseline); range 26.6–38.1 kWh_el/kg_H2 (same as near future)
- **Stack lifetime:** 71,991 hours (baseline); range 53,750–102,222 hours
- **Operating temperature:** 700–850 °C
- **Economic performance in Chile:** SOEC cost benefits 3–11 €/t_product lower MSP
- **Economic performance in The Netherlands:** SOEC can reduce MSP by 42–218 €/t_product
- **GHG emissions:** SOEC hydrogen production has much higher life cycle GHG emissions if natural gas is used compared to AEC and PEM
- **Methane self-consumption example:** For methane production with SOEC at 124.6 kt_CH4/y capacity, around one-third (41.6 kt_CH4) of produced CCU methane would need to be reused to reduce SOEC life cycle GHG emissions (assuming 90% methane boiler efficiency, LHV 49.9 MJ/kg_CH4)

---

# MODEL: claude-haiku-4.5

- **Status:** OK  |  **Words:** 606  |  **Duration:** 19694 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future baseline)
- **Description:** Electrochemical process for hydrogen production; operates at 60−90 °C; uses renewable electricity exclusively; dominates market and available at MW-scale
- **Inputs:** electricity: 55.1 kWh_el/kg_H2; water (implied, for electrolysis)
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 790.5 €/kW (system) | **OPEX:** 48.9 % of system CAPEX (stack)
- **Efficiency:** 55.1 kWh_el/kg_H2
- **Stack lifetime:** 75,532 h (range: 60,000–94,444 h)
- **TRL / maturity:** Commercially available and market-dominant at MW-scale

### Year: 2050 (long-term future baseline)
- **Description:** Same electrochemical process; operating conditions 60−90 °C; fully established commercial technology
- **Inputs:** electricity: 48.9 kWh_el/kg_H2
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 527.2 €/kW (system) | **OPEX:** 48.8 % of system CAPEX (stack)
- **Efficiency:** 48.9 kWh_el/kg_H2 (improved from 2035)
- **Stack lifetime:** 87,500 h (range: 80,000–100,000 h)
- **Investment allocation:** 30% two years before production, 50% one year before, 20% in first year; 25-year lifetime
- **Operating hours:** 8,000 h/year (8,000 h/year average capacity utilization)
- **Cost per ton CO₂ avoided:** 150–537 €/t_CO2eq,av (varies by product group and scenario; AEC best option for both scenarios)

---

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future baseline)
- **Description:** Electrochemical hydrogen production via ion exchange membrane; operates at 60−90 °C; commercially available technology; market-competitive with AEC
- **Inputs:** electricity: 57.9 kWh_el/kg_H2; water (implied); proton-exchange membrane (consumable)
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 1,047.9 €/kW (system) | **OPEX:** 42.0 % of system CAPEX (stack)
- **Efficiency:** 57.9 kWh_el/kg_H2
- **Stack lifetime:** 64,026 h (range: 40,000–90,000 h)
- **TRL / maturity:** Commercially available; market-dominant alongside AEC

### Year: 2050 (long-term future baseline)
- **Description:** Established PEM technology; 60−90 °C operation; improved efficiency and reliability
- **Inputs:** electricity: 53.1 kWh_el/kg_H2
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 473.5 €/kW (system) | **OPEX:** 39.4 % of system CAPEX (stack)
- **Efficiency:** 53.1 kWh_el/kg_H2 (improved from 2035)
- **Stack lifetime:** 85,420 h (range: 50,000–100,000 h)
- **Investment allocation:** 30% two years before production, 50% one year before, 20% in first year; 25-year lifetime
- **Operating hours:** 8,000 h/year
- **Cost per ton CO₂ avoided:** 225–537 €/t_CO2eq,av (AEC–PEM differences reduced in long-term; PEM favorability increases over time)

---

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future baseline)
- **Description:** High-temperature electrochemical hydrogen production; operates at 700−850 °C; operates below thermoneutral conditions; requires high-temperature steam input; emerging technology; much higher life cycle GHG emissions if natural gas used for heat
- **Inputs:** electricity: 34.1 kWh_el/kg_H2; thermal energy: 8.3 kWh_th/kg_H2 (high-temperature steam from natural gas boilers assumed); water vapor (implied)
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 1,739.5 €/kW (system) | **OPEX:** 27.0 % of system CAPEX (stack)
- **Efficiency:** 34.1 kWh_el/kg_H2 (electric); 8.3 kWh_th/kg_H2 (thermal)
- **Stack lifetime:** 30,308 h (range: 20,000–50,924 h) — significantly shorter than AEC and PEM
- **TRL / maturity:** Emerging technology; lower technology readiness than AEC and PEM
- **Climate impact note:** Can result in more than double life cycle GHG emissions versus AEC and PEM if natural gas used

### Year: 2050 (long-term future baseline)
- **Description:** Advanced SOEC with improved durability; 700−850 °C operation; thermal energy from various sources; high-temperature process established
- **Inputs:** electricity: 34.1 kWh_el/kg_H2 (same as 2035); thermal energy: 8.3 kWh_th/kg_H2 (same as 2035)
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 958.3 €/kW (system) | **OPEX:** 13.8 % of system CAPEX (stack) — significantly reduced from 2035
- **Efficiency:** 34.1 kWh_el/kg_H2 (electric); 8.3 kWh_th/kg_H2 (thermal) — energy requirements assumed same as near future
- **Stack lifetime:** 71,991 h (range: 53,750–102,222 h) — dramatically improved from 2035
- **Investment allocation:** 30% two years before production, 50% one year before, 20% in first year; 25-year lifetime
- **Operating hours:** 8,000 h/year
- **Cost per ton CO₂ avoided:** 225–537 €/t_CO2eq,av (range comparable to other electrolyzers; however, much higher if fossil natural gas used for thermal energy)
- **Economic outlook:** SOEC can reduce MSP by 42–218 €/t_product in The Netherlands and 3–11 €/t_product in Chile; limited cost benefit compared to AEC/PEM; choice of electrolyzer has minor economic impact versus location and electricity source

---

# MODEL: claude-fable-5

- **Status:** OK  |  **Words:** 883  |  **Duration:** 42953 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future, "by 2035")
- **Description:** Alkaline water electrolysis (AEC); operates at 60−90 °C; hydrogen produced exclusively using renewable energy; AEC and PEM dominate the market and are available at MW-scale.
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (baseline; min 47.5, max 75.0)
  - Water (feedstock, quantity not specified)
- **Outputs:**
  - Hydrogen: 1 kg_H2 per 55.1 kWh_el (baseline)
- **CAPEX:** 790.5 €/kW (system; min 437.8, max 1110.3) | **OPEX:** stack replacement 48.9% of system CAPEX (min 45.1, max 54.0)
- **Efficiency:** electric efficiency 55.1 kWh_el/kg_H2 (min 47.5, max 75.0)
- **TRL / maturity:** commercially available at MW-scale; market-dominating technology (with PEM); near-future values reflect efficiencies expected at commercial scale
- **Lifetime (stack):** 75,532 h (min 60,000, max 94,444)
- **Other:** no CAPEX scaling factor applied to H2 electrolysis; monetary values in 2020 euros; plant operation 8000 h/y (30%/70% capacity in years 1/2); investment lifetime 25 y; discount rate 6% baseline (sensitivity 5–8%); values inter-/extrapolated to 2035 where needed
- **Locations:** Iceland (geothermal, 25.6 €/MWh_el), The Netherlands (offshore wind, 63.9 €/MWh_el), Spain (PV, 34.7 €/MWh_el), Chile (PV, 26.4 €/MWh_el)
- **LCA:** AEC most favorable electrolyzer for life-cycle GHG emissions; near-future life-cycle GHG emissions of products averaged 17.1% higher (1319 kg_CO2eq/t_product) than long-term future; AEC best option in cost of CO2 avoided for both scenarios; Iceland lowest near-future CCA

### Year: post-2035 (long-term future, by 2050)
- **Description:** Same process; well-established technology after 2035.
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (baseline; min 45.47, max 52.0)
  - Water (feedstock, quantity not specified)
- **Outputs:**
  - Hydrogen: 1 kg_H2 per 48.9 kWh_el (baseline)
- **CAPEX:** 527.2 €/kW (system; min 306.5, max 774.5) | **OPEX:** stack 48.8% of system (min 46.5, max 52.9)
- **Efficiency:** 48.9 kWh_el/kg_H2 (min 45.47, max 52.0)
- **Lifetime (stack):** 87,500 h (min 80,000, max 100,000)
- **Other:** long-term electricity costs: offshore wind 40.0 €/MWh_el, PV Spain 18.1 €/MWh_el, PV Chile 13.7 €/MWh_el; MSPs of products decline 54.2% on average from near to long-term future (5.1%/y over 15 years); renewable electricity cost decrease plus electrolyzer efficiency improvements account for ~1/5 of total MSP reduction potential
- **LCA:** AEC most favorable; products with AEC in Chile: DME life-cycle GHG 3.6× higher than in The Netherlands (electricity mix effect)

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future, "by 2035")
- **Description:** Proton exchange membrane (PEM) electrolysis; operates at 60−90 °C; renewable electricity only; available at MW-scale, market-dominating with AEC.
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (baseline; min 48.8, max 83.0)
  - Water (feedstock, quantity not specified)
- **Outputs:**
  - Hydrogen: 1 kg_H2 per 57.9 kWh_el (baseline)
- **CAPEX:** 1047.9 €/kW (system; min 613.0, max 1225.9) | **OPEX:** stack 42.0% of system (min 28.6, max 60.0)
- **Efficiency:** 57.9 kWh_el/kg_H2 (min 48.8, max 83.0)
- **TRL / maturity:** commercially available at MW-scale
- **Lifetime (stack):** 64,026 h (min 40,000, max 90,000)
- **Other:** no CAPEX scaling applied; 2020 euros; 8000 operating h/y; 25-y investment lifetime; values inter-/extrapolated to 2035
- **Locations:** Iceland, The Netherlands, Spain, Chile (energy costs/emissions as in Table 3)

### Year: post-2035 (long-term future, by 2050)
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (baseline; min 47.0, max 64.0)
  - Water (feedstock, quantity not specified)
- **Outputs:**
  - Hydrogen: 1 kg_H2 per 53.1 kWh_el (baseline)
- **CAPEX:** 473.5 €/kW (system; min 257.9, max 700.5) | **OPEX:** stack 39.4% of system (min 27.8, max 55.6)
- **Efficiency:** 53.1 kWh_el/kg_H2 (min 47.0, max 64.0)
- **Lifetime (stack):** 85,420 h (min 50,000, max 100,000)
- **LCA:** PEM–AEC differences in cost of CO2 avoided reduced in long-term future

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future, "by 2035")
- **Description:** Solid oxide electrolyzer cell (SOEC); operates at 700−850 °C; operated below thermoneutral conditions; high-temperature steam assumed supplied from natural gas (HT heat >100 °C from NG-fed boilers).
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline; min 26.6, max 38.1)
  - Thermal energy (HT steam from NG): 8.3 kWh_th/kg_H2 (baseline; min 6.7, max 11.0)
  - Water/steam (feedstock, quantity not specified)
- **Outputs:**
  - Hydrogen: 1 kg_H2 per 34.1 kWh_el + 8.3 kWh_th (baseline)
- **CAPEX:** 1739.5 €/kW (system; min 593.0, max 2770.0) | **OPEX:** stack 27.0% of system (min 23.5, max 30.0)
- **Efficiency:** electric 34.1 kWh_el/kg_H2 (min 26.6, max 38.1); thermal 8.3 kWh_th/kg_H2 (min 6.7, max 11.0)
- **TRL / maturity:** less mature than MW-scale market technologies (not stated as MW-scale market-dominant)
- **Lifetime (stack):** 30,308 h (min 20,000, max 50,924)
- **Other:** natural gas cost 25.0 €/MWh_th (min 15.0, max 40.0); NG emissions 232.5 kg_CO2eq/MWh_th; 2020 euros; 8000 operating h/y; 25-y investment lifetime
- **LCA:** hydrogen production with SOEC has much higher life-cycle GHG emissions than other electrolyzers if natural gas is used; near-future olefins/aromatics in Spain/Chile with SOEC: cost of CO2 avoided €5062–26,739/t_CO2eq,av

### Year: post-2035 (long-term future, by 2050)
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (same as near future; min 26.6, max 38.1)
  - Thermal energy: 8.3 kWh_th/kg_H2 (same as near future; min 6.7, max 11.0)
  - Water/steam (feedstock, quantity not specified)
- **Outputs:**
  - Hydrogen: 1 kg_H2 per 34.1 kWh_el + 8.3 kWh_th
- **CAPEX:** 958.3 €/kW (system; min 566.5, max 1723.3) | **OPEX:** stack 13.8% of system (min 12.5, max 15.0)
- **Efficiency:** same as near future (energy requirements assumed unchanged): 34.1 kWh_el/kg_H2; 8.3 kWh_th/kg_H2
- **Lifetime (stack):** 71,991 h (min 53,750, max 102,222)
- **Other:** SOEC can reduce product MSP by 42–218 €/t_product in The Netherlands; cost benefits in Chile 3–11 €/t_product
- **LCA:** SOEC can result in more than double life-cycle GHG emissions vs AEC and PEM; for methane production with SOEC (90% boiler efficiency, 124.6 kt_CH4/y capacity, LHV 49.9 MJ/kg_CH4), about a third (41.6 kt_CH4) of produced CCU methane could be reused to reduce SOEC life-cycle GHG emissions

---

# MODEL: claude-opus-4.8

- **Status:** OK  |  **Words:** 688  |  **Duration:** 35794 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: Near future (by 2035)
- **Description:** Alkaline water electrolysis; renewable-electricity-only hydrogen production. Operating temperature 60–90 °C. Dominates market alongside PEM; available at MW-scale. No CAPEX scaling factor applied to H2 electrolysis. Reference: system sized to plant with fixed input of 360 ktCO2/y; plant operation 8000 h/y (first/second year at 30%/70% capacity); MSP investment lifetime 25 years.
- **Inputs:**
  - Electricity (renewable): 55.1 kWh_el/kg_H2 (min 47.5, max 75.0)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (basis)
- **CAPEX:** 790.5 €/kW system (min 437.8, max 1110.3) | **OPEX:** stack 48.9 % of system (min 45.1, max 54.0)
- **Efficiency:** 55.1 kWh_el/kg_H2 (min 47.5, max 75.0)
- **TRL / maturity:** Commercially dominant, available at MW-scale
- **Lifetime (stack):** 75532 h (min 60000, max 94444)
- **Location/energy source:** Iceland (geothermal), Netherlands (offshore wind), Spain/Chile (PV); renewable only

### Year: Long-term future (post-2035, by 2050)
- **Description:** Same technology, well-established at commercial scale; 60–90 °C operation; renewable electricity only.
- **Inputs:**
  - Electricity (renewable): 48.9 kWh_el/kg_H2 (min 45.47, max 52.0)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (basis)
- **CAPEX:** 527.2 €/kW system (min 306.5, max 774.5) | **OPEX:** stack 48.8 % of system (min 46.5, max 52.9)
- **Efficiency:** 48.9 kWh_el/kg_H2 (min 45.47, max 52.0)
- **Lifetime (stack):** 87500 h (min 80000, max 100000)
- **Environmental note:** AEC most favorable electrolyzer for life-cycle GHG emissions and cost per ton CO2 avoided in both scenarios

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: Near future (by 2035)
- **Description:** Proton exchange membrane electrolysis; renewable-electricity-only hydrogen. Operating temperature 60–90 °C. Dominates market with AEC; available at MW-scale. No CAPEX scaling applied to electrolysis. Same plant basis (360 ktCO2/y input, 8000 h/y, 25-year investment lifetime).
- **Inputs:**
  - Electricity (renewable): 57.9 kWh_el/kg_H2 (min 48.8, max 83.0)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (basis)
- **CAPEX:** 1047.9 €/kW system (min 613.0, max 1225.9) | **OPEX:** stack 42.0 % of system (min 28.6, max 60.0)
- **Efficiency:** 57.9 kWh_el/kg_H2 (min 48.8, max 83.0)
- **TRL / maturity:** Commercially available at MW-scale
- **Lifetime (stack):** 64026 h (min 40000, max 90000)
- **Location/energy source:** Renewable electricity (geothermal/offshore wind/PV per location)

### Year: Long-term future (post-2035, by 2050)
- **Description:** Well-established commercial PEM; 60–90 °C; renewable electricity only.
- **Inputs:**
  - Electricity (renewable): 53.1 kWh_el/kg_H2 (min 47.0, max 64.0)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (basis)
- **CAPEX:** 473.5 €/kW system (min 257.9, max 700.5) | **OPEX:** stack 39.4 % of system (min 27.8, max 55.6)
- **Efficiency:** 53.1 kWh_el/kg_H2 (min 47.0, max 64.0)
- **Lifetime (stack):** 85420 h (min 50000, max 100000)
- **Environmental note:** PEM–AEC cost/GHG differences reduce in long-term future

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: Near future (by 2035)
- **Description:** Solid oxide electrolysis cell; high-temperature operation 700–850 °C, run below thermoneutral conditions. HT steam assumed supplied from natural gas (NG). Requires both electric and thermal energy input. Renewable electricity for electric load. Same plant basis (360 ktCO2/y input, 8000 h/y, 25-year lifetime).
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (min 26.6, max 38.1)
  - Thermal energy (HT steam, from NG): 8.3 kWh_th/kg_H2 (min 6.7, max 11.0)
- **Outputs:**
  - Hydrogen: 1 kg_H2 (basis)
- **CAPEX:** 1739.5 €/kW system (min 593.0, max 2770.0) | **OPEX:** stack 27.0 % of system (min 23.5, max 30.0)
- **Efficiency (electric):** 34.1 kWh_el/kg_H2 (min 26.6, max 38.1); thermal 8.3 kWh_th/kg_H2 (min 6.7, max 11.0)
- **TRL / maturity:** Less mature than AEC/PEM; not noted as market-dominant
- **Lifetime (stack):** 30308 h (min 20000, max 50924)
- **Environmental note:** Much higher life-cycle GHG emissions if NG used for HT steam; can be >2× GHG of AEC/PEM. For methane via SOEC, ~1/3 (41.6 of 124.6 kt_CH4/y) of produced CCU methane reused (boiler 90% eff., LHV 49.9 MJ/kg_CH4) to reduce SOEC life-cycle GHG.

### Year: Long-term future (post-2035, by 2050)
- **Description:** Well-established SOEC; 700–850 °C, below thermoneutral; HT steam from NG; energy requirements assumed same as near future.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (min 26.6, max 38.1) — same as near future
  - Thermal energy (HT steam, from NG): 8.3 kWh_th/kg_H2 (min 6.7, max 11.0) — same as near future
- **Outputs:**
  - Hydrogen: 1 kg_H2 (basis)
- **CAPEX:** 958.3 €/kW system (min 566.5, max 1723.3) | **OPEX:** stack 13.8 % of system (min 12.5, max 15.0)
- **Efficiency (electric):** 34.1 kWh_el/kg_H2 (same as near future); thermal 8.3 kWh_th/kg_H2 (same as near future)
- **Lifetime (stack):** 71991 h (min 53750, max 102222)
- **Cost impact:** Can reduce product MSP by 42–218 €/t_product in The Netherlands; 3–11 €/t_product in Chile

---

# MODEL: claude-opus-4.7

- **Status:** OK  |  **Words:** 744  |  **Duration:** 26111 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** Alkaline water electrolysis for hydrogen production; operates at 60–90 °C; MW-scale; powered exclusively by renewable electricity.
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (min 47.5, max 75.0)
  - Water (implicit feedstock)
- **Outputs:**
  - Hydrogen: 1 kg H2 per 55.1 kWh_el input
- **CAPEX:** 790.5 €/kW (min 437.8, max 1110.3) — system
- **OPEX:** Stack replacement 48.9% of system cost (min 45.1, max 54.0)
- **Efficiency:** 55.1 kWh_el/kg_H2 (baseline)
- **TRL / maturity:** Commercial; dominates market together with PEM; available at MW-scale
- **Lifetime (stack):** 75,532 h (min 60,000, max 94,444)
- **Reference capacity / scale:** MW-scale; plant sized to 360 kt_CO2/y input; 8000 operating h/y
- **Location:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental:** GHG emissions of H2 depend on electricity source; AEC most favorable electrolyzer for GHG emissions in near future; lower emissions than SOEC when NG used for steam
- **Other:** Monetary values in 2020 €; investment lifetime 25 years; debt/equity 1.5; no CAPEX scaling applied to electrolysis

### Year: post-2035 (long-term future)
- **Description:** Same as near-future; commercial alkaline electrolysis, 60–90 °C, renewable power.
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (min 45.47, max 52.0)
- **Outputs:**
  - Hydrogen: 1 kg H2 per 48.9 kWh_el
- **CAPEX:** 527.2 €/kW (min 306.5, max 774.5)
- **OPEX:** Stack 48.8% of system (min 46.5, max 52.9)
- **Efficiency:** 48.9 kWh_el/kg_H2
- **TRL / maturity:** Well-established, post-2035
- **Lifetime (stack):** 87,500 h (min 80,000, max 100,000)
- **LCA / environmental:** Best electrolyzer choice for CCA in both scenarios; lower life-cycle GHG than SOEC when SOEC uses NG heat
- **Other:** Choice of electrolyzer has minor economic impact vs location/electricity source

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** PEM water electrolysis; operates at 60–90 °C; MW-scale; renewable electricity only.
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (min 48.8, max 83.0)
  - Water
- **Outputs:**
  - Hydrogen: 1 kg H2 per 57.9 kWh_el
- **CAPEX:** 1047.9 €/kW (min 613.0, max 1225.9)
- **OPEX:** Stack 42.0% of system (min 28.6, max 60.0)
- **Efficiency:** 57.9 kWh_el/kg_H2
- **TRL / maturity:** Commercial; dominates market with AEC; MW-scale available
- **Lifetime (stack):** 64,026 h (min 40,000, max 90,000)
- **Reference capacity / scale:** MW-scale; 8000 h/y operation
- **Location:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental:** Similar to AEC; higher electric consumption than AEC yields slightly higher GHG per kg H2 for same electricity source

### Year: post-2035 (long-term future)
- **Description:** Same technology, mature commercial deployment.
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (min 47.0, max 64.0)
- **Outputs:**
  - Hydrogen: 1 kg H2 per 53.1 kWh_el
- **CAPEX:** 473.5 €/kW (min 257.9, max 700.5)
- **OPEX:** Stack 39.4% of system (min 27.8, max 55.6)
- **Efficiency:** 53.1 kWh_el/kg_H2
- **Lifetime (stack):** 85,420 h (min 50,000, max 100,000)
- **LCA / environmental:** PEM–AEC differences reduced long-term

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** Solid oxide electrolyzer cell; high-temperature operation 700–850 °C; operates below thermoneutral conditions; high-temperature steam assumed supplied from natural gas.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (min 26.6, max 38.1)
  - Thermal energy (HT steam, from NG): 8.3 kWh_th/kg_H2 (min 6.7, max 11.0)
  - Water/steam feedstock
- **Outputs:**
  - Hydrogen: 1 kg H2 per 34.1 kWh_el + 8.3 kWh_th
- **CAPEX:** 1739.5 €/kW (min 593.0, max 2770.0)
- **OPEX:** Stack 27.0% of system (min 23.5, max 30.0)
- **Efficiency:** 34.1 kWh_el/kg_H2 (electric) + 8.3 kWh_th/kg_H2 (thermal)
- **TRL / maturity:** Less mature than AEC/PEM; not yet dominating market
- **Lifetime (stack):** 30,308 h (min 20,000, max 50,924)
- **Reference capacity / scale:** 8000 h/y operation; plant scaled to 360 kt_CO2/y input
- **Location:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental:** With NG-fed steam, SOEC has much higher life-cycle GHG than AEC/PEM; can more than double life-cycle GHG vs AEC/PEM; e.g., DME in Chile with AEC has 3.6× lower GHG than SOEC-based route; for methane production (124.6 kt_CH4/y, LHV 49.9 MJ/kg, 90% boiler efficiency), ~1/3 (41.6 kt_CH4/y) of CCU methane reused to lower SOEC GHG
- **Other:** SOEC economic benefit: reduces MSP by 42–218 €/t_product in NL; only 3–11 €/t_product in Chile; can incur very high CCAs (olefins/aromatics in Spain/Chile with SOEC: 5062–26739 €/t_CO2eq,av)

### Year: post-2035 (long-term future)
- **Description:** Same HT SOEC (700–850 °C), operation below thermoneutral, HT steam from NG.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (same as near future; min 26.6, max 38.1)
  - Thermal: 8.3 kWh_th/kg_H2 (same as near future; min 6.7, max 11.0)
- **Outputs:**
  - Hydrogen: 1 kg H2 per 34.1 kWh_el + 8.3 kWh_th
- **CAPEX:** 958.3 €/kW (min 566.5, max 1723.3)
- **OPEX:** Stack 13.8% of system (min 12.5, max 15.0)
- **Efficiency:** unchanged vs near future (34.1 kWh_el + 8.3 kWh_th per kg H2)
- **Lifetime (stack):** 71,991 h (min 53,750, max 102,222)
- **LCA / environmental:** GHG penalty from NG-derived HT steam persists; higher LCA emissions than AEC/PEM when NG used

---

# MODEL: gpt-5.5

- **Status:** OK  |  **Words:** 1347  |  **Duration:** 55263 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** AEC; water electrolysis; hydrogen produced exclusively using renewable energy; available at MW-scale; operates at 60-90 °C.
- **Inputs:**
  - Water: feedstock; quantity not specified
  - Renewable electricity: 55.1 kWh_el/kg_H2 baseline; 47.5-75.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen: kg_H2 basis
- **CAPEX:** 790.5 €/kW system baseline; 437.8-1110.3 €/kW | **OPEX:** stack 48.9% of system baseline; 45.1-54.0%
- **Efficiency:** 55.1 kWh_el/kg_H2 baseline; 47.5-75.0 kWh_el/kg_H2
- **TRL / maturity:** Dominates market; available at MW-scale.
- **Lifetime / scale:** stack lifetime 75532 h baseline; 60000-94444 h.
- **Time horizon:** near future, by 2035; values inter-/extrapolated to 2035 where needed.
- **Location / region:** Iceland, The Netherlands, Spain, Chile; final products transported to Basel, Switzerland.
- **Energy supply data:**
  - Geothermal electricity: 25.6 €/MWh_el; 16.5-35.0; emissions 17.0 kgCO2eq/MWh_el; 8.5-32.0
  - Offshore wind electricity: 63.9 €/MWh_el; 50.0-80.0; emissions 12.6 kgCO2eq/MWh_el; 4.6-19.0
  - PV Spain electricity: 34.7 €/MWh_el; 23.0-45.0; PV emissions 66.0 kgCO2eq/MWh_el; 20.0-123.8
  - PV Chile electricity: 26.4 €/MWh_el; 18.4-38.9; PV emissions 66.0 kgCO2eq/MWh_el; 20.0-123.8
  - PV-BESS electricity: 68.2 €/MWh_el; 50.0-88.0; emissions 124.3 kgCO2eq/MWh_el; 95.0-153.0
  - CSP-TES electricity: 82.1 €/MWh_el; 79.0-88.0; emissions 16.0 kgCO2eq/MWh_el; 9.8-24.3
- **Further data:** MSP investment lifetime 25 years; plant operation 8000 h/y except first and second year at 30% and 70% capacity; no CAPEX scaling applied to H2 electrolysis.

### Year: post-2035 / by 2050 (long-term future)
- **Description:** AEC; established water electrolysis technology; hydrogen produced exclusively using renewable energy; operates at 60-90 °C.
- **Inputs:**
  - Water: feedstock; quantity not specified
  - Renewable electricity: 48.9 kWh_el/kg_H2 baseline; 45.47-52.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen: kg_H2 basis
- **CAPEX:** 527.2 €/kW system baseline; 306.5-774.5 €/kW | **OPEX:** stack 48.8% of system baseline; 46.5-52.9%
- **Efficiency:** 48.9 kWh_el/kg_H2 baseline; 45.47-52.0 kWh_el/kg_H2
- **TRL / maturity:** Well-established technology scenario after 2035; MW-scale.
- **Lifetime / scale:** stack lifetime 87500 h baseline; 80000-100000 h.
- **Time horizon:** long-term future, post-2035 but by 2050.
- **Location / region:** Iceland, The Netherlands, Spain, Chile; final products transported to Basel, Switzerland.
- **Energy supply data:**
  - Offshore wind electricity: 40.0 €/MWh_el; 25.0-55.0; emissions 11.3 kgCO2eq/MWh_el; 4.4-18.2
  - PV Spain electricity: 18.1 €/MWh_el; 14.0-20.4; PV emissions 57.2 kgCO2eq/MWh_el; 15.9-98.4
  - PV Chile electricity: 13.7 €/MWh_el; 11.0-15.0; PV emissions 57.2 kgCO2eq/MWh_el; 15.9-98.4
  - PV-BESS electricity: 45.2 €/MWh_el; 30.0-66.0
  - CSP-TES electricity: 63.1 €/MWh_el; 47.0-88.0
  - Geothermal emissions: 13.9 kgCO2eq/MWh_el; 5.8-21.9
- **Further data:** Sensitivity includes electrolyzer stack cost, operating hours 8000 h to 4000 h, discount rate 5-8% with 6% baseline, scaling rate 0.6-0.9 with 0.7 baseline; no scaling factor for electrolysis units.

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** PEM electrolysis; water electrolysis; hydrogen produced exclusively using renewable energy; available at MW-scale; operates at 60-90 °C.
- **Inputs:**
  - Water: feedstock; quantity not specified
  - Renewable electricity: 57.9 kWh_el/kg_H2 baseline; 48.8-83.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen: kg_H2 basis
- **CAPEX:** 1047.9 €/kW system baseline; 613.0-1225.9 €/kW | **OPEX:** stack 42.0% of system baseline; 28.6-60.0%
- **Efficiency:** 57.9 kWh_el/kg_H2 baseline; 48.8-83.0 kWh_el/kg_H2
- **TRL / maturity:** Dominates market; available at MW-scale.
- **Lifetime / scale:** stack lifetime 64026 h baseline; 40000-90000 h.
- **Time horizon:** near future, by 2035; values inter-/extrapolated to 2035 where needed.
- **Location / region:** Iceland, The Netherlands, Spain, Chile; final products transported to Basel, Switzerland.
- **Energy supply data:**
  - Geothermal electricity: 25.6 €/MWh_el; 16.5-35.0; emissions 17.0 kgCO2eq/MWh_el; 8.5-32.0
  - Offshore wind electricity: 63.9 €/MWh_el; 50.0-80.0; emissions 12.6 kgCO2eq/MWh_el; 4.6-19.0
  - PV Spain electricity: 34.7 €/MWh_el; 23.0-45.0; PV emissions 66.0 kgCO2eq/MWh_el; 20.0-123.8
  - PV Chile electricity: 26.4 €/MWh_el; 18.4-38.9; PV emissions 66.0 kgCO2eq/MWh_el; 20.0-123.8
  - PV-BESS electricity: 68.2 €/MWh_el; 50.0-88.0; emissions 124.3 kgCO2eq/MWh_el; 95.0-153.0
  - CSP-TES electricity: 82.1 €/MWh_el; 79.0-88.0; emissions 16.0 kgCO2eq/MWh_el; 9.8-24.3
- **Further data:** MSP investment lifetime 25 years; plant operation 8000 h/y except first and second year at 30% and 70% capacity; no CAPEX scaling applied to H2 electrolysis.

### Year: post-2035 / by 2050 (long-term future)
- **Description:** PEM electrolysis; established water electrolysis technology; hydrogen produced exclusively using renewable energy; operates at 60-90 °C.
- **Inputs:**
  - Water: feedstock; quantity not specified
  - Renewable electricity: 53.1 kWh_el/kg_H2 baseline; 47.0-64.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen: kg_H2 basis
- **CAPEX:** 473.5 €/kW system baseline; 257.9-700.5 €/kW | **OPEX:** stack 39.4% of system baseline; 27.8-55.6%
- **Efficiency:** 53.1 kWh_el/kg_H2 baseline; 47.0-64.0 kWh_el/kg_H2
- **TRL / maturity:** Well-established technology scenario after 2035; MW-scale.
- **Lifetime / scale:** stack lifetime 85420 h baseline; 50000-100000 h.
- **Time horizon:** long-term future, post-2035 but by 2050.
- **Location / region:** Iceland, The Netherlands, Spain, Chile; final products transported to Basel, Switzerland.
- **Energy supply data:**
  - Offshore wind electricity: 40.0 €/MWh_el; 25.0-55.0; emissions 11.3 kgCO2eq/MWh_el; 4.4-18.2
  - PV Spain electricity: 18.1 €/MWh_el; 14.0-20.4; PV emissions 57.2 kgCO2eq/MWh_el; 15.9-98.4
  - PV Chile electricity: 13.7 €/MWh_el; 11.0-15.0; PV emissions 57.2 kgCO2eq/MWh_el; 15.9-98.4
  - PV-BESS electricity: 45.2 €/MWh_el; 30.0-66.0
  - CSP-TES electricity: 63.1 €/MWh_el; 47.0-88.0
  - Geothermal emissions: 13.9 kgCO2eq/MWh_el; 5.8-21.9
- **Further data:** Sensitivity includes electrolyzer stack cost, operating hours 8000 h to 4000 h, discount rate 5-8% with 6% baseline, scaling rate 0.6-0.9 with 0.7 baseline; no scaling factor for electrolysis units.

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** SOEC; solid oxide electrolysis cells; operates at 700-850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas.
- **Inputs:**
  - Water / steam: feedstock; quantity not specified
  - Renewable electricity: 34.1 kWh_el/kg_H2 baseline; 26.6-38.1 kWh_el/kg_H2
  - Thermal energy: 8.3 kWh_th/kg_H2 baseline; 6.7-11.0 kWh_th/kg_H2
  - Natural gas: used for high-temperature steam; quantity not specified
- **Outputs:**
  - Hydrogen: kg_H2 basis
- **CAPEX:** 1739.5 €/kW system baseline; 593.0-2770.0 €/kW | **OPEX:** stack 27.0% of system baseline; 23.5-30.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 and 8.3 kWh_th/kg_H2 baseline; electricity 26.6-38.1 kWh_el/kg_H2; thermal 6.7-11.0 kWh_th/kg_H2
- **TRL / maturity:** Current/emerging electrolysis technology; assessed for near future by 2035.
- **Lifetime / scale:** stack lifetime 30308 h baseline; 20000-50924 h.
- **Time horizon:** near future, by 2035; values inter-/extrapolated to 2035 where needed.
- **Location / region:** Iceland, The Netherlands, Spain, Chile; final products transported to Basel, Switzerland.
- **Energy supply data:**
  - Natural gas cost: 25.0 €/MWh_th baseline; 15.0-40.0 €/MWh_th
  - Natural gas emissions: 232.5 kgCO2eq/MWh_th
  - Geothermal electricity: 25.6 €/MWh_el; 16.5-35.0; emissions 17.0 kgCO2eq/MWh_el; 8.5-32.0
  - Offshore wind electricity: 63.9 €/MWh_el; 50.0-80.0; emissions 12.6 kgCO2eq/MWh_el; 4.6-19.0
  - PV Spain electricity: 34.7 €/MWh_el; 23.0-45.0; PV emissions 66.0 kgCO2eq/MWh_el; 20.0-123.8
  - PV Chile electricity: 26.4 €/MWh_el; 18.4-38.9; PV emissions 66.0 kgCO2eq/MWh_el; 20.0-123.8
- **LCA / environmental data:** SOEC can result in more than double life-cycle GHG emissions versus AEC and PEM when natural gas is used for heat.
- **Further data:** MSP investment lifetime 25 years; plant operation 8000 h/y except first and second year at 30% and 70% capacity; no CAPEX scaling applied to H2 electrolysis.

### Year: post-2035 / by 2050 (long-term future)
- **Description:** SOEC; solid oxide electrolysis cells; operates at 700-850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas; SOEC long-term energy requirements assumed same as near future.
- **Inputs:**
  - Water / steam: feedstock; quantity not specified
  - Renewable electricity: 34.1 kWh_el/kg_H2 baseline; 26.6-38.1 kWh_el/kg_H2
  - Thermal energy: 8.3 kWh_th/kg_H2 baseline; 6.7-11.0 kWh_th/kg_H2
  - Natural gas: used for high-temperature steam; quantity not specified
- **Outputs:**
  - Hydrogen: kg_H2 basis
- **CAPEX:** 958.3 €/kW system baseline; 566.5-1723.3 €/kW | **OPEX:** stack 13.8% of system baseline; 12.5-15.0%
- **Efficiency:** electric efficiency same as near future: 34.1 kWh_el/kg_H2 baseline; 26.6-38.1 kWh_el/kg_H2; thermal energy same as near future: 8.3 kWh_th/kg_H2 baseline; 6.7-11.0 kWh_th/kg_H2
- **TRL / maturity:** Long-term scenario assumes plant built using well-established technologies after 2035.
- **Lifetime / scale:** stack lifetime 71991 h baseline; 53750-102222 h.
- **Time horizon:** long-term future, post-2035 but by 2050.
- **Location / region:** Iceland, The Netherlands, Spain, Chile; final products transported to Basel, Switzerland.
- **Energy supply data:**
  - Natural gas emissions: 232.5 kgCO2eq/MWh_th
  - Offshore wind electricity: 40.0 €/MWh_el; 25.0-55.0; emissions 11.3 kgCO2eq/MWh_el; 4.4-18.2
  - PV Spain electricity: 18.1 €/MWh_el; 14.0-20.4; PV emissions 57.2 kgCO2eq/MWh_el; 15.9-98.4
  - PV Chile electricity: 13.7 €/MWh_el; 11.0-15.0; PV emissions 57.2 kgCO2eq/MWh_el; 15.9-98.4
  - Geothermal emissions: 13.9 kgCO2eq/MWh_el; 5.8-21.9
- **LCA / environmental data:** Hydrogen production with SOEC has much higher life-cycle GHG emissions if natural gas is used; SOEC can result in more than double life-cycle GHG emissions versus AEC and PEM.
- **Further data:** SOEC can reduce MSP by 42-218 €/t_product in The Netherlands; SOEC cost benefit in Chile 3-11 €/t_product; for methane production with SOEC, assuming 90% methane boiler efficiency, production capacity 124.6 kt_CH4/y, lower heating value 49.9 MJ/kg_CH4, around one third, 41.6 kt_CH4, of produced CCU methane reused to reduce SOEC life-cycle GHG emissions.

---

# MODEL: gpt-5.4

- **Status:** OK  |  **Words:** 1560  |  **Duration:** 71730 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** Water electrolysis using renewable electricity; operating temperature **60-90 °C**; assessed for H2 supply to FT fuels, methanol, methane, DME, ammonia, urea, olefins, and aromatics pathways.
- **Inputs:**
  - Electricity: **55.1 kWh_el/kg_H2** (min **47.5**; max **75.0**)
  - Water: **not quantified**
- **Outputs:**
  - Hydrogen: **1 kg_H2** basis for specific energy figures
  - Byproducts: **not reported**
- **CAPEX:** **790.5 €/kW** system (min **437.8**; max **1110.3**) | **OPEX:** **48.9% of system** for stack (min **45.1%**; max **54.0%**)
- **Efficiency:** **55.1 kWh_el/kg_H2** electric efficiency (min **47.5**; max **75.0**)
- **TRL / maturity:** **Market-dominant; available at MW-scale**
- **Lifetime, reference capacity, scale:** Stack lifetime **75,532 h** (min **60,000**; max **94,444**); no CAPEX scaling applied to electrolysis; whole product-system mass balances normalized to **360 ktCO2/y** input; plant operation **8,000 h/y**; first and second operating years at **30%** and **70%** capacity; MSP investment lifetime **25 y**
- **Year / horizon:** **Near future / by 2035**; values inter- or extrapolated to **2035** where needed
- **Location / region:** Assessed in **Iceland, The Netherlands, Spain, Chile**; electricity sources in the system study: **geothermal** (Iceland), **offshore wind** (The Netherlands), **PV** (Spain, Chile); hydrogen from electrolysis assumed from **renewable energy exclusively**
- **LCA / environmental:** Included in product life-cycle calculations; near-future product GHGs averaged over all products/electrolysers/locations are **17.1%** higher than long-term future, equal to **1319 kg_CO2eq/t_product** more; electricity contributes **144.5 kg_CO2eq/t_product** more on average in near future
- **Other relevant data:**
  - Monetary basis: **2020 euros**
  - Debt-to-equity ratio: **1.5**
  - Investment allocation: **30%** two years before production, **50%** one year before, **20%** in first production year
  - Near-future Iceland with AEC: LT DAC sorbent cost is around **20%** of MSPs
  - Near-future Iceland with AEC, FT fuels via RWGS: sorbent cost/consumption decline explains **33.4%** of total reduction potential between scenarios, equal to **405 €/t_product**
  - Near-future Iceland with AEC: renewable electricity cost decrease plus electrolyser efficiency improvement account together for around **one-fifth** of total MSP reduction potential

### Year: post-2035 to 2050 (long-term future)
- **Description:** Water electrolysis using renewable electricity; operating temperature **60-90 °C**; long-term scenario assumes plant built with **well-established technologies**
- **Inputs:**
  - Electricity: **48.9 kWh_el/kg_H2** (min **45.47**; max **52.0**)
  - Water: **not quantified**
- **Outputs:**
  - Hydrogen: **1 kg_H2** basis for specific energy figures
  - Byproducts: **not reported**
- **CAPEX:** **527.2 €/kW** system (min **306.5**; max **774.5**) | **OPEX:** **48.8% of system** for stack (min **46.5%**; max **52.9%**)
- **Efficiency:** **48.9 kWh_el/kg_H2** electric efficiency (min **45.47**; max **52.0**)
- **TRL / maturity:** **Well-established technology** in this scenario
- **Lifetime, reference capacity, scale:** Stack lifetime **87,500 h** (min **80,000**; max **100,000**); no CAPEX scaling applied to electrolysis; whole product-system mass balances normalized to **360 ktCO2/y** input; plant operation **8,000 h/y**; MSP investment lifetime **25 y**
- **Year / horizon:** **Long-term future / post-2035, by 2050**
- **Location / region:** Assessed in **Iceland, The Netherlands, Spain, Chile**; renewable electricity sources as above
- **LCA / environmental:** Product systems using LT DAC in the long-term future achieve life-cycle GHG reductions versus fossil counterparts of **55-88%** (FT fuels via RWGS), **54-92%** (FT via ER), **65-91%** (methanol), **62-94%** (methane), **77-96%** (DME), **72-96%** (ammonia), **68-89%** (urea), **50-87%** (olefins), **55-87%** (aromatics)
- **Other relevant data:**
  - Average MSP decline from near to long-term future: **54.2%**, equivalent to **5.1%/y** over **15 y**
  - Long-term future Iceland with AEC: point-source capture gives average MSPs **7.6%** lower than LT DAC and GHGs **10.9%** higher than LT DAC
  - Long-term future Iceland with AEC: HT DAC gives average MSPs **21.5%** higher than LT DAC and GHGs **1.5 times** higher than LT DAC

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** Water electrolysis using renewable electricity; operating temperature **60-90 °C**; assessed for H2 supply to FT fuels, methanol, methane, DME, ammonia, urea, olefins, and aromatics pathways.
- **Inputs:**
  - Electricity: **57.9 kWh_el/kg_H2** (min **48.8**; max **83.0**)
  - Water: **not quantified**
- **Outputs:**
  - Hydrogen: **1 kg_H2** basis for specific energy figures
  - Byproducts: **not reported**
- **CAPEX:** **1047.9 €/kW** system (min **613.0**; max **1225.9**) | **OPEX:** **42.0% of system** for stack (min **28.6%**; max **60.0%**)
- **Efficiency:** **57.9 kWh_el/kg_H2** electric efficiency (min **48.8**; max **83.0**)
- **TRL / maturity:** **Market-dominant; available at MW-scale**
- **Lifetime, reference capacity, scale:** Stack lifetime **64,026 h** (min **40,000**; max **90,000**); no CAPEX scaling applied to electrolysis; whole product-system mass balances normalized to **360 ktCO2/y** input; plant operation **8,000 h/y**; first and second operating years at **30%** and **70%** capacity; MSP investment lifetime **25 y**
- **Year / horizon:** **Near future / by 2035**; values inter- or extrapolated to **2035** where needed
- **Location / region:** Assessed in **Iceland, The Netherlands, Spain, Chile**; electricity sources in the system study: **geothermal**, **offshore wind**, **PV**; hydrogen from electrolysis assumed from **renewable energy exclusively**
- **LCA / environmental:** Included in product life-cycle calculations
- **Other relevant data:**
  - Monetary basis: **2020 euros**
  - Debt-to-equity ratio: **1.5**
  - Investment allocation: **30% / 50% / 20%** over the two pre-start years and first production year

### Year: post-2035 to 2050 (long-term future)
- **Description:** Water electrolysis using renewable electricity; operating temperature **60-90 °C**; long-term scenario assumes plant built with **well-established technologies**
- **Inputs:**
  - Electricity: **53.1 kWh_el/kg_H2** (min **47.0**; max **64.0**)
  - Water: **not quantified**
- **Outputs:**
  - Hydrogen: **1 kg_H2** basis for specific energy figures
  - Byproducts: **not reported**
- **CAPEX:** **473.5 €/kW** system (min **257.9**; max **700.5**) | **OPEX:** **39.4% of system** for stack (min **27.8%**; max **55.6%**)
- **Efficiency:** **53.1 kWh_el/kg_H2** electric efficiency (min **47.0**; max **64.0**)
- **TRL / maturity:** **Well-established technology** in this scenario
- **Lifetime, reference capacity, scale:** Stack lifetime **85,420 h** (min **50,000**; max **100,000**); no CAPEX scaling applied to electrolysis; whole product-system mass balances normalized to **360 ktCO2/y** input; plant operation **8,000 h/y**; MSP investment lifetime **25 y**
- **Year / horizon:** **Long-term future / post-2035, by 2050**
- **Location / region:** Assessed in **Iceland, The Netherlands, Spain, Chile**; renewable electricity sources as above
- **LCA / environmental:** Product systems using LT DAC in the long-term future achieve life-cycle GHG reductions versus fossil counterparts of **55-88%** (FT fuels via RWGS), **54-92%** (FT via ER), **65-91%** (methanol), **62-94%** (methane), **77-96%** (DME), **72-96%** (ammonia), **68-89%** (urea), **50-87%** (olefins), **55-87%** (aromatics)
- **Other relevant data:**
  - Monetary basis: **2020 euros**
  - Debt-to-equity ratio: **1.5**
  - Investment allocation: **30% / 50% / 20%**

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** Solid oxide electrolysis; operating temperature **700-850 °C**; operated **below thermoneutral conditions**; high-temperature steam assumed from **natural gas**; assessed for H2 supply to FT fuels, methanol, methane, DME, ammonia, urea, olefins, and aromatics pathways.
- **Inputs:**
  - Electricity: **34.1 kWh_el/kg_H2** (min **26.6**; max **38.1**)
  - Thermal energy: **8.3 kWh_th/kg_H2** (min **6.7**; max **11.0**)
  - High-temperature steam: **required; supplied from NG**; quantity not separately stated
  - Water: **not quantified**
- **Outputs:**
  - Hydrogen: **1 kg_H2** basis for specific energy figures
  - Byproducts: **not reported**
- **CAPEX:** **1739.5 €/kW** system (min **593.0**; max **2770.0**) | **OPEX:** **27.0% of system** for stack (min **23.5%**; max **30.0%**)
- **Efficiency:** **34.1 kWh_el/kg_H2** electric efficiency (min **26.6**; max **38.1**); thermal energy demand **8.3 kWh_th/kg_H2** (min **6.7**; max **11.0**)
- **TRL / maturity:** **Not numerically stated**
- **Lifetime, reference capacity, scale:** Stack lifetime **30,308 h** (min **20,000**; max **50,924**); no CAPEX scaling applied to electrolysis; whole product-system mass balances normalized to **360 ktCO2/y** input; plant operation **8,000 h/y**; first and second operating years at **30%** and **70%** capacity; MSP investment lifetime **25 y**
- **Year / horizon:** **Near future / by 2035**; values inter- or extrapolated to **2035** where needed
- **Location / region:** Assessed in **Iceland, The Netherlands, Spain, Chile**; electricity sources in the system study: **geothermal**, **offshore wind**, **PV**; thermal steam assumption from **NG**
- **LCA / environmental:**
  - SOEC can result in **more than double** the life-cycle GHG emissions of AEC and PEM
  - For methane production with SOEC: assuming **90%** methane boiler efficiency, production capacity **124.6 kt_CH4/y**, lower heating value **49.9 MJ/kg_CH4**, about **41.6 kt_CH4** (around one-third of produced CCU methane) is reused to reduce SOEC life-cycle GHG emissions
- **Other relevant data:**
  - Monetary basis: **2020 euros**
  - Debt-to-equity ratio: **1.5**
  - Investment allocation: **30% / 50% / 20%**
  - Near future: olefins and aromatics in Spain/Chile with SOEC have CCA values of **€5062-26739/t_CO2eq,av**

### Year: post-2035 to 2050 (long-term future)
- **Description:** Solid oxide electrolysis; operating temperature **700-850 °C**; operated **below thermoneutral conditions**; long-term scenario assumes plant built with **well-established technologies**; SOEC long-term energy requirements assumed **same as near future**
- **Inputs:**
  - Electricity: **34.1 kWh_el/kg_H2** (min **26.6**; max **38.1**)
  - Thermal energy: **8.3 kWh_th/kg_H2** (min **6.7**; max **11.0**)
  - High-temperature steam: **required; supplied from NG**; quantity not separately stated
  - Water: **not quantified**
- **Outputs:**
  - Hydrogen: **1 kg_H2** basis for specific energy figures
  - Byproducts: **not reported**
- **CAPEX:** **958.3 €/kW** system (min **566.5**; max **1723.3**) | **OPEX:** **13.8% of system** for stack (min **12.5%**; max **15.0%**)
- **Efficiency:** **34.1 kWh_el/kg_H2** electric efficiency (same as near future; min **26.6**; max **38.1**); thermal energy demand **8.3 kWh_th/kg_H2** (same as near future; min **6.7**; max **11.0**)
- **TRL / maturity:** **Well-established technology** in this scenario
- **Lifetime, reference capacity, scale:** Stack lifetime **71,991 h** (min **53,750**; max **102,222**); no CAPEX scaling applied to electrolysis; whole product-system mass balances normalized to **360 ktCO2/y** input; plant operation **8,000 h/y**; MSP investment lifetime **25 y**
- **Year / horizon:** **Long-term future / post-2035, by 2050**
- **Location / region:** Assessed in **Iceland, The Netherlands, Spain, Chile**
- **LCA / environmental:** Hydrogen production with SOEC has much higher life-cycle GHG emissions if **natural gas** is used for heat
- **Other relevant data:**
  - Long-term future SOEC can reduce MSP by **42-218 €/t_product** in **The Netherlands**
  - Long-term future SOEC cost benefit in **Chile**: **3-11 €/t_product**
  - Monetary basis: **2020 euros**
  - Debt-to-equity ratio: **1.5**
  - Investment allocation: **30% / 50% / 20%**

---

# MODEL: gpt-5.3-codex

- **Status:** OK  |  **Words:** 876  |  **Duration:** 54928 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** AEC for renewable H2 production; operating temperature **60-90 °C**; values interpolated/extrapolated to 2035 where needed.
- **Inputs:**
  - Electricity: **55.1 kWh_el/kg_H2** (min **47.5**, max **75.0**)
  - Energy source constraint: renewable electricity exclusively (system assumption)
- **Outputs:**
  - Hydrogen: functional basis **kg_H2** (from specific energy metric)
- **CAPEX:** **790.5 €/kW** (min **437.8**, max **1110.3**) | **OPEX:** stack **48.9% of system** (min **45.1%**, max **54.0%**)
- **Efficiency:** **55.1 kWh_el/kg_H2** (min **47.5**, max **75.0**)
- **TRL / maturity:** AEC dominates market; available at **MW-scale**.
- **Lifetime / scale / other technical-economic data:**
  - Stack lifetime: **75,532 h** (min **60,000 h**, max **94,444 h**)
  - Plant operation assumption (system): **8,000 h/y** (year 1: **30%** capacity; year 2: **70%**)
  - Investment lifetime (MSP framework): **25 y**
  - Debt-to-equity ratio: **1.5**
  - Monetary basis: **2020 €**
  - No scaling applied to H2 electrolysis CAPEX
- **Year/time horizon:** Near future (by **2035**)
- **Location/region:** Iceland, The Netherlands, Spain, Chile.
- **LCA / environmental data:**
  - Reported as most favorable electrolyzer option for near-future life-cycle GHG and CCA outcomes in this study.
  - Near-future average across all products/electrolyzers/locations: **+17.1%** (**+1319 kg_CO2eq/t_product**) vs long-term future; electricity contributes **+144.5 kg_CO2eq/t_product**.

### Year: post-2035 to 2050 (long-term future)
- **Description:** Long-term scenario assumes plant built with well-established technologies after 2035.
- **Inputs:**
  - Electricity: **48.9 kWh_el/kg_H2** (min **45.47**, max **52.0**)
- **Outputs:**
  - Hydrogen: functional basis **kg_H2**
- **CAPEX:** **527.2 €/kW** (min **306.5**, max **774.5**) | **OPEX:** stack **48.8% of system** (min **46.5%**, max **52.9%**)
- **Efficiency:** **48.9 kWh_el/kg_H2** (min **45.47**, max **52.0**)
- **TRL / maturity:** Treated as established for post-2035 deployment scenario.
- **Lifetime / scale / other technical-economic data:**
  - Stack lifetime: **87,500 h** (min **80,000 h**, max **100,000 h**)
  - In Iceland with AEC, switching carbon source PSC vs LT DAC: MSP **7.6%** lower, GHG **10.9%** higher (reported for long-term future).
  - DME in Chile with AEC: life-cycle GHG reported **3.6×** higher than in The Netherlands (location-electricity effect).

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** PEM electrolysis for renewable H2 production; operating temperature **60-90 °C**.
- **Inputs:**
  - Electricity: **57.9 kWh_el/kg_H2** (min **48.8**, max **83.0**)
  - Energy source constraint: renewable electricity exclusively (system assumption)
- **Outputs:**
  - Hydrogen: functional basis **kg_H2**
- **CAPEX:** **1047.9 €/kW** (min **613.0**, max **1225.9**) | **OPEX:** stack **42.0% of system** (min **28.6%**, max **60.0%**)
- **Efficiency:** **57.9 kWh_el/kg_H2** (min **48.8**, max **83.0**)
- **TRL / maturity:** PEM dominates market; available at **MW-scale**.
- **Lifetime / scale / other technical-economic data:**
  - Stack lifetime: **64,026 h** (min **40,000 h**, max **90,000 h**)
  - Plant operation assumption (system): **8,000 h/y**; investment lifetime **25 y**; debt/equity **1.5**; monetary basis **2020 €**
  - No scaling applied to H2 electrolysis CAPEX
- **Year/time horizon:** Near future (by **2035**)
- **Location/region:** Iceland, The Netherlands, Spain, Chile.
- **LCA / environmental data:** Study reports PEM-AEC differences in CCA reduce in long-term scenario.

### Year: post-2035 to 2050 (long-term future)
- **Description:** Post-2035 deployment with well-established technologies.
- **Inputs:**
  - Electricity: **53.1 kWh_el/kg_H2** (min **47.0**, max **64.0**)
- **Outputs:**
  - Hydrogen: functional basis **kg_H2**
- **CAPEX:** **473.5 €/kW** (min **257.9**, max **700.5**) | **OPEX:** stack **39.4% of system** (min **27.8%**, max **55.6%**)
- **Efficiency:** **53.1 kWh_el/kg_H2** (min **47.0**, max **64.0**)
- **TRL / maturity:** Treated as established for long-term deployment case.
- **Lifetime / scale / other technical-economic data:**
  - Stack lifetime: **85,420 h** (min **50,000 h**, max **100,000 h**)

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** SOEC hydrogen production at **700-850 °C**; operated below thermoneutral conditions; high-temperature steam assumed from natural gas.
- **Inputs:**
  - Electricity: **34.1 kWh_el/kg_H2** (min **26.6**, max **38.1**)
  - Thermal energy: **8.3 kWh_th/kg_H2** (min **6.7**, max **11.0**)
  - HT steam source assumption: NG-fed heat/steam
- **Outputs:**
  - Hydrogen: functional basis **kg_H2**
- **CAPEX:** **1739.5 €/kW** (min **593.0**, max **2770.0**) | **OPEX:** stack **27.0% of system** (min **23.5%**, max **30.0%**)
- **Efficiency:** **34.1 kWh_el/kg_H2** (min **26.6**, max **38.1**) plus thermal input **8.3 kWh_th/kg_H2** (min **6.7**, max **11.0**)
- **TRL / maturity:** Included as relevant technology; not reported as dominant market technology.
- **Lifetime / scale / other technical-economic data:**
  - Stack lifetime: **30,308 h** (min **20,000 h**, max **50,924 h**)
  - Plant operation assumption (system): **8,000 h/y**; investment lifetime **25 y**; debt/equity **1.5**; monetary basis **2020 €**
  - No scaling applied to H2 electrolysis CAPEX
- **Year/time horizon:** Near future (by **2035**)
- **Location/region:** Iceland, The Netherlands, Spain, Chile.
- **LCA / environmental data:**
  - Reported that SOEC can result in **>2×** life-cycle GHG emissions vs AEC/PEM when NG is used for heat.
  - Near-future olefins/aromatics in Spain/Chile with SOEC: CCA **€5062-26739 per t_CO2eq,av** (reported case).

### Year: post-2035 to 2050 (long-term future)
- **Description:** Long-term SOEC scenario keeps SOEC energy requirements equal to near-future values.
- **Inputs:**
  - Electricity: **same as near future** (**34.1 kWh_el/kg_H2**, min **26.6**, max **38.1**)
  - Thermal energy: **same as near future** (**8.3 kWh_th/kg_H2**, min **6.7**, max **11.0**)
- **Outputs:**
  - Hydrogen: functional basis **kg_H2**
- **CAPEX:** **958.3 €/kW** (min **566.5**, max **1723.3**) | **OPEX:** stack **13.8% of system** (min **12.5%**, max **15.0%**)
- **Efficiency:** Electric and thermal requirements unchanged from near future (assumed).
- **TRL / maturity:** Long-term scenario assumes post-2035 well-established deployment conditions.
- **Lifetime / scale / other technical-economic data:**
  - Stack lifetime: **71,991 h** (min **53,750 h**, max **102,222 h**)
  - Reported MSP effect from SOEC vs alternatives: reduction of **42-218 €/t_product** in The Netherlands; **3-11 €/t_product** in Chile.
  - Methane production case with SOEC: methane boiler efficiency **90%**; methane production capacity **124.6 kt_CH4/y**; methane LHV **49.9 MJ/kg_CH4**; methane reused internally **41.6 kt_CH4** (~one-third) to reduce SOEC life-cycle GHG emissions.

---

# MODEL: gpt-5.4-mini

- **Status:** OK  |  **Words:** 524  |  **Duration:** 13928 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: Near future (by 2035)
- **Description:** Water electrolysis; AEC operates at **60–90 °C**; renewable electricity only; market-dominant and available at **MW-scale**.
- **Inputs:** electricity **55.1 kWh_el/kg_H2** (47.5–75.0); water **not stated**; other consumables **not stated**.
- **Outputs:** hydrogen **1 kg_H2** basis; oxygen/byproducts **not stated**.
- **CAPEX:** **790.5 €/kW** (437.8–1110.3) system.
- **OPEX:** **48.9% of system** (45.1–54.0) stack.
- **Efficiency:** **55.1 kWh_el/kg_H2** (47.5–75.0).
- **TRL / maturity:** market-dominant; available at **MW-scale**.
- **Lifetime / scale:** stack lifetime **75532 h** (60000–94444).
- **Other data:** values inter- or extrapolated to **2035** where needed.

### Year: Long-term future (post-2035)
- **Description:** Water electrolysis; AEC operates at **60–90 °C**; renewable electricity only; market-dominant and available at **MW-scale**.
- **Inputs:** electricity **48.9 kWh_el/kg_H2** (45.47–52.0); water **not stated**; other consumables **not stated**.
- **Outputs:** hydrogen **1 kg_H2** basis; oxygen/byproducts **not stated**.
- **CAPEX:** **527.2 €/kW** (306.5–774.5) system.
- **OPEX:** **48.8% of system** (46.5–52.9) stack.
- **Efficiency:** **48.9 kWh_el/kg_H2** (45.47–52.0).
- **TRL / maturity:** market-dominant; available at **MW-scale**.
- **Lifetime / scale:** stack lifetime **87500 h** (80000–100000).
- **Other data:** values inter- or extrapolated to **2035** where needed.

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: Near future (by 2035)
- **Description:** Water electrolysis; PEM operates at **60–90 °C**; renewable electricity only; market-dominant and available at **MW-scale**.
- **Inputs:** electricity **57.9 kWh_el/kg_H2** (48.8–83.0); water **not stated**; other consumables **not stated**.
- **Outputs:** hydrogen **1 kg_H2** basis; oxygen/byproducts **not stated**.
- **CAPEX:** **1047.9 €/kW** (613.0–1225.9) system.
- **OPEX:** **42.0% of system** (28.6–60.0) stack.
- **Efficiency:** **57.9 kWh_el/kg_H2** (48.8–83.0).
- **TRL / maturity:** market-dominant; available at **MW-scale**.
- **Lifetime / scale:** stack lifetime **64026 h** (40000–90000).
- **Other data:** values inter- or extrapolated to **2035** where needed.

### Year: Long-term future (post-2035)
- **Description:** Water electrolysis; PEM operates at **60–90 °C**; renewable electricity only; market-dominant and available at **MW-scale**.
- **Inputs:** electricity **53.1 kWh_el/kg_H2** (47.0–64.0); water **not stated**; other consumables **not stated**.
- **Outputs:** hydrogen **1 kg_H2** basis; oxygen/byproducts **not stated**.
- **CAPEX:** **473.5 €/kW** (257.9–700.5) system.
- **OPEX:** **39.4% of system** (27.8–55.6) stack.
- **Efficiency:** **53.1 kWh_el/kg_H2** (47.0–64.0).
- **TRL / maturity:** market-dominant; available at **MW-scale**.
- **Lifetime / scale:** stack lifetime **85420 h** (50000–100000).
- **Other data:** values inter- or extrapolated to **2035** where needed.

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: Near future (by 2035)
- **Description:** Water electrolysis; **SOEC operates at 700–850 °C**; operates **below thermoneutral conditions**; **HT steam assumed from NG**; renewable electricity only.
- **Inputs:** electricity **34.1 kWh_el/kg_H2** (26.6–38.1); thermal energy **8.3 kWh_th/kg_H2** (6.7–11.0); water **not stated**; other consumables **not stated**.
- **Outputs:** hydrogen **1 kg_H2** basis; oxygen/byproducts **not stated**.
- **CAPEX:** **1739.5 €/kW** (593.0–2770.0) system.
- **OPEX:** **27.0% of system** (23.5–30.0) stack.
- **Efficiency:** electric **34.1 kWh_el/kg_H2** (26.6–38.1); thermal **8.3 kWh_th/kg_H2** (6.7–11.0).
- **TRL / maturity:** **not stated**.
- **Lifetime / scale:** stack lifetime **30308 h** (20000–50924).
- **Other data:** values inter- or extrapolated to **2035** where needed.

### Year: Long-term future (post-2035)
- **Description:** Water electrolysis; **SOEC operates at 700–850 °C**; operates **below thermoneutral conditions**; **HT steam assumed from NG**; renewable electricity only.
- **Inputs:** electricity **same as near future**; thermal energy **same as near future**; water **not stated**; other consumables **not stated**.
- **Outputs:** hydrogen **1 kg_H2** basis; oxygen/byproducts **not stated**.
- **CAPEX:** **958.3 €/kW** (566.5–1723.3) system.
- **OPEX:** **13.8% of system** (12.5–15.0) stack.
- **Efficiency:** electric **same as near future**; thermal **same as near future**.
- **TRL / maturity:** **not stated**.
- **Lifetime / scale:** stack lifetime **71991 h** (53750–102222).
- **Other data:** SOEC long-term future energy requirements assumed **same as near future**; values inter- or extrapolated to **2035** where needed.

---

# MODEL: gpt-5-mini

- **Status:** OK  |  **Words:** 1694  |  **Duration:** 217712 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2025 (current/baseline)
- **Description:** Commercial alkaline water electrolysis (AEC); operating temperature 60–90 °C; available at MW-scale; hydrogen produced using renewable electricity (monetary values in 2020 €).
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (near-future baseline value reported; see 2035 for scenario values)
  - Water: (implicit feedstock; quantity not numerically specified)
  - Auxiliary services: grid/storage as per site (Iceland: geothermal; NL: offshore wind; Spain/Chile: PV) — see location energy tables
- **Outputs:**
  - H2: 1 kg_H2 per 55.1 kWh_el (use of table unit kWh_el/kg_H2)
- **CAPEX:** 790.5 €/kW (near-future baseline; range 437.8–1110.3 €/kW)
- **OPEX:** OPEX (stack) = 48.9% of system (near-future baseline; range 45.1–54.0%)
- **Efficiency:** 55.1 kWh_el/kg_H2 (near-future baseline; range 47.5–75.0 kWh_el/kg_H2)
- **TRL / maturity:** Commercial; AEC dominates market and is available at MW-scale.
- **Lifetime / scale:** Stack lifetime 75,532 h (near-future baseline; range 60,000–94,444 h); plant operation assumption 8,000 h/year; MSP investment lifetime 25 years.
- **Year/time horizon the data applies to:** Values labeled "near future baseline" in source (near future ≈ by 2035) reported here as baseline numbers; monetary values in 2020 €.
- **Location/region:** Not technology-specific; hydrogen assumed from renewable sources — Iceland (geothermal), Netherlands (offshore wind), Spain & Chile (PV).
- **LCA / environmental:** Hydrogen via electrolysis assumed from renewable electricity; location-dependent electricity emissions in Table 3 (e.g., geothermal 17.0 kgCO2eq/MWhel near-future baseline; offshore wind 12.6 kgCO2eq/MWhel; PV 66.0 kgCO2eq/MWhel) influence AEC life-cycle GHGs.
- **Other data / notes:**
  - CAPEX scaling: no scaling applied to H2 electrolysis units.
  - Near-future min/max: CAPEX min 437.8 €/kW, max 1110.3 €/kW; OPEX min 45.1%, max 54.0%; efficiency min 47.5, max 75.0 kWh_el/kg_H2.

### Year: 2035 (near future / scenario values)
- **Description:** Near-future baseline values (table labeled "near future baseline").
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (baseline); min 47.5 kWh_el/kg_H2; max 75.0 kWh_el/kg_H2
- **Outputs:**
  - H2: 1 kg_H2 per 55.1 kWh_el (baseline; ranges as above)
- **CAPEX:** 790.5 €/kW (baseline) | range: 437.8–1110.3 €/kW
- **OPEX:** 48.9% of system (stack; baseline) | range: 45.1–54.0% of system
- **Efficiency:** 55.1 kWh_el/kg_H2 (baseline) | 47.5–75.0 kWh_el/kg_H2
- **TRL / maturity:** Commercial, MW-scale deployment documented.
- **Lifetime / scale:** Stack lifetime 75,532 h (60000–94,444 h); plant operation 8,000 h/year.
- **Year/time horizon:** near future (by 2035); monetary 2020 €.
- **Location/region:** Electricity source location affects costs/emissions (see Table 3: geothermal/offshore wind/PV costs & emissions).
- **LCA / environmental:** Location-dependent; AEC cited as favorable for life-cycle GHGs in paper vs alternatives under renewable electricity supply.
- **Other data:**
  - Long-term improvements reported (see 2050 section).

### Year: 2050 (long-term future / post-2035)
- **Description:** Long-term future baseline (post-2035) projections.
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (long-term baseline); min 45.47; max 52.0 kWh_el/kg_H2
- **Outputs:**
  - H2: 1 kg_H2 per 48.9 kWh_el (long-term baseline)
- **CAPEX:** 527.2 €/kW (long-term baseline) | range: 306.5–774.5 €/kW
- **OPEX:** 48.8% of system (stack; long-term baseline) | range: 46.5–52.9%
- **Efficiency:** 48.9 kWh_el/kg_H2 (long-term baseline) | 45.47–52.0 kWh_el/kg_H2
- **TRL / maturity:** Commercial, mainstream at scale in long-term scenario.
- **Lifetime / scale:** Stack lifetime 87,500 h (80,000–100,000 h); plant operation 8,000 h/year assumed.
- **Year/time horizon:** long-term future (post-2035; referenced to 2050 reduction potentials for supply emissions); monetary 2020 €.
- **Location/region:** Same location dependence applies (Iceland, NL, Spain, Chile) — energy-source costs/emissions improve by long-term values in Table 3.
- **LCA / environmental:** Long-term reductions in electricity emissions (e.g., PV, wind reductions) improve AEC lifecycle GHGs.
- **Other data:**
  - No scaling applied to H2 electrolysis CAPEX in study.

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2025 (current/baseline)
- **Description:** Commercial proton exchange membrane (PEM) electrolysis; operating temperature 60–90 °C; available at MW-scale; hydrogen from renewable electricity.
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (near-future baseline reference; see 2035)
  - Water: (implicit; not numerically specified)
  - Stack consumables (membranes, catalysts) — OPEX includes stack-related percentages
- **Outputs:**
  - H2: 1 kg_H2 per 57.9 kWh_el (use of kWh_el/kg_H2)
- **CAPEX:** 1,047.9 €/kW (near-future baseline; range 613.0–1225.9 €/kW)
- **OPEX:** OPEX (stack) = 42.0% of system (near-future baseline; range 28.6–60.0%)
- **Efficiency:** 57.9 kWh_el/kg_H2 (near-future baseline; range 48.8–83.0 kWh_el/kg_H2)
- **TRL / maturity:** Commercial; PEM and AEC dominate market and available at MW-scale.
- **Lifetime / scale:** Stack lifetime 64,026 h (near-future baseline; range 40,000–90,000 h); plant operation 8,000 h/year; MSP investment lifetime 25 years.
- **Year/time horizon the data applies to:** Near-future baseline values (by 2035) listed; monetary values 2020 €.
- **Location/region:** Renewable electricity source location applies (Iceland geothermal, NL offshore wind, Spain/Chile PV) — affects cost/emissions.
- **LCA / environmental:** Hydrogen assumed from renewables; electrolyzer lifecycle influenced by electricity source emissions (see Table 3).
- **Other data / notes:**
  - No CAPEX scaling applied to electrolysis units.
  - Near-future min/max: CAPEX min 613.0 €/kW, max 1225.9 €/kW; OPEX min 28.6%, max 60.0%; efficiency min 48.8, max 83.0 kWh_el/kg_H2.

### Year: 2035 (near future / scenario values)
- **Description:** Near-future baseline scenario values.
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (baseline); min 48.8; max 83.0 kWh_el/kg_H2
- **Outputs:**
  - H2: 1 kg_H2 per 57.9 kWh_el (baseline)
- **CAPEX:** 1,047.9 €/kW (baseline) | range: 613.0–1225.9 €/kW
- **OPEX:** 42.0% of system (stack; baseline) | range: 28.6–60.0%
- **Efficiency:** 57.9 kWh_el/kg_H2 (baseline) | 48.8–83.0 kWh_el/kg_H2
- **TRL / maturity:** Commercial and MW-scale deployment documented.
- **Lifetime / scale:** Stack lifetime 64,026 h (40,000–90,000 h); plant operation 8,000 h/year.
- **Year/time horizon:** near future (by 2035); monetary 2020 €.
- **Location/region:** Location-dependent electricity cost/emissions per Table 3.
- **LCA / environmental:** Location and electricity mix drive lifecycle GHGs; PEM cited alongside AEC as dominant.

### Year: 2050 (long-term future / post-2035)
- **Description:** Long-term future baseline projections.
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (long-term baseline); min 47.0; max 64.0 kWh_el/kg_H2
- **Outputs:**
  - H2: 1 kg_H2 per 53.1 kWh_el (long-term baseline)
- **CAPEX:** 473.5 €/kW (long-term baseline) | range: 257.9–700.5 €/kW
- **OPEX:** 39.4% of system (stack; long-term baseline) | range: 27.8–55.6%
- **Efficiency:** 53.1 kWh_el/kg_H2 (long-term baseline) | 47.0–64.0 kWh_el/kg_H2
- **TRL / maturity:** Commercial, projected cost/efficiency improvements by long-term scenario.
- **Lifetime / scale:** Stack lifetime 85,420 h (50,000–100,000 h); plant operation 8,000 h/year.
- **Year/time horizon:** long-term future (post-2035); monetary 2020 €.
- **Location/region:** Same location dependence as AEC; long-term electricity costs/emissions improve per Table 3.
- **LCA / environmental:** Long-term reductions in electricity-emission intensities lower PEM lifecycle GHGs.
- **Other data:**
  - PEM CAPEX/OPEX/efficiency ranges as reported above.

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2025 (current/baseline)
- **Description:** Solid oxide electrolyzer cells (SOEC); high-temperature electrolysis operating 700–850 °C; operates below thermoneutral conditions in study; HT steam assumed from natural gas in scenarios; can be coupled with external heat sources.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (near-future baseline reference; see 2035)
  - Thermal (high-temperature) energy: 8.3 kWh_th/kg_H2 (near-future baseline; range 6.7–11.0 kWh_th/kg_H2)
  - Steam/HT heat (assumed HT steam from natural gas for some cases)
  - Water: (implicit feedstock)
- **Outputs:**
  - H2: 1 kg_H2 per 34.1 kWh_el plus 8.3 kWh_th (baseline)
- **CAPEX:** 1,739.5 €/kW (near-future baseline; range 593.0–2770.0 €/kW)
- **OPEX:** OPEX (stack) = 27.0% of system (near-future baseline; range 23.5–30.0%)
- **Efficiency:** 34.1 kWh_el/kg_H2 (near-future baseline; range 26.6–38.1 kWh_el/kg_H2); thermal energy 8.3 kWh_th/kg_H2 (6.7–11.0)
- **TRL / maturity:** Demonstrated technology but less widespread than AEC/PEM at MW-scale per context; study notes SOEC-specific operational and LCA considerations.
- **Lifetime / scale:** Stack lifetime 30,308 h (near-future baseline; range 20,000–50,924 h); plant operation 8,000 h/year; MSP investment lifetime 25 years.
- **Year/time horizon the data applies to:** Near-future baseline (by 2035) values reported; monetary 2020 €.
- **Location/region:** Heat source choice (NG vs renewables) and location electricity mix strongly affect SOEC lifecycle GHGs (study notes SOEC may have much higher life-cycle GHGs if NG used).
- **LCA / environmental:** SOEC life-cycle GHGs can be higher than AEC/PEM when natural gas supplies HT steam; SOEC-specific notes: higher life-cycle GHGs in some scenarios, can double vs AEC/PEM for some products.
- **Other data / notes:**
  - SOEC electric efficiency and thermal energy reported as "same as near future" for long-term baseline in study.
  - No CAPEX scaling applied to electrolysis in study.

### Year: 2035 (near future / scenario values)
- **Description:** Near-future baseline scenario for SOEC.
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline); min 26.6; max 38.1 kWh_el/kg_H2
  - Thermal energy: 8.3 kWh_th/kg_H2 (baseline); min 6.7; max 11.0 kWh_th/kg_H2
  - HT steam source: study assumes HT steam from NG in scenarios (affects GHG)
- **Outputs:**
  - H2: 1 kg_H2 per 34.1 kWh_el + 8.3 kWh_th (baseline)
- **CAPEX:** 1,739.5 €/kW (baseline) | range: 593.0–2770.0 €/kW
- **OPEX:** 27.0% of system (stack; baseline) | range: 23.5–30.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 (baseline) | 26.6–38.1 kWh_el/kg_H2; thermal 8.3 kWh_th/kg_H2 (6.7–11.0)
- **TRL / maturity:** Demonstrated but less prevalent at MW-scale compared to AEC/PEM; operational at high temperatures (700–850 °C).
- **Lifetime / scale:** Stack lifetime 30,308 h (20,000–50,924 h); plant operation 8,000 h/year.
- **Year/time horizon:** near future (by 2035); monetary 2020 €.
- **Location/region:** If HT steam supplied from NG (study assumption for some scenarios), SOEC lifecycle GHGs increase markedly; location-specific electricity/heat emissions apply (see Table 3).
- **LCA / environmental:** Study reports SOEC can produce much higher life-cycle GHG emissions than AEC/PEM when NG is used for HT steam; examples: SOEC can more than double life-cycle GHGs vs AEC/PEM for some products; methane case: SOEC-driven methane required reuse of ~41.6 kt_CH4 to reduce SOEC GHGs in one scenario.
- **Other data:**
  - Near-future CAPEX min 593.0 €/kW, max 2770.0 €/kW; OPEX min 23.5%, max 30.0%.

### Year: 2050 (long-term future / post-2035)
- **Description:** Long-term future baseline; study assumes SOEC energy requirements same as near future for long-term baseline.
- **Inputs:**
  - Electricity: same as near-future (34.1 kWh_el/kg_H2; 26.6–38.1)
  - Thermal energy: same as near-future (8.3 kWh_th/kg_H2; 6.7–11.0)
  - HT steam: can be sourced differently in long-term scenarios; study notes HT steam assumed from NG in some cases unless otherwise supplied
- **Outputs:**
  - H2: 1 kg_H2 per 34.1 kWh_el + 8.3 kWh_th (long-term baseline; study states "same as near future")
- **CAPEX:** 958.3 €/kW (long-term baseline) | range: 566.5–1723.3 €/kW
- **OPEX:** 13.8% of system (stack; long-term baseline) | range: 12.5–15.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 (long-term baseline; same as near future) | 26.6–38.1 kWh_el/kg_H2; thermal 8.3 kWh_th/kg_H2
- **TRL / maturity:** Projected improvements in CAPEX/OPEX/lifetime by long-term; still high-temperature technology with associated system integration needs.
- **Lifetime / scale:** Stack lifetime 71,991 h (long-term baseline; range 53,750–102,222 h)
- **Year/time horizon:** long-term future (post-2035); study treats SOEC energy requirements as unchanged between near and long-term baselines.
- **Location/region:** Long-term location-specific electricity/heat decarbonization affects SOEC lifecycle impacts; study notes SOEC can reduce MSP in some locations but may increase life-cycle GHGs when NG used for heat.
- **LCA / environmental:** Long-term baseline OPEX and lifetimes improved; lifecycle GHGs remain sensitive to heat source (NG vs renewable heat) and electricity supply emissions.
- **Other data:**
  - No CAPEX scaling applied to H2 electrolysis; long-term CAPEX reduced vs near-future baseline per table.

---

# MODEL: gemini-3.1-pro-preview

- **Status:** OK  |  **Words:** 821  |  **Duration:** 29183 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: Near future (by 2035)
- **Description:** Alkaline water electrolysis (AEC) dominating the market; available at MW-scale; operates at 60−90 °C. No scaling factor applied to H2 electrolysis CAPEX.
- **Inputs:** Electricity: 55.1 kWh_el/kg_H2 (baseline), 47.5 kWh_el/kg_H2 (min), 75.0 kWh_el/kg_H2 (max)
- **Outputs:** Hydrogen (H2): 1 kg_H2
- **CAPEX:** 790.5 €/kW (baseline) | 437.8 €/kW (min) | 1110.3 €/kW (max) | **OPEX:** 48.9% of system CAPEX (baseline) | 45.1% (min) | 54.0% (max)
- **Efficiency:** Electric efficiency: 55.1 kWh_el/kg_H2 (baseline) | 47.5 kWh_el/kg_H2 (min) | 75.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Available at MW-scale (commercial scale).
- **Lifetime:** Stack lifetime: 75,532 h (baseline) | 60,000 h (min) | 94,444 h (max)
- **Location:** Evaluated for Iceland, The Netherlands, Spain, Chile.
- **LCA / environmental impact:** Best option in terms of Cost of CO2 avoided (CCA) and most favorable for life-cycle GHG emissions in the near future.
- **Other data:** Exclusively uses renewable energy in the study.

### Year: Long-term future (post-2035)
- **Description:** Alkaline water electrolysis (AEC) well-established commercial technology; operates at 60−90 °C.
- **Inputs:** Electricity: 48.9 kWh_el/kg_H2 (baseline), 45.47 kWh_el/kg_H2 (min), 52.0 kWh_el/kg_H2 (max)
- **Outputs:** Hydrogen (H2): 1 kg_H2
- **CAPEX:** 527.2 €/kW (baseline) | 306.5 €/kW (min) | 774.5 €/kW (max) | **OPEX:** 48.8% of system CAPEX (baseline) | 46.5% (min) | 52.9% (max)
- **Efficiency:** Electric efficiency: 48.9 kWh_el/kg_H2 (baseline) | 45.47 kWh_el/kg_H2 (min) | 52.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Well-established technology.
- **Lifetime:** Stack lifetime: 87,500 h (baseline) | 80,000 h (min) | 100,000 h (max)
- **Location:** Evaluated for Iceland, The Netherlands, Spain, Chile.
- **LCA / environmental impact:** Lowest Cost of CO2 avoided (CCA) for most products.
- **Other data:** Exclusively uses renewable energy in the study.

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: Near future (by 2035)
- **Description:** Proton exchange membrane (PEM) electrolysis dominating the market; available at MW-scale; operates at 60−90 °C. No scaling factor applied to H2 electrolysis CAPEX.
- **Inputs:** Electricity: 57.9 kWh_el/kg_H2 (baseline), 48.8 kWh_el/kg_H2 (min), 83.0 kWh_el/kg_H2 (max)
- **Outputs:** Hydrogen (H2): 1 kg_H2
- **CAPEX:** 1047.9 €/kW (baseline) | 613.0 €/kW (min) | 1225.9 €/kW (max) | **OPEX:** 42.0% of system CAPEX (baseline) | 28.6% (min) | 60.0% (max)
- **Efficiency:** Electric efficiency: 57.9 kWh_el/kg_H2 (baseline) | 48.8 kWh_el/kg_H2 (min) | 83.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Available at MW-scale (commercial scale).
- **Lifetime:** Stack lifetime: 64,026 h (baseline) | 40,000 h (min) | 90,000 h (max)
- **Location:** Evaluated for Iceland, The Netherlands, Spain, Chile.
- **Other data:** Exclusively uses renewable energy in the study.

### Year: Long-term future (post-2035)
- **Description:** Proton exchange membrane (PEM) electrolysis well-established commercial technology; operates at 60−90 °C.
- **Inputs:** Electricity: 53.1 kWh_el/kg_H2 (baseline), 47.0 kWh_el/kg_H2 (min), 64.0 kWh_el/kg_H2 (max)
- **Outputs:** Hydrogen (H2): 1 kg_H2
- **CAPEX:** 473.5 €/kW (baseline) | 257.9 €/kW (min) | 700.5 €/kW (max) | **OPEX:** 39.4% of system CAPEX (baseline) | 27.8% (min) | 55.6% (max)
- **Efficiency:** Electric efficiency: 53.1 kWh_el/kg_H2 (baseline) | 47.0 kWh_el/kg_H2 (min) | 64.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Well-established technology.
- **Lifetime:** Stack lifetime: 85,420 h (baseline) | 50,000 h (min) | 100,000 h (max)
- **Location:** Evaluated for Iceland, The Netherlands, Spain, Chile.
- **Other data:** Differences in cost and performance compared to AEC are reduced in this timeframe.

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: Near future (by 2035)
- **Description:** Solid oxide electrolysis cells (SOEC); operates at 700−850 °C. Operation below thermoneutral conditions. High-temperature steam assumed to be generated from natural gas (NG). No scaling factor applied to H2 electrolysis CAPEX.
- **Inputs:** Electricity: 34.1 kWh_el/kg_H2 (baseline), 26.6 kWh_el/kg_H2 (min), 38.1 kWh_el/kg_H2 (max)
- **Inputs:** Thermal energy: 8.3 kWh_th/kg_H2 (baseline), 6.7 kWh_th/kg_H2 (min), 11.0 kWh_th/kg_H2 (max)
- **Outputs:** Hydrogen (H2): 1 kg_H2
- **CAPEX:** 1739.5 €/kW (baseline) | 593.0 €/kW (min) | 2770.0 €/kW (max) | **OPEX:** 27.0% of system CAPEX (baseline) | 23.5% (min) | 30.0% (max)
- **Efficiency:** Electric efficiency: 34.1 kWh_el/kg_H2 (baseline) | 26.6 kWh_el/kg_H2 (min) | 38.1 kWh_el/kg_H2 (max)
- **Lifetime:** Stack lifetime: 30,308 h (baseline) | 20,000 h (min) | 50,924 h (max)
- **Location:** Evaluated for Iceland, The Netherlands, Spain, Chile.
- **LCA / environmental impact:** Significantly higher life-cycle GHG emissions if natural gas is used compared to other electrolyzers (more than double life-cycle GHG emissions vs AEC and PEM). 

### Year: Long-term future (post-2035)
- **Description:** Solid oxide electrolysis cells (SOEC) well-established; operates at 700−850 °C. Operation below thermoneutral conditions. 
- **Inputs:** Electricity: 34.1 kWh_el/kg_H2 (baseline), 26.6 kWh_el/kg_H2 (min), 38.1 kWh_el/kg_H2 (max)
- **Inputs:** Thermal energy: 8.3 kWh_th/kg_H2 (baseline), 6.7 kWh_th/kg_H2 (min), 11.0 kWh_th/kg_H2 (max)
- **Outputs:** Hydrogen (H2): 1 kg_H2
- **CAPEX:** 958.3 €/kW (baseline) | 566.5 €/kW (min) | 1723.3 €/kW (max) | **OPEX:** 13.8% of system CAPEX (baseline) | 12.5% (min) | 15.0% (max)
- **Efficiency:** Electric efficiency: 34.1 kWh_el/kg_H2 (baseline) | 26.6 kWh_el/kg_H2 (min) | 38.1 kWh_el/kg_H2 (max)
- **TRL / maturity:** Well-established technology.
- **Lifetime:** Stack lifetime: 71,991 h (baseline) | 53,750 h (min) | 102,222 h (max)
- **Location:** Evaluated for Iceland, The Netherlands, Spain, Chile.
- **Other data:** Can reduce minimum selling price (MSP) by 42–218 €/t_product in The Netherlands; cost benefits in Chile are 3–11 €/t_product. For methane production, assuming 90% methane boiler efficiency, 1/3 of produced CCU methane (41.6 kt_CH4) can be reused to reduce SOEC life-cycle GHG emissions.

---

# MODEL: gemini-3.5-flash

- **Status:** OK  |  **Words:** 1181  |  **Duration:** 77331 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** Water electrolysis process operating at 60–90 °C using alkaline electrolyte.
- **Inputs:**
  - Electric energy: 55.1 kWh_el/kg_H2 (min: 47.5, max: 75.0)
  - Water: quantity not specified
- **Outputs:**
  - Hydrogen: 1 kg
  - Oxygen: quantity not specified
- **CAPEX:** 790.5 €/kW (min: 437.8, max: 1110.3) | **OPEX:** 48.9% of system CAPEX (min: 45.1%, max: 54.0%)
- **Efficiency:** 55.1 kWh_el/kg_H2 (min: 47.5, max: 75.0)
- **TRL / maturity:** Dominate market, available at MW-scale.
- **Lifetime / Scale:** Stack lifetime: 75,532 h (min: 60,000 h, max: 94,444 h); Plant operating hours: 8,000 h/y (except first year at 30% and second year at 70%); Plant investment lifetime: 25 years. Reference capacity: MW-scale. No scaling factor applied to H2 electrolysis.
- **Location:** Iceland, The Netherlands, Spain, Chile. Production in Iceland averages 45%, 29%, and 17% cheaper than The Netherlands, Spain, and Chile, respectively.
- **LCA / Environmental:** Most favorable option for GHG emissions. Most favorable option for cost of CO2 avoided (CCA) (e.g., fuel/methane/methanol/DME CCA is 543–1969 €/t_CO2eq,av; ammonia/urea is 203–1087 €/t_CO2eq,av).
- **Other Technical/Economic:** Renewable electricity cost decrease plus electrolyzer efficiency improvements account together for around a fifth of total minimum selling price (MSP) reduction potential.

### Year: 2050 (long-term future)
- **Description:** Water electrolysis process operating at 60–90 °C using alkaline electrolyte.
- **Inputs:**
  - Electric energy: 48.9 kWh_el/kg_H2 (min: 45.47, max: 52.0)
  - Water: quantity not specified
- **Outputs:**
  - Hydrogen: 1 kg
  - Oxygen: quantity not specified
- **CAPEX:** 527.2 €/kW (min: 306.5, max: 774.5) | **OPEX:** 48.8% of system CAPEX (min: 46.5%, max: 52.9%)
- **Efficiency:** 48.9 kWh_el/kg_H2 (min: 45.47, max: 52.0)
- **TRL / maturity:** Well-established technology. Dominate market, available at MW-scale.
- **Lifetime / Scale:** Stack lifetime: 87,500 h (min: 80,000 h, max: 100,000 h); Plant operating hours: 8,000 h/y; Plant investment lifetime: 25 years. Reference capacity: MW-scale. No scaling factor applied to H2 electrolysis.
- **Location:** Chile, Spain, Iceland, The Netherlands. Production in Chile is lowest cost (average 5% lower than Spain, 15% lower than Iceland, almost 45% lower than The Netherlands).
- **LCA / Environmental:** Most favorable option for GHG emissions. DME production in Chile causes 3.6 times higher life-cycle GHG emissions than in The Netherlands. Best option for cost of CO2 avoided (CCA) (e.g., fuel/methane/methanol/DME CCA is 225–537 €/t_CO2eq,av; ammonia/urea is 110–266 €/t_CO2eq,av; aromatics/olefins is 395–1730 €/t_CO2eq,av).
- **Other Technical/Economic:** Choice of electrolyzer has minor economic impacts compared to location and electricity source.

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** Water electrolysis process operating at 60–90 °C using proton exchange membrane.
- **Inputs:**
  - Electric energy: 57.9 kWh_el/kg_H2 (min: 48.8, max: 83.0)
  - Water: quantity not specified
- **Outputs:**
  - Hydrogen: 1 kg
  - Oxygen: quantity not specified
- **CAPEX:** 1047.9 €/kW (min: 613.0, max: 1225.9) | **OPEX:** 42.0% of system CAPEX (min: 28.6%, max: 60.0%)
- **Efficiency:** 57.9 kWh_el/kg_H2 (min: 48.8, max: 83.0)
- **TRL / maturity:** Dominate market, available at MW-scale.
- **Lifetime / Scale:** Stack lifetime: 64,026 h (min: 40,000 h, max: 90,000 h); Plant operating hours: 8,000 h/y (except first year at 30% and second year at 70%); Plant investment lifetime: 25 years. Reference capacity: MW-scale. No scaling factor applied to H2 electrolysis.
- **Location:** Iceland, The Netherlands, Spain, Chile. Production in Iceland averages 45%, 29%, and 17% cheaper than The Netherlands, Spain, and Chile, respectively.
- **LCA / Environmental:** Near future fuel/methane/methanol/DME CCA is 543–1969 €/t_CO2eq,av; ammonia/urea is 203–1087 €/t_CO2eq,av.
- **Other Technical/Economic:** Renewable electricity cost decrease plus electrolyzer efficiency improvements account together for around a fifth of total minimum selling price (MSP) reduction potential.

### Year: 2050 (long-term future)
- **Description:** Water electrolysis process operating at 60–90 °C using proton exchange membrane.
- **Inputs:**
  - Electric energy: 53.1 kWh_el/kg_H2 (min: 47.0, max: 64.0)
  - Water: quantity not specified
- **Outputs:**
  - Hydrogen: 1 kg
  - Oxygen: quantity not specified
- **CAPEX:** 473.5 €/kW (min: 257.9, max: 700.5) | **OPEX:** 39.4% of system CAPEX (min: 27.8%, max: 55.6%)
- **Efficiency:** 53.1 kWh_el/kg_H2 (min: 47.0, max: 64.0)
- **TRL / maturity:** Well-established technology. Dominate market, available at MW-scale.
- **Lifetime / Scale:** Stack lifetime: 85,420 h (min: 50,000 h, max: 100,000 h); Plant operating hours: 8,000 h/y; Plant investment lifetime: 25 years. Reference capacity: MW-scale. No scaling factor applied to H2 electrolysis.
- **Location:** Chile, Spain, Iceland, The Netherlands. Production in Chile is lowest cost (average 5% lower than Spain, 15% lower than Iceland, almost 45% lower than The Netherlands).
- **LCA / Environmental:** PEM-AEC differences in CCA are reduced long-term.
- **Other Technical/Economic:** Choice of electrolyzer has minor economic impacts compared to location and electricity source.

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** High-temperature water electrolysis process operating at 700–850 °C below thermoneutral conditions.
- **Inputs:**
  - Electric energy: 34.1 kWh_el/kg_H2 (min: 26.6, max: 38.1)
  - Thermal energy: 8.3 kWh_th/kg_H2 (min: 6.7, max: 11.0), assumed from natural gas-fed boilers or external heat sources
  - Water/Steam: quantity not specified
- **Outputs:**
  - Hydrogen: 1 kg
  - Oxygen: quantity not specified
- **CAPEX:** 1739.5 €/kW (min: 593.0, max: 2770.0) | **OPEX:** 27.0% of system CAPEX (min: 23.5%, max: 30.0%)
- **Efficiency:** Electric efficiency: 34.1 kWh_el/kg_H2 (min: 26.6, max: 38.1) | Thermal energy requirement: 8.3 kWh_th/kg_H2 (min: 6.7, max: 11.0)
- **TRL / maturity:** Emerging technology.
- **Lifetime / Scale:** Stack lifetime: 30,308 h (min: 20,000 h, max: 50,924 h); Plant operating hours: 8,000 h/y (except first year at 30% and second year at 70%); Plant investment lifetime: 25 years. No scaling factor applied to H2 electrolysis.
- **Location:** Iceland, The Netherlands, Spain, Chile. Near future production in Iceland averages 45%, 29%, and 17% cheaper than The Netherlands, Spain, and Chile, respectively. Near future olefins and aromatics with SOEC in Spain/Chile have high CCA of €5,062–26,739 per ton of CO2 eq,av.
- **LCA / Environmental:** Much higher life cycle GHG emissions if natural gas is used compared to other electrolyzers. Can result in more than double life cycle GHG emissions vs AEC and PEM. For methane production, around a third (41.6 kt_CH4) of produced CCU methane is reused to reduce SOEC life cycle GHG emissions (assuming 90% methane boiler efficiency, production capacity 124.6 kt_CH4/y, and lower heating value of 49.9 MJ/kg_CH4).
- **Other Technical/Economic:** Requires external high-temperature steam.

### Year: 2050 (long-term future)
- **Description:** High-temperature water electrolysis process operating at 700–850 °C below thermoneutral conditions.
- **Inputs:**
  - Electric energy: 34.1 kWh_el/kg_H2 (min: 26.6, max: 38.1)
  - Thermal energy: 8.3 kWh_th/kg_H2 (min: 6.7, max: 11.0)
  - Water/Steam: quantity not specified
- **Outputs:**
  - Hydrogen: 1 kg
  - Oxygen: quantity not specified
- **CAPEX:** 958.3 €/kW (min: 566.5, max: 1723.3) | **OPEX:** 13.8% of system CAPEX (min: 12.5%, max: 15.0%)
- **Efficiency:** Electric efficiency: 34.1 kWh_el/kg_H2 (min: 26.6, max: 38.1) | Thermal energy requirement: 8.3 kWh_th/kg_H2 (min: 6.7, max: 11.0)
- **TRL / maturity:** Emerging technology. Well-established (post-2035 timeframe).
- **Lifetime / Scale:** Stack lifetime: 71,991 h (min: 53,750 h, max: 102,222 h); Plant operating hours: 8,000 h/y; Plant investment lifetime: 25 years. No scaling factor applied to H2 electrolysis.
- **Location:** Chile, Spain, Iceland, The Netherlands. Can reduce minimum selling price (MSP) of products by 42–218 €/t_product in The Netherlands, and 3–11 €/t_product in Chile.
- **LCA / Environmental:** Much higher life cycle GHG emissions if natural gas is used compared to other electrolyzers. Can result in more than double life cycle GHG emissions vs AEC and PEM.
- **Other Technical/Economic:** Choice of electrolyzer has minor economic impacts compared to location and electricity source. Requires external high-temperature steam.

---

# MODEL: claude-opus-4.8-fast

- **Status:** OK  |  **Words:** 674  |  **Duration:** 10437 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: Near future (by 2035)
- **Description:** Alkaline water electrolysis; dominates market alongside PEM; available at MW-scale; operates at 60–90 °C. Uses renewable electricity exclusively. Plant operation 8000 h/y; investment lifetime 25 years.
- **Inputs:** Electricity: 55.1 kWh_el/kg_H2 (min 47.5, max 75.0); Water (feedstock, implied)
- **Outputs:** Hydrogen: 1 kg_H2 per 55.1 kWh_el; Oxygen (byproduct, implied)
- **CAPEX:** 790.5 €/kW system (min 437.8, max 1110.3) | **OPEX:** 48.9% of system cost (stack) (min 45.1, max 54.0)
- **Efficiency:** 55.1 kWh_el/kg_H2 (electric; min 47.5, max 75.0)
- **TRL / maturity:** Commercially available at MW-scale; market-dominant
- **Lifetime (stack):** 75,532 h (min 60,000, max 94,444)
- **Location/region:** Iceland, The Netherlands, Spain, Chile (renewable electricity: geothermal, offshore wind, PV)
- **Other:** No CAPEX scaling applied to H2 electrolysis; AEC most favorable electrolyzer for life-cycle GHG emissions and cost per ton CO2 avoided (CCA)

### Year: Long-term future (post-2035, by 2050)
- **Description:** Well-established technology built after 2035; operates 60–90 °C; renewable electricity only.
- **Inputs:** Electricity: 48.9 kWh_el/kg_H2 (min 45.47, max 52.0)
- **Outputs:** Hydrogen: 1 kg_H2; Oxygen (byproduct, implied)
- **CAPEX:** 527.2 €/kW system (min 306.5, max 774.5) | **OPEX:** 48.8% of system cost (stack) (min 46.5, max 52.9)
- **Efficiency:** 48.9 kWh_el/kg_H2 (electric; min 45.47, max 52.0)
- **Lifetime (stack):** 87,500 h (min 80,000, max 100,000)
- **Other:** Values inter-/extrapolated to 2035 where needed; choice of electrolyzer has minor economic impact vs location/electricity source

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: Near future (by 2035)
- **Description:** Proton exchange membrane electrolysis; dominates market alongside AEC; available at MW-scale; operates at 60–90 °C; renewable electricity exclusively. Plant operation 8000 h/y; investment lifetime 25 years.
- **Inputs:** Electricity: 57.9 kWh_el/kg_H2 (min 48.8, max 83.0)
- **Outputs:** Hydrogen: 1 kg_H2; Oxygen (byproduct, implied)
- **CAPEX:** 1047.9 €/kW system (min 613.0, max 1225.9) | **OPEX:** 42.0% of system cost (stack) (min 28.6, max 60.0)
- **Efficiency:** 57.9 kWh_el/kg_H2 (electric; min 48.8, max 83.0)
- **TRL / maturity:** Commercially available at MW-scale; market-dominant
- **Lifetime (stack):** 64,026 h (min 40,000, max 90,000)
- **Location/region:** Iceland, The Netherlands, Spain, Chile
- **Other:** PEM–AEC cost differences reduced in long-term future

### Year: Long-term future (post-2035, by 2050)
- **Description:** Well-established technology; operates 60–90 °C; renewable electricity only.
- **Inputs:** Electricity: 53.1 kWh_el/kg_H2 (min 47.0, max 64.0)
- **Outputs:** Hydrogen: 1 kg_H2; Oxygen (byproduct, implied)
- **CAPEX:** 473.5 €/kW system (min 257.9, max 700.5) | **OPEX:** 39.4% of system cost (stack) (min 27.8, max 55.6)
- **Efficiency:** 53.1 kWh_el/kg_H2 (electric; min 47.0, max 64.0)
- **Lifetime (stack):** 85,420 h (min 50,000, max 100,000)

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: Near future (by 2035)
- **Description:** Solid oxide electrolysis cells; operates at 700–850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas (NG). Renewable electricity for electrical demand. Plant operation 8000 h/y; investment lifetime 25 years.
- **Inputs:** Electricity: 34.1 kWh_el/kg_H2 (min 26.6, max 38.1); Thermal energy: 8.3 kWh_th/kg_H2 (min 6.7, max 11.0); HT steam (from NG); Water (feedstock, implied)
- **Outputs:** Hydrogen: 1 kg_H2; Oxygen (byproduct, implied)
- **CAPEX:** 1739.5 €/kW system (min 593.0, max 2770.0) | **OPEX:** 27.0% of system cost (stack) (min 23.5, max 30.0)
- **Efficiency:** Electric 34.1 kWh_el/kg_H2 (min 26.6, max 38.1); Thermal 8.3 kWh_th/kg_H2 (min 6.7, max 11.0)
- **TRL / maturity:** Less mature; higher CAPEX; not market-dominant
- **Lifetime (stack):** 30,308 h (min 20,000, max 50,924)
- **Location/region:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental:** Much higher life-cycle GHG emissions if NG used for HT steam; can result in more than double life-cycle GHG emissions vs AEC and PEM. For methane production (90% boiler efficiency, capacity 124.6 kt_CH4/y, LHV 49.9 MJ/kg_CH4), ~41.6 kt_CH4/y (~1/3) of CCU methane reused to reduce SOEC life-cycle GHG emissions.
- **Other:** SOEC can reduce MSP by 42–218 €/t_product in The Netherlands; 3–11 €/t_product in Chile

### Year: Long-term future (post-2035, by 2050)
- **Description:** Well-established technology; operates 700–850 °C; below thermoneutral; HT steam from NG. Energy requirements assumed same as near future.
- **Inputs:** Electricity: 34.1 kWh_el/kg_H2 (min 26.6, max 38.1, same as near future); Thermal energy: 8.3 kWh_th/kg_H2 (min 6.7, max 11.0, same as near future)
- **Outputs:** Hydrogen: 1 kg_H2; Oxygen (byproduct, implied)
- **CAPEX:** 958.3 €/kW system (min 566.5, max 1723.3) | **OPEX:** 13.8% of system cost (stack) (min 12.5, max 15.0)
- **Efficiency:** Electric 34.1 kWh_el/kg_H2; Thermal 8.3 kWh_th/kg_H2 (same as near future)
- **Lifetime (stack):** 71,991 h (min 53,750, max 102,222)

---

# MODEL: claude-sonnet-5

- **Status:** OK  |  **Words:** 917  |  **Duration:** 32355 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: Near future (by 2035)
- **Description:** AEC operates at 60−90 °C; dominates market alongside PEM; available at MW-scale; hydrogen produced exclusively using renewable electricity.
- **Inputs:**
  - Electricity: 55.1 kWh_el/kg_H2 (baseline); range 47.5–75.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen (H2)
- **CAPEX:** 790.5 €/kW (system, baseline); range 437.8–1110.3 €/kW
- **OPEX:** 48.9% of system cost (stack, baseline); range 45.1–54.0%
- **Efficiency:** 55.1 kWh_el/kg_H2 (baseline); range 47.5–75.0 kWh_el/kg_H2
- **TRL / maturity:** Mature, commercially dominant technology available at MW-scale alongside PEM
- **Lifetime (stack):** 75,532 h (baseline); range 60,000–94,444 h
- **Reference scale:** Plant sized for 360 kt CO2/y input; H2 supplied to downstream chemical processes (e.g., FT fuels, methanol, ammonia, etc.)
- **Location:** Applied across all four study locations (Iceland, The Netherlands, Spain, Chile)
- **LCA/GHG:** AEC identified as most favorable electrolyzer for life-cycle GHG emissions among the three technologies (near future)
- **Other:** Best option in terms of cost of CO2 avoided (CCA) for both near- and long-term scenarios; near future averaged 144.5 kg CO2eq/t_product more emissions from electricity vs long-term future

### Year: Long-term future (post-2035, by 2050)
- **Description:** Same operating conditions (60−90 °C); well-established technology assumed by this horizon
- **Inputs:**
  - Electricity: 48.9 kWh_el/kg_H2 (baseline); range 45.47–52.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen (H2)
- **CAPEX:** 527.2 €/kW (system, baseline); range 306.5–774.5 €/kW
- **OPEX:** 48.8% of system cost (stack, baseline); range 46.5–52.9%
- **Efficiency:** 48.9 kWh_el/kg_H2 (baseline); range 45.47–52.0 kWh_el/kg_H2
- **TRL / maturity:** Well-established/commercial
- **Lifetime (stack):** 87,500 h (baseline); range 80,000–100,000 h
- **Reference scale:** Same 360 kt CO2/y basis
- **Location:** Iceland lowest MSP contributor among locations with AEC; Chile/Spain lowest long-term MSPs overall
- **LCA/GHG:** AEC most favorable electrolyzer for GHG emissions; life-cycle GHG for products 55–96% lower than fossil counterparts depending on product (see product tables); DME production in Chile with AEC causes 3.6× higher life-cycle GHG emissions than in The Netherlands (location effect, not technology)
- **Other:** SOEC could reduce MSP by 42–218 €/t_product vs AEC in The Netherlands; SOEC cost benefit vs AEC only 3–11 €/t_product in Chile — implying AEC remains close to cost-optimal; discount rate sensitivity 4–8% (baseline 6%)

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: Near future (by 2035)
- **Description:** PEM operates at 60−90 °C; dominates market alongside AEC; available at MW-scale
- **Inputs:**
  - Electricity: 57.9 kWh_el/kg_H2 (baseline); range 48.8–83.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen (H2)
- **CAPEX:** 1047.9 €/kW (system, baseline); range 613.0–1225.9 €/kW
- **OPEX:** 42.0% of system cost (stack, baseline); range 28.6–60.0%
- **Efficiency:** 57.9 kWh_el/kg_H2 (baseline); range 48.8–83.0 kWh_el/kg_H2
- **TRL / maturity:** Mature, commercially available alongside AEC at MW-scale
- **Lifetime (stack):** 64,026 h (baseline); range 40,000–90,000 h
- **Reference scale:** Plant sized for 360 kt CO2/y input basis
- **Location:** Applied across all four study locations
- **Other:** PEM–AEC economic differences narrow in the long-term future scenario

### Year: Long-term future (post-2035, by 2050)
- **Description:** Same operating conditions (60−90 °C); well-established by this horizon
- **Inputs:**
  - Electricity: 53.1 kWh_el/kg_H2 (baseline); range 47.0–64.0 kWh_el/kg_H2
- **Outputs:**
  - Hydrogen (H2)
- **CAPEX:** 473.5 €/kW (system, baseline); range 257.9–700.5 €/kW
- **OPEX:** 39.4% of system cost (stack, baseline); range 27.8–55.6%
- **Efficiency:** 53.1 kWh_el/kg_H2 (baseline); range 47.0–64.0 kWh_el/kg_H2
- **TRL / maturity:** Well-established/commercial
- **Lifetime (stack):** 85,420 h (baseline); range 50,000–100,000 h
- **Reference scale:** Same 360 kt CO2/y basis
- **Location:** Applied across Iceland, The Netherlands, Spain, Chile
- **LCA/GHG:** Life-cycle GHG emission reductions vs fossil counterparts (product-dependent, same ranges as AEC/SOEC scenario set): e.g., methanol 65–91%, ammonia 72–96%, FT fuels (RWGS) 55–88%
- **Other:** PEM–AEC cost gap reduced in long-term future; values inter-/extrapolated to 2035 baseline dataset per Table 6

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: Near future (by 2035)
- **Description:** SOEC operates at 700−850 °C, below thermoneutral conditions; high-temperature (HT) steam input assumed supplied from natural gas (NG)-fed boilers
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2
  - Thermal energy: 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2
- **Outputs:**
  - Hydrogen (H2)
- **CAPEX:** 1739.5 €/kW (system, baseline); range 593.0–2770.0 €/kW
- **OPEX:** 27.0% of system cost (stack, baseline); range 23.5–30.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 electric + 8.3 kWh_th/kg_H2 thermal (baseline)
- **TRL / maturity:** Less mature than AEC/PEM; not yet at MW-scale market dominance (AEC and PEM noted as the dominant, MW-scale technologies)
- **Lifetime (stack):** 30,308 h (baseline); range 20,000–50,924 h
- **Reference scale:** 360 kt CO2/y basis; example downstream case: methane production capacity 124.6 kt CH4/y (90% boiler efficiency, LHV 49.9 MJ/kg CH4), ~41.6 kt CH4/y (about one-third) reused internally to offset SOEC life-cycle GHG emissions
- **Location:** Applied across Iceland, The Netherlands, Spain, Chile
- **LCA/GHG:** SOEC can result in more than double the life-cycle GHG emissions vs AEC and PEM, especially when NG is used for HT steam supply
- **Other:** Highest CAPEX among the three electrolyzer technologies; economic differences from electrolyzer choice minor vs location/electricity source impact

### Year: Long-term future (post-2035, by 2050)
- **Description:** Same operating conditions (700−850 °C); energy requirements assumed unchanged from near future
- **Inputs:**
  - Electricity: 34.1 kWh_el/kg_H2 (same as near future baseline)
  - Thermal energy: 8.3 kWh_th/kg_H2 (same as near future baseline)
- **Outputs:**
  - Hydrogen (H2)
- **CAPEX:** 958.3 €/kW (system, baseline); range 566.5–1723.3 €/kW
- **OPEX:** 13.8% of system cost (stack, baseline); range 12.5–15.0%
- **Efficiency:** 34.1 kWh_el/kg_H2 + 8.3 kWh_th/kg_H2 (unchanged from near future per Table 6 note)
- **TRL / maturity:** Assumed technologically mature/well-established by long-term horizon per study framework
- **Lifetime (stack):** 71,991 h (baseline); range 53,750–102,222 h
- **Reference scale:** 360 kt CO2/y basis
- **Location:** Applied across Iceland, The Netherlands, Spain, Chile
- **LCA/GHG:** SOEC can reduce MSP by 42–218 €/t_product in The Netherlands vs other electrolyzers, but SOEC's life-cycle GHG emissions much higher if NG used for steam; life-cycle GHG reductions vs fossil counterparts still product-dependent (same ranges as overall product set, e.g., 50–96% depending on product)
- **Other:** SOEC cost benefit vs AEC only 3–11 €/t_product in Chile; large GHG penalty relative to AEC/PEM tied to natural-gas-derived heat supply assumption

---

# MODEL: kimi-k2.7-code

- **Status:** OK  |  **Words:** 1092  |  **Duration:** 134184 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** Alkaline water electrolysis (AEC); operates at 60–90 °C; commercial MW-scale technology
- **Inputs:** electricity: 55.1 kWh_el/kg_H2 (baseline), 47.5 kWh_el/kg_H2 (min), 75.0 kWh_el/kg_H2 (max); water: not quantified per kg_H2 in source
- **Outputs:** hydrogen: 1 kg_H2 (reference basis)
- **CAPEX:** 790.5 €/kW (baseline), 437.8 €/kW (min), 1110.3 €/kW (max)
- **OPEX:** stack replacement 48.9% of system CAPEX (baseline), 45.1% (min), 54.0% (max)
- **Efficiency:** 55.1 kWh_el/kg_H2 (baseline), 47.5 kWh_el/kg_H2 (min), 75.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Commercial; available at MW-scale
- **Lifetime:** stack lifetime 75,532 h (baseline), 60,000 h (min), 94,444 h (max); plant operation 8000 h/y
- **Reference capacity / scale:** MW-scale; no scaling factor applied to electrolysis in CAPEX calculations
- **Year / time horizon:** near future (by 2035); values inter- or extrapolated to 2035 where needed
- **Location / region:** Iceland, The Netherlands, Spain, Chile; electricity source by location: Iceland geothermal, Netherlands offshore wind, Spain/Chile PV
- **LCA / environmental impact:** life-cycle GHG emissions depend on electricity source; near-future baseline emission factors: geothermal 17.0 kgCO2eq/MWhel, offshore wind 12.6 kgCO2eq/MWhel, PV 66.0 kgCO2eq/MWhel
- **Other technical / economic data:** monetary values in 2020 euros; investment lifetime 25 years; debt-to-equity ratio 1.5; investment allocation 30% two years before production, 50% one year before production, 20% in first production year; plant ramp-up first year 30%, second year 70% of total capacity

### Year: post-2035 / by 2050 (long-term future)
- **Description:** same technology and operating conditions as near future
- **Inputs:** electricity: 48.9 kWh_el/kg_H2 (baseline), 45.47 kWh_el/kg_H2 (min), 52.0 kWh_el/kg_H2 (max); water: not quantified per kg_H2 in source
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 527.2 €/kW (baseline), 306.5 €/kW (min), 774.5 €/kW (max)
- **OPEX:** stack replacement 48.8% of system CAPEX (baseline), 46.5% (min), 52.9% (max)
- **Efficiency:** 48.9 kWh_el/kg_H2 (baseline), 45.47 kWh_el/kg_H2 (min), 52.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Well-established commercial technology
- **Lifetime:** stack lifetime 87,500 h (baseline), 80,000 h (min), 100,000 h (max)
- **Year / time horizon:** long-term future (post-2035 but by 2050)
- **Location / region:** same locations; long-term future electricity costs: offshore wind 40.0 €/MWhel, Spain PV 18.1 €/MWhel, Chile PV 13.7 €/MWhel; emission factors: offshore wind 11.3 kgCO2eq/MWhel, PV 57.2 kgCO2eq/MWhel

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** Proton exchange membrane electrolysis (PEM); operates at 60–90 °C; commercial MW-scale technology
- **Inputs:** electricity: 57.9 kWh_el/kg_H2 (baseline), 48.8 kWh_el/kg_H2 (min), 83.0 kWh_el/kg_H2 (max); water: not quantified per kg_H2 in source
- **Outputs:** hydrogen: 1 kg_H2 (reference basis)
- **CAPEX:** 1047.9 €/kW (baseline), 613.0 €/kW (min), 1225.9 €/kW (max)
- **OPEX:** stack replacement 42.0% of system CAPEX (baseline), 28.6% (min), 60.0% (max)
- **Efficiency:** 57.9 kWh_el/kg_H2 (baseline), 48.8 kWh_el/kg_H2 (min), 83.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Commercial; available at MW-scale
- **Lifetime:** stack lifetime 64,026 h (baseline), 40,000 h (min), 90,000 h (max); plant operation 8000 h/y
- **Reference capacity / scale:** MW-scale; no scaling factor applied to electrolysis
- **Year / time horizon:** near future (by 2035); values inter- or extrapolated to 2035 where needed
- **Location / region:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental impact:** life-cycle GHG emissions depend on electricity source; same location-specific electricity emission factors as AEC
- **Other technical / economic data:** monetary values in 2020 euros; investment lifetime 25 years; debt-to-equity ratio 1.5; investment allocation 30% two years before production, 50% one year before production, 20% in first production year; plant ramp-up first year 30%, second year 70%

### Year: post-2035 / by 2050 (long-term future)
- **Description:** same technology and operating conditions as near future
- **Inputs:** electricity: 53.1 kWh_el/kg_H2 (baseline), 47.0 kWh_el/kg_H2 (min), 64.0 kWh_el/kg_H2 (max); water: not quantified per kg_H2 in source
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 473.5 €/kW (baseline), 257.9 €/kW (min), 700.5 €/kW (max)
- **OPEX:** stack replacement 39.4% of system CAPEX (baseline), 27.8% (min), 55.6% (max)
- **Efficiency:** 53.1 kWh_el/kg_H2 (baseline), 47.0 kWh_el/kg_H2 (min), 64.0 kWh_el/kg_H2 (max)
- **TRL / maturity:** Well-established commercial technology
- **Lifetime:** stack lifetime 85,420 h (baseline), 50,000 h (min), 100,000 h (max)
- **Year / time horizon:** long-term future (post-2035 but by 2050)
- **Location / region:** Iceland, The Netherlands, Spain, Chile

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** Solid oxide electrolysis (SOEC); operates at 700–850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas (NG)
- **Inputs:** electricity: 34.1 kWh_el/kg_H2 (baseline), 26.6 kWh_el/kg_H2 (min), 38.1 kWh_el/kg_H2 (max); thermal energy: 8.3 kWh_th/kg_H2 (baseline), 6.7 kWh_th/kg_H2 (min), 11.0 kWh_th/kg_H2 (max); water/steam: not quantified per kg_H2 in source; natural gas for high-temperature steam
- **Outputs:** hydrogen: 1 kg_H2 (reference basis)
- **CAPEX:** 1739.5 €/kW (baseline), 593.0 €/kW (min), 2770.0 €/kW (max)
- **OPEX:** stack replacement 27.0% of system CAPEX (baseline), 23.5% (min), 30.0% (max)
- **Efficiency:** electric: 34.1 kWh_el/kg_H2 (baseline), 26.6 kWh_el/kg_H2 (min), 38.1 kWh_el/kg_H2 (max); thermal: 8.3 kWh_th/kg_H2 (baseline), 6.7 kWh_th/kg_H2 (min), 11.0 kWh_th/kg_H2 (max)
- **TRL / maturity:** Early commercial / developing
- **Lifetime:** stack lifetime 30,308 h (baseline), 20,000 h (min), 50,924 h (max); plant operation 8000 h/y
- **Reference capacity / scale:** no scaling factor applied to electrolysis
- **Year / time horizon:** near future (by 2035); values inter- or extrapolated to 2035 where needed
- **Location / region:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental impact:** life-cycle GHG emissions strongly depend on natural gas use for high-temperature steam; source notes high GHG emissions when natural gas is used
- **Other technical / economic data:** reported MSP reduction when using SOEC: 42–218 €/t_product in The Netherlands, 3–11 €/t_product in Chile; for methane production with SOEC, assuming 90% methane boiler efficiency, production capacity 124.6 kt_CH4/y, LHV 49.9 MJ/kg_CH4, around a third (41.6 kt_CH4) of produced CCU methane reused to reduce life-cycle GHG emissions; monetary values in 2020 euros; investment lifetime 25 years; debt-to-equity ratio 1.5; investment allocation 30% two years before production, 50% one year before production, 20% in first production year; plant ramp-up first year 30%, second year 70%

### Year: post-2035 / by 2050 (long-term future)
- **Description:** same technology and operating conditions as near future; high-temperature steam assumed from natural gas
- **Inputs:** electricity: 34.1 kWh_el/kg_H2 (baseline), 26.6 kWh_el/kg_H2 (min), 38.1 kWh_el/kg_H2 (max); thermal energy: 8.3 kWh_th/kg_H2 (baseline), 6.7 kWh_th/kg_H2 (min), 11.0 kWh_th/kg_H2 (max); water/steam: not quantified per kg_H2 in source; natural gas for high-temperature steam
- **Outputs:** hydrogen: 1 kg_H2
- **CAPEX:** 958.3 €/kW (baseline), 566.5 €/kW (min), 1723.3 €/kW (max)
- **OPEX:** stack replacement 13.8% of system CAPEX (baseline), 12.5% (min), 15.0% (max)
- **Efficiency:** electric: 34.1 kWh_el/kg_H2 (baseline), 26.6 kWh_el/kg_H2 (min), 38.1 kWh_el/kg_H2 (max); thermal: 8.3 kWh_th/kg_H2 (baseline), 6.7 kWh_th/kg_H2 (min), 11.0 kWh_th/kg_H2 (max)
- **TRL / maturity:** Well-established technology
- **Lifetime:** stack lifetime 71,991 h (baseline), 53,750 h (min), 102,222 h (max)
- **Year / time horizon:** long-term future (post-2035 but by 2050); SOEC long-term future energy requirements assumed same as near future
- **Location / region:** Iceland, The Netherlands, Spain, Chile
- **LCA / environmental impact:** same SOEC-specific GHG considerations as near future; life-cycle GHG emissions strongly depend on natural gas use for high-temperature steam

---

# MODEL: mai-code-1-flash-picker

- **Status:** OK  |  **Words:** 1114  |  **Duration:** 53262 ms

## TECHNOLOGY 1: Alkaline water electrolysis

### Year: 2035 (near future)
- **Description:** Alkaline water electrolysis (AEC); water electrolysis process; operating temperature 60–90 °C; hydrogen produced using renewable electricity; study assumes electrolysis exclusively powered by renewable energy.
- **Inputs:** electricity: 55.1 kWh_el/kg_H2 (baseline); range 47.5–75.0 kWh_el/kg_H2; no other material or consumable quantities stated in the source table.
- **Outputs:** hydrogen: 1 kg_H2 (basis for efficiency values); no byproduct quantities explicitly stated.
- **CAPEX:** 790.5 €/kW system (baseline); range 437.8–1110.3 €/kW.
- **OPEX:** 48.9% of system (stack OPEX; baseline); range 45.1–54.0%.
- **Efficiency:** electric efficiency 55.1 kWh_el/kg_H2 (baseline); range 47.5–75.0 kWh_el/kg_H2.
- **TRL / maturity:** AEC and PEM dominate the market and are available at MW-scale; no explicit TRL value stated.
- **Lifetime / scale:** stack lifetime 75,532 h (baseline); range 60,000–94,444 h; scale: MW-scale availability.
- **Year / time horizon:** near future (by 2035); monetary values in 2020 euros.
- **Location / region:** study locations: Iceland, The Netherlands, Spain, Chile; Table 6 values are not location-specific.
- **LCA / environmental:** hydrogen production is assumed to use renewable energy; AEC was identified as favorable in the broader CCU product-level GHG assessment; no direct electrolyzer-specific GHG factor is provided.
- **Other:** operating temperature 60–90 °C; electrolysis is part of the overall CCU supply-chain model.

### Year: 2050 (long-term future)
- **Description:** Alkaline water electrolysis (AEC); same process basis as near future; operating temperature 60–90 °C; renewable-powered hydrogen production.
- **Inputs:** electricity: 48.9 kWh_el/kg_H2 (baseline); range 45.47–52.0 kWh_el/kg_H2; no other material or consumable quantities stated.
- **Outputs:** hydrogen: 1 kg_H2 (basis for efficiency values); no byproduct quantities explicitly stated.
- **CAPEX:** 527.2 €/kW system (baseline); range 306.5–774.5 €/kW.
- **OPEX:** 48.8% of system (stack OPEX; baseline); range 46.5–52.9%.
- **Efficiency:** electric efficiency 48.9 kWh_el/kg_H2 (baseline); range 45.47–52.0 kWh_el/kg_H2.
- **TRL / maturity:** no explicit TRL value stated; AEC remains a mature commercial option.
- **Lifetime / scale:** stack lifetime 87,500 h (baseline); range 80,000–100,000 h; scale: MW-scale availability.
- **Year / time horizon:** long-term future (post-2035, by 2050); monetary values in 2020 euros.
- **Location / region:** study locations: Iceland, The Netherlands, Spain, Chile; Table 6 values are not location-specific.
- **LCA / environmental:** no direct electrolyzer-specific LCA value provided; broader study indicates AEC is favorable for product-level GHG outcomes.
- **Other:** operating temperature 60–90 °C.

## TECHNOLOGY 2: Proton exchange membrane electrolysis

### Year: 2035 (near future)
- **Description:** Proton exchange membrane electrolysis (PEM); water electrolysis process; operating temperature 60–90 °C; hydrogen produced using renewable electricity.
- **Inputs:** electricity: 57.9 kWh_el/kg_H2 (baseline); range 48.8–83.0 kWh_el/kg_H2; no other material or consumable quantities stated in the source table.
- **Outputs:** hydrogen: 1 kg_H2 (basis for efficiency values); no byproduct quantities explicitly stated.
- **CAPEX:** 1047.9 €/kW system (baseline); range 613.0–1225.9 €/kW.
- **OPEX:** 42.0% of system (stack OPEX; baseline); range 28.6–60.0%.
- **Efficiency:** electric efficiency 57.9 kWh_el/kg_H2 (baseline); range 48.8–83.0 kWh_el/kg_H2.
- **TRL / maturity:** PEM is a commercial technology and dominates the market alongside AEC; no explicit TRL value stated.
- **Lifetime / scale:** stack lifetime 64,026 h (baseline); range 40,000–90,000 h; scale: MW-scale availability.
- **Year / time horizon:** near future (by 2035); monetary values in 2020 euros.
- **Location / region:** study locations: Iceland, The Netherlands, Spain, Chile; Table 6 values are not location-specific.
- **LCA / environmental:** hydrogen production is assumed to use renewable energy; no direct electrolyzer-specific GHG factor is provided.
- **Other:** operating temperature 60–90 °C.

### Year: 2050 (long-term future)
- **Description:** Proton exchange membrane electrolysis (PEM); same process basis as near future; operating temperature 60–90 °C; renewable-powered hydrogen production.
- **Inputs:** electricity: 53.1 kWh_el/kg_H2 (baseline); range 47.0–64.0 kWh_el/kg_H2; no other material or consumable quantities stated.
- **Outputs:** hydrogen: 1 kg_H2 (basis for efficiency values); no byproduct quantities explicitly stated.
- **CAPEX:** 473.5 €/kW system (baseline); range 257.9–700.5 €/kW.
- **OPEX:** 39.4% of system (stack OPEX; baseline); range 27.8–55.6%.
- **Efficiency:** electric efficiency 53.1 kWh_el/kg_H2 (baseline); range 47.0–64.0 kWh_el/kg_H2.
- **TRL / maturity:** no explicit TRL value stated; PEM remains a mature commercial electrolysis option.
- **Lifetime / scale:** stack lifetime 85,420 h (baseline); range 50,000–100,000 h; scale: MW-scale availability.
- **Year / time horizon:** long-term future (post-2035, by 2050); monetary values in 2020 euros.
- **Location / region:** study locations: Iceland, The Netherlands, Spain, Chile; Table 6 values are not location-specific.
- **LCA / environmental:** no direct electrolyzer-specific LCA value provided; broader study indicates PEM is among the options assessed for CCU products.
- **Other:** operating temperature 60–90 °C.

## TECHNOLOGY 3: Solid oxide electrolysis

### Year: 2035 (near future)
- **Description:** Solid oxide electrolysis (SOEC); high-temperature electrolysis; operating temperature 700–850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas in the study.
- **Inputs:** electricity: 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2; thermal energy: 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2.
- **Outputs:** hydrogen: 1 kg_H2 (basis for efficiency values); no byproduct quantities explicitly stated.
- **CAPEX:** 1739.5 €/kW system (baseline); range 593.0–2770.0 €/kW.
- **OPEX:** 27.0% of system (stack OPEX; baseline); range 23.5–30.0%.
- **Efficiency:** electric efficiency 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2; thermal energy 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2.
- **TRL / maturity:** no explicit TRL value stated; solid oxide electrolysis is treated as an electrolysis technology option in the study.
- **Lifetime / scale:** stack lifetime 30,308 h (baseline); range 20,000–50,924 h; no explicit reference capacity or scale value provided.
- **Year / time horizon:** near future (by 2035); monetary values in 2020 euros.
- **Location / region:** study locations: Iceland, The Netherlands, Spain, Chile; Table 6 values are not location-specific.
- **LCA / environmental:** SOEC can have much higher life-cycle GHG emissions than AEC and PEM if natural gas is used for heat; the source states SOEC can result in more than double the life-cycle GHG emissions versus AEC and PEM.
- **Other:** operating temperature 700–850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas.

### Year: 2050 (long-term future)
- **Description:** Solid oxide electrolysis (SOEC); high-temperature electrolysis; operating temperature 700–850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas in the study.
- **Inputs:** electricity: 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2; thermal energy: 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2; long-term future values are stated to be the same as near future.
- **Outputs:** hydrogen: 1 kg_H2 (basis for efficiency values); no byproduct quantities explicitly stated.
- **CAPEX:** 958.3 €/kW system (baseline); range 566.5–1723.3 €/kW.
- **OPEX:** 13.8% of system (stack OPEX; baseline); range 12.5–15.0%.
- **Efficiency:** electric efficiency 34.1 kWh_el/kg_H2 (baseline); range 26.6–38.1 kWh_el/kg_H2; thermal energy 8.3 kWh_th/kg_H2 (baseline); range 6.7–11.0 kWh_th/kg_H2.
- **TRL / maturity:** no explicit TRL value stated; SOEC is assessed as a long-term future electrolysis option.
- **Lifetime / scale:** stack lifetime 71,991 h (baseline); range 53,750–102,222 h; no explicit reference capacity or scale value provided.
- **Year / time horizon:** long-term future (post-2035, by 2050); monetary values in 2020 euros.
- **Location / region:** study locations: Iceland, The Netherlands, Spain, Chile; Table 6 values are not location-specific.
- **LCA / environmental:** no separate long-term future LCA value is given; the source states SOEC can result in more than double the life-cycle GHG emissions versus AEC and PEM when natural gas is used.
- **Other:** operating temperature 700–850 °C; operation below thermoneutral conditions; high-temperature steam assumed from natural gas.

---

