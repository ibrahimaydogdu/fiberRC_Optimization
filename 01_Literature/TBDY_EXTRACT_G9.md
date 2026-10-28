# 📜 TBDY-2018 Çıkarım Raporu: G9 — Kapasite Tasarımı, Sarılma ve Düğüm Kesmesi

Bu belge; `00_System/GEMINI_TASK_G9_G10.md` görev tanımı ve `.agents/rules/anti_hallucination_evidence.md` protokolü uyarınca `Evidence/DesignGuides/TDBY2018.pdf` (SHA-256 `8d3a9a463d4a534ec2c6834c557b5f706e2ad976c2d6837f6c9b6242e38a6bb2`) birincil kaynağından çıkarılmıştır.

> **Kural Notu:** Bu belgedeki tüm maddeler Gemini tarafından birincil kaynaktan çıkarılmıştır ve durum etiketi **ÇIKARILDI** olarak işaretlenmiştir. İnceleme ve kesin onay yetkisi Claude Code'a aittir.

---

## 📑 1. ÇIKARILAN MADDELER VE KANITLAR

### 1.1. Kolon Sarılma Bölgesi ve Enine Donatı Koşulları (Madde 7.3.4)

- **Yönetmelik Maddesi:** 7.3.4 (7.3.4.1, 7.3.4.2)
- **PDF Sayfa No:** 136, 137 | **Basılı Sayfa No:** 115, 116
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Her bir kolonun alt ve üst uçlarında özel sarılma bölgeleri oluşturulacaktır. Sarılma bölgelerinin her birinin uzunluğu ... kolon serbest yüksekliğinin 1/6’sından, kolon en büyük kesit boyutunun 1.5 katından ve 500 mm’den, daha küçük olmayacaktır."*

#### Sarılma Bölgesi Uzunluğu ($\ell_k$):
$$\ell_k \ge \max\left(\frac{h_n}{6}; \, 1.5 \, b_{\max}; \, 500\text{ mm}\right)$$
- Konsol kolonlarda: $\ell_k \ge 2 \, b_{\max}$ (kolon alt ucunda).
- Temel içinde: en az kolon minimum boyutu kadar yükseklik boyunca sarılma donatısı devam ettirilecektir.

#### Sarılma Bölgesi Enine Donatı Yerleşim Sınırları (Madde 7.3.4.1a):
- Minimum donatı çapı: $\phi \ge 8\text{ mm}$
- Boyuna doğrultuda etriye/çiroz aralığı ($s$):
  $$50\text{ mm} \le s \le \min\left(\frac{b_{\min}}{3}; \, 150\text{ mm}; \, 6 \, \phi_l\right)$$
  *(Burada $\phi_l$: en küçük boyuna donatı çapıdır).*
- Etriye kolları ve çirozlar arası yatay mesafe: $a \le 25 \, \phi_e$
- Spiral adımı: göbek çapının 1/5'inden ve 80 mm'den büyük olmayacaktır.

#### Minimum Enine Donatı Alanı Denklemleri (Madde 7.3.4.1b, c, d):
- **Etriyeli Kolonlarda $N_d / (A_c f_{ck}) > 0.20$ (basınç) olması durumunda (Denk. 7.1):**
  $$A_{sh} \ge 0.30 \, s \, b_k \left[\left(\frac{A_c}{A_{ck}}\right) - 1\right] \left(\frac{f_{ck}}{f_{ywk}}\right)$$
  $$A_{sh} \ge 0.075 \, s \, b_k \left(\frac{f_{ck}}{f_{ywk}}\right)$$
  *(Her iki doğrultu için elverişsiz olanı sağlanacaktır).*
- **Dairesel Donatılı Kolonlarda $N_d / (A_c f_{ck}) > 0.20$ (basınç) olması durumunda (Denk. 7.2):**
  $$\rho_s \ge 0.45 \left[\left(\frac{A_c}{A_{ck}}\right) - 1\right] \left(\frac{f_{ck}}{f_{ywk}}\right)$$
  $$\rho_s \ge 0.12 \left(\frac{f_{ck}}{f_{ywk}}\right)$$
- **$N_d / (A_c f_{ck}) \le 0.20$ olması durumunda (Madde 7.3.4.1d):**
  Denk. (7.1) ve Denk. (7.2) ile verilen enine donatıların en az $2/3$'ü minimum enine donatı olarak kullanılacaktır.

#### Kolon Orta Bölgesi Enine Donatısı (Madde 7.3.4.2):
- Minimum çap: $\phi \ge 8\text{ mm}$
- Aralık ($s$): $s \le \min(b_{\min}/2; \, 200\text{ mm})$
- Kollar arası yatay mesafe: $a \le 25 \, \phi_e$
- **Durum:** `ÇIKARILDI (TDBY-2018, s.115-116, Madde 7.3.4, Denk. 7.1, 7.2)`

