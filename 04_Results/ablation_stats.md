# Ablation Phase 1 statistics (1 000 FE total, pre-design FEs included)

Best known (this study, feasible, not an optimum): **1390219 TL**.

## Scenario fibre

| Arm | Method | runs | feasible | best | median | mean | worst | std | median first feasible FE | feasible in first 100 (mean) | pre-design FEs | success H1 | H2 (+5 %) | H3 (+2 %) |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| R | YAKO | 30 | 23 | 1912353 | 2296774 | 2272793 | 2639208 | 183358 | 250 | 0.5 | 0 | 0.00 | 0.00 | 0.00 |
| R | BTO | 30 | 30 | 1949782 | 2054820 | 2065761 | 2212492 | 68269 | 148 | 0.5 | 0 | 0.00 | 0.00 | 0.00 |
| R | ATO | 30 | 30 | 2015857 | 2124787 | 2118123 | 2298458 | 68784 | 230 | 0.5 | 0 | 0.00 | 0.00 | 0.00 |
| A | YAKO | 30 | 26 | 1721335 | 1902464 | 1906536 | 2491777 | 149200 | 67 | 1.2 | 0 | 0.00 | 0.00 | 0.00 |
| A | BTO | 30 | 30 | 1630743 | 1715640 | 1719213 | 1822829 | 46714 | 71 | 1.2 | 0 | 0.00 | 0.00 | 0.00 |
| A | ATO | 30 | 30 | 1639132 | 1724814 | 1720365 | 1806714 | 43517 | 82 | 1.3 | 0 | 0.00 | 0.00 | 0.00 |
| C | YAKO | 30 | 30 | 1563218 | 1651566 | 1645390 | 1664921 | 24971 | 1 | 2.2 | 44 | 1.00 | 0.00 | 0.00 |
| C | BTO | 30 | 30 | 1621269 | 1664921 | 1660544 | 1664921 | 11761 | 1 | 2.2 | 44 | 0.00 | 0.00 | 0.00 |
| C | ATO | 30 | 30 | 1606545 | 1664921 | 1662197 | 1664921 | 10635 | 1 | 2.5 | 44 | 0.00 | 0.00 | 0.00 |
| ABC | YAKO | 30 | 30 | 1631726 | 1658788 | 1655879 | 1664921 | 8328 | 1 | 16.4 | 44 | 1.00 | 0.00 | 0.00 |
| ABC | BTO | 30 | 30 | 1584887 | 1639376 | 1635765 | 1664921 | 25123 | 1 | 16.4 | 44 | 0.00 | 0.00 | 0.00 |
| ABC | ATO | 30 | 30 | 1580122 | 1623480 | 1625285 | 1664921 | 25912 | 1 | 15.5 | 44 | 0.10 | 0.00 | 0.00 |

Baseline-A (10 000 FE, random start, feasible-run median, H1 targets): YAKO 1682025, BTO 1486108, ATO 1590359

Pooled median (all methods, +inf censored): A 1747943, C 1664921, ABC 1647458, R 2125877
**Phase 2 arm by the frozen rule: ABC** (runs counted: A 90, C 90, ABC 90).

## Scenario plain

