# EX2_VALIDATION_REPORT.md — Ö2 V1 ve V2-a (FEM ↔ ETABS 22)

Tarih: 2026-11-16 · Yazan: Claude Code (Developer Agent) · Spec: `EX2_SPEC.md` v2.1 §10, `VALIDATION_SPEC.md` V2 alt kapıları.
**Bulgular bölümüne girmez** (doğrulama kaydı). Git kullanılmadı. `program_work/`, `phase2_bin/`, `phase3_bin/`, `ex2_work/ref/` ve mevcut `ex2_work` kaynakları değiştirilmedi; Ex2 kodunda bulunan hatalar `EX2_CODE_LOG.md` §5d'ye **BULGU** olarak yazıldı.

## 0. Ortam ve girdiler

| Kalem | Değer |
|---|---|
| ETABS | 22.6.0 (build 22.6.0.4035), OAPI `ETABSv1.dll`; iş sonunda kapatıldı (`ApplicationExit`) |
| Yeni araç | `03_Code_Execution/ex2_work/EtabsEx2/` (`Program.vb` SHA-256 `9395c0cd…5716`, `summarize.py` `531f479f…ecc3`, `EtabsEx2.exe` `7e6d29ac…b0b0`). `Ex2Model.dll` ve donmuş DLL'lere **dosya referansı**; `program_work/EtabsProbe` yalnız okundu, OAPI kalıpları uyarlandı |
| DLL SHA-256 (koşu günlüğü) | Ex2Model `b30a2aad…e998` (pilot ile aynı), FemCore `8da9dfd2…c0c8`, RcCore `14f84b75…beef`, FemRcModel `c1a95951…f5a9` |
| Tasarımlar | **T1** = kaynak Tablo 1 kesitleri (5 grup; yalnız kuvvet/reaksiyon); **PILOT** = Q20 pilotu (`04_Results/ex2/v2a_pilot/v2a_design_x.txt`, 239 057,11 TL, T = 0); **K1** = kontrol tasarımı: pilot kolonları + kirişler 250×500 (`04_Results/ex2/v1_v2a_etabs/k1_design_x.txt`, `--evalonly` ile 240 522,97 TL, T = 0, etriye aralığını **dayanım** belirliyor, s = 203,7 mm) |
| ETABS modeli | 2B, XZ düzlemi; taban ankastre; diğer düğümlerde U2, R1, R3 tutulu (FEM ile aynı); **rijit bölge yok** (`SetEndLengthOffset(False, 0, 0, 0)`); kesit `SetRectangle(T3 = h, T2 = b)`; C30, E = 4700√30 MPa, ν = 0,2, γ = 25 kN/m³; desenler SW (öz ağırlık çarpanı 1), G (0), Q (0); U1 = 1,4(SW + G), U2 = 1,2(SW + G) + 1,6Q; modal kapalı |
| ETABS tasarım | ACI 318-14; yalnız U1 ve U2; kolon donatısı programdaki adet ve yerleşimle (Ø20, NR3/NR2 = `PerimeterCounts`, net örtü 40 mm, etriye Ø10 → ağırlık merkezi 60 mm), **kontrol** modu; kiriş örtüsü ağırlık merkezine 60 mm (d = h − 60). **Hareketli yük düzenleme katsayısı (PatLLF, tercih 16) = 0** ([K b]: ETABS'te de tek desen); framing NonSway (4); kolonlarda lu oranı 1, K = 1, Cm = 1 (program PCR-2 ile aynı), δ program-belirlemeli |
| Ham çıktılar | `04_Results/ex2/v1_v2a_etabs/raw/` (istasyon kuvvetleri, uç kuvvetleri, reaksiyonlar, geometri, kiriş istasyon/grup, kolon koşuları A/B/C/D/S, ETABS tasarım tabloları `tables_*`, `console.txt`, `etabs_ex2_log.txt`); özetler `summary_*.csv`, `summary_stdout.txt`; modeller `edb/ex2_T1.EDB`, `ex2_PILOT.EDB`, `ex2_K1.EDB` |

## 1. Kapı durumları

| Kapı | Ölçüt (kaynak) | Sonuç | Durum |
|---|---|---|---|
| **V1 (a)** geometri | düğüm/eleman, uzunluk, mesnet, yönelim (§1.1) | 16 düğüm, 21 eleman; koordinat farkı ≤ 1,3×10⁻¹⁵ m; ETABS T3 = h, T2 = b, kolon yerel 2 = global X (düzlem içi) | **GEÇTİ** |
| **V1 (b, c)** reaksiyon | FEM ↔ ETABS ≤ %0,1; T1'de U2 = 3 506,010 kN | T1: SW 201,675; G 1 440,000; Q 960,000; U1 2 298,345; **U2 3 506,010 kN** (ETABS = FEM = el hesabı; fark 0,0000 %). PILOT U2 3 580,305; K1 U2 3 562,305 kN (fark 0,0000 %) | **GEÇTİ** |
| V1 (d) tez EDB | bilgi | §5 | bilgi |
| **V2-a kuvvet (FE4)** | statik ≤ %0,1 | 3 tasarım × 5 durum × {kolon, kiriş} × {N, V2, M3}, tüm istasyonlar: en büyük fark %9,7×10⁻⁴ (yuvarlama; T1 SW kiriş V2); uçlarda bağıl fark 0 | **GEÇTİ** |
| **V2-a CD1** | ETABS D/C > 1 veya O/S iken program ≤ 1 olan kolon = 0 | PILOT ve K1'de **2** (E9, E10 = C_Inter_S2): ETABS "O/S — Delta_ns exceeds 1.4 (ACI 318-14 6.2.6; 6.6.4.5.1)"; program D/C 0,974 / 0,984 (uygun) | **KALDI** (BULGU-1) |
| V2-a CD2 | program/ETABS ≥ 0,95 (O/S dışı) | üretim modu (kat Q sınıflaması) 0,981–1,140 (PILOT), 0,980–1,146 (K1); büyütmesiz (C) 0,992–1,115 / 0,992–1,134 | GEÇTİ |
| V2-a CD3 | güvensiz yönde ≤ %5 | maks. D/C ETABS 0,788 / 0,896, program 0,974 / 0,984 → güvenli yönde | GEÇTİ |
| **V2-a B1** | gerekli As program/ETABS 0,98–1,10 | üst 0,982–1,000; **alt 0,966–1,000** (S1, S2 grupları 0,966–0,971) | **KISMEN** (BULGU-2: kiriş eksenel çekmesi) |
| **V2-a B2** | gravite Av/s 0,98–1,02 | kritik kesitte (x = d) 0,965 (PILOT), 0,969 (K1); ETABS sabitleri programa uygulandığında **1,0000**; düğümde 0,708 / 0,751 | **KISMEN** (fark ETABS'in US sabitleri ve kesit konumu; §3) |
| **V2-a toplam** | | kuvvetler birebir; P-M + e_min, eğilme ve kesme formülleri ETABS ile açıklanabilir biçimde tutarlı; **ACI 6.2.6 (δ ≤ 1,4) sınırı programda yok** → ETABS'in yetersiz saydığı kolon program tarafından uygun sayılıyor | **KALDI** |

Kaynak makale kesitleriyle (T1) D/C karşılaştırması yapılmadı (spec §10 V2-a, [K e]); T1 yalnız V1 ve kuvvetler için kullanıldı.

## 2. Kuvvetler (V1 b, V2-a FE4)

| Tasarım | Durum | ETABS ΣFz (kN) | FEM (kN) | Fark |
|---|---|---|---|---|
| T1 | SW / G / Q | 201,675 / 1 440,000 / 960,000 | aynı | 0,0000 % |
| T1 | U1 / U2 | 2 298,345 / **3 506,010** | aynı | 0,0000 % |
| PILOT | U1 / U2 | 2 385,023 / 3 580,305 | aynı | 0,0000 % |
| K1 | U1 / U2 | 2 364,023 / 3 562,305 | aynı | 0,0000 % |

İstasyon kuvvetleri (kN, kN·m) `raw/*_station_forces.csv`, uçlar `raw/*_end_forces.csv`, özet `summary_V2a_forces.csv`. En büyük değerler (U2): T1 N 1 249,95 kN, V2 216,38 kN, M3 207,71 kN·m; PILOT N 1 286,65, V2 216,38, M3 210,08. Rijit bölgesiz modelde ETABS ve FemCore aynı işaret kuralını (N çekme +, M3 sarkma +) ve aynı istasyonları kullanır.

## 3. Kirişler (B1, B2) ve açık sorular

### 3.1 Sayısal tablo (ana koşu, PatLLF = 0; tüm değerler mm²/mm veya mm²)

| Tasarım | Grup | As,üst program / ETABS | B1 üst | As,alt program / ETABS | B1 alt | Av/s program (x = d) | ETABS x = d | B2 (d) | ETABS düğüm | B2 (düğüm) | Uyumlu sabitlerle B2 | Etriye belirleyici |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| PILOT | B_S1 | 1 237,1 / 1 255,5 | 0,985 | 646,2 / 666,4 | 0,970 | 0,6186 | 0,6411 | 0,965 | 0,8736 | 0,708 | 1,000 | smax (s = 245) |
| PILOT | B_S2 | 1 201,8 / 1 223,6 | 0,982 | 678,6 / 702,3 | 0,966 | 0,6186 | 0,6411 | 0,965 | 0,8736 | 0,708 | 1,000 | smax |
| PILOT | B_S3 | 1 204,7 / 1 204,7 | 1,000 | 675,9 / 675,9 | 1,000 | 0,6186 | 0,6411 | 0,965 | 0,8736 | 0,708 | 1,000 | smax |
| K1 | B_S1 | 1 406,0 / 1 424,7 | 0,987 | 722,3 / 743,6 | 0,971 | 0,7710 | 0,7958 | 0,969 | 1,0270 | 0,751 | 1,000 | **dayanım** (s = 203,7) |
| K1 | B_S2 | 1 374,4 / 1 397,1 | 0,984 | 750,4 / 775,9 | 0,967 | 0,7710 | 0,7958 | 0,969 | 1,0270 | 0,751 | 1,000 | dayanım |
| K1 | B_S3 | 1 377,8 / 1 377,8 | 1,000 | 747,4 / 747,4 | 1,000 | 0,7710 | 0,7958 | 0,969 | 1,0270 | 0,751 | 1,000 | dayanım |

### 3.2 ETABS'in Vc denklemi (AÇIK-18) — çözüldü
48 (PILOT) ve 54 (K1) istasyonda (Av/s > Av,min) Vu/φ = Vc + (Av/s)·fyt·d doğrusal uydurması artık ≤ 0,0012 kN ile tam tutar:
- **Vc = 111,426 kN** (PILOT) / 100,055 kN (K1) = **2√f′c[psi]·bw·d** (= 0,166√f′c[MPa]·bw·d). Program ACI 318M-14 22.5.5.1 SI biçimi 0,17√f′c bw d = 114,063 / 102,424 kN (ETABS'ten %2,3 büyük).
- **fyt = 413,69 MPa** (60 ksi; ETABS ACI 20.2.2.4 üst sınırını US biriminde uygular); program 420 MPa (ACI 318M sınırı).
- d = h − 60 (ETABS ile aynı); Av,min = 0,35 bw/fy (fy = 420 ile, ikisinde aynı); eksenel kuvvet kesmede kullanılmıyor (ETABS tercihi IgnorePu = Yes; aynı Vu'da basınçlı çatı ve çekmeli kat kirişleri aynı Av/s'yi veriyor); Vc ayrıntılı (Tablo 22.5.5.1) form değil.
- Sonuç: B2 farkı (≈ %3,5) tamamen bu iki sabitten gelir; ETABS sabitleri program formülüne konunca B2 = 1,0000. Program kesme formülünü ACI 318M (SI) katsayılarıyla uygular; ETABS biraz daha muhafazakârdır (yalnız SI/US dönüşüm farkı). (2026-11-17 notu: bu cümle yalnız kesme formülü içindir; tasarımların ACI 318-14'e bütünüyle uygun olduğu anlamına gelmez — kampanya 1 kural setinde desenli yük ve kiriş eksenel kuvveti yoktu, PCR-24/25.)

### 3.3 Kesme istasyonu ve kesit yüzü (AÇIK-14)
- ETABS **tüm çıktı istasyonlarında** (0,5 m aralık, x = 0 düğüm dahil) tasarlar ve ACI 9.4.3.2 "yüzden d" indirimi uygulamaz; rijit bölgesiz modelde ETABS'in "yüz"ü düğümdür → ETABS'in kiriş grubu en büyük Av/s'si düğümdedir (PILOT 0,8736; K1 1,0270).
- Program kritik kesiti **düğümden d** alır (IA kararı). Bu yüzden program/ETABS(düğüm) = 0,708 / 0,751 (kapı dışı); karşılaştırma ETABS'in x = d'ye doğrusal enterpolasyonuyla yapıldı (§3.1).
- ACI 9.4.3.2 kritik kesiti **mesnet yüzünden** d tanımlar; gerçek yüz düğümden hc/2 uzaktadır (pilotta hc = 300–350 mm → 150–175 mm). Program düğümden d kullandığı için gerçek kurala göre **muhafazakâr**dır (Vu yüzden d'de ≈ w·hc/2 = 72,1 kN/m × 0,15 ≈ 10,8 kN daha küçük olurdu). ETABS'le birebir eşleşme için ya ETABS'e rijit uç/uç ofseti verilmeli ya da ETABS düğüm değeri raporlanmalıdır; V3 Excel'i program kuralını (düğümden d) kullanmalıdır. AÇIK-14 kararı IA'da kalır; ETABS davranışı artık belgeli.

### 3.4 Eğilme (B1)
- **Üst**: ETABS gerekli üst donatısı FEM momentinden + N·(d − h/2) ile türetilmiş: çekmeli S1/S2 kirişlerinde (N = 13–18,7 kN çekme) ETABS momentin eksenel çekmeyi de içerdiği görülür (fark 3,9–4,1 kN·m → N(d − h/2) eklenince ≤ 0,57 kN·m). Basınçlı çatı kirişlerinde (N = −38…−47 kN) ETABS = FEM (basınç yok sayılıyor). B1 üst 0,982–1,000 → kapı içinde.
- **Alt**: aynı neden (çekme) alt donatıda 0,966–0,971 verir (S1, S2); çatıda 1,000. Program kiriş eksenel kuvvetini eğilmede ve kesmede (ACI 22.5.7.1) yok sayar → **BULGU-2** (küçük, güvensiz yönde %1,5–3,4 As).
- **ETABS varsayılanı PatLLF = 0,75** ile alt donatı oranı 0,63–0,67'ye düşer (ETABS kendi yaklaşık hareketli yük düzenlemesini ekler; açıklıkta sarkma momenti 114,5 → 174,8 kN·m, açıklık ortası kesmesi 0 → 108 kN). Spec [K b] gereği ETABS'te de kapatıldı; bu, desenli yük ihmalinin etkisinin (ETABS yaklaşımıyla) büyük olduğunu gösterir: alt donatı ≈ %50 artar. Bilgi; makalede sınırlama cümlesine kanıt olarak eklenebilir (`--patterns` aracıyla birlikte).
- SDC = B (varsayılan D yerine) hiçbir sonucu değiştirmedi (koşu S).

### 3.5 Pilotta smax notu ve kontrol tasarımı
Pilotta etriye aralığını smax = d/2 = 245 mm belirler; ancak **gerekli** Av/s dayanımdan gelir (0,6186 > Av,min 0,2083), bu yüzden B2 pilotta da dayanım formülünü sınar. Aralık kuralının dayanımla bağlayıcı olduğu durum için K1 kontrol tasarımı (kirişler 250×500, kod değiştirilmeden `--evalonly`) koşuldu: etriye belirleyicisi "strength", s = 203,7 mm < smax 220 mm; sonuç pilotla aynı (B2(d) 0,969; uyumlu sabitlerle 1,000). ETABS'in x = d gereksinimi / program sağlanan Av/s: PILOT 1,000, K1 1,032; ETABS gerekli As / program sağlanan As (eğilme D/C vekili): alt ≤ 1,035, üst ≤ 1,018 (S1, S2 grupları; eksenel çekme etkisi) (ETABS sabitleriyle K1 kirişleri kesmede %3 eksik kalır; ACI 318M ile D/C = 1,000).

## 4. Kolonlar (CD1–CD3) ve açık sorular

### 4.1 Koşular
| Koşu | ETABS ayarı | Program modu | PILOT CD1 / CD2 min / CD3 | K1 CD1 / CD2 min / CD3 |
|---|---|---|---|---|
| A | NonSway, lu oranı 1, K = 1, Cm = 1, δ program-belirlemeli | üretim (kat Q sınıflaması) | **2** / 0,981 / güvenli | **2** / 0,980 / güvenli |
| A | aynı | tümü ötelenmesiz | 2 / **0,940** / güvenli | 2 / 0,941 / güvenli |
| B | Q > 0,05 katlarında Sway Ordinary | üretim | A ile birebir aynı | A ile aynı |
| C | NonSway, δns = δs = 1 | büyütmesiz | **0** / 0,992 (maks 1,115) / güvenli | 0 / 0,992 (maks 1,134) / güvenli |
| D | ETABS varsayılanları (lu, K, Cm, δ) | tümü ötelenmesiz | 2 / 0,986 / güvenli | 2 / 0,991 / güvenli |
| S | A + SDC B | tümü ötelenmesiz | A ile aynı | A ile aynı |

Eleman bazında: `summary_V2a_columns.csv`, `raw/*_columns_*.csv` (ETABS durumu ve mesajı, programın kullandığı en büyük δ düzlem içi/dışı dahil).

### 4.2 Bulgular
- **P-M etkileşimi + e_min** (koşu C): program her kolonda ETABS'e eşit veya daha muhafazakâr (0,992–1,134). Formül düzeyinde doğrulandı.
- **ACI 6.2.6 (BULGU-1)**: "Unless slenderness effects are neglected as permitted by 6.2.5, … Mu including second-order effects shall not exceed 1.4Mu due to first-order effects." (ACI 318M-14, PDF 67 / basılı 65). C_Inter_S2 (250×300, E9/E10) düzlem dışı tutulu kabulde yalnız e_min momentiyle lu/r = 3 300/(0,3·250) = 44 > 22 (M1/M2 = 1) → narinlik ihmal edilemez; programın kendi δ değerleri **δ2 = 1,949** (PILOT; K1 1,929) ve üretim modunda düzlem içi δ3 = 1,51 (Q > 0,05 katı, sınır 22) → ikisi de 1,4'ü aşar. ETABS aynı δ'yı hesaplar (U1'de δ2 = 21,05/12,55 = 1,677 = program formülü, βdns = 1) ve kolonu O/S işaretler; program 1,4 sınırını kontrol etmediği için D/C 0,974 ile uygun sayar. Pilot ve K1 bu nedenle ACI'ye göre uygun **değildir**. Kod değiştirilmedi (`EX2_CODE_LOG` §5d).
- **Ötelenmesiz ihmal sınırı**: ETABS NonSway'de 6.2.5(b) muafiyetini uygulamaz, δns'yi hep hesaplar (Cm = 1 ile). Program muafiyeti uygular → "tümü ötelenmesiz" modda E2/E3 (C_Inter_S1) program/ETABS 0,940 (güvensiz yönde %6; CD2 < 0,95). Üretim modunda S1 katı Q = 0,076 > 0,05 olduğu için sınır 22 ve δ3 = 1,397 uygulanır → 0,981. Muafiyet ACI'ye uygundur; fark ETABS'in muhafazakâr basitleştirmesidir (bilgi).
- **Q > 0,05 (AÇIK-19)**: ETABS'te Sway Ordinary ile NonSway gravite kombinasyonlarında aynı sonucu verir (koşu B = A): ETABS δs'yi P-Δ analizine bırakır, δns'yi her iki tipte uygular. ETABS Q indeksini hesaplamaz; programın kat Q sınıflaması ETABS'le doğrulanamaz (yalnız etkisi: sway katında programın δ3'ü ETABS'e yaklaşır). Sway katlarında δs uygulanmaması program ve ETABS'te ortaktır (simetrik yükte M2s ≈ 0).
- **Kolon lu (AÇIK-15)**: ETABS'in program-belirlemeli "unbraced length ratio" değeri **1,0** (koşu D ayar dökümü): rijit bölgesiz modelde ETABS de lu = eksen boyu 3,3 m kullanır. Program ile aynı; ACI 6.2.5'in net boy tanımına göre ikisi de muhafazakâr.
- ETABS βdns: U1'de 1 (program ile aynı, E9 δ2 birebir); Cm program-belirlemeli (koşu D) 0,6 + 0,4M1/M2 → D koşusunda ETABS biraz daha düşük D/C verir.

## 5. V1 (d) tez EDB (bilgi; kopya üzerinde okundu, `Sources/` dokunulmadı)
`Sources/tez_sonuç tablo grafik/örnek_2/rc/Örnek2_28_05_24_2Boyutlu_(gebrail_niğdeli_2023).EDB` (217 960 bayt) yerel kopyası: 16 düğüm, 21 çubuk; **X = 0, 5, 10, 15 m (açıklıklar 5-5-5)**, Z = 3,3 m kat; Dead 30 kN/m ve Live 20 kN/m tüm kirişlerde; Dead öz ağırlık çarpanı 1; kombinasyon "1,4D+1,6Q" = 1,4 Dead + 1,6 Live. Tezin modeli kaynağın 5-6-5 m geometrisiyle **çelişir** (spec §1 "TEZ PDF 60 tüm açıklıklar 5 m" çelişkisi EDB ile DOĞRULANDI). Kesitler o dosyadaki tez çözümüne aittir (ör. kolon 100×60, 80×60, 35×45; kiriş 100×45, 50×35, 40×40 cm); makaleye girmez.

## 6. Açık kalanlar (IA / Method Architect)
1. **BULGU-1** (ACI 6.2.6, δ ≤ 1,4) programa kısıt olarak eklenmeli mi? Eklenmezse V2-a CD1 kapısı kalır ve kampanya optimumları ETABS'te O/S çıkabilir. Düzlem dışı tutulu + e_min kabulünde 250 mm genişlikli kolonlar bu sınıra takılır.
2. **BULGU-2** (kiriş eksenel çekmesi, eğilme ve ACI 22.5.7.1 kesme): küçük; kapsam kararı.
3. B2 kapısının yorumu: ETABS US sabitleri (0,166; 60 ksi) nedeniyle 0,98 alt sınırı ACI 318M'ye doğru uygulanan programda sağlanamaz; "uyumlu sabitlerle 1,00 ± 0,02" ölçütü önerilir.
4. AÇIK-14 (yüz konumu) IA kararı; ETABS düğümü yüz sayar ve d indirimi yapmaz (belgelendi).
5. Desenli yük (PatLLF) etkisinin makaledeki sınırlama metnine eklenmesi.
6. V2-b, V2-c, V3 bu oturumda yapılmadı.

---

## 7. V2-a yeniden doğrulama (2026-11-16, paket 3 sonrası: PCR-20/21/22; pilot 2 + K1)

Yazan: Claude Code (Developer Agent). Spec: `EX2_SPEC.md` v2.2 §10. **Bulgular bölümüne girmez.** Git kullanılmadı; `program_work/`, `phase2_bin/`, `phase3_bin/`, `ex2_work/ref/`, `ex2_bin/` ve Ex2 kaynakları değiştirilmedi. §1–§6 (ilk tur) olduğu gibi korunur; aşağıdaki kapılar onların yerine geçmez, yeni ikiliyle yeniden yapılan karşılaştırmadır.

### 7.0 Ortam
| Kalem | Değer |
|---|---|
| ETABS | 22.6.0 OAPI; iş sonunda `ApplicationExit` ile kapatıldı (süreç kalmadı) |
| Program | **donmuş** `03_Code_Execution/ex2_bin/Ex2Model.dll` `d0514e2f…c6b3` (pilot 2 ile aynı); FemCore `8da9dfd2…c0c8`, RcCore `14f84b75…beef`, FemRcModel `c1a95951…f5a9` |
| Araç | `ex2_work/EtabsEx2_rerun/` (ilk araç `EtabsEx2/` değiştirilmedi; bu onun kopyası): `Program.vb` `a9c9a7c6…4697`, `EtabsEx2Rerun.vbproj` `d35a15ad…69f9`, `summarize_rerun.py` `948268b9…2876`, `EtabsEx2Rerun.exe` `c3b053a2…8c2c`. Değişiklikler: (i) kolon "unbraced length ratio" (major ve minor) = `ColumnDesignGravity.ClearHeight`/L atanır ve geri okunur; (ii) program kolon kontrolü eleman bazında `CheckGroup`'un (PCR-20/21/22) birebir aynası; ek "noex" modu (δns muafiyetsiz, iki düzlem; ETABS NonSway davranışı); (iii) ETABS PMM zarfındaki tasarım momentlerinden ETABS-örtük 6.2.6 oranı = M(tasarım)/max(M(1. mertebe), Pu·e_min); (iv) kirişte ham ve ETABS-uyumlu sabitlerle (Vc = 2√f′c[psi]·bw·d = 0,16607√f′c·bw·d; fyt = 60 ksi = 413,685 MPa) oran; (v) BULGU-2 tanısı: M + N_t(d − h/2) |
| Tasarımlar | **PILOT2** = `04_Results/ex2/v2a_pilot2/v2a_design_x.txt` (239 557,02 TL, T = 0; S1/S2 ötelenmeli Q = 0,0795/0,0849); **K1** = `04_Results/ex2/v1_v2a_etabs/k1_design_x.txt` (eski pilot kolonları + kirişler 250×500): yeni programla 240 522,97 TL, **T = 0,0935** (g6: C_Inter_S2 250×300 6.2.6 oranı 1,531 > 1,4) → K1 bu turda 6.2.6 uyuşmasının **negatif kontrolü** olarak da işlev görür |
| ETABS ayarları | §0 ile aynı (rijit bölge yok, tek desen, PatLLF = 0, K = 1, Cm = 1, δ program-belirlemeli); koşu A: tümü NonSway; B: Q > 0,05 katlarında Sway Ordinary; C: δns = δs = 1 |
| Çıktılar | `04_Results/ex2/v2a_etabs_rerun/`: `raw/` (konsol, istasyon/uç kuvvetleri, reaksiyonlar, geometri, `*_columns_{A_nonsway_vs_code, A_nonsway_vs_noex, B_swayQ_vs_code, C_nomag}.csv`, `*_beam_stations.csv`, `*_beam_groups.csv`, ETABS tablo dökümleri `tables_*`), `summary_V2a_rerun_columns.csv`, `summary_V2a_rerun_beams.csv`, `summary_stdout.txt`, `edb/ex2_PILOT2.EDB` (`9d3f3c42…fa16`), `edb/ex2_K1.EDB` (`35340174…7188`) |
| Not | İlk deneme ETABS analizinde "Go_Model_Read() failed" verdi (EDB çalışma klasörü Türkçe karakterli Google Drive yolundaydı); çalışma klasörü yerel ASCII yola alındı, EDB'ler sonra `edb/`ye kopyalandı. Başarısız denemenin dosyaları yeni koşuyla üzerine yazıldı |

### 7.1 Kapılar
| Kapı | Ölçüt | PILOT2 | K1 | Durum |
|---|---|---|---|---|
| V1 (b) reaksiyon (tekrar) | ≤ %0,1 | U2 3 565,830 kN, fark 0,0000 % | U2 3 562,305 kN, 0,0000 % | GEÇTİ |
| V2-a kuvvet (FE4) | ≤ %0,1 | tüm durum/istasyonlarda en büyük fark 0,0000 % | aynı | GEÇTİ |
| **lu ataması** | ETABS lu = program lu | 3 025 / 2 775 / 2 800 mm; geri okunan oran × L − program lu = **0,00 mm** (12 kolon) | 3 050 / 2 800 / 2 800 mm; 0,00 mm | GEÇTİ (AÇIK-15 ETABS tarafı kapandı) |
| **CD1** | ETABS D/C > 1 veya O/S iken program "uygun" (D/C ≤ 1 **ve** 6.2.6 oranı ≤ 1,4) olan kolon = 0 | **0** (ETABS O/S 0; en büyük ETABS D/C 0,878) | **0**: ETABS E9/E10 O/S "Delta_ns exceeds 1.4 (ACI 318-14 6.2.6; 6.6.4.5.1)"; program aynı kolonları 6.2.6 oranı 1,531 ile uygunsuz sayıyor (g6). Yalnız D/C ölçütüyle (eski program) sayılsaydı 2 olurdu | **GEÇTİ** (BULGU-1 ETABS ile kapandı) |
| **CD2** | program/ETABS ≥ 0,95 (O/S dışı) | koşu A üretim modu **0,976–1,155**; "noex" modu 0,976–1,155; büyütmesiz (C) 0,992–1,123 | 0,976–1,140; C 0,992–1,134 | GEÇTİ |
| **CD3** | güvensiz yönde ≤ %5 | maks. D/C ETABS 0,8779 (E15/E18, C_Edge_S3) / program 0,8762 → **%0,19** | ETABS 0,8850 / program 0,9051 → güvenli yön | GEÇTİ |
| **6.2.6 (ETABS'te hiçbir kolon O/S olmamalı)** | PILOT2'de ETABS O/S = 0; ek olarak O/S ↔ program oranı > 1,4 eşleşmesi | **O/S 0**; eşleşme 12/12 (koşu A ve B) | O/S 2 (beklenen; program da uygunsuz); eşleşme 12/12 (A, B) | **GEÇTİ** |
| **B1** | gerekli As program/ETABS 0,98–1,10 | üst 0,987–1,000; **alt 0,972–1,000** (B_S1 0,972, B_S2 0,974) | üst 0,984–1,000; **alt 0,967–1,000** | **KISMEN** (yalnız BULGU-2; §7.3) |
| **B2** | Av/s, **ETABS sabitleriyle 1,00 ± 0,02** (x = d) | **1,0000** (üç grup); ham 0,965–0,969 | **1,0000** (üç grup); ham 0,969 | **GEÇTİ** |
| **V2-a toplam** | | kuvvet, lu, P-M + e_min + δ, 6.2.6 ve kesme formülleri ETABS ile tutarlı | | **GEÇTİ** (B1 KISMEN: kiriş eksenel çekmesi, kapsam kararı IA'da) |

Ham değerlerin yanında uyumlu-sabit değerleri her grup için `summary_V2a_rerun_beams.csv`'de (`B2_at_d_raw`, `B2_at_d_ETABSconst`; düğümde ham `B2_node_raw` 0,708–0,751, AÇIK-14 bilgisi) verildi.

### 7.2 Kolonlar: ayrıntı
- **δ ve 6.2.6 oranı birebir**: ETABS-örtük 6.2.6 oranları programla aynı (PILOT2: C_Inter_S1 düzlem içi 1,3164 = 1,3164; C_Edge_S1 düzlem dışı 1,3105 = 1,3105; C_Inter_S2 1,2074 = 1,2074; C_Edge_S2 1,1551 = 1,1551; C_Inter_S3 dışı 1,1144 = 1,1144). Net lu atanınca ETABS ile programın δns'si aynı olur.
- **Ötelenmesiz katta muafiyet** (bilgi; S3, Q = 0,048): ETABS NonSway'de 6.2.5(b) muafiyetini uygulamaz; program uygular. C_Edge_S3 düzlem içinde program δ = 1, ETABS 1,035 → program/ETABS D/C 0,998 (CD2 içinde). "noex" modunda (muafiyetsiz) program 6.2.6 oranı 1,0347 = ETABS-örtük 1,0347.
- **CD2 alt ucu** (0,976, C_Inter_S1): δ iki tarafta eşit (1,3164); fark P-M yüzeyinden (program radyal D/C ↔ ETABS etkileşimi). Büyütmesiz koşu C'de aynı kolon 0,992.
- **Koşu B** (Q > 0,05 katlarında Sway Ordinary) koşu A ile birebir (ETABS gravitede δs'yi P-Δ'ya bırakır; simetrik yükte M2s = 0, program Ms,max = 0 raporluyor).
- K1 E9/E10 için ETABS'in zarf satırındaki (Bottom, U1) örtük oran 1,411; program en büyük oranı başka uç/kombinasyonda 1,531 buluyor; ikisi de > 1,4 (aynı karar).
- e_min: program 15,24 + 0,03h mm (US dönüşümü), ACI 318M 6.6.4.5.4 15 + 0,03h (A58); ETABS-örtük oranların programla aynı çıkması ETABS'in de 15,24 kullandığıyla tutarlı (çıkarım). Program 0,24 mm muhafazakâr; bilgi.

### 7.3 Kiriş eksenel çekmesi (BULGU-2, ayrı rapor)
| Tasarım | Grup | N_t,maks (kN) | B1 üst ham / eksenel düzeltmeli | B1 alt ham / eksenel düzeltmeli |
|---|---|---|---|---|
| PILOT2 | B_S1 | 13,20 | 0,987 / 1,002 | **0,972** / 0,998 |
| PILOT2 | B_S2 | 14,19 | 0,988 / 1,002 | **0,974** / 0,999 |
| PILOT2 | B_S3 | 0 (basınç) | 1,000 / 1,000 | 1,000 / 1,000 |
| K1 | B_S1 | 15,57 | 0,987 / 1,003 | **0,971** / 0,998 |
| K1 | B_S2 | 18,73 | 0,984 / 1,003 | **0,967** / 0,998 |
| K1 | B_S3 | 0 | 1,000 / 1,000 | 1,000 / 1,000 |

Eksenel düzeltmeli sütun programın kodu değil; tanı amaçlı olarak program formülüne istasyon ve kombinasyon bazında M + N_t(d − h/2) verilerek hesaplandı (`*_beam_stations.csv`, `prog_*Req_axialadj_mm2`). Düzeltme sonrası 0,998–1,003: B1 farkı tamamen kiriş eksenel çekmesinden gelir; program bunu yok sayar (güvensiz yönde ≈ %2,7–3,4 alt, %1,2–1,7 üst donatı). Kesmede ETABS de eksenel kuvveti yok sayar (IgnorePu), B2 bundan etkilenmez. Kod değiştirilmedi; kapsam kararı (kısıta eklemek ya da sınırlama olarak beyan) Method Architect/IA'da. **2026-11-17 (PCR-24):** program bu kuralı uygular (As = As(Mu − Nt(d − h/2)) + Nt/(φfy)); aynı tasarımlarda program/ETABS B1 = 1,0000 (`04_Results/ex2/campaign2_prep/b1_pcr24.csv`; ETABS yeniden koşulmadı, mevcut `v2a_etabs_rerun` çıktısı kullanıldı) → BULGU-2 kodda KAPANDI.

### 7.4 Açık kalanlar
1. BULGU-2 kapsam kararı (B1 KISMEN'in tek nedeni).
2. AÇIK-14 (kesme kritik kesiti; düğümden d) değişmedi; ETABS düğümde tasarlar (ham düğüm oranı 0,708–0,751).
3. Ötelenmeli katta 6.6.4.6.3 (kirişlerin büyütülmüş kolon momentleriyle tasarımı) programda yok; gravitede M2s = 0 olduğundan bu örnekte etkisiz (A57).
4. Bu tur ikili `ex2_bin` (d0514e2f…) için V2-a kapısını sağlar; kampanya ikilisi kararı IA'da.
