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

## V2b: kolon tasarım oranı, ETABS Concrete Frame Design (ACI 318-14) ↔ program (2026-10-30)

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
