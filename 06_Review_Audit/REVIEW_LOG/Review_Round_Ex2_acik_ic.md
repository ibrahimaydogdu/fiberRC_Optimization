# Review_Round_Ex2_acik_ic.md — İç Critic: Ö2 açık maddeleri (koşu öncesi)

**Tarih:** 2026-11-16 · **Ajan:** iç Critic (`CRITIC_PROMPT.md`) · **Süre:** hızlı tur
**Okunan:** `03_Code_Execution/EX2_CODE_LOG.md` §5, §5b, §5c; `EX2_SPEC.md` v2.1 (§A.1–A.4, §1–5); `01_Literature/CODE_PROVISIONS_EVIDENCE.md` A43–A54; `04_Results/ex2/v2a_pilot/v2a_design_eval.csv` (paket 2 pilotu, Ex2Model `b30a2aad…`).
**Okunmayan:** ACI ve MC2010 PDF'leri (madde metni bu turda yeniden açılmadı; aşağıda madde numarası verilip kanıt dosyasında satırı olmayanlar **DOĞRULANMALI** işaretlidir). Kod satır satır incelenmedi; davranış CODE_LOG ve pilot çıktısından okundu.

**Genel hüküm:** KOŞULLU ONAY. Kampanyaya (270 koşu) geçiş için iki küçük PCR (madde 1-b ve 3) ve bir V2-a ölçüt düzeltmesi (madde 5) gerekiyor. Bunlar ikili dondurulmadan önce yapılmalı. Hiçbir madde yeni optimizasyon koşusu gerektirmiyor.

---

## (1) Ötelenmesiz kabul ve Q > 0,05

**Karar: (C) seçeneği esas alınır ve ACI 6.6.4.6 yoluna bağlanarak yazılır. (A) ana seride kullanılmaz. (B) ayrı bir hesap olarak gerekmez, çünkü bu çerçevede (C) ile sayısal olarak aynı sonucu verir.**

Gerekçe:
1. ACI Q ≤ 0,05 şartı koymaz. 6.6.4.3 yalnız sınıflama yapar (A52); ötelenmeli kat izinlidir ve kendi büyütme yolu vardır. (A) seçeneği ACI dışı bir tasarım kısıtı olur. Kolonları ACI'nin istemediği bir ölçütle büyütür, maliyeti yukarı iter ve uzayı daraltır.
2. Q kiriş rijitliğine de bağlıdır (0,35Ig). Q kısıtı kirişi kolona bağlar. Lif kirişi incelterek Q'yu büyütebileceği için lifli senaryoyu ACI dışı bir ölçütle cezalandırma riski taşır. Karşılaştırmanın konusu lif olduğundan bu bağ istenmez.
3. Ö1'de Q ≤ 0,05 kısıtı vardır, ama Ö1 depremli ve yanal yüklüdür; orada δs·M2s büyüktür. Ö2'de yanal yük yoktur. Rejim farkı makalede bir cümleyle beyan edilir; tutarlılık için Ö2'ye kısıt taşımak gerekmez.
4. Ö2 geometrisi (5-6-5 m), grup yapısı (kenar/orta kolon simetrik, kat başına tek kiriş grubu) ve yük (tüm açıklıklarda aynı) simetriktir. Gravite analizinde kat ötelenmesi sıfırdır, dolayısıyla M2s = 0. Ötelenmeli katta ACI yolu M = M2ns + δs·M2s olduğundan δs ne çıkarsa çıksın tasarım momenti değişmez. Bu yüzden (B) uygulanırsa sonuç (C) ile aynıdır. Ayrıca 6.6.4.3(a) (ikinci mertebe uç moment artışı ≤ %5) simetrik gravite yükünde sağlanır; bu, "ötelenmesiz" sınıflamasını da savunulabilir kılar (madde metni **DOĞRULANMALI**, A52 yalnız (b) bendini alıntılıyor).
5. Sınıflamanın tasarıma tek etkisi narinlik ihmal sınırıdır (Q > 0,05 katında 22, diğerinde 34 + 12M1/M2 ≤ 40). Mevcut kod bunu yapıyor.

