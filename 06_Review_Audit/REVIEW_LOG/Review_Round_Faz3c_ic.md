# Review Round Faz 3c (iç Critic) — 2026-11-15

Kapsam: `04_Results/phase3_stats.md` Bölüm 2, `tools/phase3_stats.py`, ham CSV (`phase3/{bto_alldim,ato_alldim}`, `phase2`), `PREDESIGN_PROTOCOL.md` §8–§9 (Faz 3c notu), `run_phase3c.sh`, `PROGRAM_CHANGE_REQUESTS.md` PCR-13. Medyanlar ham CSV'deki `feasible_history`/`bestFeasible` alanlarından bağımsız yeniden hesaplandı; `phase3_stats.md` Bölüm 2 medyanlarıyla 12/12 hücrede birebir tutuyor. Salt-okunur; dosya değiştirilmedi.

## Doğrulananlar
- 12 tüm-boyut hücresinin hepsi n=30, tek DLL (c1a95951…f5a9), uygunsuz sonuç 0. Tohumlar 1001–1030.
- Tablo kuralı: `phase3_stats.py` maliyeti `feasible_history` üzerinden alıyor (`cost` alanından değil). Bu doğru seçim: 4 hücrede, örneğin BTO lifsiz A, çıktıdaki son `x` (Fpen-en-iyi) bazen T>0 (ceza var) ve `cost` alanı en iyi uygun maliyetten farklı (BTO A: 1 400 080 vs 1 400 709). Betik tutarlı; ancak CSV'deki `fibreKg/steelKg` alanları Fpen-en-iyi `x`'e aittir, "en iyi uygun çözüm"e değil (bkz. Madde 3 sınırlaması).
- Holm m=12 ailesi (BTO+ATO; Baseline-A dışarıda) dış hakemin önerisini karşılıyor.

## 1. İlk turdaki 9 madde

| # | Madde | Durum | Not |
|---|---|---|---|
| 1 | MaxFE "bütçe tavanı, düzleşme sınanmadı" ifadesi | AÇIK (yazım) | Veri değişmedi; makale metni yok. Tüm-boyut BTO/ATO 10k, tek-boyut YAKO/ATO 30k ile aynı banda geliyor → ifade zorunlu. |
| 2 | MaxFE sonucu tek-boyut kuralına özgü | KAPANDI (veri) | ATO tüm-boyut 10k lifsiz A 1 427 926 ≈ YAKO 30k 1 425 078; BTO tüm-boyut 10k 1 400 709 hepsinin altında. Yazıya geçmesi gerek. |
| 3 | phase3_bin ↔ phase2_bin regresyon | AÇIK | PCR-13 için "duman testi, birim testi yok" (PCR dosyası); `runs/` altında Faz 3 regresyon kaydı yok. |
| 4 | Lifli C tüm-boyut n=30 | KAPANDI | BTO lifli C 30/30; Holm 3,0e-10. Ön okuma (n=8) artık geçersiz, 30'luk değer kullanılmalı. |
| 5 | Baseline-A ek materyalde "bağlam" | KISMEN | Betikte bağlam etiketli ve Holm dışı; ek materyal metni yok. |
| 6 | PCR-12: ATO tüm-boyut kontrolü | KAPANDI (veri), AÇIK (karar/yazım) | 6 ATO hücresi koşuldu. Bkz. Madde 2. |
| 7 | Lifli/lifsiz kıyası tüm-boyutla tekrarı | KISMEN | Tekrarlandı (Madde 3); sonuç "lif pahalı/ucuz" iddiasına izin vermiyor, kapalı kalıyor. |
| 8 | Lifli ABC YAKO 1/40 tekrarlanabilirlik sapması | AÇIK | Hiçbir inceleme kaydı yok. |
| 9 | PCR-13: lifli FSD kesit küçültmüyor (neden) | AÇIK | Üstelik yeni veri sorunu ağırlaştırıyor: tüm-boyut aramalarda lif hemen hiç seçilmiyor (Madde 3). |

## 2. ATO'da tüm-boyut ve PCR-12

Ham veri (n=30, eşleşik tohum, 10 000 FE):

