# Review Round — Ö2 kampanya sonucu (iç Critic)

Tarih: 2026-11-16 · Hakem: iç Critic (Claude Code, `CRITIC_PROMPT.md`) · Kapsam: `04_Results/ex2_stats.md`, `04_Results/ex2_analysis.md`, `EX2_SPEC.md` §11.1 ve §12–13, `EX2_CODE_LOG.md` §5e–5f, `EX2_VALIDATION_REPORT.md`, ham veri `04_Results/ex2/campaign/` (yalnız okundu), `ex2_work/Ex2Model/GravityDesign.vb`, `program_work/RcCore/SfrcBeam.vb`, `program_work/FemRcModel/{BeamDesign,FibreTable}.vb`.
Yapılmayan: kod, ikili, koşu, ETABS ve git değiştirilmedi. Bağımsız yeniden hesaplar scratchpad'de Python ile yapıldı (kampanya CSV'leri okundu; `SfrcBeam.Flexure` + `Required` + `PhiMn` birebir Python kopyası).

## KARAR: **DUR (kısmi)** + geri kalanı için **REVİZYON TALEBİ**

- **DUR:** lifli 0 TL/kg hücresinin sayıları (medyanlar, %0,87, başabaş 8,39 TL/kg, sıfırlama kazançları, marjinal tablo Δ boyuna) ve "lif (0 TL/kg'da) toplam maliyeti azaltır" iddiası. Gerekçe: F0'ın ön kayıt kural 2 denetimi (a) ve (b) değerlendirmelerinde eğilmede D/C = 1,0103 olan (yani yetersiz) bir kiriş içeren tasarımla yapıldı; kök neden gerçek bir kod hatası (A1). Kırmızı çizgi 6 (koddan/şartnameden farklı formülasyon) ve "T = 0" ile güvensiz tasarım raporlanması.
- **REVİZYON (DUR değil):** lifsiz hücre ve 90 TL/kg sonuçları (aşağıda gerekçe: hatanın etkisi lifsiz yolda sıfır, 90 TL/kg kararında büyüklük olarak ihmal edilebilir), yazım ve iddia maddeleri K2–K9.
- Bulgular bölümü kapısı zaten kapalı (V3 + S2 yok; §11.1 kural 6). Bu rapor kapıyı açmaz.

---

## A1 — SFRC kiriş gerekli donatısı: GERÇEK HATA (kök neden bulundu)

**Kod:** `ex2_work/Ex2Model/GravityDesign.vb` satır 28–42 `Required` ile 92–100 `PhiMn`. Aynı yapı Ö1'de de var: `program_work/FemRcModel/BeamDesign.vb` satır 44–51 (yorum: "lcs = h - c of the previous trial").

**Mekanizma (birebir yeniden üretildi):** `Required` As üzerinde ikiye bölme yapar; i. denemede `SfrcBeam.Flexure` **bir önceki denemenin** c'sinden türetilen lcs ile çağrılır (`lcs = max(h − c, 0,2h)` karar SONRASI güncellenir). Yani her kabul/ret kararı kendi As'sine ait olmayan (bayat) lcs ile verilir. Lineer σ–w modelinde lcs büyüdükçe wu = min(0,02·lcs; 2,5) tavanda kalırken yu = wu·c/(lcs·εcu) küçülür, lif çekme kuvveti azalır, φMn düşer. Büyük As denemesinden gelen küçük lcs, lifi olduğundan **etkili** gösterir.

F0 B_S1 alt yüz (b = 250, h = 500, d = 440, C30, 4D/40: fR1 = 4,7, fR3 = 5,7, γF = 1,5; MuBot = 112,10 kN·m):

| Deneme | As (mm²) | kullanılan lcs (mm) | φMn (kN·m) | karar |
|---|---|---|---|---|
| 1 | 2 750 | 500 | 292,49 | kabul |
| 2 | 1 375 | 266,18 | 228,93 | kabul |
| **3** | **687,5** | **368,29 (bayat)** | **112,105 ≥ 112,10** | **kabul → hi = 687,5 kilitlendi** |
| 4–60 | 343,75 → 687,5 | ≈ 438,4 (tutarlı) | < 112,10 | hep ret |

