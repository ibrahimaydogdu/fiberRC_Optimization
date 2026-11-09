# Phase 2 statistics (10 000 FE total, pre-design FEs included; PCR-12 one-dimension moves for BTO/ATO)

Best known (this study, feasible, not an optimum): **1390219 TL**. Plateau = checkpoint after which every later checkpoint improves the median by < 1 % (a settling point, not an optimum).

## Scenario plain

| Arm | Method | n | infeasible at 10 000 | median 1000 | 2000 | 3000 | 5000 | 10000 | 95 % CI (10000) | best | worst | impr. 1k→2k | 2k→3k | 3k→5k | 5k→10k | plateau | gap median to best known (10000) |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| A | YAKO | 30 | 0 | 1784096 | 1630545 | 1577139 | 1535207 | 1469429 | 1459797–1476510 | 1416263 | 1530392 | 8.61 % | 3.28 % | 2.66 % | 4.28 % | not reached | 5.7 % |
| A | BTO | 30 | 0 | 1851338 | 1737675 | 1676417 | 1596565 | 1503872 | 1496928–1508079 | 1437115 | 1540322 | 6.14 % | 3.53 % | 4.76 % | 5.81 % | not reached | 8.2 % |
| A | ATO | 30 | 0 | 1766107 | 1608804 | 1535671 | 1470347 | 1440825 | 1435575–1449957 | 1397348 | 1488356 | 8.91 % | 4.55 % | 4.25 % | 2.01 % | not reached | 3.6 % |
| C | YAKO | 30 | 0 | 1646599 | 1612604 | 1579948 | 1529809 | 1478110 | 1449994–1489544 | 1427353 | 1525983 | 2.06 % | 2.03 % | 3.17 % | 3.38 % | not reached | 6.3 % |
| C | BTO | 30 | 0 | 1662283 | 1645300 | 1634728 | 1597868 | 1493623 | 1476801–1500755 | 1455593 | 1545849 | 1.02 % | 0.64 % | 2.25 % | 6.52 % | not reached | 7.4 % |
| C | ATO | 30 | 0 | 1607277 | 1556167 | 1512927 | 1450622 | 1427864 | 1420498–1441349 | 1402677 | 1498977 | 3.18 % | 2.78 % | 4.12 % | 1.57 % | not reached | 2.7 % |
| ABC | YAKO | 30 | 0 | 1656152 | 1619857 | 1593799 | 1537518 | 1475203 | 1466938–1488014 | 1441140 | 1529675 | 2.19 % | 1.61 % | 3.53 % | 4.05 % | not reached | 6.1 % |
| ABC | BTO | 30 | 0 | 1660536 | 1629237 | 1599706 | 1547582 | 1483992 | 1468641–1492141 | 1437148 | 1514510 | 1.88 % | 1.81 % | 3.26 % | 4.11 % | not reached | 6.7 % |
| ABC | ATO | 30 | 0 | 1613978 | 1551129 | 1510767 | 1478607 | 1462542 | 1456326–1472646 | 1436861 | 1500873 | 3.89 % | 2.60 % | 2.13 % | 1.09 % | not reached | 5.2 % |

Baseline-A (random start, 44 variables, 10 000 FE, feasible-run median): YAKO 1653364 (n=30), BTO 1451542 (n=30), ATO 1540297 (n=30)

## Scenario fibre

| Arm | Method | n | infeasible at 10 000 | median 1000 | 2000 | 3000 | 5000 | 10000 | 95 % CI (10000) | best | worst | impr. 1k→2k | 2k→3k | 3k→5k | 5k→10k | plateau | gap median to best known (10000) |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| A | YAKO | 30 | 0 | 1902464 | 1784897 | 1700881 | 1630694 | 1536957 | 1517971–1561860 | 1454526 | 1627993 | 6.18 % | 4.71 % | 4.13 % | 5.75 % | not reached | 10.6 % |
| A | BTO | 30 | 0 | 2070258 | 1925471 | 1822975 | 1715935 | 1590540 | 1576699–1602775 | 1525863 | 1671322 | 6.99 % | 5.32 % | 5.87 % | 7.31 % | not reached | 14.4 % |
| A | ATO | 30 | 0 | 1906359 | 1688247 | 1633206 | 1533917 | 1467811 | 1443586–1486623 | 1400212 | 1617982 | 11.44 % | 3.26 % | 6.08 % | 4.31 % | not reached | 5.6 % |
| C | YAKO | 30 | 0 | 1651566 | 1631725 | 1614218 | 1584024 | 1535180 | 1522187–1545272 | 1472299 | 1588916 | 1.20 % | 1.07 % | 1.87 % | 3.08 % | not reached | 10.4 % |
| C | BTO | 30 | 0 | 1664921 | 1656152 | 1645741 | 1628307 | 1575109 | 1561886–1584693 | 1496934 | 1623429 | 0.53 % | 0.63 % | 1.06 % | 3.27 % | not reached | 13.3 % |
| C | ATO | 30 | 0 | 1621525 | 1582215 | 1558447 | 1523790 | 1466229 | 1454720–1481751 | 1415180 | 1529740 | 2.42 % | 1.50 % | 2.22 % | 3.78 % | not reached | 5.5 % |
| ABC | YAKO | 30 | 0 | 1658788 | 1647458 | 1634332 | 1601704 | 1536005 | 1527712–1550700 | 1482076 | 1568814 | 0.68 % | 0.80 % | 2.00 % | 4.10 % | not reached | 10.5 % |
| ABC | BTO | 30 | 0 | 1663845 | 1657470 | 1642510 | 1610568 | 1545385 | 1533223–1556471 | 1500918 | 1585159 | 0.38 % | 0.90 % | 1.94 % | 4.05 % | not reached | 11.2 % |
| ABC | ATO | 30 | 0 | 1631271 | 1596739 | 1556567 | 1522510 | 1477556 | 1464506–1484169 | 1427998 | 1517028 | 2.12 % | 2.52 % | 2.19 % | 2.95 % | not reached | 6.3 % |