| Hücre | Tüm-boyut med. | Tek-boyut med. | Fark | Holm p (m=12) | A12 |
|---|---|---|---|---|---|
| ATO lifli A | 1 439 234 | 1 467 811 | %1,9 | 0,028 | 0,69 |
| ATO lifli C | 1 447 546 | 1 466 229 | %1,3 | 0,011 | 0,72 |
| ATO lifli ABC | 1 424 611 | 1 477 556 | %3,6 | 2,5e-10 | 1,00 |
| ATO lifsiz A | 1 427 926 | 1 440 825 | %0,9 | 0,0026 | 0,76 |
| ATO lifsiz C | 1 423 950 | 1 427 864 | %0,3 | 0,395 (anlamsız) | 0,56 |
| ATO lifsiz ABC | 1 424 611 | 1 462 542 | %2,6 | 9,4e-11 | 1,00 |

Okuma:
- 6/6 hücrede yön tüm-boyut lehine; 5/6 Holm sonrası anlamlı. ATO'da etki BTO'dan (%3,5–11,6) küçük ama tutarlı. "ATO tek-boyutta iyi" ölçütü tanımsızdı; veri bu ölçütü ATO için de karşılamıyor: tek-boyut ATO tüm-boyuta karşı sistematik, küçük ama gerçek bir kayıp veriyor.
- **ABC hücreleri tavan etkisiyle şişiyor.** ATO ABC (her iki senaryo) ve BTO ABC'nin en iyisi aynı 1 424 611 TL; bu kutunun (x0 −2/+3) içindeki en ucuz uygun tasarım. A kolunda aynı bütçede BTO 1 386 277'ye, ATO 1 404 368'e iniyor: kutu %1,3–2,7 daha ucuz tasarımları dışarıda bırakıyor (kapsama testi başarısızlığıyla uyumlu). ABC'deki %3,6/%2,6 fark kısmen "kutu tavanına varış hızı" farkıdır (ATO tüm-boyut tavana lifsizde 23/30, lifli 17/30 koşuda ulaşıyor; tek-boyut ulaşamıyor). ATO için en temiz kanıt A ve C hücreleridir (%0,3–1,9).
- Karıştırıcı: lifli C/ABC ATO tüm-boyut `--fibrex0 1` (363 FE ön tasarım) ile koşuldu; Faz 2'de lifli x0 = lifsiz x0 (33 FE). Yani lifli ATO C/ABC'de tüm-boyut ↔ tek-boyut farkı PCR-13 + ~330 FE bütçe farkıyla karışık. BTO lifli C ise `fibrex0` olmadan koştu (ön tasarım 44 FE); BTO ve ATO "lifli C" aynı kurulum değil. Ayrıştırılamıyor; ama lifsiz hücrelerde (bu karıştırıcı yok) yön aynı olduğundan ana çıkarım korunur.

**Seçenekler:**
- (a) ATO tek-boyut ana kalsın, tüm-boyut ek materyal. Savunulamaz: yöntem seçimi sonuca bağlı (BTO'da zararlı görülüp kaldırıldı, ATO'da "küçük" diye bırakıldı); ayrıca veri ATO'da da tüm-boyutu 5/6 hücrede tutarlı üstün gösteriyor. Dış hakemin "outcome-dependent" itirazını doğrular.
- (b) BTO ve ATO ana serisi tüm-boyut; tek-boyut (PCR-12) ek materyalde "hareket kuralı duyarlılık deneyi". YAKO doğası gereği tek-boyut kalır, makalede açıkça belirtilir. BTO/ATO'nun orijinal tanımına (Simon 2008 göç-mutasyon her özellikte; Kiran 2015) uyum bonusu, mutlak değil (dış hakem kaynağı KISMEN doğruladı; orijinal makale metni sayfa/madde ile bağlanmalı).
- (c) Her yöntem için iki varyant da ana tabloda. Kıyas kalabalığı, "algoritma yarışı" izlenimi riski.