Eksikler ve öneriler:
- **(1-a) Rapor, PCR değil (araç düzeyi):** her en iyi çözümde kat başına Q, δs = 1/(1 − Q) ve gravite analizindeki en büyük kat ötelenmesi (M2s = 0 kanıtı) tabloya yazılır. Pilotta Q = 0,084 için δs ≈ 1,09 [Ç]; 6.6.4.6.2'deki δs ≤ 1,5 eşiğinin çok altında (madde metni **DOĞRULANMALI**).
- **(1-b) Küçük PCR önerilir:** Ötelenmeli katta 6.2.5(a) sınırı kℓu/r ≤ 22'de k ötelenmeli çerçevenin etkin boy katsayısıdır (k ≥ 1). Kod k = 1 alıyor. Bu yalnız ihmal kararında güvensiz tarafta kalır; örneğin h = 550–600 mm kolon k = 1 ile muaf, k ≈ 1,2–1,3 ile muaf değildir [Ç]. En basit çözüm: Q > 0,05 katında ihmal hiç uygulanmaz, δns (Cm = 1, mevcut donmuş yol) her zaman hesaplanır. Bir satırlık değişiklik; k tahmini gerektirmez; muhafazakârdır. Etkisi küçük (sway katı küçük kolonlarda çıkar, onlar zaten 22'yi aşıyor), ama hakem itirazını kapatır.
- **(1-c) Desenli yük aracı (`--patterns`):** asimetrik desende M2s ≠ 0 olur ve orada δs uygulanmıyor. Araç yalnız rapor olduğu için sınırlama olarak yazılması yeterli. İstenirse araçta Q > 0,05 katlarında kolon momentleri δs ile çarpılabilir. Zorunlu değil.
- **(1-d) ETABS V2-a:** ETABS çerçeve tipi (NonSway/Sway) ile kodun kat sınıflaması aynı olmalı. CD3 karşılaştırması `--qcheck 0` ile ETABS NonSway arasında yapılırsa birebir kural eşleşir. Q > 0,05 katlarındaki fark ayrıca belgelenir.
- **Makale dili:** "çerçeve ötelenmesiz kabul edilmiştir" denmez. Önerilen ifade: "Katlar ACI 6.6.4.3'e göre Q ile sınıflandırılmıştır; ötelenmeli sınıftaki katlarda simetrik gravite yükü altında M2s = 0 olduğundan δs momenti değiştirmez; narinlik ihmal sınırı 22 alınmıştır."

**PCR:** (1-b) için küçük PCR (önerilir). (1-a) araç çıktısı.

---

## (2) Kesme kritik kesiti (düğümden d)

**Karar: Kod değişmez. Makalede ve V3'te kural açıkça "eleman ekseninin ucundan (düğümden) d" olarak yazılır. PCR gerekmez.**

Gerekçe:
1. ACI 9.4.3.2 bir izindir, şart değildir (A48: "shall be permitted" yapısı; (a)–(c) koşulları Ö2'de sağlanır). Yüzden d yerine mesnede daha yakın bir kesitte tasarım yapmak her zaman ACI'ye uygundur.
2. Düzgün yayılı yükte |V| mesnede doğru artar. Düğümden d alınan Vu, (hc/2 + d) noktasındakinden wu·hc/2 kadar büyüktür. Pilotta wu ≈ 72 kN/m, hc/2 = 150–175 mm → fark ≈ 11–13 kN, Vu(d) = 181 kN'ın ≈ %6–7'si [Ç]. Hata hep güvenli yöndedir.
3. hc/2 + d kuralı kirişin kesme talebini kolon grubunun düzlem içi boyutuna bağlar. Rijit bölgesiz analiz modeli ve rijit bölgesiz ETABS modeliyle tutarsızlık yaratır. V2-a ve V3 eşlemesini zorlaştırır.
4. Etki her iki senaryoya da aynı yönde uygulanır. Talebin biraz yüksek olması lifin kesme dayanımı katkısının devreye girdiği kirişleri hafifçe artırır; bu yanlılık küçüktür ve beyan edilir.

Yapılacaklar (kod dışı):
- IA kararının metni "yüzden d" yerine "yüz = düğüm varsayımıyla düğümden d (rijit bölge yok; ACI 9.4.3.2'ye göre muhafazakâr)" olarak düzeltilir (spec §A.4 ve §10 V3).
- V3 Excel'i aynı noktadan okur. V2-a B2'de ETABS'in kesme tasarım istasyonu okunur (AÇIK-18). ETABS d-kesit indirimi yapmıyorsa karşılaştırma aynı istasyondaki kuvvet ve Av/s ile yapılır.
- Sınırlamalar listesine bir satır eklenir: "kritik kesit kolon yüzünden değil düğümden d; Vu en çok wu·hc/2 kadar fazla."

**PCR:** Gerekmez.

---

## (3) Kolon lu: eksen boyu mu net yükseklik mi

**Karar: Net yükseklik. Küçük PCR gerekir (kampanyadan önce).**

Gerekçe:
1. ACI'de ℓu kolonun desteksiz (serbest) boyudur; döşeme/kiriş arası net mesafe olarak tanımlanır. Tanım metni kanıt dosyasında ayrı satır olarak yok: **DOĞRULANMALI** (6.6.4.4.? / Bölüm 2 tanımı; PDF sayfası ile A-satırı açılmalı). Q formülündeki ℓc ise eksenler arası boydur (A52); kodda ℓc = 3,3 m doğrudur ve değişmez.
2. Spec §5.1 "lu = net yükseklik" diyor; Ö1 de net yükseklik kullanıyor (MEMORY §3g, PCR-3). Kod eksen boyu kullanıyor. Bu, kod ile spec arasında beyan edilmemiş bir sapmadır. Kural 7 açısından kampanya öncesi kapanmalı.
3. Eksen boyu muhafazakârdır, ama "muhafazakâr" tek başına spec ve Ö1 ile tutarsızlığı haklı çıkarmaz. Net boy hem ACI tanımına hem Ö1'e uyar.

Uygulama:
- lu, Ö1'deki net yükseklik kuralının birebir aynısıyla hesaplanır (hangi kirişin h'si düşülüyor, Ö1 kodundan okunur, yeni kural uydurulmaz). Ö2'de kat başına tek kiriş grubu olduğundan kenar ve orta kolonda aynı değer çıkar.
- Narinlik ihmal kontrolü (6.2.5) ve δns'deki Pc aynı lu ile.
- ETABS V2-a'da "unbraced length ratio (major)" = lu/3,3 olarak atanır. Aksi hâlde ETABS rijit bölgesiz modelde eksen boyunu kullanır ve CD3 farkı çıkar. Atama değeri V2-a kaydına yazılır.
- Test: T12'ye net lu ile bir durum eklenir (ör. 300 kolon, 550 kiriş: lu = 2 750 mm, lu/r = 30,6 [Ç]).

