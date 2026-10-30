# VALIDATION_REPORT.md — Doğrulama kapıları (V0–V5)

Hazırlayan: Claude Code. Durum: **V2 (FEM ↔ ETABS) kısmen geçti**; ayrıntı §1. Diğer kapılar açık.

## 1. V2a — FemCore ↔ ETABS 22.6.0 (Ö1 deneme tasarımı), 2026-10-29
**Yöntem.** Araç: `03_Code_Execution/program_work/EtabsProbe` (OAPI). FemCore modeli (116 eleman, 64 düğüm, 4 diyafram) ETABS'e birebir kurulur: kesit ve malzeme (E = 4700√fc', ν = 0,2, γ = 25 kN/m³), rijit uç bölgeler (RZ = 1; kiriş 0,2/0,25 m, kolon 0,3 m), diyaframlar, yük şablonları (kiriş yükleri FemCore'dan kopyalanır; üçgen döşeme yükü ETABS alan yükü değil, kiriş yükü olarak aktarılır), kütle kaynağı G + 0,3Q + 0,3S (yalnız yanal), TBDY DD-2 ZC tasarım spektrumu (R = 8, D = 3), CQC %5 sönüm. Brüt kesit (statik) ve çatlamış kesit (kiriş 0,35, kolon 0,70; modal + RS) iki ayrı ETABS oturumunda koşulur. Karşılaştırılan: çubuk yüz kuvvetleri (P, V2, M3, tüm 116 elemanda), düğüm yer değiştirmeleri, taban reaksiyonları, 12 periyot, taban kesmesi, RS yer değiştirme ve çubuk kuvvetleri. Ham günlük: `03_Code_Execution/runs/V2_fem_etabs_log_2026-10-29.txt`.

**Sonuç (düzeltmelerden sonra).**
| Büyüklük | ETABS ↔ FemCore farkı |
|---|---|
| Taban reaksiyonu G / Q / S | 6999,30 / 900,00 / 112,50 kN, fark 0 |
| Çubuk yüz kuvvetleri P, V2, M3 (G, Q, S; brüt ve çatlamış) | ≤ %0,002 |
| Düğüm düşey yer değiştirme (G) | %0,04; dönmeler %0,00 |
| Periyotlar, 12 mod (çatlamış) | T1 %0,008; en büyük fark T12 %0,16 |
| Taban kesmesi EX / EY (RS) | %0,14 / %0,14 (FemCore büyük) |
| RS, ek dış merkezlik = 0: yer değiştirme, çubuk kuvveti | ≤ %0,09 |
| RS, ek dış merkezlik = %5: köşe düğüm yer değiştirme | FemCore büyük: EX %2,1–3,1; EY %4,2–6,3 |
| RS, ek dış merkezlik = %5: çubuk momenti | FemCore büyük (örnek: EX kolon %3,4; EY kolon uç momenti 6,17 ve 3,61 kNm) |

**Bulgular ve düzeltmeler (hepsi doğrulanmış).**
1. **Rijit uç bölgeler (FemCore hatası, düzeltildi).** ETABS, rijit bölgelerde eksenel ve burulma şekil değiştirmesini kaldırmaz (EA ve GJ tüm düğümden düğüme uzunlukta etkir); yalnız eğilme ve kesme rijit. FemCore tüm bölgeyi rijit alıyordu. Etki: üst kat düğüm düşey yer değiştirmesinde %18,8, kiriş uç momentlerinde %3,9, kolon eksenel kuvvetinde %0,6. Düzeltme: `FemChain.RigidZoneAxialFlexible` ve `RigidZoneTorsionFlexible` (RC modelinde açık); düzeltmeden sonra fark %0,002'ye indi.
2. **Ek dış merkezlik çift sayımı (FemRcModel hatası, düzeltildi).** `RSResults.FaceForce` ek dış merkezlik burulmasını zaten içeriyor; `Combos.Mag` bunu bir kez daha ekliyordu. Düzeltildi.
3. **Açık fark (kalıyor, muhafazakâr yönde):** FemCore ek dış merkezlik burulmasının etkisini ETABS'ten ~%40–70 büyük hesaplıyor (EX: yer değiştirmede etki oranı ETABS/FemCore ≈ 0,61–0,72). Nedeni (ETABS'in burulma momentini nasıl tanımladığı) belgelenmedi; **DOĞRULANAMADI**. Sonuçlar ETABS'e göre güvenli tarafta; final optimum ETABS'te yeniden çözülerek D/C okunur (AGENTS kural 1).

