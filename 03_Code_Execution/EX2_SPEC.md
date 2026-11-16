# EX2_SPEC.md — Örnek 2 (geometri ve yük Duysak, Nigdeli ve Bekdaş 2023'ten uyarlanmış): 2B, 3 kat × 3 açıklık, 21 elemanlı çerçeve, yalnız düşey yük

Durum: **TASLAK v2 (2026-11-16, Method Architect).** v1 → v2: iki hakem raporu (`06_Review_Audit/REVIEW_LOG/Review_Round_Ex2_spec_ic.md` KOŞULLU ONAY, `..._dis.md` BÜYÜK REVİZYON) ve IA kararları (Q1, Q2, Q3, Q8, Q9; ek kararlar a–i, 2026-11-16) işlendi. Kod, PROGRAM, Sources ve ikili dosyalar değiştirilmedi; koşu başlatılmadı. Açık sorular: `EX2_OPEN_QUESTIONS.md`.

Durum etiketleri (AGENTS kural 11): DOĞRULANDI (belge + sayfa) · KISMEN · DOĞRULANAMADI · BULUNAMADI · ERİŞİLEMEDİ. Ek etiketler: [K] kullanıcı (IA) kararı, [T] tez (ST) yazımı, [Ç] Claude'un aritmetik türetmesi, [VARSAYIM] kanıtsız öneri (kaynak girilene dek makaleye alınmaz).

ACI alıntılarının kaynağı: `Evidence/DesignGuides/ACI 318M_14.pdf` (8 906 558 bayt; SHA-256 `d2b49f8827a32fa5979a566ee3d5428d607966cb32c22b264972f152916d037d`, bu oturumda hesaplandı). Basılı sayfa = PDF − 2.

---

## A. v2 özeti (kodlama öncesi okunacak bölüm)

### A.1 Soru durumu (ayrıntı `EX2_OPEN_QUESTIONS.md`)

| # | Konu | Durum | Karar / öneri |
|---|---|---|---|
| Q1 | Grup yapısı | **KARAR [K]** | V2 kuvvet karşılaştırması 5 grup (A); ana optimizasyon 9 grup (B) |
| Q2 | Yük kombinasyonu | **KARAR [K]** | Ana: ACI 1,2D+1,6L (+1,4D zarfta); kaynak varyantları (1,0G+1,0Q, 1,4G+1,6Q) yalnız V2-b bilgi tablosu |
| Q3 | Öz ağırlık | **KARAR [K]** | Ayrıca eklenir (kolon + kiriş); ST'ye sorulmaz |
| Q4 | Kat yüksekliği 3,3 m | ÖNERİ (itiraz yok) | 3,3 m |
| Q5 | Beton C30 sabit | ÖNERİ | C30 sabit |
| Q6 | Havuz 250–450 / 300–600 / 50 mm | ÖNERİ | Kaynak aralığı, 50 mm |
| Q7 | Gravite rejimi, kolon ρ %1–%4 | ÖNERİ | Bölüm 18 ve TBDY uygulanmaz; ρ %1–%4 |
| Q8 | V2 ölçütü | **KARAR [K]** | Tek kapı FEM↔ETABS (V2-a); kaynak karşılaştırması bilgi amaçlı |
| Q9 | Kol | **KARAR [K]** | Yalnız A (rastgele başlangıç, ön tasarım yok) |
| Q10 | Kaynak-replika fiyat seti (PCR-19) | **KAPANDI [K d]** | Kod yok; `Maliyet_Metraj.xlsx` ayrı sayfada formülle |
| Q11 | Lif doz ızgarası | ÖNERİ | Ö1 ile aynı onarım (sınıra kırpma; §8) |
| Q12 | Tezin ETABS modeli | ÖNERİ | V1'de açılıp okunur |
| Q13 | Metin düzeltmeleri | Yazım aşaması | — |
| Q14 | Kaynak PDF `Papers/` | Gemini/IA | Makaleye girmeden önce |
| Q15 | 2B düzlem dışı e_min | ÖNERİ | V2-a'da kontrol; düzlem dışı tutulu varsayımı §5.3 |
| Q16 | Hareketli yük düzenlemesi | **KARAR [K b]** | Uygulanmaz; sınırlama |
| Q17 | Lif kesme katkısının sayısal kuralı | **AÇIK (IA)** | Öneri: artım (Δ) kuralı, §5.2 |
| Q18 | fR karakteristik mi | **AÇIK (IA)** | Öneri: karakteristik kabul + fR×0,8 zorunlu duyarlılık |
| Q19 | Fiyat = 0 senaryosunun koşu sayısı | **AÇIK (IA)** | Öneri: 3 × 30 = 90 koşu (toplam 270) |
| Q20 | V2-a için uygun tasarımın kaynağı | **AÇIK (IA)** | Öneri: koşu tohumları dışında tek pilot koşu |
| Q21 | Zorlanmış lif: deterministik mi yeniden optimizasyon mu | **AÇIK (IA)** | Öneri: deterministik (kesit sabit, donatı yeniden türetilir) |

### A.2 PCR listesi (v2; IA onayı bekler; tümü yeni dosya, Ö1 kaynağı ve DLL'leri derlenmez)

| PCR | Kapsam | Doğrulama |
|---|---|---|
| **PCR-14** Geometri, yük, analiz | `FrameSpec2D`, `RcFrameBuilder2D`, `RcCombos2D`: 5-6-5 m, 3,3 m, ankastre taban, düzlem dışı serbestlikler tutulu; G = 30, Q = 20 kN/m tüm kirişlerde; öz ağırlık γ = 25 kN/m³ ayrı desen; kesit yönelimi §1.1; kombinasyonlar 1,4D ve 1,2D+1,6L (ana zarf), 1,0G+1,0Q ve 1,4G+1,6Q (yalnız V2-b); hareketli yük düzenlemesi yok; birim zinciri kN, m → N, mm açık | V1 (§10) |
| **PCR-15** Gravite tasarım sınıfları (yeni sınıf; **bayrak seçeneği silindi**) | `BeamDesignGravity` (ACI 9.6.1.2, εt ≥ 0,004), `BeamShearGravity` (ACI 22.5 + Av,min + 9.7.6.2.2; lif katkısı §5.2), `ColumnDesignGravity` (`AciColumn` P-M, δns, e_min; ρ %1–%4; etriye ACI 25.7.2), servis kısıtı ACI 9.3.1.1; maliyet kuralları §5.4 | V2-a (CD1–CD3, B1, B2), S1 |
| **PCR-16** `Ex2Problem` | 9 grup (ana) ve 5 grup (V2 eşleme); b 250–450, h 300–600, 50 mm; C30 sabit; PCR-8 ρ türetmesi; lif tip + doz (kiriş grubu başına); doz onarımı Ö1 kuralı | birim test: `Decode` sınırları |
| **PCR-17** `Ex2Evaluator` | §5 kısıt listesi, Ö1 çarpımsal ceza F(1+T)², §5.4 maliyet; **ön tasarım sınıfı yok** (Q9: yalnız A) | V5 (Excel ↔ amaç ≤ %0,5) |
| **PCR-18** `Ex2Runner` + derleme yalıtımı + deterministik değerlendirme modu | Ö1 CLI bayrakları (`--method --fibre --runs --fes --fr --fibreprice --seed0 --dop --out`), PCR-11 günlüğü; ek `--evalonly <tasarım dosyası>` (zorlanmış lif, fR, fiyat, kesme formatı duyarlılığı); ayrı çözüm, donmuş DLL'lere SHA kayıtlı dosya referansı (§9.1) | V0 (Ö1 ikili SHA önce/sonra) |
| ~~PCR-19~~ | **Silindi** [K d]: kaynak fiyatıyla yeniden fiyatlama `Maliyet_Metraj.xlsx` içinde formülle | — |

