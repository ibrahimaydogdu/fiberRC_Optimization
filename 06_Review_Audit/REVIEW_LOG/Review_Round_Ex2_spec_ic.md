# Review Round Ex2 spec (iç Critic) — 2026-11-16

Kapsam: `03_Code_Execution/EX2_SPEC.md` (TASLAK v1), `EX2_OPEN_QUESTIONS.md`; çapraz kontrol için salt-okunur `program_work/FemRcModel/BeamDesign.vb` (BeamShear), `Ex1Evaluator.vb` (maliyet döngüsü), `RcCore/SfrcBeam.vb`, `RcCore/Cost.vb`, `Review_Round_Lif_fiyat_ic.md`. Hızlı inceleme (≈ 20 dk); kod çalıştırılmadı, koşulara dokunulmadı. Kaynak PDF yeniden okunmadı; spec'in alıntıları ve sayfa numaraları denetlenmedi, yalnız aritmetiği denetlendi.

IA'nın onayladığı cevaplar (girdi): Q1 V2 = 5 grup (A), ana koşu = 9 grup (B); Q2 1,2D+1,6L (S-2), kaynak varyantları V2'de raporlanır; Q3 öz ağırlık ayrıca eklenir (IA kararı); Q8 yalnız FEM↔ETABS kapı, kaynak kıyası bilgi amaçlı; Q9 yalnız A kolu (270 koşu).

## Karar

**[KOŞULLU ONAY]** Spec yapı olarak tutarlı ve uygulanabilir. Kaynak analizi (§6, sabit donatı) güçlü ve doğru. Kod yazımı, aşağıdaki **E1–E6 engelleri** spec v2'de kapatılıp IA onayı alınmadan başlamamalı. Koşu protokolü (§11), E1–E3 çözülmeden ön kayda alınmamalı.

## Aritmetik denetimi (yeniden hesaplandı)

| Kalem | Sonuç |
|---|---|
| Kolon donatı sabiti: 389,97 − 0,25·0,30·3,3·240 = 330,57; 401,85 − 0,30·0,30·3,3·240 = 330,57 | DOĞRULANDI |
| Kiriş 5 m: 700,13−105 = 685,13−90 = 760,13−165 = 595,13; 6 m: 796,75−126 = 778,75−108 = 670,75 | DOĞRULANDI |
| Düşey toplam: 30·16·3 = 1 440; 20·16·3 = 960; 1,2·1 440 + 1,6·960 = 3 264 kN | DOĞRULANDI (öz ağırlık hariç; bkz. E4) |
| 54,1 kg / (7 850 · 5) ≈ 1 378 mm² | DOĞRULANDI (etriye dahil kütle; "ortalama boyuna alan" değil, üst sınır okuması) |
| 35⁹ ≈ 7,9×10¹³; ×9³ ≈ 5,7×10¹⁶; 4D 5 + 5D 3 + 0 = 9 seçenek | DOĞRULANDI |

## Bulgular (şiddet sırasıyla)

### KRİTİK

