# Campaign statistics

Program versions in the run files: # FemRcModel.dll sha256=5287c0c7c5b69f4e8e105f190db0faaf7f053e2ec1b4bd3d244b57b6b98413ad built=2026-10-30T16:22:10Z

## Scenario fibre, fR x 0.70

| Method | runs | feasible | best | mean | worst | std | FEs |
|---|---|---|---|---|---|---|---|
| YAKO | 30 | 26 | 1547091 | 1668568 | 1814508 | 61210 | 10000 |
| BTO | 30 | 26 | 1418477 | 1482169 | 1526897 | 21856 | 10000 |
| ATO | 30 | 28 | 1498499 | 1581880 | 1693140 | 36663 | 10000 |

## Scenario fibre, fR x 0.85

| Method | runs | feasible | best | mean | worst | std | FEs |
|---|---|---|---|---|---|---|---|
| YAKO | 30 | 28 | 1546395 | 1669786 | 1808120 | 60504 | 10000 |
| BTO | 30 | 26 | 1429071 | 1480404 | 1518924 | 22809 | 10000 |
| ATO | 30 | 24 | 1510082 | 1583823 | 1691054 | 48806 | 10000 |

## Scenario fibre, fR x 1.00

| Method | runs | feasible | best | mean | worst | std | FEs |
|---|---|---|---|---|---|---|---|
| YAKO | 30 | 24 | 1574772 | 1666269 | 1801235 | 66279 | 10000 |
| BTO | 30 | 28 | 1445798 | 1485387 | 1535488 | 20455 | 10000 |
| ATO | 30 | 27 | 1511190 | 1579380 | 1644003 | 33296 | 10000 |

## Scenario plain, fR x 1.00

| Method | runs | feasible | best | mean | worst | std | FEs |
|---|---|---|---|---|---|---|---|
| YAKO | 30 | 27 | 1501179 | 1640793 | 1751440 | 67898 | 10000 |
| BTO | 30 | 28 | 1390219 | 1453026 | 1498636 | 20876 | 10000 |
| ATO | 30 | 27 | 1464873 | 1534231 | 1590402 | 29869 | 10000 |

## Mann-Whitney U (two-sided) with Holm correction (m = 21 tests)

| Comparison | U | p | p (Holm) | mean A | mean B |
|---|---|---|---|---|---|
| methods YAKO vs BTO, fibre fR 0.70 | 900 | 3.02e-11 | 6.342e-10 | 1668568 | 1482169 |
| methods YAKO vs ATO, fibre fR 0.70 | 799 | 2.572e-07 | 3.087e-06 | 1668568 | 1581880 |
| methods BTO vs ATO, fibre fR 0.70 | 8 | 6.696e-11 | 1.004e-09 | 1482169 | 1581880 |
| methods YAKO vs BTO, fibre fR 0.85 | 900 | 3.02e-11 | 6.342e-10 | 1669786 | 1480404 |
| methods YAKO vs ATO, fibre fR 0.85 | 782 | 9.533e-07 | 8.58e-06 | 1669786 | 1583823 |
| methods BTO vs ATO, fibre fR 0.85 | 4 | 4.504e-11 | 7.657e-10 | 1480404 | 1583823 |
| methods YAKO vs BTO, fibre fR 1.00 | 900 | 3.02e-11 | 6.342e-10 | 1666269 | 1485387 |
| methods YAKO vs ATO, fibre fR 1.00 | 796 | 3.256e-07 | 3.581e-06 | 1666269 | 1579380 |
| methods BTO vs ATO, fibre fR 1.00 | 7 | 6.066e-11 | 9.705e-10 | 1485387 | 1579380 |
| methods YAKO vs BTO, plain fR 1.00 | 900 | 3.02e-11 | 6.342e-10 | 1640793 | 1453026 |
| methods YAKO vs ATO, plain fR 1.00 | 813 | 8.352e-08 | 1.086e-06 | 1640793 | 1534231 |
| methods BTO vs ATO, plain fR 1.00 | 12 | 9.919e-11 | 1.389e-09 | 1453026 | 1534231 |
| YAKO plain vs fibre fR 0.70 | 358 | 0.1761 | 0.5284 | 1640793 | 1668568 |
| YAKO plain vs fibre fR 0.85 | 365 | 0.2116 | 0.5284 | 1640793 | 1669786 |
| YAKO plain vs fibre fR 1.00 | 368 | 0.2282 | 0.5284 | 1640793 | 1666269 |
| BTO plain vs fibre fR 0.70 | 142 | 5.462e-06 | 3.823e-05 | 1453026 | 1482169 |
| BTO plain vs fibre fR 0.85 | 171 | 3.831e-05 | 0.0001915 | 1453026 | 1480404 |
| BTO plain vs fibre fR 1.00 | 115 | 7.599e-07 | 7.599e-06 | 1453026 | 1485387 |
| ATO plain vs fibre fR 0.70 | 125 | 1.606e-06 | 1.285e-05 | 1534231 | 1581880 |
| ATO plain vs fibre fR 0.85 | 197 | 0.0001892 | 0.0007566 | 1534231 | 1583823 |
| ATO plain vs fibre fR 1.00 | 144 | 6.283e-06 | 3.823e-05 | 1534231 | 1579380 |
