# 🧠 MEMORY.md: Proje Hafızası (taşınabilir; Claude + Gemini ortak)

**Proje:** Lifli betonarme çerçevelerin ETABS-OAPI entegre meta-sezgisel optimizasyonu makalesi (S. Turan tezi → makale). **Son güncelleme:** 2026-10-27 · **Aşama:** 0.

## 1. Kişiler
IA = Prof. Dr. İbrahim Aydoğdu (geliştirici, proje sahibi). ST = Dr. Öğr. Üyesi Seda Turan (doktora öğrencisi, makale metni yazarı). EE = Dr. Öğr. Üyesi Engin Emsen. Yazar adı "Tura/Turan" teyit bekliyor (D2).

## 2. Kapsam (kaynaklardan; kullanıcı teyidi bekler)
3 örnek (2D 6 eleman, 2D 21 eleman, 3D 42 eleman), lifsiz vs kirişleri lifli; ACI 318-14 + fib MC2010; EN 14651 CMOD; algoritmalar YAKO/BTO/ATO/İVAO; ETABS OAPI + VB.NET.

## 3. Ana kararlar
- Mimari: şablon `Steel_Composite_Hybrid_Frame_Optimization` çok ajanlı yapısı uyarlandı (2026-10-27). Konuya özgü içerik taşınmadı.
- `Sources/` salt-okunur. Çıktılar 04/05 klasörlerine.
- Kaynak açıklamaları `Kaynak Açıklamaları` klasöründe değil, kullanıcının mesajındaydı; `PROJECT_STATE.md` §1'e işlendi.

## 3b. Kullanıcı kararları (2026-10-27)
Makale **İngilizce**; yazar **Seda Turan**; ETABS 2022 + Visual Studio 2026 kurulu, çalışma kopyasında derleme/çalıştırma izinli (Python betiği serbest); tezdeki kod şüpheli → doğrulama zorunlu, yanlışsa yenileme; VAO/İVAO elzem değil (odak optimizasyon testi değil; hakem görüşü alınır); birim fiyat için `Steel_Composite_Hybrid_Frame_Optimization/Evidence/UnitPrices` kullanılabilir, eksik kaynaklar `01_Literature/UNIT_PRICE_NEEDS.md`; kod incelemesi zorunlu. Aday program: `D:\OneDrive Akdeniz Ogrenci\OneDrive - Akdeniz Üniversitesi\myprojects\Algoritmalar\MVS2010\ETABS\SteelFamewithCompositeTubeColumn_FEM` (çelik/kompozit, FEM + ETABS doğrulamalı) betonarmeye uyarlanabilir mi. Git: depo `https://github.com/ibrahimaydogdu/fiberRC_Optimization.git`; tüm push/pull yalnız Gemini.

## 3c. Kararlar (2026-10-27, 2. tur)
Program yolu: **C hibrit** (FEM ile optimizasyon; ETABS: örnek kurma + final doğrulama). Hedef dergi: *Structures*. ETABS v22 ≡ 2022. Fiyat: ÇŞİDB resmî pozlar kullanılır (lif için tedarikçi teklifi aranmaz). EN 14651/fR verisi (`Evidence/CMOD_test_verileri.xlsx`) bir tedarikçiden alınmış; firma adı paylaşılmak istenmiyor → **kırmızı bayrak**, bkz. `01_Literature/UNIT_PRICE_EVIDENCE.md` §5.

- Fiyat yılı **2026**; lif verisi: yayınlanmış Venkateshwaran 2018 + Chen 2021 (bkz. `SedaDoktora2/Manuscript_04_09_2026.docx`); üretici verisi gizli beyanıyla. Örnek 3 uyarısı: Tapao 2017 Örnek 3'te 9 grup (B1–B3, C1–C6) var; tezde 7 grup (C1–C3, B1–B4) → eşleşme kontrol edilecek.

## 4. Riskler
Bkz. `PROJECT_STATE.md` §4.

## 5. Sıradaki adım
`TASKS.md` B (D1–D8) kararlarını kullanıcıdan al; sonra Aşama 1 (Literatür) ve Aşama 3a (V0: programı derle, ETABS modelini aç, tez sonuçlarını yeniden üret) başlat.

