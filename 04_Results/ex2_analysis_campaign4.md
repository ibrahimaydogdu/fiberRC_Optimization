# Örnek 2 (Ö2) kampanya 4 deterministik analiz ve sonuç özeti (PCR-26b: desenli ℓ/480, Ie zarfı)

Durum: **TASLAK (v2, 2026-11-19 hakem turu k4 düzeltmeleri uygulandı: iç Critic KOŞULLU ONAY, dış hakem KÜÇÜK REVİZYON; `06_Review_Audit/REVIEW_LOG/Review_Round_Ex2_k4_ic.md`, `_dis.md`), Critic + IA onayı bekler.** Bulgular bölümü kapısı kapalı: V3 (en iyi çözümlerin ETABS yeniden analizi) ve S2 yapılmadı; bu belgedeki hiçbir sayı Critic + IA onayı olmadan makaleye girmez. Ek materyale girmesi de hakem/IA onayı ister.
Hazırlayan: Claude Code (Results Analyst), 2026-11-19. Git kullanılmadı. `ex2_campaign4_bin` ve diğer dondurulmuş ikililer, `ex2_work` kaynakları, kampanya 1–3 klasörleri ve kampanya 2 analizi (`analysis2/`) değiştirilmedi; kampanya 4 çıktıları (`04_Results/ex2/campaign4/`) yalnız okundu.

