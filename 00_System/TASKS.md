# ✅ TASKS.md (Görev ve Karar Listesi)

*Son güncelleme: 2026-10-27. Durum: ⬜ açık · 🔄 sürüyor · ✅ bitti · ⛔ engelli*

## A. IA'nın düzeltme listesi (`Sources/Makale_Metni/Yapılacaklar.docx`) → görevler
| # | Görev | Sorumlu ajan | Durum |
|---|---|---|---|
| T1.1a | Tüm maliyet tablolarını (Çizelge 4, 5, 6) formüllü Excel'de baştan kur (Miktar × Birim Fiyat; Beton/Donatı/Lif satır kayması) | Developer + Results | ⬜ |
| T1.1b | Örnek 2 YAKO ve BTO sonuçlarını kontrol et | Developer | ⬜ |
| T1.1c | 21 ve 42 elemanlı çerçevelerin lifsiz tasarımlarında "N/A" kapasite oranlarını ETABS Concrete Frame Design'dan oku (MKKKO eğilme, MKO kesme) | Developer | ⬜ |
| T1.1d | 42 elemanlı 3D lifli tasarımın ötelenme değerlerini (0,1–0,2 mm) doğrula; birim/raporlama hatası | Developer | ⬜ |
| T1.2 | Denklem, Çizelge, Şekil numaraları ve metin içi atıflar; g8/g9 denklem numarası çakışması | Stylist + Critic | ⬜ |
| T1.3 | Tüm sembol ve denklemler OMML | Stylist | ⬜ |
| T2.1 | Yeni Başlık/Özet/Anahtar kelime (`MakaleMetni.docx`) gözden geçir | Stylist + Critic | ⬜ |
| T2.2 | Yeni Giriş (alt başlıksız) gözden geçir | Literature + Stylist | ⬜ |
| T2.3 | Materyal ve Metot: Çizelge 1 parametreler, APA (kurucu adları metinden çıkar), genel ceza fonksiyonu formülü ekle | Method | ⬜ |
| T2.4 | Bulgular: yapı (3.1→3.5), örnek tanıtımları, B1/C1 grup açıklaması, tablo dipnotları ("Kirişler: C40, Kolonlar: C30"), temiz şekiller | Results + Stylist | ⬜ |
| T2.5 | Yeni Sonuç bölümü gözden geçir | Stylist | ⬜ |
| T2.6 | Kaynakça APA 7; metin ↔ liste eşleşmesi | Stylist + Literature | ⬜ |

