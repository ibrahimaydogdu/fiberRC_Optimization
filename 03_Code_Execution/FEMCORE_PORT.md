# FEMCORE_PORT.md — FemCore çalışma kopyası (2026-10-28)

Kaynak (salt-okunur): `D:\OneDrive Akdeniz Ogrenci\...\SteelFamewithCompositeTubeColumn_FEM\FemCore\*.vb` (11 dosya, 2874 satır). Kopya: `program_work/FemCore/` (SDK stili `FemCore.vbproj`, net472, MathNet.Numerics 5.0.0; kaynak dosyalar değiştirilmedi). Derleme: `dotnet build -c Release` → 0 hata, 3 uyarı (BC42324: `FemModal.vb` 369, 384, 385, lambda içinde döngü değişkeni; çelik tabanda var, RC için modal/RS kullanımında kontrol edilecek).

Kaynak dosya SHA-256 (ilk 16 karakter): FemChain e2c5386d…, FemDesign 2914b5e5…, FemDesignDriver 01bd2dfb…, FemElement f51c5190…, FemEvaluator cd00192f…, FemModal 12c630f7…, FemModel 123c072a…, FemPDelta 09645b06…, FemSeismic 089d2970…, FemSkyline 8e4d495b…, FemStatic 93375370….

Çelik özel dosyalar (`FemDesign`, `FemDesignDriver`, `FemSeismic`) derlemede duruyor ama RC akışında kullanılmayacak.

## Doğrulama (FemCore.Tests; analitik) — 16/16 kontrol geçti (2026-10-28)
| Test | Beklenen (analitik) | Sonuç |
|---|---|---|
| T1 konsol kolon, uç yükü | PL³/3EI, tepki, mesnet momenti | tutuyor (≤ %0,01) |
| T2 iki ucu ankastre kiriş, yayılı yük | wL/2, wL²/12 | tutuyor |
| T3 konsol + uç kütlesi | T = 2π√(m/(3EI/L³)) | tutuyor |
| T4 portal çerçeve, rijit kiriş | H h³/(24EI), H h/4 | tutuyor (≤ %0,01; kolon eksenel şekil değiştirmesi dışlanarak) |
| T5 tek katlı 3B, rijit diyafram, 4 kolon | H h³/(4·3EI), diyafram eşit Ux, Rz = 0 | tutuyor |
| T6 P-Δ (kütle esaslı, ETABS ön ayarı) | H/(k − m g/z) | tutuyor |
| T7 tepki spektrumu, tek serbestlik | taban kesmesi m Sa g, yerdeğiştirme Sa g/ω² | tutuyor |
| T8 iki katlı kayma binası, modal | ω² = (k/m)(3 ∓ √5)/2 | T1, T2 ≤ %0,003 |
Not: T4 ilk koşuda %1 sapma verdi; neden kolonların eksenel şekil değiştirmesiyle kirişin dönmesi (gerçek davranış), testte kolon alanı büyütülerek ayrıldı. VB büyük/küçük harf duyarsız: `I` (atalet) ile döngü değişkeni `i` çakıştı → `Iz` olarak yeniden adlandırıldı. Google Drive eşzamanlama gecikmesi nedeniyle dosya bir kez eski sürümle okundu; koşu öncesi içerik doğrulanıyor.
**Henüz test edilmedi:** 3B burulma (merkezkaç yük), CQC çok modlu karşılaştırma, çatlamış rijitlik `Modifier`, P-Δ iteratif çözüm, ETABS karşılaştırması (V2).

## Sıradaki
1. 2 katlı 2B çerçeve + 3B tek kat: elle/ETABS karşılaştırması (V2 başlangıcı).
2. `FemRcModel` kurucusu: parametrik kesit (b, h, fc, lif) → `FemSection` (E = 4700√fc', çatlamış I: kiriş 0,35, kolon 0,70), grup → kesit.
3. Uç kuvvetlerini `EndForce/FaceForce` üzerinden `RcCore` kapasite kontrolüne bağlama.
