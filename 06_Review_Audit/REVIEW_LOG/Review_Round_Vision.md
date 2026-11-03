# Review_Round_Vision: "Uygulamalı mühendislik optimizasyonu vizyonu, FSD kararı, 1 000 FE hedefi" (Critic, 2026-11-03)

Karar biçimi: **[HAKEM ONAYI, KOŞULLU]** (vizyon kabul; makale iddiası için aşağıdaki şartlar). Tek cümle: "kör algoritma kıyası değil, kılavuzlu, hızlı yakınsayan uygulamalı optimizasyon" konumu Structures için **savunulabilir bir makale türüdür**; ama "FE'yi radikal düşürdük" iddiası bir **referans ölçümü, bir ablasyon ve bir kaçırma (kayıp) raporu** olmadan yapılamaz, ve bu üçü IA'nın kuralları gevşetmesiyle ortadan kalkmaz, çünkü hakemin sorusudur, adalet sorusu değildir.

## 0. Okunan ve ölçülen

Okunan (tam): `CRITIC_PROMPT.md`, `AGENTS.md`/`MEMORY.md` (bağlamla verilen), `Review_Round_Restructure.md`, `Review_Round_FSD_FUD.md`, `PREDESIGN_PROTOCOL.md` (satır 1–40), `HANDOFF.md` satır 137–140 (grep çıktısı: kayıt 68–72), `04_Results/campaign/BTO_plain_fr1.00_run01.csv` (başlık + 8 satır).
Ölçülen (yeni, Python `-I`, CSV salt-okunur; satır 3 = sonuç "Fpen,…,cost,…,T,…", yalnız `fr1.00`, 180 dosya = 3 yöntem x {plain, fibre} x 30; uygun = T = 0; betik scratchpad'de/komut satırında, projede dosya yok): lifsiz 82/90, lifli 79/90 uygun. x0 = 1 554 598 TL (HANDOFF kayıt 72'den alındı, yeniden koşturmadım).

| Ölçü (yalnız T = 0, 10 000 FE sonu) | Lifsiz | Lifli |
|---|---|---|
| En iyi (90 koşunun min.) | 1 390 219 | 1 445 798 |
| En iyi 10 medyanı | 1 435 159 | 1 470 271 |
| Tüm uygunların medyanı | 1 532 593 | 1 578 941 |
| x0'ın en iyiye göre fazlası (x0 − min)/x0 | **%10,6** (alt sınır) | **%7,0** (alt sınır) |
| x0'ın en iyi 10 medyanına göre | %7,7 | %5,4 |
| x0'dan ucuz biten uygun koşu | 51/82 (BTO 28, ATO 20, YAKO 3) | 36/79 (BTO 28, ATO 8, YAKO 0) |

Dikkat: "alt sınır" çünkü Baseline-A yakınsamadı (önceki tur S-A: %2 eşiği medyan 7 756–8 646 FE); gerçek optimum bu minimumdan **düşük olabilir**, dolayısıyla kaçırma ≥ bu değerlerdir. Lifli sütunda x0 lifsiz tasarım (doz taraması öncesi) olduğundan, lifli başlangıç maliyeti bu sütunla birebir karşılaştırılamaz (lifli x0 maliyeti ölçülmedi).
**Okunmayan:** `PreDesign.vb`, `Optimizers.vb` gövdeleri, `PROGRAM_CHANGE_REQUESTS.md` (PCR-8 metni yok; zaten yazılmamış, grep boş), `PROJECT_STATE.md`, `TASKS.md`, `AUDIT_CHECKLIST.md`, `VALIDATION_SPEC.md`, `PRICES_2026.md`, IA'nın direktifinin madde 4'ün devamı (kesik) ve tam metni, literatür (kılavuzlu/warm-start metasezgisel kaynakları: BULUNAMADI, bende yok).

## (a) Kapsam değişikliği: hangi kural gevşer, hangisi kalır

