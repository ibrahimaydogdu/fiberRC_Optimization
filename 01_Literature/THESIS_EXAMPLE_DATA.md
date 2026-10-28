# THESIS_EXAMPLE_DATA.md — Tezde yazan örnek verileri (DOĞRULANMAMIŞ; "teze %100 güvenme")

Çıkaran: alt ajan (2026-10-28), Claude kısmen kontrol etti. Statü sütunu: **TEZDE YAZAN** = tezden/ilgili dosyadan okundu, standarda/kaynağa bağlanmadı. Kısaltma: TEZ = `Sources/Makale_Metni/SEDA TURAN_DOKTORA_TEZİM_01_08_2024_imzalı.pdf` (basılı sayfa = PDF − 17); MM = `MakaleMetni.docx`; N&C = Negrin & Chagoyén (2022), Structures 38:64–75 (`Sources/tez_sonuç tablo grafik/Referans örneklerin makaleleri/2022_referans örnek_3.pdf`).

## Kritik bulgu (Claude kısmi kontrol)
Tez Örnek 3'ü "Tapo vd. 2017" ile anıyor (TEZ s. ~50, Şekil 4.19; satır 3422), ama `referans örnek_3` dosyası **N&C 2022**'dir ve gruplar (C1–C3, B1–B4 = 7 grup) onunla uyuşuyor. ETABS dosya adı "Ivan_A_Nergin_Ernesto_L_Chagoyen_2022" (ajan raporu). Tapao & Cheerarot 2017'nin 9 grubu tezdeki 7 grupla eşleşmiyor. → Örnek 3'ün kaynağı büyük olasılıkla **N&C 2022**; geometri/yük N&C sayfalarından teyit edilecek (Claude, sıradaki iş). Makaledeki "Tapao 2017 geometrisi" ifadesi düzeltilmeli.

