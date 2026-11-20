# Review Round: Ö2 kampanya 4 (PCR-26b, desenli ℓ/480, Ie zarfı), iç Critic

Tarih: 2026-11-19 · Hakem: iç Critic (`CRITIC_PROMPT.md`, AGENTS §2) · Kapsam: `04_Results/ex2_stats_campaign4.md`, `04_Results/ex2_analysis_campaign4.md`, `04_Results/ex2/analysis4/`, `04_Results/ex2/campaign4/` (270 koşu CSV), `03_Code_Execution/EX2_SPEC.md` (§11.1–11.4, §12, §13), `ex2_work/Ex2Model/Serviceability2D.vb`, `Ex2Evaluator.vb`, `Frame2D.vb`; önceki turlar `Review_Round_Ex2_k2_ic.md`, `_dis.md`.
Yapılmayan: kod, ikili, sonuç dosyası, git değiştirilmedi. Yeniden hesaplar ham CSV üzerinde bellekte (Python/scipy) yapıldı; ETABS ve koşu çalıştırılmadı.

## KARAR: **KOŞULLU ONAY**

Rakamlar ham veriyle birebir; `PatternedEnvelope` ve g5 kapsamında sonucu çeviren bir hata yok; yasak dil ihlali yok. Koşullar (aşağıda K1–K5) yerine gelmeden F0 (0 TL/kg) tasarrufu ve başabaş alt sınırı ana metne girmez. Bulgular bölümü kapısı (V3, S2, Critic + IA) kapalı kalır; bu karar onların yerine geçmez.

---

## 1. Bağımsız yeniden hesap (ham dosyalardan)

| Kontrol | Yeniden hesap | Sonuç |
|---|---|---|
| 9 hücre × 30 koşu: FEs, T = 0, en iyi/ort./medyan/kötü/std., lif>0 sayısı | `campaign4/*/EX2_*_run??.csv` 3. satır | `ex2_stats_campaign4.md:13–19, 44–52` ile **birebir** (ör. fp0 ATO 234 190,76 / 234 196,94 / 234 190,76 / 234 236,02 / 12,00; fp90 YAKO lif>0 4/30) |
| En iyiler | lifsiz 236 456,22; fp90 236 456,22; fp0 234 190,76 | ✓ |
| %0,958 | (236 456,22 − 234 190,76)/236 456,22 | 0,9581 % ✓ (medyan farkı ATO −2 265,46 ✓; BTO −2 230,44, %0,943 ✓; YAKO +447,20, %0,189 ✓) |
| MWU (plain ↔ fp0) | scipy iki yönlü | BTO 1,005e-11, ATO 1,635e-12, YAKO 0,877: `ex2_stats_campaign4.md:72–76` ile uyumlu; Holm sırası (ATO fp90↔fp0 1,05e-12 ×9 = 9,49e-12; ATO plain↔fp0 ×8 = 1,31e-11) ✓ |
| K2 → K4 farkı | 234 190,7633 − 233 916,6092 (`campaign2/.../fp0_run01.csv:3`) | **+274,15 TL (%0,117)** ✓; kalem ayrışması (kiriş 145 850,10 → 146 124,26; etriye −148,63) toplanınca tutuyor ✓ |
| F0 desenli zarf 0,9847; P 0,6962; F0Z 0,9548 | `F0_deflection_patterned.txt:505`, `P_deflection_patterned.txt:505` | ✓ (durum-başına Ie 0,9843 ≤ zarf 0,9847, beklenen sıra); F0 tüm-yüklü 0,8834, dAfter E13 = 2·3,78 + 3,48 = 11,04 mm, 11,04/12,50 = 0,8834 ✓ |
| K2 F0 vektörü K4 kurallarıyla | `F0c2_evalonly_c4rules.txt:4,21` | maliyet 233 916,61, g5 = 0,03794, Fpen = 233 916,61·(1,03794)² = 252 002 ✓ (Ö1 cezası) |
| Başabaş 9,81 TL/kg (fR ×1,0) | `breakeven_designs.csv` yeniden maksimum (T = 0, lif>0; 308 değerlendirme) | 9,8116 (tüm alt kümeler), 9,4739 (tam vektör); ×0,6/0,8/1,2: 5,1478 / 7,2291 / 13,5816 ✓. Tanım `(P − lif dışı)/kg` yalnız alt sınırdır |
| g5 ihlal sayıları (stats §1.1, analiz §1) | koşu başlıklarından `violation_counts` | K4 plain ATO 17 156, plain YAKO 55 941, fp90 YAKO 73 806, fp0 ATO 44 453; K3 fp0 ATO 37 841, plain ATO 13 515 ✓ |
| `P_marginal_main.txt` | 4D/40 tüm gruplar +23 798,92 (lif 25 200); 5D/35 +20 698,18; boyuna en çok −1 207,70; etriye −193,38 | ✓ (analiz §7) |
| İkili SHA | `ex2_campaign4_bin/Ex2Model.dll` = f7207d66… = koşu dosyası başlığı; `sha256sum -c SHA256SUMS_analysis4.txt` hiç FAIL yok | ✓ |