---

### 1.2. Kolonların Kesme Güvenliği ve Kapasite Tasarımı ($V_e$) (Madde 7.3.7)

- **Yönetmelik Maddesi:** 7.3.7 (7.3.7.1, 7.3.7.2, 7.3.7.3, 7.3.7.4, 7.3.7.5, 7.3.7.6)
- **PDF Sayfa No:** 139, 140, 141 | **Basılı Sayfa No:** 118, 119, 120
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Kolonlarda enine donatı hesabına esas alınacak kesme kuvveti Ve, Denk. (7.5) ile hesaplanacaktır."*

#### Tasarıma Esas Kesme Kuvveti (Denk. 7.5):
$$V_e = \frac{M_a + M_{\ddot{u}}}{\ell_n}$$
- $\ell_n$: kolonun serbest (net) yüksekliğidir.
- $M_a$ ve $M_{\ddot{u}}$ kolon alt ve üst uç momentleridir.

#### Alt ve Üst Momentlerin Hesabı ($M_a, M_{\ddot{u}}$):
1. **Düğüm noktasında Denk. (7.3) güçlü kolon şartı sağlandığında (Madde 7.3.7.2):**
   Birleşen kirişlerin taşıma gücü moment kapasiteleri toplamı hesaplanır (Denk. 7.6):
   $$\sum M_p = M_{pi} + M_{pj}$$
   *(Daha kesin hesap yapılmadıkça $M_{pi} \approx 1.4 M_{ri}$ ve $M_{pj} \approx 1.4 M_{rj}$ alınabilir).*  
   $\sum M_p$ momenti, düğüm noktasına birleşen kolonların elastik analiz momentleri oranında kolonlara dağıtılır; elde edilen moment ilgili kolonun $M_a$ veya $M_{\ddot{u}}$ momenti olarak alınır. Güvenli tarafta kalmak üzere doğrudan 7.3.7.3'e göre de yapılabilir.
2. **Düğüm noktasında Denk. (7.3) sağlanamadığında (Madde 7.3.7.3):**
   Kolon uç momentleri kolonların moment kapasiteleri olarak hesaplanır:
   $$M_{pa} \approx 1.4 M_{ra}, \qquad M_{p\ddot{u}} \approx 1.4 M_{r\ddot{u}}$$
   *(Moment kapasiteleri hesabında, deprem yönüyle uyumlu olarak bu momentleri en büyük yapan $N_d$ eksenel kuvveti gözönüne alınır).*
3. **Temele bağlanan kolon alt ucu (Madde 7.3.7.4):**
   $M_a$ momenti 7.3.7.3'e göre moment kapasitesi ($1.4 M_{ra}$) olarak hesaplanır.
4. **Üst Sınır / İkincil Kontrol (Madde 7.3.7.1 son paragraf):**
   Düşey yükler ile birlikte $D$ ile artırılmış depremden hesaplanan kesme kuvveti toplamının ($V_d$), Denk. (7.5) ile hesaplanan $V_e$'den küçük olması durumunda $V_e$ yerine bu kesme kuvveti kullanılacaktır ($V_e \le V_d(D)$).

#### Kesme Kuvveti Sınırları ve Kesit Boyut Kontrolü (Denk. 7.7, Madde 7.3.7.5):
$$V_e \ge V_d$$
$$V_e \le V_r$$
$$V_e \le 0.85 \, A_w \sqrt{f_{ck}}$$
- $A_w$: kolon enkesit alanı ($b \times h$).
- İkinci koşul ($0.85 A_w \sqrt{f_{ck}}$) sağlanamazsa, kesit boyutları büyütülecektir.

#### Betonun Kesme Dayanımı Katkısı $V_c$ (Madde 7.3.7.6):
- Kolon sarılma bölgesinde:
  Depremden oluşan kesme kuvveti, toplam depremli kesme kuvvetinin yarısından büyükse ($V_{\text{deprem}} > 0.5 V_{\text{toplam}}$) ve aynı zamanda $N_d / (A_c f_{ck}) \le 0.05$ ise:
  $$V_c = 0$$
  alınacaktır. Aksi durumda $V_c$ TS 500'e göre hesaplanır.
- **Durum:** `ÇIKARILDI (TDBY-2018, s.118-120, Madde 7.3.7, Denk. 7.5, 7.6, 7.7)`

---

### 1.3. Kiriş Sarılma Bölgesi ve Enine Donatı Koşulları (Madde 7.4.4)

- **Yönetmelik Maddesi:** 7.4.4
- **PDF Sayfa No:** 144 | **Basılı Sayfa No:** 123
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Kiriş mesnetlerinde kolon yüzünden itibaren kiriş yüksekliğinin iki katı kadar uzunluktaki bölge, Sarılma Bölgesi olarak tanımlanır. Bu bölge boyunca ... özel deprem etriyeleri kullanılacaktır."*

