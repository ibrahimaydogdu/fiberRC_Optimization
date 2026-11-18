# Dış hakem raporu: Örnek 2 (Ö2) kampanya 2 (post-hoc PCR-23/24/25)

**Dergi:** *Structures*. **Rol:** bağımsız dış hakem (RC çerçeve optimizasyonu, ACI 318, SFRC). İç Critic raporları okunmadı.
**Tarih:** 2026-11-17. **Okunan:** `04_Results/ex2_analysis_campaign2.md`, `04_Results/ex2_stats_campaign2.md` (§1–3), `03_Code_Execution/EX2_SPEC.md` v2.3 (§11.1, §11.2, §12, §13), önceki raporum `Review_Round_Ex2_sonuc_dis.md`. Ham veri (`04_Results/ex2/campaign2`) açılmadı/değiştirilmedi; koşu, ikili, git dokunulmadı.

## KARAR: KÜÇÜK REVİZYON

Önceki turdaki üç zorunlu kusur (desenli yük, bayat lcs, kiriş eksenel çekmesi) kod düzeyinde kapatılmış, kampanya bütünüyle yeniden koşulmuş ve eski seri seçici raporlama olmadan korunmuş. Ana bulgu (90 TL/kg'da BTO/ATO 0/60 lif; lifin diğer kalemlerdeki değeri fiyatının küçük bir kesri) düzeltmelerden sonra değişmedi ve artık daha sağlam. Kalan işler yazım/beyan düzeyindedir. **Tek koşul:** sehim kriteri aşağıdaki öneriyle (C + açık şerh) sabitlenmeli ve 0 TL/kg cümlesi ona göre yazılmalıdır. V3 (ETABS) ve S1 (lifli Mn bağımsız kontrolü) bulgular bölümü kapısı olarak geçerliliğini korur; bu karar onların yerine geçmez.

---

## 1. Önceki taleplerin durumu

| # | Talep | Durum | Not |
|---|---|---|---|
| R1 | ACI 6.4.2 desenli zarf kısıt olarak + yeniden koşu | **KARŞILANDI** | 26 kombinasyon tasarım zarfında (PCR-25); A59/A60 ile 6.4.3.2 muafiyetinin döşemeye özgü olduğu kanıtlanmış. Tek-desen donatısının desenli D/C'si 1,14–1,22 → bu düzeltme gerekliydi; maliyet etkisi +%2,1. |
| R2 | A1: sabit nokta lcs; EpsT aynı lcs; birim test; eski 270'in yeniden değerlendirmesi | **KARŞILANDI** | g8 270/270 dosyada hiç ihlal edilmemiş (kurgu gereği D/C ≤ 1 ile tutarlı). Lifsiz yol 9/9 bit-bit regresyon iyi uygulama. `reeval_pcr23_25` var. Birim testin (Mu/φMn(Required) ≤ 1+1e-6) ek materyalde bir satırla anılması yeterli. |
| R3 | Kiriş eksenel çekmesi | **KARŞILANDI** | PCR-24; 22.5.7.1 Vc katsayısı 0,97–0,98. "Grup en büyük Nt + moment zarfı eşlemesi" muhafazakâr; beyan yeterli. |
| R4 | Sehim desenli L ile; ℓ/480 ↔ "bölme taşımaz" tutarsızlığı | **KISMEN** | Hesap yapıldı (24 desen, env-Ie varyantı). Karar verilmedi → §2. |
| R5 | Karma format beyanı | **BÜYÜK ÖLÇÜDE** | §12 tablosu kodla birebir ve doğru. İki kanıt boşluğu + bir duyarlılık eksik → §3. |
| R6 | Başabaş: fiyat taraması ya da "alt sınır" ifadesi + fR aralığı | **KARŞILANDI (ifade yolu)** | Alt sınır 44 vektör × alt küme üzerinden en büyüklenmiş; öngörüm (daha az lifli tasarım daha yüksek b verir) doğrulandı (11,73 > 10,24). Üst sınır eklenmeli → §4. |
| R7 | İki-koordinat tarama + kardinalite | **KARŞILANDI** | 3 tasarımda 0 daha ucuz uygun komşu. |
| R8 | Düzlem dışı kolon duyarlılığı; gravitede Q tanımı | **AÇIK** | Önemi azaldı: F0 ↔ P kolon farkı artık +367 TL (tasarrufun %14'ü; önceki turda %61). Yine de belirleyici 6.2.6 oranı (1,380–1,382) düzlem dışı ve varsayıma bağlı; yöntem bölümünde Q'nun yatay kuvvet tanımı yazılmalı. k = 1,2 ile tek `--evalonly` (P, F0) yeterli; işaret korunuyorsa bir cümle. Zorunlu değil, güçlü öneri. |
| R9 | Testler ek materyale; fp0 ↔ fp90 raporlanmasın | **ÇÖZÜLEBİLİR** | Ön kayıt kural 8 testleri *raporlamayı* ister, *ana metinde* raporlamayı istemez. Çatışma yok: ek materyalde tam tablo (fp0 ↔ fp90 satırı "farklı amaç fonksiyonu, yorumlanmaz" notuyla); ana metin betimsel + deterministik. |
| R10 | 9.6.3.1 muafiyeti cümlesi | **KARŞILANDI** | §12 tablosunda Vu,d = 188,95 > ≈ 77 kN. |

## 2. Odak karar: sehim (ℓ/480 → 1,038; ℓ/240 → 0,519)

### 2.1 Şartname okuması
- ACI 318-14 Tablo 24.2.2: ℓ/480 "hasar görebilecek yapısal olmayan elemanları taşıyan/bunlara bağlı" çatı/döşeme, ℓ/240 "hasar görmesi olası olmayan" elemanlar için; her ikisi de "bağlantı sonrası sehim" (sürekli yükün uzun süreli kısmı + ek hareketli yükün ani kısmı) üzerinden. ℓ/360 (yalnız L, döşeme) F0'da 0,33.
- Çalışmanın boyut kısıtı g5 Tablo 9.3.1.1'e dayanıyor. Bu tablo **yalnız** "hasar görebilecek bölmeleri taşımayan veya bunlara bağlı olmayan" elemanlar için geçerlidir ve sağlandığında ACI hesaplı sehim **istemez** (9.3.2). Yani modelin kendi varsayımı içinde ℓ/240 de dahil hiçbir hesaplı sehim kontrolü zorunlu değildir; ℓ/240 hesabı ek, muhafazakâr bir rapordur.
- ℓ/480'i bağlayıcı yapmak, g5'in dayandığı öncülü reddetmek demektir. Bu durumda Tablo 9.3.1.1 kullanılamaz, hesaplı sehim **bütün** kirişlerde ve bütün desenlerde kısıt olur ve bu model (Branson Ie, λ = 2 lifsiz beton varsayımı, lif çekme rijitliği yok) bağlayıcı bir kısıt için savunulması gereken bir servis modeli haline gelir.

### 2.2 Seçeneklerin değerlendirmesi
- **(A) ℓ/480 kısıt + yeniden koşu:** Teknik olarak ucuz, ama bilimsel olarak en zayıfı. (i) Problem tanımını sonuç görüldükten sonra üçüncü kez değiştirir (post-hoc PCR-26) ve bu kez düzeltme bir kusuru gidermiyor, *sonuca bağlı bir rejim seçimi* yapıyor; hakem bunu "sonucu kurtarmak/öldürmek için kriter seçimi" olarak okur. (ii) Aşım %3,8, hesap modeli belirsizliğinin (Ie, λ, Ec, sünme) çok altında; bu büyüklükte bir eşikle tasarımı yeniden şekillendirmek modelin taşıyamayacağı bir hassasiyet varsayar. (iii) Lif çekme rijitliği yok sayıldığından kısıt lif aleyhine yanlıdır; lifli/lifsiz karşılaştırmasını sistematik olarak bozar. (iv) Çatı orta açıklığı (E20) belirleyici: çatıda 20 kN/m L zaten kaynak kabulü; hasar görebilir bölme varsayımının çatı için ayrıca gerekçelendirilmesi gerekir.
- **(B) Yalnız rapor + şerh:** Dürüst ama eksik: hangi kriterin *tasarım kriteri* olduğunu söylemezse okur F0'ı "servis şartını sağlamayan tasarım" diye okur ve 0 TL/kg cümlesi askıda kalır.
- **(C) ℓ/240 (ACI 24.2.2) kullan:** g5 / Tablo 9.3.1.1 öncülüyle tek tutarlı seçenek; üç tasarım da 0,35–0,52 ile rahat uygun.

### 2.3 Öneri: **C, B'nin şerhiyle birlikte (C+B)**. A beklenmez.
Makalede şu üçü birlikte yazılmalı:
1. **Tasarım öncülü (yöntem):** "Beams are assumed not to support or be attached to non-structural elements likely to be damaged by large deflections; the minimum-depth limits of ACI 318-14 Table 9.3.1.1 are therefore used as a constraint and the corresponding long-term limit is ℓ/240 (Table 24.2.2)."
2. **Rapor (bulgular/ek):** P, F0, F0Z için ℓ/240, ℓ/360, ℓ/480 oranları (desenli L, en kötü desen ve yer). Hesabın lif rijitlik katkısı içermediği ve λ = 2'nin lifsiz beton varsayımı olduğu.
3. **Şerh (açık, sayıyla):** "If the beams supported brittle partitions (ℓ/480), the cheapest zero-fibre-price design would exceed the limit by about 4 % at the roof mid-span under patterned live load; the same sections without fibres exceed it as well (1.004), so the exceedance follows from the shallower beams, not from the fibres. The zero-fibre-price saving is therefore conditional on the partition assumption."

Gerekçe: F0Z'nin de aşması kritik bilgi. 0 TL/kg tasarrufunun kanalı kiriş derinliğini azaltmak; sehim marjını tüketen de aynı kanal (ℓ/480 oranı P 0,70 → F0 1,04). Bu, makalenin "lif değeri hangi kanaldan gelir" katkısını güçlendiren, saklanmaması gereken bir mühendislik bulgusudur: **lifin eğilme kredisiyle sağlanan derinlik azaltımı, servis sınırının bağlayıcı olduğu rejimde kısmen ya da tamamen geri alınabilir.** Bu cümle yazılabilir; nicel bir "ℓ/480 rejiminde tasarruf X" yazılamaz (yeniden optimizasyon yok).

Yazarlar ℓ/480'in bağlayıcı olduğu bir senaryoyu (ör. dolgu duvarlı konut) savunmak isterse (A) ana seri değil, **etiketli ek duyarlılık** olarak (fp0, BTO+ATO, 10 koşu yeterli) koşulabilir; makalenin kabulü için gerekli görmüyorum.

## 3. Karma format beyanı (R5)

Tablo ve beyan cümlesi doğru ve yeterli düzeyde; kodla birebir eşleşme ve "eğilmede lif iki kez çarpanlı, kesmede MC2010-yerelden daha az muhafazakâr" ifadesi tam istenen dürüstlükte. F0'da etriyelerin tüm formatlarda smax'ta olduğu ve kesme formatının maliyeti değiştirmediği gösterimi iyi. Gönderimden önce kapatılmalı:
1. **R26.4.1.5 alıntısı:** kanıt dosyasında A-satırı yok (DOĞRULANAMADI). Makaledeki en kritik konumlandırma cümlesi ("ACI lifi yalnız kesmede tanır") buna dayanıyor; birebir alıntı + sayfa zorunlu.
2. **lcs alt sınırı 0,2h:** kaynağı bulunamadı ve kodda gerekçe yok. Ya kaynak (MC2010 veya RILEM) bulunmalı ya da "numerical lower bound introduced by the authors" diye beyan edilip F0'da etkin olup olmadığı (sabit noktada h − c > 0,2h mı) bir cümleyle gösterilmeli. Etkin değilse sorun kapanır.
3. **γF duyarlılığı:** önerilir, zorunlu değil. F0 için γF = 1,3 ile tek `--evalonly` (yeniden optimizasyon değil) yeterli; fR × 1,2 satırı zaten benzer yönde bir üst sınır veriyor.

## 4. Başabaş aralığı ve fiyat taraması (R6)

- **8,1–15,7 TL/kg (fR × 0,8–1,2) alt sınır olarak yeterli** ve doğru tanımlanmış. "Lower bound of the design-level break-even price" dışında bir adla anılmamalı.
- **Fiyat taraması gerekli değil**, çünkü 90 TL/kg sonucu yalnız alt sınırla değil kapalı bir üst sınırla da kilitlenebilir. Her uygun lifli tasarımın lif dışı maliyeti, 0 TL/kg'da bulunan en düşük maliyetten (233 916,61 TL) küçük olamaz (bu değer kanıtlanmış minimum değil, BTO+ATO 36/60 ve iki-koordinat yerel en iyi); dolayısıyla herhangi bir lifli tasarımın P'ye göre kazancı ≤ 2 539,61 TL'dir. En küçük sıfırdan farklı lif kütlesi m_min (tek grupta en düşük doz, en sığ kesitte; yazarlar ızgaradan hesaplamalı, ≈ 40 kg mertebesi) ise sistem düzeyi başabaş ≤ 2 539,61 / m_min ≈ 60 TL/kg mertebesindedir; bu 90 TL/kg'ın altındadır. **Öneri:** bu üst sınır m_min'in gerçek değeriyle hesaplanıp aralık "[8,1–15,7] ≤ b_sistem ≤ b_üst" biçiminde verilsin (fR × 1,2 için 0 TL/kg en düşük maliyeti de o varyantla alınmalı; yeniden optimizasyon gerekiyorsa yalnız alt sınır kalır ve bu cümle "fR × 1,0 için" sınırlanır). Bu, IA'nın tarama yapmama kararını bilimsel olarak savunulur kılar; hakem olarak taramayı ayrıca istemem.
- Ana metinde "ÇŞİDB 90 TL/kg alt sınırın 5,7–11 katı" ifadesi yazılabilir; "başabaş X TL/kg'dır" tek sayı yazılamaz.

## 5. "Optimum" dili

Mevcut yasak ve önerilen İngilizce cümle (§4 analiz belgesi) yerinde. Kanıt katmanları şimdi: iki bağımsız yöntem aynı vektör (lifsiz BTO 15/30, ATO 20/30; 0 TL/kg BTO 9/30, ATO 27/30) + tek ve iki-koordinat komşulukta daha ucuz uygun tasarım yok + uzay 7,9×10¹³ / 1,4×10¹⁷. Kullanılabilir terim: "cheapest feasible design found", "two-coordinate local minimum". "Optimum", "global", "near-optimal" kullanılmamalı. YAKO'nun farklı ve %0,04–0,76 pahalı tasarımı "çapraz teyit sınırı" olarak verilmeli, yöntem sıralaması olarak değil. Ö2'de yakınsamanın problem küçüklüğüne bağlandığı cümle korunmalı.

## 6. Makaleye girecek ana mesaj (öneri)

1. **Birincil:** Bu rejimde (gravite, 2B, lif eğilme ve kesmede tam MC2010 kredisi alan elverişli sınır durum), ÇŞİDB 2026 fiyatında BTO ve ATO hiçbir koşuda kirişlere lif seçmedi; lifin diğer kalemlerde yarattığı tasarruf, lif bedelinin küçük bir kesridir (kesit sabit en çok %12,6; tasarım düzeyi başabaşın alt sınırı fR'ye bağlı 8–16 TL/kg).
2. **İkincil (mekanizma):** Lif değeri etriyeden değil kiriş derinliği azaltımından (beton + kalıp) gelir; en ucuz lifsiz tasarıma göre boyuna donatı ve etriye artar. Aynı derinlik azaltımı servis marjını tüketir (ℓ/480 senaryosunda F0 ve F0Z aşar).
3. **Duyarlılık:** Lif bedeli sıfır kabul edildiğinde en ucuz lifli tasarım en ucuz lifsizden ≈ %1,1 ucuz; fR × 0,8'de %0,8; ℓ/240 öncülüne bağlı.
4. Ö1 ile birlikte: yalnız "uyumlu" (iki rejimde de mevcut fiyatta lif seçilmedi), nedensel açıklama yok.

## 7. Yapılamaz iddialar (güncel)

1. "Optimum", "global/near-optimal", "kanıtlanmış en iyi".
2. "Tasarımlar ACI 318-14'e uygundur"; yalnız "ACI 318-14 with fib MC2010 fibre contributions (declared deviation)" + tanımlı kısıt kümesi + ℓ/240 öncülü.
3. "Lif maliyeti azaltır" bağlamsız; 0 TL/kg tasarrufu ℓ/480 rejimi için; "lif servis/sehim performansını iyileştirir".
4. "Başabaş fiyat X TL/kg'dır" tek sayı; "sistem düzeyi başabaş" (yalnız alt/üst sınır).
5. "Lif ekonomik değildir/pahalıdır" genel; "lifli medyan yüksek → lif pahalı"; YAKO 90 TL/kg medyanını lif etkisi olarak okumak.
6. "Lif etriyenin yerini alır"; "lif etriyeyi azaltır" genel (üç referanstan birinde ters yön).
7. Lifli ↔ lifsiz "istatistiksel olarak anlamlı fark" (uzay kapsama; fp0 ↔ fp90 farklı amaç).
8. Algoritma üstünlüğü/sıralaması.
9. "ETABS'te doğrulandı" (V3 öncesi; lifli eğilme/kesme ETABS'te hiç doğrulanamaz, yalnız bağımsız hesapla).
10. Ö1 ile nedensel karşılaştırma; Duysak vd. ile maliyet/tasarım karşılaştırması.
11. "ℓ/480 rejiminde tasarruf %X" (yeniden optimizasyon yok).

## 8. Küçük noktalar

- F0'ın S3 katı Q = 0,0498 (eşik 0,05): sınıflama sınırda; V3'te ETABS ile birlikte bakılmalı ve ötelenmeli kabulde F0 maliyetinin değişip değişmediği bir satırla verilmeli.
- Ön kayıt şerhi (fiyat = 0 sağlaması önceden görülmüştü) makale/ek materyalde önerilen İngilizce cümleyle yer almalı; iyi uygulama.
- Kampanya 1 → 2 maliyet artışı (+%2,5–2,8) ek materyalde "superseded" etiketiyle; ana metinde yalnız kampanya 2.

## 9. Madde madde istek listesi

| # | Önem | İstek |
|---|---|---|
| K1 | Zorunlu | Sehim: ℓ/240 tasarım öncülü (C) + ℓ/480 şerhi sayıyla, F0Z dahil (B); A ana seri olarak yapılmasın. |
| K2 | Zorunlu | R26.4.1.5 birebir alıntı + sayfa (kanıt A-satırı). |
| K3 | Zorunlu | lcs ≥ 0,2h: kaynak ya da yazar sınırı beyanı + F0'da etkin olup olmadığı. |
| K4 | Güçlü öneri | Başabaşa analitik üst sınır (2 539,61 / m_min); fiyat taraması gereksiz. |
| K5 | Öneri | R8: düzlem dışı k = 1,2 ile P ve F0 `--evalonly`; Q'nun gravitede yöntem tanımı. |
| K6 | Öneri | γF = 1,3 ile F0 `--evalonly`. |
| K7 | Bilgi | Testler ek materyalde (ön kayıt kural 8 ile çatışmaz). |