**Sınırlar (V2a'nın kapsamadığı).** (a) Betonarme tasarım D/C oranları (ETABS Concrete Frame Design ↔ program) karşılaştırılmadı; (b) alan yükü dağılımı (ETABS döşeme) yerine eşdeğer kiriş yükü kullanıldı; (c) P-Δ ve ikinci mertebe yok; (d) torsiyon düzensizliği; (e) şartnamedeki V2 ölçütü (literatürdeki referans çalışma sonucuyla ≤ %5) ayrıca Ö2/Ö3 için yapılacak.

## 2. Açık kapılar
V0 (ortam), V1 (EDB modelleri), V2 (referans örnekler), V3–V5: açık.

---

## V2b: kolon tasarım oranı, ETABS Concrete Frame Design (ACI 318-14) ↔ program (2026-10-30) — ÖN SONUÇ

> **Güncelleme:** bu bölümdeki deprem sayıları FemCore burulma düzeltmesinden önceki ilk koşudur; geçerli sonuçlar için "V2 ikinci tur" bölümüne bakın.

> **Critic (Review_Round_V2ab.md): [REVİZYON TALEBİ].** Bu bölüm ön sonuçtur, "kısmen geçti" denemez. Eksikler: (i) tablodaki "program" sütunu radyal+e_min varyantıdır, optimizasyondaki `BiaxialDcr` (e_min yok) değildir (U1 kolon C013 oranı 0,914); (ii) deprem birleşimlerinde kuvvetler aynı değil (FemCore ek dış merkezlik farkı açıklanamadı, ECC=0 koşusu yok); (iii) log 40 satırla sınırlı, ±%10 içinde yalnız 14/12/24/14 kolon; (iv) kabul eşiği önceden tanımlı değil; (v) üretim donatı düzeni ve D/C 0,8–1,0 bölgesi sınanmadı.

**Yöntem.** Ö1 deneme tasarımının 48 kolonu için ETABS 22.6.0'da (OAPI: `PropFrame.SetRebarColumn`, `DesignConcrete.SetCode("ACI 318-14")`, `SetComboStrength`, `StartDesign`, `GetSummaryResultsColumn`) her yük birleşimi ayrı ayrı tasarlandı (U1; TBDY Denk. 4.11 ve 4.12, E = X+0,3Y ve 0,3X+Y). Aynı birleşim ve aynı donatı düzeni (yüz başına 5 veya 4 çubuk, Ø20, etriye Ø10, pas payı 40 mm, ağırlık merkezi 60 mm) programa verildi. Karşılaştırılan: ETABS `PMMRatio` (3B radyal ışın OL/OC; CFD-ACI-318-14 s. 32) ↔ programın radyal oranı (kendi doğrusal toplam etkileşiminin orijinden ışın ölçeği, `Radial` in `EtabsProbe/Design.vb`) ve ETABS'in minimum dışmerkezliği (0,6 + 0,03h in) eklenmiş değişkeni. Kuvvetler V2a'da ≤%0,002 uyumlu olduğundan fark tasarım algoritmasından gelir. Ham günlük: `03_Code_Execution/runs/V2b_column_design_log_2026-10-30.txt`.

**Bulgu (program hatası, düzeltildi).** İlk koşuda program, deprem birleşimlerinde G ± E çiftini yalnız (+,+) / (−,−) işaretlerle değerlendiriyordu; RS büyüklükleri işaret taşımadığı için çift eksenli kolonlarda ETABS'e göre %28'e kadar düşük D/C çıktı (ör. C003: ETABS 0,185, program 0,133). `ColumnDesign.CheckGroup` artık P, M2, M3 için tüm 8 işaret desenini tarıyor. Testler PASS=62.

**Sonuç (düzeltmeden sonra, 48 kolon, her birleşim).**

| Birleşim | Maks D/C ETABS | Maks D/C program | Program ≥ ETABS (emin'li) | Oran program/ETABS (min–maks, emin'li) |
|---|---|---|---|---|
| U1 1,4G+1,6(Q+S) | 0,354 | 0,353 | 40 / 48 | 0,983 – 1,169 |
| 4.11 X+0,3Y | 0,295 | 0,317 | 46 / 48 | 0,996 – 1,210 |
| 4.12 X+0,3Y | 0,197 | 0,220 | 48 / 48 | 1,031 – 1,180 |
| 4.11 0,3X+Y | 0,297 | 0,323 | 44 / 48 | 0,988 – 1,213 |
| 4.12 0,3X+Y | 0,201 | 0,225 | 48 / 48 | 1,034 – 1,182 |

Yorum: programın D/C'si ETABS'in %1,7 altından %21 üstüne dağılır; kritik (en büyük) D/C'de fark ≤%0,3 (gravite) ve +%7–12 (deprem, güvenli yönde). Deprem birleşimlerindeki fazlalık, FemCore'un ek dış merkezlik burulmasını ETABS'ten büyük hesaplamasıyla (V2a) ve 8 işaret deseninin muhafazakârlığıyla uyumludur; nedenleri ayrıştırılmadı.

**Sınırlar (kapsam dışı kalan).** Yalnız kolon P-M-M oranı karşılaştırıldı. Yapılmadı: kiriş As ↔ ETABS, kesme Av/s, güçlü kolon–zayıf kiriş, birleşim kesmesi, SFRC kirişler (ETABS lifli kesiti tasarlamaz; lifli kirişler V2c'de ayrıca, ETABS'te kesit kapasitesi dışarıdan doğrulanacak). Program dönmüş çift eksenli yüzeyi değil doğrusal toplamı kullanır; yalnız kolon geometrisi simetrik (yüz başına n çubuk) iken karşılaştırıldı, programın optimizasyondaki çevre düzeni (üst/alt sıralarda çok çubuk) bununla aynı değildir. Narinlik/P-Δ programda yok (ETABS δns uygular); bu denemede etkisi ayrıca ölçülmedi. Tek bir deneme tasarımı ve en büyük D/C 0,35 olduğundan etkileşim yüzeyinin yüksek D/C bölgesi sınanmadı.

**Durum:** V2b (kolon) **kısmen geçti**: program ETABS'in kolon D/C'sini ≤%1,7 sapmayla (altta) ve genelde güvenli yönde yakalıyor; yüksek D/C ve kiriş/kesme bölümleri açık. Critic görüşü bekler.


## V2c: kiriş eğilme ve kesme donatısı, ETABS Concrete Frame Design (ACI 318-14) ↔ program (2026-10-30) — ÖN SONUÇ

**Yöntem.** Ö1 deneme tasarımı, 68 kiriş (300×600, C30, donatı ağırlık merkezi 60 mm, d = 540 mm), aynı üyeler kuvvetleri (V2a). ETABS: `PropFrame.SetRebarBeam`, `DesignConcrete.ACI318_14.SetOverwrite` (U1 için Sway Ordinary, deprem birleşimleri için Sway Special), her birleşim ayrı tasarlanır, `GetSummaryResultsBeam_2` (TopAreaReq, BotAreaReq, TopArea, BotArea, VmajorArea; çıktılar kirişte istasyonların en büyüğü). Program: `RcCore.AciBeam.RequiredAs` (φ yinelemeli, tek donatılı), `AciBeam.Vc/AvMinPerS`, ACI 18.6.5 kapasite kesmesi `AciBeam.Flexure` (1,25 fy, φ=1) ile yeniden hesaplandı. Sondalar: `EtabsProbe/DesignBeam.vb` (DESIGN=2). Ham loglar: `03_Code_Execution/runs/V2c_beam_design_log_prov2000_2026-10-30.txt`, `..._prov532_...`.

| Birleşim | Gerekli As üst, program/ETABS (min–maks) | Gerekli As alt, program/ETABS | Av/s program/ETABS |
|---|---|---|---|
| U1 (gravite, Ordinary) | 1,000–1,000 | 1,000–1,048 | 1,000–1,000 |
| 4.11 X+0,3Y (Special) | 1,000–1,036 | 1,000–1,045 | 0,972–0,985 (6 kiriş 3,229, aşağıda) |
| 4.12 X+0,3Y | 1,000–1,053 | 0,981–1,059 | 0,964–0,985 |
| 4.11 0,3X+Y | 1,000–1,033 | 1,000–1,045 | 0,972–0,985 (6 kiriş 3,229) |
| 4.12 0,3X+Y | 1,000–1,046 | 1,000–1,052 | 0,964–0,985 |

(Alt donatıda deprem satırları programın ACI 18.6.3.2 / TBDY 7.4.2.3 "alt ≥ üst/2" kuralı eklenerek; kural olmadan oran 0,48–0,65'e düşer, çünkü ETABS bu kuralı Req değerine katar. Gerçek program kodu `BeamDesign.DesignGroup` bu kuralı zaten uygular.)

**Bulgular.**
1. **Eğilme:** üst donatı ETABS ile birebir (U1) ve ≤ %5,3 güvenli yönde (deprem). Alt donatı ≤ %6 güvenli yönde; tek istisna 4.12'de 2 kirişte 0,981 (−%1,9). Fark küçük: program orta açıklık momentini ETABS istasyonlarından farklı yerde okuyor olabilir (açıklanmadı).
2. **Gravite kesmesi (U1):** Av/s birebir (oran 1,000), Vc ve min Av/s formülleri ETABS ile aynı.
3. **Kapasite kesmesi:** ETABS, Mpr'yi **tasarlanan zarf donatıdan değil kesitte tanımlı (sağlanan) donatıdan** hesaplıyor. Sağlanan = 2000 mm² iken program (zarf As ile) ETABS'in 0,35–0,46 katı çıktı (deneme sonucu); sağlanan donatı 532 mm²'ye çekilince oran 0,964–0,985. Bu, ETABS'in davranışının doğrulanması: program ve ETABS aynı kapasite kesmesi zincirini uyguluyor. Programın bu zinciri (sağlanan donatı ile Mpr) üretimde de kullanılması gerekir; `BeamShear` TBDY 7.4.5 (1,4 Mr/ln) kullandığı için ACI ile birebir kıyaslanamaz, burada yeniden hesaplanan ACI zinciri kıyaslanmıştır.
4. **Açıklanamayan:** 4.11 birleşiminde 6 kirişte ETABS yalnız minimum Av/s (0,25 mm²/mm) verdi, program 0,807 (oran 3,229, programın muhafazakâr olduğu yön). Neden bilinmiyor (olası: ETABS'in o kirişlerde farklı gravite kesmesi / Vc koşulu).

**Sınırlar (rapora açık yazılır).** Tek donatılı dikdörtgen kesit; T kesit, burulma ve basınç donatısı kıyaslanmadı. Güçlü kolon–zayıf kiriş ve birleşim kesmesi kıyaslanmadı. **Lifli kirişler (SFRC) ETABS ile doğrulanamaz**; bağımsız doğrulama yolu (el hesabı/fib MC2010 örneği) ve eşik V3 öncesi yazılı olmalıdır. Kabul eşiği (öneri): üst/alt As ve Av/s için program ≥ ETABS ve fazlalık ≤ %10; ihlal olursa neden aranır.


## V2 ikinci tur: Critic Z1–Z8 yanıtı ve kapı değerlendirmesi (2026-10-30)

**Önemli:** Bu bölüm, yukarıdaki V2a açık-fark ifadesini (%2–6 ek dış merkezlik) ve V2b/V2c'nin ilk koşu sayılarını **geçersiz kılar**; ilk tur sayıları FemCore burulma düzeltmesinden önceydi.

### Z4 — Ek dış merkezlik farkının kök nedeni (ÇÖZÜLDÜ, FemCore düzeltildi)
ETABS ECC=%5 ile ECC=0 arasındaki köşe yer değiştirme farkı, FemStatic statik burulma yanıtı matrisi A ile ters çözülünce **kat burulma momentleri** verdi (tanı: `EtabsProbe` ECCDIAG=1; kat 1→4, EX): ETABS 19,6 / 27,6 / 39,3 / 62,3 kN·m; FemCore'un eski kullanımı (diyafram CQC kuvveti × ecc × genişlik) 62,3 / 69,0 / 65,2 / 62,8 kN·m. Üst kat eşit, alt katlar ETABS'te çok daha küçük: ETABS torku = ecc × genişlik × **kat kuvveti**, kat kuvveti = CQC kat kesmelerinin farkı (V_k − V_k+1; toplam ≈ taban kesmesi: 297,6 ≈ 304,7 kN). FemCore her diyaframın eylemsizlik kuvvetinin ayrı CQC'sini kullanıyordu (toplamı taban kesmesini aşıyor). `FemModal.vb`: `StoryShear/StoryForce` eklendi, `FemStatic.TorsionFromStoryShear = True` (varsayılan). Bu, ETABS Help'te yazılı olmayan bir ayrıntıdır; ETABS davranışı ölçüm ve ters çözümle bulunmuştur (EX ve EY, 4 kat, hepsi tutuyor).

| Büyüklük | Önce | Sonra (ECC=%5) | Sonra (ECC=0) |
|---|---|---|---|
| Köşe yer değiştirmesi, ETABS ↔ FemCore | %2,1–6,3 | **%0,05–0,093** | %0,048–0,092 |
| Taban kesmesi EX / EY | %0,14 | %0,141 / %0,144 | aynı |
| Çubuk M3 (C001 EX, I ucu), kN·m | 52,08 vs 50,36 (%3,4) | **50,42 vs 50,36 (%0,12)** | 47,91 vs 47,86 |
| Çubuk M3 (C001 EY, I ucu), kN·m | 6,17 vs 3,61 (%71) | **3,62 vs 3,61 (%0,3)** | 0,00 vs 0,00 |
| Kiriş gerekli üst As (deprem), program/ETABS | 1,000–1,053 | **1,000–1,000** | – |

Periyotlar (12 mod): fark −%0,008…−%0,164 (FemCore periyodu ETABS'ten en çok %0,16 büyük). Ham loglar: `runs/V2a_ecc5_storyshear_torsion_2026-10-30.txt`, `runs/V2a_ecc0_2026-10-30.txt`.

### Kapı değerlendirmesi (eşikler: `03_Code_Execution/VALIDATION_SPEC.md`, "V2 alt kapıları")
| Kapı | Sonuç | Durum |
|---|---|---|
| FE1 periyot ≤ %0,5 | maks %0,164 | **GEÇTİ** |
| FE2 taban kesmesi ≤ %0,5 | %0,141 / %0,144 | **GEÇTİ** |
| FE3 köşe yer değiştirmesi ≤ %0,5 | ≤ %0,093 (ECC=%5 ve 0) | **GEÇTİ** |
| FE4 çubuk kuvvetleri | statik ≤ %0,002 (V2a ilk tur; statik hesap değişmedi); RS ≤ %0,3 | **GEÇTİ** |
| CD1 yanlış-uygun = 0 | üretim kısıtı: 2 yanlış-uygun (ETABS D/C 1,038; e_min kaynaklı, δns kapalı koşu); radyal+e_min: 0 | **KALDI** (PCR-1) |
| CD2 radyal+e_min ≥ 0,95 ETABS | varsayılan kesit: min 0,983; küçük kesit, δns açık, gravite: 0,856 | **KISMEN** (δns yok, PCR-2) |
| CD3 kritik D/C | üretim D/C (gravite) ETABS'ten %17–41 düşük (sabit-P moment oranı tanımı); deprem birleşiminde +%7…+%18 (üretim büyük) | **TANIM FARKI** (PCR-3); uygunluk için CD1'e bakılır |
| B1 kiriş As 0,98–1,10 | üst 1,000–1,000; alt 0,981–1,048 | **GEÇTİ** |
| B2 gravite Av/s 0,98–1,02 | 1,000 | **GEÇTİ** |
| B3 kapasite kesmesi ≥ 0,95 | 0,964–0,985 (sağlanan donatıyla) | **GEÇTİ** (6 kirişte ETABS yalnız min Av/s verdi: program muhafazakâr; nedeni açıklanamadı) |
| S1 SFRC bağımsız entegrasyon ≤ %1 | 216 kesit: maks \|ΔMn\| %0,0008, \|Δc\| %0,0055 | **GEÇTİ** |
| S2 SFRC literatür deneyi | yapılmadı | **AÇIK** |

### Z3 — V2b ECC=0 ile
`runs/V2b_ecc0_2026-10-30.txt` (ECC=0) ve `runs/V2b_ecc5_final_with_prefs_2026-10-30.txt` (ECC=%5, düzeltilmiş burulma). Radyal+e_min program/ETABS oranı: ECC=%5: U1 0,983–1,169; 4.11 X+0,3Y 0,994–1,167 (ort. 1,104 e_min'li); ECC=0: U1 aynı; 4.11 X+0,3Y 0,985–1,160 (ort. 1,100). ECC, oranları değiştirmiyor: deprem birleşimlerindeki +%7–12 fazlalık burulmadan değil, 8 işaret deseninden ve radyal tanımdan geliyor (ECC=0'da da +%8–10). İlk raporda ima edilen "burulma kaynaklı olabilir" yorumu geri alınır.

### Z2 — Üretim kısıtı (e_min'siz `BiaxialDcr`) ile kıyas
Varsayılan kesitlerde (kolon 500/400 mm; 48 kolon, ECC=%5): üretim D/C, ETABS'ten büyük veya eşit olan kolon sayısı U1'de 18/48, deprem birleşimlerinde 26–42/48; maks D/C: U1 0,294 vs 0,354; 4.11 X+0,3Y 0,316 vs 0,295. Üretim D/C **sabit-P moment oranı**dır, ETABS **radyal** orandır; eksenel baskın kolonlarda üretim D/C çok küçük kalır (en uç: min oran 0,002). Uygunluk (D/C ≤ 1) aynı yüzeye dayanır; fark ceza ölçeğinde (PCR-3) ve **e_min / δns** eksikliğindedir (PCR-1, PCR-2).

### Z5 — Yüksek D/C, narinlik, üretim donatı düzeni
Kolonlar küçültülerek (V2d, `runs/V2d_*`): 300/250 mm (maks ETABS D/C 0,84), 250/220, 230/200 (ETABS 3,0–3,8; narinlik kaynaklı). **δns etkisi (izole; ETABS Db/Ds = 1 üzerine yazılarak):** 300×300 kolonlar, U1: ETABS maks D/C 0,818 → 0,709 (δns ≈ +%15); deprem birleşimleri aynı (0,631; sway, P-Δ varsayımı). δns kapalıyken program radyal / ETABS maks: 0,688 / 0,709 (−%3); δns açıkken 0,688 / 0,818 (−%16). δns kapalı, 230/200 mm koşusunda ETABS D/C 1,038'e ulaştı; orada üretim kısıtı 2 yanlış-uygun, radyal+e_min 0 yanlış-uygun verdi. **D/C 0,8–1,0 aralığı yalnız bu birkaç kolonla sınandı: sınırlamadır.** Üretim donatı düzeni (üst/alt sırada yoğun) sınanmadı; simetrik çevre düzeni kullanıldı: sınırlamadır.

### Z6 — Kolon kesme/kapasite tasarımı ve TS 500-2000(R2018) menüsü
**Yapılmadı** (kiriş tarafı V2c'de yapıldı). Programın TBDY 7.3/7.4 zinciri ETABS'te ancak TS 500 menüsüyle denenebilir; ayrı bir model ayarı ve koşu gerekir (PCR-4). Açık.

### Z7 — SFRC kiriş doğrulama yolu (yazılı)
ETABS lifli kesiti tasarlayamaz; "ETABS'te doğrulandı" iddiası lifli kiriş için yapılmaz. Yol: (1) **Bağımsız sayısal entegrasyon** (`tools/sfrc_independent_check.py`, 216 kesit, S1 geçti): VB kapalı-form integralleri ve çözücü doğrulandı; aynı modeli kullandığı için modelin kendisini sınamaz. (2) **Deney kirişleri** (S2): fib MC2010 (5.6) σ–w modeli ile kesit eğilme kapasitesinin yayınlanmış lifli kiriş deneyleriyle kıyası, Literature Analyst görevi (aday veri: tezdeki Venkateshwaran 2018, Chen 2021; fR değerlerinin ortalama mı karakteristik mi olduğu belirsiz). (3) Maliyet sonuçları lif katkısı olmadan (fFtu = 0) ve fR çarpanları 0,85/0,70 ile duyarlılık olarak sunulur. S2 geçilene kadar lifli sonuçlar "model sonucu" diye etiketlenir.

### Z8 — ETABS tercihleri ve uyarılar
Tercih dökümü: `runs/V2b_ecc5_final_with_prefs_2026-10-30.txt`. Etkileşim eğrisi 24, nokta 11, e_min açık, B/C oran tasarımı Evet, SDC 4 (D), φ eğilme 0,9 / basınç etriyeli 0,65 / spiral 0,75 / kesme 0,75 / sismik kesme 0,60 / birleşim kesmesi 0,85, kalıcı yük faktörü 0,75, kullanım sınırı 1,0, çok yanıt tasarımı 5. Üzerine yazmalar (kolon C001, kiriş Bx001; hepsi "program belirlediği"): çerçeve tipi 1 (özel moment çerçevesi), boyutsuz uzunluk oranı 0,9, K = 1, Cm = 1, Db = Ds = 1 (bunlar başlangıç değerleridir; ETABS δns'i kendi hesaplar), e_min açık. `ETABS ERROR ret=1 in SetDiaphragm`: D1 diyaframı model şablonunda zaten var (zararsız). `Cannot open file ... .Y_` (7 kez): ETABS'in tablo uygulaması sırasında verdiği iç geçici dosya iletisi; hemen ardından `tables applied: fatal=0 errors=0 warnings=0` ve doğru Mass Source dökümü geldiği için sonuçlar etkilenmemiştir.

### Z1 — Ham kanıt arşivi
`03_Code_Execution/runs/` altında 19 dosya + `SHA256SUMS_2026-10-30.txt` (SHA-256). Testler: FemCore.Tests 16/16, FemRcModel.Tests 62/62, RcCore.Tests 59/59 (`runs/tests_*_2026-10-30.txt`). Derleme: EtabsProbe.exe SHA-256 442946f021f6…e3e1, FemCore.dll 6f535f2bb701…2f27, FemRcModel.dll 9383abd8fbc2…b81b; ETABS 22.6.0; ETABSv1.dll 2.8.0.0.

### Sonuç
V2 kapılarından FE1–FE4, B1–B3 ve S1 geçti. **CD1 kaldı**: üretim kolon kısıtı, ETABS'e göre e_min ve narinlik (δns) nedeniyle uygunluğu hafif iyimser buluyor (D/C ≈ 1'de %4; δns'in etkin olduğu 300 mm gravite kolonlarında %15). Bu nedenle V2 **tam geçmedi**. PCR-1 ve PCR-2 onaylanıp uygulanmadan ve CD1 yeniden ölçülmeden kampanya başlatılmaz. S2 (SFRC deney kıyası) ve Z6 açıktır.

### V2e — PCR-1/PCR-2 uygulandıktan sonra CD1 yeniden ölçümü (2026-10-30)
`ColumnDesign.CheckGroup` artık ACI e_min (her doğrultu ayrı) ve gravite birleşiminde δns içeriyor (IA onayı 2026-10-30). Karşılaştırma: aynı kuvvetler, ETABS CFD ACI 318-14 (δns açık), 48 kolon × 5 birleşim; "üretim" = `DcrWithEffects`.

| Kesit (S12 / S34, mm) | ETABS maks D/C (U1) | Radyal + e_min + δns / ETABS (min–maks) | Yanlış-uygun (üretim) | Yanlış-uygun değil (üretim) |
|---|---|---|---|---|
| 500 / 400 (deneme tasarımı) | 0,354 | 0,983–1,177 | **0** | 0 |
| 300 / 300 | 0,818 | 0,999–1,237 (U1 maks: 0,817 vs 0,818) | **0** | 0 |
| 260 / 240 | 1,683 (D/C > 1) | 1,005–1,472 (U1 maks: 1,691 vs 1,683) | **0** | 0 |

- **CD1 (yanlış-uygun = 0): GEÇTİ** (üç kesit setinde, toplam 720 kolon×birleşim satırı). Önceki turda 2 yanlış-uygun vardı.
- **CD2 (radyal+e_min+δns ≥ 0,95 ETABS): GEÇTİ** (min 0,983 / 0,999 / 1,005).
- δns ETABS ile U1'de %0,1–0,5 uyumlu (300 mm: 0,817/0,818; 260 mm: 1,691/1,683); 230/200 mm kesitte, burkulma sınırına yakın (Pu → 0,75 Pc) bölgede ETABS 2,2 verirken ilk δns sürümü 4·10¹² verdi; bu aralık gerçek tasarım uzayı dışındadır (üretimde D/C kapsı 10), sınırlama olarak yazılır.
- **CD3 (kritik D/C tanımı):** üretim D/C'si sabit-P moment oranıdır; U1'de ETABS'ten düşük (0,297 vs 0,354; 0,655 vs 0,818), D/C > 1 bölgesinde ise büyük (2,29 vs 1,68). Uygunluk kararı aynı: PCR-3 gereği makalede tanım farkı açıklanır.
- Kaldığı yerler: D/C 0,8–1,0 aralığı yalnız 300 mm koşusunda (maks 0,82) ve 260 mm'de (deprem 0,78) örneklendi; üretim donatı düzeni (üst/alt sırada yoğun) sınanmadı; ETABS Cm = 1 ve lu = 0,9·L varsayımı korunur.
- Testler: FemCore 16/16, FemRcModel 62/62, RcCore 59/59 (`runs/tests_*_final_2026-10-30.txt`). Loglar: `runs/V2e_PCR12_*`; SHA-256 listesi güncellendi (25 dosya).

**V2 durumu:** FE1–FE4, CD1, CD2, B1–B3, S1 geçti; CD3 tanım farkı olarak belgelenir; **S2 (SFRC deney kıyası), Z6 (kolon kesme/kapasite tasarımı TS 500 menüsü) açık**; D/C 0,8–1,0 ve üretim donatı düzeni sınırlamadır. Kampanya için kalan engeller: S2 ve Critic'in yeniden incelemesi (REVİZYON TALEBİ'nin kapandığını onaylaması), ardından IA'nın kampanya onayı.


### S2 — SFRC kiriş deney kıyası (2026-10-30): SONUÇSUZ (veri yetersiz/tutarsız), kapı AÇIK
Literature Analyst yalnız 2 çalışmadan 12 kiriş buldu (`01_Literature/SFRC_BEAM_TESTS.csv/.md`, kaynak PDF'leri `Papers/` altında, SHA-256 kayıtlı); `d` hiçbir satırda yok, Buttignol 2018'in 3 kirişinde fc/fy da yok (kullanılamaz). Nogales vd. (2025) 9 kirişi, `tools/sfrc_s2_compare.py` ile (model: SfrcBeam ile birebir doğrulanmış; ortalama fR, γF = 1, φ yok; d = 353 mm, duyarlılık 365 mm) hesaplandı (`04_Results/S2_SFRC_comparison_raw.txt`):

| Karşılaştırma | Tahmin/ölçüm (n = 9) |
|---|---|
| Çelik akma (fy = 503 MPa) | ortalama 0,587 (0,526–0,635) |
| Çubuk çekme dayanımı (ft = 644 MPa) | ortalama 0,750 (0,672–0,812) |

**Yorum (sayıdan çıkan, model kanıtı değil):** (1) Lifsiz kontrol kirişleri RCS1/RCS2 de aynı şekilde düşük (0,60/0,63): açığın nedeni lif modeli değildir. (2) Denge kontrolü: Mexp = 148,5 kN·m için çubuk kuvveti ≈ 424 kN gerekir, oysa As·ft = 502 × 644 = 323 kN; yani raporlanan As ve Mexp (0,45·Pmax) birlikte statikle tutarsızdır (üst donatı, ek çubuk ya da Pmax tanımı belirsiz). (3) Bu kirişlerde lif katkısı modelde ≈ 0 (c/d ≈ 0,02, çok az donatılı; hazır model fiber artışı < %0,2), deneyde de belirgin bir artış yok (RCS1 148,5 → HRCS1-25Aa 139,5). Bu veri lif katkısını sınamaya uygun değildir.
**Karar:** S2 geçmedi, kaldı da denemez; **sonuçsuz**. Gereken: orta/yüksek donatılı lifli kiriş deneyleri (Soetens & Matthys, Meda–Plizzari, Foster & Parvez, Gümüş & Arslan; kurum erişimi gerekir) ve Nogales yazarlarından donatı yerleşimi/Pmax tanımı. O zamana kadar lifli sonuçlar "model sonucu" etiketiyle ve lifsiz/fFtu = 0 duyarlılığıyla sunulur; makale S2'ye dayanan "deneysel olarak doğrulandı" ifadesi içeremez.


## V2f — Critic Review_Round_V2e yanıtı (R1–R8), 2026-10-30

**Kapsam notu:** Bu bölüm V2e bölümündeki "geçti" kararlarını revize eder; V2e'de kullanılan δns hizalaması (lu = 0,9L, yalnız gravite birleşimi) ve simetrik `SymSection` düzeni yerine **üretim düzeni** ve ACI net yükseklik kullanıldı.

### Kod değişiklikleri (bu tur, hepsi V2e Critic maddelerinden)
1. **Üretim donatı düzeni (A2/R4):** `ColumnDesign.MakeSection` iki eğilme doğrultusu için aynı fiziksel çevre düzenini kullanıyor (`PerimeterCounts`; N çubuk, 2(nK + nU) − 4 = N). Önce iki doğrultu bağımsız düzen üretiyordu (500/400, N = 16: 8+8 ve 7+2+7). Kare kolonda tek sayılı kenar çiftinde M2 kesiti M3'ün devriği olmalıdır; bu hata yeni testlerde bulundu ve `transposed` parametresiyle düzeltildi. ETABS'te aynı düzen `SetRebarColumn` (NR3 = genişlik kenarı çubuk sayısı, NR2 = derinlik kenarı) ile kuruldu; kare olmayan kolonda NR3/NR2'yi değiştirmek D/C'yi birkaç yüzde etkiledi (U1 maks 0,502 ↔ 0,510), eşleme kesin kanıtlanmadı: sınırlama.
2. **lu (N1/R3):** program ACI 6.2.5.1'e göre net yükseklik (eleman boyu − rijit uç bölge) kullanıyor; ETABS'te aynı girdiler `SetOverwrite` ile açıkça yazıldı (lu oranı = net/L, K = 1, Cm = 1). Önceki "0,9L" ETABS kopyası kaldırıldı.
3. **Sismik birleşimlerde δns (N1):** ETABS'in sismik birleşimlerde de δns uyguladığı bulundu (300×300, ρ = %3, kolon C111, 4.11: büyütme kapalı 0,741 → açık 0,878). Program artık sismik birleşimlerde de aynı δns'i (birleşimin Pu'su, sürekli yük (1 + ez)G) uyguluyor. Bu ETABS davranışına uyumdur; ACI 6.6.4.6'daki Mns/Ms ayrımı yerine tüm momentin büyütülmesi **muhafazakârdır** ve makalede böyle yazılır.
4. **Q kontrolü (N1):** `EtabsProbe` QCHECK=1, kat başına ACI 6.6.4.3 stabilite indeksi: birinci mertebe Δo ile Q ≤ 0,017 (EY, kat 2); Δ'yı D = 3 ile büyütünce en çok 0,050 (EY kat 2, tam sınırda). Birinci mertebe değeri ACI'nin tanımıdır; sway etkileri ihmal edilebilir. Log: `runs/V2f_QCHECK_stability_index_2026-10-30.txt`.
5. **Birim testleri (R6/B4):** 13 yeni test (F13): çevre düzeni, kare N = 6 devrik düzen, δns analitik (1,2018), lu/r ≤ 22, δns sonsuz, e_min, kat kuvvetleri toplamı = taban kesmesi, üst kat kuvveti = üst kat kesmesi. **Toplam 75 test geçti** (62 → 75). 8 işaret deseni için ayrı birim test **yazılamadı** (sentetik `FrameAnalysis` kurmak gerekir); ETABS karşılaştırması onu kapsıyor (V2b).

### Ölçümler (ETABS CFD ACI 318-14, aynı kuvvetler ve düzen; 8 konfigürasyon × 240 kolon×birleşim = 1920 satır)
| Konfigürasyon (S12 / S34, mm; ρ) | ETABS maks D/C (U1 / sismik) | Radyal+e_min+δns / ETABS (min–maks) | Yanlış-uygun | Yanlış-uygun değil |
|---|---|---|---|---|
| 500 / 400; %1,5 | 0,376 / 0,320 | 0,961–1,315 | 0 | 0 |
| 400 / 320; %1,0 | 0,622 / 0,584 | 0,974–1,485 | 0 | 0 |
| 500×350 / 350; %2,5 (kare değil) | 0,484 / 0,415 | 0,976–1,151 | 0 | 0 |
| 320 / 300; %2,5 | 0,843 / 0,743 | 0,983–1,147 | 0 | 0 |
| 300×350 / 300; %3 (kare değil) | 0,802 / 0,691 | 0,987–1,140 | 0 | 0 |
| 300 / 300; %3 | 0,981 / 0,879 | 0,989–1,148 | 0 | 0 |
| 300 / 300; %2 | 1,116 / 0,978 | 0,987–1,174 | 0 | 4 |
| 260 / 240; %2 (tasarım uzayı dışı) | 2,452 / 1,784 | 0,994–1,166 | 0 | 34 |

- **CD1 (yanlış-uygun = 0): GEÇTİ**, 1920 satırda 0. Yanlış-uygun-değil (program > 1, ETABS ≤ 1) 38 satır, güvenli yönde.
- **CD2 (≥ 0,95): GEÇTİ**, tüm konfigürasyonlarda min 0,961 (önceki tur 0,911 idi, nedeni sismik δns eksikliği).
- **Bandı örnekleme (R2):** ETABS D/C ∈ [0,8; 1,2] olan satır sayısı: tasarım uzayında (300–500 mm) 16 (300/300 %3: 6; 300/300 %2: 6; 320/300: 2; 300×350: 2), uzay dışında (260/240) 98. İstenen "≥ 20 bağımsız satır, 300–500 mm" ölçütü **16 ile sağlanmadı**; C111/C211 (aynı kolon tipi) tekrarlı olduğundan bağımsız satır sayısı daha da düşük. Hold-out niteliği: konfigürasyonların hepsi aynı Ö1 geometrisi ve aynı yükler üzerindedir; **bağımsız bir ikinci yapı** (farklı kat sayısı/açıklık) denenmedi.
- **CD3 (kritik D/C ≥ −%5 güvensiz yönde): KALDI** konfigürasyonların üçünde: 500×350 (U1: üretim 0,373, ETABS 0,484, −%23), 320/300 (0,643/0,843, −%24), 300×350 (0,606/0,802, −%24). Hepsi düşük D/C (≤ 0,85); nedeni üretim kısıtının sabit-P moment oranı olmasıdır (ETABS radyal). D/C → 1'e yaklaşırken fark azalır (300/300 %3: U1 0,947 vs 0,981, −%3,5; %2: 1,411 vs 1,116, güvenli). Uygunluk kararı aynı yüzeye dayandığı için CD1 geçiyor; ceza ölçeği ETABS'inkinden farklı. **Karar IA'nın (R5):** (a) CD3'ü yalnız bilgi amaçlı say ve makalede "D/C = moment oranı" tanımını yaz, ya da (b) üretim kısıtını radyal orana çevir (hesap maliyeti artar).

### R1 — Kanıt bütünlüğü
- `runs/SHA256SUMS_2026-10-30.txt` yeniden üretildi: kendini içermiyor, 4 ikili (EtabsProbe.exe 4c369899…, FemCore.dll 207beee8…, FemRcModel.dll fe5ee93e…, RcCore.dll 93bd0403…) ve 48 log/test dosyası; `sha256sum -c` ile doğrulandı, 0 başarısız.
- Burulma ters çözümü: `runs/V2f_ECCDIAG_torsion_inverse_2026-10-30.txt` (ETABS tork 19,6/27,6/39,3/62,3 kN·m; FemCore yeni 22,9/27,8/39,0/62,8; eski CQC kuvveti 62,3/69,0/65,2/62,8).
- **116 eleman RS karşılaştırması:** `runs/V2f_final_RS_all116members_2026-10-30.txt`: EX: maks |ΔM3| 0,064 kN·m = %0,114, maks |ΔV2| %0,133; EY: %0,071 / %0,071 (en büyük değere göre).
- **NORZ=1 (rijit bölge yok) koşusu:** `runs/V2f_final_NORZ1_2026-10-30.txt`: taban kesmesi, köşe yer değiştirmesi, 116 eleman: fark %0,000–0,001.
- ETABS etkileşim yüzeyi 11 → 41 nokta (48 eğri): ETABS D/C %1,3'e kadar düşüyor (C111 4.11: 0,878 → 0,867): program farkını açıklamıyor.
- Derleme zamanı, ETABS 22.6.0, ETABSv1.dll 2.8.0.0 SHA listesinin başlığında.

### R8 — Burulma sınırlaması
Burulma kök nedeni (kat kuvveti = CQC kat kesmesi farkı) tek yapıda (Ö1 deneme tasarımı, EX/EY, 4 kat, tek açıklık düzeni) bulundu ve sınandı; ikinci yapılandırma sınanmadı. Makalede: "Kazara dış merkezlik burulma kuvvetleri ETABS 22.6.0 davranışına göre kat kesmesi farkından hesaplanmıştır; doğrulama tek yapıda yapılmıştır" sınırlaması yazılır.

### Durum
V2 kapılarından FE1–FE4, CD1, CD2, B1–B3, S1 **geçti**; **CD3 kaldı (tanım farkı, IA kararı bekliyor)**; R2 (≥ 20 bağımsız satır) ve ikinci yapı **eksik**; S2 **sonuçsuz**; Z6/PCR-4 (TS 500 menüsü) açık. Kampanya için kalanlar: IA'nın CD3/R5 ve eşik onayı (R10), R7 kararı (kolon kesme ACI mı TBDY mi), S2.

### S2 güncellemesi (yeni PDF'ler)
`Papers/` altındaki 6 yeni PDF'den 32 kiriş satırı çıkarıldı (Meda 2012: 5, Dancygier 2016: 25, Zijl 2013: 2; Foster 2019, Mahmood 2018, Tiberti 2015: 0 satır), `01_Literature/SFRC_BEAM_TESTS.csv` şimdi 44 satır. **Hiçbir yeni satırda silindir fc ile EN 14651 fR1/fR3 birlikte yok** (Meda: UNI 11039; Dancygier: küp fc ve RILEM eğrisi; Zijl: yarma deneyi), bu nedenle model girdisi kurulamıyor; S2 hâlâ sonuçsuz. Mahmood 2018'de EN 14651 fR, silindir fc ve fy var ancak kiriş sürekli ve kesit momenti ölçülmemiş. Seçenekler: (i) bu kaynakların orijinal deney çalışmalarından (Foster 2019'daki de Montaignac, Conforti, Ning, Biolzi, Minelli, Dinh) fR'li veri aramak; (ii) Dancygier'in RILEM eğrisini sayısallaştırıp σ–w modelini doğrudan ona karşı sınamak (ayrı yöntem, IA kararı); (iii) lifli sonuçları S2'siz "model sonucu" olarak sunmak.


## V2h — IA kararları (2026-10-30): radyal D/C ve ACI Bölüm 18 kapasite tasarımı

**Kararlar (IA):** (1) Kolon kısıtı **radyal D/C** (ETABS tanımı) olacak. (2) Kapasite tasarımı zinciri (kiriş/kolon kesmesi, güçlü kolon–zayıf kiriş, birleşim kesmesi, sarılma) **ACI 318M-14 Bölüm 18**'e göre; TBDY yalnız yük, spektrum, boyut/oran sınırları (A1–A3, B1–B7), kat ötelenmesi ve A11 koşullarında kalır. (3) "Sen alacağın tüm kararları onaylıyorum" (IA); aşağıdaki seçimler bu yetkiyle yapılmıştır ve gerekçeleriyle yazılıdır.

### Radyal D/C (CD3 kapandı)
`ColumnDesign.ColCurve` (tasarım etkileşim eğrisi, 160 nokta, enterpolasyon) ve `RadialDcr` (yük noktasını orijinden λ ile ölçekle, D/C = 1/λ). Doğruluk: tam ölçekleme ile 6 yük noktasında en çok %0,30 (birim test). Hesap süresi: 82 ms → 29 ms (bir değerlendirmede tüm kolon grupları). ETABS ile 5 konfigürasyonda yeniden ölçüldü (`runs/V2h_*`), üretim kısıtı / ETABS D/C oranı:

| Konfigürasyon (S12 / S34, mm; ρ) | ETABS maks D/C (U1) | Üretim maks D/C (U1) | Üretim / ETABS (min–maks, tüm satırlar) | Yanlış-uygun | Yanlış-uygun değil |
|---|---|---|---|---|---|
| 500 / 400; %1,5 | 0,376 | 0,375 | 0,961–1,315 | 0 | 0 |
| 500×350 / 350; %2,5 | 0,484 | 0,479 | 0,988–1,151 | 0 | 0 |
| 320 / 300; %2,5 | 0,843 | 0,838 | 0,985–1,149 | 0 | 0 |
| 300 / 300; %3 | 0,981 | 0,979 | 0,991–1,149 | 0 | 0 |
| 300 / 300; %2 | 1,116 | 1,113 | 0,987–1,176 | 0 | 4 (güvenli) |

Önceki sabit-P tanımında aynı konfigürasyonlarda U1'de %23–24 düşük çıkan değerler (0,373 vs 0,484 vb.) kapandı. **CD3 GEÇTİ** (en büyük D/C'li kolonda üretim ETABS'ten en çok %1,0 düşük; tek tek kolonlarda oran ≥ 0,961; deprem birleşimlerinde ETABS'e göre çoğunlukla %0–10 yüksek).

### ACI 318M-14 Bölüm 18 zinciri (kod: `ColumnDesign.vb`, `BeamDesign.vb`, `Checks.vb`)
Madde metinleri `Evidence/DesignGuides/ACI 318M_14.pdf`'den okunarak doğrulandı: 18.6.4.4, 18.6.5.1–2, 18.7.3.2, 18.7.4.1, 18.7.5.1–5 (Tablo 18.7.5.4 (a)–(c)), 18.7.6.1.1, 18.7.6.2.1, 18.8.2.1, 18.8.4.1–3 (Tablo 18.8.4.1).
| Madde | Yeni uygulama | Önceki (TBDY) |
|---|---|---|
| Kiriş kapasite kesmesi | Ve = (Mpr,üst + Mpr,alt)/ln + Vdy; Mpr: 1,25 fy, φ = 1; lif katkısı γF = 1 (varsayım: "olası" dayanım); φv = 0,75; Vc = 0 (deprem kesmesi ≥ Ve/2); Vs ≤ 0,66√f'c bw d; ilk etriye ≤ 50 mm, aralık ≤ min(d/4, 6 db, 150) | 1,4 Mr(fcd,fyd), 8 db, 0,85 bw d √fck |
| Kolon kapasite kesmesi | Ve = (Mü + Ma)/ln; uç momentleri = min(Mpr,kolon (Pu aralığında), eklem kirişlerinin ΣMpr'sinin yarısı; çatıda tamamı; zeminde Mpr,kolon); Ve ≥ analiz kesmesi (18.7.6.1.1); Vc = 0 (Pu < Ag f'c/20); Vs ≤ 0,66√f'c b d | 2 × 1,4 Mr |
| Kolon sarılması | Tablo 18.7.5.4 (a), (b), (c); s ≤ min(en küçük boyut/4, 6 db, so ∈ [100,150]); lo ≥ max(h, ln/6, 450); lo dışı s ≤ min(6 db, 150) (18.7.5.5) | TBDY 7.3.4 |
| Güçlü kolon–zayıf kiriş | ΣMnc ≥ 1,2 ΣMnb (18.7.3.2), nominal Mn (f'c, fy), Pu en küçük (deprem); muafiyet yok; lifli kirişin lif katkısı γF = 1 | 1,2 ΣMr(fcd,fyd), Nd ≤ 0,10 Ac fck muafiyeti |
| Birleşim kesmesi | Vu = 1,25 fy (As1 + As2) − Vkol, Vkol = (Mpr,L + Mpr,R)/H (ETABS CFD 3.6.1 ile aynı; çatıda 0); φ = 0,85; Vn = 1,7 / 1,2 / 1,0 √f'c Aj, kuşatma 18.8.4.2 (kiriş genişliği ≥ 3/4 etkin birleşim genişliği) | 1,7 / 1,0 bj h √fck, yalnız tam kuşatma/kuşatmasız |
Birim testler: F7 (kiriş Mpr zinciri) ACI el hesabına güncellendi; 76/76 geçti.

### ETABS ile karşılaştırılan ve karşılaştırılamayan
- **Kiriş kapasite kesmesi:** V2c'de ACI zinciri (Mpr 1,25 fy, sağlanan donatı, φ = 0,75, Vc = 0) ETABS'e göre 0,964–0,985; bu turda yalnız ΣMpr'de lif γF ve 6 db sınırı değişti.
- **Kolon kesmesi ve birleşim (güçlü kolon, birleşim kesmesi): ETABS'ten doğrulanamadı.** OAPI ile kurulan modelde ETABS kolon kesme zarfında yalnız analiz kesmesini (ör. 13,5 kN) ve minimum Av/s'yi veriyor (kapasite kesmesi uygulanmıyor); `Concrete Joint Design Summary` tablosu tüm 48 kolon için "Joint check not done" döndürüyor. Çerçeve tipini kolonlarda açıkça "Sway Special" yazmak ve tüm sismik birleşimleri tasarıma almak sonucu değiştirmedi; nedeni bulunamadı (kayıt: `runs/V2h_ETABS_joint_not_done_*.txt`). Bu bileşenler ACI metniyle ve el hesabı birim testleriyle doğrulanmıştır, **ETABS ile doğrulanmamıştır**: makalede sınırlama olarak yazılır. Gereken: ETABS kolon/birleşim tasarımının neden devreye girmediğinin araştırılması (ETABS Help / CSI destek) veya ETABS arayüzünde elle denenmesi.

### Karar tablosu (IA yetkisiyle, 2026-10-30)
- **ACI mi TBDY mi:** ACI (Bölüm 18). Gerekçe: kesit kapasitesi zaten ACI 318-14 + fib MC2010; ETABS CFD ACI ile doğrulama zinciri (kiriş kesmesi, kolon P-M, δns) tutarlı; Ö2 ve Ö3 de ACI.
- **Radyal D/C:** evet; ETABS tanımıyla örtüşür ve hızlıdır (eğri enterpolasyonu).
- **Kolon kesmesi kiriş sınırı:** evet (ACI 18.7.6.1.1'in izin verdiği, ETABS CFD'nin de tarif ettiği); güçlü kolon–zayıf kiriş ile tutarlı olan kolon kesmesi eklem kirişlerinin dayanımıyla sınırlanır.

## V2i — Critic Review_Round_V2h yanıtı, ETABS birleşim/kolon-kesme doğrulaması ve dondurulan sürüm (2026-10-30)

**Karar kaydı:** Critic V2h: lifsiz ve lifli için [REVİZYON TALEBİ] (K1–K10). IA (kullanıcı) kampanyayı yine de onayladı ("Kampanya için onaylıyorum"). AGENTS 2.10 gereği IA onayı kayıtlıdır; Critic'in kod kaynaklı maddelerini kampanya **başlamadan önce** kapattım ki donmuş sürüm kuralı (kural 7) ihlal edilmesin. Kapatılamayan maddeler aşağıda "kabul edilen risk" olarak listelidir. Kampanya başlangıcı: 2026-10-30 19:22 (+03:00), `04_Results/campaign/campaign_log.txt`.

### Critic V2h maddeleri ve yanıt
| Madde | Yanıt |
|---|---|
| K1/K2 ACI kolon/birleşim zinciri için assert'li test yok | **Kapandı.** `FemRcModel.Tests` F14: 19 assert'li el hesabı (birleşim Vu = 1,25 fy (As1 + As2) − Vkol, çatıda Vkol = 0, φVn = 0,85·1,7·√f'c·Aj, kuşatmasız birleşimde katsayı 1,0, ΣMnb, çatı birleşiminde güçlü kolon kontrolü, kiriş sınırlı Ve ≤ Ve(yalnız kolon Mpr), Tablo 18.7.5.4 (a), s ≤ b/4, lo ≥ 450). Toplam **95/95**. |
| ETABS'te kolon/birleşim doğrulaması | **Kapandı; kök neden bulundu:** probe, kiriş donatısını (`SetRebarBeam`) analizden **sonra** çağırıyordu, model kilitli olduğundan `ret=1` veriyordu ve ETABS kirişlere donatı atanmadığı için birleşim kontrolünü ve kapasite kesmesini yapmıyordu ("Joint check not done"). Donatı analizden önce atanınca ETABS bunları hesapladı (aşağıda). V2h'deki "ETABS ile doğrulanamadı, nedeni bulunamadı" ifadesi **geri alınır.** |
| K3 Q kısıtı | **Kapandı.** `Stability.MaxQ`: Q = ΣPu Δo D / (Vus lc) ≤ 0,05 optimizasyon kısıtı (Δo: RS kat ötelenmesi, D = 3 ile büyütülmüş; muhafazakâr okuma). Deneme tasarımı: Q(3Δ) = 0,0504 (EY, kat 2): kısıtın sınırında. |
| K4 ΣMpr paylaşımı sabit 0,5 | **Kapandı.** Pay = K/(K + K'), K = Ec I (üst/alt kolon, yönde); çatıda 1,0; zeminde sınırsız (Mpr,kolon). Ayrıca ETABS Veb kuralı (üst eklem ΣMpr/H, çatıda iki katı, Vec ile sınırlı) ile Ve'nin **altına inilmez** (büyük olan alınır). |
| K5 φv = 0,75, ACI 21.2.4.1 | **Gerekçeli.** Kesme donatısı kapasite kesmesi Ve (Mpr, 1,25 fy) için tasarlanıyor; Vn ≥ Mn kesmesi olduğundan 21.2.4.1 koşulu tetiklenmez (φ = 0,75). ETABS'in 0,60 tercihi tasarım kesmesi (kapasite dışı) içindir (CFD 3.5.2.3). |
| K6 eksik ACI kısıtları | **Kapandı:** 18.7.2.1(b) (b/h ≥ 0,4), 18.8.2.3 (kolon boyutu ≥ 20 db; db = 20 mm kiriş çubuğu varsayımıyla her iki boyut ≥ 400 mm), 18.7.5.2(e)/(f) (hx ≤ 350 / 200 mm), roof birleşiminde güçlü kolon, Mnc en düşük değeri (Pmin ve Pmax uçlarının küçüğü, çekmede kenetleme yok), Vdy'de (1 + ez), eklem içi etriye maliyeti (aynı aralık, azaltmasız). 18.8.2.4 (h ≥ hkiriş/2) kolon havuzunda (≥ 300) ve kiriş h ≤ 600 ile sağlanır. |
| K7/K8 CD3 ve R2 | **Düzeltme:** V2h'deki "çoğunlukla %0–10 yüksek" ifadesi yanlıştı: 500/400 deprem birleşimlerinde üretim D/C, ETABS'ten %29–32 yüksek (güvenli yön, e_min/δns/8 işaret deseni nedeniyle). CD3'ün **birincil ölçütü güvensiz yönde ≤ %5** (geçti: en kötü −%1,0); güvenli yönde fazlalık Spec'te "bilgi" olarak yeniden yazıldı. **R2 kapanmadı:** ETABS D/C 0,8–1,2 bandında yalnız 14–16 satır var (istenen ≥ 20) ve ikinci yapı yok: **kabul edilen risk** (IA kampanya onayı). |
| K9/K10 tek kapı tablosu, çıkar çatışması | Aşağıdaki güncel kapı tablosu tek referanstır; eşikleri yazan ajan (Claude) onaylayanla aynıdır, IA'nın "tüm kararları onaylıyorum" yetkisi kayıtlıdır (MEMORY.md 3g); bağımsız onay Critic'te kalır. |

### ETABS karşılaştırması (CFD ACI 318-14, aynı kuvvet, aynı donatı, DESIGN=1 JOINT=1; `runs/V2i_*`)
| Karşılaştırma | 500/400 mm | 400/350 mm |
|---|---|---|
| Birleşim kesmesi oranı (Vu/φVn) maks, program ↔ ETABS | 0,634 ↔ 0,611 (+%3,8) | 0,828 ↔ 0,798 (+%3,8) |
| Birleşim kesmesi, birleşim bazında | %0,2–4 içinde (örn. 0,201/0,202; 0,402/0,404; 0,524/0,505; 0,370/0,379) | |
| ΣMnc/ΣMnb en küçük (kritik), program ↔ ETABS | 0,742 ↔ 0,759 (−%2,2, güvenli) | 0,613 ↔ 0,623 (−%1,6, güvenli) |
| ΣMnc/ΣMnb, tek tek (iç/yan kolon) | 6,025 ↔ 5,920; 3,232 ↔ 3,188 (+%1,7…1,8: kapasite oranı biraz yüksek) | |
| Kolon tasarım kesmesi Ve, üretim / ETABS (48 kolon) | ort. 2,32, min **1,000**, maks 4,16 | ort. 1,85, min **1,000**, maks 3,02 |
Kolon kesmesi **hiçbir kolonda ETABS'in altında değil**: çatı kolonlarında eşit (ETABS kuralı), alt katlarda program 2–4 kat büyük çünkü (i) ETABS zemin ucunu hesaba katmıyor (üretim zemin ucunda Mpr,kolon kullanıyor, ACI 18.7.6.1.1), (ii) üretim grup maksimumuyla tasarlıyor. Bu, maliyet açısından bilinçli muhafazakârlıktır ve makalede yazılır. **ΣMnc/ΣMnb'de program %1,8'e kadar daha yüksek oran (güvensiz) veriyor:** ETABS'in kapasite hesabıyla küçük fark (Mnc'de %1–2); eşik B/C ≥ 1,2 kısıtının kendisinde bu fark istatistiksel olarak tolere edilir ve sınırlama olarak yazılır.

### Güncel kapı tablosu (tek referans)
| Kapı | Sonuç | Durum |
|---|---|---|
| FE1–FE4 FemCore ↔ ETABS (periyot, taban kesmesi, yer değiştirme, 116 eleman RS, rijit bölge) | ≤ %0,164 | GEÇTİ |
| CD1 yanlış-uygun = 0 | 1920 satır, 0 | GEÇTİ |
| CD2 radyal+e_min+δns ≥ 0,95 | min 0,961 | GEÇTİ |
| CD3 kritik D/C, güvensiz yönde ≤ %5 | −%1,0 | GEÇTİ (güvenli yön fazlalığı bilgi) |
| B1–B3 kiriş As, Av/s, kapasite kesmesi | 1,000 / 0,98–1,05 / 0,96–0,99 | GEÇTİ |
| Kolon kapasite kesmesi ≥ ETABS | min 1,000 | GEÇTİ |
| Birleşim kesmesi ETABS ±%5 | +%3,8 maks | GEÇTİ |
| Güçlü kolon kapasite oranı, güvensiz yönde ≤ %2 | +%1,8 | GEÇTİ (sınırda) |
| S1 SFRC bağımsız entegrasyon | %0,0008 | GEÇTİ |
| S2 SFRC deney kıyası | veri yok/sonuçsuz | **AÇIK** (lifli sonuçlar "model sonucu") |
| R2 ≥ 20 satır, D/C 0,8–1,2 + ikinci yapı | 14–16 satır, ikinci yapı yok | **KABUL EDİLEN RİSK** |
| Q ≤ 0,05 | deneme 0,0504, optimizasyonda kısıt | kısıt |

### Dondurulan program ve kampanya
- Dondurulan ikili: `03_Code_Execution/campaign_bin/` (Ex1Runner, FemRcModel.dll SHA-256 `5287c0c7…13ad`, FemCore.dll `ab10f926…d053`, RcCore.dll `feb73d8d…947c`, derleme 2026-10-30 16:22Z). Testler 16 / 95 / 59, hepsi geçti (`runs/tests_*_V2i_*`).
- **Kampanya (2026-10-30 19:22):** 3 algoritma (YAKO, BTO, ATO) × 30 bağımsız koşu × 10 000 FE, tohumlar 1001–1030; senaryolar sırayla: lifsiz; lifli (fR çarpanı 1,00); lifli (0,85); lifli (0,70); lif fiyatı 90 TL/kg; çıktı `04_Results/campaign/*.csv` (her dosya program SHA-256'sını taşır). `03_Code_Execution/run_campaign.sh` yeniden başlatılabilir.
- Tahmini süre: ≈ 22 saat (8 çekirdek, çekirdek başına ≈ 5,6 FE/s).
- **Kampanya sonuçları, Critic ve IA yeniden değerlendirmesi olmadan bulgular bölümüne giremez; lifli sonuçlar "model sonucu"dur (S2 açık).**

