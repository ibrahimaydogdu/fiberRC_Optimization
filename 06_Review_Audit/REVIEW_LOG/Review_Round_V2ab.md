# Review Round V2a/V2b — FemCore ↔ ETABS 22.6.0 (Critic, 2026-10-30)

Denetlenen: `04_Results/VALIDATION_REPORT.md` §1 (V2a) ve "V2b" bölümü. Dayanak: `03_Code_Execution/runs/V2_fem_etabs_log_2026-10-29.txt`, `.../V2b_column_design_log_2026-10-30.txt`, `program_work/EtabsProbe/{Program,Design}.vb`, `FemRcModel/{ColumnDesign,Combos}.vb`, `G11_REVIEW_V2b_PLAN.md`, `ETABS_AR_EXTRACT_G12.md`. Kod değiştirilmedi.

## KARAR: [REVİZYON TALEBİ: aşağıdaki K1–K5 ve B1 kapatılmadan V3'e geçilmez; V2b "kısmen geçti" ifadesi mevcut kanıtla desteklenmiyor, "ön sonuç / bilgi amaçlı" olarak yeniden etiketlenmeli]

## 1. Sayıların ham logla tutarlılığı (Soru 1)
**Tutan (bizzat yeniden hesaplandı):**
- V2b tablosu (5 birleşim): maks D/C, "program ≥ ETABS" sayıları ve oran min–maks değerlerinin tümü V2b log SUMMARY/RATIO satırlarıyla birebir tutuyor (log s.47–48, 120–121, 164–165, 208–209, 252–253). Sapma ifadeleri (−%1,7; +%21; kritik +%7–12: 0,317/0,295 = +7,5; 0,323/0,297 = +8,8; 0,220/0,197 = +11,7; 0,225/0,201 = +11,9; gravite −%0,3) doğru.
- V2a: G/Q/S taban reaksiyonları (V2 log s.5–7), U3 %0,04 (s.4), 12 periyot T1 −%0,008 / T12 −%0,164 (s.17–28), taban kesmesi −%0,141/−%0,144 (s.29–30), köşe düğüm farkları EX %2,1–3,1 ve EY %4,2–6,3 (s.31–38), C001 M3 +%3,4 ve 6,17/3,61 kNm (s.39, 41) tutuyor.

