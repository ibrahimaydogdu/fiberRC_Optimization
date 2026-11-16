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

### 10 000 FE süre ölçümü (ex2_work/timing_10k/)
SURE_TABLOSU_YER_TUTUCU

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

## 6. Yeniden üretim

```
cd 03_Code_Execution/ex2_work
dotnet build Ex2.sln -c Release
Ex2Model.Tests/bin/Release/net472/Ex2Model.Tests.exe          # 34/34
Ex2Runner/bin/Release/net472/Ex2Runner.exe --method ALL --runs 3 --fes 100 --seed0 900 --dop 1 --out pilot_100FE
```

Sıradaki adımlar (IA onayıyla): Q17/Q18 → `IBeamShear` lifli uygulaması ve blok kaldırma; Q20 → V2-a uygun tasarım; ETABS V1/V2-a (rijit bölgesiz, NonSway, tek desen); Q19 → koşu protokolü ön kaydı.
