# 📜 TBDY-2018 Parametre Çıkarım Raporu (Birincil Kanıt Dosyası)

Bu belge; `Evidence/DesignGuides/TDBY2018.pdf` birincil kaynak belgesinden doğrudan, birebir alıntılar ve sayfa görüntüleri ile doğrulanarak çıkarılmış yatay elastik tasarım spektrumu ve betonarme çerçeve tasarım parametrelerini içerir.

Bu çalışma `.agents/rules/anti_hallucination_evidence.md` protokolü ve `AGENTS.md` kuralları uyarınca hazırlanmıştır.

---

## 📑 1. BİRİNCİL BELGE KÜNYESİ

| Özellik | Değer |
|---|---|
| **Belge Adı** | Türkiye Bina Deprem Yönetmeliği (TBDY-2018) |
| **Resmî Gazete** | 18 Mart 2018, Sayı: 30364 (Mükerrer) |
| **Yürürlük Tarihi** | 01 Ocak 2019 |
| **Yerel Dosya Yolu** | `Evidence/DesignGuides/TDBY2018.pdf` |
| **Dosya Boyutu** | 7,367,331 bayt |
| **Toplam Sayfa** | 416 sayfa (PDF) / 395 sayfa (Basılı metin) |
| **SHA-256 Özeti** | `8d3a9a463d4a534ec2c6834c557b5f706e2ad976c2d6837f6c9b6242e38a6bb2` |
| **Sayfa No Eşlemesi** | Basılı Sayfa No = PDF Sayfa No − 21 (Bölüm 1 basılı sayfa 1 = PDF sayfa 22) |

---

## 🔍 2. ÇIKARILAN PARAMETRELER VE KANIT TABLOSU

### Madde 1: Yatay Elastik Tasarım Spektrumu $S_{ae}(T)$, Köşe Periyotları ($T_A, T_B, T_L$) ve Denklemleri

- **Yönetmelik Maddesi:** Bölüm 2, Madde 2.3.4 (2.3.4.1)
- **PDF Sayfa No:** 30
- **Basılı Sayfa No:** 9
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"yatay elastik tasarım ivme spektrumu’nun ordinatları olan yatay elastik tasarım spektral ivmeleri Sae(T), doğal titreşim periyoduna bağlı olarak yerçekimi ivmesi [g] cinsinden Denk.(2.2)’de tanımlanmıştır"*
- **Denklemler:**
  - **Yatay Elastik Spektral İvme (Denk. 2.2):**
    $$\begin{aligned}
    S_{ae}(T) &= \left(0.4 + 0.6 \frac{T}{T_A}\right) S_{DS} & \quad & (0 \le T \le T_A) \\
    S_{ae}(T) &= S_{DS} & \quad & (T_A \le T \le T_B) \\
    S_{ae}(T) &= \frac{S_{D1}}{T} & \quad & (T_B \le T \le T_L) \\
    S_{ae}(T) &= \frac{S_{D1} T_L}{T^2} & \quad & (T_L \le T)
    \end{aligned}$$
  - **Spektrum Köşe Periyotları (Denk. 2.3):**
    $$T_A = 0.2 \frac{S_{D1}}{S_{DS}}, \qquad T_B = \frac{S_{D1}}{S_{DS}}$$
  - **Sabit Yerdeğiştirme Bölgesine Geçiş Periyodu:**
    $$T_L = 6\text{ s}$$
  - **Yatay Elastik Tasarım Spektral Yerdeğiştirmesi (Madde 2.3.4.2, Denk. 2.4, PDF s. 31, Basılı s. 10):**
    $$S_{de}(T) = \frac{T^2}{4\pi^2} g \, S_{ae}(T)$$
- **Durum:** `DOĞRULANDI (TDBY-2018, s.9-10, Madde 2.3.4, Denk. 2.2, 2.3, 2.4)`

---

### Madde 2: $S_{DS}$, $S_{D1}$, Zemin Katsayıları ($F_S, F_1$), $S_S, S_1$ Bağıntıları ve Deprem Yer Hareketi Düzeyleri (DD-1…DD-4)

- **Yönetmelik Maddeleri:** Madde 2.2 (Deprem Yer Hareketi Düzeyleri), Madde 2.3.2 (Tasarım Spektral İvme Katsayıları), Madde 2.3.3 (Yerel Zemin Etki Katsayıları)
- **PDF Sayfa No:** 28, 29, 30
- **Basılı Sayfa No:** 7, 8, 9

#### 2.1. Deprem Yer Hareketi Düzeyleri (Madde 2.2, PDF s. 28, Basılı s. 7)
1. **DD-1 (Madde 2.2.1):** En büyük deprem yer hareketi; 50 yılda aşılma olasılığı %2, tekrarlanma periyodu 2475 yıl.  
   *Alıntı (≤ 25 kelime):* *"spektral büyüklüklerin 50 yılda aşılma olasılığının %2 ve buna karşı gelen tekrarlanma periyodunun 2475 yıl olduğu çok seyrek deprem yer hareketini nitelemektedir."*
2. **DD-2 (Madde 2.2.2):** Standart tasarım deprem yer hareketi; 50 yılda aşılma olasılığı %10, tekrarlanma periyodu 475 yıl.  
   *Alıntı (≤ 25 kelime):* *"spektral büyüklüklerin 50 yılda aşılma olasılığının %10 ve buna karşı gelen tekrarlanma periyodunun 475 yıl olduğu seyrek deprem yer hareketini nitelemektedir."*