**Tutmayan / ham kanıtı bulunmayan:**
- **[YÜKSEK] B1.** "Çubuk yüz kuvvetleri P, V2, M3 (116 eleman) ≤ %0,002" (Rapor s.12), "RS, ek dış merkezlik = 0: ≤ %0,09" (s.16), rijit bölge etkileri %18,8 / %3,9 / %0,6 (s.21), "etki oranı 0,61–0,72" (s.23): **ham logda yok.** V2 log yalnız 42 satır; `CompareStatic` çıktısı (Program.vb:190, "max|P| … diff") logda görünmüyor; ECC=0 ve NORZ=1 koşuları ayrı log olarak saklanmamış. AGENTS kural 11 gereği bu sayılar yeniden üretilmeden/kaynağa bağlanmadan rapora girmemeli.
- **[ORTA] B2.** "Testler PASS=62" (Rapor s.36): test çıktısı `runs/` altında yok.
- **[ORTA] B3.** `Design.vb:190` `rows.Take(40)`: V2b logunda 48 yerine 40 kolon satırı var; 8 kolon satır düzeyinde denetlenemiyor (U1 özetindeki min 0,983 logdaki 40 satırdan hesaplanan 0,988'den farklı → saklı kolonlardan biri). Özetler 48 kolonu kapsıyor ama satır denetimi eksik.
- **[ORTA] B4.** Her iki V2b oturumunda `ETABS ERROR ret=1 in SetDiaphragm` (log s.2, 49; Program.vb:73) ve çatlamış oturumda 7 kez `Cannot open file … fileName = .Y_` (log s.56–76) var; raporda hiç anılmıyor. SetDiaphragm hatası muhtemelen ETABS'in varsayılan "D1" diyaframının zaten var olmasından (varsayım, doğrulanmadı); "Y_" dosya hataları açıklanmadı. Zararsız oldukları gösterilmeli.
- **[DÜŞÜK] B5.** V2 log satırları "FemCore(RS, no torsion add)" etiketli (log s.39–42; Program.vb:339); rapor aynı satırları "ek dış merkezlik %5" olarak sunuyor (s.18). Etiket ↔ iddia çelişkisi; FemCore değerinin hangi ECC ile hesaplandığı açık yazılmalı.

## 2. Yöntem geçerliliği ve "kısmen geçti" (Soru 2)
- **[KRİTİK] K1 — Karşılaştırılan büyüklük optimizasyonda kullanılan kısıt değil.** Üretim kodu `ColumnDesign.CheckGroup` (ColumnDesign.vb:92, 106) `AciColumn.BiaxialDcr` (sabit P'de doğrusal toplam) kullanır ve e_min içermez (grep: yok). V2b ise (i) programın kendi yüzeyinin radyal ölçeğini (`Design.vb:35–46`) ve (ii) "program+emin" değişkenini (`Design.vb:175–177`) ETABS ile kıyaslıyor. Yani **üretim kodunun D/C'si (BiaxialDcr, e_min'siz) ETABS ile hiç kıyaslanmadı.** Rapor tablosunun "Maks D/C program" sütunu seçili şekilde emin'li değişkeni veriyor (4.11 için logda emin'siz 0,297/0,302, tabloda 0,317/0,323) ve başlıkta bu belirtilmiyor.
- **[KRİTİK] K2 — "−%1,7" yalnız emin'li değişken için geçerli.** Logdan yeniden hesap (emin'siz radyal / ETABS, logdaki 40 satır): U1'de min **0,914 (C013, −%8,6)**; 8 kolon < 0,95; 26 kolon < 1. E411 X+0,3Y min 0,986; E411 0,3X+Y min 0,981. Gravite birleşiminde program, üretim biçimiyle ETABS'in %8,6 altına düşebiliyor. "Program ETABS'in D/C'sini ≤ %1,7 sapmayla altta yakalıyor" cümlesi üretim kodu için yanlış/yanıltıcı. Çözüm: e_min programa eklenip (PCR; kullanıcı onayı) yeniden kıyaslanmalı veya rapor emin'siz sayıları da vermeli.
- **[YÜKSEK] K3 — Kabul eşiği önceden tanımlı değil.** `VALIDATION_SPEC.md` V2 ölçütü "literatür referans çalışması ≤ %5"tir; FEM↔ETABS veya D/C için eşik yok. G11 planı (§4.5) ">%5 ise nedenler ayrıştırılır" der; bu yapılmadı (Rapor s.48 "nedenleri ayrıştırılmadı"). Ayrıca rapordaki "V2a/V2b" adlandırması SPEC'teki V2 ile çakışıyor. Test öncesi yazılı eşik (güvenli yönde sınır, güvensiz yönde sınır) gerekir; "kısmen geçti" ölçüt olmadan öznel.
- **[YÜKSEK] K4 — "±%10 içinde" bilgisi rapordan çıkarılmış.** Logdaki RATIO satırları deprem birleşimlerinde ±%10 içindeki kolon sayısını 14, 12, 24, 14 / 48 (U1: 30/48) veriyor; rapor yalnız min–maks yazıyor. Emin'li değişkende bile kolonların %50–75'i deprem birleşimlerinde >%10 sapıyor; açıkça yazılmalı.
- **[YÜKSEK] K5 — "Aynı kuvvet" öncülü deprem birleşimleri için sağlanmıyor.** Rapor s.34: "Kuvvetler V2a'da ≤%0,002 uyumlu olduğundan fark tasarım algoritmasından gelir". V2a'nın kendisi ECC=%5'te FemCore momentlerinin %3,4 (EX) ve EY'de ~%70 (6,17 ↔ 3,61 kNm) büyük olduğunu gösteriyor; `Combos.vb:32` varsayılanı 0,05 ve V2b bununla koşulmuş (ECC değeri V2b'de belgesiz; Program.vb:198). V2b oturumu ayrıca kuvvet karşılaştırması yapmıyor (Design.vb), V2a'ya dayanıyor (ayrı oturum, aynı kod durumu doğrulanmamış). Deprem D/C farkları kısmen kuvvet farkıdır. **Zorunlu test:** V2b'yi ECC=0 ile yeniden koş (kuvvetler ≤%0,09 uyumlu iken fark yalnız tasarım algoritması), sonra ECC=%5 farkını ayrı terim olarak ver.
- **[ORTA] K6 — ETABS tasarım tercihleri denetlenmedi.** `Design.vb:118` yalnız `SetCode`; `SetPreference` çağrısı yok. ETABS varsayılanları (çerçeve tipi, φ, e_min, narinlik/δns, etkileşim yüzeyi çözünürlüğü, RS birleşimlerinde işaret işlemesi) raporda kayıtlı değil. İstasyon farkı: ETABS PMMRatio tüm istasyonların maksimumu (Design.vb:134), program yalnız iki uç.
- **[ORTA] K7 — Aynı-donatı düzeni üretimi temsil etmiyor.** `SymSection` (Design.vb:23–32) yüz başına n çubuk; üretim `MakeSection` (ColumnDesign.vb:42–55) farklı katman düzeni. Rapor bunu s.50'de anıyor (iyi) ama kıyas üretim düzenini doğrulamıyor.
- **[ORTA] K8 — Düşük D/C bölgesi.** En büyük ETABS D/C 0,354; D/C 0,8–1,0 sınanmadı. Düşük D/C'de oranlar şişebilir; "≈1" iddiası yalnız 0,15–0,35 aralığı için.

## 3. V2a ek dış merkezlik farkı %2–6 (Soru 3)
- **[YÜKSEK] E1.** Fark "kısmen geçti" için **kabul edilebilir değil**: (a) V2b deprem D/C farkına bulaşıyor (K5); (b) kök neden bilinmiyor; (c) yön bağımlı (EY, EX'in yaklaşık iki katı) → tanım farkını gösterir, yuvarlama değil. Güvenli yönde olması tek başına yeterli gerekçe değil: FemCore burulma etkisini ETABS'ten %40–70 büyük uyguluyorsa optimum gereksiz ağırlaşır.
- **[DÜŞÜK] E2.** Rapor s.23 "ETABS'in burulma momentini nasıl tanımladığı belgelenmedi" diyor; `ETABS_AR_EXTRACT_G12.md` §4b ETABS.chm'de tanımı buldu (yüzde dış merkezlik × diyafram maks. genişliği × diyafram toplam yanal kuvveti, kütle merkezi etrafında) ve kalan iki belirsizliği ve testi yazdı. Rapor bunu yansıtmalı.
- **Gerekli testler:** (1) ETABS'te ECC=%0 ve %5 çözümlerinin farkı (yalnız burulma yanıtı) ↔ FemCore `AddAccidentalTorsion` yanıtı, düğüm düğüm; (2) ECC'yi RS yerine **statik** burulma yüküyle ETABS'te uygulayıp FemCore ile kıyasla ("total lateral force" tanımı: CQC kat atalet kuvveti mi, kat kesmesi mi); (3) birleştirme kuralı (FemCore: mutlak toplam; ETABS: bilinmiyor); (4) diyafram θz ve kütle merkezi koordinatlarının karşılaştırılması. Fark kapanmazsa PCR: FemCore'u ETABS tanımına hizala veya muhafazakârlığı makalede nicel belirt.

## 4. Sekiz işaret deseni düzeltmesi (Soru 4)
- **Mantık doğru.** `ColumnDesign.vb:76–97`: Forces(0)=G'+mag, Forces(1)=G'−mag, mag ≥ 0 (`Combos.vb:100` mutlak değer) ⇒ gP=(fp+fn)/2 statik, eP=(fp−fn)/2 büyüklük; J ucunda sg=−1 ile bası pozitif (tutarlı); M2/M3'te |g ± e| ve s2/s3 taraması doğru; G±E (Eq. 4.11/4.12, ez ile) eşlemesi `Combos.vb:150–164` ile uyumlu. Pu için s1 her iki yönü tarıyor.
- **Gerekçe ampirik:** ETABS'in RS yanıtını hangi işaret kuralıyla tasarladığı ETABS belgesinde doğrulanmadı; "ETABS ile uyumlu" yerine "uyumlu görünüyor, işaret bağımsızlığı varsayımı" yazılmalı. 62 testin bu deseni kapsayıp kapsamadığı belgesiz (B2).
- **Muhafazakârlık:** P, M2, M3 için bağımsız işaret fiziksel olarak üst sınırdır (CQC sonrası korelasyon kayıp). Ölçülen: deprem birleşimlerinde ortalama oran 1,10–1,13 (ECC kuvvet farkıyla karışık, K5). D/C ≈ 1 civarında bu yaklaşık %10'luk talep artışı kolon boyutu/donatısı üzerinden maliyete yansır; etkisi ölçülmemiş. Öneri: 2 desen ↔ 8 desen etkisini kolon başına ayrı tablo ve optimum maliyette duyarlılık olarak raporla; makalede "ETABS ile aynı konservatif işaret varsayımı" olarak belirt.

## 5. Abartı, kaynaksız sayı, birim/etiket (Soru 5)
- **[YÜKSEK] A1.** Başlık "V2 kısmen geçti" ve V2b "ETABS'in D/C'sini ≤%1,7 sapmayla yakalıyor": K1–K4 nedeniyle abartılı.
- **[ORTA] A2.** V2a sonuçları tek deneme tasarımı, tek koşu; genellenebilirlik için ikinci tasarım (farklı kesit seti) gerek.
- **[ORTA] A3.** "Fazlalık ... uyumludur" (Rapor s.48): ayrıştırılmamış bir tahmin; "olası nedenler, doğrulanmadı" olarak etiketlenmeli.
- **[DÜŞÜK] A4.** Tablo başlığı "Maks D/C program" emin'li değişken olduğu belirtilmeli; −%8,6 (emin'siz) rapordan çıkarılmış (K2).
- **[DÜŞÜK] A5.** Birimler (kN, kNm, mm, oran) doğru; "N/A" yok. Doğrulama tabloları elle yazılmış; ham log ↔ tablo bağı betikle üretilmeli.
- **[DÜŞÜK] A6.** ACI 318-14'te e_min yalnız narin kolon momenti alt sınırı olarak geçer (6.6.4.5.4); ETABS CFD bunu tüm kolonlara uyguluyor; programa eklenecekse ETABS tanımı sayfa/madde ile belgelenmeli (G11'de ACI formülü DOĞRULANDI, uygulama kapsamı okunmadı).
- Rijit bölge bulgusu (eksenel/burulma esnek) G12 §1 ile DOĞRULANDI; ancak rijit uç faktörü RZ=1 modelleme tercihidir (kılavuz ≤~0,5 önerir, s.132) ve makalede gerekçelendirilmeli.

## 6. V3'e geçmeden zorunlu ek kanıtlar (Soru 6)
1. **Z1** Ham log arşivi: ECC=0, ECC=%5, NORZ=1 V2a koşuları; 62 test çıktısı; V2b 48 satırın tamamı; SHA-256 ve derleme sürümü (AGENTS kural 7).
2. **Z2** Önceden yazılı eşik (K3) ve eşiğe göre geçti/kaldı tablosu; üretim kodu (e_min'siz BiaxialDcr) ile kıyas veya e_min için onaylı PCR (K1/K2).
3. **Z3** V2b'yi ECC=0 ile tekrarla; ECC=%5 farkını ayrı raporla (K5).
4. **Z4** %2–6 ek dış merkezlik farkının kök nedeni veya FemCore'un ETABS tanımına hizalanması (E1).
5. **Z5** Yüksek D/C (0,8–1,0) ve üretim kolon donatı düzeninde en az iki ek kolon grubu; P-Δ/δns etkisinin ölçümü (G11 §3.3'te planlanmıştı) (K7, K8).
6. **Z6** Kiriş tarafı: ETABS `GetSummaryResultsBeam` (TopArea/BotArea, kesme Av/s) ↔ program kiriş As/Av; kolon kesme ve kapasite tasarımı (Mr/Ve, TBDY) ETABS "TS 500-2000(R2018)" menüsüyle (G11 §3.8).
7. **Z7** SFRC kiriş: ETABS lifli kesiti tasarlamaz; bağımsız doğrulama (fib MC2010 σ–ε kesit analizi için ikinci uygulama/literatür deneyi/referans örnek) ve kabul eşiği V3 öncesi yazılı olmalı; "ETABS'te doğrulandı" iddiası lifli kiriş için yapılamaz, sınırlama olarak yazılmalı.
8. **Z8** ETABS tercih dökümü (K6) ve `ETABS ERROR`/`Cannot open file` uyarılarının açıklaması (B4).

## 7. Kontrol listesi (bu tur)
| Madde | Durum |
|---|---|
| Rapor sayıları ham logla tutuyor | KISMEN (V2b: EVET; V2a ECC=0/NORZ/kuvvet: HAYIR) |
| Karşılaştırma üretim koduyla yapılmış | HAYIR (K1) |
| Önceden tanımlı kabul eşiği | HAYIR (K3) |
| Doğrulanmamış yorumlar etiketli | HAYIR (A3) |
| "N/A" hücre | EVET (yok) |
| Birim disiplini | EVET |

## 8. En önemli 5 madde
1. K1/K2: Üretim D/C'si (BiaxialDcr, e_min'siz) ETABS ile kıyaslanmadı; e_min'siz radyal ETABS'in %8,6 altına düşüyor (U1, C013); "−%1,7" yalnız e_min'li değişken.
2. K5/E1: "Aynı kuvvet" öncülü deprem için yanlış; %2–6 (moment ~%70'e kadar) ek dış merkezlik farkı çözülmeden D/C farkı ayrıştırılamaz; V2b'nin ECC=0 koşusu gerekli.
3. B1: V2a ana iddiaları (≤%0,002 çubuk kuvveti, ECC=0 ≤%0,09, rijit bölge %18,8/%3,9/%0,6) ham logda yok.
4. K3/K4: Önceden tanımlı eşik yok; ±%10 bilgisi (deprem 12–24/48) rapordan çıkarılmış; "V2" adlandırması VALIDATION_SPEC ile çakışıyor.
5. Z6/Z7: Kiriş/kesme/SFRC ETABS'e karşı hiç kıyaslanmadı; SFRC için bağımsız doğrulama yolu ve eşiği V3 öncesi şart.
