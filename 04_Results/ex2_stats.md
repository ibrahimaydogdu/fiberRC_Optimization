# Örnek 2 (Ö2) kampanya istatistikleri (betimsel; ön kayıt EX2_SPEC.md §11.1)

Üretici: `03_Code_Execution/tools/ex2_stats.py` (bu dosya otomatik üretilir; elle düzenlenmez). Kaynak: `04_Results/ex2/campaign/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv` (her dosyanın 3. satırı `cost`, `T`, `fibreKg`; 4. satırı `x`; `stability` satırı kat Q). Koşu başına satırlar: `04_Results/ex2/analysis/ex2_runs.csv`.

Kurallar: algoritma üstünlüğü testi yok (AGENTS kural 8, §11.1 kural 7); "optimum" yok; maliyet = son en iyi uygun çözümün toplam maliyeti (TL, ÇŞİDB 2026). Lifli uzay lifsiz uzayı kapsadığından lifli ↔ lifsiz test "arama verimliliği" ile "lif etkisi"ni ayıramaz (§11 madde 2).

## 1. Kabul / kalite denetimi (§11.1)

| Hücre | Yöntem | n | FEs = 10 000 | T = 0 (uygun) |
|---|---|---|---|---|
| plain | YAKO | 30 | 30/30 | 30/30 |
| plain | BTO | 30 | 30/30 | 30/30 |
| plain | ATO | 30 | 30/30 | 30/30 |
| fibre 90 TL/kg | YAKO | 30 | 30/30 | 30/30 |
| fibre 90 TL/kg | BTO | 30 | 30/30 | 30/30 |
| fibre 90 TL/kg | ATO | 30 | 30/30 | 30/30 |
| fibre 0 TL/kg | YAKO | 30 | 30/30 | 30/30 |
| fibre 0 TL/kg | BTO | 30 | 30/30 | 30/30 |
| fibre 0 TL/kg | ATO | 30 | 30/30 | 30/30 |

### 1.1 `violation_counts` (ön kayıt kabul denetimi; arama süresince kısıtı ihlal eden değerlendirme sayısı, hücre × yöntem 30 koşu toplamı)

Bu sayılar en iyi çözüme değil, arama sırasında değerlendirilen tüm tasarımlara aittir (her koşuda 10 000 FE). En iyi çözümler §1'de T = 0 sütunuyla denetlenir.

| Hücre | Yöntem | g1 col P-M D/C | g2 col rho <= 4 % | g3 beam eps_t >= 0.004 | g3 beam section capacity | g4 beam shear Vs <= 0.66 sqrt(fc) bw d | g6 col 2nd-order M <= 1.4 x 1st-order M |
|---|---|---|---|---|---|---|---|
| plain | YAKO | 1833 (30 koşu) | 3014 (30 koşu) | 75741 (30 koşu) | 47514 (30 koşu) | 0 (0 koşu) | 46413 (30 koşu) |
| plain | BTO | 3750 (30 koşu) | 5369 (30 koşu) | 39217 (30 koşu) | 24652 (30 koşu) | 0 (0 koşu) | 38244 (30 koşu) |
| plain | ATO | 1164 (30 koşu) | 2389 (30 koşu) | 14554 (30 koşu) | 6681 (30 koşu) | 0 (0 koşu) | 46679 (30 koşu) |
| fibre 90 TL/kg | YAKO | 3491 (30 koşu) | 5719 (30 koşu) | 85821 (30 koşu) | 67241 (30 koşu) | 58779 (30 koşu) | 50429 (30 koşu) |
| fibre 90 TL/kg | BTO | 3555 (30 koşu) | 5519 (30 koşu) | 40444 (30 koşu) | 28438 (30 koşu) | 9672 (30 koşu) | 36863 (30 koşu) |
| fibre 90 TL/kg | ATO | 1335 (30 koşu) | 2586 (30 koşu) | 21517 (30 koşu) | 13986 (30 koşu) | 11491 (30 koşu) | 46918 (30 koşu) |
| fibre 0 TL/kg | YAKO | 3399 (30 koşu) | 5658 (30 koşu) | 83218 (30 koşu) | 66350 (30 koşu) | 58267 (30 koşu) | 51127 (30 koşu) |
| fibre 0 TL/kg | BTO | 4299 (30 koşu) | 6003 (30 koşu) | 40622 (30 koşu) | 37226 (30 koşu) | 35785 (30 koşu) | 37592 (30 koşu) |
| fibre 0 TL/kg | ATO | 2339 (30 koşu) | 5120 (30 koşu) | 29100 (30 koşu) | 24750 (30 koşu) | 23614 (30 koşu) | 71835 (30 koşu) |

Sütun adları (tam): `g1 col P-M D/C (ACI 22.4; 6.6.4.5)`; `g2 col rho <= 4 % (grid bound)`; `g3 beam eps_t >= 0.004 (ACI 9.3.3.1)`; `g3 beam section capacity (As finite)`; `g4 beam shear Vs <= 0.66 sqrt(fc) bw d (ACI 22.5.1.2)`; `g6 col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)`.


