# EX2_V3_PLAN.md — Ö2 V3: kampanya 4 en iyi tasarımlarının ETABS yeniden analizi (PLAN, henüz koşulmadı)

Tarih: 2026-11-20 · Yazan: Claude Code (Developer Agent) · Dayanak: `EX2_SPEC.md` §10 V3, §11.4–11.5, §13 (IA konumlandırma kararı, seçenek 1); `EX2_VALIDATION_REPORT.md` §7 (V2-a araç ve kapılar).
**Durum:** taslak, IA onayı bekler. Onaylanmadan ETABS'e dokunulmaz. Bu plan program, ikili, katalog ve fiyat değiştirmez; PCR gerektirmez. Sonuçlar Critic + IA onayı ve S2 olmadan bulgular bölümüne girmez; ek materyal dizinine (`05_Drafts/SUPPLEMENTARY_INDEX.md` §B) eklenir.

## 1. Amaç ve sınır

V3'ün tek sorusu: **makalede raporlanacak optimum tasarımlar ETABS'te düzeltmesiz yeniden analiz edildiğinde ETABS'in kapasite oranları ≤ 1 mi?** ETABS lif katkısını bilmez; bu yüzden lifli tasarımlarda ETABS ile bağımsız Excel ayrı roller taşır (§4). Makale ifadesi (spec §10 V3): "fibre contribution not verified by ETABS; checked by independent calculation". Yasak: "ETABS lifli kirişi doğruladı", "ETABS'te doğrulandı" (kapılar bitmeden), "ACI uyumlu".

## 2. Tasarım kümesi (IA onayına sunulan)

Vektörler `04_Results/ex2/campaign4/runs.csv` ve `analysis4/*_evalonly.txt` dosyalarından **betikle** çıkarılır (elle yazılmaz); her vektör `--evalonly` ile yeniden hesaplanıp T = 0 ve maliyet eşitliği doğrulanır.

| Kademe | Tasarım | Vektör / maliyet (TL) | Neden |
|---|---|---|---|
| 1 | **P** (lifsiz en iyi) | x = 0,0,0,0,1,0,1,2,1,0,0,0,0,5,0,6,0,6 · 236 456,22 | ana metin |
| 1 | **Y90** (en iyi lifli 90 TL/kg, YAKO, lif > 0) | `runs.csv`'den; 237 799,80 | ana metinde lifli sonuç; **lif içeren tek ana-metin tasarımı** (BTO/ATO en iyi lifli 90 = P) |
| 1 | **F0** (lifli 0 TL/kg en iyi) | x = 0,0,0,0,0,0,1,2,1,0,0,0,0,5,0,5,0,4,0,5,0,5,0,5 · 234 190,76 | ek materyal; sehim varsayımı tablosunun dayanağı |
| 2 | kalan yöntem en iyileri: lifsiz YAKO/BTO/ATO, lifli 90 BTO/ATO, lifli 0 YAKO/BTO/ATO | `runs.csv` | spec §10 "her senaryo × yöntem en iyi çözümü"; ek materyal |
| ref. | **F0Z** (F0 kesitleri, lif 0) | `analysis4/F0Z_evalonly.txt` | aşım lifin mi kesitin mi: ETABS'te lifsiz kesit olarak koşar, F0'ın kesitleriyle P'nin gerçek karşılığı |

Kademe 1 zorunlu; Kademe 2 vakit varsa (araç tasarım başına ≈ birkaç dakika, V2-a deneyimi). Karar IA'da: yalnız Kademe 1 mi, Kademe 2 de mi.

## 3. ETABS koşusu (lifsiz yol; her tasarım için)

