# Örnek 2 (Ö2) kampanya sonrası deterministik analiz ve sonuç özeti

> **2026-11-17 post-hoc şerhi (PCR-23/24/25; `EX2_SPEC.md` §11.2):** bu belge **kampanya 1** (`04_Results/ex2/campaign/`, ikili `ex2_campaign_bin`) içindir ve kampanya 1 serisi **superseded** etiketlidir. İç Critic (`Review_Round_Ex2_sonuc_ic.md`) 0 TL/kg sayıları için **DUR** verdi (A1: SFRC `Required` bayat-lcs hatası; F0 B_S1 D/C 1,0103); dış hakem (`_dis.md`) desenli yük (ACI 6.4.2) ve kiriş eksenel çekmesi eksikliği nedeniyle BÜYÜK REVİZYON verdi. Düzeltilmiş kodla kampanya 2 (`04_Results/ex2/campaign2/`) koşuluyor; kampanya 1 son vektörlerinin düzeltilmiş kodla yeniden değerlendirmesi `04_Results/ex2/reeval_pcr23_25/`. Bu belgedeki tasarımlar yalnız **tanımlı kısıt kümesine göre uygun (T = 0)**dur; "ACI 318-14'e uygun" denemez (kampanya 1'de desenli yük ve kiriş eksenel kuvveti yoktu). Aşağıdaki 2026-11-17 düzeltmeleri (§8 adlandırma, §11 madde 1, 4, 6) Critic'in rakam denetimine göre yapıldı; diğer sayılar değiştirilmedi.

Durum: **TASLAK, Critic + IA onayı bekler.** Bulgular bölümüne giriş kapısı (§11.1 kural 6): V3 (en iyi çözümlerin ETABS yeniden analizi) ve S2 tamamlanmadan ve Critic + IA onayı olmadan bu belgedeki hiçbir sayı makaleye girmez.
Hazırlayan: Claude Code (Results Analyst), 2026-11-16. Git kullanılmadı; `ex2_campaign_bin`, `ex2_bin`, `ex2_work` kaynakları, `phase2/3_bin`, `program_work` değiştirilmedi; kampanya çıktısı (`04_Results/ex2/campaign/`) yalnız okundu.

**Rejim kaydı (§11.1 kural 1; bu belgedeki her lif cümlesi için geçerli):** bu yük/kısıt rejiminde (Ö2: 2B, yalnız düşey yük, ACI 318-14 gravite, Bölüm 18 yok, ana kesme formatı Q17), tek lif ürünü (65/60 BG, 4D/5D), fR tedarikçi verisi (ortalama/karakteristik durumu belirsiz, Q18), ÇŞİDB 2026 fiyatları ile; genellenemez.

## 0. Girdiler, araçlar, izlenebilirlik

| Kalem | Yer |
|---|---|
| Kampanya (270 koşu, salt-okunur) | `04_Results/ex2/campaign/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv` |
| İstatistik | `03_Code_Execution/tools/ex2_stats.py` → `04_Results/ex2_stats.md` (otomatik), koşu satırları `04_Results/ex2/analysis/ex2_runs.csv` |
| Deterministik araçlar | `03_Code_Execution/tools/ex2_analysis_run.sh` (ikili: `ex2_campaign_bin` **kopyası**, scratchpad; `sha256sum -c` 8/8 OK → `analysis/sha_check.txt`) |
| Araç çıktıları | `04_Results/ex2/analysis/*.txt` (komutlar `commands.txt`; tasarım vektörleri `x/`; SHA-256 `SHA256SUMS_analysis.txt`) |
| İkili SHA (her araç çıktısının 1. satırı) | Ex2Model.dll `d0514e2f…c6b3`, FemRcModel `c1a95951…f5a9`, RcCore `14f84b75…eef`, FemCore `8da9dfd2…0c8` (= ön kayıt §11.1) |

Atıf biçimi: `dosya:satır` (satır = araç çıktı dosyasındaki satır numarası). Araç çıktılarında 1. satır sürüm, 2. satır bayrak satırıdır.

### 0.1 Analiz edilen tasarımlar (hücre en düşük maliyetli koşular)

| Kod | Kaynak koşu | Maliyet (TL) | Not |
|---|---|---|---|
| **P** | `campaign/plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv` (satır 3–4) | 230 308,35 | Lifsiz en düşük; ATO 30/30, BTO 16/30 aynı vektör (`ex2_stats.md` §4) |
| **PY** | `campaign/plain/EX2_YAKO_plain_g9_fr1.00_fp90_run27.csv` | 230 325,40 | YAKO'nun en düşük lifsiz tasarımı (marjinal tablo yöntem başına; BTO = ATO = P) |
| **F90** | `campaign/fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv` | 230 308,35 | Lifli 90 TL/kg en düşük; üç kirişte doz 0 → etkin tasarım P ile aynı |
| **F0** | `campaign/fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv` | 228 294,22 | Lifli 0 TL/kg en düşük; üç kiriş 250×500, 4D/40 kg/m³ (240 kg); ATO 27/30, BTO 11/30 |
| F0Z | F0 kesitleri, tüm dozlar 0 (`x/F0Z.txt`) | 232 275,48 | `--zerofibre` ALL satırının kalem ayrımı (`F0Z_evalonly.txt` = `F0_zerofibre.txt:8`) |

Etkin tasarımlar (kolon b×h, h düzlem içi): P = kolonlar 250×300 (C_Edge_S1–S3, C_Inter_S3), C_Inter_S1 350×350, C_Inter_S2 300×300; kirişler B_S1 250×550, B_S2/B_S3 250×600. F0 = aynı kolonlar ama C_Inter_S1 300×400; kirişler 250×500 4D/40 (`ex2_stats.md` §4).

## 1. Kampanya istatistiği (özet; ayrıntı `04_Results/ex2_stats.md`)

- Kabul: 270/270 koşuda FEs = 10 000 ve T = 0 (`ex2_stats.md` §1).
- Medyan (TL): lifsiz YAKO 231 096,56 / BTO 230 308,35 / ATO 230 308,35; lifli 90 TL/kg 235 123,78 / 230 308,35 / 230 308,35; lifli 0 TL/kg 229 701,14 / 228 340,77 / 228 294,22 (`ex2_stats.md` §2).
- Lif > 0 koşu: 90 TL/kg'da YAKO 5/30 (40–45 kg; tek grup), BTO 0/30, ATO 0/30; 0 TL/kg'da 30/30 her yöntemde (`ex2_stats.md` §2.1).
- Yöntem içi testler (Holm, tek aile m = 9; ön kayıt kural 8: lifli ↔ lifsiz ve fp0 ↔ fp90): lifsiz ↔ 90 TL/kg BTO ve ATO'da fark yok (p_Holm = 1); YAKO'da 90 TL/kg medyanı +4 027,22 TL (+%1,74; p_Holm 9,7e-9; A12 0,95). Lifsiz ↔ 0 TL/kg: YAKO −%0,60, BTO −%0,854, ATO −%0,875 (p_Holm ≤ 1,1e-7). (`ex2_stats.md` §3.) **Lifli uzay lifsiz uzayı kapsadığından bu testler "arama verimliliği" ile "lif etkisi"ni ayıramaz** (§11 madde 2); YAKO'daki 90 TL/kg farkı bu nedenle lif etkisi olarak okunmaz (24 değişken, tek-boyut hamle).
- Çapraz teyit (sıralama değil): BTO ve ATO'nun en düşük tasarımı üç hücrede de aynı vektör (Hamming 0). YAKO'nun en düşüğü lifsizde +17,05 TL (C_Inter_S1 300×400), 0 TL/kg'da +39,60 TL (aynı kolon + B_S3 5D/35), 90 TL/kg'da +492,16 TL (kirişler 550) farklı. Hücre en düşüğüne %1 içinde: lifsiz YAKO 30/30; 90 TL/kg YAKO 4/30; 0 TL/kg YAKO 24/30 (`ex2_stats.md` §4).
- Farklı etkin en iyi tasarım (yöntem başına /30): lifsiz YAKO 25, BTO 4, ATO 1; 90 TL/kg 30/4/1; 0 TL/kg 30/7/4.
- Kat stabilitesi: 270 en iyi tasarımın **tamamında** en az iki kat ötelenmeli (Q > 0,05); 0 TL/kg ATO'da 30/30, BTO'da 25/30 tasarımda üç kat da ötelenmeli; en büyük Q = 0,1046 (≤ 1/3); gravite kat ötelenmesi 0,0000 mm (`ex2_stats.md` §5).

## 2. Her çözümde stabilite, kesme ve D/C (`*_evalonly.txt`)

| | P (= F90) | F0 | Kaynak |
|---|---|---|---|
| Toplam (TL) | 230 308,35 | 228 294,22 | `P_evalonly.txt:4`, `F0_evalonly.txt:4` |
| Kat Q (S1/S2/S3) | 0,0717 / 0,0822 / 0,0438 | 0,0689 / 0,0942 / 0,0542 | `:8` |
| Sınıf | S1, S2 ötelenmeli; S3 ötelenmesiz | üçü ötelenmeli | `:8` |
| δs (S1/S2/S3) | 1,0772 / 1,0895 / 1,0000 | 1,0740 / 1,1040 / 1,0573 | `:8` |
| Gravite kat ötelenmesi; Ms,max | 0,0000 mm; 0,0000 kN·m (simetri) | 0,0000 mm; 0,0000 kN·m | `:8` |
| En büyük ACI 6.2.6 oranı (sınır 1,4) | 1,3028 (C_Edge_S1, düzlem dışı) | 1,3757 (C_Inter_S1, düzlem dışı) | `:29` |
| Kolon P-M D/C en büyük | 0,7089 | 0,8197 | `:29` |
| Kiriş eğilme D/C en büyük | 1,0000 | **1,0103 (B_S1)** — bkz. §10 A1 | `:29` |
| Kiriş kesme D/C en büyük | 0,9812 | 0,7068 | `:29` |
| Vc,f/Vc,ACI (B_S1/B_S2/B_S3) | 1 / 1 / 1 (lif yok) | 2,1016 / 2,0946 / 2,0958 | `:26–28` |
| VRd,F(0; γc = 1)/Vc,ACI | 0,9897 / 0,8990 / 0,8989 | 1,0692 / 1,0625 / 1,0636 | `:26–28` |
| Eq. 7.7-5 doz-0 teriminde bağlayıcı mı | hayır | hayır | `:26–28` |
| Etriye belirleyicisi (kiriş) | smax (Av/s dayanımdan > Av,min); s = 245 / 270 / 270 mm | smax (Av/s Av,min'den); s = 220 mm | `:26–28` |
| F0Z (F0 kesitleri, lifsiz) etriye | — | dayanım; s = 203,7 mm, kesme D/C 1,000 | `F0Z_evalonly.txt:26–28` |

## 3. Tek-adım komşuluk taraması (Q22; `*_scan.txt`)

| Tasarım | Değerlendirme | Daha ucuz uygun komşu (tümü / ±1) | Sonuç | Kaynak |
|---|---|---|---|---|
| P | 90 | 0 / 0 | bu komşulukta yerel en iyi | `P_scan.txt:4–5` |
| F90 | 123 | 0 / 0 | bu komşulukta yerel en iyi | `F90_scan.txt:4–5` |
| F0 | 123 | 0 / 0 | bu komşulukta yerel en iyi | `F0_scan.txt:4–5` |

Yalnız "tek koordinat (ve lifli modda kiriş grubu tip×doz çifti) komşuluğunda daha ucuz uygun tasarım yok" denir; global optimum iddiası yapılmaz (§12 madde 9).

## 4. Kalem bazlı marjinal maliyet (Q21, "kesit sabitken"; `P_marginal_*.txt`)

P kesitleri sabit, her kiriş grubuna sırayla ve hep birlikte (tip, doz) ∈ {4D 20/30/40; 5D 25/35} uygulandı; donatı ve etriye yeniden türetildi; lif 90 TL/kg. Taban: `P_marginal_main.txt:6` (kiriş 143 479,75 TL; toplam 230 308,35 TL; lifli mod doz 0 = lifsiz, `:4`).

| Durum | Lif (kg) | Δ toplam (TL / %) | Δ kiriş (%) | Δ boyuna (TL) | Δ etriye (TL) | Δ beton, kalıp | Etriye belirleyicisi (sonra) | Kaynak |
|---|---|---|---|---|---|---|---|---|
| B_S1 4D/20 | 44 | +3 693,77 / +1,60 | +2,57 | −266,23 | 0,00 | 0 | smax (dayanım) | `:7` |
| B_S1 4D/40 | 88 | +7 418,43 / +3,22 | +5,17 | −501,57 | 0,00 | 0 | smax (Av,min) | `:9` |
| B_S2 4D/20 | 48 | +4 175,07 / +1,81 | +2,91 | −144,93 | 0,00 | 0 | smax (Av,min) | `:12` |
| B_S3 4D/20 | 48 | +4 175,12 / +1,81 | +2,91 | −144,88 | 0,00 | 0 | smax (Av,min) | `:17` |
| ALL 4D/20 | 140 | +12 043,96 / +5,23 | +8,39 | −556,04 | 0,00 | 0 | B_S1 smax (dayanım); B_S2, B_S3 smax (Av,min) | `:22` |
| ALL 4D/40 | 280 | +24 159,53 / +10,49 | +16,84 | −1 040,47 | 0,00 | 0 | üçü smax (Av,min) | `:24` |
| ALL 5D/25 | 175 | +15 011,37 / +6,52 | +10,46 | −738,63 | 0,00 | 0 | üçü smax (Av,min) | `:25` |
| ALL 5D/35 | 245 | +21 056,44 / +9,14 | +14,68 | −993,56 | 0,00 | 0 | üçü smax (Av,min) | `:26` |

(20 durumun tamamı `P_marginal_main.txt:7–26`; tümü T = 0.) **Gevşeyen kısıt:** lif kesme gereksinimini dayanımdan Av,min'e indiriyor ama P kesitlerinde (h = 550/600) aralık her durumda smax = d/2'de kalıyor → **etriye maliyeti 20/20 durumda değişmiyor**; kazanç yalnız boyuna donatıda (eğilmede lif katkısı) ve bu kazanç lif bedelinin yalnız %3–7'si kadar. Yöntem başına: PY (YAKO) aynı tablo, Δ boyuna −144…−1 036 TL, Δ etriye 0 (`PY_marginal_main.txt`). Varyantlar (ALL 4D/40 satırı, Δ boyuna / Δ etriye): fR × 0,8 −788,52 / 0; × 0,6 −562,57 / 0; × 1,2 −1 327,02 / 0; MC2010-yerel −1 040,47 / 0; Ö1 formatı −1 040,47 / 0 (Ö1 formatında B_S1 4D/20 ve ALL 4D/20 durumlarında etriye **+132,82 TL**, 20 durumdan 2'si; `P_marginal_o1.txt:7,22`) (`P_marginal_{fr0.8,fr0.6,fr1.2,mc2010,o1}.txt`).

Kesit sabitken başabaş (−Δ boyuna / kg): tek grup 2,81–6,43 TL/kg, hep birlikte 3,72–4,22 TL/kg (ana format). Bu kanal kesit küçültmeyi göstermez (araç başlığı, `P_marginal_main.txt:3`).

## 5. Lif grup grup sıfırlama (Q19; §11.1 kural 4) ve kesit küçültme kanalı

**F0 (0 TL/kg; `F0_zerofibre.txt:3–8`):** her grupta sıfırlama uygun kalıyor (T = 0) ve maliyeti artırıyor: B_S1 +1 388,85, B_S2 +1 293,57, B_S3 +1 298,83, hepsi +3 981,25 TL → üç grupta da etiket "useful". Kalem ayrımı (F0 → F0Z, aynı kesitler; `F0_evalonly.txt:7` ve `F0Z_evalonly.txt:7`): boyuna −2 933,57 TL, etriye −1 047,68 TL (s 203,7 → 220 mm), beton/kalıp 0.

**F90 (90 TL/kg; `F90_zerofibre.txt`):** üç grupta doz 0 ("no fibre"); sıfırlanacak lif yok.

**Arama yanlılığı kontrolü (§11.1 kural 4) sonucu:** (i) 0 TL/kg'da lif 90/90 koşuda seçildi ve sıfırlama her grupta maliyeti artırıyor; (ii) 90 TL/kg'da BTO/ATO 60/60 koşuda doz 0 ve lifsizle aynı tasarım. İki gösterge birlikte, 90 TL/kg'da lifin seçilmemesinin arama hatasından değil lif bedelinin diğer kalem tasarrufunu aşmasından kaynaklandığıyla tutarlıdır (bu rejimde).

**F0 ↔ P kalem ayrımı (farklı kesitler; `P_evalonly.txt:5,7`, `F0_evalonly.txt:5,7`):** toplam −2 014,13 TL. Kirişler −3 252,03 TL = beton −3 649,35 + kalıp −6 984,24 + boyuna +6 698,31 + etriye **+683,25**; kolonlar +1 237,90 TL (beton −60,22, donatı +1 298,12; F0'da C_Inter_S1 300×400 ve C_Edge_S3 etriye aralığı 120 mm, P'de 250 mm; `F0_evalonly.txt` kolon satırları). Yani 0 TL/kg'daki tasarruf **kesit küçültme kanalından** (kiriş h 550/600 → 500: beton + kalıp) geliyor; en düşük lifsiz tasarıma göre etriye ve boyuna donatı **artıyor**.

## 6. Ön kayıtlı üçlü koşul (§11.1 kural 2) — F0 tasarımının deterministik yeniden değerlendirmesi

Referans: en düşük lifsiz tasarım P = 230 308,35 TL (lifsiz değerlendirme kesme formatından ve fR'den bağımsız). Lif 0 TL/kg. Yeniden optimizasyon yok.

| Değerlendirme | F0 maliyet (TL), T | F0 − P (TL / %) | Lif sıfırlama kazancı, aynı kesit (TL) | F0 etriye (TL) / belirleyici | F0Z etriye (TL) | Kaynak |
|---|---|---|---|---|---|---|
| **(a) ana format Q17** | 228 294,22, 0 | −2 014,13 / −0,875 | 3 981,25 | 13 662,30 / smax (Av,min) | 14 709,98 (dayanım) | `F0_evalonly.txt:4,7`; `F0_zerofibre.txt:8` |
| **(b) MC2010-yerel** | 228 294,22, 0 | −2 014,13 / −0,875 | 3 981,25 | 13 662,30 / smax (dayanım) | 14 709,98 | `F0_evalonly_mc2010_fr1.0.txt`; `F0_zerofibre_mc2010_fr1.0.txt` |
| **(c) fR × 0,8** | 229 039,85, 0 | −1 268,50 / −0,551 | 3 235,62 | 13 662,30 / smax (dayanım) | 14 709,98 | `F0_evalonly_delta_fr0.8.txt`; `F0_zerofibre_delta_fr0.8.txt` |
| bilgi: Ö1 formatı | 228 294,22, 0 | −2 014,13 / −0,875 | 3 981,25 | 13 662,30 / smax (dayanım); kesme D/C 0,975 | 14 709,98 | `F0_evalonly_o1_fr1.0.txt` |
| bilgi: fR × 0,6 | 229 664,51, 0 | −643,84 / −0,280 | 2 610,97 | 13 662,30 / smax (dayanım) | 14 709,98 | `F0_evalonly_delta_fr0.6.txt` |
| bilgi: fR × 1,2 | 227 595,17, 0 | −2 713,18 / −1,178 | 4 680,31 | 13 662,30 / smax (Av,min) | 14 709,98 | `F0_evalonly_delta_fr1.2.txt` |

**Karar (kural 2 birebir):**
- **"Lif (0 TL/kg'da) toplam maliyeti azaltır"**: (a), (b), (c) üçünde de korunuyor (F0 uygun ve P'den ucuz) → **yazılabilir**, yalnız rejim kaydıyla, yalnız 0 TL/kg için ve "en düşük lifli tasarımın deterministik yeniden değerlendirmesi" olarak. 90 TL/kg'da iddia **yazılamaz** (lif seçilmedi; F0 tasarımı 90 TL/kg'da 249 894,22 TL, P'den +%8,50; `F0_evalonly_fp90.txt:4`).
- **"Lif etriyeyi azaltır"**: aynı kesitte (F0 ↔ F0Z) üç değerlendirmede de etriye −1 047,68 TL; ancak en düşük lifsiz tasarıma göre etriye +683,25 TL ve P kesitlerinde (marjinal tablo) 20/20 durumda Δ etriye = 0. Ön kayıt kuralı karşılaştırma referansını tanımlamıyor; üç referanstan ikisinde iddia tutmadığı için **genel cümle yazılmaz** (Critic açık noktası K2). Yazılabilecek en dar ifade: "250×500 kesitte lif, kesme etriyesi aralığını dayanım belirleyiciliğinden smax'a taşıdı".

## 7. Duyarlılık (deterministik; bilgi)

**Lif fiyatı** (F0 tasarımı, kesit ve donatı sabit; lif 240 kg; `F0_evalonly_fp{45,90,135,180}.txt:4`; P ve F90 lif içermediğinden fiyattan bağımsız 230 308,35 TL):

| Lif fiyatı (TL/kg) | ×0 | ×0,5 (45) | ×1 (90) | ×1,5 (135) | ×2 (180) |
|---|---|---|---|---|---|
| F0 maliyet (TL) | 228 294,22 | 239 094,22 | 249 894,22 | 260 694,22 | 271 494,22 |
| F0 − P (%) | −0,87 | +3,81 | +8,50 | +13,19 | +17,88 |

**fR ve kesme formatı:** §6 tablosu (F0) ve §4 varyant satırları (P, kesit sabit). Tüm varyantlarda F0 uygun kalıyor (T = 0); Vc,f/Vc,ACI: ana 2,09–2,10; MC2010-yerel 1,97–1,98; Ö1 1,16–1,18; fR × 0,8 1,97; × 0,6 1,81–1,82; × 1,2 2,21 (`F0_evalonly*.txt:26–28`).

## 8. Başabaş lif fiyatı (§11.1 kural 3)

Tanım (**tek tasarım başabaşı**, 2026-11-17 düzeltmesi: önceki "sistem düzeyi" adı yanlıştı): F0 tasarımının lif dışı maliyeti en düşük lifsiz tasarım P'ye eşit olacak lif fiyatı = (P − F0 lif dışı) / 240 kg. Bu, bulunan tek bir lifli tasarıma (F0) bağlı değerdir ve gerçek başabaşın **alt sınırıdır** (fiyat bunun altındayken en az bir lifli tasarım bulunan en ucuz lifsizden ucuzdur; üstünde daha az lifli başka bir tasarımın kazanıp kazanmadığı bilinmez); sistem düzeyi başabaş değildir. P de kanıtlanmış optimum değil, bulunan en ucuz lifsiz çözümdür. Sayılar A1 (PCR-23) nedeniyle geçersizdir (iç Critic DUR); kampanya 2 ile yeniden üretilecektir.

| Değerlendirme | Başabaş (TL/kg) | ÇŞİDB 90 TL/kg'ın |
|---|---|---|
| (a) ana Q17 | 2 014,13 / 240 = **8,39** | 1/10,7'si |
| (b) MC2010-yerel | 8,39 | — |
| Ö1 formatı | 8,39 | — |
| (c) fR × 0,8 | 1 268,50 / 240 = **5,29** | — |
| fR × 0,6 | 2,68 | — |
| fR × 1,2 | 11,30 | — |

Diğer tanımlar (bilgi): kesit sabitken (P kesitleri) 2,81–6,43 TL/kg (§4); F0 kesitlerinde grup başına lif sıfırlama 16,2–17,4 TL/kg (1 388,85/80 vb.; aynı kesitte lifsiz alternatif en ucuz lifsiz tasarım olmadığından üst gösterge). **Not:** F0 tasarımı başabaşı 8,39 TL/kg, fiyat = 0 sağlamasındaki kaba gösterge (≈ 8,4) ile sayısal olarak aynıdır çünkü iki çalışmada da aynı iki tasarım bulundu; burada kullanılan değer kampanya dosyalarından hesaplanmıştır (§11.1 kural 3 gereği sağlama değeri kullanılmadı). **Şerh (2026-11-17):** fiyat = 0 sağlama sınaması (tohum 9101–9105) ön kayıttan **önce** görülmüştü ve kural 2–4 bu bilgiyle yazıldı; "sonuç görülmeden yazıldı" beyanı yalnız kampanya sonuçları içindir. Başabaş deterministiktir: fiyat 0 < p < 8,39 TL/kg aralığında optimizasyonun başka (daha az lifli) bir tasarım seçip seçmeyeceği yeniden optimizasyon gerektirir (yapılmadı).

## 9. Desenli yük (ACI 6.4.2; kısıt değil, [K b]; `*_patterns.txt`)

| | P (= F90) | F0 | Kaynak |
|---|---|---|---|
| Tasarlanmış donatıyla kiriş eğilme D/C (B_S1/2/3) | 1,155 / 1,181 / 1,227 | 1,137 / 1,152 / 1,209 | `:6–8` |
| Kiriş kesme D/C | 1,014 / 0,958 / 0,961 | 0,726 / 0,729 / 0,734 | `:6–8` |
| Kolon D/C en büyük; 6.2.6 oranı en büyük | 0,751; 1,316 | 0,902; 1,380 | `:9–14` |
| 1'i aşan kontrol | 4 | 3 | `:15` |
| Desenli zarfla yeniden türetme | 236 316,17 TL (+6 007,82; +%2,61), T = 0 | 234 144,28 TL (+5 850,06; +%2,56), T = 0 | `:16` |
| Desenli zarfta kat ötelenmesi (S1/S2/S3), Ms,max | 0,070 / 0,105 / 0,183 mm; ≤ 1,17 kN·m | 0,123 / 0,148 / 0,288 mm; ≤ 2,46 kN·m | `:3` |

Tek desen kabulü (sınırlama) her iki tasarımda eğilmede %14–23 güvensiz yönde D/C bırakıyor; yeniden türetme maliyeti ≈ %2,6. Bu, makalede sınırlama olarak yazılmalı.

## 10. Sehim (ACI 24.2; rapor, kısıt değil; `*_deflection.txt:14`)

| | ℓ/480 en kötü oran | ℓ/240 | anlık L ℓ/360 | Belirleyen |
|---|---|---|---|---|
| P (= F90) | 0,661 | 0,331 | 0,197 | E6 (B_S1, 6 m) |
| F0 | **0,936** | 0,468 | 0,231 | E20 (B_S3, 6 m) |

Tümü ≤ 1 (OK). F0'nun sığ (500 mm) kirişleri ℓ/480 sınırına yakın; lifin rijitlik/çatlak sonrası katkısı hesapta yok (AÇIK-21 varsayımları).

## 11. Dürüst sonuç özeti (yalnız ön kayıtlı iddialar)

1. Kampanya 270/270 koşu **tanımlı kısıt kümesine göre** uygun (T = 0) ve 10 000 FE ile bitti (ACI 318-14 uyumu iddia edilmez: desenli yük ve kiriş eksenel kuvveti kampanya 1 kısıt kümesinde yoktu; §9, BULGU-2).
2. **Lif 90 TL/kg'da:** BTO ve ATO hiçbir koşuda lif seçmedi; lifli senaryonun en düşük tasarımı lifsiz en düşük tasarımla aynı (230 308,35 TL). "Lifsiz ve lifli (90 TL/kg) aynı, çünkü lif seçilmedi." YAKO 5/30 koşuda tek grupta lif seçti ve bu koşular daha pahalı kaldı (arama verimliliği; lif etkisi olarak okunmaz).
3. **Lif 0 TL/kg'da:** üç yöntem 90/90 koşuda üç kiriş grubuna da lif seçti; en düşük maliyet 228 294,22 TL, lifsiz en düşükten 2 014,13 TL (%0,87) az. Tasarruf kiriş kesitinin küçülmesinden (beton + kalıp) geliyor; boyuna donatı ve etriye en düşük lifsiz tasarıma göre artıyor. Bu tasarruf MC2010-yerel formatta ve fR × 0,8'de de korunuyor (deterministik; −%0,55'e kadar azalıyor).
4. F0 tasarımı için başabaş lif fiyatı 8,39 TL/kg (tek tasarım; gerçek başabaşın alt sınırı, sistem düzeyi değil; fR × 0,8'de 5,29 TL/kg); ÇŞİDB 90 TL/kg bunun ≈ 10,7 katı. **A1 nedeniyle geçersiz (DUR); kampanya 2 ile yeniden.**
5. Kesit sabitken (en düşük lifsiz tasarım) lif etriyeyi değiştirmiyor (smax belirleyici); yalnız boyuna donatıyı lif bedelinin %3–7'si kadar azaltıyor.
6. BTO ve ATO üç senaryoda da aynı en düşük tasarıma ulaştı; YAKO'nun en düşük çözümü aynı bölgede (**%0,007–0,21** içinde: lifsiz %0,0074, 0 TL/kg %0,0173, 90 TL/kg %0,2137; 2026-11-17 düzeltmesi, önceki "%0,02–0,21" hatalıydı); lifsizde bir kolon grubunda (C_Inter_S1 300×400 ↔ 350×350), 0 TL/kg'da bir kiriş grubunun lif tipi/dozunda (B_S3 5D/35 ↔ 4D/40), 90 TL/kg'da bir kolon grubu (C_Inter_S1) ile iki kiriş grubunda (B_S2, B_S3 550 ↔ 600) farklı (`ex2_stats.md` §4). Bu yalnız çapraz teyittir; yöntem sıralaması yapılmaz.
7. En düşük tasarımlar tek-koordinat komşuluğunda yerel en iyidir; "optimum" denmez.

**Yazılmayacaklar (§12, §11.1):** lif "ekonomiktir / değildir" genel cümlesi; "lifli medyan yüksek, lif pahalıdır"; "lif etriyenin yerini alır" (Av,min her zaman var; P'de etriye smax'ta); "lif etriyeyi azaltır" genel cümlesi (§6); lif servis performansını iyileştirir; algoritma üstünlüğü; Ö1 ile nedensel karşılaştırma; ETABS'te doğrulandı (V3 yapılmadı); 1 000 FE ara değeri; ABC/ön tasarım (Ö2'de yok).

## 12. Critic için açık noktalar

- **A1 (olası kod tutarsızlığı, bu pakette düzeltilmedi):** F0 B_S1 eğilme D/C = 1,0103 (`F0_evalonly.txt:26`; fR × 1,2'de 1,0038) — T = 0 bir tasarımda. `GravityDesign.vb` `Required` (SFRC: bisection içinde lcs eşzamanlı güncelleniyor, 60 adım) ile `PhiMn` (sabit As'de 20 adım lcs sabit noktası) farklı lcs'ye yakınsıyor; gerekli As güvensiz yönde ≈ %1. DcFlex kısıt değil, yalnız rapor. Lifsiz kirişlerde etki yok (D/C 1,0000). Developer/IA kararı: PCR gerekip gerekmediği, V3 Excel'inde lifli Mn bağımsız kontrolü (S1).
- **K2:** §11.1 kural 2'deki "lif etriyeyi azaltır" iddiasının referansı (aynı kesit mi, en düşük lifsiz tasarım mı) ön kayıtta tanımlı değil; bu belgede muhafazakâr yorumla genel cümle yazılmadı.
- **K3:** Holm ailesi m = 9 (ön kayıt kural 8'deki fp0 ↔ fp90 dahil); görevlendirmede yalnız 6 test istenmişti. Bağ (tüm değerler aynı) durumunda p = 1 yazıldı (ATO lifsiz ↔ 90 TL/kg).
- **K4:** Başabaş fiyat deterministik ve tek tasarıma bağlı; 0–8,39 TL/kg aralığında yeniden optimizasyon yapılmadı. Ön kayıt bunu istemiyor; makale cümlesi "bu tasarım için" diye sınırlanmalı.
- **K5:** 270 en iyi tasarımın tamamında ötelenmeli kat var; F0'da üç kat da ötelenmeli ve 6.2.6 oranı 1,376 (sınıra %1,7). Kısıtlar aktif bölgede; V3 ETABS'te (kat Q ↔ çerçeve tipi, lu oranı) yeniden doğrulanmalı.
- **K6:** Desenli yük her iki tasarımda eğilmede D/C 1,14–1,23; tek desen kabulünün etkisi %2,6 maliyet. Hakem bunu ana sınırlama sayabilir.
- **K7:** F0 sehim oranı 0,936 (ℓ/480); lif rijitlik katkısı ve uzun süreli sünme varsayımları (AÇIK-21) sınıra yakınlık nedeniyle önemli.
- **K8:** YAKO tek-boyut hamle (beyan) ve 90 TL/kg'da 24 değişkenli uzayda daha yüksek medyan: testler lif etkisi olarak raporlanmamalı.
- **K9:** 0 TL/kg senaryosunda kolon donatısı +1 298 TL (C_Edge_S3 etriye 120 mm): kolon kesme/etriye kuralının nedeni ayrıca incelenebilir (bilgi).
- **K10:** V3 (ETABS) ve S2 tamamlanmadı → bulgular bölümü kapısı kapalı.
