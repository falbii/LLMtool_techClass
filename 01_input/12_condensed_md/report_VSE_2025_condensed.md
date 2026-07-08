<!-- condensed from report_VSE_2025.pdf by claude-sonnet-4.6 on 2026-06-23 15:27:30 -->

[PAGE 1]

**VSE (2025): Energiezukunft 2050 – Resiliente Stromversorgung: Gesamtsystem fit machen für neue Realitäten (Update EZ2050 - Erläuterungsbericht).** Verband Schweizerischer Elektrizitätsunternehmen VSE, Aarau. 9. Januar 2025.

---

[PAGE 3]

**Struktur:** Einleitung → Energiesystem Modell → Szenarien (3.1 integriert, 3.2 isoliert) → Ergebnisse (Verbrauch, Produktion, Flexibilität, EU-Austausch, Netzausbau, Systemkosten) → Exkurse (NIMBY/Kernenergie, weniger Effizienz, Wasserstoff, kalte Dunkelflaute)

---

[PAGE 4]

- Schweiz: CO₂-Netto-Null-Ziel bis 2050
- Aktuell dominieren fossile Energieträger: Erdölprodukte (Benzin, Diesel, Heizöl), Erdgas
- Elektrifizierung von Mobilität, Wärme, Industrie erforderlich
- EZ2050-Originalstudie: Dezember 2022, VSE + Empa
- Verteilnetzstudie (mit ETH Zürich): publiziert August 2024
- Volksabstimmung «Mantelerlass» (sichere Stromversorgung): angenommen Juni 2024
- Update EZ2050: Stützjahre 2030, 2040, 2050; aktualisierte Netzkosten via vereinfachtem Netzmodell

---

[PAGE 6–7]

**Modell:**
- Software: oemof (Open Energy Modelling Framework), open-source
- Modellgerüst: Weiterentwicklung von «ehub-X» (Empa)
- Methode: MILP (mixed-integer linear programming), systemkostenminimal
- Stündliche Nachfrage nach Strom, Gase, H₂, Erdöl, Wärme
- Optimierungsgrösse: Gesamtsystemkosten = annualisierte Investitionen (overnight CAPEX) + fixe/variable OPEX (inkl. Energie- und CO₂-Kosten)
- Ansatz: «perfect-foresight» Snapshot-Modell
- Referenzjahr: REF (~2018); primäres Stützjahr: 2050; weitere Stützjahre: 2030, 2040; dazwischen: lineare Interpolation
- Bilanzschliessung: zwingend jede Stunde; Slack (fiktiver Lastabwurf) = **10'000 CHF/MWh** (VoLL)
- Nachbarländer (AT, DE, FR, IT): vereinfachte Modellierung Stromflüsse (Produktion, Verbrauch, grenzüberschreitender Handel)
- Stromnetz: Kupferplatte pro Netzebene NE1 (Übertragungsnetz) bis NE7 (Niederspannung inkl. Prosumer)
- WACC Produktion (erneuerbare Produktionsanlagen): **5%**
- WACC Netzausbau: **4%**

---

[PAGE 8–10]

**Szenarien**

- Stromgesetz («Mantelerlass»): Paket 1 in Kraft **1. Januar 2025**; Paket 2: **1. Januar 2026**
- Ausbauziele Erneuerbare (ohne Wasserkraft): **~10 TWh heute → 35 TWh bis 2035 → 45 TWh bis 2050**
- Technologien im Ausbau: PV auf Dächern, PV auf Infrastruktur/Freiflächen (inkl. alpine Anlagen), Wind, biogene Gase (inkl. Biogas), Biomasse (inkl. Abfall), Geothermie
- Wasserkraft: Ausbau gemäss «Runder Tisch» → **16 Projekte bis 2040** → **+2 TWh** zusätzliche Winterstromproduktion
- Nettoimporte Winterhalbjahr: max. **5 TWh**

**Szenario «integriert»** (Stromgesetz mit Stromabkommen):
- Schweiz in europäischem Energiemarkt (Strom + Wasserstoff), 70%-Regel
- NTC gemäss TYNDP 2024 (ENTSOE) bis 2030, angenommen konstant bis 2050
- Vollständiger Anschluss an «European Hydrogen Backbone» (EHB) ab **2040er Jahre**

**Szenario «isoliert»** (Stromgesetz ohne Stromabkommen):
- Grenzübertragungskapazitäten NTC einseitig stark eingeschränkt (Quelle: Frontier Economics, TU Graz, 2021)
- Kein (vollständiger) Anschluss an European Hydrogen Backbone (EHB)

**Tabelle 1: NTC ohne Stromabkommen («isoliert») — zeitgewichtete Durchschnittswerte [MW]**

| Werte [MW] | AT-CH (Import) | DE-CH (Import) | FR-CH (Import) | IT-CH (Import) | CH-AT (Export) | CH-DE (Export) | CH-FR (Export) | CH-IT (Export) |
|---|---|---|---|---|---|---|---|---|
| Offpeak Winter | 199 | 1203 | 847 | 307 | 915 | 976 | 698 | 10 |
| Peak Winter | 155 | 1264 | 647 | 178 | 848 | 926 | 892 | 1 |
| Offpeak Sommer | 303 | 1481 | 824 | 139 | 892 | 767 | 807 | 9 |
| Peak Sommer | 269 | 1435 | 677 | 78 | 809 | 776 | 698 | 7 |
| Durchschnitt | 237 | 1345 | 773 | 189 | 876 | 864 | 768 | 8 |

---

[PAGE 11–13]

**4.1.1 Gesamtenergieverbrauch**
- Bruttoenergie- und Endenergieverbrauch sinken bis 2050 deutlich durch Dekarbonisierung + Effizienzmassnahmen
- Importabhängigkeit: **~75% heute → unter 20% bis 2050**
- Verbrauchsbasis: EP2050+ Szenario «ZERO-BASIS»; Bevölkerungswachstum A-00-2020 (BFS, 2020); BIP-Wachstum BIP-A (Seco, 2022)
- Sektoren: Haushalte, Dienstleistung, Industrie, Landwirtschaft, Verkehr
- Verwendungszwecke: Raumwärme, Warmwasser, Prozesswärme, Beleuchtung, Klima, Lüftung/Haustechnik, I&K, Unterhaltungsmedien, Antriebe, Prozesse, Mobilität, Sonstige

**4.1.2 Stromverbrauch**
- Landesverbrauch: **~60 TWh heute → ~90 TWh bis 2050** (~+50%)
- Haupttreiber: Elektrifizierung Mobilität, Wärme/Kälte, Digitalisierung (Rechenzentren), Energieumwandlung (Elektrolyse, CCS, Grosswärmepumpen Fernwärme)
- Pro-Kopf-Stromverbrauch: sinkt weniger stark als im Stromgesetz vorgesehen (–5% bis 2050 ggü. 2000)
- Grund: stärkeres BIP-Wachstum (Seco 2022) + höhere Verbrauchsschätzung Rechenzentren
- Exogen (vorgegeben): Beleuchtung, Antriebe & Prozesse, Unterhaltung, Klima, Lüftung/Haustechnik, Verkehr non-Road, Prozesswärme (Kochen etc.), Rechenzentren, Verkehr Strasse
- Endogen (Modell): Wärme (Raumwärme, Warmwasser, industrielle Prozesswärme), Kälte, Umwandlung, Netzverluste

**Wärme:**
- Fossile (Erdöl, Erdgas, Kohle) werden durch Erneuerbare (inkl. Umweltwärme, Fernwärme) ersetzt
- Heutiger Stromverbrauch Elektroheizungen: **~3.5 TWh/Jahr**; Elektro-Warmwasserboiler: **~2 TWh/Jahr**
- Wärmepumpen ersetzen sowohl fossile Heizsysteme als auch ineffiziente Elektrowiderstandsheizungen/-boiler
- Strom + Wasserstoff für Hochtemperatur-Prozesswärme in Industrie (v.a. bei tiefen Strompreisen)
- Klimakorrektur Raumwärmebedarf: Szenario RCP 2.6, CH2018 (Meteo Schweiz, 2018), max. +2 °C

**Mobilität:**
- Stromverbrauch Mobilität: **~3 TWh/Jahr heute → ~20 TWh/Jahr bis 2050** (grösste Einzelzunahme)
- Wasserstoff: nur im Schwerverkehr (inkl. non-road: Schiffe, Baumaschinen)
- Internationaler Luft- und Schiffverkehr: nicht berücksichtigt
- Fahrzeugkilometer-Basis: «Verkehrsperspektiven 2050» Basisszenario (ARE, 2022)

---

[PAGE 14–15]

**4.2.1.1 Wasserkraft**
- «Runder Tisch»: **16 Projekte** (inkl. Chlus) bis 2040
- Massnahmen: Staudammerhöhungen (z.B. Grimsel), Neuerschliessungen (z.B. Trift)
- Wirkung: **+2 TWh** zusätzliche Winterstromproduktion; Verschiebung Sommerenergie → Winter

[PAGE 15]

**«Runder Tisch» 16 Projekte:** zusätzliche Wasserkraftproduktion +0.9 TWh; zusätzliche installierte Leistung ca. +0.2 GW. Kein weiterer Wasserkraftausbau außerhalb «Runden Tischs» angenommen. Stromgesetz-Referenz: Wasserkraft netto von heute ca. 35 TWh → 39.2 TWh/Jahr.

**4.2.1.2 Photovoltaik (PV)**
- Dach-PV Gestehungskosten: REF 120 CHF/MWh → 2050: 55 CHF/MWh
- Dach-PV Potential: 30–50 TWh/Jahr; max. angenommenes Potential: 40 TWh/Jahr
- PV-Profil: Pan-European-Climate-Database (PECD), historisches Jahr 2016; 1200 äq. Volllaststunden
- Fassaden-Potential: ~17 TWh/Jahr (nicht berücksichtigt)

[PAGE 16]

- Freiflächen-PV (besiedelte Gebiete/Infrastruktur): 33 CHF/MWh (2050); max. Potential 2.5 TWh/Jahr
- Alpine PV: 170 CHF/MWh (2050); max. Potential 2.2 TWh/Jahr

**4.2.1.3 Windenergie**
- Gesamtpotential Schweiz: 29.5 TWh/Jahr, davon 19 TWh im Winterhalbjahr
- Gestehungskosten Wind 2050: 55 CHF/MWh
- Max. Windpotential Szenario «Stromgesetz» 2050: 3.8 TWh/Jahr → ca. 2 GW, ca. 400 Windräder (Ø 5 MW)
- Variante «mehr Wind»: max. 30 TWh/Jahr
- PECD 2016-Profil: REF 1827 äq. Volllaststunden; 2050: 2000 äq. Volllaststunden
- Erneuerbare Ausbauziels Stromgesetz: 45 TWh/Jahr total bis 2050

**4.2.1.4 Andere Erneuerbare (Abfall, Geothermie, biogene Gase)**
- KVA (Kehrrichtverbrennungsanlagen): ca. 1.2 TWh/Jahr erneuerbarer Strom
- Erneuerbarer Abfallanteil: heute ca. 50% → 40%; Gesamtenergiegehalt Siedlungsabfall konstant bei ca. 12.5 TWh/Jahr bis 2050
- Biogene Gase (BHKW + Brennstoffzellen/WKK): 1.1 TWh/Jahr Stromproduktion
- Synthetisches Methan (SNG) Import: 164 CHF/MWh (nicht eingesetzt)
- Geothermie 2050: max. 0.2 TWh/Jahr

[PAGE 17]

**4.2.2 Saisonale Diskrepanz: Winterstromlücke / Sommerüberschüsse**
- Winterstromimporte gemäß Stromgesetz: max. netto 5 TWh

**4.2.2.1.1 Variante «Gaskraftwerke»**
- KKW-Abschaltungen: Beznau I 2033; Beznau II 2032; Gösgen voraussichtlich 2039; Leibstadt voraussichtlich 2044
- Ergänzende Produktion: Erdgas mit CO₂-Zertifikaten/erneuerbare Gase; fossiles Erdgas mit CCS; Wasserstoff
- Gestehungskosten CCGT mit CCS: ca. 80 CHF/MWh

[PAGE 18]

- CCGT-Annahmen (Fußnote): 90% CO₂-Abtrennung (Oxyfuel); 4000 äq. Volllaststunden; Brennstoffkosten 24 CHF/MWh_th; CAPEX (overnight) 1184 CHF/kW_el (= 1/3 teurer als ohne CCS); Amortisationszeit 23 Jahre; WACC 5%; fixe BUK 28 CHF/kW_el/Jahr; variable BUK 3.2 CHF/MWh
- CO₂-Transport & Speicherung (Ausland): 15–48 CHF/t CO₂

