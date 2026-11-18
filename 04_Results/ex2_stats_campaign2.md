# Örnek 2 (Ö2) kampanya istatistikleri (betimsel; ön kayıt EX2_SPEC.md §11.1, kampanya 2 = post-hoc §11.2 (PCR-23/24/25), ikili Ex2Model a61b0c6b…)

Üretici: `03_Code_Execution/tools/ex2_stats.py` (bu dosya otomatik üretilir; elle düzenlenmez). Kaynak: `04_Results/ex2/campaign2/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv` (her dosyanın 3. satırı `cost`, `T`, `fibreKg`; 4. satırı `x`; `stability` satırı kat Q). Koşu başına satırlar: `04_Results/ex2/analysis2/ex2_runs_campaign2.csv`.

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
| plain | YAKO | 2020 (30 koşu) | 3418 (30 koşu) | 78075 (30 koşu) | 49665 (30 koşu) | 1132 (28 koşu) | 45926 (30 koşu) |
| plain | BTO | 5352 (30 koşu) | 9129 (30 koşu) | 39495 (30 koşu) | 25355 (30 koşu) | 164 (30 koşu) | 38741 (30 koşu) |
| plain | ATO | 3481 (30 koşu) | 4386 (30 koşu) | 14296 (30 koşu) | 6536 (30 koşu) | 96 (21 koşu) | 50543 (30 koşu) |
| fibre 90 TL/kg | YAKO | 3958 (30 koşu) | 6368 (30 koşu) | 85181 (30 koşu) | 66993 (30 koşu) | 58553 (30 koşu) | 53583 (30 koşu) |
| fibre 90 TL/kg | BTO | 5528 (30 koşu) | 9361 (30 koşu) | 40639 (30 koşu) | 28799 (30 koşu) | 9700 (30 koşu) | 38728 (30 koşu) |
| fibre 90 TL/kg | ATO | 3844 (30 koşu) | 4808 (30 koşu) | 19582 (30 koşu) | 12749 (30 koşu) | 10483 (30 koşu) | 54798 (30 koşu) |
| fibre 0 TL/kg | YAKO | 3993 (30 koşu) | 6536 (30 koşu) | 84043 (30 koşu) | 66434 (30 koşu) | 57498 (30 koşu) | 50617 (30 koşu) |
| fibre 0 TL/kg | BTO | 5639 (30 koşu) | 9826 (30 koşu) | 40522 (30 koşu) | 37258 (30 koşu) | 35891 (30 koşu) | 39728 (30 koşu) |
| fibre 0 TL/kg | ATO | 4134 (30 koşu) | 5268 (30 koşu) | 21406 (30 koşu) | 17018 (30 koşu) | 15958 (30 koşu) | 61529 (30 koşu) |

Sütun adları (tam): `g1 col P-M D/C (ACI 22.4; 6.6.4.5)`; `g2 col rho <= 4 % (grid bound)`; `g3 beam eps_t >= 0.004 (ACI 9.3.3.1)`; `g3 beam section capacity (As finite)`; `g4 beam shear Vs <= 0.66 sqrt(fc) bw d (ACI 22.5.1.2)`; `g6 col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)`.


## 2. Hücre başına betimsel istatistik (TL; uygun koşular)

| Hücre | Yöntem | n | En iyi | Ortalama | Medyan | Kötü | Std. sapma | Lif > 0 koşu | Lif kg (min–maks) | Farklı etkin en iyi tasarım | Yöntemin kendi en düşüğüne ulaşan koşu | Farklı ham x |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| plain | YAKO | 30 | 236711.62 | 237353.01 | 237494.78 | 238480.10 | 596.76 | — | — | 21 | 8 | 21 |
| plain | BTO | 30 | 236456.22 | 236501.44 | 236462.41 | 236711.62 | 82.51 | — | — | 5 | 15 | 5 |
| plain | ATO | 30 | 236456.22 | 236541.35 | 236456.22 | 236711.62 | 122.45 | — | — | 2 | 20 | 2 |
| fibre 90 TL/kg | YAKO | 30 | 238260.94 | 241036.50 | 240977.61 | 244247.53 | 1625.10 | 4/30 | 0–50 | 30 | 1 | 30 |
| fibre 90 TL/kg | BTO | 30 | 236456.22 | 236509.57 | 236468.59 | 236851.04 | 104.56 | 0/30 | 0–0 | 6 | 10 | 18 |
| fibre 90 TL/kg | ATO | 30 | 236456.22 | 236515.81 | 236456.22 | 236711.62 | 109.87 | 0/30 | 0–0 | 2 | 23 | 13 |
| fibre 0 TL/kg | YAKO | 30 | 234012.58 | 235788.18 | 235917.55 | 238039.98 | 964.59 | 30/30 | 137–248 | 30 | 1 | 30 |
| fibre 0 TL/kg | BTO | 30 | 233916.61 | 233964.26 | 233944.06 | 234145.12 | 62.44 | 30/30 | 217–248 | 11 | 9 | 12 |
| fibre 0 TL/kg | ATO | 30 | 233916.61 | 233921.25 | 233916.61 | 233964.93 | 14.16 | 30/30 | 238–248 | 3 | 27 | 3 |

