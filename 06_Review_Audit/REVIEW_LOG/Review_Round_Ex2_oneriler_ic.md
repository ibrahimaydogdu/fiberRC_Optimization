# Review Round Ex2 önerileri (iç Critic) — 2026-11-16

Kapsam: `03_Code_Execution/EX2_SPEC.md` v2, `EX2_OPEN_QUESTIONS.md`, `EX2_CODE_LOG.md`, önceki rapor `Review_Round_Ex2_spec_ic.md`; çapraz kontrol için salt-okunur `program_work/RcCore/SfrcBeam.vb` (`VRdF`, `FFtukShear`, `CanSubstituteRebar`) ve `Evidence/CMOD_test_verileri.xlsx` (yalnız sayfa yapısı). Hızlı inceleme (≈ 15 dk). Kod çalıştırılmadı, koşulara ve git'e dokunulmadı. ACI/fib madde metinleri bu turda yeniden okunmadı; hafızadan gelen her madde aşağıda "DOĞRULANMALI" diye işaretlendi. [Ç] = Critic'in kaba aritmetiği (makaleye girmez).

Değerlendirilen öneriler (IA onaylı): Q17–Q22 ve altı ek kabul (kesme kritik kesiti, ötelenmesiz kabul ve lu/r sınırı, analizde rijit bölge yok, Ö1 donmuş / format farkı beyanı, ACI 6.4.2 hareketli yük düzenlemesi yok, servis yalnız 9.3.1.1).

## Karar

**[KOŞULLU ONAY]** Önerilerin çoğu savunulabilir. Ancak üç madde makalenin lif iddiasını ve "ACI 318-14 ile tasarlandı" cümlesini doğrudan etkiliyor, bu yüzden kod/protokol dondurulmadan önce kapatılmalı:

- **(1) Q17:** kural kabul edilebilir, ama ana format MC2010'un kendi güvenlik düzeyine göre lif lehine (≈ %10–15 [Ç]). Bu yön beyan edilmeli ve "MC2010-yerel" bir duyarlılık varyantı eklenmeli.
- **(2) Ötelenmesiz kabul:** spec §5.1'deki "Q yanal yük yokken hesaplanamaz" gerekçesi teknik olarak yanlış. Q bir rijitlik oranıdır ve birim yanal yükle hesaplanabilir. Küçük kolonlarda Q > 0,05 çıkabilir [Ç]. Bu yüzden lu/r sınırı ötelenmesiz değere ancak Q kontrolüyle gevşetilmeli.
- **(3) Servis:** §3'teki "30 kN/m duvarı içerir" ifadesi, §5.1'deki "kirişler hasar görebilecek bölme taşımaz" varsayımıyla çelişiyor. Bu çelişki varken ACI 9.3.1.1 tek başına dayanak olamaz.

## Madde madde

### Q17 — Vc,f = Vc,ACI + max(0, VRd,F(γc=1,0) − VRd,F(doz 0, γc=1,0)); Ö1 formatı (γc=1,5 + φ) duyarlılık

**ŞARTLI.**

Artım kuralı doğru yönde. Çift sayımı önlüyor, doz 0'da lifsize sürekli bağlanıyor ve (c) seçeneğinin lifsiz MC2010–ACI farkını lif katkısı gibi gösterme hatasını kaldırıyor. `max(0, ·)` sarmalayıcısı tutarlı. MC2010'da etriyeli eleman için VRd = VRd,F + VRd,s toplamı da (madde numarası DOĞRULANMALI) Δ + Vs yapısıyla uyumlu.

**Yanlılık analizi:**

1. **Güvenlik düzeyi: lif LEHİNE.**
   - Lif artımı ana formatta karakteristik fR, γc = 1,0 ve φ = 0,75 ile hesaplanıyor; direnç tarafı katsayısı 1/0,75 = 1,33. MC2010 tasarım düzeyinde bu katsayı γc = 1,5.
   - Yük tarafı da daha düşük kalıyor. L/D ≈ 20/33 için ACI 1,2D+1,6L ortalama ≈ 1,35, MC2010 1,35G+1,5Q ≈ 1,41 [Ç].
   - Toplam güvenlik: lif terimi için ACI-karma ≈ 1,80, MC2010-yerel ≈ 2,11. Lif katkısı MC2010'un kendi formatına göre ≈ %15 az güvenlikle sayılıyor [Ç].
   - Spec §12 kapsam cümlesi ("favourable limiting case") bu yönü niteliksel olarak söylüyor, ama sayısal yön ve büyüklük de beyan edilmeli.
