# Dış hakem raporu: Faz 3c (tüm-boyut BTO/ATO kontrolü)

Kapsam: Faz 3 dış raporum (Ek yanıt dahil), `phase3_stats.md`, ham CSV'ler (`phase3/{ato_alldim,bto_alldim,20k,30k}`, `phase2/`). Sayılar ham CSV'den bağımsız yeniden hesaplandı (Fpen satırındaki cost, fibreKg, x satırı). Salt-okunur çalışıldı; iç Critic raporu okunmadı.

## 0. Yeniden hesaplanan sayılar

Tüm-boyut, 10 000 FE, n=30 her hücre, medyan (min):
- ATO: lifsiz A 1 427 926 (1 404 368), C 1 422 509 (1 405 361), ABC 1 424 611 (1 424 611); lifli A 1 437 819 (1 410 547), C 1 441 535 (1 415 728), ABC 1 424 611 (1 424 611).
- BTO: lifsiz A 1 400 080 (1 386 277), C 1 402 991 (1 392 298), ABC 1 432 446; lifli A 1 406 575, C 1 405 600 (1 390 106), ABC 1 434 218.
- YAKO 30k (n=10): lifsiz A 1 425 078 (min 1 389 653), ABC 1 446 440; lifli A 1 455 201 (min 1 417 995), ABC 1 461 903.
- Not: tablodaki ATO medyanlarıyla küçük sapmalar var (ör. lifsiz A tek-boyut ATO: benim glob 1 440 270, tablo 1 440 825; lifli C 1 465 932 / 1 466 229). Faz 2 klasöründe tabloda sayılandan farklı dosya kümesi olabilir (ör. ek/yeniden koşu dosyası). Üretici betik hangi dosyaları aldığını (dosya sayısı, seed listesi) çıktıda yazmalı. Sonucu etkilemez (fark <%0,1).

## 1. Ön tahmin doğrulandı mı, etki neden BTO'dan küçük?

**Evet, yön doğrulandı.** ATO'da tüm-boyut tek-boyuttan 0,3–3,6 % ucuz; ABC'de A12=1,00 (p<1e-10), A/C-lifsiz'de zayıf-orta. Ancak "yapı bozuluyor" yorumu ATO için BTO kadar güçlü değil; veri iki ayrı mekanizma gösteriyor:

1. **BTO'da operatör bozulması** (önceki raporumdaki mutasyon seyrelmesi + ±1 yürüyüş) büyük etki yaratıyor (%6–12, A12≈1).
2. **ATO'da etki küçük** çünkü TSA tek-boyut kipi de x + α(best − x_r) formülünü koruyor; her tohum yine popülasyonun en iyisine doğru çekiliyor, yani arama yönü bilgisi tek boyutta da taşınıyor. BTO'da ise tek-boyut kipinde göç yalnız 1 gende ve mutasyon 1/nd'ye seyreliyor, bilgi paylaşımı neredeyse kayboluyor. Ayrıca ATO tek-boyutta zaten çok küçük popülasyonla hızlı donuyor, dolayısıyla tüm-boyut kazancı "daha çok keşif" olarak küçük kalıyor. Bu yorum kod okuması ve verinin tutarlılığıdır, ayrıştırma deneyi yapılmadı (varsayım).
3. **ABC kolunda tavan etkisi** (bkz. madde 4): ATO ABC'de tüm-boyut medyanı kutunun tabanına oturuyor, bu yüzden fark kutu tabanıyla tek-boyutun uzaklığı olarak ölçülüyor (%2,6–3,6) ve p çok küçük; bu, operatör etkisinin saf ölçüsü değil.

C-lifsiz'de anlamsızlık (p=0,40) tek başına "ATO tek-boyut yeterli" demek değil; 30 koşuda %0,3 farkı ayırt edecek güç yok. Ayrıca A-lifsizde (p=0,0026 Holm) %0,9 anlamlı. Sonuç: tek-boyut ATO'nun kaybı küçük ama sistematik.

## 2. Makalede tek-boyut sonuçlar ne olmalı?