**Seri durumu (EX2_SPEC §11.4):** kampanya 4, kampanya 3 sonucu görüldükten sonra IA kararıyla (2026-11-19, "Ie env") eklenen PCR-26b ile koşuldu: kampanya 2 kuralları (PCR-23/24/25) + g5 = ACI 24.2 Tablo 24.2.2 ℓ/480 sehim kısıtı, ACI 6.4.2 yük desenleri altında, her kiriş grubu için tüm durumlardaki en küçük Ie/Ig (zarf). Bu kural **post-hoc**tur (sonuç bilgisi sonrası seçilmiş: K1–K3 ve F0'ın desenli ℓ/480 aşımı görüldükten sonra; yön lif aleyhine; ön kayıt dışı); zaman çizelgesi ve "outcome-informed" beyan cümlesi EX2_SPEC §11.4'tedir. Kampanya 1–4 birlikte raporlanır (K2 "superseded" değil, ℓ/240-türü öncül duyarlılığı), ana seri kampanya 4'tür; "sonuç görülmeden kayıt" yalnız "kampanya 4 sonuçları görülmeden" sınırıyla kullanılır.

**Rejim kaydı (her lif cümlesi için):** bu yük/kısıt rejiminde (Ö2: 2B, yalnız düşey yük, ACI 318-14 gravite + ACI 6.4.2 desenli hareketli yük + kiriş eksenel çekmesi + ℓ/480 sehim, Bölüm 18 yok, ana kesme formatı Q17), tek lif ürünü (65/60 BG, 4D/5D), fR tedarikçi verisi, ÇŞİDB 2026 fiyatları, lif sertliği sehim modelinde yok sayılır (lif aleyhine); genellenemez. Yasak iddialar bu belgede de geçerlidir: "lif pahalı/ekonomik değil", "optimum", "MaxFE yeterli", algoritma üstünlüğü, "benchmark", "Duysak ile doğrulandı". YAKO/BTO/ATO çapraz doğrulama aracıdır.

## 0. Girdiler, araçlar, izlenebilirlik

| Kalem | Yer |
|---|---|
| Kampanya 4 koşuları (270) | `04_Results/ex2/campaign4/{plain,fibre_fp90,fibre_fp0}/`, günlük `campaign4_log.txt` (bitiş 2026-11-19 16:08:46) |
| İstatistik | `04_Results/ex2_stats_campaign4.md` (üretici `tools/ex2_stats.py`) |
| İkili | `03_Code_Execution/ex2_campaign4_bin/` (Ex2Model `f7207d66…`; `sha_check.txt`: 0 uyumsuzluk); araçlar KOPYADAN çalıştırıldı |
| Deterministik araçlar | `tools/ex2_analysis4_run.sh` (evalonly, scan, patterns, deflection, marginal, zerofibre; bayraklar `--deflcheck 1 --deflpattern 1`), `tools/ex2_analysis4_extra.ps1` (scan2, desenli sehim, başabaş), `tools/ex2_compare_campaigns_c4.py` (kampanya karşılaştırması) |
| Çıktılar | `04_Results/ex2/analysis4/` (61 girdi; `x/` dahil 65 dosya SHA-256 listesinde, `SHA256SUMS_analysis4.txt`); komutlar `commands.txt`, `extra_console.txt` |
| Çıktıda **yok** | `fibre_breakeven.ps1` (Ö1 aracıdır, `phase3_bin` kullanır; Ö2 başabaşı `ex2_analysis4_extra.ps1` içindeki `BreakEven`), ETABS (V3 yapılmadı) |

### 0.1 Analiz edilen tasarımlar (kampanya 4'ün en düşük maliyetli koşuları; `ex2_stats_campaign4.md` §4)

| Kod | Dosya | Maliyet (TL) | Not |
|---|---|---|---|
| P | `campaign4/plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv` | 236 456,22 | lifsiz en düşük; aynı vektör YAKO 1, BTO 19, ATO 23 / 30 koşuda |
| PY | `plain/EX2_YAKO_plain_g9_fr1.00_fp90_run07.csv` | 236 456,22 | YAKO'nun en düşüğü P ile aynı vektör (`PY_evalonly.txt:4`) |
| F90 | `fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` | 236 456,22 | lifli 90 TL/kg en düşük; tüm gruplarda doz 0 (P ile aynı kesit) |
| F0 | `fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run03.csv` | 234 190,76 | lifli 0 TL/kg en düşük; B_S1 250×550, B_S2 250×500, B_S3 250×550, hepsi 4D/40 |
| F0Z | F0 kesitleri, tüm kiriş dozları 0 | 237 504,92 | `F0Z_evalonly.txt:4` |
| F0_c2 | kampanya 2 F0 vektörü (`campaign2/fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv`), kampanya 4 kurallarıyla | 233 916,61 | `F0c2_evalonly_c4rules.txt` (§3) |

## 1. Kampanya 4 istatistik özeti (ayrıntı `ex2_stats_campaign4.md`)

- Kabul: 270/270 koşu 10 000 FE; tümü T = 0 (`ex2_stats_campaign4.md:13–19`, tablo 1; lifli 90 TL/kg YAKO artık 30/30 uygun, kampanya 3'te 29/30).
- En iyi/medyan (TL; `ex2_stats_campaign4.md:44–52`): lifsiz ATO 236 456,22 / 236 456,22; lifli 90 TL/kg ATO 236 456,22 / 236 456,22; lifli 0 TL/kg ATO 234 190,76 / 234 190,76 (BTO 234 190,76 / 234 225,78; YAKO 234 655,82 / 237 201,74). Lif > 0 koşu: 90 TL/kg'da BTO 0/30, ATO 0/30, YAKO 4/30; 0 TL/kg'da 30/30.
- Yöntem içi testler (Holm, m = 9; `ex2_stats_campaign4.md:68–76`): lifsiz ↔ lifli 0 TL/kg medyan farkı BTO −2 230,44 TL (−%0,943, p_Holm 7,04e-11), ATO −2 265,46 TL (−%0,958, 1,31e-11), YAKO +447,20 TL (+%0,189, p_Holm 1). Lifsiz ↔ lifli 90 TL/kg: BTO +12,37 TL (p_Holm 0,053), ATO 0,00 (bağ; test uygulanamaz), YAKO +4 002,97 TL (+%1,691, 3,72e-10). Yöntemler arası test yapılmadı.
- Arama sırasında g5 ihlal eden değerlendirme sayıları (hücre × yöntem, 30 koşu toplamı; `ex2_stats_campaign4.md` §1.1, g5 sütunu): lifsiz YAKO 55 941, BTO 20 972, ATO 17 156; lifli 90: 73 806 / 23 402 / 23 639; lifli 0: 70 782 / 29 241 / 44 453. Bu sayılar aynı tasarımın tekrar değerlendirilmesini de sayar (tekrar sayımı); kısıt etkinliğinin ölçüsü değil, kaba göstergedir (kampanya 3'te lifli 0 TL/kg ATO 37 841, lifsiz ATO 13 515).

## 2. Kampanya 2 ↔ 4 ve 3 ↔ 4 (betimsel; `analysis4/campaign2_vs_campaign4.md`, `campaign3_vs_campaign4.md`)

Kural kümeleri farklıdır (§başlık); fark "kural değişikliği + arama" etkisidir, test yapılmadı.

| Hücre (en düşük, TL) | K2 | K3 | K4 | Kaynak |
|---|---|---|---|---|
| lifsiz (üç yöntem birleşik) | 236 456,22 | 236 456,22 | 236 456,22 | `campaign2_vs_campaign4.md:9–11`, `campaign3_vs_campaign4.md:9–11` |
| lifli 90 TL/kg | 236 456,22 | 236 456,22 | 236 456,22 | aynı dosyalar `:12–14` |
| lifli 0 TL/kg | 233 916,61 | 233 916,61 | **234 190,76** | `campaign2_vs_campaign4.md:17` (ATO satırı), `campaign3_vs_campaign4.md:15–17` |

- Lifli 0 TL/kg en düşüğü K2/K3 → K4: **+274,15 TL (+%0,117)** (BTO ve ATO; `campaign2_vs_campaign4.md:16–17`). YAKO en düşüğü +%0,275 (K2 234 012,58 → K4 234 655,82; `:15`).
- Lifsiz → lifli 0 TL/kg medyan farkı: BTO −%1,065 (K2) → −%0,943 (K4), ATO −%1,074 → −%0,958 (`campaign2_vs_campaign4.md:25`, ATO satırı); YAKO medyan farkı K2'de −%0,664, K4'te **+%0,189** (`campaign2_vs_campaign4.md:23`): YAKO için lifli 0 TL/kg medyanı lifsizin üstüne çıktı (p_Holm 1, anlamlı değil).
- Lifli 0 TL/kg en sık kiriş deseni: K2 B_S1 250×500, B_S2 250×550, B_S3 250×500 (4D/40; BTO 18/30, ATO 27/30) → K4 B_S1 250×550, B_S2 250×500, B_S3 250×550 (BTO 21/30, ATO 23/30) (`campaign2_vs_campaign4.md:24–25`).

## 3. K3 ve K4'ün en iyileri neden değişti / değişmedi

**Kampanya 3 (g5 tüm-yüklü Report): en iyiler K2 ile aynı.** K2'nin F0 vektörü tüm açıklıklar yüklüyken ℓ/480 oranı 0,8808 (`analysis2/F0_deflection.txt:14`), yani g5 bu vektörde pasif; hakem bulgusu (oran 1,0379) yalnız çatı orta açıklığı yüklü desenle çıkıyordu (`analysis2/F0_deflection_patterned.txt:371`). Kısıt hatalı kapsamdaydı → en iyiler değişmedi (EX2_SPEC §11.3 sonuç notu).

**Kampanya 4 (g5 desenli, Ie zarfı): lifsiz ve 90 TL/kg en iyileri değişmedi; 0 TL/kg en iyisi değişti.**
- K2'nin F0 vektörü K4 kurallarıyla yeniden değerlendirildi: maliyet 233 916,61 TL, **g5 = 0,0379 > 0 → T = 0,0379, Fpen 252 002,24** (`analysis4/F0c2_evalonly_c4rules.txt:4,21`), yani 1,0379 aşımı kodun içinde de yakalanıyor (bağımsız PowerShell aracı 1,0379 ile tutarlı; birim testi T27). Bu vektör K4'te uygun değil.
- K4'ün F0'ı (B_S1 250×550, B_S2 250×500, B_S3 250×550; 4D/40), K2 vektörüne (B_S1 250×500, B_S2 250×550, B_S3 250×500) göre iki kirişi (B_S1, B_S3) 500 → 550 mm'ye çıkarıp B_S2'yi 550 → 500 mm'ye indirir. Kiriş kalemleri (TL; K2 vektörü K4 kurallarıyla `F0c2_evalonly_c4rules.txt:7` → K4 F0 `F0_evalonly.txt:7`): beton 22 625,97 → 23 355,84; kalıp 53 778,65 → 55 175,50; boyuna donatı 55 931,82 → 54 227,88; etriye 13 513,67 → 13 365,04; kirişler 145 850,10 → 146 124,26. Net maliyet bedeli **+274,15 TL** (toplam 233 916,61 → 234 190,76). Desenli ℓ/480 zarf oranı F0 için 0,9847 (`F0_deflection_patterned.txt:505`; en kötü durum "P S2 bays 2 E13": 2. kat, B_S2 grubu, 6 m açıklık); tüm açıklıklar yüklü Report oranı 0,8834 (`F0_deflection.txt:14`).
- Lifsiz P: kısıt bağlayıcı değil (desenli zarf oranı 0,6962, `P_deflection_patterned.txt:505`), en iyi vektör K2/K3/K4'te aynı; sehim 500–600 mm lifsiz kirişlerde (B_S1 550, B_S2/S3 600) zaten sağlanıyor.
- Lif, sehim kısıtını bağlayıcı kılan sığ kiriş kanalından tasarruf ediyordu; kısıt tasarrufu %1,074 → %0,958'e düşürdü (ATO medyan farkı; `campaign2_vs_campaign4.md:25`), ortadan kaldırmadı. Bu sayı lif sertliği ihmal edilerek (lif aleyhine) hesaplanır.

## 4. Her çözümde stabilite, kesme, D/C (`*_evalonly.txt`)

| | P (= PY = F90) | F0 | F0Z |
|---|---|---|---|
| Maliyet / T | 236 456,22 / 0 (`P_evalonly.txt:4`) | 234 190,76 / 0 (`F0_evalonly.txt:4`) | 237 504,92 / 0 (`F0Z_evalonly.txt:4`) |
| Kirişler toplam (TL) | 148 756,97 (`P_evalonly.txt:7`) | 146 124,26 (`F0_evalonly.txt:7`) | 149 438,41 (`F0Z_evalonly.txt:7`) |
| g5 (desenli ℓ/480, Ie zarfı) | 0 (`P_evalonly.txt:21`) | 0 (`F0_evalonly.txt:21`) | 0 (`F0Z_evalonly.txt:21`) |
| Kiriş eğilme D/C maks. | 1,0000 | 1,0000 | — |
| Kiriş kesme D/C maks. | 1,0000 (etriye dayanımla) | 0,7300 (`F0_evalonly.txt:31`) | — |
| Kolon P-M D/C maks. | 0,7886 | 0,7818 | — |
| ACI 6.2.6 oranı maks. (sınır 1,4) | 1,3800 (`P_evalonly.txt:31`) | 1,3755 (`F0_evalonly.txt:31`) | — |
| Kat Q (S1/S2/S3) | bkz. `P_evalonly.txt:8` | 0,0636 / 0,0883 / 0,0522 (üçü sallanan, Q > 0,05; `F0_evalonly.txt:8`) | — |

ACI 6.2.6 oranı 1,3755–1,3800 sınır 1,4'e %1,4–1,8 yakın; düzlem dışı duyarlılık hakem R8 maddesi (açık, §11).

## 5. Sehim (ACI 24.2; artık kısıt) — tüm-yüklü ve desenli

| Tasarım | Tüm-yüklü Report oranı (ℓ/480) | Desenli oran, Ie her durum için | Desenli oran, Ie zarfı | Kaynak |
|---|---|---|---|---|
| P | 0,6228 | 0,6966 | 0,6962 | `P_deflection.txt:14`; `P_deflection_patterned.txt:504–505` |
| F0 | 0,8834 | 0,9843 | **0,9847** | `F0_deflection.txt:14`; `F0_deflection_patterned.txt:504–505` |
| F0Z | 0,8556 | 0,9545 | 0,9548 | `F0Z_deflection.txt:14`; `F0Z_deflection_patterned.txt:504–505` |

Doğrulama: ℓ/480 oranı **hepsi ≤ 1** (F0 en kötü 0,9847, S2 E13); zarf ≥ durum-başına Ie değeri (0,9847 ≥ 0,9843), beklenen sıralama. ℓ/240 oranı F0 için tüm-yüklü 0,4417 (`F0_deflection.txt:14`), desenli 0,4922 (`F0_deflection_patterned.txt:504`). Kampanya 2'de F0 oranı 1,0379 idi; bu K2 vektörüdür (§3). F90 desenli sehim araç tarafından ayrıca üretilmedi (F90 = P ile aynı vektör).

**Formül notu (iç Critic B1/L1):** dSonra = ξ·dD + dL (ξ = 2): D altındaki **ani** sehim sayılmaz (ACI standart okuması: bölme/dolgu D'den sonra eklenir; G = 30 kN/m duvarı içerdiğinden bu varsayım önemlidir). `Serviceability2D.vb:168` yorumu "tüm D'nin bağlantıdan önce uygulandığı varsayılmaz (tüm uzun süreli sehim sayılır, muhafazakâr)" dese de formül ani D'yi saymaz; dondurulmuş kaynak değiştirilmediği için yorum düzeltilmedi, bu not ve EX2_SPEC §11.4 beyandır. Ani D sayılırsa F0 oranı 1,2871 (§12.2 V4). Ayrıca dL = dDL(zarf Ie) − dD(Ie_D) farkında D-durumu Ie'sinin küçük bir kısmı dL'ye taşınır (muhafazakâr yön).

## 6. Komşuluk taramaları (yalnız çapraz teyit; "optimum" değil)

| Tasarım | Tek koordinat (`*_scan.txt:4–5`) | İki koordinat (`*_scan2.txt:4–5`) |
|---|---|---|
| P | 90 değerlendirme, daha ucuz uygun 0; yerel en iyi EVET | 3 816 değerlendirme (2 516 uygun), daha ucuz 0; %0,1 içinde 2 |
| F90 | 123, daha ucuz 0; EVET | 5 559 (3 953 uygun), daha ucuz 0; %0,1 içinde 8 |
| F0 | 123, daha ucuz 0; EVET | 5 559 (3 581 uygun), daha ucuz 0; %0,1 içinde 35 |

Sonuç: üç tasarım da tek ve iki koordinatlı hamle komşuluğunda yerel en düşüktür; bu küresel en düşük olduğunu göstermez (uzay boyutu `cardinality.txt`).

## 7. Kalem bazlı marjinal maliyet, kesit sabit (`P_marginal_*.txt`, `PY_marginal_main.txt`)

P kesitlerinde (g5 sağlanır) lif zorlamalı eklenirse (kesit sabit, lif fiyatı 90 TL/kg; kesit küçültme kanalı bu tabloda görünmez): tek grup 4D/20 için +3 462,97 (B_S1) ile +4 151,83 TL (B_S2); tüm gruplar 4D/40 için +23 798,92 TL, 5D/35 için +20 698,18 TL (`P_marginal_main.txt:24,26`); boyuna donatıdaki azalma en çok −1 207,70 TL, etriye −193,38 TL; lif ücreti (+25 200 TL; 4D/40, tüm gruplar) bunun çok üstündedir. fR ×0,6/0,8/1,2 ve MC2010-yerel/Ö1 formatları için tablolar `P_marginal_fr*.txt`, `P_marginal_mc2010.txt`, `P_marginal_o1.txt` içindedir; tümünde T = 0. Bu, kesit sabit alt kümede bir ölçümdür, lifin ekonomik olmadığı sonucu değildir (§9, §10).

## 8. Lif grup grup sıfırlama ve F0 duyarlılığı (deterministik; kesitler sabit, çelik/etriye yeniden türetilir)

**Sıfırlama (`F0_zerofibre.txt:5–8`, fiyat 0):** B_S1 +777,34 TL, B_S2 +1 779,15 TL, B_S3 +757,65 TL, hepsi birden +3 314,15 TL (K2: +4 384,12 TL; `analysis2/F0_zerofibre.txt:8`). Tüm dozlar 0 ve F0 kesitleri korunursa (F0Z) maliyet 237 504,92 TL olur, P'nin (236 456,22) **üstünde**; F0'ın lifsize göre avantajı (−2 265,46 TL) kesit küçültmesiyle birlikte lifin sağladığı çelik/etriye azalmasından gelir ve lif ücreti 0 TL/kg varsayımına bağlıdır.

**F0 vektörü, duyarlılık (toplam maliyet TL; K2 değeri parantezde; hepsi T = 0):**

| Değişiklik | K4 | K2 | Kaynak |
|---|---|---|---|
| ana format, fR ×1,0 | 234 190,76 | 233 916,61 | `F0_evalonly.txt:4`; `analysis2/F0_evalonly.txt:4` |
| ana format, fR ×0,6 | 235 216,44 | 235 199,47 | `F0_evalonly_delta_fr0.6.txt` |
| ana format, fR ×0,8 | 234 726,56 | 234 585,80 | `F0_evalonly_delta_fr0.8.txt` |
| ana format, fR ×1,2 | 233 606,17 | 233 189,09 | `F0_evalonly_delta_fr1.2.txt` |
| MC2010-yerel, fR ×1,0 / ×0,8 / ×1,2 | 234 190,76 / 234 726,56 / 233 606,17 | 233 916,61 / 234 585,80 / 233 189,09 | `F0_evalonly_mc2010_fr*.txt` |
| Ö1 formülü, fR ×1,0 | 234 190,76 | 233 916,61 | `F0_evalonly_o1_fr1.0.txt` |
| lif fiyatı 45 / 90 / 135 / 180 TL/kg (kesit sabit) | 245 710,76 / 257 230,76 / 268 750,76 / 280 270,76 | 245 076,61 / 256 236,61 / 267 396,61 / 278 556,61 | `F0_evalonly_fp*.txt` |

**Kalem bazlı ayrışma, kesit sabit (F0 ↔ F0Z; `F0_evalonly*.txt:7`, `F0Z_evalonly.txt:7`; hakem K9):**

| Değerlendirme (F0 kesitleri) | Boyuna donatı (TL) | Etriye (TL) | Kiriş toplamı (TL) | Lif dışı tasarruf (F0Z'ye göre; boyuna / etriye / toplam) |
|---|---|---|---|---|
| F0Z (doz 0, lifsiz) | 56 475,64 | 14 431,43 | 149 438,41 | — |
| ana format, fR ×1,0 (= MC2010-yerel = Ö1 formülü) | 54 227,88 | 13 365,04 | 146 124,26 | 2 247,76 / 1 066,39 / 3 314,15 |
| ana/MC2010-yerel, fR ×0,6 | 55 253,56 | 13 365,04 | 147 149,93 | 1 222,08 / 1 066,39 / 2 288,48 |
| ana/MC2010-yerel, fR ×0,8 | 54 763,68 | 13 365,04 | 146 660,06 | 1 711,96 / 1 066,39 / 2 778,35 |
| ana/MC2010-yerel, fR ×1,2 | 53 643,29 | 13 365,04 | 145 539,67 | 2 832,35 / 1 066,39 / 3 898,74 |

Okuma: (1) **Kesme formatı varyantları (ana ≡ MC2010-yerel ≡ Ö1 formülü) F0'da özdeştir**: etriye tüm fR ve format varyantlarında 13 365,04 TL, çünkü etriye aralığı dayanımla değil Av,min/smax ile belirlenir (`F0_evalonly.txt:31`, `requiredAvS_byStrength False`); 2 466 başabaş satırının hiçbirinde delta ≠ mc2010 (`breakeven_designs.csv`). Bu nedenle §11.1 kural 2'nin kesme bacağı (format üçlü sınaması) F0'da **bilgisiz**dir; "üç değerlendirmede korunuyor" cümlesi gücü olduğundan yüksek gösterir ve **yazılmaz**. (2) Lifli/lifsiz fark kaynağı: etriyede lifli kirişin Av,min'e inmesi (−1 066,39 TL; fR'den bağımsız) ve boyuna donatıda **eğilme kredisi** (−1 222 ile −2 832 TL, fR ×0,6–1,2); ACI 318-14 eğilmede lifi tanımaz (R26.4.2.1(a)(11); `CODE_PROVISIONS_EVIDENCE` A62), yani lif dışı tasarrufun 2 247,76 / 3 314,15 ≈ %68'i ACI dışı eğilme kredisinden gelir. (3) Eğilme formatı (γF = 1,5 + φ) için varyant **yoktur**; γF = 1,3 duyarlılığı yapılmadı (açık). Sonuç: "lif maliyeti düşürür" cümlesi (§11.1 kural 2 koşulu) bu veriyle **yazılmaz**; yazılabilen: "lif fiyatı 0 TL/kg varsayımında ve belirtilen modelde, F0 kesitlerinde lif dışı kalemler 2 288–3 899 TL (fR ×0,6–1,2) azalır; tasarruf büyüklüğü fR'ye ve ACI dışı eğilme kredisine bağlıdır". Kesit sabit duyarlılıklar yeniden optimize edilmemiştir; lif fiyatı 0 TL/kg'dır. 45 TL/kg'da bile F0 vektörü P'den pahalıdır (245 710,76; kesit sabit).

## 9. Başabaş lif fiyatı (tek tasarım alt sınırı; `breakeven_summary.txt:4–12`)

Tanım: b = (P − lif dışı maliyet) / lif kg; 44 farklı K4 fp0 son vektörü (90 koşu) ve lif gruplarının tüm alt kümeleri, kesitler sabit, çelik yeniden türetilir; P = 236 456,22 TL. Alt sınırdır (yeniden optimizasyon yok).

| Format | fR ×0,6 | ×0,8 | ×1,0 | ×1,2 |
|---|---|---|---|---|
| ana (delta) / MC2010-yerel, tüm alt kümeler, TL/kg | 5,15 | 7,23 | 9,81 | 13,58 |
| tam vektör (üç grup lifli) | 5,15 | 7,23 | 9,47 | 11,90 |
| K2 aynı tabloda (`analysis2/breakeven_summary.txt:4–7`) | 5,40 | 8,11 | 11,73 | 15,66 |

Yorum sınırı: bu değerler, bilinen tasarımlar için lif fiyatının P'ye eşitlendiği alt sınırlardır; "lif ekonomik değil" veya gerçek başabaş fiyatı anlamına gelmez. K4'te alt sınır K2'ye göre fR ×1,0'da 11,73 → 9,81 TL/kg düştü: ℓ/480 kısıtı lifli tasarrufu azalttığı için beklenen yön. ÇŞİDB 2026 lif fiyatı 90 TL/kg bu alt sınırın 6,6–17,5 katıdır (90/13,58 – 90/5,15; fR ×1,0'da 90/9,81 ≈ 9,2); bu tek başına lifin her tasarımda ekonomik olmadığını kanıtlamaz (yeniden optimizasyon yapılmadı; tek ürün, tek fiyat noktası).

**Üst sınır girişimi (hakem K6; `analysis4/sensitivity_defl/breakeven_upper_scale.txt`):** havuzun en sığ kirişi (250×300) yerine **uygun olabilen en küçük** lifli kesit alınır (P kesitleri sabit, grup g'de 4D/20 kg/m³; g3, g8, kesme, g5 sağlanmalı): B_S1 250×500 (40,00 kg), B_S2 250×500 (40,00 kg), B_S3 250×550 (44,00 kg); lif kütlesi tabanı m_min = 40,00 kg. Kaba ölçek (P − C_ref)/m_min = (236 456,22 − 234 190,76)/40,00 = **56,64 TL/kg**, C_ref = bulunan en ucuz fp0 tasarımının lif dışı maliyeti. **Bu bir kanıtlanmış üst sınır değildir**: C_ref kanıtlanmış minimum değildir (daha ucuz lif dışı tasarım varsa ölçek büyür). 90 TL/kg'ın "kilitlenmesi" için her uygun lifli tasarımda C(x) > P − 90·m(x) olmalıdır; m = 40 kg için C > 232 856 TL, yani bulunan en iyi lif dışı maliyetin (234 190,76) 1 335 TL altı; bu kanıtlanamadı (arama küresel en düşüğü garanti etmez). Sonuç: 56,64 < 90 bulgusu 90 TL/kg'ın başabaşın üstünde olduğunu **destekler, kanıtlamaz**; makalede yalnız alt sınır cümlesi (5,15–13,58 TL/kg) ve bu kaba ölçek "approximate scale, not a bound" etiketiyle yazılabilir.

## 10. Dürüst sonuç özeti (yalnız etiketli ifadeler; bulgu değil)

1. [Post-hoc PCR-26b, kampanya 4] Lifsiz ve lifli 90 TL/kg hücrelerinde en düşük maliyet 236 456,22 TL (K2/K3/K4 aynı vektör); g5 bu tasarımlarda bağlayıcı değil (desenli zarf oranı 0,6962).
2. [Post-hoc] Lifli 0 TL/kg en düşüğü 234 190,76 TL; K2 vektörü (233 916,61 TL) K4 kurallarında g5 = 0,0379 ile uygun değil; iki sezgisel aramanın en iyileri arasındaki fark +274,15 TL (+%0,117). Bu bir gölge fiyat değil, test edilmemiş bir farktır (iki ikili, iki kural kümesi); fR ×0,8 duyarlılığından (+536 TL) küçüktür; "ℓ/480'in maliyeti 274 TL" yazılamaz.
3. [Post-hoc, lif fiyatı 0 TL/kg senaryosu] Lifli 0 TL/kg en iyisi lifsizden ≈ %1 (2 265 TL; K2: 2 540 TL) ucuz; "≈ %1 düzeyinde, ℓ/480 altında biraz azalan" biçiminde yazılır. Tasarruf lif ücretsiz varsayımındadır ve kesit küçültmesiyle birlikte oluşur (F0Z kesitleri P'den pahalı). **İstatistik p değeri verilmez**: fp0'da lif bedeli 0 olduğundan lifsiz uzay lifli uzayın alt kümesidir, test arama başarısını ölçer, lif etkisini ölçmez (EX2_SPEC §11 madde 2; fp0 ↔ fp90 farklı amaç fonksiyonu). Betimsel medyan farkı ve p'ler `ex2_stats_campaign4.md` §3'te kalır (ek materyal; yorumlanmaz).
4. [Post-hoc, fiyat 90 TL/kg] BTO/ATO'da lif 0/60 koşuda seçilmedi; YAKO 4/30 koşuda lif seçti ve bu tasarımlar daha pahalı (en düşük 237 799,80 TL). Kesit sabit alt sınır başabaş 5,15–13,58 TL/kg (§9).
5. Yasak: lifin "pahalı/ekonomik değil" olduğu, "optimum", "MaxFE yeterli", algoritma üstünlüğü iddiaları; "lif etriyeyi azaltır" da **yazılamaz** — gerekçe: F0 etriyesi Av,min/smax ile belirlenir (`F0_evalonly.txt:31`, `requiredAvS_byStrength False`), kesme formatı varyantları (ana, MC2010-yerel, Ö1) F0'da özdeştir (§8) ve aynı kesitlerde lifli/lifsiz etriye farkı −1 066,39 TL (F0Z 14 431,43 → F0 13 365,04; `F0Z_evalonly.txt:7`, `F0_evalonly.txt:7`) fR'den bağımsızdır; "K2 → K4 vektörlerinde etriye farkı −149 TL" karşılaştırması yanlıştı (farklı kesitler) ve kaldırıldı. Lif dışı tasarrufun ≈ %68'i boyuna donatıdan (ACI dışı MC2010 eğilme kredisi).
6. Ana metne girecek yön (V3 ve Critic + IA sonrası): tasarruf ≈ %1 düzeyinde, servis öncülüne ve sehim modeline duyarlıdır (§12: F0 ℓ/480 oranı 0,985; ℓ_n, ψ, ani D, Bischoff varsayımlarının hepsinde > 1; P ≤ 0,94), lif fiyatına ve fR'ye duyarlıdır; ≈ %68'i ACI'nin tanımadığı eğilme kredisine dayanır. "F0 ℓ/480'i sağlar" yalnız "under the stated deflection model" sınırıyla. Sayıların hiçbiri V3 ETABS ve Critic + IA öncesi bulgu değildir.

## 11. Açık noktalar (Critic ve IA için)

| No | Konu |
|---|---|
| O1 | g5'te lif sertliği sehim modelinde yok sayılır (lif aleyhine); kaynaksız lif rijitliği tahmini **üretilmedi (DOĞRULANAMADI)**; yalnız Ie = Ig matematiksel üst sınırı raporlandı (§12 V7: F0 0,4605, lif tahmini DEĞİL). Makalede beyan şart. |
| O2 | Zarf Ie, durum-başına Ie'ye göre muhafazakârdır (F0'da 0,9847 ↔ 0,9843); iki varyant aynı yargıyı verir. Modelin **toplam** yanlılığı "muhafazakâr" denemez: sürekli L payı ψ = 0, Branson Ie, rötre yok ve ani D sehmi sayılmaz muhafazakâr olmayan yönlerdir (§12; hakem). |
| O3 | ACI 6.2.6 oranı 1,3755–1,3800 (sınır 1,4): gravite yükü ×0,95/1,00/1,05 duyarlılığı yapıldı (§12.3): oran 1,4'e P için ×1,0375, F0 için ×1,0467 yük çarpanında ulaşır; F0 S3 Q 0,0522 (ötelenmeli), ×0,95'te 0,0496 (ötelenmesiz). EI ve düzlem dışı k duyarlılığı araçta yok: açık. |
| O4 | YAKO lifli 0 TL/kg medyanı lifsizin üstünde (+%0,189) ve YAKO tüm hücrelerde diğer ikisinden çok daha geniş dağılım; YAKO tek boyutlu hamle (EX2_SPEC) nedeniyle 1/40 sınırlaması cümlesi gerekir; algoritma karşılaştırması yapılmaz. |
| O5 | V3 ETABS (P ve F0 dahil; desenli sehim ve lif kesme için bağımsız Excel: E13 ve E20), S2 ve Ö1 bayat-lcs PCR kararı hâlâ açık. **Kapandı (2026-11-19):** R26.4.1.5 kanıtı (A62: asıl alıntı R26.4.2.1(a)(11), PDF 460), lcs kaynağı (A63: MC2010 Denk. 5.6-8; 0,2 h yazar sınırı, F0'da etkin değil). |
| O6 | Lif fiyat taraması yapılmadı; iç Critic BTO+ATO 10'ar koşu fiyat noktasını güçlü öneri olarak bırakmıştı (karar IA'da). Ana metinde "alt sınır, tek tasarım" şerhi şart. |
| O7 | `breakeven_summary.txt` alt sınırdır; 5,15–13,58 TL/kg aralığı fR belirsizliğine (×0,6–1,2) bağlıdır, tek fiyat olarak yorumlanamaz. |

## 12. Hakem turu k4 sonrası deterministik duyarlılıklar (2026-11-19; yeni kampanya yok, dondurulmuş ikili kopyadan)

Araç: `03_Code_Execution/tools/ex2_analysis4_hakem_k4.ps1` (yalnız `ex2_campaign4_bin` DLL'lerinin kamuya açık API'sini çağırır, SHA doğrulanmış kopyadan; ikili ve kaynak değişmedi); çıktılar `04_Results/ex2/analysis4/sensitivity_defl/` (`defl_sensitivity.csv`, `code_tool_match.txt`, `r8_load_sensitivity.txt`, `lcs_floor_F0.txt`, `breakeven_upper_scale.txt`, `F0c2_x.txt`). Etiket: post-hoc, rapor; bulgu değil.

### 12.1 Kod ↔ araç eşleşmesi (iç Critic B3; `code_tool_match.txt:3–6`)
Kütüphane `Deflection2D.PatternedEnvelope` (süperpozisyon motoru) maksimumu ↔ aracın V0'ı (25 durum için ayrı çözüm; farklı motor): P 0,696163517782907, F0 0,98467426953351289, F0Z 0,95482255576832042, F0_c2 1,0379386204194279; **mutlak fark 0** (tolerans 1e-9: 4/4 EŞLEŞTİ). Not: aynı yazarın aynı formülüdür; motor (süperpozisyon ↔ durum başına çözüm) bağımsızdır, formülün (ξ = 2, Branson, 24.2.3.6) bağımsız doğrulaması V3'te Excel ile yapılacaktır.

### 12.2 F0 ℓ/480 oranı varsayımlara duyarlılığı (`defl_sensitivity.csv`; hakem K4/B2)
Her satır desenli (25 durum) en kötü oran; > 1 = ℓ/480 aşılır. ψ = sürekli hareketli yük payı (dSonra = ξ(dD + ψ dL) + (1 − ψ) dL); V4 D'nin ani sehmini de sayar ((1 + ξ) dD + dL); Bischoff Ie ACI 318-19 24.2.3.5 biçimindedir (formül literatürden, kaynak PDF Evidence'ta yok → varsayım); V7 Ie = Ig matematiksel üst sınırdır, lif rijitliği tahmini **değildir**.

| Varyant | P (= F90 = PY) | F0 | F0Z (F0 kesitleri, lifsiz) | F0_c2 (K2 vektörü) |
|---|---|---|---|---|
| V0 taban (eksen ℓ, ψ 0, Branson, zarf Ie) | 0,6962 | **0,9847** | 0,9548 | 1,0379 |
| V1 net açıklık ℓ_n (ℓ − ortalama kolon derinliği) | 0,7459 | **1,0365** | 1,0051 | 1,0926 |
| V2 ψ = 0,25 | 0,7744 | **1,0796** | 1,0463 | 1,1472 |
| V3 ψ = 0,50 | 0,8527 | **1,1746** | 1,1377 | 1,2564 |
| V4 ani D sehmi sayılır | 0,8877 | **1,2871** | 1,2493 | 1,3385 |
| V5 Bischoff Ie (zarf) | 0,8124 | **1,0572** | 1,0208 | 1,1111 |
| V6 Branson Ie, durum başına | 0,6966 | 0,9843 | 0,9545 | 1,0379 |
| V7 Ie = Ig (üst sınır; lif tahmini değil) | 0,3275 | 0,4605 | 0,4605 | 0,4784 |
| V8 ψ 0,25 + Bischoff | 0,8785 | **1,1423** | 1,1031 | 1,2100 |
| V9 ψ 0,25 + Bischoff + ℓ_n (zorlama) | 0,9413 | **1,2024** | 1,1612 | 1,2737 |

Okuma: (1) F0 tabanda ℓ/480'i %1,5 payla sağlar; taban dışındaki **her** gerçekçi varsayım değişikliğinde (V1–V5, V8, V9) aşar (1,04–1,29); yalnız zarf yerine durum-başına Ie (V6) ve Ie = Ig sınırında ≤ 1 kalır. İç Critic'in ℓ_n kaba tahmini (≈ 1,036) araçla doğrulandı (1,0365); hakemin ψ = 0,25 kaba tahmini 1,13 idi, araç 1,0796 verdi. (2) Aynı kesitler lifsiz (F0Z) de aynı yönde aşar: aşım lif etkisi değil, 6 m açıklıkta 500 mm kirişin sonucudur (P 550–600 mm ≤ 0,94 kalır). (3) Sonuç: lifli 0 TL/kg tasarrufunun kanalı (kiriş derinliği azaltımı) sehim varsayımlarına duyarlıdır; "F0 ℓ/480'i sağlar" ancak "under the stated deflection model (axis span, ψ = 0, Branson envelope Ie, immediate D deflection before attachment, fibre stiffness neglected)" biçiminde yazılır. Bu varsayımlarla yeniden optimizasyon yapılmadı; bu varyantlarda bulunacak en ucuz lifli tasarımın P'ye göre durumu bilinmiyor. (4) Lif rijitliği için kaynaklı bir tahmin projede yoktur: **DOĞRULANAMADI**, sayı üretilmedi.

### 12.3 R8: gravite yükü duyarlılığı (`r8_load_sensitivity.txt`)
D, L ve öz ağırlık ×0,95 / 1,00 / 1,05; kesitler sabit, donatı yeniden türetilir; kampanya 4 kuralları. EI ve düzlem dışı k duyarlılığı araçta yok (açık).

| Tasarım | Yük | Maliyet (TL) | T | Q (S1; S2; S3) | δs | ACI 6.2.6 oranı (maks; düzlem içi; dışı) |
|---|---|---|---|---|---|---|
| P | ×0,95 | 233 554,99 | 0 | 0,0604 (ötelenmeli); 0,0776 (ö.); 0,0401 (ötelenmesiz) | 1,064; 1,084; 1 | 1,3543; 1,2322; 1,3543 |
| P | ×1,00 | 236 456,22 | 0 | 0,0636; 0,0817; 0,0422 (ötelenmesiz) | 1,068; 1,089; 1 | 1,3800; 1,2475; 1,3800 |
| P | ×1,05 | 241 332,80 | 0,0048 (g6) | 0,0668; 0,0858; 0,0443 | 1,072; 1,094; 1 | 1,4068 (g6 aşılır) |
| F0 | ×0,95 | 231 419,23 | 0 | 0,0604; 0,0839; 0,0496 (**ötelenmesiz**) | 1,064; 1,092; 1 | 1,3501 |
| F0 | ×1,00 | 234 190,76 | 0 | 0,0636; 0,0883; **0,0522 (ötelenmeli)** | 1,068; 1,097; 1,055 | 1,3755; 1,2550; 1,3755 |
| F0 | ×1,05 | 238 180,94 | 0,0201 (g6 + g5) | 0,0668; 0,0928; 0,0548 | 1,072; 1,102; 1,058 | 1,4018 (g6 aşılır) |

Kritik yük çarpanları (ikiye bölme): ACI 6.2.6 oranı 1,4'e **P için ×1,0375, F0 için ×1,0467**; yani her iki tasarım da gravite yükündeki +%3,8 / +%4,7 artışta g6'yı aşar (marj ≈ %4–5). F0 S3 katı Q = 0,0522 ötelenmeli sınıfta; yük ×0,95'te Q = 0,0496 ötelenmesiz (Q ≈ yükle doğrusal: 0,5 → 0,0261, 1,5 → 0,0783); sınıflama eşiği (0,05) F0'da yük ≈ ×0,96'dadır. Bu bir sınıflama duyarlılığıdır, kısıt ihlali değildir; V3'te ETABS Q ile birlikte bakılmalıdır. Açık: EI (±%5) ve düzlem dışı k = 1,2.

### 12.4 lcs alt sınırı (`lcs_floor_F0.txt`; hakem K3/A3)
F0 kirişlerinde SFRC eğilme lcs (üst yüz / alt yüz): B_S1 435,58 / 484,33 mm; B_S2 374,31 / 423,79 mm; B_S3 439,71 / 479,29 mm; 0,2 h = 110 / 100 / 110 mm → **alt sınır hiçbir yüzde etkin değil** (hepsi `floor_active False`). Kaynak: MC2010 Denk. 5.6-8 lcs = min{srm, y} (A63); 0,2 h yazarların sayısal sınırıdır (kaynakta yok; makalede "authors' numerical lower bound, inactive in the reported design"); srm kolu uygulanmaz (lcs = y).

### 12.5 K3/K4 ikili regresyonu (hakem K8; `04_Results/ex2/regression_pcr26/regression_pcr26.txt`)
`--deflcheck 0 --deflpattern 0` ile K3 (`ee759289…`) ve K4 (`f7207d66…`) ikilileri, K2 ikilisinin (`a61b0c6b…`) 3 tohum × 3 yöntem × {lifsiz, lifli 0, lifli 90 TL/kg} × 500 FE koşusunu (satır 3'ten itibaren: maliyet, T, x, tarihçe) **54/54 dosya çiftinde bit-bit** üretti (fark 0). Yani PCR-26/26b kodu varsayılan kapalıyken kampanya 2 kurallarını değiştirmez. `tools/regression_pcr26.sh`.
