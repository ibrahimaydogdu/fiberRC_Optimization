# Dış Hakem Raporu: Ö2 açık maddeleri (EX2_CODE_LOG §5b/§5c, EX2_SPEC v2.1)

**Tarih:** 2026-11-16 · **Hakem rolü:** bağımsız dış hakem (*Structures*; RC çerçeve optimizasyonu, ACI 318, SFRC) · **İç Critic raporları okunmadı.**
**Okunan:** `03_Code_Execution/EX2_CODE_LOG.md` (§5, §5b, §5c), `03_Code_Execution/EX2_SPEC.md` v2.1 (§A.1, §A.4, §5.1–5.2, §11–13), `01_Literature/CODE_PROVISIONS_EVIDENCE.md` A43–A54.
**Kapsam dışı:** kod, ikili, ETABS ve koşu çıktısı açılmadı. Aşağıdaki sayılar kod günlüğünden alınmıştır; hakem tarafından yeniden üretilmedi.

**Genel değerlendirme:** 270 koşuluk kampanya başlamadan önce **amaç fonksiyonunu değiştiren üç karar** (madde 1, 3, 4) kesinleşmeli ve ikili dondurulmalıdır. Madde 2 kod değişikliği gerektirmez (yalnız beyan). Madde 5 yalnız doğrulama (V2-a) için ikinci, deterministik bir tasarım gerektirir. Madde 6'da kampanyaya geçmek makuldür. Ayrıca 4, 5 ve 6 birbirine bağlıdır: etriye smax/Av,min ile belirleniyorsa lifin kesmedeki katkısı maliyete hiç yansıyamaz. Bu yüzden "lif = 0" sonucu arama başarısızlığından değil, modelin mekaniğinden kaynaklanıyor olabilir (bkz. madde 6).

---

## (1) Ötelenmesiz kabul ve Q > 0,05 (AÇIK-17, AÇIK-19)

**Karar: (B), sınırlı biçimde.** Kat Q'suyla sınıflama sürsün. Q > 0,05 olan katta ötelenmeli çerçeve kuralları eksiksiz uygulansın. (C) tek başına kabul edilemez. (A) ancak ikinci seçenek olabilir.

**Gerekçe:**
- ACI 6.6.4.3 / 6.6.4.4.1 (A52) Q ≤ 0,05 olan katı ötelenmesiz sayar. Bu şart sağlanmazsa kat ötelenmeli kabul edilir ve 6.6.4.6 uygulanır. ACI, gravite yükü altında Q > 0,05 olan bir katı yasaklamaz. Bu nedenle (A), yönetmelikte bulunmayan yapay bir kısıt olur ve uygun bölgeyi tam da "ucuz küçük kolon" tasarımlarının olduğu yerde keser. Hakemin ilk sorusu "bu kısıt nereden geliyor?" olur.
- Mevcut davranış (C), yani sınıflama yapıp δs uygulamamak, iki noktada eksiktir.
  1. **k katsayısı:** ötelenmeli katta 22 sınırı **kℓu/r** için geçerlidir ve k ≥ 1,0'dır (ACI 6.6.4.4.3, Jackson–Moreland). Kod sınır 22'yi k = 1,0 ile karşılaştırıyorsa narinliği ihmal kararı güvensiz yöndedir. **Developer'ın kontrol etmesi gereken nokta:** ötelenmeli katta hangi k kullanılıyor?
  2. **δs:** simetrik tasarım ve simetrik yükte M2s ≈ 0 kabulü fiziksel olarak savunulabilir. Yine de bu kabul kodda **hesapla** gösterilmeli, yalnız beyanla bırakılmamalıdır: δs = 1/(1 − Q) ≥ 1 (6.6.4.6.2(a)) uygulanır ve kat Q ≤ 0,33 (δs ≤ 1,5) şartı kısıt olarak kodlanır. Böylece M2s'nin sıfır olmadığı durumlarda (desenli yük aracı, asimetrik grup ataması) büyütme kendiliğinden devreye girer. Pilotta Q = 0,076/0,084 için δs ≈ 1,08–1,09 olduğundan maliyet etkisi küçük beklenir. Ancak yöntem yönetmeliğe uygun ve kapalı olur.
  3. ACI 6.2.6 sınırı (ikinci mertebe toplam moment ≤ 1,4 × birinci mertebe) da ucuz olduğu için eklenmelidir.
