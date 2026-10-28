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

## Yükler (kaynaklı türetme; kullanıcı kararı: Türk standartları, 15 cm döşeme, yatay delikli tuğla/izotuğla 8,5 ve 13,5 cm, düz çatı)
Hareketli: konut döşemesi qk = **2,0 kN/m²** (TS 498:2021 Çizelge 6; PDF 21 / basılı 14) [V]. Hareketli yük azaltması (Çizelge 7, 4 kat β = 0,95) **uygulanmaz** (muhafazakâr, makalede belirtilir) [VARSAYIM, Hakem].
Kar (düz çatı, kullanılmayan teras): Muratpaşa → kar bölgesi 1 (TS 498:2021 Ek 1, PDF 23 / basılı 16) [V]; rakım 0–200 m → **Sk = 0,75 kN/m²** (Çizelge 3, PDF 16 / basılı 9; bölge 1, 0–200 m) [V]. Şekil katsayısı μ (0–30° çatı: 0,8) TS EN 1991-1-3'te; **o standart projede yok → BULUNAMADI**; muhafazakâr olarak **s = Sk = 0,75 kN/m²** alınır [VARSAYIM]. Çatı hareketli yükü olarak kar 0,75 kN/m²; teras kullanımı olsaydı 2,0 kN/m² olurdu (TS 498 çatı satırı okunmadı).

Ölü yük (alan yükü; kolon-kiriş-döşeme öz ağırlığı program/ETABS hesaplar):
| Kalem | Hesap | Değer | Kaynak |
|---|---|---|---|
| Betonarme birim hacim ağırlığı | 24 + 1 | 25 kN/m³ | EN 1991-1-1 Tablo A.1 (PDF 34 / basılı 32) [V]; TS ISO 9194 Ek A: tabii agregalı beton 2250–2500 kg/m³ (PDF 8 / basılı 4) [V] |
| Döşeme öz ağırlığı | h = 150 mm [K] × 25 | 3,75 kN/m² | otomatik |
| Şap (normal kat) | 21 kN/m³ × 0,05 m | 1,05 kN/m² | çimento harcı 2100 kg/m³ (TS ISO 9194, PDF 8) [V]; 50 mm [VARSAYIM, yaygın uygulama] |
| Zemin kaplaması | 24 kN/m³ × 0,02 m | 0,48 kN/m² | çimento karo 2400 kg/m³ (TS ISO 9194, PDF 9) [V]; 20 mm [VARSAYIM] |
| Tavan sıvası | 19 kN/m³ × 0,015 m | 0,29 kN/m² | EN Tablo A.1 kireç-çimento harcı 18–20 kN/m³ [V]; 15 mm [VARSAYIM] |
| İç bölme duvarı (8,5 cm yatay delikli tuğla) | tuğla 10,9 kN/m³ × 0,085 = 0,93 + iki yüz sıva 3 cm × 19 = 0,57 → 1,50 kN/m² duvar; × 2,5 m (3,0 m − 0,5 m kiriş) = 3,75 kN/m; plandaki bölme yoğunluğu 0,35 m/m² | **1,3 kN/m²** (yayılı) | tuğla: TS ISO 9194 boşluklu tuğla 820–1350 kg/m³, orta değer 1085 kg/m³ [V aralık, VARSAYIM orta değer]; 0,35 m/m² [VARSAYIM] |
| **Normal kat toplamı (öz ağırlık hariç)** | 1,05 + 0,48 + 0,29 + 1,3 | **3,12 kN/m²** | |
| Dış duvar (kirişlerde çizgisel) | 13,5 cm yatay delikli tuğla: 10,9 × 0,135 = 1,47 + sıva (iç 1,5 + dış 2,5 cm) × 19 = 0,76 → 2,23 kN/m² × 2,5 m | **5,6 kN/m** | aynı kaynaklar; kalınlıklar [K] |
| Çatı (düz teras) | eğim betonu/koruma şapı ≈ 0,10 m × 21 = 2,1; yalıtım + su yalıtımı ≈ 0,15; tavan sıvası 0,29 | **≈ 2,5 kN/m²** (öz ağırlık hariç) | [VARSAYIM, kalınlıklar kaynak girilene dek makaleye alınmaz; ±%20 duyarlılık] |
Yük aktarımı: kiriş yükü FEM'e kN/m olarak (iki yönlü döşeme: üçgen/trapez eşdeğeri, hesap tabloda); final ETABS modelinde alan yükü + membran döşeme + rijit diyafram. FEM ve ETABS toplam kat ağırlığı eşitlenir (kontrol kapısı). Birim ağırlıklar TS ISO 9194 aralıklarının orta değeridir; G için ±%20 duyarlılık koşulur.

Birleşimler (TBDY/TS 498 biçimi): 1,4G + 1,6Q; G + Q ± E (E: x ve y, ±%5 ek dış merkezlik); 0,9G ± E; ACI kapasite hesabı + TBDY Bölüm 7 kısıtları. Deprem kütlesi: wG + 0,30 wQ (çatıda kar %30).

