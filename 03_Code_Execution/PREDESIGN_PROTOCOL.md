# Ön tasarım (FSD + FUD) ve havuz daraltma protokolü: TASLAK (2026-11-01)

**Durum:** DONDURULDU 2026-11-02 (IA onayı: kapsama eşiği ≥ 9/10, marj −2/+3, hedef 'ölçülen daralma ve yakınsama hızı', PCR-6/7). Program henüz değişmedi (AGENTS kural 7, 10); bundan sonra bu protokol yalnız PCR ile değişir.
Kaynak: IA önerisi (FSD+FUD, %50–80 daraltma), `06_Review_Audit/REVIEW_LOG/Review_Round_FSD_FUD.md` (Critic: REVİZYON TALEBİ, koşullu kabul).

## 1. Ortak kurallar (lifli ve lifsiz senaryo için aynı)
1. Ön tasarım **tek, deterministik, senaryodan bağımsız**; lifsiz kapasite modeliyle yapılır.
2. Lifli havuz = lifsiz havuz × {lif tipi, doz} (doz = 0 dahil). Lif tipi/dozu daraltılmaz.
3. Aynı kutu, aynı başlangıç noktası, aynı MaxFEs (10 000), aynı ceza.
4. Daraltma, kampanya optimumundan (Baseline-A) **türetilmez**; yalnız FSD+FUD sonucundan.
5. Ön tasarımın FE/analiz sayısı ayrıca raporlanır; süre kazancı FE azaltmaktan gelir, FE başına süre değişmez.

## 2. Ön tasarım adımları (PCR-6 ile kodlanır)
1. FSD, grup düzeyinde: kiriş → kolon döngüsü (kolon kesmesi ve ΣMnc ≥ 1,2 ΣMnb kiriş donatısına bağlı), en çok 5 tur.
2. FUD: ötelenme ve Q ≤ 0,05 için sanal iş duyarlılığıyla rijitlik ölçekleme.
3. Çıktı: başlangıç x ve değişken başına kutu.

## 3. Kutu kuralı (Critic §1 2d)
alt = FSD/FUD minimum − 2 adım; üst = FSD/FUD minimum + 3 adım (indeks, havuz sınırlarına kırpılır).

## 4. Kapsama testi
Kutu üretildikten sonra Baseline-A'nın (mevcut 360 koşu) en iyi uygun çözümlerinin kutuya düşme oranı raporlanır (`03_Code_Execution/tools/box_coverage.py`). Eşik: **IA onayıyla belirlenir** (öneri: en iyi 10 uygun koşunun en az 9'u kutuda).

## 5. Baseline-A üzerinde ön ölçüm (kod çalıştırmadan, yalnız CSV)
Çıktı: `04_Results/box_coverage_baselineA.txt`. Yöntem: `fR × 1,00` koşuları, T = 0 uygun koşular, maliyete göre ilk k çözümün değişken başına [min, max] kutusu; en iyi 10 çözüm kendisi hariç tutularak (leave-one-out) kutuda mı sınanır. Bu, ön tasarımın kutusu **değildir**; yalnız ön bilgi verir.

| Senaryo | Kutu kaynağı | Marj | Daralma (ortalama) | Kapsama (en iyi 10, LOO) |
|---|---|---|---|---|
| lifsiz | ilk 10 | 0 | %46 | 3/10 |
| lifsiz | ilk 10 | −2/+3 | %15 | 10/10 |
| lifsiz | ilk 30 | 0 | %36 | 8/10 |
| lifsiz | ilk 30 | −2/+3 | %10 | 10/10 |
| lifli | ilk 10 | 0 | %46 | 3/10 |
| lifli | ilk 10 | −2/+3 | %16 | 9/10 |
| lifli | ilk 30 | 0 | %35 | 6/10 |
| lifli | ilk 30 | −2/+3 | %10 | 10/10 |

