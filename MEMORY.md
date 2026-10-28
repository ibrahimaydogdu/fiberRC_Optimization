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