Sonuç: `Required` = 687,5 mm² (= 0,05·b·d / 8, ikili kesir imzası; fR × 1,2 çıktısında da AsBot = 687,5 aynen). Tutarlı (sabit noktalı) lcs ile φMn(687,5) = 110,957 kN·m → D/C = 112,10 / 110,957 = **1,0103** (`F0_evalonly.txt:26` ile birebir). Tutarlı gerekli As = **694,8 mm²** (+%1,05). Üst yüz (1 322,1 mm²) doğru (D/C 1,0000). `PhiMn` doğrudur (20 adım sabit nokta, lcs → 379,03 / 438,43 mm, 3–4 adımda yakınsıyor; harita monoton, tek sabit nokta gözlendi).

**Yaygınlık (Python taraması, b = 250, h = 400–600, 5 tip/doz, Mu 60–250 kN·m, 470 durum):** 10 durumda güvensiz (D/C 1,002–1,027; gerekli As %0,1–2,6 eksik), 5 durumda muhafazakâr (D/C 0,993–0,999; As %0,07–0,7 fazla). Hata **iki yönlü ve seyrek**, maliyet yüzeyine "gürültü" ekliyor; optimizasyon ucuz (güvensiz) noktaları seçmeye eğilimlidir (kazanan laneti: hücre en düşüğü F0'ın tam böyle bir noktada olması bununla tutarlı).

**Ek tutarsızlık (aynı PCR'de ele alınmalı):** `EpsT` (satır 44–47) SFRC'de **üçüncü** bir lcs varsayımı kullanıyor: sabit `0,7·h`. εt ≥ 0,004 kısıtı (g3) Required/PhiMn ile aynı lcs ile denetlenmiyor. Ayrıca lcs = h − c (MC2010'da lcs = min(srm, y)); srm ihmali kodda "ASSUMPTION" olarak duruyor, beyan edilmeli.

**Neden testler yakalamadı:** `DcFlex` kısıt değil, yalnız rapor; T = 0 kabul denetimi ve 101 test `Required` ↔ `PhiMn` tutarlılığını sınamıyor.

### A1.1 İddialara etkisi

- **Lifsiz hücre (90 koşu):** etki yok. `fib Is Nothing` → `AciBeam.RequiredAs` yolu (satır 30, 93). P tasarımı D/C 1,0000.
- **90 TL/kg:** BTO/ATO 60/60 koşuda doz 0 → aynı lifsiz yol, etkisiz. YAKO'nun 5 lifli koşusu hatalı değerlendirildi (yeniden değerlendirme gerekir). Kararın ("90 TL/kg'da lif seçilmedi") yönü büyüklükle korunur: hatanın bir gruptaki en büyük etkisi ≈ %2,6 × (grup boyuna donatısı) ≈ birkaç yüz TL; en ucuz tek grup lif artışı 3 694 TL (`P_marginal_main.txt:7`). Yine de sayı tabloları düzeltilmiş ikiliyle yeniden üretilmeli.
- **0 TL/kg ("lif maliyeti azaltır"):** Critic tahmini (raporlanamaz, yalnız karar için): B_S1 alt +7,3 mm² × 16 m × ≈ 0,54 TL/(mm²·m) (P'nin boyuna maliyet / As·uzunluk oranından) ≈ **+63 TL** → F0 ≈ 228 357 TL; P'ye fark ≈ −1 951 TL (≈ −%0,85); başabaş ≈ 8,1 TL/kg (8,39 değil); B_S1 sıfırlama kazancı ≈ 1 326 TL (1 388,85 değil). Düzeltme F0'ı uygunsuz yapmaz (DcFlex kısıt değil; ρl = AsTop değişmiyor; εt sınırdan uzak). Dolayısıyla düzeltilmiş F0 düzeltilmiş ikilinin uzayında uygun ve P'den ucuz kalacağından iddianın **yönü** büyük olasılıkla korunur (üst sınır argümanı: düzeltilmiş en düşük lifli maliyet ≤ düzeltilmiş F0 < P). Ancak bu Critic'in Python kopyasına dayanır; **düzeltilmiş kodla deterministik yeniden değerlendirme olmadan yazılamaz**. (c) fR × 0,8 değerlendirmesinde D/C 1,0000 (kilitlenme yok), (a) ve (b) aynı F0 vektöründe 1,0103.
- **Ö1 (Faz 1–3):** aynı bisection `FemRcModel.BeamDesign` içinde. Ö1 lifli koşuların çoğunda lif = 0 (tüm-boyut 173/180), etki sınırlı; ama lifli en iyi çözümler (lif > 0 olan 7 koşu, Faz 2 lifli hücreler) yeniden değerlendirilmeli. Donmuş Ö1 DLL'leri ayrı PCR ve IA kararı gerektirir.

### A1.2 Gerekli PCR ve koşu kapsamı (öneri)

**PCR-23 (post-hoc, hata düzeltmesi):**
1. `Required` SFRC dalı: her denemede As'ye ait tutarlı lcs (iç sabit nokta, `PhiMn` ile aynı fonksiyonu çağırmak en basit yol: `If PhiMn(b,h,d,mid,fc,fib) >= Mu`), satır 33'teki üst sınır denetimi de tutarlı lcs ile. `EpsT` aynı lcs ile.
2. Birim testi: tip × doz × h × Mu ızgarasında `Mu / PhiMn(Required(Mu)) ≤ 1 + 1e-9` (yukarıdaki 470 durumluk tarama örnek); F0 B_S1 vakası (AsBot 694,8 mm² ± 0,5) test olarak eklenir.
3. Değerlendiricide güvenlik denetimi: DcFlex > 1 + tol ise uyarı/ceza (sessiz T = 0 olmasın).
4. Yeni ikili SHA'ları; lifsiz yol için regresyon: 90 lifsiz son vektör + P eski/yeni ikiliyle **bit-bit aynı** maliyet ve kısıt (kanıt dosyası).

**Koşu kapsamı:**
- **270 koşunun tamamı gerekmez.** Lifsiz 90 koşu, regresyon bit-bit aynıysa korunur (aynı değerlendirme fonksiyonu = aynı koşul; kural 8 bozulmaz).
- **Lifli 180 koşu (fp0 + fp90) aynı tohumlarla (1001–1030) yeniden koşulmalı (önerilen ana seri).** Gerekçe: hata maliyet yüzeyini değiştiriyor (seyrek, iki yönlü); yalnız son vektörleri yeniden değerlendirmek "hatalı yüzeyde aranmış, doğru yüzeyde değerlendirilmiş" karma bir seri verir ve medyan/Holm/A12 tabloları için savunulamaz. fp90'ı da yeniden koşmak lifli iki hücrenin aynı ikiliyle karşılaştırılmasını sağlar (fp0 ↔ fp90 testi ön kayıtta var).
- **Asgari (yeniden koşu yapılamazsa):** 180 lifli son vektörün + F0/P/PY deterministik araçlarının (evalonly, zerofibre, marginal, fR/format/fiyat varyantları, patterns, deflection, scan) düzeltilmiş ikiliyle yeniden değerlendirilmesi; seri "eski arama, düzeltilmiş değerlendirme" etiketiyle, istatistik testleri yalnız ek materyalde. Bu, nitel iddia kararı için yeterli, sayısal tablolar için yetersiz.
- Her iki yolda deterministik analiz paketi (`ex2_analysis_run.sh`) yeni ikiliyle baştan üretilir; eski çıktı `superseded_` altında tutulur.

### A1.3 Ön kayıt durumu

Evet, **ön kayıt dışı değişiklik**. §11.1 kampanya ikilisini SHA ile sabitledi; ikiliyi sonuç görüldükten sonra değiştirmek post-hoc sapmadır. §11.1'in kendi kuralına göre ihlal sayılmaması için: (i) `EX2_SPEC.md`'de ayrı "post-hoc (PCR-23)" alt bölümü, §11.1 metni değiştirilmeden; (ii) düzeltmenin sonuçtan bağımsız bir doğruluk düzeltmesi olduğu, ama sonuç incelemesi sırasında bulunduğu açıkça yazılır; (iii) eski (hatalı) ve yeni seri ek materyalde birlikte verilir; (iv) seçici raporlama yok (iddia hangi seride bozulursa o raporlanır). Sessizce ikili değiştirip tablo yenilemek ihlal olur.

---

## K maddeleri

**K2 — "Lif etriyeyi azaltır" referansı:** Katılıyorum: ön kayıt referansı tanımlamıyor; referans seçimi sonuçtan sonra yapılırsa çatallanan yol olur. Analistin muhafazakâr yorumu doğru: genel cümle yok. Dar betimleyici cümle ("250×500 kesitte lif, etriye aralığını dayanımdan smax'a taşıdı; aynı kesitte etriye −1 047,68 TL") yalnız post-hoc etiketiyle ve en düşük lifsiz tasarıma göre etriyenin +683,25 TL arttığı ve P kesitlerinde 20/20 durumda Δ etriye = 0 olduğu cümlesiyle birlikte yazılabilir. A1 düzeltmesi sonrası rakamlar yeniden üretilmeli (etriye AsTop'a bağlı ρl üzerinden etkilenebilir).

**K3 — Holm m = 9:** Doğru. Ön kayıt kural 8 fp0 ↔ fp90'ı da açıkça içeriyor; ön kayıt görevlendirme metninden üstündür. Bağımsız yeniden hesap (scipy 1.17.1, iki yönlü MWU, Holm): 9 p ve 9 Holm değeri `ex2_stats.md` §3 ile birebir. Yazım: BTO/ATO lifsiz ↔ 90 TL/kg satırları "test uygulanamaz / tüm değerler aynı (bağ)" diye yazılmalı, "p = 1, fark yok" diye değil; ATO'da standart sapma 0 olan hücrelerde p ve A12 = 0 "etki büyüklüğü" değil "30/30 aynı son çözüm" bilgisidir.

**K4 — Başabaş tek tasarıma dayalı:** Katılıyorum ve bir adım öteye: 8,39 TL/kg (düzeltme sonrası ≈ 8,1) **F0 tasarımının** P'ye karşı başabaşıdır; "sistem düzeyi" adı yanıltıcı. Bu değer gerçek başabaşın **alt sınırıdır** (p bunun altındayken en az bir lifli tasarım bulunan en ucuz lifsizden ucuz; üstünde daha az lifli başka bir tasarımın kazanıp kazanmadığı bilinmiyor). Makale cümlesi: "for the cheapest fibre design found, fibre would have to cost below ≈ X TL/kg (about 1/11 of the ÇŞİDB price)". P de kanıtlanmış optimum değil, bulunan en ucuz lifsiz çözümdür.

**K5 — Aktif kısıtlar (6.2.6 = 1,3757; tüm tasarımlarda ötelenmeli kat):** Maliyet optimizasyonunda beklenen durum; hata değil. Ancak: (i) V3 ETABS'i yalnız P için değil F0 için de yapılmalı (6.2.6, lu oranı, Q sınıflaması; F0'da üç kat da ötelenmeli); (ii) F0'ın kolon değişiklikleri (C_Inter_S1 300×400, +1 237,90 TL) kiriş tasarrufunun bir kısmını yiyor; kalem ayrımında bu net olarak yazılmalı; (iii) V3'e girmeden A1 düzeltilmeli (aksi halde Excel lifli Mn kontrolü B_S1'de yetersizlik bulacak).

**K6 — Desenli yük açığı (eğilme D/C 1,14–1,23):** Ciddi sınırlama. ACI 318-14 6.4 hareketli yük düzenlemesi sürekli kirişlerde şartnamenin parçasıdır; bu nedenle tasarımlar için "ACI 318-14'e uygun" / "code-compliant" denemez, yalnız "tanımlı kısıt kümesine göre uygun (T = 0)". Olumlu bilgi: desenli zarfla yeniden türetmede F0 (234 144,28) P'den (236 316,17) hâlâ 2 171,89 TL ucuz (deterministik, yeniden optimizasyon değil; A1 sonrası yeniden üretilmeli). Bu, sınırlama paragrafında sayıyla verilmeli; hakem ana sınırlama sayacaktır. Desenli yükü kısıt yapıp yeniden koşmak IA kararıdır (ön kayıtta [K b] sınırlaması olarak kabul edilmiş, şimdi değiştirmek ayrı post-hoc olur).

**K7 — Sehim 0,936 (ℓ/480), lif rijitlik katkısı yok:** Lifin çatlak sonrası/gerilme rijitleşmesi katkısının ihmali F0 için muhafazakâr yöndedir; asıl belirsizlik uzun süreli çarpan ve ℓ/480 seçiminin (yapısal olmayan elemanlar hasar görebilir varsayımı) kendisi. Servis kısıt değil (rapor). Makalede "F0'ın sığ kirişleri ℓ/480 sınırına 0,94 oranla yaklaşır; servis kısıt değildir" cümlesi ve AÇIK-21 varsayımları yazılmalı; "lif servis performansını iyileştirir" yasağı geçerli.

**K8 — YAKO 90 TL/kg medyanı (+%1,74):** Lif etkisi olarak raporlanamaz (uzay kapsama + 24 değişken + tek-boyut hamle; YAKO 30/30 farklı son tasarım = yakınsamamış arama). Ana metinde test satırı yerine "YAKO bu uzayda 10 000 FE'de yakınsamadı" bilgisi; test tablosu ek materyalde. YAKO'nun 0 TL/kg'da 30/30 lif seçmesi, lif değişkenine erişebildiğini gösteriyor (iyi tutarlılık bilgisi).

**K9 — Kolon etriye 120 mm (F0 C_Edge_S3):** Kök neden bulundu, hata değil: F0'da Vu = 28,31 kN > 0,5·φ·Vc = 0,5·0,75·61,01 = 22,88 kN → kesme donatısı gerekli → ACI Tablo 10.7.6.5.2 s ≤ d/2 = (300 − 60)/2 = 120 mm. P'de Vu = 21,79 kN < 22,89 kN → yalnız 25.7.2.1 sınırı (min(16db, 48dt, 250) = 250 mm) (`P_evalonly.txt`, `F0_evalonly.txt` kolon satırları). Eşik süreksizliği; F0'da kiriş rijitliği azalınca kolon kesmesi artıyor. Makalede kalem ayrımında "lifli tasarımda kolon etriyesi ACI 10.6.2.1 eşiği aşıldığı için sıklaştı" diye yazılabilir.

---

## Rakam–kaynak bağımsız yeniden hesap (ham CSV'den)

| Kontrol | Sonuç |
|---|---|
| 9 hücre × n, FEs = 10 000, T = 0 | 270/270, birebir |
| 9 hücre en iyi / ortalama / medyan / kötü / std. sapma | birebir (`ex2_stats.md` §2) |
| Lif > 0 koşu ve kg aralıkları (YAKO fp90 5/30, 0–45; fp0 194–242 / 230–264 / 230–240) | birebir |
| 9 MWU p ve Holm | birebir |
| ATO medyan farkı lifsiz ↔ 0 TL/kg | −2 014,13 TL, −%0,8745 (birebir) |
| F0 @ 90 TL/kg = 228 294,22 + 240 × 90 | 249 894,22; P'ye +%8,504 (birebir) |
| Başabaş 2 014,13/240; 1 268,50/240 | 8,392; 5,285 (birebir; A1 nedeniyle geçersiz, bkz. üstü) |
| Marjinal "lif bedelinin %3–7'si" | 266,23/3 960 = %6,7; 144,93/4 320 = %3,4; 1 040,47/25 200 = %4,1 (doğru) |
| **`ex2_analysis.md` §11 madde 6 "YAKO %0,02–0,21 içinde, bir kolon grubunda farklı"** | **HATALI:** sapmalar lifsiz %0,0074, 0 TL/kg %0,0173, 90 TL/kg %0,2137 → "%0,007–0,21"; 90 TL/kg'da fark bir kolon grubu + iki kiriş grubu (B_S2, B_S3 550). Düzeltilmeli |

## Ön kayıt kurallarına uyum

| Kural | Durum |
|---|---|
| İkili SHA (§11.1) | Kampanya ve analiz ikilisi aynı (`analysis/sha_check.txt` 8/8) — UYGUN. PCR-23 sonrası post-hoc |
| Kabul denetimi | FEs ve T raporlandı; **`violation_counts` (g6, g7 dahil) okunması ön kayıtta istenmiş, `ex2_stats.md` §1'de yok** → bir sütun veya "okundu" kaydı eklenmeli (küçük) |
| Kural 1 rejim kaydı | `ex2_analysis.md` başında var — UYGUN; her lif cümlesine taşınmalı |
| Kural 2 üçlü koşul | Yöntem doğru uygulandı, ama (a)/(b) D/C > 1 tasarımla yapıldı → A1 sonrası yeniden; şu an GEÇERSİZ |
| Kural 3 başabaş | Kampanyadan hesaplanmış (sağlama değeri kullanılmamış) — UYGUN; adlandırma ve alt sınır niteliği K4 |
| Kural 4 doz-0 yanlılık kontrolü | İki yol da raporlandı — UYGUN (fp0 rakamları A1 sonrası yenilenecek) |
| Kural 5–8 | UYGUN (ABC yok, yöntemler arası test yok, m = 9) |
| "Sonuç görülmeden yazıldı" beyanı | **Şerh:** fiyat = 0 sağlama sınaması (§5f, tohum 9101–9105; fp0'da lif 5/5, ≈ 2 014 TL, fp90'da lif 0) ön kayıttan ÖNCE görülmüştü ve kural 2–4 bu bilgiyle yazıldı. Kural 3 sağlama değerini dışlıyor, dürüst; ama ön kayıt beyanı "kampanya sonuçları görülmeden; 30 koşuluk fiyat = 0 sağlaması görülmüştü" diye makale/ek materyalde açık yazılmalı |

## İddia listesi (§12) denetimi

- "Optimum" (madde 9): `ex2_analysis.md` uyuyor ("yerel en iyi", "bulunan en ucuz uygun"). **ATO 30/30 aynı son vektör** yalnız şunu gösterir: 18–24 değişkenli ayrık uzayda bu yöntemin 10 000 FE'de tekrarlanabilir biçimde aynı çekim bölgesine düştüğü ve tek-koordinat (lifli modda tip × doz çifti) komşuluğunda daha ucuz uygun komşu olmadığı. Global optimum, "kanıtlanmış en iyi" veya "yakınsadı" denemez; üç yöntemin aynı/benzer bölgeye düşmesi yalnız çapraz teyittir. Ayrıca A1 gibi değerlendirme gürültüsü varken "aynı vektöre yakınsama" kısmen değerlendirici özelliğidir.
- Madde 4–5 ("lif ekonomiktir/değildir", "lif pahalıdır"): ihlal yok. **Ancak** "lif (0 TL/kg'da) toplam maliyeti azaltır" ifadesi okuyucuyu yanıltır: 0 TL/kg bir duyarlılık senaryosudur (lif bedava). Yazım önerisi (A1 sonrası, rejim kaydıyla): "In the zero-fibre-price sensitivity scenario, the cheapest fibre design found was ≈ 0.85 % cheaper than the cheapest plain design found, owing to shallower beams; at the ÇŞİDB price no run selected fibre." Özet/sonuç cümlesinde "fibre reduces cost" geçemez.
- Madde 6 ("lif etriyenin yerini alır"): ihlal yok.
- Madde 12 ("ETABS'te doğrulandı"): ihlal yok; V3 açık.
- `ex2_analysis.md` §5 "lifin seçilmemesi … lif bedelinin diğer kalem tasarrufunu aşmasından kaynaklandığıyla tutarlıdır": kabul edilebilir (tutarlı ≠ kanıtlandı), rejim kaydıyla.

## Yapılacaklar (sıra)

1. **PCR-23** (Developer; IA onayı): `Required`/`EpsT` tutarlı lcs, testler, DcFlex güvenlik denetimi, yeni SHA; lifsiz yol bit-bit regresyonu; Ö1 `BeamDesign.Required` için ayrı karar notu.
2. Lifli 180 koşu aynı tohumlarla yeniden (önerilen) veya asgari 180 son vektör + deterministik paket yeniden değerlendirme; `EX2_SPEC.md`'de post-hoc alt bölüm (§11.1 değiştirilmez).
3. `ex2_stats.md` / `ex2_analysis.md` yeniden üretimi; §11 madde 6 düzeltmesi; başabaş adlandırması (K4); bağ testleri yazımı (K3); `violation_counts` satırı.
4. V3 (P ve F0) + S1 Excel lifli Mn bağımsız kontrolü (A1 düzeltmesinin bağımsız sınaması).
5. Sınırlama paragrafı: desenli yük (K6, sayılarla), sehim (K7), lcs = h − c (srm ihmali), ön kayıt öncesi sağlama sınaması şerhi.
6. Critic yeniden incelemesi, sonra IA.
