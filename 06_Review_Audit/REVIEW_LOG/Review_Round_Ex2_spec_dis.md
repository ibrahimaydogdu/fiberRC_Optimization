# Dış hakem raporu: EX2_SPEC.md v1 + EX2_OPEN_QUESTIONS.md (Ö2, 2B 21 elemanlı, yalnız düşey yük)

Tarih: 2026-11-16 · Hakem: bağımsız dış hakem (*Structures*; RC çerçeve optimizasyonu, SFRC) · Okunan: `03_Code_Execution/EX2_SPEC.md`, `03_Code_Execution/EX2_OPEN_QUESTIONS.md` (iç yazışmalar ve iç Critic raporu okunmadı). Program kodu incelenmedi; koda ilişkin yorumlar spec metnine dayanır.

Değerlendirilen kullanıcı kararları: V2'de 5 grup, ana koşuda 9 grup; 1,2D+1,6L; öz ağırlık belirsizse ayrıca eklenir; tek kapı FEM↔ETABS, kaynak maliyet kıyası bilgi amaçlı; yalnız A kolu (rastgele başlangıç), 30 koşu, 10 000 FE, lifli ve lifsiz.

## KARAR: BÜYÜK REVİZYON (koşu öncesi; düzeltmeler belge ve protokol düzeyinde, büyük çoğunluğu ek koşu gerektirmez)

Örnek, makalenin amacına katkı sağlayabilir. Spec kaynak denetimi bakımından iyi (aritmetik çözümleme, "yapılamaz iddialar" listesi ve kaynak-replika ayrımı yerinde). Ancak lifin maliyet etkisini ölçecek deney tasarımı şu an üç açıdan eksik: (i) lifin hangi ACI/fib hükmüyle neyin yerine geçtiği tanımlı değil, (ii) gravite tasarımında belirleyici olan servis (sehim/çatlak) kısıtları yok, (iii) "lif seçilmedi" sonucunu yorumlatacak zorlanmış-lif (forced-fibre) karşılaştırması yok. Ö1'de 180 tüm-boyut lifli koşunun 173'ünde lif = 0 çıktığı biliniyor; Ö2 aynı tasarımla koşulursa büyük olasılıkla aynı yorumlanamaz sonucu üretir.

---

## Bulgular (şiddet sırasıyla)

### KRİTİK

