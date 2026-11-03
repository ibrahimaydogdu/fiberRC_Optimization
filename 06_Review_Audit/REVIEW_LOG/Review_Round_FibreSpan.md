# Review_Round_FibreSpan: "Lif avantajsız çıktı; açıklığı büyütelim mi?" (Critic, 2026-11-01)

Karar biçimi: **[REVİZYON TALEBİ]**. Açıklığı büyütmek şu an önerilmez; önce modelin içindeki bir yapı sorunu (aşağıda B1) giderilmeli. Bu sorun giderilmeden "lif %2–3 pahalı" cümlesi makaleye girmez.

Okunan dosyalar: `06_Review_Audit/CRITIC_PROMPT.md`, `04_Results/campaign_stats.md`, `04_Results/campaign/*.csv` (360 dosya; satır 3 = sonuç, satır 4 = x vektörü, ben toplulaştırdım), `01_Literature/UNIT_PRICE_EVIDENCE.md`, `UNIT_PRICE_NEEDS.md`, `THESIS_EXAMPLE_DATA.md`, `03_Code_Execution/program_work/FemRcModel/{Ex1Problem,BeamDesign,Ex1Evaluator}.vb`, `RcCore/Cost.vb`. `Checks.vb` yalnız grep ile bakıldı. `PRICES_2026.md` ve ETABS çıktıları okunmadı. Kod ve Sources/Evidence'a dokunulmadı.

## 0. En önemli bulgu (B1): lifli senaryo lifsizin üst kümesi, ama daha pahalı çıkıyor

- `Ex1Problem.vb` satır 8–9, 45–46: lifli modda her kiriş grubunda lif tipi ve doz indeksi serbest, doz indeksi 0 = lifsiz. Yani **lifsiz tasarım lifli arama uzayının içinde**. Gerçek optimumda lifli maliyet ≤ lifsiz olmak zorundadır.
- Kampanya bunun tersini veriyor (`campaign_stats.md` satır 21–35): BTO en iyi lifsiz 1 390 219, lifli (1,00) 1 445 798 TL; ATO 1 464 873 vs 1 511 190.
- Kampanya çıktılarından (benim toplulaştırmam, 30 koşu ortalaması, 4 kiriş grubu × 30 koşu = 120 grup):

| Yöntem / senaryo | doz=0 olan grup payı | ort. lif maliyeti (TL) | ort. toplam maliyet (TL) |
|---|---|---|---|
| BTO lifli ×1,00 | 108/120 (%90) | 7 934 | 1 485 153 |
| BTO lifli ×0,70 | 111/120 (%92) | 5 535 | 1 480 383 |
| BTO lifsiz | 120/120 | 0 | 1 452 860 |
| ATO lifli ×1,00 | 89/120 (%74) | 21 335 | 1 577 475 |
| YAKO lifli ×0,85 | 64/120 (%53) | 46 655 | 1 669 656 |

- Yorum: BTO'nun lifli koşularında kirişlerin ~%90'ı fiilen lifsiz, lif maliyeti toplamın %0,4'ü ve yine de ortalama +%2,2 pahalı. Fark lifin kendisinden gelmiyor; **arama uzayı büyüdüğü (+8 değişken: tip, doz) ve sonlu bütçe (10 000 FE) sabit kaldığı için** yakınsama kalitesi düşüyor. Mann–Whitney'deki "anlamlı fark" bir algoritma-verimlilik farkıdır, lif etkisi değildir.
- fR çarpanlarının (1,00/0,85/0,70) maliyeti değiştirmemesi de aynı nedenle beklenen bir şey: çoğu kirişte lif zaten yok.
- Sonuç: mevcut kampanya "lif ekonomik değil" iddiasını **destekleyemez**; "10 000 FE bütçesiyle genişletilmiş uzay daha kötü aranıyor" diyebilir. Bu, metodolojik bir kusur olarak hakem tarafından hemen yakalanır.

## 1. Karar tablosu

