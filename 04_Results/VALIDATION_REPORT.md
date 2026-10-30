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