## 3d. Kararlar (2026-10-28, 3. tur)
- **Yük koşulları:** Örnek 1 (özgün) depremli (TBDY-2018 spektrumu); Örnek 2 ve Örnek 3 kaynaklarındaki tanımlı yüklemeyle kalır, **deprem alınmaz** (Ö2: yalnız yerçekimi, ACI kombinasyonu 1,2D+1,6L; Ö3: yerçekimi + rüzgâr, N&C 2022 kombinasyonları). 
- **Ö1 taşıyıcı sistem:** A11 (süneklik düzeyi yüksek; R=8, D=3) esas; TBDY Bölüm 7.3/7.4 şartları kısıt olarak eklenir. A31 (R=4, D=2,5) kullanıcı tarafından "kabul"; ancak TBDY 4.3.4.3 (PDF 54 / basılı 33) A31'i yalnız DTS=3 ve 4'te izinli kılar; tezdeki SDS=1,2 → DTS=1 ile **A31 uygulanamaz**. A31 yalnız düşük deprem bölgesi (SDS<0,5) senaryosu olarak koşulur; bu senaryonun SDS/SD1/zemin değerleri kullanıcıdan istenecek.
- **Ö3 kaynağı:** Negrin & Chagoyén 2022 (Tapao değil); kaynakça ve metin düzeltilecek (THESIS_EXAMPLE_DATA.md).

## 3e. Kararlar (2026-10-28, 4. tur)
- **Ö1 geometri:** açıklık 6 m, kat yükseklikleri 3 m + 3 m (MakaleMetni §3.1.1); tezdeki Şekil 4.13 (5 m; 4+3 m) kullanılmaz. Yeni Ö1 yükleri/kesit havuzu bu geometriye göre yeniden kurulur.
- **Ö1 zemin sınıfı ve DD-3:** kullanıcı "alalım, büyük olsun" dedi; hangi değerlerin kastedildiği (konum/SDS-SD1 büyüklüğü) teyit bekliyor.
- **Ö1 deprem (kullanıcı kararı 2026-10-28):** zemin sınıfı **ZC**; ana senaryo tez DD-2 (SDS=1,2; SD1=0,3); **duyarlılık analizi yapılacak** (düşük/yüksek talep, çarpanlar kullanıcıya sorulacak); "büyük olsun" Hakem önerisine bırakıldı (konum seçilmez). Ss/S1/DD-3 kullanıcıda yok → konumsuz AFAD okunamaz; ZC için Ss,S1 Tablo 2.1/2.2 ters çözümüyle (G10 doğrulaması sonrası), DD-3/λ için konum veya TBDY 4.9.1 kuralı gerekli.
- **AFAD (2026-10-28):** Antalya Muratpaşa 36.899312 K, 30.674937 D, ZC, **DD-1 (2475 yıl)**: Ss=1,113 S1=0,290 SDS=1,336 SD1=0,435 PGA=0,513 g PGV=21,363 (Evidence/AFAD/AFAD_Muratpasa_ZC_DD1_28102026.pdf, SHA-256 defcc0cf…8ad42, 570071 bayt). Bilgi Al: PGA(475 yıl, DD-2)=0,263 g. Tasarım DD-2 ister; DD-2 ve DD-3 raporları bekleniyor. DD adlandırması: DD-1=2475, DD-2=475 yıl (TBDY).
- **AFAD DD-2 ve DD-3 geldi (2026-10-28):** Muratpaşa ZC DD-2: Ss=0,563 S1=0,151 SDS=0,718 SD1=0,226 PGA=0,263; DD-3: SDS=0,294 SD1=0,091; ayrıntı 01_Literature/AFAD_MURATPASA.md. Ana senaryo seçimi (Muratpaşa mı, tez SDS=1,2 mi) kullanıcı onayı bekliyor.
- **KİLİT (kullanıcı onayı 2026-10-28):** Ö1 ana deprem senaryosu = Muratpaşa DD-2, ZC (SDS=0,718; SD1=0,226; AFAD_MURATPASA.md). Duyarlılık: DD-1 gerçek değerlerle (SDS=1,336; SD1=0,435); tez SDS=1,2/SD1=0,3 yalnız WP5 tez yeniden üretimi için. DTS/BYS ve λ: G10 doğrulaması sonrası kesinleşir.
- **Lif fiyatı (kullanıcı 2026-10-28):** tedarikçiye e-posta atıldı, yanıt beklenir. Yanıt gelmezse seçenekler: (a) tezdeki 3,50/4,75 $/kg (kanıtsız, düşük güven) veya (b) ÇŞİDB 90 TL/kg + duyarlılık/break-even; Claude önerisi (b), tez fiyatları ek duyarlılık noktası.
- **Ö1 yeniden tanımı (kullanıcı onayı 2026-10-28):** Ö1 = 3B, 4 katlı, perdesiz konut çerçevesi, 3×2 açıklık, 5×5 m, kat 3 m, 116 eleman (48 kolon + 68 kiriş), ~10 grup; deprem Muratpaşa DD-2 ZC, A11. 2B 6 elemanlı örnek = **doğrulama örneği** (FEM↔ETABS). Makalede 3 optimizasyon örneği (Ö2, Ö3, Ö1) + doğrulama örneği. Yük kaynakları: TS 498:2021, TS ISO 9194, EN 1991-1-1 (Evidence/DesignGuides). Ayrıntı: 03_Code_Execution/EX1_SPEC.md, EX0_SPEC_2D_validation.md.
- **Ö1 yükleri/lif uzayı (2026-10-28):** kar Sk=0,75 kN/m² (TS 498:2021 bölge 1, 0–200 m; μ için TS EN 1991-1-3 yok → s=Sk); BYS=6 (Tablo 3.3, HN=12 m, DTS=2); TBDY 7.2: C25 alt sınır, donatı B420C; kesit hesabı ACI (TBDY 7.2.4 TS 500 ister → sapma makalede belirtilecek); lif 65/60BG verisi yalnız C30/C40: 4D C30 20–40, C40 20–35; 5D 25–35 kg/m³ (ekstrapolasyon yok önerisi). Ayrıntı EX1_SPEC.md.