### 2.1 Lif > 0 olan koşular (hücre, yöntem, koşu, maliyet, lif kg, kiriş grubu tip/doz)

- fibre 90 TL/kg YAKO: 4/30 koşuda lif > 0; run04 241237.38 TL, 45 kg (B_S1 5D/25); run10 241683.57 TL, 50 kg (B_S3 5D/25); run19 241555.09 TL, 40 kg (B_S3 4D/20); run21 243304.46 TL, 50 kg (B_S1 5D/25)
- fibre 90 TL/kg BTO: 0/30 koşuda lif > 0
- fibre 90 TL/kg ATO: 0/30 koşuda lif > 0
- fibre 0 TL/kg YAKO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×3; B_S1 5D/35 B_S2 5D/35 B_S3 5D/35 ×3; B_S1 5D/35 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 4D/40 B_S3 5D/35 ×2; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×2; B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 5D/30 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 5D/25 ×2; B_S1 5D/35 B_S2 4D/35 B_S3 4D/40 ×1; B_S1 5D/25 B_S2 5D/35 B_S3 5D/30 ×1; B_S1 4D/40 B_S2 5D/25 B_S3 4D/40 ×1; B_S1 5D/25 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 5D/25 B_S2 4D/40 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 5D/35 ×1; B_S1 5D/30 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 5D/35 B_S2 -/0 B_S3 5D/30 ×1; B_S1 5D/30 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 5D/25 B_S2 4D/40 B_S3 5D/35 ×1
- fibre 0 TL/kg BTO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×20; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×4; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 4D/35 ×1; B_S1 4D/35 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1
- fibre 0 TL/kg ATO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×27; B_S1 5D/35 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1

## 3. Yöntem içi senaryo karşılaştırması (Mann–Whitney U iki yönlü; Holm, tek aile m = 9)

A12 = P(birinci grubun maliyeti < ikinci grubun). Fark = medyan(ikinci) − medyan(birinci); GA95 = bağımsız bootstrap (2 000, tohum 2468). İki grubun tüm değerleri aynı ise (bağ; 30/30 aynı son çözüm) test **uygulanamaz** ve p/A12 sütunlarına "uygulanamaz (bağ)" yazılır (Holm hesabında p = 1); standart sapması 0 olan hücrelerde p ve A12 etki büyüklüğü değil, "30/30 aynı son çözüm" bilgisidir. fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırır (lif fiyatı maliyete girer); ön kayıtta olduğu için verilir, yorum için kullanılmaz. Yöntemler arası test YAPILMADI.

| Yöntem | Birinci | İkinci | n1 | n2 | Medyan 1 (TL) | Medyan 2 (TL) | Fark (TL) | Fark (%) | GA95 fark (TL) | p | p (Holm) | A12 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| YAKO | plain | fibre 90 TL/kg | 30 | 30 | 237494.78 | 240977.61 | 3482.84 | 1.466 | 2578.65 … 4691.31 | 4.72e-11 | 1.89e-10 | 0.994 |
| YAKO | plain | fibre 0 TL/kg | 30 | 30 | 237494.78 | 235917.55 | -1577.23 | -0.664 | -2086.15 … -836.35 | 2.97e-08 | 8.9e-08 | 0.083 |
| YAKO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 240977.61 | 235917.55 | -5060.06 | -2.100 | -6110.02 … -4091.74 | 3.02e-11 | 1.51e-10 | 0.000 |
| BTO | plain | fibre 90 TL/kg | 30 | 30 | 236462.41 | 236468.59 | 6.19 | 0.003 | 0.00 … 12.37 | 0.373 | 0.745 | 0.563 |
| BTO | plain | fibre 0 TL/kg | 30 | 30 | 236462.41 | 233944.06 | -2518.35 | -1.065 | -2529.08 … -2512.16 | 1.69e-11 | 1.18e-10 | 0.000 |
| BTO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 236468.59 | 233944.06 | -2524.53 | -1.068 | -2529.08 … -2514.43 | 1.72e-11 | 1.18e-10 | 0.000 |
| ATO | plain | fibre 90 TL/kg | 30 | 30 | 236456.22 | 236456.22 | 0.00 | 0.000 | -127.70 … 0.00 | 0.399 | 0.745 | 0.450 |
| ATO | plain | fibre 0 TL/kg | 30 | 30 | 236456.22 | 233916.61 | -2539.61 | -1.074 | -2667.31 … -2539.61 | 9.65e-13 | 7.72e-12 | 0.000 |
| ATO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 236456.22 | 233916.61 | -2539.61 | -1.074 | -2539.61 … -2539.61 | 5.88e-13 | 5.29e-12 | 0.000 |