**Öneri: (b).** Faz 2'nin BTO/ATO bölümü (lifli/lifsiz karşılaştırması dahil) tüm-boyut serisiyle yeniden kurulmalı; Faz 2 tek-boyut sonuçları "PCR-12 tek-boyut varyant, ek materyal" olarak kalmalı, "exploitation artırır" cümlesi silinmeli, PCR-12 post-hoc etiketli (Faz 1 sonrası eklendi, tüm-boyut kontrolüyle yönü olumsuz bulundu). Kayıt: karar sonuç görülmeden önce değil, görüldükten sonra alındı; bunun beyanı gerekli.

## 3. ATO ABC lifsiz = lifli medyan (1 424 611): neden ve lif dağılımı

**Sebep: aynı tohum/aynı x0 değil, tavan etkisi + lif=0.** Doğrulama:
- İki hücrede de x0 maliyeti 1 664 921, ön tasarım FE'si lifsizde 33, lifli 363; lifli x0'ın seçilen adayı lifsiz (doz 0) (PCR-13 duman testi ile tutarlı). Kutu iki senaryoda aynı kesit kutusu.
- 30 koşunun 23'ü (lifsiz) ve 17'si (lifli) tam olarak 1 424 611,198 TL'ye ulaşıyor; bu, kutudaki en ucuz uygun tasarım. Medyan (15./16. sıra) iki hücrede de bu değer. Lif dozu: lifli ABC ATO'da lif>0 olan koşu 0/30 (fibreKg=0; tüm 30 `x` vektörünün lif genleri kutu tavanına uygun sıfır dozlu). Yani lifli hücrenin en iyi tasarımı lifsizle aynı; iki medyanın eşitliği hata değil, "lifli ABC'nin çözümü lifsiz tasarımdır" sonucu.
- Lifsiz ATO ABC'de yalnız 6 farklı `x` (30 koşuda), yani çözüm uzayı tükenmiş; bu, bu hücrelerin kıyaslamada bilgi taşımadığını gösterir (tavan: medyan dağılımın üst-sol köşesinde).

**Tüm lifli hücrelerde lif sayımı (CSV `fibreKg>0`, Fpen-en-iyi `x`):**

| Hücre | n | lif>0 koşu |
|---|---|---|
| BTO tüm-boyut lifli A / C / ABC | 30/30/30 | 0 / 0 / 0 |
| ATO tüm-boyut lifli A / C / ABC | 30/30/30 | 3 / 4 / 0 |
| Faz 2 BTO lifli A / C / ABC (tek-boyut) | 30/30/30 | 22 / 13 / 24 |
| Faz 2 ATO lifli A / C / ABC (tek-boyut) | 30/30/30 | 8 / 9 / 6 |
| Faz 2 YAKO lifli A / C / ABC (tek-boyut) | 30/30/30 | 25 / 14 / 14 |

Yeni verinin söylediği:
1. Tüm-boyut aramalarda lifli çözüm lifsiz (doz 0) çözüme çöküyor: 180 tüm-boyut lifli koşudan yalnız 7'sinde lif var (ATO A 3, ATO C 4); bunların en iyi uygun maliyetleri (1 442 360–1 485 629) o hücrelerin lifsiz medyanından (1,424–1,428 M) pahalı. Hiçbir lifli koşu lifsiz hücrenin medyanına inmedi, lif kullanan hiçbir tüm-boyut çözüm yararlı değil.
2. Lif kullanımı hareket kuralına çok duyarlı (BTO lifli A: tek-boyut 22/30, tüm-boyut 0/30). Tek-boyut Faz 2'de lif seçimi, aramanın kötü performansından (rastgele yürüyüş/seyrek mutasyon) kaynaklanıyor olabilir, ekonomik tercih olarak okunamaz.
3. Lifli arama uzayı lifsizi (doz 0) kapsar. Tüm-boyutta lifli medyan lifsizden hâlâ %0,0–1,7 yüksek (BTO A +%0,4; C +%0,25; ABC +%0,1; ATO A +%0,8; C +%1,7; ATO ABC 0, ikisi de tavan). Teoride lifli optimum ≤ lifsiz; farkın işareti arama gürültüsü/ek boyut (lif genleri) maliyetidir.
4. Sınırlama: CSV'de yalnız Fpen-en-iyi `x` saklı; en iyi uygun çözümün lif durumu doğrudan doğrulanamıyor (bir lifli A ATO ve bir lifli C ATO koşusunda hem lif>0 hem T>0). Sayım bu nedenle "çıktıdaki son tasarım" için geçerlidir; makalede "optimum tasarımda lif" denmemeli.

