# Dış hakem raporu: Faz 3 (BTO tüm-boyut / tek-boyut, MaxFE 20k/30k)

Kapsam: `phase3_stats.md`, `phase3_stats.py`, PREDESIGN_PROTOCOL §8–9, `phase2_stats.md`, `maxfe_decision_20k.txt`, ham CSV başlıkları, `Optimizers.vb`. Salt-okunur çalışıldı. Lifli C tüm-boyut hücresi kısmi (n=8; dosya sayısı 8, seed 1,4,7,...,22).

## 0. Doğrulanan sayılar

- Tüm-boyut / tek-boyut BTO medyanları (10 000 FE): lifli A 1 406 575 / 1 590 540 (%11,6 ucuz); lifli C 1 404 149 / 1 575 109 (%10,9, n=8); lifsiz A 1 400 709 / 1 503 872 (%6,9); lifsiz C 1 403 512 / 1 493 623 (%6,0). Yani "%7–12" aslında **%6,0–11,6**. p değerleri tabloyla uyumlu (Holm sonrası ≤ 1,8e-10; lifli C 4,1e-8).
- Tüm-boyut BTO'nun 10 000 FE medyanı (1,400–1,407 M), ATO ve YAKO'nun **30 000 FE** değerlerinden de düşük: lifsiz A ATO "oturmuş" 1 437 806, YAKO 30k 1 425 078; lifli A ATO 1 418 829, YAKO 1 455 201 (lifli ABC: 1 448 348 / 1 466 878). Fark %1,3–3,9.
- Tüm-boyut BTO koşularında lifli senaryoda **38/38 koşuda lif miktarı 0** (fibreKg 0,00; lifli A 30/30, lifli C 8/8). Phase 2 tek-boyutta lifli A'da 36 koşunun yalnız 3'ünde ilk üç değer lifli. Lifli 30k A hücresinde 20 koşudan yalnız 1'i lifli (kısmi örnek).
- İkili sürümü: Phase 2 `a2b418c2…` (2026-11-04), Phase 3 `c1a95951…` (2026-11-09). Başlıkta Phase 2 `oneDim,1` yazıyor, tüm-boyutta yok; tohumlar 1001+ aynı.

## 1. Kod ve istatistik

**Tek-/tüm-boyut karşılaştırmasının geçerliliği.** Ana yön (tüm-boyut ≪ tek-boyut) fark büyüklüğü (A12 = 1,00; dağılımlar neredeyse ayrık) nedeniyle gizli karıştırıcılarla açıklanamaz. Ancak şu karıştırıcılar not edilmeli:
1. İki farklı DLL (PCR-13 ve diğer değişiklikler arasındaki fark protokolde "lifsiz senaryo etkilenmez" diye geçiyor; lifli kolda da lifli x0 seçimi lifsize düştü). Lifsiz A için bunu kanıtlayan bir bit-bit regresyon (aynı tohum, `--onedim 1`, yeni DLL → Phase 2 sonucunu üretiyor mu) raporda yok; bu tek küçük koşu ile kapatılabilir.
2. Tohum aynı olsa da rastgele akışlar farklı; çiftli test değil, bağımsız örnek testi (Mann–Whitney) doğru seçim. Tamam.
3. "Baseline-A BTO'dan da ucuz" karşılaştırması (44 değişken, rastgele başlangıç) ile A kolu (türetilmiş rho/sınıf, farklı değişken uzayı) aynı sorunu ölçmüyor; kaynağı hareket kuralı değil, arama uzayı farkı da olabilir. Holm ailesine (m=6) bu iki testin karıştırılması gereksiz; yalnız aynı-uzay testleri (4 adet) bir ailede olmalı. Sonuç değişmez.
4. Sansür/eksik hücre: lifli C n=8, seed 1,4,7,… (dop 8 ile paralel). Bitmiş koşular rastgele bir alt küme gibi görünüyor, ancak bitiş sırası dolaylı olarak koşu süresine (FE maliyeti/uygunluk) bağlıysa yanlılık olabilir. Hücre bitmeden n=8 ile p vermek yerine tamamlanmasını bekleyin ya da "kısmi, n=8" etiketi koruyun.
5. FE sayımı tutarlı görünüyor (plain_A: FEs 10 000, predesignEvals yok; fibre_C: FEs 9 956 + 44 = 10 000). `at()` toplam FE'ye ön tasarımı ekliyor. Tamam.
6. İstatistik betiği: bootstrap GA'sı n=10 üzerinde medyan için çok kaba; "oturdu" kararında 10 koşuluk medyan, "20k=25k=30k aynı değer" (ör. lifli A ATO 1 418 829 üç noktada) çoğu koşunun **donduğunu** (popülasyon çöküşü) gösteriyor; bu iyileşme sıfır = yakınsama değil durgunluk.