2. **Taban farkı: yön sabit değil.**
   - Δ, MC2010 tabanından mutlak artım olarak ekleniyor. MC2010 lifsiz tabanı > ACI Vc olduğunda lif artımı ACI'ye göre oransal olarak büyür (lif lehine). Bu durum küçük d ve yüksek ρl için geçerli: spec örneği d = 240 mm, ρl = %1 → +%15.
   - Büyük d ve düşük ρl'de tersi geçerli. Örnek: d = 540 mm, ρl = %0,5 → MC2010 lifsiz ≈ 0,72 MPa < ACI 0,93 MPa [Ç] (lif aleyhine).
   - Sonuç: yanlılık kesite bağlı. Oranlı alternatif Vc,ACI·VRd,F(f)/VRd,F(0) ters yönde aynı sorunu taşır, dolayısıyla Δ'yı değiştirmeye gerek yok. Ama en iyi çözümlerde VRd,F(0; γc=1)/Vc,ACI oranı raporlanmalı (yönün kanıtı).
3. **vmin tabanı: lif ALEYHİNE (küçük).** Doz 0 terimi Eq. 7.7-5 alt sınırına (`vmin`) takılırsa lif önce bu tabanı aşmak zorunda kalır ve Δ küçülür. Muhafazakâr. Kaç kirişte bağlayıcı olduğu raporlanmalı.
4. **ρl seçimi: lif LEHİNE olabilir.**
   - ΔVF ∝ (100ρl)^(1/3) olduğundan lif katkısı boyuna donatıyla büyür. ACI Vc'de ρl yok.
   - Kritik kesitte (mesnet) ρl = çekme donatısı = AsTop olmalı. Grup boyunca sürekli kabul ankraj koşulunu dolaylı sağlıyor, bu durum beyan edilmeli.
   - EC2'deki ρl ≤ 0,02 tavanının MC2010 Eq. 7.7-4'te olup olmadığı **DOĞRULANMALI**. Yoksa da muhafazakâr tavan olarak konması önerilir.
   - Optimizer, kesme için boyuna donatı artırma yolunu teorik olarak kullanabilir; donatı gereksinimden türetildiği için risk düşük.
5. **σcp:** Kirişte NEd iki terimde de 0 alınmalı. Gravite çerçevesinde kiriş eksenel kuvveti küçük ve işareti belirsiz; sıfır alma muhafazakâr ve basit. Kodda açıkça yazılmalı.
6. **Geçerlilik koşulu:** MC2010 5.6.3'teki fR3k/fR1k > 0,5 (ve fR1k/fLk > 0,4) koşulları (`CanSubstituteRebar`) her (tip, doz) için bir kez kontrol edilip tabloya yazılmalı. Sağlanmayan noktada Δ = 0 alınmalı.

**Duyarlılık varyantı (Ö1 formatı γc = 1,5 + φ):** Kalabilir, ama çift güvenlik içerdiği için lif aleyhine bir alt sınırdır. Tek başına ana formatın karşıtı olarak sunulursa aralık yanıltıcı olur. **Alternatif (öneri):** üçüncü deterministik varyant "MC2010-yerel": Vu ≤ φ(Vc,ACI + Vs) + ΔVF(γc = 1,5), yani lif terimine yalnız kendi kodunun katsayısı uygulanır. Böylece üç nokta sıralanır: ana (lehine) → MC2010-yerel → Ö1 (aleyhine). Ek koşu gerekmez, yalnız `--evalonly`.

### Q18 — fR karakteristik kabul + fR × 0,8 duyarlılık

**ŞARTLI.**

