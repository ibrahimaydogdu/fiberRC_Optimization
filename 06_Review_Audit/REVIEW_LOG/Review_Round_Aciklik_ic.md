# Review Round Açıklık duyarlılığı (iç Critic) — 2026-11-16

Kaynaklar: `01_Literature/THESIS_EXAMPLE_DATA.md` (Ö2/Ö3 geometrisi), `program_work/FemRcModel/RcFrameBuilder.vb` (FrameSpec), `Ex1Problem.vb` (havuz), `Ex1Runner/Program.vb`, önceki tur raporu `Review_Round_Lif_fiyat_ic.md`. Salt-okunur; koşu yok.

## Doğrulanan teknik durum
- `FrameSpec.Lx/Ly` (5,0 m) parametre olarak var, ancak Runner `New FrameSpec` ile varsayılanı kullanıyor; komut satırı bayrağı yok. Açıklık değiştirmek = Runner'da küçük kod değişikliği, yani **PCR + IA onayı** (AGENTS kural 7); lifsiz referans, ön tasarım (FSD) ve kutu kapsama testi her açıklık için yeniden kurulmalı.
- Kesit havuzu sabit: kiriş h 300–800 mm, b 250–500 mm; kolon 300–700 mm. 10 m açıklıkta h/L = 1/12,5 sınırında; 8–10 m'de havuzun kendisi kısıt koyabilir (havuz değişirse PCR + tüm referanslar yeniden).
- Ö2 (2B, yalnız yerçekimi) ve Ö3 (N&C 2022: 5 m × 6 m açıklık, 2 kat, rüzgâr; kat 3,5 m) açıklıkları 5–6 m; Ö1 5×5 m. Üç örnek de aynı açıklık bandında; hiçbirinde "kiriş dayanımı bağlayıcı olur" rejimi test edilmiyor.

## Soru 1: ihtiyaç var mı?
Evet, ama "zorunlu" değil, "hakem sorusunu karşılayan en ucuz kanıt" olarak. Gerekçe: lif=0 bulgusu aktif kısıtların kiriş dayanımı dışında (Q, ACI 18.8.2.3, güçlü kolon, birleşim) olmasından geliyor. Hakem "bu geometri lif için uygunsuz seçilmiş" diyecek. Açıklık, kiriş dayanım/kesme talebini artıran, lifin tek kaldıraç noktasına (kiriş As/orta bölge) en doğrudan etki eden parametre. Ancak model yapısı (sarılma bölgesinde lif=0, Mpr'ye lif ekleme, VRd,F formatı; önceki rapor Bölüm 1.2) nedeniyle büyük açıklıkta da lif kazancı garantili değil; çalışma hem "lif devreye girer" hem "girmez" çıkabilir, ikisi de bilgilendirici.

## Soru 2: bu çalışma olmadan değer ne?
- Tek başına "lif etkisiz" sonucu: hakem için **yetersiz** (negatif sonuç, yalnız tek geometri, tek fR kaynağı, tek fiyat). Yine de dürüst raporlanırsa kabul edilebilir bir **mühendislik bulgusu**: "ACI 18 kapasite tasarımlı, kolon/birleşim/stabilite sınırlı konut çerçevesinde SFRC kirişin yapısal kazancı sıfır; kırılma fiyatı ≤ 0" (kırılma fiyatı (a) + (b0) ile verilir).
- Özgün değer yine de ağırlıkla yöntem tarafında: FEM-ETABS bütünleşik, FSD ön tasarımlı, TBDY/ACI zincirli lifli/lifsiz çerçeve çerçevesi ve lif etkisinin hangi kısıta bağlı olduğu tanısı. Açıklık çalışması bu tanıyı "kısıt rejimi haritası"na çevirir, makaleyi güçlendirir ama olmadan makale savunulabilir kalır (Structures düzeyi için sınırda).
- Ö2/Ö3 açıklıkları (5–6 m) 2 katlı, düşük yükte kiriş dayanımı bağlayıcı olabilir (özellikle Ö3 hareketli yük 4 kN/m², rüzgâr); bunlar zaten lif-kiriş rejimini test eden doğal örnekler. Hakem Ö1'in lif=0 sonucunu Ö2/Ö3'teki sonuçla birlikte okuyacak: Ö2/Ö3'te lif seçilirse "kısıt rejimine bağlı" mesajı açıklık çalışması olmadan da verilebilir. Bu iki örneğin lifli sonuçları henüz bu projede koşulmadı; **önce onlara bakmak** daha ucuz ve daha az post-hoc.

## Soru 3: seçenekler

| Seçenek | Koşu süresi (10k FE, 8 çekirdek) | Risk | Karar |
|---|---|---|---|
| (A) Yapılmaz; lif=0 bulgusu + (a) + (b0) + Ö2/Ö3 sonuçları + sınırlama cümlesi | 0 ek | Hakem "tek geometri" itirazı; kapsam sınırı yazılmalı | Ö2/Ö3 lif sonucu lif>0 verirse **yeterli**; hepsi lif=0 ise (B) |
| (B) Makalede kısa ek analiz: 2 açıklık (örn. 8 ve 10 m), tek yöntem (BTO tüm-boyut A kolu), lifsiz+lifli (fiyat 90, fR 1,0), n=10 | 2 açıklık × 2 senaryo × 10 koşu = 4 hücre ≈ 4 × 1,6 = **6,4 saat** + FSD referansı/kutu kontrolü ≈ +1–2 saat; kodlama + regresyon ≈ yarım gün | PCR gerekir (Runner bayrağı; havuz 10 m için yeterliliği kontrolü); post-hoc etiketi; küçük şişme (1 tablo + 1 paragraf); tek yöntem, "algoritma yarışı" yok | **Öneri (koşullu)** |
| (C) Ayrı çalışma (ikinci makale): 5–10 m, 6 açıklık, çok yöntem, fiyat ve fR taraması, ETABS V3 | ≈ 3 açıklık+ × 4 hücre × 30 koşu ≈ 50+ saat + kod/ETABS | Bu makalenin kapsamını şişirmez; ancak kod modeli (Mpr'de lif, sarılma bölgesi) varsayımları önce netleşmeli | Yalnız (B) lif>0 gösterirse |

## Karar ve öneri
1. **Şimdi açıklık koşusu başlatılmasın.** Sıra: (a) + (b0) → Ö2/Ö3 lifli/lifsiz sonuçları → ancak üçünde de lif yararsızsa (B).
2. (B) yapılırsa ön kayıtlı olmalı: açıklıklar (8, 10 m), n=10, tek yöntem, başarı ölçütü "lif dozu >0 koşu ≥ 5/10 veya medyan lifsizden ≥ %1 aşağıda", sonuç ne olursa raporlanır; PCR (Lx/Ly bayrağı) IA onayıyla; DLL yeni sürüm olacağından Ö1 5 m sonuçları regresyonla (bit-bit) korunur (`phase3_bin` ↔ yeni ikili).
3. (C) bu makalenin parçası değil; "Gelecek çalışma" cümlesi yeter ("lifin avantajı kiriş dayanımının bağlayıcı olduğu uzun açıklıklarda incelenebilir").
4. Makalede açıklık çalışması yapılmasa da şu cümle şart: lif sonucu 5×5 m açıklık, 4 kat, ACI 18 kapasite tasarımı ve belirtilen model varsayımlarına özgüdür.

Sınır: bu rapor Ö2/Ö3 lifli koşu sonuçlarını görmedi (projede henüz yok); karar bu nedenle koşullu. V3 ETABS + S2 + IA onayı şartı geçerli.