| # | Soru | Karar / öneri | Risk | Gerekçe | Kanıt |
|---|---|---|---|---|---|
| 1 | Sonucu görünce açıklık/yük değiştirmek | **Ö1'i sessizce değiştirme.** Yalnız şu koşullarda savunulur: (i) değişiklik gerekçesi sonuçtan bağımsız yazılır, (ii) orijinal Ö1 sonucu da tam raporlanır, (iii) açıklık tek bir yeniden seçilmiş değer değil **L tarayan önceden tanımlı parametrik çalışma** olarak sunulur (ör. 5, 6, 7, 8 m), (iv) tüm L'ler için sonuç raporlanır, negatifler dahil | Yüksek (HARKing / seçici raporlama; Structures hakemi "açıklık lif lehine olana kadar ayarlandı" der) | Tek bir "lifin kazandığı" açıklığı seçip sunmak, hipotezi sonuçtan sonra kurmaktır. Tarama + negatif sonuçların raporu aynı bilgiyi dürüst verir | `campaign_stats.md` 21–35; MEMORY 3e (Ö1 kilidi: 5×5 m) |
| 2a | Negatif sonucu raporla + açıklık duyarlılığı ek analiz | **Önerilir, ama B1 düzeltildikten sonra.** Şu anki negatif sonuç raporlanmaya hazır değil | Orta | Negatif sonuç ancak aramanın lif lehine sınırı gerçekten yokladığı gösterilirse bilgi taşır | §0 |
| 2b | Lifin gerçek mekanizmaları | Koddan: lif katkısı **yalnız orta bölge kesmesi** (`BeamDesign.vb` 154–156, VRdF) ve eğilmede. Sarılma bölgesinde lif katkısı 0 (147). Sarılma bölgesi etriye aralığı ACI 18.6.4.4 detaylandırma sınırıyla (d/4, 6db, 150 mm; satır 152) belirleniyor, kesme talebiyle değil: lif burada etriye azaltamaz. Dolayısıyla modelde lifin tasarruf kanalı yalnız eğilme donatısı ve orta bölge etriyesi; ikisi de küçük kalemler | Orta | Rebar ≈ 0,77 M TL, formwork ≈ 0,46 M TL, beton ≈ 0,25 M TL (BTO ort., csv satır 3); etriye payı ayrı raporlanmıyor. Optimumda boyuna donatı/etriye/kesit ayrımı henüz kampanya çıktısında yok | `BeamDesign.vb` 147–156; csv sütunları |
| 2c | Muhafazakâr varsayım / yapı hatası | (1) **Mpr'de lif katkısı var** (γF=1, satır 111–116): lif eğilme kapasitesini artırdıkça Mpr ve dolayısıyla kapasite tasarımı kesme talebi Ve ve güçlü-kolon ΣMnc ≥ 1,2ΣMnb talebi artar; lif, kirişi güçlendirip kolonu ve etriyeyi cezalandırıyor. Bu fiziksel ama IA'nın gözden geçirmesi gereken bir tasarım tercihi (lif katkısı Mpr'de dahil mi?). (2) Sarılma bölgesinde lif katkısı 0 (kod yorumu "Critic kararı bekliyor"): bu kararı ben burada veriyorum, bkz. aşağı. (3) Lifli betonda beton fiyat farkı yok (Cost.vb 59–62: beton fiyatı yalnız fc'ye bağlı; lif ayrı kalem). Pompa, karışım ve işçilik farkı modelde yok → **lif lehine** yanlı. Yani maliyet tarafı lifli senaryoyu zaten kayırıyor, bu yönde muhafazakâr değil. (4) Doz uzayı 20–40 kg/m³, doz ızgarası dışı değerler en yakın ölçülen noktaya onarılıyor; onarma, doz indeksini tek-değerli olmayan (düz bölgeli) bir arazi haline getirir, aramayı zorlaştırır | Orta | Karar (2): sarılma bölgesinde lif=0 muhafazakâr varsayımı **kalabilir**, ancak makalede duyarlılık olarak (lif katkısı açık/kapalı) tek ek koşuyla gösterilmeli; gizlenmemeli | `BeamDesign.vb` 111–116, 147; `Ex1Problem.vb` 45–46; `Cost.vb` 55–64 |
| 2d | Hangi örnek lifi test eder | **Ö1'e dokunma.** Ö2 (yalnız yerçekimi, 2B 21 eleman) ve Ö3 (yerçekimi + rüzgâr, Negrin & Chagoyén 2022; x 5 m, y 6 m; 3,5 m kat) kaynak geometrisiyle kurulacak; lif etkisini sismik detaylandırma sınırları (Vc=0, 150 mm etriye, Mpr zinciri) bastırmayan **deprem içermeyen** problemler onlardır. Eğilme ve kesmenin yerçekimi tarafından yönetildiği geniş açıklık orada doğal bir tarama konusudur, ve kaynağa bağlı olduğundan HARKing değildir | Düşük-orta | Ö1'in A11 yüksek süneklik zinciri lif lehine en elverişsiz rejimdir (kesme zaten donatı ve detaylandırma ile yönetiliyor). Bu, "lifin fayda sağladığı koşul haritası" için bilgilendirici bir bulgudur | `THESIS_EXAMPLE_DATA.md` 27–35, 51–58; MEMORY 3d |
| 3 | Fiyat | Aşağıda §2 | | | |
| 4 | Konumlandırma | Aşağıda §3 | | | |

