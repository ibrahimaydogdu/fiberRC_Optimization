# Örnek 2 (Ö2) kampanya 2 deterministik analiz ve sonuç özeti (post-hoc PCR-23/24/25)

Durum: **TASLAK, Critic + IA onayı bekler.** Bulgular bölümü kapısı (§11.1 kural 6) kapalı: V3 (en iyi çözümlerin ETABS yeniden analizi) ve S2 yapılmadı; bu belgedeki hiçbir sayı Critic + IA onayı olmadan makaleye girmez.
Hazırlayan: Claude Code (Results Analyst), 2026-11-17. Git kullanılmadı. `ex2_campaign2_bin`, `ex2_campaign_bin`, `ex2_bin`, `ex2_work` kaynakları ve Ö1 klasörleri değiştirilmedi; kampanya çıktıları (`04_Results/ex2/campaign/`, `campaign2/`) yalnız okundu. Kampanya 1 raporları (`ex2_stats.md`, `ex2_analysis.md`) ezilmedi.

**Seri durumu (EX2_SPEC §11.2):** kampanya 2, kampanya 1 sonuçları görüldükten sonra yazılan post-hoc PCR-23/24/25 ile koşuldu (ön kayıt dışı; sonuçtan bağımsız doğruluk/şartname düzeltmeleri). Kampanya 1 "superseded (PCR-23/24/25 öncesi)" etiketiyle ek materyalde kalır; iki seri birlikte raporlanır (§2).

**Rejim kaydı (§11.1 kural 1; bu belgedeki her lif cümlesi için):** bu yük/kısıt rejiminde (Ö2: 2B, yalnız düşey yük, ACI 318-14 gravite + ACI 6.4.2 desenli hareketli yük, kiriş eksenel çekmesi, Bölüm 18 yok, ana kesme formatı Q17), tek lif ürünü (65/60 BG, 4D/5D), fR tedarikçi verisi (ortalama/karakteristik durumu belirsiz, Q18), ÇŞİDB 2026 fiyatları ile; genellenemez. Tasarımlar "ACI 318-14 with fib MC2010 fibre contributions (declared deviation)" ve tanımlı kısıt kümesi sınırıyla uygundur (§12 madde 0).

## 0. Girdiler, araçlar, izlenebilirlik

