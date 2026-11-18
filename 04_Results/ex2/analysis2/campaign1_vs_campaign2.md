# Ö2 kampanya 1 ↔ kampanya 2 karşılaştırması (betimsel; post-hoc etkisi)

Üretici: `03_Code_Execution/tools/ex2_compare_campaigns.py` (otomatik). Kaynak: kampanya 1 `ex2/analysis/ex2_runs.csv`, kampanya 2 `ex2/analysis2/ex2_runs_campaign2.csv` (her ikisi `ex2_stats.py` koşu satırları). Kampanya 1 = ön kayıtlı seri (ikili Ex2Model `d0514e2f…`; desenli yük yok, kiriş eksenel kuvveti yok, SFRC bayat-lcs hatası; **superseded**); kampanya 2 = post-hoc §11.2 (ikili `a61b0c6b…`; PCR-23/24/25). İki seri farklı tasarım kurallarıyla arandığından fark "kural değişikliği + arama" etkisidir; test yapılmadı, yöntem sıralaması yok.

## 1. Hücre medyanları ve en düşükler (TL)

| Hücre | Yöntem | Medyan K1 | Medyan K2 | Δ medyan (%) | En düşük K1 | En düşük K2 | Δ en düşük (%) | Lif > 0 koşu K1 | Lif > 0 koşu K2 | Lif kg medyanı K1 / K2 |
|---|---|---|---|---|---|---|---|---|---|---|
| lifsiz | YAKO | 231096.56 | 237494.78 | +2.769 | 230325.40 | 236711.62 | +2.773 | — | — | — |
| lifsiz | BTO | 230308.35 | 236462.41 | +2.672 | 230308.35 | 236456.22 | +2.669 | — | — | — |
| lifsiz | ATO | 230308.35 | 236456.22 | +2.669 | 230308.35 | 236456.22 | +2.669 | — | — | — |
| lifli 90 TL/kg | YAKO | 235123.78 | 240977.61 | +2.490 | 230800.51 | 238260.94 | +3.232 | 5/30 | 4/30 | 0 / 0 |
| lifli 90 TL/kg | BTO | 230308.35 | 236468.59 | +2.675 | 230308.35 | 236456.22 | +2.669 | 0/30 | 0/30 | 0 / 0 |
| lifli 90 TL/kg | ATO | 230308.35 | 236456.22 | +2.669 | 230308.35 | 236456.22 | +2.669 | 0/30 | 0/30 | 0 / 0 |
| lifli 0 TL/kg | YAKO | 229701.14 | 235917.55 | +2.706 | 228333.82 | 234012.58 | +2.487 | 30/30 | 30/30 | 218 / 220 |
| lifli 0 TL/kg | BTO | 228340.77 | 233944.06 | +2.454 | 228294.22 | 233916.61 | +2.463 | 30/30 | 30/30 | 240 / 248 |
| lifli 0 TL/kg | ATO | 228294.22 | 233916.61 | +2.463 | 228294.22 | 233916.61 | +2.463 | 30/30 | 30/30 | 240 / 248 |

## 2. Lif kararı (ön kayıt kural 4 göstergeleri) ve senaryo farkı

| Yöntem | Lifsiz → 0 TL/kg medyan farkı K1 (%) | K2 (%) | Lifsiz → 90 TL/kg medyan farkı K1 (%) | K2 (%) | 0 TL/kg en sık kiriş deseni K1 | K2 |
|---|---|---|---|---|---|---|
| YAKO | -0.604 | -0.664 | +1.743 | +1.466 | B_S1 250x500 4D/40; B_S2 250x500 5D/35; B_S3 250x500 5D/35 (3/30) | B_S1 250x500 4D/40; B_S2 250x500 5D/35; B_S3 250x500 4D/40 (3/30) |
| BTO | -0.854 | -1.065 | +0.000 | +0.003 | B_S1 250x500 4D/40; B_S2 250x500 4D/40; B_S3 250x500 4D/40 (22/30) | B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 (18/30) |
| ATO | -0.875 | -1.074 | +0.000 | +0.000 | B_S1 250x500 4D/40; B_S2 250x500 4D/40; B_S3 250x500 4D/40 (28/30) | B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 (27/30) |

## 3. Hücre en düşük etkin tasarımları (BTO/ATO/YAKO birleşik; ilk dosya)

| Hücre | K1 en düşük (TL) ve etkin tasarım | K2 en düşük (TL) ve etkin tasarım |
|---|---|---|
| lifsiz | 230308.35 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv`) | 236456.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`plain/EX2_ATO_plain_g9_fr1.00_fp90_run02.csv`) |
| lifli 90 TL/kg | 230308.35 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 350x350; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv`) | 236456.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv`) |
| lifli 0 TL/kg | 228294.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x500 4D/40; B_S3 250x500 4D/40 (`fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv`) | 233916.61 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 (`fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv`) |

