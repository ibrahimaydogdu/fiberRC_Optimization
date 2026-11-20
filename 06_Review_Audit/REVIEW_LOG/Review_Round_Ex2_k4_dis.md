# Dış hakem raporu: Örnek 2 (Ö2) kampanya 4 (post-hoc PCR-26b, desenli ℓ/480, zarf Ie)

**Dergi:** *Structures*. **Rol:** bağımsız dış hakem (yapısal betonarme, SFRC, meta-sezgisel optimizasyon). İç Critic raporları okunmadı.
**Tarih:** 2026-11-19. **Okunan:** `04_Results/ex2_stats_campaign4.md`, `04_Results/ex2_analysis_campaign4.md`, `04_Results/ex2/analysis4/` (ham çıktılar), `04_Results/ex2/campaign4/` (koşu dosyalarından örneklem), `03_Code_Execution/EX2_SPEC.md` §11–§13 (§11.1–11.4 dahil), `03_Code_Execution/ex2_work/Ex2Model/Serviceability2D.vb` (`Deflection2D.Report`, `PatternedEnvelope`), `Ex2Evaluator.vb` (g5 bağlantısı), `Ex2Problem.vb` (kesit havuzu), önceki dış hakem raporu `06_Review_Audit/REVIEW_LOG/Review_Round_Ex2_k2_dis.md`. Hiçbir dosya değiştirilmedi, koşu yapılmadı, git kullanılmadı.

## KARAR: KÜÇÜK REVİZYON (koşullu)