**PCR:** Gerekir (tek satırlık değişiklik + test + ETABS ataması).

---

## (4) ΔVF ≈ Vc'nin %80'i: ana format lif lehine fazla mı, ana seri hangisi

**Karar: Ana seri Q17 artım kuralı olarak kalır (IA'nın 2026-11-16 ön kaydı). Değiştirilmez. Duyarlılık seti yeterli, ama sonuç dili aşağıdaki kurala bağlanır. PCR gerekmez.**

Gerekçe:
1. %80'in kaynağı format değil, MC2010 Eq. 7.7-4'ün yapısıdır. Lif terimi küp kökün içinde (1 + 7,5 fFtuk/fctk) çarpanıyla girer; 4D 30 kg/m³'te bu çarpan büyük çıkar. ρl = AsTop kararıyla (paket 2) ΔVF, CODE_LOG §5b'deki AsBot örneğinden ≈ (AsTop/AsBot)^(1/3) oranında daha da büyür [Ç]. Bu, modelin kendi sonucudur.
2. Formatın lif lehine olan tek kısmı katsayıdır: ana formatta lif terimine φ = 0,75, MC2010-yerelde 1/γc = 0,667 uygulanır. Fark ≈ %11. Yani **MC2010-yerel varyantı zayıf bir duyarlılıktır**; ana sonuçtan ancak %11 uzaklaşır. Ö1 varyantı (γc = 1,5, Vc yerine VRd,F, AsBot) daha geniş bir alt sınırdır. fR × 0,8 ve × 0,6 ise asıl belirsizliği (fR'nin karakteristik olup olmadığı, Q18 ve AÇIK-22) yoklar. Bu üçü birlikte yeterli bir aralık verir; MC2010-yerel tek başına yetmez.
3. Asıl risk format değil veridir: fR değerleri ortalama ise lif katkısı güvensiz yönde abartılır. Ayrıca dozlar ACI 26.4.2.2(d)'nin 60 kg/m³ alt sınırının altındadır; lif kesme katkısı ACI'den beyanlı sapmadır (spec §5.2). Bu iki husus makalede açık yazılır.
4. Pratik etki sınırlıdır. Doz < 60 kg/m³ olduğundan Av,min her lifli kirişte kalır; ΔVF yalnız etriyeyi dayanımın belirlediği kirişlerde Vs'yi düşürebilir. Pilotta etriyeyi smax belirliyor ve dayanımla smax arasındaki fark küçük (kesme D/C 0,98). Ana formatın lif lehine olması, "lif seçilmedi" sonucunu zayıflatmaz; tersine güçlendirir (lif lehine varsayımla bile seçilmedi).
5. Koşu sonucunu gördükten sonra ana formatı değiştirmek sonuca bağlı seçim olur (post-hoc). Bu, Faz 3'teki "ATO kalsın" tartışmasıyla aynı türden bir hatadır.

Ön kayda alınması gereken sonuç dili kuralı:
- "Lif kesmede etriye/maliyet azaltır" iddiası yalnız ana formatta **ve** MC2010-yerel **ve** fR × 0,8'de korunuyorsa yazılır. "fR belirsizliğine dayanıklı" ifadesi için × 0,6 ve Ö1 varyantında da korunmalı.
- "Lif seçilmedi" sonucu ana formatta yazılır; lif lehine format olduğu not edilir.
- Her en iyi çözümde Vc,f/Vc,ACI, VRd,F(0; γc = 1)/Vc,ACI ve etriyeyi belirleyen ölçüt (strength / Avmin / smax) tabloya girer (kod bunları zaten yazıyor).
- MC2010 7.7'de etriyeli elemanda VRd,F + VRd,s toplamının izinli olduğu madde metni hâlâ **DOĞRULANMALI** (önceki tur notu; A54 yalnız 7.7-4/7.7-5'i kapsıyor). Ayrıca Eq. 7.7-4'teki 0,18 katsayısının karakteristik mi ortalama mı kalibre edildiği **DOĞRULANMALI**; γc = 1,0 ile elde edilen değerin "ACI nominaline eşdeğer" sayılması bu bilgiye bağlıdır.

**PCR:** Gerekmez. Sonuç dili kuralı spec §11/§12'ye ön kayıt olarak yazılmalı (koordinatör/Method Architect).

---

## (5) Pilotta etriye aralığını smax belirliyor; B2 dayanım kuralını sınamıyor mu

**Karar: İkinci pilot gerekmez. V2-a B2 ölçütü "sağlanan aralık" değil "dayanımdan gereken Av/s" olarak tanımlanırsa mevcut pilot dayanım kuralını sınar. Küçük araç düzeltmesi önerilir; PCR (değerlendirici) gerekmez.**

Gerekçe (pilot çıktısı `v2a_design_eval.csv`):
- B_S1: bw = 250, h = 550, d = 490, Vu(d) = 181,03 kN, Vc = 114,06 kN. Dayanımdan Vs,gerekli = 181,03/0,75 − 114,06 = 127,3 kN → Av/s = 127 300/(420 × 490) ≈ 0,62 mm²/mm. Av,min/s = max(0,062√30, 0,35) × 250/420 ≈ 0,21 mm²/mm [Ç]. **Dayanım ihtiyacı Av,min'in ≈ 3 katıdır.** Aralığı smax'ın belirlemesinin nedeni dayanım aralığının (≈ 250 mm) smax'a (245 mm) çok yakın olmasıdır, Av,min değil.
- ETABS kiriş kesme çıktısı gerekli Av/s verir (aralık değil). Dayanım ihtiyacı Av,min'in üstünde olduğundan ETABS değeri dayanım kuralından gelir. B2'de bu değer kodun dayanım Av/s'siyle karşılaştırılırsa 22.5 denklemi doğrudan sınanır. ETABS'in Av/s raporunda Av,min'i nasıl birleştirdiği V2-a'da belgelenir (AÇIK-18).
- Pilot çıktısındaki `anyStirrupByStrength(Vs>Avmin),False` etiketi yanıltıcı: Av/s bakımından dayanım > Av,min doğru, yanlış olan "aralığı dayanım belirliyor" önermesi.

Öneriler:
- `--evalonly` çıktısına grup başına `AvS_strength`, `AvS_min`, `AvS_provided` sütunları eklenir; etiket "spacing governed by strength" olarak düzeltilir. Bu yalnız araç/çıktı değişikliğidir, amaç fonksiyonunu etkilemez; yine de ikili SHA'sı değişeceği için kampanya öncesi yapılmalı ve CODE_LOG'a yazılmalı.
- B2 kabul ölçütü: Av/s(dayanım) kod ↔ ETABS 0,98–1,02, aynı istasyonda (madde 2).
- Dayanımın aralığı açıkça belirlediği bir durum istenirse yeni optimizasyon koşusu yapılmaz; pilot vektörde bir kiriş grubunun h'si bir adım düşürülerek (`--evalonly`) deterministik bir ek tasarım üretilir. Bu tasarımın uygun olması gerekmez; B2 karşılaştırması kuvvet ve gerekli Av/s üzerinedir.

**PCR:** Değerlendiricide gerekmez; araç çıktısı için küçük değişiklik (kampanya öncesi).

---

## (6) Lifli süre koşularının en iyisinde lif = 0: 270 koşuya geçmek makul mü

**Karar: Evet, makul. Koşular yapılır. Bu üç sonuç raporlanmaz (tohum 9101, kampanya dışı). Koşulları aşağıda.**

Gerekçe:
1. Sonuç mekanik olarak beklenen yöndedir ve deterministik araçla açıklanmıştır: `--marginal` (tool_smoke) tek grupta lif eklemenin boyuna donatıdan 254–493 TL kazandırdığını, lif bedelinin 3 960–7 920 TL olduğunu gösteriyor. 90 TL/kg'da lif bir mertebe pahalı kalıyor. Av,min (doz < 60 kg/m³) ve smax etriyeyi sabitlediğinden kesmeden kazanç da yok. Lif = 0 bir arama hatası değil, bu fiyatta beklenen sonuçtur.
2. Bu durumda 90 TL/kg kampanyası bir "lif ekonomik mi" deneyi değil, "bu fiyatta optimizer lifi seçer mi" doğrulamasıdır. Asıl bilgi veren kısım fiyat = 0 senaryosudur (90 koşu, `--zerofibre` ile required / neutral / useful / harmful ayrımı). İkisi birlikte yürütülmelidir.
3. Maliyet düşüktür: lifli koşu ≈ 34–50 s (10 000 FE), 270 koşu birkaç saatlik iştir. Yeniden koşu riski yok, bu yüzden pilotu tekrarlamaya gerek yok.

Koşullar:
- Madde 1-b, 3 ve 5'teki değişiklikler yapılıp testler geçtikten, ikili SHA'sı CODE_LOG'a yazıldıktan **sonra** başlanır. Kampanya ortasında kod değişmez (kural 7).
- Ön kayıt (spec §11) şu cümleleri içerir: "90 TL/kg'da lif = 0 beklenmektedir (marjinal tablo); bu sonuç 'lif ekonomik değildir' biçiminde genellenmez, yalnız bu fiyat, bu fR verisi ve doz aralığı için geçerlidir." Basabaş fiyatı (Faz 3'teki tabloya benzer) her en iyi çözüm için deterministik hesaplanır.
- Arama yanlılığı kontrolü: lif değişkeninin kodlamasında doz 0'ın seçilme olasılığı (onarım kuralı "sınıra kırpma" dahil) bir kez belgelenir. Fiyat = 0 senaryosunda lif bazı koşularda seçilmiyorsa bu, aramanın lif bölgesini dolaşıp dolaşmadığı sorusunu doğurur. `--scan` (lifli modda tip × doz çiftleri) en iyi çözümlerde bunu yoklar.
- Fiyat = 0'da bile tüm koşularda lif nötr/sıfır çıkarsa makalenin ana bulgusu "bu çerçevede lifin yapısal kaldıracı yok (Av,min ve smax belirleyici)" olur; bu sonuç Critic ve IA'ya ayrıca sunulur.

**PCR:** Gerekmez (koşu protokolü ön kaydı yeterli).

---

## Özet tablo

| # | Karar | PCR | Kampanya öncesi şart |
|---|---|---|---|
| 1 | (C): Q ile sınıflama + ACI 6.6.4.6 dili; M2s = 0 (simetri) | Küçük PCR önerilir: Q > 0,05 katında narinlik ihmali kaldırılır (k sorunu) | Q, δs, kat ötelenmesi raporu; ETABS çerçeve tipi eşlemesi |
| 2 | Düğümden d korunur (muhafazakâr, ACI'ye uygun) | Hayır | Spec/IA metni düzeltmesi; sınırlama satırı; V3/ETABS aynı istasyon |
| 3 | lu = net yükseklik (Ö1 kuralı birebir) | **Evet** | ETABS unbraced length ratio ataması; T12 testi |
| 4 | Ana seri Q17 artım kalır | Hayır | Sonuç dili kuralı ön kayıt; MC2010 VRd,F + VRd,s ve 0,18 katsayısı DOĞRULANMALI |
| 5 | B2 ölçütü = dayanımdan gereken Av/s; ikinci pilot yok | Araç çıktısı (küçük) | Av/s sütunları; etiket düzeltmesi |
| 6 | 270 koşuya geçilir | Hayır | 1-b, 3, 5 sonrası ikili dondurma; ön kayıt cümleleri; doz-0 yanlılık kontrolü |

**IA onayı gerekir** (kural 10): madde 3 PCR'si ve madde 1-b PCR'si; madde 4 sonuç dili kuralının ön kayda girmesi.
