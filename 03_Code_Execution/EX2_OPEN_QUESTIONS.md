# EX2_OPEN_QUESTIONS.md — Ö2 soruları (EX2_SPEC.md v2 ile birlikte; güncelleme 2026-11-16)

v2: iki hakem raporu (`Review_Round_Ex2_spec_ic.md`, `_dis.md`) ve IA kararları (Q1, Q2, Q3, Q8, Q9; ek a–i) işlendi. Önerilerin hiçbiri IA onayı olmadan uygulanmaz.

## 1. Kapanan sorular (IA kararı)

| # | Soru | Karar [K] | Spec yeri |
|---|---|---|---|
| Q1 | Grup yapısı | V2 kuvvet karşılaştırması 5 grup (A); ana optimizasyon 9 grup (B) | §1.2 |
| Q2 | Yük kombinasyonu | Ana: 1,2D+1,6L (1,4D zarfta, belirleyici değil [Ç]); 1,0G+1,0Q ve 1,4G+1,6Q yalnız V2-b'de raporlanır | §3 |
| Q3 | Öz ağırlık | Ayrıca eklenir (kolon + kiriş); ST'ye sorulmaz | §3, §10 V1 |
| Q8 | V2 ölçütü | Tek kapı FEM↔ETABS; kaynak karşılaştırması bilgi amaçlı; V2-d çıkarıldı | §10 |
| Q9 | Kol | Yalnız A; ön tasarım sınıfı yazılmaz | §11 |
| Q10 | Kaynak-replika fiyat seti | PCR-19 silindi; `Maliyet_Metraj.xlsx` ayrı sayfada formülle [K d] | §7 |
| Q16 | Hareketli yük düzenlemesi (ACI 6.4.2) | Uygulanmaz; sınırlama; ETABS'te de tek desen [K b] | §3, §12 |
| — | Lif kesme güvenlik formatı | Ana: ACI nominal ile tutarlı (γc = 1,0, karakteristik fR, φ = 0,75); Ö1 formatı (γc = 1,5 + φ) duyarlılık; Ö1 donmuş, fark makalede beyan [K a] | §5.2 |
| — | Servis | ACI 9.3.1.1 yükseklik sınırı kısıt (havuzda pasif [Ç]); hesaplı sehim/çatlak sınırlama [K c] | §5.1 |
| — | Derleme | Ayrı çözüm, donmuş DLL'lere SHA kayıtlı dosya referansı; PCR-15 bayrak seçeneği silindi [K d] | §9.1 |
| — | V2-a | Kaynak kesitleriyle yalnız kuvvet; D/C uygun tasarımla [K e] | §10 |
| — | V1, kalıp, etriye, yönelim, 1,4D, çatı yükü, ötelenmesiz kabul | Yazıldı [K f] | §1.1, §3, §5.1, §5.3, §5.4, §10 |
| — | Koşu sayısı, fiyat = 0, marjinal maliyet tablosu, lifli V3 | 180 + fiyat = 0 senaryosu; tablo zorunlu; lifli V3 yöntemi yazıldı [K g] | §10 V3, §11 |
| — | Makale dili | "benchmark", "validated against Duysak" yasak; Ö2 "elverişli sınır durum"; Ö1 ile nedensel karşılaştırma yok [K h] | §0, §12 |
| — | ACI 9.6.3.1 / 26.4.2.2(d) | **DOĞRULANDI**: "Contain at least 60 kg of deformed steel fibers per cubic meter of concrete." (ACI 318M-14, PDF 462 / basılı 460); Tablo 9.6.3.1 PDF 140 / 138 (A17). 20–40 kg/m³ < 60 → Av,min her zaman [K i] | §5.2 |

## 2. Açık sorular (IA'ya)

