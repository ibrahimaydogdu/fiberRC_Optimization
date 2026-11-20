# Örnek 2 (Ö2) kampanya istatistikleri (betimsel; ön kayıt EX2_SPEC.md §11.1, campaign4)

Üretici: `03_Code_Execution/tools/ex2_stats.py` (bu dosya otomatik üretilir; elle düzenlenmez). Kaynak: `04_Results/ex2/campaign4/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv` (her dosyanın 3. satırı `cost`, `T`, `fibreKg`; 4. satırı `x`; `stability` satırı kat Q). Koşu başına satırlar: `04_Results/ex2/campaign4/runs.csv`.

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

| Hücre | Yöntem | g1 col P-M D/C | g2 col rho <= 4 % | g3 beam eps_t >= 0.004 | g3 beam section capacity | g4 beam shear Vs <= 0.66 sqrt(fc) bw d | g5 beam deflection after attachment <= l/480 | g6 col 2nd-order M <= 1.4 x 1st-order M |
|---|---|---|---|---|---|---|---|---|
| plain | YAKO | 2051 (30 koşu) | 3384 (30 koşu) | 77064 (30 koşu) | 50010 (30 koşu) | 1272 (29 koşu) | 55941 (30 koşu) | 44243 (30 koşu) |
| plain | BTO | 4699 (30 koşu) | 8115 (30 koşu) | 38365 (30 koşu) | 25145 (30 koşu) | 161 (30 koşu) | 20972 (30 koşu) | 38376 (30 koşu) |
| plain | ATO | 3205 (30 koşu) | 3905 (30 koşu) | 8586 (30 koşu) | 3954 (30 koşu) | 46 (19 koşu) | 17156 (30 koşu) | 50800 (30 koşu) |
| fibre 90 TL/kg | YAKO | 4023 (30 koşu) | 6398 (30 koşu) | 81474 (30 koşu) | 64599 (30 koşu) | 55449 (30 koşu) | 73806 (30 koşu) | 51011 (30 koşu) |
| fibre 90 TL/kg | BTO | 5237 (30 koşu) | 8768 (30 koşu) | 38871 (30 koşu) | 27811 (30 koşu) | 8945 (30 koşu) | 23402 (30 koşu) | 40402 (30 koşu) |
| fibre 90 TL/kg | ATO | 3646 (30 koşu) | 4568 (30 koşu) | 10328 (30 koşu) | 6420 (30 koşu) | 5527 (30 koşu) | 23639 (30 koşu) | 53576 (30 koşu) |
| fibre 0 TL/kg | YAKO | 4074 (30 koşu) | 6294 (30 koşu) | 82961 (30 koşu) | 65630 (30 koşu) | 57063 (30 koşu) | 70782 (30 koşu) | 52136 (30 koşu) |
| fibre 0 TL/kg | BTO | 5259 (30 koşu) | 9387 (30 koşu) | 38968 (30 koşu) | 35924 (30 koşu) | 34190 (30 koşu) | 29241 (30 koşu) | 40289 (30 koşu) |
| fibre 0 TL/kg | ATO | 4025 (30 koşu) | 5033 (30 koşu) | 11754 (30 koşu) | 9119 (30 koşu) | 8423 (30 koşu) | 44453 (30 koşu) | 62733 (30 koşu) |

Sütun adları (tam): `g1 col P-M D/C (ACI 22.4; 6.6.4.5)`; `g2 col rho <= 4 % (grid bound)`; `g3 beam eps_t >= 0.004 (ACI 9.3.3.1)`; `g3 beam section capacity (As finite)`; `g4 beam shear Vs <= 0.66 sqrt(fc) bw d (ACI 22.5.1.2)`; `g5 beam deflection after attachment <= l/480 (ACI 24.2; Table 24.2.2; live-load patterns; Ie envelope)`; `g6 col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)`.


## 2. Hücre başına betimsel istatistik (TL; uygun koşular)

| Hücre | Yöntem | n | En iyi | Ortalama | Medyan | Kötü | Std. sapma | Lif > 0 koşu | Lif kg (min–maks) | Farklı etkin en iyi tasarım | Yöntemin kendi en düşüğüne ulaşan koşu | Farklı ham x |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| plain | YAKO | 30 | 236456.22 | 237209.41 | 236754.53 | 238654.36 | 689.97 | — | — | 19 | 1 | 19 |
| plain | BTO | 30 | 236456.22 | 236486.49 | 236456.22 | 236754.53 | 81.62 | — | — | 4 | 19 | 4 |
| plain | ATO | 30 | 236456.22 | 236515.81 | 236456.22 | 236711.62 | 109.87 | — | — | 2 | 23 | 2 |
| fibre 90 TL/kg | YAKO | 30 | 237799.80 | 241088.39 | 240757.50 | 246140.59 | 2166.59 | 4/30 | 0–70 | 30 | 1 | 30 |
| fibre 90 TL/kg | BTO | 30 | 236456.22 | 236580.09 | 236468.59 | 237835.41 | 312.39 | 0/30 | 0–0 | 7 | 10 | 19 |
| fibre 90 TL/kg | ATO | 30 | 236456.22 | 236498.79 | 236456.22 | 236711.62 | 96.81 | 0/30 | 0–0 | 2 | 25 | 10 |
| fibre 0 TL/kg | YAKO | 30 | 234655.82 | 237156.02 | 237201.74 | 239766.75 | 1306.13 | 30/30 | 170–272 | 30 | 1 | 30 |
| fibre 0 TL/kg | BTO | 30 | 234190.76 | 234281.31 | 234225.78 | 234736.09 | 154.75 | 30/30 | 202–264 | 10 | 13 | 11 |
| fibre 0 TL/kg | ATO | 30 | 234190.76 | 234196.94 | 234190.76 | 234236.02 | 12.00 | 30/30 | 245–256 | 4 | 23 | 5 |