**4.2.2.1.2 Variante «Kernenergie LTO»**
- KKW Gösgen: Verlängerung um 20 Jahre bis 2059
- Zusatzinvestitionen Modernisierung/Nachrüstung: 2000 CHF/kW
- Fixe BUK: 200 CHF/kW/Jahr
- Stromgestehungskosten LTO: 65 CHF/MWh bei 7800 äq. Volllaststunden (vs. ca. 50 CHF/MWh im heutigen Betrieb)
- Referenz ETH-Studie (2023): Ø Kosten Laufzeitverlängerung CH-KKW (1 GW) um 10 Jahre = 1 Mrd. CHF
- Gasverbrauch mit LTO: ca. 50% weniger als ohne LTO

[PAGE 19]

**4.2.2.1.3 Variante «mehr Import»**
- Winterstromimport-Limit erhöht auf max. 10 TWh netto (vs. 5 TWh im Stromgesetz)
- Entspricht Elcom-Richtwert und Winterstromimportsaldo «EP2020+»

**4.2.2.1.4 Variante «mehr Wind»**
- Windausbau optimal: 24.7 TWh total, davon 15.1 TWh Winterhalbjahr
- PV-Ausbau reduziert auf: 17 TWh
- Ca. 2/3 der Windproduktion fällt im Winter an
- Ergänzende Gasproduktion in Variante «mehr Wind»: 2.7 TWh

[PAGE 20]

**4.2.2.2 Sommerüberschüsse**
- PV-Einspeisebegrenzung (peak shaving) gemäß Stromgesetz: max. 3% der PV-Jahresproduktion = ca. 1 TWh/Jahr
- Zusätzlicher Flexibilitätsbedarf Sommer: ca. 1.0–1.5 TWh

[PAGE 21]

**4.3 Flexibilität**

Flexibilitätsbedarf aus stündlicher Residuallast:

- **Tag/Nacht:** Kurzfristiger Bedarf bis zu 126 GWh/Tag (inkl. Einspeisebegrenzung) im Sommer; entspricht idealem Speicher ca. 63 GWh. Jährlich: 36 TWh (7× mehr als heute).

[PAGE 22]

- **Werktage/Wochenende:** Jährlich 9 TWh; 150–300 GWh/Woche.
- **Sommer/Winter:** Totale jährliche Residuallast Szenario «Stromgesetz mit Stromabkommen» (Variante «Gas»): 27 TWh (heute 18 TWh); davon 23 TWh durch saisonale Flexibilität abdeckbar (idealer saisonaler Speicher ~11.5 TWh); restliche 4 TWh müssen zusätzlich erzeugt werden.

**Speicheroptionen:**
- Pumpspeicher (NE1): heute ca. 200 GWh / 4 GW verfügbar
- Batterien (NE5 + NE7): zusätzlich 21 GWh / 7.5 GW bis 2050

[PAGE 23]

- Bestehende Hydro-Speicher: 8600 GWh; + «Runder Tisch» 16 Projekte: +2000 GWh → total zukünftig 10600 GWh saisonale Flexibilität

**Tabelle 2: Kurzfristige Speicheroptionen bis 2050** *(Pumpspeicher: reiner Umwälzbetrieb inkl. Ober-/Unterwasser-Limitierung; V2G/H-Potential nicht berücksichtigt)*

[PAGE 24]

**4.5 Netzausbau**
- Heutige jährliche Netzkosten: ca. 4.2 Mrd. CHF/Jahr
- Durch Erneuerungen, Erdverkabelungen und Neuerschliessungen allein: +127% → gut 5.3 Mrd. CHF/Jahr bis 2050

Netzannahmen (BFE Netzstudie 2022):
- Erdverkabelung Ersatz Freileitung NE7: 200'000 CHF/km
- «Erneuerung/Ersatz Bestandsnetz»: sukzessive Erneuerung nach 40 Jahren; angenommenes Durchschnittsalter heute 20 Jahre

[PAGE 25]

- «Erdverkabelung»: NE7 + NE5: 100% bis 2050; NE3: 33%; NE1: 5%
- «Neuerschliessungen»: Bevölkerungswachstum +18% (2020–2050, BFS 2020); davon 2/3 führen zu Neuerschliessungen, 1/3 durch Verdichtung ins Bestandsnetz integriert

[PAGE 26]

