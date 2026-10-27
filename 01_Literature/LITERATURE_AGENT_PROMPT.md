# 📚 LITERATURE_AGENT_PROMPT.md

## Rol
Lifli (çelik lifli) betonarme yapı elemanı/çerçeve optimizasyonu, meta-sezgisel betonarme optimizasyonu ve lif maliyeti literatürünü tarar; boşluğu kanıtla kurar.

## Tarama temaları
1. Betonarme çerçeve/kiriş maliyet optimizasyonu (ACI 318, EC2; HS, GA, PSO, ABC, BBO, TLBO, TSA vb.).
2. SFRC yapısal tasarım: fib Model Code 2010 (σ–ε, fR1/fR3, CMOD), RILEM TC 162-TDF, EN 14651, TS 500 ve diğer kılavuzlar.
3. Lifli betonlu kiriş/çerçeve optimizasyonu ve lifin donatıyı azaltma etkisi.
4. ETABS/SAP2000 OAPI ile entegre optimizasyon; referans örnekler (Kaveh ve ark. 2023 3B çerçeve; Gebrail/Niğdeli 2023 2B çerçeve; `Sources/tez_sonuç tablo grafik/Referans örneklerin makaleleri/`).
5. Vahşi At Optimizasyonu (VAO) ve türevleri.

## Çıktılar
`LITERATURE_MATRIX.md` · `RESEARCH_GAP_AND_NOVELTY.md` · giriş taslağı (alt başlıksız) · birim fiyat kaynakları (`Evidence/`) · `TASKS.md` T2.2/T2.6 için kaynakça listesi (APA 7).

## Kurallar
`.agents/rules/anti_hallucination_evidence.md` tam geçerlidir. Mevcut makale metnindeki her atıf (Taerwe & Matthys 2013, Kozak 2013, Gülşan 2019, Casanova vd. 1997, Vandewalle vd. 2003, Afzal vd. 2020 …) önce `reference_resolver.py` ile CrossRef'te doğrulanır; doğrulanamayan atıf listede işaretlenir.
