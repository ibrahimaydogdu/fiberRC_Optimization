# Ö2 kampanya 2 ↔ kampanya 4 karşılaştırması (betimsel)

Üretici: `03_Code_Execution/tools/ex2_compare_campaigns_c4.py` (otomatik). Kaynak: `ex2/analysis2/ex2_runs_campaign2.csv` (kampanya 2) ve `ex2/campaign4/runs.csv` (kampanya 4); her ikisi `ex2_stats.py` koşu satırları. Kampanya 2 = PCR-23/24/25; 3 = + g5 tüm-yüklü ℓ/480 (PCR-26); 4 = + g5 desenli, Ie zarfı (PCR-26b). Kural kümeleri farklıdır; fark kural değişikliği + arama etkisidir; test yapılmadı, yöntem sıralaması yok.

## 1. Hücre medyanları ve en düşükler (TL)

| Hücre | Yöntem | Medyan K2 | Medyan K4 | Δ medyan (%) | En düşük K2 | En düşük K4 | Δ en düşük (%) | Lif > 0 koşu K2 | Lif > 0 koşu K4 | Lif kg medyanı K2 / K4 |
|---|---|---|---|---|---|---|---|---|---|---|
| lifsiz | YAKO | 237494.78 | 236754.53 | -0.312 | 236711.62 | 236456.22 | -0.108 | — | — | — |
| lifsiz | BTO | 236462.41 | 236456.22 | -0.003 | 236456.22 | 236456.22 | +0.000 | — | — | — |
| lifsiz | ATO | 236456.22 | 236456.22 | +0.000 | 236456.22 | 236456.22 | +0.000 | — | — | — |
| lifli 90 TL/kg | YAKO | 240977.61 | 240757.50 | -0.091 | 238260.94 | 237799.80 | -0.194 | 4/30 | 4/30 | 0 / 0 |
| lifli 90 TL/kg | BTO | 236468.59 | 236468.59 | +0.000 | 236456.22 | 236456.22 | +0.000 | 0/30 | 0/30 | 0 / 0 |
| lifli 90 TL/kg | ATO | 236456.22 | 236456.22 | +0.000 | 236456.22 | 236456.22 | +0.000 | 0/30 | 0/30 | 0 / 0 |
| lifli 0 TL/kg | YAKO | 235917.55 | 237201.74 | +0.544 | 234012.58 | 234655.82 | +0.275 | 30/30 | 30/30 | 220 / 227 |
| lifli 0 TL/kg | BTO | 233944.06 | 234225.78 | +0.120 | 233916.61 | 234190.76 | +0.117 | 30/30 | 30/30 | 248 / 256 |
| lifli 0 TL/kg | ATO | 233916.61 | 234190.76 | +0.117 | 233916.61 | 234190.76 | +0.117 | 30/30 | 30/30 | 248 / 256 |

## 2. Lif kararı (ön kayıt kural 4 göstergeleri) ve senaryo farkı

| Yöntem | Lifsiz → 0 TL/kg medyan farkı K2 (%) | K4 (%) | Lifsiz → 90 TL/kg medyan farkı K2 (%) | K4 (%) | 0 TL/kg en sık kiriş deseni K2 | K4 |
|---|---|---|---|---|---|---|
| YAKO | -0.664 | +0.189 | +1.466 | +1.691 | B_S1 250x500 4D/40; B_S2 250x500 5D/35; B_S3 250x500 4D/40 (3/30) | B_S1 250x500 4D/40; B_S2 250x500 5D/35; B_S3 250x500 5D/35 (2/30) |
| BTO | -1.065 | -0.943 | +0.003 | +0.005 | B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 (18/30) | B_S1 250x550 4D/40; B_S2 250x500 4D/40; B_S3 250x550 4D/40 (21/30) |
| ATO | -1.074 | -0.958 | +0.000 | +0.000 | B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 (27/30) | B_S1 250x550 4D/40; B_S2 250x500 4D/40; B_S3 250x550 4D/40 (23/30) |

## 3. Hücre en düşük etkin tasarımları (BTO/ATO/YAKO birleşik; ilk dosya)

| Hücre | K2 en düşük (TL) ve etkin tasarım | K4 en düşük (TL) ve etkin tasarım |
|---|---|---|
| lifsiz | 236456.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`plain/EX2_ATO_plain_g9_fr1.00_fp90_run02.csv`) | 236456.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`plain/EX2_ATO_plain_g9_fr1.00_fp90_run01.csv`) |
| lifli 90 TL/kg | 236456.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv`) | 236456.22 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 300x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550; B_S2 250x600; B_S3 250x600 (`fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv`) |
| lifli 0 TL/kg | 233916.61 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x500 4D/40; B_S2 250x550 4D/40; B_S3 250x500 4D/40 (`fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv`) | 234190.76 — C_Edge_S1 250x300; C_Edge_S2 250x300; C_Edge_S3 250x300; C_Inter_S1 300x400; C_Inter_S2 300x300; C_Inter_S3 250x300; B_S1 250x550 4D/40; B_S2 250x500 4D/40; B_S3 250x550 4D/40 (`fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run03.csv`) |

