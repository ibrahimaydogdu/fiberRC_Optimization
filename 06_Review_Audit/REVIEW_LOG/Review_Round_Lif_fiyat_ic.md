# Review Round Lif fiyatı duyarlılığı (iç Critic) — 2026-11-16

Kapsam: tüm-boyut BTO/ATO ham CSV (`phase3/{bto_alldim,ato_alldim}`, 360 koşu: 180 lifsiz + 180 lifli), `BeamDesign.vb`, `SfrcBeam.vb`, `Ex1Problem.vb`, `Ex1Evaluator.vb`, `Cost.vb`, PCR-13. Salt-okunur; koşu başlatılmadı. Yalnız `phase3_bin` (FemRcModel.dll SHA c1a95951…f5a9) ile tek-iş-parçacıklı, toplam ≈ 70 FE'lik deterministik probe çalıştırıldı (araç: kullanıcı dosyası değil, scratchpad betiği `probe.ps1/probe2.ps1`; proje dışı, kayıt altına alınmadı).

## 1. Ana bulgu: lif=0 arama artefaktı değil, model yapısının sonucu

### 1.1 Aktif kısıtlar (ham CSV `violation_counts`, 180 lifsiz + 180 lifli koşu toplamı)
Kirişin dayanım kısıtları neredeyse hiç ihlal edilmiyor; ihlaller kolon/birleşim/stabilite kısıtlarında.

| Kısıt | Lifsiz ihlal sayısı | Lifli ihlal sayısı |
|---|---|---|
| ACI 6.6.4.3 stabilite Q ≤ 0,05 | 775 626 | 734 814 |
| ACI 18.8.2.3 kolon boyutu ≥ 20 db (kiriş donatısı) | 627 273 | 621 547 |
| C1a kolon etriye aralığı ≥ 50 | 337 064 | 310 380 |
| A3 kolon ρ ≤ %4 | 96 521 | 98 233 |
| A5 güçlü kolon | 72 626 | 74 170 |
| C1f birleşim kesmesi | 65 397 | 55 946 |
| B7 kiriş εt / ρ ≤ %2 | 2 / 2 | 10 / 9 |

Lif yalnız kiriş eğilme (As) ve orta bölge kesmesini etkiler. Sınırlayıcı kısıtlar (Q, kolon boyutu/db, güçlü kolon, birleşim) kiriş dayanımından bağımsız; lifin etkileyebileceği bir kısıt aktif değil. Bu, PCR-13 "sınırlayıcı kısıt dayanım değil" bulgusunun nedenidir ve veriyle doğrulandı.

### 1.2 Kod okumasından üç yapısal neden (`BeamDesign.vb`)
1. Sarılma bölgesi kesmesinde lif katkısı 0 (`vcConf = 0` olduğunda; ayrıca Vc=0 anahtarı), yani lif etriye azaltmıyor; kritik bölgede etriye aralığı d/4, 6db, 150 mm üst sınırı ve `AvMinPerS` belirliyor.
2. Orta bölgede lifli kiriş `VRdF` (fib MC2010 7.7-4, γc=1,5) ile, lifsiz kiriş ACI `Vc = 0,17 √fc b d` ile tasarlanıyor; iki farklı güvenlik formatı. Orta bölge zaten d/2-300 mm ve Av,min ile sınırlı.
3. Kapasite tasarımı (ACI 18.6.5): `MrOf` lif katkısını (γF=1) olasılıklı momente ekliyor; bu Ve ve dolayısıyla etriye, güçlü kolon ve birleşim talebini artırıyor.

### 1.3 Deterministik kanıt (probe; en iyi lifsiz tasarım BTO_plain_A run29, uygun, maliyet 1 386 277 TL, çelik 10 026 kg)
Aynı kesit vektörü, yalnız 4 kiriş grubunun lif tipi/dozu değiştirilerek `EvaluateFull` ile yeniden değerlendirildi (`--derive 1` modeli, fR ölçek 1,0):

| Durum | Maliyet (TL) | Çelik (kg) | Not |
|---|---|---|---|
| Lifsiz | 1 386 277 | 10 026 | referans, T=0 |
| **Lif ücreti 0 TL/kg**, 4D, 20 kg/m³ | 1 403 064 | 10 267 | +%1,2 |
| **0 TL/kg**, 5D, 35 kg/m³ (üst doz) | 1 401 773 | 10 249 | +%1,1; T=0 |
| 90 TL/kg, aynı | 1 474 551 – 1 527 179 | | lif maliyeti 71–125 bin TL |
| 0 TL/kg, 5D, fR ×3 | 1 507 317 | 11 772 | **fR artınca çelik artıyor** |
| 0 TL/kg, 5D, fR ×10 | 1 880 064 (T=2,2) | 17 137 | uygunsuz |

Başka bir lifsiz en iyi (run21, 1 391 279): lif ücreti 0'da +%1,1–1,5. Kesit boyutunda tek-adım yerel iyileştirme (49 FE, lif ücreti 0, 5D/35): 1 401 773 → 1 399 289, lifsiz optimumun yine +%0,9 üstünde.

