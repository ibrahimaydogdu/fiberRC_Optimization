# Örnek 2 (Ö2) kampanya istatistikleri (betimsel; ön kayıt EX2_SPEC.md §11.1, campaign3)

Üretici: `03_Code_Execution/tools/ex2_stats.py` (bu dosya otomatik üretilir; elle düzenlenmez). Kaynak: `04_Results/ex2/campaign3/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv` (her dosyanın 3. satırı `cost`, `T`, `fibreKg`; 4. satırı `x`; `stability` satırı kat Q). Koşu başına satırlar: `04_Results/ex2/campaign3/runs.csv`.

Kurallar: algoritma üstünlüğü testi yok (AGENTS kural 8, §11.1 kural 7); "optimum" yok; maliyet = son en iyi uygun çözümün toplam maliyeti (TL, ÇŞİDB 2026). Lifli uzay lifsiz uzayı kapsadığından lifli ↔ lifsiz test "arama verimliliği" ile "lif etkisi"ni ayıramaz (§11 madde 2).

## 1. Kabul / kalite denetimi (§11.1)

| Hücre | Yöntem | n | FEs = 10 000 | T = 0 (uygun) |
|---|---|---|---|---|
| plain | YAKO | 30 | 30/30 | 30/30 |
| plain | BTO | 30 | 30/30 | 30/30 |
| plain | ATO | 30 | 30/30 | 30/30 |
| fibre 90 TL/kg | YAKO | 30 | 30/30 | 29/30 |
| fibre 90 TL/kg | BTO | 30 | 30/30 | 30/30 |
| fibre 90 TL/kg | ATO | 30 | 30/30 | 30/30 |
| fibre 0 TL/kg | YAKO | 30 | 30/30 | 30/30 |
| fibre 0 TL/kg | BTO | 30 | 30/30 | 30/30 |
| fibre 0 TL/kg | ATO | 30 | 30/30 | 30/30 |

### 1.1 `violation_counts` (ön kayıt kabul denetimi; arama süresince kısıtı ihlal eden değerlendirme sayısı, hücre × yöntem 30 koşu toplamı)

Bu sayılar en iyi çözüme değil, arama sırasında değerlendirilen tüm tasarımlara aittir (her koşuda 10 000 FE). En iyi çözümler §1'de T = 0 sütunuyla denetlenir.

| Hücre | Yöntem | g1 col P-M D/C | g2 col rho <= 4 % | g3 beam eps_t >= 0.004 | g3 beam section capacity | g4 beam shear Vs <= 0.66 sqrt(fc) bw d | g5 beam deflection after attachment <= l/480 | g6 col 2nd-order M <= 1.4 x 1st-order M |
|---|---|---|---|---|---|---|---|---|
| plain | YAKO | 1958 (30 koşu) | 3193 (30 koşu) | 77030 (30 koşu) | 49175 (30 koşu) | 1067 (29 koşu) | 45904 (30 koşu) | 45344 (30 koşu) |
| plain | BTO | 4706 (30 koşu) | 8350 (30 koşu) | 38486 (30 koşu) | 25139 (30 koşu) | 149 (30 koşu) | 16614 (30 koşu) | 37995 (30 koşu) |
| plain | ATO | 3698 (30 koşu) | 4607 (30 koşu) | 10877 (30 koşu) | 5278 (30 koşu) | 46 (18 koşu) | 13515 (30 koşu) | 53161 (30 koşu) |
| fibre 90 TL/kg | YAKO | 3977 (30 koşu) | 6276 (30 koşu) | 83565 (30 koşu) | 65508 (30 koşu) | 56995 (30 koşu) | 57848 (30 koşu) | 51604 (30 koşu) |
| fibre 90 TL/kg | BTO | 5055 (30 koşu) | 8759 (30 koşu) | 39015 (30 koşu) | 27857 (30 koşu) | 8814 (30 koşu) | 17805 (30 koşu) | 40944 (30 koşu) |
| fibre 90 TL/kg | ATO | 3635 (30 koşu) | 4567 (30 koşu) | 11803 (30 koşu) | 7075 (30 koşu) | 5854 (30 koşu) | 18224 (30 koşu) | 54591 (30 koşu) |
| fibre 0 TL/kg | YAKO | 4094 (30 koşu) | 6410 (30 koşu) | 83636 (30 koşu) | 65533 (30 koşu) | 57124 (30 koşu) | 56960 (30 koşu) | 51653 (30 koşu) |
| fibre 0 TL/kg | BTO | 5370 (30 koşu) | 9286 (30 koşu) | 39141 (30 koşu) | 36052 (30 koşu) | 34494 (30 koşu) | 17775 (30 koşu) | 39108 (30 koşu) |
| fibre 0 TL/kg | ATO | 4325 (30 koşu) | 5387 (30 koşu) | 13189 (30 koşu) | 9656 (30 koşu) | 8820 (30 koşu) | 37841 (30 koşu) | 62065 (30 koşu) |