**Yapılamaz iddialar (güncel):**
- "Lifli çerçeve lifsizden pahalı/ucuz/ekonomik/ekonomik değil." (Arama uzayı kapsama + hareket kuralına duyarlılık + 90 TL/kg tek fiyat + PCR-13 tanılanmamış.)
- "Optimum tasarımda lif kullanılmadı." (yalnız: bu arama kuralı, bu fiyat ve fR modelinde, son tasarımlarda lif ≈ hiç seçilmedi.)
- ABC hücrelerinden herhangi bir lifli/lifsiz veya yöntem kıyası (tavan etkisi).
**Yapılabilir:** "Tüm-boyut BTO/ATO aramalarında 180 lifli koşunun 173'ünde son tasarımda lif yok; lifli senaryonun medyan maliyeti lifsizin %0,1–1,7 üstünde (yani lifli uzay lifsizin altına inmedi)." Lifin ne zaman anlamlı olduğu ancak fiyat/fR duyarlılığı ve kırılma fiyatıyla gösterilir (IA'nın sonraki adımı).

## 4. DLL farkı ve geçerlilik

Faz 2 DLL a2b418c2…875b (4 Kas) ↔ Faz 3 c1a95951…f5a9 (9 Kas). Kayıtlı kanıt: PCR-13 "varsayılan davranış değişmedi" beyanı + duman testi (lifsiz 33 FE/1 664 921). Regresyon kaydı yok; birim testi yok. Geçerlilik için gerekli:
1. phase3_bin ile `--onedim 1` BTO ve ATO, A/C/ABC lifsiz, ≥3 tohum (1001–1003), 1 000 FE ve tercihen 10 000 FE; çıktı `feasible_history` ve `x` Faz 2 CSV'siyle bit-bit karşılaştırılsın (tek hücre için ≈ dakikalar/saat). Birebir ise itiraz kapanır; değilse fark kaynağı (PCR-13 dışındaki değişiklikler) bulunup beyan edilir.
2. Lifli x0 (PCR-13) yalnız Faz 3'te var; lifli tek-boyut ↔ tüm-boyut karşılaştırmasında "sürüm + x0 + bütçe" üç karıştırıcıdır. Lifsiz hücrelerde yalnız DLL karıştırıcı kalır; bu nedenle ana çıkarım lifsiz hücrelere dayandırılmalı.
3. `--dop 8` sıra etkisi (1/40 lifli ABC YAKO sapması) bit-bit tekrarlanabilirliği zayıflatıyor; regresyon testi `--dop 1` ile koşulmalı.
Yapılmazsa: sürüm farkı makalede karıştırıcı olarak beyan edilir ve karşılaştırma "önerilen değil, ek" kalır.

## 5. YAKO ile BTO/ATO tüm-boyut karşılaştırması

Veri (medyan, TL): lifsiz A: YAKO 10k (Faz 2, n=30) 1 469 429; YAKO 30k (n=10) 1 425 078; ATO tüm-boyut 10k 1 427 926; BTO tüm-boyut 10k 1 400 709. Lifli A: YAKO 10k 1 536 957, 30k 1 455 201; ATO 1 439 234; BTO 1 406 575. Tüm değerler 1,40–1,47 M bandında; en iyi bilinen uygun tasarım 1 386 277 (BTO lifsiz A).

Nasıl yorumlanır (yöntem üstünlüğü olmadan):
- Yapılabilir: "Üç aramanın en iyi bilinen uygun maliyetleri ~%5 aralıktadır; hareket kuralı ve bütçe farkı yüzünden bu bir yöntem sıralaması değildir; farklar kural/bütçe duyarlılığını gösterir." Çapraz teyit iddiası en fazla "aynı bölgeye yaklaşma, %2–3 içinde" olarak zayıflatılır; optimum için teyit denmez (en iyi çözümler farklı tasarımlar, BTO ortalamayı 1,40 M'a çekerken YAKO 30k'da 1,425–1,455 M).
- Yapılamaz: "BTO > YAKO", "tek-boyut doğal olarak daha kötü" (YAKO tek-boyutlu doğası gereği; BTO/ATO kuralı değiştirince eşitlendi/ geçti), "yöntemler aynı optimuma yakınsadı".
- Bütçe asimetrisi: YAKO 30k n=10 (3 kat bütçe, n farklı) ile BTO/ATO 10k n=30 aynı grafikte yan yana verilmemeli; ya YAKO 10k n=30 ile (tüm-boyut BTO/ATO BTO %4,7, ATO %2,8 daha ucuz lifsiz A'da) ya da bütçe etiketli ayrı panelde. ABC'de YAKO 30k (lifsiz 1 447 323) kutu tavanına (1 424 611) ulaşamazken ATO 23/30 ulaşıyor; bu yöntem farkı değil, hareket kuralı/bütçe farkıdır, ama YAKO tek-boyutlu ABC'si için "tek-boyut kuralı tavana varamıyor" tespiti yazıya dürüstçe girmeli.
- Dış hakem önerisi (YAKO'nun çok-boyutlu varyantı, ya da tüm üç yöntemin "kendi en iyi" kuralı) bu veriyle hâlâ doğru: YAKO'ya tüm-boyut kontrolü yapılmadan "BTO/ATO tüm-boyut YAKO'dan iyi" gibi bir okuma, YAKO'nun tek-boyutluluğunun etkisinden ayrılamaz. Sınırlama olarak yazılmalı (YAKO tüm-boyut kontrolü kapsam dışı mı, IA kararı).

## Karar

**[REVİZYON TALEBİ]** (veri tarafı büyük ölçüde tamam; kalan yazım, kayıt ve ayrıştırma işleri)

1. **PCR-12 kararı:** BTO ve ATO ana serisi tüm-boyut; tek-boyut ek materyal, post-hoc etiketli; "exploitation artırır" ve "ATO tek boyutta iyi ise kalsın" kaldırılsın. ABC hücreleri tavan etkisi etiketiyle verilsin, kıyas yargısı ABC'den çıkarılmasın.
2. **Regresyon kaydı** (Madde 4: ≥3 tohum, tek-boyut BTO/ATO, `--dop 1`, bit-bit) ve lifli C/ABC `fibrex0` + FE-bütçe karıştırıcısının beyanı; yoksa sürüm farkı karıştırıcı.
3. **Lifli/lifsiz bölümü yeniden kurulsun:** yalnız tüm-boyut BTO/ATO seri; iddia "lifli uzay lifsizin altına inmedi, lif seçilmedi" düzeyinde; Fpen-en-iyi `x` sınırlaması beyan; fiyat/fR duyarlılığı ve kırılma fiyatı olmadan ekonomi yargısı yok. Lif genlerini tanılayan kısa kontrol önerisi: lif genleri sabit sıfırlı vs serbest arama farkı (PCR-13 ile birlikte nedeni araştırma).
4. **YAKO karşılaştırması bütçe/n eşitlenerek** (10k n=30 ya da etiketli ayrı panel); YAKO tüm-boyut sınırlaması makalede yazılsın veya küçük kontrol yapılsın.
5. **İlk turdan açık kalanlar:** madde 1 (MaxFE ifadesi), 8 (lifli ABC YAKO tekrarlanabilirlik), 9 (PCR-13 nedeni); V3 ETABS + S2 + IA onayı koşulu geçerli.

Genel onay verilmedi; bulgular bölümüne giriş için V3 ETABS + S2 + IA onayı hâlâ gerekli.

AUDIT_CHECKLIST: sayı–kaynak eşleşmesi [EVET: 12/12 hücre ham CSV'den yeniden üretildi]; sürüm izlenebilirliği [KISMEN: DLL farkı için regresyon yok]; N/A hücre [EVET, yok]; ETABS V3 [HAYIR, kapsam dışı]; iddia sınırları [EVET, yukarıda].