## Örnek 1 (2D, 6 eleman) — tez yeni örnek kuruyor, dış kaynak gösterilmiyor
| Değer | Tezde yazan | Kaynak | Not |
|---|---|---|---|
| Sistem | 2 kat, tek açıklık, 6 eleman | Şekil 4.13 (TEZ s.39) | |
| Açıklık / kat yüksekliği | 5 m; 4 m (zemin) + 3 m (üst) | Şekil 4.13 etiketleri | **MM §3.1.1: 6 m açıklık, 3,0 m kat → ÇELİŞKİ** |
| Yük | G = 30 kN/m, Q = 20 kN/m | TEZ s.38 | |
| Kombinasyon | 1,4G+1,6Q; 0,9D∓E; D+Q∓E (Çiz. 4.5, s.39) | TEZ | **MM: ACI 318-14, 1,2G+1,0Q±1,0E → ÇELİŞKİ** |
| Kesit havuzu b / h | 250…600 mm / 225…1000 mm | Çiz. 4.5 | h=225 yazım hatası olabilir |
| Donatı | Ø 6/10/12/…/32; adet 1–10 | Çiz. 4.5 | |
| Lif türü, dozaj, beton | tür 2/3; 20–40 kg/m³ (5'er); C30/C40 | Çiz. 4.5 | |
| Gruplar | 4 grup: 1. kat kirişler, 2. kat kirişler, 1. kat kolonlar, zemin kat kolonlar | Çiz. 4.6/4.7 | kolon havuzu ayrı yazılmamış (BULUNAMADI) |
| Deprem | DD-2; SDS = 1,2; SD1 = 0,3; I = 1; D = 3; R = 8 | TEZ s.38 | zemin sınıfı, TA/TB/TL, spektrum BULUNAMADI; "TBDY/ASCE" kelimesi tezde yok |
| Ötelenme sınırı | kod varsayılanı 1/300 (displimit 0,0636) | v2 `MainForm.Designer.vb` | Ö1 için kullanıldığı BULUNAMADI |
| fc, fy, E | **BULUNAMADI** (Ö1 için); S420 yalnız Ö2/Ö3 (s.43, 47) | | |
Kesit gösterimi tutarsız: Çiz. 4.6 "70X25/55X30" birimi ve sırası belirsiz; lifsiz kiriş h = 70 cm, lifli 35 cm.

## Örnek 3 (3D, 42 eleman)
| Değer | Tezde yazan | Kaynak | Not |
|---|---|---|---|
| Plan | x: 5 m, y: 6 m açıklık; 2 kat | TEZ s.47; N&C Şekil 1 | eleman sayısı 42 çıkarımı doğrulanmadı |
| Kat yüksekliği | metinde 3,3 m; Şekil 4.20 ve N&C Şekil 1: 3,5 m | | **ÇELİŞKİ** |
| Malzeme | C30; S420 (fy = 420 MPa); N&C: E = 220 GPa(!) | TEZ s.47; N&C Tablo 2 | E = 220 GPa standart dışı: N&C'de teyit |
| Gruplar | C1 iç, C2 kenar, C3 köşe; B1 x-iç, B2 x-kenar, B3 y-iç, B4 y-kenar | TEZ s.47; MM | tez metni bir yerde "orta kenar" diyor, MM farklı |
| Kiriş havuzu | b = 200/300/350/400/450/600; h = 300…600 | Çiz. 4.15 s.48 | metinle çelişkili (b 200–400, h 250–600, 50 mm) |
| Kolon havuzu | tezde ayrı tablo YOK; sonuçlarda tümü "25X25" | Çiz. 4.17/4.18 | N&C kolon aralığı 250–500 mm; tez 250'ye yapışmış |
| Yük | G 4,80 (1. kat) / 5,43 (çatı) kN/m²; Q 4,00 / 0,80 kN/m²; rüzgâr 0,92 (0–5 m), 1,01 (7 m) kN/m² | Çiz. 4.16; N&C Tablo 1 | |
| Kombinasyon | 1,2D+1,6L; 1,2D+0,8W; 1,2D+1,4W+0,5L; 0,9D+1,4W; rüzgâr x ve y | N&C | **Örnek 3'te DEPREM YOK, yalnız rüzgâr** |
| Eğilme yönleri | ±X ±Y tanımı BULUNAMADI; rüzgâr her iki eksende → kolonlar iki yönlü eğilme alır | N&C | `AciColumn.BiaxialDcr` gerekli (Hakem kararı) |
| Sonuç tabloları | Çiz. 4.17 "N/A", "NA"; Çiz. 4.18 MKÖ 0,13–0,24 mm / MEÜKÖ 0,10–0,20 mm (MKÖ > MEÜKÖ, şüpheli) ↔ lifsiz 6,6–7,1 mm | | AGENTS §2.3 ihlali; yeniden hesap |

## TBDY-2018 (yalnız Örnek 1; Ö2'de deprem yok, Ö3'te rüzgâr)
Tezde: DD-2, SDS = 1,2, SD1 = 0,3, I = 1, D = 3, R = 8. **BULUNAMADI:** zemin sınıfı, Ss/S1, TA/TB/TL, spektrum dosyası, DTS/BYS, ETABS spektrum tanımı. WP0 haritası TA = 0,05 s, TB = 0,25 s hesabını [ÇK] (çıkarım) etiketliyor. Tez TBDY'yi atıf etmiyor; spektrumun TBDY mi ASCE biçimi mi olduğu kanıtlanamadı. ACI Bölüm 18 detayları (D = 3, R = 8 ile birlikte) uygulanıyor mu belirsiz.

## Sonuç — makale için etkisi
1. Örnek 3'te deprem yok → **TBDY yalnız Ö1 (ve istenirse Ö2) için gerekli**; Ö3 rüzgâr+yerçekimi. Bu, TBDY çıkarımı işini küçültüyor.
2. Örnek 3 kaynağı N&C 2022 olarak düzeltilecek (kaynakça + metin); 7 grup / 9 grup çelişkisi çözülmüş olur.
3. Ö1 için geometri (5 m / 4+3 m mi, 6 m / 3+3 m mi), fc/fy/E ve kolon havuzu kullanıcıdan/EDB'den teyit.


## CLAUDE DOĞRULAMA — Örnek 3 kaynağı (2026-10-28, sayfa görüntüsünden)
Kaynak: Negrin & Chagoyén (2022), *Structures* 38, 64–75, `Sources/tez_sonuç tablo grafik/Referans örneklerin makaleleri/2022_referans örnek_3.pdf` (2 919 438 bayt; SHA-256 76f7e35b15cfb7392b631c24b104fd7b704df3c56c78b086843a6591bc27747a; 12 sayfa). PDF 2 = s.65, PDF 3 = s.66.
| Kalem | N&C 2022 (görüntü) | Tezle karşılaştırma |
|---|---|---|
| Sistem | 3 boyutlu iskelet betonarme yapı (Şekil 1a), 2 kat; x yönü 5 m, y yönü 6 m açıklık; kat yüksekliği **3,5 m** (Şekil 1b) | Tez metni 3,3 m diyor; **3,5 m doğru** (şekil + N&C) |
| Gruplar | B1–B4 (kiriş), C1–C3 (kolon) = 7 grup, 12 ayrık değişken | Tezdeki 7 grupla **uyuşuyor** (Tapao'nun 9 grubu değil) |
| Yükler (Tablo 1) | Ölü 4,80 (kat) / 5,43 (çatı) kN/m²; hareketli 4,00 / 0,80; rüzgâr 0,92 (0–5 m), 1,01 (7 m) kN/m² | Tezle **aynı** |
| Kombinasyon (Denk. 1–4) | 1,2D+1,6L; 1,2D+0,8W; 1,2D+1,4W+0,5L; 0,9D+1,4W; W hem x hem y → altı kombinasyon | Deprem **yok** |
| Değişken aralıkları (Tablo 3) | B1,B2: h 300–600, b 200–400; B3,B4: h 250–500, b 200–400; C1–C3: x ve y 250–500 mm; fc kirişte ve kolonda 25–40 MPa (5 MPa adım); boyutlar 5 cm katı | Tez kiriş havuzunu (b 450/600?) bununla çelişkili aktarmış; **N&C aralıkları esas alınacak** |
| Malzeme | Çelik fy = 420 MPa, E = 220 GPa (Tablo 2 notu); ACI 318-14 tasarım; SAP2000 API + MATLAB | E = 220 GPa standart dışı (N&C'nin kabulü), tezde 200 GPa kullanılmış olabilir → ETABS modelinde kontrol |
| Maliyet | N&C kendi birim maliyetleri (USD); biz ÇŞİDB 2026 ile yeniden hesaplarız | |
**Sonuç:** Örnek 3'ün kaynağı N&C 2022; makale ve kaynakçada "Tapao & Cheerarot (2017)" atfı düzeltilecek. Örnek 3'te kolonlar iki yönlü eğilme alır (açıklıklar x ve y'de farklı → yerçekimi ve rüzgâr iki eksende moment üretir) → `AciColumn.BiaxialDcr` gerekli (Hakem kararıyla uyumlu). Eleman sayısı 42 çıkarımı: 2×2 açıklık, 9 kolon × 2 kat = 18 kolon + 24 kiriş (Şekil 1a/1b ile uyumlu; ETABS modelinden teyit edilecek).
