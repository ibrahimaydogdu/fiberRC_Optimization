# Dış hakem raporu: Örnek 2 (Ö2) kampanya sonuçları

**Dergi:** *Structures*. **Rol:** bağımsız dış hakem (RC çerçeve optimizasyonu, ACI 318, SFRC). İç Critic raporları okunmadı.
**Tarih:** 2026-11-16. **Okunan:** `04_Results/ex2_stats.md`, `04_Results/ex2_analysis.md`, `03_Code_Execution/EX2_SPEC.md` (§5, §11, §11.1, §12, §13), `EX2_CODE_LOG.md` §5e–5f, `EX2_VALIDATION_REPORT.md` §7, `ex2_work/Ex2Model/GravityDesign.vb` (1–100), `program_work/RcCore/SfrcBeam.vb` (ilgili satırlar), `04_Results/ex2/analysis/F0_evalonly.txt`, `F0_patterns.txt`, `F0_deflection.txt`. Kampanya klasörü yalnız okundu; hiçbir koşu, ikili veya ETABS dosyasına dokunulmadı.

## KARAR: BÜYÜK REVİZYON

Gerekçe tek cümle: kampanyanın raporladığı tüm en iyi tasarımlar, ACI 318-14 6.4.2'nin **zorunlu** hareketli yük düzenlemesini sağlamadığı için kiriş eğilmesinde %14–23 yetersizdir (F0'da ayrıca A1 ve BULGU-2 kaynaklı yetersizlikler eklenir); lif ekonomisine ilişkin ana sonuç (90 TL/kg'da seçilmiyor) büyük olasılıkla bundan etkilenmez, ancak 0 TL/kg tasarrufu (%0,87) ve başabaş (8,39 TL/kg) bu kusurların büyüklüğünden küçüktür ve yeniden koşu olmadan yayımlanamaz. Gereken iş ucuzdur (koşu ≈ 1 dk, 270 koşu saatler mertebesinde).

---

## 1. Bulguların güvenilirliği ve makaleye katkısı

**1.1 Sağlam olan (yeniden koşudan sonra da büyük olasılıkla korunur).**
- 90 TL/kg'da lifin seçilmemesi: BTO/ATO 60/60 koşuda doz 0, 0 TL/kg sağlamasında 90/90 koşuda lif seçiliyor, grup grup sıfırlama her grupta maliyeti artırıyor (`ex2_analysis.md` §5). Arama yanlılığı kontrolü ön kayda uygun ve ikna edici. Lifin kazandırdığı diğer kalem tasarrufu ile lif bedeli arasında **yaklaşık bir büyüklük mertebesi** fark var (8,39 vs 90 TL/kg; fR × 1,2'de bile 11,30). Bu oran, aşağıdaki kusurların (birkaç yüzde) çok üstünde; bu nedenle "bu rejimde, 90 TL/kg'da lif seçilmedi" bulgusu dayanıklıdır.
- Tasarruf kanalının ayrıştırılması (F0 ↔ P ↔ F0Z kalem ayrımı, kesit sabit marjinal tablo) makalenin en değerli kısmıdır: lifin değeri etriyeden değil, **kiriş derinliği küçültme (beton + kalıp)** kanalından geliyor; aynı kesitte etriye kazancı Av,min ve smax nedeniyle sınırlı. Bu, literatürdeki "lif etriyenin yerini alır" sezgisine karşı somut, yeniden üretilebilir bir sayısal kanıttır.

