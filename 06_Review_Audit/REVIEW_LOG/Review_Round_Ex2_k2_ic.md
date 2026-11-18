# Review Round — Ö2 kampanya 2 sonucu (iç Critic)

Tarih: 2026-11-17 · Hakem: iç Critic (Claude Code, `CRITIC_PROMPT.md`) · Kapsam: `04_Results/ex2_analysis_campaign2.md`, `04_Results/ex2_stats_campaign2.md`, `EX2_SPEC.md` v2.3 (§3, §5.1, §A.4, §11.1, §11.2, §12, §13), önceki turlar `Review_Round_Ex2_sonuc_ic.md` ve `_dis.md`; ham veri `04_Results/ex2/campaign2/` ve `ex2/analysis2/` (yalnız okundu).
Yapılmayan: kod, ikili, koşu, git, başka dosya değiştirilmedi. Yeniden hesaplar scratchpad dışında, ham CSV üzerinde Python ile (bellekte) yapıldı.

## KARAR: **REVİZYON TALEBİ** (DUR değil)

- Sayılar ham veriyle birebir; önceki turdaki A1/R1–R3 açıkları kapandı. 90 TL/kg sonucu ("BTO/ATO 0/60 koşuda lif seçmedi") sağlam ve sehimden etkilenmiyor (P = F90, ℓ/480 oranı 0,697).
- **0 TL/kg tasarrufu (%1,07) ve başabaş alt sınırı (8,1–15,7 TL/kg) şu an makaleye giremez:** dayandıkları tasarımlar projenin kendi yük varsayımıyla (§3 "30 kN/m duvarı içerir") gerekli olan ℓ/480 sehim sınırını aşıyor. Sehim kararı (aşağıda: **A**) uygulanıp fp0 sonuçları yeniden üretilmeden bu iki sayı yazılmaz.
- Bulgular bölümü kapısı kapalı kalır (V3, S1/S2 yok; §11.1 kural 6).

---

## 1. Bağımsız yeniden hesap (ham CSV, `campaign2/`)

| Kontrol | Sonuç |
|---|---|
| 9 hücre × 30 koşu, FEs = 10 000, T = 0 | 270/270 — birebir (stats2 §1) |
| En iyi / medyan / kötü / std. sapma, en düşüğe ulaşan koşu sayısı (8/15/20; 1/10/23; 1/9/27) | 9 hücrede birebir (stats2 §2, §4) |
| Lif > 0 koşu ve kg aralığı (fp90 YAKO 4/30, 0–50; fp0 137–248 / 217–248 / 238–248) | birebir |
| 9 MWU p ve Holm (m = 9; scipy) | birebir (ör. ATO lifsiz ↔ fp0 9,65e-13 / 7,72e-12; BTO ve ATO lifsiz ↔ fp90 0,373 / 0,399 → Holm 0,745) |
| F0 − P = 233 916,61 − 236 456,22 | −2 539,61 TL, −%1,074 ✓ |
| F0 @ 90 TL/kg = 233 916,61 + 248 × 90 | 256 236,61 TL; P'ye +%8,37 ✓ |
| F0 tek-tasarım başabaşı 2 539,61 / 248 | 10,24 TL/kg ✓ |
| K1 → K2 medyan: 236 456,22 / 230 308,35; 233 916,61 / 228 294,22 | +%2,67; +%2,46 ✓ |
| Sehim F0 E20 desenli (yalnız çatı orta açıklığı yüklü): dAfter = 2·3,757 + 5,460 | 12,974 mm; 12,974 / 12,50 = **1,0379** ✓ (`F0_deflection_patterned.txt:371`) |
| Başabaş maksimumunun tasarımı (BTO fp0 run02) | kalıp 94 112,63 TL = F0 ile aynı → **kiriş kesitleri F0 ile aynı (B_S3 250×500, 6 m)** |

Rakam hatası bulunmadı.

## 2. Sehim kararı (odak): öneri **A**