| Kalem | Yer |
|---|---|
| Kampanya 2 (270 koşu, salt-okunur) | `04_Results/ex2/campaign2/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv`; günlük son satırı "ALL DONE ex2 campaign2" (`campaign2_log.txt`) |
| İstatistik | `tools/ex2_stats.py` → `04_Results/ex2_stats_campaign2.md` (otomatik); koşu satırları `ex2/analysis2/ex2_runs_campaign2.csv` |
| Kampanya 1 ↔ 2 | `tools/ex2_compare_campaigns.py` (yeni) → `ex2/analysis2/campaign1_vs_campaign2.md` |
| Deterministik araçlar (Ex2Runner) | `tools/ex2_analysis2_run.sh` (yeni; `ex2_analysis_run.sh` eşdeğeri) → `ex2/analysis2/*.txt`, komutlar `commands.txt` |
| Ek deterministik araçlar (Runner'da olmayan) | `tools/ex2_analysis2_extra.ps1` (yeni; donmuş DLL'lerin yalnız public API'sini çağırır): iki-koordinat taraması, desenli L ile sehim, tüm fp0 son vektörleri üzerinde başabaş, uzay kardinalitesi; komut `extra_commands.txt` |
| İkili | `ex2_campaign2_bin` scratchpad **kopyası**; `sha256sum -c` 8/8 OK (`analysis2/sha_check.txt`); her çıktının 1. satırı Ex2Model `a61b0c6b…c95d8`, FemRcModel `c1a95951…`, RcCore `14f84b75…`, FemCore `8da9dfd2…` (= §11.2) |
| Çıktı SHA-256 | `ex2/analysis2/SHA256SUMS_analysis2.txt` (61 dosya) |
| Araç SHA-256 | `ex2_stats.py` `89b3019d…3071` (değişiklik: koşu-CSV yolu argümandan, kampanya 2 etiketinde "farklı ham x" sütunu; kampanya 1 çıktısı başlık yolu dışında birebir yeniden üretildi), `ex2_analysis2_run.sh` `a68ab663…8725`, `ex2_analysis2_extra.ps1` `36f6d9e2…a741`, `ex2_compare_campaigns.py` `2b92565c…7cbd` |

Atıf biçimi `dosya:satır`; `analysis2/` = `04_Results/ex2/analysis2/`, `stats2` = `04_Results/ex2_stats_campaign2.md`.

### 0.1 Analiz edilen tasarımlar

| Kod | Kaynak koşu | Maliyet (TL) | Not |
|---|---|---|---|
| **P** | `campaign2/plain/EX2_ATO_plain_g9_fr1.00_fp90_run02.csv` | 236 456,22 | Lifsiz en düşük; ATO 20/30, BTO 15/30 aynı vektör (stats2:88–89) |
| **PY** | `campaign2/plain/EX2_YAKO_plain_g9_fr1.00_fp90_run03.csv` | 236 711,62 | YAKO'nun en düşüğü (8/30; stats2:87); marjinal tablo yöntem başına |
| **F90** | `campaign2/fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` | 236 456,22 | Üç kirişte doz 0 → etkin tasarım P ile aynı (stats2:100–101) |
| **F0** | `campaign2/fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv` | 233 916,61 | ATO 27/30, BTO 9/30 (stats2:112–113); kirişler 250×500 / 250×550 / 250×500, üçü 4D/40 (248 kg; `F0_evalonly_fp90.txt:7` lif 22 320 TL / 90) |
| F0Z | F0 kesitleri, üç doz 0 (`x/F0Z.txt`) | 238 300,73 | `F0Z_evalonly.txt:4` = `F0_zerofibre.txt:8` |

Etkin tasarım: P = kolonlar 250×300 (C_Edge_S1, S2; C_Inter_S3), C_Edge_S3 300×300, C_Inter_S1 300×400, C_Inter_S2 300×300; kirişler 250×550 / 250×600 / 250×600. F0 = aynı kolonlar, yalnız C_Edge_S3 250×300 (stats2:88, 113).

## 1. Kampanya 2 istatistik özeti (ayrıntı `stats2`)

- **Kabul:** 270/270 koşuda FEs = 10 000 ve T = 0 (stats2:11–19). `violation_counts`: g1, g2, g3 (iki satır), g6 her hücrede, g4 259/270 dosyada; **g7 (Q ≤ 1/3) ve g8 (kiriş eğilme D/C ≤ 1) 270 dosyanın hiçbirinin `violation_counts` satırında yok** → arama boyunca hiç ihlal edilmedi (stats2:25–37). g8'in hiç tetiklenmemesi PCR-23(3) düzeltmesinin tasarım yolunda D/C ≤ 1'i kurgu gereği sağladığıyla tutarlı; g8 bu seride yalnız güvenlik denetimi işlevi gördü.
- **Medyan (TL):** lifsiz YAKO 237 494,78 / BTO 236 462,41 / ATO 236 456,22; lifli 90 TL/kg 240 977,61 / 236 468,59 / 236 456,22; lifli 0 TL/kg 235 917,55 / 233 944,06 / 233 916,61 (stats2:44–52).
- **Lif > 0 koşu:** 90 TL/kg'da YAKO 4/30 (40–50 kg, tek grup; dördü de 241 237–243 304 TL, hücre en düşüğünden pahalı), BTO 0/30, ATO 0/30; 0 TL/kg'da 30/30 her yöntemde, 137–248 kg (stats2:47–52, 56–61).
- **Çözüm çeşitliliği (farklı etkin / ham x, /30):** lifsiz YAKO 21/21, BTO 5/5, ATO 2/2; 90 TL/kg 30/30, 6/18, 2/13 (ham x farkı lif tipi geninden, doz 0); 0 TL/kg 30/30, 11/12, 3/3 (stats2:44–52).
- **Yöntem içi testler (MWU + Holm, m = 9; bu seride bağ durumu yok):** lifsiz ↔ 90 TL/kg BTO p_Holm 0,745, ATO 0,745 (medyan farkı +6,19 / 0,00 TL); YAKO +3 482,84 TL (+%1,466; p_Holm 1,9e-10; A12 0,994). Lifsiz ↔ 0 TL/kg: YAKO −%0,664, BTO −%1,065, ATO −%1,074 (p_Holm ≤ 8,9e-8) (stats2:69–77). Lifli uzay lifsiz uzayı kapsadığından bu testler "arama verimliliği" ile "lif etkisi"ni ayıramaz (§11 madde 2); fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırır, yorumlanmaz (dış hakem R9: ek materyal).
- **Çapraz teyit (sıralama değil):** BTO ve ATO'nun en düşük tasarımı üç hücrede de aynı etkin tasarım (lifsiz ve 0 TL/kg'da ham Hamming 0; 90 TL/kg'da Hamming 2, yalnız doz 0'daki lif tipi genleri). YAKO'nun en düşüğü lifsizde +255,40 TL (%0,108; C_Edge_S3 250×300, B_S2 550), 90 TL/kg'da +1 804,72 TL (%0,763), 0 TL/kg'da +95,97 TL (%0,041; C_Inter_S1 350×350, B_S1/B_S2 5D/35) (stats2:87–117). Hücre en düşüğüne %1 içinde: lifsiz YAKO 30/30, 90 TL/kg YAKO 2/30, 0 TL/kg YAKO 23/30; BTO/ATO 30/30 (stats2:93, 105, 117).
- **Kat stabilitesi:** 270 en iyi tasarımın tamamında en az iki kat ötelenmeli (Q > 0,05); en büyük Q 0,0994 (≤ 1/3) (stats2:123–131).

## 2. Kampanya 1 ↔ kampanya 2 (post-hoc etkisi; `analysis2/campaign1_vs_campaign2.md`)

| Hücre | Medyan K1 → K2 (BTO / ATO) | Δ (%) | Lif > 0 koşu K1 → K2 (YAKO/BTO/ATO) | Kaynak |
|---|---|---|---|---|
| Lifsiz | 230 308,35 → 236 462,41 / 236 456,22 | +2,67 | — | `campaign1_vs_campaign2.md:10–11` |
| Lifli 90 TL/kg | 230 308,35 → 236 468,59 / 236 456,22 | +2,67 | 5/0/0 → 4/0/0 | `:13–14`, `:12` |
| Lifli 0 TL/kg | 228 340,77 / 228 294,22 → 233 944,06 / 233 916,61 | +2,45 / +2,46 | 30/30/30 → 30/30/30 | `:16–17` |

- Tüm hücre medyanları +%2,45–2,77 arttı (`:9–17`); bu, desenli yük + kiriş eksenel çekmesi + tutarlı SFRC kapasitesinin maliyet etkisidir (kural değişikliği + arama; test yapılmadı).
- **Lif kararı değişmedi:** 90 TL/kg'da BTO/ATO yine 0/30; 0 TL/kg'da yine 30/30 (`:12–17`).
- Lifsiz → 0 TL/kg medyan farkı büyüdü: ATO −%0,875 → −%1,074, BTO −%0,854 → −%1,065, YAKO −%0,604 → −%0,664 (`:23–25`).
- En düşük tasarımlar: lifsizde C_Edge_S3 250×300 → 300×300 ve C_Inter_S1 350×350 → 300×400; 0 TL/kg'da B_S2 250×500 → 250×550 (`:31–33`).

