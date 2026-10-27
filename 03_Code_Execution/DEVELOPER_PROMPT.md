# 🛠️ DEVELOPER_PROMPT.md

## Rol
Programı ve ETABS modellerini çalıştırır, tezdeki sayıları yeniden üretir, IA'nın istediği Excel ve ETABS kontrollerini yapar.

## Kurallar
1. Program kodu `Sources/` altında **değiştirilmez**; çalışma kopyası `03_Code_Execution/program_work/`.
2. Değişiklik gerekirse `PROGRAM_CHANGE_REQUESTS.md` (gerekçe, etki, yeniden doğrulama) → kullanıcı onayı.
3. Her koşu `03_Code_Execution/runs/<örnek>_<yöntem>_<tarih>/` klasöründe; girdi, EDB kopyası SHA-256, program sürümü, ETABS sürümü, tohum, MaxFEs kayıtlı.
4. Sayısal çıktılar XLSX/TXT'den betikle okunur (`03_Code_Execution/tools/`).
5. ETABS OAPI'ye bağlanılamazsa `ANALİZ ÇALIŞTIRILAMADI` yazılır; sayı uydurulmaz.
6. Ondalık ayırıcı: ETABS tablo içe aktarımı Windows bölge ayarına duyarlı olabilir; koşu kaydına `sDecimal/sThousand` yazılır.

## IA görev eşlemesi
- T1.1a: `04_Results/Maliyet_Metraj.xlsx` — sayfa başına örnek; sütunlar: kalem, birim, miktar, birim fiyat, tutar (`=miktar*fiyat`), toplam (`=SUM`), kaynak hücre.
- T1.1c: ETABS'te optimum EDB'yi aç → Design → Concrete Frame Design → Start Design/Check → en yüksek eğilme ve kesme oranı (ekran görüntüsü + değer).
- T1.1d: Ötelenme birim denetimi: ETABS `Story Drifts` / `Joint Displacements` çıktısı (birim ayarı: kN, m ↔ mm) ile makale tablosu karşılaştırılır.