**1.2 Kırılgan olan.**
- **0 TL/kg tasarrufu %0,87 (2 014 TL)**; bu büyüklük, desenli yükle yeniden tasarım maliyetinden (%2,6), BULGU-2 kiriş eksenel çekme açığından (alt donatıda %2,7–3,4) ve A1'den (§3) küçük ya da aynı mertebededir. Desenli zarfla aynı kesitlerde yeniden türetmede fark korunuyor (P 236 316 vs F0 234 144 TL, `ex2_analysis.md` §9), bu iyi bir işarettir; ancak bu bir yeniden **optimizasyon** değildir ve F0'ın desenli yükte sehim sınırını aşma riski var (§4.3).
- **Başabaş 8,39 TL/kg tek bir tasarıma (F0) bağlı ve bir alt sınırdır.** Tanım (P − F0 lif dışı)/240 kg, 0 TL/kg optimumunu kullanıyor; bu tasarım lif dışı maliyeti en aza indirir ama "kg başına tasarruf"u en büyüklemez. Daha düşük dozlu bir tasarım kg başına daha fazla tasarruf sağlayabilir (F0 kesitlerinde grup bazında 16,2–17,4 TL/kg gözlenmiş). Gerçek başabaş = max(tasarım) [P − lif dışı maliyet]/kg ≥ 8,39. Makalede ya "≥ 8,4 TL/kg (F0 tasarımı için)" yazılmalı ya da kısa bir fiyat taraması (ör. 2, 5, 10, 15, 20, 30 TL/kg; BTO+ATO, 10 koşu) ile lifin seçilmeyi bıraktığı fiyat aralığı doğrudan bulunmalı. Tarama ucuzdur ve makalenin tek nicel lif ekonomisi sonucunu sağlamlaştırır.
- **fR'ye duyarlılık yüksek:** fR × 0,8 → 5,29; × 0,6 → 2,68 TL/kg. fR verisinin gizli tedarikçi kaynaklı ve karakteristik/ortalama durumunun belirsiz olması (Q18) bu sayının raporlanma biçimini belirler: tek sayı değil aralık ("2,7–11,3 TL/kg; ana değer 8,4") verilmeli.

**1.3 İstatistik bölümü.** Mann–Whitney/Holm burada bilgi taşımıyor: ATO hücrelerinin varyansı 0, BTO neredeyse 0; p = 5,5e-13 iki sabit dağılımın farklı olduğunu söylüyor. Daha önemlisi **fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırıyor** (maliyete lif fiyatı giriyor); fark totolojiktir. Öneri: Ö2'de çıkarımsal testler ek materyale; ana metinde betimsel tablo + deterministik yeniden değerlendirme kanıt olarak sunulsun. fp0 ↔ fp90 testleri hiç raporlanmasın (ön kayıtta olduğu için ek materyalde "anlamsız: farklı amaç" notuyla kalabilir).

## 2. "Optimum" mu, çapraz teyit mi?

Yasak iddia doğru konmuş; "optimum" denemez. Kanıt üç katmanlıdır ve bu sırayla yazılmalıdır:
1. İki bağımsız yöntem (BTO 16/30, ATO 30/30) farklı tohumlardan **aynı tasarım vektörüne** (Hamming 0) ulaştı; üçüncü yöntem (YAKO) %0,02–0,21 içinde, bir kolon grubunda farklı.
2. Bulunan tasarım tek-koordinat komşuluğunda (90/123 komşu) yerel en iyidir.
3. Arama uzayı küçüktür (18/24 ayrık değişken).

Önerilen ifade: "The cheapest feasible design found in this study was reached independently by two algorithms in 46 of 60 runs and no cheaper feasible design exists in its one-step neighbourhood; this is strong cross-confirmation but not a proof of global optimality." Güçlendirmek için iki ucuz ek (öneri, zorunlu değil): (a) uzay kardinalitesini (her değişkenin seçenek sayısı çarpımı) raporlayın; (b) iki-koordinat komşuluk taraması (C(18,2)·k² mertebesinde, birkaç yüz–birkaç bin değerlendirme, dakikalar). İki-adım yerel en iyilik, "küçük uzayda güçlü çapraz teyit" iddiasını belirgin biçimde sağlamlaştırır. Ö1'de yöntemlerin aynı vektöre inmediği hatırlanarak, yakınsama "algoritma kalitesi" değil **problem küçüklüğü** ile açıklanmalı; yöntem sıralaması yapılmamalı (kural 8).

## 3. A1: lifli eğilmede `Required` ↔ `PhiMn` tutarsızlığı

