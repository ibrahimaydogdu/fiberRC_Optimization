# TBDY_VERIFY_G9_G10.md — Claude doğrulaması (2026-10-28, sayfa görüntüsünden)

Kaynak: `Evidence/DesignGuides/TDBY2018.pdf` (SHA-256 8d3a9a46…8a6bb2); PDF 37, 76, 77, 136–141, 144–147 sayfa görüntüsü okundu (basılı = PDF − 21).

## G9 — DOĞRULANDI (denklem ve sınırlar)
| Madde / Denklem | PDF / basılı | Sonuç |
|---|---|---|
| 7.3.4.1(a)–(d), Denk. 7.1, 7.2 (sarılma boyu, φ ≥ 8, s sınırları, Ash) | 136–137 / 115–116 | birebir doğru |
| 7.3.4.2 orta bölge | 137 / 116 | doğru |
| 7.3.5.1–7.3.5.3 (Denk. 7.3, Nd en küçük; muafiyet a/b/c) | 137–139 / 116–118 | doğru |
| 7.3.6, Denk. 7.4 (α ≥ 0,70, 1/α) | 139 / 118 | doğru |
| 7.3.7.1–7.3.7.6, Denk. 7.5–7.7 | 139–141 / 118–120 | doğru; Vc: TS 500 |
| 7.4.4 (2 hk, sk ≤ min(d/4; 8 φl; 150)), 7.4.5, Denk. 7.9, 7.10 | 144–145 / 123–124 | doğru (şekil sk ≤ hk/4 yazar, metin "etkili yükseklik" der) |
| 7.5.1, 7.5.2, Denk. 7.11–7.13, 7.5.2.3 | 146–147 / 125–126 | doğru |
Düzeltme: görev metnindeki "7.3.4/7.3.7" numaraları doğruydu; Claude'un C1 satırındaki "7.3.7 sarılma" ifadesi 7.3.4'tür.

## G10 — kısmen
- DOĞRULANDI: 4.9.1.1–4.9.1.4 (Denk. 4.32, 4.33, 4.34a/b; κ = 1; λ = Sae(DD-3)/Sae(DD-2)) — **Denk. 4.34 gerçekte `λ δ/h ≤ 0,008 κ`** (Gemini'nin yazımıyla eşdeğer). 7.3.5.3 ve 7.3.6 (yukarıda). Tablo 3.2 (DTS): SDS < 0,33 → 4; 0,33–0,50 → 3; 0,50–0,75 → 2; ≥ 0,75 → 1 (BKS = 2,3; BKS = 1 için "a"). **Önceki "0,25/0,5/0,75" varsayımı yanlıştı.**
- DOĞRULANAMADI (görüntü bakılmadı): Gemini'nin Tablo 2.1/2.2 (Fs, F1) ve Tablo 3.3 (BYS) "birebir aynı" beyanı, "Tablo 3.1 = I katsayıları" ifadesi (Tablo 3.1 başlığı kontrol edilecek).

## Sonuçlar
- Muratpaşa DD-2: SDS = 0,718 → **DTS = 2** (BKS = 3 varsayımı; konut/ofis). A11 (süneklik düzeyi yüksek) DTS 2'de izinli; A31 yalnız DTS 3-4 → yine uygulanamaz. B6 (alt donatı ≥ %50 üst) DTS 1/1a/2/2a için geçerli → Ö1'de geçerli.
- λ ≈ 0,40–0,41 → izinli kat ötelemesi ≈ 0,0195–0,0199 h (tez 1/300 = 0,0033 idi; çok gevşek). Ö1'de ötelenme kısıtı büyük olasılıkla bağlayıcı değil; kuvvet/güçlü kolon ve kesme kısıtları belirleyici olur.
- Eksik: Fs/F1 (Tablo 2.1/2.2) ve BYS (Tablo 3.3) görüntü kontrolü; Ö1 bina kullanım sınıfı (BKS) teyidi.