## 2. Hücre başına betimsel istatistik (TL; uygun koşular)

| Hücre | Yöntem | n | En iyi | Ortalama | Medyan | Kötü | Std. sapma | Lif > 0 koşu | Lif kg (min–maks) | Farklı etkin en iyi tasarım | Yöntemin kendi en düşüğüne ulaşan koşu |
|---|---|---|---|---|---|---|---|---|---|---|---|
| plain | YAKO | 30 | 230325.40 | 231229.29 | 231096.56 | 232473.74 | 593.41 | — | — | 25 | 1 |
| plain | BTO | 30 | 230308.35 | 230331.89 | 230308.35 | 230800.29 | 88.85 | — | — | 4 | 16 |
| plain | ATO | 30 | 230308.35 | 230308.35 | 230308.35 | 230308.35 | 0.00 | — | — | 1 | 30 |
| fibre 90 TL/kg | YAKO | 30 | 230800.51 | 234718.73 | 235123.78 | 237662.87 | 1977.49 | 5/30 | 0–45 | 30 | 1 |
| fibre 90 TL/kg | BTO | 30 | 230308.35 | 230328.28 | 230308.35 | 230695.32 | 69.80 | 0/30 | 0–0 | 4 | 16 |
| fibre 90 TL/kg | ATO | 30 | 230308.35 | 230308.35 | 230308.35 | 230308.35 | 0.00 | 0/30 | 0–0 | 1 | 30 |
| fibre 0 TL/kg | YAKO | 30 | 228333.82 | 229752.03 | 229701.14 | 232471.39 | 1018.10 | 30/30 | 194–242 | 30 | 1 |
| fibre 0 TL/kg | BTO | 30 | 228294.22 | 228328.46 | 228340.77 | 228371.39 | 28.81 | 30/30 | 230–264 | 7 | 11 |
| fibre 0 TL/kg | ATO | 30 | 228294.22 | 228298.40 | 228294.22 | 228340.77 | 12.80 | 30/30 | 230–240 | 4 | 27 |

### 2.1 Lif > 0 olan koşular (hücre, yöntem, koşu, maliyet, lif kg, kiriş grubu tip/doz)

- fibre 90 TL/kg YAKO: 5/30 koşuda lif > 0; run05 235315.29 TL, 45 kg (B_S2 5D/25); run09 237662.46 TL, 45 kg (B_S2 5D/25); run22 235784.24 TL, 45 kg (B_S3 5D/25); run24 236521.32 TL, 45 kg (B_S2 5D/25); run29 235186.10 TL, 40 kg (B_S2 4D/20)
- fibre 90 TL/kg BTO: 0/30 koşuda lif > 0
- fibre 90 TL/kg ATO: 0/30 koşuda lif > 0
- fibre 0 TL/kg YAKO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 5D/35 B_S3 5D/35 ×6; B_S1 5D/35 B_S2 4D/40 B_S3 5D/35 ×5; B_S1 5D/35 B_S2 5D/35 B_S3 5D/35 ×3; B_S1 5D/25 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 4D/40 B_S2 5D/30 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×2; B_S1 4D/20 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 5D/25 B_S2 4D/40 B_S3 5D/30 ×1; B_S1 5D/25 B_S2 5D/35 B_S3 5D/35 ×1; B_S1 5D/35 B_S2 4D/40 B_S3 5D/25 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×1; B_S1 5D/35 B_S2 5D/30 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/25 ×1; B_S1 5D/35 B_S2 4D/40 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×1
- fibre 0 TL/kg BTO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×27; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×2; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1
- fibre 0 TL/kg ATO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×28; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1

## 3. Yöntem içi senaryo karşılaştırması (Mann–Whitney U iki yönlü; Holm, tek aile m = 9)

A12 = P(birinci grubun maliyeti < ikinci grubun). Fark = medyan(ikinci) − medyan(birinci); GA95 = bağımsız bootstrap (2 000, tohum 2468). İki grubun tüm değerleri aynı ise (bağ; 30/30 aynı son çözüm) test **uygulanamaz** ve p/A12 sütunlarına "uygulanamaz (bağ)" yazılır (Holm hesabında p = 1); standart sapması 0 olan hücrelerde p ve A12 etki büyüklüğü değil, "30/30 aynı son çözüm" bilgisidir. fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırır (lif fiyatı maliyete girer); ön kayıtta olduğu için verilir, yorum için kullanılmaz. Yöntemler arası test YAPILMADI.