**Kod okuması (`GravityDesign.vb`).**
- `Required` (satır 28–42): ikiye bölmede her adımda `SfrcBeam.Flexure(..., lcs)` bir **önceki** orta noktanın tarafsız ekseninden türetilen `lcs` ile çağrılıyor (satır 37), sonra `lcs` bu adımın c'siyle güncelleniyor (satır 38). Kabul kararı (satır 39) böylece başka bir As'ye ait `lcs` ile veriliyor; döngü sonunda dönen `hi`, kendi tutarlı `lcs` sabit noktasında değerlendirilmemiş son kabul edilen orta noktadır. Ayrıca ilk uygulanabilirlik denetimi (satır 33) `lcs = h` ile yapılıyor.
- `PhiMn` (satır 92–100): verilen As'de `lcs = max(h − c, 0,2h)` için 20 adımlık sabit nokta yinelemesi; doğru (tutarlı) tanım budur.
- `SfrcBeam`: wu = min(lcs·εFu, 2,5 mm), çekme bölgesinde w(y) = lcs·εcu·y/c, w > wu bölgesi katkı vermez. Kapasite `lcs`'ye doğrudan bağlı; `Required` içindeki yanlış `lcs` kapasiteyi bir yönde ya da diğerinde kaydırabilir, F0 B_S1'de güvensiz yönde kaydırmış.
- **Ek tutarsızlık (raporlarda yok):** `EpsT` (satır 44–47) lifli kesitte **sabit `lcs = 0,7h`** kullanıyor. Yani aynı kesit için üç ayrı `lcs` tanımı var (tasarım: yol bağımlı; D/C: sabit nokta; εt ≥ 0,004 kısıtı g3: 0,7h). g3 bir **kısıttır**; yanlış lcs ile değerlendirilmesi uygunluk kararını etkileyebilir.

**Kök neden.** Sabit nokta yinelemesi ile ikiye bölme iç içe değil, aynı döngüde birleştirilmiş; `lcs` güncellemesi orta nokta değerine "gecikmeli" bağlı. Sonuç yol bağımlıdır (aynı Mu, farklı başlangıç aralığında farklı As verebilir); F0'da yalnız B_S1'in etkilenmesi (B_S2/B_S3 = 1,0000) bunun belirtisidir.