- Netzkosten heute: ca. 4.2 Mrd. CHF/Jahr → 2050: gut 5.3 Mrd. CHF/Jahr (Basis ohne Kapazitätserweiterungen, +127%)
- Kapazitätserweiterungen (Szenario-abhängig) in CHF/kW, kalibriert anhand CHF/km-Kosten (z.B. Freileitung NE7-Ersatz: 200'000 CHF/km); Elcom-Kalibrierung auf Basis Tätigkeitsbericht 2023

[PAGE 26]

- Netzkostenanstieg dämpfbar durch Einspeisebegrenzung (Peak Shaving) max. 3% der Jahresproduktion der PV-Anlagen.
- Statisch (fixe Begrenzung z.B. 70% der installierten Leistung): Netzkosten 2050 ca. **6.9 Mrd. CHF/Jahr** (Reduktion um **2 Mrd. CHF/Jahr** gegenüber ohne Begrenzung: **8.9 Mrd. CHF/Jahr**).
- Dynamisch (bedarfsgerecht): weitere Dämpfung um ca. **0.5 Mrd. CHF/Jahr**.
- Netzausbaukosten: 75'000 CHF/km für Kapazitätserweiterung NE7 (BFE Netzstudie 2022).

[PAGE 27]

Weitere Massnahmen zur Dämpfung der Netzkosten (nicht explizit modelliert, außer statische/dynamische PV-Einspeisebegrenzung und Heim-/Grossbatterien NE5):
- Ausrichtung PV-Anlagen für Winterstromproduktion
- Eigenverbrauchsoptimierung via Heimbatterien, Vehicle-to-Home/Vehicle-to-Grid, Grossbatterien
- Dynamische Tarifierung und Demand-Side-Management (DSM)
- Intelligente Netzsteuerung, Q(U)-Regelung, regelbare Ortsnetztransformatoren (rONT)
- Potenzielle Einsparung weiterer Netzausbaukosten: schätzungsweise **10–20%** (VSE-Verteilnetzstudie, 2024).

[PAGE 28]

**4.6 Systemkosten**

- Systemkosten = annualisierte Kapitalkosten + fixe/variable Betriebs- und Unterhaltskosten (inkl. Brenn-/Treibstoffkosten, CO2-Kosten) + Netzkosten.
- Nicht enthalten: Investitionen in Elektrofahrzeuge, Ladeinfrastruktur, Prozessanpassungen Industrie, Gebäudesanierungen, Subventionen, Steuern, Abgaben.
- Heutige Gesamtenergiesystemkosten: ca. **30 Mrd. CHF/Jahr** (BFE Gesamtenergiestatistik).
- CO2-Preisannahmen (aus TYNDP 2022 / WEO 2020 «Sustainable Development»): REF/2030/2040/2050 → **30/78/123/168 CHF/t CO2**.
- CO2-Emissionen fossiler Energieträger: Treibhausgasinventar BAFU (2022).
- Mit Stromabkommen: Systemkosten in allen Varianten (leicht) tiefer als ohne.
- Variante «mehr Wind»: tiefste Systemkosten; «Gas», «LTO», «mehr Import» unterscheiden sich wenig.

[PAGE 29]

Abbildung 17: Annualisierte Systemkosten 2050 für Szenarien «Stromgesetz mit Stromabkommen» (integriert) und «Stromgesetz ohne Stromabkommen» (isoliert), Varianten «Gas», «LTO», «mehr Import», «mehr Wind».

[PAGE 30–31]

**5.1 Szenario «NIMBY» (inkl. Neubau Kernenergie)**

- Erneuerbare 2050: **25 TWh** (vs. **45 TWh** mit Stromgesetz).
- Nur PV auf Dächern/Infrastrukturbauten; kein Windausbau, kein PV Freiflächen/alpin, kein Wasserkraftausbau.
- Variante «LTO»: Langzeitbetrieb KKW Gösgen (**1 GW**) und Leibstadt (**1.2 GW**).
- Variante «Neubau»: min. **300 MW** (SMR oder konventionell Gen. 3) bis 2050; Betriebszeit **60 Jahre**.
  - CAPEX Neubau: **8000 CHF/kW**; fixe O&M: **120 CHF/kW/Jahr**.
  - Volllaststunden: **7800 äq.**; WACC: **5%**
  - LCOE Neubau: **90 CHF/MWh**
  - LCOE LTO: **65 CHF/MWh**
  - LCOE bestehende Anlagen (regulärer Betrieb): ca. **50 CHF/MWh**
  - CAPEX-Vergleich neue KKW Mitteleuropa (ETH-Studie 2023): **7'600–12'600 CHF/kW**
  - PSI 2019 Schätzung LCOE neue KKW: **70–120 CHF/MWh** (BFE, 2024)
- Verbrauch 2050: **91 TWh/Jahr**; Winterimportrestriktionen: max. **5 TWh**; alle anderen Rahmendaten wie «Stromgesetz».
- Modellresultat: nur minimaler Kernenergie-Neubau von **300 MW** bis 2050 (wegen hoher Kosten gegenüber Alternativen).

**Tabelle 3: LCOE neue Kernkraftwerke [CHF/MWh] in Abhängigkeit CAPEX und WACC**

| CAPEX (CHF/kW) | WACC 3% | WACC 5% | WACC 8% |
|---|---|---|---|
| 5000 | 53 | 64 | 82 |
| 8000 | 73 | 90 | 118 |
| 12000 | 99 | 125 | 168 |

[PAGE 32]

- Szenario «NIMBY»: Bedarf ergänzende Winterproduktion verdoppelt sich.
- Systemkosten «NIMBY» mit Stromabkommen tiefer als ohne; tiefer als «Stromgesetz» mit «Gas» (wegen geringerer Netzausbaukosten/PV-Investitionen), aber nicht gegenüber «Stromgesetz» mit «mehr Wind».
- «NIMBY» mit «LTO»: etwas günstiger; «Neubau» kein Kostenvorteil bei Systemkosten.

**5.2 Verbrauch: weniger Effizienz**

- Szenario «Stromgesetz ohne Effizienzmassnahmen» basiert auf EP2050+ Szenario «Weiter-wie-bisher».
- Landesstromverbrauch 2050 «ineffizient»: +**7 TWh** (+**8%**) gegenüber «effizient» → fast **100 TWh/Jahr**.
- Systemkosten «ineffizient» 2050: rund +**20%** höher.

[PAGE 34–35]

**5.3 Alternative Energieträger: Wasserstoff**

- H2 2050 verwendet primär in: Mobilität (Schwerverkehr), (WKK-)Brennstoffzellen, Hochtemperatur-Prozesswärme Industrie.
- Inländische H2-Produktion: Sommer aus Stromüberschüssen bei grossen Laufwasserkraftwerken (>10 MW); **4400 Volllaststunden**.
- Winter: H2-Import (saisonale H2-Speicherung in Kavernen zu teuer).
- Importpreis grüner H2 (franco Landesgrenze) 2050: **126 CHF/MWhLHV** = **4.2 CHF/kg H2**.
- Herkunft: 2030/2040 EU (Pipeline); 2050 MENA (z.B. Oman/Nordafrika, Pipeline inkl. European Hydrogen Backbone EHB).

**Tabelle 4: Parameter importierter grüner Wasserstoff**

| Kenngrösse | Einheit | 2030 | 2040 | 2050 | Quelle |
|---|---|---|---|---|---|
| Herkunft | Region | EU | EU | MENA | |
| Äq. Vollaststunden | h | 4000 | 4000 | 3000 | EP2050+ |
| Wirkungsgrad (HHV) | %HHV | 72% | 73% | 75% | EP2050+ |
| Wirkungsgrad (LHV) | %LHV | 61% | 62% | 63% | |
| CAPEX (CH) | CHF/kW_el | 1200 | 1000 | 900 | VSE |
| CAPEX (CH) | CHF/kW_H2_LHV | 1972 | 1621 | 1420 | Berechnung |
| Amortisation | Jahre | 15 | 15 | 15 | EP2050+ |
| WACC | % | 5% | 5% | 10% | EP2050+ |
| Annuität (CRF) | % | 10% | 10% | 13% | Berechnung |
| CAPEX (annualisiert) | CHF/kW_H2/Jahr | 190 | 156 | 187 | Berechnung |
| CAPEX (annualisiert) | CHF/kWh_LHV | 0.047 | 0.039 | 0.062 | Berechnung |
| Fixe O&M (FOM) | %CAPEX | 3% | 3% | 3% | VSE |
| FOM | CHF/kW_H2/Jahr | 59 | 49 | 43 | Berechnung |
| FOM | CHF/kWh_LHV | 0.015 | 0.012 | 0.014 | Berechnung |
| Variable O&M (VOM) | CHF/kWh_LHV | 0.000 | 0.000 | 0.000 | VSE |
| Relevanter Strompreis | CHF/kWh_el | 0.050 | 0.050 | 0.020 | VSE |
| Relevanter Strompreis | CHF/kWh_LHV | 0.082 | 0.081 | 0.032 | Berechnung |
| Gestehungskosten (LCOH) | CHF/kWh_LHV | 0.144 | 0.132 | 0.108 | Berechnung |
| LCOH | CHF/kg H2 | 4.8 | 4.4 | 3.6 | Berechnung |
| Transport (international) Typ | | Pipeline EU | Pipeline EU | Pipeline MENA | |
| Transport Distanz | km | 1000 | 1000 | 2000 | VSE |

[PAGE 37]

| Kenngrösse | Einheit | 2030 | 2040 | 2050 | Quelle |
|---|---|---|---|---|---|
| Transport | CHF/kg H2/1000 km | 0.35 | 0.33 | 0.30 | Alpiq |
| Transport | CHF/kWh_LHV | 0.011 | 0.010 | 0.018 | Berechnung |
| Transport | CHF/kg H2 | 0.4 | 0.3 | 0.6 | Berechnung |
| Kosten (franco Landesgrenze) | CHF/kWh_LHV | 0.155 | 0.142 | 0.126 | Berechnung |
| Kosten (franco Landesgrenze) | CHF/kg H2 | 5.2 | 4.7 | 4.2 | Berechnung |

[PAGE 38]

**Tabelle 5: Parameter inländischer Transport und Aufbereitung Wasserstoff**

| Kenngrösse | Einheit | 2030 | 2040 | 2050 | Quelle |
|---|---|---|---|---|---|
| Transport (inländisch) Typ | | LKW | Pipeline + LKW | Pipeline | |
| Transport (inländisch) | CHF/kWh_LHV | 0.020 | 0.010 | 0.005 | Alpiq |
| Transport (inländisch) | CHF/kg H2 | 0.67 | 0.33 | 0.17 | Berechnung |
| Kosten (Endverbrauch CH) | CHF/kWh_LHV | 0.175 | 0.152 | 0.131 | Berechnung |
| Kosten (Endverbrauch CH) | CHF/kg H2 | 5.8 | 5.1 | 4.4 | Berechnung |
| Aufbereitung Tankstelle | CHF/kWh_LHV | 0.051 | 0.026 | 0.017 | EP2050+ |
| Aufbereitung Tankstelle | CHF/kg H2 | 1.7 | 0.8 | 0.6 | Berechnung |
| Transport + Tankstelle CH | CHF/kWh_LHV | 0.071 | 0.036 | 0.022 | Berechnung |
| Transport + Tankstelle CH | CHF/kg H2 | 2.4 | 1.2 | 0.7 | Berechnung |
| Kosten (Tankstelle) | CHF/kWh_LHV | 0.226 | 0.177 | 0.148 | Berechnung |
| Kosten (Tankstelle) | CHF/kg H2 | 7.5 | 5.9 | 4.9 | Berechnung |

**5.4 Wetterextreme: kalte Dunkelflaute**

- Referenzklimadaten: Kalenderjahr 2016; Extremwinter: 2005/06 (Dez–Feb europaweit kalt, trocken, windarm; Sonneneinstrahlung durchschnittlich).
- Temperatureinfluss auf Verbrauch abgeschätzt via Regression populations-gewichteter Temperaturdaten und Verbrauchsdaten Klimajahre 1995, 2007, 2009 (wöchentliche Basis).
- Stromerzeugungsprofile: Pan-European-Climate-Database (PECD) für 2005/06 und 2016.

[PAGE 39–41]

- Klima 2005/06 vs. 2016, Dez–Feb 2050 Schweiz + Nachbarländer (Variante «Gas»):
  - Stromverbrauch CH: +**2 TWh** (+**8%**)
  - Schweizer Stromproduktion: leicht reduziert
  - Wegfallende Nettoimporte aus Nachbarländern: -**4 TWh** (Dez–Feb)
  - Resultierende Winterstromlücke: +**6 TWh** → durch zusätzliche inländische ergänzende Produktion zu decken.
- Variante «mehr Wind» (Klima 2005/06 vs. 2016): zusätzliche ergänzende inländische Produktion +**8 TWh** (Dez–Feb).

[PAGE 43–44]

**Tabelle 6: Exogene Energiepreise inländisch produzierte Energieträger [CHF/MWhLHV]**

| Energieträger | Herkunft | Stützjahr | Grenzübertr.kosten | Quelle | CO2-Gehalt [kg CO2eq/MWhLHV] | CO2 EU ETS [CHF/t CO2] | Kosten (Energietr.+CO2) [CHF/MWhLHV] |
|---|---|---|---|---|---|---|---|
| Biotreibstoffe | CH | REF | 205 | EP2050+ | 0 | 78 | 205 |
| Biotreibstoffe | CH | 2030 | 294 | EP2050+ | 0 | 123 | 294 |
| Biotreibstoffe | CH | 2040 | 291 | EP2050+ | 0 | 168 | 291 |
| Biotreibstoffe | CH | 2050 | 288 | EP2050+ | 0 | 30 | 288 |
| Biomethan | CH | REF | 164 | EP2050+ | 0 | 78 | 164 |
| Biomethan | CH | 2030 | 168 | EP2050+ | 0 | 123 | 168 |
| Biomethan | CH | 2040 | 173 | EP2050+ | 0 | 168 | 173 |
| Biomethan | CH | 2050 | 177 | EP2050+ | 0 | 30 | 177 |
| Umweltwärme | CH | REF | 0 | VSE | 0 | 78 | 0 |
| Umweltwärme | CH | 2030 | 0 | VSE | 0 | 123 | 0 |
| Umweltwärme | CH | 2040 | 0 | VSE | 0 | 168 | 0 |
| Umweltwärme | CH | 2050 | 0 | VSE | 0 | 30 | 0 |
| Abfall KVA | CH | REF | 2 | EP2050+ | 332 | 78 | 28 |
| Abfall KVA | CH | 2030 | 2 | EP2050+ | 332 | 123 | 43 |
| Abfall KVA | CH | 2040 | 2 | EP2050+ | 332 | 168 | 58 |
| Abfall KVA | CH | 2050 | 2 | EP2050+ | 332 | 30 | 12 |
| Abfall Zement | CH | REF | 2 | EP2050+ | 332 | 78 | 28 |
| Abfall Zement | CH | 2030 | 2 | EP2050+ | 332 | 123 | 43 |
| Abfall Zement | CH | 2040 | 2 | EP2050+ | 332 | 168 | 58 |
| Abfall Zement | CH | 2050 | 2 | EP2050+ | 332 | 30 | 12 |
| Feste Biomasse (Holz) | CH | REF | 43 | EP2050+ | 0 | 78 | 43 |
| Feste Biomasse (Holz) | CH | 2030 | 48 | EP2050+ | 0 | 123 | 48 |
| Feste Biomasse (Holz) | CH | 2040 | 54 | EP2050+ | 0 | 168 | 54 |
| Feste Biomasse (Holz) | CH | 2050 | 59 | EP2050+ | 0 | 30 | 59 |

[PAGE 45–48]

**Tabelle 7: Exogene Energiepreise importierte Energieträger (franco Landesgrenze) [CHF/MWhLHV]**

| Energieträger | Herkunft | Stützjahr | Grenzübertr.kosten | Quelle | CO2-Gehalt [kg CO2eq/MWhLHV] | CO2 EU ETS [CHF/t CO2] | Kosten (Energietr.+CO2) [CHF/MWhLHV] |
|---|---|---|---|---|---|---|---|
| Biomethan | Import | REF | 164 | EP2050+ | 202 | 78 | 180 |
| Biomethan | Import | 2030 | 168 | EP2050+ | 0 | 123 | 168 |
| Biomethan | Import | 2040 | 173 | EP2050+ | 0 | 168 | 173 |
| Biomethan | Import | 2050 | 177 | EP2050+ | 0 | 30 | 177 |
| Erdgas | Import | REF | 31 | EP2050+ | 202 | 78 | 47 |
| Erdgas | Import | 2030 | 34 | EP2050+ | 202 | 123 | 58 |
| Erdgas | Import | 2040 | 34 | EP2050+ | 202 | 168 | 68 |
| Erdgas | Import | 2050 | 24 | EP2050+ | 202 | 30 | 30 |
| H2 grün | Import | REF | 0 | VSE | 0 | 78 | 0 |
| H2 grün | Import | 2030 | 155 | VSE | 0 | 123 | 155 |
| H2 grün | Import | 2040 | 142 | VSE | 0 | 168 | 142 |
| H2 grün | Import | 2050 | 126 | VSE | 0 | 30 | 126 |
| SNG | Import | REF | 0 | VSE | 0 | 78 | 0 |
| SNG | Import | 2030 | 201 | VSE | 0 | 123 | 201 |
| SNG | Import | 2040 | 185 | VSE | 0 | 168 | 185 |
| SNG | Import | 2050 | 164 | VSE | 0 | 30 | 164 |
| Biotreibstoffe | Import | REF | 205 | EP2050+ | 0 | 78 | 205 |
| Biotreibstoffe | Import | 2030 | 294 | EP2050+ | 0 | 123 | 294 |
| Biotreibstoffe | Import | 2040 | 291 | EP2050+ | 0 | 168 | 291 |
| Biotreibstoffe | Import | 2050 | 288 | EP2050+ | 0 | 30 | 288 |
| Synthetische Treibstoff (PtL) | Import | REF | 468 | EP2050+ | 0 | 78 | 468 |
| Synthetische Treibstoff (PtL) | Import | 2030 | 401 | EP2050+ | 0 | 123 | 401 |
| Synthetische Treibstoff (PtL) | Import | 2040 | 355 | EP2050+ | 0 | 168 | 355 |
| Synthetische Treibstoff (PtL) | Import | 2050 | 319 | EP2050+ | 0 | 30 | 319 |
| Erdölprodukte | Import | REF | 55 | EP2050+ | 265 | 78 | 75 |
| Erdölprodukte | Import | 2030 | 52 | EP2050+ | 265 | 123 | 85 |
| Erdölprodukte | Import | 2040 | 46 | EP2050+ | 265 | 168 | 90 |
| Erdölprodukte | Import | 2050 | 30 | EP2050+ | 265 | 30 | 37 |
| Rohöl | Import | REF | 47 | EP2050+ | 265 | 78 | 68 |
| Rohöl | Import | 2030 | 47 | EP2050+ | 265 | 123 | 80 |
| Rohöl | Import | 2040 | 42 | EP2050+ | 265 | 168 | 87 |
| Rohöl | Import | 2050 | 29 | EP2050+ | 265 | 30 | 37 |
| Feste Biomasse (Holz) | Import | REF | 41 | EP2050+ | 0 | 78 | 41 |
| Feste Biomasse (Holz) | Import | 2030 | 46 | EP2050+ | 0 | 123 | 46 |
| Feste Biomasse (Holz) | Import | 2040 | 51 | EP2050+ | 0 | 168 | 51 |
| Feste Biomasse (Holz) | Import | 2050 | 56 | EP2050+ | 0 | 30 | 56 |
| Kernbrennstäbe | Import | REF | 1.7 | TYNDP2020 | 0 | 78 | 2 |
| Kernbrennstäbe | Import | 2030 | 1.7 | TYNDP2020 | 0 | 123 | 2 |
| Kernbrennstäbe | Import | 2040 | 1.7 | TYNDP2020 | 0 | 168 | 2 |
| Kernbrennstäbe | Import | 2050 | 1.7 | TYNDP2020 | 0 | 30 | 2 |
| Abfall KVA | Import | REF | 1.9 | EP2050+ | 332 | 78 | 28 |
| Abfall KVA | Import | 2030 | 1.9 | EP2050+ | 332 | 123 | 43 |
| Abfall KVA | Import | 2040 | 1.9 | EP2050+ | 332 | 168 | 58 |
| Abfall KVA | Import | 2050 | 1.9 | EP2050+ | 332 | 30 | 12 |
| Braunkohle | Import | REF | 6.5 | TYNDP2020 | 364 | 78 | 35 |
| Braunkohle | Import | 2030 | 6.5 | TYNDP2020 | 364 | 123 | 51 |
| Braunkohle | Import | 2040 | 6.5 | TYNDP2020 | 364 | 168 | 68 |
| Braunkohle | Import | 2050 | 6.5 | TYNDP2020 | 364 | 30 | 17 |

[PAGE 49]

**7.3 Stromverbrauch**

Tabelle 8: Jahresstromverbrauch (GWh/Jahr) pro Szenario, Variante und Verbraucher für REF und 2050.

| Verbrauch | Jahr | REF | 2050 Gas integriert stat.(3%) | 2050 Gas isoliert stat. | 2050 LTO integriert stat.(3%) | 2050 LTO isoliert stat. | 2050 mehr integriert stat.(3%) | 2050 mehr isoliert stat.(3%) | 2050 mehr Wind integriert stat.(3%) | 2050 mehr Wind isoliert stat. |
|---|---|---|---|---|---|---|---|---|---|---|
| EE-Ausbau | | | Stromg. | Stromg. | Stromg. | Stromg. | Stromg. | Stromg. | Stromg. | Stromg. |
| Stromabkommen | | | integriert | isoliert | integriert | isoliert | integriert | isoliert | integriert | isoliert |
| PV Einspeisebegrenzung | | | stat. (3%) | stat. | stat. (3%) | stat. | stat. (3%) | stat. (3%) | stat. (3%) | stat. |
| Verbrauch | | | effizient | effizient | effizient | effizient | effizient | effizient | effizient | effizient |
| Variante | | | Gas Imp. | Gas Imp. | LTO | LTO | mehr | mehr | mehr Wind | mehr Wind |
| Konv. Endverbrauch Eisenbahn (NE1) | | 2590 | 3550 | 3550 | 3550 | 3550 | 3550 | 3550 | 3550 | 3550 |
| Konv. Endverbrauch Verkehr (NE3) | | 136 | 187 | 187 | 187 | 187 | 187 | 187 | 187 | 187 |
| Konv. Endverbrauch Strombedarf NE 3 | | 672 | 545 | 545 | 545 | 545 | 545 | 545 | 545 | 545 |
| Konv. Endverbrauch Strombedarf NE 5 | | 12041 | 9584 | 9584 | 9584 | 9584 | 9584 | 9584 | 9584 | 9584 |
| Konv. Endverbrauch Strombedarf NE 7 | | 14522 | 12050 | 12050 | 12050 | 12050 | 12050 | 12050 | 12050 | 12050 |
| Konv. Endverbrauch Strombedarf Prosumer | | 14756 | 13441 | 13441 | 13441 | 13441 | 13441 | 13441 | 13441 | 13441 |
| Wärme/Kälte Klimatisierung (Elektro AC) | | 1180 | 1382 | 1382 | 1382 | 1382 | 1382 | 1382 | 1382 | 1382 |
| Wärme/Kälte Klimatisierung (WP) | | 139 | 602 | 602 | 602 | 602 | 602 | 602 | 602 | 602 |
| Wärme/Kälte El. Heizung (DHW) | | 2539 | 88 | 88 | 88 | 88 | 88 | 88 | 29 | 29 |
| Wärme/Kälte El. Heizung (PH) | | 3021 | 5212 | 5212 | 5212 | 5212 | 5212 | 5212 | 5212 | 5212 |

[PAGE 50]

| Verbrauch | REF | 2050 Gas int. | 2050 Gas isol. | 2050 LTO int. | 2050 LTO isol. | 2050 mehr int. | 2050 mehr isol. | 2050 mehr Wind int. | 2050 mehr Wind isol. |
|---|---|---|---|---|---|---|---|---|---|
| Wärme/Kälte El. Heizung (SH) | 3748 | 108 | 108 | 108 | 108 | 108 | 108 | 35 | 35 |
| Wärme/Kälte Wärmepumpe (DHW) | 199 | 1876 | 1876 | 1876 | 1876 | 1876 | 1876 | 1876 | 1876 |
| Wärme/Kälte Wärmepumpe (PH) | 84 | 257 | 257 | 257 | 257 | 257 | 257 | 257 | 257 |
| Wärme/Kälte Wärmepumpe (SH) | 1606 | 7497 | 7497 | 7497 | 7497 | 7497 | 7497 | 7497 | 7497 |
| e-Mobilität e-Mobilität (NE7) | — | 8373 | 8373 | 8373 | 8373 | 8373 | 8373 | 8373 | 8373 |
| e-Mobilität e-Mobilität (NE5) | — | 7526 | 7526 | 7526 | 7526 | 7526 | 7526 | 7526 | 7526 |
| Rechenzentren | — | 3000 | 3000 | 3000 | 3000 | 3000 | 3000 | 3000 | 3000 |
| CCS CCS Gas-Kombi | — | 580 | 570 | 291 | 301 | 266 | 507 | 225 | 233 |
| CCS CCS Kohle-Brenner (PH) | — | 4 | 4 | 4 | 4 | 4 | 4 | 4 | 4 |
| CCS CCS DAC Schweiz | — | 598 | 596 | 510 | 514 | 504 | 572 | 722 | 685 |
| CCS CCS KVA | — | 136 | 136 | 136 | 136 | 136 | 136 | 136 | 136 |
| CCS CCS Ölheizung (PH) | — | 13 | 13 | 13 | 13 | 13 | 13 | 13 | 13 |
| CCS CCS Stahl/Chemie | — | 108 | 108 | 108 | 108 | 108 | 108 | 108 | 108 |

[PAGE 51]

| Verbrauch | REF | 2050 Gas int. | 2050 Gas isol. | 2050 LTO int. | 2050 LTO isol. | 2050 mehr int. | 2050 mehr isol. | 2050 mehr Wind int. | 2050 mehr Wind isol. |
|---|---|---|---|---|---|---|---|---|---|
| CCS CCS Abfall-Brenner (PH) | — | 356 | 356 | 356 | 356 | 356 | 356 | 356 | 356 |
| CCS CCS Holz BHKW | — | 185 | 185 | 185 | 185 | 185 | 185 | 185 | 185 |
| Elektrolyse Elektrolyse (NE3) | — | — | — | — | — | — | — | — | — |
| Elektrolyse Elektrolyse (Laufwasser) | — | 3487 | 3229 | 3565 | 3873 | 3601 | 3313 | 3854 | 3485 |
| Grosswärmepumpen Grosswärmepumpe | — | 2700 | 2700 | 2700 | 2700 | 2700 | 2700 | 2700 | 2700 |
| Export (netto) Export | 597 | — | — | 336 | 328 | — | — | — | — |
| Netzverluste Netzverluste | 5188 | 5582 | 5413 | 5648 | 5488 | 5635 | 5403 | 5482 | 5321 |
| Batterien Batterien | — | 664 | 539 | 664 | 534 | 665 | 565 | 487 | 380 |
| Pumpspeicher Pumpspeicher | 653 | 1460 | 1463 | 1523 | 1591 | 1511 | 1688 | 1254 | 689 |

[PAGE 52]

**7.4 Stromerzeugung (Energie)**

Tabelle 9: Jahresstromproduktion (GWh/Jahr) pro Szenario, Variante und Technologie für REF und 2050.

| Produktion (GWh/Jahr) | REF | 2050 Gas int. | 2050 Gas isol. | 2050 LTO int. | 2050 LTO isol. | 2050 mehr int. | 2050 mehr isol. | 2050 mehr Wind int. | 2050 mehr Wind isol. |
|---|---|---|---|---|---|---|---|---|---|
| Import (netto) | — | 2'890 | 2'099 | — | — | 7'927 | 3'364 | 5'604 | 4'329 |
| Wind Wind (Generator) | 137 | 3'685 | 3'735 | 3'686 | 3'727 | 3'683 | 3'729 | 24'722 | 25'897 |
| PV PV Dach | 2'408 | 32'672 | 32'672 | 32'672 | 32'672 | 32'672 | 32'672 | 7'199 | 6'216 |
| PV PV alpin | — | 1'788 | 1'835 | 1'789 | 1'804 | 1'781 | 1'825 | — | — |
| PV PV utility-scale | — | 1'812 | 1'839 | 1'822 | 1'813 | 1'801 | 1'853 | 9'999 | 10'001 |
| Gaskraftwerke Gaskombi (CCS) | — | 9'568 | 9'406 | 4'804 | 4'974 | 4'389 | 8'362 | 3'710 | 3'842 |
| Kernenergie LTO | — | — | — | 7'815 | 7'468 | — | — | — | — |
| Kernenergie Neues KKW | — | — | — | — | — | — | — | — | — |
| Kernenergie Bestehende KKW | 23'197 | — | — | — | — | — | — | — | — |
| Laufwasser (> 10 MW) | 13'243 | 13'243 | 13'243 | 13'243 | 13'243 | 13'243 | 13'243 | 13'243 | 13'243 |

[PAGE 53]

| Produktion (GWh/Jahr) | REF | 2050 Gas int. | 2050 Gas isol. | 2050 LTO int. | 2050 LTO isol. | 2050 mehr int. | 2050 mehr isol. | 2050 mehr Wind int. | 2050 mehr Wind isol. |
|---|---|---|---|---|---|---|---|---|---|
| Laufwasser (< 10 MW) | 3'311 | 3'430 | 3'430 | 3'430 | 3'430 | 3'430 | 3'430 | 3'430 | 3'430 |
| Speicherwasser | 17'972 | 18'739 | 18'739 | 18'739 | 18'739 | 18'739 | 18'739 | 18'739 | 18'739 |
| KVA KVA (CCS) | — | 1'637 | 1'637 | 1'637 | 1'637 | 1'637 | 1'637 | 1'637 | 1'637 |
| KVA KVA | 2'339 | 702 | 702 | 702 | 702 | 702 | 702 | 702 | 702 |
| WKK/BZ Biogas BHKW | 92 | 1'140 | 1'140 | 1'140 | 1'140 | 1'140 | 1'140 | — | — |
| WKK/BZ Gas-BHKW | 622 | — | — | — | — | — | — | 529 | 232 |
| WKK/BZ Geothermie BHKW | — | 200 | 200 | 200 | 200 | 200 | 200 | 250 | 250 |
| WKK/BZ Holz-BHKW (CCS) | — | 725 | 725 | 725 | 725 | 725 | 725 | 725 | 725 |

[PAGE 54]

**7.5 Leistungen (nur Strom)**

Tabelle 10: Installierte Leistung zur Stromerzeugung (GW) pro Szenario, Variante und Technologie für REF und 2050.

| Leistung (GW) | REF | 2050 Gas int. | 2050 Gas isol. | 2050 LTO int. | 2050 LTO isol. | 2050 mehr int. | 2050 mehr isol. | 2050 mehr Wind int. | 2050 mehr Wind isol. |
|---|---|---|---|---|---|---|---|---|---|
| Wind Wind (Generator) | 0.075 | 1.900 | 1.900 | 1.900 | 1.900 | 1.900 | 1.900 | 12.462 | 12.968 |
| PV PV Dach | 2.000 | 27.138 | 27.138 | 27.138 | 27.138 | 27.138 | 27.138 | 5.980 | 5.164 |
| PV PV alpin | 0.000 | 1.509 | 1.509 | 1.509 | 1.509 | 1.509 | 1.509 | 0.000 | 0.000 |
| PV PV utility-scale | 0.000 | 1.682 | 1.682 | 1.682 | 1.682 | 1.682 | 1.682 | 7.148 | 7.148 |
| Gaskraftwerke Gaskombi (CCS) | 0.000 | 2.477 | 2.466 | 1.413 | 1.317 | 1.918 | 2.249 | 2.051 | 1.085 |
| Kernenergie LTO | 0.000 | 0.000 | 0.000 | 1.011 | 1.011 | 0.000 | 0.000 | 0.000 | 0.000 |
| Kernenergie Neues KKW | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 |
| Kernenergie Bestehende KKW | 2.974 | — | — | — | — | — | — | — | — |
| Laufwasser (> 10 MW) | 2.871 | 2.871 | 2.871 | 2.871 | 2.871 | 2.871 | 2.871 | 2.871 | 2.871 |
| Laufwasser (< 10 MW) | 0.718 | 0.744 | 0.744 | 0.744 | 0.744 | 0.744 | 0.744 | 0.744 | 0.744 |

[PAGE 55]

| Leistung (GW) | REF | 2050 Gas int. | 2050 Gas isol. | 2050 LTO int. | 2050 LTO isol. | 2050 mehr int. | 2050 mehr isol. | 2050 mehr Wind int. | 2050 mehr Wind isol. |
|---|---|---|---|---|---|---|---|---|---|
| Speicherwasser | 8.200 | 8.379 | 8.379 | 8.379 | 8.379 | 8.379 | 8.379 | 8.379 | 8.379 |
| KVA KVA (CCS) | 0.000 | 0.301 | 0.301 | 0.301 | 0.301 | 0.301 | 0.301 | 0.301 | 0.301 |
| KVA KVA | 0.430 | 0.129 | 0.129 | 0.129 | 0.129 | 0.129 | 0.129 | 0.129 | 0.129 |
| WKK/BZ Biogas BHKW | 0.071 | 0.714 | 0.710 | 0.714 | 0.714 | 0.714 | 0.693 | 0.000 | 0.000 |
| WKK/BZ Gas-BHKW | 0.380 | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 | 0.901 | 0.712 |
| WKK/BZ Geothermie BHKW | 0.000 | 0.029 | 0.029 | 0.029 | 0.029 | 0.029 | 0.029 | 0.029 | 0.029 |
| WKK/BZ Holz-BHKW (CCS) | 0.000 | 0.083 | 0.083 | 0.083 | 0.083 | 0.083 | 0.083 | 0.083 | 0.083 |
| WKK/BZ Holz BHKW | 0.070 | 0.055 | 0.055 | 0.055 | 0.055 | 0.055 | 0.055 | 0.055 | 0.055 |

[PAGE 56]

**7.6 Technische Parameter Technologien**

Tabelle 11: Technische Parameter der Modellierung pro Technologie für REF und 2050.

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | (Strom-)Wirkungsgrad | Leistung (GW) | Max. Jahresproduktion / äq. Volllaststunden (GWh) |
|---|---|---|---|---|---|---|
| KVA | 2050 | Abfall | KVA Netzebene 5 | 20% | 0.129 | 701 |
| KVA CCS | 2050 | Abfall | KVA Strom KVA (CCS) | 20% | 0.301 | 1637 |
| Gas-Kombi CH4 (CCGT) | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Netzebene 3 | 60% | endogen (invest) | — |
| Gas-Kombi | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Netzebene 3 | 60% | endogen (invest) | — |
| CCS Gas-Kombi | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Strom Gaskraftwerk (CCS) | 60% | endogen (invest) | — |
| Holz BHKW | 2050 | Feste Biomasse/Holz | Netzebene 5 | 20% | endogen (invest) | — |
| CCS Holz BHKW | 2050 | Feste Biomasse/Holz | Strom Holz BHKW (CCS) | 20% | endogen (invest) | — |
| Methanisierung (Sabatier) | 2050 | Wasserstoff (H2) Grossbezüger (Gaskraftwerke) | Sabatier | 85% | endogen (invest) | — |

[PAGE 57]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Fischer-Tropsch | 2050 | Wasserstoff (H2) Grossbezüger (Gaskraftwerke) | Fischer-Tropsch | 55% | endogen (invest) | — |
| Ölheizung (PH) | 2050 | Erdölprodukte | Ind. Prozesswärme | 85% | endogen (invest) | 149 |
| CCS Ölheizung (PH) | 2050 | Erdölprodukte | Prozesswärme Öl-Brenner (CCS) | 85% | endogen (invest) | 224 |
| Kohle-Brenner (PH) | 2050 | Steinkohle | Ind. Prozesswärme | 85% | endogen (invest) | 0 |
| CCS Kohle-Brenner (PH) | 2050 | Steinkohle | Prozesswärme Kohle-Brenner (CCS) | 85% | endogen (invest) | 50 |
| Abfallverbrennung (PH) | 2050 | Abfall Zemenwerk | Ind. Prozesswärme | 85% | endogen (invest) | 505 |
| Abfall-Brenner (PH) | 2050 | Abfall Zemenwerk | Ind. Prozesswärme | 85% | endogen (invest) | 505 |
| CCS Abfall-Brenner (PH) | 2050 | Abfall Zemenwerk | Prozesswärme Abfall-Brenner (CCS) | 85% | endogen (invest) | 2023 |
| Gas-BHKW | REF | Methan (Gase) Grossbezüger (Hochdruck) | Netzebene 5 | 30% | endogen (invest) | — |

[PAGE 58]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Gas-BHKW | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Netzebene 5 | 30% | endogen (invest) | — |
| Biogas BHKW (inkl. ARA) | REF | Biogas (inkl. ARA) | Netzebene 5 | 30% | endogen (invest) | — |
| Biogas BHKW (inkl. ARA) | 2050 | Biogas (inkl. ARA) | Netzebene 5 | 30% | endogen (invest) | — |
| Geothermie BHKW | 2050 | Umweltwärme | Netzebene 5 | 20% | endogen (invest) | 200 |
| Holz BHKW | REF | Feste Biomasse/Holz | Netzebene 5 | 15% | endogen (invest) | — |
| Brennstoffzelle (Quartier) | 2050 | Methan (Gase) Kleinbezüger (Niederdruck) | Netzebene 5 | 64% | endogen (invest) | 1500 |
| KVA | REF | Abfall | KVA Netzebene 5 | 20% | 0.43 | 2339 |
| Laufwasser (Turbine > 10 MW) | REF | Laufwasser gross | Netzebene 3 | 100% | 2.871143188 | — |
| Laufwasser (Turbine > 10 MW) | 2050 | Laufwasser gross | Netzebene 3 | 100% | 2.871143188 | — |
| Laufwasser (Turbine, < 10 MW) | REF | Laufwasser klein | Netzebene 5 | 100% | 0.717785797 | — |

[PAGE 59]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Laufwasser (Turbine, < 10 MW) | 2050 | Laufwasser klein | Netzebene 5 | 100% | 0.743657928 | — |
| Speicherwasser bestehend (Turbine) | REF | Speicherwasser | Netzebene 1 | 85% | 8.2 | — |
| Speicherwasser bestehend (Turbine) | 2050 | Speicherwasser | Netzebene 1 | 85% | 8.379 | — |
| Pumpspeicher (Turbine) | REF | Pumpspeicher (bestehend) | Übertragungsnetz NE1 | 85% | 3.2 | 4000 |
| Pumpspeicher (Turbine) | 2050 | Pumpspeicher (bestehend) | Übertragungsnetz NE1 | 85% | 4.2 | — |
| Pumpspeicher (Pumpe) | REF | Übertragungsnetz NE1 | Pumpspeicher (bestehend) | 85% | 2.7 | 4000 |
| Pumpspeicher (Pumpe) | 2050 | Übertragungsnetz NE1 | Pumpspeicher (bestehend) | 85% | 3.7 | — |
| Pumpspeicher neu (Turbine) | 2050 | Pumpspeicher (neu) | Übertragungsnetz NE1 | 85% | 1.8 | — |
| Pumpspeicher neu (Pumpe) | 2050 | Übertragungsnetz NE1 | Pumpspeicher (neu) | 85% | endogen (invest) | — |

[PAGE 60]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Gasturbine H2 | 2050 | Wasserstoff (H2) Grossbezüger (Gaskraftwerke) | Netzebene 3 | 42% | endogen (invest) | — |
| Gas-Kombi H2 | 2050 | Wasserstoff (H2) Grossbezüger (Gaskraftwerke) | Netzebene 3 | 60% | endogen (invest) | — |
| Gasturbine CH4 (OCGT) | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Netzebene 3 | 42% | endogen (invest) | — |
| Elektrolyse (NE3) | 2050 | Hochspannungsnetz NE3 Grossbezüger (Gaskraftwerke) | Wasserstoff (H2) | 63% | endogen (invest) | — |
| Elektrolyse (Laufwasser gross) | 2050 | Laufwasser gross Elyse-RoR gross | Wasserstoff (H2) | 63% | endogen (invest) | — |
| H2-Heizung (PH) | 2050 | Wasserstoff (H2) Kleinbezüger (Fernwärme/Industrie) | Ind. Prozesswärme | 85% | endogen (invest) | 1806 |
| Kohleverbrennung (SH) | REF | Steinkohle | Raumwärme | 80% | endogen (invest) | 36 |
| Kohleverbrennung (DHW) | REF | Steinkohle | Warmwasser | 70% | endogen (invest) | 1 |

[PAGE 61]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Kohleverbrennung (PH) | REF | Steinkohle | Ind. Prozesswärme | 85% | endogen (invest) | 1010 |
| Abfallverbrennung (SH) | REF | Abfall Zemenwerk | Raumwärme | 80% | endogen (invest) | 45 |
| Abfallverbrennung (DHW) | REF | Abfall Zemenwerk | Warmwasser | 70% | endogen (invest) | 5 |
| Abfallverbrennung (PH) | REF | Abfall Zemenwerk | Ind. Prozesswärme | 85% | endogen (invest) | 2528 |
| Abfall-Brenner (PH) | REF | Abfall Zemenwerk | Ind. Prozesswärme | 85% | endogen (invest) | 2528 |
| Gasheizung (SH) | REF | Methan (Gase) Kleinbezüger (Niederdruck) | Raumwärme | 93% | endogen (invest) | 21539 |
| Gasheizung (SH) | 2050 | Methan (Gase) Kleinbezüger (Niederdruck) | Raumwärme | 93% | endogen (invest) | 5700 |
| Gasheizung (DHW) | REF | Methan (Gase) Kleinbezüger (Niederdruck) | Warmwasser | 72% | endogen (invest) | 2692 |
| Gasheizung (DHW) | 2050 | Methan (Gase) Kleinbezüger (Niederdruck) | Warmwasser | 72% | endogen (invest) | 661 |

[PAGE 62]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Gasheizung (PH) | REF | Methan (Gase) Grossbezüger (Hochdruck) | Ind. Prozesswärme | 85% | endogen (invest) | 7975 |
| Gasheizung (PH) | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Ind. Prozesswärme | 85% | endogen (invest) | 6122 |
| Holzheizung (SH) | REF | Feste Biomasse/Holz | Raumwärme | 77% | endogen (invest) | 6762 |
| Holzheizung (SH) | 2050 | Feste Biomasse/Holz | Raumwärme | 77% | endogen (invest) | 3518 |
| Holzheizung (DHW) | REF | Feste Biomasse/Holz | Warmwasser | 52% | endogen (invest) | 337 |
| Holzheizung (DHW) | 2050 | Feste Biomasse/Holz | Warmwasser | 52% | endogen (invest) | 309 |
| Holzheizung (PH) | REF | Feste Biomasse/Holz | Ind. Prozesswärme | 85% | endogen (invest) | 2270 |
| Holzheizung (PH) | 2050 | Feste Biomasse/Holz | Ind. Prozesswärme | 85% | endogen (invest) | 2263 |
| El. Heizung (SH) (Prosumer) | REF | Verteilnetz NE7 (Prosumer) | Raumwärme | 93% | endogen (invest) | 3542 |

[PAGE 63]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| El. Heizung (SH) (Prosumer) | 2050 | Verteilnetz NE7 (Prosumer) | Raumwärme | 93% | endogen (invest) | 100 |
| El. Heizung (DHW) (Prosumer) | REF | Verteilnetz NE7 (Prosumer) | Warmwasser | 79% | endogen (invest) | 2079 |
| El. Heizung (DHW) (Prosumer) | 2050 | Verteilnetz NE7 (Prosumer) | Warmwasser | 79% | endogen (invest) | 70 |
| El. Heizung (PH) | REF | Hochspannungsnetz NE3 | Ind. Prozesswärme | 85% | endogen (invest) | 2837 |
| El. Heizung (PH) | 2050 | Hochspannungsnetz NE3 | Ind. Prozesswärme | 85% | endogen (invest) | 4430 |
| Ölheizung (SH) | REF | Erdölprodukte | Raumwärme | 87% | endogen (invest) | 25288 |
| Ölheizung (DHW) | REF | Erdölprodukte | Warmwasser | 66% | endogen (invest) | 2724 |
| Ölheizung (PH) | REF | Erdölprodukte | Ind. Prozesswärme | 85% | endogen (invest) | 2509 |
| Solarthermie (SH) (Haushalte) | REF | Solarthermie | Raumwärme | 100% | 2.674572996 | 267 |
| Solarthermie (SH) (Haushalte) | 2050 | Solarthermie | Raumwärme | 100% | 5 | 500 |

[PAGE 64]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Solarthermie (DHW) (Haushalte) | REF | Solarthermie | Warmwasser | 100% | 4.956552585 | 495 |
| Solarthermie (DHW) (Haushalte) | 2050 | Solarthermie | Warmwasser | 100% | 15 | 1500 |
| Solarthermie (PH) (Industrie/Fernwärme) | REF | Solarthermie | Ind. Prozesswärme | 100% | 0.159642252 | 15 |
| Solarthermie (PH) (Industrie/Fernwärme) | 2050 | Solarthermie | Ind. Prozesswärme | 100% | 10 | 1000 |
| Solarthermie (Fernwärme) | 2050 | Solarthermie (Industrie/Fernwärme) | Fernwärme | 100% | 15 | 1500 |
| Fernwärme (SH) | REF | Fernwärme | Raumwärme | 93% | endogen (invest) | 3469 |
| Fernwärme (SH) | 2050 | Fernwärme | Raumwärme | 93% | endogen (invest) | 14000 |
| Fernwärme (DHW) | REF | Fernwärme | Warmwasser | 77% | endogen (invest) | 508 |
| Fernwärme (DHW) | 2050 | Fernwärme | Warmwasser | 77% | endogen (invest) | 3000 |
| Fernwärme (PH) | REF | Fernwärme | Ind. Prozesswärme | 85% | endogen (invest) | 1343 |

[PAGE 65]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Fernwärme (PH) | 2050 | Fernwärme | Ind. Prozesswärme | 85% | endogen (invest) | 4000 |
| Biogas Netzeinspeisung (Hochdruck) | REF | Biogas (inkl. ARA) Grossbezüger (Hochdruck) | Methan (Gase) Grossbezüger (Hochdruck) | 100% | 10 | — |
| Biogas Netzeinspeisung (Hochdruck) | 2050 | Biogas (inkl. ARA) Grossbezüger (Hochdruck) | Methan (Gase) Grossbezüger (Hochdruck) | 100% | 10 | — |
| Gasnetz (Hochdruck-Niederdruck) | REF | Methan (Gase) Grossbezüger (Hochdruck) | Methan (Gase) Kleinbezüger (Niederdruck) | 98% | 10 | — |
| Gasnetz (Hochdruck-Niederdruck) | 2050 | Methan (Gase) Grossbezüger (Hochdruck) | Methan (Gase) Kleinbezüger (Niederdruck) | 98% | 10 | — |
| H2-Netz (Verteilung CH) | 2050 | Wasserstoff (H2) Grossbezüger (Gaskraftwerke) | Wasserstoff (H2) Kleinbezüger (Fernwärme/Industrie) | 95% | 10 | — |
| H2-Tankstelle | 2050 | Wasserstoff (H2) Kleinbezüger (Fernwärme/Industrie) | Wasserstoff (H2) Tankstelle | 95% | 10 | — |
| Öl Raffinerie (Cressier) | REF | Rohöl | Erdölprodukte | 95% | 16.24236111 | 32484 |

[PAGE 66]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Prosumer (Batterie) Heimbatterie | REF | Prosumer | Verteilnetz NE7 (Prosumer) | 100% | 2 | — |
| Prosumer (Batterie) Heimbatterie | 2050 | Prosumer | Verteilnetz NE7 (Prosumer) | 100% | 33.64091799 | — |
| Prosumer (Netzbezug) | REF | Verteilnetz NE7 (Prosumer) | Verteilnetz NE7 (Prosumer) | 100% | 10 | — |
| Prosumer (Netzbezug) | 2050 | Verteilnetz NE7 (Prosumer) | Verteilnetz NE7 (Prosumer) | 100% | 10 | — |
| CCS Holz-BHKW (CCS) | 2050 | Strom Holz BHKW (CCS) | Netzebene 5 | 100% | 10 | — |
| CCS Gaskombi (CCS) | 2050 | Strom Gaskraftwerk (CCS) | Netzebene 3 | 100% | 10 | — |
| KVA (CCS) | 2050 | Strom KVA (CCS) | Netzebene 5 | 100% | 10 | — |
| CCS Prozesswärme Ölbrenner | 2050 | Prozesswärme Öl-Brenner (CCS) | Ind. Prozesswärme | 100% | 10 | — |
| CCS Prozesswärme Kohlebrenner | 2050 | Prozesswärme Kohle-Brenner (CCS) | Ind. Prozesswärme | 100% | 10 | — |

[PAGE 67]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| CCS Prozesswärme Abfallbrenner | 2050 | Prozesswärme Abfall-Brenner (CCS) | Ind. Prozesswärme | 100% | 10 | ∞ |
| Prozessdampf KVA | REF | Fernwärme (KVA) | Ind. Prozesswärme | 85% | endogen (invest) | 1800 |
| Prozessdampf KVA | 2050 | Fernwärme (KVA) | Ind. Prozesswärme | 85% | endogen (invest) | 1800 |
| Fernwärme KVA | REF | Fernwärme (KVA) | Fernwärme | 85% | endogen (invest) | 2200 |
| Fernwärme KVA | 2050 | Fernwärme (KVA) | Fernwärme | 85% | endogen (invest) | 2200 |
| CCS Stahl/Chemie | 2050 | Hochspannungsnetz NE3 | CO2 Punktquelle | 18% | 0.068 | 600 |
| CCS DAC Schweiz | 2050 | Hochspannungsnetz NE3 | CO2 Punktquelle | 40% | endogen (invest) | ∞ |
| CO2 Transport & Speicherung (bis 0.5 Mt) | 2050 | CO2 | CO2 Speicher | 0% | 41.67 | 500 |
| CO2 Transport & Speicherung (bis 2 Mt) | 2050 | CO2 | CO2 Speicher | 0% | 125 | 1500 |

[PAGE 68]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| CO2 Transport & Speicherung (bis 5 Mt) | 2050 | CO2 | CO2 Speicher | 0% | 250 | 3000 |
| CO2 Transport & Speicherung (über 5 Mt) | 2050 | CO2 | CO2 Speicher | 0% | 250 | ∞ |
| CCS DAC Ausland | 2050 | CO2 | CO2 Speicher | 0% | 5000 | 5000 |
| Klimatisierung (Wärmepumpe) | REF | Verteilnetz NE7 (Prosumer) | Raumkälte | 520% | endogen (invest) | 725 |
| Klimatisierung (Wärmepumpe) | 2050 | Verteilnetz NE7 (Prosumer) | Raumkälte | 520% | endogen (invest) | 3131 |
| Klimatisierung (Elektro AC) | REF | Verteilnetz NE7 (Prosumer) | Raumkälte | 280% | endogen (invest) | 11254 |
| Klimatisierung (Elektro AC) | 2050 | Verteilnetz NE7 (Prosumer) | Raumkälte | 420% | endogen (invest) | 11254 |
| Wärmepumpe (SH) | REF | Verteilnetz NE7 (Prosumer) | Raumwärme | 400% | endogen (invest) | 6424 |
| Wärmepumpe (SH) | 2050 | Verteilnetz NE7 (Prosumer) | Raumwärme | 400% | endogen (invest) | 29987 |
| Wärmepumpe (DHW) | REF | Verteilnetz NE7 (Prosumer) | Warmwasser | 400% | endogen (invest) | 797 |

[PAGE 69]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Wärmepumpe (DHW) | 2050 | Verteilnetz NE7 (Prosumer) | Warmwasser | 400% | endogen (invest) | 7502 |
| Grosswärmepumpe (FW) | 2050 | Mittelspannungsnetz NE5 | Fernwärme | 400% | endogen (invest) | 10800 |
| Wärmepumpe (PH) | REF | Mittelspannungsnetz NE5 | Ind. Prozesswärme | 400% | endogen (invest) | 342 |
| Wärmepumpe (PH) | 2050 | Mittelspannungsnetz NE5 | Ind. Prozesswärme | 400% | endogen (invest) | 1026 |
| Wind | REF | — | Wind | 100% | endogen (invest) | 1826 |
| Wind | 2050 | — | Wind | 100% | endogen (invest) | 1999 |
| PV Dach | REF | — | Prosumer PV | 100% | endogen (invest) | 1203 |
| PV Dach | 2050 | — | Prosumer PV | 100% | endogen (invest) | 1203 |
| PV alpin | 2050 | — | PV alpin | 100% | endogen (invest) | 1458 |
| PV Freifläche (utility) | 2050 | — | PV Freifläche | 100% | endogen (invest) | 1399 |

[PAGE 70]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Solarthermie (gross) | REF | — | Solarthermie (Industrie/Fernwärme) | 100% | endogen (invest) | 1946 |
| Solarthermie (gross) | 2050 | — | Solarthermie (Industrie/Fernwärme) | 100% | endogen (invest) | 1946 |
| Solarthermie (klein) | REF | — | Solarthermie (Haushalte) | 100% | endogen (invest) | 1946 |
| Solarthermie (klein) | 2050 | — | Solarthermie (Haushalte) | 100% | endogen (invest) | 1946 |
| Import aus AT (Jahr/Winter) | REF | Strom AT | Netzebene 1 | 98% | 1.2 | — |
| Import aus AT (Jahr/Winter) | 2050 | Strom AT | Netzebene 1 | 98% | 1.2 | — |
| Export nach AT (Jahr/Winter) | REF | Übertragungsnetz NE1 | Strom AT | 98% | 1.2 | — |
| Export nach AT (Jahr/Winter) | 2050 | Übertragungsnetz NE1 | Strom AT | 98% | 1.2 | — |
| Import aus DE (Jahr/Winter) | REF | Strom DE | Netzebene 1 | 98% | 2.6 | — |
| Import aus DE (Jahr/Winter) | 2050 | Strom DE | Netzebene 1 | 98% | 4.4 | — |

[PAGE 71]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Export nach DE (Jahr/Winter) | REF | Übertragungsnetz NE1 | Strom DE | 98% | 4.2 | — |
| Export nach DE (Jahr/Winter) | 2050 | Übertragungsnetz NE1 | Strom DE | 98% | 4.2 | — |
| Import aus FR (Jahr/Winter) | REF | Strom FR | Netzebene 1 | 98% | 3.7 | — |
| Import aus FR (Jahr/Winter) | 2050 | Strom FR | Netzebene 1 | 98% | 4.5 | — |
| Export nach FR (Jahr/Winter) | REF | Übertragungsnetz NE1 | Strom FR | 98% | 1.4 | — |
| Export nach FR (Jahr/Winter) | 2050 | Übertragungsnetz NE1 | Strom FR | 98% | 2.2 | — |
| Import aus IT Nord (Jahr/Winter) | REF | Strom IT | Netzebene 1 | 98% | 1.9 | — |
| Import aus IT Nord (Jahr/Winter) | 2050 | Strom IT | Netzebene 1 | 98% | 3.1 | — |
| Export nach IT Nord (Jahr/Winter) | REF | Übertragungsnetz NE1 | Strom IT | 98% | 4.4 | — |
| Export nach IT Nord (Jahr/Winter) | 2050 | Übertragungsnetz NE1 | Strom IT | 98% | 5.8 | — |

[PAGE 72]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Import aus AT (Sommer) | REF | Strom AT | Netzebene 1 | 98% | 1.2 | — |
| Import aus AT (Sommer) | 2050 | Strom AT | Netzebene 1 | 98% | 1.2 | — |
| Export nach AT (Sommer) | REF | Übertragungsnetz NE1 | Strom AT | 98% | 1.2 | — |
| Export nach AT (Sommer) | 2050 | Übertragungsnetz NE1 | Strom AT | 98% | 1.2 | — |
| Import aus DE (Sommer) | REF | Strom DE | Netzebene 1 | 98% | 2.6 | — |
| Import aus DE (Sommer) | 2050 | Strom DE | Netzebene 1 | 98% | 4.4 | — |
| Export nach DE (Sommer) | REF | Übertragungsnetz NE1 | Strom DE | 98% | 4.2 | — |
| Export nach DE (Sommer) | 2050 | Übertragungsnetz NE1 | Strom DE | 98% | 4.2 | — |
| Import aus FR (Sommer) | REF | Strom FR | Netzebene 1 | 98% | 3.7 | — |
| Import aus FR (Sommer) | 2050 | Strom FR | Netzebene 1 | 98% | 4.5 | — |

[PAGE 73]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) | Max. Jahresprod. (GWh) |
|---|---|---|---|---|---|---|
| Export nach FR (Sommer) | REF | Übertragungsnetz NE1 | Strom FR | 98% | 1.4 | — |
| Export nach FR (Sommer) | 2050 | Übertragungsnetz NE1 | Strom FR | 98% | 2.2 | — |
| Import aus IT Nord (Sommer) | REF | Strom IT | Netzebene 1 | 98% | 1.9 | — |
| Import aus IT Nord (Sommer) | 2050 | Strom IT | Netzebene 1 | 98% | 3.1 | — |
| Export nach IT Nord (Sommer) | REF | Übertragungsnetz NE1 | Strom IT | 98% | 4.4 | — |
| Export nach IT Nord (Sommer) | 2050 | Übertragungsnetz NE1 | Strom IT | 98% | 5.8 | — |
| AT - DE | REF | Strom AT | Strom DE | 98% | 5.4 | — |
| AT - DE | 2050 | Strom AT | Strom DE | 98% | 7.5 | — |
| AT - IT | REF | Strom AT | Strom IT | 98% | 0.7 | — |
| AT - IT | 2050 | Strom AT | Strom IT | 98% | 0.9 | — |
| DE - AT | REF | Strom DE | Strom AT | 98% | 5.4 | — |
| DE - AT | 2050 | Strom DE | Strom AT | 98% | 7.5 | — |
| DE - FR | REF | Strom DE | Strom FR | 98% | 3 | — |

