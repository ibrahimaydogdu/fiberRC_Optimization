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