| Yöntem | Birinci | İkinci | n1 | n2 | Medyan 1 (TL) | Medyan 2 (TL) | Fark (TL) | Fark (%) | GA95 fark (TL) | p | p (Holm) | A12 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| YAKO | plain | fibre 90 TL/kg | 30 | 30 | 231096.56 | 235123.78 | 4027.22 | 1.743 | 2642.70 … 4811.40 | 2.43e-09 | 9.71e-09 | 0.949 |
| YAKO | plain | fibre 0 TL/kg | 30 | 30 | 231096.56 | 229701.14 | -1395.42 | -0.604 | -1875.73 … -786.47 | 3.63e-08 | 1.09e-07 | 0.086 |
| YAKO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 235123.78 | 229701.14 | -5422.64 | -2.306 | -6235.66 … -3980.88 | 6.07e-11 | 3.03e-10 | 0.008 |
| BTO | plain | fibre 90 TL/kg | 30 | 30 | 230308.35 | 230308.35 | 0.00 | 0.000 | -17.04 … 17.04 | 0.915 | 1 | 0.492 |
| BTO | plain | fibre 0 TL/kg | 30 | 30 | 230308.35 | 228340.77 | -1967.58 | -0.854 | -2014.13 … -1967.58 | 1.3e-11 | 9.13e-11 | 0.000 |
| BTO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 230308.35 | 228340.77 | -1967.58 | -0.854 | -2014.13 … -1967.58 | 1.35e-11 | 9.13e-11 | 0.000 |
| ATO | plain | fibre 90 TL/kg | 30 | 30 | 230308.35 | 230308.35 | 0.00 | 0.000 | 0.00 … 0.00 | uygulanamaz (bağ) | uygulanamaz (bağ) | uygulanamaz (bağ) |
| ATO | plain | fibre 0 TL/kg | 30 | 30 | 230308.35 | 228294.22 | -2014.13 | -0.875 | -2014.13 … -2014.13 | 6.14e-14 | 5.53e-13 | 0.000 |
| ATO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 230308.35 | 228294.22 | -2014.13 | -0.875 | -2014.13 … -2014.13 | 6.14e-14 | 5.53e-13 | 0.000 |

## 4. Çapraz teyit (sıralama değil): üç yöntemin en düşük maliyetli tasarımları

Etkin tasarım = kolon b×h; kiriş b×h; lif yalnız doz > 0 ise (tip/onarılmış doz). Hamming = ham x vektörlerinde farklı gen sayısı.

### plain

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 230325.40 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_YAKO_plain_g9_fr1.00_fp90_run27.csv` |
| BTO | 230308.35 | 16 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_BTO_plain_g9_fr1.00_fp90_run01.csv` |
| ATO | 230308.35 | 30 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 2, ATO–YAKO 2, ATO–BTO 0.

Hücre genel en düşük: 230308.35 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 3/21/30; BTO 29/30/30; ATO 30/30/30.

### fibre 90 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 230800.51 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x550; B_S3 250x550 | `fibre_fp90/EX2_YAKO_fibre_g9_fr1.00_fp90_run03.csv` |
| BTO | 230308.35 | 16 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_BTO_fibre_g9_fr1.00_fp90_run01.csv` |
| ATO | 230308.35 | 30 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 4, ATO–YAKO 6, ATO–BTO 2.

Hücre genel en düşük: 230308.35 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/2/4; BTO 29/30/30; ATO 30/30/30.

### fibre 0 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 228333.82 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x500 4D/40; B_S3 250x500 5D/35 | `fibre_fp0/EX2_YAKO_fibre_g9_fr1.00_fp0_run23.csv` |
| BTO | 228294.22 | 11 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x500 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_BTO_fibre_g9_fr1.00_fp0_run01.csv` |
| ATO | 228294.22 | 27 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x500 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 2, ATO–YAKO 2, ATO–BTO 0.

Hücre genel en düşük: 228294.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 6/10/24; BTO 30/30/30; ATO 30/30/30.

## 5. Kat stabilitesi (PCR-22; §A.5: kaç en iyi tasarımda ötelenmeli kat çıktığı)

| Hücre | Yöntem | Ötelenmeli katı olan koşu | Ötelenmeli kat sayısı dağılımı (0/1/2/3) | Q en büyük (tüm koşular) | Kat ötelenmesi en büyük mutlak değer (mm) |
|---|---|---|---|---|---|
| plain | YAKO | 30/30 | 0/0/28/2 | 0.0911 | 0.0000 |
| plain | BTO | 30/30 | 0/0/30/0 | 0.0840 | 0.0000 |
| plain | ATO | 30/30 | 0/0/30/0 | 0.0822 | 0.0000 |
| fibre 90 TL/kg | YAKO | 30/30 | 0/0/22/8 | 0.0976 | 0.0000 |
| fibre 90 TL/kg | BTO | 30/30 | 0/0/30/0 | 0.0840 | 0.0000 |
| fibre 90 TL/kg | ATO | 30/30 | 0/0/30/0 | 0.0822 | 0.0000 |
| fibre 0 TL/kg | YAKO | 30/30 | 0/0/6/24 | 0.1046 | 0.0000 |
| fibre 0 TL/kg | BTO | 30/30 | 0/0/5/25 | 0.0959 | 0.0000 |
| fibre 0 TL/kg | ATO | 30/30 | 0/0/0/30 | 0.0950 | 0.0000 |