Öneri:
- **Ana tablo: tüm-boyut BTO ve ATO** (orijinal BBO/TSA tanımına uygun; hem BTO hem ATO için kontrol yapıldı, kural sonuca bağlı değil). Tek-boyut sonuçlar **ek materyalde** "tek-boyut hamle varyantı (PCR-12)" adıyla, BTO/ATO etiketi taşımadan, yan yana tablo olarak (fark %0,3–11,6) verilsin. Hiç saklamak yanlış olur: PCR-12 post-hoc karar, etkisi büyük, şeffaflık gerekir.
- Ana metinde 3–5 cümle: karar ne zaman, neden alındı (Faz 1 öncesi gözlem), ne bulundu, "exploitation artırır" gerekçesi verinin çürüttüğü için kaldırıldı. "ATO tek boyutta iyi ise kalsın" kuralı yazılmamalı; kural şimdi "ikisi de ölçüldü, tüm-boyut orijinal tanım olduğu için ana tablo".
- Faz 2 tek-boyut sonuçları kendi içinde (YAKO ile) kullanılabilir ama yöntem karşılaştırması ana metinde yalnız tüm-boyut ve YAKO üzerinden yapılsın; zaten üstünlük iddiası yok.

**İki DLL (a2b418c2… / c1a95951…) için yeterli kayıt**, hakem düzeyinde:
1. Bit-bit regresyon: yeni DLL ile `--onedim 1`, Faz 2'den 2 hücre (lifsiz A BTO, lifsiz A ATO), aynı tohum 1001–1003 → cost ve x satırının Faz 2 CSV'siyle birebir aynı olması. Lifli kol için PCR-13 farkı (x0 seçimi) olduğundan lifli hücrede birebirlik beklenmez; lifli için "ön tasarım çıktısı x0, FE sayısı" farkı açıkça yazılmalı.
2. DLL farkının tam listesi (PCR numaraları, etkilenen yol) ve her DLL'nin SHA-256'sı ek tabloda.
3. Faz 3 içinde karşılaştırmalar (tüm-boyut vs. YAKO 30k) aynı DLL'de yapılmış; Faz 2 vs Faz 3 karşılaştırması (tüm-boyut / tek-boyut) DLL karıştırıcısı taşır; regresyon geçerse bu kapanır. Regresyon yapılmadan önce sonucu "doğrulanmış karşılaştırma" diye yazmayın.

## 3. Çapraz teyit

En iyi konfigürasyonlar (lifsiz A, medyan; kutusuz): BTO 1,400 M, ATO 1,428 M, YAKO-30k 1,425 M. Minimumlar: BTO 1 386 277, YAKO-30k 1 389 653, ATO 1 404 368. Yani **%2 bandı**: medyanlar 1,400–1,428 M (aralık %2,0), en iyi koşular 1,386–1,404 M (%1,3). Lifli A: BTO 1,407; ATO 1,438; YAKO-30k 1,455 (%3,4 fark), lifli en iyiler 1,390–1,418.

