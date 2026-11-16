# Review Round Faz 3 (iç Critic) — 2026-11-13

Kapsam: `04_Results/phase3_stats.md`, `PREDESIGN_PROTOCOL.md` §8–§9, `maxfe_decision_20k.txt`, ham CSV (`phase3/{20k,30k,bto_alldim}`, `phase2`), PCR-12/13, `SHA256SUMS_phase2/phase3`. Sayılar ham CSV'den bağımsız yeniden hesaplandı. Lifli C tüm-boyut hücresi 8/30 (ön okuma).

## Doğrulananlar
- phase3_stats.md medyanları ham CSV ile tutarlı. Tüm-boyut ve tek-boyut tohumları aynı (1001–1030; 30/30 örtüşme; lifli C 8/8). Eşleşik fark (tüm-boyut lehine, min–medyan): lifsiz A %2,9–7,1; lifsiz C %4,1–6,0; lifli A %8,2–13,2; lifli C (n=8) %7,5–12,7. Her hücrede en kötü tüm-boyut koşusu, en iyi tek-boyut koşusundan ucuz (tam ayrışma, A12=1,00 doğru).
- 30k koşuları aynı tohumlarla sıfırdan koşulmuş; 20k kontrol noktası 20k koşusuyla 39/40 koşuda birebir aynı. **İstisna:** lifli ABC YAKO'da 1 koşuda 20k noktası farklı (tekrarlanabilirlik sapması; `--dop 8` sıra etkisi olabilir; kök neden bilinmiyor).
- Oturma kararı dosyası (2/8 oturmuş, 6 hücre yeniden koşu) betik ve protokolle tutarlı.

## 1. MaxFE okuması
**Yapılabilir (betimsel):** "Bu hareket kuralı ve bu bütçe aralığında 25k→30k medyan iyileşmesi YAKO'da %0,0–0,6, ATO'da 0,0–0,33; bütçe ön kayıtlı üst sınır olan 30 000 FE ile kesildi."
**Yapılamaz:** "MaxFE=30 000 yeterlidir / yakınsama sağlandı / optimum bulundu."
1. Ön kayıtlı kural 30k için durma/oturma ölçütü içermiyor; 30k yalnız bütçe tavanı. Tavan koymak meşru ama "tavan" ≠ "plato". Zayıflık gerçek: düzleşme 30k'da sınanmadı. Makalede "MaxFE=30 000 bütçe tavanıdır, 30k'da düzleşme ön kayıtlı ölçütle doğrulanmamıştır" yazılsın; sonradan ölçüt eklenirse post-hoc etiketli olsun.
2. Lifli A YAKO 20k→30k %1,06 (eşik %1 üstü); 20k→30k koşu bazında hâlâ iyileşenler: lifli A YAKO 8/10, lifli ABC YAKO 8/10, lifsiz A YAKO 6/10. Düzleşme yalnız medyan düzeyinde.
3. **Kritik:** BTO tüm-boyut 10 000 FE medyanları (lifsiz A 1 400 709; lifli A 1 406 575) ATO/YAKO'nun 30k medyanlarından düşük (lifsiz A YAKO 1 425 078, A ATO 1 437 806; lifli A YAKO 1 455 201, A ATO 1 418 829). Yani tek-boyut ATO/YAKO "platosu" problemin değil hareket kuralının durgunluğu olabilir; MaxFE sonucu bu kurala özgüdür, genelleştirilemez.
4. Lifsiz ATO (A, ABC) 20k'da "oturdu" sayılıp 30k'ya taşınmadı; medyan 15k'dan beri sabit (n=10, ayrık medyan). Bütçe asimetrisi (bu hücreler 20k, diğerleri 30k) tabloda açık; "oturdu" yerine "medyan değişmedi (n=10)"; yerel optimuma sıkışma ihtimali belirtilsin.
5. n=10 ile bootstrap GA geniş (ör. lifli A ATO %0,00–2,24); güç düşük.

