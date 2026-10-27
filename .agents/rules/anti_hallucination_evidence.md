# 🛡️ Anti-Hallucination, Birincil Kanıt ve Sıfır Sentetik Veri Protokolü (GENELLEŞTİRİLMİŞ VE KESİN KURAL)

Bu belge; bu çalışmadaki tüm araştırma, literatür, veri toplama, yönetmelik/birim fiyat parametresi, doğrulama ve akademik yazım işlerinde yapay zekâ ajanları tarafından **istisnasız ve tavizsiz** uygulanacak birleşik doğruluk protokolüdür.

---

## 🔒 1. ORİJİNAL BELGE ZORUNLULUĞU VE AJAN-ÜRETİMİ "ÖZET/KANIT" YASAĞI
1. **Orijinal Format İndirme:** Veri kaynağı olan belge **orijinal biçiminde** (yayıncının resmî PDF'i; yönetmelik PDF'i; birim fiyat listesi PDF/XLSX) indirilip `Papers/` (makaleler) veya `Evidence/` (yönetmelikler, birim fiyatlar, yazılım belgeleri) klasörüne konur. Adlandırma: `<YazarYıl>_<kısa ad>.<uzantı>` veya `<Kurum>_<Belge>_<Yıl/Sürüm>.<uzantı>`.
2. **Ajan Üretimi "Kanıt" Yasağı:** Ajanın kendi ürettiği HTML/özet kartları, tek sayfalık bilgi fişleri veya yapay "arşiv PDF'leri" **ASLA kanıt sayılamaz ve kanıt klasörlerine konulamaz.**
3. **Kriptografik Doğrulama:** İndirilen her belgenin **tam bayt boyutu** ve **SHA-256** özeti raporda listelenir.

## 🔍 2. DOĞRUDAN BİREBİR ALINTI VE SAYFA/TABLO/MADDE NO
1. Her veri satırı için orijinal belgeden **birebir alıntı (≤ 25 kelime)** ve sayfa/tablo/madde/denklem numarası yazılır. Yorumlanmış, benzerleştirilmiş veya hafızadan tamamlanmış ifade yasaktır.
2. Belgede yoksa `BULUNAMADI` yazılır.
3. Şu değerlerin her biri kaynağıyla eşleştirilir: yönetmelik katsayıları ve sınırları (ACI 318-14 donatı oranı ve kesme sınırları, fib Model Code 2010 lifli beton σ–ε parametreleri, öteleme sınırı), yük parametreleri, malzeme özellikleri (beton sınıfı, donatı sınıfı, lif tipi ve CMOD/EN 14651 test değerleri), birim fiyatlar (yıl, bölge, para birimi, kalem), literatürden alınan karşılaştırma değerleri.

## 🚫 3. SIFIR TAHMİN VE SIFIR SENTETİK VERİ
1. "Genel kabul", "tipik değer", "makul aralık" kanıt değildir.
2. Analiz sonuçları yalnızca programın bu projede fiilen çalıştırılmasıyla üretilir; çalıştırılamıyorsa `ANALİZ ÇALIŞTIRILAMADI` raporlanır. Programın kendi belgelerindeki sayılar (`YETENEKLER_RAPORU.md`, `DEGISIKLIKLER.md`, `FEM_SURUM_KAYITLARI.md`) yeniden üretilmeden makaleye yazılamaz.
3. Optimizasyon sonuçları (final maliyetler, istatistikler, geçiş katları, süreler) kayıtlı koşu dosyalarından üretilir; dosyası olmayan sayı raporlanamaz.

## 📊 4. KALEM KALEM DURUM VE TOPLU "%100" YASAĞI
Her satıra ayrı ayrı: `DOĞRULANDI (belge + sayfa/tablo/madde)` · `KISMEN (eksik yön)` · `DOĞRULANAMADI` · `BULUNAMADI` · `ERİŞİLEMEDİ (404, paywall, form)`. Toplu "%100 doğrulandı" ifadesi yasaktır.

## 🌐 5. BAĞLANTI SAĞLIĞI VE ERİŞİLEMEZLİK
1. 404, DNS hatası veya bot engeli veren bağlantılar kanıt gösterilemez (HTTP 200 kontrolü yapılır).
2. DOI'ler CrossRef üzerinden doğrulanır (`.agents/tools/reference_resolver.py`); başlık, yazar, cilt ve sayfa CrossRef kaydıyla birebir eşleşmelidir.
3. Paywall/form/lisans gerektiren kaynaklar (ücretli standartlar, maliyet veri tabanları) `ERİŞİLEMEDİ` işaretlenir; URL ve dosya adıyla kullanıcıdan manuel indirme istenir.

## 📋 6. STANDART RAPORLAMA ÇIKTILARI
1. `*_EVIDENCE_*.md`: belgeler tablosu (Ad, Boyut, SHA-256, URL) + bulgular tablosu (Belge, Sayfa/Tablo/Madde, Birebir Alıntı, Durum) + kullanıcının manuel yapacakları.
2. `*_SOURCES_*.csv`: makinece okunabilir kaynak matrisi.
