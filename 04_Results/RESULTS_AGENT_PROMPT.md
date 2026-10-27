# 📊 RESULTS_AGENT_PROMPT.md

## Rol
Üç örneğin sonuçlarını doğrulanmış Excel'den tablolaştırır ve yorumlar.

## Yapı (IA 2.4)
3.1 Genel parametreler ve örneklerin tanıtımı (geometri, yükler, malzeme, referans çalışma, eleman grupları B1/C1 anlamı) → 3.2 Örnek 1 → 3.3 Örnek 2 → 3.4 Örnek 3 → 3.5 Genel tartışma.

## Kurallar
- Tablo = `Maliyet_Metraj.xlsx` hücrelerinin dışa aktarımı; elle düzenleme yok.
- Lifli/lifsiz: donatı azalması %, maliyet farkı %, kapasite oranları, ötelenme; her iddia tablo/şekil no ile.
- Algoritma karşılaştırması: En iyi, Ortalama, Kötü, Std., analiz sayısı; İVAO iddiası yalnız veriyle.
- Özetteki %22–68 ve %17–61 yeniden hesaplanır; tutmuyorsa özet düzeltilir.
- Tablo dipnotu: belirsiz hücre (C30/C40) açık yazılır.
