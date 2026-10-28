# EX1_SPEC.md — Örnek 1 (özgün): 3B, 4 katlı betonarme konut çerçevesi

Durum: **TASLAK v2 (2026-10-28).** Kullanıcı onayları: plan (4 kat, 3×2 açıklık, 5×5 m, kat 3 m) ✔; 2B 6 elemanlı örnek doğrulama örneği olarak kalır (`EX0_SPEC_2D_validation.md`) ✔; deprem Muratpaşa DD-2 ZC ✔; beton C30/C40, S420, n = 0,30, boyut aralıkları, lif dozajı/tipleri ✔ (Hakem görüşüyle). Etiketler: [K] kullanıcı kararı, [V] sayfa görüntüsünden doğrulandı, [VARSAYIM] kanıtsız öneri (kaynak girilene dek makaleye alınmaz).

## Büyüklük
Perdesiz, düzenli, simetrik 3B çerçeve [K]: x yönünde 3 açıklık × 5,0 m, y yönünde 2 açıklık × 5,0 m, 4 kat × 3,0 m. **116 eleman** (48 kolon + 68 kiriş; kat başına 17 kiriş: x yönünde 3×3 = 9, y yönünde 2×4 = 8). Plan simetrik, burulma düzensizliği kısıtı kontrol edilir.

### Tasarım grupları (~10) [Hakem önerisi, onaya açık]
- Kolon: köşe / kenar / iç × (1–2. kat, 3–4. kat) = 6 grup.
- Kiriş: x yönü / y yönü × (1–2. kat, 3–4. kat) = 4 grup.
- Lif: yalnız kirişlerde (tip ve dozaj kiriş grubu başına ya da tek global; yöntem kararı Method Architect).

## Deprem (Muratpaşa DD-2, ZC) [K, V]
SDS = 0,718; SD1 = 0,226; TA = 0,063 s; TB = 0,315 s; TL = 6 s; DTS = 2 (Tablo 3.2); A11: R = 8, D = 3; I = 1, BKS = 3 (konut, Tablo 3.1). Kütle: wG + n·wQ, **n = 0,30** (TBDY Tablo 4.3, PDF 68 / basılı 47) [V]; çatıda kar yükünün %30'u (4.5.9.2). Etkin kesit rijitliği (TBDY Tablo 4.2): kiriş 0,35, kolon 0,70, kesme 1,00 (yalnız deprem birleşimlerinde, 4.5.8.3) [V]. Duyarlılık: DD-1 (SDS = 1,336; SD1 = 0,435). λ ≈ 0,40–0,41 → izinli kat ötelemesi ≈ 0,0195 h (A11_CONSTRAINTS C2). Kısıtlar: `A11_CONSTRAINTS.md`.

## Yükler (kaynaklı türetme)
Hareketli: konut döşemesi qk = **2,0 kN/m²** (TS 498:2021 Çizelge 6; PDF 21 / basılı 14) [V]. Hareketli yük azaltması (Çizelge 7, 4 kat β = 0,95) **uygulanmaz** (muhafazakâr, makalede belirtilir) [VARSAYIM, Hakem]. Kar: Muratpaşa kar bölgesi 1 (TS 498:2021 Ek 1, PDF 22) [V]; **kar yükü değeri ayrıca okunacak (BULUNAMADI)**, çatı kullanımı (kullanılmayan teras/çatı) kararı bekliyor.