Sütun adları (tam): `g1 col P-M D/C (ACI 22.4; 6.6.4.5)`; `g2 col rho <= 4 % (grid bound)`; `g3 beam eps_t >= 0.004 (ACI 9.3.3.1)`; `g3 beam section capacity (As finite)`; `g4 beam shear Vs <= 0.66 sqrt(fc) bw d (ACI 22.5.1.2)`; `g5 beam deflection after attachment <= l/480 (ACI 24.2; Table 24.2.2)`; `g6 col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)`.


## 2. Hücre başına betimsel istatistik (TL; uygun koşular)

| Hücre | Yöntem | n | En iyi | Ortalama | Medyan | Kötü | Std. sapma | Lif > 0 koşu | Lif kg (min–maks) | Farklı etkin en iyi tasarım | Yöntemin kendi en düşüğüne ulaşan koşu | Farklı ham x |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| plain | YAKO | 30 | 236711.62 | 237325.54 | 237501.83 | 238574.45 | 619.96 | — | — | 19 | 8 | 19 |
| plain | BTO | 30 | 236456.22 | 236503.93 | 236456.22 | 236754.53 | 98.79 | — | — | 4 | 16 | 4 |
| plain | ATO | 30 | 236456.22 | 236490.27 | 236456.22 | 236711.62 | 88.30 | — | — | 2 | 26 | 2 |
| fibre 90 TL/kg | YAKO | 29 | 237603.78 | 240546.46 | 240223.78 | 244011.91 | 1695.09 | 4/29 | 0–50 | 29 | 1 | 29 |
| fibre 90 TL/kg | BTO | 30 | 236456.22 | 236502.22 | 236468.59 | 236754.53 | 89.24 | 0/30 | 0–0 | 6 | 12 | 18 |
| fibre 90 TL/kg | ATO | 30 | 236456.22 | 236515.81 | 236456.22 | 236711.62 | 109.87 | 0/30 | 0–0 | 2 | 23 | 12 |
| fibre 0 TL/kg | YAKO | 30 | 233916.61 | 236003.12 | 236112.16 | 237746.74 | 1231.92 | 30/30 | 190–248 | 29 | 1 | 29 |
| fibre 0 TL/kg | BTO | 30 | 233916.61 | 233961.20 | 233939.51 | 234162.52 | 71.27 | 30/30 | 227–248 | 9 | 12 | 10 |
| fibre 0 TL/kg | ATO | 30 | 233916.61 | 233920.50 | 233916.61 | 233984.93 | 15.03 | 30/30 | 227–248 | 3 | 28 | 3 |

### 2.1 Lif > 0 olan koşular (hücre, yöntem, koşu, maliyet, lif kg, kiriş grubu tip/doz)