#### Sarılma Bölgesi Uzunluğu:
$$\ell_{\text{sarılma}} = 2 \, h_k \qquad (h_k: \text{kiriş yüksekliği})$$

#### Sarılma Bölgesi Enine Donatı Kuralları:
- Minimum enine donatı çapı: $\phi \ge 8\text{ mm}$
- İlk etriyenin kolon yüzüne uzaklığı: $\le 50\text{ mm}$
- Etriye aralığı ($s_k$):
  $$s_k \le \min\left(\frac{d}{4}; \, 8 \, \phi_l; \, 150\text{ mm}\right)$$
  *(Burada $d$: kiriş faydalı yüksekliği, $\phi_l$: en küçük boyuna donatı çapıdır).*
- Kiriş eksenine dik doğrultuda etriye kolları aralığı: $\le 350\text{ mm}$
- Sarılma bölgesi dışında: TS 500 enine donatı koşulları geçerlidir.
- **Durum:** `ÇIKARILDI (TDBY-2018, s.123, Madde 7.4.4)`

---

### 1.4. Kirişlerin Kesme Güvenliği ve Kapasite Tasarımı ($V_e$) (Madde 7.4.5)

- **Yönetmelik Maddesi:** 7.4.5 (7.4.5.1, 7.4.5.2, 7.4.5.3)
- **PDF Sayfa No:** 144, 145 | **Basılı Sayfa No:** 123, 124
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Kirişlerde enine donatı hesabına esas alınacak kesme kuvveti, Ve, depremin soldan sağa veya sağdan sola etkimesi durumları için ayrı ayrı ve elverişsiz sonuç verecek şekilde, Denk.(7.9) ile bulunacaktır"*

#### Tasarıma Esas Kesme Kuvveti (Denk. 7.9):
$$V_e = V_{dy} \pm \frac{M_{pi} + M_{pj}}{\ell_n}$$
- $V_{dy}$: düşey yüklerden oluşan basit kiriş kesme kuvveti.
- $\ell_n$: kirişin serbest (net) açıklığı.
- $M_{pi}, M_{pj}$: kiriş uçlarındaki moment kapasiteleri ($M_{pi} \approx 1.4 M_{ri}$, $M_{pj} \approx 1.4 M_{rj}$).
- Düşey yükler ile birlikte $D$ ile artırılmış depremden hesaplanan kesme kuvveti $V_e$'den küçükse, $V_e$ yerine bu kuvvet alınabilir.

#### Kesit Boyut / Ezilme Sınırı (Denk. 7.10):
$$V_e \le V_r$$
$$V_e \le 0.85 \, b_w \, d \sqrt{f_{ck}}$$
*(Bu koşul sağlanamazsa kesit boyutları büyütülecektir).*

#### Betonun Kesme Katkısı $V_c$ (Madde 7.4.5.3):
Kiriş sarılma bölgesinde, sadece depremden oluşan kesme kuvveti depremli durumdaki toplam kesme kuvvetinin yarısından büyükse:
$$V_c = 0$$
alınacaktır. Çerçeve kirişlerinde pilye katkısı kabul edilmez.
- **Durum:** `ÇIKARILDI (TDBY-2018, s.123-124, Madde 7.4.5, Denk. 7.9, 7.10)`

---

### 1.5. Kolon-Kiriş Birleşim Bölgelerinin Kesme Güvenliği (Madde 7.5)

- **Yönetmelik Maddesi:** 7.5 (7.5.1, 7.5.2)
- **PDF Sayfa No:** 146, 147 | **Basılı Sayfa No:** 125, 126
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Gözönüne alınan deprem doğrultusunda kolon-kiriş birleşim bölgelerindeki kesme kuvveti, Denk.(7.11) ile hesaplanacaktır"*

#### Birleşim Sınıflandırması (Madde 7.5.1):
- **Kuşatılmış Birleşim:** Kirişlerin kolona 4 taraftan birleşmesi VE her bir kiriş genişliğinin birleştiği kolon genişliğinin en az 3/4'ü ($b_w \ge 0.75 b$) olması durumu.
- **Kuşatılmamış Birleşim:** Yukarıdaki koşulları sağlamayan tüm birleşimler (kenar, köşe birleşimleri ve en az bir kirişi dar olan iç birleşimler).

#### Birleşim Kesme Kuvveti $V_e$ (Denk. 7.11):
$$V_e = 1.25 \, f_{yk} (A_{s1} + A_{s2}) - V_{\text{kol}}$$
- $A_{s1}$: kirişin bir tarafındaki mesnet üst veya alt çekme donatısı alanı.
- $A_{s2}$: kirişin diğer tarafındaki mesnet alt veya üst çekme donatısı alanı (kolona tek taraftan saplanıyorsa $A_{s2} = 0$).
- $V_{\text{kol}} = \min(V_a; \, V_{\ddot{u}})$: düğüm noktasının hemen üstündeki ve altındaki kolon kesme kuvvetlerinin küçüğüdür.