## 3. Her çözümde stabilite, kesme, D/C (`*_evalonly.txt`)

| | P (= F90) | F0 | F0Z (F0 kesitleri, lifsiz) | Kaynak |
|---|---|---|---|---|
| Toplam (TL) | 236 456,22 | 233 916,61 | 238 300,73 | `:4` |
| Kat Q (S1/S2/S3) | 0,0636 / 0,0817 / 0,0422 | 0,0688 / 0,0900 / **0,0498** | = F0 | `:8` |
| δs (S1/S2/S3) | 1,0679 / 1,0890 / 1,0000 | 1,0738 / 1,0989 / 1,0000 | = F0 | `:8` |
| Ms,max (kN·m; desenli zarf) | 2,29 / 1,03 / 1,92 | 2,49 / 1,29 / 1,79 | = F0 | `:8` |
| En büyük 6.2.6 oranı (sınır 1,4) | 1,3800 (C_Inter_S1 dışı) | 1,3821 (C_Inter_S1 dışı) | 1,3821 | `:22` |
| Kolon P-M D/C en büyük | 0,7886 | 0,8317 (C_Edge_S3) | 0,8317 | `:21–26` |
| Kiriş eğilme D/C (üç grup) | 1,0000 | 1,0000 | 1,0000 | `:27–29` |
| Kiriş Nt (kN, B_S1/B_S2/B_S3); Vc eksenel katsayısı | 11,86 / 10,97 / 0; 0,975 / 0,979 / 1 | 13,16 / 12,95 / 0; 0,970 / 0,973 / 1 | = F0 | `:27–29` |
| Kiriş kesme D/C | 1,0000 / 0,9669 / 0,9604 | 0,7277 / 0,6952 / 0,7301 | 1,0000 / 1,0000 / 1,0000 | `:27–29` |
| Vc,f/Vc,ACI | 1 / 1 / 1 | 2,1568 / 2,0488 / 2,1125 | 1 / 1 / 1 | `:27–29` |
| Etriye belirleyicisi; s (mm) | B_S1 dayanım 234,2; B_S2/B_S3 smax (Av/s dayanımdan) 270 | üçü smax (Av,min'den) 220 / 245 / 220 | üçü dayanım 190,2 / 232,9 / 191,1 | `:27–29` |

Not: F0'ın S3 katı Q = 0,0498 ile ötelenmesiz sınıfta, eşiğe 0,0002 uzak (sınıflama duyarlı; Critic noktası). Belirleyici 6.2.6 oranı düzlem dışı (varsayıma bağlı, R8 açık).

## 4. Komşuluk taramaları ve uzay büyüklüğü

| Tasarım | Tek-koordinat (değerlendirme / daha ucuz uygun) | İki-koordinat (değerlendirme / uygun / daha ucuz uygun / %0,1 içinde ucuz olmayan uygun) | Kaynak |
|---|---|---|---|
| P | 90 / 0 | 3 816 / 2 791 / **0** / 2 | `P_scan.txt:4–5`, `P_scan2.txt:4–5` |
| F90 | 123 / 0 | 5 559 / 4 300 / **0** / 8 | `F90_scan.txt:4–5`, `F90_scan2.txt:4–5` |
| F0 | 123 / 0 | 5 559 / 4 307 / **0** / 28 | `F0_scan.txt:4–5`, `F0_scan2.txt:4–5` |

Ham uzay kardinalitesi: lifsiz 18 değişken 7,88×10¹³; lifli 24 değişken 1,36×10¹⁷ (`cardinality.txt:2–3`). Yazılabilecek ifade (dış hakem §2 önerisiyle uyumlu): "the cheapest feasible design found was reached by two algorithms (BTO 15/30, ATO 20/30 plain; 9/30 and 27/30 at zero fibre price) and no cheaper feasible design exists in its one- and two-coordinate neighbourhoods; this is cross-confirmation, not a proof of global optimality." İki-koordinat taraması iki değişkenin aynı anda değiştiği tüm çiftleri kapsar; üç ve daha fazla değişkenli hamleler taranmadı.

## 5. Desenli yük (ACI 6.4.2) artık tasarım zarfında: zarf sonuçları (`*_patterns.txt`)

Araç, aynı kesitlerde tek desen (tüm açıklıklar yüklü) ile tasarlanmış donatının desenli zarftaki D/C'sini ve desenli zarfla yeniden türetmenin maliyetini verir; kampanya 2 tasarımları zaten desenli zarfla türetilmiştir.

| | P (= F90) | F0 | Kaynak |
|---|---|---|---|
| Kombinasyon | 26 (1,4D; 1,2D+1,6L; 24 desen) | 26 | `:4` |
| Tek desenle tasarım maliyeti → desenli (kampanya 2 kuralı) | 231 511,04 → 236 456,22 (+4 945,18; +%2,14) | 229 046,77 → 233 916,61 (+4 869,84; +%2,13) | `:5`, `:16` |
| Mu artışı (tek → desen), kiriş üst / alt | +%4,6–5,0 / +%14,6–22,1 | +%4,2–4,8 / +%12,5–20,8 | `:6–8` |
| Tek desen donatısının desenli eğilme D/C'si | 1,155 / 1,196 / 1,221 | 1,136 / 1,193 / 1,208 | `:6–8` |
| Kolon D/C tek → desen (en büyük) | 0,758 → 0,789 | 0,760 → 0,832 | `:9–14` |

Kampanya 2'de bu açık kapalıdır (g8 ve desenli zarf; `P_evalonly.txt:18`, D/C 1,0000).

## 6. Kalem bazlı marjinal maliyet, kesit sabit (Q21; `P_marginal_*.txt`, `PY_marginal_main.txt`)

P kesitleri sabit, (tip, doz) ∈ {4D 20/30/40; 5D 25/35} sırayla her kiriş grubuna ve hep birlikte; lif 90 TL/kg. Taban `P_marginal_main.txt:6` (kiriş 148 756,97; toplam 236 456,22; lifli mod doz 0 = lifsiz `:4`). 20/20 durum T = 0.

| Durum | Lif (kg) | Δ toplam (TL / %) | Δ boyuna (TL) | Δ etriye (TL) | Etriye belirleyicisi (sonra) | Kaynak |
|---|---|---|---|---|---|---|
| B_S1 4D/20 | 44 | +3 462,97 / +1,46 | −303,65 | −193,38 | smax (Av/s dayanımdan) | `:7` |
| B_S1 4D/40 | 88 | +7 160,22 / +3,03 | −566,40 | −193,38 | smax (Av,min) | `:9` |
| B_S2 4D/20 | 48 | +4 151,83 / +1,76 | −168,17 | 0 | smax (Av,min) | `:12` |
| B_S3 4D/20 | 48 | +4 143,30 / +1,75 | −176,70 | 0 | smax (Av,min) | `:17` |
| ALL 4D/20 | 140 | +11 758,10 / +4,97 | −648,52 | −193,38 | B_S1 smax (dayanım); B_S2, B_S3 smax (Av,min) | `:22` |
| ALL 4D/40 | 280 | +23 798,92 / +10,06 | −1 207,70 | −193,38 | üçü smax (Av,min) | `:24` |
| ALL 5D/35 | 245 | +20 698,18 / +8,75 | −1 158,44 | −193,38 | üçü smax (Av,min) | `:26` |

- **Etriyeyi belirleyen sütun:** P'de B_S1 etriyesi dayanımdan (s = 234,2 < smax 245); lif onu smax'a taşıyor ve **B_S1'i içeren 10/20 durumda Δ etriye −193,38 TL**; B_S2/B_S3 zaten smax'ta (270) → Δ etriye 0 (kampanya 1'de 20/20 durumda 0 idi, `ex2_analysis.md:88`). Beton ve kalıp kesit sabit olduğundan 0.
- Kesit sabitken başabaş (−Δ boyuna − Δ etriye)/kg: ana format 3,26–11,30 TL/kg (en büyük B_S1 4D/20), hep birlikte 5,00–6,03 TL/kg. Yöntem başına PY (YAKO): 3,45–11,45 TL/kg (Δ etriye −381,96…0). Varyantlar (aynı tanım): MC2010-yerel ana formatla aynı (3,26–11,30); fR × 0,8 2,48–9,76; × 0,6 1,77–8,30; × 1,2 4,11–12,92; Ö1 formatı 2,44–9,99 ve Ö1'de B_S1 4D/20 ve ALL 4D/20 durumlarında etriye **artıyor** (+96,27 / +147,48 TL; `P_marginal_o1.txt:7,22`) (`P_marginal_{mc2010,fr0.8,fr0.6,fr1.2,o1}.txt:7–26`).
- Kanal göstergesi: kesit sabitken lifin kazandırdığı kalem tasarrufu lif bedelinin (90 TL/kg) en çok %12,6'sı kadar (11,30/90).

## 7. Lif grup grup sıfırlama (§11.1 kural 4) ve kalem ayrımı

**F0 (0 TL/kg; `F0_zerofibre.txt:3–8`):** her grupta sıfırlama uygun kalıyor (T = 0) ve maliyeti artırıyor: B_S1 +1 781,21, B_S2 +775,64, B_S3 +1 827,28, hepsi +4 384,12 TL → üç grupta "useful". Aynı kesitte (F0 → F0Z; `F0_evalonly.txt:7`, `F0Z_evalonly.txt:7`): boyuna −2 819,61 TL, etriye −1 564,51 TL (s 190–233 → 220–245 mm, dayanımdan smax'a), beton/kalıp 0. Grup başına aynı-kesit göstergesi 1 781,21/80 = 22,3; 775,64/88 = 8,8; 1 827,28/80 = 22,8 TL/kg (aynı kesitteki lifsiz alternatif en ucuz lifsiz tasarım olmadığından üst gösterge, başabaş değil).
**F90 (`F90_zerofibre.txt:5–7`):** üç grupta doz 0; sıfırlanacak lif yok.

**Arama yanlılığı kontrolü (kural 4) sonucu:** (i) 0 TL/kg'da 90/90 koşu lif seçti ve F0'da her grubun sıfırlanması maliyeti artırıyor; (ii) 90 TL/kg'da BTO/ATO 60/60 koşuda doz 0. İkisi birlikte, 90 TL/kg'da lifin seçilmemesinin arama hatasından değil, lif bedelinin diğer kalem tasarrufunu aşmasından kaynaklandığıyla **tutarlıdır** (bu rejimde; kanıt değil).

**F0 ↔ P kalem ayrımı (farklı kesitler; `P_evalonly.txt:5,7`, `F0_evalonly.txt:5,7`):** toplam −2 539,61 TL (−%1,074). Kirişler −2 906,87 TL = beton −2 919,48 + kalıp −5 587,39 + boyuna +5 258,77 + etriye **+341,24**; kolonlar +367,26 TL (C_Edge_S3 300×300 → 250×300 ve etriye aralığı 300 → 120 mm; `P_evalonly.txt:25`, `F0_evalonly.txt:25`; iç Critic K9 kök nedeni: Vu > 0,5φVc eşiği). Tasarruf **kesit küçültme kanalından** (kiriş h 550/600/600 → 500/550/500: beton + kalıp) geliyor; en düşük lifsiz tasarıma göre boyuna donatı ve etriye artıyor (kampanya 1 ile aynı yön; `ex2_analysis.md:100`).

## 8. Ön kayıtlı üçlü koşul (§11.1 kural 2) — F0'ın deterministik yeniden değerlendirmesi

Referans P = 236 456,22 TL (lifsiz; kesme formatından ve fR'den bağımsız). Lif 0 TL/kg. Yeniden optimizasyon yok.

| Değerlendirme | F0 maliyet (TL), T | F0 − P (TL / %) | Aynı kesitte lif sıfırlama kazancı (TL) | F0 etriye (TL) / belirleyici (B_S1/B_S2/B_S3) | F0Z etriye (TL) | Kaynak |
|---|---|---|---|---|---|---|
| **(a) ana Q17** | 233 916,61, 0 | −2 539,61 / −1,074 | 4 384,12 | 13 513,67 / smax (Av,min) ×3 | 15 078,18 (dayanım) | `F0_evalonly.txt:4,7,27–29`; `F0_zerofibre.txt:8` |
| **(b) MC2010-yerel** | 233 916,61, 0 | −2 539,61 / −1,074 | 4 384,12 | 13 513,67 / smax (dayanım), smax (Av,min), smax (dayanım) | 15 078,18 | `F0_evalonly_mc2010_fr1.0.txt:4,27–29`; `F0_zerofibre_mc2010_fr1.0.txt:8` |
| **(c) fR × 0,8** | 234 585,80, 0 | −1 870,42 / −0,791 | 3 714,92 | 13 513,67 / smax (dayanım), smax (Av,min), smax (dayanım) | 15 078,18 | `F0_evalonly_delta_fr0.8.txt:4,27–29`; `F0_zerofibre_delta_fr0.8.txt:8` |
| bilgi: Ö1 formatı | 233 916,61, 0 | −2 539,61 / −1,074 | 4 384,12 | 13 513,67 / smax (dayanım) ×3; kesme D/C 0,961–0,990 | 15 078,18 | `F0_evalonly_o1_fr1.0.txt` |
| bilgi: fR × 0,6 | 235 199,47, 0 | −1 256,75 / −0,532 | 3 101,26 | 13 513,67 / smax (dayanım) ×3 | 15 078,18 | `F0_evalonly_delta_fr0.6.txt`, `F0_zerofibre_delta_fr0.6.txt` |
| bilgi: fR × 1,2 | 233 189,09, 0 | −3 267,13 / −1,382 | 5 111,63 | 13 513,67 / smax (Av,min) ×3 | 15 078,18 | `F0_evalonly_delta_fr1.2.txt`, `F0_zerofibre_delta_fr1.2.txt` |
| bilgi: MC2010-yerel fR × 0,8 / × 1,2 | 234 585,80 / 233 189,09, 0 | (c) ve fR × 1,2 ile aynı | 3 714,92 / 5 111,63 | 13 513,67 | 15 078,18 | `F0_evalonly_mc2010_fr{0.8,1.2}.txt` |

F0'da etriye üç değerlendirmede de smax'ta olduğundan **kesme formatı F0'ın maliyetini değiştirmiyor**; fark yalnız fR üzerinden eğilme (boyuna donatı) kanalından geliyor.

**Karar (kural 2 birebir):**
- **"0 TL/kg'da en ucuz bulunan lifli tasarım, en ucuz bulunan lifsiz tasarımdan ucuzdur"**: (a), (b), (c) üçünde de korunuyor → yazılabilir; yalnız rejim kaydıyla, yalnız 0 TL/kg duyarlılık senaryosu için ve "deterministik yeniden değerlendirme" olarak (iç Critic yazım önerisi: "In the zero-fibre-price sensitivity scenario, the cheapest fibre design found was ≈ 1.1 % cheaper than the cheapest plain design found, owing to shallower beams; at the ÇŞİDB price no BTO/ATO run selected fibre."). "Fibre reduces cost" bağlamsız yazılamaz. **Şerh:** ön kayıtlı üçlü koşul sehimi içermez; F0 desenli L altında ℓ/480'i aşıyor (§11). ℓ/480 bağlayıcı sayılırsa bu cümle savunulamaz (R4 kararı IA'da).
- **90 TL/kg'da** iddia yazılamaz: lif seçilmedi; F0 tasarımı 90 TL/kg'da 256 236,61 TL, P'den +%8,37 (`F0_evalonly_fp90.txt:4`).
- **"Lif etriyeyi azaltır"**: aynı kesitte (F0 ↔ F0Z) üç değerlendirmede de etriye −1 564,51 TL; P kesitlerinde (marjinal) yalnız dayanım belirleyici B_S1'de −193,38 TL, B_S2/B_S3'te 0, Ö1 formatında iki durumda artış; en ucuz lifsiz tasarıma göre F0'da etriye +341,24 TL. Referans ön kayıtta tanımlı değil; üç referanstan birinde tersine döndüğü için **genel cümle yazılmaz** (iç Critic K2 ile aynı karar). En dar ifade (post-hoc etiketli): "aynı kesitte lif, kiriş etriye aralığını dayanım belirleyiciliğinden smax'a taşıdı".

## 9. Duyarlılık (deterministik; bilgi)

**Lif fiyatı** (F0 tasarımı, kesit ve donatı sabit, 248 kg; P ve F90 lif içermez, 236 456,22 TL sabit):

| Lif fiyatı (TL/kg) | 0 | 45 (×0,5) | 90 (×1) | 135 (×1,5) | 180 (×2) |
|---|---|---|---|---|---|
| F0 maliyet (TL) | 233 916,61 | 245 076,61 | 256 236,61 | 267 396,61 | 278 556,61 |
| F0 − P (%) | −1,07 | +3,65 | +8,37 | +13,09 | +17,80 |
| Kaynak | `F0_evalonly.txt:4` | `F0_evalonly_fp45.txt:4` | `_fp90.txt:4` | `_fp135.txt:4` | `_fp180.txt:4` |

**fR ve kesme formatı:** §8 tablosu (F0) ve §6 varyantları (P, kesit sabit). Tüm varyantlarda F0 uygun (T = 0). Vc,f/Vc,ACI (B_S1/B_S2/B_S3): ana 2,16 / 2,05 / 2,11; MC2010-yerel 2,03 / 1,93 / 1,99; Ö1 1,23 / 1,15 / 1,26; fR × 0,8 2,02 / 1,92 / 1,98; × 0,6 1,86 / 1,78 / 1,83; × 1,2 2,28 / 2,16 / 2,23 (`F0_evalonly*.txt:27–29`).

**Ö1 formatı:** F0 maliyeti değişmiyor (233 916,61), kesme D/C 0,99'a çıkıyor (`F0_evalonly_o1_fr1.0.txt:27`).

## 10. Başabaş lif fiyatı (§11.1 kural 3; tek tasarım alt sınırı)

Tanım: b = (P − tasarımın lif dışı maliyeti) / lif kg; P = bulunan en ucuz lifsiz (kanıtlanmış optimum değil). Herhangi bir uygun lifli tasarım için b, gerçek (sistem düzeyi) başabaşın **alt sınırıdır**: lif fiyatı b'nin altındayken o lifli tasarım P'den ucuzdur; üstünde başka bir tasarımın kazanıp kazanmadığı yeniden optimizasyon gerektirir (yapılmadı; R6 fiyat taraması açık).

| fR | F0 tasarımı (248 kg) | Bulunan tasarımlar arasında en büyük alt sınır (44 farklı fp0 son vektörü × lif grubu alt kümeleri, 304 değerlendirme/satır, tümü T = 0) | Kaynak |
|---|---|---|---|
| × 0,6 (bilgi) | 1 256,75/248 = 5,07 | **5,40** (BTO run02, üç grup 5D/35, 217 kg) | `breakeven_summary.txt:8` |
| **× 0,8** | 1 870,42/248 = 7,54 | **8,11** (aynı tasarım) | `:9` |
| **× 1,0 (ana; MC2010-yerel aynı)** | 2 539,61/248 = 10,24 | **11,73** (BTO run02, yalnız B_S1 + B_S3 lifli, 140 kg); tam vektörlerde 11,04 | `:10`, `:14` |
| **× 1,2** | 3 267,13/248 = 13,17 | **15,66** (aynı alt küme); tam vektörlerde 14,21 | `:11` |

Raporlanabilir biçim (tek sayı değil, §12 madde 0): **"bu rejimde başabaş lif fiyatının alt sınırı fR × 0,8–1,2 için ≈ 8,1–15,7 TL/kg (fR × 1,0'da ≈ 11,7 TL/kg); ÇŞİDB 90 TL/kg bunun ≈ 5,7–11 katı"**. Kampanya 1'deki 8,39 TL/kg (F0 tek tasarım, A1 nedeniyle geçersiz; `ex2_analysis.md:136`) yerine geçer. Dış hakemin öngördüğü gibi en büyük alt sınır en çok lif içeren F0'dan değil daha az lifli bir tasarımdan geliyor (11,73 > 10,24). Kesit sabit kanal (§6) 3,26–11,30 TL/kg; fiyat = 0 sağlamasındaki kaba gösterge (≈ 8,4) kullanılmadı (kural 3). Tüm alt kümeler `breakeven_designs.csv`.

## 11. Sehim (ACI 24.2; rapor, kısıt değil)

Tüm açıklıklar yüklü L (`*_deflection.txt:14`) ve ACI 6.4.2'nin 24 L deseni (`*_deflection_patterned.txt:504–505`; aynı formüller, Ie her yük durumunun grup momentlerinden, "env" varyantı = grubun tüm durumlardaki en küçük Ie'si). Ek aracın "full" satırları `Deflection2D.Report` çıktısını birebir üretiyor (ör. F0 E20 0,8808: `F0_deflection_patterned.txt:11` = `F0_deflection.txt:12`).

| | ℓ/480 tüm yüklü | ℓ/480 desenli (en kötü; yer) | ℓ/240 desenli | ℓ/360 (L) desenli | Kaynak |
|---|---|---|---|---|---|
| P (= F90) | 0,6228 | 0,6966 (S1 orta açıklık yüklü, E6) | 0,3483 | 0,2655 | `P_deflection.txt:14`; `P_deflection_patterned.txt:504` |
| **F0** | 0,8808 | **1,0379 — AŞILDI** (yalnız çatı orta açıklığı yüklü, E20: dAfter 12,97 > 12,50 mm) | 0,5190 | 0,3276 | `F0_deflection.txt:14`; `F0_deflection_patterned.txt:371, 504–505` |
| F0Z (F0 kesitleri, lifsiz) | 0,8505 | 1,0035 — AŞILDI (aynı yer) | 0,5018 | 0,3147 | `F0Z_deflection_patterned.txt:371, 504` |

F0'da "tüm katlar orta açıklık" deseni de 1,0220 (`F0_deflection_patterned.txt:458`). Varsayımlar: λ = 2 (ρ′ = 0), sürekli yük D, lifin rijitlik katkısı yok (lif aleyhine muhafazakâr), F0'da lif As'yi azalttığı için Icr ve Ie biraz düşük (F0 > F0Z). **Sonuç:** ℓ/240 ("hasar görebilecek bölme taşımaz"; g5 ve Tablo 9.3.1.1 varsayımıyla tutarlı) kullanılırsa üç tasarım da uygun; ℓ/480 kullanılırsa F0 (ve aynı kesitte lifsiz F0Z) desenli L altında %0,4–3,8 aşıyor. Dış hakem R4 tutarsızlığı bu sayıyla somutlaştı; karar IA'da (ℓ/240 rapor mu, ℓ/480 kısıt mı). ℓ/480 kısıt olursa 0 TL/kg tasarrufu (§8) yeniden optimizasyonsuz yazılamaz.

## 12. Karma güvenlik formatı beyanı (dış hakem R5; kodun gerçek davranışı)

| Bileşen | Kodun yaptığı (dosya:satır) | Etkin güvenlik | Şartname durumu |
|---|---|---|---|
| Lifli kiriş eğilmesi | `program_work/RcCore/SfrcBeam.vb:58–85`: basınç ACI Whitney bloğu (β1, fc′ azaltmasız), donatı fy, lif çekme bloğu σ(w)/γF (`:36–42`, `:142–158`), γF = 1,5 (`:8`; MC2010 Tablo 5.6-1, kanıt B10); Mn'nin tamamı ACI φ(εt) ile çarpılır (`:81–82`; `Aci.vb:17–23`: çekme kontrollü 0,90, geçişte 0,65–0,90) | Lif terimi γF ve φ birlikte (çekme kontrollüde 1,5/0,90 ≈ 1,67); beton ve donatı yalnız φ | Lifin eğilme katkısı ACI 318-14'te tanınmıyor (dış hakem R26.4.1.5 atfı; **kanıt dosyasında A-satırı yok → bu pakette DOĞRULANAMADI**); MC2010 σ–w lineer modeli (fFts = 0,45 fR1 `:22–25`, fFtu 5.6-6 `:27–32`), wu = min(lcs·0,02; 2,5 mm) (`:15–20`) |
| lcs | `ex2_work/Ex2Model/GravityDesign.vb:48–60`: lcs = max(h − c, 0,2h) sabit noktası (PCR-23, \|Δlcs\| ≤ 10⁻⁶ mm, 60 adım; yakınsamazsa son iki iterenin küçük φMn'lisi) | — | MC2010 lcs = min(srm, y): srm ihmal (beyan); **0,2h alt sınırının kaynağı bu pakette bulunamadı** (kodda yorum yok) |
| Kiriş eksenel çekmesi (PCR-24) | `GravityDesign.vb:102–114`: As = As(Mu − Nt(d − h/2)) + Nt/(φ fy), φ eğilme parçasından | φ | ACI 22.2 dengesi; basınç ihmal |
| Kesme, ana (Q17) | `GravityDesign.vb:262–279`: Vc,f = Vc,ACI + max(0; VRd,F(fFtuk; γc = 1,0) − VRd,F(0; γc = 1,0)), ρl = AsTop, σcp = 0; kontrol Vu ≤ φ(Vc,f + Vs), φ = 0,75 (`:353`, `:427`); fFtuk kesme için wu = 1,5 mm (`SfrcBeam.vb:94–101`; B18) | Lif artımı γc = 1,0 + φ = 0,75 | ACI dışı (26.4.2.2(d) doz 60 kg/m³ karşılanmıyor); MC2010 Eq. 7.7-4 (B16) artım olarak, γc = 1,0 |
| Kesme, MC2010-yerel (varyant) | `GravityDesign.vb:282–294`: Vu ≤ φ(Vc,ACI + Vs) + ΔVF(γc = 1,5) | Lif artımı yalnız γc = 1,5 (φ yok) | Varyant |
| Kesme, Ö1 formatı (varyant) | `GravityDesign.vb:297–312`: VRd,F(γc = 1,5; ρl = AsBot) Vc yerine, sonra φ = 0,75 | γc = 1,5 + φ | Ö1 ile aynı formül; Ö1'in rejim kuralları taşınmadı |
| Av,min | `GravityDesign.vb:408`: eşik düz Vc,ACI ile (Vu > 0,5φVc) | — | Lif Av,min'i kaldırmıyor (A17); F0 B_S1'de Vu,d = 188,95 kN > 0,17φ√fc′ bw d ≈ 77 kN → 60 kg/m³ ile de Tablo 9.6.3.1 muafiyeti uygulanamazdı (R10) |
| fR | `program_work/FemRcModel/FibreTable.vb:55–62`: fRScale fR1 ve fR3'ü eğilme ve kesmede birlikte ölçekler; tablo değerleri "test ortalaması, karakteristik değil" (kod yorumu) | — | Q18 belirsizliği; ×0,8 zorunlu duyarlılık |

**Beyan cümlesi (öneri):** "Fibre contributions are taken from fib MC2010 and are outside ACI 318-14 (which credits steel fibres only in shear, and only above 60 kg/m³). In flexure the fibre tensile block is divided by γF = 1.5 and the whole nominal moment is multiplied by the ACI φ (fibre term factored twice); in shear the MC2010 fibre increment is computed with γc = 1.0 and multiplied by the ACI φ = 0.75. The format is therefore conservative for fibres in flexure and less conservative than the MC2010-local variant (γc = 1.5) in shear; for the cheapest fibre design found the shear format does not change cost (stirrups at smax in all formats), whereas fR changes it through flexure." Son yarım cümle F0 için §8'de sayıyla gösterildi; genel değildir.

**Hakemin formülasyonu doğru mu:** "Eğilmede γF = 1,5 + φ; kesmede γc = 1 + φ" kodla uyumlu (ana seri). Ek ayrıntılar: (i) kesmede γc = 1,0 yalnız lif artımı ΔVF için; taban Vc ACI 22.5.5.1 ve PCR-24'te 22.5.7.1 eksenel katsayısıyla; (ii) eğilmede φ çekme kontrollü bölgede 0,90, geçişte daha küçük; (iii) eğilmede γF için duyarlılık (ör. B10'daki γF ≥ 1,3) koşulmadı.

## 13. Dürüst sonuç özeti (yalnız ön kayıtlı / post-hoc etiketli iddialar)

1. Kampanya 2 (post-hoc §11.2) 270/270 koşu tanımlı kısıt kümesine göre uygun (T = 0), 10 000 FE; g7 ve g8 arama boyunca hiç ihlal edilmedi (stats2:11–37).
2. **90 TL/kg:** BTO ve ATO hiçbir koşuda lif seçmedi (0/60); lifli senaryonun en düşük tasarımı lifsizle aynı (236 456,22 TL). YAKO 4/30 koşuda tek grupta lif seçti ve bu koşular daha pahalı kaldı (arama verimliliği; lif etkisi olarak okunmaz). Kampanya 1'den değişmedi (§2).
3. **0 TL/kg (duyarlılık senaryosu):** 90/90 koşu lif seçti; en düşük maliyet 233 916,61 TL, en ucuz lifsizden 2 539,61 TL (%1,07) az. Tasarruf kiriş kesitinin küçülmesinden (beton + kalıp) geliyor; en ucuz lifsiz tasarıma göre boyuna donatı ve etriye artıyor. Fark (a) MC2010-yerel formatta ve (c) fR × 0,8'de de korunuyor (−%0,79'a iniyor). **Şerh:** F0 desenli L altında ℓ/480'i %3,8 aşıyor (ℓ/240 uygun) (§11).
4. **Başabaş:** alt sınır fR × 0,8–1,2 için ≈ 8,1–15,7 TL/kg (fR × 1,0: ≈ 11,7); ÇŞİDB 90 TL/kg bunun 5,7–11 katı. Sistem düzeyi başabaş değil, yeniden optimizasyonla fiyat taraması yapılmadı.
5. Kesit sabitken lif, P'de yalnız dayanımın belirlediği B_S1 etriyesini (−193,38 TL) ve boyuna donatıyı azaltıyor; kalem tasarrufu lif bedelinin en çok %12,6'sı.
6. BTO ve ATO üç senaryoda aynı en düşük tasarıma ulaştı; YAKO'nunki %0,04–0,76 içinde ve farklı. Bu en düşük tasarımlar tek- ve iki-koordinat komşuluğunda daha ucuz uygun komşusu olmayan tasarımlardır; "optimum" denmez. Bu yalnız çapraz teyittir; yöntem sıralaması yapılmaz.
7. Post-hoc kurallar maliyeti tüm hücrelerde %2,5–2,8 artırdı; lif kararını değiştirmedi.

**Yazılmayacaklar (§12, §11.1):** "Tasarımlar ACI 318-14'e uygundur" (yalnız "ACI 318-14 with fib MC2010 fibre contributions (declared deviation)"); lif "ekonomiktir / değildir" genel cümlesi; "lifli medyan yüksek, lif pahalıdır"; "lif etriyenin yerini alır"; "lif etriyeyi azaltır" genel cümlesi; "fibre reduces cost" bağlamsız; başabaşı tek sayı olarak; lif servis performansını iyileştirir; algoritma üstünlüğü; YAKO'nun 90 TL/kg medyanının lif etkisi olarak okunması; "optimum"; "ETABS'te doğrulandı"; Ö1 ile nedensel karşılaştırma; 1 000 FE ara değeri; ABC/ön tasarım.

## 14. Açık noktalar (Critic ve IA için)

| # | Konu | Durum |
|---|---|---|
| O1 | **V3** ETABS yeniden analizi (P ve F0; 26 kombinasyon desenli zarf, kat Q ↔ çerçeve tipi, lu oranı, kiriş Nt) | AÇIK; bulgular kapısı kapalı |
| O2 | **S1** lifli Mn bağımsız Excel kontrolü (F0 kirişleri, PCR-23 sabit nokta lcs) ve **S2** (SFRC deney) | AÇIK |
| O3 | **R4 sehim:** F0 ℓ/480 desenli 1,0379 (AŞILDI), ℓ/240 0,519; ℓ/240 rapor mu, ℓ/480 kısıt mı | IA kararı; ℓ/480 seçilirse kısıt + yeniden koşu |
| O4 | **R5 format:** §12 tablosu hazır; ACI R26.4.1.5 ve lcs alt sınırı 0,2h için kanıt satırı yok (Literature Analyst); γF duyarlılığı koşulmadı | KISMEN |
| O5 | **R6 fiyat taraması** (yeniden optimizasyonla lifin seçilmeyi bıraktığı fiyat aralığı) | AÇIK; bu pakette yalnız alt sınır |
| O6 | **R7** iki-koordinat taraması ve kardinalite | YAPILDI (§4) |
| O7 | **R8** düzlem dışı kolon varsayımı duyarlılığı (k, lu); belirleyici 6.2.6 oranı 1,380–1,382 düzlem dışı; gravitede Q hesabının yöntem tanımı | AÇIK (donmuş ikilide parametre yok; ayrı araç veya PCR) |
| O8 | F0 S3 katı Q = 0,0498 (eşik 0,05) — sınıflama sınırda; V3'te ETABS çerçeve tipi ile birlikte bakılmalı | Critic |
| O9 | Ö1 aynı bayat-lcs kalıbı (`04_Results/ex2/o1_required_audit.md`) | Ayrı PCR + IA |
| O10 | İstatistik raporlama: dış hakem R9 (testler ek materyale; fp0 ↔ fp90 raporlanmasın) ile ön kayıt kural 8 çatışması | IA/Critic kararı |