| Arm | Method | runs | feasible | best | median | mean | worst | std | median first feasible FE | feasible in first 100 (mean) | pre-design FEs | success H1 | H2 (+5 %) | H3 (+2 %) |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| R | YAKO | 30 | 25 | 1928912 | 2226203 | 2202693 | 2620698 | 181243 | 201 | 0.5 | 0 | 0.00 | 0.00 | 0.00 |
| R | BTO | 30 | 30 | 1836253 | 1959431 | 1960772 | 2074238 | 63821 | 108 | 0.5 | 0 | 0.00 | 0.00 | 0.00 |
| R | ATO | 30 | 30 | 1877889 | 1994386 | 1999139 | 2156730 | 65784 | 201 | 0.5 | 0 | 0.00 | 0.00 | 0.00 |
| A | YAKO | 30 | 30 | 1565182 | 1784096 | 1778126 | 2389699 | 148073 | 40 | 1.5 | 0 | 0.10 | 0.00 | 0.00 |
| A | BTO | 30 | 30 | 1503313 | 1635948 | 1624292 | 1702705 | 42802 | 40 | 1.5 | 0 | 0.00 | 0.00 | 0.00 |
| A | ATO | 30 | 30 | 1521132 | 1649120 | 1638803 | 1729477 | 41934 | 62 | 1.4 | 0 | 0.03 | 0.00 | 0.00 |
| C | YAKO | 30 | 30 | 1591387 | 1646599 | 1644799 | 1664921 | 21129 | 1 | 2.5 | 33 | 0.57 | 0.00 | 0.00 |
| C | BTO | 30 | 30 | 1567687 | 1639203 | 1631999 | 1664921 | 28505 | 1 | 2.5 | 33 | 0.00 | 0.00 | 0.00 |
| C | ATO | 30 | 30 | 1568806 | 1641834 | 1634273 | 1664921 | 31341 | 1 | 2.6 | 33 | 0.00 | 0.00 | 0.00 |
| ABC | YAKO | 30 | 30 | 1620013 | 1656152 | 1651164 | 1664921 | 14200 | 1 | 15.3 | 33 | 0.43 | 0.00 | 0.00 |
| ABC | BTO | 30 | 30 | 1527254 | 1580546 | 1580473 | 1613049 | 18988 | 1 | 15.3 | 33 | 0.00 | 0.00 | 0.00 |
| ABC | ATO | 30 | 30 | 1487296 | 1564591 | 1559393 | 1608368 | 32928 | 1 | 14.4 | 33 | 0.30 | 0.00 | 0.00 |

Baseline-A (10 000 FE, random start, feasible-run median, H1 targets): YAKO 1654762, BTO 1453826, ATO 1545194

Pooled median (all methods, +inf censored): A 1651929, C 1644253, ABC 1591592, R 2026241
**Phase 2 arm by the frozen rule: ABC** (runs counted: A 90, C 90, ABC 90).

## Mann-Whitney U (two-sided) with Holm correction (m = 48 tests); A12 = P(first arm cheaper)