[PAGE 74]

[PAGE 75]

**Tabelle: Netzebenen – Technische Parameter (Leistung GW, Max. Jahresproduktion/Volllaststunden GWh)**

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) / Volllaststunden (GWh) |
|---|---|---|---|---|---|
| DE - FR | REF | Strom DE | Strom FR | 98% | 4.8 |
| DE - FR | 2050 | Strom DE | Strom FR | 98% | 4.8 |
| FR - DE | REF | Strom FR | Strom DE | 98% | 3 |
| FR - DE | 2050 | Strom FR | Strom DE | 98% | 4.8 |
| FR - IT | REF | Strom FR | Strom IT | 98% | 4.4 |
| FR - IT | 2050 | Strom FR | Strom IT | 98% | 4.5 |
| IT - AT | REF | Strom IT | Strom AT | 98% | 0.5 |
| IT - AT | 2050 | Strom IT | Strom AT | 98% | 0.7 |
| IT - FR | REF | Strom IT | Strom FR | 98% | 2.2 |
| IT - FR | 2050 | Strom IT | Strom FR | 98% | 2.2 |
| Netzebene El1 - NE1 | REF | Netzebene 1 | Übertragungsnetz | 99% | 10.9 |
| Netzebene El1 - NE1 | 2050 | Netzebene 1 | Übertragungsnetz | 99% | 10.9 |
| Trafo NE2 (1-3) | REF | Übertragungsnetz | Netzebene 3 | 100% | 7.8 |
| Trafo NE2 (1-3) | 2050 | Übertragungsnetz | Netzebene 3 | 100% | 7.8 |