Baseline-A (random start, 44 variables, 10 000 FE, feasible-run median): YAKO 1640554 (n=30), BTO 1483300 (n=30), ATO 1585763 (n=30)

## Tests at 10 000 FE (Mann-Whitney U, two-sided, Holm over m = 36; A12 = P(first group cheaper))

| Comparison | p | p (Holm) | A12 |
|---|---|---|---|
| plain YAKO: A vs C | 0.4376 | 1 | 0.56 |
| plain BTO: A vs C | 0.02709 | 0.5417 | 0.33 |
| plain ATO: A vs C | 0.03265 | 0.6204 | 0.34 |
| plain pooled: A vs C | 0.3473 | 1 | 0.46 |
| plain YAKO: A vs ABC | 0.1297 | 1 | 0.61 |
| plain BTO: A vs ABC | 9.211e-05 | 0.002119 | 0.21 |
| plain ATO: A vs ABC | 8.146e-05 | 0.001955 | 0.80 |
| plain pooled: A vs ABC | 0.5078 | 1 | 0.53 |
| plain YAKO: C vs ABC | 0.3871 | 1 | 0.57 |
| plain BTO: C vs ABC | 0.03387 | 0.6204 | 0.34 |
| plain ATO: C vs ABC | 7.599e-07 | 1.976e-05 | 0.87 |
| plain pooled: C vs ABC | 0.1082 | 1 | 0.57 |
| fibre YAKO: A vs C | 0.6309 | 1 | 0.46 |
| fibre BTO: A vs C | 0.0251 | 0.5271 | 0.33 |
| fibre ATO: A vs C | 0.9823 | 1 | 0.50 |
| fibre pooled: A vs C | 0.2676 | 1 | 0.45 |
| fibre YAKO: A vs ABC | 0.8418 | 1 | 0.48 |
| fibre BTO: A vs ABC | 3.805e-07 | 1.027e-05 | 0.12 |
| fibre ATO: A vs ABC | 0.2905 | 1 | 0.58 |
| fibre pooled: A vs ABC | 0.05155 | 0.8248 | 0.42 |
| fibre YAKO: C vs ABC | 0.5444 | 1 | 0.55 |
| fibre BTO: C vs ABC | 0.0001365 | 0.003003 | 0.21 |
| fibre ATO: C vs ABC | 0.234 | 1 | 0.59 |
| fibre pooled: C vs ABC | 0.2918 | 1 | 0.45 |
| arm A YAKO: fibre vs plain | 1.102e-08 | 3.087e-07 | 0.07 |
| arm A BTO: fibre vs plain | 5.494e-11 | 1.813e-09 | 0.01 |
| arm A ATO: fibre vs plain | 0.03387 | 0.6204 | 0.34 |
| arm A pooled: fibre vs plain | 2.655e-11 | 9.026e-10 | 0.21 |
| arm C YAKO: fibre vs plain | 2.034e-09 | 5.898e-08 | 0.05 |
| arm C BTO: fibre vs plain | 2.61e-10 | 8.091e-09 | 0.02 |
| arm C ATO: fibre vs plain | 4.787e-05 | 0.001197 | 0.19 |
| arm C pooled: fibre vs plain | 2.21e-13 | 7.734e-12 | 0.18 |
| arm ABC YAKO: fibre vs plain | 8.101e-10 | 2.43e-08 | 0.04 |
| arm ABC BTO: fibre vs plain | 7.389e-11 | 2.365e-09 | 0.01 |
| arm ABC ATO: fibre vs plain | 0.09926 | 1 | 0.38 |
| arm ABC pooled: fibre vs plain | 3.379e-14 | 1.216e-12 | 0.17 |
