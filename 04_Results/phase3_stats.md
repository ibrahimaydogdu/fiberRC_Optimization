# Phase 3 statistics (descriptive; pre-design FEs included in totals)

## Part 1: MaxFE curves (10 runs per cell, seeds identical at 20k and 30k)

Plateau is not an optimum. Cells settled at 20k (pre-registered rule) were not re-run at 30k.

| Scenario | Arm | Method | n(20k) | n(30k) | med 10k | 15k | 20k | 25k | 30k | impr 10→20 | 20→30 | 25→30 | CI95 impr 10→30 | infeasible at last cp | status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| fibre | A | ATO | 10 | 10 | 1424465 | 1419680 | 1418829 | 1418829 | 1418829 | 0.40 % | 0.00 % | 0.00 % | 0.00–2.24 % | 0 | 30k run |
| fibre | A | YAKO | 10 | 10 | 1538676 | 1491419 | 1470848 | 1458352 | 1455201 | 4.41 % | 1.06 % | 0.22 % | 3.52–7.56 % | 0 | 30k run |
| fibre | ABC | ATO | 10 | 10 | 1475249 | 1461584 | 1453083 | 1453083 | 1448348 | 1.50 % | 0.33 % | 0.33 % | 0.19–3.22 % | 0 | 30k run |
| fibre | ABC | YAKO | 10 | 10 | 1532340 | 1502357 | 1480990 | 1475976 | 1466878 | 3.35 % | 0.95 % | 0.62 % | 3.06–5.47 % | 0 | 30k run |
| plain | A | ATO | 10 | 0 | 1441318 | 1437806 | 1437806 | - | - | 0.24 % | - | - | - | 0 | settled at 20k |
| plain | A | YAKO | 10 | 10 | 1472247 | 1443006 | 1428665 | 1426538 | 1425078 | 2.96 % | 0.25 % | 0.10 % | 1.48–4.77 % | 0 | 30k run |
| plain | ABC | ATO | 10 | 0 | 1456583 | 1456583 | 1456583 | - | - | 0.00 % | - | - | - | 0 | settled at 20k |
| plain | ABC | YAKO | 10 | 10 | 1466630 | 1457869 | 1452779 | 1447323 | 1447323 | 0.94 % | 0.38 % | 0.00 % | 0.39–3.42 % | 0 | 30k run |

## Part 2: all-dimension vs one-dimension (PCR-12) moves, BTO and ATO, 10 000 FE total

Supplementary (IA decision 2026-11-09/13). Cells with n < 30 are partial. Baseline-A (random start, 44 variables, other build) is context only, not in the Holm family.

| Method | Scenario | Arm | n all-dim | median all-dim | n one-dim | median one-dim | one-dim dearer by | infeasible all-dim | infeasible one-dim |
|---|---|---|---|---|---|---|---|---|---|
| ATO | fibre | A | 30 | 1439234 | 30 | 1467811 | 1.9 % | 0 | 0 |
| ATO | fibre | ABC | 30 | 1424611 | 30 | 1477556 | 3.6 % | 0 | 0 |
| ATO | fibre | C | 30 | 1447546 | 30 | 1466229 | 1.3 % | 0 | 0 |
| ATO | plain | A | 30 | 1427926 | 30 | 1440825 | 0.9 % | 0 | 0 |
| ATO | plain | ABC | 30 | 1424611 | 30 | 1462542 | 2.6 % | 0 | 0 |
| ATO | plain | C | 30 | 1423950 | 30 | 1427864 | 0.3 % | 0 | 0 |
| BTO | fibre | A | 30 | 1406575 | 30 | 1590540 | 11.6 % | 0 | 0 |
| BTO | fibre | ABC | 30 | 1434218 | 30 | 1545385 | 7.2 % | 0 | 0 |
| BTO | fibre | C | 30 | 1406962 | 30 | 1575109 | 10.7 % | 0 | 0 |
| BTO | plain | A | 30 | 1400709 | 30 | 1503872 | 6.9 % | 0 | 0 |
| BTO | plain | ABC | 30 | 1432446 | 30 | 1483992 | 3.5 % | 0 | 0 |
| BTO | plain | C | 30 | 1403512 | 30 | 1493623 | 6.0 % | 0 | 0 |

Baseline-A medians (feasible runs, context only): ATO fibre 1585763 (n=30), ATO plain 1540297 (n=30), BTO fibre 1483300 (n=30), BTO plain 1451542 (n=30)

| Comparison | p | p (Holm, m=12) | A12 (P all-dim cheaper) |
|---|---|---|---|
| ATO fibre A: all-dim vs one-dim (Phase 2) | 0.01383 | 0.02766 | 0.69 |
| ATO fibre ABC: all-dim vs one-dim (Phase 2) | 2.277e-11 | 2.505e-10 | 1.00 |
| ATO fibre C: all-dim vs one-dim (Phase 2) | 0.003501 | 0.0105 | 0.72 |
| ATO plain A: all-dim vs one-dim (Phase 2) | 0.0006549 | 0.002619 | 0.76 |
| ATO plain ABC: all-dim vs one-dim (Phase 2) | 7.857e-12 | 9.428e-11 | 1.00 |
| ATO plain C: all-dim vs one-dim (Phase 2) | 0.3953 | 0.3953 | 0.56 |
| BTO fibre A: all-dim vs one-dim (Phase 2) | 3.02e-11 | 3.007e-10 | 1.00 |
| BTO fibre ABC: all-dim vs one-dim (Phase 2) | 3.007e-11 | 3.007e-10 | 1.00 |
| BTO fibre C: all-dim vs one-dim (Phase 2) | 3.02e-11 | 3.007e-10 | 1.00 |
| BTO plain A: all-dim vs one-dim (Phase 2) | 3.02e-11 | 3.007e-10 | 1.00 |
| BTO plain ABC: all-dim vs one-dim (Phase 2) | 4.713e-11 | 3.007e-10 | 0.99 |
| BTO plain C: all-dim vs one-dim (Phase 2) | 3.02e-11 | 3.007e-10 | 1.00 |