### 2.1 Lif > 0 olan koşular (hücre, yöntem, koşu, maliyet, lif kg, kiriş grubu tip/doz)

- fibre 90 TL/kg YAKO: 4/30 koşuda lif > 0; run03 244968.15 TL, 70 kg (B_S1 5D/35); run07 242093.21 TL, 40 kg (B_S2 4D/20); run23 246140.59 TL, 48 kg (B_S3 4D/20); run26 244093.97 TL, 50 kg (B_S2 5D/25)
- fibre 90 TL/kg BTO: 0/30 koşuda lif > 0
- fibre 90 TL/kg ATO: 0/30 koşuda lif > 0
- fibre 0 TL/kg YAKO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 5D/35 B_S3 5D/35 ×5; B_S1 5D/35 B_S2 4D/40 B_S3 5D/35 ×4; B_S1 5D/35 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 5D/35 ×2; B_S1 4D/40 B_S2 5D/35 B_S3 5D/25 ×2; B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×2; B_S1 5D/35 B_S2 5D/35 B_S3 4D/40 ×2; B_S1 5D/25 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 5D/30 B_S2 5D/35 B_S3 5D/25 ×1; B_S1 5D/35 B_S2 5D/25 B_S3 5D/25 ×1; B_S1 4D/40 B_S2 5D/25 B_S3 5D/35 ×1; B_S1 5D/25 B_S2 5D/35 B_S3 5D/25 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 5D/35 B_S2 5D/35 B_S3 5D/25 ×1; B_S1 5D/25 B_S2 4D/40 B_S3 5D/25 ×1; B_S1 5D/25 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 5D/30 ×1
- fibre 0 TL/kg BTO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×22; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×2; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/30 B_S2 4D/35 B_S3 5D/30 ×1; B_S1 4D/40 B_S2 4D/35 B_S3 5D/30 ×1; B_S1 5D/35 B_S2 5D/35 B_S3 4D/35 ×1; B_S1 5D/35 B_S2 4D/40 B_S3 5D/35 ×1; B_S1 4D/40 B_S2 5D/35 B_S3 5D/35 ×1
- fibre 0 TL/kg ATO: 30/30 koşuda lif > 0; kiriş lif deseni sayımı: B_S1 4D/40 B_S2 4D/40 B_S3 4D/40 ×23; B_S1 5D/35 B_S2 4D/40 B_S3 4D/40 ×5; B_S1 4D/40 B_S2 5D/35 B_S3 4D/40 ×1; B_S1 4D/40 B_S2 4D/40 B_S3 5D/35 ×1

## 3. Yöntem içi senaryo karşılaştırması (Mann–Whitney U iki yönlü; Holm, tek aile m = 9)

A12 = P(birinci grubun maliyeti < ikinci grubun). Fark = medyan(ikinci) − medyan(birinci); GA95 = bağımsız bootstrap (2 000, tohum 2468). İki grubun tüm değerleri aynı ise (bağ; 30/30 aynı son çözüm) test **uygulanamaz** ve p/A12 sütunlarına "uygulanamaz (bağ)" yazılır (Holm hesabında p = 1); standart sapması 0 olan hücrelerde p ve A12 etki büyüklüğü değil, "30/30 aynı son çözüm" bilgisidir. fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırır (lif fiyatı maliyete girer); ön kayıtta olduğu için verilir, yorum için kullanılmaz. Yöntemler arası test YAPILMADI.

