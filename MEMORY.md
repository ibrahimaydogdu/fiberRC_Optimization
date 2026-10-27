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