Önceki raporda "çapraz teyit başarısız" demiştim (1,40 vs 1,42–1,47). Yeni veri bunu **kısmen** düzeltiyor: ATO tüm-boyut ve YAKO-30k artık %1,5–2,0 içinde, BTO en düşük. Hâlâ bu farklar seed gürültüsünün üstünde (BTO-ATO A12 yüksek; IQR'lar ayrık değil ama ayrışıyor), yani yöntemler **aynı noktaya** yakınsamıyor; **aynı bölgeye / %2–3 bandına** ulaşıyor.

Yöntem üstünlüğü iddiasına izin vermez: (i) YAKO yalnız n=10 ve farklı bütçe (30k vs 10k), (ii) üç yöntem farklı FE tüketimiyle kıyaslanıyor, (iii) BTO'nun kazancı kısmen operatör ayarı kararlarına duyarlı, (iv) kutu/ön tasarım uzayı ortak ama hiperparametreler ayarlanmamış. Fark makalede **betimleyici** olmalı, sıralama cümlesi kurulmamalı.

Yazılabilecek "çapraz teyit" ifadesi (örnek): "Ön tasarımdan başlayan üç bağımsız arama kuralı, aynı 44 değişkenli uzayda ve 10 000–30 000 FE bütçeyle, lifsiz senaryoda en iyi koşular 1,386–1,404 M TL, medyanlar 1,400–1,428 M TL aralığında sonuç verdi (±%1,0 medyan aralığı). Aralık, bu sonuçların tek bir aramanın artefaktı olmadığını gösterir; ancak hiçbir yöntemin global optimuma ulaştığı kanıtlanmamıştır ve yöntemler arası fark istatistiksel olarak tutarlı olsa da hiçbiri üstün ilan edilmemiştir." Kaçınılacaklar: "yakınsadı", "optimum", "aynı optimuma". Teyit iddiası mutlaka en düşük maliyetin 1,386 M olduğu (en iyi bilinen) ve ortalama farkın %2 olduğu ile birlikte verilmeli. "Çapraz teyit" etiketi yerine "tutarlılık kontrolü (cross-check)" daha dürüst.

## 4. ATO ABC: lifsiz ve lifli medyanın birebir aynı (1 424 611)

Ham CSV: lifsiz ABC'de 23/30 koşu tam 1 424 611 (hepsi **aynı x**: 6 farklı sonuç var, 3,4,0,0,3,3,0,0,4,4,…); lifli ABC'de 17/30 koşu 1 424 611 (farklı x vektörleri, lif genleri 0/1 karışık ama maliyet aynı). Minimum her iki hücrede 1 424 611; üst uç 1 428 422 / 1 434 250. Sebep: **ABC kutusunun en iyi noktası**. Kutu (havuz kısıtı) küçük olduğundan ATO tüm-boyut her zaman aynı noktaya iniyor; bu nokta kutu-içi bir (muhtemelen global) optimum, yakınsama göstergesi ama *kutuya özgü*. Kanıt: kutusuz kollarda 1 386 277 (BTO A) ve 1 389 653 (YAKO A) daha ucuz; kutu bu iyileşmeyi dışlıyor (en az %2,7). Bu, daha önce Faz 1'de yazılan "kutu optimumu kapsamıyor" bulgusuyla tutarlı ve makalede açıkça yazılmalı: ABC kutusu sonucu hızlandırıyor ama ~%2–3 daha pahalı tasarıma kilitliyor. BTO ABC'de 8/30 (lifsiz), 2/30 (lifli) bu değere iniyor; yani BTO'nun kutu içinde de çeşitliliği daha yüksek.

Lifli x genlerinde 1'ler görünmesine rağmen fibreKg=0 çıkıyor (17 koşu): lif genleri bu çözümlerde etkisiz (inaktif/doz 0'a eşlenen kod). Kodu doğrulayın: gen değeri 1 ile doz 0 arasındaki eşleme, yanlışlıkla lif yokluğunu "lif geni=1" gibi göstermek makalede kafa karıştırır; tasarım vektörü yerine kg/m³ raporlanmalı.

**Lif=0 sayımları (fibreKg>0 koşu sayısı, ham CSV):** ATO tüm-boyut lifli: A 3/30, C 4/30, ABC 0/30 → 7/90 lifli, **83/90 lif=0**. BTO tüm-boyut lifli: 0/90 (**90/90 lif=0**). Toplam 7/180 lifli. Faz 2 tek-boyut ATO: A 8, C 9, ABC 6 (23/90); BTO tek-boyut: A 22, C 13, ABC 24 (59/90); YAKO 30k lifli A 1/10, ABC 3/10. Tek-boyutta lif daha sık çünkü daha kötü aramanın artefaktı (rastgele lif genleri); iyi aramada lif seçilmiyor, ve seçildiği 7 ATO koşusunda maliyet (1,428–1,486 M) tüm-boyut lif=0 medyanından kötü ya da eşit.