### A.3 Kodlama öncesi kapılar (iç Critic E1–E6) ve dış hakem koşu-öncesi listesi

| Kapı | İçerik | Durum (v2) | Yer |
|---|---|---|---|
| E1 | Lif kesme güvenlik formatı | **Belgede kapandı [K a]**; sayısal kural Q17 IA teyidi bekler | §5.2 |
| E2 | ACI Tablo 9.6.3.1 / 26.4.2.2(d) lif muafiyeti | **KAPANDI: DOĞRULANDI** (PDF 140/138 ve PDF 462/460); 20–40 kg/m³ < 60 kg/m³ → Av,min her zaman | §5.2 |
| E3 | Hareketli yük düzenlemesi | **KAPANDI [K b]**: uygulanmaz, sınırlama; ETABS'te de tek desen | §3, §12 |
| E4 | V1 öz ağırlıklı ölçüt + kesit yönelimi | **Belgede kapandı** | §1.1, §10 V1 |
| E5 | Derleme yalıtımı | **Belgede kapandı [K d]**; uygulama V0'da SHA ile | §9.1 |
| E6 | Ö2 maliyet kuralları + lifli V3 yöntemi | **Belgede kapandı [K f, g]** | §5.4, §10 V3 |
| Dış K2 / M1 | Fiyat = 0 senaryosu, kalem bazlı marjinal maliyet tablosu, koşu sayısı | **Karar [K g]**; fiyat = 0 koşu sayısı Q19 | §11 |
| Dış K3 | V2-a D/C uygun tasarımla | **Karar [K e]**; tasarımın kaynağı Q20 | §10 V2-a |
| Dış B1 | Servis | **Karar [K c]**: ACI 9.3.1.1 kısıt; hesaplı sehim/çatlak yok (sınırlama) | §5.1 |
| Dış B2 | Vc + VRd,F çift sayım, kısmi katsayılar, fR karakteristik | §5.2'de yazıldı; fR durumu Q18 | §5.2 |
| Dış B6 | V1 öz ağırlık | E4 ile kapandı | §10 |

**Kod yazımı şartı:** Q17 ve Q18 IA tarafından cevaplanmadan PCR-15 kesme sınıfı yazılmaz. Diğer PCR'ler (14, 16, 18'in yalıtım kısmı) A.2 onayıyla başlayabilir. Koşu protokolü (§11) Q19–Q21 cevaplanınca ön kayda alınır.

---

## 0. Kaynak belge (kanıt)

| Kalem | Değer |
|---|---|
| Dosya | `Sources/tez_sonuç tablo grafik/Referans örneklerin makaleleri/Örnek2_06_05_24_2Boyutlu_(gebrail_niğdeli_2023)I_ö2.pdf` |
| Bayt / SHA-256 | 532 161 bayt; `9d30b88eee609f883d916618ac5e7f4bc0e221b9c81bc899fa85ee30f20cee1d` |
| Künye (PDF s.2) | Duysak, Y., Nigdeli, S. M. ve Bekdaş, G. (2023). *2 Boyutlu Betonarme Çerçeve Sistemin Çiçek Tozlaşma Algoritması ile Optimum Tasarımı.* 23. Ulusal Mekanik Kongresi, 04–08 Eylül 2023, Konya Teknik Üniversitesi. |
| Okunabilirlik | Metin katmanlı (PyMuPDF; 8 PDF sayfası). Basılı sayfa = PDF − 1. Şekil 1, 2 sayfa görüntüsünden okundu. |
| Tür | Türkçe ulusal kongre bildirisi; hakemli dergi makalesi değil; FPA; lifsiz. DOI **BULUNAMADI**. |
| Konum uyarısı | Dosya yalnız `Sources/` altında; `Papers/` kopyası Q14. |
| İç tutarsızlık | Özet "ACI-318"; kaynakça [16] "ACI 318-08" (PDF s.7). 318-08 → 318-14 farkı gravite eğilme/kesmesinde küçüktür; asıl sapma kaynakları kombinasyon, öz ağırlık ve sabit donatıdır (dış M5). |

**Makaledeki rol [K h]:** "geometry and loading adapted from Duysak et al. (2023)". "Benchmark", "validated against Duysak", "compared with the reference optimum" ifadeleri **yasak**. Kaynak bildirinin sınırlılıkları (bildiri, DOI yok, donatı sabit, kombinasyon ve öz ağırlık yok, ACI 318-08) sınırlama olarak yazılır.

---

## 1. Geometri ve grup yapısı

| Parametre | Kaynak (birebir alıntı ≤25 kelime) | Yer | Durum |
|---|---|---|---|
| Sistem | "3 açıklıklı 3 katlı 2 boyutlu betonarme sistem maliyet açısından optimize edilmiştir." | PDF s.4 | DOĞRULANDI |
| Açıklıklar | "Sistemde kenar açıklıklar 5 metre iç açıklık 6 metredir" → **5 – 6 – 5 m** | PDF s.4; Şekil 1 | DOĞRULANDI |
| Kat yüksekliği | "tüm katlarda kat yüksekliği 3.3 metredir" | PDF s.4 | DOĞRULANDI |
| Mesnet | Şekil 1: dört kolon tabanında ankastre | PDF s.4 görüntü | KISMEN (çizimden) |
| Numaralama | Şekil 2: düğüm 1–4 (taban) … 13–16 (çatı); kolon 1–4, 8–11, 15–18; kiriş 5–7, 12–14, 19–21 | PDF s.5 görüntü | DOĞRULANDI |
| Eleman sayısı | 12 kolon + 9 kiriş = **21** | Şekil 2, Tablo 1 | DOĞRULANDI |

**Çelişkiler:** TEZ PDF 60 "tüm açıklıklar 5 metre" (kaynakla çelişir); MM §3.1.2 "iki açıklık, 5,0 m" (yanlış). Esas: 5-6-5 m [K S-1]. `THESIS_EXAMPLE_DATA.md`'de Ö2 bölümü yok.