### 2.1 Sayılar (ℓ/480, ACI 24.2.2 "after attachment"; λ = 2, lif rijitliği yok)

| Tasarım | Tüm açıklıklar yüklü | Desenli L (en kötü) | ℓ/240 desenli | Kaynak |
|---|---|---|---|---|
| P (lifsiz en ucuz) = F90 (90 TL/kg en ucuz; doz 0) | 0,6228 | **0,6966** (S1 orta açıklık, E6) | 0,3483 | `P_deflection*.txt` |
| F0 (0 TL/kg en ucuz) | 0,8808 | **1,0379 AŞILDI** (E20, çatı orta açıklık yalnız) | 0,5190 | `F0_deflection*.txt` |
| F0Z (F0 kesitleri, lif 0) | 0,8505 | **1,0035 AŞILDI** | 0,5018 | `F0Z_deflection_patterned.txt` |

Yani lifsiz ve 90 TL/kg tasarımlarında oran 0,697 (%30 pay); aşım yalnız 0 TL/kg tasarımında. **F0Z de aşıyor → aşımın nedeni lif değil, 6 m açıklıkta 500 mm kiriş (ℓ/h = 12)**; lif As'yi azalttığı için Icr'yi biraz düşürüp aşımı 1,0035 → 1,0379'a büyütüyor (lif rijitlik katkısı olmadığından muhafazakâr).

### 2.2 Kısıt kümesi "bölme taşımaz" ile hangisi tutarlı?

Yalnız §5.1 satır 205'e bakılırsa **C** tutarlıdır: ACI 9.3.1.1 yalnız hasar görebilecek bölme taşımayan kirişlere uygulanır, ACI 24.2.2'de buna karşılık gelen satır ℓ/240'tır (ve anlık L için ℓ/360 kat, ℓ/180 çatı). Ancak **spec'in kendisi iç çelişkili**:

