# EX2_SPEC.md — Örnek 2 (geometri ve yük Duysak, Nigdeli ve Bekdaş 2023'ten uyarlanmış): 2B, 3 kat × 3 açıklık, 21 elemanlı çerçeve, yalnız düşey yük

Durum: **TASLAK v2.3 (2026-11-17: post-hoc PCR-23/24/25, §A.6, §11.2; kampanya 2 koşuluyor).** Önceki: **TASLAK v2.2 (2026-11-16; v2 Method Architect, v2.1 güncellemesi Developer Agent: IA kararları Q17–Q22, servis (a) ve hakem 2. turu, §A.4; v2.2: PCR-20/21/22, §A.5).** v1 → v2: iki hakem raporu (`06_Review_Audit/REVIEW_LOG/Review_Round_Ex2_spec_ic.md` KOŞULLU ONAY, `..._dis.md` BÜYÜK REVİZYON) ve IA kararları (Q1, Q2, Q3, Q8, Q9; ek kararlar a–i, 2026-11-16) işlendi. Kod, PROGRAM, Sources ve ikili dosyalar değiştirilmedi; koşu başlatılmadı. Açık sorular: `EX2_OPEN_QUESTIONS.md`.

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
| Q17 | Lif kesme katkısının sayısal kuralı | **KARAR [K 2026-11-16]** | Artım kuralı ana seri; ρl = AsTop, σcp = 0, fR3/fR1 > 0,5; varyantlar MC2010-yerel ve Ö1 (deterministik) — §A.4 |
| Q18 | fR karakteristik mi | **KARAR [K]** | Karakteristik kabul + beyan; fR × 0,8 ve × 0,6 (eğilme + kesme birlikte) deterministik |
| Q19 | Fiyat = 0 senaryosunun koşu sayısı | **KARAR [K]** | 3 × 30 = 90 koşu (toplam 270); en iyi çözümlerde grup grup lif sıfırlama (`--zerofibre`) |
| Q20 | V2-a için uygun tasarımın kaynağı | **KARAR [K]; yapıldı** | Tohum 9001, ATO, 1 000 FE; T = 0, kritik D/C ≥ 0,8 ölçütü (`EX2_CODE_LOG` §5c) |
| Q21 | Zorlanmış lif: deterministik mi yeniden optimizasyon mu | **KARAR [K]** | Deterministik, "kesit sabitken" başlığı; etriye belirleyici ölçüt sütunu (`--marginal`) |
| Q22 | Komşuluk taraması | **KARAR (koordinatör 2026-11-16)** | Lifsiz ve lifli en iyi çözümlerde tek geçiş koordinat taraması (`--scan`); yalnız "bu komşulukta yerel en iyi / değil" |

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

