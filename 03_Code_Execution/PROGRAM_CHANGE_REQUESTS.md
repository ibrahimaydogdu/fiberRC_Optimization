# PROGRAM_CHANGE_REQUESTS.md
Biçim: `PCR-N | tarih | gerekçe | beklenen etki | gereken yeniden doğrulama | durum (talep/onaylı/uygulandı)`

Henüz talep yok. İlk aday (D8 onayı bekliyor): `Inputinfo.txt` sabit `E:\` yolları ve program-model eşleşmesi incelemesi.

## Taslak PCR'ler (2026-10-30, V2 doğrulamasından; Critic E1/K1/K2). Durum: TALEP, IA onayı bekliyor. Onaysız uygulanmaz.
- **PCR-1 | 2026-10-30 | Kolon kısıtına ACI e_min eklenmesi (`ColumnDesign.CheckGroup`, her işaret deseni için M ≥ Pu·(15,24 mm + 0,03 h), bir doğrultuda ayrı ayrı) | Gerekçe: V2b/V2d'de ETABS (e_min açık) D/C > 1 iken üretim kısıtı ≤ 1 (2 kolon, D/C 1,04); radyal+e_min varyantında 0 yanlış-uygun | Etki: kolon boyutu/donatı biraz artar; maliyet duyarlılığı koşuyla ölçülür | Yeniden doğrulama: V2b/V2d, FemRcModel.Tests | **UYGULANDI 2026-10-30** (IA onayı 2026-10-30; `ColumnDesign.DcrWithEffects`, V2e: yanlış-uygun 0)**
- **PCR-2 | 2026-10-30 | Narinlik (ACI 6.6.4.5 δns, Cm, EI = 0,4EcIg/(1+βdns), k=1) kolon kısıtına eklenmesi VEYA kolon alt sınırının 300 → 400 mm yapılması | Gerekçe: V2d'de 300 mm kolonlarda ETABS U1 D/C'si δns ile 0,709 → 0,818 (+%15); deprem birleşimlerinde etki yok (sway, P-Δ varsayımı); programda narinlik yok | Etki: yalnız gravite birleşiminde ince kolonlar | Yeniden doğrulama: V2d | **UYGULANDI 2026-10-30** (IA onayı: δns eklenir, alt sınır 300 mm korunur; `ColumnDesign.DeltaNs`; sway çerçeve sınırı lu/r ≤ 22, lu = 0,9 × eleman boyu, Cm = 1, βdns = 1,4G/Pu; V2e: ETABS ile 0,999–1,237)**
- **PCR-3 | 2026-10-30 | Kolon D/C tanımının raporlanması: üretim kısıtı sabit-P moment oranı (ETABS radyal OL/OC değil); ceza ölçeği ETABS D/C'sinden farklıdır | Gerekçe: V2b: gravite birleşiminde üretim D/C 0,29–0,49 iken ETABS 0,35–0,84 (eksenel baskın kolon) | Etki: yalnız raporlama/ceza ölçeği; uygunluk (≤ 1) aynı yüzeyi kullanır | Yeniden doğrulama: yok (makalede açıklama) | talep (kod değişikliği gerekmez; makale metnine not)**
- **PCR-4 | 2026-10-30 | Kapasite kesmesi: ETABS ACI zinciri (Mpr, 1,25 fy, sağlanan donatı) ile programın TBDY 7.4.5 zinciri (1,4 Mr/ln, fcd/fyd) farklı şartnamelerdir; makalede TBDY zinciri kullanılacaksa ETABS ile doğrulama TS 500-2000(R2018) menüsüyle yapılmalı | Gerekçe: V2c | Etki: yok (yöntem seçimi) | Yeniden doğrulama: V2c TS 500 menüsü | talep**

Uygulanan düzeltmeler (hata düzeltmesi, formülasyon değişikliği değil; Developer yetkisi): (a) `ColumnDesign.CheckGroup` 8 işaret deseni (2026-10-30, V2b); (b) `FemStatic.AddAccidentalTorsion` burulma tork kuvveti: diyafram CQC kuvveti yerine CQC kat kesmesi farkı (`TorsionFromStoryShear`, V2c tanı 2026-10-30; ETABS ile %0,1 altı uyum).


**PCR-1/2 uygulama notu (2026-10-30):** `ColumnDesign.vb`: `EminBase = 15,24 mm`, `DeltaNs`, `DcrWithEffects`, `LuRatio = 0,9`, `DcrCap = 10`. δns yalnız gravite birleşiminde (deprem birleşimlerinde sway/P-Δ varsayımı, ETABS ile aynı); e_min tüm birleşimlerde, her doğrultu ayrı. Parametreler ETABS davranışına hizalanmıştır (ETABS'in program belirlediği uzunluk oranı 0,9 ve Cm = 1); ACI 6.6.4.5.3'ün Cm = 0,6 + 0,4 M1/M2 ifadesi daha az muhafazakâr olurdu (300 mm kolonda ETABS'e göre %11 düşük kaldı). Bu seçim makalede varsayım olarak yazılır.

**PCR-2 revizyon notu (2026-10-30, V2f; Critic V2e N1/A2 yanıtı):** δns artık (a) lu = net kat yüksekliği (ACI 6.2.5.1), (b) sismik birleşimlerde de (ETABS davranışı, muhafazakâr tüm-moment büyütmesi, βdns = (1+ez)G/Pu), (c) üretim donatı düzeni iki doğrultu için tutarlı (`PerimeterCounts`, `MakeSection(..., transposed)`). Önceki "lu = 0,9L" ETABS kopyası kaldırıldı. Birim testler 75/75.

**PCR-3 açık karar (IA):** Kolon kısıtı (a) sabit-P moment oranı olarak kalsın, makalede tanım yazılsın (CD3 bilgi amaçlı), ya da (b) radyal orana çevrilsin (ETABS tanımı; hesap maliyeti artar). Öneri: (a) ile başla, radyal oranı yalnız raporlama için final tasarımlarda hesapla.

**PCR-3 UYGULANDI (2026-10-30, IA kararı: radyal D/C):** `ColumnDesign.ColCurve/RadialDcr`; ETABS ile 5 konfigürasyon (V2h). **PCR-4 KARARA BAĞLANDI (IA yetkisiyle Claude):** kapasite tasarımı ACI 318M-14 Bölüm 18; TS 500 menüsü karşılaştırması gerekmiyor. **PCR-5 UYGULANDI:** ACI zinciri `BeamDesign.BeamShear` (Mpr 1,25 fy, 6 db), `ColumnShear.Design` (ΣMpr sınırı, Tablo 18.7.5.4), `Checks.StrongColumn` (ACI 18.7.3.2), `JointShear` (ACI 18.8.4); testler 76/76. Kayıt: `04_Results/VALIDATION_REPORT.md` V2h.

