# Dış hakem raporu: lif fiyatı belirsizliği ve "lif seçilmedi" bulgusu

Kapsam: Faz 3 ve 3c dış raporlarım, `PREDESIGN_PROTOCOL.md` §8-9, ham CSV (`04_Results/phase3/{ato,bto}_alldim/`), model kodu (`FibreTable.vb`, `BeamDesign.vb`, `Ex1Evaluator.vb`, `Ex1Problem.vb`). Salt-okunur; koşu başlatılmadı (yalnız CSV okuması). İç Critic raporu okunmadı. Web erişimi kullanılmadı, bu yüzden literatür maddeleri "doğrulanamadı" etiketlidir.

## 0. Ham CSV'den doğrulanan sayılar (tüm-boyut, 10 000 FE, n=30/hücre)

- Lif > 0 koşu sayısı: ATO lifli A 3, C 4, ABC 0 (7/90); BTO lifli A/C/ABC 0/0/0. Toplam 7/180 (önceki raporumla aynı).
- Medyan çelik kütlesi: lifsiz A+C (ATO+BTO, 120 koşu) 10 270 kg (Ç1–Ç3 10 179–10 368). Lif seçilen 7 ATO koşusunda 10 338–10 642 kg: **lif seçilen koşularda çelik tasarrufu yok** (hepsi lifsiz medyanının üstünde veya eşit).
- Bu 7 koşuda lif-dışı maliyet (cost − fibreCost): 1 407 686; 1 430 670; 1 432 319; 1 454 088; 1 419 328; 1 440 427; 1 438 775 TL. Hepsi lifsiz ATO en iyisinin (1 404 368) ve BTO en iyisinin (1 386 277) **üstünde**. Yani lif fiyatı **sıfır** olsaydı bile bu 7 tasarım lifsiz en iyiyi geçmiyor. Bu tasarımlarla "kırılma fiyatı" alt sınırı p_L = (J_lifsiz − N(x))/K(x) negatif/tanımsız: kanıtlanmış bir kırılma fiyatı yok.
- fibreCost/fibreKg = 90,00 TL/kg (örn. 21 089,25 / 234,33): fiyat bağlantısı doğru.
- **Yeni gözlem (önceki raporlarda yoktu):** raporlanan "en iyi" satırda T>0 (ceza aktif, yani en iyi sonuç ihlalli) olan koşu sayısı: ATO lifsiz 2/90, ATO lifli 7/90, BTO lifsiz 5/90, BTO lifli 7/90 = 21/360. Lif>0 olan 7 koşudan en az 2'sinde T>0 (T=0,00026; 0,00034). Bunlar feasible optimum değil; lif sayımı ve maliyet istatistiğinden ayrı bayraklanmalı (ya "uygun çözüm bulunamadı" sayılmalı ya da `bestFeasible` kullanılmalı). Kaynak: `Fpen` satırındaki `T`; `feas_summary` satırında `bestFeasible` var.

## 1. Fiyat belirsizliği altında en küçük savunulabilir analiz

Doğrudan cevap: yalnız "bu fiyatta lif seçilmedi" yetmez (hakem fiyatı sorar), tam fiyat+fR ızgarası ise gereksiz (makaleyi şişirir). **Önerim: tek şekil/tek tablo ile "teknik sınır testi + iki fiyat noktası":**