- `Evidence/CMOD_test_verileri.xlsx` numune düzeyinde veri içermiyor. Yalnız doz × beton sınıfı tabloları var (üretici tasarım tablosu biçiminde). Bu yüzden fRk istatistikle türetilemez ve "karakteristik mi" sorusu bu dosyadan çözülemez.
- × 0,8 tek başına yetersiz olabilir. EN 14651 fR saçılımı yüksektir (literatürde değişkenlik katsayısı sık %15–30; hafıza, kaynak DOĞRULANMALI). Veri ortalama ise fRk/fRm ≈ 1 − 1,645·CoV ≈ 0,5–0,75 olur.
- **Öneri:** × 0,8 zorunlu kalsın, ek bir **× 0,6** noktası eklensin. Karar ölçütü önceden yazılsın: "lif seçimi × 0,6'da da korunuyorsa sonuç fR belirsizliğine dayanıklıdır".
- fR çarpanı eğilme (S1) ve kesmeye **birlikte** uygulanmalı.
- `--evalonly` donatıyı yeniden türettiği için çözüm uygun kalabilir. Bu durumda maliyet artışı raporlanmalı; kesit yetersiz kalırsa uygunsuzluk raporlanmalı.
- Workbook'taki Venkateshwaran sayfaları, yayımlanmış veriyle tek bir kontrol noktası (dış O7) için ucuz bir yol sağlar. Önerilir (Q22 ile birlikte).

### Q19 — Fiyat = 0 için 90 koşu (toplam 270)

**KATILIYORUM, bir ekle.**

- Maliyet düşük: ≈ 4–6 ms/FE pilot ölçümü [CODE_LOG §4] → 10 000 FE ≈ 1 dk/koşu, 90 koşu ≈ 1–1,5 saat [Ç; tahmin, makaleye girmez].
- Aynı tohumlar (1001–1030) ve aynı protokol kullanılmalı.
- **Ek (zorunlu öneri):** 0 TL/kg'da lif maliyetsiz olduğu için optimizer, lifin işe yaramadığı kirişlerde bile dozu rastgele bırakabilir. "Lif seçildi" ile "lif nötr" ayrılamaz. Her en iyi çözümde lif grup grup sıfırlanıp deterministik olarak yeniden değerlendirilmeli; uygun ve maliyet eşitse "nötr" etiketlenmeli. Bu yapılmadan 0 TL/kg tip/doz dağılımı birincil çıktı (§11 madde 1-i) olarak yorumlanamaz.

### Q20 — V2-a uygun tasarımı: tohum aralığı dışı 1 000 FE pilot

**KATILIYORUM, koşullu.**

- Tasarım T = 0 olmalı.
- CD2 ≥ 0,95 kapısının anlamlı olması için en az bir kirişte kesme D/C, bir kirişte eğilme D/C ve bir kolonda P-M D/C ≥ ≈ 0,8 olmalı. Tasarım çok güvenli taraftaysa ETABS–FEM D/C farkı ölçülemez. Değilse ikinci pilot yapılmalı (başka tohum) veya en yüksek D/C'li uygun nokta seçilmeli.
- Tohum, FE, Ex2 ikili SHA'sı ve tasarım vektörü V2-a öncesi dondurulmalı; sonuç tablolarına girmemeli.

### Q21 — Kalem bazlı marjinal maliyet deterministik

**KATILIYORUM; yorum sınırı yazılmalı.**

- Kesit sabitken lif yalnız etriye ve (S1 ile) boyuna donatıyı etkileyebilir. Lifin asıl ekonomik kanalı olan kesit küçültme bu tabloda görünmez; o kanal 0 TL/kg optimizasyonunda görünür. Tablo başlığı "kesit sabitken" olmalı.
- **Ek:** her kiriş grubunda etriyeyi belirleyen ölçüt (dayanım / Av,min / smax = d/2) sütun olarak raporlanmalı. Av,min veya smax belirleyiciyse lifin etriye etkisi tanım gereği 0'dır, tablo yalnız lif maliyetini gösterir. Bunu okuyucu bilmeli.

### Q22 — Komşuluk taraması yok

**KATILMIYORUM (ucuz olduğu için yapılmalı).**