**Yorum (kanıta bağlı kısım):**
- Marjsız kutu optimumu sık kaçırıyor (3/10 ile 8/10). Marj olmadan daraltma güvenilir değil.
- Kritik: −2/+3 marjıyla, Baseline-A'dan türetilmiş kutuların daralması yalnız %10–16. IA'nın %50–80 hedefi bu veriden **desteklenmiyor**.
- Sınırlama: Baseline-A koşuları yakınsamamış (Critic S-A), bu yüzden en iyi çözümler dağınık ve kutu geniş. Bir FSD/FUD kutusu daha dar olabilir ama bu **kanıt değil, varsayım**; PCR-6 sonrası ölçülecek.
- Bu ölçüm in-sample ve iyimser (kutu, test edilen çözümlerin komşularından kurulmuş).
- Lif dozu üst sınırı olarak gözlenen en büyük indeks kullanıldı (`Ex1Problem.vb`'de Doses = {0, 20, 25, 30, 35, 40}, yani gerçek üst sınır 5); doz değişkeni payda etkisini biraz küçültür.

## 6. Kararlar (IA, 2026-11-02: hepsi onaylandı)
1. Kapsama eşiği ve marj (−2/+3) onayı.
2. Daraltma hedefini "%50–80" yerine "ölçülen daralma" olarak raporlamak (makale bunu önceden iddia etmez).
3. Ek hedef: daralma yerine başlangıç noktası ve yakınsama hızı (23–25 kat başlangıç cezası) ön tasarımın asıl yararı olabilir.
4. PCR-5 (havuz sınırı) ve PCR-6 (ön tasarım aracı) yazımına izin.

---

## 7. Kullanıcı kararı 2026-11-03 (IA direktifi; Critic: Review_Round_Restructure.md, Review_Round_Vision.md) — post-hoc protokol revizyonu, kayıtlıdır

**Kapsam/felsefe.**
1. Proje "algoritma yarıştırması" değil **uygulamalı mühendislik optimizasyonudur**; hedef, kabul edilebilir hesap bütçesinde ve hızlı yakınsamayla ekonomik çözüm. FSD ön tasarımı çekirdek yönlendirici (warm-start / rehber) olarak kalır; FSD'yi bırakma önerisi reddedildi.
2. YAKO, BTO, ATO **rakip değil, doğrulama araçlarıdır (verifikatör)**: sonucun kalitesini farklı karakterli üç aramayla çapraz teyit eder. Makalede "X algoritması Y'den üstündür" yargısı yapılmaz; küçük farklar not olarak yazılabilir.
3. 10 000 FE kampanyası **kalır**. Deneyler iki amaçla planlanır: (i) 1 000 FE'de hızlı yakınsama başarısı, (ii) 10 000 FE'de nihai oturmuş durum.
4. Ön tasarım (FSD/FUD, lifli kolda doz taraması dahil) FE'leri **toplam bütçeye dahildir** (arama FE'si = toplam FE − ön tasarım FE'si).
5. Bağımsız duyarlılık analizleri (lifli-kapasiteli x0 dahil) kabul edildi. Kolon beton sınıfı da türetilir (PCR-8 genişletmesi).
6. YAKO/BTO/ATO'nun FSD noktasında takılması büyük sorun sayılmaz; ayar değişikliği ancak gerekirse ve kayda geçerek yapılır.

**Donmuş ablasyon protokolü (sonuç görülmeden; ikili `03_Code_Execution/ablation_bin`).**
- Kollar (her biri lifsiz ve lifli, fR × 1,00, 3 yöntem × 30 koşu, aynı tohumlar (1001–1030), eşleştirilmiş): **R** legacy 44 değişken, rastgele başlangıç, tam havuz; **A** türetilmiş rho + sınıf (24 / lifli 32 değişken), rastgele başlangıç, tam havuz; **C** A + ön tasarım x0 (kutusuz, tam havuz); **ABC** C + ön tasarım kutusu (−2/+3). (Hiyerarşi kuralı kolu **yok**: PCR-9 yapılmadı.)
- **Faz 1: toplam 1 000 FE** (ön tasarım FE'si dahil). **Faz 2: 10 000 FE** yalnız Faz 1'de ön kayıtlı kuralla seçilen kol(lar)la: lifsiz ve lifli için, Faz 1'de en düşük medyan "en iyi uygun maliyet" veren kol (A, C, ABC arasından; R referanstır, 10 000 FE referansı Baseline-A'dır); kesin kural: yöntem × senaryo başına değil, **tüm yöntemlerin birleşik medyanı**.
- Birincil ölçüt: toplam 1 000 FE sonunda en iyi uygun maliyet (TL). Uygun çözüm bulamayan koşu +∞ (sansürlü). İkincil: ilk 100 FE'de uygun oran, ilk uygun çözüm FE'si, FE-hedef eğrisi, başarı oranı (H1: Baseline-A'nın aynı yöntem için 10 000 FE medyanı; H2: en iyi bilinen +%5; H3: +%2; en iyi bilinen = bu çalışmada bulunmuş en ucuz uygun çözüm, 'optimum' değil), kısıt ihlal sayıları (PCR-11 günlüğü).
- İstatistik: Mann–Whitney U + Holm (kollar arası, eşleştirilmiş tohumla Wilcoxon duyarlılık), etki büyüklüğü A12.
- Lifli kolda ön tasarım lifsiz kapasiteyle yapılır; **lifli-kapasiteli x0 ile duyarlılık kolu** ayrı koşulur (Faz 1 sonrası).
- Yapılabilir iddia: 'ön tasarım dahil 1 000 FE'de kol X, rastgele aramanın 10 000 FE medyanına ulaştı'. Yapılamaz: 'optimum', 'ön tasarım optimumu kaçırmaz', 'pratik optimum' (kaçırma miktarı raporlanır).
- Kapsama testi (≥ 9/10) ABC kolunun kutusu için başarısız kaldı (§5); ABC kolu bu nedenle açıkça 'doğrulanmamış kutu' olarak etiketlenir.


---

## 8. Faz 2 protokolü (IA kararı 2026-11-04; Critic: koşullu onay, Faz 1 sonrası) — koşu başlamadan donduruldu

- **Amaç:** MaxFE'yi belirlemek (FE-medyan eğrisi, düzleşme noktası = "oturduğu yer", optimum değil) ve sonra eşit koşullarda lifli/lifsiz karşılaştırması.
- **Kollar:** ABC, A ve kutusuz C; lifsiz + lifli (fR × 1,00); 3 yöntem × 30 koşu (tohum 1001–1030) = 540 koşu × 10 000 FE (ön tasarım FE'si dahil). ABC kutusu "doğrulanmamış" etiketli (§5). Kolları IA onayladı (Critic önerisi ABC + A + kutusuz C).
- **PCR-12 (IA kararı):** BTO ve ATO hamle başına **tek rastgele boyut** değiştirir (`--onedim 1`; YAKO zaten böyle). Gerekçe: x0 çevresinde exploitation. Parametre Faz 1 sonuçlarına göre ayarlanmadı (k = 1, YAKO ile tutarlılık). Varsayılan kapalı; kapalıyken Faz 1 ikilisiyle birebir (6/6 çıktı aynı). Faz 1 (tüm boyut) ile Faz 2 (tek boyut) BTO/ATO sayıları farklı sürümdür; tablolarda belirtilir. İkili: `phase2_bin`, SHA256SUMS_phase2_2026-11-04.txt (FemRcModel.dll a2b418c2…875b).
- **Düzleşme ölçütü (donuk):** kontrol noktaları 1 000/2 000/3 000/5 000/10 000 FE; ardışık noktalar arası medyan iyileşmesi < %1 (bootstrap %95 GA ile); yöntem × senaryo × kol bazında.
- **Raporlama kuralları (Critic):** H1 sansürlü ve x0'dan bağımsız (x0'dan iyileşme şartı) raporlanır, YAKO için H1 kanıt sayılmaz; lifli x0 = lifsiz x0 (1 664 921 TL) eşitliği açıklanır (ön tasarım lifsiz kapasiteyle); lifli-kapasiteli x0 duyarlılık kolu Faz 2 sonrası/yanında; S2 açık, ETABS V3 öncesi bulgular bölümüne girmez.
- **Makale notu (kısa, dürüst):** "BTO ve ATO'da, YAKO ile tutarlı olarak, her harekette tek tasarım değişkeni güncellenir; bu, ön tasarım çevresinde sömürü (exploitation) kapasitesini artırır."

---

## 9. Faz 3 protokolü (IA kararı 2026-11-09; Critic: koşullu onay) — 20 000 FE koşusu başlamadan donduruldu, POST-HOC kayıt

- **Post-hoc kaydı:** §1 madde 1 ("ön tasarım senaryodan bağımsız, lifsiz kapasite") **değiştirildi**: ön tasarım senaryoya özgüdür; lifsiz senaryoda lifsiz kapasite, lifli senaryoda lifli kapasite (PCR-13). Karar Faz 2 (10 000 FE) sonuçları görüldükten sonra alındı. Faz 2 lifli C/ABC sonuçları atılmaz; "lifsiz-x0 duyarlılığı" olarak ek materyaldedir. Kod Faz 2 optimum çözümlerine bakılmadan yazıldı; ayarlanabilir parametre yok.
- **PCR-13 (lifli x0):** lif tipi (2) × doz (5) ızgarasının her noktası için aynı FSD/FUD lifli kapasiteli değerlendiriciyle koşulur (dört kiriş grubu aynı tip/doz); lifsiz ön tasarım da aday (doz 0); en düşük cezalı maliyetli aday x0 olur. Tüm adayların FE'si toplam bütçeye dahildir. Kutu seçilen adaydan (−2/+3). Lifsiz senaryo etkilenmez (33 FE, aynı x0).
- **Test bulgusu (koşu öncesi):** 363 FE; 10 lifli adayın hepsi lifsiz adayla aynı kiriş/kolon kesitlerine indi (33 FE'de durdu), maliyetleri lifsiz x0 + lif maliyeti; seçilen aday lifsiz (doz 0). Yani FSD'de lif, kesit küçültmeye izin vermedi; sınırlayıcı kısıt dayanım değil. Bu bir bulgu olarak raporlanır (neden: ayrıca incelenecek).
- **20 000 FE koşusu:** ATO + YAKO; kollar A ve ABC (lifli ABC `--fibrex0 1`); lifsiz + lifli; 10 koşu (tohum 1001–1010); 8 hücre = 80 koşu. İkili `phase3_bin` (SHA256SUMS_phase3_2026-11-10.txt; FemRcModel.dll c1a95951…f5a9). Betik: `run_phase3.sh` (yeniden başlatılabilir), çıktı `04_Results/phase3/`.
- **Ön kayıtlı oturma kuralı** (`tools/maxfe_decision.py`): hücre oturmuş sayılır, eğer 20k medyanı 10k medyanından < %1 iyileşmişse, 15k→20k < %0,5 ve 10k→20k medyan iyileşmesinin bootstrap %95 GA üst sınırı ≤ %1,5. ≥ 6/8 hücre oturmuşsa MaxFE = 20 000. Aksi halde oturmayan hücreler 30 000 FE ile (sıfırdan, aynı tohumlar) yeniden koşulur; 30 000 üst sınırdır, başka uzatma yok. IA'dan izin alınmaz (IA kararı).
- **Sonraki:** MaxFE seçildikten sonra ≥ 30 koşuyla lifli/lifsiz karşılaştırması + fR (0,8/1,0/1,2) ve lif fiyatı duyarlılığı; açıklık taraması (3B Ö1, 5–10 m); bulgular bölümüne giriş V3 ETABS + S2 sonrası.
- **En sonda, yalnız ek materyal:** BTO tüm-boyut kontrolü (A ve C, lifsiz/lifli, 30 koşu, 10 000 FE, `--onedim` yok); ana metinde PCR-12 için beş cümlelik beyan şart (tek boyut; Faz 1 sonrası eklendi, ayarlanmadı; Faz 1 ile 2 farklı sürüm; BTO Baseline-A'dan %2–7 kötü, etki ayrıştırılmadı; BTO'dan yöntem sıralaması çıkarılmaz).
- **Faz 3 ön sonuç ve IA notu (2026-11-13):** 30k aşaması bitti; BTO tüm-boyut (10 000 FE) tek-boyuttan %7–12 ucuz (Holm p < 1e-7), Baseline-A BTO'dan da ucuz (`04_Results/phase3_stats.md`, `tools/phase3_stats.py`). Yani PCR-12 tek-boyut kuralı BTO için kalite kaybettirdi; "exploitation artırır" gerekçesi BTO için desteklenmedi. **IA notu: "ATO tek boyutta iyi ise kalsın."** ATO tek-boyutla kalır (ATO tüm-boyut kontrolü yapılmadı); BTO'nun tek-boyut sonuçları için hakem görüşü istenir. Makaledeki PCR-12 beyanı bu veriye göre yeniden yazılacak (hakem sonrası). İç Critic + dış hakem görüşü istendi (aşağıda/REVIEW_LOG).
- **Faz 3c (IA kararı 2026-11-13; iç Critic + dış hakem sonrası):** BTO ve ATO tüm-boyut hamlesiyle (`--onedim` yok) kalan hücrelerde de koşulur: BTO ABC; ATO A (3b), C, ABC; lifsiz + lifli; 30 koşu; 10 000 FE; lifli hücrelerde lifli-x0 (`--fibrex0 1`). Stage 3'teki BTO lifli C hücresi `--fibrex0` olmadan koştu (x0 aynı, 363 FE fark). Betikler: `run_phase3b.sh` (ATO A), `run_phase3c.sh` (ATO C/ABC, BTO ABC; 3b'nin "ALL DONE 3b" yazmasını bekler). Toplam ≈ 240 koşu, ≈ 2 gün. Amaç: tek-boyut kuralının (PCR-12) BTO/ATO'da yapısal etkisini ayırmak; "BTO/ATO esas sonuçları" tüm-boyutla yeniden kurulabilir. YAKO'ya dokunulmaz (ABC doğası gereği tek boyut). 100k FE kontrolü yapılmaz (dış hakem de gerekli görmedi; ön tasarım + kutu varken 30k tavan yeter). Hakem raporları: `06_Review_Audit/REVIEW_LOG/Review_Round_Faz3_ic.md`, `..._dis.md`.
- **IA kararı 2026-11-16 (hakem turu Faz 3c sonrası):** Ortak seçenek (b) kabul: BTO/ATO ana seri **tüm-boyut**; tek-boyut (PCR-12) yalnız ek materyalde "post-hoc varyant"; YAKO tek-boyut olarak beyan edilir. "ATO kalsın" notu geri çekildi. DLL regresyonu (phase3_bin `--onedim 1`, A kolu lifsiz BTO/ATO, tohum 1001–1003, `--dop 1`) `03_Code_Execution/run_dll_regression.sh` ile başlatıldı, çıktı `04_Results/regression_dll/`. Lif bulgusu makalenin ana gövdesi (firmalar fiyat yanıtı vermiyor); fiyat duyarlılığı için hakem görüşü alınıyor (`Review_Round_Lif_fiyat_*.md`). YAKO 1/40 sapması: optimizasyon dışı elzem değil → araştırılmaz, sınırlama olarak kaydedilir (lifli ABC YAKO, 20k/30k birebir tekrar 39/40; kök neden bilinmiyor; `--dop 8` sıra etkisi şüphesi). IA: makale optimizasyon analizleriyle şişirilmeyecek.
- **DLL regresyonu sonucu (2026-11-16):** phase3_bin (FemRcModel.dll c1a95951…f5a9) ile `--onedim 1 --derive 1`, `--dop 1`, A kolu lifsiz BTO ve ATO, tohum 1001–1003 (6 koşu, 10 000 FE) → Faz 2 CSV'leriyle (phase2_bin a2b418c2…875b) **6/6 birebir aynı** (header satırındaki DLL hash/tarih hariç; yalnız meta satırındaki `run` etiketi farklı: tohum başına ayrı süreç olduğu için `run,1`). Betik `run_dll_regression.sh`, çıktı `04_Results/regression_dll/`. Böylece Faz 2 tek-boyut ile Faz 3 tüm-boyut kıyası aynı hesap çekirdeğiyle yapılmıştır (regresyon kapsamı: lifsiz A; lifli/ABC kolu kapsam dışı).
- **Kırılma fiyatı tablosu (2026-11-16, `tools/fibre_breakeven.ps1`, `04_Results/fibre_breakeven/`):** lifsiz en iyi uygun tasarımlar (BTO 26, ATO 30) × 2 tip × doz 1–5 × fR {0,8; 1,0; 1,2}, fiyat 0: Δ lif-dışı maliyet BTO min −4 448 / medyan +15 016 / maks +21 164 TL; ATO −10 814 / +10 199 / +20 991; Δ<0 olan konfigürasyon BTO 149/780, ATO 404/900 (çoğu düşük doz veya fR×0,8); kırılma fiyatı en çok 8,02 TL/kg (ÇŞİDB 90 TL/kg'ın çok altında). Not: fR düştükçe maliyetin düşmesi (Mpr'ye lif katkısı ve kapasite tasarımı) açıklama gerektirir; kesit vektörü sabit, yeniden optimize edilmedi.