3. **DD-3 (Madde 2.2.3):** Sık deprem yer hareketi; 50 yılda aşılma olasılığı %50, tekrarlanma periyodu 72 yıl.  
   *Alıntı (≤ 25 kelime):* *"spektral büyüklüklerin 50 yılda aşılma olasılığının %50 ve buna karşı gelen tekrarlanma periyodunun 72 yıl olduğu sık deprem yer hareketini nitelemektedir."*
4. **DD-4 (Madde 2.2.4):** Servis deprem yer hareketi; 50 yılda aşılma olasılığı %68 (30 yılda %50), tekrarlanma periyodu 43 yıl.  
   *Alıntı (≤ 25 kelime):* *"spektral büyüklüklerin 50 yılda aşılma olasılığının %68 (30 yılda aşılma olasılığı %50) ve buna karşı gelen tekrarlanma periyodunun 43 yıl olduğu çok sık deprem yer hareketini nitelemektedir."*

#### 2.2. Tasarım Spektral İvme Katsayıları Bağıntısı (Madde 2.3.2.2, Denk. 2.1, PDF s. 29, Basılı s. 8)
- *Alıntı (≤ 25 kelime):* *"harita spektral ivme katsayıları SS ve S1, aşağıdaki şekilde tasarım spektral ivme katsayıları SDS ve SD1’e dönüştürülür"*
$$S_{DS} = S_S F_S, \qquad S_{D1} = S_1 F_1$$
*(Burada $F_S$ ve $F_1$ yerel zemin etki katsayılarıdır; ara değerler için doğrusal enterpolasyon yapılır).*

#### 2.3. Tablo 2.1 – Kısa Periyot Bölgesi İçin Yerel Zemin Etki Katsayıları ($F_S$) (PDF s. 29, Basılı s. 8)

| Yerel Zemin Sınıfı | $S_S \le 0.25$ | $S_S = 0.50$ | $S_S = 0.75$ | $S_S = 1.00$ | $S_S = 1.25$ | $S_S \ge 1.50$ |
|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **ZA** | 0.8 | 0.8 | 0.8 | 0.8 | 0.8 | 0.8 |
| **ZB** | 0.9 | 0.9 | 0.9 | 0.9 | 0.9 | 0.9 |
| **ZC** | 1.3 | 1.3 | 1.2 | 1.2 | 1.2 | 1.2 |
| **ZD** | 1.6 | 1.4 | 1.2 | 1.1 | 1.0 | 1.0 |
| **ZE** | 2.4 | 1.7 | 1.3 | 1.1 | 0.9 | 0.8 |
| **ZF** | \multicolumn{6}{c|}{Sahaya özel zemin davranış analizi yapılacaktır (Bkz. 16.5).} |

#### 2.4. Tablo 2.2 – 1.0 Saniye Periyot İçin Yerel Zemin Etki Katsayıları ($F_1$) (PDF s. 29, Basılı s. 8)

| Yerel Zemin Sınıfı | $S_1 \le 0.10$ | $S_1 = 0.20$ | $S_1 = 0.30$ | $S_1 = 0.40$ | $S_1 = 0.50$ | $S_1 \ge 0.60$ |
|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **ZA** | 0.8 | 0.8 | 0.8 | 0.8 | 0.8 | 0.8 |
| **ZB** | 0.8 | 0.8 | 0.8 | 0.8 | 0.8 | 0.8 |
| **ZC** | 1.5 | 1.5 | 1.5 | 1.5 | 1.5 | 1.4 |
| **ZD** | 2.4 | 2.2 | 2.0 | 1.9 | 1.8 | 1.7 |
| **ZE** | 4.2 | 3.3 | 2.8 | 2.4 | 2.2 | 2.0 |
| **ZF** | \multicolumn{6}{c|}{Sahaya özel zemin davranış analizi yapılacaktır (Bkz. 16.5).} |

- **Durum:** `DOĞRULANDI (TDBY-2018, s.7-9, Madde 2.2, 2.3.2, 2.3.3, Tablo 2.1, Tablo 2.2)`

---

### Madde 3: Yerel Zemin Sınıfları Tanımı (ZA…ZE, ZF)

- **Yönetmelik Maddesi:** Bölüm 16, Madde 16.4 (16.4.1, Tablo 16.1; Bölüm 2 Madde 2.3.3.1 atfı)
- **PDF Sayfa No:** 364
- **Basılı Sayfa No:** 343
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Bölüm 2’ye göre deprem tasarım spektrumlarının tanımlanmasında esas alınacak yerel zemin sınıfları, bu Bölüm’e göre yapılacak zemin araştırmaları ile belirlenecektir."*

#### Tablo 16.1 – Yerel Zemin Sınıfları