### 1.1 Kesit yönelimi (iç O3; E4) [K f]
- **Kolon:** `b` = çerçeve düzlemine dik boyut; `h` = çerçeve düzlemi içindeki boyut (düzlem içi eğilmede kesit yüksekliği). Kaynak Tablo 1 "25×30" = b × h → h = 300 mm düzlem içinde. Düzlem dışı eğilme yalnız ACI e_min (PCR-1).
- **Kiriş:** `b` = gövde genişliği bw; `h` = toplam yükseklik; döşeme/tabla yok (dikdörtgen kesit).
- `RcFrameBuilder2D` ve ETABS OAPI modeli aynı kurala bağlanır (ETABS: yerel 2 ekseni düzlem içi, `t3` = h); V1'de 3 eleman için görsel ve sayısal kontrol.

### 1.2 Tasarım grupları [K Q1]
| Kullanım | Kolon grubu | Kiriş grubu | Toplam |
|---|---|---|---|
| V2-a kuvvet karşılaştırması (kaynak Tablo 1 kesitleri) | 2 (kenar, orta; tüm yükseklik) | 3 (kat başına) | **5** (A) |
| Ana optimizasyon | 6 (kenar/orta × 3 kat) | 3 (kat başına) | **9** (B) |

(A) ⊂ (B). Tez (C, 6 grup) ile karşılaştırma yapılmaz; fark makalede bir cümleyle yazılır. 5 m ve 6 m kirişler aynı grupta → 5 m kirişler 6 m zarfıyla donatılır (beyan; iç Y4).

---

## 2. Malzeme

| Parametre | Kaynak (alıntı) | Yer | Durum |
|---|---|---|---|
| Beton | "beton sınıfı C30" | PDF s.4 | DOĞRULANDI; fc' = 30 MPa [VARSAYIM: C30/37 adlandırması] |
| Donatı | "çelik sınıfı için ise S420 seçilmiştir" | PDF s.4 | KISMEN; fy = fyt = 420 MPa |
| Es, Ec, γ, ν | — | — | BULUNAMADI; proje: Es 200 GPa, Ec = 4700√fc' (ACI 19.2.2.1.b), γ = 25 kN/m³ |
| Beton sınıfı değişkeni | Kaynak C30 sabit; tez sonuçlarında C40 | | **C30 sabit** (Q5) |