**K1. Lifin minimum kesme donatısı ile ilişkisi tanımsız (ACI 318-14 9.6.3.1 / 26.4.1.5.1 ile fib MC2010 karışımı).**
Spec §5 satırı "ACI 22.5 Vc + Vs; 9.6.3.1 min Av; … lifli: VRd,F" yazıyor; lifli kirişte min Av'nin kalkıp kalkmadığı belli değil. ACI 318-14, kesme donatısı yerine lifi yalnız sınırlı koşullarda kabul eder (deforme çelik lif, en az ≈ 60 kg/m³ [100 lb/yd³] doz, ASTM C1609 ölçütleri, fc' ≤ 40 MPa, h ≤ 600 mm, Vu ≤ φ0,17√fc' bw d; madde numaraları ve sayılar Literature/Method tarafından ACI metninden birebir doğrulanmalı). Projenin doz ızgarası 20–40 kg/m³ bu alt sınırın altında. Dolayısıyla:
- min Av korunursa lif yalnız min Av'yi aşan etriye ihtiyacını azaltabilir; lifin ekonomik kaldıracı çok küçük kalır ve sonuç tasarım gereği "lif = 0" olur;
- min Av fib MC2010 hükmüyle (SFRC'de min donatının kaldırılma koşulu) kaldırılırsa bu, ACI'nin izin vermediği dozda ACI tasarımından sapmadır ve açıkça beyan edilmelidir.
Hangi yolun seçildiği, madde numarasıyla, spec'e ve makaleye yazılmalı. Bu karar lif sonucunu tek başına belirleyebilir.

**K2. Lifin "neyi ikame ettiği" ölçülmüyor: zorlanmış-lif (forced-fibre) senaryosu yok.**
Lifli tasarım uzayı lifsiz uzayı kapsar (doz 0 izinli). Bu yüzden lifli ile lifsiz arasındaki her fark ya lifin gerçek etkisi ya da büyüyen uzayın arama verimsizliğidir; Mann–Whitney sonucu bu ikisini ayıramaz (Ö1'deki "lifli %1–6 pahalı" bulgusu bu nedenle iddia edilemez olmuştu). Önerilen ek tasarım (hesap yükü düşük):
- (a) Her kiriş grubunda doz sabitlenerek (4D/5D × 20, 30, 40 kg/m³; tüm kirişler aynı) kesit ve donatı yeniden optimize edilir ya da en azından lifsiz en iyi çözümden başlanarak kesit/etriye yeniden boyutlandırılır;
- (b) Sonuç "lifin marjinal maliyeti" tablosu olarak verilir: beton/donatı/etriye/kalıp/lif kalemlerindeki değişim (TL ve %), hangi kısıtın gevşediği.
- (c) 0 TL/kg senaryosu (spec §8'de var, kullanıcı kararında açıkça yok) **zorunlu** tutulmalıdır; lifin teknik yararını fiyattan ayıran tek ölçü budur. 0 TL/kg'de bile lif seçilmiyorsa sebep fiyat değil, kısıt formülasyonudur (bkz. K1, B1).
Bu tablo olmadan Ö2 "lifin maliyet etkisi" sorusuna cevap vermez.

**K3. V2-a, kaynak Tablo 1 kesitleriyle yapılırsa büyük olasılıkla tasarım kapasitesi aşılır; D/C karşılaştırması anlamsızlaşır.**
Kaba kontrol (hakem hesabı, kanıt değil): 1,2·30 + 1,6·20 = 68 kN/m, 6 m açıklıkta wL²/12 ≈ 204 kN·m; 250×300 kirişte εt ≥ 0,004 sınırında φMn ≈ 95–100 kN·m. ETABS CFD bu kesitlerde "O/S" uyarısı verir ve donatı/D/C okunamaz. Önerim: V2-a'da kuvvet karşılaştırması (moment, kesme, eksenel, reaksiyon) Tablo 1 kesitleriyle; D/C ve tasarım donatısı karşılaştırması **uygun (feasible) bir tasarımla** (ör. lifsiz en iyi ya da FSD çözümü) yapılmalı. Aksi halde tek kapı (FEM↔ETABS) da işlevsiz kalır.

### BÜYÜK

**B1. Servis kısıtları (sehim, çatlak) yok; gravite rejiminde bu hem tasarımı hem lif sonucunu taraflı yapar.**
Yalnız düşey yük altında kiriş boyutunu çoğu zaman sehim belirler. Spec §5'te ACI 9.3.1.1 minimum yükseklik (veya 24.2 hesaplı sehim) ve 24.3 donatı aralığı/çatlak kontrolü yok. Sonuç: optimizer en sığ kesiti yoğun donatıyla seçer (εt ≥ 0,004 sınırına dayanır), uygulamada kabul edilemeyecek kiriş çıkabilir. Ayrıca SFRC'nin literatürde en sağlam yararı servis durumundadır (çatlak genişliği, rijitlik); servis kısıtı olmayan bir formülasyonda lifin tek kaldıracı kesmedir ve lif sistematik olarak küçümsenir. En az: ACI 9.3.1.1 minimum h kısıtı (veya sehim kontrolü, L/240–L/480 seçimi gerekçeli) eklenmeli; lif katkılı çatlak kontrolü yapılmıyorsa bu sınırlama olarak yazılmalı.

**B2. Güvenlik formatlarının karışması (ACI yük faktörleri + φ ile fib MC2010 γc/γF) lifin kesme katkısında belirleyici hale geliyor.**
Ö1'de lif katkısı sarılma bölgesinde sıfırlandığı için bu sorun ikincildi; Ö2'de lif kesme katkısı "tam kullanılacak" (spec §5), dolayısıyla VRd,F'nin ACI 1,2D+1,6L etkilerine hangi kısmi katsayılarla karşı konduğu, ACI Vc ile VRd,F'nin toplanıp toplanmadığı (MC2010 VRd,F betonu da içerir; Vc + VRd,F çift sayım olur) ve VRd,F üst sınırı açıkça yazılmalı. fR1/fR3'in karakteristik mi ortalama mı olduğu spec'te "DOĞRULANAMADI": VRd,F fFtuk (karakteristik) ister; ortalama değer kullanılırsa lif katkısı güvensiz yönde abartılır. Bu belirsizlik çözülmeden ya da muhafazakâr bir dönüşüm (ör. varyasyon katsayısıyla karakteristiğe indirgeme, kaynaklı) uygulanmadan lifli sonuç savunulamaz.

**B3. Ö1 ile karşılaştırılabilirlik (soru 2): doğrudan değil; ancak doğru çerçevelenirse değerli.**
Ö1 ile Ö2 arasında rejim (sismik Bölüm 18 / gravite), geometri (3B/2B), yük, grup yapısı, beton sınıfı serbestliği ve ön tasarım kolu aynı anda farklı. Lif sonucundaki fark rejime atfedilemez (karışık etkiler). Savunulabilir çerçeve şudur: Ö2, lifin kesme katkısının tam sayıldığı **lif için elverişli sınır durum**dur. Lif Ö2'de 90 TL/kg'de de seçilmiyorsa "bu fiyat ve bu lif verisiyle, lifin tam kredi aldığı gravite durumunda bile lif seçilmedi" denebilir; bu, Ö1 bulgusunu güçlendirir ama genelleştirmez. Lif Ö2'de seçiliyorsa "Ö1'de seçilmemesinin nedeni Bölüm 18 hükümleri (Vc = 0, sarılmada lif katkısı 0)" denemez; yalnız "uyumlu" denebilir. Rejim etkisini gerçekten ayırmak isteniyorsa tek kontrollü deney: Ö1 en iyi çözümlerinde kiriş kesme tasarımı gravite kuralıyla yeniden değerlendirilir (deterministik; yeniden optimizasyon değil). Bu yapılmazsa makalede "rejim farkı" nedensel dille yazılmamalı.

**B4. Kaynak maliyet modeli sorununa çözüm (soru 3): ilke doğru, V2-d gereksiz ve yanıltıcı olabilir.**
"Kaynak-replika fiyat seti + bilgi amaçlı" yaklaşımı yeterli **yalnız** V2-c (beton bedeli 1 936,08 TL ve toplam 11 486 TL'nin aritmetik yeniden üretimi) için. V2-d (bizim optimumumuzu replika fiyatlarla 11 486 TL'ye kıyaslamak) kaynak donatısı sabit ve büyük olasılıkla dayanımı sağlamayan bir çözümle yapılan bir kıyastır; hangi yönde çıkarsa çıksın okuyucu "X % daha ucuz/pahalı" diye okur. Öneri: V2-d ya kaldırılsın ya da yalnız ek materyalde, V2-b (kaynak kesitlerinin bizim modelde D/C ve gerekli donatısı) ile yan yana ve "kaynak çözümü 1,2D+1,6L altında uygun değildir/uygundur" bilgisiyle verilsin. PCR-19 (SourceReplica) yalnız bu iki tablo için gerekiyorsa, kod değiştirmek yerine Excel'de (formüllü, kural 2) hesap yeterlidir; regresyon riski taşıyan bir kod değişikliğine değmez.

**B5. Kaynak seçimi zayıf; "benchmark/validation" dili kullanılmamalı.**
Kaynak hakemli dergi değil, ulusal kongre bildirisi; DOI yok; donatı sabit; kombinasyon ve öz ağırlık yok; ACI 318-08. *Structures* hakemi "neden bu kaynak" diye sorar. Makalede "geometry and loading adapted from Duysak et al. (2023)" denmeli; "benchmark", "validated against", "compared with the reference optimum" ifadeleri kullanılmamalı. Mümkünse Literature Analyst hakemli dergilerdeki 2B gravite RC çerçeve kıyas örneklerinden birini (ör. Camp, Pezeshk ve Hansson 2003, J. Struct. Eng.; künye ve geometri doğrulanmalı) alternatif ya da ek kıyas olarak değerlendirmeli. Kullanıcı Duysak'ta kalırsa bu sınırlama açıkça yazılmalı.

**B6. Öz ağırlık kararı ile V1 kontrol büyüklüğü uyumsuz.**
Kullanıcı kararı "belirsizse ayrıca eklenir"; spec V1 reaksiyon ölçütü öz ağırlık hariç 3 264 kN. V1 ölçütü öz ağırlık dahil, kesite bağlı ifade olarak (Σ 1,2·γc·A·L eklenerek) yeniden yazılmalı. 30 kN/m'nin döşeme + kaplama + duvarı içerdiği ve kiriş öz ağırlığını içermediği varsayımı makalede açıkça yazılmalı. Kolon öz ağırlığının da eklendiği belirtilmeli.

### ORTA

**O1. Lifli/lifsiz istatistik karşılaştırmasının yorumu önceden yazılmalı.**
Lifli uzay ⊇ lifsiz uzay olduğundan, lifli medyanın lifsizden yüksek çıkması lifin pahalı olduğunu göstermez. Ön kayıt (protokol) şunu demeli: birincil lif çıktısı K2'deki zorlanmış-lif tablosu ve en iyi lifli çözümlerdeki doz dağılımıdır; Mann–Whitney ikincildir ve "arama verimliliği" ile "lif etkisi"ni ayıramadığı yazılır.

**O2. Küçük uzay, sonucun bağımsız teyidine izin veriyor; kullanılmalı.**
Lifsiz uzay 35⁹ (≈ 7,9×10¹³); gravite altında kiriş ve kolon grupları zayıf bağlıdır. En iyi çözüm etrafında grup grup tam tarama (koordinat bazlı, her grup 35 kesit × lif seçenekleri; yüzlerce FE) yerel en iyilik teyidi verir. Uygulamalı mühendislik makalesi için "üç algoritma aynı çözümde buluştu + komşulukta daha ucuz uygun çözüm yok" iddiası, "optimum" iddiası yapılmadan sunulabilecek en güçlü kanıttır. Düşük maliyetli; şiddetle önerilir.

**O3. ETABS V3 lifli kirişin kesme katkısını doğrulayamaz.**
Ö2'de lifin katkısı esas olarak kesmede; ETABS CFD lif bilmez. "Eşdeğer donatılı kesit" ifadesi belirsiz. Öneri: V3'te lifli kirişler için eğilme ve kolonlar ETABS ile, lifli kesme kapasitesi ise bağımsız formüllü Excel hesabıyla (VRd,F, girdiler ve kısmi katsayılar görünür) doğrulanır; makalede "fibre contribution not verified by ETABS; checked by independent calculation" diye yazılır.

**O4. Gravite rejiminin ETABS'teki karşılığı.**
"Ordinary moment frame" ACI 18.3'e (SDC B) atıf yapar; gravite-only örnek için doğru ifade "Bölüm 18 uygulanmaz (yanal kuvvet direnci istenmez)". ETABS ACI 318-14 CFD'de seçilecek çerçeve tipi (Sway Ordinary / Non-Sway) ve bunun kolon kesmesi ve narinlik hesabına etkisi V2-a'da belgelenmeli; FEM'deki δns/δs seçimiyle tutarlı olmalı. 2B çerçevenin düzlem dışında tutulu (braced) varsayımı ve düzlem dışı narinlik için kullanılan k ve lu açıkça yazılmalı (Q15 ile birlikte).

**O5. Kolon ρ ızgarası %1–%4 (Q7) kabul edilebilir, ama sınır beyanı gerekli.**
Gravite kolonlarında belirleyici çoğunlukla ρmin = %1 ve kesit alt sınırı olacak; kolon maliyeti hemen hemen sabit kalır. Bu, lifin yalnız kirişte olmasıyla birlikte toplam maliyetteki lif payını küçültür; makalede lif etkisi **kiriş maliyeti üzerinden de** (yalnız toplam yapı değil) raporlanmalı, aksi halde etki sulandırılmış görünür.

**O6. Doz ızgarası boşlukları ve 5D'nin dar aralığı.**
C30'da 5D yalnız 25–35 kg/m³. Onarım operatörünün dozu hangi yöne yuvarladığı (yukarı/aşağı/en yakın) belirtilmeli; maliyet yanlılığı yaratabilir. Ayrıca 20–40 kg/m³ aralığı ACI'nin lifle kesme donatısı ikamesi için öngördüğü dozun altında kalıyor (K1): makalede bu aralığın neden seçildiği (veri mevcudiyeti) yazılmalı.

**O7. Gizli üretici verisi.**
fR değerleri gizli bir tedarikçiden; Ö2'de lif katkısı tam kullanıldığı için sonuç bu veriye Ö1'den daha duyarlı. fR çarpanı 0,8/1,2 deterministik yeniden değerlendirmesi zorunlu; ek olarak, yayımlanmış bir veri setiyle (MEMORY'de anılan Venkateshwaran 2018 / Chen 2021 gibi) bir kontrol noktası verilmesi hakem itirazını azaltır.

### KÜÇÜK

**M1.** Koşu sayısı tutarsızlığı: kullanıcı kararı "lifli ve lifsiz" (2 senaryo × 3 yöntem × 30 = 180), spec §11 3 senaryo (270). Hangisi geçerli, protokolde tek satırla sabitlenmeli (K2(c) gereği 0 TL/kg önerilir).

**M2.** 1 000 FE "erken yakınsama" ikincil çıktısı makaleye giriyorsa ön kayıtta tanımlanmalı; girmiyorsa çıkarılmalı (iddia sayısı az olsun).

**M3.** §6 madde 4'teki "mekanik olarak tutarsız" (çatı kirişi h = 55 cm) yargısı makaleye taşınmamalı; çatı kirişinin kolon tutulması farklıdır, bu tek başına tutarsızlık kanıtı değildir. Spec'teki "kaynak hatalıdır hükmü verilmez" ilkesiyle uyumlu dil kullanılmalı.

**M4.** 5 grup (V2) / 9 grup (ana) kararı uygun. Kapı yalnız FEM↔ETABS olduğundan 5 grup yalnız bilgi tabloları için anlamlı; V2-d kaldırılırsa (B4) 5 gruplu koşuya gerek kalmayabilir.

**M5.** ACI 318-08 → 318-14 farkı gravite eğilme/kesmesinde küçüktür; sapma kaynağı olarak abartılmamalı. Asıl sapma kaynakları kombinasyon, öz ağırlık ve sabit donatıdır.

**M6.** Süre tahmini (§11) ölçülmeden makaleye girmemeli (spec zaten söylüyor); 100 FE ölçümü protokol onayından önce yapılmalı.

---

## Sorulara kısa cevaplar

1. **Katkı ve tasarım:** Katkı potansiyeli var (lifin kesmede tam kredi aldığı rejim). Mevcut tasarım bunu ölçmüyor: K1, K2, B1, B2 giderilmeli.
2. **Ö1 ile karşılaştırılabilirlik:** Doğrudan değil (karışık etkiler). "Lif için elverişli sınır durum" olarak savunulabilir; nedensel rejim iddiası yalnız B3'teki kontrollü yeniden değerlendirmeyle.
3. **Kaynak maliyet modeli çözümü:** V2-c için yeterli; V2-d kaldırılmalı veya ek materyale; kod değişikliği (PCR-19) yerine Excel önerilir.
4. **Yapılamaz iddialar (spec §12'ye ek):** "Lif kesme donatısının yerini alır" (ACI dozu karşılanmıyor); "lifli medyan yüksek, lif pahalıdır"; "Ö2 ile Ö1 farkı sismik detaylamadan kaynaklanır"; "kaynak örnekle doğrulandı / benchmark"; "lif servis performansını iyileştirir" (servis kısıtı yok); "ETABS lifli kirişi doğruladı". Makalede belirtilmesi gereken sınırlamalar: gravite-only ve düzlem dışı tutulu varsayım, servis kısıtı yok (eklenmezse), ACI/fib karma güvenlik formatı, fR verisinin gizli ve karakteristik olup olmadığının belirsizliği, lif dozunun ACI ikame alt sınırının altında olması, kaynak bildirinin sınırlılıkları, öz ağırlık varsayımı, lif katkısının ETABS ile doğrulanamaması.
5. **Diğer eksikler:** yukarıda O1–O7, M1–M6.

## Koşu öncesi asgari şartlar (kontrol listesi)
- [ ] K1: lifli kirişte min Av kuralı ve dayanak maddesi yazıldı.
- [ ] K2: zorlanmış-lif tablosu ve 0 TL/kg senaryosu protokolde.
- [ ] K3: V2-a D/C karşılaştırması uygun bir tasarımla.
- [ ] B1: en az ACI 9.3.1.1 min yükseklik veya sehim kısıtı; yoksa beyan.
- [ ] B2: VRd,F ile Vc toplama kuralı, kısmi katsayılar, fR karakteristik/ortalama kararı.
- [ ] B6: V1 reaksiyon ölçütü öz ağırlıkla güncellendi.
- [ ] M1: senaryo/koşu sayısı tek satırda sabitlendi.