| Yerel Zemin Sınıfı | Zemin Cinsi | Üst 30 m Ortalama $V_{S,30}$ [m/s] | Üst 30 m Ortalama $(N_{60})_{30}$ [darbe/30 cm] | Üst 30 m Ortalama $c_{u,30}$ [kPa] |
|:---:|:---|:---:|:---:|:---:|
| **ZA** | Sağlam, sert kayalar | $> 1500$ | – | – |
| **ZB** | Az ayrışmış, orta sağlam kayalar | $760 - 1500$ | – | – |
| **ZC** | Çok sıkı kum, çakıl ve sert kil tabakaları veya ayrışmış, çok çatlaklı zayıf kayalar | $360 - 760$ | $> 50$ | $> 250$ |
| **ZD** | Orta sıkı – sıkı kum, çakıl veya çok katı kil tabakaları | $180 - 360$ | $15 - 50$ | $70 - 250$ |
| **ZE** | Gevşek kum, çakıl veya yumuşak – katı kil tabakaları veya $PI > 20$ ve $w > \%40$ koşullarını sağlayan toplamda 3 metreden daha kalın yumuşak kil tabakası ($c_u < 25\text{ kPa}$) içeren profiller | $< 180$ | $< 15$ | $< 70$ |
| **ZF** | Sahaya özel araştırma ve değerlendirme gerektiren zeminler:<br>1) Deprem etkisi altında çökme ve potansiyel göçme riskine sahip zeminler (sıvılaşabilir zeminler, yüksek derecede hassas killer vb.),<br>2) Toplam kalınlığı 3 metreden fazla turba ve/veya organik içeriği yüksek killer,<br>3) Toplam kalınlığı 8 metreden fazla olan yüksek plastisiteli ($PI > 50$) killer,<br>4) Çok kalın ($> 35\text{ m}$) yumuşak veya orta katı killer. | – | – | – |

- **Durum:** `DOĞRULANDI (TDBY-2018, s.343, Tablo 16.1)`

---

### Madde 4: Deprem Tasarım Sınıfı (DTS) ve Bina Yükseklik Sınıfı (BYS) Tabloları

- **Yönetmelik Maddeleri:** Madde 3.2 (Tablo 3.2) ve Madde 3.3.2 (Tablo 3.3)
- **PDF Sayfa No:** 37, 38
- **Basılı Sayfa No:** 16, 17

#### 4.1. Tablo 3.2 – Deprem Tasarım Sınıfları (DTS) (PDF s. 37, Basılı s. 16)
- *Alıntı (≤ 25 kelime):* *"Deprem Tasarım Sınıfları (DTS), Tablo 3.2’ye göre belirlenecektir."*

| DD-2 Deprem Yer Hareketi Düzeyinde Kısa Periyot Tasarım Spektral İvme Katsayısı ($S_{DS}$) | Bina Kullanım Sınıfı $BKS = 1$ | Bina Kullanım Sınıfı $BKS = 2, 3$ |
|:---|:---:|:---:|
| $S_{DS} < 0.33$ | **DTS = 4a** | **DTS = 4** |
| $0.33 \le S_{DS} < 0.50$ | **DTS = 3a** | **DTS = 3** |
| $0.50 \le S_{DS} < 0.75$ | **DTS = 2a** | **DTS = 2** |
| $0.75 \le S_{DS}$ | **DTS = 1a** | **DTS = 1** |

#### 4.2. Tablo 3.3 – Bina Yükseklik Sınıfları ve Deprem Tasarım Sınıflarına Göre Tanımlanan Bina Yükseklik Aralıkları (PDF s. 38, Basılı s. 17)
- *Alıntı (≤ 25 kelime):* *"Deprem etkisi altında tasarımda binalar yükseklikleri bakımından sekiz Bina Yükseklik Sınıfı’na (BYS) ayrılmıştır."*
- *(Not: Sayfa görüntüsünden doğrulanmıştır: BYS $\ge 4$ için DTS = 3, 3a ile DTS = 4, 4a sütunları birleşmektedir).*

| Bina Yükseklik Sınıfı | $\text{DTS} = 1, 1a, 2, 2a$ [$H_N$ aralığı, m] | $\text{DTS} = 3, 3a$ [$H_N$ aralığı, m] | $\text{DTS} = 4, 4a$ [$H_N$ aralığı, m] |
|:---:|:---:|:---:|:---:|
| **BYS = 1** | $H_N > 70$ | $H_N > 91$ | $H_N > 105$ |
| **BYS = 2** | $56 < H_N \le 70$ | $70 < H_N \le 91$ | $91 < H_N \le 105$ |
| **BYS = 3** | $42 < H_N \le 56$ | $56 < H_N \le 70$ | $56 < H_N \le 91$ |
| **BYS = 4** | $28 < H_N \le 42$ | \multicolumn{2}{c|}{$42 < H_N \le 56$} |
| **BYS = 5** | $17.5 < H_N \le 28$ | \multicolumn{2}{c|}{$28 < H_N \le 42$} |
| **BYS = 6** | $10.5 < H_N \le 17.5$ | \multicolumn{2}{c|}{$17.5 < H_N \le 28$} |
| **BYS = 7** | $7 < H_N \le 10.5$ | \multicolumn{2}{c|}{$10.5 < H_N \le 17.5$} |
| **BYS = 8** | $H_N \le 7$ | \multicolumn{2}{c|}{$H_N \le 10.5$} |

- **Durum:** `DOĞRULANDI (TDBY-2018, s.16-17, Tablo 3.2 ve Tablo 3.3)`

---

### Madde 5: Taşıyıcı Sistem Davranış Katsayısı $R$ ve Dayanım Fazlalığı $D$ (Tablo 4.1)

