# 💰 UNIT_PRICE_EVIDENCE.md (birim fiyat kanıt raporu, 2026-10-27)

Kural: `.agents/rules/anti_hallucination_evidence.md`. Birincil kaynak = ÇŞİDB (Çevre, Şehircilik ve İklim Değişikliği Bakanlığı) Birim Fiyatları PDF'leri. `beton donatı birim maliyetleri.xlsx` ST'nin türev tablosudur, **kanıt değildir**.

## 1. Belgeler (`Evidence/BirimFiyatlar/`)
| Dosya | Bayt | SHA-256 |
|---|---|---|
| 1--2024-birim-fiyatlar-20240123142524.pdf (648 s., 01.01.2024) | 13.720.804 | 642ece84b6c06289b119bff30087f5db4e3b854479bbfaee3f40344146d939c8 |
| 2023-2-birim-fiyatlar-170723-20230717105538.pdf | 9.648.497 | 9453e323b2083fb71319fa767f092a10fc99c74a2d7c99abd7a6e48a4ae293f3 |
| 2023_1-b-r-m-f-yatlari-1-20230130125553.pdf | 11.176.911 | 626dee41df250a9af76b154471d65f452d95ba46b24f60f61f7208a1e3ef0353 |
| bf-2022-3-turkce-20220907143000.pdf | 8.759.907 | 8e41901aa03865db563949c91a73f6a89e65b040701c17a582092c2e2b669baf |
| bf-2022-2-turkce-20220907142858.pdf | 6.100.097 | bdcc2a4e00aba7b8eced23c4b645eeae82ecc8e4640aa33c13b5826c7c630ec7 |
| bf-2022-1-turkce-20220907142750.pdf | 14.223.489 | f7a7f73f13dc019081f491994f117ea9552f5a3a38784dc65755bb30dbec44b4 |
| beton donatı birim maliyetleri.xlsx (ST türevi) | 23.316 | e933afbb7f1d8ed3d250df49a9253704b5cb315960c8aa7feea7242baeb0cee6 |

