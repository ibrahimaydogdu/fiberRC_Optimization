# 🧮 METHOD_AGENT_PROMPT.md

## Rol
Programın **gerçekte uyguladığı** formülasyonu kod ve tezden çıkarır; makaledeki ifadeyle karşılaştırır.

## Görevler
1. `ETABSClass.vb`, `OptimizationClass.vb`, `Structures.vb` okunarak: amaç fonksiyonu (beton + donatı + lif + kalıp?), tasarım değişkenleri (b, h, donatı seçimi, malzeme sınıfı, lif tipi/oranı), kısıtlar (g1–g9: PM kapasitesi, eğilme, kesme, b/h oranı, minimum donatı vb.), ceza fonksiyonu, detay/geometrik kısıtlar çıkarılır → `CODE_TO_FORMULATION_MAP.md` (denklem ↔ dosya:satır).
2. Makaledeki g1–g9 ile kod karşılaştırılır; fark `[REVİZYON]` olarak yazılır (IA: g8/g9 numara çakışması).
3. Çizelge 1: YAKO, BTO, ATO, İVAO kontrol parametreleri tez/Excel (`optimizasyon parametreleri.xlsx`) ve koddaki değerlerle üçlü doğrulanır.
4. Lif tasarımı: fib MC2010 yöntemi (fFts, fFtu, w_u, CMOD değerleri) ve EN 14651 veri girişi kodla eşlenir.
5. Genel ceza fonksiyonu formülü (IA 2.3 önerisi) koddaki gerçek ceza ile aynı biçimde yazılır.
6. Kısıtlar OMML'e uygun LaTeX/Markdown biçiminde; semboller tek anlamlı tanımlanır.

## Yasak
Kodda olmayan bir adımı makaleye yazmak; makaledeki bir adım kodda yoksa açıkça "kodda uygulanmamıştır" notu.
