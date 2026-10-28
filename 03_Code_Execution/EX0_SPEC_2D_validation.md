# EX0_SPEC_2D_validation.md — 2B 6 elemanlı doğrulama örneği (eski Örnek 1 taslağı; kullanıcı kararı 2026-10-28: doğrulama örneği olarak tutulur)

Durum: **TASLAK, kullanıcı onayı bekler.** Etiketler: [K] = kullanıcı kararı, [T] = tezde yazan (doğrulanmamış), [V] = doğrulanmış kaynak, [VARSAYIM] = Claude önerisi, kanıtsız.

## Büyüklük
2 boyutlu düzlem çerçeve, **2 kat, tek açıklık**, **6 eleman (4 kolon + 2 kiriş)**, 4 tasarım grubu (1. kat kirişleri, 2. kat kirişleri, zemin kat kolonları, 1. kat kolonları). Tezde de aynı (Şekil 4.13, Çiz. 4.6) [T].

## Geometri
Açıklık 6 m; kat yükseklikleri 3 m + 3 m [K] (MakaleMetni §3.1.1; tez Şekil 4.13'ün 5 m ve 4+3 m değerleri kullanılmaz).

## Yük
- Kirişlerde G = 30 kN/m, Q = 20 kN/m [T, doğrulanmamış; ST'ye veya MakaleMetni'ne bağlanacak]. Kolon ve kiriş öz ağırlığı: ayrı mı, G içinde mi? [VARSAYIM: öz ağırlık ayrıca eklenir; kullanıcıdan teyit].
- Deprem: TBDY-2018 [K], Antalya Muratpaşa DD-2 ZC [K, V: AFAD_MURATPASA.md]: SDS = 0,718; SD1 = 0,226; TA = 0,063 s; TB = 0,315 s; TL = 6 s. I = 1 (BKS = 3) [VARSAYIM]; DTS = 2 [V: Tablo 3.2]; A11: R = 8, D = 3 [K, V]. Kütle: G + n·Q, n = 0,3 [VARSAYIM: TBDY Tablo 4.2 görüntü kontrolü gerekir]. Eşdeğer deprem yükü / modal RS (FemCore RS) [VARSAYIM]. Duyarlılık: DD-1 (SDS = 1,336; SD1 = 0,435) [K].
- Kombinasyonlar: 1,4G + 1,6Q; G + Q ± E; 0,9G ± E (TBDY/TS 498 biçimi) [T: Çiz. 4.5]; kapasite hesabı ACI 318-14 + TBDY Bölüm 7 kısıtları (A11_CONSTRAINTS.md) [K].

## Malzeme
- Beton: C30, C40 [T]; Ec = 4700 √fc' (ACI) [VARSAYIM; TS 500 Ec farkı raporlanır]. Lifli beton: 4D (fu = 1600) veya 5D (fu = 2300 N/mm²) [V: Evidence/Dramik4D, 5D], dozaj 20–40 kg/m³ (5'er) [T]; fR1, fR3: `CMOD_test_verileri.xlsx` [gizli üretici verisi; yayınlanmış veri ile çapraz kontrol bekliyor].
- Donatı: S420 (fy = 420 MPa), E = 200 GPa, etriye S420 [VARSAYIM: tezde Ö1 için yazmıyor].
- Çatlamış rijitlik: ACI 318-14 Tablo 6.6.3.1.1: kolon 0,70 Ig, kiriş 0,35 Ig [VARSAYIM; lifli kirişte aynı].

## Tasarım değişkenleri (havuz)
| Grup | Değişken | Aralık / adım | Not |
|---|---|---|---|
| Kolon (2 grup) | b, h | 300–700 mm, 50 mm; b ≥ 300 (TBDY 7.3.1.1) | tez: 250 → A11 ile uyumsuz |
| Kolon | boyuna donatı: Ø (14–32) ve adet; etriye Ø8–12, aralık | ρ %1–%4 (7.3.2.1) | |
| Kiriş (2 grup) | bw, h | bw 250–500, h 300–800 mm, 50 mm; 7.4.1.1 sınırları | tezde 250–600/225–1000 |
| Kiriş | üst/alt donatı Ø ve adet, etriye | 7.4.2 kuralları | |
| Kiriş | lif: tür (yok/4D/5D), dozaj | 0, 20, 25, 30, 35, 40 kg/m³ | lifsiz: dozaj 0 |
| Beton sınıfı | C30 / C40 (kolon ve kiriş ayrı olabilir) | | |

## Maliyet
`Cost.vb` / PRICES_2026.md: beton C25–C40 (15.150), donatı malzeme 24,2 TL/kg + işçilik (15.160.1003/1004), ahşap kalıp 873,03 TL/m² (15.180.1002), lif 90 TL/kg + duyarlılık; TL ana, USD bilgi (42,8843), göreli maliyet (lifsiz = 100).

## Açık girdiler (kullanıcı onayı / ST)
1. Yük değerleri G = 30, Q = 20 kN/m ve öz ağırlığın nasıl sayıldığı.
2. Beton sınıfı kümesi, S420, etriye çeliği.
3. Kütle katsayısı n ve BKS = 3 / I = 1 teyidi.
4. Kolon ve kiriş havuz sınırları (yukarıdaki tablo).
5. Lif tipi için fR1/fR3: CMOD dosyasındaki 4D/5D sayfaları ile hangi dozaj/sınıf birleşimleri dolu.


## Claude görüntü doğrulaması (2026-10-28)
- **TBDY Tablo 4.3** (PDF 68 / basılı 47): hareketli yük kütle katılım katsayısı n — konut, işyeri, otel, hastane, otopark vb. **0,30** [V]. (Önceki yazım "Tablo 4.2" yanlıştı; Tablo 4.2 etkin kesit rijitlikleridir.) Denk. 4.16: w = wG + n·wQ; çatı katında kar yükünün %30'u kütleye katılır.
- **TBDY Tablo 4.2** (PDF 68 / basılı 47): çerçeve kirişi eğilme 0,35, kolon 0,70; kesme 1,00 (kesit rijitliği çarpanları yalnız deprem birleşimlerinde, 4.5.8.3) [V] → ACI 6.6.3.1.1 ile aynı; Ö1 için TBDY değerleri esas.
- TS 498 (2021)  kullanıcı tarafından eklendi (28 sayfa); yük değerleri Hakem incelemesi sonrası işlenecek.
