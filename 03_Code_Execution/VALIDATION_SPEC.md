# 🧪 VALIDATION_SPEC.md (Doğrulama Kapısı)

V0–V5 sağlanmadan bulgular bölümü yazılmaz. Sonuçlar `04_Results/VALIDATION_REPORT.md`'ye yazılır. ⚠️ ölçütler kullanıcı onayıyla kesinleşir.

- **V0 Ortam:** ETABS sürümü, OAPI sürümü, Visual Studio/MSBuild, .NET 4.7.2; kaynak koddan derleme başarılı; derlenmiş `bin/*` ↔ yeni derleme karşılaştırması (SHA-256, davranış).
- **V1 Model:** `2B_Örnek1_k.EDB`, `3B_Örnek2_kaveh2023_15_04_2024.EDB` ve tez `örnek_1/2/3_son` EDB'leri açılır; geometri, yük, kombinasyon, malzeme sayfa belgeye ve makale tanıtımına karşı kontrol edilir (kat yüksekliği, açıklık, yükler, B1/C1 grupları).
- **V2 Referans örnek:** Lifsiz tasarım, literatürdeki referans çalışmanın (Kaveh 2023; Gebrail/Niğdeli 2023; Örnek 3 referansı) sonucuyla karşılaştırılır; fark yüzde olarak raporlanır ⚠️ ≤ %5.
- **V3 Tasarım kapasite oranları:** Tüm optimum tasarımlar ETABS'e yazılıp düzeltmesiz yeniden analiz; eğilme (MKKKO) ve kesme (MKO) oranları ≤ 1,0 ⚠️; hiçbir "N/A".
- **V4 Ötelenme:** Birim zinciri (ETABS birimi → tablo birimi) elle kontrol; 3D lifli ötelenme 0,1–0,2 mm değerleri açıklanır ya da düzeltilir.
- **V5 Maliyet:** Metraj ve maliyet Excel'i formüllü; makale tablosu ↔ Excel hücre bağı; toplam maliyet, programın raporladığı amaç değeriyle ≤ %0,5 uyumlu ⚠️.
- **V6 Tekrarlanabilirlik (yeni koşu yapılacaksa):** 30 koşu, tohumlar kayıtlı.

## V2 alt kapıları: FemCore/program ↔ ETABS kabul eşikleri (yazılı, 2026-10-30; Critic Z2/K3)
**Not (dürüstlük):** Eşikler V2a/V2b/V2c'nin ilk koşuları görüldükten sonra yazıldı; ancak bu eşiklere göre karar, FemCore burulma düzeltmesinden SONRA yeniden alınan loglarla verilir. Değerler ⚠️ IA onayıyla kesinleşir.

| Kapı | Ölçüt | Eşik |
|---|---|---|
| FE1 | Periyotlar (12 mod), FemCore ↔ ETABS | ≤ %0,5 |
| FE2 | Taban kesmesi (EX, EY) | ≤ %0,5 |
| FE3 | Köşe yer değiştirmesi (RS, ECC=0 ve ECC=%5) | ≤ %0,5 |
| FE4 | Çubuk uç kuvvetleri (statik ≤ %0,1; RS büyük bileşen ≤ %0,5) | belirtilen |
| CD1 | Yanlış-uygun yok: ETABS kolon D/C > 1 iken programın üretim kısıtı ≤ 1 olan kolon sayısı | 0 |
| CD2 | Radyal+e_min programı / ETABS oranı (ETABS D/C > 0,02), tüm kolon ve birleşimler | ≥ 0,95 |
| CD3 | Kritik (maks) D/C farkı | **birincil: güvensiz yönde ≤ %5**; güvenli yönde fazlalık yalnız bilgi (revizyon 2026-10-30, V2i) |
| B1 | Gerekli kiriş As (üst, alt), program / ETABS | ≥ 0,98 ve ≤ 1,10 |
| B2 | Gravite Av/s (program / ETABS) | 0,98–1,02 |
| B3 | Kapasite kesmesi Av/s (ACI zinciri, sağlanan donatı ile) | ≥ 0,95 |
| S1 | SFRC eğilme Mn: bağımsız sayısal entegrasyon ↔ `SfrcBeam.Flexure` | ≤ %1 |
| S2 | SFRC eğilme Mn: yayınlanmış deney kirişleri (literatür; Literature Analyst görevi), tahmin/ölçüm | ortalama 0,90–1,10, tek tek ≥ 0,80 (⚠️) |
| CV1 | Kolon tasarım kesmesi Ve, üretim / ETABS (V2i) | ≥ 1,00 (her kolon) |
| JS1 | Birleşim kesmesi oranı, program ↔ ETABS | ±%5 |
| SC1 | ΣMnc/ΣMnb kritik oran, program ↔ ETABS | güvensiz yönde ≤ %2 |
| Q1 | ACI 6.6.4.3 stabilite indeksi | ≤ 0,05 (kısıt) |