**Lif (kaynakta yok; çalışmanın katkısı):** `FibreTable.vb` C30 ızgarası: 4D 20–40 kg/m³ (5'er), 5D 25–35 kg/m³. Veri gizli üretici verisi (kırmızı bayrak, MEMORY §3c). Dozların tümü ACI 26.4.2.2(d)'nin 60 kg/m³ alt sınırının altındadır (§5.2); bu aralık **veri mevcudiyeti** nedeniyle seçilmiştir ve makalede böyle yazılır (dış O6).

---

## 3. Yükler

| Parametre | Kaynak / karar | Durum |
|---|---|---|
| G, Q | "Tüm açıklıklarda 30 kN/m ölü yük ve 20 kN/m hareketli yük bulunmaktadır." (PDF s.4); üç katta, çatı dahil | DOĞRULANDI |
| Öz ağırlık | Kaynakta BULUNAMADI. **[K Q3] Ayrıca eklenir: kolon ve kiriş**, γ = 25 kN/m³, eksen boyu (rijit bölge yok). Varsayım: 30 kN/m döşeme + kaplama + duvarı içerir, kiriş öz ağırlığını içermez (makalede açık yazılır). ETABS: `SelfWeightMultiplier = 1` yalnız G deseninde; FEM'de ayrı öz ağırlık yükü, çift sayım yok (Ö1 F4 deseni örnek) | [K] |
| Kombinasyon | Kaynakta BULUNAMADI. **[K Q2] Ana zarf: U = 1,4D (5.3.1a) ve U = 1,2D + 1,6L (5.3.1b).** ACI Tablo 5.3.1: "U = 1.4D (5.3.1a)", "U = 1.2D + 1.6L + 0.5(Lr or S or R) (5.3.1b)" (PDF 60 / basılı 58) | DOĞRULANDI (madde); karar [K] |
| 1,4D belirleyiciliği [Ç] | 1,4D > 1,2D + 1,6L ⇔ D > 8L = 160 kN/m; D = 30 + öz ağırlık (≤ ≈ 7 kN/m) → belirleyici değil; yine de zarfa konur (iç O1) | hesap |
| Çatı yükü | Çatıdaki Q = 20 kN/m **L olarak** alınır (Lr değil). 5.3.1c ile karşılaştırma [Ç]: Lr sayılsaydı çatı 1,6 ve katlar 1,0 (5.3.1c) veya katlar 1,6 ve çatı 0,5 (5.3.1b) olurdu; L kabulü her elemanda 1,6 verir ve her iki zarfı da kapsar (muhafazakâr). Beyan edilir | [K f] |
| Hareketli yük düzenlemesi | **[K b] Uygulanmaz; tüm açıklıklar aynı anda yüklü.** ACI 6.4.2: "For one-way slabs and beams, it shall be permitted to assume (a) and (b): (a) Maximum positive Mu near midspan occurs with factored L on the span and on alternate spans" (PDF 71 / basılı 69). Gerekçe: kaynakla uyum ve FE maliyeti. Sonuç: açıklık pozitif momenti ve kesme düşük tahmin edilebilir (L/D = 20/30; etki ölçülmedi). **Sınırlama** (§12). ETABS V2-a/V3'te de tek desen (zarf FEM ile aynı) | DOĞRULANDI (madde); karar [K] |
| Yanal yük | "… düşey yükler altında minimum maliyeti hesaplanmıştır." (PDF s.3) | DOĞRULANDI |
| Analiz | Doğrusal elastik, brüt kesit (`fa.Gross`), statik; modal/RS yok | — |

**Kaynak varyantları (yalnız V2-b bilgi tablosu) [K Q2]:** 1,0G + 1,0Q (tez "kombinasyon uygulanmadı"), 1,4G + 1,6Q (MM §3.1.2); her biri öz ağırlıklı ve öz ağırlıksız.

**Düşey toplam [Ç]:** dış yük G = 30 × 16 × 3 = 1 440 kN; Q = 20 × 16 × 3 = 960 kN. Öz ağırlık kesite bağlıdır; ölçüt §10 V1.

---

## 4. Kesit havuzu / tasarım değişkenleri

| Parametre | Kaynak | Durum |
|---|---|---|
| b | "minimum kesit genişliği (bmin) 250 mm, maksimum genişlik (bmax) 450 mm" (PDF s.5) | DOĞRULANDI |
| h | "minimum (hmin) yüksekliği 300 mm, maksimum (hmax) yükseklik ise 600 mm" (PDF s.5) | DOĞRULANDI |
| Adım | 50 mm [T] (kaynakta BULUNAMADI) | Q6 |
| Donatı | Kaynakta BULUNAMADI; bizde gereksinimden türetilir | — |

**Havuz:** kolon ve kiriş b = 250…450 (5), h = 300…600 (7); C30 sabit; kolon ρ PCR-8 türetmesi (ızgara %1–%4; ACI 10.6.1.1 %8 sınırı içinde); kiriş donatısı ve etriye gereksinimden; lif: kiriş grubu başına tip (4D/5D) + doz (0, 20, 25, 30, 35, 40).
Değişken sayısı (9 grup): lifsiz **18**; lifli **24**. Uzay: 35⁹ ≈ 7,9×10¹³ (lifsiz); ≈ 5,7×10¹⁶ (lifli; kiriş grubu başına 9 malzeme seçeneği) [Ç].

---

## 5. Kısıtlar ve kod hükümleri (gravite rejimi; ACI 318-14)

Kaynak yalnız "ACI-318 … tasarım kuralları" ve eğilme/kesme dayanımı bildirir (PDF s.4, s.6); servis, narinlik, ceza BULUNAMADI. Rejim: **Bölüm 18 uygulanmaz (yanal kuvvet direnci istenmez)**; "ordinary moment frame" ifadesi kullanılmaz (ACI 18.3'e gönderme yapar; dış O4).

### 5.1 Kısıt listesi

| # | Kısıt | Madde | Durum |
|---|---|---|---|
| g1 | Kolon P-M D/C ≤ 1 (2B: düzlem içi analiz momenti × δns; düzlem dışı yalnız e_min, PCR-1) | ACI 22.4, 6.6.4.5 | Ö1 `AciColumn` aynen |
| g2 | Kolon ρ: 0,01 Ag ≤ Ast (ızgara %1–%4) | ACI 10.6.1.1 (A22) | DOĞRULANDI (`CODE_PROVISIONS_EVIDENCE.md`) |
| g3 | Kiriş eğilmesi: As ≥ As,min; εt ≥ 0,004 | ACI 9.6.1.2 (A09), 9.3.3.1 | Ö1 TBDY ekleri (alt ≥ %50 üst, ρ ≤ %2) **kaldırılır** |
| g4 | Kiriş kesmesi: Vu ≤ φ(Vc,f + Vs), Vs ≤ 0,66√fc' bw d; Av ≥ Av,min | ACI 22.5, 22.5.1.2 (A14), 9.6.3.3 (A18) | §5.2 |
| g5 | **Servis: h ≥ ℓ/18,5 (bir ucu sürekli: kenar açıklıklar), h ≥ ℓ/21 (iki ucu sürekli: iç açıklık)** | ACI Tablo 9.3.1.1 | DOĞRULANDI: "overall beam depth h shall satisfy the limits in Table 9.3.1.1, unless the calculated deflection limits of 9.3.2 are satisfied" (PDF 132 / basılı 130); fy = 420 → düzeltme yok (9.3.1.1.1) |
| g5 [Ç] | 5 m: 5 000/18,5 = 270 mm; 6 m: 6 000/21 = 286 mm (ℓ = eksen açıklığı, muhafazakâr) | | Havuz alt sınırı 300 mm ile **pasif**; yine kısıt olarak kodlanır ve raporlanır |
| — | Madde 9.3.1.1 yalnız "not supporting or attached to partitions or other construction likely to be damaged by large deflections" kirişlere uygulanır (PDF 132) → **varsayım:** kirişler hasar görebilecek bölmeleri taşımaz. Hesaplı sehim (24.2) ve çatlak kontrolü (24.3) **yapılmaz: sınırlama** [K c] | | beyan |
| — | Narinlik: çerçeve **ötelenmesiz (nonsway) kabul** [K f]; Q indeksi yanal yük yokken hesaplanamaz (iç O4); düzlem içi k = 1,0, lu = net yükseklik, Cm = 1 (PCR-2); düzlem dışı tutulu varsayımı, k = 1,0. ETABS: "NonSway" çerçeve tipi (V2-a'da belgelenir) | ACI 6.6.4 | **varsayım**, §12 |
| — | Drift, ACI 6.6.4.3 Q ≤ 0,05, güçlü kolon, birleşim kesmesi, Mpr kapasite kesmesi, Vc = 0, sarılma bölgeleri (ACI 18.6–18.8), TBDY 7.3/7.4 | | **Uygulanmaz** [K f] |
| — | ACI 9.7.7 yapısal bütünlük ve 9.7.3.8 süreklilik kuralları | | **KISMEN**: boyuna donatı grup boyunca sürekli kabul edildiğinden (§5.4) dolaylı sağlanır; ayrıca kontrol edilmez (beyan; iç Y4) |

Ceza: Ö1 çarpımsal F(1 + T)² (`Penalty`), AGENTS kural 8.

### 5.2 Kesme: lif katkısı ve güvenlik formatı (E1, E2) [K a]

**ACI hükümleri (DOĞRULANDI):**
- 9.6.3.1: "A minimum area of shear reinforcement, Av,min, shall be provided in all regions where Vu > 0.5ϕVc" (PDF 140 / basılı 138; A17). Tablo 9.6.3.1 çelik lifli beton muafiyeti yalnız 26.4.1.5.1(a), 26.4.2.2(d), 26.12.5.1(a) sağlanırsa ve fc' ≤ 40 MPa, h ≤ 600 mm, Vu ≤ 0,17φ√fc' bw d ise geçerlidir (A17).
- 26.4.2.2(d): "Steel fiber-reinforced concrete used for shear resistance shall satisfy (1) and (2): (1) Conform to ASTM C1116M. (2) Contain at least 60 kg of deformed steel fibers per cubic meter of concrete." (PDF 462 / basılı 460). **DOĞRULANDI** (iç K2'deki "≈ 60 kg/m³" hafıza bilgisi doğru).
- R26.4.1.5: "For structural applications, the Code only addresses the use of discontinuous deformed steel fibers in resisting shear." (PDF 460 / basılı 458).

**Sonuç kuralı:** Doz ızgarası 20–40 kg/m³ < 60 kg/m³ → **ACI lif muafiyeti hiçbir lifli kirişte kullanılamaz.** Av,min (9.6.3.3) Vu > 0,5φVc olan her bölgede **lifli ve lifsiz kirişte aynen** uygulanır; eşikteki Vc = lifsiz ACI Vc. fib MC2010 Eq. 7.7-8 (`MinStirrupWaived`) ile Av,min kaldırılmaz. Lif yalnız Av,min üstündeki etriye ihtiyacını azaltabilir; "lif etriyeyi kaldırır" beklentisi kurulmaz (makalede yazılır). Lifin ACI'nin tanımadığı dozda kesme kapasitesine katılması **ACI'den beyan edilen sapmadır** (fib MC2010 dayanağıyla).

**Ana seri formatı [K a]:** lif katkısı ACI nominal formatıyla tutarlı hesaplanır: fib MC2010 Eq. 7.7-4 (`SfrcBeam.VRdF`) **γc = 1,0**, **karakteristik fR** (fFtuk, wu = 1,5 mm; `FFtukShear`), ardından ACI φ = 0,75. Böylece tek güvenlik katsayısı φ'dir (Ö1'deki γc = 1,5 + φ çift güvenliği kalkar).

**Çift sayım yasağı (dış B2):** MC2010 VRd,F beton ve lif katkısını tek terimde verir; ACI Vc ile VRd,F **toplanmaz**. Önerilen sayısal kural (**Q17, IA teyidi gerekli**):

> Vc,f = Vc,ACI + ΔVF, ΔVF = VRd,F(fFtuk; γc = 1,0) − VRd,F(fFtuk = 0; γc = 1,0); Vn = Vc,f + Vs; Vs ≤ 0,66√fc' bw d; Vu ≤ φVn.

Gerekçe [Ç]: doz 0'da Eq. 7.7-4 nominal değeri ACI Vc'den farklıdır. Örnek: bw = 250, d = 240 mm, ρl = %1, fck = 30 MPa → k = 1,913; 0,18·k·(100·0,01·30)^(1/3) = 1,07 MPa, ACI 0,17√30 = 0,93 MPa (+%15). VRd,F doğrudan Vc yerine konursa, lifsiz betonun MC2010 ile ACI arasındaki farkı lifin katkısı gibi görünür (lif lehine yanlılık). Artım kuralı yalnız lifin etkisini ekler; doz 0'da Vc,f = Vc,ACI (süreklilik). Alternatif (Q17-b): Vc,f = max(Vc,ACI; VRd,F(γc = 1)). VRd,F'nin Eq. 7.7-5 alt sınırı (`vmin`) ΔVF hesabında iki terimde de aynı uygulanır. Basınç diyagonali sınırı ACI 22.5.1.2 (Vs üst sınırı) ile.

**fR karakteristik mi (dış B2; Q18):** `FibreTable.vb` değerlerinin karakteristik mi ortalama mı olduğu **DOĞRULANAMADI** (EX1_SPEC). IA kararı "karakteristik fR" iken veri ortalama ise lif katkısı güvensiz yönde abartılır. Öneri: kaynak teyit edilene dek değerler karakteristik kabul edilir (beyan), fR × 0,8 deterministik yeniden değerlendirme **zorunlu** ek tablo olur.

**Duyarlılık varyantı (Ö1 formatı) [K a]:** VRd,F γc = 1,5 + φ = 0,75 (Ö1 `BeamDesign.vb` biçimi). Yalnız en iyi çözümlerin deterministik yeniden değerlendirmesi (`--evalonly`; yeniden optimizasyon değil): uygunluk ve etriye/maliyet farkı. **Ö1 donmuştur** (değiştirilmez); Ö1 ile Ö2 arasındaki format farkı makalede beyan edilir.

### 5.3 Etriye kuralları (gravite) [K f]
- Kiriş: Av/s gereksinimden; smax = d/2 ≤ 600 mm, Vs > 0,33√fc' bw d ise d/4 ≤ 300 mm (ACI 9.7.6.2.2; A19). Sarılma bölgesi (2h, SConf) **yok**.
- Kolon: ACI 25.7.2 bağlantı etriyesi (s ≤ 16 db, 48 dt, en küçük boyut) [madde metni bu oturumda yeniden okunmadı: V0 öncesi `CODE_PROVISIONS_EVIDENCE.md`'ye alınır]. Lk sarılma ve birleşim içi etriye (ACI 18.8.3.1) **yok**.
- Birleşim bölgesi enine donatısı (ACI 15) maliyete katılmaz; madde doğrulanmadı (sınırlama listesi).

### 5.4 Maliyet kuralları (iç Y4; E6) [K f]
- Beton: Σ b·h·L (eksen boyu; Ö1 boy kuralı korunur).
- **Kalıp: kiriş (b + 2h)·L** (döşeme yok; Ö1'deki `b + 2(h − SlabThickness)` kullanılmaz); kolon 2(b + h)·L.
- Boyuna donatı: grup başına `AsTop + AsBot`, grup boyunca sürekli (kesme/bindirme yok) — varsayım tablosu.
- Etriye: §5.3 gravite aralığıyla; sarılma ve birleşim etriyesi yok.
- Lif: doz × kiriş beton hacmi × fiyat (90 veya 0 TL/kg).
- Birim fiyatlar: ÇŞİDB 2026 (`PRICES_2026.md`), §7.
- `RcCore.Cost` fonksiyonları değiştirilmez; bir fonksiyon döşeme düşümünü içeriden yapıyorsa hesap `Ex2Evaluator` içinde yeniden yazılır.

---

## 6. Kaynak makalenin sonuçları (bilgi amaçlı; geçiş kapısı değil [K Q8])

Kaynak Tablo 1 (PDF s.6): toplam "11486 ₺".

| Eleman | Konum | b×h (cm) | Maliyet (TL) | Eleman | Konum | b×h (cm) | Maliyet (TL) |
|---|---|---|---|---|---|---|---|
| 1 | K1 kenar | 25×30 | 389,97 | 12 | B2-1 (5 m) | 25×30 | 685,13 |
| 2 | K1 orta | 30×30 | 401,85 | 13 | B2-2 (6 m) | 25×30 | 778,75 |
| 3 | K1 orta | 30×30 | 401,85 | 14 | B2-3 (5 m) | 25×30 | 685,13 |
| 4 | K1 kenar | 25×30 | 389,97 | 15 | K3 kenar | 25×30 | 389,97 |
| 5 | B1-1 (5 m) | 25×35 | 700,13 | 16 | K3 orta | 30×30 | 401,85 |
| 6 | B1-2 (6 m) | 25×35 | 796,75 | 17 | K3 orta | 30×30 | 401,85 |
| 7 | B1-3 (5 m) | 25×35 | 700,13 | 18 | K3 kenar | 25×30 | 389,97 |
| 8 | K2 kenar | 25×30 | 389,97 | 19 | B3-1 (5 m) | 25×55 | 760,13 |
| 9 | K2 orta | 30×30 | 401,85 | 20 | B3-2 (6 m) | 25×55 | 868,75 |
| 10 | K2 orta | 30×30 | 401,85 | 21 | B3-3 (5 m) | 25×55 | 760,13 |
| 11 | K2 kenar | 25×30 | 389,97 | | | | |

Durum: DOĞRULANDI (PDF s.6 metin katmanı).

**Aritmetik denetimi [Ç; iç Critic yeniden hesapladı: DOĞRULANDI]:**
1. Toplam 11 485,95 TL ≈ 11 486.
2. Eleman maliyeti = 240 TL/m³ × b·h·L + sabit donatı bedeli: kolon 330,57 TL (tüm kolonlar), 5 m kiriş 595,13 TL, 6 m kiriş 670,75 TL (tüm h değerlerinde aynı). Kalıp yok.
3. Beton 1 936,08 TL (%16,9); donatı 9 549,87 TL (%83,1).
4. Donatı eleman tipine göre sabittir; metindeki "donatı alanını sabit tutarak" cümlesi (PDF s.6) bunu doğrular. Kaynak çözümü bu nedenle yeniden üretilebilir bir optimum olarak kullanılmaz; yalnız geometri/yük tanımı ve tek bir tasarım noktası olarak kullanılır.
5. Çatı kirişinde h = 55 cm çıkması hakkında spec v1'deki "mekanik olarak tutarsız" yargısı **geri çekildi** (dış M3: çatı kirişinin kolon tutulması farklıdır). Makalede kaynak hakkında hüküm cümlesi kurulmaz.

**Kaynakta bulunmayan:** donatı çapı/adedi, etriye, D/C, sehim, FPA parametreleri, koşu sayısı (BULUNAMADI). Tez Çizelge 4.11–4.14 (USD, farklı maliyet modeli, N/A hücreler, 6 grup) makaleye alınmaz.

---

## 7. Birim fiyatlar

| Kalem | Kaynak (PDF s.4) | ÇŞİDB 2026 (proje) | Oran |
|---|---|---|---|
| Beton | "C30 Beton m³ fiyatı 240 TL" | 15.150.1006 **3 649,35 TL/m³** | ×15,2 |
| Donatı | "donatının ton fiyatı 11000 TL" | 24,20 + 44,68 = **68,88 TL/kg** (Ø14–28); Ø8–12: 70,68 | ×6,3 |
| Kalıp | yok | 15.180.1002 **873,03 TL/m²** | — |
| Lif | yok | 10.300.4014 **90,00 TL/kg**; ek senaryo 0 TL/kg | — |

**Kaynak fiyatıyla yeniden fiyatlama [K d]:** kod değişikliği yok (PCR-19 silindi). `04_Results/Maliyet_Metraj.xlsx` içinde ayrı "Kaynak-replika" sayfası: (i) Tablo 1 kesitlerinin beton bedeli 1 936,08 TL ve toplam 11 486 TL formülle yeniden üretilir (V2-c); (ii) kaynak fiyatlarıyla optimizasyon **yapılmaz**. Ana (ÇŞİDB 2026) sonuçlar kaynak TL değerleriyle karşılaştırılmaz.

---

## 8. Lif tasarım uzayı

- Lif yalnız kirişlerde; 4D (fu = 1600 N/mm²), 5D (fu = 2300 N/mm²); doz 0, 20, 25, 30, 35, 40 kg/m³; fR1/fR3 `FibreTable.vb` (üretici 65/60 BG; EN 14651; karakteristik durumu Q18).
- C30 ızgarası: 4D 20–40, 5D 25–35. **Doz onarımı (Ö1 `Ex1Problem.RepairDose` mantığı):** doz ≤ 0 → 0; aksi hâlde [alt, üst] aralığına kırpılır (5D'de 20 → 25 yukarı, 40 → 35 aşağı). Kırpma yönü maliyete etki eder; makalede tek cümleyle yazılır (dış O6). Kod Ex2'de yeniden yazılır (Ö1 sınıfı çağrılmaz ya da donmuş DLL'den okunur).
- Lif fiyatı: **90 TL/kg** (ana) ve **0 TL/kg** (zorunlu senaryo [K g]). Lif fiyatı ×0,5/×1,5/×2 ve fR ×0,8/×1,2: en iyi çözümlerin deterministik yeniden değerlendirmesi (bilgi).
- Ö2 lifli sonuçları tez Çizelge 4.12/4.14 ile kıyaslanmaz.

---

## 9. Program: yeni dosyalar ve derleme yalıtımı

`Optimizers.Run(method, obj, lb, ub, maxFEs, seed, x0)` olduğu gibi kullanılır. Ö1 sınıfları değiştirilmez.

| # | Mevcut (Ö1) | Ö2'de sorun | Ö2 (yeni, Ex2 projesinde) |
|---|---|---|---|
| 1 | `FrameSpec` | eşit olmayan açıklık, 2B, kN/m yük | `FrameSpec2D` (PCR-14) |
| 2 | `RcFrameBuilder.Build` | 3B, tributary, diyafram | `RcFrameBuilder2D` (Şekil 2 numaralaması; §1.1 yönelim) |
| 3 | `RcCombos.Analyze` | modal, RS, TBDY | `RcCombos2D` (1,4D; 1,2D+1,6L; V2-b varyantları; desen yok) |
| 4 | `Ex1Evaluator` | rejim, maliyet döngüsü | `Ex2Evaluator` (PCR-17; §5.4) |
| 5 | `BeamDesign.DesignGroup` | TBDY ekleri | `BeamDesignGravity` (PCR-15) |
| 6 | `BeamShear.DesignGroup` | Mpr, Vc = 0, γc = 1,5 + φ | `BeamShearGravity` (§5.2) |
| 7 | `ColumnDesign`, `ColumnShear` | biaxial, sismik detay, Mpr | `ColumnDesignGravity` (`AciColumn` P-M, δns, e_min) |
| 8 | `Checks.*`, `Stability.MaxQ`, `Tbdy` | Ö2'de yok | çağrılmaz |
| 9 | `Ex1Problem` | havuz, grup | `Ex2Problem` (PCR-16) |
| 10 | `PreDesign` | — | **yok** (Q9: yalnız A) |
| 11 | `Ex1Runner` | — | `Ex2Runner` (PCR-18; `--evalonly`) |

### 9.1 Derleme yalıtımı (iç Y2; E5) [K d]
1. Ö2 **ayrı Visual Studio çözümü** (ör. `03_Code_Execution/ex2_work/`, `program_work` dışında); proje referansı **kullanılmaz** (proje referansı başvurulan projeyi yeniden derler).
2. `FemCore.dll`, `RcCore.dll`, `FemRcModel.dll` (gerekirse) **dosya referansı** olarak, `phase3_bin` ikilileriyle aynı SHA-256'ya sahip donmuş kopyalardan (`ex2_work/ref/`) bağlanır; SHA'lar V0'da `phase3_bin`'den okunup Ö2 koşu günlüğüne yazılır.
3. `program_work` kaynakları ve DLL'leri **derlenmez**; Ex2 derleme çıktısı `phase2_bin`, `phase3_bin` ve çalışan koşu klasörlerine yazılmaz.
4. V0: Ö1 ikililerinin SHA-256'sı Ex2 derlemesinden **önce ve sonra** yazılır; fark varsa Ö2 durur.
5. Gravite tasarımı yeni sınıflarla yazılır; kod çoğaltması kabul edilir. **PCR-15 bayrak (`DesignMode.Gravity`) seçeneği silindi.**

---

## 10. Doğrulama planı (V0–V5)

- **V0 Ortam:** Ex2 derlemesi; §9.1 SHA önce/sonra; ETABS sürümü; DLL SHA'ları günlükte.
- **V1 Model [K f; E4]:**
  - (a) Şekil 1/2 ↔ model: düğüm/eleman numarası, uzunluk, mesnet, kesit yönelimi (§1.1).
  - (b) **G deseni reaksiyon toplamı = 1 440 + γ·ΣV**, ΣV = Σkolon b·h·3,3 + Σkiriş b·h·L (eksen boyu, düğümde örtüşme her iki elemanda sayılır; ETABS ile aynı kural, rijit bölge yok). **Q deseni = 960 kN. 1,2D+1,6L = 1,2(1 440 + γΣV) + 1 536 kN. 1,4D = 1,4(1 440 + γΣV).** Eşik: FEM ↔ elle hesap ≤ %0,1; FEM ↔ ETABS ≤ %0,1.
  - (c) Sayısal kontrol noktası [Ç], kaynak Tablo 1 kesitleri: ΣV = 3,267 (kolon) + 4,800 (kiriş) = 8,067 m³ (ayrıca 8,067 × 240 = 1 936,08 TL, §6 ile tutarlı); γΣV = 201,68 kN; G = 1 641,68 kN; **1,2D+1,6L = 3 506,01 kN**; 1,4D = 2 298,35 kN.
  - (d) Tezin ETABS modeli (`örnek_2/…EDB`) açılıp açıklığı okunur (Q12). Yeni ETABS modeli OAPI ile kaynak geometrisiyle kurulur.
- **V2 (tek kapı FEM ↔ ETABS [K Q8]):**
  - **V2-a kuvvet (kaynak Tablo 1 kesitleri, 5 grup) [K e]:** kiriş uç ve açıklık momentleri, mesnet kesmeleri, kolon eksenel ve momentleri, reaksiyonlar; 1,2D+1,6L ve 1,4D. Eşik FE4 (statik ≤ %0,1). Bu kesitlerde **D/C karşılaştırması yapılmaz** (kaynak kesitleri büyük olasılıkla yetersiz; ETABS "O/S" verir; dış K3).
  - **V2-a D/C (uygun tasarım, lifsiz) [K e]:** CD1 = 0, CD2 ≥ 0,95, CD3 güvensiz yönde ≤ %5, B1 (kiriş As 0,98–1,10), B2 (gravite Av/s 0,98–1,02) (`VALIDATION_SPEC.md` V2 alt kapıları; Ö1'e özgü FE1–FE3, B3, CV1, JS1, SC1, Q1 uygulanmaz). ETABS: ACI 318-14, NonSway, hareketli yük düzenlemesi yok, aynı kombinasyonlar. Uygun tasarımın kaynağı Q20.
  - Q15 (2B düzlem dışı e_min) V2-a'da kontrol edilir.
  - **V2-b (bilgi):** Tablo 1 kesitlerinin bizim modelde D/C ve gerekli donatısı; 1,2D+1,6L, 1,4G+1,6Q, 1,0G+1,0Q; öz ağırlıklı/öz ağırlıksız. "Kaynak hatalıdır" hükmü verilmez; ek materyal.
  - **V2-c (bilgi):** kaynak fiyatlarıyla beton bedeli 1 936,08 TL ve toplam 11 486 TL, Excel formülüyle.
  - ~~V2-d~~ **çıkarıldı** [K d] (kaynak fiyatıyla optimum kıyası yanıltıcı; dış B4).
  - S1 (SFRC eğilme Mn, bağımsız entegrasyon ≤ %1) lifli kirişler için geçerli kalır.
- **V3 Optimum tasarımların ETABS yeniden analizi** (her senaryo × yöntem en iyi çözümü; 1,2D+1,6L ve 1,4D; NonSway; N/A yasak):
  - Lifsiz: ETABS CFD eğilme ve kesme D/C ≤ 1,0, kolon P-M D/C ≤ 1,0.
  - **Lifli (koşudan önce yazılı yöntem) [K g; iç O7, dış O3]:** (1) kolonlar ve kiriş eğilmesi ETABS CFD ile (lif eğilme katkısı ETABS'te yok → ETABS eğilme D/C lifsiz kesit için okunur; lifli Mn ayrıca S1 yöntemiyle Excel'de); (2) **kiriş kesmesi:** ETABS'ten Vu (kritik kesitte, mesnet yüzünden d uzaklıkta) okunur; kapasite bağımsız formüllü Excel'de hesaplanır: Vc,ACI, ΔVF (Eq. 7.7-4 girdileri bw, d, ρl, fck, fctk, fFtuk, γc görünür), Vs (sağlanan Av/s), φ; D/C = Vu/[φ(Vc,f + Vs)] ≤ 1,0; (3) ETABS'in lifsiz kesme D/C'si yanında "lif katkısız" sütunu olarak raporlanır (> 1 olabilir; N/A değil). Makale ifadesi: "fibre contribution not verified by ETABS; checked by independent calculation". Excel dosyası `04_Results/` altında, IA kontrolüne sunulur.
- **V4 Ötelenme:** yanal yük yok; tez MKÖ 0,09–0,21 mm açıklanır (simetri; birim zinciri).
- **V5 Maliyet:** `Maliyet_Metraj.xlsx` formüllü; amaç ↔ Excel ≤ %0,5; kaynak-replika ayrı sayfa.

---

## 11. Koşu protokolü (sonuç görülmeden yazılı onay şart)

**Sabit satır [K g; dış M1]:** **3 yöntem (YAKO, BTO, ATO) × 2 senaryo (lifsiz; lifli 90 TL/kg) × 30 koşu = 180 koşu** + **fiyat = 0 senaryosu (zorunlu)**: öneri 3 × 30 = 90 koşu, toplam 270 (Q19).
- Kol: **yalnız A** [K Q9]: rastgele başlangıç, tam havuz, ön tasarım yok, kutu yok. Makale cümlesi (iç O6): "Example 2 was run without the predesign stage; in Example 1 the combined effect of the predesign arms was not significant." Ö1 ön tasarım bulguları Ö2'ye genellenmez.
- BTO ve ATO tüm-boyut hamle (`--onedim` yok); YAKO tek boyut (doğası; beyan). Tohum 1001–1030; aynı MaxFE, havuz, ceza; **10 000 FE** üst sınır (düzleşme/optimum iddiası yok).
- 1 000 FE ara değeri günlükte kalır, makaleye **girmez** (dış M2).
- Süre: Ö2 modeli yazıldıktan sonra **100 FE ölçümü protokol onayından önce** yapılır ve ön kayda yazılır; tahmin (3–12 dk/koşu) makaleye girmez (iç D3, dış M6).

**Lif çıktılarının hiyerarşisi (ön kayıt; dış O1, K2):**
1. **Birincil:** (i) en iyi lifli çözümlerdeki tip/doz dağılımı (90 ve 0 TL/kg); (ii) **kalem bazlı marjinal maliyet tablosu [K g, zorunlu]:** lifsiz en iyi çözümlerde (yöntem başına) her kiriş grubuna sırayla ve hep birlikte (tip, doz) ∈ {4D: 20, 30, 40; 5D: 25, 35} uygulanır; kesit sabit, donatı ve etriye yeniden türetilir (`--evalonly`; Q21); beton, boyuna donatı, etriye, kalıp, lif kalemlerinde TL ve % değişim ile gevşeyen kısıt raporlanır; **kiriş maliyeti ve toplam yapı maliyeti ayrı** (dış O5).
2. **İkincil:** lifli ↔ lifsiz Mann–Whitney U + Holm, A12. Lifli uzay lifsiz uzayı kapsadığından bu test "arama verimliliği" ile "lif etkisi"ni ayıramaz; bu cümle sonuç bölümünde yazılır.
3. **Deterministik duyarlılık (en iyi çözümler):** fR × 0,8 (zorunlu, Q18) ve × 1,2; lif fiyatı × 0,5/× 1,5/× 2; kesme formatı Ö1 varyantı (γc = 1,5 + φ, §5.2); kaynak varyant kombinasyonları yalnız V2-b'de.

**Yapılmayan / öneri düzeyinde kalan:** açıklık taraması yok; Ö1 en iyi çözümlerinin gravite kuralıyla yeniden değerlendirilmesi (dış B3) yapılmaz → Ö1–Ö2 nedensel karşılaştırması yapılmaz [K h]; en iyi çözüm komşuluğunda grup grup tarama (dış O2) ve yayımlanmış lif verisiyle kontrol noktası (dış O7) IA'ya öneri olarak bırakıldı (Q22, düşük öncelik).

---

## 12. Makale yazımı: kapsam bildirimi, sınırlamalar, yapılamaz iddialar

**Kapsam bildirimi (taslak):** "Example 2 uses the geometry and loading of the planar, three-storey, three-bay frame of Duysak et al. (2023) (5–6–5 m bays, 3.3 m storeys, G = 30 kN/m and Q = 20 kN/m on every beam; vertical loads only). The source reports neither load combinations, self-weight treatment nor reinforcement detailing; here, member self-weight is added and ACI 318-14 combinations 1.4D and 1.2D + 1.6L are used, with Chapter 18 not applied. Costs use 2026 ÇŞİDB unit prices and are not comparable with the source's total. Steel fibres are an addition of this study. Because fibres receive full shear credit in this gravity-only case, Example 2 represents a limiting case favourable to fibres."

**Sınırlamalar (makalede beyan):** hareketli yük düzenlemesi yok (ACI 6.4.2) [K b]; ötelenmesiz ve düzlem dışı tutulu kabul [K f]; servis yalnız ACI 9.3.1.1 yükseklik sınırı, hesaplı sehim ve çatlak kontrolü yok [K c]; ACI (φ) ile fib MC2010 (Eq. 7.7-4, γc = 1,0) karma format ve Ö1'den farklı format (Ö1 γc = 1,5 + φ; Ö1 donmuş) [K a]; lif dozu ACI 26.4.2.2(d) alt sınırının (60 kg/m³) altında, lifin kesme katkısı ACI'den sapma; fR verisi gizli üretici kaynaklı, karakteristik olup olmadığı belirsiz (Q18); öz ağırlık varsayımı [K Q3]; çatı yükü L kabulü; kaynak bildirinin sınırlılıkları; lif kesme katkısı ETABS ile doğrulanmadı (bağımsız hesapla kontrol); birleşim bölgesi enine donatısı maliyet dışı; boyuna donatı grup boyunca sürekli.

**Yapılamaz iddialar:**
1. "Kaynak optimumu yeniden üretildi / doğrulandı", "validated against Duysak", "benchmark", "compared with the reference optimum" [K h].
2. "Bizim maliyetimiz kaynaktan %X düşük/yüksek" (fiyat kümeleri farklı; V2-d çıkarıldı).
3. "FPA'dan YAKO/BTO/ATO üstündür"; algoritma üstünlüğü (kural 8).
4. "Lif Ö2'de ekonomiktir / değildir" genel olarak; §5.2 formatı, Q18 fR durumu ve gizli veri beyan edilmeden hiçbir lif ekonomisi cümlesi yazılmaz; yalnız bu geometri, yük, fiyat (90/0 TL/kg), veri ve rejim için.
5. "Lifli medyan yüksek, lif pahalıdır" (uzay kapsama; §11 madde 2).
6. "Lif kesme donatısının yerini alır" (ACI dozu karşılanmıyor; Av,min her zaman var).
7. "Lif servis performansını iyileştirir" (servis kısıtı yalnız boyut sınırı).
8. **Ö1 ile nedensel karşılaştırma** ("Ö1'de lifin seçilmemesinin nedeni Bölüm 18'dir", "Ö2–Ö1 farkı sismik detaylamadan kaynaklanır") [K h]. Ö2 yalnız "lifin kesmede tam kredi aldığı elverişli sınır durum" olarak sunulur; Ö1 sonucu ile en çok "uyumlu" denir; Ö1 ve Ö2 lif sonuçları rejim farkı açıklanmadan yan yana konmaz.
9. "Optimum" (global); yalnız "bu çalışmada bulunan en ucuz uygun çözüm".
10. Tez Çizelge 4.11–4.14 değerleri.
11. Kaynak için "ACI 318-14 ile tasarlanmıştır" (kaynak ACI 318-08).
12. "ETABS'te doğrulandı" (V3 öncesi) ve "ETABS lifli kirişi doğruladı" (lif katkısı Excel'de).
13. Deprem veya lifin sismik etkisi üzerine her çıkarım.

**Hakem noktaları (açık):** kaynak seçimi zayıf (dış B5: Literature Analyst, hakemli dergide 2B gravite RC çerçeve örneği arayabilir; ör. Camp, Pezeshk ve Hansson 2003, künye DOĞRULANAMADI); hareketli yük düzenlemesinin olmaması; servis kısıtının yalnız boyut sınırı olması.

---

## 13. Açık kalemler (IA'ya)
Q17 lif kesme sayısal kuralı (Δ / max) · Q18 fR karakteristik kabulü ve × 0,8 zorunlu duyarlılık · Q19 fiyat = 0 koşu sayısı · Q20 V2-a uygun tasarımın kaynağı · Q21 zorlanmış lif deterministik mi · Q22 komşuluk taraması ve yayımlanmış lif verisi kontrol noktası (öneri). Eski Q4–Q7, Q11–Q15 öneri/yazım düzeyinde; itiraz yoksa öneri uygulanır.