Ölü yük (alan yükü; kolon-kiriş öz ağırlığı program kesitten hesaplar, ayrıca eklenmez):
| Kalem | Hesap | Değer | Kaynak |
|---|---|---|---|
| Betonarme birim hacim ağırlığı | 24 (normal beton) + 1 (donatı) | 25 kN/m³ | EN 1991-1-1 Tablo A.1 (PDF 34 / basılı 32): "Increase by 1 kN/m³ for normal percentage of reinforcing" [V]; TS ISO 9194 Ek A: tabii agregalı beton 2250–2500 kg/m³ [V] |
| Döşeme öz ağırlığı | h = 150 mm × 25 | 3,75 kN/m² | h = 150 mm [VARSAYIM: L/35 ≈ 143 mm ≤ 150]; hesaplama ETABS/FEM'de otomatik |
| Şap | 21 kN/m³ × 0,05 m | 1,05 kN/m² | çimento harcı: TS ISO 9194 2100 kg/m³ [V]; EN Tablo A.1 19–23 kN/m³ [V]; kalınlık 50 mm [VARSAYIM] |
| Zemin kaplaması | çimento karo 24 kN/m³ × 0,02 m | 0,48 kN/m² | TS ISO 9194 Ek A (kaplama): çimento karo 2400 kg/m³ [V, PDF 9]; 20 mm [VARSAYIM] |
| Tavan sıvası | 19 kN/m³ × 0,015 m | 0,29 kN/m² | EN Tablo A.1: kireç-çimento harcı 18–20 kN/m³ [V]; 15 mm [VARSAYIM] |
| Bölme duvarı (yayılı eşdeğer) | | 1,00 kN/m² | [VARSAYIM; EN 1991-1-1 6.3.1.2(8): ≤ 1,0 kN/m duvar için 0,5 kN/m² ek hareketli. Burada kalıcı yük sayılır] |
| **Normal kat toplamı (öz ağırlık hariç)** | 1,05 + 0,48 + 0,29 + 1,00 | **2,82 kN/m²** + döşeme 3,75 | |
| Dış duvar (kirişlerde çizgisel) | delikli tuğla 200 mm (13 kN/m³ [TS ISO 9194: 1150–1450 kg/m³ aralığı]) × 2,5 m + iki yüz sıva | ≈ 7,9 kN/m | duvar kalınlığı/yüksekliği ve sıva [VARSAYIM] |
Yük aktarımı: kiriş yükü FEM'e kN/m olarak (iki yönlü döşeme: üçgen/trapez eşdeğeri, hesap tabloda); final ETABS modelinde alan yükü + membran döşeme + rijit diyafram. FEM ve ETABS toplam kat ağırlığı eşitlenir (kontrol kapısı).

Birleşimler (TBDY/TS 498 biçimi): 1,4G + 1,6Q; G + Q ± E (E: x ve y, ±%5 ek dış merkezlik); 0,9G ± E; ACI kapasite hesabı + TBDY Bölüm 7 kısıtları.

## Malzeme [K]
Beton C30/C40 (kolon ve kiriş ayrı olabilir); donatı ve etriye S420, E = 200 GPa; Ec = 4700 √fc' (ACI) [VARSAYIM; TS 500 farkı raporlanır]. Lif tipleri 4D (fu = 1600 N/mm²) ve 5D (fu = 2300 N/mm²) [V: veri sayfası]; fR1, fR3 `CMOD_test_verileri.xlsx`; dozaj 0, 20, 25, 30, 35, 40 kg/m³. Lif yalnız kirişlerde.

## Tasarım değişkeni havuzu [K]
Kolon b, h = 300–700 mm (50 mm); ρ %1–4; kiriş bw = 250–500, h = 300–800 mm (50 mm); TBDY 7.3 / 7.4 kısıtları (A11_CONSTRAINTS). Gruplama (6 kolon + 4 kiriş) yukarıda.

## Maliyet
`Cost.vb` / PRICES_2026.md. Beton C25–C40 (15.150), donatı malzeme 24,2 TL/kg + işçilik, ahşap kalıp 873,03 TL/m², lif 90 TL/kg + duyarlılık; TL ana, USD bilgi, göreli maliyet (lifsiz = 100). Metraj: kirişlerde beton + donatı + etriye + kalıp, kolonlarda aynı; döşeme sabit (optimizasyon dışı, maliyete katılmaz).

## Açık kalemler
1. Kar yükü (TS 498:2021) ve çatı kullanım kararı.
2. Döşeme kalınlığı, şap/kaplama/sıva kalınlıkları, bölme ve dış duvar yükü [VARSAYIM] → kullanıcı onayı; duyarlılık (G ±%20).
3. TBDY DTS = 2 için bina yükseklik sınıfı (Tablo 3.3) ve 7.2 minimum beton sınıfı kontrolü (DOĞRULANAMADI).
4. Torsiyon düzensizliği (ηb) kısıtı ve ek dış merkezlik.
5. fR1/fR3 için 4D/5D dozaj–sınıf tablosunun doluluğu (CMOD dosyası).