| Yöntem | Birinci | İkinci | n1 | n2 | Medyan 1 (TL) | Medyan 2 (TL) | Fark (TL) | Fark (%) | GA95 fark (TL) | p | p (Holm) | A12 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| YAKO | plain | fibre 90 TL/kg | 30 | 30 | 236754.53 | 240757.50 | 4002.97 | 1.691 | 2417.86 … 5284.01 | 7.45e-11 | 3.72e-10 | 0.989 |
| YAKO | plain | fibre 0 TL/kg | 30 | 30 | 236754.53 | 237201.74 | 447.20 | 0.189 | -565.95 … 1039.69 | 0.877 | 1 | 0.512 |
| YAKO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 240757.50 | 237201.74 | -3555.76 | -1.477 | -5095.24 … -2093.76 | 8.89e-10 | 3.56e-09 | 0.039 |
| BTO | plain | fibre 90 TL/kg | 30 | 30 | 236456.22 | 236468.59 | 12.37 | 0.005 | 0.00 … 12.37 | 0.0176 | 0.0527 | 0.666 |
| BTO | plain | fibre 0 TL/kg | 30 | 30 | 236456.22 | 234225.78 | -2230.44 | -0.943 | -2265.46 … -2219.89 | 1.01e-11 | 7.04e-11 | 0.000 |
| BTO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 236468.59 | 234225.78 | -2242.81 | -0.948 | -2277.83 … -2232.26 | 1.76e-11 | 1.05e-10 | 0.000 |
| ATO | plain | fibre 90 TL/kg | 30 | 30 | 236456.22 | 236456.22 | 0.00 | 0.000 | 0.00 … 0.00 | 0.529 | 1 | 0.467 |
| ATO | plain | fibre 0 TL/kg | 30 | 30 | 236456.22 | 234190.76 | -2265.46 | -0.958 | -2265.46 … -2265.46 | 1.63e-12 | 1.31e-11 | 0.000 |
| ATO | fibre 90 TL/kg | fibre 0 TL/kg | 30 | 30 | 236456.22 | 234190.76 | -2265.46 | -0.958 | -2265.46 … -2265.46 | 1.05e-12 | 9.49e-12 | 0.000 |

## 4. Çapraz teyit (sıralama değil): üç yöntemin en düşük maliyetli tasarımları

Etkin tasarım = kolon b×h; kiriş b×h; lif yalnız doz > 0 ise (tip/onarılmış doz). Hamming = ham x vektörlerinde farklı gen sayısı.

### plain

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 236456.22 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_YAKO_plain_g9_fr1.00_fp90_run07.csv` |
| BTO | 236456.22 | 19 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_BTO_plain_g9_fr1.00_fp90_run02.csv` |
| ATO | 236456.22 | 23 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **EVET**. Ham x Hamming: BTO–YAKO 0, ATO–YAKO 0, ATO–BTO 0.

Hücre genel en düşük: 236456.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 3/22/30; BTO 27/30/30; ATO 23/30/30.

### fibre 90 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 237799.80 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 250x400; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x550; B_S3 250x600 | `fibre_fp90/EX2_YAKO_fibre_g9_fr1.00_fp90_run14.csv` |
| BTO | 236456.22 | 10 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_BTO_fibre_g9_fr1.00_fp90_run01.csv` |
| ATO | 236456.22 | 25 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 | `fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 7, ATO–YAKO 5, ATO–BTO 2.

Hücre genel en düşük: 236456.22 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/0/3; BTO 26/29/30; ATO 25/30/30.

### fibre 0 TL/kg

| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |
|---|---|---|---|---|
| YAKO | 234655.82 | 1 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 400x300; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550 5D/35; B_S2 250x550 5D/35; B_S3 250x550 5D/35 | `fibre_fp0/EX2_YAKO_fibre_g9_fr1.00_fp0_run04.csv` |
| BTO | 234190.76 | 13 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550 4D/40; B_S2 250x500 4D/40; B_S3 250x550 4D/40 | `fibre_fp0/EX2_BTO_fibre_g9_fr1.00_fp0_run01.csv` |
| ATO | 234190.76 | 23 | C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550 4D/40; B_S2 250x500 4D/40; B_S3 250x550 4D/40 | `fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run03.csv` |

Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **HAYIR**. Ham x Hamming: BTO–YAKO 9, ATO–YAKO 9, ATO–BTO 0.

Hücre genel en düşük: 234190.76 TL. Genel en düşüğe %0,1 / %0,5 / %1 içinde kalan koşu (yöntem başına, /30): YAKO 0/4/9; BTO 25/30/30; ATO 30/30/30.

## 5. Kat stabilitesi (PCR-22; §A.5: kaç en iyi tasarımda ötelenmeli kat çıktığı)

| Hücre | Yöntem | Ötelenmeli katı olan koşu | Ötelenmeli kat sayısı dağılımı (0/1/2/3) | Q en büyük (tüm koşular) | Kat ötelenmesi en büyük mutlak değer (mm) |
|---|---|---|---|---|---|
| plain | YAKO | 30/30 | 0/0/30/0 | 0.0860 | 0.4218 |
| plain | BTO | 30/30 | 0/0/30/0 | 0.0851 | 0.2689 |
| plain | ATO | 30/30 | 0/0/30/0 | 0.0845 | 0.2689 |
| fibre 90 TL/kg | YAKO | 30/30 | 0/1/28/1 | 0.0938 | 0.5694 |
| fibre 90 TL/kg | BTO | 30/30 | 0/0/30/0 | 0.0845 | 0.2708 |
| fibre 90 TL/kg | ATO | 30/30 | 0/0/30/0 | 0.0845 | 0.2689 |
| fibre 0 TL/kg | YAKO | 30/30 | 0/0/27/3 | 0.0963 | 0.6244 |
| fibre 0 TL/kg | BTO | 30/30 | 0/0/4/26 | 0.0889 | 0.2521 |
| fibre 0 TL/kg | ATO | 30/30 | 0/0/0/30 | 0.0883 | 0.2521 |

