# Ek materyal (Supplementary Material) dizini — test, doğrulama ve duyarlılık sonuçları

**Durum:** taslak dizin (2026-11-19, Claude Code). IA kararı (2026-11-19): "yaptığımız tüm test sonuçları ek materyalde sunulabilir". Buradaki hiçbir sonuç, Critic + IA onayı, V3 ETABS ve S2 kapıları geçilmeden ana metnin bulgular bölümüne girmez; ek materyalde yer alması da hakem/IA onayı ister. Yasak iddialar: "lif pahalı/ekonomik değil", "optimum", "MaxFE yeterli", algoritma üstünlüğü, "benchmark", "Duysak ile doğrulandı".

Tüm yollar proje köküne göredir. Her koşu dosyası program sürümünü (DLL SHA-256) başlığında taşır.

## A. Birim ve regresyon testleri
| Konu | Dosya | Sonuç |
|---|---|---|
| Ö2 model birim testleri (T1–T27) | `03_Code_Execution/ex2_work/Ex2Model.Tests/Program.vb` (çalıştır: `Ex2Model.Tests.exe`) | 129/129 geçti (2026-11-19) |
| PCR-23 bit-bit regresyon (lifsiz yol) | `03_Code_Execution/ex2_work/regression_pcr23/` (`regression_vs_campaign1.txt`) | 9/9 |
| Ö1 DLL regresyonu (Faz 2 `a2b418c2` ↔ Faz 3 `c1a95951`) | `04_Results/regression_dll/` | bkz. dosya |
| Ö1 bayat-lcs denetimi | `04_Results/ex2/o1_required_audit.md`, `.csv` | etki küçük; Ö1 dondurulmuş |

## B. ETABS doğrulaması (V1, V2-a)
| Konu | Dosya |
|---|---|
| Ö2 V1/V2-a raporu (CD1–CD3, B1, B2, ACI 6.2.6, BULGU-1/2) | `03_Code_Execution/EX2_VALIDATION_REPORT.md` |
| ETABS ham çıktıları ve modeller | `04_Results/ex2/v1_v2a_etabs/`, `04_Results/ex2/v2a_etabs_rerun/` (ASCII yoldan yeniden koşu) |
| Pilot koşular | `04_Results/ex2/v2a_pilot/`, `v2a_pilot2/` |
| Ö1 doğrulama raporu | `04_Results/VALIDATION_REPORT.md` |

## C. Ö2 optimizasyon kampanyaları (270 koşu / kampanya; 30 tohum; 10 000 FE)
| Kampanya | Kural seti | Yol | İstatistik |
|---|---|---|---|
| 1 | ön kayıtlı (PCR-14…22) | `04_Results/ex2/campaign/` | `04_Results/ex2_stats.md`, `ex2_analysis.md` |
| 2 | + PCR-23/24/25 (tutarlı lcs, kiriş eksenel çekme, desenli yük) | `04_Results/ex2/campaign2/` | `04_Results/ex2_stats_campaign2.md`, `ex2_analysis_campaign2.md` |
| 3 | + PCR-26 (g5: tüm-yüklü ℓ/480) | `04_Results/ex2/campaign3/` | `04_Results/ex2_stats_campaign3.md` — **sonuç:** en iyiler kampanya 2 ile aynı, desenli aşım sürdü |
| 4 | + PCR-26b (g5: desenli yük, Ie zarfı) | `04_Results/ex2/campaign4/` (bitti 2026-11-19 16:08; 270/270) | `04_Results/ex2_stats_campaign4.md` — en iyiler: lifsiz 236 456,22; lifli 90 TL/kg 236 456,22; lifli 0 TL/kg 234 190,76 TL (lifsize göre %0,958; ℓ/480 desenli altında) |

## D. Ö2 deterministik araçlar ve duyarlılık (`04_Results/ex2/analysis2/`, 61 dosya + SHA256SUMS)
Komşuluk taraması (tek ve iki koordinat; yalnız çapraz teyit), desenli sehim (`*_deflection_patterned.txt`; F0 ℓ/480 oranı 1,0379), marjinal maliyet tablosu (ana format / MC2010-yerel / Ö1 formülü / fR ×0,6–1,2), lif başa-baş (break-even) tablosu (`04_Results/fibre_breakeven/`, alt sınır ≈ 8,1–15,7 TL/kg), fiyat=0 kontrolü (`04_Results/ex2/price0_check/`), PCR-23/24/25 yeniden değerlendirme (`04_Results/ex2/reeval_pcr23_25/reeval_summary.md`). Araçlar: `03_Code_Execution/tools/ex2_*`, `fibre_breakeven.ps1`, `price0_summary.py`.