1. **Kırılma fiyatı kesin olarak bulunamaz.** J_lifli(p) = min_x [N(x) + p·K(x)] p'de içbükeydir; arama bunu yalnız yaklaşık verir. Bulunan her uygun tasarım yalnız **alt sınır** verir: p* ≥ p_L. Üst sınır arama optimalliği gerektirir, ispatlanamaz. Makalede "kırılma fiyatı" değil, "lifin seçilmeye başladığı fiyat (aramada gözlenen)" ve "alt sınır" kullanılmalı.
2. **Teknik sınır koşusu (esas):** lif fiyatı = 0 (veya 1 TL/kg) ile tüm-boyut BTO lifli A kolu, n=10, fR=1,0. Fiyat sıfırken lif bedava; sonuç hâlâ lifsizden iyi değilse (maliyet ≥ lifsiz en iyi, çelik kütlesi düşmüyorsa) **hiçbir fiyat lifi haklı kılmaz**; bu, firmalardan fiyat alamama sorununu ortadan kaldırır. Süre ≈ 1,6 saat (BTO, 8 çekirdek). Bu tek koşu bulgunun anahtarıdır.
3. **Fiyat noktası:** aynı düzen 30 TL/kg (n=10, 1,6 saat) yalnız teknik sınırda lif seçiliyorsa; 90 TL/kg zaten var. Üç nokta (0 / 30 / 90) fiyat "eğrisi" için yeter.
4. **fR taraması:** yalnız koşullu. Fiyat 0'da lif seçilmiyorsa fR ölçeği 1,5 ile bir koşu (price 0, n=10, 1,6 saat) "tablodaki fR değerleri ortalama; karakteristik değil" belirsizliğinin yönünü kapatır (fR artınca bile lif işe yaramıyorsa sonuç sağlam). fR 0,8/1,2 gerekli değil.
5. **Ek materyale:** koşulsuz tam ızgara (fiyat × fR × kol × yöntem) gerekmez.

**Literatür (doğrulanamadı):** SFRC elemanların maliyet/tasarım optimizasyonunda tipik uygulama, lif dozunu ve tipini değişken alıp lif fiyatı ve kapasite modelini (RILEM/fib σ-ε) sabit tutmak, ardından lif fiyatı veya lif performansı için duyarlılık vermektir. Aday kaynaklar (web ile doğrulanamadı, hatırlamaya dayalı; yazarlar DOI ile teyit etmeli): Barros ve Figueiras (1999) SFRC eğilme modellemesi (J. Mater. Civ. Eng.), Barros vd. (2005) SFRC çatlama sonrası davranış (Materials and Structures), Soetens ve Matthys (2014) ve Blanco vd. (2013) SFRC kalıcı tasarım yönergeleri (fib MC2010 yöntemi), Kytinou/Chalioris tipi SFRC kiriş optimizasyonları (ad ve yıl kesin değil). Bu listenin hiçbirini "kırılma fiyatı çalışması" olarak atıf göstermeyin; doğrulanana dek "doğrulanamadı".

## 2. lif=0: arama artefaktı mı, gerçek sonuç mu?

Kanıt dengesi **gerçek (model-içi) sonuç lehinde**, ama doğrulanmadığı için "gerçek" diye yazılmaz:

Destekleyen:
- Seçildiği 7 ATO koşusunda çelik kütlesi düşmüyor (10 338–10 642 kg; lifsiz medyan 10 270). Lif varsa donatı azalması beklenir; gözlenmiyor. Bu, rastgele/atıl lif geni görünümü (ATO ABC'deki fibreKg=0 ve gen=1 tutarsızlığı dahil).
- Lif sıfır fiyatta bile (bu 7 tasarım) lifsiz en iyiyi geçmiyor.
- BTO'da 0/90 (daha iyi arama → daha az lif), ATO'da 7/90 (zayıf arama), tek-boyutlu varyantta daha sık (BTO 59/90): lif seçimi arama kalitesiyle **azalıyor**; bu artefakt (gürültü) örüntüsüdür, lifin yararlı olduğu örüntü değildir.

Model koduna göre mekanizma (okunan satırlar; ETABS ile doğrulanmadı):
- **Kesit boyutlarını rijitlik/geometri belirler, dayanım değil.** Tek bir BTO koşusunun ihlal sayıları: Q ≤ 0,05 kararlılık (ACI 6.6.4.3) 4 044, kolon boyutu ≥ 20 db (ACI 18.8.2.3) 2 881, etriye aralığı 1 671, güçlü kolon (ACI 18.7.3.2) 545. Lif rijitliği (Gross modeli) değiştirmez; Q ve ötelenme aynı kalır. PCR-13 bulgusu (lifli FSD aynı kesitlere iniyor) **destekleniyor**, çelişki yok.
- **Kiriş boyuna donatı alt sınırlarla bağlı olabilir.** `BeamDesign.DesignGroup`: `AsMin = max(ACI MinAs, 0,8 fctd/(fy/1,15) b d)` lif için artırılmıyor; `AsBot ≥ 0,5 AsTop`; `ρ ≤ %2`; `εt ≥ 0,004`. Eğilme talebi bu alt sınırın altında kalan gruplarda lif donatıyı azaltamaz (lif yalnız `Required` fonksiyonunda katkı verir). Hangi gruplarda As = AsMin olduğu henüz ölçülmedi: **bu hipotezdir**, ucuz kontrolle (§2.1) test edilir.
- **Kapasite tasarımı lif aleyhine işliyor:** `MrOf` lif katkısını γF=1 ile Mpr'a ekler; kiriş Mpr arttıkça kiriş kesmesi `Ve = (Mpr,top+Mpr,bot)/ln + Vdy` ve güçlü-kolon talebi (ΣMnc ≥ 1,2 ΣMnb) artar. Lif donatı azaltmadığında Mpr yükselir, etriye ve kolon talebi büyür: lif net maliyet getirir.
- Sarılma bölgesinde lif kesme katkısı 0 (`vcConf`; ZeroVc doğru ise zaten Vc=0): kesme tarafında lif avantajı yok.
- **Lif ızgarası lehe değil:** `FibreTable`: yalnız C30/C40, 20–40 kg/m³, **alt doz 20, ekstrapolasyon yok**. Yani minimum lif maliyeti 20 × 90 = 1 800 TL/m³ kiriş betonu; betonun kendisi ≈ 3 750 TL/m³ (245 788 / 65,64). Ortalama donatı fiyatı ≈ 69,7 TL/kg (709 669 / 10 180); 20 kg/m³ lif ≈ 25,8 kg çelik/m³ eşdeğeridir. Lifin kendini ödemesi için kiriş donatısının yaklaşık %20 azalması gerekir. 10–15 kg/m³ gibi düşük dozlar (SFRC yapısal kullanımda yaygın) arama uzayında yok: **model lif lehine kurulmamış; bu sınır makalede yazılmalı.**
- fR değerleri "test ortalaması, karakteristik olduğu doğrulanmadı" (kodda yorum): **lif lehine iyimser**. Buna rağmen lif seçilmedi, bu bulguyu güçlendirir. Sarılma bölgesi kesme katkısı 0 ve γF=1 (Mpr) ise **aleyhe/ihtiyatlı**. Net yön belirsiz; duyarlılık yalnız fR ölçeği için yapılmış.

### 2.1 Ucuz deterministik kontrol (arama yok, dakikalar)

Mevcut CSV'lerdeki lifsiz en iyi x vektörleri (ATO/BTO A kolu, en iyi 5–10 koşu) için `Ex1Evaluator` ile tek değerlendirme (bir `--evalx` kipi ya da küçük bir betik; **kod değişikliği PCR ve IA onayı gerektirir**, hakem olarak ben yapmıyorum):
1. Dört kiriş grubunda lif tip×doz ızgarasını (4D/5D × 20–40) sırayla koyup kesitleri sabit tutarak yeniden değerlendir.
2. Raporla: her kiriş grubu için `AsTop`, `AsBot`, `AsMin`, hangisi bağlayıcı (AsMin / 0,5 AsTop / talep), ΔçelikKg, Δmaliyet(fiyat 0), uygunluk (T), aktif kısıtlar (güçlü-kolon, Ve, joint).
3. Sonuç yorumu: (a) AsMin'de duran gruplar çoksa → "alt sınırlar bağlayıcı, lif donatı azaltamaz" (gerçek sonuç); (b) AsMin üstündeyse ve ΔçelikKg yine ≈ 0 → modelde lif katkısının zayıf olduğu (fR1 2,7–4,7 MPa) sonucu, kod hatası şüphesi için fR ölçeği 3 ile bir kez dene (hata/artefakt testi); (c) ΔçelikKg belirgin düşüyor ama uygunluk bozuluyorsa kapasite tasarımı/güçlü kolon sınırlayıcı. Bu sonuç fiyat taramasından bağımsız ve en açıklayıcıdır.
4. Fiyat 0'da toplam maliyet farkı → p_L alt sınırı (§1.1) doğrudan hesaplanır.

Bu kontrol ucuzdur ama **optimumda ikinci derece etkiyi** (lif varken kesitlerin küçülmesi) göstermez; onu §1.2'nin fiyat-0 aramasi kapatır. İkisi birlikte artefakt/gerçek ayrımını yapar.

## 3. Öneri: asgari iş, süre, iddialar

**Asgari iş (toplam ≈ 4–5 saat CPU + yazılım ~yarım gün):**
| # | İş | Süre | Not |
|---|---|---|---|
| a | Deterministik kontrol (§2.1) | CPU dakikalar; kod PCR yarım gün | IA onayı |
| b | BTO lifli A, fiyat 0, fR 1,0, n=10 | 1,6 saat | teknik sınır |
| c | b'de lif seçilmezse fR=1,5, fiyat 0, n=10 | 1,6 saat | koşullu |
| d | b'de lif seçilirse fiyat 30, n=10 | 1,6 saat | koşullu |
| e | T>0 koşularını işaretle; `bestFeasible` ile yeniden sayım | CPU yok | betik |

(30 koşuluk hücre 4,7 saat; gerekmez, tanımlayıcı sonuç n=10 ile yeterli.)

**Ana metin (kısa, 1 tablo + 1 paragraf):** lifli/lifsiz çerçeve karşılaştırması; fiyat 0/90 (ve gerekirse 30), fR 1,0 (ve gerekirse 1,5), lif>0 koşu sayısı, çelik kütlesi, maliyet; kiriş grup tablosu (a'dan). Yöntem kısıtları (doz ≥ 20, yalnız C30/C40, fR ortalama, γF=1, sarılmada lif katkısı 0, PCR-12/tüm-boyut notu).
**Ek materyal:** 7 lif-seçili koşunun ayrıntısı, T>0 listesi, ham CSV özetleri, tek-boyut varyant sonuçları.

**Yapılabilir iddialar:**
- "Bu model, fiyat 90 TL/kg (ÇŞİDB 2026) ve fR (tedarikçi tablosu, karakteristik olduğu doğrulanmadı) ile, arama uzayında lif seçilmedi ve [b/c sonucu] fiyat 0'da dahi lifli en iyi lifsizden ucuz çıkmadı."
- "Lif, bu çerçevede kesit boyutunu belirleyen rijitlik/geometri kısıtlarını (Q, kolon boyutu) etkilemez; kiriş donatısı [a sonucu] alt sınırlarla bağlı." (a yapılırsa.)
- "Lif seçimi gözlenen koşularda donatı azalması göstermedi."

**Yapılamaz iddialar:**
- "Lif ekonomik değildir/lif lifsizden pahalıdır" (genel), "kırılma fiyatı X TL/kg'dir" (arama optimali ispatsız; yalnız alt sınır), "lif tüm lif dozlarında/yapılarda yararsızdır" (doz < 20, C25 ve C50, tek örnek), "lif seçilmemesi gerçek optimumdur" (global optimum kanıtsız), lifli > lifsiz medyan farkı yorumu (aynı uzay, gürültü), ETABS doğrulaması olmadan "kapasite oranı" iddiaları (V3 şartı).
- Lif sayımı T>0 koşuları ayıklanmadan yazılmaz.

## 4. Kararım

Lif bulgusu makalenin gövdesi olabilir **ancak** deterministik kontrol (a) ve fiyat-0 koşusu (b) olmadan "lif seçilmedi" bulgusu, firma yanıtı olmayan tek fiyata dayandığı için hakem tarafından "fiyata bağlı, sonuç yok" diye reddedilir. (a)+(b) ile fiyat belirsizliği sorunu iddia düzeyinde kapanır. Karar: **küçük revizyon**; bu iki iş yapılmadan lifli/lifsiz başlığı kapatılmaz.