- **Yönetmelik Maddesi:** Bölüm 4, Madde 4.3 (4.3.2.1, Tablo 4.1 A11 ve A31 satırları)
- **PDF Sayfa No:** 56
- **Basılı Sayfa No:** 35
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"çeşitli süneklik düzeyleri için EK 4A’da tanımlanan Taşıyıcı Sistem Davranış Katsayısı R ve Dayanım Fazlalığı Katsayısı D Tablo 4.1’de verilmiştir."*

#### Tablo 4.1 – Betonarme Çerçeve Sistemleri

| Taşıyıcı Sistem Kodu | Bina Taşıyıcı Sistemi Tanımı | Taşıyıcı Sistem Davranış Katsayısı ($R$) | Dayanım Fazlalığı Katsayısı ($D$) | İzin Verilen Bina Yükseklik Sınıfları ($\text{BYS}$) |
|:---:|:---|:---:|:---:|:---:|
| **A11** | **Süneklik Düzeyi Yüksek:** Deprem etkilerinin tamamının moment aktaran süneklik düzeyi yüksek betonarme çerçevelerle karşılandığı binalar | **8** | **3** | $\text{BYS} \ge 3$ |
| **A31** | **Süneklik Düzeyi Sınırlı:** Deprem etkilerinin tamamının moment aktaran süneklik düzeyi sınırlı betonarme çerçevelerle karşılandığı binalar *(Bkz. 4.3.4.1, 4.3.4.3: sadece DTS = 3 ve DTS = 4 binalarda uygulanabilir)* | **4** | **2.5** | $\text{BYS} \ge 7$ |

- **Durum:** `DOĞRULANDI (TDBY-2018, s.35, Tablo 4.1 A11 ve A31)`

---

### Madde 6: Bina Önem Katsayısı $I$ (Tablo 3.1) ve Bina Kullanım Sınıfı (BKS)

- **Yönetmelik Maddesi:** Bölüm 3, Madde 3.1 (3.1.1, 3.1.2, Tablo 3.1)
- **PDF Sayfa No:** 36
- **Basılı Sayfa No:** 15
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Bina Kullanım Sınıfları’na bağlı olarak Bina Önem Katsayıları Tablo 3.1’de tanımlanmıştır."*

#### Tablo 3.1 – Bina Kullanım Sınıfları ve Bina Önem Katsayıları

| Bina Kullanım Sınıfı | Binanın Kullanım Amacı | Bina Önem Katsayısı ($I$) |
|:---:|:---|:---:|
| **BKS = 1** | **Deprem sonrası kullanımı gereken binalar, insanların uzun süreli ve yoğun olarak bulunduğu binalar, değerli eşyanın saklandığı binalar ve tehlikeli madde içeren binalar:**<br>a) Deprem sonrasında hemen kullanılması gerekli binalar (Hastaneler, dispanserler, sağlık ocakları, itfaiye bina ve tesisleri, PTT ve diğer haberleşme tesisleri, ulaşım istasyonları ve terminalleri, enerji üretim ve dağıtım tesisleri, vilayet, kaymakamlık ve belediye yönetim binaları, ilk yardım ve afet planlama istasyonları)<br>b) Okullar, diğer eğitim bina ve tesisleri, yurt ve yatakhaneler, askeri kışlalar, cezaevleri, vb.<br>c) Müzeler<br>d) Toksik, patlayıcı, parlayıcı, vb. özellikleri olan maddelerin bulunduğu veya depolandığı binalar | **1.5** |
| **BKS = 2** | **İnsanların kısa süreli ve yoğun olarak bulunduğu binalar:**<br>Alışveriş merkezleri, spor tesisleri, sinema, tiyatro, konser salonları, ibadethaneler, vb. | **1.2** |
| **BKS = 3** | **Diğer binalar:**<br>BKS=1 ve BKS=2 için verilen tanımlara girmeyen diğer binalar (Konutlar, işyerleri, oteller, bina türü endüstri yapıları, vb.) | **1.0** |

- **Durum:** `DOĞRULANDI (TDBY-2018, s.15, Tablo 3.1)`

---

### Madde 7: Deprem Yükü Azaltılmış Tasarım Spektrumu $S_{aR}(T)$ ve $R_a(T)$ Bağıntısı

- **Yönetmelik Maddeleri:** Madde 4.4.1 (Denk. 4.8) ve Madde 4.2.1.2 (Denk. 4.1a, 4.1b)
- **PDF Sayfa No:** 52, 61
- **Basılı Sayfa No:** 31, 40

#### 7.1. Azaltılmış Tasarım Spektral İvmesi $S_{aR}(T)$ (Madde 4.4.1, Denk. 4.8, PDF s. 61, Basılı s. 40)
- *Alıntı (≤ 25 kelime):* *"azaltılmış tasarım ivme spektrumu’nun belirli bir T doğal titreşim periyodu için ordinatı olan azaltılmış tasarım spektral ivmesi SaR(T), Denk.(4.8) ile tanımlanmıştır"*
$$S_{aR}(T) = \frac{S_{ae}(T)}{R_a(T)}$$
*(Burada $S_{ae}(T)$, DD-2 deprem yer hareketi için Denk. 2.2 ile belirlenen yatay elastik tasarım spektral ivmesidir).*