- **Makale dili:** "Çerçeve ötelenmesiz kabul edilmiştir" cümlesi kaldırılmalıdır. Yerine şu yazılmalıdır: "Her değerlendirmede kat stabilite indeksi Q (ACI 6.6.4.4.1) hesaplanmış; Q ≤ 0,05 katlar ötelenmesiz, diğerleri ötelenmeli (6.6.4.6) olarak tasarlanmıştır." Kaç en iyi tasarımda ötelenmeli kat çıktığı raporlanmalıdır. Düzlem dışı "tutulu" varsayımı (2B model) ayrı bir sınırlama olarak kalır.
- **(A) ne zaman seçilir:** IA basitlik isterse (A) uygulanabilir. Bu durumda Q ≤ 0,05 kısıtı "tasarım tercihi (ötelenmesiz çerçeve hedefi)" diye beyan edilmelidir. Ayrıca en iyi çözümlerde kısıtın etkin olup olmadığı (kaç koşuda bağlayıcı) ve gevşetilince maliyet farkı raporlanmalıdır.

## (2) Kesme kritik kesiti: düğümden d mi, mesnet yüzünden d mi (AÇIK-14)

**Karar: Kod değişmesin; tanım ve beyan düzeltilsin.**

**Gerekçe:**
- ACI 9.4.3.2 (A48) yüzden d uzaklığa kadar olan kesitlerin kritik kesitteki Vu ile tasarlanmasına **izin verir**, bunu zorunlu tutmaz. Düğümden d almak Vu'yu yaklaşık w·hc/2 kadar büyütür. Pilot için kabaca 70 kN/m × 0,15 m ≈ 10 kN, yani Vu'nun yaklaşık %5'i. Bu fark güvenli yöndedir ve yönetmelik ihlali değildir.
- Analiz modelinde rijit bölge yok. Eğilme momenti de düğümde (eksende) alındığından kesme ile eğilme aynı referansta tutarlıdır. Kesmeyi yüze taşıyıp eğilmeyi eksende bırakmak tutarsızlık yaratır.
- Etriye zaten smax ile belirleniyor (madde 5). Bu yüzden düzeltmenin maliyet etkisi büyük olasılıkla sıfırdır. Kampanya öncesi kod değiştirmek riski artırır, getirisi yoktur.
- **Yan etki (beyan edilmeli):** büyütülmüş Vu, lifin kesme katkısının işe yarayacağı bölgeyi yapay olarak genişletir ve **hafifçe lif lehine** çalışır. Kesme smax ile belirlendiği sürece bu etki pratikte görünmez.
- **Yapılacaklar:** (i) makalede "mesnet yüzü" yerine "düğüm ekseninden d uzaklık (ACI 9.4.3.2'ye göre güvenli yönde; rijit uç bölgesi modellenmedi)" yazılsın. (ii) V3 Excel'i ve ETABS okuma istasyonu aynı tanıma bağlansın. ETABS'in kesmeyi hangi istasyonda tasarladığı V2-a'da okunmalıdır (AÇIK-18). ETABS d indirimi yapmıyorsa fark ETABS tarafında güvenli yöndedir ve raporlanır. (iii) İsteğe bağlı olarak en iyi çözümlerde hc/2 + d deterministik yeniden değerlendirmesi yapılsın (tek satırlık duyarlılık).

## (3) Kolon lu: eksen mi, net yükseklik mi (AÇIK-15)

**Karar: lu = net yükseklik (3,3 m − üstteki kiriş h). Q ve EI için ℓc = 3,3 m (eksen) kalsın.** Kampanyadan önce kodlanmalıdır.

**Gerekçe:**
- ACI'de ℓu, kolonun döşeme/kirişler arasındaki net desteksiz boyudur. ℓc (6.6.4.4.1, Q ve kritik yük için) ise eksenden eksene boydur. İki büyüklük yönetmelikte ayrı tanımlanır ve spec §5.1 de "lu = net yükseklik" der. Ö1'de (PCR serisi, MEMORY §3g) lu net alınmıştır. Ö2'nin farklı olması örnekler arası tutarlılığı bozar.
- Etki küçük değildir. Pilottaki 300 mm kolonda lu/r eksenle 3 300/90 = 36,7, netle (kiriş 550) 2 750/90 = 30,6 olur. Bu değerler 22 ile 34 + 12M1/M2 ≤ 40 arasındaki sınırların tam ortasındadır. Narinliğin ihmal edilip edilmeyeceği ve δns değişir. Dolayısıyla seçilen kolon kesitleri de değişir. "Muhafazakâr" olduğu için eksen boyu bırakılırsa ucuz kolonlu tasarımlar sistematik olarak cezalanır ve maliyet raporu ACI'nin istediğinden pahalı olur.
- **ETABS tutarlılığı:** uç ofseti yoksa ETABS desteksiz boyu eleman boyu (3,3 m) olarak alabilir. V2-a'da ETABS'te "unbraced length ratio (major) = (3,3 − hb)/3,3" açıkça atanmalı ve okunarak teyit edilmelidir. Aksi hâlde V2-a farkı yanlış yere bağlanır.