## 2. BTO tüm-boyut vs tek-boyut
**Doğru çıkarım:** tek-boyut kuralı BTO için tüm-boyuta göre eşleşik %3–13 kalite kaybettirdi; yön tutarlı, istatistik kesin (n=30, Holm m=6, p ≤ 1,8e-10; lifli C n=8 ön okuma, n=30'da yeniden hesaplanmalı).
**Sınırlamalar:**
- **Sürüm farkı:** Faz 2 DLL a2b418c2…875b (4 Kas); tüm-boyut koşusu c1a95951…f5a9 (9 Kas, PCR-13 eklenmiş). PCR-13 varsayılanı değiştirmemeli (lifsiz 33 FE ve x0 1 664 921 aynı), ancak phase3_bin ile phase2_bin arasında **resmi regresyon kaydı yok** (pcr6/7/8/11 regresyonları var; PCR-13 için yalnız duman testi). **Talep:** phase3_bin ile `--onedim 1` BTO (≥3 tohum, 1000 FE) Faz 2 CSV'siyle birebir karşılaştırılsın; birebir ise itiraz kapanır.
- Tohum, bütçe (10 000 FE, ön tasarım FE'si dahil; C kolu 33/44 FE iki tarafta aynı) ve türetilmiş değişkenler iki tarafta aynı: karşılaştırma hareket kuralı farkını adil ölçer (A ve C kolları bağımsız teyit değil, aynı türetme).
- **Baseline-A karşılaştırması geçerli değil:** farklı sürüm (campaign_bin), farklı değişken sayısı (44 vs 24/32), rastgele başlangıç. Ek materyalde "bağlam" etiketiyle; üstünlük/ayrıştırma yargısı yok. Not: tüm-boyut BTO'nun Baseline-A'yı geçmesi, Faz 2'deki "BTO %2–7 kötü" farkının büyük kısmının hareket kuralından geldiğini düşündürür (kesin değil).
- **Lifli/lifsiz etkisi hareket kuralına duyarlı:** BTO A kolunda lifli−lifsiz tek-boyutta +%5,8 (1 590 540 vs 1 503 872), tüm-boyutta +%0,4 (1 406 575 vs 1 400 709). Faz 2'deki "lifli %1,0–5,8 pahalı" bulgusu kısmen hareket kuralı artefaktıdır.

## 3. PCR-12 kararı
IA'nın "ATO tek boyutta iyi ise kalsın" gerekçesi desteklenmiyor: ATO için tüm-boyut kontrolü yok, "iyi" ölçütü tanımsız. Hakem bunu "kural seçimi sonuçlara bakarak yapıldı; BTO'da zarar gösterdiniz, ATO'da ölçmediniz" diye okur. Kabul edilemez risk.
**Öneri: (a)+(c).** ATO tüm-boyut kontrolü (A kolu, lifsiz+lifli, 10 000 FE, tercihen 30 koşu, aynı tohum; `--onedim` kaldırılır, kod hazır) eklenip sonuç ek materyale; ana metinde kısa beyan. (b) tek başına yetmez: ana seri tek-boyutlu, BTO'yu tüm-boyut raporlamak yöntem kıyasını tutarsız yapar. ATO/BTO ana sonuçları "değiştirilmiş (tek-boyut) sürüm" diye adlandırılmalı. "Exploitation artırır" cümlesi silinmeli.
**Beyan taslağı (EN):** "In the main experiments BTO and ATO modify a single randomly selected design variable per move, as YAKO does; this rule was introduced after the first pilot campaign and was not tuned on its results. A supplementary control with all-dimension moves (10 000 FE, 30 runs per cell, same seeds, later program build) gave BTO medians 7–12 % lower than the one-dimension rule (Holm-adjusted p < 1e-7), so the originally stated rationale (greater exploitation) is not supported for BTO. [ATO control: reported in the supplement / not performed.] BTO and ATO costs should therefore be read as properties of the modified move rule, and no ranking or superiority of algorithms is claimed."

## 4. İddia sınırları ve istatistik
**Yapılamaz:** algoritma üstünlüğü; "optimum" (yalnız "best feasible found"); MaxFE yeterliliği/yakınsama; lifin ekonomik/pahalı olduğu (hareket kuralına duyarlı; ayrıca PCR-13: lifli-kapasiteli FSD lifsiz kesitlere indi, nedeni tanılanmadı); kural etkisi ile türetme etkisinin ayrıştığı; tek-boyut platosunun problem platosu olduğu.
**İstatistik:**
- Part 1 betimsel ve doğru etiketli; "Plateau is not an optimum" korunmalı.
- Part 2: Holm m=6 yalnız bu altı testi kapsıyor (kabul); Faz 2 ve Faz 3 aileleri ayrı, birleşik ailede sonuç tam ayrışma nedeniyle değişmez (ek materyalde not). Eşleşik tohum yapısı var; eşleşik Wilcoxon yan analiz olarak eklenebilir.
- Sansür asimetrisi: lifsiz ATO hücreleri 20k'da kesildi; 10k→30k GA'ları hücreler arası karşılaştırılamaz. Uygunsuz çözüm sütunu hep 0, bu yönde sansür yok.
- p-hacking: kural ön kayıtlı, hücre sayısı sabit; belirgin risk yok. Risk post-hoc kararlardadır (PCR-12/13 sonuç görüldükten sonra); protokolde etiketli.

## Karar
**[REVİZYON TALEBİ]**
1. MaxFE: "30 000 bütçe tavanı, düzleşme sınanmadı" ifadesi; "oturdu/yeterli/yakınsadı" yok; lifsiz ATO 20k kesimi ve ayrık-medyan notu.
2. MaxFE sonucunun tek-boyut kuralına özgü olduğu yazılsın (BTO tüm-boyut 10k < ATO/YAKO 30k).
3. phase3_bin ↔ phase2_bin regresyonu (≥3 tohum, `--onedim 1` BTO, 1000 FE, birebir) kayda geçirilsin; yoksa sürüm farkı karıştırıcı olarak beyan.
4. Lifli C tüm-boyut n=30'a ulaşınca phase3_stats yeniden üretilsin; ön okuma yazıya girmesin.
5. Baseline-A karşılaştırması ek materyalde, "bağlam" etiketli.
6. PCR-12: ATO tüm-boyut kontrolü (ideal) veya açık sınırlama; "exploitation artırır" silinsin; yukarıdaki beyan; ATO/BTO "değiştirilmiş sürüm".
7. Lifli/lifsiz kıyası tüm-boyut kuralıyla (en az BTO, tercihen ATO) tekrarlanana dek "lif pahalı/ucuz" iddiası yok.
8. Lifli ABC YAKO'daki 1/40 tekrarlanabilirlik sapmasının nedeni araştırılsın.
9. PCR-13 bulgusunun (lif FSD'de kesit küçültmüyor) nedeni, bulgular bölümünden önce tanılansın.

Genel onay verilmedi; bulgular bölümüne giriş V3 ETABS + S2 + IA onayı koşulu geçerli.

AUDIT_CHECKLIST: sayı–kaynak eşleşmesi [EVET]; sürüm izlenebilirliği [KISMEN: PCR-13 regresyonu yok]; N/A hücre [EVET, yok]; ETABS V3 [HAYIR, bu aşamada kapsam dışı].