[PAGE 75]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) / Volllaststunden (GWh) |
|---|---|---|---|---|---|
| Trafo NE2 (3-1) | REF | Hochspannungsnetz | Netzebene 1 | 100% | 7.8 |
| Trafo NE2 (3-1) | 2050 | Hochspannungsnetz | Netzebene 1 | 100% | 7.8 |
| Netzebene El3 - NE3 | REF | Netzebene 3 | Hochspannungsnetz | 99% | 8.9 |
| Netzebene El3 - NE3 | 2050 | Netzebene 3 | Hochspannungsnetz | 99% | 8.9 |
| Trafo NE4 (3-5) | REF | Hochspannungsnetz | Netzebene 5 | 99% | 8.2 |
| Trafo NE4 (3-5) | 2050 | Hochspannungsnetz | Netzebene 5 | 99% | 8.2 |
| Trafo NE4 (5-3) | REF | Mittelspannungsnetz | Netzebene 3 | 99% | 8.2 |
| Trafo NE4 (5-3) | 2050 | Mittelspannungsnetz | Netzebene 3 | 99% | 8.2 |
| Netzebene NE5 - El5 | REF | Netzebene 5 | Mittelspannungsnetz | 98% | 9.1 |
| Netzebene NE5 - El5 | 2050 | Netzebene 5 | Mittelspannungsnetz | 98% | 9.1 |

