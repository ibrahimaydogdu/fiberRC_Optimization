# GEMINI_TASK_G11_ETABS_CFD.md — ETABS Concrete Frame Design kılavuzları çıkarımı (2026-10-30)

Kullanıcı tek komut verir: **"G11"**. Önce `GEMINI.md` "commit" adımlarını uygula (bekleyen Claude dosyaları: HANDOFF.md satır (51)–(55), VALIDATION_REPORT.md, EtabsProbe/, Ex1Runner/, FemRcModel/Optimizers.vb, FibreTable.vb, Ex1Problem.vb), sonra aşağıdaki çıkarımı yap. Kurallar: `.agents/rules/anti_hallucination_evidence.md`. `Evidence/` ve `Sources/` salt-okunur, git'e girmez. **Kod dosyalarını değiştirme.**

## Kaynaklar (orijinal format, değiştirme)
| Dosya | SHA-256 | Bayt |
|---|---|---|
| `Evidence/DesignGuides/CFD-ACI-318-14.pdf` | 6e9e44322cbc5bc8125ed8c63e28268c818741100b01f1b19f21c63357e689eb | 3 086 351 |
| `Evidence/DesignGuides/CFD-TS-500-2000(R 2018).pdf` | 43c4d40b1e7fee8218e95e4f67821edcbbd661261c2d5ecedb7fcb1b2224836e | 2 362 396 |

Bunlar CSI "Concrete Frame Design Manual" kılavuzlarıdır. Amaç: ETABS Concrete Frame Design'ın (ACI 318-14 ve TS 500-2000) **kolon/kiriş D/C oranını ve kapasitelerini nasıl hesapladığını** sayfa sayfa belgelemek. Claude bunu bizim programın (FemRcModel: ACI kolon P–M etkileşimi, SFRC kiriş eğilmesi, kesme) hesabıyla karşılaştıracak (V2b). Her iki kılavuz için ayrı çıkar.

## Her kalem için zorunlu biçim
- Kılavuz, bölüm başlığı, denklem no, **PDF ve basılı sayfa** (ikisi de),
- birebir alıntı (≤25 kelime, orijinal dilde),
- değişken tanımları ve birimleri,
- durum etiketi: **ÇIKARILDI / KISMEN / BULUNAMADI**. Tahmin yok; bulunamayan kalemi "BULUNAMADI" yaz ve kılavuzun hangi bölümüne baktığını belirt. (Doğrulandı etiketini yalnız Claude verir.)

## ÇIKARILACAK KALEMLER

### A. ACI 318-14 kılavuzu
1. **Tasarım kombinasyonları ve φ:** varsayılan yük kombinasyonları (1,4D; 1,2D+1,6L; deprem/rüzgâr içerenler), φ katsayıları (eğilme/çekme kontrollü, basınç kontrollü, kesme, burulma) ve ETABS'in ε_t'ye göre geçiş uygulaması.
2. **Kolon P–M–M etkileşimi:** etkileşim yüzeyinin nasıl oluşturulduğu (kaç nokta, eğilme eksenleri, 2 eksenli eğilme için yüzey/ışın yöntemi), Pn,max sınırı (0,80 / 0,85 çarpanları), **D/C oranının tanımı** (hangi uzunluk oranı: kapasite ışınına mesafe mi, toplam yük oranı mı).
3. **Narinlik ve moment büyütme:** sway/nonsway tanımı, δ_ns, δ_s, minimum moment (e_min), k çarpanı seçimi; ETABS bunları varsayılan olarak yapıyor mu.
4. **Kiriş eğilme tasarımı:** dikdörtgen/T kesit, gerekli donatı hesabı, çift donatı koşulu, minimum ve maksimum donatı sınırları, ε_t ≥ 0,004 kontrolü.
5. **Kesme tasarımı (kiriş ve kolon):** Vc denklemleri, Vs, Av/s, minimum kesme donatısı, φVn, ve **sismik (sünek) çerçeve için kapasite tasarımı kesme kuvveti** (Ve = 2 × ... gibi; hangi moment: Mpr, 1,25fy).
6. **Burulma tasarımı:** burulma eşiği, Tn, Al/At.
7. **Örtü ve d' varsayımları:** kiriş ve kolon için ETABS'in kullandığı donatı ağırlık merkezi mesafesi / örtü girdileri (Concrete Frame Design Preferences ve kesit tasarım girdileri).
8. **Sünek çerçeve (Special moment frame) kontrolleri:** güçlü kolon–zayıf kiriş (ΣMc ≥ 1,2 ΣMb), eklenti, birleşim bölgesi kesmesi; ETABS bunları hangi tablo/sütunda raporluyor.
9. **Çıktı tabloları:** `Design Data / Concrete Frame Design Summary` tablolarında (DatabaseTables) hangi sütun adları **D/C oranı**, kontrol edilen kombinasyon ve istasyonu verir. OAPI'den (`DesignConcrete.GetSummaryResultsColumn / GetSummaryResultsBeam`, ilgili `DatabaseTables` tablo adları) okunabilecek alan adları kılavuzda geçiyorsa listele; geçmiyorsa BULUNAMADI.

### B. TS 500-2000 (R 2018) kılavuzu
Aynı 1–9 maddelerini TS 500 için çıkar, ek olarak:
10. **Malzeme ve katsayılar:** fcd = fck/γc, fyd = fyk/γs, γc ve γs değerleri; basınç bloğu parametreleri (k1, εcu); TS 500 Mr (φ yok) yaklaşımı.
11. **Kesme:** Vcr, Vc (Denk. 8.x), Vmax, minimum etriye, **Vc = 0 koşulları** (deprem etkisinde sarılma bölgesi). Varsa ETABS'in Vc'yi sıfırlama koşulunu uygulayıp uygulamadığı.
12. **Sismik kapasite tasarımı:** Ve = Vdy + (Mra + Mrü)/ln gibi ifadeler ve ETABS'in uygulaması; kılavuzun TBDY-2018 ile ilişkisi (TS 500 + TBDY seçeneği var mı).
13. İki kılavuz arasında **farklı olan davranışları** (D/C tanımı, φ, kombinasyonlar) tek tabloda karşılaştır.

### C. Bu kılavuzlarda olması beklenmeyen ama aranacak (varsa çıkar, yoksa BULUNAMADI de)
- Rijit uç bölge (Rigid zone factor), kesit çatlama modifikatörleri (0,35 / 0,70), kütle kaynağı, diyafram ek dış merkezlik (%5) — bunlar genelde **Analysis Reference / CSI Help**'te yer alır. Bu kılavuzlarda geçmiyorsa kullanıcıya şu belge isteğini yaz: "ETABS Analysis Reference Manual (Rigid End Zones, Diaphragm Eccentricity, Response Spectrum Eccentricity bölümleri)".

## Çıktı
`01_Literature/ETABS_CFD_EXTRACT_G11.md` — iki bölüm (ACI, TS 500), madde numaralarına göre. Sonunda "Programa etkisi" başlığı **açmayın**, yorum yazmayın; yalnız çıkarım. Karşılaştırmayı Claude yapar.

## Bitiş
`00_System/HANDOFF.md`'ye satır ekle ("Gemini, G11 çıkarıldı, dosya, BULUNAMADI kalemleri"). Kullanıcıya tek satır rapor.