Kampanya 4 sayısal olarak tutarlı ve tekrar üretilebilir (§5). Ana bulgu (90 TL/kg'da BTO/ATO 60/60 koşuda lif seçmiyor) K1–K4 boyunca hiç değişmedi; kriter değişikliği bu bulguyu etkilemiyor. Post-hoc kriter seçimi lif aleyhine işlediği için "sonucu kurtarma" şüphesi zayıftır, ama **zaman çizelgesiyle birlikte açıkça** beyan edilmezse hakem güvenini zedeler. Yeni bir kampanya istemiyorum. Koşullar: (i) §1'deki beyan paketi; (ii) F0 için deterministik sehim duyarlılıkları (§2.4; F0'ın ℓ/480 marjı yalnız %1,5); (iii) EX2_SPEC §12'nin bayat sınırlama/yasak maddelerinin güncellenmesi; (iv) önceki turdan açık kalan K2 (R26.4.1.5) ve K3 (lcs ≥ 0,2h) zorunlu maddeleri. Bunlar karşılanmazsa karar BÜYÜK REVİZYON'a döner.

---

## 1. Soru 1: Post-hoc kriter seçimi; beyan ve güvenilirlik

**Olgular.**
- Önceki dış hakem ℓ/240 öncülünü (C+B) önermiş, ℓ/480'in (A) ana seri yapılmamasını açıkça istemişti (`Review_Round_Ex2_k2_dis.md` §2.3, K1). IA A'yı seçti (K3, PCR-26), K3'ün kısıtı yanlış kapsamda kaldı (tüm-yüklü Report; desenli aşım giderilmedi, `EX2_SPEC.md` §11.3 sonuç notu), ardından desenli zarf Ie ile K4 koşuldu (§11.4).
- Kriter, **F0'ın desenli ℓ/480'i 1,0379 ile aştığı biliniyorken** seçildi; P (0,70) ve F90'ın kısıttan etkilenmeyeceği de biliniyordu. Yani seçim sonuca bilgilidir (outcome-informed). 'env' ↔ 'case' seçimi de hesap süresine göre yapıldı (§11.3: env ≈ 2–3×, case ≈ 10× FE süresi).
- **Etiket tutarsızlığı:** §11.4 başlığı "sonuç görülmeden önce kayıt" derken `ex2_analysis_campaign4.md` satır 6 "post-hoc (ön kayıt dışı)" der. İkisi farklı düzeyde doğru (K4 sonucundan önce yazıldı; K2/K3 sonucu ve hakem bulgusundan sonra seçildi) ama makalede tek ifade gerekir.

**Güvenilirlik yargısı.** Zedelemiyor, şu üç nedenle: (a) değişiklik lif lehine değil aleyhine (fp0 tasarrufu %1,074 → %0,958); (b) başlık bulgusu (90 TL/kg'da lif seçilmez) K1–K4'te aynı; (c) K2 seti silinmiyor, birlikte raporlanıyor. Zedeleyecek olan, öncülün **sonuçtan sonra seçildiği** bilgisinin saklanması ya da K2'nin "superseded" diye ana metinden çıkarılmasıdır.

**Beyan önerisi (zorunlu).**
1. Yöntem bölümünde öncül, bina tipine bağlanarak a priori mühendislik gerekçesiyle yazılmalı: Türkiye'deki betonarme çerçevelerde kirişler genellikle hasar görebilir tuğla dolgu duvar taşıdığından ℓ/480 (Tablo 24.2.2) savunulabilir bir öncüldür. Bu gerekçe, ℓ/480'i "fibre sonucunu değiştirmek için" seçilmiş bir eşik olmaktan çıkarıp mühendislik öncülüne dönüştürür. Gerekçe yine de **sonradan** eklendiği için zaman çizelgesi gizlenmemeli.
2. Ek materyalde bir zaman çizelgesi tablosu: K1 (ön kayıt) → K2 (PCR-23/24/25, kusur düzeltmeleri) → K3 (PCR-26, tüm-yüklü ℓ/480, kapsam hatası) → K4 (PCR-26b, desenli ℓ/480). Her satırda: tetikleyici (kusur mu, öncül seçimi mi), sonuçlar görülmüş müydü, en düşük maliyetler (`campaign2_vs_campaign4.md`, `campaign3_vs_campaign4.md`).
3. Önerilen cümle: "The ℓ/480 deflection constraint (patterned live load, envelope Ie) was adopted after the results of campaigns 2–3 and an external review had shown that the cheapest zero-fibre-price design exceeded it; the decision is therefore outcome-informed. Unlike PCR-23–25, which corrected errors, PCR-26b changes the serviceability premise. Campaign 2 results (equivalent to an ℓ/240 premise, under which the reported designs satisfy the limit) are reported alongside as a premise sensitivity."
4. Ana metinde K2 ↔ K4 tek satırlık "öncül duyarlılığı" olarak verilmeli (ℓ/240 öncülü: fp0 tasarrufu %1,074; ℓ/480 öncülü: %0,958). "Kampanya 2 yerine geçti" ifadesi kullanılmamalı; K2 burada kusurlu değil, başka bir öncüldür. (Not: K2'nin ℓ/240 altında uygunluğu yalnız en iyi tasarımlar için gösterildi, `analysis2` F0 0,519; tüm 270 son vektör için bir satırlık ℓ/240 sayımı eklenmesi iyi olur.)
5. "Sonuç görülmeden kayıt" ifadesi yalnız "before campaign-4 results were seen" sınırıyla kullanılmalı.

## 2. Soru 2: Zarf Ie ve lif sertliğinin ihmali

### 2.1 Kod okuması (`Serviceability2D.vb:268–357`)
- Her kiriş grubu için D-durumu Ie/Ig (`rD`, yalnız SW+G momentleri) ve tüm 25 durumun en küçüğü olan zarf Ie/Ig (`rEnv`) hesaplanır; Ie Branson (24.2.3.5a), pozitif/negatif kesit ortalaması (24.2.3.6/7), momentler grup içinde **tüm açıklıkların** en büyüğü.
- dD, rD ile kurulmuş modelden; dDL, rEnv ile kurulmuş tek modelden eleman başına birim L desenlerinin süperpozisyonuyla; dAfter = 2·dD + (dDL − dD). ACI'nin artımsal yöntemiyle (ΔL = ΔD+L − ΔD) uyumlu.
- Durum kümesi = tüm açıklıklar + `RcCombos2D.PatternCombos` (24 desen); yükleme kat bazlı, ACI 6.4.2'nin "incelenen kat" yaklaşımıyla tutarlı. Kat-arası dama tahtası desenleri yok; ACI bunu istemez, beyan yeterli.
- Doğrulama: ham çıktı 'case' 0,9843 ↔ 'env' 0,9847 (`F0_deflection_patterned.txt:504–505`), K2 vektöründe kod g5 = 0,0379 (`F0c2_evalonly_c4rules.txt:21`), PowerShell 'case' 1,0379 ile uyumlu. Zarf ≥ durum-başına sıralaması beklenen yönde. İki varyant arasındaki fark (0,0004) karar açısından önemsiz; 'env' seçimi bu örnekte sonucu belirlemiyor.

### 2.2 Yanlılık yönü (muhafazakâr ↔ muhafazakâr olmayan)
| Varsayım | Yön | Not |
|---|---|---|
| Tek zarf Ie (yüklü olmayan açıklıkta da en düşük Ie) | muhafazakâr | Sayısal etkisi küçük (0,0004) |
| Grup içi en büyük moment (5 m açıklığa 6 m Ie'si) | muhafazakâr | Belirleyici 6 m açıklıkta etkisiz |
| Tüm D sünmesi bağlantı sonrası, ρ′ = 0 | muhafazakâr | |
| **Lif çekme rijitliği / çatlama momenti artışı ihmal** | muhafazakâr, **yalnız lif aleyhine** | Model lifin As'yi düşürmesini (Icr ↓, sehim ↑) görür, rijitlik katkısını görmez → asimetrik |
| Sürekli hareketli yük oranı = 0 (L tamamen geçici) | **muhafazakâr değil** | Konutta L'nin bir kısmı sürekli kabul edilir |
| Branson Ie, düşük ρ (F0 alt donatısı ≈ %0,7–0,8) | **muhafazakâr değil** | Bischoff (ACI 318-19) düşük ρ'da daha büyük sehim verir |
| Rötre/kısıt çatlaması yok (tam fr = 0,62√fc′) | **muhafazakâr değil** | |
| Kolonlar brüt Ig | hafif muhafazakâr değil | |

Sonuç: "muhafazakâr" nitelemesi (EX2_SPEC §11.4, `ex2_analysis_campaign4.md` O2) **genel olarak** yazılamaz; yalnız "Ie zarfı, durum-başına Ie'ye göre muhafazakârdır" yazılabilir. Modelin toplam yanlılığının yönü belirsizdir; lif açısından ise yanlılık kesin olarak lif aleyhinedir.

### 2.3 Savunulabilirlik
Zarf Ie bir optimizasyon kısıtı için savunulabilir (tek faktorizasyon, sürekli tepki, durum-başına Ie ile aynı karar). Lif sertliğinin ihmali de "lif aleyhine, karşılaştırmayı lif lehine bozmaz" gerekçesiyle savunulabilir; ancak bu durumda makale **"ℓ/480 tasarrufu azaltır" yönündeki K2→K4 farkını lif sertliği dahil edilse de geçerli bir bulgu gibi sunamaz** (§3).

### 2.4 Zorunlu deterministik duyarlılık (yeniden optimizasyon yok; F0, F0Z, P `--evalonly` düzeyinde)
F0'ın desenli oranı 0,9847: marj %1,5, model belirsizliğinin çok altında. Kaba tahmin: E13'te dL ≈ 3,5–4 mm; L'nin %25'i sürekli sayılırsa dAfter'a ≈ 2 × 0,25 × 3,6 ≈ 1,8 mm eklenir ve oran 12,5 mm sınırına göre ≈ 1,13'e çıkar. Bu nedenle şu tek satırlık kontroller istenir:
1. Sürekli L oranı ψ = 0,25 ve 0,5 (TS 498/EN 1990 yarı-kalıcı oranıyla gerekçelendirilerek).
2. Bischoff Ie (ACI 318-19 24.2.3.5) ile oran.
3. Lif rijitliği için üst sınır varyantı (ör. SFRC çatlama momentinde fR1 tabanlı artış ya da MC2010 çekme rijitleşmesi; kaynaklı, beyanlı).
Bu üçü F0'ın "ℓ/480'i sağlar" etiketinin dayanıklılığını gösterir. Sağlamıyorsa makale cümlesi "under the stated deflection model" sınırıyla yazılır; yeni kampanya gerekmez.

## 3. Soru 3: K2 ↔ K4 karşılaştırması (+274,15 TL; %1,074 → %0,958)

**Gösterir:**
- Desenli ℓ/480 kısıtı lifsiz ve 90 TL/kg en iyi tasarımlarında pasif (P zarf oranı 0,6962, `P_deflection_patterned.txt:505`); en ucuz lifsiz tasarım K2/K3/K4'te bire bir aynı vektör.
- fp0 kanalında (kiriş derinliği azaltımı) kısıt etkin: K2 vektörü K4 kurallarında uygun değil (g5 = 0,0379), arama kirişleri yeniden dağıtıyor (B_S1, B_S3 500→550; B_S2 550→500) ve bulunan en ucuz uygun fp0 tasarımının bedeli +274,15 TL (+%0,117). Bunun ardından F0 oranı 0,9847, yani kısıt bağlayıcı.
- Sıfır lif fiyatı tasarrufu azalıyor ama yok olmuyor (−2 540 → −2 265 TL).

**Göstermez:**
- Bir gölge fiyat (shadow price) değildir: iki sezgisel aramanın en iyi bulguları arasındaki farktır; iki ikili (a61b0c6b… ↔ f7207d66…) ve iki kural seti arasındadır, test yapılmamıştır. K2 vektörünün K4 ikilisiyle maliyetinin bire bir aynı çıkması (233 916,61) maliyet yolunun değişmediğine iyi bir dolaylı kanıttır; yine de `--deflcheck 0` ile 3 tohumluk bit-bit regresyonun bir satırla kayda geçmesi önerilir (EX2_SPEC'te K3/K4 için regresyon kaydı bulamadım).
- Lifin sehim üzerindeki etkisini göstermez (lif rijitliği yok sayılıyor); "lif sehimi artırır" ya da "lif servis sınırını zorlar" diye okunamaz. Doğru mekanizma: derinlik azaltımı servis marjını tüketir (F0Z, aynı kesitler lifsiz, 0,9548).
- 274 TL (%0,12) duyarlılık büyüklüklerinin altındadır: fR × 0,8 tek başına F0'ı +535,80 TL değiştirir (`ex2_analysis_campaign4.md` §8), lif fiyatı 45 TL/kg +11 520 TL. Bu nedenle "%1,074 → %0,958" iki ondalıklı hassasiyetle yorumlanmamalı; "≈ %1 düzeyinde, ℓ/480 altında biraz azalan" yeterlidir.
- Medyanlar arası fark (BTO −%1,065 → −%0,943) kampanyalar arası test edilmedi; yalnız betimseldir.
- YAKO'da fp0 medyanının lifsizin üstüne çıkması (+%0,189, p_Holm = 1) arama zorluğunu gösterir, lif etkisini değil.

## 4. Soru 4: Yazılabilir / yazılamaz iddialar

**Yazılabilir (rejim kaydı + "under the stated deflection model" ile):**
1. 90 TL/kg'da (ÇŞİDB 2026) BTO ve ATO 60/60 koşuda kirişlere lif seçmedi; en ucuz bulunan lifli ve lifsiz tasarım aynı vektör (236 456,22 TL). YAKO'nun 4/30 lifli koşusu daha pahalı (en düşüğü 237 799,80 TL).
2. Kesit sabit alt sınır başabaş fiyatı fR × 0,6–1,2 için 5,15–13,58 TL/kg (`breakeven_summary.txt`); 90 TL/kg bunun ≈ 6,6–17 katı. Tek sayı değil, alt sınır.
3. Lif fiyatı sıfır kabul edilirse en ucuz bulunan lifli tasarım lifsizden ≈ %1 (2 265 TL) ucuz; ℓ/240 öncülünde ≈ %1,07, ℓ/480 öncülünde ≈ %0,96. Tasarruf kesit küçültmesiyle birlikte oluşur; aynı kesitler lifsiz (F0Z) lifsiz en iyiden pahalıdır (237 504,92 > 236 456,22).
4. Mekanizma: tasarım düzeyinde kazanç beton + kalıptan (derinlik azaltımı); aynı kanal sehim marjını tüketir (P 0,70 → F0 0,98).
5. Kesit sabitken lifin lif dışı tasarrufu 3 314,15 TL'dir; bunun **2 247,76 TL'si boyuna donatıdan** (lifin eğilme katkısı, fib MC2010; ACI 318-14 dışı) ve 1 066,39 TL'si etriyeden gelir (F0 ↔ F0Z, `F0_evalonly.txt:7`, `F0Z_evalonly.txt:7`). Bu ayrım yazılmalı, çünkü fp0 tasarrufunun ≈ %68'i ACI'nin tanımadığı eğilme kredisine dayanır (R26.4.1.5 alıntısı bu yüzden zorunlu; §6).

**Yazılamaz:**
1. "Lif tasarruf sağlar / maliyeti düşürür" bağlamsız (yalnız 0 TL/kg'da var; 45 TL/kg'da bile F0 vektörü P'den 9 254 TL pahalı, kesit sabit).
2. "Lif ekonomik değildir / pahalıdır" genel (alt sınır başabaş, tek ürün, gizli fR verisi, tek rejim).
3. "Tasarımlar ACI 318-14'e uygundur" (karma format; eğilmede lif kredisi ACI dışı).
4. "ℓ/480 kısıtı muhafazakârdır" genel (§2.2); "F0 ℓ/480'i sağlar" model sınırı olmadan.
5. "ℓ/480 kısıtının maliyeti 274 TL'dir" gölge fiyat olarak; "lif sehimi artırır/azaltır"; "lif servis performansını iyileştirir".
6. "Optimum", "global", "MaxFE yeterli", algoritma sıralaması; lifli ↔ lifsiz "anlamlı fark" lif etkisi olarak (uzay kapsama).
7. "Başabaş X TL/kg" tek sayı; "sistem düzeyi başabaş" (üst sınır kurulmadıkça, §6 K4).
8. "ETABS'te doğrulandı" (V3 öncesi); sehim için ETABS doğrulaması zaten verilemez (ETABS çerçeve tasarımı hesaplı sehim kontrolü yapmaz).

**Düzeltilmesi gereken gerekçe:** `ex2_analysis_campaign4.md` §10 madde 5, "lif etriyeyi azaltır yazılamaz" yasağını "etriye kalemindeki fark K2→K4 vektörlerinde yalnız −149 TL" ile gerekçelendiriyor. Bu yanlış karşılaştırmadır: aynı F0 kesitlerinde lif açık/kapalı farkı etriyede **−1 066,39 TL** (F0Z 14 431,43 → F0 13 365,04). Yasak doğru kalabilir, ama gerekçesi §12 madde 6 (Av,min her zaman var; doz ACI 26.4.2.2(d) altında) ve §11.1 kural 2 üçlü sınaması olmalı; zerofibre'ın fR × 0,8 ve MC2010 varyantlarında etriye kalemi ayrıca raporlanmalı (`F0_zerofibre_*` dosyaları yalnız toplam veriyor).

## 5. Soru 5: İstatistik ve sayısal tutarlılık (ham dosyadan bağımsız kontrol)

| Kontrol | Yöntem | Sonuç |
|---|---|---|
| 270 koşu, FEs = 10 000, T = 0 | `analysis4/ex2_runs_campaign4.csv` taraması | 270/270 doğru |
| `analysis4/ex2_runs_campaign4.csv` ↔ `campaign4/runs.csv` | ilk 9 sütun `diff` | aynı |
| Betimsel istatistik (9 hücre: en iyi, ortalama, medyan, kötü, std, lif>0) | Python ile yeniden hesap | `ex2_stats_campaign4.md:44–52` ile tümü 2 ondalığa eşit |
| Mann–Whitney p (9 satır) | scipy, iki yönlü | `:69–77` ile eşit (ör. ATO plain ↔ fp0 1,63e-12; BTO plain ↔ fp90 0,0176) |
| Holm (m = 9) | elle sıralama | tüm düzeltilmiş p'ler tutarlı (0,0176 × 3 = 0,0527; monotonluk ile 0,877 → 1) |
| g5 ihlal sayıları | 30 ham dosyadan toplam | fp0 ATO 44 453, plain ATO 17 156, fp90 BTO 23 402: tablo §1.1 ile eşit |
| Ham koşu ↔ özet | `fibre_fp0/EX2_ATO_…_run03.csv` | maliyet 234 190,763; lif 256 kg; x vektörü `F0_evalonly.txt:3` ile aynı |
| K2→K4 farkları | aritmetik | 234 190,76 − 233 916,61 = 274,15; 2 539,61/236 456,22 = %1,074; 2 265,46/236 456,22 = %0,958 |
| Sıfırlama toplamı | 777,34 + 1 779,15 + 757,65 | 3 314,14 ≈ ALL 3 314,15 (toplamsal) |
| F0 kalemleri | `F0_evalonly.txt:7` | analiz §3'teki kiriş kalemleriyle eşit |

Küçük tutarsızlıklar:
- `ex2_analysis_campaign4.md` §5: "ℓ/240 oranı F0 için 0,4417" tüm-yüklü değerdir; desenli ℓ/240 0,4922'dir (`F0_deflection_patterned.txt:504`). Desenli değer verilmeli.
- §8 son paragrafta üçlü sınama değerleri "(234 190,76; 234 726,56 / 234 726,56)" okunması güç; "ana 234 190,76; MC2010-yerel 234 190,76; fR × 0,8 234 726,56" diye yazılmalı.
- İstatistik notu: ATO fp0'da 23/30 koşu aynı değerde; MWU bağ düzeltmeli normal yaklaşımla hesaplanıyor; p değerleri doğru ama ana metinde değil ek materyalde verilmeli (önceki K7 maddesi; hâlâ geçerli). Ana metinde betimsel + deterministik yeterli.
- §1.1 ihlal sayıları "arama sırasında kısıt etkinliği" olarak yorumlanmamalı; aynı tasarım birçok kez değerlendirilebilir (tekrar sayımı). Analiz §1 son madde "g5 aramada sık etkindir" ifadesi bu sınırla yazılmalı.

## 6. Soru 6: Eksik/zayıf noktalar ve V3 öncesi iş listesi

| # | Önem | İstek | Durum (önceki tur) |
|---|---|---|---|
| K1 | Zorunlu | §1 beyan paketi: zaman çizelgesi, "outcome-informed" cümlesi, K2 = ℓ/240 öncül duyarlılığı olarak ana metinde; §11.4 başlık/analiz etiketi birleştirilsin | yeni |
| K2 | Zorunlu | R26.4.1.5 birebir alıntı + sayfa (kanıt A-satırı). Önemi arttı: fp0 lif dışı tasarrufun ≈ %68'i eğilme kredisinden | AÇIK (önceki K2) |
| K3 | Zorunlu | lcs ≥ 0,2h: kaynak ya da yazar sınırı beyanı + F0'da (K4 kesitleri) etkin olup olmadığı | AÇIK (önceki K3) |
| K4 | Zorunlu | F0 sehim duyarlılığı: ψ = 0,25/0,5 sürekli L, Bischoff Ie, lif rijitliği üst sınır varyantı (§2.4); `--evalonly` düzeyinde | yeni |
| K5 | Zorunlu | EX2_SPEC §12 güncellemesi: "hesaplı sehim desenli L ile yapılmadı (R4, AÇIK)", "servis yalnız ACI 9.3.1.1 yükseklik sınırı, hesaplı sehim yok [K c]" ve yapılamaz madde 7 gerekçesi "servis kısıtı yalnız boyut sınırı" artık yanlış; ℓ/480 öncülü, Tablo 9.3.1.1'in kısıt olmadığı ve modelin yanlılık tablosu yazılmalı | yeni |
| K6 | Güçlü öneri | Başabaş üst sınırı. Dikkat: kaba sınır (P − C_fp0,min)/m_min, havuzun en sığ kirişiyle (250×300, 16 m, 20 kg/m³ → m ≈ 24 kg) 2 265,46/24 ≈ 94 TL/kg verir ve 90 TL/kg'ı **kilitlemez**. Sınır, bir grubun **uygun** olabileceği en küçük kesitle (ör. g3 εt ≥ 0,004 için gerekli en küçük h; 250×450 → ≈ 63 TL/kg, 250×500 → ≈ 57 TL/kg) kurulmalı ve C_fp0,min'in kanıtlanmış minimum olmadığı yazılmalı. Kurulamazsa yalnız alt sınır cümlesi kalır | AÇIK (önceki K4) |
| K7 | Öneri | R8: ACI 6.2.6 oranı F0 1,3755, P 1,3800 (sınır 1,4; marj %1,4–1,8); düzlem dışı k = 1,2 ile P ve F0 `--evalonly`. Ayrıca F0 S3 katı Q = 0,0522 (K2'de 0,0498): ötelenmeli/ötelenmesiz sınıflaması K2→K4'te döndü; V3'te ETABS ile birlikte bakılmalı | AÇIK (önceki K5) |
| K8 | Öneri | K3/K4 ikilileri için `--deflcheck 0` ile 3 tohum bit-bit regresyon kaydı (K2 kurallarını üretiyor mu) | yeni |
| K9 | Öneri | §10 madde 5 gerekçesinin düzeltilmesi (§4 sonu); zerofibre varyantlarında kalem bazlı (boyuna/etriye) çıktı | yeni |
| K10 | Bilgi | Testler ek materyalde; fp0 ↔ fp90 satırı "farklı amaç fonksiyonu, yorumlanmaz" notuyla | önceki K7 |
| K11 | V3 kapsamı | P ve F0 (K4 vektörleri) ETABS'te: kiriş/kolon D/C, desen zarfı, eksenel çekme, Q. Sehim ve lif katkısı ETABS'te doğrulanamaz: E13 (F0) ve E20 (K2 vektörü) için **bağımsız Excel** sehim hesabı (Branson, λ, desen) ve lifli Mn/Vn bağımsız kontrolü (S1) | AÇIK |

## 7. Özet

Sayılar doğru ve tekrar üretilebilir; zarf Ie uygulaması ACI'nin artımsal yöntemiyle tutarlı ve karar açısından 'case' ile özdeş. Asıl risk yöntemsel değil anlatısaldır: ℓ/480 öncülü sonuç görüldükten sonra seçildi ve F0 bu öncülü %1,5 marjla, lif rijitliğini yok sayan ama sürekli hareketli yükü de yok sayan bir modelle sağlıyor. Başlık bulgusu (90 TL/kg'da lif seçilmiyor) bu tartışmadan bağımsızdır ve korunur; fp0 cümlesi "≈ %1, servis öncülüne ve sehim modeline duyarlı, ≈ %68'i ACI dışı eğilme kredisine dayalı" diye yazılmalıdır.