#### 7.2. Deprem Yükü Azaltma Katsayısı $R_a(T)$ (Madde 4.2.1.2, Denk. 4.1a, 4.1b, PDF s. 52, Basılı s. 31)
- *Alıntı (≤ 25 kelime):* *"doğrusal elastik deprem yüklerinin azaltılmasında esas alınacak Deprem Yükü Azaltma Katsayısı Ra(T) aşağıdaki şekilde tanımlanmıştır"*
$$\begin{aligned}
R_a(T) &= \frac{R}{I} & \quad & (T > T_B) \\
R_a(T) &= D + \left(\frac{R}{I} - D\right) \frac{T}{T_B} & \quad & (T \le T_B)
\end{aligned}$$
*(Burada $R$ Taşıyıcı Sistem Davranış Katsayısı, $D$ Dayanım Fazlalığı Katsayısı, $I$ Bina Önem Katsayısı, $T$ doğal titreşim periyodu, $T_B$ Denk. 2.3 ile tanımlanan spektrum köşe periyodudur).*

- **Durum:** `DOĞRULANDI (TDBY-2018, s.31, 40, Madde 4.2.1.2 ve 4.4.1)`

---

### Madde 8: Göreli Kat Ötelemesi Sınırları ($\delta_{i,\max} / h_i$ Oranları)

- **Yönetmelik Maddesi:** Bölüm 4, Madde 4.9.1 (4.9.1.1, 4.9.1.2, 4.9.1.3, 4.9.1.4)
- **PDF Sayfa No:** 76, 77
- **Basılı Sayfa No:** 55, 56
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"etkin göreli kat ötelemelerinin kat içindeki en büyük değeri δi,max^(X), aşağıda (a) veya (b)’de verilen koşulları sağlayacaktır."*

#### 8.1. Tanımlar ve Bağıntılar
- **Azaltılmış Göreli Kat Ötelemesi (Madde 4.9.1.1, Denk. 4.32):**
  $$\Delta_i^{(X)} = u_i^{(X)} - u_{i-1}^{(X)}$$
- **Etkin Göreli Kat Ötelemesi (Madde 4.9.1.2, Denk. 4.33):**
  $$\delta_i^{(X)} = \frac{R}{I} \Delta_i^{(X)}$$

#### 8.2. Göreli Kat Ötelemesi Sınır Değerleri (Madde 4.9.1.3, Denk. 4.34a, 4.34b)
- **(a) Gevrek dolgu duvarları çerçeveye tamamen bitişik (esnek derzsiz) ise:**
  $$\frac{\delta_{i,\max}^{(X)}}{h_i} \le 0.008 \, \frac{\kappa}{\lambda} \qquad \text{[Denk. 4.34a]}$$
- **(b) Dolgu duvarları ile çerçeve arasında esnek derz yapılması veya dolgu duvarın bağımsız olması durumunda:**
  $$\frac{\delta_{i,\max}^{(X)}}{h_i} \le 0.016 \, \frac{\kappa}{\lambda} \qquad \text{[Denk. 4.34b]}$$

#### 8.3. Katsayılar (Madde 4.9.1.4)
- **$\kappa$ katsayısı:**
  - Betonarme binalarda: $\kappa = 1.0$
  - Çelik binalarda: $\kappa = 0.5$
- **$\lambda$ katsayısı:**
  - Binanın hakim titreşim periyodu $T$ için DD-3 deprem yer hareketinin elastik tasarım spektral ivmesinin, DD-2 deprem yer hareketinin elastik tasarım spektral ivmesine oranı:
    $$\lambda = \frac{S_{ae}(T, \text{DD-3})}{S_{ae}(T, \text{DD-2})}$$

- **Durum:** `DOĞRULANDI (TDBY-2018, s.55-56, Madde 4.9.1, Denk. 4.32, 4.33, 4.34a, 4.34b)`

---

### Madde 9: Yük Kombinasyonları ($G, Q, E$, Doğrultu Birleştirmesi ve Düşey Deprem)

- **Yönetmelik Maddeleri:** Madde 4.4.2 (Yatay Doğrultu Birleştirmesi), Madde 4.4.3 (Düşey Deprem Etkisi), Madde 4.4.4 (Deprem Etkisinin Diğer Etkilerle Birleştirilmesi)
- **PDF Sayfa No:** 62, 63
- **Basılı Sayfa No:** 41, 42
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"Taşıyıcı sistem elemanlarının tasarımında esas alınmak üzere, deprem etkisini içeren yük birleşimleri Denk.(4.11) ve Denk.(4.12) ile tanımlanmıştır"*

#### 9.1. Yük Birleşimleri (Madde 4.4.4.1, Denk. 4.11, 4.12)
$$\text{Birleşim 1 (Denk. 4.11):} \quad G + Q + 0.2 S + E_d^{(H)} + 0.3 E_d^{(Z)}$$
$$\text{Birleşim 2 (Denk. 4.12):} \quad 0.9 G + H + E_d^{(H)} - 0.3 E_d^{(Z)}$$
*(Burada $G$: sabit yük, $Q$: hareketli yük, $S$: kar yükü, $H$: yatay zemin itkisi, $E_d^{(H)}$: tasarıma esas yatay deprem etkisi, $E_d^{(Z)}$: düşey deprem etkisidir).*

#### 9.2. Yatay Deprem Doğrultu Birleştirmesi (Madde 4.4.2.1, Denk. 4.9)
$$E_d^{(H)} = \pm E_d^{(X)} \pm 0.3 E_d^{(Y)}$$
$$E_d^{(H)} = \pm 0.3 E_d^{(X)} \pm E_d^{(Y)}$$