[PAGE 76]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) / Volllaststunden (GWh) |
|---|---|---|---|---|---|
| Trafo NE6 (5-7) | REF | Mittelspannungsnetz | Netzebene 7 | 99% | 7.3 |
| Trafo NE6 (5-7) | 2050 | Mittelspannungsnetz | Netzebene 7 | 99% | 7.3 |
| Trafo NE6 (7-5) | REF | Netzebene 7 | Netzebene 5 | 99% | 7.3 |
| Trafo NE6 (7-5) | 2050 | Netzebene 7 | Netzebene 5 | 99% | 7.3 |
| Netzebene NE7 - El7 | REF | Netzebene 7 | Verteilnetz NE7 | 96% | 7 |
| Netzebene NE7 - El7 | 2050 | Netzebene 7 | Verteilnetz NE7 | 96% | 7 |
| Netzebene El7 - NE7 (Prosumer) | REF | Verteilnetz NE7 | Netzebene 7 | 96% | 7 |
| Netzebene El7 - NE7 (Prosumer) | 2050 | Verteilnetz NE7 | Netzebene 7 | 96% | 7 |
| Netzebene El1 - NE1 (invest) | REF | Netzebene 1 | Übertragungsnetz | 99% | endogen |
| Netzebene El1 - NE1 (invest) | 2050 | Netzebene 1 | Übertragungsnetz | 99% | endogen |
| Trafo NE2 (1-3) NE1 (invest) | REF | Übertragungsnetz | Netzebene 3 | 100% | endogen |

