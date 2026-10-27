# 🔗 PROGRAM_REFERENCE.md (Optimizasyon Programı — Salt-Okunur Referans)

## 1. Kimlik
| Alan | Değer |
|---|---|
| Program | Lifli/lifsiz betonarme çerçeve optimizasyonu (ETABS OAPI, VB.NET) |
| Kaynak kod | `Sources/Betonarme/Lİfli/v2/` (`BetonarmeCerceve.sln`, `Betornarme.vbproj`) |
| Dosyalar | `ETABSClass.vb` (1704 satır, ETABS/OAPI + tasarım), `OptimizationClass.vb` (1044, algoritmalar), `Structures.vb` (426), `MainForm.vb` (362), `MainForm.Designer.vb` (1261) |
| Ortam | .NET Framework 4.7.2; ETABS OAPI (`CSI API ETABS v1.chm`) |
| Derlenmiş sürümler | `bin/Debug`, `bin/Release` — kaynak kodla eşleştiği DOĞRULANAMADI |
| ETABS modelleri | `Sources/Betonarme/2B_Örnek1_k.EDB`, `3B_Örnek2_kaveh2023_15_04_2024.EDB`; çalışma klasörleri `v2/ETABS/{Deneme,Seda}` |
| Sürüm kontrolü | Git yok. Son kaynak değişikliği: ETABSClass/Structures/MainForm 2024-05-13, OptimizationClass 2025-04-29 |
| ETABS sürümü | DOĞRULANAMADI (kullanıcıdan alınacak) |

## 2. Kurallar
1. Salt-okunur: ajanlar `Sources/` altındaki dosyayı değiştirmez. Çalışma kopyası `03_Code_Execution/program_work/` altında alınır.
2. Program değişikliği: `03_Code_Execution/PROGRAM_CHANGE_REQUESTS.md` → kullanıcı onayı → çalışma kopyasında uygulama → regresyon.
3. Programın kendi çıktıları (`örnek_globalbestprint*.xlsx`) kanıt değil karşılaştırma tabanıdır; makale sayıları yeniden üretilir veya tez tablosuna atfedilir.
4. Bilinen riskler: `Inputinfo.txt` içinde `E:\OneDrive...` sabit yolları var (bu makinede yok); program eski bir Kompozit projeden türemiş olabilir; ETABS OAPI sürüm uyumu denetlenmeli.