#### 9.3. Düşey Deprem Etkisi Yaklaşık Hesabı (Madde 4.4.3.2, Denk. 4.10)
Özel düşey spektrum hesabı gerektirmeyen binalarda:
$$E_d^{(Z)} \approx \frac{2}{3} S_{DS} G$$
Bu ifade Denk. 4.11 ve 4.12'ye konulduğunda ($0.3 E_d^{(Z)} \approx 0.2 S_{DS} G$):
$$\text{Birleşim 1:} \quad (1 + 0.2 S_{DS}) G + Q + 0.2 S + E_d^{(H)}$$
$$\text{Birleşim 2:} \quad (0.9 - 0.2 S_{DS}) G + H + E_d^{(H)}$$

#### 9.4. TS 500 İle Karşılaştırma / Atıf Notu
TS 500 (2000) Madde 6.2.5'te deprem yükü birleşimleri $1.0G + 1.0Q + 1.0E$ ve $0.9G + 1.0E$ şeklinde genel biçimde verilmiştir. Ancak TDBY-2018 Madde 4.4.4, deprem hesabı için TS 500'deki bu katsayıların yerine yukarıda verilen ve düşey deprem etkisi $0.3 E_d^{(Z)}$ ile iki doğrultulu yatay birleşimi içeren formülasyonu şart koşar.

- **Durum:** `DOĞRULANDI (TDBY-2018, s.41-42, Madde 4.4.2, 4.4.3, 4.4.4)`

---

### Madde 10: Betonarme Çerçeve Kiriş ve Kolon Boyut ve Donatı Şartları (Bölüm 7)

- **Yönetmelik Maddeleri:** 
  - Süneklik Düzeyi Yüksek: Kolonlar (7.3), Kirişler (7.4)
  - Süneklik Düzeyi Sınırlı: Kolonlar (7.7), Kirişler (7.8)
- **PDF Sayfa No:** 135, 137, 142, 143, 155, 156
- **Basılı Sayfa No:** 114, 116, 121, 122, 134, 135

#### 10.1. Süneklik Düzeyi Yüksek Kolonlar (Madde 7.3)
1. **Minimum Enkesit Boyutu (Madde 7.3.1.1, PDF s. 135, Basılı s. 114):**
   - *Alıntı (≤ 25 kelime):* *"Dikdörtgen kesitli kolonların en küçük enkesit boyutu 300 mm’den ve dairesel kolonların çapı 350 mm’den küçük olmayacaktır."*
   - Dikdörtgen kolon: $b_{\min} \ge 300\text{ mm}$
   - Dairesel kolon: $D \ge 350\text{ mm}$
2. **Eksenel Basınç / Brüt Alan Sınırı (Madde 7.3.1.2, PDF s. 135, Basılı s. 114):**
   $$A_c \ge \frac{N_{dm}}{0.40 f_{ck}} \qquad \left(\text{veya } \frac{N_{dm}}{A_c f_{ck}} \le 0.40\right)$$
   *(Burada $N_{dm}$, $G + Q + E$ ortak etkisi altında hesaplanan eksenel basınç kuvvetlerinin en büyüğüdür).*
3. **Boyuna Donatı Oranı $\rho$ Sınırları (Madde 7.3.2.1, 7.3.2.2, PDF s. 135, Basılı s. 114):**
   - *Alıntı (≤ 25 kelime):* *"Kolonlarda boyuna donatı alanı, brüt kesitin %1’inden az, %4’ünden daha büyük olmayacaktır. Kolonlarda 14φ den daha ince ... donatı kullanılmayacaktır."*
   - Minimum donatı oranı: $\rho_{\min} = 0.01$ (%1)
   - Maksimum donatı oranı: $\rho_{\max} = 0.04$ (%4)
   - Bindirmeli ek bölgesinde toplam donatı oranı: $\rho \le 0.06$ (%6)
   - Minimum donatı çapı: $\phi \ge 14\text{ mm}$ (dairesel kolonlarda en az 6 adet).
4. **Kolonların Kirişlerden Daha Güçlü Olması Koşulu (1.2 Oranı) (Madde 7.3.5.1, Denk. 7.3, PDF s. 137, Basılı s. 116):**
   - *Alıntı (≤ 25 kelime):* *"kolonların taşıma gücü momentlerinin toplamı, o düğüm noktasına birleşen kirişlerin kolon yüzündeki kesitlerindeki taşıma gücü momentleri toplamından en az %20 daha büyük olacaktır"*
   - **Denklem (7.3):**
     $$(M_{ra} + M_{r\ddot{u}}) \ge 1.2 \, (M_{ri} + M_{rj})$$
   - İstisnalar (Madde 7.3.5.3, PDF s. 138-139):
     - Düğüm noktasına birleşen her iki kolonda da $N_d / (A_c f_{ck}) \le 0.10$ ise bu koşul aranmaz.
     - Tek katlı binalarda ve en üst kat düğüm noktalarında aranmaz.
     - Kat bazında izin verilen sapma (Madde 7.3.6.1): $\alpha_i = V_{is} / V_{ik} \ge 0.70$ olması şartıyla bazı kolonlarda sağlanamamasına izin verilebilir.

