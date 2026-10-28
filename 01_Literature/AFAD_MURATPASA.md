# AFAD_MURATPASA.md — Örnek 1 deprem girdisi (Antalya, Muratpaşa, ZC)

Kaynak: AFAD TDTH özet raporları, 28.10.2026, nokta 36.899312° K, 30.674937° D, zemin ZC. Dosyalar `Evidence/AFAD/` (git dışı). Değerler PDF çıktısından birebir.

| Düzey | Dosya (SHA-256, bayt) | Ss | S1 | SDS | SD1 | PGA (g) | PGV (cm/s) |
|---|---|---|---|---|---|---|---|
| DD-1 (2475 yıl) | AFAD_Muratpasa_ZC_DD1_28102026.pdf (defcc0cf…8ad42, 570071) | 1,113 | 0,290 | 1,336 | 0,435 | 0,513 | 21,363 |
| DD-2 (475 yıl) | AFAD_Muratpasa_ZC_DD2_28102026.pdf (9dd7b4b3…298ff, 567270) | 0,563 | 0,151 | 0,718 | 0,226 | 0,263 | 11,125 |
| DD-3 (72 yıl) | AFAD_Muratpasa_ZC_DD3_28102026.pdf (1798bdc4…de16f, 569409) | 0,226 | 0,061 | 0,294 | 0,091 | 0,107 | 4,475 |

## Tutarlılık (Claude hesabı; Fs/F1 hafızadan, TBDY Tablo 2.1/2.2 görüntü kontrolü G10 sonrası)
- DD-2: Fs(Ss=0,563) = 1,3 − 0,1·(0,063/0,25) = 1,275 → 0,563×1,275 = 0,718 ✓; F1 = 1,5 → 0,151×1,5 = 0,2265 ✓
- DD-3: Fs = 1,3 → 0,294 ✓; F1 = 1,5 → 0,0915 ✓. DD-1: Fs = 1,2 → 1,336 ✓; F1 = 1,5 → 0,435 ✓

## Sonuçlar (DOĞRULAMA BEKLİYOR: TBDY Tablo 3.1–3.3)
- Tez Ö1: SDS = 1,2 / SD1 = 0,3 (DD-2) → DTS 1. Muratpaşa DD-2: SDS = 0,718 → muhtemel DTS 2 (SDS 0,5–0,75); A31 yine yalnız DTS 3-4'te izinli → uygulanamaz.
- λ için DD-1/DD-3 değerleri artık elde; formül G10 ile (TBDY 4.9.1).