- fibre 90 TL/kg YAKO: 4/30 koşuda lif > 0; run04 240680.06 TL, 50 kg (B_S3 5D/25); run14 242331.45 TL, 50 kg (B_S2 5D/25); run22 243059.18 TL, 50 kg (B_S2 5D/25); run25 244011.91 TL, 50 kg (B_S2 5D/25)
- fibre 90 TL/kg BTO: 0/30 koşuda lif > 0
- fibre 90 TL/kg ATO: 0/30 koşuda lif > 0
- fibre 0 TL/kg YAKO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 5D/35 B_S2 5D/35 B_S3 5D/35 ×5; B_S1 5D/35 B_S2 4D/40 B_S3 5D/35 ×3; B_S1 5D/35 B_S2 5D/25 B_S3 5D/35 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×2; B_S1 4D/40 B_S2 4D/40 B_S3 5D/30 ×2; B_S1 5D/25 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 4D/40 B_S2 5D/30 B_S3 5D/35 ×1; B_S1 5D/25 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 5D/30 B_S2 5D/35 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 5D/25 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 5D/25 B_S3 4D/40 ×1; B_S1 5D/35 B_S2 5D/30 B_S3 4D/40 ×1; B_S1 5D/35 B_S2 5D/35 B_S3 5D/25 ×1; B_S1 5D/25 B_S2 5D/35 B_S3 5D/35 ×1; B_S1 5D/25 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×1; B_S1 5D/35 B_S2 5D/30 B_S3 5D/25 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 5D/30 B_S3 4D/40 ×1
- fibre 0 TL/kg BTO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×22; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×4; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×2; B_S1 5D/35 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×1
- fibre 0 TL/kg ATO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×28; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1

## 3. Yöntem içi senaryo karşılaştırması (Mann–Whitney U iki yönlü; Holm, tek aile m = 9)

A12 = P(birinci grubun maliyeti < ikinci grubun). Fark = medyan(ikinci) − medyan(birinci); GA95 = bağımsız bootstrap (2 000, tohum 2468). İki grubun tüm değerleri aynı ise (bağ; 30/30 aynı son çözüm) test **uygulanamaz** ve p/A12 sütunlarına "uygulanamaz (bağ)" yazılır (Holm hesabında p = 1); standart sapması 0 olan hücrelerde p ve A12 etki büyüklüğü değil, "30/30 aynı son çözüm" bilgisidir. fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırır (lif fiyatı maliyete girer); ön kayıtta olduğu için verilir, yorum için kullanılmaz. Yöntemler arası test YAPILMADI.

| Yöntem | Birinci | İkinci | n1 | n2 | Medyan 1 (TL) | Medyan 2 (TL) | Fark (TL) | Fark (%) | GA95 fark (TL) | p | p (Holm) | A12 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| YAKO | plain | fibre 90 TL/kg | 30 | 29 | 237501.83 | 240223.78 | 2721.95 | 1.146 | 2404.33 … 3925.53 | 2.44e-10 | 9.75e-10 | 0.980 |
| YAKO | plain | fibre 0 TL/kg | 30 | 30 | 237501.83 | 236112.16 | -1389.66 | -0.585 | -2216.77 … -73.58 | 3.09e-05 | 9.26e-05 | 0.187 |
| YAKO | fibre 90 TL/kg | fibre 0 TL/kg | 29 | 30 | 240223.78 | 236112.16 | -4111.62 | -1.712 | -5230.74 … -3206.29 | 5.47e-11 | 2.73e-10 | 0.002 |
| BTO | plain | fibre 90 TL/kg | 30 | 30 | 236456.22 | 236468.59 | 12.37 | 0.005 | -12.37 … 12.37 | 0.401 | 0.653 | 0.559 |
| BTO | plain | fibre 0 TL/kg | 30 | 30 | 236456.22 | 233939.51 | -2516.71 | -1.064 | -2551.98 … -2512.16 | 1.42e-11 | 9.95e-11 | 0.000 |
| BTO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 236468.59 | 233939.51 | -2529.08 | -1.070 | -2551.98 … -2512.16 | 1.66e-11 | 9.98e-11 | 0.000 |
| ATO | plain | fibre 90 TL/kg | 30 | 30 | 236456.22 | 236456.22 | 0.00 | 0.000 | 0.00 … 0.00 | 0.326 | 0.653 | 0.550 |
| ATO | plain | fibre 0 TL/kg | 30 | 30 | 236456.22 | 233916.61 | -2539.61 | -1.074 | -2539.61 … -2539.61 | 1.94e-13 | 1.75e-12 | 0.000 |
| ATO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 236456.22 | 233916.61 | -2539.61 | -1.074 | -2539.61 … -2539.61 | 4.23e-13 | 3.38e-12 | 0.000 |

