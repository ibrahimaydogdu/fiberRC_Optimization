# Review Round V2h: V2f/V2h (R1–R8 kapanışı, radyal D/C, ACI 318M-14 Bölüm 18 zinciri) (Critic, 2026-10-30)

Denetlenen: `04_Results/VALIDATION_REPORT.md` V2f (170–214) ve V2h (216–250); `03_Code_Execution/PROGRAM_CHANGE_REQUESTS.md`; `VALIDATION_SPEC.md` (13–29); `program_work/FemRcModel/{ColumnDesign,BeamDesign,Checks,Ex1Evaluator,Combos}.vb`; `FemRcModel.Tests/Program.vb`; `EtabsProbe/{Design,DesignBeam,Program}.vb`; `runs/` (58 dosya, `SHA256SUMS_2026-10-30.txt`); `Evidence/DesignGuides/ACI 318M_14.pdf` ve `CFD-ACI-318-14.pdf` (pdftotext). Kod değiştirilmedi. Python yalnız ham logları ayrıştırmak için kullanıldı.

ACI madde metinleri bizzat okundu ve koda karşı kıyaslandı: 18.6.4.4, 18.6.5.1–2, 18.7.3.2, 18.7.5.1–5 + Tablo 18.7.5.4, 18.7.6.1.1, 18.7.6.2.1, 18.8.2.1, 18.8.4.1–3, ayrıca 18.8.2.3–4, 21.2.4.1/21.2.4.3. (pdftotext √ simgesini düşürüyor; katsayılar 1,7/1,2/1,0 ve CFD'deki 20/15/12 ile tutarlı.)

## KARAR
- **Lifsiz (kolon/kiriş/birleşim zinciri, Ö1 lifsiz kampanya): [REVİZYON TALEBİ: aşağıdaki K1–K6 ve K9–K10 kapanmadan kampanya açılmaz.]** Radyal D/C ve R1/R3/R4 gerçekten ilerledi; ancak ACI kolon/birleşim zinciri için tek bir doğrulayıcı (assert'li) test yok, "birim testleriyle doğrulandı" ifadesi yanlış; Q ve δs kısıtı yok; ΣMpr paylaşımı ve φv gerekçesiz.
- **Lifli (SFRC kirişli) kampanya: [REVİZYON TALEBİ: lifsiz için K1–K10 + S2 (sonuçsuz) + lif fiyatı.]** Lifli kirişin Mpr/Mnb/Vc=0 davranışı hiçbir deneyle sınanmadı (S2 sonuçsuz); lifli sonuçlar yalnız "model sonucu" etiketiyle sunulabilir ve hakem onayı verilmez.
- Kampanya (AGENTS 2.10) ve eşik onayı IA'dan **ayrıca** alınmalıdır (bkz. §6).

## 1. R1–R8 durumu (Soru 1)

| R | Durum | Kanıt |
|---|---|---|
| R1 kanıt bütünlüğü | **KARŞILANDI (V2h metni eksik)** | `sha256sum -c`: 61/61 OK, liste kendini içermiyor, `runs/` 57 dosya + 4 ikili hepsi listede (bizzat). ECCDIAG, NORZ=1, 116 eleman RS, QCHECK logları var: RS EX maks ΔM3 %0,114, ΔV2 %0,133; EY %0,071/%0,071 (log:85, 88); NORZ=1 taban kesmesi ve köşe yer değiştirmesi %0,000–0,001. Eksik: Rapor:199'daki özetler (EtabsProbe 4c369899…, FemRcModel fe5ee93e…) güncel listeyle (7e8e0e69…, 6f2e266e…) uyuşmuyor; V2h için derleme zamanı/özet ve log→derleme eşlemesi yok (V2f logları 17:05, V2h 18:34, exe 18:35). Bkz. K9. |
| R2 hold-out, ≥20 bağımsız satır | **AÇIK** | V2h logları: ETABS D/C ∈ [0,8; 1,2] satırı 6+6+2+0+0 = 14 (C111/C211 ve benzer eşleşmeler yüzünden bağımsız ≈7), ikinci yapı yok; V2h bunu hiç anmıyor. Bkz. K8. |
| R3 narinlik girdileri | **KARŞILANDI (Q kısmı eksik)** | lu = net yükseklik (`ColumnDesign.vb:200, 227`), ETABS'te `SetOverwrite` ile lu/K/Cm yazılıyor (`Design.vb:142–149`), sismik δns, δns birim testi 1,2018. Q yalnız deneme tasarımı için hesaplandı; `V2f_QCHECK` EY kat 2 Q(3Δ) = **0,0504 > 0,05** (rapor "tam sınırda" diyor); Q kısıtı yok. Bkz. K3. |
| R4 üretim donatı düzeni | **KARŞILANDI** | `PerimeterCounts`/`transposed`, ETABS'te aynı düzen, 2 kare olmayan konfigürasyon (500×350, 300×350) min oran 0,988/0,987; NR3/NR2 swap farkı U1'de %1,6 ve V2h'de 500×350 oranı 0,986–1,136: eşleme pratikte çözülmüş. |
| R5 CD3/CD2/rapor/spec | **KISMEN** | CD3 radyal D/C ile güvensiz yönde kapandı (bkz. K7), ama Spec güncellenmedi, rapor başlığı/Z6/kapı tablosu eski (K9, K10); CD2 tanımı hâlâ δns'siz (V2e D4). |
| R6 birim testler | **ZAYIF** | Sayı 62 → 75 → 76; ancak V2h zinciri (kolon Ve, Tablo 18.7.5.4, ΣMnc/ΣMnb, birleşim kesmesi) için **sıfır assert** (K1). 8 işaret deseni için test yok (V2f kabul etti). |
| R7 TBDY mi ACI mı | **KARARA BAĞLANDI (IA)** | ACI Böl. 18; Spec ve makale kapsamı güncellenmedi (K10). |
| R8 burulma sınırlaması | **KARŞILANDI (metin yazılı, ikinci yapı yok)** | `VALIDATION_REPORT.md:206–207`. Makale taslağı yok (`05_Drafts/Paper1` boş), sınırlama yalnız raporda. |

## 2. ACI zinciri ↔ ACI metni (Soru 2)

**Metne uyan (bizzat doğrulandı):**
- Mpr: 1,25 fy, φ = 1 (`BeamDesign.vb:111–117`, `ColumnDesign.vb:275, 335`); ACI 18.6.5.1, R18.6.5, 18.7.6.1.1, 18.8.2.1 ✓.
- Kiriş Ve = (Mpr,üst + Mpr,alt)/ln + Vdy; Vc = 0 koşulu Vseis ≥ 0,5 Ve (`BeamDesign.vb:141–144`) ✓ 18.6.5.2; hoop 50 mm / d/4 / 6 db / 150 (`:152`) ✓ 18.6.4.4; Vs ≤ 0,66√f'c bw d ✓.
- Tablo 18.7.5.4: (a) 0,3(Ag/Ach − 1) f'c/fyt, (b) 0,09 f'c/fyt, (c) 0,2 kf kn Pu/(fyt Ach); tetik Pu > 0,3 Ag f'c veya f'c > 70; kf, kn (`ColumnDesign.vb:379–390`) ✓. bc = B − 2·TieCover (merkezden merkeze), Ach = dış-dış (`:370, 386`) ✓. Ash/s karşılaştırması ve ayak sayısı tutarlı (V2 doğrultusu, bw = B, ayak sayısı nB).
- lo = max(h, ln/6, 450) (`:405`) ✓ 18.7.5.1; sınır = min(min boyut/4, 6 db, so), so = 100 + (350 − hx)/3 ∈ [100,150] (`:376–378`) ✓ 18.7.5.3; lo dışı min(6 db, 150) (`:403`) ✓ 18.7.5.5.
- Kolon Vc = 0: Pu < Ag f'c/20 (en küçük Pu, deprem dahil) (`:383`) ✓ 18.7.6.2.1(b); (a) "Ve tümüyle deprem kaynaklı" varsayımı makul.
- Ve ≥ analiz kesmesi (`:365`) ✓ 18.7.6.1.1 son cümle.
- ΣMnc ≥ 1,2 ΣMnb, nominal Mn, φ = 1, muafiyet yok (`Checks.vb:49–113`) ✓ madde 18.7.3.2 (muafiyet yalnız 18.7.3.3 ile).
- Birleşim: Vu = 1,25 fy (As1 + As2) − Vkol, Vkol = ΣMpr/H, çatıda 0 (`Checks.vb:174–175`) ✓ 18.8.2.1 ve CFD §3.6.1 (CFD aynen "no column at the top of the joint → shear taken as zero"); Vn katsayıları 1,7/1,2/1,0, λ = 1, φ = 0,85 (`:198–199`) ✓ Tablo 18.8.4.1, 21.2.4.3; bj = min(kolon genişliği, bw + h) ✓ 18.8.4.3 (ortalı kirişte ikinci sınır kolon genişliği). Kuşatma sayımı (4 yüz → 1,7; 3 yüz veya 2 karşılıklı yüz → 1,2; diğer → 1,0, `:196–198`) ✓ 18.8.4.1–2 (köşe: 2 komşu yüz → 1,0 doğru).

**Hata/eksik (dosya:satır):**
- **[YÜKSEK] K5. φv = 0,75 kolonda gerekçesiz** (`ColumnDesign.vb:273`; kiriş için `BeamDesign.vb:107` sorunsuz). ACI 21.2.4.1: φ = 0,60 "member'ın nominal kesme dayanımı, nominal moment dayanımına karşılık gelen kesmeden azsa". Kolon Ve'si kirişlerin ΣMpr'si ile sınırlandığında (`:361–364`) Vn = Ve/0,75 kolonun kendi Mn'ine karşılık gelen kesmeden küçük olabilir; ETABS tercihinde de "Phi shear seismic = 0,6" (`runs/V2h_ETABS_joint_not_done…:23`). Ya φ = 0,60 alınmalı ya da Vn ≥ 2 Mnc/ln koşulu kontrol edilmeli.
- **[ORTA] K6a–d. Güçlü kolon–zayıf kiriş**: (a) çatı eklemi hariç (`Checks.vb:56`, `NStory − 1`); ACI 18.7.3.1–3'te çatı/üst kat istisnası yok, CFD 3.6.4 da yalnız "alt kolonu olan eklem" koşulu koyar; (b) Mnc yalnız en küçük Pu'da (`:78–84`), en büyük Pu ucu ve çekme için `Math.Max(pu, 0)` (`:79`) net çekmede Mn'yi abartır; ACI "en düşük eğilme dayanımı" der, aralığın iki ucu değerlendirilmeli; (c) Mnb ve Mpr döşeme donatısını içermiyor (18.7.3.2 T-kiriş cümlesi; ΣMpr/Vu'yu da düşük bırakır); modelde döşeme çubuğu yoksa varsayım olarak yazılmalı; (d) `jr.Exempt` ölü kod (`:111–112`).
- **[ORTA] K11. Zincirde tanımlı ama kısıt/maliyeti olmayan ACI maddeleri:** 18.8.2.3 (kolon boyutu ≥ 20 db kiriş çubuğu: 20 mm → 400 mm, alt sınır 300 mm), 18.8.2.4 (kolon h ≥ kiriş h/2), 18.7.2.1(b) (en küçük/dik boyut ≥ 0,4), 18.7.5.2(e)(f) (hx ≤ 350/200), 18.8.3 eklem içi enine donatı (`Ex1Evaluator.vb:90`: etriye yalnız net boyda; eklem bölgesi maliyeti 0). Kısıt eklenmeli veya makalede dışarıda tutulduğu yazılmalı.
- **[DÜŞÜK] K14.** Çekmede kolon Vc azaltılmıyor (`ColumnDesign.vb:392` `Math.Max(NdMin, 0)`; ACI 22.5.7), yalnız orta bölge; kuşatma testi 0,75·B (bj değil) (`Checks.vb:191–193`); MprMax 9 nokta örnekleme (`:304–307`).
- **[ORTA] K13. Vdy yükü:** `BeamDesign.vb:124` G + Q + 0,2 S (1,0 çarpanlar); ACI 18.6.5.1 "factored tributary gravity load" ve Denk. 4.11 (1 + ez) G içermiyor (ez ≈ 0,14). V2c B3 oranının 0,964–0,985 (<1) çıkması ile uyumlu sistematik düşüklük olabilir (kanıt değil, hipotez).

## 3. Kolon kesmesinin ΣMpr ile sınırlanması (Soru 3)
- **ACI uyumu: izin verilmiş.** 18.7.6.1.1 "column shears need not exceed those calculated from joint strengths based on Mpr of the beams"; R18.7.6.1.1 "distribution ... should be based on analysis". Çatıda tamamı, zeminde sınırsız (Mpr,kolon) doğru ve CFD ile aynı yönde (`ColumnDesign.vb:359–364`).
- **Güvenlik: koşullu.** Sabit 0,5 paylaşım (`:361, 364`) analize dayalı değil. (i) Kolon kesiti yukarı doğru küçüldüğünde alt kolon payı > 0,5 olur → alt kolonun Ve'si eksik tahmin (güvensiz); (ii) çatı kolonunun alt ucu 0,5 pay alır: Ve = (ΣMpr,çatı + 0,5 ΣMpr,alt)/ln ≈ 1,5 ΣMpr/ln = 0,625 ΣMpr/m (ln = 2,4 m) iken ETABS Veb = ΣMpr/(H/2) = 0,667 ΣMpr/m (CFD §3.4.4.1): **~%6 düşük**; (iii) ara katlarda Σ/ln = 0,417 vs ETABS Σ/H = 0,333: ~%25 yüksek (güvenli). Sonuç: ortalama güvenli, çatı ve kesit geçişlerinde güvensiz. Düzeltme: EI/L ile paylaşım ya da muhafazakâr üst sınır (paylaşımsız tam ΣMpr her uca), gerekçesi tabloda. Bu karar IA yetkisi kapsamında Claude'a ait; yapılmadan kampanya olmaz.

## 4. Radyal D/C ve CD3 (Soru 4)
Bizzat yeniden hesaplanan (`runs/V2h_radial_*`):
- U1 üretim/ETABS maks D/C: 0,375/0,376; 0,479/0,484 (−%1,0); 0,838/0,843 (−%0,6); 0,979/0,981 (−%0,2); 1,113/1,116 (−%0,3). Güvensiz yön ≤ %1,0 ✓ (Rapor:230 doğru).
- Satır düzeyi (1200 satır): yanlış-uygun 0, yanlış-uygun-değil 4; min oran 0,961 (log "0,960/0,961", yuvarlama).
- **[ORTA] K7. CD3 "GEÇTİ" yalnız güvensiz yön için doğru.** Spec güvenli yönü ≤ %15 sınırlar. Deprem birleşimlerinde üretim/ETABS maks D/C: 500/400 → 0,407/0,316 (+%28,8), 0,302/0,230 (+%31,3), 0,414/0,320 (+%29,4), 0,309/0,235 (+%31,5); diğerleri +%7,7…+%12,6. Rapordaki "deprem birleşimlerinde çoğunlukla %0–10 yüksek" (Rapor:231) logla çelişir: 20 deprem girdisinden 16'sı > +%10. Düşük D/C bölgesidir (< 0,45, kısıtı bağlamaz), ama ya Spec CD3'e "D/C ≥ 0,8 için" koşulu IA onayıyla önceden yazılmalı ya da sonuç "güvensiz yönde geçti, güvenli yönde 500/400'de aştı (muhafazakâr 8 işaret deseni)" diye dürüstçe yazılmalı.
- **Yeterlilik:** Radyal D/C'nin ETABS ile uyumu 5 konfigürasyonda ikna edici. Sınırlar: (i) enterpolasyon testi tek kesit (400×400), 6 yük noktası, tol. %0,5, ölçülen %0,30 (`Tests/Program.vb:355–364`); çekme ve yüksek eksenel uç bölge örneklenmedi; (ii) V2h'de ETABS 11 etkileşim noktası (`log:11`), V2f'de 41 nokta ETABS D/C'yi %1,3'e kadar düşürüyor; yani ETABS(11) biraz yüksek; test üretim lehine değil aleyhine eğik, kabul edilebilir; (iii) probe'un "PRODUCTION" döngüsü `CheckGroup`'u çağırmıyor, mantığı yeniden yazıyor (`Design.vb:195–225`): `CheckGroup.DcrMax` ile birebir eşitlik testi yok [DÜŞÜK]; (iv) CD1 gücü zayıf (R2).

## 5. ETABS'te kolon kesmesi/birleşim kontrolü devrede değil (Soru 5)
**Gözlem (log):** `GetSummaryResultsJoint items=0`; `Concrete Joint Design Summary` 48/48 "Joint check not done." (`V2h_ETABS_joint_not_done…:410–423`), BC/JS oranları boş (`:424–`); kolon kesme zarfı 13,5 kN ve Av ≈ 0,00016 m²/m (analiz kesmesi + minimum); program/ETABS Av/s 1,8–17 (anlamsız oran). Çerçeve tipi ön-okumada "1 = Sway special (program determined)" (CSI API: 0 Default, 1 Sway special), SDC = 4 = D (API: 4 = D), "Design for B/C capacity ratio" = 2 = **Yes**, Phi joint shear 0,85. Yani bariz tercih kaynaklı nedenler elenmiş.

**Yeterli bağımsız kanıt mı? Hayır.** (a) ETABS kolon Ve/birleşim/SCWB karşılaştırması yok; (b) ETABS-dışı bağımsız kanıt da yok (K1: assert'li el hesabı testi yok); (c) kiriş kapasite kesmesi V2c'de ETABS'le 0,964–0,985 uyumlu (yalnız bu bileşen kanıtlı). "ACI metninden okundu" doğrulama değildir.

**Makaledeki sınırlama:** Rapor:244–246 doğru ve dürüst ("ETABS ile doğrulanmamıştır"). Ancak (i) makale taslağı yok (`05_Drafts/Paper1` boş), sınırlama henüz metinde değil; (ii) AGENTS 2.1 ("her optimum tasarım ETABS'te yeniden analiz edilip tasarım kapasite oranları okunmuş") bu bileşenler için sağlanamaz; bu bir kural istisnasıdır ve IA'nın açık kaydı gerekir; (iii) makalede "ETABS ile doğrulanmış kapasite tasarımı" biçiminde bir ifade kullanılamaz; kapsam şöyle yazılmalı: ETABS ile doğrulananlar (P-M D/C, kiriş As, gravite Av/s, kiriş Mpr kesmesi, FemCore yanıtları), doğrulanamayanlar (kolon kapasite kesmesi, ΣMnc/ΣMnb, birleşim kesmesi, sarılma).

**Makul hipotezler (hiçbiri doğrulanmadı; ayırt edici deney yanda):**
1. **Birleşen eleman tasarım sonucu yok:** CFD §3.6.4 koşulu "the connecting member design results are available". Joint koşusunda her 4 kiriş grubunda `SetRebarBeam ret=1` (`log:29, 31, 33, 35, 168–175`; V2c'deki DesignBeam yolunda hata yok) ve `SetDiaphragm ret=1`; kirişler tasarım modunda, kolon-kiriş eşleşmesi kurulmamış olabilir. Deney: aynı modelde kiriş donatısını V2c yoluyla (DesignBeam.vb:59) ata, `GetSummaryResultsBeam`'in dolu döndüğünü doğrula, sonra joint tablosuna bak.
2. **"Seismic load combo" tanımı:** CFD §3.6.4 "the load combo involves seismic load". Tasarım birleşimleri RS durumlarını içeriyor; ETABS bunları "deprem" saymıyor olabilir (yalnız Quake yük deseni?). Deney: aynı birleşimi statik Quake deseniyle (EX) kur veya GUI'de "Select Design Combos" ve "Seismic" işaretini kontrol et.
3. **Kiriş-kolon eşleşmesi/ofset:** Joint tasarımı "kolonun en üst istasyonunda" değerlendirilir (CFD §2.3); RZ = 1 rijit bölge ve uç ofsetleri istasyonu birleşim yüzüne taşıdığı için kirişler bulunamıyor olabilir. Deney: RZ = 0 (veya 0,5) küçük bir modelde yeniden dene.
4. **OAPI özgü / dosya durumu:** aynı EDB'yi ETABS GUI'sinde aç, "Concrete Frame Design → Display Design Info → Joint Shear / Column Shear" ve Detailed Design Info ile bak; OAPI tablosu boş, GUI dolu ise sorun OAPI/oturum durumudur. Sonuç ne olursa olsun CSI desteğine minimal örnek (tek katlı 1×1 açıklık) gönderilebilir.
- ETABS Help'te (`01_Literature/ETABS_Help_text/*.txt`, ETABS_chm ve CSI_API) "Joint check not done" durumunun tetik koşulunu veren metin **bulunamadı**; yukarıdakiler CFD kılavuzu (§2.7, §3.4.4.1, §3.6.1, §3.6.4) ve log gözleminden türetilen çıkarımlardır.

## 6. IA'ya ayrıca sorulması gerekenler (Soru 6)
"Alacağın tüm kararları onaylıyorum" yetkisi (MEMORY 3g: "yetki: Claude'un doğrulama/uygulama kararları") aşağıdakileri **kapsamaz**; açık IA onayı gerekir:
1. **Kampanya başlangıcı** (AGENTS 2.10: Critic onayı olsa da IA açık onayı şart). Lifsiz ve lifli ayrı.
2. **Kabul eşikleri ve kapı tanımı** (Spec ⚠️, R10): eşikleri aynı ajanın hem koyup hem onaylaması çıkar çatışmasıdır; CD2 (+δns), CD3 (güvenli yön sınırı/ D/C ≥ 0,8 koşulu), yeni ACI zinciri kapıları, R2 için feragat veya ikinci yapı. Eşikler koşudan **önce** IA onayı ile kilitlenmeli.
3. **AGENTS 2.1 istisnası:** kolon kapasite kesmesi, SCWB, birleşim kesmesi için ETABS doğrulaması yapılamıyor; makalede bu kapsamla yayına gidilecek mi, yoksa ETABS joint çalışana kadar bekleyecek mi?
4. **TBDY'den ACI Böl. 18'e geçişin sonucu:** Türkiye'deki A11 konut yapısında TBDY 7.3–7.5 detaylaması (sarılma, SCWB, birleşim) ve TS 500 kontrolü artık yapılmıyor; MEMORY 3f'de söz verilen "TS 500 Mr/Vc kontrol tablosu" sürüyor mu, düşüyor mu? Makalede "TBDY detaylaması denetlenmedi" ifadesi IA'nın bilerek verdiği karar mı?
5. **Lifli kampanya:** S2 sonuçsuz; lifli sonuçlar "model sonucu" etiketiyle ve fR çarpanı duyarlılığı (0,85/0,70, fFtu = 0) ile mi sunulacak? Lif fiyatı tedarikçi yanıtı bekleniyor.
6. **R2 hold-out:** 2B 6 elemanlı doğrulama örneği (FEM↔ETABS) ikinci yapı olarak mı kullanılacak, yoksa feragat mi?

Claude'un kendi yetkisinde (IA'ya sormadan yapabileceği): φv = 0,60 kararı, ΣMpr paylaşımı, SCWB çatı/çekme, Q kısıtı, ek ACI kısıtları, test ekleme.

## 7. Kampanyadan önce zorunlu şartlar (Soru 7)
- **C1 (K1)** Assert'li birim testler, el hesabıyla: `ColumnShear.Design` (Ve, `beams ≠ Nothing` ile ΣMpr sınırı çalışırken; Vc = 0 sınırı; Tablo 18.7.5.4 (a)/(b)/(c) üç tetik; so, lo), `JointMprSum`, `StrongColumn` (Pmin/Pmax/çekme), `JointShear` (Vkol, bj, kuşatma sayımı 4/3/2 karşılıklı/2 komşu, Vn). Çıkış: PASS sayısının bu testlerle artması.
- **C2 (K3)** Q (veya TBDY θ) kısıtı ya da final optimumda her kat için rapor; drift sınırına yaklaşan tasarımlar için δs; `Q(3Δ) = 0,0504` ifadesi düzeltilmeli. Kaba tahmin (benim hesabım): drift sınırında Δe/h = 0,008/(0,40 · 8) = 0,0025 → Q ≈ (ΣPu/Vus)(Δo/h) ≈ 29 × 0,0025 ≈ 0,07 > 0,05 (birinci kat); 300 mm kolonlu tasarımlar için de sınıra yakın.
- **C3 (K4, K5, K6, K13)** ΣMpr paylaşımı, φv (21.2.4.1), SCWB (çatı, uçlar, çekme), Vdy (ez), döşeme donatısı varsayımı; her biri gerekçesiyle tabloya.
- **C4 (K11)** 18.8.2.3, 18.8.2.4, 18.7.2.1(b), hx, eklem içi etriye maliyeti: kısıt ya da yazılı dışlama.
- **C5 (K8)** R2: ≥ 20 bağımsız satır (farklı fc, ρ 2–4 %, 300–400 mm, bandı hedefleyen konfigürasyonlar) + ikinci yapı (2B örnek); ya da IA feragati.
- **C6 (K9, K10)** Tek güncel kapı tablosu; Rapor:3, 99–113, 124–126, 190, 199, 209 güncellenmeli; V2h derleme zamanı ve ikili özetleri ve log→derleme eşlemesi; Spec'e CD2/CD3 revizyonu ve ACI zinciri kapıları; IA imzası.
- **C7 (§5)** ETABS joint/kapasite kesmesi için yukarıdaki en az iki ayırt edici deney (GUI + 1. hipotez) denenip kaydedilmeli; çözülmezse sınırlama metni ve el hesabı bağımsız kontrolü.
- **C8 (K12)** Final optimum için çubuk ayrıklaştırması + zincirin yeniden koşusu + ETABS yeniden analizi (P-M D/C, kiriş As, gravite Av/s, kiriş Mpr kesmesi).
- **C9** IA onayları (§6).

## 8. Kontrol listesi
| Madde | Durum |
|---|---|
| Rapor sayıları ham logla tutuyor | EVET (V2h tablo, CD2 min 0,961/0,960 yuvarlama); HAYIR ("%0–10 yüksek", K7) |
| SHA-256 listesi doğrulandı | EVET (61/61, bizzat); Rapor:199 özetleri bayat |
| Önceden tanımlı eşik ve eşikle tutarlı karar | KISMEN (CD3 güvenli yön, CD2 tanımı, IA onayı yok) |
| Karşılaştırma üretim koduyla | KISMEN (kolon P-M: evet fonksiyon düzeyinde, `CheckGroup` değil) |
| Doğrulanmamış yorumlar etiketli | KISMEN (ΣMpr paylaşımı, φv, ETABS joint) |
| "N/A" hücre | EVET (yok) |
| Birim disiplini | EVET |
| ACI kolon/birleşim zinciri birim testle sınanmış | **HAYIR** |

## 9. En önemli 5 madde
1. **K1/K2:** ACI kolon/birleşim zinciri (Ve, Tablo 18.7.5.4, SCWB, joint) için assert'li test yok (`Tests/Program.vb:220–256` yalnız `Console.WriteLine`; `:223` beams = Nothing, ΣMpr sınırı hiç çalışmıyor); ETABS'te de doğrulanmadı; "birim testleriyle doğrulandı" ifadesi yanlış.
2. **K3:** Q/δs kısıtı yok; Q yalnız deneme tasarımında (0,0504 > 0,05); optimumda drift sınırına yaklaşınca Q ≈ 0,07.
3. **K4/K5:** ΣMpr paylaşımı sabit 0,5 (çatıda ETABS'ten %6 düşük, kesit geçişinde güvensiz) ve φv = 0,75 için ACI 21.2.4.1 koşulu denetlenmiyor.
4. **K7/K8:** CD3 "geçti" yalnız güvensiz yönde; güvenli yönde 500/400'de +%29…+%31 (Spec ≤ %15), "%0–10" ifadesi yanlış; R2 (≥ 20 satır, ikinci yapı) açık (14 satır).
5. **K9/K10 + §6:** Rapor/Spec bayat ve tek güncel kapı tablosu yok; eşikleri aynı ajan onayladı; kampanya ve eşik onayı IA'dan ayrıca alınmalı; AGENTS 2.1 istisnası IA kaydı gerektirir.