## TBDY sınıflama (Claude görüntü doğrulaması)
- **Tablo 3.3** (PDF 38 / basılı 17) [V]: HN = 4 × 3,0 = 12 m, DTS = 2 sütunu (1, 1a, 2, 2a): 10,5 < HN ≤ 17,5 → **BYS = 6**. Tablo 4.1 A11 için BYS ≥ 3 koşulu sağlanır.
- **TBDY 7.2** (PDF 132–134 / basılı 111–113) [V]: 7.2.5.1 "C25'ten daha düşük dayanımlı beton kullanılamaz"; 7.2.5.3(a) TS EN 206 C25–C80; (b) TS 708 B420C ve B500C donatı; S420 yalnız Rm/Re ≥ 1,35 ve karbon eşdeğeri ≤ %0,55 ile kullanılabilir. → **C30/C40 uygun; donatı B420C (fy = 420 MPa) alınır, "S420" yazılmaz** [VARSAYIM tercih: B420C].
- **TBDY 7.2.4:** "kesit hesaplarında TS 500'deki kabuller ve elastisite modülü kullanılması zorunludur". **Bizim kapasite hesabımız ACI 318-14 → TBDY'den bilinçli sapma;** makalede açıkça yazılır ve ACI/TS 500 fark duyarlılığı raporlanır (Critic'e bildirildi).

## Malzeme [K]
Beton C30/C40 (kolon ve kiriş ayrı olabilir); donatı ve etriye B420C (fy = 420 MPa), E = 200 GPa; Ec = 4700 √fc' (ACI) [VARSAYIM; TS 500 farkı raporlanır]. Lif tipleri 4D (fu = 1600 N/mm²) ve 5D (fu = 2300 N/mm²) [V: veri sayfası]; fR1, fR3 `CMOD_test_verileri.xlsx`; dozaj 0, 20, 25, 30, 35, 40 kg/m³. Lif yalnız kirişlerde.

## Tasarım değişkeni havuzu [K]
Kolon b, h = 300–700 mm (50 mm); ρ %1–4; kiriş bw = 250–500, h = 300–800 mm (50 mm); TBDY 7.3 / 7.4 kısıtları (A11_CONSTRAINTS). Gruplama (6 kolon + 4 kiriş) yukarıda.

## Maliyet
`Cost.vb` / PRICES_2026.md. Beton C25–C40 (15.150), donatı malzeme 24,2 TL/kg + işçilik, ahşap kalıp 873,03 TL/m², lif 90 TL/kg + duyarlılık; TL ana, USD bilgi, göreli maliyet (lifsiz = 100). Metraj: kirişlerde beton + donatı + etriye + kalıp, kolonlarda aynı; döşeme sabit (optimizasyon dışı, maliyete katılmaz).

## Lif veri ızgarası (Claude kontrolü, CMOD_test_verileri.xlsx; üretici verisi, 65/60 BG, MPa)
- **4D 65/60 BG:** C30/37 için dozaj 20, 25, 30, 35, 40 kg/m³ dolu; C40/50 için 20, 25, 30, 35 dolu, **40 yok**; C25/30 ve C35/45 yok. fR1 (C30): 2,7 / 3,3 / 3,8 / 4,3 / 4,7.
- **5D 65/60 BG:** C30/37 ve C40/50 için yalnız 25, 30, 35 kg/m³; **20 ve 40 yok**. fR1 (C30): 3,1 / 3,6 / 4,1.
- Yayınlanmış Venkateshwaran 2018 verisi (sayfa 4D_/5D_) 35/60 ve 80/60 geometri, C35/45 üzerinde → 65/60 verisi için **doğrudan çapraz kontrol değil.**
- **Öneri:** tasarım uzayı ölçülen aralıkla sınırlı (ekstrapolasyon yok): C30: 4D 20–40, 5D 25–35; C40: 4D 20–35, 5D 25–35; beton C30/C40 ile örtüşüyor. Aralık dışı dozaj uygunsuz sayılır. [VARSAYIM; kullanıcı onayı]

## Açık kalemler
1. Düz çatıda μ = 0,8 (TS EN 1991-1-3) ve teras kullanımı; kalınlıklar ve duvar/bölme yoğunluğu [VARSAYIM] → G ±%20 duyarlılık.
2. TBDY 7.2.4 / ACI sapması makalede gerekçelendirilecek.
3. Torsiyon düzensizliği (ηb) kısıtı ve ek dış merkezlik (plan simetrik).
4. Lif dozaj uzayının ölçülen aralıkla sınırlanması (yukarıdaki öneri) kullanıcı onayı.
5. Lifin sarılma bölgesinde Vc = 0 ve VRd,F (A11_CONSTRAINTS C1e) Critic kararı.
