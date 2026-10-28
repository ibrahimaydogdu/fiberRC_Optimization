# WP0_ADAPTATION_MAP.md: Çelik/kompozit FEM tabanından lifli/lifsiz betonarme çerçeve optimizasyonuna uyarlama haritası

Tarih: 2026-10-27. Yol C (arama = FEM çözücü; ETABS v22 = örnek kurma/okuma + final doğrulama). Bu belge WP0 çıktısıdır.

## 0. Yöntem, sınırlar, işaretler

- **Salt-okunur inceleme.** Kod derlenmedi, çalıştırılmadı; hiçbir kaynak dosya değiştirilmedi. Tek çıktı bu dosyadır.
- Okunan taban: `D:\OneDrive Akdeniz Ogrenci\OneDrive - Akdeniz Üniversitesi\myprojects\Algoritmalar\MVS2010\ETABS\SteelFamewithCompositeTubeColumn_FEM` (kısaltma: **FS**). Belgeler (FEM_SURUM_KAYITLARI, PROGRAM_KURALLARI, YETENEKLER_RAPORU, ModelBuilder/README) tam; kod dosyaları atıf verilen aralıklarda okundu (ETABSClass.vb, KOD_INCELEME_RAPORU.md, DEGISIKLIKLER.md, KULLANIM_KILAVUZU.md, HybridColumns.vb, CompositeColumn.vb, ExcelExport.vb yalnızca başlık/imza ve ilgili bölümler düzeyinde tarandı).
- v2 kaynak: `...\Sources\Betonarme\Lİfli\v2\` (kısaltma **v2**); v2 bulguları `CODE_REVIEW_v2.md` (kısaltma **CR**) üzerinden, v2 kodundan yalnızca OAPI çağrı dökümü bizzat çıkarıldı.
- Her iddia şu etiketlerden birini taşır:
  - **[DK] doğrulanan-kod**: ilgili kod satırı bu incelemede okundu.
  - **[BD] belgeye dayalı**: depodaki kayıt/rapor söylüyor; kod satırı ayrıca okunmadı veya ölçüm başka kişiye ait.
  - **[ÇK] çıkarım**: gözlemden/mühendislik bilgisinden çıkarım; doğrulama gerekir. Yönetmelik sayıları (ACI/TBDY/fib) bu depodan değil genel bilgiden gelir, **şartname metniyle teyit edilmeden kullanılmamalıdır**.
- İş büyüklüğü S/M/L: **S** ≤ 3 iş günü, **M** 4–10, **L** > 10. Hepsi **ölçülmemiş tahmin** (CR §7.4 tahminleriyle aynı niteliktedir).

---

## 1. FEM tabanının mimarisi

### 1.1 Proje/çözüm yapısı

| Proje | Tür | Dosya (FS) | Not |
|---|---|---|---|
| `SteelFrameWithCompositeTubeColumns` (çıktı `FrameSap2000.exe`) | WinForms, .NET Fx 4.7.2 | `SteelFrameWithCompositeTubeColumns.vbproj:8-14`; FemCore'a `ProjectReference` `:206` | Optimizasyon programı (form, yöntemler, ETABS sınıfı) [DK] |
| `FemCore` | Kütüphane | `FemCore/FemCore.vbproj:8-13`; MathNet.Numerics 5.0.0 `:41-42` | ETABS'siz çözücü + çelik tasarım [DK] |
| `FemTool` | Konsol | `FemTool/FemTool.vbproj:8-14`, FemCore ref `:82` | `EtabsReader.vb` (model okuyucu) + `Program.vb` (`selftest/static/modal/rs/designcmp/calibdump`...) [DK/BD] |
| `ModelBuilder` | Konsol/Form | `ModelBuilder/*.vb` | CSV → ETABS 22 modeli [DK/BD] |

- `.sln` yalnızca ana program ve ModelBuilder'ı listeler (`SteelFrameWithCompositeTubeColumnsETABS.sln`); FemCore ana projeye referansla, FemTool ayrı bağlanır [DK].
- Yol/ayar: `App.config` (`Evaluator = Etabs|Fem`, `FemRatioMargin`, `FemArchiveSize`) [BD; `Backends.vb:32,288`].
- Büyüklük: `ETABSClass.vb` ≈3000 satır (+ `.Backends/.Aisc341/.Boxes/.Cache`), `FemTool/Program.vb` 1243, `OptimizationClass.vb` 778, `OptimizationMethods.vb` 677 satır [DK, boş olmayan satır sayımı].

### 1.2 FemCore sınıfları (dosya:satır)

| Dosya | İçerik |
|---|---|
| `FemModel.vb:6-168` | Veri modeli: `FemNode` (rijit diyafram indeksi, ek kütle), `FemSection` (E, G, A, As2, As3, J, I22, I33, **`Modifier(7)`**: alan, As2, As3, burulma, I22, I33, kütle, ağırlık; çelik alanları S/Z/R/Fy/Fu/NoDesign...), `FemElement` (uç mafsal, uç ofseti, rijit bölge, eleman `Modifier`, `Group`, `Story`), `FemPattern/FemCombo` (`Terms`, `RSTerms`, `Nonlinear`), `FemModel` (`AxialFactor/FlexuralFactor` DAM 0.8, `DesignCombos`, `Prefs`, `Notes`) |
| `FemElement.vb` | Timoshenko 12-DOF eleman, geometrik rijitlik, yoğunlaştırma, yük vektörleri (`LocalStiffness :10`, `GeometricStiffness :40`) |
| `FemChain.vb` | Uç ofsetli eleman zinciri (`Build :59`), `EffectiveSection :50-56` (section × object modifier uygular), `FaceForces :276` |
| `FemStatic.vb` / `FemSkyline.vb` | Doğrusal statik çözücü (`Solve :244`), profil (skyline) Cholesky + ters Cuthill-McKee; rijit diyafram (kat başına 3 DOF); paralel eleman döngüleri |
| `FemModal.vb` | Topaklı kütle (`ComputeMasses :93`, kütle `S.UnitMass·A·Mod(0)·Mod(6)` `:101`), alt uzay yinelemesi (`ModalSolve :180`), CQC tepki spektrumu (`ResponseSpectrum :263`), kazara dışmerkezlik, `FemSpectrum` (tablo değerli, `:10`) |
| `FemPDelta.vb` | `UseMassPDelta :11-24` (ETABS "Non-iterative Based on Mass": her kütleli düğümün iki yanal ötelemesine −m·g/z), `SolveIterative :41-83` (doğrusal olmayan P-Δ, tutarlı/topaklı geometrik rijitlik), `DesignNonlinear :87` |
| `FemEvaluator.vb` | Orkestratör: `AssignSection :63,113` (grup → kesit), `Analyze :120` (statik + modal + RS), `CombinationForces :170` (istasyon kuvvetleri), `Displacements :219` (± RS dahil), `Design :256`, `SeismicErrors :71` |
| `FemDesign.vb`, `FemDesignDriver.vb`, `FemSeismic.vb` | **Çelik özel:** AISC 360-22 (E/F/G/H), AISC 341 (SCWB, kompaktlık, Lb/ry). `FemDesignDriver.StationForces :23`, `RSStationForces :68`, `DesignElement :165` (zarf: `MemberRatios :4`) |

Hepsi [DK].

### 1.3 Veri akışı: model okuma → FEM → ceza → optimizer → final ETABS

1. **Model okuma (ETABS bir kez).** `EtabsReader.Read` (`FemTool/EtabsReader.vb:24-32`): birim `kN_m_C`; `ReadPoints :108` (koordinat, mesnet, `GetDiaphragm`), `ReadFrames :136` (`GetSection`, `GetModifiers`, `GetReleases`, `GetEndLengthOffset`, `GetTransformationMatrix`, `GetInsertionPoint`, **`GetGroupAssign` :194-195**), `ReadSection :202` (**yalnız I kesit**: `PropFrame.GetISection_1 :205`, aksi halde `Note("section ... is not an I section")`), `ReadPatterns :229`, `ReadCases :299` (statik/lineer + RespCombo düzleştirme), `ReadMassAndSpectra :53` (Mass Source tablosu, `Func.GetValues`, RS durumları; `ReadDesignSettings :35` → `DesignSteel.GetComboStrength` ve "Steel Frame Design Preferences - AISC 360-22" tablosu). Desteklenmeyen özellikler `FemModel.Notes`'a yazılır (alan elemanı, yay, mafsallı olmayan ofset...) [DK].
2. **Arka uç seçimi.** `ETABS_Class.SelectEvaluator` (`ETABSClass.Backends.vb:104-117`): `FormInfo.UseFem` ise `FemBackend.TryCreate` (`:310-348`) modeli okur, `Notes` boş değilse **ETABS'e geri döner**; şablon kesit çelik malzemeden (`:338-340`) [DK].
3. **Arayüz.** `IStructureEvaluator` (`StructureEvaluator.vb:27-41`): `AssignSections`, `Analyze`, `ReadDisplacements`, `ReadSteelDesign` (→ `GroupDesign` sınıfı `:5-11`), `ReadFrameForces` (kompozit kolon için istasyon kuvvetleri), `CompositeScwbRatio`. İki uygulama: `EtabsEvaluator` (`Backends.vb:121`), `FemBackend` (`:274`) [DK].
4. **Optimizer döngüsü.** `OptimizationClass.Main :45-64` → yöntem → `Eval :377-390` (`UBLBCheck`, `ETABSModel.Evaluate`, `GlobalBestCheck`, `MemoryUpdate`) → `ETABS_Class.Evaluate` (`ETABSClass.vb:1543`: önbellek anahtarı = onarım öncesi vektör `:1548-1557`) → `EvaluateCore :1620` → `EvaluateGroups :1631` → `SetAndAnalyze :1647` (`Backend.AssignSections` → `Backend.Analyze`; aynı vektörse analiz atlanır `:1649`) [DK].
5. **FEM analizi.** `FemBackend.AssignSections :452-467` (grup → `FemSection`, `O.Iter += 1` `:462`) → `Analyze :469` → `FemEvaluator.Analyze :120-139` (`UseMassPDelta` → `Solve` → `ModalSolve` → `ResponseSpectrum`); P-Δ'lı doğrusal olmayan kombinasyonlar tembel çözülür (`NonlinearSolution :142-156`, topaklı yedek) [DK].
6. **Ceza.** `Penalty :1955-1980`: `CombinedRepair :1984-1999` (kısıtlar → `RepairSteps :2013` en büyük adım → **tek yeniden analiz** → kısıtlar yeniden) veya sıralı `F_Evaluate_Drift :2063`/`G_Evaluate_PMM :2263`; kısıtlar: kat ötelemesi (`F1 :2088`), tepe (`F3 :2227`), PMM (`G1 :2336`), geometrik (`H :2465`); toplam `Ceza = max(kat) + tepe + max(PMM−1,0) + geometrik` `:1974-1979`; `PenalizedCost = Maliyet·(1+P)^3` `:1642` (`PENALTY_EXPONENT = 3 :19`, `FAILED_PENALTY = 10 :79`). Analiz başarısızlığı hata değil ceza (`PROGRAM_KURALLARI.md:203`) [DK].
7. **Maliyet.** `CostStProfile :2484-2510` ve `CostBreakdown :2513-2570` aynı miktar/fiyatları kullanmak zorunda (`PROGRAM_KURALLARI.md:289`); `CostItem_` (`Structures.vb:315-335`) kalemleri; `UnitCosts_` (`:202-212`: Steel/Rebar/Concrete/Formwork) [DK].
8. **Final ETABS doğrulaması.** `Opt_Finalize` (`OptimizationClass.vb:516-648`): en iyi tasarım FEM'de yeniden değerlendirilir `:538-545`, `SwitchToEtabs` (`Backends.vb:20-26`), ETABS'te **onarımsız** `Evaluate(applyRepair:=False)` `:549`; `FinalizeUnmodified :435-503`: en iyi + FEM aday arşivinden (`ArchiveFeasible Backends.vb:34-48`, en ucuz 20) en çok 10 aday ETABS'te maliyet sırasıyla denenir; ilk geçen son tasarım; geçen yoksa koruma adımları yalnızca teşhis (`StepUpViolations :58-95`); `_best.EDB` kaydı `:638`; Excel `Yazdir_Final :732` → `ExcelExport.WriteResult :30` [DK].
9. **Dayanıklılık.** Çalışma kopyası (`CreateWorkCopy ETABSClass.vb:279`), yedek/devam (`Backup_Write/Read OptimizationClass.vb:658-716`), sonuç önbelleği, `DialogGuard.vb`, ETABS yeniden başlatma (`RestartETABS :524`), `Clock(...)` zamanlama [DK/BD].

Doğrulama kaydı (çelik): FEM−ETABS statik/modal/RS %0.000–0.3, D/C ort. ±%0.1, belirleyici üyelerde ≤ %1.4 (`FEM_SURUM_KAYITLARI.md:33-35,76-80,106-112,179-192`) [BD]. **Betonarme için hiçbir doğrulama yoktur.**

---

## 2. Betonarme için eksik / uyarlanması gereken bileşenler

### 2.1 Eleman tipi ve kesit okuma (RC kiriş/kolon; T-kesit yok)

- **Var:** `FemSection` genel: A, As2, As3, J, I22, I33 (`FemModel.vb:18`); eleman matematiği malzemeden bağımsız [DK]. `FemSection.E` kesit başınadır (`:17`), yani beton sınıfına göre farklı E mümkün [DK].
- **Eksik:** (a) `EtabsReader.ReadSection :202-227` yalnız I kesit okur; dikdörtgen için `PropFrame.GetRectangle` (v2 kullanıyor, CR/v2 grep) + beton `PropMaterial.GetMPIsotropic/GetOConcrete_1/GetORebar` eklenmeli [DK, çözüm ÇK]. (b) `FemBackend.SectionOf :351-365` ve `Template` (çelik malzeme, `:338-340`) yerine **parametrik dikdörtgen kesit üreticisi**: b·h → A, `As2 = As3 = 5/6·A` (ÇK; ETABS dikdörtgen kayma alanı ile karşılaştırılacak), I22/I33, J (dikdörtgen burulma sabiti), E = f(fck) (ETABS beton malzemesindeki Ec ile uyum zorunlu), birim ağırlık 25 kN/m³ (ÇK). (c) Tasarım tarafı (`FemDesign*.vb`) çelik özeldir; `MemberRatios` zarfı ve `StationForces/CombinationForces` yeniden kullanılır, kapasite hesabı yeni modülde.
- **T-kesit yok:** döşeme çerçeve elemanı rijitliğine katılmaz; döşeme yükleri kirişe çizgisel/yamuk yük olarak verilir ve rijit diyafram kullanılır (ModelBuilder yöntemi: `ModelBuilder/README.md:71`; FEM alan elemanını desteklemez `YETENEKLER_RAPORU.md:294`) [BD]. Örnek modeller tamamen çubuk + diyafram olmalıdır; v2 örneklerinde `AreaObj` okunuyor (v2 grep), bu örneklerin çubuk modele çevrilmesi gerekir [ÇK].
- **ETABS tarafında lifli kiriş:** ETABS SFRC kiriş tasarımı desteklemez (CR §7.6) [BD]; lifli kirişin final doğrulaması = ETABS'ten alınan iç kuvvetler + kendi fib MC2010 modülü.

### 2.2 Çatlamış rijitlik (ACI 318-14 Tablo 6.6.3.1.1)

- **Mekanizma var:** `FemSection.Modifier(7)` ve `FemElement.Modifier(7)`; `EffectiveSection :52-55` ikisini çarpar; okuyucu ETABS'ten `PropFrame.GetModifiers` / `FrameObj.GetModifiers` alır (`EtabsReader.vb:152-154,221-223`) [DK]. Yani çatlamış rijitlik **yeni kod gerektirmeden** I22/I33 çarpanı olarak verilebilir.
- **Değerler (ÇK, şartname metniyle teyit edilecek):** kiriş `I = 0.35·Ig`, kolon `I = 0.70·Ig`, `A = 1.0·Ag` (Tablo 6.6.3.1.1(a)); servis düzeyi ötelenme için ayrı katsayı olup olmadığı ve TBDY-2018'in çatlamış rijitlik çarpanları şartnameden doğrulanmalı. Uç ofset/kolon-kiriş birleşim bölgesi rijitliği (rz) ETABS modeli ile aynı tutulmalı (FEM'de zincir modeli var: `FemChain.Build :59`).
- **Tuzaklar [DK]:**
  - `ComputeMasses :101` kütleyi `S.Modifier(0)·E.Modifier(0)` ile çarpar: alan çarpanı ≠ 1 yapılırsa kütle de değişir. RC için alan çarpanı 1.0 kalmalı (ACI zaten 1.0 önerir), kütle/ağırlık çarpanları 1.0.
  - **DAM (EA, EI × 0.8)** çelik özeldir: `FemChain.Build :85` çelik olmayan (`NoDesign=False`) kesitlere `AxialFactor/FlexuralFactor` uygular; değerler `Backends.vb:318` ile "Steel Frame Design Preferences" tablosundan gelir. RC modelinde `M.AxialFactor = M.FlexuralFactor = 1.0` zorlanmalı, aksi halde kiriş/kolon rijitliği ×0.8 azalır [DK + ÇK].
  - ETABS'in çatlamış modifier'ı ve FEM'in aynı olduğu V2'de sınanır (periyot, kat ötelemesi).

### 2.3 P-Δ ve narinlik

- **Var:** kütle esaslı P-Δ (ETABS ön ayarı birebir taklit, çelikte %0.000–0.3) ve yinelemeli doğrusal olmayan P-Δ (topaklı yedek) (`FemPDelta.vb`; `FEM_SURUM_KAYITLARI.md:65-93`); `FormInfo.PDelta` anahtarı (`Backends.vb:298`) [DK/BD]. Malzemeden bağımsız mekanizma olduğu için betonda da aynı ETABS ön ayarı varsa uyuşması beklenir [ÇK], ama **betonda ölçülmedi**.
- **Eksik:** ACI 318-14 §6.6.4 narinlik/büyütme (δns, δs; kolon kritik yükü Pc, `0.75` rijitlik azaltması vb.) FEM tasarım katmanında yoktur (çelikte B1 de yok: `FEM_SURUM_KAYITLARI.md:102`). ETABS ACI tasarımı kolonlarda bunu uygular [ÇK]. Karar gerekir (Açık Soru S-4): (a) P-Δ analizi + moment büyütmesi, (b) yalnız P-Δ analizi. V2 en büyük belirsizlik buradadır.

### 2.4 Kesit katalogları (b, h, donatı ayrık)

- **Tabandaki yapı [DK]:** katalog = `WSections` (AISC16M.xml'den, **alana göre artan sıralı**; `PROGRAM_KURALLARI.md:153`), `InitializeSections :1225/_ReadXML :1236/_ImportToModel :1297`, otomatik seçim listesi `EnsureAutoSelectList :1388`, sınır `Initialize_UBLB :1401`, süneklik süzgeci `InitializeDuctilityFilter :1721`. Onarım adımları `CatalogCount(v)·0.5·ln(oran)` kadar indeks ilerletir (`RepairSteps :2013-2036`, `DRIFT_LOG_MULTIPLIER = PMM_LOG_MULTIPLIER = 0.5`): **indeks artışı "daha güçlü" anlamına gelmek zorundadır**.
- **Yeni (S/M):** programatik katalog üreticisi: kolon `(b,h)` ve kiriş `(b,h)` ızgarası (adım örn. 50 mm; alt/üst sınırlar ve oran kuralları örnek bazlı; Kaveh&Sabzi 2011: kiriş h/b 1.5–2.5, kolon 1–2, adım 50 mm, 14×14 kiriş / 5×5 kolon veritabanı [BD, makale metni okundu]), ayrı **donatı düzeni kataloğu** (çap × adet × sıra; kolon çevresel dağılım; etriye çapı/aralığı) ve her kombinasyon için ön hesaplı kapasite/maliyet (Kaveh&Sabzi Tablo 1–2 yaklaşımı).
- **Sıralama tuzağı (ÇK):** alana göre sıralama 2B bir (b,h) kümesinde rijitlik açısından monoton değildir (aynı alan, farklı I). Onarım adımlarının "indeks artarsa güçlenir" varsayımı için sıralama anahtarı `I33` (veya EI) olmalı; eşitlikte alan/maliyet. Tabanda benzer sorun W kesitte "alana göre bir sonraki kesit daha sığ olabilir" olarak bilinir ve `StepUpViolations :69-77` `Imajor/Zmajor ≥ mevcut` süzgeciyle çözülmüştür [DK] → aynı desen RC'de `I33` ve kapasite süzgeciyle uygulanmalı.
- Geometrik kısıtlar (`GeoCons_ Structures.vb:168-171`, `InitializeGeometricCons :1321`, `H_Evaluate_GeometricPenalty :2465`): kolon–kolon (üst ≤ alt boyut) ve kiriş–kolon (kiriş genişliği ≤ kolon) RC'de yeniden tanımlanıp kullanılabilir ("Flange/Depth" → b/h) [ÇK].

### 2.5 Tasarım değişkeni kodlaması (lif dozajı, sınıf)

- **Tabandaki yapı [DK]:** `Member_.DesignVariables() As Integer` (`Structures.vb:299-305`), her değişken bir katalog indeksi; sınırlar `Lb/Ub` (hibritte `GUb/GLb`); rastgele üretim **üst sınır dahil** `Lb + Int((Ub−Lb+1)·Rnd())` (`OptimizationClass.vb:359-361`). Çok değişkenli grup için kurulu emsal: hibrit kolon (`HybridColumns.vb`; vektör `[NG grup değişkeni][...][tip değişkenleri]`, `GroupVector/DecodeHybrid/EncodeHybrid`, `PROGRAM_KURALLARI.md:83-100`; `EvaluateCore :1620-1629` vektörü gruba çevirir ve onarılmışı geri yazar).
- **Öneri (ÇK):** her kiriş grubu için ayrı tam sayı değişkenler `[kesit (b,h) indeksi, donatı düzeni indeksi, lif tipi indeksi, lif dozajı indeksi (0 = lifsiz)]`, her kolon grubu için `[kesit indeksi, donatı düzeni indeksi]`; beton sınıfı **global veya grup başına** değişken olarak ayrıca (Açık Soru S-6: makale 2.1.2). Lifli/lifsiz **tek kodlama ve tek hesap yolu** (dozaj 0 = lifsiz). v2'nin 16 satırlı birleşik tablo indeksi (CR F-27/F-05) bırakılmalı; fR1…fR4 tablosu (tip, dozaj, fck) üzerinden aranır (veri kaynağı WP2).
- Donatı ayrık değişken yerine "iç döngüde en ucuz yeterli düzen" seçimi (ETABS otomatik tasarımın v2 lifsiz yolu gibi) değişken sayısını düşürür ama maliyet tanımını tasarım modülüne bağlar (CR F-10 riski); seçim Açık Soru S-5.
- 3B 42 eleman, 7 grup → değişken sayısı ≈ 3 kolon × 2 + 4 kiriş × 4 = 22 (ÇK; yukarıdaki kodlamayla).

### 2.6 Kısıt ve ceza yapısı

- **Tabandaki yapı [DK]:** `Penalty :1955-1980` (bkz. 1.3-6). PMM kısıtı kesit oranı − 1; `GroupDesign` bir **sınıf** (`StructureEvaluator.vb:5`), yani değer atama referansla taşınır; ancak `Group_` bir **Structure** (`Structures.vb:140-155`): `Groups(i).PMMRatio` atamaları dizi elemanı üzerinde yapılmalı (liste elemanında ByVal kopya hatası olur; CR F-01'in mekanizması).
- **Yeni kısıtlar (ÇK):** g1 (Pu/φPn–Mu/φMn, kolon P-M), g2 (kolon kesme), g3 (kiriş eğilme φMn), g4 (kiriş kesme: ACI Vc+Vs / lifli: VRd,F γc dahil + VRd,max), g5 (çatlak genişliği, servis kombinasyonu), g6–g7 (kat ve tepe ötelemesi), g8–g9 (ρmin/ρmax; v2'de sessiz kırpmaydı), ACI Av,min/etriye aralığı, geometrik (CtoC, BtoC); R=8 (yüksek süneklik) ise güçlü kolon–zayıf kiriş (ACI 18.7.3.2 / TBDY) (S-3).
- **Ceza birleşimi:** tabandaki `max` tabanlı toplama (CR F-26'daki sorun: gruplar arası maksimum) RC'de **her kısıt ve grup için normalize ihlal toplamı** olarak tanımlanmalı ve makalede yazılmalı (S-7); üs 3 ve `FAILED_PENALTY = 10` korunabilir (`ETABSClass.vb:19,79`) [ÇK].
- **Ötelenme:** servis düzeyi sınırı için tabanda `DriftComboMode_.LateralCasesOnly` (`Structures.vb:219-223`) ve `SRV_<desen>` durumları (`EnsureServiceLateralCases :887`) var (CR F-25 çözümü) [DK/BD].

### 2.7 Grup mantığı (3B, 42 eleman: C1–C3, B1–B4)

- **Tabandaki yapı [DK]:** grup = ETABS grubu (`FrameObj.GetGroupAssign`, `EtabsReader.vb:194-195`); `FemElement.Group` (`FemModel.vb:55`); `FemEvaluator` grup→eleman sözlüğü (`:22-30,56-60`); değişken grupları = tasarım prosedürü *Steel Frame Design* olanlar (`PROGRAM_KURALLARI.md:158`); değişken indeksi ≠ grup indeksi, `SteelFrameDesignGroupIDs(v)` / `VarIndex(Name)` ile eşlenir, "`Sect_Ind(...)` dizisine asla grup indeksiyle erişilmez" (`:155-157`); bir çubuk iki grupta olamaz (`InitializeFrames` kontrolü).
- **Uyarlama:** değişken grupları "Concrete Frame Design" prosedürlü gruplar olmalı (`DesignProcedure_ Structures.vb:131`); `ETABSClass.vb` içindeki `SteelFrameDesign*` adlı alanlar yeniden adlandırılmalı (M). Kat bandı/grup adı örnek modele özgü (C1–C3 kolon, B1–B4 kiriş) → ModelBuilder'da grup adı ve eleman ataması kurulur [ÇK].
- **Eleman sayısı aritmetiği (ÇK, geometri doğrulanmalı):** 2B 6 eleman = 2 kat × 1 açıklık (4 kolon + 2 kiriş); 2B 21 eleman = **3 kat × 3 açıklık** (12 kolon + 9 kiriş), "3 kat 2 açıklık" 15 eleman verir (S-1); 3B 42 = 2 kat × (3×3 kolon aksı = 2×2 açıklık): 18 kolon + 24 kiriş.

### 2.8 Yük kombinasyonları

- **Tabanda:** kombinasyonlar ETABS'te tanımlıdır (ModelBuilder `Combos.vb:11-17`: `RespCombo.Add/SetCaseList`; ASCE 7-22 35 adet), FEM bunları okuyup düzleştirir (`EtabsReader.ReadCases :299-352`); `FemCombo.Terms/RSTerms`; RS sonucu büyüklük olarak ±eklenir (`FemEvaluator.CombinationForces :192-208`, `Displacements :238-248`) → "1.2G+Q±E" **tek RS kombinasyonu** ile iki işareti otomatik alır [DK]. Tasarım kombinasyonları `DesignSteel.GetComboStrength` (`EtabsReader.vb:37`) → RC için `DesignConcrete.GetComboStrength/SetComboStrength` (ÇK).
- **Yeni (S):** ModelBuilder'a ACI/TBDY kombinasyon seti: `1.4G+1.6Q`, `1.2G+Q±E`, `0.9G±E` (ve gerekirse 100%+30% ortogonal). Not: `1.4G+1.6Q` TS 500'e özgü; ACI 318-14 5.3.1 karşılığı `1.4D` ve `1.2D+1.6L` (+1.0E'li kombinasyonlar) (S-2) [ÇK].
- **Servis kombinasyonları** (çatlak genişliği g5 ve ötelenme): ayrı tanım gerekir (v2'de yoktu, CR F-11/F-25).

### 2.9 RS / ELF depremi (Ö1: DD-2, S_DS = 1.2, S_D1 = 0.3, R = 8)

- **Var [DK/BD]:** FEM spektrumu ETABS fonksiyonundan tablo olarak okur (`EtabsReader.vb:77-82`), RS durumu ölçek çarpanı/sönüm/dışmerkezlik ile (`:84-100`), modal + CQC + kazara dışmerkezlik %0.2 uyumlu (`FEM_SURUM_KAYITLARI.md:55-60`). ModelBuilder ASCE 7-22 fonksiyonu + `Ie·g/R` ölçeği ile RS kurar, ELF kesmesini yalnızca ölçekleme için hesaplar (**ELF deseni üretmez**; `README.md:83`). RS ölçek çarpanı tasarımdan bağımsız sabittir (`YETENEKLER_RAPORU.md:279`).
- **Eksik (RC için):**
  1. **TBDY-2018 spektrum fonksiyonu** (yatay elastik spektrum; DD-2 için S_DS = 1.2, S_D1 = 0.3 ⇒ T_A = 0.2·S_D1/S_DS = 0.05 s, T_B = S_D1/S_DS = 0.25 s [ÇK, formül şartnameden doğrulanmalı]) ve R/D/I azaltması (Ra(T)); ASCE 7 biçimi yalnız R ile böler (S-3).
  2. **ELF** istenirse FEM'de yok: T1 FEM modalinden (`ModalSolve`), Vt = kütle × S_aR(T1), kat kuvvetleri (+ΔF_N) yanal desen olarak `FemPattern.NodalLoads` ile üretilir (`NodalMass FemModal.vb:144` kütleleri verir) [ÇK]. İki işaretli ELF için ayrı ± kombinasyon gerekir (RS'te otomatik).
  3. RC'de rijitlik tasarımla çok değiştiğinden RS taban kesme alt sınırı ölçeklemesi (TBDY/ASCE) **her değerlendirmede yeniden** hesaplanmalı; tabandaki "sabit çarpan" yaklaşımı yetmez (`YETENEKLER_RAPORU.md:279`) [ÇK].
  4. Kütle kaynağı (G + nQ) `Mass Source Definition` okuması hazır (`EtabsReader.vb:53-75`) [DK].

### 2.10 Maliyet fonksiyonu

- **Tabanda [DK]:** `CostStProfile`/`CostBreakdown` (bkz. 1.3-7); `CostItem_` kalemleri (çelik/donatı/beton/kalıp), `UnitCosts_` (göreli birim fiyat). Çelik çubuk `G.GroupLength × alan × birim ağırlık`; kompozitte donatı kg/beton m³/kalıp m² hazır (`:2496-2501`).
- **Yeni (M):** `Maliyet = Σ_grup [beton(sınıf) × hacim + donatı kg × fiyat + (kiriş) lif kg × fiyat + (ops.) kalıp m²]`; lif kg = dozaj (kg/m³) × **yalnız lifli kirişin beton hacmi**; donatı kg tek fonksiyondan (boyuna + etriye çevre/aralık + kanca/bindirme payı), lifli/lifsiz **aynı tanım** (CR F-09). Birim fiyatlar tek tabloda (beton sınıfı → fiyat, lif tipi → fiyat; ÇŞİDB 2026, WP4); **tablo eksikse hata** (CR F-05). `CostItem_`'a lif kg/maliyet alanı eklenir (`Structures.vb` alanı silmek yedekleri bozar, eklemek güvenli: `PROGRAM_KURALLARI.md:296`). `CostBreakdown` aynı veri yapısından üretilirse F-24 türü satır kayması olmaz.

---

## 3. Yeniden kullan / uyarla / sıfırdan yaz

| Bileşen | Karar | İş (ölçülmemiş tahmin) | Gerekçe/Not |
|---|---|---|---|
| FemCore: veri modeli, `FemElement`, `FemChain`, `FemStatic`, `FemSkyline`, `FemModal`, `FemPDelta` | **Yeniden kullan** | S | Malzemeden bağımsız; `Modifier` hazır. DAM'ı RC için 1.0'a zorlama [DK] |
| FemEvaluator (`Analyze`, `Displacements`, `CombinationForces`) | **Yeniden kullan** | S | Çelik tasarım çağrısı `Design :256` ayrılır |
| `FemDesign.vb`, `FemDesignDriver.DesignElement`, `FemSeismic.vb` | **Çıkar / bırak** | — | Çelik özel (AISC); `StationForces/RSStationForces` yeniden kullanılır |
| `EtabsReader` | **Uyarla** | M | Dikdörtgen kesit + beton/donatı malzeme + `DesignConcrete.GetComboStrength`; alan elemanı notu |
| `FemBackend` (`SectionOf`, `TryCreate`, `ReadSteelDesign`) | **Uyarla** | M | `FemRcBackend`: parametrik kesit, çelik şablon bağımlılığını kaldır |
| `IStructureEvaluator` | **Uyarla** | S | `ReadSteelDesign` → `ReadConcreteDesign` (+ lifli kiriş oranı), `ReadFrameForces` zaten var |
| `ETABS_Class` (çalışma kopyası, v22 yol keşfi, `SessionSettings`, kombinasyon seçimi, `DialogGuard`, önbellek, yedek) | **Yeniden kullan** | S | CR F-03/F-08/F-20/F-22 çözümlerini taşır |
| `ETABS_Class` çelik katmanı (W kataloğu, AISC 341, tüp/kutu/kompozit, hibrit) | **Çıkar + uyarla** | L | ≈3000 satır çelik bağımlı; fork-ve-buda önerisi (CR §7.4 WP1) |
| Kısıt/ceza/onarım iskeleti (`Penalty`, `CombinedRepair`, `RepairSteps`, `StepVariable`, `H_Evaluate`) | **Uyarla** | M | Kısıt kümesi ve katalog semantiği değişir; mekanizma korunur |
| Optimizasyon yöntemleri (16, parametre kataloğu, Levy, tohum) | **Yeniden kullan** | S | ETABS'siz doğrulanmış (ilk 15 yöntem referansla birebir `YETENEKLER_RAPORU.md:254`) |
| VAO (Vahşi At) | **Sıfırdan yaz** (karar bekliyor) | S–M | 16 yöntemde yok; bkz. §5 |
| Final doğrulama protokolü (aday arşivi + `FinalizeUnmodified` + koruma) | **Uyarla** | M | RC'de "koruma adımı" = b/h veya donatı büyütme (I33 ve kapasite süzgeci) |
| Excel (`ExcelExport`) | **Uyarla** | S–M | Kalem listesi beton/donatı/lif/(kalıp); tek veri yapısı |
| ModelBuilder | **Uyarla/sıfırdan** | M–L | Dikdörtgen kesit, beton/donatı malzeme, TBDY spektrum, ACI/TBDY kombinasyonu, 3 örnek model (v2 EDB'leri varsa yalnız çubuk modele dönüştürme) |
| ACI 318-14 kiriş/kolon/detay/metraj modülü | **Sıfırdan** | L | WP1; saf kütüphane (CompositeColumn.vb emsali, ETABS'siz) |
| fib MC2010 SFRC modülü (σ-w, fFts/fFtu, VRd,F, w_k, As,min) | **Sıfırdan** | L | WP2; fR1…fR4 yayınlanmış veri |
| Kolon P-M etkileşimi (iki eksenli), narinlik/büyütme | **Sıfırdan** | M–L | FEM'de yok |
| Çatlamış rijitlik | **Yapılandırma** (kod yok) | S | `Modifier` ile; V2'de ETABS'le uyum |
| TBDY spektrum + ELF üreticisi | **Sıfırdan** | S–M | |
| Maliyet katmanı + birim fiyat tablosu | **Sıfırdan/uyarla** | M | WP4 |
| ETABS RC final doğrulama (`DesignConcrete` okuma, tablo okuma) | **Sıfırdan** | M | §6 |
| Koşu altyapısı (`RunBatch.ps1`, `Asama7_runs.csv`, Ajan akışı) | **Yeniden kullan** | S | |

---

## 4. v2'nin 8 kritik hatası ve yeni tasarımda önlenmesi

Not: CR'de **F-01…F-07 "KRİTİK"**, F-08 "YÜKSEK"tir (toplam 7 kritik + F-08); planın "8 kritik" ifadesi F-01…F-08 olarak ele alındı. F-09…F-36 için kısa kural §4.2'dedir.

### 4.1 F-01…F-08

| CR | Hata (v2) | Yeni tasarım kuralı | Test |
|---|---|---|---|
| F-01 | Kolon PMM oranı/cezası hiç taşınmıyor (`ByVal` struct; ceza hep 0) | Oran/ceza **sınıf (ref tip) veya dönüş değeri** ile taşınır; `Group_` struct olduğundan (`Structures.vb:140`) dizi elemanı üzerinden atanır (tabanda `GroupDesign` sınıfı `StructureEvaluator.vb:5` emsal); kolon g1/g2 her değerlendirmede hesaplanır; değer yoksa **hata** | Bilerek yetersiz kolon → ceza > 0 ve oran > 1; tüm gruplarda `PMMRatio ≠ 0` iddiası; ETABS'te aynı kolon "O/S" ⇒ FEM oranı > 1 |
| F-02 | Kolon grupları donatıyı kümülatif topluyor (`SReinforcement` kolon döngüsünde sıfırlanmıyor) | Maliyet yalnız **grup yerel** değişkenlerle; sınıf düzeyinde birikimli alan yok (tabandaki `CostStProfile` yerel `StructureWeight`: `ETABSClass.vb:2485-2486` [DK]) | Σ(grup kalemleri) = bağımsız yeniden hesap; `CostBreakdown` toplamı = `CostStProfile` (kural `PROGRAM_KURALLARI.md:289`); grup sırası permütasyonu maliyeti değiştirmez |
| F-03 | Lifli yolda kombinasyon seçimi bozuluyor, ötelenme yalnız son kombinasyondan | FEM'de çıktı seçimi durumu **yok**: `Fem.Displacements(Current, nm)` ada göre (`Backends.vb:493`) [DK]; ETABS arka ucunda her `JointDispl` öncesi seçim **açıkça** kurulur ve seçili küme günlüğe yazılır | Aynı tasarımda ötelenme: lifli ve lifsiz yol aynı hesaplayıcıdan; FEM ↔ ETABS final ötelenme farkı ≤ %2; lifli ötelenme ≈ lifsiz mertebesinde (50× fark = alarm) |
| F-04 | `Modifier_PMM` kiriş vektörünü rastgele büyütüyor, yeniden analiz yok, onarım `Rnd()` | Onarım **belirleyici** (log-adım formülü, rastgelelik yok) ve **her onarım sonrası yeniden analiz + kısıtlar yeniden** (tabanda `CombinedRepair :1984-1999` ve kural `PROGRAM_KURALLARI.md:196-198`) [DK]; maliyet ve ceza son vektörden | Onarım sonrası `Cost(vec)` yeniden hesapla = raporlanan; aynı vektör/aynı tohum ⇒ aynı sonuç (iki koşu karşılaştırması) |
| F-05 | Maliyet beton adlarına bağlı (C50 bedava, lif tipi 6 bedava, malzeme güncellenmiyor) | Sınıf/lif → fiyat **tek tablo**; tabloda olmayan sınıf/tip = **hata (fail-fast)**; global `SelectedMaterial` yok, malzeme tasarım vektöründen türetilir | Katalogdaki her (sınıf, lif tipi, dozaj) için fiyat > 0 testi; maliyet monotonluğu (dozaj↑ ⇒ maliyet↑ aynı kesitte) |
| F-06 | fib kesme `0.18` (γc eksik), ortalama dayanımlar | `ConcreteFib` modülünde γc, fctk/fFtuk, VRd,max ve birim testler (WP2); formül tek yerde | fib Bulletin / elle hesap vektörleri; CR §F-06 sayıları (b·d = 250×325, 0.12 ve 0.18 farkı) regresyon vektörü olarak |
| F-07 | Eğilme nominal (φ yok), karma TS500/MC2010 Asmax, "???" β | ACI 318-14: β1, εt–φ geçişi, ρmin, εt ≥ 0.004 sınırı; lifli kesitte fib bloğu + γ; kapasite = **tasarım** değeri | Elle ACI vektörü: b=250, h=350, d=325, As=1605, f'c=29.6, fy=413.7 ⇒ a = 105.6 mm, Mn = 180.7 kN·m (CR §F-07); **φ**: CR φ=0.9 almış, oysa εt = 0.0047 < 0.005 ⇒ φ ≈ 0.878, φMn ≈ 158.7 kN·m (ÇK, el hesabı) → test vektörü şartnameyle teyit edilerek kullanılmalı |
| F-08 | Orijinal EDB üzerine kaydediliyor; uygun çözüm yoksa yanlış üye değerlendirilip `Best` ile uyumsuz | Çalışma kopyası (`CreateWorkCopy :279`, `E3` artık `File.Save` yapmaz `PROGRAM_KURALLARI.md:191,276`) [DK/BD]; "uygun çözüm yok" → açık hata/uyarı, `GlobalBest.PenalizedCost = ∞` korunur (`Opt_Finalize :522-524`) [DK]; final dosya adı `<model>_best.EDB` ayırıcılı (`:638`) | Orijinal EDB SHA-256 koşu öncesi = sonrası; uygun çözüm bulunmayan sentetik koşu → "feasible yok" bildirimi, dosya yazılmaz |

### 4.2 Diğer önemli v2 hatalarının tasarım karşılıkları (özet)

- **F-09/F-10/F-12/F-13** (metraj ve kapasite tutarsızlığı, As tek adım ölçek, kesme hep ≤ 1, donatısız yol): lifli/lifsiz tek hesap yolu; As **ayrık katalogdan** seçilir (sürekli ölçekleme yok); kesme için Vs üst sınırı ve Av,min zorunlu; donatısız FRC koşulu `FR1/fL > 0.4` ve `FR3/FR1 > 0.5` yalnız yönetmelik biçiminde.
- **F-11/F-25** (çatlak, servis): servis kombinasyonu ve I_ii/σs doğru formül; limit parametre.
- **F-14/F-23** (indeks karışması): `VarIndex` sözlüğü, `Sect_Ind` asla grup indeksiyle erişilmez (`PROGRAM_KURALLARI.md:155-157`) [DK].
- **F-15/F-17** (ATO/VAO hatalı): taban yöntemleri yeniden kullanılır; VAO sıfırdan literatüre göre (§5).
- **F-16** (üst sınır üretilmiyor): `RandomVariable` üst sınır dahil (`OptimizationClass.vb:359-361`) [DK].
- **F-18** (tohum): `MainForm.SetRandomSeed`, "yeni kodda `New Random()` yok" (`PROGRAM_KURALLARI.md:270-272`) [BD]; ABC/TSA kodları `Rnd()` kullanıyor [DK, `OptimizationMethods.vb:182,327,589-598`].
- **F-19** (bütçe): tabanda sayaç `O.Iter` her `AssignSections`'ta artar (`Backends.vb:462`), önbellek isabetleri saymaz (`PROGRAM_KURALLARI.md:192`) [DK/BD] → "analiz bütçesi" (onarım dahil). Yöntemler arası adil karşılaştırma için eşit **analiz** bütçesi veya eşit **aday** bütçesi seçilip makalede tanımlanmalı (S-8).
- **F-20** (sessiz hata): API hatası → ceza/uyarı, `ret ≠ 0` yukarıya (`PROGRAM_KURALLARI.md:199-203`).
- **F-21/F-22** (malzeme adı sabit, ETABS 19 yolu): malzemeyi modelden oku, birimi/`fc` tanımı tek yerde; yol keşfi `FindInstalledETABS :353` (v22/19) [DK].
- **F-24** (makale tablosu satır kayması): tüm tablolar `CostItem_` tek veri yapısından otomatik.

---

## 5. Optimizasyon yöntemleri: tabanda neler var, v2 ile eşleşme

| v2 (Türkçe) | Taban karşılığı | Dosya:satır | Parametreler (varsayılan) |
|---|---|---|---|
| YAKO (ABC) | `Main_ArtificialBeeColony` | `OptimizationMethods.vb:297-337`; katalog `:52-54` | `Limit` (0 = koloni × değişken), `MR` (0 = tek değişken; >0 değişim oranı); gözcü arı rulet, kâşif arı (opsiyonel Levy) |
| BTO (BBO) | `Main_BioGeographyBased` + `Init_BioGeographyBased` | `OptimizationClass.vb:111-131`, `:37-44`; katalog `OptimizationMethods.vb:48` | `MutationRate` 0.1; `Mu_i = (N−i)/N`, `Lamda = 1−Mu`, rulet |
| ATO (TSA, Tree-Seed) | `Main_TreeSeed` | `OptimizationMethods.vb:586-609`; katalog `:72-73` | `ST` 0.1, `SeedsMin` 2, `SeedsMax` 5 |
| HS | `Main_HarmonySearch` | `OptimizationClass.vb:65-110`; katalog `OptimizationMethods.vb:46-47` | `PAR` 0.6, `HMCR` 0.9, tür Static/Dynamic/Adaptive |
| WOA, DO, ACO, BSO, CSA, FA, GOA, TLBO, GWO, HBA, AO, SSO | var (16 yöntem) | `Structures.vb:268-285` | `OptimizationMethods.vb:46-84` |
| **VAO / WHO / İVAO (Vahşi At)** | **YOK** | — | 16 yöntemin hiçbirinde bulunmuyor [DK]; v2'deki `WildHorses` (CR F-17) taşınmaz |

- Ortak parametreler: `MemorySize`, `MaxFuncEvaluation`, `MemoryUpdateType` (6 seçenek, `Structures.vb:286-293`), `ClearDuplicates`, `LevyFlight` (`OptInfo_ :242-254`) [DK]. Tabandaki yeni yöntemler (ABC/TSA...) sürekli konumu `ToMember` ile **yuvarlayıp sınırlar** (`:164-172`) ve `EvaluateOnly/EvalAt` kullanır (formdaki Memory update bu yöntemlerde kullanılmaz: `PROGRAM_KURALLARI.md:261`).
- **Eşleşme uyarısı:** v2 YAKO tüm boyutları bir anda değiştirir (CR F-... YAKO notu); tabanda `MR = 1` bu davranışa yaklaşır, `MR = 0` standart tek boyutlu ABC'dir. v2 hatalarını (CR F-15, F-17, F-29) taşımamak için taban yöntemleri **literatür sürümüyle** kullanılmalı; "v2 ile eşleşme" değil "v2 sonuçlarıyla farkın nedeni" (WP5) belgelenmelidir.
- VAO: tabanda yok; plan kararı (WP0 sonrası, `PLAN_PATH_C.md:30`): FEM tabanında hazır olmadığı için dahil etmek **S–M** iş (ÇK, ölçülmedi), İVAO ayrı makale. İndeks tabanlı ayrık kodlamada ABC/TSA/BBO doğrudan çalışır; sürekli konumlu yöntemler `ToMember` yuvarlaması kullanır.
- RC'ye özgü: indeks anlamlılığı (§2.4 sıralama) ve **çok değişkenli grup** yapısı (donatı, lif) yöntemlerin komşuluk adımlarını (±1 indeks) anlamlı kılmak için gerekli; lif dozajı/lif tipi değişkenleri sıralı olmalı.

---

## 6. ETABS v22 arka ucu: gereken OAPI çağrıları

Referans dosya: `...\Sources\Betonarme\Lİfli\v2\CSI API ETABS v1.chm` **mevcut** (3 215 699 bayt, dosya tarihi 2021-03-18). Tarih ETABS v22'den eskidir; v19/v20 dönemine ait olabilir (ÇK). Bu belge CHM'yi **okumadı**; v22 imzaları DLL'e karşı derlemeyle doğrulanmalıdır.

| Amaç | Çağrı | Kanıt |
|---|---|---|
| Örnek aç/kaydet, çalışma kopyası | `File.OpenFile`, `File.Save`, `File.NewBlank` (ModelBuilder), `SetModelIsLocked(False)` (kilitli modelde `SetSection`/tablo etkisiz) | v2 grep; `PROGRAM_KURALLARI.md:231-233`; `FEM_SURUM_KAYITLARI.md:171` [DK/BD] |
| Süreç/oturum | `StartInstance/ExitInstance/RestartETABS/SessionSettings/DialogGuard`; birim `SetPresentUnits(kN_mm_C)` | `ETABSClass.vb:424,500,524,453`; `DialogGuard.vb` [DK] |
| Geometri/yük okuma (FEM) | `PointObj.GetNameList/GetCoordCartesian/GetRestraint/GetDiaphragm`; `FrameObj.GetNameList/GetPoints/GetSection/GetModifiers/GetReleases/GetEndLengthOffset/GetTransformationMatrix/GetInsertionPoint/GetGroupAssign`; `LoadPatterns`; `LoadCases.StaticLinear.GetLoads/ResponseSpectrum.GetLoads/ModalEigen.GetNumberModes`; `RespCombo.GetNameList/GetCaseList`; `Func.GetValues`; tablolar: `Mass Source Definition` | `EtabsReader.vb` [DK] |
| RC kesit/malzeme okuma | `PropFrame.GetRectangle`, `GetSectProps`, `GetModifiers`; `PropMaterial.GetMPIsotropic`, `GetOConcrete_1`, `GetORebar`, `GetWeightAndMass` | v2 grep [DK] (v22'de aynı imza ÇK) |
| Örnek kurma (ModelBuilder) | `PropMaterial.SetMaterial/SetMPIsotropic/SetOConcrete_1/SetWeightAndMass/SetMassSource_1`, `PropFrame.SetRectangle`, kesit modifier (`PropFrame.SetModifiers`/`FrameObj.SetModifiers`), `FrameObj.AddByCoord/SetGroupAssign/SetLoadDistributed`, `PointObj.SetRestraint/SetDiaphragm/SetLoadForce`, `Story.SetStories_2`, `Diaphragm.SetDiaphragm`, `Func` (RS fonksiyonu), `LoadCases.ModalEigen`, `RespCombo.Add/SetCaseList`, `DesignConcrete.SetCode` + `SetComboStrength`, `DatabaseTables.*` | ModelBuilder çağrı dökümü (çelik karşılıkları) [DK]; RC karşılıkları ÇK |
| Donatı verisi (kolon/kiriş) | `PropFrame.SetRebarColumn/SetRebarBeam` (v2/`PROGRAM_KURALLARI.md:227` `SetRebarColumn` anılıyor) | [BD/ÇK]; v22 imzası doğrulanmalı |
| Analiz ve sonuç | `Analyze.RunAnalysis` (taban: ETABS sürecinde `SetSolverOption_3`), `Results.Setup.DeselectAllCasesAndCombosForOutput/SetComboSelectedForOutput/SetCaseSelectedForOutput`, `Results.JointDispl`, `Results.FrameForce`, `Results.BaseReact`, `Results.ModalPeriod` | v2 grep (`Setup`, `JointDispl`, `FrameForce`, `RunAnalysis`); taban `SelectOutput* :1121-1145` [DK]; `BaseReact/ModalPeriod` V2 karşılaştırması için ÇK |
| Concrete Frame Design (final doğrulama) | `DesignConcrete.SetCode("ACI 318-14")`, `StartDesign`, `GetSummaryResultsBeam`, `GetSummaryResultsColumn`; tasarım kombinasyonu `GetComboStrength/SetComboStrength`; `FrameObj.GetDesignProcedure/SetDesignProcedure`; tercih/overwrite (`DesignConcrete.ACI318_14.Get/SetPreference/Overwrite`) | v2 grep (SetCode, StartDesign, GetSummaryResults*, GetDesignProcedure) [DK]; **alt arayüz adları ÇK** |

Riskler (ETABS RC tasarım okuma):
- v2 `GetSummaryResultsBeam` donatı alanı/hata metni verir, **kapasite oranı vermez** (CR §5.1); kiriş oranı için tablo okuma veya kendi hesap gerekir [BD].
- ETABS 22.6'da `DesignCompositeColumn.GetSummaryResults` **kaymış veri** döndürdü, sonuçlar `DatabaseTables` ile okundu (`PROGRAM_KURALLARI.md:228`) [BD]: `DesignConcrete.GetSummaryResults*` için aynı sorun olup olmadığı **ilk iş olarak** denenmeli; yedek: "Concrete Beam/Column Summary - ACI 318-14" tabloları (adlar ÇK).
- `SetComboStrength` ayarlanmazsa tasarım kombinasyonu boş kalabilir (CR F-31/§5.2); `SetRectangle` kesiti her seferinde yeniden tanımlar ve donatı/modifier verisini sıfırlayabilir (CR F-31, ŞÜPHELİ) → final doğrulamada kesitler **bir kez** atanır, donatı/modifier sonra yazılır.
- ETABS SFRC tanımaz: lifli kiriş oranı kendi modülümüzden; ETABS yalnız analiz kuvvetleri, ötelenme, periyot ve kolon/lifsiz kiriş ACI oranı için (BD, CR §7.6).
- Final aşamada tüm kombinasyonlar **açıkça** seçilir (F-03).

---

## 7. Doğrulama planı önerisi

Plan kapısı: V2 geçmeden WP6 başlamaz (`PLAN_PATH_C.md:24`). Aşağıdaki V1–V3 tanımı `PLAN_PATH_C.md` ile uyumlu bir öneridir (V4–V6 plan dosyasında WP4/WP6'ya bağlı).

**V1 (birim, ETABS'siz; WP1–WP2).**
- ACI 318-14 modülü: elle hesaplı kiriş (tek/çift donatı, εt–φ geçişi, ρmin/ρmax), kolon P-M noktaları (saf basınç Pn,max, dengeli nokta, saf eğilme), kesme Vc/Vs/Av,min, detay (aralık/örtü), donatı metrajı (kg). CR F-07 vektörü (b=250,h=350,d=325,As=1605) φ'si şartnameyle teyit edilerek [ÇK] kullanılır.
- fib MC2010: fFts, fFtu, VRd,F (γc dahil), w_k, As,min; yayınlanmış fR1…fR4 verisiyle (Venkateshwaran 2018, Chen 2021 [BD plan]) ve fib Bulletin örnekleriyle.
- Tasarım değişkeni kodlama gidiş-dönüşü, katalog sıralama monotonluğu, maliyet `CostBreakdown = CostStProfile`, `RandomVariable` üst sınır üretimi.
- Regresyon testi vektörleri CR'deki sayısal kanıtlardan (VRd 164 188 N → γc'li 109 900 N; I_ii 837.3e6 yerine 670.6e6).

**V2 (FEM ↔ ETABS; WP3).** Hedef: D/C, ötelenme, periyot farkı ≤ %1–2 (`PLAN_PATH_C.md:17`; taban hedefi: ort. ≤ %1, %95 ≤ %2, en büyük ≤ %5 `FEM_SURUM_KAYITLARI.md:5`).
- Örnekler: Ö1 (2B 6 eleman), Ö2 (2B 21), Ö3 (3B 42), her biri ≥ 3 rastgele tasarım (taban kalibrasyon yöntemi: `FemTool calibdump`, `tools/calibration/`) [BD].
- Karşılaştırılacak: periyotlar (çatlamış rijitlik), kat ötelemeleri (statik + RS), taban kesme, iç kuvvet zarfları (M, V, N istasyon düzeyinde), P-Δ açık/kapalı, kolon ACI oranı (ETABS Concrete Frame Design ile), kiriş lifsiz ACI oranı. Lifli kirişler için yalnız kuvvet/rijitlik karşılaştırması.
- Tuzaklar: DAM 0.8'in RC'de devreye girmemesi (§2.2), kütle çarpanı, uç ofset, rijit diyafram, kazara dışmerkezlik, ETABS "Non-iterative based on mass" davranışı, RC için sünme/βdns varsayımı.
- Kıyas çerçevesi: **Kaveh & Sabzi 2011** (Int. J. Civil Eng. 9(3), `Papers\Kaveh2011.pdf` mevcut, 14 sayfa; metin çıkarıldı): 3 açıklıklı 4 katlı düzlem RC çerçeve, 28 eleman (12 kiriş + 16 kolon), 4 grup (2 kiriş, 2 kolon), ACI 318-08, D = 22.3 kN/m, L = 10.7 kN/m, f'c = 23.5 MPa, fy = 392 MPa, kombinasyonlar `1.2D+1.6L`, `1.2D+1.0L±1.4E`, `0.9D±1.4E`, birim fiyatlar C_c = 105 $/m³, C_s = 0.9 $/kg, C_f = 92 $/m² (kalıp dahil), donatı D19/D22/D25, adım 50 mm; **optimum 22 207 $** (B1 500×300 5D22/3D19, B2 500×300 5D22/4D19, C1 350×350 8D25, C2 300×300 6D25; 9250 analiz HBB-BC) [BD, makale metni]. Notlar: (i) ACI 318-**08** ve E katsayısı 1.4; kıyas için ACI 318-14'ün ilgili farklarına karşı modülün "318-08 modu" veya sonuçların farkını açıklayan not gerekir; (ii) yanal yük değerleri Şekil 5'te (metinde yok; şekilden okunmalı) [Açık Soru]; (iii) kıyas lifsizdir (dozaj 0) ve yalnız maliyet/optimum tasarımı yeniden üretir, kesin eşleşme beklenmez (formülasyon farkları: kalıp, donatı düzeni veritabanı, narinlik).
- V2'de elde edilen FEM−ETABS farkı büyükse 3B örnek (Ö3) ETABS ile veya daha az iterasyonla koşulur (hesap bütçesi kapısı `PLAN_PATH_C.md:24`).

**V3 (optimizer + final doğrulama; WP6).**
- Belirlenimcilik: aynı tohum = aynı sonuç; farklı tohum farklı.
- Final: en iyi tasarım ETABS'te onarımsız yeniden analiz → ceza = 0; kolonlar ETABS ACI oranı ≤ 1; lifli kiriş oranı kendi modülünden (ETABS iç kuvvetleri ile) ≤ 1; `_best.EDB` kaydı; orijinal EDB değişmedi (SHA-256).
- Aday arşivi akışı çalışıyor mu (`FinalizeUnmodified` benzeri), en kötü durum süresi.
- Kaveh&Sabzi kıyası: lifsiz mod, ABC/BBO/TSA ile aynı analiz bütçesi, ≥ 30 koşu; en iyi maliyetin 22 207 $ civarı olup olmadığı.
- Lifli mod duyarlılık: dozaj↑ ⇒ kiriş kesiti ↓ eğilimi, maliyet ayrışımı (beton/donatı/lif) tablo kaymasız.

---

## 8. Riskler ve açık sorular

**Riskler**
1. **V2 en büyük risk [BD plan, ÇK]:** RC çatlamış rijitlik + P-Δ + narinlik büyütmesi FEM–ETABS uyumu ilk kez ölçülecek; çelikteki %0.1–1.4 sonucu betona taşınamaz.
2. **Taban çelik bağımlılığı [DK]:** `ETABS_Class` ≈3000 satır çelik katmanıyla iç içe (W kataloğu, AISC 341, tüp/kutu/kompozit, hibrit); budama büyük bir iştir ve regresyon riski taşır. Alternatif: yalnızca `FemCore` + yeniden kullanılan yöntem sınıflarıyla yeni ince bir host; ancak `OptimizationClass` `ETABS_Class`'a doğrudan bağlıdır (`Eval :377-390`, `LogError :368`). Karar gerekir (S-9).
3. **`FemBackend.TryCreate` kapısı [DK]:** `Notes` boş değilse ETABS'e döner (`Backends.vb:336-337`); RC modelinde alan elemanı, I-olmayan kesit notu, steel material şablonu yüzünden FEM sessizce devre dışı kalabilir; testle "FEM kullanıldı" günlüğü zorunlu.
4. **ETABS RC tasarım API'si v22'de güvenilir mi [ÇK]:** §6 uyarıları.
5. **Lif veri riski [BD plan]:** üretici fR verisi gizli; yayınlanmış veri birincil; ortalama/karakteristik statüsü (CR F-06 notu) belirsiz.
6. **Yönetmelik karışımı [BD]:** ACI 318-14 yük kombinasyonu + TBDY spektrum/ötelenme kapsamı net tanımlanmalı (CR §7.6).
7. **Ayrık indeks kodlamada komşuluk [ÇK]:** (b,h,donatı) 1B sıralama ±1 adımı yöntemleri bozabilir; sıralama anahtarı ve (ops.) bileşik değişken stratejisi test edilmeli.
8. **P-Δ yedek çözümü [DK/BD]:** tutarlı geometrik rijitlik kararsızsa topaklıya düşüş; RC'de hafif/esnek rastgele başlangıç tasarımlarında sık olabilir.
9. **Performans tahminleri [BD]:** çelik 84/525 elemanlı modeller için ölçüldü (0.15–0.45 s/tasarım); 6/21/42 elemanlı RC için ölçülmedi, RC tasarım modülü (P-M eğrisi) hesabı FEM çözümünden baskın olabilir [ÇK].

**Açık sorular (karar/bilgi gerekli)**
- **S-1 Geometri:** "2B 21 eleman (3 kat 2 açıklık)" 15 eleman verir; 21 = 3 kat × 3 açıklık. Hangisi? 3B 42 = 2 kat × 2×2 açıklık (5 m × 6 m aks) doğru mu? Ö1 6 = 2 kat × 1 açıklık mı?
- **S-2 Kombinasyon:** `1.4G+1.6Q` TS 500'e özgü; ACI 318-14: `1.4D`, `1.2D+1.6L`, `1.2D+1.0E+1.0L`, `0.9D+1.0E`. Hangisi esas? Kaveh kıyası ACI 318-08 (1.4E).
- **S-3 Deprem:** RS mi ELF mi, hangisi arama sırasında? Spektrum TBDY-2018 (R, D, I azaltması; R=8 yüksek süneklik ⇒ D) mi, ASCE 7 (yalnız R) mi? R=8 ise ACI Ch.18 / TBDY özel detay (SCWB, kapasite tasarımı, sık etriye) kapsamda mı?
- **S-4 P-Δ ve narinlik:** FEM'de yalnız P-Δ analizi mi, ACI δns/δs büyütmesi de mi? ETABS final modeli hangi ayarla?
- **S-5 Donatı:** ayrık değişken (çap×adet×sıra) mı, iç döngüde en ucuz yeterli düzen mi? Etriye çapı/aralığı?
- **S-6 Beton sınıfı/lif sınıfı:** "sınıf" = beton sınıfı mı, fib kalıntı dayanım sınıfı mı? Grup başına mı, global mi? Lif tipi/dozaj kümesi (makale: Tür-2/3, 20–40 kg/m³)?
- **S-7 Ceza:** `Σ max(0, g_j)` (öneri) vs tabandaki `max`; üs 3 korunsun mu? Makalede yazılacak tanım.
- **S-8 Bütçe:** eşit analiz (onarım dahil) mi eşit aday mı? (`Backends.vb:462` analiz sayar.)
- **S-9 Kod yapısı:** tam fork-ve-buda mı (ETABSClass), yoksa ince yeni host mu?
- **S-10 Çatlamış rijitlik:** yalnız 0.35/0.70 mi; servis ötelenmesinde ayrı çarpan; sünme etkisi?
- **S-11 Kaveh Şekil 5:** yanal yük değerleri (şekilden), ACI 318-08 uygulama modu.
- **S-12 Örnek modeller:** v2 EDB'leri çubuk modele (alan elemanı yok) çevrilecek mi, yoksa ModelBuilder ile sıfırdan mı kurulacak?