## 4. Çapraz teyit (sıralama değil): üç yöntemin en düşük maliyetli tasarımları

Etkin tasarım = kolon b×h; kiriş b×h; lif yalnız doz > 0 ise (tip/onarılmış doz). Hamming = ham x vektörlerinde farklı gen sayısı.

### plain

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 236711.62 | 8 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x550; B_S3 250x600 | `plain/EX2_YAKO_plain_g9_fr1.00_fp90_run03.csv` |
| BTO | 236456.22 | 15 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_BTO_plain_g9_fr1.00_fp90_run02.csv` |
| ATO | 236456.22 | 20 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_ATO_plain_g9_fr1.00_fp90_run02.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 2, ATO–YAKO 2, ATO–BTO 0.

Hücre genel en düşük: 236456.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/21/30; BTO 27/30/30; ATO 20/30/30.

### fibre 90 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 238260.94 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x350; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x350; B_S1 250x550; B_S2 250x550; B_S3 250x550 | `fibre_fp90/EX2_YAKO_fibre_g9_fr1.00_fp90_run11.csv` |
| BTO | 236456.22 | 10 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_BTO_fibre_g9_fr1.00_fp90_run01.csv` |
| ATO | 236456.22 | 23 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 7, ATO–YAKO 5, ATO–BTO 2.

Hücre genel en düşük: 236456.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/0/2; BTO 26/30/30; ATO 23/30/30.

### fibre 0 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 234012.58 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 5D/35; B_S2 250x550 5D/35; B_S3 250x500 4D/40 | `fibre_fp0/EX2_YAKO_fibre_g9_fr1.00_fp0_run04.csv` |
| BTO | 233916.61 | 9 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_BTO_fibre_g9_fr1.00_fp0_run01.csv` |
| ATO | 233916.61 | 27 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 | `fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 6, ATO–YAKO 6, ATO–BTO 0.

Hücre genel en düşük: 233916.61 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 1/7/23; BTO 30/30/30; ATO 30/30/30.

## 5. Kat stabilitesi (PCR-22; §A.5: kaç en iyi tasarımda ötelenmeli kat çıktığı)

| Hücre | Yöntem | Ötelenmeli katı olan koşu | Ötelenmeli kat sayısı dağılımı (0/1/2/3) | Q en büyük (tüm koşular) | Kat ötelenmesi en büyük mutlak değer (mm) |
|---|---|---|---|---|---|
| plain | YAKO | 30/30 | 0/0/29/1 | 0.0883 | 0.4761 |
| plain | BTO | 30/30 | 0/0/30/0 | 0.0845 | 0.2708 |
| plain | ATO | 30/30 | 0/0/30/0 | 0.0845 | 0.2689 |
| fibre 90 TL/kg | YAKO | 30/30 | 0/1/23/6 | 0.0945 | 0.7662 |
| fibre 90 TL/kg | BTO | 30/30 | 0/0/30/0 | 0.0858 | 0.2689 |
| fibre 90 TL/kg | ATO | 30/30 | 0/0/30/0 | 0.0845 | 0.2689 |
| fibre 0 TL/kg | YAKO | 30/30 | 0/0/11/19 | 0.0994 | 0.7217 |
| fibre 0 TL/kg | BTO | 30/30 | 0/0/27/3 | 0.0908 | 0.2897 |
| fibre 0 TL/kg | ATO | 30/30 | 0/0/30/0 | 0.0900 | 0.2680 |