## 2. Fiyat ve başa baş

- **Kanıt durumu:** ÇŞİDB 2024 poz 10.300.4014/4024 (tutkallı/tutkalsız 0,90×60) DOĞRULANDI; fiyatı lif sınıfına (3D/4D/5D, çekme 1 240/1 600/2 300 MPa) göre ayırmaz (`UNIT_PRICE_EVIDENCE.md` §2, §7). Koddaki 90 TL/kg, `Cost.vb` satır 16'da "10.300.4014, 2026" olarak etiketli; 2026 PDF'ini ben açmadım (`PRICES_2026.md` okunmadı), onay Literature ajanında. Tedarikçi teklifi bekleniyor (MEMORY 3e); bu yüzden **4D/5D için fiyat kanıtsızdır**. IA'nın "ÇŞİDB'den fazla olur" beklentisi makul ama kanıtsız: bir sayı uydurmayacağız.
- **Yön:** Lif fiyatı ↑ ⇒ lifin tasarruf etmesi gereken donatı/beton miktarı ↑ ⇒ başa baş daha zor. 90 TL/kg zaten 4D/5D için alt sınır sayılmalı; negatif sonuç daha da güçlenir, ama yalnız B1 giderilirse bir anlam taşır.
- **Başa baş (yalnız koddaki sabitlerden hesaplanan büyüklük sırası):** 30 kg/m³ × 90 TL/kg = 2 700 TL/m³ beton; C30 beton birim fiyatı 3 649 TL/m³ (`Cost.vb` 9). Donatı efektif ≈ 24,2 + 44,7–46,5 ≈ 69–71 TL/kg (Cost.vb 12–14,28–29). Lif dozunun maliyetini çıkarmak için ≈ 38 kg donatı/m³ kiriş betonu tasarruf gerekir (30 kg/m³ lif başına). Bu hesap kodda fiyatı değiştirmeden, `WithFibreFactor` ile **fiyat çarpanı tarayan** bir duyarlılık olarak sunulmalı (ör. ×0,5…×3); sonuç "kritik lif fiyatı" olarak makaleye girer. Böylece fiyat belirsizliği sonucun kendisi olur, kullanıcının önsel beklentisi (lif ucuzlamaz) de sınanmış olur.
- "Lifin ucuzlaması beklenmemeli" önermesi: **kısmen doğru ve makul bir çerçeve**; ancak bunu makaleye "beklenti" olarak değil, kritik lif fiyatı p* olarak yazın. p* < ÇŞİDB ise sonuç net negatif, p* > ÇŞİDB ise koşullu pozitif.
- Lif dışı kalemler: beton yalnız fc'ye bağlı fiyat; lifli beton için ek bedel (pompa/karışım/işçilik) yok (P5 açık, BULUNAMADI). Bu, lif lehine yanlı. Kalıp ve donatı işçiliği modelde var (Cost.vb 59–61). Lif ile çalışmada donatı bağlama işçiliği azalırsa bu kazanç da modelde yok (aleyhine yanlı); ikisi birlikte "varsayım tablosu"nda yazılmalı.

## 3. Makale konumlandırması (Structures)