## B. Açık kararlar (Master → Kullanıcı)
| # | Konu | Soru | Durum |
|---|---|---|---|
| D1 | Hedef dergi ve dil | **Karar: İngilizce; hedef dergi *Structures* (kullanıcı).** Kapsam/şablon/Data availability kontrolü Aşama 1'de | ✅ |
| D2 | Yazar adı | **Karar: Seda Turan** | ✅ |
| D3 | Program çalıştırma | **ETABS v22 = 2022 teyit edildi (kullanıcı).** **Karar: ETABS v2022 + Visual Studio 2026 kurulu; çalışma kopyasında derleme/çalıştırma izni var; gerekirse Python betiği yazılabilir.** Not: bu makinede `C:\Program Files\Computers and Structures\ETABS 22` görünüyor (sürüm 22; "V2022" ile aynı mı teyit) | ✅ |
| D4 | Yeni koşu | **KARAR: Yol C (hibrit) — optimizasyon boyunca FEM çözücü; ETABS yalnız örnek oluşturma/açılış ve final doğrulamada.** **KOD İNCELEMESİ TAMAM (36 bulgu, 8 kritik): sonuçlar yeniden üretilmeli.** **Karar: tezdeki kodun doğruluğu şüpheli; doğrulama zorunlu, yanlış çıkarsa yeniden üretim/yeniden koşu.** Kod incelemesi sonucuna göre Master yeniden sunar. Çelik/kompozit FEM programının (`SteelFamewithCompositeTubeColumn_FEM`) betonarmeye uyarlanması aday yol (fizibilite: `CODE_REVIEW_v2.md`) | 🔄 |
| D5 | VAO/İVAO | **Karar: elzem değil; odak optimizasyon testi olmayacak.** (Not: İVAO = İyileştirilmiş Vahşi At Optimizasyonu; Gemini'nin yazdığı "Vektör Arama" yanlıştır.) Dahil edilip edilmeyeceği Critic görüşüyle kesinleşir | 🔄 |
| D6 | Fiyat tabanı | **Karar: ST'nin fiyatları kanıtsız; `Steel_Composite_Hybrid_Frame_Optimization/Evidence/UnitPrices` (ÇŞİDB 2019–2026, BD PWD, MahaPWD, BLS) kullanılabilir.** 2024 ÇŞİDB PDF'leri yüklendi; beton, donatı, işçilik, kalıp ve çelik lif pozları görselden doğrulandı (`UNIT_PRICE_EVIDENCE.md`). Kalan: tezin fiyat yılı + lif türü teyidi | 🔄 |
| D7 | Referans örnekler | **Üç örnek yeterli (kullanıcı).** Adaylar: Ö2 = Duysak, Bekdaş, Niğdeli 2023 (PDF mevcut); Ö3 = Tapao & Cheerarot 2017, Eng. Struct. 151:802–820 (`örnek_4 olabilir mi.pdf` bu makale; PDF görüntü tabanlı); Ö1 = DOĞRULANAMADI (makale metninde atıf yok; Tapao 2017'nin atıf yaptığı Kaveh & Sabzi çalışması aday) → Gemini görevi G7. `2022_referans örnek_3.pdf` = Negrin & Chagoyén 2022 (Structures 38), örnek değil karşılaştırma literatürü. Eski soru: Üç referans PDF'ten hangisi hangi örnek (`Referans örneklerin makaleleri/`) ve "örnek_4 olabilir mi" PDF'inin kapsamı | ⬜ |
| D8 | Program hataları | **Karar: kod incelemesi zorunlu.** Rapor: `03_Code_Execution/CODE_REVIEW_v2.md` | 🔄 |

## C. Gemini senkron adımları (kullanıcı)
| # | Adım | Durum |
|---|---|---|
| G1 | Antigravity'de klasör açıldı | ✅ |
| G2 | MCP ayarı eklendi (Gemini: filesystem, sequential-thinking, reference-resolver, hound) | ✅ |
| G3 | Drive masaüstü uygulaması senkron durumunu "güncel" yap; klasörü "Çevrimdışı kullanılabilir" yap | ⬜ |
| G4 | Git: uzak depo `https://github.com/ibrahimaydogdu/fiberRC_Optimization.git`. **Tüm git push/pull/commit işlemlerini yalnız Gemini yapar** (kullanıcı kararı 2026-10-27); Claude git yazma komutu çalıştırmaz | 🔄 |
| G5 | Gemini ilk özetini verdi (2026-10-27) | ✅ |
| G6 | Her oturum sonunda HANDOFF.md güncellenmeden oturumu kapatma | ⬜ |

## D. Gemini görevleri
*Not: Örnek 3, Tapao & Cheerarot 2017 geometrisini kullanır; eleman gruplaması **değiştirilmiştir** (kullanıcı beyanı) — makalede 'geometri Tapao ve Cheerarot (2017)'ten alınmış, gruplama bu çalışmada yeniden tanımlanmıştır' diye yazılır.*
| # | Görev | Durum |
|---|---|---|
| G8 | Senkron testi: `HANDOFF.md`'ye `Gemini` satırı ekle (tarih, 'test okudum/yazdım'), `git status` çıktısını yaz; `git init` + remote henüz yapılmadı | ⬜ |
| G7 | ~~Örnek 1 referansı~~ **Kapandı (2026-10-27):** `Papers/Kaveh2011.pdf` (SHA-256 a83b61ca…82f96c0; 1.958.696 bayt) okundu: örnekleri 3 açıklıklı 4/8/12 katlı çerçeveler (D=22,3, L=10,7 kN/m) → **Örnek 1'in (2 katlı, 6 elemanlı, G=30/Q=20 kN/m) kaynağı değildir.** Kaveh & Sabzi 2012 listeden çıkarıldı (tam metin yok; tezde atıf da yoksa kaynakçaya girmez). **Hakem kararı (kullanıcı yetkisi): Örnek 1 özgün örnek olarak kalır;** Kaveh & Sabzi 2011'in 4 katlı çerçevesi (Tablo 4 sonuçları) kod doğrulaması V2'de lifsiz kıyas verisi olarak kullanılır (makaleye girmek zorunda değil). Örnek 1 için kaynak bulunamadı → makalede 'bu çalışmada tanımlanan örnek' olarak sunulur (ST'ye soruldu) | ✅ |
