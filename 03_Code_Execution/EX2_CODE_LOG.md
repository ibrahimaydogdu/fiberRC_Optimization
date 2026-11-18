# EX2_CODE_LOG.md — Ö2 kod iskeleti (PCR-14…18 kapsamı; Q17/Q18 hariç)

Tarih: 2026-11-16 · Yazan: Claude Code (Developer Agent) · Spec: `EX2_SPEC.md` v2, `EX2_OPEN_QUESTIONS.md`.
Git kullanılmadı. `program_work/` kaynakları, `phase2_bin/`, `phase3_bin/` ve Ö1 DLL'leri değiştirilmedi ve derlenmedi (V0 aşağıda). Koşu kampanyası başlatılmadı; yalnız 100 FE süre pilotu yapıldı.

---

## 1. Ne yazıldı

Ayrı çözüm: `03_Code_Execution/ex2_work/Ex2.sln` (spec §9.1: `program_work` dışında). Ö1'e **proje referansı yok**; `FemCore.dll`, `RcCore.dll`, `FemRcModel.dll` (ve bağımlılıklar `MathNet.Numerics.dll`, `System.ValueTuple.dll`) `ex2_work/ref/` altındaki, `phase3_bin` ile **aynı SHA-256'lı** kopyalara **dosya referansı** (HintPath) ile bağlı.

| Dosya | PCR | İçerik |
|---|---|---|
| `Ex2Model/Frame2D.vb` | 14 | `FrameSpec2D` (5-6-5 m, 3 × 3,3 m, G = 30, Q = 20 kN/m, γ = 25), `RcFrameBuilder2D` (Şekil 2 numaralaması N1–N16, E1–E21; ankastre taban; düzlem dışı U2/R1/R3 tutulu; kesit yönelimi §1.1; rijit bölge yok), desenler SW (öz ağırlık), G, Q; `RcCombos2D` (U1 = 1,4D, U2 = 1,2D+1,6L, D = SW + G; V2-b varyantları 1,0G+1,0Q ve 1,4G+1,6Q, öz ağırlıklı/öz ağırlıksız, yalnız `withVariants`); `Analysis2D` (yüz kuvvetleri, ≤ 0,5 m istasyon kuvvetleri, kalıcı eksenel yük, düşey reaksiyon toplamı) |
| `Ex2Model/GravityDesign.vb` | 15 (kesme hariç) | `BeamDesignGravity` (ACI 9.6.1.2 As,min; εt ≥ 0,004; TBDY ekleri yok; lifli eğilme Ö1 bisection mantığı yeniden yazıldı; ACI Tablo 9.3.1.1 h ≥ ℓ/18,5 kenar, ℓ/21 iç açıklık); **`IBeamShear` arayüzü**, `AciPlainShear` (ACI 22.5.5.1), **`FibreShearPlaceholderQ17` (ΔVF = 0; kodda "PLACEHOLDER Q17")**; `BeamShearGravity` (Vs ≤ 0,66√fc' bw d; Av,min eşiği lifsiz Vc ile, lifli kirişte de; smax ACI 9.7.6.2.2); `ColumnDesignGravity` (düzlem içi P-M, δns, e_min, radyal D/C — donmuş DLL'deki `ColumnDesign.MakeSection/BarCount/ColCurve/DeltaNs/DcrWithEffects` yardımcıları; Ö1 `CheckGroup` çağrılmaz; etriye ACI 25.7.2.1 + gravite kesmesi) |
| `Ex2Model/Ex2Evaluator.vb` | 17 | Ex2Design → maliyet + kısıtlar + Ö1 cezası F(1+T)²; kolon ρ türetmesi (PCR-8 gravite biçimi: ızgara %1–%4, bisection, güçlü kolon geçişi yok); maliyet §5.4 (kalıp kiriş (b+2h)L, kolon 2(b+h)L; beton ve boyuna donatı eksen boyu; tek aralıklı etriye; lif = doz × kiriş hacmi × fiyat); `RcCore.Cost.Member` + `PriceSet` (ÇŞİDB 2026, Ö1 ile aynı kaynak). Lifli kiriş + yer tutucu kesme modeli → `InvalidOperationException("PLACEHOLDER Q17 …")` |
| `Ex2Model/Ex2Problem.vb` | 16 | 9 grup (C_Edge_S1..S3, C_Inter_S1..S3, B_S1..S3) / 5 grup (C_Edge, C_Inter, B_S1..S3); b 250–450, h 300–600, 50 mm; C30 sabit; lif tipi 4D/5D + doz {0,20,25,30,35,40}; `RepairDose` (Ö1 kuralı yeniden yazıldı); `FromSizes`, `SourceTable1` |
| `Ex2Runner/Program.vb` | 18 | Ö1 bayrakları `--method --fibre --runs --fes --fr --fibreprice --seed0 --dop --out` + `--groups 9|5` + `--evalonly <dosya>`; PCR-11 günlüğü; her koşu dosyasında DLL SHA'ları ve duvar süresi; kol A (rastgele başlangıç, kutu/ön tasarım yok); `OneDimMove = False` (BTO/ATO tüm-boyut). **`--fibre 1` çıkış kodu 2 ile reddedilir (PLACEHOLDER Q17).** Lifsiz koşuda kesme modeli `AciPlainShear` |
| `Ex2Model.Tests/Program.vb` | — | 34 kontrol (V1 kontrol noktası dahil), §3 |
| `Ex2.sln`, `*.vbproj` | 18 | net472, `Deterministic`, Ö1 derleyici ayarları |

Değişken sayısı: 9 grup lifsiz **18**, lifli **24**; 5 grup lifsiz 10, lifli 16. Lifsiz uzay 35⁹ = 7,88×10¹³ (testte doğrulandı).

## 2. Dosyalar ve SHA-256

### 2.1 Donmuş referanslar (`ex2_work/ref/` = `phase3_bin`)
| Dosya | Bayt | SHA-256 |
|---|---|---|
| FemRcModel.dll | 70 144 | `c1a95951560715777d137be63ebf313102f69dd6e772265eb42ff850b228f5a9` |
| FemCore.dll | 89 600 | `8da9dfd2e9d6f74e992c0ccbf4dbc5b742500ba1876725623db4503a11c9c0c8` |
| RcCore.dll | 13 312 | `14f84b7552de4020dd653cde19a5e7ccf258b0afa491f6f0be476751a6bdbeef` |
| MathNet.Numerics.dll | 1 590 272 | `85cbaa2c0a37b8d5517c33930027be26c53df056abc89bf6e7718693720ba254` |
| System.ValueTuple.dll | 24 776 | `ad95c2b972a0bdd45513e26813ecd397b9cc557e66e9fe26ce5633a695c41d1a` |

Ex2Runner çıktı klasörüne kopyalanan `FemRcModel.dll` aynı SHA (`c1a95951…`). Her koşu dosyasının ilk satırı bu SHA'ları yeniden hesaplayıp yazar.

### 2.2 Yeni kaynak dosyalar
| Dosya (`03_Code_Execution/` altında) | Bayt | SHA-256 |
|---|---|---|
| ex2_work/Ex2.sln | 3 981 | `8497fd239b01be13812024e15736d0da7007761a7dea6b7e96465640038444e8` |
| ex2_work/Ex2Model/Ex2Model.vbproj | 1 269 | `9fc19d4a733e1e623fb222cae352e1558e815cdd6049c48474699f6cb49cd0bb` |
| ex2_work/Ex2Model/Frame2D.vb | 12 641 | `e17db92426ae17f4e71e88a1c932353c40d6260acb439e5ba097543d48daabb3` |
| ex2_work/Ex2Model/GravityDesign.vb | 16 167 | `6b3ad2bb40a53454bfabc7bf479ca305410d6a8964d840abe6b72feba5615587` |
| ex2_work/Ex2Model/Ex2Evaluator.vb | 8 452 | `80a117088fd21d096da031e35293943a6970aac808416a5513330119c9b697e1` |
| ex2_work/Ex2Model/Ex2Problem.vb | 5 821 | `c40ff3c0e18833de78c009d2a43557bda2c7c20342a02cf97a1b815e0e25d6a2` |
| ex2_work/Ex2Runner/Ex2Runner.vbproj | 1 332 | `72a798b239c7d03b4bdc379c4d35b3449fc11dcc3df44a147dd99025e250e1b1` |
| ex2_work/Ex2Runner/Program.vb | 10 866 | `7c25ebd29f1eb9e0086c715e6eff2860b9ae692cd2488553dd01931e0e1a4bcc` |
| ex2_work/Ex2Model.Tests/Ex2Model.Tests.vbproj | 1 239 | `1d1b97be2472626cf581064613b8fd1dbcaee648d52ff8889d3f588d44456633` |
| ex2_work/Ex2Model.Tests/Program.vb | 9 886 | `4eb7d732c849b885dca1e7bf8ea2c0a6d3f4cf4dbaf21bbaf1bc94e1f0449151` |

### 2.3 Derleme çıktıları (Release, net472)
| Dosya | SHA-256 |
|---|---|
| Ex2Model/bin/Release/net472/Ex2Model.dll | `d392bfce5167632a59fa9522bdf096af0277b432a4cf877322b234a8abd8cfd9` |
| Ex2Runner/bin/Release/net472/Ex2Runner.exe | `494bf8f82f7ff1b2c72c598f45ff2d3b88f231b2dda1fa11f4a48c27a90373dc` |
| Ex2Model.Tests/bin/Release/net472/Ex2Model.Tests.exe | `93efb929bd8736a19d4d60418df13b89bc5d383b29ed35938eab91da7daa9a35` |

Derleme: .NET SDK 10.0.401, `dotnet build Ex2.sln -c Release` (Ö1 projeleriyle aynı SDK-stil vbproj ve net472). 0 uyarı, 0 hata.

## 3. V0 ve testler

**V0 (spec §9.1 madde 4):** `phase2_bin` + `phase3_bin` 24 dosyanın SHA-256'sı Ex2 derlemesinden önce ve sonra hesaplandı: **birebir aynı**. `program_work` içinde en yeni yazım 2026-11-09 08:34 (Ex1Runner obj dosyası); bu oturumda yazım yok. Çalışan Ex1Runner işlemi yoktu.

**Birim testleri:** `Ex2Model.Tests.exe` → **34 geçti, 0 kaldı.**