**Lifli/lifsiz kıyası bu yüzden ne iddiaya izin verir?**
- İzin verilen: "Bu fiyat (90 TL/kg), bu fR modeli ve bu ACI+fib kapasite modelinde, 180 lifli senaryo koşusunun 173'ünde arama lif kullanmayan tasarıma indi; lif kullananlar daha pahalı çıktı. Lif bu problemde maliyet açısından seçilmedi."
- Yasak: "lifli çerçeve lifsizden pahalıdır/ekonomiktir" (lifli arama uzayı lifsizi kapsıyor; Phase 3 lifli medyanı lifsizden %0,4–0,7 yüksek, aynı tasarım ve arama gürültüsü). Lifli ≥ lifsiz fark tüm-boyutta anlam taşımıyor; tek çıkarım: lifli uzay lifsizden iyi optimum bulamadı.
- Gereken ek: lif fiyatı / fR (kırılma fiyatı) duyarlılığı. Lif fiyatı düşürüldüğünde (örn. 20–90 TL/kg) ve fR ölçeği artırıldığında lif seçimi ne zaman başlıyor? Bu olmadan makalenin "lifli vs lifsiz" başlığı sonuçsuz kalır. Ek olarak PCR-13'te lifli FSD'nin kesitleri küçültmemesi (lif donatıyı azaltmıyor) nedeni açıklanmalı (sınırlayıcı kısıt dayanım değil; kapasite tasarımı, sarılma bölgesinde lif katkısı 0 varsayımı, sünek detay şartları).

## 5. Karar

**Küçük revizyon** (önceki: büyük). Gerekçe: ATO tüm-boyut kontrolü yapıldı ve BTO ile tutarlı çıktı, kural sonuca bağlı değil; çapraz tutarlılık %2–3 bandında gösterildi; ana tablo tüm-boyut olabilir. Hâlâ **kabul değil**: aşağıdaki maddeler kapatılmalı.

En önemli 5 madde:
1. **Regresyon ve DLL kaydı**: yeni DLL'de `--onedim 1` ile Faz 2'den birebir yeniden üretim (2 hücre × 3 tohum) + DLL değişiklik tablosu. Tek-boyut sonuçlar ek materyalde, "varyant" adıyla.
2. **Lifli/lifsiz iddiası daraltılsın**: 173/180 koşuda lif=0; "lif seçilmedi" bulgusu + fiyat/fR duyarlılığı (kırılma fiyatı). Ekonomik karşılaştırma cümlesi yok. PCR-13 nedeni (lif neden kesit küçültmüyor) açıklanmalı.
3. **ABC kutusu bulgusu**: ATO ABC medyanı kutu tabanı (1 424 611) ve kutu en az %2,7 daha ucuz tasarımları dışlıyor; makalede kutu "başlatıcı yardım" değil "kısıt" olarak tanıtılsın, A ve C kolları ana sonuç olsun.
4. **Çapraz tutarlılık ifadesi** yukarıdaki dilde: bant %2–3, en iyi bilinen 1,386 M, optimum/yakınsama iddiası yok, YAKO n=10 ve 30k farkı not; mümkünse YAKO'yu tüm-boyut kollar için 10k'da da n=30 koşturun (aynı bütçe, adil kıyas).
5. **Bütçe ve istatistik raporlaması**: Holm ailesi yalnız aynı-uzay testlerle; Faz 2/3 dosya kümesi betiğin çıktısında yazılsın (fark ≈%0,1 tutarsızlık); 30k hücrelerinin n'si ve "settled at 20k" hücreleri belirtilsin.

**MaxFE/bütçe ifadesi** (öneri): "Her koşu, ön tasarım FE'si dahil, 10 000 FE ile sınırlandırıldı (YAKO için ek olarak 20 000 ve 30 000 FE kontrol noktaları, n=10). Bütçe, hesaplama maliyeti nedeniyle seçilmiş bir üst sınırdır; maliyet–FE eğrilerinin düzleştiği veya bulunan çözümlerin global optimum olduğu iddia edilmemektedir. Tüm-boyut BTO ve ATO 10 000 FE'de YAKO'nun 30 000 FE değerleriyle %2 içinde kalmıştır." "MaxFE yeterli" veya "yakınsadı" yazılmaz.
