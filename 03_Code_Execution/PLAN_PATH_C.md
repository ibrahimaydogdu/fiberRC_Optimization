# 🧭 PLAN_PATH_C.md (Yol C: hibrit — FEM ile arama, ETABS ile kurma ve final doğrulama)

*Taslak 2026-10-27. Süreler `CODE_REVIEW_v2.md` tahminidir (25–40 iş günü), ölçülmemiştir. Kullanıcı onayı bekler.*

## Mimari
1. **ETABS (v22):** örnek EDB'den geometri/yük/kombinasyon okuma (açılış) ve **yalnız final tasarım doğrulaması** (düzeltmesiz yeniden analiz + Concrete Frame Design oranları).
2. **FEM çözücü (`SteelFamewithCompositeTubeColumn_FEM/FemCore`):** arama sırasındaki çerçeve analizi. Betonarme için çatlamış rijitlik (ACI 318-14 §6.6.3) ve P-Δ davranışı FEM'de tanımlanmalı; ETABS'le uyumu V2'de ölçülür.
3. **Yeni modüller (sıfırdan):** (a) ACI 318-14 kiriş/kolon kapasitesi (eğilme, kesme, P-M, detay, ρmin/ρmax); (b) fib MC2010 SFRC (σ–w, fFts/fFtu, kesme ve çatlak genişliği); (c) birim fiyat tablosu (ÇŞİDB 2026) + maliyet fonksiyonu.
4. **Yeniden kullanılacaklar:** optimizasyon yöntemleri (v2 `OptimizationClass` ile FEM tabanındaki yöntemler; YAKO/BTO/ATO eşdeğerleri karşılaştırılır), ETABS arka ucu, Excel çıktısı, koşu kaydı.

## İş paketleri
| WP | İçerik | Çıktı / Kapı |
|---|---|---|
| WP0 | FEM tabanı ve v2 yerinde okunur; betonarmeye uyarlama haritası; **v2'nin 8 kritik hatası** yeni kodda tekrarlanmaz kontrol listesi | `CODE_REVIEW_v2.md` §fizibilite + harita |
| WP1 | ACI 318-14 modülü + birim testler (elle hesaplı örnekler) | V1 |
| WP2 | fib MC2010 SFRC modülü; girdi = yayınlanmış fR1…fR4 (Venkateshwaran 2018, Chen 2021) | V1 |
| WP3 | FEM–ETABS doğrulama: Ö1, Ö2, Ö3 + Kaveh 2011 4 katlı çerçeve (lifsiz kıyas) | V2: ETABS ile D/C, ötelenme, periyot farkı ⚠️ ≤ %1–2 |
| WP4 | Maliyet (2026 ÇŞİDB) + duyarlılık (beton/donatı/lif/kalıp oranı, 2022–2026 gözlenen aralık) | V5 |
| WP5 | **Tez yeniden üretimi:** tezin eski fiyatlarıyla v2 çıktısı ↔ yeni kod karşılaştırması; farkların nedeni (F-01…F-11) belgelenir | `04_Results/REPRODUCTION_REPORT.md` |
| WP6 | Optimizasyon koşuları (algoritma sayısı Critic görüşüyle; en az 30 koşu) + final ETABS doğrulaması | V3–V6 |
| WP7 | Tablolar (formüllü Excel), şekiller, bulgular | `Maliyet_Metraj.xlsx` |

## Kapılar ve riskler
- V2 geçmeden WP6 başlamaz. 3B (Ö3) FEM–ETABS farkı büyükse o örnek ETABS ile ya da daha az iterasyonla koşulur (hesap bütçesi kapısı).
- FEM tabanı **çelik/kompozit** için doğrulanmış; betonarme eleman (çatlamış rijitlik, kolon P-M, kiriş T-kesit/döşeme etkisi) ilk kez eklenir → V2 en büyük risk.
- Veri riski: üretici fR verisi gizli; yayınlanmış veriler birincil.
- Kaynak kodu D: sürücüsündeki program salt-okunur; çalışma kopyası `03_Code_Execution/program_work/`.

## Onaylar (kullanıcı, 2026-10-27)
1. Algoritma kümesi: ABC (YAKO), BBO (BTO), TSA (ATO) esas; VAO dahil olabilir — **karar iç Hakem'de** (WP0 sonrası, FEM tabanında VAO hazırsa maliyeti düşük → dahil; İVAO ayrı makale).
2. Örnek 1: özgün örnek (onaylandı).
3. ETABS lisansı otomasyonda kullanılabilir (onaylandı).
4. **WP0 başlatıldı** (alt ajan; çıktı `WP0_ADAPTATION_MAP.md`).