Kampanya 4 deterministik analizi (PCR-26b; `04_Results/ex2/analysis4/` + SHA256SUMS_analysis4.txt, özet `04_Results/ex2_analysis_campaign4.md`): evalonly, tek/iki koordinatlı tarama, desenli ℓ/480 (zarf ve durum-başına Ie; hepsi ≤ 1, F0 0,9847), marjinal maliyet, lif sıfırlama, fR/format/fiyat duyarlılığı, başabaş alt sınırı (fR ×1,0: 9,81 TL/kg), kampanya 2↔4 ve 3↔4 karşılaştırması. Araçlar: `tools/ex2_analysis4_run.sh`, `ex2_analysis4_extra.ps1`, `ex2_compare_campaigns_c4.py`.

Hakem turu k4 sonrası duyarlılıklar (2026-11-19; `04_Results/ex2/analysis4/sensitivity_defl/` + SHA256SUMS_hakem_k4.txt; özet `04_Results/ex2_analysis_campaign4.md` §12): F0/P/F0Z/F0_c2 için ℓ/480 oranı varsayım duyarlılığı (net açıklık, ψ = 0,25/0,5, ani D, Bischoff, durum-başına Ie, Ie = Ig sınırı), kod ↔ araç eşleşmesi (4/4, fark 0), R8 gravite yükü ×0,95/1,00/1,05 ve kritik çarpanlar (ACI 6.2.6: P ×1,0375, F0 ×1,0467), lcs 0,2 h alt sınırı (F0'da etkin değil), başabaş kaba ölçeği (m_min 40 kg → 56,64 TL/kg; kanıtlanmış üst sınır değil). Regresyon: `04_Results/ex2/regression_pcr26/` (K3/K4 ikilileri `--deflcheck 0` ile K2'yi 54/54 dosyada bit-bit üretir). Kanıt satırları: `01_Literature/CODE_PROVISIONS_EVIDENCE.md` §2c (A62 ACI R26.4.2.1(a)(11) PDF 460; A63 MC2010 Denk. 5.6-8). Araçlar: `03_Code_Execution/tools/ex2_analysis4_hakem_k4.ps1`, `regression_pcr26.sh`. Hakem raporları: `06_Review_Audit/REVIEW_LOG/Review_Round_Ex2_k4_ic.md`, `_dis.md`.

**Konumlandırma kararı (IA 2026-11-19, seçenek 1):** 0 TL/kg lifli sonucu (K4 234 190,76 TL) ve F0 tasarımı yalnız ek materyalde yer alır; yanında sehim-varsayımı tablosu (`sensitivity_defl/`, analiz §12) zorunlu. Ana metin: lifsiz P + 90 TL/kg lifli (lif BTO/ATO 0/60, YAKO 4/30) + başabaş alt sınırı.

## E. Ö1 (3B, 116 eleman) kampanyaları
`04_Results/campaign_stats.md` (Faz 0), `ablation_stats.md` (Faz 1, 720 koşu), `phase2_stats.md` (Faz 2, 540), `phase3_stats.md` (Faz 3/3b/3c; BTO/ATO tek-boyut ↔ tüm-boyut), `04_Results/phase3/fibre_price0/` (fiyat=0), `S2_SFRC_comparison_raw.txt` (SFRC deney kıyası; sonuçsuz).

## F. Hakem ve denetim kayıtları
`06_Review_Audit/REVIEW_LOG/` (Review_Round_Ex2_spec/oneriler/acik/sonuc/k2 × ic/dis; Faz 3 turları). Kod şartname kanıtları: `01_Literature/CODE_PROVISIONS_EVIDENCE.md` (A1–A61).

## G. Ana metin için ayrılacak yöntem notları
PCR-12 (BTO/ATO tek-boyut) ek materyalde post-hoc etiketli; ana seri tüm-boyut. Ö1 koşu ve ikili sürümleri: `00_System/PROGRAM_REFERENCE.md`.
