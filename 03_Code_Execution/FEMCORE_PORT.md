# FEMCORE_PORT.md — FemCore çalışma kopyası (2026-10-28)

Kaynak (salt-okunur): `D:\OneDrive Akdeniz Ogrenci\...\SteelFamewithCompositeTubeColumn_FEM\FemCore\*.vb` (11 dosya, 2874 satır). Kopya: `program_work/FemCore/` (SDK stili `FemCore.vbproj`, net472, MathNet.Numerics 5.0.0; kaynak dosyalar değiştirilmedi). Derleme: `dotnet build -c Release` → 0 hata, 3 uyarı (BC42324: `FemModal.vb` 369, 384, 385, lambda içinde döngü değişkeni; çelik tabanda var, RC için modal/RS kullanımında kontrol edilecek).

Kaynak dosya SHA-256 (ilk 16 karakter): FemChain e2c5386d…, FemDesign 2914b5e5…, FemDesignDriver 01bd2dfb…, FemElement f51c5190…, FemEvaluator cd00192f…, FemModal 12c630f7…, FemModel 123c072a…, FemPDelta 09645b06…, FemSeismic 089d2970…, FemSkyline 8e4d495b…, FemStatic 93375370….

Çelik özel dosyalar (`FemDesign`, `FemDesignDriver`, `FemSeismic`) derlemede duruyor ama RC akışında kullanılmayacak.

## Doğrulama (FemCore.Tests; analitik)
| Test | Sonuç |
|---|---|
| T1 konsol kolon uç yükü: uç yerdeğiştirme PL³/3EI, mesnet kuvveti ve momenti | tutuyor (≤ %0,01) |
| T2 iki ucu ankastre kiriş, yayılı yük: mesnet tepkisi wL/2, uç momenti wL²/12 | tutuyor |
| T3 konsol + uç kütlesi: periyot 2π√(m/(3EI/L³)) | tutuyor (≤ %0,05) |
Toplam 6/6. **Henüz test edilmedi:** P-Δ (`FemPDelta`), rijit diyafram, 3B (burulma), tepki spektrumu (CQC), çatlamış rijitlik `Modifier`, ETABS karşılaştırması.

## Sıradaki
1. 2 katlı 2B çerçeve + 3B tek kat: elle/ETABS karşılaştırması (V2 başlangıcı).
2. `FemRcModel` kurucusu: parametrik kesit (b, h, fc, lif) → `FemSection` (E = 4700√fc', çatlamış I: kiriş 0,35, kolon 0,70), grup → kesit.
3. Uç kuvvetlerini `EndForce/FaceForce` üzerinden `RcCore` kapasite kontrolüne bağlama.