## (4) ΔVF ≈ Vc'nin %80'i: ana format lif lehine fazla mı, ana seri hangisi?

**Karar: Ana seri MC2010-yerel olsun** (Vc,ACI ile φ; lif artımı ΔVF γc = 1,5 ile, ek φ yok). Q17 "Vc + ΔVF(γc = 1,0) sonra φ" üst sınır duyarlılığına, Ö1 formatı ise yalnız Ö1 ile karşılaştırma duyarlılığına inmeli. Kampanya başlamadığı için değişikliğin maliyeti sıfırdır. Değişiklik IA kararıdır (Q17 yeniden açılır).

**Gerekçe:**
- ΔVF ≈ 0,82 Vc büyüklüğü Eq. 7.7-4'ün doğal sonucudur: (1 + 7,5 fFtuk/fctk) çarpanı küp kök içinde ≈ 1,8 kat artış verir. Kodlama hatası şüphesi yoktur. Sorun büyüklükte değil, **güvenlik formatındadır.**
- Q17 ana kuralında ΔVF, γc = 1,0 ve karakteristik fFtuk ile hesaplanıp ACI φ = 0,75 ile çarpılıyor. Etkin bölen 1/0,75 = 1,33'tür. MC2010 aynı terimi γc = 1,5 ile verir. Ana kural lif terimini MC2010 tasarım değerinden **%12,5 daha cömert** kullanır. Bu karma formatın (γc = 1 + φ) yayımlanmış bir dayanağı yoktur. MC2010-yerel ise her terime kendi yönetmeliğinin güvenlik formatını uygular ve hakem karşısında savunulması en kolay seçenektir.
- Ek bağlam (beyan edilmeli): ACI 318-14 çelik lifi kesme **dayanımı** olarak tanımaz. Lifi yalnız belirli koşullarda Av,min yerine kabul eder (26.4.2.2(d), ≥ 60 kg/m³). Ö2 dozları (20–40 kg/m³) bu eşiğin altındadır. Lifin kesme katkısı tümüyle MC2010'a dayanır, bu yüzden MC2010'un kendi γc'si kullanılmalıdır. fR'nin karakteristik olup olmadığı DOĞRULANAMADI (Q18, AÇIK-22). γc = 1,0 bu belirsizliği ikinci kez lif lehine çevirir.
- Lif sıfır çıkarsa da sıralama önemlidir. Cömert format altında bile lif seçilmiyorsa sonuç dayanıklıdır, ancak ana seri savunulabilir formatta olmalıdır. Lif seçilirse ana seri zaten savunulabilir formattadır ve üst sınır duyarlılığı "lifin en iyimser kazancını" gösterir.
- **Kontrol:** Av,min ve smax'ın lif varken de aynen korunduğu kodda teyit edilmeli. `--marginal` çıktısında etriye Δ = 0 olması bunu destekliyor. Bu doğru ve muhafazakâr seçim beyan edilmelidir, çünkü ACI 9.6.3.1'in lif istisnası ≥ 60 kg/m³ gerektirir.

## (5) Pilotta etriyeyi smax belirliyor; B2 dayanım kuralı sınanmıyor

**Karar: V2-a'ya ikinci bir doğrulama tasarımı eklensin.** Bu tasarım deterministik olarak seçilir, yeni bir optimizasyon kampanyası değildir. Bunun dışında bir değişiklik gerekmez.

**Gerekçe ve yöntem:**
- V2-a'nın amacı FEM ↔ ETABS uyumudur, optimallik değildir. Bu yüzden doğrulama tasarımı elle kurulabilir. Örneğin pilot vektörde kiriş yüksekliği havuzdan 1–2 basamak düşürülür (250×400–450) ve kolonlar uygun bırakılır. `--evalonly` ile "etriyeyi belirleyen = strength" ve Vs > 0,33√fc′ bw d (d/4 dalı) olan bir grup aranır. `--scan` çıktısındaki uygun komşular da aday olabilir. Kampanya tohumları kullanılmaz.
- Böylece B2'de Av/s dayanımla belirlenmiş olur ve ETABS'in Vc denklemi ile Vs istasyonu (AÇIK-18) gerçekten sınanır. smax ile belirlenen tasarımda iki programın aynı Av/s'yi vermesi yalnız aralık kuralının aynı olduğunu gösterir.
- Aynı mantıkla V3 (lifli Excel kontrolü) için ΔVF'nin etriyeyi gerçekten azalttığı (strength rejimi) bir **lifli** tasarım seçilmelidir. Lif katkısını sınamayan bir V3 bu kontrolün amacına hizmet etmez.
- Her iki tasarım da "doğrulama tasarımı, sonuç değil" etiketiyle `04_Results/ex2/v2a_pilot/` altında ayrı dosyalara yazılmalıdır.