## 4. Çapraz teyit (sıralama değil): üç yöntemin en düşük maliyetli tasarımları

Etkin tasarım = kolon b×h; kiriş b×h; lif yalnız doz > 0 ise (tip/onarılmış doz). Hamming = ham x vektörlerinde farklı gen sayısı.

### plain

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 236711.62 | 8 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x550; B_S3 250x600 | `plain/EX2_YAKO_plain_g9_fr1.00_fp90_run03.csv` |
| BTO | 236456.22 | 16 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_BTO_plain_g9_fr1.00_fp90_run01.csv` |
| ATO | 236456.22 | 26 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 2, ATO–YAKO 2, ATO–BTO 0.

Hücre genel en düşük: 236456.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/22/30; BTO 25/30/30; ATO 26/30/30.

### fibre 90 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 237603.78 | 1 | C_Edge_S1 300x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x550 | `fibre_fp90/EX2_YAKO_fibre_g9_fr1.00_fp90_run27.csv` |
| BTO | 236456.22 | 12 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_BTO_fibre_g9_fr1.00_fp90_run01.csv` |
| ATO | 236456.22 | 23 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 6, ATO–YAKO 5, ATO–BTO 1.

Hücre genel en düşük: 236456.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/1/4; BTO 27/30/30; ATO 23/30/30.

### fibre 0 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 233916.61 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_YAKO_fibre_g9_fr1.00_fp0_run25.csv` |
| BTO | 233916.61 | 12 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_BTO_fibre_g9_fr1.00_fp0_run01.csv` |
| ATO | 233916.61 | 28 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **EVET**. Ham x Hamming: BTO–YAKO 0, ATO–YAKO 0, ATO–BTO 0.

Hücre genel en düşük: 233916.61 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 2/10/15; BTO 28/30/30; ATO 30/30/30.

## 5. Kat stabilitesi (PCR-22; §A.5: kaç en iyi tasarımda ötelenmeli kat çıktığı)

| Hücre | Yöntem | Ötelenmeli katı olan koşu | Ötelenmeli kat sayısı dağılımı (0/1/2/3) | Q en büyük (tüm koşular) | Kat ötelenmesi en büyük mutlak değer (mm) |
|---|---|---|---|---|---|
| plain | YAKO | 30/30 | 0/0/30/0 | 0.0858 | 0.5838 |
| plain | BTO | 30/30 | 0/0/30/0 | 0.0851 | 0.2689 |
| plain | ATO | 30/30 | 0/0/30/0 | 0.0845 | 0.2689 |
| fibre 90 TL/kg | YAKO | 30/30 | 0/1/27/2 | 0.0900 | 0.8122 |
| fibre 90 TL/kg | BTO | 30/30 | 0/0/30/0 | 0.0851 | 0.2708 |
| fibre 90 TL/kg | ATO | 30/30 | 0/0/30/0 | 0.0845 | 0.2689 |
| fibre 0 TL/kg | YAKO | 30/30 | 0/0/18/12 | 0.0950 | 0.5429 |
| fibre 0 TL/kg | BTO | 30/30 | 0/0/26/4 | 0.0908 | 0.2897 |
| fibre 0 TL/kg | ATO | 30/30 | 0/0/30/0 | 0.0900 | 0.2680 |

