# Dış hakem raporu (2. tur): EX2_SPEC.md v2 önerileri (Q17–Q22 ve yöntem kabulleri)

Tarih: 2026-11-16 · Hakem: bağımsız dış hakem (*Structures*; RC çerçeve optimizasyonu, SFRC) · Okunan: `03_Code_Execution/EX2_SPEC.md` (v2), `03_Code_Execution/EX2_OPEN_QUESTIONS.md`, hakemin 1. tur raporu `Review_Round_Ex2_spec_dis.md`. İç Critic raporu ve iç yazışmalar okunmadı. Kod incelenmedi. Hakem hesapları [H] ile işaretlidir; bunlar kanıt değil, kontrol önerisidir.

## KARAR: KÜÇÜK REVİZYON

v2, 1. turdaki kritik bulguların üçünü de belge düzeyinde kapatıyor. Kodlama başlamadan önce kapatılması gereken üç şart var: (Ş1) Q17 kuralının eksik tanımları (ACI Vc denklemi, ρl, üst sınır, Ö1 varyantının tanımı); (Ş2) ACI 9.3.1.1'in bu örnekte uygulanabilirliği ile "30 kN/m duvarı içerir" varsayımı arasındaki çelişki; (Ş3) hareketli yük düzenlemesinin etkisinin en azından deterministik olarak ölçülmesi. Hiçbiri yeni optimizasyon koşusu gerektirmiyor.

---

## 1. 1. tur bulgularının durumu