Rakam hatası bulunmadı. Küçük not: koşu başlığında `Ex2Model built=2026-11-19T05:07:15Z`, analiz araç çıktılarında aynı SHA ile `built=2026-11-19T17:33:35Z` (dosya zamanı kopya tarihi gibi görünüyor). SHA aynı olduğu için sorun değil, ama izlenebilirlik satırında bir cümleyle açıklanmalı (AGENTS kural 7: derleme tarihi).

## 2. Kod incelemesi: `PatternedEnvelope` ve g5

Doğrulananlar (`Serviceability2D.vb`):
- **Süperpozisyon (:279–292, :322–345):** SW ve G moment/yer değiştirmeleri sabit, her kiriş için birim L deseni (`Q_E<n>`, `QD_E<n>`) lineer toplanıyor; 1 + 24 durum (`PatternCombos` Frame2D.vb:201–219: kat başına ve tüm katlar × {1,3},{2},{1,2},{2,3},{1},{3}) ✓. Süperpozisyon doğrusal analizde geçerli.
- **Ie zarfı (:296–316):** `ratioOf` grubun tüm elemanlarındaki en büyük hogging/sagging servis momentiyle Ie(üst), Ie(alt) ortalaması (ACI 24.2.3.6) ✓; `rEnv` = durumlar üzerinde en küçük Ie/Ig; `rD` = yalnız SW + G momentiyle (boş desen listesi) ✓. Zarf ≥ durum-başına (0,9847 ≥ 0,9843) ✓.
- **Orta açıklık sehimi (:224–230):** basit mesnetli kontrol: θI = −θJ = wL³/24EI için L/8·(θI−θJ) − wL⁴/384EI → 5wL⁴/384EI; sağ ankastre → wL⁴/384EI; işaretler tutarlı ✓ (Report ile aynı fonksiyon).
- **dAfter (:352):** `dA = 2·dD + (dDL − dD)`; ξ = 2,0 (ρ′ = 0) ✓. **Not:** `dDL` zarf-Ie modelinden (en küçük Ie), `dD` ise D-durumu Ie'sinden hesaplanıyor; yani dL = dDL(Ie,min) − dD(Ie,D), dD'nin bir kısmını da dL'ye taşıyor. Bu **muhafazakâr** yöne (dA ≈ dD + dDL); sayı "2dD + dL" tanımından biraz büyük ve doküman bunu belirtmiyor (L1).
- **Sınır (:353):** ℓ/480, ℓ = eksen açıklığı (e.Length), tüm kiriş elemanları ✓.
- **g5 (`Ex2Evaluator.vb:182–195`):** `q − 1` (üst sınır 10), NaN/∞ → 10; g3 ihlalinde (`SectionOk/EpsTOk` değil) hesaplanmıyor, −1 (çift ceza ve NaN yok) ✓; ad (`G5PatternName`) kampanya 3 g5'inden ayrı ✓; `DeflPatterned` yalnız `Patterned` ile (Ex2Runner `--deflpattern 1`) ✓; koşu başlığı `deflcheck,True,deflpattern,True` ✓.
- **Yön (lif sertliği yok sayımı):** Mcr (lifsiz fr), Icr (lif çekme katkısı yok), Ec aynı. Gerçekte lif Mcr ve Icr'yi artırır → sehim azalır → ihmal **lif aleyhine** ✓. Lifli kirişte azalan As, Icr'yi düşürüyor; bu doğru yönde ama lif çekme rijitliği kadar telafi edilmiyor (beyan edilmiş, O1).
- **Sürekli yük payı:** sürekli = D (SW + G), L tümüyle geçici; ACI 24.2.4.1.1 ξ = 2 (≥ 5 yıl) ✓. Hareketli yükün sürekli kısmı alınmadı (muhafazakâr olmayan yön yok: L'nin sürekli payı ihmal edilmesi dA'yı düşürür, ama `dDL`'nin D kısmı yukarıdaki kayma ile kısmen karşılanıyor; etkisi dokümanda sayısal olarak verilmemiş, L2).
- **ℓ/480 yalnız desenli mi?** Hayır: `cases(0)` = tüm açıklıklar yüklü + 24 desen; g5 hepsinin zarfı ✓. Kampanya 3 niceliği (tüm-yüklü) bu zarfın alt kümesi.

Bulgular (kodun sonucu çevirmediği, ama makale beyanını etkileyen):

**B1 (ORTA): Yorum satırı ile formül çelişiyor.** `Serviceability2D.vb:168` "all of D is applied before attachment is NOT assumed (whole long-term deflection counted, conservative)" diyor; formül (`:261`, `:352`) yalnız λ·dD (uzun süreli ek) + dL'yi sayıyor, D altındaki **ani** dD'yi saymıyor, yani ACI R24.2.2/24.2.4 okuması: "bağlantıdan önce D'nin ani sehimi oluşur". F0'ın en kötü kirişinde (E13) dD = 3,78 mm; ani D de sayılsaydı oran 0,98 → ≈ 1,15 olurdu. Bu ACI'nin standart okuması olduğundan hata sayılmaz, ama §3'te G = 30 kN/m "duvarı içerir" olduğu için makalede açıkça yazılmalıdır: "immediate deflection under D is assumed to occur before attachment of the partitions; only λ·δD and δL count". Yorum satırı düzeltilmeli (kod değil, açıklama). EX2_SPEC §11.4 bu varsayımı kaydetmiyor.

**B2 (YÜKSEK): ℓ tanımı duyarlılığı yapılmadı (önceki turdan açık).** İç Critic (`Review_Round_Ex2_k2_ic.md` §2.4 madde 2) "ℓ tanımı (eksen/net) önceden yazılsın; diğeri duyarlılık olarak rapor edilsin" demişti. K4 dokümanı yalnız eksen açıklığını kullanıyor. Net açıklık (≈ 5 700 mm) ile F0 oranı yaklaşık 0,9847 × 6 000/5 700 ≈ **1,036 > 1** olur (kaba ölçek, aracın çıktısı değil); P ≈ 0,733 kalır. F0 kısıt sınırında (%1,5 pay) olduğu için bu tasarımın "ℓ/480'i sağlıyor" yargısı ℓ tanımına bağlı ve makalede şerhsiz yazılamaz. F0, `--evalonly` ile net açıklık için tek satırda raporlanabilir (kodda parametre yoksa PowerShell aracıyla).

**B3 (ORTA): Kod ↔ bağımsız araç doğrulaması K4 F0 için yok.** T27 (`Ex2Model.Tests/Program.vb:497–505`) yalnız K2 F0 vektöründe `env ≥ 1,0379 − 10⁻³` ve `env ≥ Report` sınar; eşitsizlik zayıf (üst sınırı yok). K4 en iyileri için kod içi `PatternedEnvelope` maksimumu (P 0,6962, F0 0,9847, F0Z 0,9548) hiç yazdırılmamış; elimizde yalnız `g5 = 0` ve PowerShell araç değeri var. Araç ve kod aynı yazarın aynı mantığı olduğundan "bağımsız" ancak kısmi. İstenen: üç vektör için koddan değer yazdırılması (`--deflection`-benzeri `--evalonly` çıktısı) ve 4 hanede eşleşme.

**B4 (DÜŞÜK): Ek doğrulama zayıflığı.** `PatternedEnvelope` mesnet kolon rijitliğini brüt (1,0 Ig) alıyor, kirişler Ie oranıyla; momentler (Ma) brüt kesitli analizden. Konvansiyonel; beyan yeterli.

## 3. Ön kayıt / post-hoc durumu (§11 kuralları)

- §11.4 başlığı "sonuç görülmeden önce kayıt" diyor: K4 sonuçları için doğru, ama kural (PCR-26b), K1–K3 sonuçları ve K2 F0'ın 1,0379 aşımı **görüldükten sonra** yazıldı; `ex2_analysis_campaign4.md:6` bunu "post-hoc, ön kayıt dışı" diye etiketliyor, `EX2_SPEC.md:481–486` etiketlemiyor. **Düzeltme (K1):** §11.4 başlığına "post-hoc (K1–K3 sonuçları görüldükten sonra)" eklensin; makale/ek cümlesi: "the deflection constraint was added after campaigns 1–3 were seen".
- Yön: değişiklikler lifli çözüm aleyhine (kısıt ve lif sertliği ihmali); lehine seçici raporlama göstergesi yok. Üç seri (K1: superseded; K2; K3; K4) birlikte raporlanıyor, ana seri K4 koşudan **önce** ilan edilmiş ✓ (`EX2_SPEC.md:486`).
- Ie türü (env) seçimi maliyet gerekçesiyle (≈2–3× vs ≈10× süre) yapılmış; case varyantı F0'da 0,9843 (fark 0,0004), aynı yargı → seçim sonucu etkilemiyor ✓ (O2).
- §11.1 kural 2 (lif maliyeti düşürür için üç format birlikte): `analysis4/F0_evalonly_mc2010_*.txt` ve `_delta_*` dosyalarında maliyetler **bire bir aynı** (234 190,76 / 234 726,56 / 233 606,17; `breakeven_designs.csv` delta ≡ mc2010, 2 466 satırın hiçbirinde fark yok). Neden: F0'da etriye tüm formatlarda `Av,min`/smax ile belirleniyor (`F0_evalonly.txt:31`, `requiredAvS_byStrength False`), kesme formatı maliyeti değiştirmiyor. Sonuç: kural 2'nin (a)–(b) bacağı **bilgilendirici değil**; tasarrufun kaynağı boyuna donatı/kesit (eğilme, ACI dışı γF = 1,5 + φ; `F0_zerofibre`, F0Z) ve **eğilme formatı için varyant yok**. Bu yüzden "lif maliyeti düşürür" cümlesi yalnızca fR ×0,6–1,2 (ve lif fiyatı 0) duyarlılığına dayanır; analiz §8 son paragrafı bunu "üç yeniden değerlendirmede korunuyor" diye sunarak gücünü olduğundan yüksek gösteriyor (K3).

## 4. Yasak iddia ve dil denetimi (§12, §13)

Taranan: `ex2_analysis_campaign4.md`, `ex2_stats_campaign4.md`. "optimum", "benchmark", "üstün", "MaxFE yeterli", "ETABS'te doğrulandı", "Duysak ile doğrulandı", "lif ekonomik/pahalı" ifadeleri **yalnız yasak listesi/ret bağlamında** geçiyor; olumlu iddia olarak yok ✓. Yöntemler arası test yok ✓; YAKO/BTO/ATO "çapraz teyit" ✓; komşuluk taramaları "küresel en düşük değil" ✓.

Sınır ihlalleri / düzeltilmesi gerekenler:
- **D1 (ORTA):** `ex2_analysis_campaign4.md:136` ("Dürüst sonuç özeti" madde 3) lifli 0 TL/kg ↔ lifsiz farkını "p_Holm < 1e-10" ile veriyor. Dış hakem (`Review_Round_Ex2_k2_dis.md` §7 madde 7) "lifli ↔ lifsiz istatistiksel anlamlı fark" iddiasını yasakladı (uzay kapsama; fp0 ↔ fp90 farklı amaç). fp0'da lif bedeli 0 olduğundan lifsiz uzay lifli uzayın alt kümesidir; tasarruf yapı gereği ≤ 0'dır, test arama başarısını ölçer. p değeri özetten çıkarılmalı, "ayrıştırılamaz" cümlesi eklenmeli.
- **D2 (DÜŞÜK):** `ex2_analysis_campaign4.md:130` "ÇŞİDB 90 TL/kg bu alt sınırın çok üstündedir" cümlesi, dış hakemin önerdiği ifade ("alt sınırın 5,7–11 katı" gibi sayıyla) yerine nitel; sayı ver (90/9,81 ≈ 9,2; aralıkta 6,6–17,5 kat), "tek başına ekonomik değil kanıtı değil" sonrası cümle korunsun. Ayrıca bu fiyat karşılaştırması yeniden optimizasyonsuz alt sınırdır.
- **D3 (DÜŞÜK):** `ex2_analysis_campaign4.md:130` sonunda "hakem istenen fiyat duyarlılığı IA tarafından reddedildi" ifadesi analiz belgesine ait değil (süreç notu), makaleye taşınmamalı; O6 zaten fiyat noktası taramasını açık bırakıyor.

## 5. Açık maddeler (izleme)

| # | Madde | Durum (K4 sonrası) |
|---|---|---|
| A1 | ACI 6.2.6 oranı 1,3800 (P) / 1,3755 (F0), sınır 1,4; düzlem dışı, tutulu k = 1 (hakem R8) | **AÇIK.** `P_evalonly.txt` col C_Inter_S1 ratio_out 1,3800; `F0_evalonly.txt` 1,3755. F0 ↔ P kolon farkı küçük; duyarlılık (k = 1,2 veya lu = eksen, P ve F0) hâlâ yok. Bu kısıtla F0 (S2/S3 Q 0,0883/0,0522 ötelenmeli) sınıflaması da sınırda. |
| A2 | R26.4.1.5 birebir alıntı | **AÇIK, DOĞRULANAMADI** (`rg R26.4.1.5` yalnız spec/analiz/hakem dosyalarında geçiyor; `01_Literature/` ve `Evidence/` altında A-satırı yok). Makaledeki "ACI lifi yalnız kesmede tanır" cümlesi buna dayanıyor. Literature Analyst işi. |
| A3 | lcs ≥ 0,2h kaynağı / F0'da etkinlik | **AÇIK.** K4 analizinde F0 için lcs/h gösterilmemiş; kaynak ya da "authors' numerical bound" beyanı ve etkin olup olmadığı tek satırda gösterilmeli. |
| A4 | V3 ETABS (P ve F0; desenli sehim ve lif kesme için bağımsız Excel) | **AÇIK** (kapı). |
| A5 | S2 (EN 14651 fR + fc birlikte veren deney) | **AÇIK** (kapı). |
| A6 | ℓ tanımı duyarlılığı (net açıklık) | **AÇIK** (yeni vurgu: B2). |
| A7 | γF duyarlılığı (eğilme; dış hakem önerisi, zorunlu değil) | Yapılmadı. K4 tasarrufunun kanalı eğilme olduğu için (§3) önceliği arttı: F0'ı γF = 1,3 ile tek `--evalonly`. |
| A8 | Spec bayat bölümleri | **AÇIK:** `EX2_SPEC.md:205` ("kirişler hasar görebilecek bölmeleri taşımaz; hesaplı sehim yapılmaz"), `:446` ("hesaplı sehim desenli L ile yapılmadı", "servis yalnız ACI 9.3.1.1"), `:456` (yasak iddia 7) ve `:469` (v2.3 açık listesi) K3/K4 kararlarıyla çelişiyor; §11.3/11.4 dosyanın sonunda §13'ten sonra (numaralama düzeni, AUDIT_CHECKLIST §B). §3 "duvarı içerir" ile §5.1 çelişkisi (iç Critic k2 §2.4 madde 4) giderilmemiş. |
| A9 | Fiyat taraması (O6) | IA kararıyla yapılmadı; ana metinde "alt sınır, tek tasarım" şerhi şart. |
| A10 | YAKO 1/40 sınırlaması (O4) | Beyan cümlesi hazır olmalı; algoritma kıyası yok ✓. |

## 6. Makaleye girebilir / girmez

Girebilir (rejim kaydı + post-hoc etiketi ile, V3 sonrası): 90 TL/kg'da BTO/ATO 0/60 koşuda lif seçmedi; en ucuz lifsiz ve lifli(90) tasarımın aynı olduğu; kısıtın (ℓ/480 desenli) lifsiz tasarımda bağlayıcı olmadığı (0,6962); lifli(0) tasarrufunun K2'den K4'e −%1,074 → −%0,958'e indiği ve kanalın kesit küçültmesi olduğu (F0Z P'den pahalı); başabaş alt sınırı 5,15–13,58 TL/kg (fR ×0,6–1,2; "alt sınır" etiketiyle).

Girmez: "lif maliyeti düşürür" cümlesi (K3: format bacağı vacuous, eğilme formatı sınanmadı), "ℓ/480 sağlanıyor" şerhsiz (B2), p değerleri lif etkisi olarak (D1), V3 öncesi her "doğrulandı" ifadesi, optimum/algoritma üstünlüğü/MaxFE yeterli.

## 7. Koşullar (KOŞULLU ONAY için)

- **K1** §11.4 başlığına post-hoc etiketi; `Serviceability2D.vb:168` yorumu ve B1 varsayımı (ani D sehimi bağlantıdan önce) spec §12 sınırlamalarına yazılsın (kod davranışı değişmez).
- **K2** ℓ duyarlılığı (net açıklık) F0, P, F0Z için raporlansın (B2); F0 > 1 çıkarsa makalede "tasarruf servis marjıyla ve ℓ tanımıyla sınırlı" cümlesi zorunlu.
- **K3** `analysis_campaign4.md` §8 son paragraf ve §10 madde 3 yeniden yazılsın: kural 2'nin kesme bacağı F0'da etriye `Av,min` ile belirlendiği için bilgisiz; "lif maliyeti düşürür" iddiası yazılmaz; p değeri kalkar (D1).
- **K4** Üç en iyi vektör için `PatternedEnvelope` değerleri koddan yazdırılıp PowerShell aracı ile 4 hanede karşılaştırılsın (B3).
- **K5** A8 (spec tutarlılığı), A2 (R26.4.1.5 alıntısı), A3 (lcs ≥ 0,2h) kapanmadan makale yazımına geçilmez; A1 (6.2.6 duyarlılığı) ve A4/A5 bulgular bölümü kapısıdır.

Atıf: Serviceability2D.vb:168, 224–230, 261, 279–357; Ex2Evaluator.vb:182–195; Frame2D.vb:201–219; EX2_SPEC.md:205, 446, 456, 469, 481–489; ex2_analysis_campaign4.md:6, 118, 130, 136, 149; ex2_stats_campaign4.md:13–19, 44–52, 68–76; analysis4/F0_deflection_patterned.txt:505, P_deflection_patterned.txt:505, F0c2_evalonly_c4rules.txt:4,21, F0_evalonly.txt:31, breakeven_summary.txt:4–12, breakeven_designs.csv.