**K1. Lif kesme katkısının güvenlik formatı tutarsız; Ö2'nin ana iddiasını belirler.** Spec §5 "lifin kesme katkısı Ö2'de tam kullanılır (Ö1'den kritik fark)" diyor. Mevcut kod (`BeamDesign.vb` satır 155–165) lifsiz kirişte ACI nominal `Vc = 0,17√fc·b·d`, lifli kirişte fib MC2010 tasarım değeri `VRdF(..., γc = 1,5)` kullanıyor ve ikisini aynı `Ve/φ − Vc` ifadesine koyuyor. Lifli kirişte γc = 1,5 ile φ = 0,75 birlikte uygulanıyor (çift güvenlik). Düşük dozda lifli kirişin "Vc"si lifsizinkinden düşük çıkabilir. Bu sorun Ö1'de ikincildi (`Review_Round_Lif_fiyat_ic.md` §1.2-2), çünkü kiriş kesmesi aktif değildi. Ö2'de kesme aktif kısıt olacak: 25×30 kirişte Vu ≈ 68·5/2·1,15 ≈ 195 kN, φVc ≈ 42 kN. Bu yüzden format doğrudan lif sonucunu belirler. PCR-15'te ya (i) `VRdF` nominal biçimde (γc = 1,0) ve ACI φ ile kullanılmalı, ya da (ii) ölçek farkı belgelenip lehte/aleyhte yönü bir deterministik testle (doz 0'da VRdF ↔ ACI Vc) gösterilmelidir. Karar gerekçesiyle varsayımlar tablosuna girer.

**K2. ACI 318-14'te lifin asgari kesme donatısı yerine geçme koşulu.** ACI 318-14 Tablo 9.6.3.1, SFRC'nin Av,min yerine geçmesine yalnız 26.4.1.5.1(a) ve 26.4.2.2(d) koşullarında izin verir. Bu koşullar arasında doz ≥ 100 lb/yd³ (≈ 60 kg/m³), h ≤ 600 mm ve Vu ≤ φ0,17√fc·bw·d vardır. Bu madde **hafızadan** yazıldı; `CODE_PROVISIONS_EVIDENCE.md`'ye birebir alıntıyla eklenip DOĞRULANMALI. Bizim dozlarımız 20–40 kg/m³; mevcut kod Av,min'i her zaman uyguluyor (satır 156). Bu doğru ve muhafazakâr, ama spec'te açıkça yazılmalı. Sonuç: lif yalnız Av,min üstündeki etriye ihtiyacını azaltabilir. Spec §5 tablosuna ve makalenin varsayım tablosuna girmeli; "lif etriyeyi kaldırır" türü bir beklenti kurulmamalı.

### YÜKSEK

**Y1. Hareketli yük düzenlemesi (pattern loading) tanımlanmamış.** ACI 318-14 6.4.2, döşeme/çatı kirişlerinde en büyük açıklık ve mesnet momentleri için hareketli yükün düzenlenmesini ister (açıklık + bir atlayan açıklık; mesnede komşu açıklıklar). Ö2 yalnız düşey yüklü, sürekli üç açıklıklı bir çerçeve ve L/D = 20/30 oranında büyük. Pozitif açıklık momenti ile kesme bu yüzden belirgin artabilir (tahmin, ölçülmedi). Spec ve Q2 "ACI 318-14 ile tasarım" diyor, ama yalnız tüm açıklıklar yüklü kombinasyon var. Ö1 kodunda da düzenleme yok (`RcFrameBuilder.vb` yalnız G, Q, S desenleri). İki seçenek var: (a) kat başına düzenleme desenleri eklenir (PCR-14/15 kapsamı; FE süresi ≈ desen sayısı kadar artar), ya da (b) yapılmaz ve kaynakla uyum gerekçesiyle sınırlama olarak beyan edilir. **IA kararı gerekir (yeni Q16).** ETABS V2-a/V3'te aynı seçenek kullanılmalı; aksi hâlde ETABS CFD ile FEM farklı zarf okur.

**Y2. Var olan DLL'lerin yeniden derlenme riski (donmuş Ö1 ikilisi).** PCR-15 "bayrak (`DesignMode.Gravity`) veya yeni sınıf" diyor. Bayrak seçeneği `FemRcModel.dll`'i değiştirir. PCR-19 (`PriceSet` varyantı, `Cost.Member` kalıp bayrağı) `RcCore.dll`'i değiştirir. İkisi de Ö1 regresyonunu yeniden açar; DLL SHA uyuşmazlığı (Faz 2/3) zaten açık bir maddedir. Ayrıca Visual Studio'da *proje referansı*, başvurulan projeleri yeniden derler. Öneriler:
- Ex2 ayrı bir çözüm olsun. FemCore/RcCore/FemRcModel'e **dosya referansıyla**, SHA'sı kayıtlı donmuş kopyalardan (`phase3_bin` ile aynı SHA) bağlansın.
- Ö1 kaynak dosyası ve `program_work` DLL'i hiç derlenmesin. Gravite tasarımı Ex2 projesinde yeni sınıf olarak yazılsın (kod çoğaltma kabul edilir).
- V0'da Ö1 ikililerinin SHA-256'sı önce ve sonra yazılsın. Çalışan koşu (Ex1Runner/phase3_bin) dosyalarına derleme çıktısı yazılmasın.
- **PCR-15 bayrak seçeneği silinsin.**

**Y3. PCR-19 gereksiz; Excel ile yapılabilir.** V2-c (beton bedeli 1 936,08 TL) ve V2-d (kaynak fiyatlarıyla bilgi kıyası) kod değişikliği gerektirmez. Bizim optimum tasarımın metrajı (m³, kg, m²) zaten günlükte; `Maliyet_Metraj.xlsx` içinde ayrı bir "kaynak-replika" sayfasında formülle hesaplanır (kural 2'ye de uygun). Ancak V2-d'de "kaynak fiyatlarıyla bizim optimum" koşusu isteniyorsa (optimizasyonun kaynak fiyat setiyle yeniden yapılması), bu yeni bir PriceSet gerektirir. Bu da Ex2 projesinde tanımlanmalı, `RcCore`'da değil (Y2). Öneri: V2-d yalnız deterministik yeniden fiyatlama olsun; kaynak fiyatlarıyla optimizasyon yapılmasın (kaynak donatısı sabit olduğundan anlamı zaten zayıf, §6). Böylece PCR-19 düşer.

**Y4. Ö1 maliyet döngüsünün Ö2'ye taşınamayan varsayımları.** Spec §9 #4 "maliyet döngüsü aynı: `Cost.Member`" diyor, ama `Ex1Evaluator.vb` satır 168–194'te sismik ve döşemeye bağlı varsayımlar var:
- kiriş kalıbı `b + 2(h − SlabThickness)` (0,15 m döşeme düşülüyor). Ö2'de döşeme modellenmiyor, dolayısıyla `b + 2h` olmalı. Kaynak yükünün döşeme içerip içermediği BULUNAMADI; beyan edilmeli.
- kirişte 2h sarılma bölgesi + `SConf`, kolonda `Lk` sarılma + birleşim içi etriye (ACI 18.8.3.1). Bunlar Ö2'de gravite kuralına (ACI 25.7.2 kolon, 9.7.6.2.2 kiriş) dönmeli.
- boyuna donatı grup başına tek `AsTop + AsBot` olarak tüm boy boyunca alınıyor (kesme/bindirme yok). Ö2'de de kabul edilebilir, ama varsayım tablosuna girmeli. 5 m ve 6 m kirişler aynı grupta olduğundan 5 m kirişler 6 m zarfıyla donatılır (aşırı tasarım; (B) gruplamanın sonucu, beyan).
- ACI 9.7.7 yapısal bütünlük donatısı ve 9.7.3.8 süreklilik kuralları spec'te yok; maliyete etkisi küçük, ama "ACI ile tasarlandı" iddiası için değinilmeli (KISMEN uygulandığı yazılır).

### ORTA

**O1. 1,4D kombinasyonu (ACI 5.3.1a) zarfa girmeli.** Bu yüklerde belirleyici olmaz (1,4D > 1,2D+1,6L için D > 160 kN/m gerekir). Yine de kodda zarfa konmalı ya da "belirleyici değil" satırı [Ç] eklenmeli. Çatıdaki Q = 20 kN/m, ACI'de Lr mı L mi? 1,2D+1,6L ile L sayılması muhafazakârdır; beyan edilmeli.

**O2. V1 kontrol büyüklüğü Q3 kararıyla çelişiyor.** §3 ve §10 V1 "reaksiyon toplamı = 3 264 kN (öz ağırlık hariç)" diyor; Q3 kararıyla öz ağırlık ayrıca eklenir. V1 ölçütü iki parça yazılmalı:
- G deseni toplamı = 1 440 + γ·ΣV_eleman (kolon ve kirişte uç ofseti/net boy kuralı ETABS ile aynı);
- 1,2D+1,6L toplamı buna göre hesaplanır.

Öz ağırlık ETABS'te `SelfWeightMultiplier = 1` ile verilmeli, FEM'de çift sayım olmamalı (Ö1 testi F4 deseni örnek).

**O3. Kesit yönelimi tanımsız.** Kolon için b, h her ikisi 250–600 aralığında, ama hangi boyutun çerçeve düzleminde (eğilme ekseni) olduğu spec'te yazılı değil. Tablo 1'deki "25×30" b×h ise h düzlem içinde olmalı. `RcFrameBuilder2D` ve ETABS modeli aynı kurala bağlanmalı. Aksi hâlde V2-a'da sahte fark çıkar.

**O4. Narinlik/yanal ötelenme sınıfı.** Spec Q ≤ 0,05 kontrolünü kaldırıp δns'yi tutuyor. Yanal yük yokken ACI 6.6.4.3 Q indeksi hesaplanamaz; çerçeve "ötelenmesiz" kabul edilmiş olur. Bu bir varsayımdır: yanal tutulma kaynağı yok, perde yok. Varsayım tablosuna girmeli; ETABS CFD'de "Sway/Nonsway" ayarı aynı seçilmeli (V2-a).

**O5. Servis kısıtları.** Sehim kontrolü yok. ACI 318-14 Tablo 9.3.1.1 min. yükseklik: 5 m kenar açıklık (bir ucu sürekli) L/18,5 = 270 mm, 6 m iç açıklık L/21 = 286 mm. Havuz alt sınırı h = 300 mm her ikisini karşılar [Ç]. Bu satır eklenirse "sehim kontrolü yapılmadı" eleştirisi kapanır (≥ 2 m kirişler için de geçerli). Çatlak kontrolü (24.3) yok; beyan edilmeli.

**O6. Q9 (yalnız A) ile yöntem anlatısı.** Proje vizyonu (MEMORY §3h) FSD ön tasarımını "çekirdek rehber" olarak tanımlıyor. Ö2'nin yalnız rastgele başlangıçla (A) koşulması savunulabilir: küçük uzay ve Ö1'de A/C/ABC birleşik farkının olmaması. Ama makalede tek cümleyle açıklanmalı: "Ö2'de ön tasarım kullanılmadı; Ö1'de ön tasarımın birleşik etkisi anlamlı değildi". Ö1 ana serisi hangi kolsa Ö2 ile karıştırılmamalı. Ö1'in ön tasarım bulguları Ö2'ye genellenemez; tersi de geçerli.

**O7. V3'te lifli kiriş yöntemi tanımsız.** "Ö1 V3 yöntemine bağlı" deniyor, ama Ö1 V3 henüz yapılmadı. Ö2'de kesme aktif olacağından (K1), ETABS'in lif katkısını görmemesi lifli optimumda kesme D/C > 1 verecektir. Lifli V3'ün ölçütü kod yazılmadan önce yazılı olmalı. Önerilen yöntem: ETABS eğilme/kesme talebi okunur, kapasite elle (fib) hesaplanır, Excel'de D/C yazılır. Aksi hâlde lifli tasarımlarda N/A riski doğar (kural 3).

### DÜŞÜK

- **D1.** Lif kesme katkısı ve Av,min. Kaynak sonucunda kolonlarda min. donatı kullanılmış ("minimum donatı oranları", PDF s.5). Bizim ρ ≥ %1 alt sınırı, ACI 10.6.1.1 ile uyumlu.
- **D2.** §12 "Yapılamaz iddialar" listesi yeterli. Şu eklenmeli: "Ö2'de lif ekonomiktir" ifadesi, K1/K2 kararı ve fR veri kaynağı (gizli üretici) beyan edilmeden yazılamaz. Ö1 (lif = 0) ile Ö2 sonucu, rejim farkıyla açıklanmadan yan yana konmaz.
- **D3.** Süre tahmini (3–12 dk/koşu) ölçülmemiş; spec bunu zaten şart koşuyor. 100 FE ölçümü ön kayda yazılsın.
- **D4.** Kaynak PDF `Papers/` altına kopyalanmadı (Q14). Makaleye girmeden önce kapatılmalı (kural 11), kod için engel değil.
- **D5.** "Gebrail" (kaynakta Bekdaş) yazar adı ve tez/MM metin düzeltmeleri (Q13): yazım aşaması.

## Sorulara cevap

1. **İç tutarlılık / uygulanabilirlik:** Büyük ölçüde tutarlı. Dört çelişki/eksik var: V1 öz ağırlık (O2), kesit yönelimi (O3), maliyet döngüsünün "aynı" sayılması (Y4) ve "lif kesme katkısı tam" ifadesi (K1/K2). Kod yapısı (yeni Ex2 sınıfları, `Optimizers.Run` aynen) uygulanabilir.
2. **PCR-14…19:**
   - PCR-14, 16, 17, 18 yeterli; hepsi yeni dosya, risk düşük.
   - PCR-15: bayrak seçeneği silinmeli (Y2). Kesme formatı (K1), Av,min kuralı (K2) ve pattern kararı (Y1) eklenmeli.
   - PCR-19 düşmeli ya da Ex2'ye taşınmalı (Y3).
   - Donmuş Ö1 ikilisi yalnız dosya-referansı + SHA kaydı + `program_work` derlenmemesi ile korunur.
3. **IA cevaplarının savunulabilirliği:**
   - Q1 (5/9 grup): savunulabilir; (A) ⊂ (B) doğru. Makalede tezin 6 grubuyla fark ve 5 m/6 m kirişlerin aynı grupta olduğu yazılır.
   - Q2 (1,2D+1,6L): savunulabilir, ama ACI tutarlılığı için 1,4D ve pattern kararı gerekir (Y1, O1). Makalede kaynağın kombinasyon vermediği ve MM'deki "1,4G+1,6Q" ifadesinin düzeltildiği yazılır.
   - Q3 (öz ağırlık ayrıca): muhafazakâr ve savunulabilir. Varsayım tablosunda "[K] IA kararı, kaynakta BULUNAMADI" etiketiyle yazılır. V2-b'de "dahil" varyantı bilgi olarak raporlanır.
   - Q8 (yalnız FEM↔ETABS kapısı): doğru, §6 ile gerekçeli. V2-a eşikleri sayısal olarak ön kayda yazılmalı. "validated against Duysak et al." yazılamaz.
   - Q9 (yalnız A, 270 koşu): savunulabilir (O6 cümlesiyle). 0 TL/kg senaryosu lifin teknik etkisini ayırdığı için yararlı.
4. **Atlanan kalemler:**
   - Yük/kombinasyon: pattern loading (Y1), 1,4D (O1), çatı Lr (O1).
   - Kısıt: Av,min/SFRC (K2), sehim min. yükseklik (O5), ötelenmesiz varsayımı (O4), yapısal bütünlük (Y4).
   - Birim: kN/m ↔ N·mm zinciri `FrameSpec2D`'de açıkça yazılmalı; V1 toplamı bunu yakalar.
   - Maliyet: kaynak sabit donatı sorunu spec'te doğru çözülmüş (kapı değil, bilgi). Kalıp/sarılma/döşeme varsayımları Ö2'ye uyarlanmalı (Y4). V2-d yüzde farkı başlık sayısı olarak kullanılmamalı, çünkü kaynağın donatısı tasarım değil sabit.
5. **Kod öncesi engeller (spec v2'de kapatılacak):**
   - **E1** K1 kesme güvenlik formatı kararı.
   - **E2** K2 ACI Tablo 9.6.3.1 maddesinin kanıtla doğrulanması ve Av,min kuralının yazılması.
   - **E3** Y1 pattern loading kararı (Q16, IA).
   - **E4** O2 V1 ölçütünün öz ağırlıkla yeniden yazılması + O3 kesit yönelimi.
   - **E5** Y2 derleme yalıtımı (dosya-referans, SHA, PCR-15 bayrak silme, PCR-19 düşürme).
   - **E6** Y4 Ö2 maliyet kuralları (kalıp b + 2h, gravite etriye boyları) ve O7 lifli V3 ölçütü.

## AUDIT_CHECKLIST

| Madde | Durum |
|---|---|
| Sayı–kaynak eşleşmesi | EVET (spec aritmetiği yeniden hesaplandı); kaynak alıntıları yeniden okunmadı |
| ACI 9.6.3.1/26.4.2.2 SFRC maddesi | HAYIR (hafızadan; kanıt dosyasına girmeli) |
| Sürüm izlenebilirliği | KISMEN (Ex2 derleme yalıtımı tanımlanmalı) |
| İddia sınırları | EVET (§12 yeterli, D2 eki) |
| ETABS V3 | HAYIR (yapılmadı; lifli V3 yöntemi tanımsız) |