#### Kesme Dayanımı Üst Sınırları (Denk. 7.12 ve 7.13):
- **(a) Kuşatılmış Birleşimlerde (Denk. 7.12):**
  $$V_e \le 1.7 \, b_j \, h \, \sqrt{f_{ck}}$$
- **(b) Kuşatılmamış Birleşimlerde (Denk. 7.13):**
  $$V_e \le 1.0 \, b_j \, h \, \sqrt{f_{ck}}$$
*(Bu sınırlar aşılırsa kolon ve/veya kiriş boyutları büyütülecektir).*

#### Etkili Birleşim Genişliği $b_j$ Tanımı (Şekil 7.10):
- Kiriş genişliği kolon genişliğinden büyük olamaz kuralı çerçevesinde:
  - $b_{w1} > b_{w2}$ durumunda veya tek kiriş durumunda:
    - $b_{w1} \ge b/2$ ise: $b_j = b$
    - $b_{w1} < b/2$ ise: $b_j = 2 \min(b_1; \, b_2)$
    - Her durumda: $b_j \le b_{w1} + h$

#### Birleşim Bölgesi Enine Donatısı (Madde 7.5.2.3):
- **Kuşatılmış Birleşimlerde:** Alttaki kolonun sarılma bölgesi donatısının en az %40'ı ($\ge 0.40 A_{sh}$); $\phi \ge 8\text{ mm}$, $s \le 150\text{ mm}$.
- **Kuşatılmamış Birleşimlerde:** Alttaki kolonun sarılma bölgesi donatısının en az %60'ı ($\ge 0.60 A_{sh}$); $\phi \ge 8\text{ mm}$, $s \le 100\text{ mm}$.
- **Durum:** `ÇIKARILDI (TDBY-2018, s.125-126, Madde 7.5, Denk. 7.11, 7.12, 7.13)`

---

## 📊 2. DEĞİŞKENLER VE BİRİM TABLOSU

| Sembol | Tanım | Birim | İlgili Denklem |
|---|---|:---:|:---:|
| $f_{ck}$ | Beton karakteristik silindir basınç dayanımı | MPa ($\text{N/mm}^2$) | Denk. 7.1, 7.7, 7.10, 7.12 |
| $f_{ywk}$ | Enine donatının karakteristik akma dayanımı | MPa ($\text{N/mm}^2$) | Denk. 7.1, 7.2 |
| $f_{yk}$ | Boyuna donatının karakteristik akma dayanımı | MPa ($\text{N/mm}^2$) | Denk. 7.11 |
| $A_{sh}$ | $s$ aralığındaki enine donatı toplam kesit alanı | $\text{mm}^2$ | Denk. 7.1 |
| $s$ / $s_k$ | Enine donatı (etriye/çiroz) boyuna aralığı | mm | Denk. 7.1, Şekil 7.3, 7.8 |
| $b_k$ | Kolon çekirdek enkesit boyutu (etriye dışından dışına) | mm | Denk. 7.1, Şekil 7.3 |
| $A_c$ | Kolon brüt enkesit alanı ($b \times h$) | $\text{mm}^2$ | Denk. 7.1, 7.2 |
| $A_{ck}$ | Kolon çekirdek alanı (etriye dış eksenleri ile çevrelenen) | $\text{mm}^2$ | Denk. 7.1, 7.2 |
| $V_e$ | Kapasite tasarımı esas kesme kuvveti | kN / N | Denk. 7.5, 7.9, 7.11 |
| $V_d$ | Düşey yükler ve depremden hesaplanan kesme kuvveti | kN / N | Denk. 7.7 |
| $V_r$ | Kesit kesme dayanımı kapasitesi ($V_c + V_w$) | kN / N | Denk. 7.7, 7.10 |
| $M_{ra}, M_{r\ddot{u}}$ | Kolon uçları taşıma gücü momentleri | $\text{kN}\cdot\text{m}$ | Denk. 7.5 |
| $M_{ri}, M_{rj}$ | Kiriş uçları taşıma gücü momentleri | $\text{kN}\cdot\text{m}$ | Denk. 7.6, 7.9 |
| $\ell_n$ | Eleman net serbest açıklığı / yüksekliği | m / mm | Denk. 7.5, 7.9 |
| $b_j$ | Kolon-kiriş birleşimi etkili genişliği | mm | Denk. 7.12, 7.13 |
| $A_{s1}, A_{s2}$ | Birleşime saplanan kiriş çekme donatısı alanları | $\text{mm}^2$ | Denk. 7.11 |