- §3 (satır 160, [K Q3]): "30 kN/m döşeme + kaplama + **duvarı içerir**".
- §A.4 (satır 70, v2.1, kampanya 1'den önce): "ACI 24.2 hesaplı sehim (**ℓ/480**; ℓ/240 ve anlık L ℓ/360 bilgi) … §3'teki 'duvarı içerir' ifadesi bu kararla **ℓ/480 raporunun gerekçesidir**."
- §5.1 (satır 205): "varsayım: kirişler hasar görebilecek bölmeleri taşımaz".

Kiriş üstünde duvar (bu yapı tipinde tuğla dolgu) hasar görebilecek yapısal olmayan elemandır; bu durumda Tablo 9.3.1.1 muafiyeti kullanılamaz, ACI 9.3.2 gereği hesaplı sehim 24.2.2 sınırlarını sağlamalıdır → ℓ/480 bir **şartname gereği** olur, yalnız rapor değil. Kampanyadan önce birincil sınır olarak ℓ/480 seçildi, ℓ/240 "bilgi" etiketiyle verildi.

### 2.3 Seçeneklerin değerlendirmesi

| | Lehine | Aleyhine | Hüküm |
|---|---|---|---|
| **A** ℓ/480 bağlayıcı kısıt + yeniden koşu | §3 yük varsayımı ve §A.4'teki kampanya öncesi ℓ/480 seçimiyle tutarlı; hakem R4'ün iki kabul edilebilir yolundan biri; düzeltme lif aleyhine yönde (sonucu güzelleştirmek için yapılmış sayılmaz); lifsiz ve 90 TL/kg hücreleri etkilenmez (0,697), değişen yalnız fp0 ve başabaş; maliyeti düşük (270 koşu, saatler) | Post-hoc (PCR-26, kampanya 3); 0 TL/kg tasarrufu küçülebilir ya da kaybolabilir | **ÖNERİLEN** |
| **B** yalnız rapor + "ℓ/480 sağlanmıyor" şerhi | Yeniden koşu yok; tam şeffaflık | Kendi yük varsayımımıza göre şartnameyi sağlamayan bir tasarımın tasarrufu ve başabaşı yazılır; başabaş maksimumu da aynı B_S3 = 500 kesitli tasarımdan geliyor (§1 son satır) → başabaş da şerhli olur; R4 (Zorunlu: "ya ℓ/240 rapor, ya ℓ/480 kısıt") karşılanmaz | Yalnız A yapılamazsa geri dönüş yolu; o durumda 0 TL/kg tasarrufu ve başabaş **ana bulgu değil**, yalnız şerhli ek materyal |
| **C** ℓ/240, gerekçeli | §5.1 metniyle tutarlı; yeniden koşu yok; üç tasarım uygun | §3 "duvarı içerir" varsayımının, F0'ın ℓ/480'i aştığı **görüldükten sonra** değiştirilmesini gerektirir (birincil sonuç değiştirme; lif lehine yön) → hakem için en zayıf savunma; Türkiye tipi tuğla dolgulu çerçevede "bölme hasar görmez" fiziksel olarak zayıf | Önerilmez. IA yine de seçerse: §3 metni "duvar yükü yok / hasar görmeyen hafif bölme" diye açıkça değişmeli, post-hoc etiketlenmeli ve ℓ/480 oranları (P 0,697; F0 1,038; F0Z 1,004) makalede aynen verilmeli (B'nin şeffaflığıyla birlikte) |

### 2.4 A uygulanırsa kampanya öncesi yazılacaklar (PCR-26, §11.3 post-hoc)

1. Kısıt (ör. g10): max(dAfter / (ℓ/480)) ≤ 1; dAfter = λ·dD + dL, λ = 2 (ρ′ = 0), sürekli yük = D (SW + G), L geçici; **24 desen + tüm açıklıklar yüklü** zarfı; Ie her yük durumu için (araçtaki "case" varyantı), lif rijitlik katkısı yok (beyan: lif aleyhine muhafazakâr). Hareketli yükün sürekli kısmı alınmaz (beyan).
2. **ℓ tanımı** önceden yazılsın: araç eksen açıklığını kullanıyor (6 000 mm). Net açıklıkla (≈ 5 700 mm) F0 oranı ≈ 12,974 / 11,875 = 1,093, P ≈ 0,733 olurdu (Critic hesabı; araç çıktısı değil). Hangisi seçilirse diğeri duyarlılık olarak rapor edilir.
3. "Desenli L sehimi ACI 24.2'de zorunlu mu" bu pakette **DOĞRULANMADI** (24.2 madde metni kanıt dosyasında yok); desen zarfı muhafazakâr seçim olarak beyan edilir. Kanıt satırı Literature Analyst işi.
4. §5.1 satır 205 varsayımı "kirişler hasar görebilecek duvar taşır; Tablo 9.3.1.1 yerine 9.3.2 hesaplı sehim (ℓ/480) kısıtı" diye düzeltilir; g5 rapor olarak kalır.
5. Kapsam: AGENTS kural 8 (aynı koşul) gereği **270 koşunun tamamı**, aynı tohumlar (1001–1030); kampanya 2 "superseded (PCR-26 öncesi)" etiketiyle ek materyalde. Beklenen: lifsiz ve 90 TL/kg en iyi tasarımları değişmez (P kısıtı sağlıyor ve yeni kısıt yalnız tasarım çıkarır); değişirse o da raporlanır.
6. Ön kayıtlı üçlü koşul (kural 2), başabaş (kural 3, aynı tanım, ℓ/480-uygun tasarımlar üzerinden), doz-0 yanlılık kontrolü (kural 4) yeni seride aynen; iddia hangi seride bozulursa o raporlanır.
7. Fiyat taraması (R6) varsa **bu kısıtla** ve fiyat noktaları kampanyadan önce yazılarak yapılır (§5).

## 3. Ön kayıtlı üçlü koşul (§11.1 kural 2)

Yöntem doğru uygulandı: F0 deterministik olarak (a) Q17, (b) MC2010-yerel, (c) fR × 0,8'de yeniden değerlendirildi; üçünde T = 0 ve F0 < P (−%1,07 / −%1,07 / −%0,79). F0'da etriye üç formatta da smax'ta olduğu için format maliyeti değiştirmiyor; fark yalnız fR üzerinden eğilmeden. **Ancak** kural 2 yalnız kesme/fR boyutunu sınar; tasarımın kısıt kümesine uygunluğunu varsayar. ℓ/480 (§2) bu varsayımı bozduğu için "0 TL/kg'da en ucuz lifli tasarım en ucuz lifsizden ucuz" cümlesi A sonrası yeniden sınanmadan yazılmaz. "Lif etriyeyi azaltır" genel cümlesinin yazılmaması kararı doğru (P'de B_S2/B_S3 Δ etriye 0, Ö1 formatında artış, F0 ↔ P'de +341 TL).

## 4. Kampanya 1 ↔ 2

Doğru ve dürüst sunulmuş: medyanlar +%2,45–2,77 (yeniden hesapla teyit), lif kararı değişmedi (90 TL/kg BTO/ATO 0/30 → 0/30; 0 TL/kg 30/30 → 30/30). İki seri ek materyalde birlikte (§11.2) — seçici raporlama yok. Yazım notu: K1 sayıları yalnız "superseded (desenli yük, kiriş eksenel çekmesi ve tutarlı lcs yok)" etiketiyle; K1'in 0 TL/kg tasarrufu (−%0,87) ve 8,39 TL/kg ana metne girmez. "+%2,5–2,8 kural etkisi" cümlesi nedensel değil betimsel kalmalı (kural değişikliği + arama birlikte; analiz bunu zaten söylüyor).

## 5. "Optimum" dili ve yasak iddialar (§12, §13)

- "Optimum" yok; §4 ve §13 madde 6 ifadesi ("tek- ve iki-koordinat komşuluğunda daha ucuz uygun komşusu olmayan; çapraz teyit, global en iyi kanıtı değil") uygun. Ek: aynı vektöre yakınsama "problem küçüklüğü" ile açıklanır, algoritma kalitesiyle değil (dış hakem §2). A sonrası komşuluk taramaları yeni kısıtla yenilenmeli.
- §13 madde 3 "(%0,79'a iniyor)" ve "ℓ/240 uygun" yan cümlesi C'yi örtük kabul ediyor; A kararıyla bu madde yeniden yazılacak.
- §13 madde 5 ("kesit sabitken lif … B_S1 etriyesini −193,38 TL azaltıyor") genel "lif etriyeyi azaltır" iddiası değil, ama makalede aynı cümlede "B_S2/B_S3'te 0, Ö1 formatında iki durumda artış" yazılmadan kullanılmaz.
- §11'e eklenmeli: F0Z'nin de aştığı (aşımın kesit kaynaklı olduğu) ve aşımın yalnız desenli L'de olduğu (tüm açıklıklar yüklü 0,881). İki bilgi de §13 madde 3'te eksik.
- "ACI 318-14'e uygundur" yasağı A sonrasında da geçerli; ifade "ACI 318-14 with fib MC2010 fibre contributions (declared deviation), within the defined constraint set".
- Yasak listesi (§12 madde 0–13) taslakta ihlal edilmemiş.

## 6. Fiyat taraması (hakem R6)

Alt sınır tablosu (§10) dürüst tanımlı (tek tasarım, alt sınır, fR aralığı). Ama: (i) alt sınırın maksimumu F0 ile aynı kiriş kesitli bir tasarımdan geliyor (BTO fp0 run02; kalıp kalemi F0 ile özdeş) → ℓ/480 altında büyük olasılıkla uygunsuz; (ii) A sonrası fp0 tasarrufu küçülürse alt sınır 90 TL/kg'nın 1/10'undan daha da düşebilir. Öneri: A kampanyasından sonra, **fiyat noktaları önceden yazılarak** (ör. 5, 10, 15, 20, 30 TL/kg), BTO + ATO, 10 koşu, ℓ/480 kısıtıyla küçük tarama. Bu, makalenin tek nicel lif-ekonomisi sayısını (lifin seçilmeyi bıraktığı fiyat aralığı) alt sınırdan doğrudan ölçüme çevirir. Zorunlu değil, güçlü öneri; yapılmazsa "alt sınır, tek tasarım, yeniden optimizasyon yok" şerhi aynen.

## 7. R8 ve R9 ↔ kural 8 çelişkisi

- **R9 ↔ §11.1 kural 8:** gerçek çelişki yok. Kural 8 testin *yapılmasını ve raporlanmasını* ister, yerini belirtmez. Çözüm: 9 testin tamamı (fp0 ↔ fp90 dahil, "farklı amaç fonksiyonu, yorumlanmaz" notuyla) ek materyalde; ana metinde betimsel tablo + deterministik kanıt + "lifli uzay lifsizi kapsadığından test arama verimliliğini lif etkisinden ayıramaz" cümlesi. Hiçbir test satırı atılmaz → seçici raporlama yok, R9 de karşılanır.
- **R8 (düzlem dışı kolon):** belirleyici 6.2.6 oranı düzlem dışı ve sınıra yakın (C_Inter_S1 1,380 / 1,382; sınır 1,4, pay %1,3); tamamen varsayıma (tutulu, k = 1, yalnız e_min) dayanıyor. F0 ↔ P kolon farkı artık küçük (+367 TL), dolayısıyla F0 − P işaretini tek başına çevirmesi olası değil; ama k > 1 ile her iki tasarım g6'yı ihlal edebilir. Bulgular kapısından önce deterministik duyarlılık (k = 1,2 veya lu = eksen, P ve A-sonrası en iyi lifli tasarım) gerekir; donmuş ikilide parametre yoksa ayrı araç/PCR. Sehim kararını bekletmez. Gravitede Q'nun hesap yöntemi (birim yanal yük, 0,70Ig/0,35Ig) yöntem bölümünde açık yazılmalı.
- F0'ın S3 katında Q = 0,0498 (eşik 0,05): sınıflama sınırda; A sonrası tasarımda da bakılmalı, V3'te ETABS çerçeve tipiyle karşılaştırılmalı.

## 8. Makaleye girecek ana mesaj (öneri; A sonrası rakamlarla, rejim kaydıyla)

> "In a gravity-only frame that is a limiting case favourable to fibres (full fib MC2010 credit in flexure and shear), no BTO or ATO run selected steel fibres at the 2026 ÇŞİDB price (90 TL/kg); at fixed sections the savings fibres produced in other cost items were at most about one eighth of their price. The value of fibres came through shallower beams (concrete and formwork), not through stirrups, and it is bounded by serviceability: [A sonrası: ℓ/480 bağlayıcı iken 0 TL/kg senaryosu sonucu]."

Ana bulgu = 90 TL/kg kararı + kalem ayrımı (kanal) + başabaş mertebesi (aralık, alt sınır). 0 TL/kg tasarrufu ikincil duyarlılık sonucudur; A sonrası işareti ne çıkarsa o yazılır. Sehim-lif etkileşimi (sığ kiriş tasarrufu sehimle sınırlanıyor) kendi başına raporlanabilir bir bulgudur, saklanmamalı.

## 9. Yapılacaklar (sıra)

1. IA: sehim kararı (Critic önerisi **A**); §3 / §5.1 çelişkisinin giderilmesi.
2. Developer: PCR-26 (ℓ/480 kısıtı, §2.4 maddeleri) + EX2_SPEC §11.3 post-hoc ön kaydı (kampanyadan önce); lifsiz/lifli bit-bit regresyonu (yeni kısıt kapalıyken kampanya 2 ile aynı).
3. Kampanya 3 (270 koşu) → stats/analysis yeniden üretimi (kampanya 2 superseded, ek materyalde).
4. Öneri: önceden yazılmış fiyat noktalarıyla fiyat taraması (R6).
5. R8 deterministik duyarlılık; V3 (P ve yeni F0) ve S1; sonra Critic + IA.
