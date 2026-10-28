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
| C1a | Kolon sarılma bölgesi boyu lk ≥ max(ln/6; 1,5 bmax; 500 mm); etriye φ ≥ 8; 50 ≤ s ≤ min(bmin/3; 150; 6 φl); a ≤ 25 φe | 7.3.4.1(a) | 136 / 115 | tasarım detayı; Ash ≥ Denk. 7.1 (Nd > 0,20 Ac fck) ve ≥ 2/3 (aksi) | **DOĞRULANDI** (Gemini G9, Claude görüntü) |
| C1b | Kolon orta bölge: φ ≥ 8, s ≤ min(bmin/2; 200 mm) | 7.3.4.2 | 137 / 116 | detay | **DOĞRULANDI** |
| C1c | Kolon kesme: Ve = (Ma+Mü)/ln (Denk. 7.5); Ma,Mü: 7.3.7.2 (ΣMp = 1,4(Mri+Mrj) dağıtımı) veya 7.3.7.3 (1,4 Mr); Ve ≥ Vd; Ve ≤ Vr; Ve ≤ 0,85 Aw √fck (Denk. 7.7); Vc = 0 koşulları | 7.3.7.1–7.3.7.6 | 139–141 / 118–120 | g = Ve/Vr − 1 ; g = Ve/(0,85 Aw √fck) − 1. **Vr ve Vc TS 500'e göre; bizde ACI/MC2010 (Vc, Vs) kullanılır → makalede belirtilir.** | **DOĞRULANDI** |
| C1d | Kiriş sarılma bölgesi 2 hk; φ ≥ 8; ilk etriye ≤ 50 mm; sk ≤ min(d/4; 8 φl; 150 mm) | 7.4.4 | 144 / 123 | detay | **DOĞRULANDI** |
| C1e | Kiriş kesme: Ve = Vdy ± (Mpi+Mpj)/ln (Denk. 7.9), Mp ≈ 1,4 Mr; Ve ≤ Vr; Ve ≤ 0,85 bw d √fck (Denk. 7.10); sarılma bölgesinde deprem kesmesi ≥ toplamın yarısı ise Vc = 0; pilye katkısı yok | 7.4.5.1–7.4.5.3 | 144–145 / 123–124 | g = Ve/Vr − 1 ; g = Ve/(0,85 bw d √fck) − 1. **Lifli kirişte Vc=0 ve fibre katkısı VRd,F sarılma bölgesinde ele alınmalı: Critic kararı gerekir.** | **DOĞRULANDI** |
| C1f | Birleşim kesmesi: Ve = 1,25 fyk (As1+As2) − Vkol (Denk. 7.11); kuşatılmış Ve ≤ 1,7 bj h √fck (7.12), kuşatılmamış ≤ 1,0 bj h √fck (7.13); enine donatı ≥ %40 (kuşatılmış, s ≤ 150) / ≥ %60 (kuşatılmamış, s ≤ 100) | 7.5.1, 7.5.2 | 146–147 / 125–126 | g = Ve/(1,7 bj h √fck) − 1 (iç, kuşatılmış), 1,0 katsayısı kenar/köşe | **DOĞRULANDI** (not: 7.5 yalnız süneklik düzeyi yüksek çerçeve) |
| A5-ist | A5 muafiyetleri: (a) iki kolonda da Nd ≤ 0,10 Ac fck; (b) tek kat / üst kata devam etmeyen düğüm; (c) perde zayıf doğrultu; 7.3.6: αi = Vis/Vik ≥ 0,70 (Denk. 7.4), aksi halde tüm çerçeveler süneklik düzeyi sınırlı | 7.3.5.3, 7.3.6 | 138–139 / 117–118 | A5 muafiyeti kodda uygulanır (en üst kat düğümleri g = 0) | **DOĞRULANDI** |
| C2 | Göreli kat ötelemesi: λ·δi,max/hi ≤ 0,008 κ (Denk. 4.34a, dolgu duvarı bitişik) veya ≤ 0,016 κ (4.34b); κ = 1 (betonarme); δ = (R/I)Δ (Denk. 4.33), Δ = ui − ui-1 azaltılmış yüklerle (4.32); λ = Sae(T, DD-3)/Sae(T, DD-2) (4.9.1.4) | 4.9.1 | 76–77 / 55–56 | g = λ δmax/(0,008 κ h) − 1. **Muratpaşa ZC: λ ≈ 0,40–0,41 (SDS: 0,294/0,718 = 0,4095; T > TB için SD1: 0,091/0,226 = 0,4027) → sınır δ/h ≈ 0,0195–0,0199 (hesabı T'ye göre kodda yapılır).** | **DOĞRULANDI** (Denk. 4.32–4.34, 4.9.1.4 görüntüden) |

## Etkileri
- Kolon havuzu alt sınırı 300 mm olmak zorunda; tezdeki 250 mm kolonlar (Çiz. 4.17/4.18'de "25X25") **A11 ile uyumsuz**.
- Güçlü kolon koşulu (A5) kolon ve kiriş değişkenlerini birbirine bağlar: ceza grubu "A5" eleman düğüm bazında hesaplanır; ETABS final doğrulamasında `Concrete Frame Design` (ACI 318-14 SMF, kolon-kiriş kapasite oranı) ile karşılaştırılır.
- Lifli kirişte ρmin/ρmax ve alt/üst donatı oranı **lifli olmayan referans kurallarla aynen** uygulanır (tezde bunları gevşeten kaynak yok); bu, lifin donatı ikamesini sınırlar ve makalede belirtilmelidir.
- Kapasite tasarımı kesmesi (C1) çıkarılana kadar Ö1 kesme kontrolü yalnız hesap kesme kuvveti ile yapılır (eksik; Critic'e bildirilecek).