## (6) Üç lifli koşunun en iyisinde lif = 0; 270 koşuya geçmek makul mü?

**Karar: Evet, 270 koşuya geçilsin. Önce 1, 3 ve 4 kodlanmalı, ikili dondurulmalı (SHA-256) ve aşağıdaki iki ön koşul sağlanmalıdır.**

**Gerekçe:**
- Lif = 0 sonucu **mekanik olarak beklenen** bir sonuçtur, arama başarısızlığına işaret etmez. Etriye smax/Av,min ile belirlendiğinden kesmedeki lif katkısı hiçbir etriyeyi azaltamaz (`--marginal`: etriye Δ = 0). Eğilmedeki kazanç küçüktür (boyuna donatıda −254…−493 TL) ve lif bedeli bunu çok aşar (+3 960…+7 920 TL). Fiyat 90 TL/kg iken lif = 0 tutarlı bir sonuçtur. Bu, ön kayıtlı sorunun (lifli ↔ lifsiz) meşru bir cevabıdır ve kampanyayı geçersiz kılmaz.
- Kampanya ucuzdur. 10 000 FE koşu 32–50 s sürer, 270 koşu tek iş parçacığında birkaç saat alır. Ön kaydı (§11, Q19) değiştirmek, koşuyu yapmaktan daha büyük bir metodolojik risk taşır.
- Fiyat = 0 senaryosu (90 koşu) tam olarak bu sorunun tanı aracıdır. Lif fiyatı sıfırken de lif çıkmıyorsa, lifin bu çerçevede yapısal kazanç sağlamadığı (smax rejimi) ayrıştırılmış olur. Lif çıkıyorsa basabaş fiyat hesaplanabilir. `--zerofibre` etiketleri (required/neutral/useful/harmful) burada esastır. Fiyat = 0'da "neutral" lif algoritma için kayıtsızdır ve rastgele görünebilir. Bu durum "lif seçildi" diye okunmamalıdır.

**Ön koşullar:**
1. Kampanya dışı tohumla **fiyat = 0, 1 000–2 000 FE, 2–3 koşu** sağlama sınaması yapılsın. Lif değişkenine ulaşılabildiği (doz kodlaması, 6 düzeyde mutasyon olasılığı) ve en az bir koşuda lif > 0 olan uygun çözümler üretildiği görülmelidir. Bu sağlanmazsa sorun kodlamadadır ve 270 koşu boşa gider.
2. Sonuç yorumu önceden yazılsın. "Bu geometri, yük, fiyat, fR verisi ve smax ile belirlenen etriye rejiminde lif seçilmedi" türünden, rejime bağlı bir ifade kullanılmalıdır. "Lif ekonomik değildir" genellemesi yapılmamalıdır (spec §13 madde 4 ile uyumlu). Makale için mekanizma tablosu (etriyeyi belirleyen kural × grup, ΔVF'nin etriyeye etkisi) en iyi çözümlerde zorunlu olsun.

---

**Özet tablo**

| # | Konu | Karar | Kod değişikliği (kampanya öncesi) |
|---|---|---|---|
| 1 | Q > 0,05 | (B) kat bazlı sınıflama + ötelenmeli katta k ≥ 1, δs = 1/(1−Q), Q ≤ 0,33, 6.2.6; (A) yalnız beyanlı alternatif | Evet |
| 2 | Kesme kritik kesiti | Düğümden d korunsun (güvenli yön); "yüz" ifadesi düzeltilsin; ETABS istasyonu V2-a'da | Hayır (beyan) |
| 3 | Kolon lu | Net yükseklik; ℓc = 3,3 m; ETABS'te ℓu oranı açıkça atanır | Evet |
| 4 | Lif kesme formatı | Ana = MC2010-yerel (γc = 1,5); Q17 γc = 1 üst sınır; Ö1 karşılaştırma | Evet (varsayılan bayrak; IA kararı) |
| 5 | smax belirleyici pilot | V2-a/V3 için strength rejiminde deterministik ikinci tasarım (lifsiz + lifli) | Hayır |
| 6 | Lif = 0, 270 koşu | Geçilsin; önce fiyat = 0 sağlama sınaması ve yorumun ön kaydı | Hayır |

**Hüküm: KÜÇÜK REVİZYON.** Madde 1, 3 ve 4 kampanya öncesi uygulanmalı ve hepsi IA onayına bağlıdır. 5 ve 6 doğrulama/protokol düzeyindedir.
