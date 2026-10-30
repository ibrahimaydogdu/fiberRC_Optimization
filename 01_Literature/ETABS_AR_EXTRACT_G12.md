# G12: CSI Analysis Reference Manual çıkarımı (Claude, 2026-10-30)

Kaynak: `Evidence/DesignGuides/CSI Analysis Reference Manual.pdf` · SHA-256 `f50d9a87bad14f817304e2a50e8622790da9261d581df5aeb854c4390c49becf` · 9 857 164 bayt · 477 PDF sayfası · "ISO# GEN062708M1 Rev.20, March 2025" (SAP2000, ETABS, SAFE, CSiBridge, CSiPlant ortak). Yöntem: `pdftotext -layout` + PyMuPDF sayfa eşleme. Not: ETABS'e özgü bölümler (ör. otomatik ek dış merkezlik) bu ortak kılavuzda yer almaz.

## 1. Rijit uç bölge: eksenel ve burulma
- Bölüm 7.10 End Offsets, "Effect upon ..."; PDF s. 131–132 (basılı 7-23/7-24).
- Alıntı: "The rigid-zone offsets never affect axial and torsional deformation. The full element length is assumed to be flexible for these deformations." (PDF s. 132)
- Alıntı: "...fraction of each end offset that is assumed to be rigid for bending and shear deformation." (PDF s. 131)
- Denk. 7-7: Lf = L − rigid·(ioff + joff) (eğilme ve kesme esnek boyu).
- 7.10.4: "All internal forces and moments are output at the faces of the supports... No output is produced within the end offset." (PDF s. 132)
- Durum: **DOĞRULANDI**. V2a Bulgu 1 (FemCore'da A ve J'nin tüm boyda esnek alınması) kılavuzla uyumlu.
- Not: rigid-end factor (0–1) kavramı var; bizim ETABS modelinde değeri OAPI ile atanıp atanmadığı EtabsProbe'da (ve FemCore'da rijit uzunluğun tamamı) **kayıtlıdır**, tasarımda kullanılan faktör kılavuz önerisi (≤ ~0,5, s. 132) ile **farklı olabilir**; teyit edilecek (FemCore ve ETABS aynı faktörü kullanıyor → V2a'da fark yok; fakat makalede gerekçe gerekir).

## 2. Diyafram, kütle atalet momenti
- Bölüm 5.12 Constraint kütle/rijitlik; PDF s. 76 (basılı 5-15/5-16).
- Alıntı: "The moments of the translational stiffnesses, masses, and loads are taken about the center of mass of the constrained joints. If the joints have no mass, the centroid is used."
- Dönel kütle: kısıtlı düğümlerin ötelenme kütlelerinin ikinci momenti ana düğüme eklenir. Durum: **DOĞRULANDI** (V2a'da kütle atalet uyumu: RS dış merkezliksiz ≤%0,09).

## 3. Kütle kaynağı
- 17.21 Mass Source, PDF s. 305. Üç katkı: Element Self Mass, Additional Mass, Specified Load Patterns; varsayılan: ilk ikisi. Yükten kütle: yük/g, düğümlere eşit dağıtılır, her üç ötelenmeye etkir. Durum: **DOĞRULANDI**.
- Uyarı (aynı bölüm): öz kütleyi hem malzemeden hem yük örüntüsünden sayma. Ex1'de kütle kaynağı tablosu ile kontrol edildi (V2a).

## 4. Ek dış merkezlik (%5), RS burulma etkisi
- Kılavuzda aranan terimler: "accidental", "eccentricity ratio", RS eccentricity. Sonuç: **BULUNAMADI**. Tek eşleşen PDF s. 461 köprü hareketli yük bölümüdür (ilgisiz). 
- Sonuç: FemCore ↔ ETABS %2–6'lık fark bu kılavuzla **açıklanamaz**. Gerekli kaynak: ETABS Help ("Diaphragm Eccentricity", "Response Spectrum Case Data: Eccentricity"), ETABS Verification/Theory notu ya da CSI teknik bilgi tabanı.

## 5. Özet tablo
| Kalem | Durum |
|---|---|
| Rijit bölgede eksenel/burulma tam esnek | DOĞRULANDI |
| Rigid-end faktörü tanımı (Denk. 7-7) | DOĞRULANDI |
| Diyafram kütle momenti merkezi | DOĞRULANDI |
| Mass Source tanımı | DOĞRULANDI |
| %5 ek dış merkezlik | BULUNAMADI |
