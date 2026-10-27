# 📍 PROJECT_STATE.md

**Aşama:** 0 — kaynak envanteri ve platform kurulumu. Hiçbir analiz çalıştırılmadı, hiçbir sayı doğrulanmadı.

## 1. Kaynak envanteri (`Sources/`, salt-okunur)
| Kaynak | Rol | Durum |
|---|---|---|
| `Makale_Metni/DOKTORA_TEZİM_01_08_2024_st.docx`, `SEDA TURAN_..._imzalı.pdf` | ST doktora tezi | Okunmadı (büyük); Aşama 1–2'de bölüm bölüm okunacak |
| `Makale_Metni/ÇERÇEVE İÇİN.docx` | ST'nin makale metni (eski) | Okundu (özet/giriş); lifsiz/lifli, YAKO/BTO/ATO/VAO/İVAO |
| `Makale_Metni/MakaleMetni.docx` | IA'nın yeniden yapılandırdığı metin | Okundu (yapı): Özet, Giriş, 2.1–2.4, 3.1–3.5, Sonuç, Kaynaklar; Çizelge 1 algoritma parametreleri |
| `Makale_Metni/Yapılacaklar.docx` | IA'nın ST'ye görev listesi | Okundu → `TASKS.md` A |
| `Makale_Metni/Book1.xlsx` | ST'nin tabloları (Sheet1, Sayfa1, Sayfa2) | İçerik okunmadı |
| `tez_sonuç tablo grafik/` | Tez sonuçları: örnek_1/2/3_son (EDB, rc, sfrc), globalbest, metraj, optimizasyon parametreleri, referans PDF'ler | Dosya düzeyinde envanterlendi |
| `Betonarme/Lİfli/v2/` | VB.NET + ETABS OAPI program kaynak kodu | Kod incelenmedi |
| `Betonarme/*.EDB` | 2B Örnek 1, 3B Örnek 2 (Kaveh 2023) ETABS modelleri | Açılmadı |

## 2. Makale iskeleti (mevcut)
Üç örnek: Ö1 2D 2 katlı 6 elemanlı; Ö2 2D 3 katlı 21 elemanlı; Ö3 3D 2 katlı 42 elemanlı. Lifsiz (RC) ve kirişleri lifli (SFRC) tasarım; ACI 318-14 + fib MC2010; EN 14651 CMOD testi; algoritmalar YAKO, BTO, ATO, İVAO. Özette %22–68 donatı azalması, %17–61 maliyet avantajı iddiası var → **DOĞRULANAMADI** (Excel'den yeniden üretilecek).

## 3. IA'nın saptadığı kritik sorunlar
Maliyet tablolarında satır kayması, N/A kapasite oranları, 3D ötelenme birim şüphesi, numaralama/atıf, sembol yazımı. Ayrıntı `TASKS.md` A.

## 4. Riskler
- Programın derlenmiş exe'si ile kaynak kodun eşleşmesi doğrulanmamış; kod 2024-05/2025-04 tarihli.
- ETABS modelleri ve OAPI sürümü bu makinede kullanılabilir mi bilinmiyor.
- Tez metni ile makale arasında sayı tutarlılığı doğrulanmamış.