[PAGE 77]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) / Volllaststunden (GWh) |
|---|---|---|---|---|---|
| Trafo NE2 (1-3) NE1 (invest) | 2050 | Übertragungsnetz | Netzebene 3 | 100% | endogen |
| Trafo NE2 (3-1) NE3 (invest) | REF | Hochspannungsnetz | Netzebene 1 | 100% | endogen |
| Trafo NE2 (3-1) NE3 (invest) | 2050 | Hochspannungsnetz | Netzebene 1 | 100% | endogen |
| Netzebene El3 - NE3 (invest) | REF | Netzebene 3 | Hochspannungsnetz | 99% | endogen |
| Netzebene El3 - NE3 (invest) | 2050 | Netzebene 3 | Hochspannungsnetz | 99% | endogen |
| Trafo NE4 (3-5) NE3 (invest) | REF | Hochspannungsnetz | Netzebene 5 | 99% | endogen |
| Trafo NE4 (3-5) NE3 (invest) | 2050 | Hochspannungsnetz | Netzebene 5 | 99% | endogen |
| Trafo NE4 (5-3) NE5 (invest) | REF | Mittelspannungsnetz | Netzebene 3 | 99% | endogen |
| Trafo NE4 (5-3) NE5 (invest) | 2050 | Mittelspannungsnetz | Netzebene 3 | 99% | endogen |
| Netzebene NE5 - El5 (invest) | REF | Netzebene 5 | Mittelspannungsnetz | 98% | endogen |

[PAGE 78]

| Technologie | Jahr | Energieträger (ein) | Energieträger (aus) | Wirkungsgrad | Leistung (GW) / Volllaststunden (GWh) |
|---|---|---|---|---|---|
| Netzebene NE5 - El5 (invest) | 2050 | Netzebene 5 | Mittelspannungsnetz | 98% | endogen |
| Trafo NE6 (5-7) NE5 (invest) | REF | Mittelspannungsnetz | Netzebene 7 | 99% | endogen |
| Trafo NE6 (5-7) NE5 (invest) | 2050 | Mittelspannungsnetz | Netzebene 7 | 99% | endogen |
| Trafo NE6 (7-5) (invest) | REF | Netzebene 7 | Netzebene 5 | 99% | endogen |
| Trafo NE6 (7-5) (invest) | 2050 | Netzebene 7 | Netzebene 5 | 99% | endogen |
| Netzebene NE7 - El7 (invest) | REF | Netzebene 7 | Verteilnetz NE7 | 96% | endogen |
| Netzebene NE7 - El7 (invest) | 2050 | Netzebene 7 | Verteilnetz NE7 | 96% | endogen |
| Netzebene El7 - NE7 (Prosumer) (invest) | REF | Verteilnetz NE7 | Netzebene 7 | 96% | endogen |
| Netzebene El7 - NE7 (Prosumer) (invest) | 2050 | Verteilnetz NE7 | Netzebene 7 | 96% | endogen |

[PAGE 79]

**Tabelle 12: Wirtschaftliche Parameter – Technologien (REF und 2050)**
VOM = variable Betriebs- & Unterhaltskosten; CAPEX = (overnight) Investitionen; FOM = fixe Betriebs- & Unterhaltskosten

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| KVA | 0 | 4600 | 96 | 40 | 0 | 0 |
| KVA CCS | 0 | 6118 | 128 | 40 | 0 | 0 |
| Gas-Kombi CH4 (CCGT) | 0.002 | 890 | 21 | 23 | 0 | 0 |
| Gas-Kombi | 0.002 | 890 | 21 | 23 | 0 | 0 |
| CCS Gas-Kombi | 0.003 | 1183 | 27 | 23 | 0 | 10 |
| Holz BHKW | 0.004 | 1400 | 135 | 15 | 0 | 0 |
| CCS Holz BHKW | 0.004 | 1862 | 179 | 15 | 0 | 0 |
| Methansierung (Sabatier) | 0.006 | 1500 | 40 | 30 | 0 | — |
| Fischer-Tropsch | 0.015 | 3750 | 85 | 35 | 0 | — |
| Ölheizung (PH) | 0 | 300 | 9 | 17 | 0 | 0 |
| CCS Ölheizung (PH) | 0 | 399 | 11 | 17 | 0 | 0 |
| Kohle-Brenner (PH) | 0 | 650 | 13 | 25 | 0 | 0 |
| CCS Kohle-Brenner (PH) | 0 | 864 | 17 | 25 | 0 | 0 |
| Abfallverbrennung (PH) | 0 | 650 | 13 | 25 | 0 | 5 |

[PAGE 80]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Abfall-Brenner (PH) | 0 | 650 | 13 | 25 | 0 | 5 |
| CCS Abfall-Brenner (PH) | 0 | 864 | 17 | 25 | 0 | 20 |
| Gas-BHKW | 0.004 | 1000 | 12 | 15 | 0 | 0 |
| Gas-BHKW | 0.004 | 1000 | 12 | 15 | 0 | 1 |
| Biogas BHKW (inkl. ARA) | 0.004 | 1100 | 12 | 15 | 0 | 0 |
| Biogas BHKW (inkl. ARA) | 0.004 | 1100 | 12 | 15 | 0 | 1 |
| Geothermie BHKW | 0.005 | 8000 | 222 | 30 | 0 | 0 |
| Holz BHKW | 0.004 | 1400 | 135 | 15 | 0 | 0 |
| Brennstoffzelle (Quartier) | 0 | 3000 | 45 | 23 | 0 | 0 |
| KVA | 0 | 4600 | 96 | 40 | — | — |
| Laufwasser (Turbine > 10 MW) | 0 | 3200 | 64 | 80 | — | — |
| Laufwasser (Turbine > 10 MW) | 0 | 3200 | 64 | 80 | — | — |
| Laufwasser (Turbine, < 10 MW) | 0 | 3200 | 64 | 80 | — | — |
| Laufwasser (Turbine, < 10 MW) | 0 | 3200 | 64 | 80 | — | — |
| Speicherwasser bestehend (Turbine) | 0 | 3000 | 30 | 80 | — | — |
| Speicherwasser bestehend (Turbine) | 0 | 3000 | 30 | 80 | — | — |
| Pumpspeicher (Turbine) | 0 | 3640 | 9 | 80 | — | — |

