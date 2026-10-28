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

## E. WP0 sonrası açık sorular (`WP0_ADAPTATION_MAP.md` §8; kullanıcı kararı gerekir)
| # | Soru | Durum |
|---|---|---|
| S-1 | **KARAR: Ö2 = 3 kat × 3 açıklık, 21 eleman (5-6-5 m; kullanıcı, yanlış işaretlemeyi düzeltti). Makaledeki 'iki açıklık' düzeltilecek.** Eski not — ÇELİŞKİ: kullanıcı '3 kat × 2 açıklık (15 eleman)' seçti; ancak kaynak Duysak, Bekdaş & Niğdeli (2023) PDF'i **3 açıklıklı 3 katlı** çerçeve veriyor (kenar açıklıklar 5 m, iç açıklık 6 m; 30 kN/m ölü, 20 kN/m hareketli) = 21 eleman. Tez de 21 elemanlı model sonuçları veriyor. Öneri: 3×3 açıklık (5-6-5 m); kullanıcı yeniden teyit edecek. Eski not: **Ö2 geometrisi:** makale "2 açıklık 3 kat" diyor ama 21 eleman = 3 kat × **3 açıklık** (2 açıklık 15 eleman verir; Ö1=6 ve Ö3=42 aritmetiği tutuyor — elle doğrulandı). Açıklık sayısı ve Duysak vd. (2023) şeklini kontrol | ⬜ |
| S-2 | **Yük kombinasyonu:** `1.4G+1.6Q` TS 500'e özgü; ACI 318-14'te 1.2D+1.6L. Makale ACI diyor → düzeltilmeli (Ö2 yalnız düşey yük) | ⬜ |
| S-3 | **Karar: RS + TBDY-2018** (ACI 318-14 yalnız kesit tasarımı) | ✅ |
| S-4…S-12 | P-Δ/narinlik, donatı kodlaması, lif/beton sınıfı, ceza, bütçe, kod yapısı (fork-ve-buda mı ince host mu), çatlamış rijitlik, Kaveh Şekil 5 yükleri, EDB→çubuk model | ⬜ |
| H1 | VAO/İVAO makaleden **çıkarıldı** (kullanıcı onayı); algoritmalar YAKO, BTO, ATO | ✅ |
| S-5 | **Karar: donatı iç döngüde en ucuz yeterli düzen** (optimizer kesit/malzeme/lif dozajını arar) | ✅ |
| S-9 | **KARAR: ince yeni host** (FemCore + optimizasyon yöntemleri kütüphane; çelik katman taşınmaz) | ✅ |

## F. Standart kanıtları (2026-10-28)
| # | Konu | Durum |
|---|---|---|
| K1 | ACI 318M-14: 42 satır madde kanıtı (`CODE_PROVISIONS_EVIDENCE.md`); 18.3/18.4 (orta çerçeve) ve 18.7.6 KISMEN | 🔄 |
| K2 | fib Cilt 2 eklendi: `FIBBUL-0056…-First-complete-draft-vol2.pdf` (**Bulletin 56 = MC2010 'first complete draft', nihai/yayımlanmış metin değil**). VRd,F Denklem (7.7-4)/(7.7-5) PDF s.179–180'de var; nihai metinle farkı DOĞRULANAMADI — makalede 'fib MC2010' atfı için nihai metin (Ernst & Sohn 2013) teyidi gerekir | 🔄 |
| K3 | **v2 (`ETABSClass.vb:1061`): lineer model, wu = 1,5 mm SABİT** (fFts=0,45fR1; fFtu=fFts−(wu/CMOD3)(…), CMOD3=2,5). MC2010: lineerde wu = lcs·εFu (≤2,5 mm; εFu=%2), rijit-plastikte wu=CMOD3. 1,5 mm gerekçesi tezde/kodda DOĞRULANAMADI. **Öneri:** yeni kodda MC2010'a göre wu=min(lcs·εFu;2,5); tez yeniden üretiminde (WP5) 1,5 mm ile; fark duyarlılık olarak raporlanır | ⬜ kullanıcı onayı |
| K3b | v2:1072 lif koşulundaki çift bölme: **CODE_REVIEW_v2.md F-13 ile zaten bulunmuş** (yeni bulgu değil); WP5'te doğrulanacak | ✅ (kayıtlı) |
| K4 | Dramix 4D 65/60 BG veri sayfası eklendi | ✅ |
| K5 | TBDY-2018 PDF eklendi; spektrum maddeleri çıkarılacak | ⬜ |
| K6 | Test örnekleri: StructurePoint kiriş + kolon (`TEST_EXAMPLES.md`); SFRC örneği yok | 🔄 |

## D. Gemini görevleri
*Not: Örnek 3, Tapao & Cheerarot 2017 geometrisini kullanır; eleman gruplaması **değiştirilmiştir** (kullanıcı beyanı) — makalede 'geometri Tapao ve Cheerarot (2017)'ten alınmış, gruplama bu çalışmada yeniden tanımlanmıştır' diye yazılır.*
| # | Görev | Durum |
|---|---|---|
| G8 | Senkron testi: `HANDOFF.md`'ye `Gemini` satırı ekle (tarih, 'test okudum/yazdım'), `git status` çıktısını yaz; `git init` + remote henüz yapılmadı | ⬜ |
| G7 | ~~Örnek 1 referansı~~ **Kapandı (2026-10-27):** `Papers/Kaveh2011.pdf` (SHA-256 a83b61ca…82f96c0; 1.958.696 bayt) okundu: örnekleri 3 açıklıklı 4/8/12 katlı çerçeveler (D=22,3, L=10,7 kN/m) → **Örnek 1'in (2 katlı, 6 elemanlı, G=30/Q=20 kN/m) kaynağı değildir.** Kaveh & Sabzi 2012 listeden çıkarıldı (tam metin yok; tezde atıf da yoksa kaynakçaya girmez). **Hakem kararı (kullanıcı yetkisi): Örnek 1 özgün örnek olarak kalır;** Kaveh & Sabzi 2011'in 4 katlı çerçevesi (Tablo 4 sonuçları) kod doğrulaması V2'de lifsiz kıyas verisi olarak kullanılır (makaleye girmek zorunda değil). Örnek 1 için kaynak bulunamadı → makalede 'bu çalışmada tanımlanan örnek' olarak sunulur (ST'ye soruldu) | ✅ |