| Test | Sonuç |
|---|---|
| V1 (a) numaralama | 16 düğüm, 21 eleman; kolonlar 1–4, 8–11, 15–18; kirişler 5–7, 12–14, 19–21; açıklıklar 5-6-5; kolon 3,3 m; N16 = (16; 0; 9,9); taban ankastre; rijit bölge yok |
| V1 (a) yönelim | Kolon E1: yerel 2 = global X (düzlem içi), Depth = h = 0,30 m, Width = b = 0,25 m, I33 = b h³/12; kiriş E5: yerel 2 = global Z, Depth = 0,35 m |
| V1 (b, c) kaynak Tablo 1 kesitleri | ΣV = **8,067 m³**; SW = 201,675 kN; G = 1 440,000 kN; Q = 960,000 kN; **1,2D+1,6L = 3 506,010 kN**; 1,4D = 2 298,345 kN (bağıl hata ≤ 2×10⁻¹⁶; eşik %0,1) |
| Yük eşlemesi | E13 (6 m, 25×30) 1,2D+1,6L uç kesmeleri toplamı = 421,5 kN (= (1,2·31,875 + 32)·6) |
| Simetri | E1 ve E4 eksenel kuvveti eşit (503,058 kN) |
| Maliyet hacmi | Değerlendirici beton hacmi 8,067 m³; 8,067 × 240 = 1 936,08 TL (§6 ile tutarlı); beton bedeli 8,067 × 3 649,35 TL |
| PCR-16 | 18/24 (9 grup), 10/16 (5 grup) değişken; Decode(lb) = 250×300, Decode(ub) = 450×600, C30; sınır dışı indis reddedilir |
| §8 doz onarımı | 5D 20→25, 40→35; 4D 20→20; 0→0 |
| Q17 bloğu | Lifli kiriş değerlendirmesi "PLACEHOLDER Q17" hatası verir; doz 0 lifli modda değerlendirilir; `Ex2Runner --fibre 1` çıkış kodu 2 |
| Determinizm | Aynı vektör iki kez aynı Fpen |

V2-b varyant reaksiyonları (bilgi; ETABS ile karşılaştırılmadı): 1,0G+1,0Q +SW 2 601,675; noSW 2 400,000; 1,4G+1,6Q +SW 3 834,345; noSW 3 552,000 kN.

**Bilgi (V2-b ön görünümü, bizim model, makaleye girmez):** kaynak Tablo 1 kesitleri bizim gravite kurallarımızla uygunsuz (T = 2,038): B_S1 (25×35) ve B_S2 (25×30) kirişlerinde Mu⁻ ≈ 207 kN·m için kesit yetersiz; B_S3 (25×55) uygun; kolon ρ türetmesiyle C_Edge %2,75 (D/C 0,954), C_Inter %2,25 (D/C 0,978). Spec §10 V2-a'daki "kaynak kesitleri büyük olasılıkla yetersiz" öngörüsüyle uyumlu; "kaynak hatalıdır" hükmü verilmez.

## 4. 100 FE süre pilotu (spec §11)

Komut: `Ex2Runner --method ALL --runs 3 --fes 100 --seed0 900 --dop 1 --out ex2_work/pilot_100FE` (lifsiz, 9 grup, rastgele başlangıç, tohum 901–903: kampanya tohumları 1001–1030 dışında). Makine: 8 mantıksal işlemci; `FEM_THREADS` tanımsız.

| Yöntem | Koşu 1 | Koşu 2 | Koşu 3 |
|---|---|---|---|
| YAKO | 0,77 s (7,68 ms/FE; JIT ısınması dahil) | 0,79 s (7,93) | 0,41 s (4,14) |
| BTO | 0,41 s (4,09) | 0,60 s (6,00) | 0,50 s (4,96) |
| ATO | 0,38 s (3,78) | 0,34 s (3,43) | 0,38 s (3,83) |

9 koşu toplam 6,17 s duvar süresi. Isınma sonrası ≈ **3,4–6 ms/FE** (tek iş parçacığı). Bu, 100 FE ölçümüdür; 10 000 FE koşu süresi için doğrusal ölçekleme bir tahmindir ve makaleye girmez (spec §11). Pilot sonuç dosyaları `ex2_work/pilot_100FE/` (sonuçları raporlanmaz).

Not: YAKO (sn = 100) ve BTO (np = 100) ilk 100 FE'yi aynı tohumla üretilen rastgele başlangıç popülasyonuna harcar; bu yüzden pilot sonuçları iki yöntemde aynı çıktı (Ö1 `Optimizers` özelliği; kampanyada aynı tohumla iki yöntemin başlangıç popülasyonu ortaktır).

`--evalonly` sınaması: `ex2_work/pilot_100FE/evalonly_example.txt` vektörü → maliyet 267 063,37 TL, T = 0, kalem dökümü ve grup sonuçları yazdırıldı.

## 5. AÇIK maddeler ve spec'ten sapmalar