- 9 grupta b, h ± 1 adım ≈ 36 komşu, lifli senaryoda artı tip/doz komşuları. Yüzlerce değerlendirme × ≈ 5 ms → saniyeler [Ç].
- Senaryo başına genel en iyi çözümde 1-adım yerel tarama yapılmalı. Sonuç yalnız "1-adım yerel en iyi" ya da "değil" olarak raporlanmalı ("optimum" iddiası yok, §12 madde 9 korunur).
- Q22 "yüksek maliyet" gerekçesi pilot süre ölçümüyle geçersiz kalıyor.

### Kesme kritik kesiti: yüzden d uzakta

**ŞARTLI.**

- ACI 9.4.3.2 izni (mesnet tepkisi uç bölgede basınç oluşturuyor, yük üstten uygulanıyor, yüz ile kritik kesit arasında tekil yük yok) gravite kirişi–kolon birleşiminde genel olarak sağlanır. Madde metni ve koşulları `CODE_PROVISIONS_EVIDENCE.md`'ye birebir alıntıyla **DOĞRULANMALI** (bu turda okunmadı).
- Uygulama koşulları:
  - (a) Rijit bölge olmadığından yüz = düğüm + c/2 (c = kolonun düzlem içi h'si; kolon grubuna göre değişir). Vu = V_düğüm − wu(c/2 + d) analitik hesaplanmalı; ≤ 0,5 m istasyon enterpolasyonu kullanılmamalı.
  - (b) Aynı kesit hem optimizasyonda, hem V2-a B2 (ETABS Av/s karşılaştırması aynı x'te okunur) hem de lifli V3 Excel'de kullanılmalı (AÇIK-4 böylece kapanır).
  - (c) Av,min eşiği (Vu > 0,5φVc) aynı Vu ile kontrol edilmeli.
- Eğilmede düğüm momenti, kesmede yüz + d kullanılması karma bir kabul ama muhafazakâr yönde. Varsayım tablosuna girmeli.
- Lif yönünden nötr: lifli ve lifsiz aynı.

### Ötelenmesiz kabul; lu/r kontrolü ötelenmesiz sınıra göre

**ŞARTLI (mevcut haliyle KATILMIYORUM).**

- **Gerekçe hatası:** ACI 6.6.4.3(b)'deki Q = ΣPu·Δo/(Vus·lc) bir kat rijitliği oranıdır. Δo/Vus herhangi bir (birim) yanal yükle elde edilir, dolayısıyla "yanal yük yokken hesaplanamaz" ifadesi yanlış ve spec §5.1'den çıkarılmalı.
- Kaba örnek [Ç]: zemin katta 4 kolon 300×300, Ec ≈ 25,7 GPa, kiriş esnekliğiyle kat rijitliği ≈ 11–12 kN/mm, ΣPu ≈ 3 500 kN, lc = 3,3 m → Q ≈ 0,09 > 0,05. 400×400 kolonlarla Q ≈ 0,03. Optimizer küçük kolonlara gittiğinden sınıflama tasarıma bağlı.
- Simetrik gravite yükünde birinci mertebe yanal ötelenme ≈ 0. Bu yüzden ötelenmeli sınıflama talebi pratikte değiştirmez (δs ile büyütülecek M2s ≈ 0); 6.6.4.3(a) "%5 moment artışı" yolu da savunulabilir. Ama perdesiz bir moment çerçevesine "nonsway" demek hakem tarafından sorgulanır.
- lu/r sınırı ayrı bir konu. 22 → 34 + 12M1/M2 (≤ 40) geçişi lu/r 22–34 arasındaki kolonlarda δns büyütmesini tamamen kaldırır ve **daha az muhafazakâr** olur. Örnek: 300'lük kolonda lu/r ≈ 2 800/90 ≈ 31 [Ç], yani tam bu bölgede.
- **Öneri (biri seçilmeli):**
  - (i) Kod 22'de kalsın (mevcut, muhafazakâr, ötelenmeli sınıflamayla da uyumlu). ETABS V2-a CD3 farkı beyan edilsin veya ETABS'te aynı davranış ayarlansın.
  - (ii) Ötelenmesiz sınıra geçilecekse, her en iyi çözümde (tercihen her FE'de kısıt olarak; ek bir yük durumu, faktörleştirme yeniden kullanılırsa maliyeti düşük, DOĞRULANMALI) kat başına Q hesaplanıp Q ≤ 0,05 gösterilmeli. Sınır ifadesinde M1/M2 kullanılıyorsa Cm = 1 bırakılması tutarsız ama muhafazakâr; beyan edilmeli.
- 34 + 12M1/M2 madde metni de okunmadı (CODE_LOG AÇIK-2); **DOĞRULANMALI**.

### Analiz modelinde rijit bölge yok (ETABS da aynı)

**KATILIYORUM.**

- FEM ↔ ETABS eşleşmesi (FE4 ≤ %0,1) için doğru seçim. Düğüm momentleri mesnet yüzü momentlerinden büyük olduğundan eğilmede muhafazakâr.
- Beyan edilmeli. ETABS modelinde "End Length Offsets = 0" ve "rigid zone factor = 0" ayarları V1'de ekran/tablo kaydıyla belgelenmeli.
- Kesme için yüz konumu yukarıdaki maddeyle tanımlanmalı.

### Ö1 donmuş; Ö1–Ö2 format tutarsızlığı makalede beyan

**ŞARTLI.** Makaleye etkisi küçük değil.

- Ö1 lif kesme katkısını çift güvenlikle (γc = 1,5 + φ) saydı ve tüm-boyut lifli 180 koşuda lif 173 koşuda 0 çıktı. Ö2 daha cömert formatla lif seçerse, okuyucu "lif Ö1'de seçilmedi, Ö2'de seçildi" farkını rejim (sismik ↔ gravite) ile format arasında ayıramaz. §12 madde 8 nedensel cümleyi yasaklıyor, ama yan yana iki tablo zaten bu çıkarımı davet eder.
- Ö1'deki formatın lif aleyhine olduğu da beyan edilmek zorunda. Bu, Ö1'in "lif seçilmedi" sonucunun bir kısmının formattan gelebileceğini kabul etmek demektir.
- **Alternatif (beyandan güçlü, Ö1'i değiştirmeden):**
  - Ö1 en iyi çözümlerinde kiriş etriyesini sarılma bölgesi dışında belirleyen ölçüt Ö1 günlüklerinden okunmalı. 2h sarılma bölgesinde lif katkısı zaten 0 kabul edildi (MEMORY §3f). Sarılma dışı bölgede etriye Av,min/smax belirleyiciyse, format farkı Ö1 sonucunu değiştirmez ve bu bir cümleyle **kanıtlı** yazılabilir.
  - Belirleyici değilse, Ö1 en iyi lifli/lifsiz çözümleri Ö2 formatıyla Excel'de deterministik yeniden değerlendirilmeli (kod değişikliği yok).
  - Bu kontrol yapılmadan Ö1 ve Ö2 lif sonuçları aynı bölümde sunulmamalı.

### ACI 6.4.2 hareketli yük düzenlemesi uygulanmaz

**ŞARTLI** (IA kararı [K b] var; yeniden açmıyorum, ama asgari bir kontrol öneriyorum).

- L/D ≈ 20/33 ≈ 0,6 ile iç açıklık pozitif momenti ve mesnet kesmesi düzenlemeyle belirgin artabilir (ölçülmedi).
- 6.4.2 metni "permitted to assume" diyor; düzenlemenin zorunluluğu 6.4.1–6.4.3 bütünü ve yorum metninden okunmalı (**DOĞRULANMALI**). Zorunluysa "ACI 318-14 ile tasarlandı" cümlesi kurulamaz, yalnız "ACI 318-14 kesit hükümleri, tek yükleme deseni" denebilir.
- **Asgari öneri:** her senaryonun en iyi çözümleri 6.4.2 (a)(b) desenleriyle deterministik kontrol edilsin, aşan D/C'ler raporlansın. Optimizasyona girmez; yaklaşık 4–6 ek yük durumu, saniyeler sürer.
- Lif yönü belirsiz: kesme artışı lifin işini artırır, açıklık momenti eğilme donatısını artırır.

### Servis: yalnız ACI 9.3.1.1 yükseklik sınırı

**ŞARTLI — iç çelişki var.**

- §3 "30 kN/m döşeme + kaplama + duvarı içerir" diyor. §5.1 ise 9.3.1.1'in uygulanabilirliği için "kirişler hasar görebilecek bölme taşımaz" varsayıyor. İkisi birlikte doğru olamaz.
- **Seçenekler:**
  - (i) §3'teki "duvar" ifadesi kaldırılıp yük kaynakta tanımlandığı gibi ("ölü yük") bırakılsın ve bölme yok varsayımı beyan edilsin. En az iş, ama hakem için zayıf.
  - (ii) En iyi çözümlerde ACI 24.2 hesaplı sehim (Ie, uzun süreli çarpan, ℓ/480 veya ℓ/240 sınırı; madde metni **DOĞRULANMALI**) deterministik kontrol olarak yapılsın. Lifin sehime etkisi modellenmediği için lifsiz rijitlik kullanılsın (lif aleyhine, muhafazakâr).
- Havuz alt sınırı 300 mm ile 9.3.1.1 zaten pasif. Bu yüzden "servis kısıtı" ifadesi optimizasyonda hiçbir etki göstermez ve makalede kısıt olarak sunulmamalı.

## Özet tablo

| Öneri | Karar | Kod/protokol dondurma öncesi şart |
|---|---|---|
| Q17 Δ kuralı | ŞARTLI | ρl = AsTop, σcp = 0, MC2010 geçerlilik kontrolü; "MC2010-yerel" varyant; lehine yön ve ≈ %15 beyanı; VRd,F(0)/Vc,ACI oranı raporu |
| Q18 fR × 0,8 | ŞARTLI | × 0,6 noktası; eğilme + kesme birlikte; önceden yazılı dayanıklılık ölçütü |
| Q19 90 koşu | KATILIYORUM | 0 TL/kg'da "nötr lif" sıfırlama testi |
| Q20 pilot | KATILIYORUM | T = 0 ve kritik D/C ≥ ≈ 0,8; vektör dondurulur |
| Q21 deterministik | KATILIYORUM | Etriye belirleyici ölçüt sütunu; "kesit sabitken" başlığı |
| Q22 tarama yok | KATILMIYORUM | 1-adım yerel tarama (saniyeler) |
| Yüz + d kesme | ŞARTLI | ACI 9.4.3.2 doğrulaması; analitik Vu; FEM/ETABS/Excel aynı kesit |
| Ötelenmesiz + lu/r | ŞARTLI | Gerekçe düzeltilir; ya 22 korunur ya Q ≤ 0,05 gösterilir |
| Rijit bölge yok | KATILIYORUM | ETABS ayarı V1'de belgelenir |
| Ö1–Ö2 format | ŞARTLI | Ö1 etriye belirleyici ölçüt kontrolü veya Excel yeniden değerlendirme |
| 6.4.2 yok | ŞARTLI | En iyi çözümlerde desen kontrolü; zorunluluk metni doğrulanır |
| Servis 9.3.1.1 | ŞARTLI | "Duvar" çelişkisi giderilir; tercihen hesaplı sehim kontrolü |

## AUDIT_CHECKLIST

| Madde | Durum |
|---|---|
| Sayı–kaynak eşleşmesi | KISMEN ([Ç] değerleri kaba; makaleye girmez) |
| ACI 9.4.3.2, 6.6.4.3, 6.2.5, 6.4.1–6.4.3, 24.2; MC2010 Eq. 7.7-4 ρl tavanı | HAYIR (bu turda okunmadı; kanıt dosyasına alıntıyla girmeli) |
| fR karakteristik durumu | DOĞRULANAMADI (workbook numune verisi içermiyor) |
| İddia sınırları | KISMEN (lif lehine format yönü ve Ö1 aleyhine format yönü beyan edilmeli) |
| İç tutarlılık | HAYIR (§3 "duvar" ↔ §5.1 bölme varsayımı; §5.1 Q gerekçesi) |