#### 10.2. Süneklik Düzeyi Yüksek Kirişler (Madde 7.4)
1. **Minimum Enkesit Boyutu (Madde 7.4.1.1, PDF s. 142, Basılı s. 121):**
   - *Alıntı (≤ 25 kelime):* *"Kiriş gövde genişliği en az 250 mm olacaktır. ... Kiriş yüksekliği, döşeme kalınlığının 3 katından ve 300 mm’den daha az olmayacaktır."*
   - Kiriş gövde genişliği: $b_w \ge 250\text{ mm}$
   - Genişlik üst sınırı: $b_w \le h + b_{\text{kolon}}$
   - Kiriş yüksekliği: $h \ge 300\text{ mm}$ ve $h \ge 3 h_f$ ($h_f$: döşeme kalınlığı).
   - Yükseklik/genişlik oranı üst sınırı: $h \le 3.5 b_w$
   - Eksenel basınç sınırı (Madde 7.4.1.2, PDF s. 143): $N_d \le 0.10 A_c f_{ck}$ (aşarsa kolon olarak boyutlandırılır).
2. **Boyuna Donatı Sınırları (Madde 7.4.2, PDF s. 143, Basılı s. 122):**
   - Minimum mesnet çekme donatısı oranı (Madde 7.4.2.1, Denk. 7.8):
     $$\rho \ge 0.8 \, \frac{f_{ctd}}{f_{yd}}$$
   - Donatı çapı ve sürekli donatı (Madde 7.4.2.2): $\phi \ge 12\text{ mm}$, alt ve üstte açıklık boyunca en az 2 çubuk sürekli olacaktır.
   - Alt / Üst donatı oranı (Madde 7.4.2.3): DTS=1, 1a, 2, 2a binalarda kiriş mesnedindeki alt donatı, aynı mesnetteki üst donatının en az %50'si olacaktır ($A_{s,\text{alt}} \ge 0.50 A_{s,\text{üst}}$). Diğer durumlarda en az %30.
   - Maksimum çekme donatısı oranı (Madde 7.4.2.4):
     $$\rho \le 0.02 \quad (\%2) \qquad \text{ve} \qquad \rho \le \rho_{\max} (\text{TS 500})$$

#### 10.3. Süneklik Düzeyi Sınırlı Kolonlar (Madde 7.7)
1. **Enkesit Koşulları (Madde 7.7.1, PDF s. 155, Basılı s. 134):**
   - 7.3.1.1'deki $b \ge 300\text{ mm}$ (dairesel $D \ge 350\text{ mm}$) kuralı geçerlidir.
   - Eksenel gerilme koşulu (Denk. 7.23): $A_c \ge N_{dm} / (0.40 f_{ck})$.
2. **Boyuna Donatı Koşulları (Madde 7.7.2, PDF s. 155, Basılı s. 134):**
   - 7.3.2 koşulları aynen geçerlidir (%1 $\le \rho \le$ %4; $\phi \ge 14\text{ mm}$; ekte $\le$ %6).
3. **Güçlü Kolon Koşulu:**
   - Süneklik düzeyi sınırlı kolonlarda Denk. (7.3) (1.2 şartı) zorunlu DEĞİLDİR; elemanlar Bölüm 4'ten elde edilen ve Dayanım Fazlalığı Katsayısı $D$ ($D=2.5$) ile artırılmış deprem etkilerine göre tasarlanır (Madde 7.7.5.1).

#### 10.4. Süneklik Düzeyi Sınırlı Kirişler (Madde 7.8)
1. **Enkesit Koşulları (Madde 7.8.1, PDF s. 156, Basılı s. 135):**
   - 7.4.1.1 koşulları aynen geçerlidir ($b_w \ge 250\text{ mm}$, $h \ge 300\text{ mm}$, $h \le 3.5 b_w$).
2. **Boyuna Donatı Koşulları (Madde 7.8.2, PDF s. 156, Basılı s. 135):**
   - 7.4.2 koşulları aynen geçerlidir ($\rho \ge 0.8 f_{ctd}/f_{yd}$, $\phi \ge 12\text{ mm}$, mesnette alt donatı üstün en az %50'si [DTS 1, 2] veya %30'u, $\rho \le 0.02$ ve TS 500 sınırı).

- **Durum:** `DOĞRULANDI (TDBY-2018, s.114, 116, 121, 122, 134, 135; Madde 7.3, 7.4, 7.7, 7.8)`

---

## 📊 3. DOĞRULAMA DURUM MATRİSİ