| # | Konu | Durum |
|---|---|---|
| AÇIK-1 | **Q17/Q18** lif kesme kuralı ve fR karakteristik kabulü | Kodlanmadı. `FibreShearPlaceholderQ17` ΔVF = 0 döndürür; değerlendirici lifli kirişte hata verir, runner `--fibre 1` reddeder. Q17 cevabından sonra yalnız bu sınıf yazılıp blok kaldırılacak; lifli eğilme (S1) kodu mevcut ama hiçbir koşuda çalışmadı |
| AÇIK-2 | δns narinlik sınırı | Donmuş `ColumnDesign.DeltaNs` lu/r ≤ 22'de büyütmeyi atlar (ACI 6.2.5(a), ötelenmeli çerçeve sınırı); spec ötelenmesiz kabul ediyor, ötelenmesiz sınır (ACI 6.2.5(b): 34 + 12 M1/M2 ≤ 40; madde metni bu oturumda okunmadı) daha gevşektir. Kod 22 ile kaldı (muhafazakâr; PCR-2 "Cm = 1" ile aynı). ETABS "NonSway" ile V2-a CD3 karşılaştırmasında fark çıkabilir; karar Method Architect/IA |
| AÇIK-3 | Rijit uç bölgeleri | Analiz modelinde yok (spec §3/§10 V1 "rijit bölge yok" ifadesi hacim/öz ağırlık için yazılmış; analiz modeli için açık değil). ETABS V2-a modeli de uç ofsetsiz kurulmalı, aksi hâlde FE4 eşiği tutmaz |
| AÇIK-4 | Kesme kritik kesiti | Optimizasyonda Vu = düğümdeki (yüz = düğüm) |V2| (muhafazakâr; ACI 9.4.3.2'deki d uzaklığı izni kullanılmadı). Spec §10 V3 lifli Excel kontrolü "mesnet yüzünden d uzaklıkta" Vu diyor → optimizasyon ile V3 arasında kesit tutarsızlığı; tek kural seçilmeli |
| AÇIK-5 | Etriye bölgelemesi | Kiriş grubunda tek etriye aralığı (grup en büyük Vu'su); kolonda tek aralık. Spec §5.3 bölgeleme belirtmiyor; maliyet muhafazakâr |
| AÇIK-6 | Vu ≤ 0,5φVc durumu | Av,min gerekmez; yine de smax aralığında nominal etriye konur [VARSAYIM] |
| AÇIK-7 | Kolon etriyesi | ACI 25.7.2.1 sınırı (16db, 48dt, en küçük boyut) spec §5.3 gibi; madde metni okunmadı. Ek olarak kolon gravite kesmesi (ACI 22.5.6.1 Vc, Av,min — 10.6.2.1 metni okunmadı), "col shear Vs ≤ 0,66√fc' b d" ve "col tie spacing ≥ 50" kısıtları spec g-listesine **ek** olarak kondu. Bacak sayısı Ö1 kuralı (her boyuna çubuk desteklenir; ACI 25.7.2.3'e göre muhafazakâr) [VARSAYIM] |
| AÇIK-8 | εt ve "kesit yetersiz" kısıtları | Ö1 gibi ikili (±1), sürekli değil |
| AÇIK-9 | Kiriş/kolon detay sabitleri | Kiriş d = h − 60 (iki yüz), basınç donatısı ihmal; kolon Ø20 sabit, BarCount yuvarlaması gerçek ρ'yu ızgaranın üstüne çıkarabilir (ör. 300×300'de 4Ø20 = %1,40); ρ ≥ %1 gerçek alanla kontrol edilir. Ö1 ile aynı |
| AÇIK-10 | Kalıp ve beton örtüşmesi | Eksen boyu kullanıldığından düğüm bölgesi beton ve kalıbı kolon ve kirişte iki kez sayılır (spec V1 hacim kuralıyla tutarlı; Excel V5'te aynı kural uygulanmalı) |
| AÇIK-11 | Q20 (V2-a uygun tasarım) | Önerilen pilot koşu (ATO, 1 000 FE, tohum 1000) yapılmadı; IA cevabı bekleniyor |
| AÇIK-12 | ETABS | V1 (d) (tezin EDB'si) ve FEM↔ETABS V1/V2-a yapılmadı; bu oturumda ETABS açılmadı |
| AÇIK-13 | 1,4D'de β | 1,4D kombinasyonunda βdns = 1 (tümü kalıcı); 1,2D+1,6L'de β = 1,2D eksenel / Pu. Spec yazmıyor; Ö1 mantığıyla tutarlı |

**Spec'ten bilinçli farklar (karar gerektirmez, beyan):**
1. Klasör `03_Code_Execution/ex2_work/` (spec §9.1 örneğiyle aynı; görev metnindeki `program_work_ex2/` yerine spec'e uyuldu).
2. `ColumnDesignGravity`, P-M/δns/e_min/radyal D/C hesabını yeniden yazmak yerine donmuş `FemRcModel.dll`'deki yan etkisiz public yardımcıları çağırır (§9.1 "donmuş DLL'den okunur"); TBDY/Bölüm 18 içeren `CheckGroup`, `ColumnShear`, `BeamDesign`, `BeamShear`, `Checks` çağrılmaz.
3. Tasarım vektörü yalnız etkin değişkenleri içerir (lifsizde lif değişkeni yer almaz); Ö1 eski modundaki sabit girdiler yok.
4. V2-b varyant kombinasyonları `RcCombos2D.Analyze(..., withVariants:=True)` ile hesaplanır, optimizasyon zarfına girmez; runner'da raporlama çıktısı henüz yok.
5. Fiyat = 0 senaryosu yalnız lifli koşuda anlamlıdır; `--fibreprice 0` derlenir ama lifli koşu bloklu olduğundan sınanmadı.

## 5b. Güncelleme 2026-11-16 (2. oturum): Q17/Q18 uygulandı, ACI kontrolleri, Q20 pilotu, 10 000 FE süre ölçümü

IA kararları (2026-11-16): Q17 artım kuralı ana seri; Ö1 formatı (γc = 1,5 + φ) bayraklı duyarlılık; Q18 fR karakteristik kabul + fR × 0,8 bayraklı duyarlılık; Q19 fiyat = 0 senaryosu 90 koşu; Q20 tek pilot (1 000 FE, kampanya tohumları dışı); Q21 marjinal maliyet tablosu deterministik; Q22 yok; kesme kritik kesiti yüzden d (V3 kuralı); ötelenmesiz kabul korunur, lu/r sınırı düzeltilir; analiz modelinde rijit bölge yok.

### Değişenler
| Dosya | Değişiklik |
|---|---|
| `Ex2Model/GravityDesign.vb` | `FibreShearPlaceholderQ17` ve `IBeamShear.IsPlaceholder` **silindi**. Yeni `FibreShearIncrementQ17`: Vc,f = Vc,ACI + max(0, VRd,F(fFtuk; γc = 1,0) − VRd,F(0; γc = 1,0)); VRd,F = `RcCore.SfrcBeam.VRdF` (Eq. 7.7-4, Eq. 7.7-5 alt sınırı iki terimde), fctk = `FctkMin`, fFtuk = `FFtukShear` (wu = 1,5 mm), ρl = AsBot (Ö1 ile aynı), σcp = 0. Yeni `FibreShearO1Format` (duyarlılık): lifli kirişte Vc yerine VRd,F(γc = 1,5), sonra φ; lifsiz kirişte Vc,ACI. **Kiriş kesmesi:** Vu artık düğüm yüzünde değil, eleman ucundan **d** uzaklıkta (`ShearAt`, istasyonlar arası doğrusal; ACI 9.4.3.2, A48); Av,min eşiği, Vs ≤ 0,66 ve smax aynı Vu ile. **Kolon:** `DeltaNsNonSway` (ACI 6.2.5(b)(c): lu/r ≤ min(34 + 12 M1/M2, 40), M1/M2 < 0 tek eğrilik; A49; sınır aşılırsa donmuş `ColumnDesign.DeltaNs` aynen, Cm = 1); etriye: Vu > 0,5φVc ise ACI 10.7.6.5.2 aralık sınırı (d/2 ≤ 600 veya d/4 ≤ 300) eklendi (A46); kolon Vc'ye 22.5.7.1 çekme dalı eklendi (A24) |
| `Ex2Model/Ex2Evaluator.vb` | Q17 yer tutucu bloğu kaldırıldı |
| `Ex2Runner/Program.vb` | `--fibre 1` engeli kaldırıldı; `--shearformat delta|o1` (varsayılan delta; o1 dosya etiketi `fibreO1`); fR × 0,8 = mevcut `--fr 0.8` (`FibreTable.MakeBeamFibre` fRScale, fR1 ve fR3 birlikte); `--evalonly` çıktısına senaryo satırı, Vu(d), Vc,ACI, Vc,f, ΔVF, Av,min, doz, fR1/fR3 eklendi |
| `Ex2Model.Tests/Program.vb` | T8 (yer tutucu testleri) yerine Q17 testleri; T10–T13 eklendi |
| `01_Literature/CODE_PROVISIONS_EVIDENCE.md` | §2b Ö2 satırları A43–A50 (görevde yazılan `03_Code_Execution/CODE_PROVISIONS_EVIDENCE.md` yok; spec'in andığı tek kanıt dosyası `01_Literature/` altında olduğundan oraya eklendi, kopya açılmadı) |

### ACI metnine göre kolon kesme kısıtları denetimi (görev 1)
| Kodlayıcının kısıtı | ACI metni | Sonuç |
|---|---|---|
| s ≤ min(16 db, 48 dt, en küçük boyut) | 25.7.2.1(b), A43 | Uyumlu |
| Vu > 0,5φVc → Av,min | 10.6.2.1, A47 | Uyumlu |
| Vc = 0,17(1 + Nu/14Ag)√fc′ bw d, Nu en küçük basınç | 22.5.6.1, A23 | Uyumlu; çekme dalı (22.5.7.1) eksikti → eklendi |
| Vs ≤ 0,66√fc′ bw d | 22.5.1.2, A14 | Uyumlu |
| Kesme donatısı aralığı | 10.7.6.5.2, A46 | **Eksikti → eklendi** (ör. pilot tasarımda C_Edge_S3 250×300: s = 120 mm = d/2) |
| s ≥ 50 mm | ACI'de yok (25.7.2.1(a) net aralık ≥ 4/3 dagg) | Pratik kısıt olarak kaldı [VARSAYIM; Ö1 ile aynı] |
| Bacak sayısı (her çubuk desteklenir) | 25.7.2.3, A44 | Muhafazakâr [VARSAYIM] |

### SHA-256 (yeni)
| Dosya | Bayt | SHA-256 |
|---|---|---|
| ex2_work/Ex2Model/GravityDesign.vb | 20 732 | `5b37ef71a998cc4fe01056141025eb1df118caf28e121b8d8d3980e3a1077623` |
| ex2_work/Ex2Model/Ex2Evaluator.vb | 8 143 | `44b04e8e6ec04ffbff9c16a50e587b8011d4dcf3f4160b4ecefb0dec33fd831d` |
| ex2_work/Ex2Runner/Program.vb | 12 110 | `01a54c760732e08e7f338a6af7716c81ea07425da43a63be3a769b147bb5e323` |
| ex2_work/Ex2Model.Tests/Program.vb | 16 459 | `f79534f0becd23075cc364e0240d86ff94d9b1fc65149bee9f8a3139fd0831a8` |
| Ex2Model.dll (Release) | 33 792 | `ee5b1478ebbb30675e01f73a4ceda42583209669783bc09e7174da80499c907d` |
| Ex2Runner.exe (Release) | 21 504 | `a1bb590648f44555c0759d68a457166a2e43e4b1ec269720874bdeea42d94657` |
| Ex2Model.Tests.exe (Release) | 29 184 | `c9219526b431d04e949007d19751d9f2b1c42ecf81cb87b89f98f9b7b7ef036a` |

Donmuş referanslar (`ex2_work/ref/`) değişmedi (§2.1). Derleme: `dotnet build Ex2.sln -c Release`, 0 uyarı, 0 hata. **V0:** `phase2_bin` + `phase3_bin` 24 dosyanın SHA-256'sı derleme öncesi/sonrası birebir aynı; `program_work` içinde bu oturumda yazım yok.

### Testler: 53 geçti, 0 kaldı
Yeni/değişen: T8 doz 0 lifli mod Fpen = lifsiz Fpen (aynı kesit; 288 810,77 TL), doz 0'da her grupta Vc,f = Vc,ACI; Q17 modeli lifsiz kirişte = `AciBeam.Vc`; lifli (4D, 30 kg/m³) değerlendirme engelsiz, ΔVF > 0, lif bedeli = lif kg × 90. T10 ΔVF ≥ 0 ve dozda azalmayan (bw 250/350/450, h 300/450/600, ρl %0,3/%1/%2, 4D 20–40, 5D 25–35); fR × 0,8 → ΔVF küçülür; O1 formatı = VRd,F(γc = 1,5), lifsizde Vc,ACI. T11 x = 0 istasyon kesmesi = yüz kesmesi; E13'te |V(0)| − |V(d)| = w·d = 70,25 × 0,24 = 16,86 kN; B_S2 grup Vu = en büyük düğüm kesmesi − w·d. T12 ötelenmesiz sınır (tek eğrilik eşit uç momentleri 22; çift eğrilik 40; M1/M2 = −0,5 → 28), lu/r = 36,7 çift eğrilikte δ = 1. T13 kolon etriye aralığı ≤ 25.7.2.1 ve 10.7.6.5.2.

**Bilgi (makaleye girmez):** 300×500 (d = 440), As = 900 mm², 4D 30 kg/m³: Vc,ACI = 122,9 kN, ΔVF = 101,0 kN (Vc'nin ≈ %82'si); Ö1 formatı VRd,F(1,5) = 139,9 kN. Lifin kesmedeki kaldıracı büyüktür; fR × 0,8 ve Ö1 formatı duyarlılıkları bu yüzden önemlidir. Kaynak Tablo 1 kesitlerinde (V2-b bilgi) ötelenmesiz sınır sonrası kolon ρ: C_Edge %1,00 (D/C 0,960), C_Inter %1,50 (0,988); önceki oturum %2,75 / %2,25 idi (22 sınırı). Kirişler yine yetersiz (T = 2).

### Q20 pilotu (V2-a uygun tasarım; 04_Results/ex2/v2a_pilot/)
Komut: `Ex2Runner --method ATO --fibre 0 --runs 1 --fes 1000 --seed0 9000 --dop 1` (tohum 9001; kampanya 1001–1030 dışı). **1 000 FE'de uygun tasarım bulundu** (T = 0; ilk uygun FE 1; 753/1 000 uygun değerlendirme), 5 000 FE'ye çıkılmadı. Maliyet 235 306,37 TL (beton 35 323,88; donatı 105 538,10; kalıp 94 444,39). x = `0,0,2,1,0,0,2,1,0,0,0,1,0,4,0,4,0,4`: kolon b×h (b düzlem dışı) C_Edge_S1 250×300, S2 350×350, S3 250×300; C_Inter_S1 350×350, S2 250×300, S3 250×350; tüm ρ = %1 ızgara (4Ø20); D/C 0,43–0,94; kirişler B_S1–S3 250×500 (AsTop ≈ 1 373–1 402, AsBot ≈ 726–752 mm²; Vu(d) = 183,68 kN; s = 203,7 mm). Dosyalar: `EX2_ATO_plain_g9_fr1.00_fp90_run01.csv`, `v2a_design_x.txt`, `v2a_design_eval.csv`. Sonuç raporlanmaz; yalnız V2-a girdisi. Not: 1 000 FE pilotu en ucuz çözüm değildir (kolon boyutları kat sırasıyla monoton değil); V2-a için yeterli.

### 10 000 FE süre ölçümü
Bu oturumun ilk ölçümü (paket 1 ikilisi, Ex2Model `ee5b1478…`) `ex2_work/timing_10k/superseded_pkg1/` altına taşındı: lifsiz 25,2–27,0 s, lifli 26,7–41,4 s. Paket 2 (Q kontrolü her FE'de) sonrası geçerli ölçüm §5c'dedir.

### Açıklar (yeni / güncel)
| # | Konu | Durum |
|---|---|---|
| AÇIK-1 | Q17/Q18 | **KAPANDI** (uygulandı) |
| AÇIK-2 | δns narinlik sınırı | **KAPANDI**: ACI 6.2.5(b)(c) ötelenmesiz sınır uygulandı (A49) |
| AÇIK-4 | Kesme kritik kesiti | **KAPANDI**: optimizasyon ve V3 aynı kural (yüzden d, yüz = düğüm) |
| AÇIK-14 | Kesme kritik kesitinde "yüz" | **AÇIK**: rijit bölge olmadığından yüz düğümde alındı (d düğümden ölçülür). Gerçek kolon yüzü düğümden hc/2 uzaktadır; kritik kesit hc/2 + d alınsaydı Vu küçülürdü. Kod muhafazakâr taraftadır. V3 Excel'i ve ETABS okuma noktası aynı kurala bağlanmalı (ETABS rijit bölgesiz modelde istasyon konumu V2-a'da kontrol edilecek) |
| AÇIK-15 | Kolon lu | **AÇIK**: spec §5.1 "lu = net yükseklik" diyor; rijit bölge olmadığından kod lu = eksen boyu 3,3 m kullanıyor (muhafazakâr; lu/r büyür). Net yükseklik (3,3 − kiriş h) istenirse kodda tek satır; Method Architect/IA kararı. ETABS'in lu'su V2-a'da okunmalı |
| AÇIK-16 | Lifli kirişte ρl | **AÇIK**: ΔVF için ρl = AsBot (Ö1 ile aynı). Mesnet (hogging) bölgesinde çekme donatısı AsTop'tur; AsBot ΔVF'yi küçültür (muhafazakâr). Spec §5.2 yazmıyor; V3 Excel'inde aynı seçim kullanılmalı |
| AÇIK-17 | ACI 6.2.5 tutulu çerçeve koşulu | Bilgi: ACI "tutulu" sayılmayı ≥ 12 kat yanal rijitlikli bağlantı elemanına bağlar; Ö2'de yoktur. Ötelenmesiz kabul spec [K f] varsayımı olarak kalır (§12 sınırlama) |
| AÇIK-6, 8, 9, 10, 12, 13 | | Değişmedi (§5) |
| AÇIK-11 | Q20 | **KAPANDI** (pilot yapıldı) |

## 5c. Paket 2 (2026-11-16, aynı gün): servis (a), hakem 2. turu (`Review_Round_Ex2_oneriler_ic.md`, `_dis.md`)

IA kararı servis (a): 30 kN/m korunur, ACI 9.3.1.1 bağlayıcı kısıt değil, ACI 24.2 sehim (ℓ/480) yalnız en iyi çözümlere rapor. §5b'deki şu kalemler **bu paketle değişti**: ρl = AsBot → **AsTop** (AÇIK-16 kapandı; Ö1 varyantı AsBot'ta kaldı); g5 kısıtı kaldırıldı; ötelenmesiz sınır artık kat Q'suna bağlı; Q20 pilot vektörü yeniden üretildi (eski dosyalar `04_Results/ex2/v2a_pilot/superseded_pkg1/`).

### Değişenler
| Dosya | Değişiklik |
|---|---|
| `Ex2Model/GravityDesign.vb` | `IBeamShear.ConcreteShear(bw, d, fc, asTop, asBot, fib)`; `FibreShearCommon` (σcp = 0 sabiti, fR3/fR1 > 0,5 geçerliliği `SfrcParams.CanSubstituteRebar`, ΔVF(γc), Eq. 7.7-5 doz-0 bağlayıcılık testi); `FibreShearIncrementQ17` (ρl = AsTop), **yeni `FibreShearMc2010Local`** (Vc,ACI + ΔVF(γc = 1,5)/φ ≡ φ(Vc + Vs) + ΔVF(1,5)), `FibreShearO1Format` (Ö1 `BeamDesign.VcOf` + `Ex1Evaluator` birebir: AsBot, FctkMin, FFtukShear, γc = 1,5, σcp = 0), `ShearModels.ByName`. Kesme sonucu: Vc,f/Vc,ACI, VRd,F(0; γc = 1)/Vc,ACI, vmin bağlayıcı mı, fR geçerli mi, **etriyeyi belirleyen (strength / Avmin / smax / smax-nominal)**, kesme D/C (sağlanan etriye). Eğilme D/C (`PhiMn`, sağlanan donatı). Kolon: `CheckGroup(..., nonSwayStorey)` — Q > 0,05 katında düzlem içi sınır 22 (donmuş `DeltaNs`), aksi hâlde 6.2.5(b)(c); düzlem dışı her zaman ötelenmesiz |
| `Ex2Model/Serviceability2D.vb` (yeni) | `Stability2D.StoreyQ`: birim yanal yük (kat başına 1 kN, düğümlere eşit), ACI 6.6.3.1.1(a) rijitlikleri (kolon 0,70Ig, kiriş 0,35Ig, alan 1,0Ag), ΣPu = katın kolon eksenel toplamı (tasarım kombinasyonlarının en büyüğü), ℓc = 3,3 m. `Deflection2D.Report`: Ie (24.2.3.5) grup başına, kritik pozitif/negatif kesit ortalaması (24.2.3.6), Mcr = 0,62√fc′ Ig/yt, Icr tek donatılı dönüştürülmüş kesit, λΔ = 2,0 (ρ′ = 0), D sürekli / L geçici; iki FEM çözümü (D ve D+L, Ie ayrı); açıklık ortası sehim kirişin kirişine göre Hermite formülü + wℓ⁴/(384EIe) |
| `Ex2Model/Frame2D.vb` | `Combo2D.QOn/FQP`; `RcCombos2D.PatternCombos` (ACI 6.4.1 kat başına + tüm katlar; açıklık kümeleri {1,3}, {2}, {1,2}, {2,3}, {1}, {3}: 24 desen); `Analyze(..., patterned)` kiriş başına Q_E deseni ekler (yalnız araç); genel desen sayısıyla birleştirme |
| `Ex2Model/Ex2Evaluator.vb` | `Ex2Options` (Patterned = False, **QCheck = True** varsayılan); kat Q'su her değerlendirmede; **g5 kısıt listesinden çıkarıldı** (G_Depth yalnız rapor); kiriş kalem maliyetleri (beton, boyuna, etriye, kalıp, lif; TL ve kg) |
| `Ex2Model/Ex2Problem.vb` | `Opts` alanı |
| `Ex2Runner/Program.vb`, `Ex2Runner/Tools.vb` (yeni) | Bayraklar: `--shearformat delta|mc2010|o1`, `--fr` (0,8 / 0,6; eğilme + kesme birlikte), `--qcheck 1|0`, `--patterned 0|1`. Araçlar: `--evalonly` (+ pilot ölçütü satırı), `--marginal` (Q21; lifsiz vektör → her kiriş grubu tek tek ve ALL × {4D 20/30/40, 5D 25/35}; kesit sabit; kiriş ve toplam maliyet ayrı; kalem Δ; grup başına etriye/eğilme belirleyicisi, AsTop/AsBot, s), `--scan` (Q22; her değişken tüm aralıkta tek tek, ±1 alt kümesi ayrıca; lifli modda kiriş grubu (tip, doz) çiftleri), `--patterns` (tasarlanmış donatıyla desenli D/C + desenli zarfla yeniden türetme maliyeti), `--zerofibre` (grup grup ve hepsi; etiket required / neutral / useful / harmful), `--deflection`. Koşu dosyasına `qcheck` ve kat Q'ları yazılır; varyant biçimli koşu etiketi `fibreMC2010` / `fibreO1`, `--qcheck 0` etiketi `_noQ` |
| `Ex2Model.Tests/Program.vb` | T14–T18 eklendi |
| `01_Literature/CODE_PROVISIONS_EVIDENCE.md` | A51 (6.4.1/6.4.2), A52 (6.6.4.3/6.6.4.4.1 + R6.6.4.6.2), A53 (24.2; KISMEN), A54 (MC2010 Eq. 7.7-4/7.7-5 numaraları teyit; ρl ≤ 0,02 sınırı maddede yok) |
| `EX2_SPEC.md` | v2.1: §A.1 Q17–Q22 karar satırları, yeni §A.4, §5.1 g5 ve narinlik satırları, §13 |

### Testler: 71 geçti, 0 kaldı
T14: ana model ρl = AsTop; Ö1 varyantı AsBot ile Ö1 formülü; MC2010-yerel = Vc + ΔVF(1,5)/φ; ana ≥ MC2010-yerel; fR3/fR1 ≤ 0,5 → ΔVF = 0; C30 ızgarasının tüm noktaları geçerli; fR × 0,6 fR1 ve fR3'ü ölçekler, eğilme dayanımını ve ΔVF'yi birlikte düşürür. T15: g5 kısıt listesinde yok. T16: kat Q > 0, büyük kolonda küçük Q, `--qcheck 0` Q hesaplamaz. T17: 26 kombinasyon; kiriş başına Q_E desenlerinin toplamı = 960 kN; desenli MuBot ≥ tek desen. T18: Hermite açıklık ortası formülü basit mesnetli FEM kirişinde 5wℓ⁴/384EI'yi verir (Ry işareti doğrulandı); rapor 9 kiriş, sehimler > 0, 0 < Ie/Ig ≤ 1.

**Bilgi (makaleye girmez):** orta havuz tasarımı Q = 0,040 / 0,052 / 0,034 (S1/S2/S3); 250×300 kolonlarla Q = 0,109 / 0,099 / 0,052. **Küçük kolonlu (ucuz) tasarımlarda Q > 0,05 çıkar → ötelenmesiz kabul bu çerçevede tasarıma bağlıdır**; kod artık katı Q ile sınıflar (iç Critic (ii) seçeneği). Kaynak Tablo 1 kesitlerinde kolon ρ yine %2,75 / %2,25 (22 sınırı; Q > 0,05).

### Q20 pilotu (yeniden üretildi; 04_Results/ex2/v2a_pilot/)
Komut aynı (ATO, lifsiz, 1 000 FE, tohum 9001, `--dop 1`). x = `1,0,2,0,1,1,4,0,0,0,0,1,0,5,0,5,0,5`; maliyet 239 057,11 TL; **T = 0**. Kolonlar (b×h) C_Edge_S1 300×300, S2 350×300, S3 300×350; C_Inter_S1 450×300, S2 250×300, S3 250×350; tümü ρ ızgarası %1; D/C 0,42–0,97. Kirişler 250×550 (üç kat); eğilme D/C 1,00; kesme D/C 0,98 (etriye smax = d/2 = 245 mm ile belirleniyor; Vs,gerekli > 0 ama aralık smax'ta). Kat Q = 0,076 / 0,084 / 0,038 → S1 ve S2 ötelenmeli sınıfta (sınır 22). **Pilot ölçütü: OK** (T = 0; en büyük kiriş eğilme D/C 1,00, kiriş kesme D/C 0,98, kolon P-M D/C 0,97; hepsi ≥ 0,8). Uyarı: dış hakemin "Vs > 0 ve etriye Av,min üstünde" koşulu aralık bakımından sağlanmıyor (etriye smax'ta; dayanım belirleyici değil); V2-a B2 (Av/s 0,98–1,02) karşılaştırması bu tasarımda smax kuralını sınar, dayanım kuralını sınamaz. Gerekirse ikinci pilot (başka tohum) IA kararı. Dosyalar: `EX2_ATO_plain_g9_fr1.00_fp90_run01.csv`, `v2a_design_x.txt`, `v2a_design_eval.csv`.

### Araç sınaması (pilot vektörü; `ex2_work/tool_smoke/`, sonuç değil)
- `--marginal`: doz 0 lifli mod = lifsiz (239 057,11 TL). Tek grup lif: boyuna donatı −254…−493 TL, etriye 0 (smax belirleyici), lif +3 960…+7 920 TL → kiriş maliyeti +%2,6…+%5,2.
- `--scan`: 90 değerlendirme; 9 daha ucuz uygun komşu (6'sı ±1) → pilot yerel en iyi değil (1 000 FE pilotu için beklenen).
- `--patterns`: tasarlanmış donatıyla desenli zarfta kiriş eğilme D/C 1,14–1,18, kesme D/C 1,01–1,02, kolon D/C ≤ 0,99; 1'i aşan 6 kontrol; desenli zarfla yeniden türetme +4 791 TL (+%2,0), T = 0.
- `--deflection`: en kötü oran ℓ/480 için 0,72 (6 m kirişler), ℓ/240 0,36, anlık L ℓ/360 0,20 → OK.

### 10 000 FE süre (paket 2 ikilisi; `ex2_work/timing_10k/`, `--dop 1`, tohum 9101)
| Senaryo | YAKO | BTO | ATO |
|---|---|---|---|
| Lifsiz (18 değişken) | 32,58 s (3,26 ms/FE) | 32,41 s (3,24) | 31,56 s (3,16) |
| Lifli 90 TL/kg (24 değişken, Q17 ana) | 49,50 s (4,95) | 37,42 s (3,74) | 33,86 s (3,39) |

Tek iş parçacığı, 8 mantıksal işlemcili makine. Paket 2'deki kat Q çözümü süreyi paket 1'e göre ≈ %20–25 artırdı (lifsiz 25–27 s → 32–33 s). Bu ölçümlerin maliyet/x sonuçları raporlanmaz (pilot tohum 9101, kampanya dışı). Bilgi: üç lifli koşunun en iyi çözümlerinde lif dozu 0 çıktı (`--zerofibre` sınaması: "no fibre"). Kampanya süresi tahmini makaleye girmez.

### SHA-256 (paket 2)
| Dosya (`03_Code_Execution/ex2_work/`) | Bayt | SHA-256 |
|---|---|---|
| Ex2Model/Frame2D.vb | 15 547 | `b0cff68adf3c04570c3ed5a183220e332766e05890f3932d4b4a6d794c71fcc0` |
| Ex2Model/GravityDesign.vb | 27 905 | `d7772fa4b33523f1eb48ebe0e89983b7299ae48c8a3f786c5a85aaf153aeb457` |
| Ex2Model/Serviceability2D.vb | 10 770 | `c4995f73f9a9e3edff04aff2835303ef718db4873c0a895f805d20f2bd056659` |
| Ex2Model/Ex2Evaluator.vb | 9 801 | `ff252090775f35efa651eff3f31db0be8273db58f15f3cee4ed5ce6cfe125431` |
| Ex2Model/Ex2Problem.vb | 6 058 | `11d36b21ee928f70a24880ea390ef236c0a071f75dd734e80a9e6d5c905bb7bb` |
| Ex2Runner/Program.vb | 11 074 | `05240bc7dacf25e62fb6583b566113cb3ba38f2ecb329fa887a79c74acacbba3` |
| Ex2Runner/Tools.vb | 20 214 | `92ce6d41ac50ddbaf6a750afc58f63a98e0ff21ade5e512a423e86dc057f098f` |
| Ex2Model.Tests/Program.vb | 23 298 | `8f7b4d35b6c73cfbf6d7ab9227a1aec4de3abb3883f78a41cf5df9aed666c20f` |
| Ex2Model.dll (Release) | 43 008 | `b30a2aad984032f107f10dc281da29bd7d120f952e7b1828f9cc63eb30d2e998` |
| Ex2Runner.exe (Release) | 39 936 | `7cf0d3d25133e23b679270c497b830bce5371bec1fbb0e9ee7c554e355c87213` |
| Ex2Model.Tests.exe (Release) | 37 376 | `605ed2dd97bac7305b1a22e1e178194932a0176ad425cfc9aed9f956f7eba847` |

§5b'deki SHA tablosu paket 1 içindir (geçersiz). Donmuş `ref/` DLL'leri değişmedi; V0 sonucu aşağıda.

### Açıklar (paket 2 sonrası)
| # | Konu | Durum |
|---|---|---|
| AÇIK-14 | Kesme kritik kesitinde yüz (düğüm mü düğüm + hc/2 mi) | AÇIK (iç Critic ve dış hakem hc/2 + d ve analitik Vu öneriyor; IA kararı "rijit bölge yok, yüzden d" kodda düğümden d olarak uygulandı). Not: istasyon enterpolasyonu düzgün yayılı yükte analitikle aynıdır (T11) |
| AÇIK-15 | Kolon lu (eksen 3,3 m mı net mi) | AÇIK |
| AÇIK-16 | ρl | KAPANDI (AsTop) |
| AÇIK-18 | ETABS Vc denklemi ve kesme istasyonu | V2-a B2'de teyit (ETABS ACI 318-14 kiriş Vc'si ve tasarım istasyonu) |
| AÇIK-19 | Q > 0,05 bulgusu | Ötelenmesiz kabul tasarımların bir kısmında geçersiz; kod kat bazında sınıflıyor (sınır 22). Sway katında δs uygulanmıyor (simetrik tasarım + simetrik yük, M2s ≈ 0; desenli yük aracında asimetrik yük var, orada δs yok: sınırlama). Makale dili Method Architect/IA |
| AÇIK-20 | Q modeli ayrıntıları | Kat başına 1 kN düğümlere eşit dağıtıldı; ΣPu kolon eksenel toplamı (kat altı); ℓc = 3,3 m; ETABS ile karşılaştırılmadı |
| AÇIK-21 | Sehim raporu varsayımları | Ie grup bazında (grubun en büyük momentleri), kolon brüt, lifin rijitlik etkisi yok, sürekli hareketli yük oranı 0, ℓ = eksen açıklığı; A53 KISMEN (24.2.4.1.1 denklem gövdesi render edilmedi) |
| AÇIK-22 | Q18 numune verisi | `Evidence/CMOD_test_verileri.xlsx` numune düzeyinde veri içermiyor (iç Critic); bu oturumda yeniden açılmadı |

## 6. Yeniden üretim

```
cd 03_Code_Execution/ex2_work
dotnet build Ex2.sln -c Release
Ex2Model.Tests/bin/Release/net472/Ex2Model.Tests.exe          # 34/34
Ex2Runner/bin/Release/net472/Ex2Runner.exe --method ALL --runs 3 --fes 100 --seed0 900 --dop 1 --out pilot_100FE
```

Sıradaki adımlar (IA onayıyla): Q17/Q18 → `IBeamShear` lifli uygulaması ve blok kaldırma; Q20 → V2-a uygun tasarım; ETABS V1/V2-a (rijit bölgesiz, NonSway, tek desen); Q19 → koşu protokolü ön kaydı.

## 5d. ETABS V1 / V2-a bulguları (2026-11-16; ayrıntı `EX2_VALIDATION_REPORT.md`) — kod DEĞİŞTİRİLMEDİ

| # | BULGU | Kanıt | Etki |
|---|---|---|---|
| **BULGU-1** | `ColumnDesignGravity` ACI 318M-14 **6.2.6** sınırını ("Mu including second-order effects shall not exceed 1.4Mu due to first-order effects", PDF 67 / basılı 65) kontrol etmiyor; δ > 1,4 olan kolon D/C ≤ 1 ise uygun sayılıyor | Pilot ve K1'de C_Inter_S2 (250×300, E9/E10): program δ2 (düzlem dışı, e_min, lu/r = 44) = 1,949 / 1,929, δ3 (Q > 0,05 katı) = 1,51; ETABS 22 aynı δ'yı hesaplayıp "O/S — Delta_ns exceeds 1.4 (ACI 318-14 6.2.6; 6.6.4.5.1)" veriyor; program D/C 0,974 / 0,984 | V2-a CD1 = 2 → KALDI. Pilot (Q20) ACI'ye göre uygun değil. Öneri: g1'e δ ≤ 1,4 kısıtı (6.2.5 ile ihmal edilmeyen yönlerde) — Method Architect/IA kararı |
| **BULGU-2** | Kiriş eksenel kuvveti eğilmede ve kesmede (ACI 22.5.7.1 çekme) yok sayılıyor | S1, S2 kirişleri 13–18,7 kN çekmede; ETABS üst/alt gerekli As'yi Mu + N(d − h/2) ile hesaplıyor (fark ≤ 0,57 kN·m'ye iner); program/ETABS As 0,966–0,987 | Küçük, güvensiz yönde %1,5–3,4 As; B1 alt 0,966 (< 0,98). Kesmede ETABS de eksenel kuvveti yok sayıyor (IgnorePu) |
| bilgi | ETABS kiriş kesmesi Vc = 2√f′c[psi] bw d (0,166√f′c) ve fyt ≤ 60 ksi (413,69 MPa) kullanıyor; program ACI 318M (0,17; 420 MPa) | 102 istasyonda doğrusal uydurma, artık ≤ 0,0012 kN | Program hatası değil; B2(d) 0,965–0,969, uyumlu sabitlerle 1,000 |
| bilgi | ETABS kesmeyi düğüm dahil tüm istasyonlarda tasarlar, ACI 9.4.3.2 d indirimi yok; lu oranı (program-belirlemeli) = 1,0 (eksen boyu); NonSway ve Sway Ordinary gravitede aynı | ETABS çıktıları | AÇIK-14/15/18/19 için belge |

Yeni dosyalar (yalnız doğrulama aracı, kampanya kodu değil): `ex2_work/EtabsEx2/EtabsEx2.vbproj`, `Program.vb` (SHA-256 `9395c0cdb1a31d5ee646620c1148c3a50e497a3bec55e6c30d3372265c4b5716`), `summarize.py` (`531f479f7f5b77c614f86129b6271def33be24dd06122d8705a860fceeceecc3`), `bin/Release/net472/EtabsEx2.exe` (`7e6d29ac4811c895f3dda92b4ae0c1f536896bf903fde31eb48640b3b9a7e0b0`). Ex2Model.dll (`b30a2aad…`) ve `ref/` değişmedi.

## 5e. Paket 3 (2026-11-16): PCR-20/21/22 + araç çıktısı (iç Critic `Review_Round_Ex2_acik_ic.md` + dış hakem `_dis.md` ortak görüşü + ETABS V2-a BULGU-1; IA "kararları hakemlerle al" yetkisi)

Git kullanılmadı; ETABS açılmadı; kampanya koşusu yok. `program_work/`, `phase2_bin/`, `phase3_bin/`, `ex2_work/ref/` değiştirilmedi.

### Değişenler
| Dosya | Değişiklik |
|---|---|
| `Ex2Model/GravityDesign.vb` | **PCR-21** `ColumnDesignGravity.ClearHeight` (Ö1 `RcFrameBuilder` ofset kuralı birebir: lu = L − hb(alt)/2 − hb(üst)/2, taban 0); kolon kontrolü bu lu ile (narinlik ihmali ve Pc). **PCR-22** `DeltaNsNoExemption` (donmuş `DeltaNs` formülü, 22 muafiyeti olmadan; Cm = 1); ötelenmeli katta (Q > 0,05) M3 = abs(M2ns + δs·M2s) × δns, muafiyet yok; ötelenmesiz katta önceki 6.2.5(b)(c) yolu. **PCR-20** `Ratio626Of` = δns·max(abs(M2ns + δs M2s), Pu·e_min) / max(abs(M), Pu·e_min), her uç/kombinasyon, düzlem içi ve dışı; sonsuz δ → 10. `ColumnGravityResult`: LuMin/LuMax, Ratio626/In/Out, Delta3Max/Delta2Max, DeltaSMax, AnySway; Governing'e 6.2.6. `CheckGroup(..., stab As StoreyStability)` (önceki `nonSwayStorey As Boolean()` yerine). Kiriş kesmesi: `AvSStrength`, `AvSMin`, `AvSProvided`, `SMax` alanları; `StirrupGov` etiketi "smax" → "smax(Av/s by strength)" / "smax(Av/s by Avmin)". Kesme kritik kesiti (düğümden d) **değişmedi** |
| `Ex2Model/Serviceability2D.vb` | `Stability2D.Assess` → `StoreyStability` (Q, NonSway, δs = 1/(1 − Q), kat ötelenmesi = kat ortalaması ux farkı (mm), Ms,max, net kat ötelenme kuvveti H_k, sway yüz kuvvetleri). M2s: her kat düğümünde U1 tutulu analiz (`Analyze(braced:=True)`) → kat tepkileri toplamı H_k = −ΣR → tutulmamış brüt çerçeveye, kat düğümlerine eşit dağıtılarak. Yalnız ötelenmeli kat varsa çözülür (2 ek FEM çözümü). İlk deneme (tek düğüm tutulu, M2s = M − M_tutulu) simetrik çerçevede kiriş eksenel uzamasından sahte M2s (0,45–1,3 kN·m) verdi → net kat kuvveti yöntemine geçildi (T21) |
| `Ex2Model/Frame2D.vb` | `Analysis2D.Factors(combo)`; bayraklar WithVariants/Patterned/Braced; `Analyze(..., braced)` (tüm kat düğümleri U1 tutulu; istasyon kuvveti yok) |
| `Ex2Model/Ex2Evaluator.vb` | Kısıtlar **g6** "col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)" = max oran/1,4 − 1; **g7** "storey Q <= 1/3 (delta_s <= 1.5; ACI 6.6.4.6.2(a))" = 3Q − 1 (`--qcheck 0`'da pasif). İhlal sayıları koşu dosyasının `violation_counts` satırına `RunLog` ile girer. `Ex2Result.Stability` |
| `Ex2Runner/Program.vb` | Koşu dosyasına `stability,` satırı (kat başına Q, sınıf, δs, kat ötelenmesi, Ms,max) ve `columns_ACI6.2.6` satırı (oran, D/C, lu, δ3, δ2, δs) |
| `Ex2Runner/Tools.vb` | `--evalonly` kolon satırı: lu_mm, sway, deltaS, delta_ns_in/out, ACI6.2.6_ratio_in/out, oran/1,4, gov; kiriş satırı: AvS_strength, AvS_min, AvS_required, AvS_provided, smax; pilot satırı: maxACI6.2.6ratio, `requiredAvS_byStrength`, `spacingGovernedByStrength(s<smax)` (yanıltıcı `anyStirrupByStrength(Vs>Avmin)` kaldırıldı); `StabilityLine`; `--patterns` desenli zarf için `Assess` (M2s ≠ 0 → δs) ve 6.2.6 oranı |
| `Ex2Model.Tests/Program.vb` | T19–T22 (30 kontrol) |
| `EtabsEx2/Program.vb` | Yalnız **not** (yorum, `SetColumnOverwrites` üstü): sonraki ETABS turunda unbraced length ratio (öğe 3 ve 4) = ClearHeight/L açıkça atanacak ve okunacak; `ProgColumnDcr/ProgMaxDelta` PCR-21/22'ye göre güncellenecek; 6.2.6 oranı raporlanacak. Araç **derlenmedi ve çalıştırılmadı** (`EtabsEx2.exe` `7e6d29ac…b0b0` aynı) |

### Testler: 101 geçti, 0 kaldı (önceki 71)
- V1: T3 1,2D+1,6L = **3 506,010 kN** (bağıl hata 1,3×10⁻¹⁶) geçiyor.
- T19 (lu net): 1. kat 3,025 m; 2. kat 2,75 m; Critic örneği lu/r = 2 750/90 = 30,6 (çift eğrilikte δ = 1, tek eğrilikte > 1); net lu eksen boyundan küçük δ verir.
- T20 (6.2.6): oran formülü (moment, e_min, δs dalları; sonsuz δ → 10); g6 listede ve = max(0, oran/1,4 − 1); **ihlal örneği**: tüm kolonlar 250×300 → oran 2,70, g6 > 0, T > 0. Eski Q20 pilot vektörü yeni kodla **uygunsuz** (T = 0,0793; C_Inter_S2 düzlem dışı oran 1,511) → ETABS BULGU-1 ile tutarlı.
- T21 (Q > 0,05): `DeltaNsNoExemption` = donmuş `DeltaNs` (lu/r > 22); lu/r = 15,3'te de > 1; 250×300 tasarımı Q = 0,177/0,230/0,147 → δs = 1/(1 − Q); simetrik yükte Ms,max ≤ 4×10⁻¹⁵ kN·m, kat ötelenmesi ≤ 2×10⁻¹⁵ mm (M2s = 0 hesapla gösterildi); g7 = max(0, 3Qmax − 1); desenli (asimetrik) zarfta Ms,max 1,41/1,81/2,36 kN·m; net kat kuvvetleri tutulmamış çerçevenin kat ortalaması ötelenmesini %0,66 farkla verir (işaret doğrulandı); δs D/C'yi değiştirir; `--qcheck 0`'da g7 pasif, δs = 1.
- T22: AvPerS = max(AvS_strength, AvS_min); AvS_provided = Av/s; etiket "smax(Av/s by strength)".

### V0 (Ö1 ikilileri)
`phase2_bin` + `phase3_bin` + `ex2_work/ref` 29 dosyanın SHA-256'sı derleme öncesi, sonrası ve dondurma sonrası **birebir aynı**; `program_work` içinde yazım yok.

### Pilot 2 (V2-a girdisi; `04_Results/ex2/v2a_pilot2/`; sonuç raporlanmaz)
`Ex2Runner --method ATO --fibre 0 --runs 1 --fes 1000 --seed0 9000 --dop 1` (tohum 9001). **1 000 FE'de uygun** (T = 0; 553/1 000 uygun; 5 000'e çıkılmadı). x = `0,0,0,0,2,0,4,0,2,0,1,0,0,5,0,4,0,4`; maliyet **239 557,02 TL**. Kolonlar (b×h, h düzlem içi): C_Edge_S1 250×300, C_Edge_S2 250×300, C_Edge_S3 350×300, C_Inter_S1 450×300, C_Inter_S2 350×300, C_Inter_S3 300×300; ρ = %1. Kirişler B_S1 250×550, B_S2/B_S3 250×500. Kat Q = 0,0795 / 0,0849 / 0,0483 → S1, S2 ötelenmeli (δs = 1,086 / 1,093), S3 ötelenmesiz; kat ötelenmesi 0,0000 mm, Ms,max 0,0000 kN·m.

| Kolon | lu (mm) | P-M D/C | δns içi / dışı | 6.2.6 oranı içi / dışı (sınır 1,4) |
|---|---|---|---|---|
| C_Edge_S1 | 3 025 | 0,694 | 1,197 / 1,311 | 1,197 / 1,311 |
| C_Inter_S1 | 3 025 | 0,611 | 1,316 / 1,120 | **1,316** / 1,120 |
| C_Edge_S2 | 2 775 | 0,694 | 1,103 / 1,155 | 1,103 / 1,155 |
| C_Inter_S2 | 2 775 | 0,559 | 1,207 / 1,144 | 1,207 / 1,144 |
| C_Edge_S3 | 2 800 | **0,876** | 1,000 / 1,025 | 1,000 / 1,025 |
| C_Inter_S3 | 2 800 | 0,358 | 1,000 / 1,114 | 1,000 / 1,114 |

Kirişler: eğilme D/C 1,000; kesme D/C 0,981 (B_S1, s = smax = 245; gerekli Av/s dayanımdan 0,619 > Av,min 0,208) ve **1,000 (B_S2, B_S3: s = 203,7 < smax 220, aralığı dayanım belirliyor)** → V2-a B2 dayanım kuralını doğrudan sınar (dış hakem md. 5 bu tasarımla karşılanabilir). Pilot ölçütü OK (T = 0; kritik D/C ≥ 0,8; en büyük 6.2.6 oranı 1,316). Dosyalar: `EX2_ATO_plain_g9_fr1.00_fp90_run01.csv`, `v2a_design_x.txt`, `v2a_design_eval.csv`, `pilot_console.txt`; `label_draft/` = aynı Ex2Model, rapor etiketi düzeltilmeden önceki runner ile özdeş koşu (sayılar aynı).

### 10 000 FE süre (`ex2_work/timing_10k/`, `--dop 1`, tohum 9101; paket 2 ölçümü `superseded_pkg2/`)
| Senaryo | YAKO | BTO | ATO |
|---|---|---|---|
| Lifsiz (18 değişken) | 46,25 s (4,62 ms/FE) | 46,98 s (4,70) | 46,68 s (4,67) |
| Lifli 90 TL/kg (24 değişken) | 64,68 s (6,47) | 51,70 s (5,17) | 51,47 s (5,15) |

Paket 2'ye göre ≈ %40–45 uzun (ötelenmeli kat varken her FE'de 2 ek FEM çözümü). Bilgi (raporlanmaz): altı koşu T = 0; lifli koşularda lif = 0; en büyük 6.2.6 oranları 1,30–1,38; tüm en iyilerde S1/S2'de Q > 0,05.

### Dondurulmuş ikili (`03_Code_Execution/ex2_bin/` + `SHA256SUMS_ex2.txt`; henüz kampanya ikilisi DEĞİL, ETABS V2-a yeniden doğrulaması sonrası kesinleşir)
`sha256sum -c` OK; `ex2_bin/Ex2Runner.exe --evalonly` pilot 2 sonucunu aynen üretti.
| Dosya | SHA-256 |
|---|---|
| Ex2Model.dll | `d0514e2f1e270f368333cb15c733cd90478d1bbcf7e66e341106ff122b82c6b3` |
| Ex2Runner.exe | `297b22f8bdf6db1818c62333e627cf1b18fd5a3969bb1b43f2bf261aeb67b7fa` |
| Ex2Runner.exe.config | `ad8267122c40d37dcd9d99e1412d763187a1825732f378db8867f3ecf675cf70` |
| FemCore / FemRcModel / RcCore / MathNet / ValueTuple | `ref/` ile aynı (§2.1) |

Kaynak SHA-256: Frame2D.vb `910350f1…9c7c`, GravityDesign.vb `84851c94…6d82`, Serviceability2D.vb `b2cba0ca…f41c`, Ex2Evaluator.vb `ccad5c97…fa60`, Ex2Problem.vb değişmedi, Ex2Runner/Program.vb `71aec11a…ea62`, Tools.vb `bc013a68…0f38`, Tests/Program.vb `300427d0…267c`, EtabsEx2/Program.vb `c6924b14…89fc` (yalnız yorum); Ex2Model.Tests.exe `4f61e501…f31d`. `dotnet build Ex2.sln -c Release`: 0 uyarı, 0 hata.

### Açıklar (paket 3 sonrası)
| # | Durum |
|---|---|
| BULGU-1 (6.2.6) | **Kodda KAPANDI** (PCR-20, g6); ETABS yeniden doğrulaması AÇIK |
| AÇIK-15 (lu) | **KAPANDI** (PCR-21); ETABS lu oranı ataması AÇIK |
| AÇIK-19 (Q > 0,05) | **KAPANDI** (PCR-22) |
| AÇIK-14 (kesme kesiti) | Kod değişmedi (iki hakem: düğümden d + beyan) |
| BULGU-2 (kiriş eksenel çekme) | AÇIK (kapsam kararı) |
| AÇIK-23 (yeni) | 6.2.6 ve 6.6.4.6.2 (δs ≤ 1,5) madde metinleri kanıt dosyasında A-satırı yok: AÇIK (Literature Analyst) |
| AÇIK-24 (yeni) | Ötelenmeli katta δns·abs(M2ns + δs M2s) zarfı (Cm = 1) muhafazakâr basitleştirme; ilgili ACI madde metni okunmadı: DOĞRULANMALI |
| V2-a yeniden | Yeni pilotla ETABS CD1–CD3, B1, B2: AÇIK |
| Dış hakem md. 4 / 6 | MC2010-yerel ana format önerisi; fiyat = 0 sağlama sınaması: bu pakette ele alınmadı |

## 5f. ETABS V2-a yeniden doğrulaması + fiyat = 0 sağlama sınaması (2026-11-16) — Ex2 kodu DEĞİŞTİRİLMEDİ

Git kullanılmadı; `program_work/`, `phase2_bin/`, `phase3_bin/`, `ex2_work/ref/`, `ex2_bin/` ve Ex2 kaynakları değiştirilmedi.

- **Kanıt**: `01_Literature/CODE_PROVISIONS_EVIDENCE.md` A55 (6.2.6), A56 (6.6.4.6.2), A57 (6.6.4.6.1/.3/.4), A58 (6.6.4.5.4) eklendi → **AÇIK-23 KAPANDI**; **AÇIK-24**: 6.6.4.6.4 ötelenmeli kolonlarda boy boyunca etkinin 6.6.4.5 ile (Cm, 6.6.4.6.1'deki M1, M2 ile) hesabına izin veriyor; programın Cm = 1 zarfı bu yolun muhafazakâr biçimi (yorum) → DOĞRULANDI (madde), 6.6.4.6.3 programda yok (sınırlama; gravitede M2s = 0).
- **V2-a yeniden** (`EX2_VALIDATION_REPORT.md` §7; araç `ex2_work/EtabsEx2_rerun/`, ilk araç `EtabsEx2/` değişmedi; çıktı `04_Results/ex2/v2a_etabs_rerun/`): PILOT2 ve K1, donmuş `ex2_bin` Ex2Model `d0514e2f…`. lu ataması birebir (0,00 mm); CD1 = 0 / 0 (K1'de ETABS O/S 2 = program g6 ihlali; eşleşme 12/12); CD2 0,976–1,155; CD3 güvensiz %0,19; PILOT2'de ETABS O/S yok; ETABS-örtük 6.2.6 oranları programla aynı; B2 ETABS sabitleriyle 1,0000 (ham 0,965–0,969); B1 alt 0,967–0,974 (yalnız BULGU-2; eksenel düzeltmeyle 0,998–1,003). **Yeni BULGU yok.**
- **Bilgi** (BULGU değil, donmuş DLL): `ColumnDesign.EminBase` = 15,24 mm (US 0,6 in); ACI 318M 6.6.4.5.4 = 15 mm (A58) → program 0,24 mm muhafazakâr.
- **BULGU-2** açık kalır (kapsam kararı).

### Fiyat = 0 sağlama sınaması (dış hakem şartı; KAMPANYA DIŞI, sonuç bölümüne girmez)
Kopya `ex2_bin` (scratchpad'e kopyalandı, `sha256sum -c` 8/8 OK). `Ex2Runner --method BTO|ATO --fibre 1 --fibreprice 0|90` ve `--fibre 0`, `--runs 5 --fes 10000 --seed0 9100 --dop 5` (tohumlar **9101–9105**), ana format (Q17, `delta`), qcheck açık. Çıktı `04_Results/ex2/price0_check/` (30 koşu dosyası, `console.txt`, `price0_summary.csv`, `price0_stdout.txt`; özet aracı `03_Code_Execution/tools/price0_summary.py`). Lif dozu > 0 tasarım vektöründeki doz indeksinden (kiriş grubu başına) okundu.

| Senaryo | Yöntem | Maliyet medyan (min–maks), TL | T = 0 | Lif dozu > 0 olan koşu | Lif (kg) |
|---|---|---|---|---|---|
| Lifli, lif 0 TL/kg | BTO | 228 333,82 (228 294,22–228 340,77) | 5/5 | **5/5** (3 grup; 1 koşuda B_S3 bir alt doz) | 230–240 |
| Lifli, lif 0 TL/kg | ATO | 228 294,22 (228 294,22–228 333,82) | 5/5 | **5/5** | 230–240 |
| Lifli, lif 90 TL/kg | BTO | 230 321,77 (230 308,35–230 325,40) | 5/5 | 0/5 | 0 |
| Lifli, lif 90 TL/kg | ATO | 230 308,35 (tüm koşular) | 5/5 | 0/5 | 0 |
| Lifsiz | BTO | 230 308,35 (230 308,35–230 325,40) | 5/5 | — | — |
| Lifsiz | ATO | 230 308,35 (tüm koşular) | 5/5 | — | — |

Yorum (rejime bağlı, genellenemez): **bu yük/kısıt rejiminde** (Ö2 gravite, ana kesme formatı Q17, C30, 4D/5D lif ızgarası, ÇŞİDB diğer fiyatlar) lif fiyatı 0 olduğunda her iki yöntem her koşuda üç kiriş grubuna da lif seçti ve en iyi maliyet lifsiz/90 TL/kg sonucundan ≈ 2 014 TL (%0,87) düşük çıktı; 90 TL/kg'da hiçbir koşu lif seçmedi ve lifli senaryo lifsizle aynı tasarıma indi. Sağlama amacı karşılandı: optimizasyon lif değişkenine erişebiliyor ve lifin (diğer kalemlerde) sağladığı tasarrufu bulabiliyor (kalem ayrımı bu sınamada incelenmedi); 90 TL/kg'da lifin seçilmemesi arama hatası değil, bu rejimde lif maliyetinin tasarrufu aşmasıyla tutarlı. Kaba gösterge (hesaplanmış başabaş değil): 2 014 TL / 240 kg ≈ 8,4 TL/kg. 5 koşu/hücre, istatistik testi yapılmadı. Süre: ≈ 13–16 ms/FE (5 paralel iş).

## 5g. Paket 4 (2026-11-17): post-hoc PCR-23/24/25 + kampanya 2 (iç Critic `Review_Round_Ex2_sonuc_ic.md` A1/K6 + dış hakem `_dis.md` R1–R3; IA "hakemlerle karar al" yetkisi)

Git kullanılmadı; `program_work/`, `phase2_bin/`, `phase3_bin/`, `ex2_work/ref/`, `ex2_bin/`, `ex2_campaign_bin/` değiştirilmedi (V0: 35 dosyanın SHA-256'sı derleme öncesi/sonrası birebir); kampanya 1 çıktısı (`04_Results/ex2/campaign/`) yalnız okundu. ETABS çalıştırılmadı. Ön kayıt eki: `EX2_SPEC.md` §11.2 (kampanya 2 başlamadan önce yazıldı).

### Değişenler
| Dosya | Değişiklik |
|---|---|
| `Ex2Model/GravityDesign.vb` | **PCR-23:** `FlexFixed` (SFRC: her As için kendi sabit nokta lcs = max(h − c, 0,2h), \|Δlcs\| ≤ 10⁻⁶ mm, en çok 60 adım, yakınsamazsa son iki iterenin küçük φMn'li olanı; düz beton: `AciBeam.Flexure`). `PhiMn` = `FlexFixed(...).PhiMn` (soğuk başlangıç). `RequiredBending`: SFRC ikiye bölmesi (48 adım) her denemeyi **kendi** tutarlı kapasitesiyle yargılar (ılık başlangıç), ardından soğuk başlangıçlı `FlexFixed` ile φMn(As) ≥ Ms sağlanana dek `RaiseUntil` ile yükseltir; üst sınır denetimi de tutarlı kapasiteyle. `EpsT` aynı tutarlı lcs ile (eski sabit 0,7h kaldırıldı). **PCR-23(3)** (`consistentPlain`, `Ex2Options.FlexCheck`): donmuş `AciBeam.RequiredAs` φ iterasyonu 50 adımda bitiyor; geçiş bölgesinde (0,004 ≤ εt < 0,005) φMn(As) Ms'nin %0,04'e kadar altında kalabiliyor (yeni bulgu; g8 ile yakalandı) → `RaiseUntil`: göreli 10⁻¹²·2ᵏ adımlarla en küçük yeterli yükseltme (aşım ≤ 2×; k ≤ 40). Geçiş bölgesinde d(φMn)/dAs küçük olduğundan %0,04'lük moment açığı ≈ %1,7 donatı gerektirebilir (çoğunda εt < 0,004 → g3). **PCR-24:** `GroupTension` (istasyon N'si çekme pozitif; T24 ile doğrulandı: S1/S2 kirişleri çekmede, çatı basınçta), `Required(Mu, Nt)` = As(Mu − Nt(d − h/2)) + Nt/(φ fy) (φ eğilme parçasının şekil değiştirmesinden), `BendingSteel` (As − Nt/(φ fy) sabit noktası; Pn = Nt/φ), `FaceDc`, `EpsT(…, Nt)`; kesmede `AxialFactor` = max(0; 1 − Nt/(3,5 b h)) (ACI 22.5.7.1) Vc,ACI'ye; ΔVc tüm kesme formatlarında Vc,f'den düşülür; Nt = 0'da tüm yollar bit-bit eskisi. `BeamGravityResult.NtMax`, `BeamShearGravityResult.AxialFactor`; `DesignGroup(..., withAxial, consistentPlain)`; `RequiredFace` (test kancası) |
| `Ex2Model/Ex2Evaluator.vb` | `Ex2Options`: **Patterned = True** (PCR-25, ACI 6.4.2 desenleri tasarım zarfında: 26 kombinasyon, kiriş + kolon + Q/δs), **BeamAxial = True** (PCR-24), **FlexCheck = True** (PCR-23(3)); `Ex2Options.Campaign1()` = üçü kapalı (kampanya 1 kuralları). Kısıt **g8** "beam flexure D/C <= 1 (ACI 9.5.1; envelope incl. ACI 6.4.2 patterns)" = min(DcFlex − (1 + 10⁻⁶); 10), yalnız SectionOk ve εt ≥ 0,004 olan gruplarda (g3 ihlalinde çift ceza yok), yalnız FlexCheck açıkken listeye girer |
| `Ex2Runner/Program.vb` | `--patterned` (varsayılan 1), `--beamaxial` (1), `--flexcheck` (1); koşu dosyası 2. satırına `wallSeconds`'tan sonra `,patterned,…,beamaxial,…,flexcheck,…`; `--patterned 0 --beamaxial 0 --flexcheck 0` = kampanya 1 kuralları |
| `Ex2Runner/Tools.vb` | `--evalonly` kiriş satırına `Nt_kN`, `VcAxialFactor`; `--patterns` aynı `FaceDc` ve seçeneklerle; `--marginal` seçenekleri aktarır |
| `Ex2Model.Tests/Program.vb` | T1–T22 açıkça `Campaign1()` kurallarıyla (önceki sayılar korunur); **T23** (PCR-23: 470 durumluk tarama b 250, h 400–600, 5 tip/doz, Mu 60–250 kN·m → Mu/φMn(Required) ≤ 1 + 10⁻⁹ (en büyük 0,999999999999997), en küçüklük (1 ppm az donatı → D/C > 1), tutarlı lcs; F0 B_S1 alt **694,75 mm²** (eski 687,5; Critic 694,8); lifsiz yol = `AciBeam.RequiredAs`; düz beton FlexCheck 5 532 durum, donmuş RequiredAs en büyük D/C 1,000392 → düzeltilmiş ≤ 1); **T24** (PCR-24: formül, D/C = 1, moment dengesi, istasyon N işareti, 22.5.7.1 katsayısı, eksenel kapalıyken katsayı = 1); **T25** (PCR-25: varsayılanlar, 26 kombinasyon, desenli Mu ≥ tek desen, g8 listede ve 0, kampanya 1'de g8 yok, 120 rasgele tasarımda εt ≥ 0,004 olan 259 grupta D/C ≤ 1 + 10⁻⁶, kolon desenli zarfta) |
| yeni: `ex2_work/O1RequiredAudit/` | Ö1 salt-okunur denetim aracı (Ex2.sln dışında; `04_Results/ex2/o1_required_audit.md`) |
| yeni: `run_ex2_campaign2.sh`, `tools/ex2_reeval_pcr23_25.sh` | kampanya 2 betiği; kampanya 1 son vektörlerinin üç kural setiyle `--evalonly` taraması |
| `tools/ex2_stats.py` | §1.1 `violation_counts` tablosu; bağ testleri "uygulanamaz (bağ)"; kaynak klasörü ve etiket parametreli. `04_Results/ex2_stats.md` yeniden üretildi (diğer bölümler birebir aynı) |

### Testler: 124 geçti, 0 kaldı (önceki 101)
V1: T3 1,2D+1,6L = **3 506,010 kN** (bağıl hata 1,3×10⁻¹⁶) geçiyor. Çıktı `04_Results/ex2/campaign2_prep/unit_tests_124.txt` (ilk dondurulan ikilinin 123 testi `superseded_unit_tests_123_binary74d94426.txt`). T23'e eklenen test: FlexCheck, donmuş RequiredAs zaten yeterliyse donatıyı değiştirmez; en büyük yükseltme %1,72 (geçiş bölgesi). `dotnet build Ex2.sln -c Release`: 0 uyarı, 0 hata.

### Lifsiz bit-bit regresyon (PCR-23 (1)–(2))
Yeni ikili, `--patterned 0 --beamaxial 0 --flexcheck 0`, lifsiz YAKO/BTO/ATO × tohum 1001–1003: 9/9 koşu dosyası kampanya 1 ile sürüm satırı ve `wallSeconds` dışında **birebir** (`ex2_work/regression_pcr23/regression_vs_campaign1.txt`). İlk denemede g8 kampanya 1 kurallarında da açıktı ve εt ≥ 0,004 olan arama noktalarında düz beton RequiredAs açığını yakaladığı için (ATO run01'de 18 değerlendirme) yörüngeyi değiştirdi → g8 ve düz beton düzeltmesi `FlexCheck` seçeneğine bağlandı (kampanya 2 kuralı).

### PCR-24 ↔ ETABS V2-a B1 (ETABS koşulmadı; `04_Results/ex2/v2a_etabs_rerun/raw/*_beam_groups.csv`)
`04_Results/ex2/campaign2_prep/b1_pcr24.csv`: program (desen yok, eksenel açık) / ETABS gerekli As: üst **1,0000**, alt **1,0000** (kampanya 1 kuralları 0,967–0,988); Nt program = ETABS tanısı (13,20 / 14,19 / 15,57 / 18,73 kN). (İlk dondurulan ikili `74d94426…` FlexCheck'te sabit %0,01 adım kullanıyordu ve kayan nokta eşitliğinde bile bir adım atarak B1 = 1,0001 ve lifsiz maliyette +%0,001 veriyordu; adım geometrik en küçük yükseltmeye çevrildi, ikili yeniden donduruldu, o ikiliyle başlatılan kampanya 2 ilk 6 dakikada durduruldu (tamamlanan koşu yok; günlük `campaign2/aborted_start_binary74d94426_log.txt`).) Kesme katsayısı 0,957–0,973 (ETABS kesmede eksenel kuvveti yok sayar → program daha muhafazakâr).

### PCR-25 kanıtı
`01_Literature/CODE_PROVISIONS_EVIDENCE.md` A59 (6.4.2, R6.4.2, 6.4.3, 6.4.3.2: L ≤ 0,75D muafiyeti yalnız iki doğrultulu döşeme sistemleri; kirişlerde muafiyet BULUNAMADI → desen uygulanır), A60 (6.6.2.2 kolonlar), A61 (9.5.2.1, 22.5.7.1, PCR-24). Ö2'de L/D = 0,67 (öz ağırlıkla ≈ 0,60) olsa da muafiyet kiriş-çerçeveye uygulanmaz.

### Dondurulmuş ikili `03_Code_Execution/ex2_campaign2_bin/` (`SHA256SUMS_ex2_campaign2_2026-11-17.txt`, `sha256sum -c` 8/8 OK)
Ex2Model.dll `a61b0c6b4ee15114716cc437842a41a4225451b443bee512f57f3898c92c95d8`; Ex2Runner.exe `3a515a72ee579209b605fee1c768d725f11df1e359c664edb0e8a6529abc5983`; .config, FemCore, FemRcModel, RcCore, MathNet, ValueTuple kampanya 1 ile aynı. Kaynak SHA-256: GravityDesign.vb `5832d433…6edb`, Ex2Evaluator.vb `f9ee83e7…b213`, Ex2Runner/Program.vb `927483fd…82f9`, Tools.vb `c193b9c7…7893`, Tests/Program.vb `2d0d733d…d835`, Frame2D/Serviceability2D/Ex2Problem değişmedi; Ex2Model.Tests.exe `af3a1dd1…4d91`; run_ex2_campaign2.sh `d20942a8…cdb`; ex2_reeval_pcr23_25.sh `d30de09b…a61`; ex2_stats.py `7efdc0a7…d1d6`; O1RequiredAudit/Program.vb `d59d9e37…a712`.

### Süre (bilgi, makaleye girmez)
10 000 FE, `--dop 1`, tohum 9101, başka işlerle eşzamanlı: lifsiz ATO 188 s (18,8 ms/FE), lifli ATO 221 s (22,1 ms/FE) — kampanya 1'in ≈ 4 katı (26 kombinasyon, desenli kat stabilitesi).

### Kampanya 2
`run_ex2_campaign2.sh` yeniden dondurulan ikiliyle başlatıldı (nohup; saat aşağıdaki doğrulama satırında); 270 koşu, `04_Results/ex2/campaign2/`, günlük `campaign2_log.txt`, son satır "ALL DONE ex2 campaign2". Yeniden başlatma: `cd 03_Code_Execution; nohup bash ./run_ex2_campaign2.sh > /dev/null 2>&1 &` (var olan koşu dosyalarını atlar).

### Kampanya 1 son vektörlerinin yeniden değerlendirmesi (`--evalonly`; `04_Results/ex2/reeval_pcr23_25/reeval_summary.md`, `reeval_runs.csv`; araç `tools/ex2_reeval_pcr23_25.sh` + `tools/ex2_reeval_summary.py`)
Üç kural seti: old (kampanya 1 ikilisi), p23 (yeni ikili `a61b0c6b…`, kampanya 1 yük kuralları + PCR-23 (1)–(3)), new (kampanya 2 kuralları). Yeniden optimizasyon değildir.
- **Güvenlik taraması (old):** lifli 0 TL/kg hücresinde 90 son vektörün **52'sinde** kiriş eğilme D/C > 1 + 10⁻⁶ (en büyük **1,0103**); lifsiz ve 90 TL/kg hücrelerinde 0 (D/C ≤ 1,0000). Tüm 270'te T = 0 (DcFlex kısıt değildi).
- **p23:** lifsiz ve 90 TL/kg maliyetleri değişmedi (medyan %0,000); 0 TL/kg medyan **+%0,027** (−%0,001 – +%0,028), D/C ≤ 1,0000, T > 0 yok. F0 (`EX2_ATO_fibre_g9_fr1.00_fp0_run01`): 228 294,22 → **228 357,12 TL** (+62,90; iç Critic tahmini ≈ +63). P ile fark 1 951,23 TL (%0,85); F0 tasarımı başabaşı 1 951,23/240 = **8,13 TL/kg** (eski 8,39) — deterministik, yeniden optimizasyon değil; kampanya 2 sonuçları esastır.
- **new:** maliyet medyanı **+%2,83–2,87** (aralık +%2,27 – +%4,23); 90 TL/kg hücresinde 2 vektör g6 (6.2.6) ile uygunsuz, diğerleri T = 0 (donatı yeniden türetildiği için). P 236 825,02 TL, F0 234 852,10 TL (F0 1 972,92 TL daha ucuz; yalnız bu iki sabit kesit için).

### Kampanya 2 başlangıç doğrulaması
Başlangıç 2026-11-17 11:34:48 (ikili `a61b0c6b…`, `sha256sum -c` 8/8 OK, günlük `04_Results/ex2/campaign2/campaign2_log.txt`). İlk 7 lifsiz koşu dosyası (YAKO/ATO; tohumlar 1001, 1007, 1012, 1015, 1018, 1023, 1026): sürüm satırı `a61b0c6b…`, FEs = 10 000, `patterned,True,beamaxial,True,flexcheck,True`, **T = 0**, `violation_counts` satırı g1–g6 sayılarıyla dolu. Süre ≈ 790–810 s/koşu (≈ 80 ms/FE, 8 paralel iş; ilk dakikalarda yeniden değerlendirme işi de koşuyordu) → 270 koşu tahmini ≈ 7–8 saat (bilgi). Sonuçlar bu belgeye yazılmadı (kampanya bitmeden yorum yok).