**OneDimMove implementasyonu (Optimizers.vb).**
- BTO (BBO): tek-boyut kipinde habitat başına bir j seçilip yalnız o gende göç (olasılık λᵢ) ve mutasyon (0,1) uygulanıyor. Orijinal BBO'da göç ve mutasyon **her gende** bağımsız. Burada (a) mutasyon etkin oranı gen başına 0,1/nd ≈ 0,003 (nd=32–44) iken tüm-boyutta 0,1; yani mutasyon ~30–40 kat azalmış, (b) en iyi habitatlar (λ≈0) hareketsiz, (c) `v(j)=snapshot(i)(j)` olduğunda (göç gerçekleşmediğinde veya kaynak aynı değeri verdiğinde; popülasyon yakınsadıkça sık) satır 164 ±1 rastgele adım ekliyor. Yani tek-boyut BTO, BBO'dan çok "%50'ye varan ±1 rastgele yürüyüş + seyrek göç" hibriti; algoritmanın özü (genler arası bilgi paylaşımı, mutasyonla çeşitlilik) bozulmuş. Bu yüzden sonuç "BTO tek-boyutta kötü" değil "bu operatör BTO değil".
- ATO (TSA): Kiran (2015) orijinalinde tohum üretiminde boyutlar ST olasılığına göre güncelleniyor; kodun tüm-boyut kipi buna yakın. Tek-boyut kipi her tohumda bir boyuta indirgiyor ve null hamlelerde ±1 ekliyor. Bu, TSA'nın hamle formülünü (x + α(best − x_r)) korur ama gerçek TSA'da çoklu boyut eşzamanlı değişir. Tek-boyut ATO ~20 ağaçlı küçük popülasyonla hızlıca donuyor (yukarıdaki durgunluk); "ATO tek boyutta iyi" sonucu **görecelidir**: BTO tüm-boyut ATO tek-boyutu %1,3–2,6 (lifsiz A: 1 400 709 / 1 437 806 = %2,6; lifli A: 1 406 575 / 1 418 829 = %0,9) geçiyor. "İyi" ancak ATO'nun kendi tüm-boyut kontrolü olmadan bilinemez. Bu bilgi eksikliği, makalenin "çapraz teyit" amacını doğrudan zayıflatıyor: yöntemlerin aynı bölgeye yakınsadığı iddia edilemiyor (1,40 M vs 1,42–1,47 M).

## 2. "Tek-boyut BTO/ATO raporlansın, ATO tüm-boyut kontrolü yapılmasın" yaklaşımı

Hakem olarak **kabul etmem**: (i) kural Faz 1 sonrası, Faz 2 öncesi eklenmiş ve gerekçesi ("exploitation artırır") BTO için sizin verinizle çürütülmüş; (ii) ATO için aynı çürütmenin geçerli olmadığını gösteren tek bir veri yok; (iii) sonuçta en iyi bilinen çözümü, raporlanmayan bir konfigürasyon (BTO tüm-boyut) veriyor. "ATO iyi görünüyorsa kalsın" ölçütü sonuca göre seçim (outcome-dependent) olarak okunur.

