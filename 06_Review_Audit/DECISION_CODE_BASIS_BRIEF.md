# Karar dosyası: betonarme şartname tabanı (ACI 318-14 mi, TS 500 + TBDY mi?)

Hazırlayan: Claude Code, 2026-10-29. Hakem (Critic) görüşü için. Her olgu sayfa görüntüsünden doğrulanmıştır (etiket **[V]**) ya da varsayımdır (**[VARSAYIM]**).

## 1. Olgular
- **[V] TBDY 7.2.4 (PDF 133 / basılı 112):** "TS 500'de verilen kabuller, betonda oluşan gerilme dağılımı ve elastisite modülünün kullanılması zorunludur" (C50 üstü için TS EN 1992-1). TBDY 7.2.2: hesap kuralları TS 498 yükleri, TS 500, TS 708, TS EN 13670.
- **[V] TBDY (PDF 131 / basılı 110):** Mri, Mrj, Mra, Mrü = "fcd ve fyd'ye göre hesaplanan taşıma gücü momenti" (φ yok).
- **[V] TBDY 7.4.5.3 ve 7.3.7.6:** Vc, TS 500'e göre belirlenir. 7.4.5.1 Denk. 7.9: Ve = Vdy ± (Mpi+Mpj)/ln, Mp ≈ 1,4 Mr.
- **[V] TS 500 (PDF 24 / basılı 17, Denk. 6.2):** fcd = fck/γmc, γmc = 1,5; fyd = fyk/γms, γms = 1,15.
- **[V] TS 500 (PDF 28–29 / basılı 21–22):** εcu = 0,003; eşdeğer blok 0,85 fcd, a = k1 c; Çizelge 7.1: k1 = 0,85 (C16–C25), 0,82 (C30), 0,79 (C35), 0,76 (C40), 0,73 (C45), 0,70 (C50).
- **[V] TS 500 (PDF 36–37):** Vcr = 0,65 fctd bw d (1 + γ Nd/Ac) (Denk. 8.1; γ = 0,07 basınç); Vr = Vc + Vw (8.3); Vc = 0,8 Vcr (8.4); Vw = (Asw/s) fywd d (8.5); depremde sargı bölgelerinde Vc "Afet Bölgelerinde Yapılacak Yapılar Hakkında Yönetmelik" değerleri (yani TBDY).
- **[V] TS 500 (PDF 30):** kirişte ρ − ρ' ≤ 0,85 ρb (7.4), ρ ≤ 0,02 (7.5), boyuna donatı Ø ≥ 12 mm; TBDY 7.4.2.4 de ρ ≤ %2.
- **TS 500 fib/lif içermez.** Lif katkısı yalnız fib MC2010 (Bulletin 56 / MC2010 5.6, 7.7) ile mümkün.
- **[V] ACI 318M-14:** Tablo 20.6.1.3.1 (PDF 337 / basılı 335): hava koşuluna açık olmayan kiriş/kolonda etriye, çirozlar için örtü 40 mm; açıkta 50 mm (No.19–57). 18.6.4.4: kiriş sargı etriyesi s ≤ min(d/4; 6 db; 150), ilk etriye ≤ 50 mm; 18.6.4.6: sargısız bölgede s ≤ d/2. 18.7.5.3: kolon s ≤ min(bmin/4; 6 db; so), so = 100 + (350 − hx)/3 ∈ [100; 150].
- **Üç örneğin kaynakları:** Ö2 (Kaveh benzeri 2B çerçeve) ve Ö3 (Negrin & Chagoyén 2022) ACI 318-14 ile tasarlanmış literatür örnekleri; tez (ST) ACI 318-14 + fib MC2010 kullanmış; Ö1 (özgün, 3B 4 katlı, Antalya DD-2, ZC) TBDY-2018 ile yüklenmiş.
- Programda şu an: kapasiteler ACI 318M-14 (φ ile); TBDY kapasite tasarımı momentleri Mr ise fcd/fyd/k1 (TS 500) ile; kesme Vc/Vs ACI + φ=0,75; örtüler ACI'ye uyuyor (kiriş 60 mm = 40 + 10 + 10; kolon 50 mm, ACI'ye göre 60 olmalı).

## 2. Seçenekler
**A. Tek şartname ACI 318-14 (+ fib MC2010 lif).** Ö2, Ö3 kaynaklarıyla aynı; tez ve ST'nin metniyle tutarlı. Ö1'de TBDY yalnızca yük/spektrum ve kısıtlar (A11) için kullanılır; Mr de φMn ile. Dezavantaj: TBDY 7.2.4 "zorunlu TS 500" ihlali Ö1'de gerekçelendirilmeli; hakem Türkiye'deki bina için TBDY'ye uyumu sorabilir.
**B. Örnek bazlı:** Ö2/Ö3 ACI; Ö1 TS 500 + TBDY (Mr, Vc, Vw, k1, γ) + fib lif. Dezavantaj: iki şartname, programda iki mod, makale daha karmaşık; "lifli/lifsiz" karşılaştırması iki şartnamede ayrı sonuç verir. Avantaj: Ö1 gerçek Türk uygulaması, TBDY'ye tam uyumlu.
**C. Tümü TS 500 + TBDY (+ fib lif).** Ö2/Ö3 kaynak sonuçlarıyla doğrudan karşılaştırma bozulur (kaynaklar ACI); büyük yeniden kodlama.

## 3. Hakeme sorular
1. Hangi seçenek (A/B/C)? Gerekçe: dergi (*Structures*) okuyucusu, tez uyumu, hakem riski, iş yükü.
2. Seçenek A ise Ö1'de TBDY 7.2.4 sapması makalede nasıl gerekçelendirilir ve hangi duyarlılık analizi (ACI ↔ TS 500 Mr ve Vc) gerekir?
3. Vc = 0 anahtarı (Vseis > Ve/2) süreksiz; optimizasyonda nasıl ele alınmalı (TBDY kuralı olduğu gibi mi, yumuşatılmış ceza mı)?
4. Lifli kirişte sarılma bölgesinde Vc = 0 iken lif katkısı VRd,F'nin sıfır alınması (muhafazakâr) kabul edilebilir mi?
5. Açık varsayımlar (orta bölge aralığı, örtüler, φ, Mr tanımı) için minimum gerekçe seti.

## 4. Hakem görüşü ve karar (2026-10-29)
Critic: Seçenek A (hafif B ile: Ö1 final optimumuna TS 500 kontrol hesabı). Soru 3: TBDY Vc=0 kuralı olduğu gibi; Soru 4: lifte sarılma bölgesinde 0 kabul edilebilir, kaynak atfı aranmalı (DOĞRULANAMADI); Soru 5: varsayımlar tablosu. **Kullanıcı kararı (IA, 2026-10-29): A.**