## 3f. Karar (2026-10-29, 5. tur)
- **Şartname tabanı = A:** tüm örneklerde kesit kapasitesi ACI 318-14 + fib MC2010 (lif); Ö1'de TBDY yalnız yük/spektrum/A11 kısıtları ve kapasite tasarımı (Mr = fcd/fyd, φ yok; TBDY s.110 + TS 500 Denk. 6.2, Çizelge 7.1 doğrulandı). TBDY 7.2.4 (TS 500 zorunlu) sapması makalede beyan; Ö1 final optimumuna TS 500 Mr/Vc kontrol hesabı + ACI/TS 500 oran tablosu eklenir. Vc=0 anahtarı TBDY'deki gibi kodlanır (yumuşatma yok); lifli kirişte sarılma bölgesinde lif katkısı 0 (muhafazakâr, kaynak atfı aranacak). Varsayımlar tablosu (değer, madde, gerekçe, duyarlılık). Örtüler ACI 20.6.1.3.1: 40 mm → kiriş bar merkezi 60, kolon 60, etriye ekseni 45. Hakem görüşü: 06_Review_Audit/DECISION_CODE_BASIS_BRIEF.md.

## 3g. Karar (2026-10-30, 6. tur: V2 doğrulaması)
- **Kolon kısıtı radyal D/C** (ETABS OL/OC; `ColumnDesign.ColCurve/RadialDcr`); CD3 geçti.
- **Kapasite tasarımı zinciri ACI 318M-14 Bölüm 18** (kiriş ve kolon kesmesi Mpr 1,25 fy, güçlü kolon ΣMnc ≥ 1,2 ΣMnb, birleşim kesmesi 1,7/1,2/1,0, Tablo 18.7.5.4 sarılma); TBDY yalnız yük/spektrum/boyut-oran sınırları/ötelenme/A11. Kolon kesmesi ΣMpr kirişlerle sınırlanır. IA: "ACI TBDY tercihini sen yap... S ile alacağın tüm kararları onaylıyorum" (yetki: Claude'un doğrulama/uygulama kararları, gerekçesiyle kayda geçirilir).
- ETABS OAPI ile kolon kapasite kesmesi ve birleşim kontrolü devreye girmiyor ("Joint check not done"): bu bileşenler ETABS ile doğrulanamadı (sınırlama).
- δns tüm birleşimlerde (ETABS davranışı), lu = net yükseklik, Cm = 1; PCR-1..3 uygulandı. S2 (SFRC deney) sonuçsuz: EN 14651 fR + silindir fc birlikte veren kiriş deneyi bulunamadı.

## 3h. Karar (2026-11-03, 7. tur: ön tasarım ve vizyon)
- **Vizyon:** uygulamalı mühendislik optimizasyonu; FSD ön tasarımı çekirdek rehber (warm-start); hedef 1 000 FE'de hızlı yakınsama + 10 000 FE'de oturmuş durum; ön tasarım FE'si bütçeye dahil. YAKO/BTO/ATO rakip değil, doğrulama aracı (algoritma üstünlüğü yargısı yok). Ayrıntı: `PREDESIGN_PROTOCOL.md` §7, AGENTS.md kural 8.
- **PCR-6/7/8/11 uygulandı:** havuz kutusu, ön tasarım (FSD kapsama testi ≥ 9/10 BAŞARISIZ: kutu optimumu kapsamıyor), kolon rho + beton sınıfı türetmesi (24/32 değişken), koşu günlüğü. Legacy mod Baseline-A ile birebir (regresyon 6/6). Hiyerarşi kuralı (PCR-9) ve geometrik ön koşul (PCR-10) yapılmadı (kaynak/bağımsız pilot gerekir).
- **Ablasyon (Faz 1, 1 000 FE; kollar R, A, C, ABC; lifsiz + lifli; 30 koşu)** donmuş ikiliyle başlatıldı; sonuç Critic + IA onayı olmadan bulgular bölümüne girmez. Git yalnız Gemini (Claude commit etmez).


## 3i. Karar (2026-11-04/05, 8. tur: Faz 1 sonucu ve Faz 2)
- **Faz 1 (720 koşu, 1 000 FE) bitti:** R→A %17–20 iyileşme; C/ABC medyanı ön tasarım noktasına (1 664 921 TL, lifli = lifsiz x0) yapışık; ABC en iyi (birleşik medyan lifsiz 1,59 M, lifli 1,65 M); H2/H3 hiç sağlanmadı; BTO/ATO 10 000 FE rastgele medyanın %2–10 üstünde (lifsiz ATO %1,3). Critic: KOŞULLU ONAY (`04_Results/ablation_stats.md`). H1 başarısı YAKO için kanıt sayılmaz (x0 hedefin altında).
- **IA kararı:** Faz 2 = 540 koşu (ABC, A, kutusuz C × lifsiz/lifli × 3 yöntem × 30) × 10 000 FE; amaç MaxFE (düzleşme) belirleme, sonra eşit koşulda lifli/lifsiz. **PCR-12:** BTO/ATO hamle başına tek boyut (`--onedim 1`; exploitation; makalede kısa ve dürüst not; k Faz 1'e göre ayarlanmadı). Protokol: `PREDESIGN_PROTOCOL.md` §8; ikili `phase2_bin`.
- **Koşu:** `03_Code_Execution/run_phase2.sh` (22:53 başladı, ≈ 80 saat), `run_phase2_guard.sh` eksik koşuları tamamlar. Runner iki kez sessizce erken bitti (kök neden bilinmiyor); çıktı `04_Results/phase2/`. Yeniden başlatma: `nohup bash ./run_phase2_guard.sh`. Faz 2 bitince düzleşme analizi (FE-medyan eğrisi, <%1), lifli-kapasiteli x0 kolu, Critic + IA; bulgular bölümüne giriş V3 ETABS ve S2 sonrası.

## 3j. Karar (2026-11-09/10, 9. tur: Faz 2 sonucu, lifli x0)
- **Faz 2 (540 koşu, 10 000 FE) bitti** (`04_Results/phase2_stats.md`, `tools/phase2_stats.py`). Düzleşme yok (5k→10k %1,1–7,3); A/C/ABC birleşik fark yok, yöntem bazında değişir (BTO'da ABC kutusu A'dan kötü); lifli senaryo lifsizden %1,0–5,8 pahalı (A-ATO en iyiler %0,2 eşit; ATO'da Holm anlamsız); BTO Faz 2 Baseline-A'dan %2–7 kötü (PCR-12 etkisi rho/sınıf türetmesinden ayrılamıyor). Critic: KOŞULLU ONAY (yapılamaz iddialar: lif pahalı/ekonomik değil, optimum, MaxFE=10 000 yeterli).
- **IA kararları (2026-11-09):** (1) Lifli senaryoda x0 lifli kapasiteyle üretilir (önceki hata; lifli x0 = lifsiz x0 idi); lifsizde lifsiz-x0. Post-hoc kayıt (PCR-13, protokol §9). (2) MaxFE: önce 20 000 FE (ATO+YAKO, A ve ABC, lifli+lifsiz, 10 koşu); ön kayıtlı kuralla otomatik 30 000'e geçiş, IA izni gerekmez. (3) BTO tüm-boyut kontrolü en sonda, yalnız ek materyal; ana metinde 5 cümlelik PCR-12 beyanı şart. (4) Açıklık duyarlılığı (3B Ö1, 5–10 m) lifli-x0 + MaxFE sonrası. IA: "onay almadan işlemlere başla", 24 saat sonra kontrol, push Gemini'de.
- **PCR-13 sonucu (2026-11-09):** lifli kapasiteli FSD (10 tip×doz adayı, 363 FE) lifsiz FSD ile aynı kesitlere indi; seçilen x0 lifsiz (doz 0) = Faz 2 x0 → lifli x0 değişikliği başlangıcı değiştirmedi (yalnız 363 FE harcadı). Sınırlayıcı kısıt dayanım değil (nedeni incelenmeli; "lif donatıyı düşürmüyor" bulgusu için kritik, Critic'e sor). Faz 3 zinciri: `run_phase3.sh` (20k → otomatik karar → gerekirse 30k → BTO tüm-boyut), protokol §9.