En ucuz savunulabilir yol (öneri sırası):
1. **ATO tüm-boyut kontrolünü yap**, tüm hücrelerde değil: lifsiz A ve lifli A, 10 000 FE, 30 koşu (ya da 20 koşu). Mevcut çıktılarda ATO tüm-boyut zaten `--onedim` kapalı kipiyle çalışıyor; bu 60 koşu BTO'dakiyle aynı mekanik. Maliyet: BTO 4 hücrenin yarısı kadar.
2. Veya en azından ATO ve YAKO için BTO ile aynı sorgunun "tüm-boyut en iyi" olduğunu söyleyip **ana tablolarda üç yöntemin en iyi konfigürasyonunu** (yöntem başına tek/tüm-boyut seçimi önceden belirtilmiş bir kuralla) raporla; ya da hareket kuralını yöntem hiperparametresi olarak açıkla ve ikisini de göster.
3. Ana metin: PCR-12'yi "operatör seçimi" olarak, kanıtlarıyla (%6–12 fark) yaz, "exploitation artırır" ifadesini kaldır.
4. Ek: tek-boyut BTO'yu "BTO" diye değil "tek-boyut göç/mutasyon varyantı" diye adlandır ya da tamamen ek materyale taşı.

## 3. 30 000 FE üst sınır mı?

Raporlamak yeterli değil. Verinizdeki en güçlü karşı kanıtlar:
- YAKO 20→30k iyileşmesi %0,25–1,06; 25→30k %0,0–0,62; ama 10→30k GA'sı %3,0–7,6 (lifli A) ve %0,4–3,4 (lifsiz ABC). Adım başına <%1 kuralı, yavaş ama sürekli azalan (güç yasası) eğrilerde yanıltır: toplam kazanç küçük adımların toplamıdır. Ön kayıtlı "oturma" kuralı bu nedenle YAKO için hiç tatmin edilmedi (kural sonucu 6/8 hücre yeniden koşuldu; bu doğru), ama 30k'da da düz değil.
- BTO tüm-boyut 10k'da 30k YAKO/ATO'dan %1,3–3,9 ucuz: düzleşme "bu algoritma için" yakınsama değil, bölgesel durgunluk.

Düzleşme/yakınsama iddiası için gereken kanıt: (a) bir yöntem × hücre için 100 000 FE'lik 5–10 koşu (maliyet: ≈0,27 s/FE → ≈7,5 saat/koşu tek çekirdekte; 8 paralelde tamamı gün içinde); (b) FE–maliyet eğrisinin üssel/güç yasası uydurması ve asimptot tahmini; (c) bağımsız uzun koşu veya farklı algoritmayla (tüm-boyut BTO) aynı maliyet bandına ulaşma (çapraz teyit); (d) global kanıt isteniyorsa küçük alt problemde (örn. 2D 6 elemanlı) tam sayım. 30k'yı "bütçe üst sınırı, yakınsama kanıtlanmadı" diye raporlamak dürüst; "MaxFE yeterli" demek değil.

## 4. Makalenin yapabileceği / yapamayacağı iddialar

Yapabilir:
- Hesaplama bütçesi 10k–30k FE'de, ön tasarım (44 FE) dahil, ulaşılan **en iyi bilinen** maliyet ≈ 1,40 M TL (tüm-boyut BTO) ve diğer yöntemlerle 1,42–1,47 M TL; tüm koşular uygun (infeasible 0).
- Ön tasarımın (FSD) başlangıç noktasını tanımlaması ve rastgele başlangıca göre iyileştirmesi (Faz 1: %17–20; tablodan sonra doğrulanmalı).
- Hareket kuralının sonucu büyük ölçüde değiştirdiği (%6–12): bu bir bulgu olarak.
- Lifli senaryoda, bu birim fiyat (90 TL/kg) ve fR modelinde, tüm-boyut BTO optimum arayışı **38/38 koşuda lif kullanmadı** (lifsiz tasarım).

