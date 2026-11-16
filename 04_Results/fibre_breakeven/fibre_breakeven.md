# Lif fiyatı = 0 deterministik kırılma tablosu

- DLL: `phase3_bin/FemRcModel.dll` SHA-256 = `c1a95951560715777d137be63ebf313102f69dd6e772265eb42ff850b228f5a9`
- Tarih: 2026-11-16 10:03 ; betik: `03_Code_Execution/tools/fibre_breakeven.ps1` (Top=30, fR olcekleri 0.8/1/1.2)
- Yöntem: kesit vektörü sabit; 4 kiriş grubunun lif tipi (4D/5D) ve doz indeksi (1..5 = 20, 25, 30, 35, 40 kg/m3; sınıfa göre ızgaraya onarılır) değişir; lif fiyatı 0 TL/kg; `--derive 1` modeli (Ex1Problem useFibre=true, deriveRho=true); optimizasyon yok.
- Δ = (toplam - lif ücreti) - lifsiz yeniden-değerlendirme maliyeti (TL). Kırılma fiyatı = -Δ/fibreKg (Δ<0), aksi halde `<=0`.
- BTO plain_A: 30 CSV, T=0 count 26, selected 26
- ATO plain_A: 30 CSV, T=0 count 30, selected 30
- Kaynak dosyalar (en düşük maliyetli T=0 lifsiz A tasarımları, `04_Results/phase3/<yöntem>_alldim/plain_A/`):
  - BTO : BTO_plain_fr1.00_run29.csv, BTO_plain_fr1.00_run21.csv, BTO_plain_fr1.00_run12.csv, BTO_plain_fr1.00_run07.csv, BTO_plain_fr1.00_run16.csv, BTO_plain_fr1.00_run11.csv, BTO_plain_fr1.00_run08.csv, BTO_plain_fr1.00_run05.csv, BTO_plain_fr1.00_run15.csv, BTO_plain_fr1.00_run30.csv, BTO_plain_fr1.00_run25.csv, BTO_plain_fr1.00_run14.csv, BTO_plain_fr1.00_run03.csv, BTO_plain_fr1.00_run28.csv, BTO_plain_fr1.00_run22.csv, BTO_plain_fr1.00_run01.csv, BTO_plain_fr1.00_run09.csv, BTO_plain_fr1.00_run27.csv, BTO_plain_fr1.00_run13.csv, BTO_plain_fr1.00_run23.csv, BTO_plain_fr1.00_run26.csv, BTO_plain_fr1.00_run10.csv, BTO_plain_fr1.00_run18.csv, BTO_plain_fr1.00_run19.csv, BTO_plain_fr1.00_run20.csv, BTO_plain_fr1.00_run02.csv
  - ATO : ATO_plain_fr1.00_run22.csv, ATO_plain_fr1.00_run11.csv, ATO_plain_fr1.00_run05.csv, ATO_plain_fr1.00_run29.csv, ATO_plain_fr1.00_run19.csv, ATO_plain_fr1.00_run13.csv, ATO_plain_fr1.00_run06.csv, ATO_plain_fr1.00_run21.csv, ATO_plain_fr1.00_run18.csv, ATO_plain_fr1.00_run12.csv, ATO_plain_fr1.00_run30.csv, ATO_plain_fr1.00_run16.csv, ATO_plain_fr1.00_run28.csv, ATO_plain_fr1.00_run04.csv, ATO_plain_fr1.00_run02.csv, ATO_plain_fr1.00_run26.csv, ATO_plain_fr1.00_run23.csv, ATO_plain_fr1.00_run25.csv, ATO_plain_fr1.00_run27.csv, ATO_plain_fr1.00_run10.csv, ATO_plain_fr1.00_run09.csv, ATO_plain_fr1.00_run14.csv, ATO_plain_fr1.00_run15.csv, ATO_plain_fr1.00_run24.csv, ATO_plain_fr1.00_run17.csv, ATO_plain_fr1.00_run01.csv, ATO_plain_fr1.00_run03.csv, ATO_plain_fr1.00_run07.csv, ATO_plain_fr1.00_run08.csv, ATO_plain_fr1.00_run20.csv

## Özet (lif ücreti 0 TL/kg)

