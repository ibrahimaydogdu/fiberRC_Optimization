# PRICES_2026.md — ÇŞİDB 2026 birim fiyat kanıtı (Cost modülünün tek kaynağı)

Doğrulayan: Claude Code, 2026-10-28. Yöntem: sayfa görüntüsü (PyMuPDF 100 dpi) okundu; **metin katmanı etiket/fiyat kaydırdığı için kullanılmadı** (ör. metin katmanı C30/37'yi 1505, ahşap kalıbı plywood olarak etiketliyordu). Kural: `.agents/rules/anti_hallucination_evidence.md`.

## Belge
| Dosya | Bayt | SHA-256 | Not |
|---|---|---|---|
| `Evidence/BirimFiyatlar/2026/CSIDB_BirimFiyatlar_2026_v20260119.pdf` (kopya; kaynak: `Steel_Composite_Hybrid_Frame_Optimization/Evidence/UnitPrices/Turkiye/CSIDB_BF_2026/`) | 15 596 975 | d8495a6b117111a4d2c9ed975527512c1dccd8371c2bb40c6d21700af0aeb307 | Sayfa üstü tarih "01.01.2026"; 741 sayfa. Değiştirilme: 2026-01-19 (PDF modDate). |
| `…_v20260115.pdf` (aynı klasör) | 15 596 850 | 5320ce56507d10b56914fed1a623bbe1a3c76060373ab9a8920153304499bf34 | Eski sürüm; v0119'dan fark yalnız PDF s.35 ve s.191'de iskele kalemlerinin birimi (m²→m³). Fiyatlar aynı. **Esas: v20260119.** |

Sayfa sütunu "PDF n / basılı m".

## Fiyatlar (2026, TL; görüntüden okundu)
| ID | Poz | Tanım (görüntüden) | Birim | Fiyat | Sayfa | Kullanım |
|---|---|---|---|---|---|---|
| P-C25 | 10.130.1505 | C 25/30 beton harcı (gri, normal, pompa hariç) | m³ | 2.600,00 | PDF 22 / 15 | Malzeme |
| P-C30 | 10.130.1506 | C 30/37 beton harcı (pompa bedeli hariç) | m³ | **2.700,00** | PDF 22 / 15 | Malzeme |
| P-C35 | 10.130.1507 | C 35/45 beton harcı | m³ | 2.850,00 | PDF 22 / 15 | Malzeme |
| P-C40 | 10.130.1508 | C 40/50 beton harcı | m³ | 2.950,00 | PDF 22 / 15 | Malzeme |
| P-D30 | 15.150.1006 | Hazır beton C 30/37, **pompayla basılan, dökülmesi (nakil dahil)** | m³ | **3.649,35** | PDF 189 / 175 | Yerinde beton (malzeme+döküm) |
| P-D35 | 15.150.1007 | aynı, C 35/45 | m³ | 3.836,85 | PDF 190 / 176 | |
| P-D40 | 15.150.1008 | aynı, C 40/50 | m³ | 3.961,85 | PDF 190 / 176 | |
| P-S1 | 10.130.1704 | Beton çelik çubuğu nervürlü Ø 8-12 mm (S420, B420B-C, B500B-C), Fabrikada | kg | **24,20** | PDF 23 / 16 | Donatı malzeme |
| P-S2 | 10.130.1705 | aynı, Ø 14-32 mm | kg | **24,20** | PDF 23 / 16 | Donatı malzeme |
| P-L1 | 15.160.1003 | Ø 8–Ø 12 nervürlü çubuk kesilmesi, bükülmesi, yerine konulması | ton | 46.482,43 | PDF 191 / 177 | Donatı işçilik |
| P-L2 | 15.160.1004 | Ø 14–Ø 28 nervürlü çubuk kesilmesi, bükülmesi, yerine konulması | ton | 44.681,18 | PDF 191 / 177 | Donatı işçilik |
| P-F1 | 15.180.1002 | Ahşaptan düz yüzeyli beton ve betonarme kalıbı yapılması | m² | **873,03** | PDF 191 / 177 | Kalıp |
| P-F2 | 15.180.1003 | Plywood ile düz yüzeyli betonarme kalıbı | m² | 1.028,33 | PDF 191 / 177 | Kalıp (alternatif) |
| P-FB | 10.300.4014 | Tutkallı ve iki ucu kancalı tel (TS EN 14889-1), **0,90 mm çap, 60 mm boy** | kg | **90,00** | PDF 95 / 88 | Lif (Dramix 3D/4D/5D 65/60 geometrisi) |
| P-FB2 | 10.300.4024 | Tutkalsız, 0,90 mm × 60 mm | kg | 80,00 | PDF 95 / 88 | Lif duyarlılığı |

## Kararlar gerektiren noktalar
1. **Beton çift sayım riski:** 10.130.1506 (2.700; yalnız malzeme, pompa hariç) ile 15.150.1006 (3.649,35; malzeme + pompa + döküm) birlikte kullanılamaz. Fark 949,35 TL/m³ = döküm, nakil, pompa. **Öneri:** tüm yerinde beton için 15.150 (P-D*) kullanmak; kalıp ve donatı işçiliği ayrı kalemlerde. Karar kullanıcıdadır (D-fiyat).
2. **Lif maliyeti:** Lifli betonda ek kalem yalnız lif (kg/m³ × 90 TL/kg). ÇŞİDB'de "lifli beton" için ayrı bir fark kalemi yok (P5, BULUNAMADI). Dozaj kg/m³ olarak girdi (fR1/fR3 ↔ dozaj eşlemesi veri kaynağından).
3. **Donatı işçilik** ton cinsinden, malzeme kg cinsinden: birim dönüşümü Cost modülünde açık yazılacak (1 ton = 1000 kg).
4. **Etriye** Ø8–12 sınıfına girer (P-S1/P-L1); boyuna donatı çapına göre P-S1/P-S2 ve P-L1/P-L2 ayrımı gerekir (Ø8–12 ile Ø14–28 ayrı işçilik fiyatı). Ø28–32 aralığı: malzeme P-S2, işçilik 15.160.1005 (Ø28 üstü: 43.749,93 TL/ton; metin katmanından, görüntüde doğrulanmadı) — katalogda Ø>28 yoksa gerekmez.
5. Zaman duyarlılığı için 2025 listesi (aynı poz düzeni; örn. 10.300.4014 = 75,00 TL/kg, 10.130.1704/1705 = 21,00 TL/kg, 15.160.1003 = 39.429,35 TL/ton, 15.180.1002 = 705,75 TL/m²) metin katmanından alındı, **görüntüyle doğrulanmadı** → duyarlılık analizi öncesi doğrulanmalı.
6. Tez fiyatları (2024) bu dosyadaki 2026 fiyatlarıyla karşılaştırılacak (WP5); farklar fiyat güncellemesi olarak raporlanır.

## Kur (USD gösterimi için; Hakem kararı 2026-10-28: KOŞULLU KABUL)
Karar: Optimizasyon ve ana tablolar **TL**; USD yalnız bilgi amaçlı ek sütun; ayrıca göreli maliyet (lifsiz = 100). Tek sabit kur; yıl ortalaması kullanılmaz. Tüm kalemler aynı kurla çarpıldığı sürece optimum değişmez (Penalty çarpımsal kalmalı: Method ajanı teyidi bekler). ST'nin yıl karışımı/ortalama kur Excel'i kullanılmaz.

Birincil kaynak: TCMB günlük kur bülteni (XML), `https://www.tcmb.gov.tr/kurlar/YYYYMM/GGAAYYYY.xml`; paracevirici.com ikincil/ticari olduğu için atıf kaynağı değildir.
| Dosya | Bayt | SHA-256 | İçerik |
|---|---|---|---|
| `Evidence/Kur/TCMB_kur_31122025.xml` | 9 489 | c2d74ec39e8373e030e2a29c1f97d6680af58333aa4749d0e36edf41a63666ba | Bülten 2025/249, 31.12.2025; USD Döviz Alış 42,8457; Döviz Satış 42,9229; Efektif Alış 42,8157; Efektif Satış 42,9873 (TL) |
| `Evidence/Kur/TCMB_kur_02012026.xml` | 9 092 | 5e18fa062e4e134d6f931522eb3e9ef9f4d3965058c323ddc96a7bca12ebe0df | 02.01.2026; USD Döviz Alış 42,8810; Döviz Satış 42,9582 (karşılaştırma) |
- 01.01.2026 için TCMB bülteni yok (`/kurlar/202601/01012026.xml` → HTTP 404, erişim 2026-10-28) → liste tarihine en yakın önceki yayın günü **31.12.2025**.
- Döviz Alış/Satış seçimi tek cümleyle makalede belirtilir. **Öneri: Döviz Alış, 31.12.2025 = 42,8457 TL/USD** (kullanıcı onayı bekler). Alış-satış farkı %0,18; sonucu etkilemez.
- Lif fiyatının üreticide hangi para birimine endeksli olduğu DOĞRULANAMADI → lif fiyatı duyarlılık senaryosu şart.
- Excel'de kur tek hücrede, tüm USD sütunları formülle (`=TL/Kur`) üretilir.

## Kullanıcı kararları (2026-10-28) ve lif duyarlılığı
1. **Beton:** 15.150 (pompa, nakil, döküm dahil) esas: C25/30 3.524,35 · C30/37 3.649,35 · C35/45 3.836,85 · C40/50 3.961,85 TL/m³ (PDF 189–190 / 175–176, görüntüden). 10.130 (yalnız malzeme) kullanılmaz; çift sayım yok.
2. **Kur:** TCMB 31.12.2025 USD Döviz Alış ile Döviz Satış'ın aritmetik ortalaması = (42,8457 + 42,9229)/2 = **42,8843 TL/USD**; makalede "TCMB forex buying/selling mean" yazılır.
3. **Lif fiyatı duyarlılık senaryosu (zorunlu):** baz 90,00 TL/kg (10.300.4014, 2026). Senaryolar çarpanla: ×0,50 · ×0,75 · ×1,00 · ×1,25 · ×1,50 · ×2,00 (`PriceSet.WithFibreFactor`). Adlandırılmış karşılaştırma noktaları: tutkalsız 10.300.4024 = 80,00 TL/kg (×0,889; görüntüden doğrulandı); 2025 listesi 10.300.4014 = 75,00 TL/kg (×0,833; metin katmanından, görüntüyle doğrulanmadı). Her çarpan için: optimum tasarım, lifli/lifsiz maliyet oranı ve lifin maliyet avantajını yitirdiği **başabaş lif fiyatı** raporlanır. Üretici fiyatı ÇŞİDB'den yüksekse (lif USD/EUR'a endeksli olabilir, DOĞRULANAMADI) ×2,00 senaryosu bunu kapsar.
4. Kod: `RcCore/Cost.vb` (`PriceSet`, `Cost.Member`); test T4 (elle hesap). Başka yerde fiyat yazılmaz.