**Makale türü değişimi kabul.** Başlık/özet "YAKO/BTO/ATO karşılaştırması" eksenli ise yeniden çerçevelenmeli: **birincil katkı** = ETABS/FEM entegre, kılavuzlu (FSD ön tasarım + yapısal ön koşul) bir tasarım çerçevesi, lifli/lifsiz ekonomisi; **ikincil** = üç metasezgiselin bu çerçevedeki motor olarak davranışı. Başlıkta "karşılaştırma" geçiyorsa algoritma sıralaması sonuç bölümünün asli iddiasıdır ve kural 8 tam geçerlidir; "kılavuzlu çerçeve" başlığında sıralama alt bulgudur.

| Kural / madde | IA kararıyla gevşer mi | Gerekçe |
|---|---|---|
| 8: "tezdeki koşu sayısı ve bütçe korunur" | **Evet**, zaten Ö1 yeniden tanımlı; yeni kampanya yeni bütçe | Tez verisi bu örnekte kullanılmıyor |
| 8: "aynı MaxFEs, havuz, ceza" **algoritmalar arasında** | **Hayır** (yalnız kılavuz kolu ile kılavuzsuz kol arasında farklı olabilir) | Aynı kılavuzda üç algoritma aynı x0, aynı kutu, aynı bütçe, aynı ceza; yoksa sıralama iddiası yapılamaz |
| 8: "lifli/lifsiz aynı kurallarla" | **Hayır** | (b) |
| 8: ≥ 30 koşu, MWU + Holm | **Hayır**, bütçe 1 000 FE'ye inse de | Metaheuristik stokastik; 1 000 FE'de varyans daha büyük (önceki tur: BTO lifsiz 1 000 FE aralık 1,836–2,074 M); Holm ailesi önceden tanımlı |
| "Rastgele arama ile kıyas eşitliği/adalet" (tekrar eden argüman) | **Evet** (IA'nın tespiti doğru: mühendislikte amaç adalet değil sonuç) | Ama kıyas **raporlanmaz** demek değildir; (b)–(c) |
| 1: ETABS'te doğrulanmamış optimum raporlanamaz | **Hayır** | V3 kapısı |
| 11: kaynaksız kural/sayı | **Hayır** | Yapısal kılavuzların her biri atıflı veya türetilmiş olmalı |
| 10: IA onayı, Critic onayı yetmez | Değişmez | Ama bu direktif IA'nın sözlü/yazılı onayıdır; kayda geçirilir (e) |

**Referans ölçüm şart mı: evet.** "X FE'de Y kalitesine ulaştık" cümlesi, Y'nin *neye göre* kaliteli olduğunu ister. Referans = Baseline-A (rastgele başlangıç, tam havuz, 10 000 FE, 30 koşu). Bu referansı tutmak **adaletle ilgili değil, ölçümle ilgilidir**: iyileşme yüzdesi ve FE tasarrufu ancak bir sıfır noktasına göre tanımlanır. Rastgele arama "yüceltilmez", bir *olgu* olarak (ölçek referansı) kalır. Uyarı: Baseline-A zayıf bir rakiptir (saman adam riski): hakem "10 000 FE rastgele başlayan BTO'yu yenmek kolay" der. Bu yüzden **referans kümesi** üç kolla genişletilmeli (hepsi aynı FE eksenine çizilir; maliyeti düşük):
1. R: Baseline-A (rastgele başlangıç, 44 değişken) — arşiv, zaten var.
2. G: **x0 tek başına** (algoritmasız; 1 554 598 TL, 63 FE) ve **x0'dan yerel arama/açgözlü iniş** (aynı bütçe). Restructure raporunun 4a "yapısal uyarı"sı: x0 verilince algoritmanın ne eklediği ölçülmezse "kılavuz yeter" ile "algoritma + kılavuz" ayrılamaz.
3. W: x0'dan **ısıtmalı başlatma** (warm-start) yapılmış ama kutu/rho/ön koşul kısıtı olmayan metasezgisel (yalnız C faktörü).
Bu kollar olmadan "FE'yi radikal düşürdük" cümlesinin **nedensel kısmı** (hangi bileşen sayesinde) yazılamaz; yalnız betimsel kısmı ("bu protokol 1 000 FE'de R'nin 10 000 FE medyanına eşit/altında") yazılabilir.

**Metin/tablo düzeyinde iddialar**

| Yapılabilir | Yapılamaz |
|---|---|
| "Önerilen çerçeve, 1 000 FE'de (ön tasarım dahil) R kolunun 10 000 FE medyan uygun maliyetine ulaştı/ulaşamadı (Tablo, 30 koşu, %95 GA)" | "Optimumu bulduk", "global optimuma yakın" (referans yakınsamadı) |
| "FE-to-target, hedef = R'nin 10 000 FE medyanı (1,53 M TL lifsiz)" | "BTO en iyi algoritmadır" (yalnız "bu çerçevede, bu bütçede, Holm düzeltmeli testte") |
| "Kılavuzun tasarruf kaynağı A/B/C ablasyonu ile şu orandadır" (ablasyon varsa) | "Ön tasarım optimumu kaçırmaz" (kaçırma ölçülmeden) |
| "Kılavuzlu çözüm en iyi bilinen çözümden %X yukarıdadır" (BKS tanımı verilmişse) | "Rastgele arama adaletsizdir, bu yüzden kıyas yok" gibi gerekçe cümleleri (kıyası değil, kıyasın amacını yazın) |
| "Lifli/lifsiz fark, bu kılavuzlu çerçevede ve ACI Bölüm 18 + sarılmada lif = 0 varsayımlarında şu büyüklüktedir" | "Lif kirişte X % tasarruf sağlar" genel iddiası |

## (b) Adaletsizlik kaygısının kalkması lifli/lifsiz karşılaştırmasını etkiler mi

**Etkiler, ters yönde:** "kıyas adaleti" kaygısı **algoritma-rastgele** ekseninde kalkıyor; **lifli-lifsiz** ekseninde **olduğu gibi kalıyor** ve daha da önem kazanıyor, çünkü artık başlangıç noktası ve kutu tasarımcı kararıdır ve bir senaryoya sızabilir. Gerekçe (hakem dili): lifli/lifsiz karşılaştırması bir *nedensel* iddiadır ("lif maliyeti düşürür/düşürmez"); nedensellik için iki kolda **tek fark lif** olmalıdır. Aynı x0, aynı kutu, aynı ceza, aynı bütçe, aynı seed. Aksi hâlde fark lifle kılavuz farkından ayrılmaz.

**Ön tasarım lifsiz yapılırsa yanlılık var mı: evet, çift yönlü, ikisi de raporlanmalı.**
- *Lif aleyhine:* x0 lifsiz kapasite ile bulunan, ayrıca **kolon ağırlıklı** (kolon hacmi payı vekil ölçümle x0'da %46, Baseline-A en iyilerinde medyan %39; Restructure 3b) bir noktadır. Lifin etkisi yalnız kirişte (Mpr + orta bölge kesmesi). Kolon ağırlıklı başlangıç + kolon-kiriş zinciri (A5) kirişi küçültmeyi zorlaştırıyor olabilir; yani lifin işleyeceği yer (kiriş) kılavuzla daralmış olabilir. Baseline-A'da lifli en iyi (1,446 M) lifsiz en iyiden (1,390 M) **yüksek**: lifin avantaj getirmemesi aramanın yetersizliği ya da gerçek ekonomi olabilir, ayrılmadı.
- *Lif lehine:* lifli kolda doz taraması (+40 FE) ve lifsiz uzayın lifli uzayın alt kümesi olması nedeniyle lifli x0 ≤ lifsiz x0 inşa gereği garanti; "lifli ≤ lifsiz" bulgusu kısmen **tanım gereği** çıkar (FSD_FUD turu 1d ile aynı mantık).
- *Bütçe asimetrisi:* ön tasarım FE'si lifsiz 63, lifli 103 (Restructure 4b); bütçeye dahil edilirse lifli koldan 40 FE fazladan eksilir.
**Gerekli:** (i) iki senaryoda aynı x0 + yalnız lifli kolda ortak doz taraması (mevcut protokol, korunur); (ii) **duyarlılık kolu**: lifli kapasiteyle ayrı FSD yapılmış x0 ile aynı deney (en az BTO, 30 koşu), iki x0 ile fark işareti değişmiyorsa iddia sağlam; (iii) lifsiz optimum + doz taraması eşleştirilmiş analizi (FSD_FUD 1d) ayrı etiketle; (iv) sonuç dili: "lifli/lifsiz fark kılavuzdan bağımsızdır" ancak (ii) geçerse yazılır.

## (c) 1 000 FE hedefinin metodolojisi

**Tanım (koşu öncesi donsun):**
- FE = FEM analizi (kampanya FE sayacı); **ön tasarım FE'si bütçeye dahildir** (birincil): lifsiz 63, lifli 103 (Restructure 4b); hariç tutan analiz ikincil. Kılavuz maliyeti (kod geliştirme, kural türetme süresi) FE ile ölçülmez; "tek seferlik, problem sınıfına özgü" diye beyan edilir. Duvar süresi de raporlanır (rho türetmesi FE başına süreyi artırır; kaynaklı dakika değeri 4,3 mü 6,2 mi hâlâ teyit bekliyor, Restructure Q8).
- **Başarı** = 1 000 FE bitiminde **uygun (T = 0)** en iyi çözüm bulunması ve maliyeti ≤ hedef.
- Hedef üç kademe, hepsi önceden: (H1) R'nin 10 000 FE sonu medyan uygun maliyeti (1,53 M lifsiz / 1,58 M lifli; yukarıdaki ölçüm); (H2) BKS'nin +%5'i; (H3) BKS'nin +%2'si. BKS = tüm kampanyalarda bulunmuş **en düşük uygun maliyet** (şu an lifsiz 1 390 219, lifli 1 445 798), tarih ve kaynakla **güncellenir**, "optimum" diye adlandırılmaz.
- **Metrikler:** (1) 1 000 FE sonunda en iyi uygun maliyet (medyan, çeyrekler, min–maks; 30 koşu); (2) ε-gap = (maliyet − BKS)/BKS, medyan ve %95 bootstrap GA; (3) **FE-to-target** (H1/H2/H3'e ilk ulaşılan FE; ulaşamayan koşu sansürlü, silinmez, Kaplan–Meier + log-rank); (4) başarı oranı (H1/H2/H3'e ulaşan koşu/30) ve Fisher kesin testi; (5) ilk uygun çözümün FE'si; (6) T > 0 ile biten oran (ceza boşluğu; PCR-11 günlüğü şart, mevcut CSV'de T geçmişi yok); (7) duvar süresi.
- **BKS belirsizliği:** referans yakınsamadığı için ε-gap **optimistik değil, yukarı yanlı bir ölçüt** değildir; tersine gap'i *küçük* gösterir (BKS gerçek optimumdan yüksek olabilir). Bu yüzden iki önlem: (i) 50 000 FE BTO x 30 koşu tavan referansı (FSD_FUD önerisi, hâlâ geçerli) ile BKS'yi iyileştir; (ii) makalede ε-gap'ten önce "BKS ≥ optimum" ifadesi.
- **Tek referans (Baseline-A) sınırı:** (i) tek derleme, tek ceza, tek seed dizisi; (ii) yakınsamamış; (iii) *kendi geliştirme sürecinde görüldü* (iki başarısız kapsama testi ve hiyerarşi analizi ona bakılarak yapıldı), dolayısıyla yeni kural/hedef için **temiz test kümesi değil** (Restructure 2a notu). Çözüm: **bağımsız pilot** (yeni seed, 30 koşu BTO 10 000 FE; ~4–6 saat) hedef (H1–H3) ve BKS'yi donduran küme olsun; Baseline-A yalnız tarihî ölçüm. Pilot, kılavuz kuralları donduktan sonra, kuralların tasarımına bakılmadan yapılır.
- **Bütçe 1 000 FE'nin seçilmesi post-hoc**: hedef üçüncü kez kaydı (kapsama → yakınsama hızı → 1 000 FE). Bu yasak değil, **HARKing kaydı** gerektirir: PREDESIGN_PROTOCOL'e tarihli revizyon "1 000 FE hedefi, bağımsız pilot sonrası belirlenir; pilotun yakınsama eğrisinden gerekçelendirilir" şeklinde. 1 000 sayısı yuvarlak bir seçimse bu açıkça yazılır; ayrıca 500/1 000/2 000/5 000 FE'de aynı metrikler raporlanır (eşiği tek noktada "yakalamak" yerine FE–kalite eğrisi).

## (d) "Kolon ağırlıklı yerel optimum kabul" ifadesi

Mevcut veriden kaçırma tahmini (yukarıdaki tablo): x0, Baseline-A'nın en iyisine göre **lifsizde ≥ %10,6, lifli ≥ %7,0** pahalı; en iyi 10 medyanına göre %7,7 / %5,4. Bu **x0'ın** kaçırmasıdır; makalenin iddiası ise **x0'dan sonra aramanın** ulaştığı noktadır. Yani doğru ölçüt: 1 000 FE sonundaki ε-gap. Kaçırma "tahmini" bu yüzden iki kademeli verilir: (i) kılavuz noktası x0 için ≥ %7–11 (alt sınır, Baseline-A yakınsamadı); (ii) kılavuzlu aramanın sonu için ε-gap (deney sonrası).
İfade önerisi: **"pratik optimum" denmez** (optimumluk iddiası referansla desteklenmedi); "uygulanabilir, hızla elde edilen, en iyi bilinen çözümün %X üzerinde (BKS tabanlı) bir tasarım" denir. "Pratik" kelimesi ancak X raporlanırsa kullanılır. IA'nın "kolon ağırlıklı optimum sorun değil" tezi **mühendislik açısından savunulabilir** (kolon-kiriş yönlendirmesi, kapasite tasarımı), makale açısından şartlıdır:
1. Kaçırma miktarı (TL ve %) tabloda, lifli ve lifsiz ayrı.
2. Kolon/kiriş maliyet payı (çelik dahil, vekil beton hacmi değil) x0 ve BKS için raporlanır; çünkü lifin etkisi yalnız kirişte, kolon ağırlıklı çözüm **lifin kazancını sistematik küçültebilir** (b). Bu, lifli/lifsiz sonucu doğrudan etkileyen **yönlü yanlılıktır**; makale "lif faydası kılavuzlu kolon ağırlıklı çözüm bölgesinde bu kadar" diye *sınırlandırılmış* yazmalı.
3. Kolon ağırlıklı bölgede lifin etkisiz kalması "lif ekonomik değil" sonucuna **dönüştürülemez**.
Not: kolon payı ölçümüm hacim vekili (çelik/etriye hariç) ve kolon/kiriş eleman sayıları benim varsayımımdır (Restructure §0); bu raporda yeniden hesaplamadım.

## (e) Kurallar ve süreç

**Fiilen değişen:** (i) kural 8'in "kör kıyas/adalet" ruhu (kılavuzlu kol ile rastgele kol arasındaki eşit koşul); (ii) amaç hiyerarşisi: birincil = hızlı yakınsama + uygulanabilir ekonomi; (iii) FSD tartışması kapatıldı (karar kaydı); (iv) bütçe odağı 10 000 → 1 000 FE; (v) PCR-8 (kolon rho türetmesi, 44 → 38) onaylanmış sayılır. **Değişmeyen:** kural 1 (V3 ETABS), 2–4, 6, 7 (kod yalnız PCR + onayla), 10 (IA onayı), 11, 12.
**Kayda geçirilecek (kullanıcı kararı 2026-11-03):** AGENTS.md "Makale (aktif)" paragrafına ve kural 8'e "kılavuzlu uygulamalı optimizasyon; kıyas adaleti yalnız algoritmalar arası ve lifli/lifsiz arası; referans (Baseline-A, bağımsız pilot) ölçek olarak tutulur" notu; MEMORY.md yeni bölüm 3h: (1) amaç/vizyon, (2) FSD kararı kapalı, (3) hedef ≤ 1 000 FE (tanım ve metrikler donmuş olarak), (4) PCR-8 onayı, (5) hiyerarşi kuralının durumu (reddedildi, gevşek sürüm ancak bağımsız pilotla); PROJECT_STATE: karar tarihi, hangi turun hangi kararı değiştirdiği, Baseline-A'nın "tarihî ölçüm + görülmüş" etiketi. PREDESIGN_PROTOCOL.md'ye post-hoc revizyon kaydı (tarih, gerekçe, neden değişti, kapsama testi 0/10 ve 0/30 başarısızlığı).
**Critic yine de durdurur:** (1) bağımsız pilot ve donmuş hedef/metrik olmadan "1 000 FE" iddiası içeren metin; (2) V3 (ETABS) olmayan optimum; (3) PCR-8 kodu, onaysız veya legacy bayraksız (Baseline-A regresyonu 6/6 birebir şartı); (4) kaynaksız hiyerarşi/ön koşul kuralı; (5) Baseline-A'ya bakılarak FSD ayarının dördüncü kez yinelenmesi; (6) ceza değişikliği; (7) lifli/lifsiz kıyası iki farklı başlangıç/kutu ile; (8) lif fiyatı/EN 14651 kırmızı bayrağı (tedarikçi yanıtı beklemede); (9) "global/optimum" kelimeleri referanssız.

## (f) Net öneri (≤ 10 satır)

1. Vizyon kabul: makale "kılavuzlu uygulamalı optimizasyon" olarak çerçevelenir; başlıkta "karşılaştırma" yerine "kılavuzlu çerçeve"; YAKO/BTO/ATO ikincil bulgu.
2. Referans zorunlu: R = Baseline-A (tarihî) + **bağımsız pilot** (BKS ve hedefi dondurur) + G (x0 ve x0'dan yerel arama) + W (yalnız warm-start). Yoksa "FE'yi düşürdük" yazılamaz.
3. Ablasyon zorunlu (A rho, B ön koşul, C x0), 30 koşu, eşleştirilmiş seed; yoksa tasarruf kaynağı yazılamaz.
4. 1 000 FE tanımı: ön tasarım FE'si dahil, başarı = uygun + hedef (H1 R medyanı, H2 BKS+%5, H3 BKS+%2); metrikler ε-gap, FE-to-target (sansürlü), başarı oranı, FE–kalite eğrisi (500/1 000/2 000/5 000).
5. Lifli/lifsiz: aynı x0/kutu/ceza/seed; lifli-kapasiteli x0 duyarlılık kolu; kolon ağırlıklı çözüm lif lehine değil aleyhine yanlı olduğu için sonuç "bu kılavuzlu bölgede" ile sınırlı.
6. "Pratik optimum" yok; "kaçırma ≥ %7–11 (x0, alt sınır)" ve deney sonu ε-gap raporlanır.
7. Hiyerarşi kuralı hâlâ kaynaksız ve Baseline-A'nın en iyi 10'unun 8'ini dışarıda bırakıyor: yeniden gündeme gelirse atıflı + bağımsız pilot kapsama ≥ 9/10; PCR-8 (legacy bayraklı) onaylı.
8. Kayıt: AGENTS kural 8 notu, MEMORY 3h, PROJECT_STATE, PREDESIGN_PROTOCOL post-hoc revizyonu (kullanıcı kararı 2026-11-03).
9. IA'ya sorular: (Q1) madde 4'ün kesik devamı nedir ("Çalışma Pren…" ile biten; "Çalışma Prensipleri/Prensibi"?) ve tam metin; (Q2) 1 000 FE ana kampanyanın yerini mi alıyor, yoksa 10 000 FE tam kampanya da (kılavuzlu uzayda) koşulacak mı; (Q3) ön tasarım FE'si bütçeye dahil mi; (Q4) bağımsız pilot (4–6 saat) ve lifli-FSD duyarlılık kolu kabul mü; (Q5) başlık/özetteki YAKO/BTO/ATO iddiası düşürülsün mü; (Q6) PCR-8'de kolon beton sınıfı ve etriye çapı türetme kararı (Restructure Q2, Q7).
10. Süre/FE: 4,3 mü 6,2 dk/1 000 FE teyidi (Restructure Q8) gerekli; ölçülmeden duvar süresi iddiası yazılmaz.

## Okunmayan / doğrulanmamış (özet)
`PreDesign.vb`, `Optimizers.vb`, `PROGRAM_CHANGE_REQUESTS.md` (PCR-8 yok), `PROJECT_STATE.md`, `TASKS.md`, `AUDIT_CHECKLIST.md`, `VALIDATION_SPEC.md`, `PRICES_2026.md`; IA direktifinin tam metni (madde 4 kesik); literatür (warm-start/kılavuzlu metasezgisel; BULUNAMADI); kolon payı vekil sayıları önceki turdan alındı (yeniden hesaplanmadı); x0 maliyeti HANDOFF kayıt 72'den; lifli x0 maliyeti bilinmiyor.