| No | Parametre / Konu | İlgili Madde(ler) | PDF Sayfa | Basılı Sayfa | Doğrulama Durumu |
|:---:|:---|:---|:---:|:---:|:---:|
| 1 | Yatay elastik tasarım spektrumu $S_{ae}(T)$, $T_A, T_B, T_L$, Denk. 2.2-2.3 | 2.3.4 (2.3.4.1) | 30 | 9 | **DOĞRULANDI** |
| 2 | $S_{DS}, S_{D1}$, Zemin etki katsayıları $F_S, F_1$ (Tablo 2.1, 2.2), DD-1…DD-4 | 2.2, 2.3.2, 2.3.3 | 28, 29, 30 | 7, 8, 9 | **DOĞRULANDI** |
| 3 | Yerel zemin sınıfları (ZA…ZE, ZF) tanımı (Tablo 16.1) | 16.4.1, Tablo 16.1 | 364 | 343 | **DOĞRULANDI** |
| 4 | DTS (Tablo 3.2) ve BYS (Tablo 3.3) tabloları | 3.2, 3.3.2 | 37, 38 | 16, 17 | **DOĞRULANDI** |
| 5 | Taşıyıcı sistem $R$ ve $D$ katsayıları (Tablo 4.1 A11, A31) | 4.3, Tablo 4.1 | 56 | 35 | **DOĞRULANDI** |
| 6 | Bina önem katsayısı $I$ (Tablo 3.1) ve BKS | 3.1.1, 3.1.2 | 36 | 15 | **DOĞRULANDI** |
| 7 | Azaltılmış spektrum $S_{aR}(T)$ (Denk. 4.8) ve $R_a(T)$ (Denk. 4.1a,b) | 4.2.1.2, 4.4.1 | 52, 61 | 31, 40 | **DOĞRULANDI** |
| 8 | Göreli kat ötelemesi sınırları ($\delta_{i,\max}/h_i \le 0.008\kappa/\lambda$) | 4.9.1 (Denk. 4.32-4.34) | 76, 77 | 55, 56 | **DOĞRULANDI** |
| 9 | Yük kombinasyonları ($G, Q, E$, Denk. 4.11-4.12, 4.9-4.10) | 4.4.2, 4.4.3, 4.4.4 | 62, 63 | 41, 42 | **DOĞRULANDI** |
| 10 | Çerçeve kolon/kiriş boyut ve donatı şartları, güçlü kolon 1.2 $M_{ra}$ | 7.3, 7.4, 7.7, 7.8 | 135, 137, 142, 143, 155, 156 | 114, 116, 121, 122, 134, 135 | **DOĞRULANDI** |


---

## CLAUDE DOĞRULAMA NOTU (2026-10-28) — Gemini çıkarımının sayfa görüntüsüyle kontrolü
Gemini kendi "DOĞRULANDI" etiketlerini yazdı; Claude aşağıdakileri **PDF sayfa görüntüsünden** (PyMuPDF, 100 dpi) kontrol etti:
| Kalem | PDF / basılı | Sonuç |
|---|---|---|
| Denk. 2.2 (Sae), Denk. 2.3 (TA = 0,2 SD1/SDS; TB = SD1/SDS), TL = 6 s | 30 / 9 | **DOĞRULANDI (görüntü)** |
| SDS = SS·FS, SD1 = S1·F1 (Denk. 2.1); Tablo 2.1 (FS), Tablo 2.2 (F1) | 29 / 8 | **DOĞRULANDI (görüntü; tüm hücreler eşleşti)** |
| Ra(T) Denk. 4.1a,b | 52 / 31 | **DOĞRULANDI (görüntü)** |
| Tablo 4.1 A11 (R=8, D=3, BYS ≥ 3), A31 (R=4, D=2,5, BYS ≥ 7) | 56 / 35 | **DOĞRULANDI (görüntü)** |
| DD-1…DD-4, Tablo 3.1–3.3, Tablo 16.1, SaR (Denk. 4.8), ötelenme (4.32–4.34), kombinasyonlar (4.9–4.12), Bölüm 7 şartları | çeşitli | **Gemini çıkarımı, görüntü kontrolü BEKLİYOR** (Cost/spektrum koduna girmeden önce yapılacak) |
Sayfa ofseti: basılı = PDF − 21 (görüntüyle doğrulandı: PDF 29 → "8", PDF 30 → "9", PDF 52 → "31", PDF 56 → "35").

## Örnek 1 için sonuçlar (tezdeki SDS = 1,2; SD1 = 0,3; I = 1; D = 3; R = 8 ile; Claude hesabı)
- TA = 0,2·0,3/1,2 = **0,05 s**; TB = 0,3/1,2 = **0,25 s**; TL = 6 s. SDS = 1,2 ≥ 0,75 → **DTS = 1** (BKS = 3, I = 1,0). Bina yüksekliği 7 m → BYS = 8 (Tablo 3.3, DTS 1: H ≤ 7). A11 (BYS ≥ 3) izinli.
- R = 8, D = 3 **A11: süneklik düzeyi yüksek çerçeve** demektir → TBDY Bölüm 7.3/7.4 şartları (kolon en küçük boyut ≥ 300 mm; kiriş bw ≥ 250 mm, h ≥ 300 mm; boyuna donatı kolon %1–%4, kiriş ρ ≤ 0,02; güçlü kolon 1,2 oranı) uygulanmalıdır. **Tez kesit havuzu (b = 250 mm, kolon 25×25 sonuçları) bu şartlarla uyumsuz görünüyor** (kolon < 300 mm). Karar gerekli: A11 şartlarını kısıt olarak ekle mi, yoksa R/D'yi düşürüp (A31, R = 4) ya da deprem etkisini kaldırıp mı ilerle?
- Ötelenme sınırı Denk. 4.34a: δ/h ≤ 0,008 κ/λ (κ = 1 betonarme). λ için DD-3 spektrumunun SDS/SD1'i gerekli → **tezde yok (BULUNAMADI)**; tez 1/300 varsayılanını kullanmış (TBDY değil).
- ACI 318-14 Bölüm 18 (özel moment çerçevesi) ile TBDY Bölüm 7 arasında seçim: makalede "ACI 318-14 kesit tasarımı + TBDY-2018 deprem yükü ve çerçeve detay sınırları" birleşimi yazılacaksa hangi detay kuralının bağlayıcı olduğu açıkça belirtilmeli.
