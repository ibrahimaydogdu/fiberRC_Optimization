# A11_CONSTRAINTS.md — Örnek 1 TBDY-2018 süneklik düzeyi yüksek çerçeve (A11: R = 8, D = 3) kısıt listesi

Karar: kullanıcı 2026-10-28 ("A11 şartlarını ekle"). Kaynak: `Evidence/DesignGuides/TDBY2018.pdf` (SHA-256 8d3a9a46…8a6bb2; basılı sayfa = PDF − 21). **Doğrulandı** = Claude sayfa görüntüsünden okudu. Kısıt biçimi: normalize g = talep/kapasite − 1 (`Penalty`); ceza türü = kısıt grubu.

| # | Kısıt | TBDY madde | PDF / basılı | g (ceza) biçimi | Durum |
|---|---|---|---|---|---|
| A1 | Dikdörtgen kolon en küçük kenar ≥ 300 mm | 7.3.1.1 | 135 / 114 | g = 300/min(b,h) − 1 (ayrık havuz; çoğunlukla zorunlu alt sınır) | **DOĞRULANDI** |
| A2 | Kolon eksenel: Ac ≥ Ndm/(0,40 fck) | 7.3.1.2 | 135 / 114 | g = Ndm/(0,40 fck Ac) − 1; Ndm: G+Q+E birleşimi en büyük basınç | **DOĞRULANDI** |
| A3 | Kolon boyuna donatı %1 ≤ ρ ≤ %4; Ø ≥ 14 mm | 7.3.2.1 | 135 / 114 | g = 0,01·Ag/As − 1 ; g = As/(0,04 Ag) − 1 | **DOĞRULANDI** |
| A4 | Bindirmeli ek bölgesinde toplam ρ ≤ %6 | 7.3.2.2 | 135 / 114 | Ek bölgesi detayı modellenmiyorsa raporda "uygulanmadı" yazılır | **DOĞRULANDI** (kod: kapsam dışı notu) |
| A5 | Güçlü kolon–zayıf kiriş: (Mra+Mrü) ≥ 1,2 (Mri+Mrj), her deprem yönü ve işaret için, Nd en küçük | 7.3.5.1, 7.3.5.2, Denk. 7.3 | 137 / 116 | g = 1,2 (Mri+Mrj)/(Mra+Mrü) − 1; istisnalar 7.3.5.3 (üst kat, tek kat, Nd ≤ 0,10 Ac fck) | **DOĞRULANDI** (istisnaların tam metni PDF 138–139'dan ayrıca okunacak) |
| B1 | Kiriş gövde genişliği bw ≥ 250 mm; bw ≤ (h + kolon genişliği) | 7.4.1.1(a) | 142 / 121 | g = 250/bw − 1 ; g = bw/(h+bc) − 1 | **DOĞRULANDI** |
| B2 | h ≥ max(300 mm, 3·hf); h ≤ 3,5 bw | 7.4.1.1(b) | 142 / 121 | g = max(300,3hf)/h − 1 ; g = h/(3,5 bw) − 1 | **DOĞRULANDI** |
| B3 | Kiriş eksenel basınç Nd ≤ 0,10 Ac fck (aksi halde kolon sayılır) | 7.4.1.2 | 143 / 122 | g = Nd/(0,10 Ac fck) − 1 | **DOĞRULANDI** |
| B4 | Mesnet çekme donatısı ρ ≥ 0,8 fctd/fyd (Denk. 7.8) | 7.4.2.1 | 143 / 122 | g = 0,8 fctd/(fyd ρ) − 1 | **DOĞRULANDI** (lifli kiriş için aynen uygulanır; lifin ρmin'i düşürme yetkisi kaynakta yok) |
| B5 | Boyuna donatı Ø ≥ 12 mm; alt ve üstte ≥ 2 sürekli çubuk | 7.4.2.2 | 143 / 122 | ayrık katalogda sağlanır (kontrol) | **DOĞRULANDI** |
| B6 | DTS = 1, 1a, 2, 2a: mesnette alt donatı ≥ %50·üst donatı | 7.4.2.3 | 143 / 122 | g = 0,5·As,üst/As,alt − 1 (Ö1: DTS = 1) | **DOĞRULANDI** |
| B7 | Çekme donatısı oranı ρ ≤ %2 ve TS 500 maksimumu | 7.4.2.4 | 143 / 122 | g = ρ/0,02 − 1; TS 500 sınırı: **TS 500 metni projede yok → BULUNAMADI** (yerine ACI εt ≥ 0,004) | **DOĞRULANDI** (%2); TS 500 kısmı BULUNAMADI |
| C1 | Kolon ve kiriş kesme: kapasite tasarımı (Ve), sarılma bölgesi etriye (7.3.4, 7.4.5…), kolon-kiriş birleşim kesmesi (7.5) | Bölüm 7.3.4, 7.3.7, 7.4.4–7.4.5, 7.5 | — | — | **ÇIKARILMADI** (sıradaki iş; Şekil 7.6'da 1,4 Mr kapasite tasarımı görüldü) |
| C2 | Göreli kat ötelemesi δ ≤ 0,008 κ/λ·h (Denk. 4.34a), δ = (R/I)Δ | 4.9.1 | 76–77 / 55–56 | g = δmax/(0,008 κ h/λ) − 1; λ için DD-3 (SDS, SD1) **tezde yok** → kullanıcı verisi gerekli | Gemini çıkarımı; görüntü kontrolü BEKLİYOR |

## Etkileri
- Kolon havuzu alt sınırı 300 mm olmak zorunda; tezdeki 250 mm kolonlar (Çiz. 4.17/4.18'de "25X25") **A11 ile uyumsuz**.
- Güçlü kolon koşulu (A5) kolon ve kiriş değişkenlerini birbirine bağlar: ceza grubu "A5" eleman düğüm bazında hesaplanır; ETABS final doğrulamasında `Concrete Frame Design` (ACI 318-14 SMF, kolon-kiriş kapasite oranı) ile karşılaştırılır.
- Lifli kirişte ρmin/ρmax ve alt/üst donatı oranı **lifli olmayan referans kurallarla aynen** uygulanır (tezde bunları gevşeten kaynak yok); bu, lifin donatı ikamesini sınırlar ve makalede belirtilmelidir.
- Kapasite tasarımı kesmesi (C1) çıkarılana kadar Ö1 kesme kontrolü yalnız hesap kesme kuvveti ile yapılır (eksik; Critic'e bildirilecek).