- Negatif/koşullu sonuç Structures'da kabul edilebilir **yalnız** şu koşulda: arama yeterliliği kanıtlanmış (B1), fiyat belirsizliği duyarlılıkla ele alınmış, lifli doğrulama sınırı (S2 açık, lifli sonuçlar "model sonucu") açıkça yazılmış. Aksi halde hakem "lifli sonuç doğrulanmamış, üstelik arama zayıf" der.
- Savunulabilir katkı: ETABS-OAPI entegre iş akışı, ACI Bölüm 18 kapasite tasarımı zinciri ve ETABS ile V2 doğrulaması, ve "lifin hangi koşulda (açıklık, yük, fiyat, deprem/deprem yok) ekonomik olduğu" haritası. Bu üçüncüsü, negatif sonucu bilgi haline getirir. Başlık/özet "lifli çerçeve daha ekonomik" iddiasını taşımamalı.
- Önerilen minimum ek çalışma:
  1. **B1 çözümü:** lifli aramayı lifsiz en iyi çözümle tohumlama (seeding) veya lifsiz optimum üzerinde doz ve tip taraması (lif ekleyerek aynı kesitlerde maliyet/kapasite kontrolü); tasarım uzayı alt kümesi olduğundan lifli ≤ lifsiz doğrulaması. Alternatif: bütçeyi artırma (örn. aynı yöntem 30 koşu, FE ↑) ve yakınsama eğrisi. Bu bir yöntem düzeltmesidir, sonuç oynaması değildir; ancak Kural 7/8 gereği programa ve kampanya tasarımına dokunacağı için PCR + IA onayı şart.
  2. Ö1 için kritik lif fiyatı taraması (fiyat çarpanı), sarılma bölgesi lif katkısı açık/kapalı, Mpr'de lif katkısı dahil/hariç duyarlılıkları.
  3. Optimumlarda boyuna donatı, etriye, kesit, lif kg ayrımı (şu anki csv'de yalnız toplam; etriye payı raporlanmalı).
  4. Ö2 ve Ö3'ü kaynak geometrisiyle kur; açıklık taraması yalnız bunlarda, önceden yazılmış protokolle (L değerleri, yükler, sonuç tablosuna bütün L'ler).
- Onay kapıları (AGENTS kural 10): K1 B1 yöntem düzeltmesi + PCR; K2 duyarlılık/tarama protokolünün sonuç görülmeden yazılı onayı (bu dosya ekine); K3 negatif sonucun makalede raporlanması; K4 Ö2/Ö3 kurulum şartnamesi.

## 4. Net öneri (≤10 satır)

1. Ö1'in açıklığını değiştirme; mevcut sonucu "lif pahalı" diye yorumlama. Lifli uzay lifsizi içerirken lifli maliyet yüksek çıkıyor, yani kampanya arama yetersizliği ölçüyor (BTO'da kirişlerin %90'ı doz=0).
2. Önce lifli aramayı lifsiz optimum ile tohumla / doz taraması yap (PCR, IA onayı), kampanyayı yeniden koştur.
3. Fiyat belirsizliğini kritik lif fiyatı (p*) taramasıyla çöz; 4D/5D için sayı uydurma, tedarikçi teklifi gelene kadar 90 TL/kg alt sınırdır.
4. Ö1 sismik zinciri lifi bastırıyor (sarılmada lif=0, 150 mm detay sınırı, Mpr'de lif etkisi); bunu bulgu olarak yaz ve duyarlılıkla göster.
5. Açıklık/geometri taramasını yalnız Ö2/Ö3 (deprem yok, kaynağa bağlı geometri) içinde, önceden yazılmış protokolle yap; negatifleri de raporla.
6. Makaleyi "lifin ekonomik olduğu koşullar haritası + doğrulanmış ETABS iş akışı" olarak konumlandır.

## 5. IA'ya sorulacaklar

1. B1 (lifli ≥ lifsiz paradoksu) için yöntem düzeltmesini (tohumlama veya FE artırma) onaylıyor musun? Hangisi?
2. Mpr'de lif katkısı (γF=1) dahil kalsın mı, hariç mi? Sarılma bölgesinde lif=0 için kaynak atfı (ACI/fib) var mı?
3. Lifli beton için pompa/karışım ek bedeli varsayımı eklensin mi (P5 açık)? Değer kaynağı?
4. Tedarikçiden 4D/5D fiyatı gelirse hangi yıl/birim kullanılacak? Gelmezse p* taraması aralığı ne olsun (alt/üst çarpan)?
5. Açıklık taraması için L değerleri ve yükleri sonuç görülmeden onaylanacak mı (yalnız Ö2/Ö3 içinde)?
