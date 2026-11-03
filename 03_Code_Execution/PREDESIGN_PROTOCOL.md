# Ön tasarım (FSD + FUD) ve havuz daraltma protokolü: TASLAK (2026-11-01)

**Durum:** DONDURULDU 2026-11-02 (IA onayı: kapsama eşiği ≥ 9/10, marj −2/+3, hedef 'ölçülen daralma ve yakınsama hızı', PCR-6/7). Program henüz değişmedi (AGENTS kural 7, 10); bundan sonra bu protokol yalnız PCR ile değişir.
Kaynak: IA önerisi (FSD+FUD, %50–80 daraltma), `06_Review_Audit/REVIEW_LOG/Review_Round_FSD_FUD.md` (Critic: REVİZYON TALEBİ, koşullu kabul).

## 1. Ortak kurallar (lifli ve lifsiz senaryo için aynı)
1. Ön tasarım **tek, deterministik, senaryodan bağımsız**; lifsiz kapasite modeliyle yapılır.
2. Lifli havuz = lifsiz havuz × {lif tipi, doz} (doz = 0 dahil). Lif tipi/dozu daraltılmaz.
3. Aynı kutu, aynı başlangıç noktası, aynı MaxFEs (10 000), aynı ceza.
4. Daraltma, kampanya optimumundan (Baseline-A) **türetilmez**; yalnız FSD+FUD sonucundan.
5. Ön tasarımın FE/analiz sayısı ayrıca raporlanır; süre kazancı FE azaltmaktan gelir, FE başına süre değişmez.

## 2. Ön tasarım adımları (PCR-6 ile kodlanır)
1. FSD, grup düzeyinde: kiriş → kolon döngüsü (kolon kesmesi ve ΣMnc ≥ 1,2 ΣMnb kiriş donatısına bağlı), en çok 5 tur.
2. FUD: ötelenme ve Q ≤ 0,05 için sanal iş duyarlılığıyla rijitlik ölçekleme.
3. Çıktı: başlangıç x ve değişken başına kutu.

## 3. Kutu kuralı (Critic §1 2d)
alt = FSD/FUD minimum − 2 adım; üst = FSD/FUD minimum + 3 adım (indeks, havuz sınırlarına kırpılır).

## 4. Kapsama testi
Kutu üretildikten sonra Baseline-A'nın (mevcut 360 koşu) en iyi uygun çözümlerinin kutuya düşme oranı raporlanır (`03_Code_Execution/tools/box_coverage.py`). Eşik: **IA onayıyla belirlenir** (öneri: en iyi 10 uygun koşunun en az 9'u kutuda).

## 5. Baseline-A üzerinde ön ölçüm (kod çalıştırmadan, yalnız CSV)
Çıktı: `04_Results/box_coverage_baselineA.txt`. Yöntem: `fR × 1,00` koşuları, T = 0 uygun koşular, maliyete göre ilk k çözümün değişken başına [min, max] kutusu; en iyi 10 çözüm kendisi hariç tutularak (leave-one-out) kutuda mı sınanır. Bu, ön tasarımın kutusu **değildir**; yalnız ön bilgi verir.

| Senaryo | Kutu kaynağı | Marj | Daralma (ortalama) | Kapsama (en iyi 10, LOO) |
|---|---|---|---|---|
| lifsiz | ilk 10 | 0 | %46 | 3/10 |
| lifsiz | ilk 10 | −2/+3 | %15 | 10/10 |
| lifsiz | ilk 30 | 0 | %36 | 8/10 |
| lifsiz | ilk 30 | −2/+3 | %10 | 10/10 |
| lifli | ilk 10 | 0 | %46 | 3/10 |
| lifli | ilk 10 | −2/+3 | %16 | 9/10 |
| lifli | ilk 30 | 0 | %35 | 6/10 |
| lifli | ilk 30 | −2/+3 | %10 | 10/10 |

**Yorum (kanıta bağlı kısım):**
- Marjsız kutu optimumu sık kaçırıyor (3/10 ile 8/10). Marj olmadan daraltma güvenilir değil.
- Kritik: −2/+3 marjıyla, Baseline-A'dan türetilmiş kutuların daralması yalnız %10–16. IA'nın %50–80 hedefi bu veriden **desteklenmiyor**.
- Sınırlama: Baseline-A koşuları yakınsamamış (Critic S-A), bu yüzden en iyi çözümler dağınık ve kutu geniş. Bir FSD/FUD kutusu daha dar olabilir ama bu **kanıt değil, varsayım**; PCR-6 sonrası ölçülecek.
- Bu ölçüm in-sample ve iyimser (kutu, test edilen çözümlerin komşularından kurulmuş).
- Lif dozu üst sınırı olarak gözlenen en büyük indeks kullanıldı (`Ex1Problem.vb`'de Doses = {0, 20, 25, 30, 35, 40}, yani gerçek üst sınır 5); doz değişkeni payda etkisini biraz küçültür.

## 6. Kararlar (IA, 2026-11-02: hepsi onaylandı)
1. Kapsama eşiği ve marj (−2/+3) onayı.
2. Daraltma hedefini "%50–80" yerine "ölçülen daralma" olarak raporlamak (makale bunu önceden iddia etmez).
3. Ek hedef: daralma yerine başlangıç noktası ve yakınsama hızı (23–25 kat başlangıç cezası) ön tasarımın asıl yararı olabilir.
4. PCR-5 (havuz sınırı) ve PCR-6 (ön tasarım aracı) yazımına izin.