## 2. 2024 bulguları (PDF sayfası görsel olarak okundu; metin çıkarımı sütunları kaydırdığı için **görüntü esas alındı**)
| Kalem | Poz | Fiyat | Birim | Yer | Belge, sayfa | Durum |
|---|---|---|---|---|---|---|
| C30/37 hazır beton (pompa hariç) | 10.130.1506 | 2.020,00 TL | m³ | işbaşında | 2024 PDF s.21 (basılı -15-) | DOĞRULANDI |
| C40/50 hazır beton | 10.130.1508 | 2.280,00 TL | m³ | işbaşında | s.21 | DOĞRULANDI |
| C25/30, C35/45 | 1505 / 1507 | 1.950 / 2.140 TL | m³ | işbaşında | s.21 | DOĞRULANDI |
| Nervürlü donatı Ø8–12 / Ø14–32 (malzeme) | 10.130.1704 / 1705 | 18,70 TL | kg | **fabrikada** | s.22 (-16-) | DOĞRULANDI |
| Donatı kesme-bükme-yerleştirme Ø8–12 | 15.160.1003 | 33.511,95 TL | ton | iş | s.177 (-164-) | DOĞRULANDI |
| Aynı, Ø14–28 | 15.160.1004 | 32.514,45 TL | ton | iş | s.177 | DOĞRULANDI |
| Ahşap düz yüzeyli betonarme kalıbı | 15.180.1002 | 524,41 TL | m² | iş | s.177 | DOĞRULANDI |
| Plywood kalıp | 15.180.1003 | 634,51 TL | m² | iş | s.177 | DOĞRULANDI |
| **Çelik tel (TS EN 14889-1) — tutkalsız, iki ucu kancalı** 0,75×60 mm | 10.300.4023 | **49,00 TL** | kg | işbaşında | s.92 (-86-) | DOĞRULANDI |
| Aynı, tutkallı 0,75×60 mm | 10.300.4013 | 58,00 TL | kg | işbaşında | s.92 | DOĞRULANDI |
| Aynı, 0,55×30/35 / 0,75×30/35 / 0,90×60 tutkalsız | 4021 / 4022 / 4024 | 60 / 52 / 43 TL | kg | işbaşında | s.92 | DOĞRULANDI |
| Deforme çelik tel tutkallı / tutkalsız | 4001 / 4002 | 50 / 45 TL | kg | işbaşında | s.92 | DOĞRULANDI |
| Sentetik makro fiber, TS EN 14651+A1 CMOD kriterli | 10.300.4121 | 760,00 TL | m³ beton | işbaşında | s.92 | DOĞRULANDI (sentetik; çelik lif değil) |
| Lifli betonun **hazır beton fiyat farkı** (karışım, pompa, işçilik) | – | – | – | – | – | BULUNAMADI (ÇŞİDB'de ayrı poz görülmedi) |

**Sonuç:** Çelik lif birim fiyatı için ÇŞİDB 2024 birincil kaynağı bulundu; tedarikçi teklifine gerek kalmadı. Lif türü/boyu tezdeki lifle eşleşmelidir (bkz. P6).

## 3. ST'nin Excel'i hakkında bulgular
1. 2024 sütunu (beton C30/37–C50/60, donatı 18,70) PDF ile **uyumlu** (s.21–22).
2. Beton **pompa hariç**, donatı yalnız **malzeme (fabrikada)**; kesme-bükme-yerleştirme (15.160.1003/1004: ≈33,5 TL/kg) ve **kalıp** Excel'de yok. Maliyet modeli bu kalemleri içeriyor mu, kodda kontrol edilecek (`CODE_REVIEW_v2.md`).
3. Excel yıllar arası TL fiyatları dolar kuruna bölüp **ortalama** alıyor (`O10…O14`). Kur aralığı ve yıl karışımı, oranı bozar; 2022-1/2022-2 sütunlarında boş hücreler (C30/37 var, C25/30 yok) bulunuyor. Kullanım: yalnız tek yıl (2024) TL fiyatı; dolar çevrimi yapılacaksa tek resmî kur (TCMB) kaynağıyla.
4. `dolar kuru` sayfasında `#VALUE!` hücresi (A47) ve `AVERAGE(B2:E5)` biçiminde anlamsız formül (H1). Kur değerleri kaynağı belirtilmemiş: `ERİŞİLEMEDİ` (TCMB teyidi gerekir).
5. 2022–2023 sütunları bu raporda **DOĞRULANAMADI** (metin çıkarımı güvenilmez; sayfa sayfa görsel doğrulama gerektirir). Yalnız 2024 kullanılacaksa gerekmez.

## 4. Açık kararlar
- Tezde kullanılan fiyat yılı (2024 mü, başka mı) → kodun sabitleri ve tez tablosu ile eşleştirilecek (D6).
- Hangi kalemler dahil: beton + donatı malzemesi + donatı işçiliği + kalıp + lif. Pompa ve lifli beton karışım farkı ayrı poz yok; varsayım yazılı olacak.

## 5. Lif verisi (`Evidence/CMOD_test_verileri.xlsx`) — kırmızı bayrak (2026-10-27)
- Dosya: sayfalar `RC_becart`, `4D_becart`, `5D_becart`, `3D_`, `4D_`, `5D_`; fR1–fR4 (ve fRL) tabloları, dozaj (kg/m³) × beton sınıfı. 3D/4D/5D sayfalarında "VENKATESHWARAN ET AL." notu var (yayınlanmış kaynak olabilir; DOĞRULANAMADI).
- Sorunlar: (1) Ham test yok, "yüzlerce testin ortalaması" iddiası doğrulanamaz; (2) firma adı gizli, ama 3D/4D/5D ve "65/60, 80/60, BG" adlandırması tedarikçinin ticari ürün kodlarıdır — adı saklamak okurun ürünü tanımasına engel olmaz, ayrıca ticari taraf verisi çıkar çatışması doğurur; (3) kaynak belge ve tarih yok; (4) dozaj/sınıf aralığında boş hücreler var (ör. 4D/5D C20–C25, 40 kg/m³).
- Öneri: Kaynak türü "üretici tarafından sağlanan tasarım verisi (kişisel iletişim)" olarak açıkça yazılır; Structures'ın veri/çıkar beyanı kurallarına uyulur; mümkünse aynı değerler yayınlanmış bir kaynakla (ör. Venkateshwaran ve ark.) çapraz doğrulanır; hakem istekleri için fR1/fR3 duyarlılık analizi eklenir. Yayın öncesi firmanın yazılı izni (ad belirtme veya belirtmeme) alınır. Anonimleştirme kararı yazarların (IA) sorumluluğundadır.

## 6. Lif verisi kaynağı — çözüm (2026-10-27, 3. tur)
- Bekaert izin vermiyor. **Çözüm:** IA'nın `SedaDoktora2/Manuscript_04_09_2026.docx` makalesi aynı durumu çözmüş: DataSet1 = üretici verisi (gizli; Data Availability Statement ile "ticari gizlilik"), DataSet2 = Venkateshwaran, Tan & Li (2018), *Structural Concrete* 19, 352–365; DataSet3 = Chen vd. (2021), *Structures* 33, 3644–3654. Bu makalenin Tablo 2'sindeki değerler (fRL 3,84; fR1 2,38; fR3 2,76 … lif 20 kg/m³, f'c 38,6 MPa, d=0,9 mm, L=60 mm) `CMOD_test_verileri.xlsx` `3D_` sayfasıyla **eşleşiyor** → bu sayfalar yayınlanmış veridir.
- **Karar önerisi:** Çerçeve makalesinde yalnız yayınlanmış veriler (Venkateshwaran 2018; Chen 2021) birincil kaynak olur; Bekaert verisi kullanılırsa DataSet1 gibi "gizli üretici verisi" olarak beyan edilir. Künyeler Crossref ile doğrulanacak (Literature ajanı).
- **Düzeltme:** Önceki "3D → 0,75 mm" çıkarımı hatalıydı: Dramix 3D hem 65/60 (d≈0,90) hem 80/60 (d=0,75) olabilir; Tablo 2'ye göre kullanılan lif d=0,9×60 mm → ÇŞİDB poz 10.300.4014 (tutkallı, 55 TL/kg, 2024) veya 4024 (tutkalsız 43). 2026 pozları aynı pozla yeniden okunacak.
- Tezdeki 3,50/4,75 $/kg kaynaksız ve ÇŞİDB 2024'ten (≈1,7 $/kg) 2–2,8 kat yüksek. ÇŞİDB lif sınıfına göre fiyat ayırmaz → lif fiyatı duyarlılık senaryosu zorunlu.
- Fiyat yılı: **2026** (kullanıcı kararı). Önce tezi tezin kendi fiyatlarıyla yeniden üret, sonra 2026'ya geç.