Yapamaz:
- "X yöntemi Y'den iyidir" ya da yöntem sıralaması (sizin de politikanız; ayrıca kod versiyonları farklı).
- "Lifli çerçeve lifsizden pahalı/ucuz" (Phase 2: %1,0–5,8 pahalı). Lifli arama uzayı lifsizi (doz=0) içerir; lifli optimum ≤ lifsiz optimum olmalı. Lifli senaryonun daha pahalı çıkması arama başarısızlığıdır, ekonomik bulgu değil. Tüm-boyut BTO'da (lif=0) lifli 1 406 575 / lifsiz 1 400 709 farkı (%0,4) aynı tasarımın arama gürültüsü. Lifin ne zaman ekonomik olduğunu ancak fiyat/fR duyarlılığı ile (kırılma fiyatı) göstermek mümkün.
- "Optimum", "yakınsadı", "MaxFE yeterli".
- ETABS V3 öncesi bulgular bölümüne giriş (protokolünüz zaten kısıtlıyor).

## 5. Karar: **Büyük revizyon** (yayın bu haliyle uygun değil; deneyler kapatılırsa küçük revizyona iner)

En önemli 5 madde:
1. **Operatör bütünlüğü:** Tek-boyut BTO/ATO, orijinal BBO/TSA'dan (gen başına göç/mutasyon, çoklu boyut) saptı; ±1 null-hamle düzeltmesi ek bir operatör. BTO tek-boyutta %6–12 kötü. ATO'nun tüm-boyut kontrolü yapılmalı (en az lifsiz+lifli A, 20–30 koşu).
2. **Çapraz teyit başarısız:** en iyi bilinen çözüm (tüm-boyut BTO, 1,40 M) diğer yöntemlerin 30k sonuçlarının %1,3–3,9 altında; "teyit" iddiası ancak yöntemler aynı banda yakınsayınca yapılabilir. En iyi tüm-boyut konfigürasyonları (üç yöntem) raporlanmalı.
3. **Düzleşme iddiası:** adım başı <%1 kuralı ve 30k tavanı yakınsama kanıtı değil (YAKO 10→30k GA'sı %3–7,6; ATO "oturması" donma). 100k FE'lik birkaç koşu + eğri uydurma veya çapraz teyit gerekli; aksi halde "bütçe tavanı" olarak raporla.
4. **Lifli/lifsiz kıyası geçersiz arama sonucu:** lifli senaryo lifsizi kapsıyor; tüm-boyut koşularda lif 0; fark arama artefaktı. Lifli x0'ın lifsize düştüğü (PCR-13) ayrı bir bulgu (lif FSD'de kesit küçültmüyor) ve nedeni incelenmeli (kısıt dayanım değil mi? kayma/sarılma bölgesinde lif katkısı 0 varsayımı?). Lif için fiyat-fR duyarlılığı şart.
5. **Karıştırıcılar ve raporlama:** farklı DLL'ler (regresyon testi ekleyin), Baseline-A karşılaştırması ayrı uzay, Holm ailesi yeniden tanımlanmalı, lifli C n=8 tamamlanmalı; post-hoc kararlar (PCR-12, PCR-13) makalede açıkça etiketlenmeli ve "ATO tek boyutta iyi ise kalsın" gibi sonuca bağlı seçim kuralı yazılmamalı.

---

## Ek yanıt (koordinatör soruları)

