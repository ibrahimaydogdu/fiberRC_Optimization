# docx_style (Akademik Word Biçim Kuralları ve Derleyici)

Bu modül, Markdown dosyalarından Q1 standartlarına uygun, Times New Roman tabanlı ve kesin biçim kurallı Microsoft Word (`.docx`) belgeleri üretir.

## 🚀 Hızlı Kullanım

```powershell
python -I -X utf8 tools/docx_style/md2docx.py <girdi.md> <cikti.docx>
```

Örnek (Taslak makale derleme):
```powershell
python -I -X utf8 tools/docx_style/md2docx.py 05_Drafts/Paper1/MANUSCRIPT_FINAL.md 05_Drafts/Paper1/MANUSCRIPT_FINAL.docx
```

---

## 🛠️ Bileşenler

1. **`md2docx.py` (Ana Dönüştürücü):**
   - Pandoc ikilisini sistem PATH'inde veya standart konumlarda (`AppData\Local\Programs\pandoc`, `Program Files`) otomatik bulur.
   - Windows ASCII dışı yol uyumluluğu için `reference.docx` ve Lua süzgecini geçici dizin üzerinden çalıştırır.
   - **Denklem Tablosu Temizliği (Postprocess):**
     - Pandoc'un varsayılan `Table` stili tek satırlı denklem tablolarında alt kenarlık (bottom border) ve başlık çizgisi üretebilmektedir.
     - `md2docx.py`, hem `oMath` varlığı hem de `SEQ Denklem` / `(N)` alanlarını analiz ederek denklem tablolarını tespit eder.
     - Tablo düzeyinde `tblBorders` (top, bottom, left, right, insideH, insideV) `none` yapılır, `tblStyle` temizlenir, `tblLook` bayrakları sıfırlanır (`0000`).
     - Hücre düzeyinde (`tcPr/tcBorders`) tüm kenarlıklar kesin olarak `none` yapılır ve `cnfStyle` / `pBdr` temizlenir.
     - Denklem sola, numara parantezi sağa hizalanır.

2. **`house_style.py` (Biçim Şablonu ve Kurallar):**
   - **Ana Başlık (Title):** Times New Roman 14 pt, kalın, ortalı, siyah, 12 pt önce / 12 pt sonra.
   - **Heading 1:** TNR 14 pt, kalın, sola hizalı, siyah, 12 pt önce / 12 pt sonra.
   - **Heading 2:** TNR 13 pt, kalın, sola hizalı, siyah, 6 pt önce / 6 pt sonra.
   - **Heading 3 (4+ dahil):** TNR 12 pt, kalın, sola hizalı, siyah, 6 pt önce / 6 pt sonra.
   - **Normal / Body Text:** TNR 12 pt, siyah, 1.0 satır aralığı, 6 pt önce / 6 pt sonra.
   - **Tablo İçi (Compact):** TNR 12 pt, siyah, 1.0 satır aralığı, 0 pt önce / 0 pt sonra.
   - **Sayfa:** A4 (21,0 × 29,7 cm), tüm kenar boşlukları 2,5 cm.
   - `build_reference()` ile referans Word belgesi oluşturulur.

3. **`captions_equations.lua` (Pandoc Lua Süzgeci):**
   - Tablo başlıkları tablonun üstünde (`Tablo N.`).
   - Figür başlıkları figürün altında (`Figür N.`).
   - `$$...$$` matematik blokları 2 sütunlu tabloya dönüştürülür: solda denklem, sağda `(N)` Word SEQ alanı.
   - Word'de açıldığında Ctrl+A ardından F9 ile alanlar güncellenebilir.

4. **`ornek_cikti.docx`:**
   - Düzeltilmiş ve kenarlıkları doğrulanmış referans çıktı dosyası.

---

## 📝 Markdown Yazım Kuralları

- **Tablo:** Tablo tanımından hemen sonra `Table: Tablo başlığı buraya` şeklinde yazılır.
- **Figür:** `![Figür başlığı buraya](gorsel_yolu.png)` şeklinde yazılır.
- **Denklem:** Paragraf içinde tek başına `$$ ... $$` şeklinde blok denklem olarak yazılır.
