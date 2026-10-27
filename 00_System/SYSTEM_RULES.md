# 📜 SYSTEM_RULES.md (Akademik Üretim Anayasası)

## 1. Dil ve üslup (Anti-AI)
- **Kara liste:** delve, tapestry, plethora, pivotal, paramount, holistic, seamless, cutting-edge, state-of-the-art, underscore, showcase, groundbreaking, revolutionary. Türkçe karşılıkları: "önemli bir rol oynar", "çığır açan", "benzersiz", "mükemmel".
- **Klişe girişler** ("Son yıllarda ... giderek artan ilgi") tek başına paragraf açamaz; paragraf sayısal veya atıflı bir olguyla açılır.
- **Yol haritası paragrafı yasak** ("Makalenin devamında ... bölümde sunulmuştur").
- **Cümle uzunluğu:** teknik cümle ≤ ~30 kelime; bir cümlede tek mekanizma.
- **Nesnellik:** iddia = sayı + birim + kaynak (tablo/şekil no).
- **Kanıtsız öncelik iddiası** ("ilk kez", "özgün") literatür taramasıyla belgelenmeden yazılmaz.

## 2. Makale yapısı (IA düzeltme listesinden)
Başlık · Özet · Anahtar kelimeler · Giriş (alt başlıksız, akıcı) · 2 Materyal ve Metot (2.1 problem, 2.2 algoritmalar, Çizelge 1 algoritma parametreleri, 2.3 uygulama) · 3 Bulgular ve Tartışma: 3.1 Genel parametreler ve örneklerin tanıtımı → 3.2 Örnek 1 → 3.3 Örnek 2 → 3.4 Örnek 3 → 3.5 Genel tartışma · Sonuç (çıkarım + gelecek öneriler) · Kaynakça (APA 7).

## 3. Tablo ve şekil kuralları
1. Tüm maliyet tabloları formüllü Excel'den üretilir (`Miktar × Birim Fiyat`); satır kayması ve kalem karışması yasak.
2. Birim her başlıkta yazılır; "N/A" yasak.
3. Belirsiz kısaltma yasak: "C30/C40" yerine "Kirişler: C40, Kolonlar: C30"; 3D çerçevede B1, C1 gibi eleman grupları metinde tanımlanır.
4. Şekiller: ETABS ham görüntüsünden aks/grid/gereksiz detay temizlenir; yalnız taşıyıcı sistem, mesnetler belirgin.
5. Sembol ve denklemler OMML (Word Denklem Düzenleyicisi); düz metin Σ, ρ, ∅, Δ yasak. Denklem numaraları (1), (2) … sıralı; tekrarlayan numara (ör. g8, g9 için (7),(8) çakışması) denetlenir.

## 4. Kod, simülasyon ve veri
1. Koşu hijyeni: koşu klasörü + girdi + program sürümü + ETABS sürümü + tohum + bütçe kayıtlı.
2. Sonuçlar program çıktısından (XLSX/TXT) betikle okunur; elle kopyalama yok.
3. Sentetik veri yasağı; gerçek dosya yoksa analiz betiği durur.
4. Final tasarım: ETABS'te düzeltmesiz yeniden analiz + `Concrete Frame Design` kapasite oranları (eğilme, kesme) okunur.

## 5. Hakem geçit protokolü
Alt ajan üretir → Critic `AUDIT_CHECKLIST.md` ile denetler (`[REVİZYON TALEBİ]` / `[HAKEM ONAYI VERİLDİ]`) → kullanıcı teyidi → ilerleme. Mikro hatalar alt ajan düzeltir; makro tercihler Master üzerinden kullanıcıya gider; kural gevşetme yasak; en çok 3 iterasyon.