**Etki.** (i) F0 B_S1 üst donatısı ≈ %1 eksik: tahmini maliyet etkisi onlarca TL (B_S1 boyuna donatı kaleminin ≈ %1'i); 2 014 TL tasarrufu ve "90 TL/kg'da seçilmiyor" sonucunu değiştirmez. Başabaşı en çok ≈ 0,2–0,3 TL/kg aşağı çeker. (ii) Ama `DcFlex` kısıt değil, `g3` yalnız "As sonlu" denetliyor; dolayısıyla **optimizasyon bu hatayı göremez** ve 270 koşunun lifli tasarımlarında (fp0'da 90/90, fp90 YAKO 5/30) yetersizliğin dağılımı bilinmiyor; F0 dışındaki tasarımlarda daha büyük olabilir. (iii) Hata yalnız lifli kesitlerde var → lif lehine sistematik yanlılık (küçük de olsa yön tek tarafa). (iv) ρl = AsTop/(bw d) kesme lif artımına giriyor; eksik As, ΔVF'yi de çok az düşürür (ters yönde, ihmal edilebilir).

**Düzeltme.** `Required` içinde kabul kararını tutarlı kapasiteyle verin: `If PhiMn(b, h, d, mid, fc, fib) >= Mu Then hi = mid Else lo = mid` (PhiMn zaten sabit noktayı çözüyor; maliyet ≈ 20× Flexure çağrısı, kiriş başına önemsiz). Satır 33'teki ön denetim de `PhiMn(hi)` olmalı. `EpsT` lifli dalı aynı sabit nokta `lcs` ile hesaplanmalı. Sabit nokta yakınsaması (20 adım) için bir artık denetimi (|Δlcs| < 0,1 mm) ve yakınsamazsa uyarı eklenmeli; c(lcs) monoton olmayabilir, salınım durumunda iki değerin muhafazakâr olanı alınmalı. Birim test: rasgele (b, h, fib, Mu) için `Mu / PhiMn(Required(Mu)) ≤ 1 + 1e-6`.

**Yeniden koşu ihtiyacı.** DLL değişeceği için ön kayıtlı ikilinin dışına çıkılıyor. A1 tek başına küçük olduğundan "yalnız yeniden değerlendirme" savunulabilir; ancak §4'teki desenli yük ve BULGU-2 düzeltmeleri zaten yeniden koşu gerektiriyor. Öneri: A1 + desenli yük kısıtı + BULGU-2 (eksenel çekme) tek bir PCR'de birleştirilsin, yeni ön kayıtla 270 koşu (veya en azından plain + fp0 + fp90 BTO/ATO) yeniden koşulsun; mevcut kampanya "superseded" etiketiyle saklansın. Yeniden koşmadan önce, mevcut 270 en iyi tasarım düzeltilmiş DLL ile `--evalonly` yapılıp T > 0 olanların sayısı raporlanmalı (A1'in kampanya genelindeki büyüklüğü).

**Makalede.** "FEM ↔ ETABS doğrulaması lifli eğilmeyi kapsamaz (ETABS lif hesaplamaz); lifli eğilme kapasitesi bağımsız hesapla (Excel, S1) kontrol edildi" cümlesi ancak bu kontrol yapıldıktan sonra yazılabilir.

## 4. Desenli yük, 6.2.6, sehim ve lif rijitliği

**4.1 Desenli yük (ana kusur).** ACI 318-14 6.4.2 sürekli kirişler için hareketli yükün (a) açıklıkta en büyük pozitif moment için ilgili ve alternatif açıklıklarda, (b) mesnette en büyük negatif moment için komşu açıklıklarda düzenlenmesini öngörür; bu bir seçenek değil, tasarım yükü tanımıdır. Ö2'de L/D = 20/30 = 0,67 (yüksek), dolayısıyla etki büyüktür: tasarlanmış donatıyla eğilme D/C 1,14–1,23 (sehimde pozitif moment +13–21%). Bu, "sınırlama" olarak yazılamaz; raporlanan her tasarım ACI'ye uygun değildir. Kaynak makalenin (Duysak vd. 2023) desen kullanmaması bunu mazur göstermez; çalışma kendi kodunu ACI 318-14 olarak beyan ediyor. **Zorunlu:** 26 kombinasyonlu desenli zarf (`--patterns` altyapısı zaten var) kısıt olarak açılmalı ve kampanya yeniden koşulmalı. Eklenen FE maliyeti küçük (kombinasyon sayısı artar, rijitlik matrisi aynı).

**4.2 6.2.6 oranı 1,376 (sınıra %1,7).** Aktif kısıt olması kendiliğinden sorun değil (optimum sınırda durur), ve ETABS ile oran eşleşmesi 12/12 iyi. Ancak belirleyici olan **düzlem dışı** oran (C_Inter_S1 dışı 1,3757; desenli 1,3800) ve düzlem dışı denetim tamamen varsayıma dayalı (tutulu, k = 1, yalnız e_min). Bu, 2B modelin sınırıdır: optimum kolon (300×400 ↔ 350×350) ve dolayısıyla F0 ↔ P kolon maliyet farkı (+1 238 TL, F0 tasarrufunun %61'i büyüklüğünde) kısmen bu varsayımdan geliyor. Öneri: (a) düzlem dışı varsayımı makalede açık yazılsın; (b) duyarlılık olarak düzlem dışı k = 1,2 veya lu = eksen boyu ile F0 ve P yeniden değerlendirilsin; F0 − P farkının işareti korunuyorsa yazılsın. Ayrıca simetrik gravitede kat Q'nun nasıl elde edildiği (Vus ve Δo hangi yatay kuvvetle) yöntem bölümünde açıkça tanımlanmalı; hakemler "gravitede Q > 0,05" sonucunu sorgulayacaktır.

**4.3 Sehim.** F0'da ℓ/480 oranı 0,936 (E20, 6 m), **tüm açıklıklar yüklü** L ile. Desenli L (alternatif açıklık) orta açıklık sehimini artırır; dL ≈ %25–35 artarsa E20 oranı > 1 olur (dAfter 11,70 → ≈ 12,7–13,0 mm > 12,50). Yani desenli yük kısıtı açıldığında sehim de **bağlayıcı** hale gelebilir ve F0'ın 500 mm kirişleri geçersizleşebilir; bu, 0 TL/kg tasarrufunu doğrudan tehdit eder. Ek tutarsızlık: g5 için kirişlerin "hasar görebilecek bölmeleri taşımadığı" varsayılıyor (Tablo 9.3.1.1 bu varsayıma dayanıyor), ama sehim raporunda ℓ/480 (hasar görebilecek eleman) ölçütü öne çıkarılıyor. Biri seçilmeli: ya ℓ/240 (varsayımla tutarlı; F0 0,47) ya da bölmeler hasar görebilir ve 9.3.1.1 kullanılamaz, sehim kısıt olmalı. Öneri: sehim desenli L ile hesaplanıp en az rapor olarak verilsin; ℓ/480 seçilirse kısıt yapılmalı.

**Lif rijitliği.** Hesapta lifin çatlak sonrası çekme rijitliği (tension stiffening) yok; bu lifli kiriş sehimini olduğundan büyük gösterir, yani lif aleyhine muhafazakârdır. Buna karşılık SFRC sünme/rötre katsayısı (λ = 2,0) lifsiz beton varsayımıdır. Makalede "lifin servis katkısı hesaba katılmadı (lif aleyhine muhafazakâr)" denebilir; "lif servis performansını iyileştirir" denemez (zaten yasak).

**4.4 Lif eğilme katkısı ACI dışında (raporlarda eksik).** §12 sınırlamalar listesi yalnız **kesmedeki** ACI sapmasını beyan ediyor. Oysa R26.4.1.5 ("the Code only addresses ... steel fibers in resisting shear") gereği lifin **eğilme** katkısı ACI'de hiç tanınmıyor, ve F0 tasarrufunun ana kanalı tam da budur (aynı kesitte boyuna −2 934 TL, etriye −1 048 TL). Eğilme formatı da kesmeden farklı: eğilmede lif gerilmesi γF = 1,5'e bölünüp üstüne ACI φ = 0,9 uygulanıyor (çift güvenlik), kesmede γc = 1,0 + φ. Bu karma format hem beyan edilmeli hem de tutarlı hale getirilmeli ya da gerekçelendirilmeli. Ayrıca MC2010 lcs = min(srm, y) iken kod y kullanıyor (srm göz ardı); bu SfrcBeam'in w ≤ wu kesme mantığında muhafazakâr yönde olabilir ama yazılmalı.

**4.5 ACI muafiyetiyle ilgili konum.** Doz ızgarası 20–40 kg/m³ < 60 kg/m³ (26.4.2.2(d)); ayrıca Vu,d = 183,7 kN > 0,17φ√fc′ bw d ≈ 77 kN olduğundan bu kirişler 60 kg/m³ ile bile Tablo 9.6.3.1 muafiyetine giremezdi. Bu, makaleye yazılması gereken güçlü bir cümledir: Ö2 kirişlerinde ACI'nin lif için tanıdığı tek kullanım (Av,min'in kaldırılması) zaten uygulanamaz; lifin tüm değeri ACI dışı (MC2010) kredilerden gelir.

## 5. Makalede konumlandırma ve Ö1 ile birlikte okuma

**Ö2'nin rolü.** Ö2, spec'in kendi tanımıyla "lif lehine sınır durum"dur (yalnız gravite, Bölüm 18 yok, kesmede tam MC2010 kredisi, eğilmede MC2010 kredisi). Bu elverişli rejimde bile lif 90 TL/kg'da seçilmiyor ve 0 TL/kg'da kazanç < %1. Bu, "sınır durum argümanı" olarak yazılmalı: "even in a gravity-only frame where fibres receive full MC2010 credit in flexure and shear, the cost-optimised design did not select fibres at the 2026 unit price; the fibre contribution was worth about one tenth of its price". Ö1 (sismik, 3B; tüm-boyut lifli koşuların 173/180'inde lif = 0) ile birlikte okunduğunda ana mesaj: **iki farklı rejimde de optimizasyon, mevcut birim fiyatta kirişlerde çelik lifi seçmedi; lifin diğer kalemlerde yarattığı değer fiyatının küçük bir kesridir.** Makalenin katkısı "lif ekonomik değildir" yargısı değil, **optimizasyona gömülü başabaş fiyat** ölçüsü ve lif değerinin hangi kanaldan (kesit küçültme, boyuna donatı, etriye) geldiğinin ayrıştırılmasıdır. Önerilen başlık/özet çerçevesi: "a design-level break-even fibre price".

**Yan yana koyma kuralı.** Ö1 ile nedensel karşılaştırma yasağı doğru. Ancak iki örnekte de başabaş hesabı aynı tanımla yapılmışsa (P − F lif dışı)/kg, iki sayı tek tabloda "rejim kaydıyla" verilebilir; "uyumlu" denir, neden gösterilmez.

**Yapılamaz iddialar (ek / teyit).**
1. "Optimum", "global en iyi" (yalnız "bulunan en ucuz uygun tasarım"; §2'deki ifade).
2. "Lif maliyeti %0,87 azaltır" bağlamsız; yalnız "lif bedeli sıfır kabul edildiğinde, bu tasarım için" ve desenli yük düzeltmesinden sonra.
3. "Başabaş fiyat 8,39 TL/kg'dır" tek sayı olarak (alt sınır ve fR'ye duyarlı; aralık verin).
4. "Lif ekonomik değildir / pahalıdır" genel; "lif etriyenin yerini alır"; "lif etriyeyi azaltır" genel.
5. "Tasarımlar ACI 318-14'e uygundur" (desenli yük düzeltilmeden; ayrıca lifin eğilme/kesme katkısı ACI dışı, yalnız "ACI 318-14 with fib MC2010 fibre contributions (declared deviation)").
6. "Lifli ve lifsiz senaryolar arasında istatistiksel olarak anlamlı fark" (uzay kapsama + fp0/fp90 farklı amaç).
7. Algoritma üstünlüğü; YAKO'nun 90 TL/kg'daki yüksek medyanının lif etkisi olarak okunması.
8. "ETABS'te doğrulandı" (V3 yok; lifli eğilme/kesme ETABS'te hiç doğrulanamaz).
9. Lifin servis/sehim üstünlüğü.
10. Duysak vd. ile maliyet ya da tasarım karşılaştırması.

## 6. Madde madde istek listesi

| # | Önem | İstek |
|---|---|---|
| R1 | Zorunlu | ACI 6.4.2 desenli yük zarfı kısıt olarak; kampanya yeniden (yeni ön kayıt, eski kampanya "superseded"). |
| R2 | Zorunlu | A1 düzeltmesi: `Required` kabulünü sabit nokta `PhiMn` ile; `EpsT` lifli dalında aynı `lcs`; birim test `Mu/PhiMn(Required(Mu)) ≤ 1`. Önce mevcut 270 tasarımın düzeltilmiş DLL ile `--evalonly` taraması (T > 0 sayısı). |
| R3 | Zorunlu | BULGU-2 (kiriş eksenel çekmesi, alt donatıda %2,7–3,4 güvensiz) ya kısıta alınsın ya da R1 ile aynı PCR'de düzeltilsin; "sınırlama" olarak bırakılırsa 0 TL/kg tasarrufundan büyük bir açık bırakılmış olur. |
| R4 | Zorunlu | Sehim desenli L ile; ℓ/480 ↔ "bölme taşımaz" varsayımı tutarsızlığı giderilsin (ya ℓ/240 rapor, ya ℓ/480 kısıt). |
| R5 | Zorunlu | Lifin eğilme katkısının ACI dışı olduğu ve eğilme (γF = 1,5 + φ) ↔ kesme (γc = 1 + φ) format farkı beyan edilsin; lcs = y seçimi gerekçelendirilsin. |
| R6 | Güçlü öneri | Başabaş için fiyat taraması (BTO+ATO, 10 koşu, 5–6 fiyat noktası) veya "≥ 8,4 TL/kg (F0 için)" ifadesi; fR aralığıyla birlikte (2,7–11,3). |
| R7 | Öneri | İki-koordinat komşuluk taraması ve uzay kardinalitesi; "güçlü çapraz teyit" ifadesi (§2). |
| R8 | Öneri | Düzlem dışı kolon varsayımı duyarlılığı (k, lu) ve gravitede Q hesabının yöntem tanımı. |
| R9 | Öneri | Çıkarımsal testler ek materyale; fp0 ↔ fp90 testi raporlanmasın. |
| R10 | Bilgi | ACI muafiyeti tartışması: Vu > 0,17φ√fc′ bw d olduğundan 60 kg/m³ ile de Av,min kalkmazdı (§4.5); makalede konumlandırma cümlesi olarak kullanın. |

**Yeniden değerlendirmede beklenti.** R1–R4 sonrası, 90 TL/kg sonucu ve "kesit küçültme kanalı" bulgusu büyük olasılıkla korunur; 0 TL/kg tasarrufunun işareti (F0 sehim ve desenli eğilme nedeniyle) belirsizdir ve makale bu belirsizliği taşıyabilecek biçimde (ana bulgu = başabaş mertebesi, ikincil = 0 TL/kg ayrıştırması) kurulmalıdır.
