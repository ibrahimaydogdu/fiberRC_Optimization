# 🧪 VALIDATION_SPEC.md (Doğrulama Kapısı)

V0–V5 sağlanmadan bulgular bölümü yazılmaz. Sonuçlar `04_Results/VALIDATION_REPORT.md`'ye yazılır. ⚠️ ölçütler kullanıcı onayıyla kesinleşir.

- **V0 Ortam:** ETABS sürümü, OAPI sürümü, Visual Studio/MSBuild, .NET 4.7.2; kaynak koddan derleme başarılı; derlenmiş `bin/*` ↔ yeni derleme karşılaştırması (SHA-256, davranış).
- **V1 Model:** `2B_Örnek1_k.EDB`, `3B_Örnek2_kaveh2023_15_04_2024.EDB` ve tez `örnek_1/2/3_son` EDB'leri açılır; geometri, yük, kombinasyon, malzeme sayfa belgeye ve makale tanıtımına karşı kontrol edilir (kat yüksekliği, açıklık, yükler, B1/C1 grupları).
- **V2 Referans örnek:** Lifsiz tasarım, literatürdeki referans çalışmanın (Kaveh 2023; Gebrail/Niğdeli 2023; Örnek 3 referansı) sonucuyla karşılaştırılır; fark yüzde olarak raporlanır ⚠️ ≤ %5.
- **V3 Tasarım kapasite oranları:** Tüm optimum tasarımlar ETABS'e yazılıp düzeltmesiz yeniden analiz; eğilme (MKKKO) ve kesme (MKO) oranları ≤ 1,0 ⚠️; hiçbir "N/A".
- **V4 Ötelenme:** Birim zinciri (ETABS birimi → tablo birimi) elle kontrol; 3D lifli ötelenme 0,1–0,2 mm değerleri açıklanır ya da düzeltilir.
- **V5 Maliyet:** Metraj ve maliyet Excel'i formüllü; makale tablosu ↔ Excel hücre bağı; toplam maliyet, programın raporladığı amaç değeriyle ≤ %0,5 uyumlu ⚠️.
- **V6 Tekrarlanabilirlik (yeni koşu yapılacaksa):** 30 koşu, tohumlar kayıtlı.
