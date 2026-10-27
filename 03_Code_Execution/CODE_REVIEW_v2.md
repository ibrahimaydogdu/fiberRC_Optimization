# CODE_REVIEW_v2.md — Lifli/lifsiz betonarme çerçeve optimizasyon programı (v2) kod incelemesi

Tarih: 2026-10-27. Kapsam: `Sources\Betonarme\Lİfli\v2\` içindeki `ETABSClass.vb` (1704 satır), `OptimizationClass.vb` (1044), `Structures.vb` (426), `MainForm.vb` (362) tam okundu; `MainForm.Designer.vb` yalnızca varsayılan metinler için tarandı; `Betornarme.vbproj`, `Inputinfo.txt`, `ETABS\Seda\Output.xml` ve `ErrorLog.txt` kanıt için okundu. Makale metni (`MakaleMetni.docx`, pandoc ile) Denklem 1–10 ve Çizelge 1, 4, 5, 6 ile karşılaştırıldı.

Yöntem ve sınırlar:
- Kod ÇALIŞTIRILMADI, DERLENMEDİ, hiçbir kaynak dosya DEĞİŞTİRİLMEDİ. Bu dosya tek çıktıdır.
- Her bulgunun durumu işaretlidir:
  - **DOĞRULANAN-kod**: kod okuyarak kesin (VB.NET anlambilimi tartışmasız).
  - **DOĞRULANAN-sayısal**: depodaki `Output.xml` değerleri veya makale tablolarındaki aritmetikle yeniden hesaplanarak teyit edildi.
  - **ŞÜPHELİ**: tahmin; çalışma zamanı veya EDB içeriği görülmeden kesinleşmez. Doğrulama yolu belirtildi.
- Uyarı: `ETABS\Seda\Output.xml` 1 Nisan 2024 tarihli, `ETABSClass.vb` 13 Mayıs 2024 tarihli. XML eski sürümden olabilir (örneğin `Fiber_Catlak.M = 1e7` sabit değeri XML'de var, güncel kodda `Md` kullanılıyor, `ETABSClass.vb:1280-1281`). XML'den çıkarılan sonuçlar bu yüzden "sürüm farkı olabilir" notuyla verildi.
- Kısaltmalar: `E:` = ETABSClass.vb, `O:` = OptimizationClass.vb, `M:` = MainForm.vb, `S:` = Structures.vb.

---

## 1. Özet tablo

| No | Şiddet | Dosya:satır | Bulgu | Etkilenen makale tablosu/denklemi | Durum |
|---|---|---|---|---|---|
| F-01 | KRİTİK | E:987, 984-990, 998-999, 1475, 1504, 1518 | Kolon PMM oranı ve ETABS hata cezası hiç kullanılmıyor: `ConsGetSummaryResultsCoulmn(Frame As Frame_ ...)` ByVal, `Ratio` ve `Add` yerel değişkenleri hep 0. Kolon cezası daima 0. | g1, g2 (Denk. 2-3); Çizelge 4-6 MKKKO ("N/A"); Denk. 1 (kolonlar fiilen kısıtsız) | DOĞRULANAN-kod + -sayısal (Output.xml: 10 grubun tümünde `PMM_Ratio`=0, `Penalties`=0) |
| F-02 | KRİTİK | E:945, 962, 969, 973-1000 | `SReinforcement` yalnız kiriş döngüsünde sıfırlanıyor; kolon döngüsünde sıfırlanmıyor, kolon grupları kümülatif toplanıyor. `CostStructure` bunları topluyor (E:917) → kolon donatısı kat kat fazla sayılıyor. | Denk. 1 (ΣW_çelik); Çizelge 4-6 donatı kg ve toplam maliyet; "%22-%68 donatı azalması" | DOĞRULANAN-kod; etki büyüklüğü çalıştırmadan hesaplanmadı |
| F-03 | KRİTİK | E:1018-1023 (+E:392-395, 1640-1662) | Lifli modda her kiriş için `DeselectAllCasesAndCombosForOutput` + tek kombinasyon seçimi yapılıyor, sonda yalnız SON kombinasyon seçili kalıyor. Sonraki değerlendirmelerde `UpdateJointDisp` yalnız bu kombinasyondan ötelenme okur. | g6, g7 (Denk. 7-8); Çizelge 6 MKÖ/MEÜKÖ (lifli 0.13-0.24 mm, lifsiz 6.6-7.1 mm) | DOĞRULANAN-kod; çalışma zamanı teyidi gerekli (Bölüm 4) |
| F-04 | KRİTİK | E:850-856, 1570-1596, 682-684 | `Modifier_PMM` kiriş vektörünü rastgele büyütüyor ama ceza ≤ 1 iken yeniden analiz/yeniden Setsection yok. `Member.DesignVariables` (ByRef) değişiyor, maliyet/ceza eski tasarımdan. Ayrıca onarım `Rnd()` ile rastgele: aynı vektör farklı sonuç verir. | Tüm optimum tasarımlar (Çizelge 4-6); "en iyi çözüm" tekrarlanabilirliği | DOĞRULANAN-kod |
| F-05 | KRİTİK | E:416-417 vs 717, 725; E:671, 714-720, 925-937 | Maliyet yalnız "C30" ve "C40" adlarına atanıyor, malzeme seçimi "C50" adını arıyor → C50 betonu maliyeti 0 olabilir. `CMOD_data` 16. satırı (`37.7`) ne 37 ne 50: malzeme güncellenmez (önceki değerlendirmenin malzemesi kalır); lif tipi 6 için `l_mal` atanmaz → lif bedava. | Denk. 1; Çizelge 5-6 "C30/C40" belirsizliği; C40 maliyeti 55.49 $/m³ | C50 maliyeti: ŞÜPHELİ (EDB malzeme adlarına bağlı); satır 16 ve `l_mal`: DOĞRULANAN-kod |
| F-06 | KRİTİK | E:1250-1251 | Fib MC2010 kesme: `0.18·k·…` kullanılmış, `γc = 1.5` bölünmemiş (0.12 olmalı). VRd,F 1.5 kat fazla. | g4 (Denk. 5); Çizelge 4-6 MKO | DOĞRULANAN-sayısal (Output.xml'de Vrd = 164 188 N yalnız 0.18 ile yeniden üretiliyor) |
| F-07 | KRİTİK | E:1159-1163, 1223-1225, 1059-1060 | Eğilme kapasitesi (MRdu) nominal: basınç bloğunda `fcm` (ortalama), çelikte `fsy` (karakteristik), φ ve γ yok; `beta = 0.57` ("???" notlu); Asmax TS500 tipi (0.85ρb), ACI değil. | g3 (Denk. 4); Çizelge 4-6 MKKKO | DOĞRULANAN-sayısal (aşağıda hesap) |
| F-08 | YÜKSEK | E:741; O:964-971, 977-978 | Orijinal EDB her değerlendirmede üzerine kaydediliyor (`File.Save(ETABSFile)`). Uygun çözüm bulunamazsa `Evaluate(GlobalBest)` yanlış üyeyi (sıfır vektörü) değerlendirip kaydediyor, `Yazdir_Final(Bestmember)` ile uyumsuz. Dosya adı `…EDBbest.EDB` (ayırıcısız). | Tüm tablolar (yeniden üretilebilirlik) | DOĞRULANAN-kod |
| F-09 | YÜKSEK | E:1112-1124 vs 1452-1459 | Donatı metrajı lifli ve lifsiz yolda farklı tanımlı: lifli = |M| tek katman As_ + stirrup `Asw` (mm²) doğrudan uzunlukla çarpılıyor (spacing ve çevre yok); lifsiz = ETABS üst+alt+kesme, kesme için sabit ×1000. Lifli yol eksik sayıyor. | Denk. 1; "donatı %22-68 azalıyor" (Özet, Çizelge 4-6) | DOĞRULANAN-kod + -sayısal (Asw = 48 mm²/200 mm) |
| F-10 | YÜKSEK | E:1141-1148, 1160-1163 | As_ tek adımda `As·Md/Mrdu` ile ölçekleniyor (doğrusal olmayan ilişki), clamp (Asmin/Asmax) sonrası Mrdu yeniden hesaplanmıyor. Sonuç: As kapasiteyle tutarsız. | g3, g8, g9 | DOĞRULANAN-sayısal (Output.xml: Md=96.1 kN·m, Mrdu=181.6, As=1605 mm², oran 0.53) |
| F-11 | YÜKSEK | E:1276-1277, 1298-1300, 1105-1106, 1056-1057 | Çatlak: I_ii'ye hatalı fazladan terim, örtü terimi (k·dd·4) yanlış çarpılıyor, Wd<0 → 0 kırpılıyor, ULS momenti (servis değil), limit 0.3 mm sabit, d_ = 25 mm sabit. | g5 (Denk. 6); MÇKO | DOĞRULANAN-sayısal (I_ii 837.3e6 yerine 670.6e6) |
| F-12 | YÜKSEK | E:1087-1095, 1256-1268 | Kesme kısıtı yapısal olarak hep sağlanıyor: gerekirse `Asw` serbestçe artırılıp oran tam 1 yapılıyor; VRd,max (ezilme) yok, ACI Av,min yok, `Asw_min` yalnızca `VrdF<Vd` dalında. | g4; Çizelge 4 "MKO = 1" | DOĞRULANAN-sayısal (Output.xml `Ratio_Shear = 1`, `Asw = 48.04 mm²`) |
| F-13 | YÜKSEK | E:1071, 1132-1136, 1097-1099 | Donatısız (As=0) yol: koşulda çift bölme (`FR_L1/fL` zaten FR1/fL); MRdu numune boyutundan (hz=100, a=25 mm) hesaplanıyor, kesit derinliğinden değil; "kesme" oranı V yerine M'den (Sigma1 = 6M/(bh²)). | g3, g4; "lif donatının yerini alır" iddiası | DOĞRULANAN-kod; Output.xml: FR_L1=0.54 → 0.108 < 0.4 |
| F-14 | YÜKSEK | E:1605-1608, 1626-1629, 1583-1585, 834-838 | Onarım indeks hataları: `Sect_Ind(ID)` içinde ID = `Groups` dizisi indisi (tasarım vektörü indisi değil); kolon `Modifier_PMM`'de kiriş sayısı ofseti yok; `Modifier_Geometric` kiriş için `RC_Sections` (kolon listesi) indeksi üretiyor. | Çizelge 4-6 kesit seçimleri | DOĞRULANAN-kod; etkisi grup adlandırmasına bağlı |
| F-15 | YÜKSEK | O:554-567, 58 | ATO: `Member` sıfırdan oluşturuluyor, `rand > 0.8` olmayan ~%80 değişken kopyalanmıyor (0 → Lb). `ns` ifadesinde parantez hatası (n_low + (n_high − n_low·r)). | Çizelge 4-6 ATO sütunu; "ATO en başarılı" (Örnek 2) | DOĞRULANAN-kod |
| F-16 | YÜKSEK | O:95, 125, 755; E:1682 | `Lb + Floor((Ub−Lb)·Rnd())` üst sınırı hiç üretmiyor. Lifsiz modda malzeme değişkeni Ub=1 → başlangıç ve HS/BTO rastgele adımlarında hep 0 (yalnız C30); lifli modda CMOD son satırı (16. sat.) hiç örneklenmiyor. | Lifli-lifsiz adil karşılaştırma (Özet, 3.5) | DOĞRULANAN-kod |
| F-17 | YÜKSEK | O:61-65, 575-748 (özellikle 635, 643, 666, 706-708, 739) | VAO/İVAO: tek `WildHorses` yöntemi (İVAO ayrımı kodda yok); `TDR` hesaplanıp kullanılmıyor; sıralama `CostValue` ile (ceza yok sayılıyor); çaprazlamada `Range(0, NStallion−1)` son aygırı dışlıyor, NStallion=2 ise indeks hatası; `−GlobalBest` terimi indeks uzayında Lb'ye kırpılıyor. | Çizelge 1; 2.2.4; "İVAO 5477 $ ile açık ara en iyi" (Çizelge 6) | DOĞRULANAN-kod (İVAO'nun başka sürümde olması: ŞÜPHELİ) |
| F-18 | YÜKSEK | O: tüm `Rnd()` çağrıları; `New Random()` (O:55, 298, 307, 401, 442, 478, 525, 543, 612, 632) | `Randomize()` hiç çağrılmıyor → `Rnd()` her süreçte aynı diziyi üretir (HS, BTO, Whale, Dandelion, onarımlar, VAO bölümleri). `New Random()` zaman tohumlu, hızlı ardışık çağrılarda aynı değer (`GetNormalRandom` u=v). Tohum kaydı/kontrolü yok. | "Bağımsız koşular"; istatistik; Şekil 4.14-4.22 | DOĞRULANAN-kod |
| F-19 | YÜKSEK | E:736, 683-684; M:25-33; O:47-66 | Bütçe sayacı `Setsection` çağrısı sayısı (onarım ve yeniden analizler dahil), aday değerlendirmesi sayısı değil. Döngü yalnızca tur sınırında kontrol ediliyor; yöntemler tur başına farklı sayıda değerlendirme yapıyor (YAKO 2/üye, ATO ns/ağaç, VAO (ngroup+1)×NStallion). Eşit bütçe yok. | Çizelge 1 "Maksimum Döngü Sayısı 10 000"; yöntem karşılaştırması | DOĞRULANAN-kod |
| F-20 | YÜKSEK | E:253, 355, 368, 1446, 1494, 1658, 1011-1041; ErrorLog.txt | Sessiz hata yutma: `Return ret` ile ret=0 dönerek başlatma başarılı sanılıyor; `GetSummaryResults*` hatasında `Frame` değişmeden (ceza 0, donatı 0) devam; `JointDispl` hatasında `Exit Sub` ile eski ötelenme verisi kalıyor; birçok `Errorlogprint` sonrası devam. Depodaki `ErrorLog.txt` tekrar eden `Results.JointDispl` hatalarını içeriyor. | Tüm sonuçlar | DOĞRULANAN-kod + kanıt (ErrorLog.txt) |
| F-21 | YÜKSEK | E:436-442, 1052, 1059; Output.xml | Donatı malzemesi "A615Gr60" adıyla sabit: fsy=413.685 MPa (makale S420 = 420). `fc` ETABS'ten 37 MPa geliyor, kodda `fckcy = 0.8·fc` = 29.6; aynı fc ETABS ACI kolon tasarımında f'c olarak kullanılıyor (küp mü silindir mi tutarsız). | Çizelge 3.1 malzeme parametreleri; g1-g3 | fsy: DOĞRULANAN-sayısal; küp/silindir: ŞÜPHELİ |
| F-22 | YÜKSEK | E:94-96; Betornarme.vbproj:69-71; Inputinfo.txt; M:207; O:1001 | Sabit yollar ve tek sürüm: ETABS 19 yolu ve `ETABSv1.dll` HintPath'i ETABS 19; `Inputinfo.txt` E:\OneDrive…; `BackUp.xml` göreli yol (eşzamanlı koşular birbirini ezer). ETABS 22 için ayar yok. | Yeniden üretilebilirlik | DOĞRULANAN-kod; v22 uyumu ŞÜPHELİ |
| F-23 | YÜKSEK | O:897 | `GlobalBestPrint` kolon kesitini `DesignVariables(i + ColumnDesignGroupIDs.Count)` ile okuyor (olması gereken `+ BeamDesignGroupIDs.Count`) → yanlış kolon etiketi veya sınır dışı indeks; malzeme/lif değişkeni yazdırılmıyor. | Çizelge 4-6 kesit satırları | DOĞRULANAN-kod |
| F-24 | YÜKSEK | Makale Çizelge 4, 5, 6 (kodda döküm üreten fonksiyon yok) | Lifli sütunlarda "Donatı / Beton / Lif Maliyeti" satırları döngüsel kaymış: "Donatı Maliyeti" satırında lif bedeli, "Beton Maliyeti" satırında donatı bedeli, "Lif Maliyeti" satırında beton bedeli var. "Lif Miktarı" kg değeri lif maliyetiyle uyuşmuyor. Çizelge 5 YAKO lifsiz donatı bedeli 843.23 kg × 2.30 ≠ 2059.52. | Çizelge 4, 5, 6 satır etiketleri | DOĞRULANAN-sayısal (Bölüm 3.3) |
| F-25 | ORTA | E:387-395, 1523-1569; S:181; MainForm.Designer.vb:660 | Ötelenme tüm kombinasyonlarda (ULS) ölçülüyor, makale "servis yükleri" diyor; `DispLimit` (0.0636) okunuyor ama hiç kullanılmıyor; sınır h/R (R=300 varsayılan) sabit. | g6, g7 | DOĞRULANAN-kod |
| F-26 | ORTA | E:682, 863-867, 855, 884, 912; E:970, 999 | Ceza yapısı makalede tanımsız: `Maliyet·(1+P)³`; P = gruplar arası MAKSİMUM (toplam değil); ETABS hata cezası süreksiz (hatalı istasyon oranı); g8/g9 (ρmin/ρmax) ceza değil sessiz kırpma; g1/g2 kolonlarda yok (F-01). Makale "ihlalde amaç fonksiyonuna ceza eklenir" diyor. | Denk. 2-10, 2.1.3 | DOĞRULANAN-kod |
| F-27 | ORTA | E:1665-1685; S:314-427 | Tasarım değişkenleri makaledekiyle farklı: donatı çapı/adedi değişken değil (As sürekli, fi_s türetilir), lif tipi+dozaj+beton sınıfı tek bir 16 satırlık tablo indeksine bağlı, kesitler ETABS'teki "BEAM"/"COLUMN" adlı hazır kesitler. | 2.1.2, 3.1 | DOĞRULANAN-kod |
| F-28 | ORTA | E:1056-1057, 1063, 1145, 1173 | d = h − 25 mm sabit (Output.xml'de φ26 donatıyla uyumsuz: sadece yarıçap 13 mm); fi_s tam sayıya yuvarlanıp As ile tutarsız; `Cons_Fiber_Func` sabit 12 mm kullanıyor. | g3, g5 | DOĞRULANAN-kod |
| F-29 | ORTA | O:903-912, 516-520; M:27-30 | `Memory.Distinct()` dizi alanı nedeniyle (referans eşitliği) tekrarları silmez; `ClearDuplicates` bayrağı hiç kontrol edilmeden çağrılır; her turda `Memory` sıralanınca `Trial(i)` / `ParVec(i)` ile üye eşleşmesi bozulur; YAKO kâşif arıdan sonra `Trial` sıfırlanmaz. | YAKO, BTO | DOĞRULANAN-kod |
| F-30 | ORTA | E:262-282, 147, 306, 324, 600-602, 186-189 | Başlatma sağlamlığı: `InitilizeSlabArea` Integer döndürür (alan kesilir), `Points(Pnt)` string ile dizi indisler, yanlış indeks (j2); `A992Fy50` çelik malzemesi okunmaya çalışılıyor (beton modelinde yoksa başlatma düşer); `Base` katında kolon yoksa `Where(...)(0)` hatası; `FindIndex` = −1 indeks kontrolü yok; `Zcoord = Level` tam double eşitliği. | 3D örnek (Örnek 3) | DOĞRULANAN-kod; çalışma durumu ŞÜPHELİ |
| F-31 | ORTA | E:729, 733, 750-753 | `SetRectangle` her değerlendirmede mevcut kesiti yeniden tanımlıyor (stiffness modifier ve donatı tasarım verisi sıfırlanıyor olabilir); `DesignConcrete.SetCode` her tasarımda tekrar çağrılıyor (tercihler/override sıfırlanabilir); tasarım kombinasyonları (`SetComboStrength`) hiç ayarlanmıyor. | g1, g2; N/A (Bölüm 5) | ŞÜPHELİ |
| F-32 | ORTA | E:916 (hacim) vs makale Çizelge 4 | Beton hacmi tabloları kesit etiketleriyle elle tutmuyor (örnek: Çizelge 4 lifli YAKO, 35×25 kiriş 2×6 m + 50×30 kolon 4×3 m = 2.85 m³; tabloda 3.15 m³; fark 0.18-0.72 m³ değişken). Etiket sırası (D×B?) veya hacmin başka kaynaktan alınması. | Çizelge 4 beton miktarı | ŞÜPHELİ |
| F-33 | ORTA | M:314, 296-297, 344; O:927 | Arayüz: `Check_Structure` çıktıyı `OutputLoc & "xml"` yazar (nokta yok; `Output.xmlxml` depoda var); `Control()` `FormInfo_Read`'den önce; `Application.DoEvents()` her değerlendirmede (yeniden giriş: Start'a tekrar basılabilir); `time_calc` hatalı (`Minutes*60`); optimizasyon UI iş parçacığında. | Operasyonel | DOĞRULANAN-kod |
| F-34 | ORTA | O:76-89, 190, 132-133 | HS dinamik PAR/HMCR: `Costs.Max−Costs.Min` = 0 bölme, `List(Of Double) = Select()` dönüşümü çalışma zamanı hatası; HS perde ayarı `Floor(±küçük)` ile asimetrik (hep −1 veya 0); Dandelion ortalaması N−1'e bölünüyor; Whale/HS bazı yollarda tür dönüşümü `CInt` (banker yuvarlaması) ve `Floor` karışık. | YAKO/BTO dışı yöntemler | DOĞRULANAN-kod |
| F-35 | DÜŞÜK | E:53, 699, 707, 730, 734; O:1-8 | `Close()` içinde `MsgBox` (toplu koşuyu bloklar); `Stop` ifadeleri; gereksiz `Imports DocumentFormat.OpenXml…` ve `…VisualStyleElement.Rebar`; kullanılmayan `Gamma`, `Enhanced_Dandelion_Main`; `Finalize` çıktı yazar. | — | DOĞRULANAN-kod |
| F-36 | DÜŞÜK | E:1160, 1165-1242, 1303-1319 | Aşırı sabit ve yarı belgeli parametreler: Newton başlangıcı 34.258, `Tbm = 1.8 fctm`, `epsilon_Fu = 0.02`, `B_ = 0.6`, `k = 1`, Secant'ta başarısızlıkta −1 döner (kontrol yok), `Cons_Fiber_Egrilik` sonucu hiçbir yerde kullanılmıyor. | g3, g5 | DOĞRULANAN-kod |

---

## 2. Ayrıntılı bulgular

### 2.1 Formülasyon

**F-01 Kolon oranları ve cezası yok (E:973-1000, 1475-1520).**
Kolon döngüsünde `Ratio` ve `Add` yerel değişkenleri 0 olarak tanımlanıyor (E:985-986) ve hiçbir yerde güncellenmiyor. `ConsGetSummaryResultsCoulmn` çağrısı `Frame` parametresini ByVal alıyor (E:1475), içeride atanan `Frame.Ratio_PMM = PMMRatio.Max` (E:1504) ve `Frame.Penalty = Add` (E:1518) çağırana dönmüyor. Sonuç: `Groups(i).PMMRatio` = 0, `Groups(i).Penalty = Max(0 − 1 + 0, 0) = 0`.
- Senaryo: kolon kesiti ETABS'te "O/S" (aşırı gerilmiş) çıksa bile ceza 0 olur; optimizasyon kolonu en küçük kesite çeker, yalnız ötelenme ve geometri sınırlar. Makale g1/g2 (Pu/φPn, Mu/φMn) fiilen uygulanmıyor.
- Kanıt: `Output.xml` 75-98. satırlar, 10 grubun `PMM_Ratio` ve `Penalties` değerlerinin tamamı 0. Çizelge 4-6'daki "N/A" ve kolon "25X25" (minimum) sonuçları ile uyumlu.
- Ek: kiriş grubu için `Groups(i).PMMRatio` hiç atanmıyor (yalnızca kolon döngüsünde, E:998); beşinci grupta raporlanan oranlar da 0.
- Çözüm önerisi (uygulama değil): parametreyi ByRef yapmak veya sonuç döndürmek; `PMMRatio.Max` ve `Penalty` değişkenlerini doldurmak.

**F-02 Kümülatif donatı (E:945, 969, 988, 997).**
Kiriş döngüsünde her grupta `SReinforcement = 0` (E:945), kolon döngüsünde sıfırlama yok. Birinci kolon grubu: `SReinforcement` = (son kiriş grubunun toplamı) + r1; ikinci grup: önceki + r2; vb. `Groups(i).SumReinforcement = SReinforcement` (E:997). `CostStructure` `Groups.Sum(c.SumReinforcement)` kullanıyor (E:917).
- Senaryo: k kolon grubu, son kiriş grubu donatısı B, kolon grubu donatıları r1..rk ise toplam = (B+r1) + (B+r1+r2) + … → B terimi k kez sayılır. Lifsiz modda B (ETABS üst+alt+kesme) lifli moda göre daha büyük olduğundan, yanlılık lifsiz tasarımı daha çok cezalandırır → makalenin "lifli daha ucuz" sonucuna yön verir.
- Çalıştırmadan etki büyüklüğü verilemez; ölçüm yolu: çalışma kopyasında grup başına `SumReinforcement` ve ETABS tablolarından bağımsız toplam.

**F-06 Kesme, γc eksik (E:1250-1253).**
`VRd,F = [0.18·k·(100·ρ·(1+7.5·fFtu/fctm)·fck)^(1/3) + 0.15·σcp]·b·d`. fib MC2010 (7.7-?) ve EC2 ile uyumlu biçim `0.18/γc` (γc=1.5 → 0.12). Kod 0.18 kullanıyor.
- Sayısal doğrulama (Output.xml, kiriş 6): k=1.7845, p1=0.01976, fFtu/fctm=1.182/2.8066, fck=29.6 → (100·0.01976·4.158·29.6)^(1/3) = 6.24; 0.18·1.7845·6.24 + 0.15·0.1058 = 2.020 MPa; × (250·325) = 164 188 N = XML'deki `Vrd`. 0.12 ile: 1.352 MPa → ≈109 900 N (%33 düşük).
- Ayrıca `fctm` yerine karakteristik `fctk` ve `fFtuk` (karakteristik) kullanılmalı; kod ortalama (fctm, fR1/fR3 tablodaki değerler) kullanıyor. fR1k/fR3k değerlerinin ortalama mı karakteristik mi olduğu kaynak veride (Chen vd., Venkateshwaran & Tan) doğrulanmalı. ŞÜPHELİ.

**F-07 Eğilme kapasitesi (E:1159-1163, 1223-1225).**
- Basınç bloğu: `xu = (fsy·As + fFt·b·h)/((2/3)·b·fcm + fFt·b)` → `fcm` (= fckcy + 8) kullanılıyor, MC2010 tasarım dayanımı `fcd` yerine. `fsy` (413.7) kullanılıyor, `fyd` (359.7) hesaplanıp yalnız kesmede kullanılıyor (E:1264).
- `beta = 0.57` (E:1224, yorum "???") ve (2/3) bloğu aynı kaynaktan doğrulanmamış; paraboloik blok için ağırlık merkezi 0.375·xu, parabol-dikdörtgen için 0.416·xu.
- Sayısal: Output.xml kiriş 6 (b=250, h=350, d=325, As=1605): MRdu = 181.6 kN·m. Aynı kesit için ACI 318-14: a = 1605·413.7/(0.85·29.6·250) = 105.6 mm; Mn = 1605·413.7·(325−52.8) = 180.7 kN·m; φMn = 0.9·180.7 = 162.6 kN·m. Kod nominal kapasiteyi (φ yok) kullanıyor: ACI'ye göre %11, MC2010 γs=1.15 ile ~%14 güvensiz.
- Asmax/Asmin: `ρb = 0.85·k1·fcd/fyd·600/(600+fyd)`, `k1 = 0.85−0.006(fc−25)` ≥ 0.70 (TS500 biçimi); ACI β1 = 0.85−0.05(f'c−28)/7 ≥ 0.65 ve ρmax (εt ≥ 0.004), ρmin = max(0.25√f'c, 1.4)/fy kullanılmıyor. Makale "ACI 318-14 ve fib MC2010" diyor; kiriş tasarımı ne ACI ne saf MC2010 (karma TS500/MC2010).
- `As,min` MC2010 7.6 biçimi (`kc·k·(fctm−fFts)·Act/σs`) ile uyumlu ama Mrdu bu As_min kırpılmadan ÖNCE hesaplanıyor (F-10).

**F-10 As ölçekleme (E:1141-1148, 1160-1163).**
Başlangıç As_ = 2φ6 = 56.5 mm²; Mrdu hesaplanır; `As ← As·Md/Mrdu` (tek adım). Mrdu As ile doğrusal olmadığından (özellikle lif katkısı ve baskı bloğu), As gereğinden çok farklı olabilir.
- Kanıt: Output.xml kiriş 6: Md = 96.06 kN·m, Mrdu = 181.57 kN·m (oran 0.529), As_ = 1605 mm² (ρ=%1.98). Kapasite ihtiyacın 1.89 katı; optimizasyon bu As'ye göre donatı maliyeti hesaplıyor. Gerçek gerekli As (ACI) ≈ 96.06e6/(0.9·413.7·0.9·325) ≈ 880 mm² mertebesinde (kaba el hesabı).
- Asmin/Asmax kırpması Mrdu hesabından sonra (E:1161-1162): As > Asmax ise Mrdu büyük As ile hesaplanmış kalıyor (güvensiz), As < Asmin ise Mrdu küçük kalıyor (muhafazakâr).

**F-11 Çatlak genişliği (E:1270-1302).**
1. `I_ii = (b/3)x³ + b·x·(x/2)² + αe·As·(d−x)²` (E:1276-1277): ikinci terim çifte sayım. Doğrusu `b x³/3 + αe As (d−x)²`. Output.xml: x=138.72, b=250, As=1605, αe=8.044 → doğru I = 222.5e6 + 448.1e6 = 670.6e6 mm⁴; kod 837.3e6 (XML değeri) → σs %20 eksik.
2. `Wd = 0.5·((k·dd·4) + φ/ρs,ef)·((fctm−fFts)/Tbm)·(σs−β σsr)/Es` (E:1298): örtü terimi (k·dd·4) boyutsuz `(fctm−fFts)/Tbm` ile çarpılıyor. MC2010 7.6: `ls,max = k·c + 0.25·(fctm−fFts)/τbms·φ/ρs,ef`, `wd = 2·ls,max·(…)/Es`. Doğru ifadede örtü terimi toplanır. `Cons_Fiber_Func` (E:1363) ise doğru biçimi kullanıyor: iki yer tutarsız.
3. `σsr` için `ps` (E:1292-1293) kullanılıyor, MC2010 `ρs,ef`; `Wd<0 → 0` (E:1300): çoğu durumda σs < β σsr olduğundan çatlak oranı 0 çıkıyor (Output.xml: Ratio_Crack 0.0066 ve 0).
4. Moment ULS kombinasyonundan (`Md` = |M3| faktörlü); servis (karakteristik/sık) kombinasyonu yok. Makale g5 "servis" diyor.
5. Limit 0.3 mm kodda sabit (E:1105-1106), form parametresi değil.
6. Sonuç: çatlak kısıtı pratikte bağlayıcı değil; Çizelge 4-6 MÇKO 0.20-0.37 değerlerinin anlamı yok ("liflerin üstün çatlak kontrolü" yorumu kanıtlanmıyor).

**F-12 Kesme kısıtı yapısal olarak hep ≤ 1 (E:1087-1095, 1256-1268).**
`VrdF < Vd` ise `Asw = (Vd − VrdF)·s/(z·fyd)` ile stirrup alanı gereken değere ayarlanıyor ve oran tam 1.0 oluyor; üst sınır yok. Ratio_Shear = 1 (Output.xml); Çizelge 4 lifli kolonlarda MKO = 1 aynı sebep. `Asw` = 48 mm² / 200 mm (iki kol φ5.5), ACI 9.6.3.3 Av,min veya TBDY sık sargı detaylandırması yok. VRd,max (basınç çubuğu) kontrolü yok. `Asw_min` yalnızca `VrdF<Vd` dalında (E:1260), yani beton yeterliyse lif yetersiz olsa da minimum etriye aranmıyor.
- Senaryo: g4 hiçbir zaman ihlal edilemez; makaledeki "lif kesme donatısını azaltır" iddiası bu kısıt üzerinden test edilemiyor.

**F-13 Donatısız yol (E:1071, 1130-1138, 1097-1099).**
- E:1071: `FR_L1 / fL > 0.4`; ama `FR_L1 = FR1/fL` zaten (E:1067). MC2010 koşulu `FR1/fL > 0.4` ve `FR3/FR1 > 0.5`; kod `FR1/fL² > 0.4` ve `(FR3/FR1)/(FR1/fL) > 0.5` değerlendiriyor. Çizelge verisi için (FR1/fL = 0.54, 0.74…) doğru koşul çoğu satırda sağlanırdı; kodda yalnız 16. satırda (0.524/0.611²=1.40) sağlanıyor. Yani fiilen tüm tasarımlar donatılı; "lif donatının yerini alır" yolu kapalı.
- E:1132-1136: `hz = 100`, `a = 25`, `hsp = 75 mm` (EN 14651 numune boyutları). MRdu = fFtu·b·hsp²/2 kesit derinliğinden bağımsız.
- E:1097-1099: `Sigma1 = 6·Md/(b·h²)`: eğilme gerilmesi; "asal gerilme" için V kullanılmalıydı (τ ≈ 1.5·V/(b·h)).

**F-21 Malzeme (E:436-442, 1052).** `GetORebar("A615Gr60")` ve `GetMPUniaxial("A615Gr60")` sabit; Output.xml fsy = 413.685 MPa (60 ksi). Makale S420 (420 MPa). Tüm malzeme adları için (çelik adları dahil) döngü çalışıyor, ret kontrolleri kısmi (GetMPIsotropic ve GetOConcrete_1 ret'i kontrol edilmiyor, E:413, 428). `fckcy = 0.8·fc` (E:1052): Output.xml fckcy=29.6 → fc=37; `CMOD_data` ilk sütunu 37 ve 50. ETABS ACI kolon tasarımı aynı 37 MPa'yı f'c (silindir) sayar; küp (C30/37) ise kolon dayanımı %25 şişkin hesaplanır. EDB malzeme tanımı görülmeden kesin değil. ŞÜPHELİ.

### 2.2 Kısıt ve ceza

**F-26 Ceza yapısı** (E:682, 842-868, 969-970, 998-999):
- Toplam ceza P = (ETABS_print.Penalties.Max, yani gruplar arası maksimum) + max(kat ötelenme cezaları X,Y) + üst kat X cezası + üst kat Y cezası + max(kolon-kolon) + max(kiriş-kolon). Gruplar toplanmıyor: bir yapıda birden çok grup ihlal etse ceza yalnızca en kötüsü kadar.
- Penalized cost = Cost·(1+P)³ (E:682) makalede yok.
- Lifli kiriş cezası PenM+PenV+PenWd (oran−1, üst sınırsız); lifsiz kiriş cezası "hata içeren istasyon oranı" (0-1.5 arası, süreksiz). İkisi aynı ölçekte değil.
- g8/g9 (ρmin/ρmax): kodda As_ kırpılıyor (E:1161-1162); ihlal cezası yok. Kolon ρ sınırı ETABS'e bırakılmış ama ETABS sonucu F-01 nedeniyle kullanılmıyor.
- g6: kat ötelemesi cezası `max/limit − 1` (E:1541) makaleyle uyumlu; limit `h/R` (R=300) ve mutlak mm olarak (kN_mm_C).
- g7: üst kat (X ve Y) toplanıyor (E:866-867); makale tek δmax kısıtı.
- GlobalBest yalnız `Penalty = 0` ise güncelleniyor (O:874); ETABS "warning" cezası `DiscardWarnings` kapalıysa 0.5·ağırlık ile hiç sıfırlanmaz.

### 2.3 Maliyet

- Beton: `Σ A·L·1e-9` (m³) (E:916). Boyutlar mm olduğundan doğru (SetPresentUnits kN_mm_C, E:143, noktalar mm). Düğüm bindirmeleri sayılmıyor (çift sayım), kalıp yok (makale Denk. 1'de de yok).
- Birim fiyatlar: C30 49.19, C40 55.49 (form); C50 yok (F-05). Donatı 2.30 $/kg (form). Lif: 3.50 ve 4.75 $/kg `CostFiber` içinde sabit (E:928-934); makale "Lif Türü-2 ve Türü-3" diyor, kod 4/5 (3 → 2.25 ulaşılamaz dal, 6 → tanımsız).
- Lif maliyeti `dos·l_mal·BeamVolume` (E:936, 920-921): yalnız kiriş hacmi, doğru (Denk. 1 ile uyumlu).
- Çelik: `ΣSumReinforcement·7.85e-6 kg/mm³` (E:917) birim doğru; miktar hatalı (F-02, F-09).
- Beton sınıfı tek: `SelectedMaterial` tüm yapıya (kolonlar dahil) uygulanıyor (E:713-731); makale 3.4 "bazı kirişlerde C40" diyor (grup bazında sınıf yok).
- Maliyet hesabı `SelectedMaterial` global alanına bağlı (E:34, 923): tek evrensel durum, onarım yeniden çağrılarında değişir.
- "Kalem karışması/satır kayması" kaynağı kodda değil makale tablolarında (F-24); kodda döküm üreten fonksiyon yok, tablolar dışarıda derlenmiş.

### 2.4 ETABS OAPI kullanımı

- Birim: `SetPresentUnits(kN_mm_C)` (E:143) doğru yerde (noktalar okunmadan önce). Ec/fc/G `×1000` doğru (kN/mm² → MPa, E:414-415, 429, 438-443). Birim hatası ötelenmenin 50 kat küçük olmasını açıklamaz (mm/cm farkı 10, m/mm 1000).
- Model açma/kaydetme: orijinal EDB'yi doğrudan açıyor, her değerlendirmede `File.Save` (E:741) ile üzerine yazıyor; çalışma kopyası yok (F-08). Google Drive/OneDrive eşzamanlama ile çakışabilir.
- Kombinasyon seçimi: `InitilizeLoadCases` tüm `RespCombo`'ları bir kez seçiyor (E:386-395); lifli modda sonradan bozuluyor (F-03). Yük durumları (case) seçilmiyor, yalnız kombinasyonlar. Tasarım kombinasyonları (`DesignConcrete.SetComboStrength`) ayarlanmıyor (kullanıcı EDB'de otomatik oluşturduysa çalışır).
- İndeks/sıra kayması: `Groups` indisi ile tasarım vektörü indisi karışıyor (F-14). `BeamDesignGroupIDs`, `ColumnDesignGroupIDs` grup indislerini tutuyor; `Sect_Ind` önce kirişler sonra kolonlar, sonra malzeme. `GetNameList`'ten gelen grup sıralaması alfabetik; kiriş grupları adı "K..." (Kiriş), kolonlar "C/Kolon..." ise sıra tersine döner ve F-14 devreye girer. EDB grup adları görülemedi (örnek EDB ikili): ŞÜPHELİ.
- `FrameForce`/`JointDispl` ret kontrolleri: `Errorlogprint` sonrası devam (E:1011-1041), `JointDispl` hatasında çıkış (E:1658) ve eski `PointDisp` kalıyor.
- Performans: her nokta için ayrı `JointDispl` (E:1641-1662), her kiriş × her kombinasyon için `FrameForce` (E:1016-1040), `Points.ToList()` döngü içinde (O(n²)), `RB_Sections` tembel `OrderBy` (her erişimde yeniden sıralama).
- ETABS 22: `ProgramPath` ETABS 19 (E:94); `ETABSv1.dll` 19 hint'i. cHelper/CreateObject yolu v22'de çalışır ama referans DLL sürümü ile exe sürümü uyuşmazlığı kırılgandır; `PropMaterial.GetOConcrete_1`, `DesignConcrete.SetCode("ACI 318-14")` ve `GetSummaryResultsBeam/Column` imzaları v22 DLL'iyle derlenerek doğrulanmalı. ŞÜPHELİ.

### 2.5 Optimizasyon algoritmaları

- **Sınır kontrolü:** `Eval` her aday için `UBLBCheck` yapıyor (O:921), yani HS/Whale/Dandelion taşmaları kırpılıyor. VAO doğrudan `ETABSClass.Evaluate` çağırıyor ama öncesinde `UBLBCheck` var (O:679, 712). Üst sınırın dışlanması rastgele üretimde F-16.
- **Tür dönüşümü:** `Member.DesignVariables` Integer(); Option Strict Off (vbproj:60). `Math.Floor` Double'ı örtük daraltıyor; atamalarda `Floor` ve doğrudan atama (banker yuvarlaması) karışık (O:101, 147, 154, 177-179, 530). Aynı sezgisel adım farklı kesme biçimleriyle uygulanıyor.
- **YAKO (O:452-531):** tüm boyutlar bir anda değişiyor (standart YAKO tek boyut); gözcü arı olasılığı `0.1 + 0.9·GlobalBest/Memory(t)`; `GlobalBest` hiç uygun çözüm bulunmadıysa +∞ → `Prob = ∞` veya NaN; NaN ise `While True` döngüsü sonsuza gider. Kâşif arı sonrası `Trial` sıfırlanmaz, her turda yeniden tetiklenir (O:516-520).
- **BTO (O:111-130):** Lamda/Mu sıralı üyelere bağlı; `Memory` her turda sıralanıyor (M:27) ama tur içinde güncellemelerle sıra bozuluyor. `Roulette_wheel` `SelectIndex <= x.Length` sınırı bir fazla (`x(SelectIndex)` taşabilir, O:769-771).
- **ATO:** F-15.
- **VAO/İVAO:** F-17. Makale İVAO'yu "en kötü üyelerin elenmesi + açgözlü seçim" olarak tanımlıyor; kodda ayrı sürüm yok. Çizelge 6'daki İVAO lifli sonucu (donatı 1857 kg; diğerleri 2510-2562 kg, aynı kesitlerle) F-02, F-04, F-05 gibi değerlendirme açıklarının sömürüsü olabilir; yöntemin gerçek üstünlüğü kanıtlanmış sayılmamalı. ŞÜPHELİ.
- **Rastgele tohum:** F-18. `Rnd()` aynı dizi ⇒ BTO/HS/Whale/Dandelion koşuları (ETABS deterministik ise) birebir aynı. Çok koşulu istatistik (ortalama/std) bu yöntemlerde anlamsız.
- **MaxFEs sayımı:** F-19. `iter` Math testte +1/aday (O:938), ETABS'te `Setsection` başına +1 (E:736); onarım adımları `Setsection`'u tekrar çağırdığı için bir aday 1-4 sayıya mal olur. `Evaluate` içinde `iter_ = Iter` ByRef ile döner.
- **En iyi çözümün güncellenmesi:** `GlobalBestCheck` (O:864-901): `BestValue` ceza dahil en iyiyi tutuyor, `GlobalBest` yalnız P=0. `History` `BestValue`'ya göre (cezalı) yazılıyor; uygun olmayan çözümlerin maliyeti yakınsama grafiğine giriyor (Şekil 4.14-4.22 cezalı maliyet olabilir). Opt_Finalize `GlobalBest` yeniden değerlendiriyor (O:977): F-04 nedeniyle raporlanan maliyetle son kayıt farklı çıkabilir.
- **Yedek/devam:** `Backup_Read` göreli yol, RNG durumu yedeklenmiyor; yedeği yükleyince `FormInfo` üzerine yazılıyor (kullanıcının girdiği formdakiler kaybolur, sonra `FormInfo_Write`).

### 2.6 Sabit yollar, sürüm, global durum, yarış koşulu

- `Inputinfo.txt`: `E:\OneDrive - Akdeniz Üniversitesi\myprojects\...` (iki satır, EDB ve Output.txt).
- `E:94`: `Program Files\Computers and Structures\ETABS 19\ETABS.exe`; `Betornarme.vbproj:70`: `C:\Program Files\Computers and Structures\ETABS 19\ETABSv1.dll`. v22 için yeniden başvuru ve `.vbproj` güncellemesi gerekir. `bin\Debug\ETABSv1.dll` kopyası 19 sürümü.
- Global durum: `SelectedMaterial` (E:34), `SReinforcement` (E:36), `ret` (sınıf alanı, tüm alt fonksiyonlarda paylaşılıyor ve `InitilizeSlabArea`'da bile ezilir, E:265), `ETABS_print`, `Iter`; `Rnd()` süreç geneli. Tek iş parçacığı (UI), ama `Application.DoEvents` yeniden girişe izin veriyor (O:927, 688, 721).
- Eşzamanlı koşu: aynı klasörde ErrorLog.txt (E:1687), `BackUp.xml` (O:1001), orijinal EDB'ye kaydetme (E:741): iki örnek paralel çalıştırılamaz. ETABS ikinci örneği ayrı süreç olarak açar ama aynı EDB'yi kilitler.
- Birden çok ETABS sürümü: yalnızca bir yol; sürüm sorgusu yok (`SapModel.GetVersion`).

### 2.7 N/A kapasite oranları (özel bölüm → Bölüm 5)

---

## 3. Doğrulama eserleri

### 3.1 Output.xml sayısal kontrolleri (1 Nisan 2024, eski sürüm olabilir)

| Büyüklük | XML | Yeniden hesap | Sonuç |
|---|---|---|---|
| fckcy | 29.6 | 0.8·37 | fc=37 MPa |
| fctm | 2.8066 | 0.3·29.6^0.66 | Nesne başlatıcıda kendi alanına başvuru (`.fctm = …Fiber_RC_design.fckcy`) beklenen biçimde çalışıyor (VB yerinde atar) |
| fFts, fFtu | 1.215, 1.182 | 0.45·2.7; 1.215−0.6(1.215−1.7+0.54) | MC2010 5.6-4/5.6-5 ile uyumlu |
| I_ii | 837.3e6 | doğrusu 670.6e6 | F-11 |
| Vrd | 164 188 N | 0.18 ile yeniden üretildi | F-06 |
| Mrdu/Md | 181.6/96.06 | — | As aşırı (F-10) |
| fi_s, dd | 26, 25 | — | d tutarsızlığı (F-28) |
| `PMM_Ratio`, `Penalties` | tümü 0 | — | F-01 |
| TopStoryDrift_Ratio | 0.0947 mm; Y: 0 | — | 2B örnek (Y yönü 0), mm biriminde |
| Reinforcement (kiriş 6) | 3.03e6 mm³ | — | birim: mm³ |

### 3.2 Beton ve lif bedeli aritmetiği (makale)

Çizelge 4 lifli YAKO: 657.92 kg × 2.30 = 1513.2 ($, tabloda "Beton Maliyeti" satırında 1513.21); 3.15 m³ × 49.19 = 154.95 (tabloda "Lif Maliyeti" satırında); 110.25 = 31.5 kg × 3.50 (tabloda "Donatı Maliyeti" satırında); 31.5 kg = 1.05 m³ kiriş × 30 kg/m³ (kiriş 2 × 6 m × 0.35 × 0.25 = 1.05 m³). "Lif Miktarı = 94.5 kg" = 3.15 m³ (tüm beton) × 30 → tablodaki lif miktarı toplam beton hacminden, bedel yalnız kiriş hacminden. Çizelge 5 YAKO lifli: 1469.19 × 2.30 = 3379.2 ("Beton Maliyeti" satırı); 8.75 × 49.19 = 430.4 ("Lif Maliyeti" satırı). Çizelge 6 YAKO lifli: 2552.71 × 2.30 = 5871.2 ("Beton Maliyeti" satırı). Toplamlar etkilenmez (üç kalem toplamı aynı). Yalnız satır etiketleri ve lif kg değeri yanlış.
Ayrıca Çizelge 5 YAKO ve BTO lifsiz: 2876.39 × 2.30 = 6615.7 ✓; Çizelge 4 YAKO lifsiz: 843.23 × 2.30 = 1939.4 ≠ 2059.52 (toplam 2276.448 hücreleriyle tutarlı → 843.23 kg veya 2059.52 $ hatalı).

---

## 4. Ötelenmenin 0.1-0.2 mm çıkması: olası nedenler (önem sırasıyla)

Makalede Çizelge 6 (3B): lifsiz MKÖ 6.64-7.08 mm, lifli 0.13-0.24 mm; MEÜKÖ lifsiz 9.9-11.0 mm, lifli 0.10-0.20 mm. Kiriş kesitleri lifli tasarımda yalnız ~%0-40 küçülmüş, kolonlar aynı (25×25). Rijitlik bu kadar farkı açıklamaz (50 kat). Ayrıca lifli MEÜKÖ (0.10) < MKÖ (0.13): üst kat yer değiştirmesi katlar arası farktan küçük olamaz (aynı kombinasyonda). Bu çelişki ölçüm tutarsızlığını gösteriyor.

1. **Kombinasyon seçimi (DOĞRULANAN-kod):** Lifli modda `Cons_Fiber_RC_design` her kiriş × her kombinasyon için `DeselectAllCasesAndCombosForOutput` + `SetComboSelectedForOutput(MyName(i))` çağırıyor (E:1018-1023); döngü sonunda yalnız son kombinasyon seçili kalıyor ve geri yüklenmiyor. Sonraki `Penalty()` → `UpdateJointDisp` (E:1640) yalnız o kombinasyonu okur. İlk değerlendirmede tüm kombinasyonlar seçili (E:392-395), ötelenme doğru; sonraki tüm değerlendirmelerde yalnız son kombinasyon (kombinasyon listesinin son adı, çoğu modelde yanal yük içermeyen düşey kombinasyon veya tek rüzgâr kombinasyonu). Lifsiz yol (`ConsGetSummaryResultsBeam`) seçime dokunmuyor, bu yüzden lifsiz ötelenme normal çıkar. Bu, "lifli 0.1 mm, lifsiz 7 mm" kontrastını doğrudan açıklıyor.
   - Doğrulama (kod değiştirmeden): çalışma kopyasında `Results.Setup.GetComboSelectedForOutput` ile seçili kombinasyonları her `UpdateJointDisp` öncesi günlüğe yazmak; lifli koşuda 1 kombinasyon, lifsizde tüm kombinasyonlar görülmeli. Alternatif: optimum EDB'yi ETABS'te açıp `Display > Story Drifts` ile her kombinasyon için karşılaştırma (VALIDATION_SPEC, T1.1d).
2. **Yük yok:** Çizelge 5'te (2B 21 elemanlı) yalnız 1.4G+1.6Q kombinasyonu var; yanal yük yok, ötelenme simetri bozukluğundan ~0.1 mm olabilir (makale metni de bunu söylüyor). Bu durumda ötelenme kısıtı hiç bağlayıcı değil; ölçüm doğru ama anlamsız.
3. **Eski veri (DOĞRULANAN-kod):** `JointDispl` hatasında `Exit Sub` (E:1658) ve ErrorLog.txt'de tekrar eden "Problem occured on :Results.JointDispl" kayıtları var; `Points(i).PointDisp` önceki değerlendirmeden kalır veya hiç doldurulmamış (`Nothing` → NullReference).
4. **Yanlış kat/kombinasyon kümesi:** Üst kat noktaları `Zcoord = StructureHeight` tam eşitlikle (E:1555); eşleşme yoksa `Max()` hatası (sessiz değil). Katlar arası ötelenme yalnız kolon uç noktalarından (E:1528-1536). 3B'de kolonlarla kesişmeyen düğümler (döşeme düğümleri) yok sayılıyor. Düşük olasılık.
5. **Birim:** `kN_mm_C`; mm/cm/m farkı 10-1000 kat verir, 50 kat verecek hata yok. Düşük olasılık. Yine de `SetPresentUnits` ret ≠ 0 ise başlatma durur (E:143-144).
6. **ULS vs servis (F-25):** Tüm kombinasyonlar (ULS) okunduğundan lifsiz ötelenme büyük çıkar; lifli doğru okunmuş olsa servis farkı en fazla 1.4-1.7 kat; 50 kat değil.

Sonuç: neden büyük olasılıkla 1 (kod yolu kanıtlı); doğrulama yolu yukarıda. 

---

## 5. N/A kapasite oranları (lifsiz tasarım)

Makalede Çizelge 5-6 lifsiz sütunlarında MKKKO ve MKO "N/A". Kod okumasından olası nedenler:

1. **Oranlar hiç depolanmıyor (DOĞRULANAN-kod, ana neden):** ETABS beton kirişi için `GetSummaryResultsBeam` donatı alanlarını ve hata/uyarı metnini döndürür; kapasite oranı döndürmez. `ConsGetSummaryResultsBeam` (E:1427-1474) `Frame.Ratio_PMM`, `Ratio_Shear`, `Ratio_Crack` alanlarını hiç doldurmuyor (yalnız `Penalty` ve `Reinforcement`). Dolayısıyla lifsiz kirişler için oran daima 0 (veya raporlama sırasında N/A). Kolon için PMM oranı ETABS'ten okunuyor ama F-01 (ByVal) yüzünden kayboluyor; `Groups.PMMRatio` = 0 (Output.xml teyit).
2. **ETABS tasarım sonucu yok:** `Design()` her seferinde `SetCode("ACI 318-14")` + `StartDesign` çağırıyor (E:750-753). Tasarım kombinasyonları (`SetComboStrength`, `GetComboStrength`) hiç ayarlanmıyor; `Combinations_.DesignSteelStrength/DesignCompStrength` alanları boş. Kullanıcı EDB'de "Design Load Combinations"ı seçmediyse veya otomatik oluşturma kapalıysa ETABS "N/A" veya sonuçsuz döner; `GetSummaryResults*` `ret ≠ 0` verir, kod `ret = -1` yazıp `Exit Sub` (E:1446, 1494) ve Frame ceza=0, donatı=0 bırakır (sessizce "uygun ve bedava").
3. **Model kilitli/kilitsiz sırası:** `Setsection`/`Modifier_*` modeli kilitten çıkarır (sonuçları siler). `Modifier_PMM` içinde `Setsection → Analysis → Design → ConsPMM` sırası var, ancak `Penalty_PMM` sonrası yeniden tasarım/okuma tutarlı değil (F-04). Kilitsiz modelde `GetSummaryResults*` başarısız olur.
4. **Kolon donatı yapılandırması:** `SetRectangle` ile kesit her seferinde yeniden tanımlanıyor (E:729, 733). ETABS'te kolon kesitinin donatı verisi (`SetRebarColumn`/kapak/çubuk sayısı) ve tasarım tipi bu çağrıdan sonra korunmazsa `GetSummaryResultsColumn` N/A (veya varsayılan) döner. ŞÜPHELİ; doğrulama: tasarım sonrası `PropFrame.GetRebarColumn`.
5. **Tasarım prosedürü:** `FrameObj.GetDesignProcedure` okunuyor (E:231) ama `ConcreteFrameDesign` olduğu doğrulanmıyor; grup içinde "No design" veya "Program determined" varsa N/A.
6. **Lifli kirişlerde ETABS tasarımı zaten kullanılmıyor** ama `StartDesign` çalışıyor (zaman kaybı; kolonlar için gerekli).

Doğrulama (kod değiştirmeden): optimum EDB'yi ETABS'te aç, `Design > Concrete Frame Design > Start Design/Check`, `Display Design Info` ile N/A olan çerçeveleri ve Design Load Combinations seçimini kaydet (DEVELOPER_PROMPT T1.1c).

---

## 6. Önerilen onarım sırası (uygulama değil; öncelik)

1. F-01, F-02, F-03, F-04, F-05, F-06, F-07 (sonucu değiştirir; tablolar yeniden üretilmeli).
2. F-09/F-10/F-11/F-12/F-13 (lifli kapasite formülleri ve metraj).
3. F-14, F-16, F-17, F-15, F-18, F-19 (arama/yöntem adaleti).
4. F-08, F-20, F-22 (operasyonel güvenlik).
5. F-24 (makale tabloları): lifli sütunlarda satır etiketlerini düzelt; Çizelge 4 YAKO lifsiz donatı hücresi.

---

## 7. Uyarlama fizibilitesi: `SteelFamewithCompositeTubeColumn_FEM` tabanından betonarme (lifli/lifsiz) çerçeve programına

İncelenen: `README.md`, `YETENEKLER_RAPORU.md` (başlıklar + Bölüm 3, 4, 11, 12, 16), `KOD_INCELEME_RAPORU.md` başlıkları, `DEGISIKLIKLER.md` başlıkları, `Ajan/`, `ModelBuilder/README.md`, ve sınıf/fonksiyon imzaları (`StructureEvaluator.vb`, `ETABSClass.Backends.vb`, `FemCore/*.vb`, `OptimizationMethods.vb`, `ExcelExport.vb`). Dosyalar tam okunmadı, çalıştırılmadı.

### 7.1 Diğer programın profili (belgelerden)

- Çelik/kompozit kolonlu 3B çerçeve, ETABS 22 (22.6 ile test) veya 19; 16 metasezgisel yöntem; kendi FEM çözücüsü (~0.15-0.45 s/tasarım, ETABS 14-50 s); final ETABS doğrulaması (`_best.EDB`); AISC 360-22/341 tasarımı FEM içinde, kompozit kolon iç çözücü; çalışma kopyası `%TEMP%\SteelFrameOpt` (orijinale dokunmaz); durdur/devam, SHA-256'lı yedek, Excel çıktısı, toplu koşu betiği, `ModelBuilder` (CSV → ETABS modeli), `Ajan/` iş akışı.
- Doğrulama kayıtlı: yöntemler (ETABS'siz test, ilk 15 yöntem referansla birebir), FEM-ETABS farkı (D/C ort. ±%0.1; belirleyici üyelerde ≤ %1.4), ETABS yolu özgün programla birebir.
- Betonarme için doğrudan yok: beton kesiti, ACI 318, SFRC, donatı detayı, TBDY spektrumu (ModelBuilder ASCE 7-22 kullanıyor).

### 7.2 Yeniden kullanılabilir bileşenler

| Bileşen | Dosya/konum | Betonarme için durum | Çaba (tahmin) |
|---|---|---|---|
| Optimizasyon yöntemleri (16, parametre kataloğu, Levy, test problemi) | `OptimizationMethods.vb` (`MethodCatalog`, `InitMethodState`), `OptimizationClass.vb`, `SocialSpider.vb` | Doğrudan kullanılır; ETABS'siz doğrulanmış. Eksik: İVAO (VAO bulunmuyor: 16 yöntem listesinde yok) | İVAO ekleme: 2-3 iş günü |
| Değerlendirici arayüzü | `StructureEvaluator.vb` (`IStructureEvaluator`: AssignSections, Analyze, ReadDisplacements, ReadSteelDesign, ReadFrameForces) | Kullanılır; `ReadSteelDesign` → `ReadConcreteDesign` (oran + donatı), `ReadFrameForces` zaten var | 1-2 gün |
| ETABS arka ucu | `ETABSClass.Backends.vb` (`EtabsJointDisplacements`, `EtabsSteelDesign`), `ETABSClass.vb` (3189 sat.) | Çalışma kopyası, v22/19 yol keşfi, `SetPresentUnits`, kombinasyon seçimi (`DeselectAll…` sonra yalnız ihtiyaç duyulan kombinasyonlar, sonuç önbelleği), diyalog koruması (`DialogGuard.vb`), periyodik ETABS yeniden başlatma: v2'nin F-03, F-08, F-20, F-22 sorunlarının hazır çözümü | `EtabsConcreteDesign` yazımı: 3-5 gün |
| FemCore (çözücü) | `FemModel/FemStatic/FemModal/FemSeismic/FemPDelta/FemSkyline/FemElement` | Kesit sınıfı genel (`FemSection`: A, As2, As3, I22, I33, J, `Modifier()` 8 elemanlı); dikdörtgen beton kesit eklenir, çatlamış rijitlik (kirişte 0.35·Ig, kolonda 0.7·Ig, TBDY/ACI 6.6.3.1.1) `Modifier` ile verilir. Tasarım kısmı (`FemDesign.vb`) çelik özel, yeniden yazılacak | Beton kesit + modifier okuma: 3-4 gün |
| FemTool / EtabsReader | `FemTool/EtabsReader.vb` | Modeli bir kez okur; dikdörtgen kesit, beton malzeme, modifier, grup, kombinasyon, yükler okuma eklenmeli; alan elemanı (döşeme) FEM dışı: model yalnız çubuk olmalı veya yükler çubuğa verilmeli (v2 örnekleri çubuk modeli) | 4-6 gün |
| Excel çıktısı | `ExcelExport.vb` (`WriteResult`, `WriteCheck`): özet, grup bazlı maliyet dökümü, tasarım, bağlayıcı kısıtlar, yakınsama | Maliyet dökümü tek veri yapısından üretilirse F-24 türü satır kaymaları ortadan kalkar; kalem listesi beton/donatı/lif/(kalıp) olarak değişir | 2-3 gün |
| ModelBuilder | `ModelBuilder/*.vb`, CSV → ETABS modeli | Çelik W kesit havuzu ve ASCE 7-22 yükleri; betonarme için dikdörtgen kesit havuzu, malzeme, TBDY-2018 spektrumu/kombinasyonları (DD-2), 3 örnek çerçeve (6, 21, 42 eleman) gerekir. v2 EDB'leri kullanılacaksa gerekmez | 5-8 gün (yeniden üretim isteniyorsa) |
| Ajan akışı, toplu koşu | `Ajan/` (Gorevler, Hafiza, Yetenekler), `tools/RunBatch.ps1`, `Asama7_runs.csv` | Aynen: koşu kaydı, tohum yönetimi, çok tohumlu istatistik; `03_Code_Execution` içindeki DEVELOPER_PROMPT/VALIDATION_SPEC ile uyumlu | 1-2 gün |
| Dayanıklılık altyapısı | Durdur/devam, SHA-256 yedek, ErrorLog, çalışma kopyası | Aynen | 0 |
| FEM-ETABS doğrulama yöntemi | `FEM_SURUM_KAYITLARI.md`, `tools/calibration` | Aynı yöntem betonarme için tekrarlanır | WP7'de |

### 7.3 Yazılması gerekenler

1. **ACI 318-14 betonarme modülü (FemCore'a `ConcreteDesign.vb`):**
   - Kiriş eğilmesi: β1, εt-φ geçişi (0.65-0.90), ρmin (9.6.1.2), ρmax/εt ≥ 0.004, tek/çift donatı, ayrık çubuk seçimi (çap, adet, sıra); kesme Vc (22.5.5.1), Vs, Av,min (9.6.3.3), s sınırı, Vs ≤ 0.66√f'c·bw·d; burulma (gerekirse).
   - Kolon: P-M etkileşim eğrisi (Pn,max, φ(εt)), iki eksenli (Bresler/PCA kontur veya lif tabanlı), narinlik (6.2.5, P-Δ + 0.35/0.7 Ig ile), ρ 1-8%, minimum çubuk sayısı, etriye sıklığı ve kesme; sismik süneklik (TBDY-2018 / ACI 18): güçlü kolon-zayıf kiriş (18.7.3.2), sık sargı bölgeleri, birleşim kesmesi (18.8.4).
   - Detay ve metraj: donatı kg (boyuna + etriye çevre/aralık, bindirme/kanca payı), kalıp alanı (isteğe bağlı), her iki yol (lifli/lifsiz) için aynı tanım.
   - ETABS ACI sonuçlarına karşı kalibrasyon (YETENEKLER 16.3 benzeri).
2. **fib MC2010 SFRC modülü:**
   - Malzeme: fR1k/fR3k → fFts (5.6-4), fFtu (5.6-5), wu = min(εFu·lcs, 2.5), CMOD1/CMOD3; ortalama/karakteristik/tasarım ayrımı (γF = 1.5), ölçek ve yönlenme faktörleri; fLOP ve sünekliği sınıflandırma (FR1/fLOP > 0.4, FR3/FR1 > 0.5).
   - ULS: lifli+donatılı kesit eğilmesi (gerilme bloğu, dayanım azaltmaları), donatısız FRC koşulu; kesme VRd,F (γc dahil) + VRd,max; minimum donatı; çatlak genişliği (7.6, servis kombinasyonu ile); lif donatı yönlenmesi.
   - Lif verisi tablosunun kaynağı ve ortalama/karakteristik statüsü belgelenmeli (CMOD tablosu).
   - ETABS SFRC desteklemez: doğrulama el hesabı/ fib Bulletin örnekleri ile birim testleri (VALIDATION_SPEC).
3. **Maliyet ve kısıt katmanı:** beton sınıfı bazlı birim fiyat tablosu (C25-C50, tek kaynak), donatı, lif (tip × dozaj), kalıp (isteğe bağlı); makaledeki g1-g9'a bire bir kısıtlar; ceza fonksiyonunun makalede tanımlanması; servis kombinasyonları ile ötelenme/çatlak; ΣP ceza.
4. **Tasarım değişkeni kümesi:** makale 2.1.2'ye uyum (b, h, fck, çap, adet, lif tipi, dozaj) ya da makalenin kesit havuzuna uyarlanması.
5. **İVAO:** makaledeki "en kötü üyelerin elenmesi + açgözlü seçim" tanımına göre uygulama ve yöntem karşılaştırması için eşit bütçe/tohum.

### 7.4 İş paketleri (tahmini iş günü; tahmin, ölçüm değil)

| WP | İçerik | Bağımlılık | Süre |
|---|---|---|---|
| WP0 | v2'nin teşhis kopyasında (Sources dışında, `program_work/`) bulguları örnek koşuyla teyit: F-01/02/03/04/05; Çizelge 4-6 sayılarını ETABS'e karşı yeniden hesap | EDB'ler ve ETABS lisansı | 3-5 |
| WP1 | Yeni proje iskeleti (FEM sürümünden kopya), çelik/kompozit kodunun kaldırılması, v22 başvurusu | — | 3-4 |
| WP2 | ETABS okuma/ModelBuilder: beton malzeme, dikdörtgen kesit havuzu, modifier, grup, TBDY/ACI kombinasyonları, 3 örnek model (6/21/42 elemanlı) | WP1 | 8-12 |
| WP3 | ACI 318-14 modülü (kiriş + kolon + detay/metraj) ve birim testleri | WP1 | 15-25 |
| WP4 | fib MC2010 SFRC modülü, lif tablosu, birim testleri (el hesabı/fib bulletin) | WP1 | 10-15 |
| WP5 | Maliyet/metraj/kısıt/ceza katmanı (g1-g9, makaleyle tek tanım), Excel döküm | WP3, WP4 | 6-8 |
| WP6 | FEM çözücüde beton kesit ve rijitlik modifier; FEM-ETABS kalibrasyonu (ötelenme, iç kuvvet, ACI oranları) | WP2, WP3 | 8-12 |
| WP7 | ETABS doğrulama arka ucu: final tasarım ETABS ACI ile doğrulama, aday arşivi ve koruma adımları (diğer programdaki akış) | WP6 | 4-6 |
| WP8 | Yöntemler: İVAO uygulaması, tohum/bütçe, çok tohumlu koşu betikleri, yedek | WP5 | 5-8 |
| WP9 | Üretim koşuları ve Çizelge 4-6'nın yeniden üretimi, kıyas raporu | WP6-WP8 | 5-8 + hesaplama süresi |
| Toplam | | | ~65-105 iş günü (tek geliştirici, tahmin) |

Hesaplama süresi tahmini (diğer programın ölçümlerinden): FEM ile 10 000 değerlendirme dakikalar-saatler; ETABS ile aynı koşu (ETABS 14 s/tasarım, 84 elemanlı; v2 örnekleri daha küçük, 3-8 s tahmini) saat-gün; 3 örnek × 2 yol × 4 yöntem × ≥10 tohum = 240 koşu → ETABS ile pratik değil, FEM ile mümkün.

### 7.5 Seçenekler

- **Seçenek A: v2'yi onar ve ETABS 22'de yeniden koş (en kısa yol).** WP0 + F-01…F-24 onarımı + 3 örnek × 8 koşu. Süre ~15-25 iş günü kodlama + günler/haftalar ETABS koşusu (tahmin). Risk: kod tabanı kırılgan (global durum, onarım rastgeleliği, ETABS yavaş); çok tohum istatistiği zor; fib formülleri yine el ile yeniden doğrulanmalı. Makale tarihi baskın ise.
- **Seçenek B: FEM tabanına tam port (önerilen, kalıcı).** WP1-WP9. ETABS yalnız okuma ve final doğrulama; çok tohumlu istatistik ve duyarlılık (lif fiyatı, dozaj) mümkün; doğrulama kayıtlı bir süreçle. Süre ~65-105 iş günü (tahmin). Betonarme ACI modülünün ETABS ile uyuşması en büyük risk (YETENEKLER raporundaki çelik için %0.1-1.4 sonucu betonda garanti değil).
- **Seçenek C: Hibrit.** Kolon ve lifsiz kiriş tasarımı ETABS ACI'dan okunur (ACI modülü yazılmaz), yalnız SFRC kiriş modülü (WP4) ve maliyet/metraj yazılır; çözücü ETABS (yavaş) veya FEM analiz + ETABS tasarım. Süre ~25-40 iş günü (tahmin); hızlanma sınırlı (ETABS tasarımı her adımda gerekir).

Öneri: WP0'ı hemen yap (hangi makale sayısının hangi hatadan etkilendiğini ölç), makalenin zaman baskısına göre A veya C ile Çizelge 4-6'yı yeniden üret; uzun vadeli çalışmada (tez/ikinci makale) B'ye geç.

### 7.6 Riskler

- Lif verisi (fR1/fR3/fL) kaynağı ve ortalama/karakteristik statüsü belirsiz; 16 satırlı tablo makaledeki parametre kümesiyle (Tür-2/3, 20-40 kg/m³) birebir değil.
- Çizelge 4-6 sayıları mevcut hatalı kodla üretildiyse (F-01…F-07), yeniden üretilince "lifli %17-61 ucuz" sonucu değişebilir; makale bulguları (özellikle İVAO üstünlüğü, "donatı %68 azalma") yeniden doğrulanmadan yayımlanmamalı.
- ETABS'te lifli beton tasarımı yok: doğrulama yalnız el hesabı ve birim testleridir.
- ACI 318-14 ve TBDY-2018 birlikte kullanım kapsamı (makale ACI yük kombinasyonları + TBDY ötelenme) açık tanımlanmalı.

---

## 8. Ek: bilinmeyenler (çalıştırmadan çözülemeyenler)

- EDB içeriği: malzeme adları ("C30", "C40" veya "C50"), fc değerleri, grup adları (F-05, F-14, F-21), kolon donatı verisi, tasarım kombinasyonları.
- Makale sayılarının hangi program sürümüyle üretildiği (`ETABSClass.vb` 13.05.2024; `OptimizationClass.vb` 29.04.2025; Output.xml 01.04.2024).
- F-02 ve F-09'un çizelge sayılarına etki büyüklüğü.
- ETABS 22'de `ETABSv1.dll` 19 ile derlenmiş kodun davranışı.