### A.4 v2.1 (2026-11-16): IA kararları ve hakem 2. turu (`Review_Round_Ex2_oneriler_ic.md`, `_dis.md`) sonrası uygulanan kurallar
- **Kesme (Q17):** Vc,ACI = 22.5.5.1 basit denklemi (0,17λ√fc′ bw d; Tablo 22.5.5.1 ayrıntılı form kullanılmaz). Vc,f = Vc,ACI + max(0, VRd,F(γc = 1) − VRd,F(0; γc = 1)); VRd,F = fib Bulletin 56 Vol. 2 Eq. (7.7-4), alt sınır Eq. (7.7-5) iki terimde (numaralar PDF 179–180'den teyit, kanıt A54); **ρl = AsTop/(bw d)** (mesnet çekme donatısı; grup boyunca sürekli donatı ≥ ℓbd + d koşulunu sağlar), **σcp = 0**; geçerlilik fR3/fR1 > 0,5 (MC2010 Eq. 5.6-3; sağlanmazsa ΔVF = 0; fR1/fL > 0,4 fL olmadığı için kontrol edilemez). Her çözümde Vc,f/Vc,ACI ve VRd,F(0; γc = 1)/Vc,ACI oranı ve Eq. 7.7-5'in doz-0 teriminde bağlayıcı olup olmadığı raporlanır. Deterministik varyantlar: **MC2010-yerel** Vu ≤ φ(Vc,ACI + Vs) + ΔVF(γc = 1,5); **Ö1** = Ö1 kodunun birebir formülü (VRd,F(γc = 1,5; ρl = AsBot) Vc yerine, sonra φ; biçim ve toplama kuralı birlikte farklıdır). Sıralama: ana ≥ MC2010-yerel (lif terimi).
- **fR (Q18):** fR × 0,8 ve × 0,6 eğilme (S1) ve kesmeye birlikte (`--fr`).
- **Servis [K 2026-11-16, seçenek (a)]:** kaynaktaki 30 kN/m korunur; ACI 9.3.1.1 bağlayıcı kısıt **değildir** (yalnız rapor); ACI 24.2 hesaplı sehim (ℓ/480; ℓ/240 ve anlık L ℓ/360 bilgi) yalnız en iyi çözümlere deterministik rapor (`--deflection`); makalede sınırlama. §3'teki "duvarı içerir" ifadesi bu kararla ℓ/480 raporunun gerekçesidir.
- **Ötelenmesiz kabul ve Q:** her değerlendirmede kat Q = ΣPu Δo/(Vus ℓc) birim yanal yükle (ACI 6.6.4.4.1; kolon 0,70Ig, kiriş 0,35Ig; A52) hesaplanır. Q ≤ 0,05 olan katta düzlem içi narinlik ihmal sınırı 34 + 12M1/M2 ≤ 40 (6.2.5(b)(c)); Q > 0,05 olan katta 22 (6.2.5(a); donmuş δns, Cm = 1). §5.1'deki "Q yanal yük yokken hesaplanamaz" gerekçesi **geri çekildi**. Simetrik tasarım ve yükte M2s ≈ 0 olduğundan δs uygulanmaz (beyan). Düzlem dışı: tutulu varsayımı (ötelenmesiz sınır). **v2.2: bu maddedeki "Q > 0,05 katında 22" ve "δs uygulanmaz" kuralları PCR-22 ile değişti (§A.5).**
- **Desenli yük (ACI 6.4.1/6.4.2):** optimizasyon kısıtı değil [K b]; en iyi çözümlerde deterministik araç (`--patterns`): tasarlanmış donatıyla D/C ve desenli zarfla yeniden türetme maliyeti.
- **Araçlar:** `--marginal` (Q21), `--scan` (Q22), `--zerofibre` (Q19), `--patterns`, `--deflection`; hepsi koşu sonrası, yalnız en iyi çözümlerde.

### A.5 v2.2 (2026-11-16): PCR-20/21/22 (iç Critic `Review_Round_Ex2_acik_ic.md` + dış hakem `_dis.md` ortak görüşü + ETABS V2-a BULGU-1; IA "kararları hakemlerle al" yetkisi)
| PCR | Kural | Kod | Durum |
|---|---|---|---|
| **PCR-20** | ACI 318M-14 **6.2.6**: ikinci mertebe etkilerini içeren Mu ≤ 1,4 × birinci mertebe Mu (6.2.5 ile narinliği ihmal edilmeyen kolonlarda). Oran = δns·max(\|M2ns + δs M2s\|, Pu·e_min) / max(\|M\|, Pu·e_min), her uçta, her ana kombinasyonda, **düzlem içi ve düzlem dışı** ayrı; e_min = 15,24 + 0,03h (PCR-1 ile aynı). Kısıt **g6** = max(oran)/1,4 − 1 (grup bazında); koşu dosyasında `violation_counts` içinde sayılır | `ColumnDesignGravity.Ratio626Of`, `ColumnGravityResult.Ratio626/In/Out`; `Ex2Evaluator` g6 | Uygulandı; ETABS yeniden doğrulaması bekler (V2-a CD1) |
| **PCR-21** | Kolon **lu = net yükseklik**, Ö1 kuralı birebir: lu = L − h_kiriş(alt)/2 − h_kiriş(üst)/2 (düğüme bağlanan en derin kiriş; taban ucu 0). Ara katlarda eşit kirişle lu = 3,3 − hb, 1. katta 3,3 − hb/2. Narinlik ihmal kontrolü (6.2.5) ve δns'deki Pc bu lu ile. Q (6.6.4.4.1) için **ℓc = 3,3 m (eksen)** değişmedi | `ColumnDesignGravity.ClearHeight` | Uygulandı. ETABS: "unbraced length ratio" (major ve minor) = lu/L açıkça atanmalı (ETABS kendiliğinden 1,0 = eksen boyu alıyor; V2-a koşu D) — `EtabsEx2/Program.vb` notu; araç bu pakette derlenmedi/çalıştırılmadı |
| **PCR-22** | Q > 0,05 katında (ötelenmeli, 6.6.4.3): (i) narinlik ihmali (22 sınırı) **hiç uygulanmaz** (k ≥ 1 hesaplanmıyor; k = 1 güvensiz), δns (Cm = 1, EI = 0,4EcIg/(1 + βdns), Pc lu ile) her zaman hesaplanır; (ii) δs = 1/(1 − Q) ≥ 1 (6.6.4.6.2(a)), M2 = M2ns + δs M2s (6.6.4.6.1), toplam δns·\|M2\| (muhafazakâr zarf); (iii) **g7: Q ≤ 1/3** (δs ≤ 1,5; aksi hâlde (b)/(c) gerekir, kodlanmadı); (iv) M2s = net kat ötelenme kuvvetlerinin (her kat düğümünde U1 tutulu analiz tepkilerinin kat toplamı, eksi işaretle, kat düğümlerine eşit dağıtılmış) tutulmamış brüt çerçevedeki momentleri; düğüm tepkileri tek tek değil kat toplamı kullanılır (kiriş eksenel uzamasıyla simetrik yayılma ötelenme sayılmaz). Simetrik çerçeve + simetrik yükte M2s = 0 ve kat ötelenmesi 0 (hesapla gösterilir, raporlanır); kod geneldir (desenli yük aracında M2s ≠ 0 ve δs devreye girer). Düzlem dışı: tutulu varsayımı (ötelenmesiz sınır) değişmedi | `Stability2D.Assess`, `StoreyStability` (Q, NonSway, δs, kat ötelenmesi (kat ortalaması), Ms,max, net kat kuvveti), `RcCombos2D.Analyze(braced:=True)`, `ColumnDesignGravity.DeltaNsNoExemption` | Uygulandı. Her en iyi çözümde Q, δs, kat ötelenmesi ve Ms,max koşu dosyasına (`stability,` satırı) ve `--evalonly` çıktısına yazılır |
| küçük | Araç çıktısı: kiriş başına `AvS_strength`, `AvS_min`, `AvS_required`, `AvS_provided`, `smax`; etriye etiketi "smax" yerine "smax(Av/s by strength)" / "smax(Av/s by Avmin)"; pilot satırında "requiredAvS_byStrength" ve "spacingGovernedByStrength(s<smax)" ayrı | `BeamShearGravityResult`, `Tools` | Uygulandı; kesme kritik kesiti (düğümden d) **değişmedi** |

Makale dili (iç Critic + dış hakem): "Her değerlendirmede kat stabilite indeksi Q (ACI 6.6.4.4.1) hesaplanmış; Q ≤ 0,05 katlar ötelenmesiz, diğerleri ötelenmeli (6.6.4.6; δs = 1/(1 − Q), Q ≤ 1/3) tasarlanmıştır; simetrik gravite yükü altında M2s = 0'dır. Ötelenmeli katlarda narinlik ihmali kullanılmamıştır. Düzlem dışında kolonlar tutulu kabul edilmiştir." "Çerçeve ötelenmesiz kabul edilmiştir" cümlesi kullanılmaz. Kaç en iyi tasarımda ötelenmeli kat çıktığı raporlanır.

### A.6 v2.3 (2026-11-17): post-hoc PCR-23/24/25 (iç Critic `Review_Round_Ex2_sonuc_ic.md` + dış hakem `_dis.md`; IA "hakemlerle karar al" yetkisi; ayrıntı §11.2, `EX2_CODE_LOG` §5g)
| PCR | Kural | Durum |
|---|---|---|
| **PCR-23** | SFRC eğilme kapasitesi her As için tutarlı sabit nokta lcs ile (Required, PhiMn, εt aynı fonksiyon); g8 = kiriş eğilme D/C ≤ 1; donmuş `AciBeam.RequiredAs` geçiş bölgesi açığı kapatıldı | Uygulandı; lifsiz yol (1)–(2) bit-bit aynı |
| **PCR-24** | Kiriş eksenel çekmesi: eğilmede ACI 22.2 dengesi (As(Mu − Nt(d − h/2)) + Nt/(φfy)), kesmede ACI 22.5.7.1; basınç ihmal | Uygulandı; ETABS V2-a B1 = 1,0000 (BULGU-2 kodda KAPANDI) |
| **PCR-25** | ACI 6.4.2 desenli hareketli yük **tasarım zarfında** (kiriş ve kolon; [K b] geri alındı); L ≤ 0,75D muafiyeti kirişlere uygulanmaz (6.4.3.2 yalnız iki doğrultulu döşeme; kanıt A59, A60) | Uygulandı |

Kampanya 1 (§11.1) **superseded** etiketiyle korunur; kampanya 2 (§11.2) aynı tohumlarla 270 koşu. Desenli zarfta hesaplı sehim (ACI 24.2) hâlâ yalnız rapor ve tüm açıklıklar yüklü L ile (dış hakem R4: AÇIK).

**Kod yazımı şartı (v2, tarihsel):** Q17 ve Q18 IA tarafından cevaplanmadan PCR-15 kesme sınıfı yazılmaz. Diğer PCR'ler (14, 16, 18'in yalıtım kısmı) A.2 onayıyla başlayabilir. Koşu protokolü (§11) Q19–Q21 cevaplanınca ön kayda alınır.

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
| Hareketli yük düzenlemesi | **v2.3 (PCR-25, 2026-11-17): uygulanır — 24 desen + 2 ana kombinasyon tasarım zarfında (kampanya 2); aşağıdaki [K b] metni kampanya 1 içindir (superseded).** **[K b] Uygulanmaz; tüm açıklıklar aynı anda yüklü.** ACI 6.4.2: "For one-way slabs and beams, it shall be permitted to assume (a) and (b): (a) Maximum positive Mu near midspan occurs with factored L on the span and on alternate spans" (PDF 71 / basılı 69). Gerekçe: kaynakla uyum ve FE maliyeti. Sonuç: açıklık pozitif momenti ve kesme düşük tahmin edilebilir (L/D = 20/30; etki ölçülmedi). **Sınırlama** (§12). ETABS V2-a/V3'te de tek desen (zarf FEM ile aynı) | DOĞRULANDI (madde); karar [K] |
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
| g1 | Kolon P-M D/C ≤ 1 (2B: düzlem içi analiz momenti × δns; düzlem dışı yalnız e_min, PCR-1; v2.2: ötelenmeli katta \|M2ns + δs M2s\| × δns, PCR-22) | ACI 22.4, 6.6.4.5, 6.6.4.6 | Ö1 `AciColumn` aynen |
| g6 | **v2.2 (PCR-20):** ikinci mertebe M ≤ 1,4 × birinci mertebe M, kolon, iki düzlem | ACI 6.2.6 (PDF 67 / basılı 65; `EX2_VALIDATION_REPORT` §4.2 alıntı) | DOĞRULANDI (madde metni V2-a raporunda); kanıt dosyasına A-satırı Literature Analyst işi |
| g7 | **v2.2 (PCR-22):** kat Q ≤ 1/3 (δs ≤ 1,5; 6.6.4.6.2(a) yöntemi geçerlilik sınırı) | ACI 6.6.4.6.2 | Madde metni bu pakette yeniden açılmadı: **DOĞRULANMALI** (A52 R6.6.4.6.2'yi kapsıyor; 1,5 eşiği iç Critic notunda da DOĞRULANMALI) |
| g2 | Kolon ρ: 0,01 Ag ≤ Ast (ızgara %1–%4) | ACI 10.6.1.1 (A22) | DOĞRULANDI (`CODE_PROVISIONS_EVIDENCE.md`) |
| g3 | Kiriş eğilmesi: As ≥ As,min; εt ≥ 0,004 | ACI 9.6.1.2 (A09), 9.3.3.1 | Ö1 TBDY ekleri (alt ≥ %50 üst, ρ ≤ %2) **kaldırılır** |
| g8 | **v2.3 (PCR-23/24/25):** kiriş eğilme D/C ≤ 1 (sağlanan donatıyla, desenli zarf ve eksenel çekme dahil; tolerans 10⁻⁶; g3 ihlalinde değerlendirilmez) | ACI 9.5.1, 22.2, 6.4.2 | Kampanya 2; kampanya 1'de yok (DcFlex yalnız rapordu) |
| g4 | Kiriş kesmesi: Vu ≤ φ(Vc,f + Vs), Vs ≤ 0,66√fc' bw d; Av ≥ Av,min | ACI 22.5, 22.5.1.2 (A14), 9.6.3.3 (A18) | §5.2 |
| g5 | ~~Servis: h ≥ ℓ/18,5 / ℓ/21~~ **v2.1: kısıt değil, yalnız rapor [K servis (a)]; ACI 24.2 sehim raporu §A.4** | ACI Tablo 9.3.1.1 | DOĞRULANDI: "overall beam depth h shall satisfy the limits in Table 9.3.1.1, unless the calculated deflection limits of 9.3.2 are satisfied" (PDF 132 / basılı 130); fy = 420 → düzeltme yok (9.3.1.1.1) |
| g5 [Ç] | 5 m: 5 000/18,5 = 270 mm; 6 m: 6 000/21 = 286 mm (ℓ = eksen açıklığı, muhafazakâr) | | Havuz alt sınırı 300 mm ile **pasif**; yine kısıt olarak kodlanır ve raporlanır |
| — | Madde 9.3.1.1 yalnız "not supporting or attached to partitions or other construction likely to be damaged by large deflections" kirişlere uygulanır (PDF 132) → **varsayım:** kirişler hasar görebilecek bölmeleri taşımaz. Hesaplı sehim (24.2) ve çatlak kontrolü (24.3) **yapılmaz: sınırlama** [K c] | | beyan |
| — | Narinlik: **v2.2: kat Q ile sınıflama; ötelenmeli katta ihmal yok + δs (PCR-22); lu net yükseklik kodda uygulandı (PCR-21; v2.1'e kadar kod eksen boyu kullanıyordu) (§A.5)**; **v2.1: kat Q ile sınıflama (§A.4)**; eski metin: çerçeve **ötelenmesiz (nonsway) kabul** [K f]; ~~Q indeksi yanal yük yokken hesaplanamaz (iç O4)~~ (geri çekildi); düzlem içi k = 1,0, lu = net yükseklik, Cm = 1 (PCR-2); düzlem dışı tutulu varsayımı, k = 1,0. ETABS: "NonSway" çerçeve tipi (V2-a'da belgelenir) | ACI 6.6.4 | **varsayım**, §12 |
| — | Drift, ACI 6.6.4.3 Q ≤ 0,05 (kısıt olarak; v2.2'de yalnız sınıflama, kısıt g7 = Q ≤ 1/3), güçlü kolon, birleşim kesmesi, Mpr kapasite kesmesi, Vc = 0, sarılma bölgeleri (ACI 18.6–18.8), TBDY 7.3/7.4 | | **Uygulanmaz** [K f] |
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
  - **V2-a durumu (v2.2, 2026-11-16):** ilk ETABS turu (`EX2_VALIDATION_REPORT.md`) V1 GEÇTİ, FE4 GEÇTİ, CD1 **KALDI** (ACI 6.2.6, BULGU-1), B1/B2 KISMEN. PCR-20/21/22 sonrası yeni pilot (`04_Results/ex2/v2a_pilot2/`) ile **V2-a yeniden yapılmalı** (CD1–CD3, B1, B2): ETABS'te kolon unbraced length ratio (major/minor) = ClearHeight/L açıkça atanır ve okunur; çerçeve tipi kat Q sınıflamasıyla eşlenir; program 6.2.6 oranı ETABS "Delta_ns exceeds 1.4" mesajıyla yan yana raporlanır. Yeni pilotta B_S2/B_S3 etriye aralığını **dayanım** belirliyor → B2 dayanım kuralını doğrudan sınar. B2 ölçütü önerisi (ETABS US sabitleri 0,166 √f′c ve 60 ksi nedeniyle) "uyumlu sabitlerle 1,00 ± 0,02" IA kararı bekliyor. `ex2_bin/` ikilisi bu yeniden doğrulamadan sonra kampanya ikilisi olarak kesinleşir.
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

### 11.1 Kampanya ön kaydı (2026-11-16/17; sonuç görülmeden yazıldı, koşu başlamadan önce)

Yazan: Claude Code (Developer Agent), koordinatör görevlendirmesiyle. Bu bölüm koşu başlamadan dosyaya yazıldı; koşu sonrası değiştirilmez (değişiklik gerekirse "post-hoc" etiketiyle ayrı alt bölüm açılır).

**İkili (kampanya ikilisi):** `03_Code_Execution/ex2_campaign_bin/` = `ex2_bin/` birebir kopyası (V2-a yeniden doğrulaması GEÇTİ, `EX2_VALIDATION_REPORT.md` §7). `SHA256SUMS_ex2_campaign_2026-11-16.txt` ile `ex2_bin/SHA256SUMS_ex2.txt` satır satır aynı; iki klasörde `sha256sum -c` 8/8 OK. Betik her başlatmada SHA'yı denetler, uyuşmazsa durur.

| Dosya | SHA-256 |
|---|---|
| Ex2Model.dll | `d0514e2f1e270f368333cb15c733cd90478d1bbcf7e66e341106ff122b82c6b3` |
| Ex2Runner.exe | `297b22f8bdf6db1818c62333e627cf1b18fd5a3969bb1b43f2bf261aeb67b7fa` |
| Ex2Runner.exe.config | `ad8267122c40d37dcd9d99e1412d763187a1825732f378db8867f3ecf675cf70` |
| FemCore.dll | `8da9dfd2e9d6f74e992c0ccbf4dbc5b742500ba1876725623db4503a11c9c0c8` |
| FemRcModel.dll | `c1a95951560715777d137be63ebf313102f69dd6e772265eb42ff850b228f5a9` |
| RcCore.dll | `14f84b7552de4020dd653cde19a5e7ccf258b0afa491f6f0be476751a6bdbeef` |
| MathNet.Numerics.dll | `85cbaa2c0a37b8d5517c33930027be26c53df056abc89bf6e7718693720ba254` |
| System.ValueTuple.dll | `ad95c2b972a0bdd45513e26813ecd397b9cc557e66e9fe26ce5633a695c41d1a` |

**Koşu matrisi (toplam 270 koşu):**

| Hücre (klasör `04_Results/ex2/campaign/…`) | Senaryo | Yöntemler | Koşu |
|---|---|---|---|
| `plain` | lifsiz (18 değişken) | YAKO, BTO, ATO | 3 × 30 = 90 |
| `fibre_fp90` | lifli, lif 90 TL/kg (24 değişken) | YAKO, BTO, ATO | 3 × 30 = 90 |
| `fibre_fp0` | lifli, lif 0 TL/kg (zorunlu senaryo, Q19) | YAKO, BTO, ATO | 3 × 30 = 90 |

Ortak ayarlar: yalnız **A kolu** (rastgele başlangıç, tam havuz; ön tasarım yok, kutu yok, ABC yok) [K Q9]; **9 grup**; C30; ana kesme formatı Q17 (`--shearformat delta`); fR × 1,0; `--qcheck 1` (kat Q sınıflaması, g7); **10 000 FE** üst sınır; **tohumlar 1001–1030** (`--seed0 1000`; her yöntem ve senaryoda aynı tohum dizisi). Kampanya dışı tohumlar kullanılmaz: pilot 9001, süre ölçümü 9101, fiyat = 0 sağlama 9101–9105. **BTO ve ATO tüm-boyut hamle** (Ex2Runner `Optimizers.OneDimMove = False`; `--onedim` bayrağı yok), **YAKO tek boyut** (yöntemin yapısı; beyan) — Ö1 ana seri kuralı (MEMORY §3k: BTO/ATO ana seri tüm-boyut) ve bu belge §11 ile aynı. `--dop 8` (makinede 8 mantıksal çekirdek; çalışan Ö1 koşusu yok); `wallSeconds` yalnız günlüktür, makaleye girmez. Betik: `03_Code_Execution/run_ex2_campaign.sh` (yeniden başlatılabilir); günlük `04_Results/ex2/campaign/campaign_log.txt`, son satır "ALL DONE ex2 campaign".

**Süre (ön kayıt; spec §11 şartı):** 10 000 FE, `--dop 1`, tohum 9101 (`EX2_CODE_LOG` §5e): lifsiz 46–47 s/koşu (4,6–4,7 ms/FE), lifli 51–65 s/koşu (5,2–6,5 ms/FE). Paralel koşuda iş başına süre uzar (fiyat = 0 sağlamasında 5 paralel işte 13–16 ms/FE). Süre makaleye girmez.

**Kabul / kalite denetimi (koşu sonrası, sonuç yorumundan önce):** her koşu dosyasında FEs = 10 000, en iyi çözümde T = 0 olup olmadığı, `violation_counts` (g6, g7 dahil) ve `stability,` satırı okunur. T > 0 kalan koşu atılmaz; sayısıyla raporlanır.

**Yorum kuralları (bağlayıcı):**
1. **Rejime bağlılık.** Her lif cümlesi şu kayıtla yazılır: "bu yük/kısıt rejiminde (Ö2: 2B, yalnız düşey yük, ACI 318-14 gravite, Bölüm 18 yok, ana kesme formatı Q17), tek lif ürünü (65/60 BG, 4D/5D), fR tedarikçi ortalaması/karakteristik durumu belirsiz (Q18), ÇŞİDB 2026 fiyatları ile; genellenemez".
2. **"Lif etriyeyi / maliyeti azaltır" iddiası** yalnız şu üç kesme/fR değerlendirmesinin **üçünde de** korunursa yazılır: (a) ana format (Q17, `delta`), (b) MC2010-yerel varyant (`--shearformat mc2010`), (c) fR × 0,8 (`--fr 0.8`). Biri bozulursa iddia yazılmaz; yalnız hangi varyantta bozulduğu raporlanır. Kontrol en iyi çözümlerin deterministik yeniden değerlendirmesiyle yapılır (yeniden optimizasyon değil).
3. **Başabaş lif fiyatı raporlanır** (en iyi çözümlerde lifin kazandırdığı diğer kalem tasarrufu / lif kg; `--marginal` ve `--zerofibre` ile, deterministik). Fiyat = 0 sağlamasındaki kaba gösterge (≈ 8,4 TL/kg) kampanya sonucu değildir, kullanılmaz.
4. **Doz 0 için arama yanlılığı kontrolü:** 90 TL/kg hücresinde en iyi çözümlerde lif dozu 0 çıkarsa bunun arama hatası olmadığı iki yolla sınanır: (i) lifli en iyi çözümlerde (fiyat = 0 ve 90) **grup grup lif sıfırlama** (`--zerofibre`; maliyet ve uygunluk değişimi); (ii) **fiyat = 0 sağlaması** (`fibre_fp0` hücresi: lif bedava iken optimizasyonun lif seçip seçmediği). Her ikisi de raporlanır.
5. **ABC / ön tasarım yok.** Ö2'de ön tasarım kolları koşulmaz; Ö1 ön tasarım bulguları Ö2'ye genellenmez (§11 makale cümlesi).
6. **Bulgular bölümü kapısı:** V3 (en iyi çözümlerin ETABS yeniden analizi, §10) ve S2 tamamlanmadan ve Critic + IA onayı olmadan sonuçlar bulgular bölümüne girmez.
7. **Algoritma üstünlüğü iddiası yok** (AGENTS kural 8, §12 madde 3): YAKO/BTO/ATO çapraz teyit aracıdır; yöntemler arası test yalnız tutarlılık bilgisidir.
8. İstatistik: lifli ↔ lifsiz (ve fp0 ↔ fp90) Mann–Whitney U + Holm, A12; §11 madde 2'deki "arama verimliliği ile lif etkisi ayrılamaz" cümlesi geçerlidir. §12 yapılamaz iddialar listesi aynen geçerlidir ("optimum" yok; "lif pahalıdır/ekonomiktir" genel cümlesi yok; "lif etriyenin yerini alır" yok).

**Yapılmayan:** 1 000 FE ara değeri makaleye girmez; MaxFE düzleşme iddiası yok; Ö1 ile nedensel karşılaştırma yok [K h].

**Yapılmayan / öneri düzeyinde kalan:** açıklık taraması yok; Ö1 en iyi çözümlerinin gravite kuralıyla yeniden değerlendirilmesi (dış B3) yapılmaz → Ö1–Ö2 nedensel karşılaştırması yapılmaz [K h]; en iyi çözüm komşuluğunda grup grup tarama (dış O2) ve yayımlanmış lif verisiyle kontrol noktası (dış O7) IA'ya öneri olarak bırakıldı (Q22, düşük öncelik).


### 11.2 Post-hoc PCR-23/24/25 (2026-11-17; kampanya 1 sonuçları GÖRÜLDÜKTEN sonra; §11.1 metni değiştirilmedi)

Yazan: Claude Code (Developer Agent), koordinatör görevlendirmesiyle; karar dayanağı iç Critic (`Review_Round_Ex2_sonuc_ic.md`, A1, K6) + dış hakem (`Review_Round_Ex2_sonuc_dis.md`, R1–R3), IA'nın "hakemlerle karar al" yetkisi. Bu alt bölüm **kampanya 2 başlamadan önce** yazıldı. Değişiklikler ön kayıt dışıdır ve sonuç incelemesi sırasında bulunmuştur; sonuçtan bağımsız doğruluk/şartname düzeltmeleridir. **Eski seri (kampanya 1) ve yeni seri (kampanya 2) ek materyalde birlikte raporlanır; seçici raporlama yok** (iddia hangi seride bozulursa o raporlanır).

| PCR | Neden | Kural (kampanya 2) | Kampanya 1 ile fark |
|---|---|---|---|
| **PCR-23** | `Required` SFRC ikiye bölmesi her denemeyi bir önceki denemenin lcs'si ile yargılıyordu (bayat lcs); F0 B_S1 alt D/C = 1,0103; `EpsT` üçüncü bir lcs (0,7h) kullanıyordu (iç Critic A1; dış hakem §3) | SFRC kapasitesi her As için kendi sabit nokta lcs'si (`FlexFixed`, |Δlcs| ≤ 10⁻⁶ mm, yakınsamazsa son iki iterenin küçüğü); `Required` kabul kararı bu kapasiteyle + soğuk başlangıçlı doğrulama; `EpsT` aynı lcs ile. **(3)** g8 = kiriş eğilme D/C ≤ 1 (+10⁻⁶; g3 ihlalinde değerlendirilmez) ve donmuş `AciBeam.RequiredAs`'ın geçiş bölgesindeki (0,004 ≤ εt < 0,005) φ iterasyon açığının (D/C ≤ 1,0004) en küçük yükseltmeyle (göreli 10⁻¹²·2ᵏ adımlar, aşım ≤ 2×) kapatılması (`Ex2Options.FlexCheck`) | Lifsiz yol: (1)–(2) bit-bit aynı (aşağıda); (3) yalnız kampanya 2 |
| **PCR-24** | ETABS V2-a BULGU-2: kiriş eksenel çekmesi (13–19 kN) yok sayılıyordu; alt donatı %2,7–3,4 güvensiz | Grup en büyük çekmesi Nt (tasarım zarfı) her yüzün moment zarfıyla eşlenir (muhafazakâr): As = As(Mu − Nt(d − h/2)) + Nt/(φ fy) (ACI 22.2 dengesi, akmış donatı; εt ve φ eğilme parçasından; Pn = Nt/φ); kesmede ACI 22.5.7.1 Vc = 0,17(1 + Nu/(3,5Ag))√fc′ bw d ≥ 0 (Nu çekmede negatif); MC2010 lif artımında σcp = 0 (iki terimde sadeleşir). Basınç ihmal | Lifsiz ve lifli yol değişir |
| **PCR-25** | Dış hakem R1: ACI 6.4.2 desenli yük zorunlu; L ≤ 0,75D muafiyeti (6.4.3.2) yalnız iki doğrultulu döşeme sistemleri için (kanıt A59, A60) | `RcCombos2D.PatternCombos` (24 desen: kat başına ve tüm katlar × {1,3},{2},{1,2},{2,3},{1},{3}) + 1,4D + 1,2D+1,6L = 26 kombinasyon **tasarım zarfı** (kiriş eğilme/kesme, kolon P-M, 6.2.6, Q/δs) | Lifsiz ve lifli yol değişir; [K b] kararı geri alındı |

**İkili:** `03_Code_Execution/ex2_campaign2_bin/` (`SHA256SUMS_ex2_campaign2_2026-11-17.txt`, `sha256sum -c` 8/8 OK):

| Dosya | SHA-256 |
|---|---|
| Ex2Model.dll | `a61b0c6b4ee15114716cc437842a41a4225451b443bee512f57f3898c92c95d8` |
| Ex2Runner.exe | `3a515a72ee579209b605fee1c768d725f11df1e359c664edb0e8a6529abc5983` |
| Ex2Runner.exe.config | `ad8267122c40d37dcd9d99e1412d763187a1825732f378db8867f3ecf675cf70` |
| FemCore.dll / FemRcModel.dll / RcCore.dll / MathNet / ValueTuple | kampanya 1 ile aynı (`8da9dfd2…`, `c1a95951…`, `14f84b75…`, `85cbaa2c…`, `ad95c2b9…`) |

**Lifsiz bit-bit regresyon (PCR-23 (1)–(2)):** yeni ikili `--patterned 0 --beamaxial 0 --flexcheck 0` ile lifsiz YAKO/BTO/ATO, tohum 1001–1003 (9 koşu): koşu dosyaları kampanya 1 dosyalarıyla sürüm satırı ve `wallSeconds` dışında **9/9 birebir** (maliyet, x, kısıt sayıları, tarihçe). Kanıt `03_Code_Execution/ex2_work/regression_pcr23/`.

**Kapsam:** PCR-24 ve PCR-25 lifsiz yolu da değiştirdiği için **270 koşunun tamamı** yeniden koşulur: `plain`, `fibre_fp90`, `fibre_fp0` × YAKO/BTO/ATO × 30; tohumlar 1001–1030 (`--seed0 1000`); diğer tüm ayarlar §11.1 ile aynı (A kolu, 9 grup, Q17, fR × 1,0, `--qcheck 1`, 10 000 FE, BTO/ATO tüm-boyut, YAKO tek boyut, `--dop 8`) + `--patterned 1 --beamaxial 1 --flexcheck 1`. Çıktı `04_Results/ex2/campaign2/` (eski seri `04_Results/ex2/campaign/` değişmeden kalır); betik `03_Code_Execution/run_ex2_campaign2.sh` (SHA denetimi, yeniden başlatılabilir, son satır "ALL DONE ex2 campaign2").

**Kabul ve yorum kuralları:** §11.1'in kabul denetimi ve yorum kuralları 1–8 aynen geçerlidir (g8 dahil `violation_counts`). İstatistik ve deterministik araç paketi (`ex2_stats.py`, `ex2_analysis_run.sh` eşdeğeri) yeni seri için ayrıca üretilir; eski seri "kampanya 1 (superseded: PCR-23/24/25 öncesi)" etiketiyle ek materyalde kalır. Kampanya 1'in 270 son vektörünün düzeltilmiş kodla deterministik yeniden değerlendirmesi (`--evalonly`, üç kural seti) `04_Results/ex2/reeval_pcr23_25/` altındadır; bu yeniden değerlendirme istatistik tablosu yerine geçmez.

**Şerh (iç Critic, ön kayıt durumu):** §11.1'deki "sonuç görülmeden yazıldı" beyanı kampanya sonuçları içindir; fiyat = 0 sağlama sınaması (tohum 9101–9105; `EX2_CODE_LOG` §5f) ön kayıttan **önce** görülmüştü ve kural 2–4 bu bilgiyle yazıldı (kural 3 sağlama değerini dışlar). Makale/ek materyal cümlesi: "pre-registered before the campaign results were seen; a 30-run zero-fibre-price sanity check had been seen".

---

## 12. Makale yazımı: kapsam bildirimi, sınırlamalar, yapılamaz iddialar

**Kapsam bildirimi (taslak):** "Example 2 uses the geometry and loading of the planar, three-storey, three-bay frame of Duysak et al. (2023) (5–6–5 m bays, 3.3 m storeys, G = 30 kN/m and Q = 20 kN/m on every beam; vertical loads only). The source reports neither load combinations, self-weight treatment nor reinforcement detailing; here, member self-weight is added and ACI 318-14 combinations 1.4D and 1.2D + 1.6L are used, with Chapter 18 not applied. Costs use 2026 ÇŞİDB unit prices and are not comparable with the source's total. Steel fibres are an addition of this study. Because fibres receive full shear credit in this gravity-only case, Example 2 represents a limiting case favourable to fibres."

**Sınırlamalar (makalede beyan):** ~~hareketli yük düzenlemesi yok (ACI 6.4.2) [K b]~~ (v2.3: kampanya 2'de desenli zarf uygulanır; kampanya 1 için sınırlama olarak kalır); kiriş eksenel çekmesi grup en büyüğü ile moment zarfı eşlenerek (muhafazakâr), kesmede ACI 22.5.7.1, MC2010 lif artımında σcp = 0 (v2.3); lifin **eğilme** katkısı ACI dışıdır (R26.4.1.5 yalnız kesme) ve eğilmede γF = 1,5 + φ, kesmede γc = 1,0 + φ karma format (dış hakem R5; beyan); lcs = h − c (MC2010 lcs = min(srm, y); srm ihmali beyan); hesaplı sehim desenli L ile yapılmadı (R4, AÇIK); ~~ötelenmesiz kabul~~ (v2.2: kat Q sınıflaması + ötelenmeli katta δs, §A.5) ve düzlem dışı tutulu kabul [K f]; δs yalnız 6.6.4.6.2(a) yöntemiyle (Q ≤ 1/3 kısıtı); ötelenmeli katta δns·(M2ns + δs M2s) zarfı (muhafazakâr); kesme kritik kesiti düğümden d (kolon yüzünden değil; Vu en çok wu·hc/2 fazla); servis yalnız ACI 9.3.1.1 yükseklik sınırı, hesaplı sehim ve çatlak kontrolü yok [K c]; ACI (φ) ile fib MC2010 (Eq. 7.7-4, γc = 1,0) karma format ve Ö1'den farklı format (Ö1 γc = 1,5 + φ; Ö1 donmuş) [K a]; lif dozu ACI 26.4.2.2(d) alt sınırının (60 kg/m³) altında, lifin kesme katkısı ACI'den sapma; fR verisi gizli üretici kaynaklı, karakteristik olup olmadığı belirsiz (Q18); öz ağırlık varsayımı [K Q3]; çatı yükü L kabulü; kaynak bildirinin sınırlılıkları; lif kesme katkısı ETABS ile doğrulanmadı (bağımsız hesapla kontrol); birleşim bölgesi enine donatısı maliyet dışı; boyuna donatı grup boyunca sürekli.

**Yapılamaz iddialar:**
0. (v2.3) "Tasarımlar ACI 318-14'e uygundur" / "code-compliant" — kampanya 1 için hiç (desenli yük ve kiriş eksenel kuvveti yoktu); kampanya 2 için yalnız "ACI 318-14 with fib MC2010 fibre contributions (declared deviation)" ve tanımlı kısıt kümesi sınırıyla. "Başabaş fiyat X TL/kg'dır" tek sayı olarak (tek tasarım başabaşı = alt sınır; sistem düzeyi değil; fR aralığıyla verilir).
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
v2.3 (2026-11-17): BULGU-2 **KAPANDI** (PCR-24; B1 1,0000); desenli yük **KAPANDI** (PCR-25, tasarım zarfında); A1 **KAPANDI** (PCR-23). Açık: kampanya 2 sonuçları + istatistik/deterministik paket yeniden üretimi; desenli L ile hesaplı sehim ve ℓ/480 ↔ "bölme taşımaz" tutarsızlığı (dış R4); eğilmede γF + φ ↔ kesmede γc = 1 + φ format beyanı (R5); fiyat taraması / başabaş aralığı (R6); düzlem dışı kolon varsayımı duyarlılığı (R8); V3 (P ve F0 dahil) ve S1 lifli Mn bağımsız kontrolü; Ö1 aynı bayat-lcs kalıbı (etki `04_Results/ex2/o1_required_audit.md`; Ö1 düzeltmesi ayrı PCR + IA).
v2.2 (2026-11-16): kolon lu **KAPANDI** (net yükseklik, PCR-21); Q > 0,05 makale dili ve δs **KAPANDI** (PCR-22, §A.5); ACI 6.2.6 **KAPANDI** (PCR-20, kısıt g6); kesme kritik kesiti: kod değişmez, "düğümden d" beyanı (iç Critic ve dış hakem ortak). Hâlâ açık: V2-a'nın yeni pilotla ETABS'te yeniden yapılması (CD1 kapısı), B2 ölçütünün "uyumlu sabitlerle" yorumu, BULGU-2 (kiriş eksenel çekmesi) kapsam kararı, 6.6.4.6.2 (1,5 eşiği) ve 6.2.6 madde metinlerinin kanıt dosyasına A-satırı, dış hakem madde 4 (ana lif kesme formatı MC2010-yerel önerisi; iç Critic Q17'nin korunmasını öneriyor; bu pakette değiştirilmedi), dış hakem madde 6 ön koşulu (fiyat = 0 sağlama sınaması). v2.1 notu: Q17–Q22 kararlandı (§A.1, §A.4). Açık (v2.1): kesme kritik kesitinde yüz konumu (düğüm mü düğüm + hc/2 mi; `EX2_CODE_LOG` AÇIK-14), kolon lu (eksen mi net mi; AÇIK-15), ETABS'in Vc denklemi ve kesme istasyonu (V2-a B2), Q > 0,05 bulgusunun makale dili (ötelenmesiz kabul yerine kat bazlı sınıflama), yayımlanmış lif verisiyle kontrol noktası (dış O7). Eski Q4–Q7, Q11–Q15 öneri/yazım düzeyinde; itiraz yoksa öneri uygulanır.

## 11.3 Kampanya 3 — PCR-26 (IA kararı 2026-11-18, seçenek A; sonuç görülmeden önce kayıt)
- **Karar:** sehim ihtilafında (iç Critic A, dış hakem C+B) IA "A önerisini yap" dedi: ACI 318M-14 Tablo 24.2.2 ℓ/480 (eklenti sonrası toplam sehim) **bağlayıcı kısıt g5**; hesap `Deflection2D.Report` ile birebir aynı (rapor sürümü değişmedi). Lif sertliği yok sayılır (lif aleyhine, muhafazakâr; makalede beyan).
- **Kod:** `Ex2Options.DeflCheck` (varsayılan False → kampanya 1/2 kuralları birebir), `--deflcheck 1`; g3 ihlalli (As sonsuz / εt<0,004) tasarımda g5 hesaplanmaz (çift ceza/NaN yok). Test T26; birim testi 127/127.
- **İkili:** `ex2_campaign3_bin/` (Ex2Model ee759289…; FemRcModel c1a95951… dondurulmuş Ö1; SHA256SUMS_ex2_campaign3_2026-11-18.txt). Betik `run_ex2_campaign3.sh`; çıktı `04_Results/ex2/campaign3/`. 270 koşu (plain, fibre_fp90, fibre_fp0), 30 tohum (1001–1030), 10 000 FE; ≈26 ms/FE → ≈2,5 saat.
- **Raporlama kuralı:** kampanya 1, 2, 3 birlikte raporlanır; ana seri kampanya 3. Lif fiyat taraması **yok** (IA daha önce reddetti); iç Critic'in önerdiği BTO+ATO 10'ar koşu fiyat noktası IA isterse yapılır. İddia kuralı §11.1 aynen geçerli (lif maliyet düşürür = ana format + MC2010-yerel + fR×0,8'de birlikte; "lif etriyeyi azaltır" yazılamaz; "optimum" yok).