**Okuma:** Bu modelde lif ücretsiz olsa bile aynı tasarım pahalanıyor; kırılma fiyatı ≤ 0 TL/kg. Tek tasarım ve yerel arama olduğundan kanıt "güçlü gösterge", kesin optimum değil; bunu kesinleştirmek için Bölüm 2 (b0) gerekir. Çelik artışı lifin etkisiz kalmasının ötesinde bir tutarsızlık (artık-sünek kapasite modeli talebi büyütüyor): fR büyüdükçe donatı artması beklenmeyen davranış. Muhtemel nedenler 1.2-3 (Mpr'ye lif ekleme, γF=1) ve 1.2-2 (iki farklı kesme formatı). Bu **modelleme varsayımı** olarak makalede beyan edilmeli; "lif doğası gereği yararsız" denemez.

## 2. Soru 1: asgari yeterli tasarım

| Seçenek | Ne yapar | Süre | İzin verdiği iddia | Değerlendirme |
|---|---|---|---|---|
| (a) deterministik kırılma fiyatı (30 en iyi lifsiz tasarım × tip × doz × fR ∈ {0,8; 1,0; 1,2}, fiyat 0, optimizasyon yok) | Lifin kesit değişmeden etkisi; kırılma fiyatı = Δ(lif dışı maliyet)/fibreKg | dakikalar (≈ 30×12×3 ≈ 1 100 FE, tek iş parçacığı ≈ 8 dk) | "Verilen tasarımlarda lif ücretsiz bile olsa maliyeti düşürmüyor (kırılma fiyatı ≤ 0)"; optimum iddiası yok | **Gerekli**, sıfır risk |
| (b0) fiyat = 0 ile 10 koşu BTO tüm-boyut A lifli | Arama lifin tam ücretsiz hâlinde kesiti yeniden şekillendirebilir mi | ≈ 1,6 saat (10 koşu, 8 çekirdek) | "Ücretsiz lifle bile optimum lifsiz medyandan ayrışmıyor" (veya ayrışıyorsa kırılma fiyatı sıfırdan büyük) | **Yeterli üst sınır deneyi**; bence tek zorunlu koşu |
| (b) fiyat taraması 90→45→22,5→9, BTO A | Fiyat–lif dozu eğrisi | 10 koşu/fiyat: 3 fiyat ≈ 4,8 saat; 30 koşu/fiyat ≈ 14 saat | "Lif x TL/kg altında kullanılır" | (b0) lif=0 kalırsa **gereksiz**; ayrıca fiyat taraması 90→9 TL/kg zaten (a)'nın bir alt kümesi |
| (c) fR taraması 0,8/1,0/1,2 | fR belirsizliği | koşu: 3×1,6 = 4,8 saat | fR duyarlılığı | (a)'ya içerilir (deterministik); koşu gereksiz |

Fiyat taraması ancak (b0) lifsizden ayrışırsa (≥ %1 ya da lif dozu medyanı > 0) anlamlıdır; o durumda kırılma fiyatı (a) ile deterministik, ardından (b) 90/45/22,5 için 10 koşuda doğrulanır.

## 3. Soru 2: lif=0 sonucu ve fiyat taramasının anlamı

- Kanıt, lif=0'ın **arama artefaktı değil** modelin sonucu olduğu yönünde: (i) kiriş dayanım kısıtları aktif değil (Bölüm 1.1), (ii) lif sarılma bölgesinde 0 katkı, orta bölgede minimum etriyeyle sınırlı, (iii) fiyat 0'da bile maliyet artıyor. Arama artefaktı itirazı (180 koşunun 173'ü lifsiz) bu probe ile büyük ölçüde kapanıyor; kalan belirsizlik (b0) ile kapatılır.
- Fiyat 90'dan 9'a inse de bu model, tasarımda lif seçmeyecektir; fiyat taraması (b) **bu modelde anlamsız**, çünkü fiyat bir engel değil. Kırılma fiyatı negatif/≤ 0 ifadesi tek satırlık (a) sonucuyla verilir.
- Çalışılan şey "lif fiyatı duyarlılığı" yerine "lifin yapısal kazancı sıfır" tanısı olmalı: asıl duyarlılık fiyat değil **kod/model varsayımlarıdır** (sarılma bölgesinde lif katkısı 0, kapasite tasarımında Mpr'de lif, VRd,F formatı, fR ölçeği). Bunlar PCR-13 "neden araştırılmalı" maddesini kapatır.

## 4. Makale için lif bulgusu nasıl yazılır (öneri)

Yazılabilir:
- "Bu çerçevede (4 katlı, perdesiz, ACI 318M-14 Bölüm 18 kapasite tasarımı zinciri, EN 14651 fR verisi tek üretici, lif yalnız kirişte, sarılma bölgesinde lif katkısı yok), kiriş dayanım kısıtları optimumda aktif değildir; sınırlayıcı kısıtlar kolon boyutu/kiriş donatı çapı (ACI 18.8.2.3), stabilite indeksi, güçlü kolon ve birleşim kontrolleridir; lif bu kısıtları gevşetmez."
- "Lif ücreti sıfır alınsa bile aynı kesit vektörü %1,1–1,5 pahalanır; lifin beton maliyeti dışındaki kazancı (donatı/etriye) bu modelde yoktur." (a ve probe sayıları; 30 tasarım üzerinden yeniden üretilmeli.)
- "360 tüm-boyut koşunun 180 lifli olanında 173'ünde son tasarımda lif yok" (Fpen-en-iyi x sınırlaması beyan edilir).

Yazılamaz:
- "Lif bu tür çerçevelerde ekonomik değildir/yararsızdır" (model varsayımları + tek fR kaynağı + tek yük kombinasyonu + tek geometri).
- "Lif fiyatı X TL/kg'ın altında ekonomik olur" (kanıt tersini söylüyor).
- "Optimum lifsizdir" (optimum iddiası yok).
- Üç lif varsayımının (Mpr'de γF=1, VRd,F γc, fR ölçeği) lifin lehine/aleyhine nicel etkisi, yalnız ek deneyle (lif etkisini Mpr'den çıkaran kontrol) iddia edilebilir.

## 5. Soru 3: asgari iş, süre, DLL

Yapılacak (hepsi `phase3_bin`, c1a95951…f5a9, tüm tüm-boyut hücrelerle aynı):
1. **(a) deterministik kırılma fiyatı tablosu**, resmî araçla: 30 BTO lifsiz A + 30 ATO lifsiz A en iyi tasarım (uygun) × {4D, 5D} × dozlar × fR {0,8; 1,0; 1,2} × fiyat 0. Araç gerekmez (probe betiğinin resmî sürümü). ≈ 30 dakika CPU.
2. **(b0) ücretsiz lif koşusu**: `Ex1Runner.exe --method BTO --fibre 1 --fibreprice 0 --derive 1 --runs 10 --fes 10000 --fr 1.0 --seed0 1000 --dop 8 --out <yeni dizin>` (A kolu bayrakları `run_phase3.sh` satır 45 ile aynı: yalnız `--derive 1`, ön tasarım yok; tohumlar 1001–1010; lifsiz A hücresiyle eşleşik). ≈ 1,6 saat. Çıktı: lif dozu >0 koşu sayısı, medyan maliyetin lifsiz A (1 400 709) ile farkı.
3. **Karar kuralı (sonuç görülmeden yazılmalı):** (b0) medyanı lifsizden ≥ %1 aşağıdaysa veya lif >0 koşu ≥ 5/10 ise fiyat taraması (45; 22,5) 10 koşuyla; aksi hâlde fiyat taraması yapılmaz, "kırılma fiyatı ≤ 0" yazılır.
4. İstenirse (IA kararı) lif etkisi Mpr'den çıkarılan **tanı** koşusu (lif genleri açık, Mpr'de lif 0): modelin lif aleyhine yapısal tutarsızlığını ayırır. Bu bir PCR gerektirir (kod değişikliği); IA onayı olmadan yapılmaz. Makale için zorunlu değil, ama hakem bu soruyu sorar (lif artınca çelik artması).

Toplam: ≈ 2,2 saat (a + b0); ek koşul çıkarsa +3,2 saat (45 ve 22,5 TL/kg, 10 koşu). 30 koşu istenirse b0 ≈ 4,7 saat.

## 6. Sınırlar ve kayıt
- Probe tek makine, kayıt dışı; (a) resmî olarak `runs/` altına yazılmadan makaleye girmez.
- Probe tasarımları `x` (Fpen-en-iyi) vektörlerinden alındı; T=0 olanlar seçildi.
- b0 öncesi bu kural kaydedilmeli (post-hoc değil, ön kayıtlı).
- Diğer açık işler (V3 ETABS, S2, DLL regresyon bit-bit) bu raporun kapsamı dışında, geçerlidir.

## Karar

**[REVİZYON TALEBİ]** (fiyat taraması bu modelde gereksiz; yerine a + b0)

1. Fiyat taraması (b) ve fR taraması (c) koşusu yapılmasın; (a) deterministik tablo + (b0) fiyat=0, 10 koşu yeterli.
2. Makale lif bulgusunu "yapısal kazanç yok, kırılma fiyatı ≤ 0, model varsayımlarına bağlı" olarak yazsın; ekonomi yargısı ve optimum iddiası olmasın.
3. Mpr'de lif, VRd,F formatı ve fR-çelik artışı anomalisi varsayım tablosunda beyan edilsin (isteğe bağlı tanı koşusu IA kararı).
4. Bulgular bölümüne giriş için V3 ETABS + S2 + IA onayı koşulu geçerli.

AUDIT_CHECKLIST: sayı–kaynak eşleşmesi [EVET: ihlal sayıları 360 ham CSV'den; probe değerleri DLL'den, kayıt altı değil]; sürüm izlenebilirliği [EVET: phase3_bin]; iddia sınırları [EVET]; ETABS V3 [HAYIR].