**1) "Tüm-boyut daha iyi" neye dayanıyor?**
- (a) Literatür: Simon (2008), *IEEE Trans. Evol. Comput.* 12(6), DOI 10.1109/TEVC.2008.919004: göç, çözümün her özelliği (gen) için ayrı ayrı uygulanır (habitat başına λ ile "bu özellik değişsin mi" kararı; kaynak habitat μ'ye göre seçilir); mutasyon da özellik başına sabit/ türetilmiş oranla uygulanır. Durum: **KISMEN doğrulandı**. Orijinal makale metnine erişemedim; yalnız sonraki makaleleri özetleyen web sonuçlarında ("migration works on individual features... immigration rate decides whether each feature is modified"; Markov modeli makalesinde özellik başına p_m) bunu gördüm (Wikipedia BBO sayfası; core.ac.uk/download/357210794.pdf). Ayrıntı (ör. mutasyonun tür sayısı olasılığına bağlı oluşu) için orijinal makaleye bakılmalı; Wikipedia'yı atıf olarak kullanmayın. Yani tüm-gen kuralı orijinal tanım olarak görünüyor, ama bunu "BBO'nun üstünlüğü" gerekçesi yapmam.
- (b) Yalnız bizim veri: aynı bütçe (10 000 FE), aynı arm, 30 koşu, tüm-boyut tek-boyuttan %6,0–11,6 ucuz (A12=1,00). Bu, "bu problemde ve bu uygulamada tüm-boyut operatörü tek-boyut varyantından iyi" demenin ötesine geçmez; orijinal tanıma uygunluk ayrı bir argüman (savunulabilirlik), veri ise ayrı (etki büyüklüğü). İkisi birbirini doğrulayan bağımsız iki gerekçe, ama biri diğerinin kanıtı değil. Tek-boyutun neden kötü olduğu (mutasyon ~nd kat seyrelmesi, ±1 yürüyüş) kod okumasından çıkarım; ayrıştırma deneyi (yalnız mutasyonu gen başına tut, vb.) yapılmadı.

**2) 100 000 FE önerisinin gerekçesi ve literatür bütçeleri.**
- Gerekçem literatür değil, sizin verinizdi: YAKO'da 10→30k kazancı %3–7,6 (GA) ve bütçe tavanında eğri düz değil; 100k "düzleşmeyi doğrudan görmek" için bir kontrol deneyi önerisiydi, standart bir bütçe iddiası değildi.
- Literatürdeki tipik FE/analiz sayıları: web aramasında betonarme çerçeve optimizasyonu için dergi makalelerinde değişken sayısı + FE sayısı veren **hiçbir kaynağı doğrulayamadım**. Dolayısıyla: Kaveh/Sahab/Aydoğdu/Gholizadeh vb. için sayı yazmıyorum; tüm kalemler **DOĞRULANAMADI**. Bulunan tek ilgili işaret: Taghizadeh & Gholizadeh (IJOCE, 2026) performans-tabanlı betonarme optimizasyonunda analiz sayısının ana maliyet olduğunu ve vekil model önerdiğini söylüyor (sayı yok); Boscardin, Yepes & Kripka (*Autom. Constr.*) betonarme çerçeve optimizasyonunda kıyas problemi eksikliğini belirtiyor (sayı çıkarılamadı). Hafızamdan sayı vermek uydurma olur; literatür taraması (Literature Analyst) kaynak-sayı tablosunu üretmeli.
- Ön tasarım + kutu ile 100k gerekli mi? **Hayır, ana iddia için gerekli değil.** Öngörüm: kutu/x0 uzayı daralttığı için, tüm-boyut BTO 10k'da zaten 1,40–1,41 M'a ulaşıyor ve YAKO/ATO'nun 30k değerlerinden iyi; düzleşme noktasını bu BTO için ~10–15k FE, ATO/YAKO için (tek-boyut) 30k'nın ötesinde (≥ 40–60k, çünkü 25→30k hâlâ %0,2–0,6) bekliyorum. Bu tahmin ekstrapolasyondur, doğrulanmadı; gerekçesi: 20→30k kazançlarının azalan ama sıfırdan farklı olması ve BTO tüm-boyutun daha iyi bölgeye ulaşması. Önerim: 30k'yı "bütçe tavanı" olarak koruyun; düzleşme/yakınsama iddiası yapmayacaksanız 100k gerekmez. İddia isterseniz ucuz kontrol: tek hücrede (lifsiz A) 3 yöntem × 5 koşu × 60k FE ya da tüm-boyut ATO/YAKO'yu 10–30k'da çalıştırıp BTO-tüm-boyut bandına yaklaşıp yaklaşmadığına bakmak (çapraz teyit; 100k'dan çok ucuz ve daha bilgilendirici).