[PAGE 81]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Pumpspeicher (Turbine) | 0 | 3640 | 9 | 80 | — | — |
| Pumpspeicher (Pumpe) | 0 | 1890 | 9 | 80 | — | — |
| Pumpspeicher (Pumpe) | 0 | 1890 | 9 | 80 | — | — |
| Pumpspeicher neu (Turbine) | 0 | 3640 | 9 | 80 | — | — |
| Pumpspeicher neu (Pumpe) | 0 | 1890 | 9 | 80 | 0 | 2 |
| Gasturbine H2 | 0.002 | 538 | 24 | 23 | 0 | — |
| Gas-Kombi H2 | 0.002 | 979 | 43 | 23 | 0 | — |
| Gasturbine CH4 (OCGT) | 0.002 | 489 | 21 | 23 | 0 | — |
| Elektrolyse (NE3) | 0 | 1419 | 42 | 15 | 0 | 34 |
| Elektrolyse (Laufwasser gross) | 0 | 1419 | 42 | 15 | 0 | 10 |
| H2-Heizung (PH) | 0 | 765 | 1 | 25 | 0 | 18 |
| Kohleverbrennung (SH) | 0 | 650 | 13 | 25 | 0 | 0 |
| Kohleverbrennung (DHW) | 0 | 650 | 13 | 25 | 0 | 0 |
| Kohleverbrennung (PH) | 0 | 650 | 13 | 25 | 0 | 10 |
| Abfallverbrennung (SH) | 0 | 650 | 13 | 25 | 0 | 0 |
| Abfallverbrennung (DHW) | 0 | 650 | 13 | 25 | 0 | 0 |
| Abfallverbrennung (PH) | 0 | 650 | 13 | 25 | 0 | 25 |

[PAGE 82]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Abfall-Brenner (PH) | 0 | 650 | 13 | 25 | 0 | 25 |
| Gasheizung (SH) | 0 | 1900 | 114 | 20 | 0 | 22 |
| Gasheizung (SH) | 0 | 1900 | 114 | 20 | 0 | 6 |
| Gasheizung (DHW) | 0 | 1900 | 114 | 25 | 0 | 3 |
| Gasheizung (DHW) | 0 | 1900 | 114 | 25 | 0 | 1 |
| Gasheizung (PH) | 0 | 300 | 9 | 25 | 0 | 8 |
| Gasheizung (PH) | 0 | 300 | 9 | 25 | 0 | 6 |
| Holzheizung (SH) | 0 | 2600 | 156 | 25 | 0 | 7 |
| Holzheizung (SH) | 0 | 2600 | 156 | 25 | 0 | 4 |
| Holzheizung (DHW) | 0 | 2600 | 156 | 25 | 0 | 3 |
| Holzheizung (DHW) | 0 | 2600 | 156 | 25 | 0 | 3 |
| Holzheizung (PH) | 0.003 | 500 | 15 | 25 | 0 | 23 |
| Holzheizung (PH) | 0.003 | 500 | 15 | 25 | 0 | 23 |
| El. Heizung (SH) | 0.001 | 65 | 1 | 25 | 0 | 35 |
| El. Heizung (SH) | 0.001 | 65 | 1 | 25 | 0 | 1 |
| El. Heizung (DHW) | 0.001 | 65 | 1 | 25 | 0 | 21 |
| El. Heizung (DHW) | 0.001 | 65 | 1 | 25 | 0 | 1 |

[PAGE 83]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| El. Heizung (PH) | 0 | 275 | 1 | 30 | 0 | 28 |
| El. Heizung (PH) | 0 | 275 | 1 | 30 | 0 | 44 |
| Ölheizung (SH) | 0.001 | 2500 | 150 | 20 | 0 | 25 |
| Ölheizung (DHW) | 0.001 | 2500 | 150 | 20 | 0 | 3 |
| Ölheizung (PH) | 0.001 | 300 | 9 | 17 | 0 | 3 |
| Fernwärme (SH) | 0 | 1400 | 14 | 40 | 0 | 35 |
| Fernwärme (SH) | 0 | 1400 | 14 | 40 | 0 | 140 |
| Fernwärme (DHW) | 0 | 1400 | 14 | 40 | 0 | 5 |
| Fernwärme (DHW) | 0 | 1400 | 14 | 40 | 0 | 30 |
| Fernwärme (PH) | 0 | 1400 | 14 | 40 | 0 | 13 |
| Fernwärme (PH) | 0 | 1400 | 14 | 40 | 0 | 40 |
| Biogas Netzeinspeisung (Hochdruck) | 0.001 | — | — | — | — | — |
| Biogas Netzeinspeisung (Hochdruck) | 0.001 | — | — | — | — | — |
| Gasnetz (Hochdruck-Niederdruck) | 0.029 | — | — | — | — | — |
| Gasnetz (Hochdruck-Niederdruck) | 0.022 | — | — | — | — | — |
| H2-Netz (Verteilung CH) | 0.005 | — | — | — | — | — |
| H2-Tankstelle | 0.017 | — | — | — | — | — |

[PAGE 84]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Prozessdampf KVA | 0 | 10 | 0 | 40 | 0 | 1800 |
| Prozessdampf KVA | 0 | 10 | 0 | 40 | 0 | 1800 |
| Fernwärme KVA | 0 | 10 | 0 | 40 | 0 | 2200 |
| Fernwärme KVA | 0 | 10 | 0 | 40 | 0 | 2200 |
| CCS Stahl/Chemie | 0.08 | 3000 | 120 | 20 | — | — |
| CCS DAC Schweiz | 0.125 | 15000 | 225 | 25 | 0 | 5 |
| CO2 Transport & Speicherung (bis 0.5 Mt) | -0.153 | — | — | — | — | — |
| CO2 Transport & Speicherung (bis 2 Mt) | -0.145 | — | — | — | — | — |
| CO2 Transport & Speicherung (bis 5 Mt) | -0.143 | — | — | — | — | — |
| CO2 Transport & Speicherung (über 5 Mt) | -0.12 | — | — | — | — | — |
| CCS DAC Ausland | 0.182 | — | — | — | — | — |
| Klimatisierung (Wärmepumpe) | 0 | 0 | 0 | 20 | 0 | 1 |
| Klimatisierung (Wärmepumpe) | 0 | 0 | 0 | 20 | 0 | 3 |
| Klimatisierung (Elektro AC) | 0 | 962 | 4 | 20 | 0 | 11 |
| Klimatisierung (Elektro AC) | 0 | 962 | 4 | 20 | 0 | 11 |
| Wärmepumpe (SH) | 0 | 2700 | 81 | 20 | 0 | 6 |
| Wärmepumpe (SH) | 0 | 2700 | 81 | 20 | 0 | 30 |

[PAGE 85]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Wärmepumpe (DHW) | 0 | 2700 | 81 | 20 | 0 | 1 |
| Wärmepumpe (DHW) | 0 | 2700 | 81 | 20 | 0 | 8 |
| Grosswärmepumpe (FW) | 0 | 800 | 12 | 20 | 0 | 11 |
| Wärmepumpe (PH) | 0 | 800 | 12 | 20 | 0 | 0 |
| Wärmepumpe (PH) | 0 | 800 | 12 | 20 | 0 | 1 |
| Wind | 0 | 2500 | 39 | 25 | — | 0 |
| Wind | 0 | 1253 | 21 | 25 | 1 | 2 |
| PV Dach | 0 | 2647 | 41 | 33 | 2 | 2 |
| PV Dach | 0 | 1020 | 21 | 33 | 0 | 34 |
| PV alpin | 0 | 3031 | 56 | 33 | 1 | 2 |
| PV Freifläche (utility) | 0 | 536 | 12 | 33 | 0 | 2 |
| Solarthermie (gross) | 0 | 1588 | 31 | 33 | 0 | 1946 |
| Solarthermie (gross) | 0 | 612 | 12 | 33 | 0 | 1946 |
| Solarthermie (klein) | 0 | 3970 | 79 | 33 | 0 | 1946 |
| Solarthermie (klein) | 0 | 1530 | 30 | 33 | 0 | 1946 |
| Netzebene El1 - NE1 | 0.001 | 530 | 13 | 40 | — | — |
| Netzebene El1 - NE1 | 0.001 | 1394 | 34 | 40 | — | — |

[PAGE 86]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Trafo NE2 (1-3) | 0.001 | 104 | 2 | 35 | — | — |
| Trafo NE2 (1-3) | 0.001 | 113 | 2 | 35 | — | — |
| Trafo NE2 (3-1) | 0.001 | 0 | 0 | 35 | — | — |
| Trafo NE2 (3-1) | 0.001 | 0 | 0 | 35 | — | — |
| Netzebene El3 - NE3 | 0.001 | 633 | 18 | 40 | — | — |
| Netzebene El3 - NE3 | 0.001 | 763 | 22 | 40 | — | — |
| Trafo NE4 (3-5) | 0.001 | 354 | 10 | 35 | — | — |
| Trafo NE4 (3-5) | 0.001 | 364 | 10 | 35 | — | — |
| Trafo NE4 (5-3) | 0.001 | 0 | 0 | 35 | — | — |
| Trafo NE4 (5-3) | 0.001 | 0 | 0 | 35 | — | — |
| Netzebene NE5 - El5 | 0.001 | 1178 | 41 | 40 | — | — |
| Netzebene NE5 - El5 | 0.001 | 1296 | 45 | 40 | — | — |
| Trafo NE6 (5-7) | 0.001 | 622 | 21 | 35 | — | — |
| Trafo NE6 (5-7) | 0.001 | 699 | 24 | 35 | — | — |
| Trafo NE6 (7-5) | 0.001 | 0 | 0 | 35 | — | — |
| Trafo NE6 (7-5) | 0.001 | 0 | 0 | 35 | — | — |
| Netzebene NE7 - El7 | 0.001 | 2233 | 122 | 40 | — | — |

[PAGE 87]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Netzebene NE7 - El7 | 0.001 | 2487 | 136 | 40 | — | — |
| Netzebene El7 - NE7 | 0.001 | 0 | 0 | 40 | — | — |
| Netzebene El7 - NE7 | 0.001 | 0 | 0 | 40 | — | — |
| Netzebene El1 - NE1 (invest) | 0.001 | 357 | 8 | 40 | 0 | 30 |
| Netzebene El1 - NE1 (invest) | 0.001 | 715 | 17 | 40 | 0 | 30 |
| Trafo NE2 (1-3) (invest) | 0.001 | 104 | 2 | 35 | 0 | 30 |
| Trafo NE2 (1-3) (invest) | 0.001 | 209 | 5 | 35 | 0 | 30 |
| Trafo NE2 (3-1) (invest) | 0.001 | 0 | 0 | 35 | 0 | 30 |
| Trafo NE2 (3-1) (invest) | 0.001 | 0 | 0 | 35 | 0 | 30 |
| Netzebene El3 - NE3 (invest) | 0.001 | 209 | 6 | 40 | 0 | 30 |
| Netzebene El3 - NE3 (invest) | 0.001 | 418 | 12 | 40 | 0 | 30 |
| Trafo NE4 (3-5) (invest) | 0.001 | 354 | 10 | 35 | 0 | 30 |
| Trafo NE4 (3-5) (invest) | 0.001 | 709 | 21 | 35 | 0 | 30 |
| Trafo NE4 (5-3) (invest) | 0.001 | 0 | 0 | 35 | 0 | 30 |
| Trafo NE4 (5-3) (invest) | 0.001 | 0 | 0 | 35 | 0 | 30 |
| Netzebene NE5 - El5 (invest) | 0.001 | 523 | 18 | 40 | 0 | 30 |
| Netzebene NE5 - El5 (invest) | 0.001 | 1047 | 36 | 40 | 0 | 30 |

[PAGE 88]

| Technologie | VOM (CHF/kWh) | CAPEX (CHF/kW) | FOM (CHF/kW/Jahr) | Amortisation (Jahre) | Min. Kapazität (GW) | Max. Kapazität (GW) |
|---|---|---|---|---|---|---|
| Trafo NE6 (5-7) (invest) | 0.001 | 622 | 21 | 35 | 0 | 30 |
| Trafo NE6 (5-7) (invest) | 0.001 | 1245 | 43 | 35 | 0 | 30 |
| Trafo NE6 (7-5) (invest) | 0.001 | 0 | 0 | 35 | 0 | 30 |
| Trafo NE6 (7-5) (invest) | 0.001 | 0 | 0 | 35 | 0 | 30 |
| Netzebene NE7 - El7 (invest) | 0.001 | 837 | 46 | 40 | 0 | 30 |
| Netzebene NE7 - El7 (invest) | 0.001 | 1675 | 92 | 40 | 0 | 30 |
| Netzebene El7 - NE7 (invest) | 0.001 | 0 | 0 | 40 | 0 | 30 |
| Netzebene El7 - NE7 (invest) | 0.001 | 0 | 0 | 40 | 0 | 30 |