Araç: `03_Code_Execution/ex2_work/EtabsEx2_rerun/` kopyası (`EtabsEx2_V3/`; V2-a aracı DEĞİŞTİRİLMEZ; SHA'lar günlüğe). Model ayarları V2-a ile aynı (EX2_VALIDATION_REPORT §0, §7.0): XZ düzlemi, rijit bölge yok, C30, tek yük deseni, **PatLLF = 0**, NonSway, lu = net yükseklik atanıp geri okunur, ACI 318-14, yalnız U1 = 1,4D ve U2 = 1,2D + 1,6L. Çalışma klasörü **yerel ASCII yol** (Google Drive/Türkçe karakter `Go_Model_Read` hatası verdi); EDB'ler sonra `04_Results/ex2/v3_etabs/edb/`e kopyalanır; iş sonunda `ApplicationExit` ve süreç kalmadığı kontrolü.

Araçta yapılacak değişiklikler (yalnız kopyada): (i) tasarım vektörünü `Ex2Model.dll` (`ex2_campaign4_bin`, SHA f7207d66…) ile okuyup kesit/donatı/etriye çıkarmak (V2-a aracı yalnız lifsiz `x` dosyası okuyordu; **lifli vektör okuması denenecek, olmazsa** kesitleri ve program donatısını `--evalonly` çıktısından alan ara dosya); (ii) ETABS'e program donatısı verilmez, **ETABS'in gerekli As ve Av/s'si** okunur (V2-a ile aynı mantık) ve programın gerekli/sağlanan değerleriyle karşılaştırılır; (iii) sehim bacağı (§5).

### Kapılar (V2-a eşikleriyle aynı, Kademe 1 hepsi için)

| Kapı | Ölçüt | Not |
|---|---|---|
| V3-1 reaksiyon/kuvvet | ΣFz ve istasyon kuvvetleri FEM ↔ ETABS ≤ %0,1 | V2-a'da 0,0000 % |
| V3-2 lu ataması | geri okunan lu − program lu = 0 mm | |
| V3-3 kolon D/C (CD1) | ETABS O/S veya D/C > 1 olan kolon = 0 | P ve F0'da program 6.2.6 oranı sınırında (1,3800 / 1,3755 < 1,4): **ETABS O/S çıkarsa BULGU, kapı KALDI** |
| V3-4 CD2, CD3 | program/ETABS ≥ 0,95; güvensiz yönde ≤ %5 | |
| V3-5 kiriş eğilmesi | lifsiz kesitler (P, F0Z, Y90 lifsiz yönü): ETABS gerekli As ≤ sağlanan As; program/ETABS 0,98–1,10 | BULGU-2 PCR-24 ile kodda kapalı |
| V3-6 kiriş kesmesi | Av/s ETABS-uyumlu sabitlerle (0,166 √f′c; 60 ksi) 1,00 ± 0,02 (x = d) ve Vu/φVn ≤ 1 | AÇIK-14: program düğümden d; ETABS düğümü raporlanır, ayrı sütun |
| V3-7 N/A | hücre boş/N/A yok | yoksa `ANALİZ ÇALIŞTIRILAMADI` + gerekçe |

**Lifli tasarımlarda (F0, Y90) beklenen ve kapı dışı kalan:** ETABS lif eğilme kredisi (MC2010) ve lif kesme artımını bilmez; ETABS gerekli As > program boyuna donatısı çıkması **beklenir** ve "lif katkısız" sütunu olarak raporlanır (spec §10 V3 (3); N/A değil). F0'da boyuna donatı tasarrufunun %68'i bu krediden (2 248 / 3 314 TL) olduğundan bu fark büyük olabilir. Kolonlar lifsizdir; kolon kapıları (V3-3/4) F0'da da tam geçerlidir.

## 4. Lif katkısı: bağımsız formüllü Excel (ETABS'te yapılamaz)

Dosya: `04_Results/ex2/v3_etabs/V3_fibre_independent.xlsx`; her hücre **formül**, girdiler görünür (kural 2, IA 1.1). IA kontrolüne sunulur. Sayfalar:
1. **Girdiler:** F0 ve Y90 lifli grupların tip/doz, fR1/fR3 (`Evidence/CMOD_test_verileri.xlsx` karakteristik, fR ×1,0), bw, h, d, fck, γc, γF, kesit ve program donatısı; kaynak sayfa/madde atıfı.
2. **Eğilme (S1):** lifli Mn'in bağımsız entegrasyonu (MC2010 σ–ε çok-doğrulu bağıntı), program Mn ile ≤ %1 (VALIDATION_SPEC S1). Karşılaştırma ETABS'in lifsiz Mn'iyle: fark = lif kredisi (rapor, yargı yok).
3. **Kesme:** Vc,ACI + ΔVF (MC2010 Denk. 5.6-8; A63), `max(0, ΔVF(γc=1,0))`, Av,min uygulaması; kritik kesit **düğümden d** (program kuralı); D/C = Vu/[φ(Vc,f + Vs)] ≤ 1; ETABS'ten Vu okunur. Av,min belirleyici olan F0'da sonuç kesme formatından bağımsız çıkması beklenir (§11.5: üçlü format özdeş).
4. **Bayraklı varyantlar:** fR ×0,8; MC2010-yerel kesme biçimi (`--shearformat mc2010`); bayrak olmadan ana tablo.
5. **Maliyet (V5 parçası):** metraj × birim fiyat formülü, program maliyetiyle ≤ %0,5 (P: 236 456,22; F0: 234 190,76; Y90: 237 799,80).

## 5. Sehim bacağı (F0 için kritik; P için doğrulama)

Hedef: programın ℓ/480 sayısını (F0 0,9847; P 0,6962; F0Z 0,9548) ETABS'e **yük ve rijitlik varsayımlarını açıkça eşleyerek** çapraz kontrol etmek; üretilen sayı ETABS'in doğrulaması değil, **varsayım eşleşmesi kontrolüdür**. Yöntem:
- Kritik kiriş/yük deseni, `analysis4/F0_patterns.txt` ve `F0_deflection_patterned.txt`'ten (kod ↔ araç eşleşmeyen bacak yok: 4/4, fark 0) alınır.
- ETABS'te kiriş grubuna **özellik düzenleyici I22 = Ie/Ig** (programın zarf Ie'si; `analysis4` çıktısından) atanır; D ve L desenleri ayrı yük durumu; sehim = 2·δD + δL (programın λ varsayımı) **sonradan işlenerek** hesaplanır; ETABS bunu hesaplamaz.
- Kabul: ETABS ↔ program açıklık sehimi ≤ %2 (aynı Ie ve aynı desen verildiğinde; fark yalnız FE ayrıklaştırmasıdır). Bu kapı geçerse program sehimi hesabının yeniden üretildiği söylenir; **ℓ/480'in doğru sınır olduğu söylenmez** (duyarlılık tablosu sürer: net açıklık 1,037, Bischoff 1,057, ψ 0,25 → 1,080, ani D 1,287).
- Ie hesabının (ACI 24.2.3.5, Mcr, Icr) bağımsız Excel kontrolü: P ve F0'ın en kritik kiriş grubu için.