| # | Soru | Neden gerekli | Öneri | Karara bağlı sonuç |
|---|---|---|---|---|
| **Q17** | Lif kesme katkısının sayısal kuralı: (a) **artım**: Vc,f = Vc,ACI + [VRd,F(fFtuk; γc=1) − VRd,F(0; γc=1)]; (b) Vc,f = max(Vc,ACI; VRd,F(γc=1)); (c) Vc yerine doğrudan VRd,F(γc=1) | MC2010 VRd,F beton + lifi tek terimde verir; doğrudan konursa (c) doz 0'da bile ACI Vc'den ≈ %15 yüksek [Ç, ρl = %1 örneği] → lifsiz beton farkı lif katkısı gibi görünür | **(a)**: yalnız lifin etkisini ekler, doz 0'da lifsizle süreklidir, çift sayım yok | Lifin kesmedeki kaldıracı; PCR-15 kesme sınıfı bu karar olmadan yazılmaz |
| **Q18** | `FibreTable.vb` fR değerleri karakteristik mi ortalama mı? (DOĞRULANAMADI) | IA "karakteristik fR" dedi; veri ortalama ise lif katkısı güvensiz yönde abartılır (dış B2) | Kaynak teyit edilene dek karakteristik kabul + beyan; fR × 0,8 deterministik yeniden değerlendirme **zorunlu** tablo; yayımlanmış veriyle (Venkateshwaran 2018 / Chen 2021) tek kontrol noktası isteğe bağlı | Lifli sonuçların savunulabilirliği |
| **Q19** | Fiyat = 0 senaryosu kaç koşu? | Karar "180 koşu + fiyat = 0 senaryosu"; sayı yazılı değil (dış M1 tek satır ister) | 3 yöntem × 30 = 90 koşu (aynı protokol); toplam 270 | CPU süresi; ön kayıt satırı |
| **Q20** | V2-a D/C karşılaştırması için uygun tasarım nereden? (koşulardan önce gerekiyor) | Kaynak kesitleri büyük olasılıkla yetersiz | 30 tohumun dışında (ör. tohum 1000) tek pilot koşu: lifsiz, 9 grup, ATO, 1 000 FE; sonuç raporlanmaz, yalnız V2-a girdisi. Aynı koşu 100 FE süre ölçümünü de verir | V2-a'nın koşu öncesi kapanması |
| **Q21** | Zorlanmış lif (marjinal maliyet tablosu): deterministik mi, sabit dozla yeniden optimizasyon mu? | Dış K2(a) iki seçenek verir | **Deterministik**: lifsiz en iyi çözümlerde kesit sabit, donatı/etriye yeniden türetilir (`--evalonly`, ≈ 30 değerlendirme/çözüm); yeniden optimizasyon gerekirse ayrıca sorulur | İş yükü; tablonun yorumu ("kesit sabitken lifin etkisi") |
| **Q22** | (düşük öncelik) En iyi çözüm çevresinde grup grup komşuluk taraması (dış O2) | "Optimum" demeden yerel en iyilik kanıtı | Öneri; yüzlerce FE | Makale iddiasının gücü |

## 3. Öneri düzeyindeki sorular (itiraz yoksa öneri uygulanır)

| # | Soru | Öneri |
|---|---|---|
| Q4 | Kat yüksekliği | 3,3 m (kaynak DOĞRULANDI) |
| Q5 | Beton sınıfı | C30 sabit |
| Q6 | Havuz | b 250–450, h 300–600, 50 mm |
| Q7 | Gravite rejimi, kolon ρ | Bölüm 18 ve TBDY yok; ρ %1–%4 (gravite kolonlarında ρmin ve alt kesit belirleyici olabilir; lif etkisi kiriş maliyeti üzerinden de raporlanır) |
| Q11 | Doz ızgarası | Ö1 kuralı: aralığa kırpma (5D: 20 → 25, 40 → 35); makalede bir cümle |
| Q12 | Tezin ETABS modeli | V1'de açılıp açıklık okunur |
| Q13 | Metin düzeltmeleri | MM §3.1.2 "three bays (5–6–5 m)", "1.4G + 1.6Q" → 1,2D+1,6L; tez "tüm açıklıklar 5 m"; "Gebrail" → Bekdaş |
| Q14 | Kaynak PDF `Papers/` | Gemini/IA kopyalar; makaleye girmeden önce |
| Q15 | 2B düzlem dışı e_min | V2-a'da ETABS ile kontrol; düzlem dışı tutulu, k = 1,0 |

**Hakem noktaları:** kaynak seçimi zayıf (bildiri, DOI yok, donatı sabit); hareketli yük düzenlemesi yok; servis yalnız boyut sınırı; lif dozu ACI alt sınırının altında; gizli üretici lif verisi; Ö1–Ö2 format farkı.