| Bulgu | Durum | Not |
|---|---|---|
| K1 lif ↔ Av,min | **GİDERİLDİ** | ACI 26.4.2.2(d) 60 kg/m³ birebir alıntıyla doğrulanmış; Av,min lifli/lifsiz kirişte aynen; sapma beyanı yazılmış (§5.2) |
| K2 zorlanmış lif + 0 TL/kg | **GİDERİLDİ (Q19, Q21'e bağlı)** | Marjinal maliyet tablosu birincil çıktı; kiriş ve toplam maliyet ayrı |
| K3 V2-a uygun tasarım | **GİDERİLDİ (Q20'ye bağlı)** | Kuvvet kaynak kesitleriyle, D/C uygun tasarımla |
| B1 servis | **KISMEN** | 9.3.1.1 eklendi ama havuzda pasif; uygulanabilirlik koşulu §3 yük varsayımıyla çelişiyor (bkz. §3 madde 6) |
| B2 format, çift sayım, fR | **BÜYÜK ÖLÇÜDE GİDERİLDİ** | Δ kuralı doğru yön; eksik tanımlar Q17'de; fR çarpanı Q18'de |
| B3 Ö1 karşılaştırılabilirlik | **GİDERİLDİ** | Nedensel karşılaştırma yasak listesinde; "elverişli sınır durum" çerçevesi |
| B4 V2-d / PCR-19 | **GİDERİLDİ** | V2-d çıkarıldı, PCR-19 Excel'e taşındı |
| B5 kaynak seçimi | **KISMEN** | Dil düzeltildi; alternatif hakemli kaynak araması açık hakem noktası olarak duruyor (kabul edilebilir) |
| B6 V1 öz ağırlık | **GİDERİLDİ** | V1(c) sayıları hakem tarafından yeniden hesaplandı [H]: ΣV = 3,267 + 4,800 = 8,067 m³; G = 1 641,68 kN; 1,2D+1,6L = 3 506,01 kN; 1,4D = 2 298,35 kN. Doğru |

---

## 2. Açık sorular (Q17–Q22)

### Q17: Vc,f = Vc,ACI + max(0, VRd,F(γc = 1,0) − VRd,F(doz 0, γc = 1,0)); Ö1 formatı (γc = 1,5 + φ) duyarlılık — **ŞARTLI**

Gerekçe (katılım): Artım kuralı çift sayımı önler ve doz 0'da lifsiz tasarımla süreklidir. MC2010 Eq. 7.7-5 (spec'te 7.7-4 diye anılıyor; numara PDF'ten teyit edilmeli) katsayısı 0,18/γc, γc = 1,0'da karakteristik düzeyde bir dayanım verir; ACI nominal Vn ile aynı düzeyde olduğundan tek güvenlik katsayısının φ olması tutarlıdır. vmin'in iki terimde de uygulanması muhafazakârdır (vmin lif içermez; baz terimde vmin belirleyiciyse ΔVF küçülür). Hakem kontrolü [H]: spec'teki örnek (k = 1,913; 1,07 vs 0,93 MPa) doğru; (c) seçeneğinin lif lehine yanlılığı gerçektir.

Şartlar (kodlamadan önce yazılmalı):
1. **ACI Vc denklemi tek olarak seçilmeli:** 22.5.5.1 basit (0,17λ√fc' bw d) mı, Tablo 22.5.5.1 ayrıntılı (ρw ve Vu d/Mu terimli) mı. ETABS'in kullandığı ile aynı olmalı (B2 kapısı buna bağlı).
2. **ρl tanımı:** kritik kesitte çekme donatısı (mesnette üst donatı, AsTop/bw d), MC2010'un istediği gibi en az (ℓd + d) kadar uzatıldığı varsayımı beyan edilmeli. Grup boyunca sürekli donatı kabulü (§5.4) bunu dolaylı sağlar; yazılsın.
3. **Üst sınır:** ΔVF için açık sınır yok. Öneri: Vc,f ≤ 0,42√fc' bw d [VARSAYIM; ACI'de lif için doğrudan karşılığı yok] veya en azından Vc,f/Vc,ACI oranı en iyi çözümlerde raporlanmalı. Oran 2'yi geçiyorsa hakem bunu sorar.
4. **Ö1 varyantının tanımı:** "γc = 1,5 + φ" Ö1'de Δ biçiminde mi, yoksa VRd,F'nin Vc yerine doğrudan konması biçiminde mi uygulanıyor? Doğrudan ise varyant iki şeyi birden değiştirir (format + toplama kuralı). Varyant **Ö1 kodunun birebir formülü** olarak tanımlanmalı ve bu fark tabloda yazılmalı; aksi hâlde "format etkisi" diye okunamaz.
5. Av,min eşiği (Vu > 0,5φVc) lifsiz Vc,ACI ile — spec böyle; doğru, korunmalı.

Alternatif: (b) max kuralı yalnız ek duyarlılık satırı olarak kalabilir; ana seri (a).

### Q18: fR karakteristik kabul + fR × 0,8 duyarlılık — **ŞARTLI**

Gerekçe: Belirsizliğin beyanı ve zorunlu duyarlılık doğru yaklaşım. Ancak 0,8 çarpanı ortalamadan karakteristiğe geçişi temsil etmekte zayıf: EN 14651 kirişlerinde fR3 değişkenlik katsayısı literatürde tipik olarak %15–30 aralığında bildirilir [H; kaynakla doğrulanmalı]; fRk ≈ fRm(1 − 1,64·CoV) ile CoV = %20'de çarpan ≈ 0,67, %25'te ≈ 0,59 olur. 0,8 yalnız CoV ≈ %12'ye karşılık gelir.

Şartlar ve alternatif:
1. Önce `Evidence/CMOD_test_verileri.xlsx` içinde **numune düzeyinde** sonuç olup olmadığı kontrol edilsin; varsa fR1k, fR3k MC2010 §5.6 / EN 14651 istatistiğiyle doğrudan hesaplanır ve Q18 kapanır.
2. Yoksa duyarlılık çarpanı **0,8 ve 0,6** (iki nokta) olsun; 0,6 hem ortalama veri olasılığını hem gizli kaynak riskini kapsar. ×1,2 bilgi amaçlı kalabilir.
3. Tabloda, uygunluğu bozulan (D/C > 1) en iyi çözüm sayısı ve etriye artışı ile yeniden uygun hâle getirme maliyeti (etriye yeniden türetilerek) ayrı sütunlarda verilmeli.

### Q19: fiyat = 0 için 3 × 30 = 90 koşu (toplam 270) — **KATILIYORUM**

Gerekçe: 0 TL/kg senaryosu lifin teknik yararını fiyattan ayıran tek optimizasyon ölçüsüdür; deterministik marjinal tablo (Q21) kesit sabitken etkiyi verir, 90 koşu ise lifin kesit küçültmeye izin verip vermediğini gösterir. İki çıktı birbirini tamamlar. Aynı tohumlar (1001–1030) ve aynı protokol yeterli.
Not: ön kayıtta 0 TL/kg sonucunun birincil ölçüsü "en iyi çözümlerde lif seçilme oranı ve kiriş maliyeti (lif hariç) farkı" olarak yazılsın; 0 TL/kg'de bile lif seçilmiyorsa yorumu ("bu formülasyonda lifin kaldıracı yok") önceden yazılsın. CPU yetersiz kalırsa koşu sayısı 30'un altına indirilmesin (kural 8); gerekirse 0 TL/kg senaryosu sonraya bırakılsın, kısaltılmasın.

### Q20: V2-a uygun tasarımı 1 000 FE pilot koşudan (tohum 1000, ATO, lifsiz) — **ŞARTLI**

Gerekçe: Koşu tohumlarından bağımsız tek pilot makul ve süre ölçümünü de verir. Ancak:
1. Rastgele başlangıçlı 1 000 FE'de T = 0 (tam uygun) çözüm garanti değil. Kabul ölçütü yazılsın: T = 0; yoksa pilot 2 000 FE'ye uzatılır veya elle (mühendislik) uygun tasarım kurulur.
2. V2-a D/C kapısının anlamlı olması için tasarımda **aktif kısıtlar** bulunmalı: en az birkaç kirişte eğilme D/C ≥ 0,9, en az bir kirişte Vs > 0 (Av,min üstü), en az bir kolonda P-M D/C ≥ 0,7. Pilot çözüm bunu sağlamıyorsa (ör. her şey Av,min ve ρmin'de) CD2/B2 testi boş geçer. Ölçüt ön kayda girsin.
3. Pilot çözüm ana koşulara başlangıç olarak verilmez; SHA ve tohum günlüğe yazılır, sonuç makalede raporlanmaz (spec böyle; korunmalı).

### Q21: kalem bazlı marjinal maliyet deterministik (kesit sabit, donatı/etriye yeniden türetilir) — **KATILIYORUM (ek sütunla)**

Gerekçe: Hesap yükü küçük, yorumu açık ("kesit sabitken lifin etkisi" = alt sınır). Kesit küçültme etkisi Q19 koşularıyla ayrıca ölçülüyor; yeniden optimizasyon gerekmez.
Ek: tabloya her kiriş grubunda lifsiz çözümün kritik kesitte **Av,min'de olup olmadığı** sütunu eklenmeli. Etriye zaten Av,min'de ise lifin kesme yararı tanım gereği 0'dır ve bu, sonucun nedenini tek bakışta gösterir. Lifin eğilmeye (S1) katkısının boyuna donatıyı değiştirip değiştirmediği de ayrı satırda verilmeli.

### Q22: komşuluk taraması yapılmaz — **KATILMIYORUM**

Gerekçe: Bu, 1. turda maliyeti en düşük, getirisi en yüksek öneriydi. Koordinat bazlı tarama (9 grup × 35 kesit = 315 değerlendirme; lifli çözümde kiriş grupları için ayrıca 3 × 9 malzeme seçeneği = 27) tek çözüm başına ≈ 350 değerlendirmedir; 10 000 FE'lik bir koşunun %3,5'i. "Optimum" iddiası yasakken yerel en iyilik kanıtı makalenin en güçlü savunmasıdır ve Ö1'deki "üç algoritma farklı yerlerde duruyor" eleştirisini Ö2'de önler.
Alternatif (asgari): yalnız lifsiz ve lifli (90 TL/kg) **genel en iyi** iki çözüm için tek tur koordinat taraması; daha ucuz uygun komşu bulunursa raporlanır (ve "en ucuz bulunan çözüm" güncellenir, post-hoc etiketiyle). Üç yöntemin en iyi çözümü birebir aynıysa tarama yine de yapılmalı; aynılık yerel en iyilik kanıtı değildir.

---

## 3. Yöntem kabulleri

1. **Kesme kontrolü mesnet yüzünden d uzakta — ŞARTLI.** ACI 9.4.3.2 koşulları (mesnet reaksiyonu uç bölgede basınç oluşturur, yüz ile kritik kesit arasında tekil yük yok) kolona çerçevelenen ve yayılı yüklü kirişlerde sağlanır; kabul edilebilir, madde atfıyla yazılsın. Şart: rijit bölge olmadığından analiz eksen modelidir; "yüz" konumu geometrik olarak (eksen − h_kolon/2) tanımlanmalı ve **ETABS'in kesme tasarım istasyonu ile aynı kural** olduğu V2-a B2 kapısında gösterilmeli. ETABS'te otomatik uç uzunluk ofsetleri (rijitlik çarpanı 0 olsa bile) tasarımı yüzde yapabilir; FEM aynı istasyonu kullanmazsa B2 (0,98–1,02) gereksiz yere başarısız olur.
2. **Ötelenmesiz çerçeve kabulü — ŞARTLI.** Simetrik geometri (5-6-5 m) ve simetrik düşey yük altında ikinci mertebe uç momenti artışı ihmal edilebilir; ACI 6.6.4.3(a) (%5 artış ölçütü) ile gerekçelendirilmeli (Q indeksi yanal yük yokken tanımsız; spec bunu doğru söylüyor). Ayrıca "çerçeve, yanal yükleri başka elemanların taşıdığı bir binanın gravite çerçevesidir" varsayımı yazılmalı; aksi hâlde düşey yüklü ve perdesiz bir çerçeve için ötelenmesiz kabul okuyucuya keyfî görünür. Düzlem dışı: b = 250 mm, lu ≈ 2,8 m'de klu/r ≈ 37 [H] — ACI 6.2.5 sınırına (34 + 12M1/M2 ≤ 40) yakın; düzlem dışı narinliğin e_min momenti üzerinden δns ile gerçekten hesaplandığı V2-a'da gösterilmeli (Q15).
3. **Rijit bölge yok — KATILIYORUM.** Eksen momentleri yüz momentlerinden büyük olduğundan eğilme muhafazakârdır; maliyet hafif yukarı yanlıdır. Şart: eğilme tasarım kesitinin de (eksen mi yüz mü) madde 1 ile tutarlı tanımlanması ve ETABS ile eşlenmesi. Beton hacminde düğüm örtüşmesinin iki elemanda sayılması (§10 V1) beyan edilmiş; yeterli.
4. **Ö1 donmuş; Ö1–Ö2 format tutarsızlığı makalede beyan — KATILIYORUM.** Şart: Ö1 formatlı duyarlılık satırı (Q17 madde 4) makalede ana metinde (ek materyalde değil) bir cümle ve tek sayıyla verilmeli; çünkü Ö2'nin "lif için elverişli" olmasının bir kısmı rejimden değil formattan (γc = 1,0) gelir ve okuyucu bunu bilmelidir.
5. **Hareketli yük düzenlemesi uygulanmaz (sınırlama) — KATILMIYORUM (asgari alternatifle şartlı kabul).** L/D = 20/30 ≈ 0,67 yüksek bir orandır; iç açıklık pozitif momenti ve kenar açıklık iç mesnet kesmesi düzenlemeyle belirgin artar [H; yaklaşık %10–30, ölçülmeli]. Gerekçe olarak verilen FE maliyeti zayıftır: doğrusal 2B modelde rijitlik matrisi bir kez çarpanlarına ayrılır, açıklık başına birim L durumları ek sağ taraftır ve süperpozisyonla zarf alınır; değerlendirme süresine etkisi ihmal edilebilir. Hakem *Structures*'ta bunu "yalnız düşey yüklü örnekte yük düzenlemesi neden yok" diye sorar; "kaynakla uyum" gerekçesi kaynak kombinasyonları bile uygulamadığı için tutmaz.
   - Tercih: ACI 6.4.2 (a)–(b) desenleri (alternatif açıklıklar + bitişik açıklıklar; her kat için 6.4.1.1 izniyle yalnız o kat) süperpozisyonla zarfa eklenir; ETABS'te karşılığı desenli hareketli yük durumları.
   - IA kararı [K b] korunursa asgari şart: en iyi çözümler (her senaryo × yöntem) `--evalonly` ile desenli zarf altında deterministik yeniden değerlendirilir; D/C > 1 olan eleman sayısı ve uygunluğu geri getirmenin maliyeti raporlanır. Bu yapılmadan sınırlama cümlesi "etki ölçülmedi" olarak kalır ve kabul edilemez.
6. **Servis yalnız ACI 9.3.1.1 yükseklik sınırı — ŞARTLI (iç çelişki var).** Spec §5.1, 9.3.1.1'i "kirişler hasar görebilecek bölmeleri taşımaz" varsayımıyla uyguluyor; spec §3 ise 30 kN/m'nin "döşeme + kaplama + **duvar**ı içerdiğini" söylüyor. Duvar taşıyan kirişte 9.3.1.1 uygulanamaz; ACI 24.2 hesaplı sehim ve Tablo 24.2.2'deki ℓ/480 (bölme taşıyan) sınırı gerekir. Ayrıca havuz alt sınırı 300 mm ile kısıt pasif olduğundan fiilen servis kısıtı yoktur.
   - Seçenek A (tercih): en iyi çözümler için ACI 24.2.3.5 Ie ve 24.2.4 uzun süreli çarpanla deterministik sehim kontrolü (ℓ/480 ve ℓ/240), sonuç tablosu; optimizasyona eklenmesi gerekmez.
   - Seçenek B: yük varsayımı yeniden yazılır ("30 kN/m döşeme ve kaplamadır; bölmeler büyük sehimden hasar görmeyecek türdendir") ve 9.3.1.1 bu gerekçeyle uygulanır. Bu durumda "duvar" ifadesi §3'ten çıkarılmalı.
   Hangisi seçilirse seçilsin, kaynak 30 kN/m'nin içeriğini söylemediği için bu bir varsayımdır ve makalede tek cümleyle yazılmalı.

---

## 4. Koşu öncesi kontrol listesi (2. tur)
- [ ] Ş1 (Q17): ACI Vc denklemi, ρl tanımı, Vc,f üst sınırı veya oran raporu, Ö1 varyantının birebir tanımı, MC2010 denklem numarası teyidi.
- [ ] Q18: CMOD verisinde numune düzeyi kontrolü; yoksa ×0,8 ve ×0,6.
- [ ] Q20: pilotta T = 0 ve aktif kısıt ölçütü ön kayıtta.
- [ ] Q21: "Av,min'de mi" sütunu.
- [ ] Q22: en az iki genel en iyi çözüm için koordinat taraması.
- [ ] Ş2: 9.3.1.1 ↔ duvar çelişkisi (Seçenek A veya B).
- [ ] Ş3: hareketli yük düzenlemesi zarfta veya en iyi çözümlerde deterministik ölçüm.
- [ ] Kesme/eğilme tasarım istasyonu (yüz/eksen) FEM ↔ ETABS aynı; V2-a'da gösterilir.
- [ ] Ötelenmesiz kabul ACI 6.6.4.3(a) ve "gravite çerçevesi" varsayımıyla yazılı.