## 6. Ek kontroller (aynı oturumda, ucuz)

- **R8 / ACI 6.2.6:** ETABS-örtük oran ↔ program oranı (V2-a'da 12/12 eşleşti) P ve F0 için; kritik çarpan (P ×1,0375, F0 ×1,0467) ETABS'te gravite ×1,03 ve ×1,05 ile **O/S eşiği** olarak sınanır (varsa program ↔ ETABS eşik uyumu).
- **S3 katı Q = 0,0522 (F0):** ETABS Q indeksini hesaplamaz (V2-a AÇIK-19); program Q'su ETABS'le doğrulanamaz → raporda sınırlama olarak yazılır, yeni iddia yok.
- **Kolon e_min, P-M:** koşu C (büyütmesiz) ile program ≥ ETABS D/C (0,992–1,134 beklenir).

## 7. Çıktılar ve kayıt

`04_Results/ex2/v3_etabs/` (raw, summary_V3_*.csv, edb/, console, SHA256SUMS), `03_Code_Execution/EX2_VALIDATION_REPORT.md` §8 (V3 raporu; mevcut §1–7 korunur), `EX2_V3_PLAN.md` durum satırı, `SUPPLEMENTARY_INDEX.md` §B, HANDOFF, PROJECT_STATE. Her koşu: ETABS sürümü (22.6.0.4035), ikili SHA'ları (Ex2Model f7207d66…, FemRcModel c1a95951…, RcCore 14f84b75…, FemCore 8da9dfd2…), araç SHA'ları. Git: Gemini.

## 8. Başarısızlık durumları ve kararları

| Durum | Eylem |
|---|---|
| ETABS kolon O/S (6.2.6; P/F0 marjı %1,5–2) | kapı KALDI; BULGU kaydı; program/kampanya değişmez; "uygun" iddiası ilgili tasarım için çekilir; IA'ya PCR seçeneği (g6'ya ETABS uyumu, kampanya 5) sunulur |
| ETABS lifsiz kesit D/C > 1 (F0, Y90) | beklenen: "lif katkısız" sütunu; Excel kapısı (§4) belirler |
| Excel lifli Mn/Vn ↔ program > %1 | BULGU; lifli sonuç ek materyalden de çekilir; PCR gerekebilir |
| Sehim ETABS ↔ program > %2 | desen/Ie eşlemesi kontrol; çözülmezse sehim sayısı "doğrulanamadı" |
| Model açılamıyor | ASCII yola taşı (V2-a notu); olmazsa `ANALİZ ÇALIŞTIRILAMADI` |

## 9. IA kararları (onay istenen)

1. **Kapsam:** yalnız Kademe 1 (P, Y90, F0, F0Z) mi, Kademe 2 de mi? (önerim: Kademe 1 önce, sonra IA'ya sor)
2. **Y90 seçimi:** en iyi YAKO lifli 90 TL/kg tasarımı (237 799,80) ana metin lifli tasarımı olarak kullanılsın mı?
3. **Excel ve ETABS sehim bacağı** (§4–§5) bu oturumda mı, ayrı oturumda mı?
4. ETABS'i bu makinede çalıştırma ve çalışırken IDE/makinenin açık kalması onayı (V2-a'da olduğu gibi; süre tahmini kayıt dışı).

Tahmini iş (kayıt dışı, makaleye girmez): araç kopyası + lifli vektör okuma ≈ yarım gün; ETABS koşusu tasarım başına V2-a deneyimine göre dakikalar; Excel ve rapor ≈ bir gün.