| Comparison | p | p (Holm) | A12 |
|---|---|---|---|
| fibre pooled R vs A | 3.401e-24 | 1.394e-22 | 0.06 |
| fibre YAKO: R vs A (paired Wilcoxon p=7.63e-06) | 4.131e-06 | 2.892e-05 | 0.15 |
| fibre BTO: R vs A (paired Wilcoxon p=1.86e-09) | 3.02e-11 | 1.017e-09 | 0.00 |
| fibre ATO: R vs A (paired Wilcoxon p=1.86e-09) | 3.02e-11 | 1.017e-09 | 0.00 |
| fibre pooled R vs C | 3.151e-32 | 1.512e-30 | 0.00 |
| fibre YAKO: R vs C (paired Wilcoxon p=2.38e-07) | 2.684e-11 | 1.017e-09 | 0.00 |
| fibre BTO: R vs C (paired Wilcoxon p=1.86e-09) | 4.111e-12 | 1.644e-10 | 0.00 |
| fibre ATO: R vs C (paired Wilcoxon p=1.86e-09) | 4.111e-12 | 1.644e-10 | 0.00 |
| fibre pooled R vs ABC | 4.737e-31 | 2.179e-29 | 0.00 |
| fibre YAKO: R vs ABC (paired Wilcoxon p=2.38e-07) | 2.807e-11 | 1.017e-09 | 0.00 |
| fibre BTO: R vs ABC (paired Wilcoxon p=1.86e-09) | 3.001e-11 | 1.017e-09 | 0.00 |
| fibre ATO: R vs ABC (paired Wilcoxon p=1.86e-09) | 3.012e-11 | 1.017e-09 | 0.00 |
| fibre pooled A vs C | 1.141e-24 | 4.794e-23 | 0.07 |
| fibre YAKO: A vs C (paired Wilcoxon p=2.98e-08) | 2.763e-11 | 1.017e-09 | 0.00 |
| fibre BTO: A vs C (paired Wilcoxon p=1.02e-07) | 1.44e-08 | 1.729e-07 | 0.09 |
| fibre ATO: A vs C (paired Wilcoxon p=3.86e-07) | 2.688e-07 | 2.957e-06 | 0.13 |
| fibre pooled A vs ABC | 2.702e-26 | 1.162e-24 | 0.04 |
| fibre YAKO: A vs ABC (paired Wilcoxon p=2.98e-08) | 2.89e-11 | 1.017e-09 | 0.00 |
| fibre BTO: A vs ABC (paired Wilcoxon p=1.86e-09) | 2.656e-09 | 4.515e-08 | 0.05 |
| fibre ATO: A vs ABC (paired Wilcoxon p=1.86e-09) | 6.108e-10 | 1.16e-08 | 0.03 |
| fibre pooled C vs ABC | 4.126e-11 | 1.017e-09 | 0.23 |
| fibre YAKO: C vs ABC (paired Wilcoxon p=0.102) | 0.2792 | 1 | 0.58 |
| fibre BTO: C vs ABC (paired Wilcoxon p=5.43e-05) | 2.921e-07 | 2.957e-06 | 0.14 |
| fibre ATO: C vs ABC (paired Wilcoxon p=5.26e-06) | 3.771e-09 | 5.279e-08 | 0.08 |
| plain pooled R vs A | 4.246e-29 | 1.868e-27 | 0.02 |
| plain YAKO: R vs A (paired Wilcoxon p=1.49e-06) | 3.778e-10 | 7.555e-09 | 0.03 |
| plain BTO: R vs A (paired Wilcoxon p=1.86e-09) | 3.02e-11 | 1.017e-09 | 0.00 |
| plain ATO: R vs A (paired Wilcoxon p=1.86e-09) | 3.02e-11 | 1.017e-09 | 0.00 |
| plain pooled R vs C | 4.239e-31 | 1.992e-29 | 0.00 |
| plain YAKO: R vs C (paired Wilcoxon p=5.96e-08) | 2.686e-11 | 1.017e-09 | 0.00 |
| plain BTO: R vs C (paired Wilcoxon p=1.86e-09) | 2.982e-11 | 1.017e-09 | 0.00 |
| plain ATO: R vs C (paired Wilcoxon p=1.86e-09) | 2.865e-11 | 1.017e-09 | 0.00 |
| plain pooled R vs ABC | 4.824e-31 | 2.179e-29 | 0.00 |
| plain YAKO: R vs ABC (paired Wilcoxon p=5.96e-08) | 2.677e-11 | 1.017e-09 | 0.00 |
| plain BTO: R vs ABC (paired Wilcoxon p=1.86e-09) | 3.02e-11 | 1.017e-09 | 0.00 |
| plain ATO: R vs ABC (paired Wilcoxon p=1.86e-09) | 3.02e-11 | 1.017e-09 | 0.00 |
| plain pooled A vs C | 0.02794 | 0.1397 | 0.41 |
| plain YAKO: A vs C (paired Wilcoxon p=6.91e-07) | 3.577e-07 | 3.219e-06 | 0.12 |
| plain BTO: A vs C (paired Wilcoxon p=0.393) | 0.4917 | 1 | 0.55 |
| plain ATO: A vs C (paired Wilcoxon p=0.792) | 0.9234 | 1 | 0.49 |
| plain pooled A vs ABC | 8.549e-11 | 1.795e-09 | 0.22 |
| plain YAKO: A vs ABC (paired Wilcoxon p=2.35e-06) | 4.509e-07 | 3.607e-06 | 0.12 |
| plain BTO: A vs ABC (paired Wilcoxon p=5.59e-05) | 8.883e-06 | 5.33e-05 | 0.17 |
| plain ATO: A vs ABC (paired Wilcoxon p=4.66e-08) | 3.497e-09 | 5.246e-08 | 0.06 |
| plain pooled C vs ABC | 1.813e-09 | 3.264e-08 | 0.24 |
| plain YAKO: C vs ABC (paired Wilcoxon p=0.207) | 0.403 | 1 | 0.56 |
| plain BTO: C vs ABC (paired Wilcoxon p=6.15e-08) | 1.092e-08 | 1.42e-07 | 0.07 |
| plain ATO: C vs ABC (paired Wilcoxon p=3.73e-09) | 3.065e-09 | 4.904e-08 | 0.05 |