| Yöntem | Konfig. sayısı | T=0 konfig. | Δ<0 konfig. | Δ lif-dışı (TL) min/med/maks | Δ toplam (TL) min/med/maks | Δ çelik (kg) min/med/maks | Kırılma fiyatı (Δ<0 olanlar) |
|---|---|---|---|---|---|---|---|
| BTO | 780 | 780 | 149 | min -4,448 / medyan 15,016 / maks 21,164 | min -4,448 / medyan 15,016 / maks 21,164 | min -65 / medyan 216 / maks 304 | min 884 / medyan 2,054 / maks 4,337 (TL/ton) |
| ATO | 900 | 900 | 404 | min -10,814 / medyan 10,199 / maks 20,991 | min -10,814 / medyan 10,199 / maks 20,991 | min -157 / medyan 147 / maks 302 | min 671 / medyan 2,129 / maks 8,022 (TL/ton) |

## Tasarım başına en iyi konfigürasyon (Δ lif-dışı en küçük, T=0 tercihli)

| Yöntem | Kaynak | Lifsiz maliyet (TL) | Lifsiz çelik (kg) | Tip / doz idx / fR | T | fibreKg | Δ lif-dışı (TL) | Δ toplam (TL) | Δ çelik (kg) | Kırılma fiyatı (TL/kg) |
|---|---|---|---|---|---|---|---|---|---|---|
| BTO | BTO_plain_fr1.00_run29.csv | 1,386,277 | 10,026 | 4D / 1 / 0.8 | 0.000 | 794 | -1,023 | -1,023 | -15 | 1.2884 |
| BTO | BTO_plain_fr1.00_run21.csv | 1,391,279 | 10,045 | 5D / 4 / 1 | 0.000 | 1,383 | 14,771 | 14,771 | 212 | <=0 |
| BTO | BTO_plain_fr1.00_run12.csv | 1,392,240 | 10,264 | 4D / 1 / 0.8 | 0.000 | 792 | -1,999 | -1,999 | -29 | 2.5250 |
| BTO | BTO_plain_fr1.00_run07.csv | 1,394,415 | 10,235 | 4D / 1 / 1 | 0.000 | 771 | -2,213 | -2,213 | -32 | 2.8722 |
| BTO | BTO_plain_fr1.00_run16.csv | 1,395,322 | 10,008 | 5D / 1 / 0.8 | 0.000 | 1,017 | -1,440 | -1,440 | -21 | 1.4165 |
| BTO | BTO_plain_fr1.00_run11.csv | 1,395,521 | 10,183 | 5D / 1 / 0.8 | 0.000 | 988 | -2,244 | -2,244 | -33 | 2.2718 |
| BTO | BTO_plain_fr1.00_run08.csv | 1,396,748 | 10,088 | 4D / 1 / 0.8 | 0.000 | 790 | -1,090 | -1,090 | -16 | 1.3792 |
| BTO | BTO_plain_fr1.00_run05.csv | 1,396,762 | 10,276 | 4D / 1 / 0.8 | 0.000 | 794 | -2,001 | -2,001 | -29 | 2.5210 |
| BTO | BTO_plain_fr1.00_run15.csv | 1,397,377 | 10,096 | 4D / 3 / 1.2 | 0.000 | 1,214 | 15,223 | 15,223 | 219 | <=0 |
| BTO | BTO_plain_fr1.00_run30.csv | 1,397,985 | 9,968 | 5D / 4 / 1.2 | 0.000 | 1,417 | 15,259 | 15,259 | 219 | <=0 |
| BTO | BTO_plain_fr1.00_run25.csv | 1,399,600 | 10,150 | 5D / 1 / 0.8 | 0.000 | 990 | -1,738 | -1,738 | -25 | 1.7557 |
| BTO | BTO_plain_fr1.00_run14.csv | 1,399,844 | 10,034 | 4D / 1 / 0.8 | 0.000 | 808 | -958 | -958 | -14 | 1.1863 |
| BTO | BTO_plain_fr1.00_run03.csv | 1,400,316 | 10,344 | 5D / 1 / 0.8 | 0.000 | 965 | -2,732 | -2,732 | -40 | 2.8320 |
| BTO | BTO_plain_fr1.00_run28.csv | 1,400,374 | 10,196 | 5D / 4 / 0.8 | 0.000 | 1,386 | -2,722 | -2,722 | -40 | 1.9633 |
| BTO | BTO_plain_fr1.00_run22.csv | 1,400,445 | 10,270 | 4D / 1 / 1 | 0.000 | 770 | -2,214 | -2,214 | -32 | 2.8736 |
| BTO | BTO_plain_fr1.00_run01.csv | 1,401,928 | 10,282 | 5D / 4 / 1.2 | 0.000 | 1,353 | 12,229 | 12,229 | 175 | <=0 |
| BTO | BTO_plain_fr1.00_run09.csv | 1,402,554 | 10,176 | 5D / 3 / 0.8 | 0.000 | 1,218 | -1,991 | -1,991 | -29 | 1.6354 |
| BTO | BTO_plain_fr1.00_run27.csv | 1,403,210 | 10,389 | 4D / 5 / 1 | 0.000 | 1,386 | 13,337 | 13,337 | 191 | <=0 |
| BTO | BTO_plain_fr1.00_run13.csv | 1,403,828 | 10,333 | 4D / 2 / 1.2 | 0.000 | 963 | -3,561 | -3,561 | -52 | 3.6980 |
| BTO | BTO_plain_fr1.00_run23.csv | 1,403,932 | 10,270 | 4D / 3 / 0.8 | 0.000 | 1,151 | -2,694 | -2,694 | -39 | 2.3396 |
| BTO | BTO_plain_fr1.00_run26.csv | 1,404,056 | 10,094 | 5D / 4 / 1.2 | 0.000 | 1,420 | 15,371 | 15,371 | 221 | <=0 |
| BTO | BTO_plain_fr1.00_run10.csv | 1,405,029 | 10,256 | 5D / 4 / 0.8 | 0.000 | 1,420 | -2,736 | -2,736 | -40 | 1.9272 |
| BTO | BTO_plain_fr1.00_run18.csv | 1,405,211 | 10,296 | 5D / 5 / 1 | 0.000 | 1,343 | -4,448 | -4,448 | -65 | 3.3125 |
| BTO | BTO_plain_fr1.00_run19.csv | 1,407,026 | 10,272 | 4D / 1 / 0.8 | 0.000 | 769 | -1,092 | -1,092 | -16 | 1.4193 |
| BTO | BTO_plain_fr1.00_run20.csv | 1,417,605 | 10,140 | 5D / 4 / 1.2 | 0.000 | 1,452 | -2,277 | -2,277 | -33 | 1.5688 |
| BTO | BTO_plain_fr1.00_run02.csv | 1,419,445 | 10,219 | 5D / 2 / 0.8 | 0.000 | 1,013 | -1,386 | -1,386 | -20 | 1.3689 |
| ATO | ATO_plain_fr1.00_run22.csv | 1,404,368 | 10,206 | 4D / 1 / 0.8 | 0.000 | 811 | -1,034 | -1,034 | -15 | 1.2747 |
| ATO | ATO_plain_fr1.00_run11.csv | 1,407,103 | 10,336 | 4D / 1 / 0.8 | 0.000 | 771 | -1,741 | -1,741 | -25 | 2.2589 |
| ATO | ATO_plain_fr1.00_run05.csv | 1,408,852 | 10,170 | 4D / 1 / 0.8 | 0.000 | 812 | -1,026 | -1,026 | -15 | 1.2635 |
| ATO | ATO_plain_fr1.00_run29.csv | 1,412,814 | 10,260 | 5D / 5 / 1 | 0.000 | 1,455 | 14,376 | 14,376 | 207 | <=0 |
| ATO | ATO_plain_fr1.00_run19.csv | 1,416,141 | 10,393 | 5D / 1 / 0.8 | 0.000 | 987 | -2,738 | -2,738 | -40 | 2.7755 |
| ATO | ATO_plain_fr1.00_run13.csv | 1,417,839 | 10,460 | 4D / 1 / 0.8 | 0.000 | 791 | -1,978 | -1,978 | -29 | 2.5002 |
| ATO | ATO_plain_fr1.00_run06.csv | 1,418,170 | 10,525 | 4D / 1 / 0.8 | 0.000 | 770 | -2,346 | -2,346 | -34 | 3.0483 |
| ATO | ATO_plain_fr1.00_run21.csv | 1,418,721 | 10,430 | 4D / 3 / 0.8 | 0.000 | 1,210 | -3,269 | -3,269 | -47 | 2.7021 |
| ATO | ATO_plain_fr1.00_run18.csv | 1,418,913 | 10,337 | 5D / 4 / 1 | 0.000 | 1,466 | 14,902 | 14,902 | 214 | <=0 |
| ATO | ATO_plain_fr1.00_run12.csv | 1,419,420 | 10,275 | 5D / 1 / 0.8 | 0.000 | 1,008 | -2,065 | -2,065 | -30 | 2.0479 |
| ATO | ATO_plain_fr1.00_run30.csv | 1,420,497 | 10,217 | 4D / 5 / 1.2 | 0.000 | 1,497 | -3,727 | -3,727 | -54 | 2.4906 |
| ATO | ATO_plain_fr1.00_run16.csv | 1,420,634 | 10,357 | 4D / 5 / 1.2 | 0.000 | 1,516 | -4,951 | -4,951 | -72 | 3.2667 |
| ATO | ATO_plain_fr1.00_run28.csv | 1,421,872 | 10,264 | 4D / 5 / 0.8 | 0.000 | 1,609 | -2,120 | -2,120 | -31 | 1.3178 |
| ATO | ATO_plain_fr1.00_run04.csv | 1,427,526 | 10,351 | 5D / 1 / 0.8 | 0.000 | 1,012 | -1,522 | -1,522 | -22 | 1.5038 |
| ATO | ATO_plain_fr1.00_run02.csv | 1,427,578 | 10,325 | 4D / 5 / 1.2 | 0.000 | 1,414 | -2,939 | -2,939 | -43 | 2.0786 |
| ATO | ATO_plain_fr1.00_run26.csv | 1,428,274 | 10,146 | 5D / 3 / 1.2 | 0.000 | 1,268 | -2,002 | -2,002 | -29 | 1.5794 |
| ATO | ATO_plain_fr1.00_run23.csv | 1,429,820 | 10,368 | 5D / 4 / 1 | 0.000 | 1,378 | -2,924 | -2,924 | -42 | 2.1212 |
| ATO | ATO_plain_fr1.00_run25.csv | 1,430,472 | 10,663 | 4D / 5 / 1.2 | 0.000 | 1,534 | -10,814 | -10,814 | -157 | 7.0483 |
| ATO | ATO_plain_fr1.00_run27.csv | 1,430,840 | 10,593 | 5D / 1 / 0.8 | 0.000 | 935 | -3,474 | -3,474 | -50 | 3.7163 |
| ATO | ATO_plain_fr1.00_run10.csv | 1,431,244 | 10,555 | 5D / 4 / 0.8 | 0.000 | 1,379 | -3,543 | -3,543 | -51 | 2.5688 |
| ATO | ATO_plain_fr1.00_run09.csv | 1,431,743 | 10,330 | 5D / 3 / 1 | 0.000 | 1,226 | -2,929 | -2,929 | -43 | 2.3886 |
| ATO | ATO_plain_fr1.00_run14.csv | 1,431,749 | 10,386 | 5D / 2 / 1 | 0.000 | 1,007 | -2,334 | -2,334 | -34 | 2.3182 |
| ATO | ATO_plain_fr1.00_run15.csv | 1,433,016 | 10,546 | 4D / 5 / 1.2 | 0.000 | 1,436 | -7,471 | -7,471 | -108 | 5.2039 |
| ATO | ATO_plain_fr1.00_run24.csv | 1,436,590 | 10,422 | 4D / 3 / 1.2 | 0.000 | 1,181 | -3,158 | -3,158 | -46 | 2.6747 |
| ATO | ATO_plain_fr1.00_run17.csv | 1,437,851 | 10,308 | 5D / 1 / 1.2 | 0.000 | 1,056 | -1,282 | -1,282 | -19 | 1.2144 |
| ATO | ATO_plain_fr1.00_run01.csv | 1,441,390 | 10,403 | 4D / 2 / 1.2 | 0.000 | 1,044 | -1,662 | -1,662 | -24 | 1.5914 |
| ATO | ATO_plain_fr1.00_run03.csv | 1,441,911 | 10,543 | 4D / 5 / 1.2 | 0.000 | 1,654 | 8,868 | 8,868 | 128 | <=0 |
| ATO | ATO_plain_fr1.00_run07.csv | 1,452,743 | 10,757 | 4D / 4 / 1.2 | 0.000 | 1,464 | -6,625 | -6,625 | -96 | 4.5244 |
| ATO | ATO_plain_fr1.00_run08.csv | 1,454,981 | 10,587 | 4D / 2 / 1 | 0.000 | 984 | -2,008 | -2,008 | -29 | 2.0414 |
| ATO | ATO_plain_fr1.00_run20.csv | 1,457,536 | 10,760 | 5D / 4 / 0.8 | 0.000 | 1,307 | -3,059 | -3,059 | -44 | 2.3412 |

## Ek sayımlar

- BTO : Δ toplam > 0 olan konfig. 631 / 780; en küçük Δ toplam -4,448 TL; en küçük Δ lif-dışı -4,448 TL; T>0 konfig. 0
- ATO : Δ toplam > 0 olan konfig. 496 / 900; en küçük Δ toplam -10,814 TL; en küçük Δ lif-dışı -10,814 TL; T>0 konfig. 0

Sınırlamalar: kesit vektörü her koşunun Fpen-en-iyi x değeri (T=0 olanlar); kesitler lifli koşullara yeniden optimize edilmez (b0 ayrı); tek fR kaynağı, fR ölçeği tablo değerlerine çarpan; lif katkısı sarılma bölgesinde 0, Mpr/VRd,F varsayımları modele aittir.
