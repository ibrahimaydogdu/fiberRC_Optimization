# Kampanya 1 son vektörlerinin düzeltilmiş kodla deterministik yeniden değerlendirmesi (PCR-23/24/25)

Üretici: `03_Code_Execution/tools/ex2_reeval_summary.py` (otomatik). Girdi: `04_Results/ex2/reeval_pcr23_25/{x,old,p23,new}/` (`tools/ex2_reeval_pcr23_25.sh`). **old** = kampanya 1 ikilisi (`ex2_campaign_bin`, d0514e2f…); **p23** = kampanya 2 ikilisi (`ex2_campaign2_bin`, 74d94426…) kampanya 1 yük kurallarıyla (`--patterned 0 --beamaxial 0`; PCR-23 (1)–(3) açık: tutarlı SFRC kapasitesi, g8, düz beton RequiredAs düzeltmesi); **new** = kampanya 2 kuralları (PCR-23 + PCR-24 eksenel çekme + PCR-25 desenli zarf). Yeniden optimizasyon DEĞİLDİR: kesitler sabit, donatı/etriye/kolon ρ yeniden türetilir. İstatistik tablolarının yerine geçmez (`EX2_SPEC.md` §11.2).

| Hücre | Kural seti | n | T > 0 (uygunsuz) | En çok ihlal edilen kısıt (koşu) | Kiriş eğilme D/C en büyük | D/C > 1 + 10⁻⁶ koşu | Maliyet değişimi medyan (min–maks), % (old'a göre) | En düşük maliyet (TL) |
|---|---|---|---|---|---|---|---|---|
| lifsiz | old | 90 | 0 | — | 1.0000 | 0 | 0.000 (0.000 – 0.000) | 230308.35 (T = 0) |
| lifsiz | p23 | 90 | 0 | — | 1.0000 | 0 | 0.000 (0.000 – 0.000) | 230308.35 (T = 0) |
| lifsiz | new | 90 | 0 | — | 1.0000 | 0 | 2.830 (2.486 – 3.554) | 236747.64 (T = 0) |
| lifli 90 TL/kg | old | 90 | 0 | — | 1.0000 | 0 | 0.000 (0.000 – 0.000) | 230308.35 (T = 0) |
| lifli 90 TL/kg | p23 | 90 | 0 | — | 1.0000 | 0 | 0.000 (0.000 – 0.000) | 230308.35 (T = 0) |
| lifli 90 TL/kg | new | 90 | 2 | g6 col 2nd-order M <= 1.4 x 1st-order M (2) | 1.0000 | 0 | 2.830 (2.295 – 4.062) | 236747.64 (T = 0) |
| lifli 0 TL/kg | old | 90 | 0 | — | 1.0103 | 52 | 0.000 (0.000 – 0.000) | 228294.22 (T = 0) |
| lifli 0 TL/kg | p23 | 90 | 0 | — | 1.0000 | 0 | 0.027 (-0.001 – 0.028) | 228340.77 (T = 0) |
| lifli 0 TL/kg | new | 90 | 0 | — | 1.0000 | 0 | 2.873 (2.273 – 4.233) | 234801.38 (T = 0) |

## Hücre en düşük (kampanya 1) tasarımları

| Hücre | Koşu | old maliyet / T / D/C | p23 maliyet / T / D/C | new maliyet / T / D/C | new ihlaller |
|---|---|---|---|---|---|
| lifsiz | `EX2_ATO_plain_g9_fr1.00_fp90_run01` (+45 aynı maliyetli) | 230308.35 / 0 / 1.0000 | 230308.35 / 0 / 1.0000 | 236825.02 / 0 / 1.0000 |  |
| lifli 90 TL/kg | `EX2_ATO_fibre_g9_fr1.00_fp90_run01` (+45 aynı maliyetli) | 230308.35 / 0 / 1.0000 | 230308.35 / 0 / 1.0000 | 236825.02 / 0 / 1.0000 |  |
| lifli 0 TL/kg | `EX2_ATO_fibre_g9_fr1.00_fp0_run01` (+37 aynı maliyetli) | 228294.22 / 0 / 1.0103 | 228357.12 / 0 / 1.0000 | 234852.10 / 0 / 1.0000 |  |

