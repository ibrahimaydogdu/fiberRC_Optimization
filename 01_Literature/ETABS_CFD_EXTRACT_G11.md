# ETABS Concrete Frame Design (CFD) Kılavuzları Çıkarımı (G11)

**Tarih:** 2026-10-30  
**Ajan:** Gemini / Antigravity  
**Kural Seti:** `.agents/rules/anti_hallucination_evidence.md`  
**Amaç:** ETABS Concrete Frame Design motorunun ACI 318-14 ve TS 500-2000(R2018) standartları altında betonarme çerçeve elemanlarını (kolon, kiriş, birleşim bölgesi) nasıl boyutlandırdığını, kapasite ve D/C oranlarını hangi analitik formülasyonlarla hesapladığını birincil kılavuz belgelerinden sayfa sayfa belgelemek.

---

## 📁 İncelenen Kaynak Belgeler

| Belge | Dosya Yolu | SHA-256 | Bayt |
|---|---|---|---|
| **ETABS CFD ACI 318-14** | `Evidence/DesignGuides/CFD-ACI-318-14.pdf` | `6e9e44322cbc5bc8125ed8c63e28268c818741100b01f1b19f21c63357e689eb` | 3 086 351 |
| **ETABS CFD TS 500-2000** | `Evidence/DesignGuides/CFD-TS-500-2000(R 2018).pdf` | `43c4d40b1e7fee8218e95e4f67821edcbbd661261c2d5ecedb7fcb1b2224836e` | 2 362 396 |

---

# BÖLÜM A: ACI 318-14 KILAVUZU ÇIKARIMI (`CFD-ACI-318-14.pdf`)

### A.1 Tasarım Kombinasyonları ve $\phi$ Katsayıları
- **Bölüm & Madde:** Chapter 2.1, 2.2, 3.2; ACI 5.3.1, Table 5.3.1, Table 21.2.1, Table 21.2.2, ASCE 7-10 12.4.2.3
- **Sayfa:** PDF 11–13, 21–23, 27–29 (Basılı: 2-1 – 2-3, 3-4 – 3-6, 3-10 – 3-12)
- **Birebir Alıntı (≤25 kelime):**  
  > *"When the section is tension controlled, a φ factor for tension control is used... When the section is within the transition region, φ is linearly interpolated"* (PDF s. 28, Basılı 3-11)  
  > *"The reliability factor, ρ, and DL multiplier are automatically applied to all program default design combinations when the ACI 318-14 code is selected."* (PDF s. 13, Basılı 2-3)
- **Değişkenler ve Birimler:**  
  - $D$: Ölü yük (Dead load) [lb veya kN]  
  - $L$: Hareketli yük (Live load) [lb veya kN]  
  - $Lr$: Çatı hareketli yükü (Roof live load) [lb veya kN]  
  - $S$: Kar yükü (Snow load) [lb veya kN]  
  - $W$: Rüzgâr yükü (Wind load) [lb veya kN]  
  - $E$: Deprem yükü (Earthquake load) [lb veya kN]  
  - $S_{DS}$: Kısa periyot tasarım spektral ivme katsayısı [boyutsuz]  
  - $\rho$: ASCE 7-10 güvenilirlik (yedeklilik) katsayısı (reliability factor) [boyutsuz]  
  - $\varepsilon_t$: En dış çekme donatısı net çekme birim şekildeğiştirmesi [in/in veya mm/mm]  
  - $\varepsilon_y = f_y / E_s$: Donatı akma birim şekildeğiştirmesi [in/in veya mm/mm]  
  - $\phi_t$: Çekme kontrollü durum için dayanım azaltma katsayısı (varsayılan: 0,90) [boyutsuz]  
  - $\phi_c$: Basınç kontrollü durum için dayanım azaltma katsayısı (bağlantılı/etriyelide: 0,65; spiralli/fretlide: 0,75) [boyutsuz]  
  - $\phi_s$: Kesme ve burulma için dayanım azaltma katsayısı (normal: 0,75; sismik sünek çerçeve faktörlü yükleme: 0,60; kapasite kesmesinde: 0,75; birleşim kesmesinde: 0,85) [boyutsuz]
- **ETABS Yük Kombinasyonları:**  
  - $1.4D$ (ACI 5.3.1a)  
  - $1.2D + 1.6L + 0.5(L_r \text{ veya } S)$ (ACI 5.3.1b)  
  - $1.2D + 1.0L + 1.6(L_r \text{ veya } S)$ (ACI 5.3.1c)  
  - $1.2D + 1.6(0.75PL) + 0.5L_r$ (ACI 5.3.1b, 6.4)  
  - $0.9D \pm 1.0W$ (ACI 5.3.1f)  
  - $1.2D + 1.0L + 0.5(L_r \text{ veya } S) \pm 1.0W$ (ACI 5.3.1d)  
  - $1.2D + 1.6(L_r \text{ veya } S) \pm 0.5W$ (ACI 5.3.1c)  
  - $0.9D \pm 1.0E$ (ACI 5.3.1g) ve $1.2D + 1.0L + 0.2S \pm 1.0E$ (ACI 5.3.1e)  
  - Özel sismik etki devredeyken (ASCE 7-10 12.4.2.3): $(0.9 - 0.2S_{DS})D \pm \rho E$, $(1.2 + 0.2S_{DS})D + 1.0L \pm \rho E$, $(1.2 + 0.2S_{DS})D + 1.0L + 0.2S \pm \rho E$
- **$\phi$ Geçiş Fonksiyonu:**  
  $$\phi = \begin{cases} \phi_c & \varepsilon_t \le \varepsilon_y \\ \phi_c + (\phi_t - \phi_c)\frac{\varepsilon_t - \varepsilon_y}{0.005 - \varepsilon_y} & \varepsilon_y < \varepsilon_t < 0.005 \\ \phi_t & \varepsilon_t \ge 0.005 \end{cases}$$
- **Durum:** **ÇIKARILDI**

---

### A.2 Kolon P–M–M Etkileşimi ve D/C Oranı Tanımı
- **Bölüm & Madde:** Chapter 3.4.1, 3.4.2, 3.4.2.3; ACI 22.2, 22.4.2, 21.2.2
- **Sayfa:** PDF 24–29, 32–33 (Basılı: 3-7 – 3-12, 3-15 – 3-16)
- **Birebir Alıntı (≤25 kelime):**  
  > *"Point C is defined as the point where the line OL (if extended outwards) will intersect the failure surface... The capacity ratio, CR, is given by the ratio OL / OC."* (PDF s. 32, Basılı 3-15)  
  > *"The biaxial column capacity check is based on the generation of consistent three-dimensional interaction surfaces. It does not use any empirical formulations that extrapolate uniaxial interaction curves"* (PDF s. 9, Basılı 1-2)
- **Değişkenler ve Birimler:**  
  - $P_u$: Tasarım eksenel yükü [lb veya kN] (basınç pozitif)  
  - $M_{u2}, M_{u3}$: Büyütülmüş iki eksenli tasarım eğilme momentleri [lb-in veya kN-m]  
  - Nokta $L$: Yük noktası koordinatları $(P_u, M_{u2}, M_{u3})$  
  - Nokta $O$: Etkileşim uzayının orijini $(0, 0, 0)$  
  - Nokta $C$: Orijinden yük noktası $L$'ye uzanan doğrunun 3B kırılma (kapasite) yüzeyini kestiği nokta  
  - $CR = OL / OC$: Kolon kapasite oranı (Capacity Ratio / D/C) [boyutsuz]  
  - $P_0 = 0.85 f'_c (A_g - A_{st}) + f_y A_{st}$: Sıfır dışmerkezlik eksenel kapasitesi [lb veya kN]  
  - $\phi P_{n,\max}$: Maksimum eksenel tasarım yükü; etriyeli/bağlantılı için $0.80 \phi P_0$, spiralli/fretli için $0.85 \phi P_0$ (burada $\phi = \phi_c = 0.65$ veya $0.75$) [lb veya kN]  
  - Donatı oranı sınırları: Ordinary/Intermediate için %1–%8 (ACI 10.6.1.2); Special için %1–%6 (ACI 18.7.4.1)
- **D/C Oranı Yöntemi:** Radyal Işın Yöntemi (Radial Ray Method). D/C oranı, bileşke yük vektörünün orijine mesafesinin ($OL$), aynı eksantriklik doğrultusundaki sınır kapasite vektörü uzunluğuna ($OC$) oranıdır ($CR = OL/OC$). Tekil eksenel veya tekil moment oranı değil, 3 boyutlu uzayda tam radyal orandır.
- **Durum:** **ÇIKARILDI**

---

### A.3 Narinlik ve Moment Büyütme
- **Bölüm & Madde:** Chapter 2.8, 3.4.2.1, 3.4.2.2, Appendix A, Appendix B; ACI 6.6.4.1, 6.6.4.4.2, 6.6.4.5, 6.6.4.6
- **Sayfa:** PDF 16, 29–32, 77–80 (Basılı: 2-6, 3-12 – 3-15, A-1 – B-2)
- **Birebir Alıntı (≤25 kelime):**  
  > *"The factor δs is the moment magnification factor for moments causing sidesway. The program takes this factor to be 1 because... assumed to be obtained from a second order elastic (P-Δ) analysis"* (PDF s. 30, Basılı 3-13)  
  > *"The factored moments are further increased, if required, to obtain minimum eccentricities of (0.6 + 0.03h) inches... applied in only one direction at a time."* (PDF s. 29–30, Basılı 3-12 – 3-13)
- **Değişkenler ve Birimler:**  
  - $e_{\min} = 0.6 + 0.03h$ [in] (minimum dışmerkezlik; tek seferde yalnız tek bir doğrultuda uygulanır)  
  - $M_{\min} = P_u (0.6 + 0.03h)$ [lb-in]  
  - $M = M_{ns} + \delta_s M_s$: İkinci mertebe toplam moment ($\delta_s = 1.0$ kabul edilir, çünkü ETABS yapıda $P-\Delta$ analizinin zaten çalıştırıldığını varsayar)  
  - $M_c = \delta_{ns} M$: Bireysel kolon kararlılığı büyütülmüş tasarım momenti  
  - $\delta_{ns} = \frac{C_m}{1 - P_u / (0.75 P_c)} \ge 1.0$: Non-sway moment büyütme katsayısı ($\delta_{ns} > 1.4$ ise uyarı verir; $P_u \ge 0.75 P_c$ ise göçme ilan edilir)  
  - $C_m = 0.6 + 0.4 (M_a / M_b) \ge 0.4$ (açıklıkta enine yük yoksa; enine yük varsa veya uzunluk kullanıcı tarafından ezilmişse $C_m = 1.0$)  
  - $P_c = \frac{\pi^2 EI_{\text{eff}}}{(k l_u)^2}$: Euler burkulma yükü; $k$ varsayılan olarak $1.0$ alınır (kullanıcı değiştirebilir); $EI_{\text{eff}} = \frac{0.4 E_c I_g}{1 + \beta_{dns}}$
- **Durum:** **ÇIKARILDI**

---

### A.4 Kiriş Eğilme Tasarımı
- **Bölüm & Madde:** Chapter 2.5, 3.5.1, 3.5.1.2; ACI 9.3.3.1, 9.6.1.2, 9.6.1.3, 18.6.3.1, 18.6.3.2, 22.2
- **Sayfa:** PDF 14, 46–55 (Basılı: 2-4, 3-29 – 3-38)
- **Birebir Alıntı (≤25 kelime):**  
  > *"For simplicity, the program enforces εs,min = 0.005... When the applied moment exceeds the moment capacity at this design condition, the area of compression reinforcement is calculated"* (PDF s. 47–48, Basılı 3-30 – 3-31)  
  > *"It is assumed that the design ultimate axial force does not exceed 0.10 φ f'c Ag; hence, all of the beams are designed ignoring axial force."* (PDF s. 47, Basılı 3-30)
- **Değişkenler ve Birimler:**  
  - $M_u$: Eksenel kuvvet ihmal edilerek hesaplanan tasarım eğilme momenti [lb-in veya kN-m]  
  - Negatif momentte daima Dikdörtgen kesit ($b_w \times h$), pozitif momentte Dikdörtgen veya T-kesit ($b_f, b_w, h, d_s$)  
  - Eşdeğer basınç bloğu derinliği: $a = d - \sqrt{d^2 - \frac{2 |M_u|}{0.85 \phi f'_c b}}$ ($\phi = 0.90$)  
  - Sınır derinlik: $c_{\max} = \frac{\varepsilon_{c,\max}}{\varepsilon_{c,\max} + \varepsilon_{s,\min}} d = \frac{0.003}{0.003 + 0.005} d = \frac{3}{8} d = 0.375 d$; $a_{\max} = \beta_1 c_{\max}$  
  - $a \le a_{\max}$ ise tek donatılı: $A_s = \frac{M_u}{\phi f_y (d - a/2)}$  
  - $a > a_{\max}$ ise çift donatılı: $M_{uc} = \phi C_c (d - a_{\max}/2)$, $M_{us} = M_u - M_{uc}$, $A'_s = \frac{M_{us}}{\phi (f'_s - 0.85 f'_c)(d - d')}$, $A_s = A_{s1} + A_{s2}$  
  - Donatı sınırları:  
    - Minimum çekme donatısı: $A_{s,\min} = \max\left(\frac{3\sqrt{f'_c}}{f_y} b_w d, \, \frac{200}{f_y} b_w d\right)$ veya $\frac{4}{3} A_{s,\text{req}}$ (ACI 9.6.1.2, 9.6.1.3)  
    - Maksimum donatı: $\rho \le 0.04$ (Ordinary/Intermediate); Special Moment Frames için $\rho \le 0.025$ (ACI 18.6.3)  
    - Mesnet alt/üst donatı oranı: $M_n^{(+)} \ge 0.5 M_n^{(-)}$ (Special, ACI 18.6.3.2); Intermediate için $M_n^{(+)} \ge \frac{1}{3} M_n^{(-)}$  
    - Açıklık moment oranı: Herhangi bir kesitteki kapasite $\ge \frac{1}{4} \max(M_{n,\text{end}}^{(+)}, M_{n,\text{end}}^{(-)})$ (Special); Intermediate için $\ge \frac{1}{5}$
- **Durum:** **ÇIKARILDI**

---

### A.5 Kesme ve Sismik Kapasite Kesmesi Tasarımı
- **Bölüm & Madde:** Chapter 3.4.4, 3.5.2; ACI 18.6.5.1, 18.6.5.2, 18.7.6.1, 18.7.6.2, 21.2.4, 22.5
- **Sayfa:** PDF 34–45, 55–60 (Basılı: 3-17 – 3-28, 3-38 – 3-43)
- **Birebir Alıntı (≤25 kelime):**  
  > *"The maximum probable moment strengths are determined using a strength reduction factor of 1.0 and the reinforcing steel stress equal to α fy, where α is equal to 1.25"* (PDF s. 37, 56–57; Basılı 3-20, 3-39 – 3-40)  
  > *"for Special Moment Frame design, if... Pu < 0.05 Ag f'c, if... VE ≥ 0.5 Vu... within a distance lo... Vc is taken as zero"* (PDF s. 42, 58; Basılı 3-25, 3-41)
- **Değişkenler ve Birimler:**  
  - $V_u$: Tasarım kesme kuvveti [lb veya kN]  
  - Beton kesme dayanımı (kiriş): $V_c = 2 \lambda \sqrt{f'_c} b_w d$ [lb]  
  - Beton kesme dayanımı (kolon eksenel basınç): $V_c = 2 \left(1 + \frac{P_u}{2000 A_g}\right) \lambda \sqrt{f'_c} A_{cv} \le 3.5 \lambda \sqrt{f'_c} A_{cv} \sqrt{1 + \frac{P_u}{500 A_g}}$ [lb, psi]  
  - Maksimum kesme sınırı: $V_n \le V_c + 8 \sqrt{f'_c} b_w d$ (aşılırsa kesit yetersiz hatası)  
  - Gerekli etriye: $V_u \le \frac{1}{2} \phi V_c$ ise $A_v/s = 0$; $\frac{1}{2} \phi V_c < V_u \le \phi V_{n,\max}$ ise $\frac{A_v}{s} = \frac{V_u - \phi V_c}{\phi f_{yt} d} \ge \max\left(0.75\sqrt{f'_c}\frac{b_w}{f_{yt}}, \, 50\frac{b_w}{f_{yt}}\right)$  
  - **Sismik Kapasite Kesmesi (Special Moment Frame):**  
    - Kiriş: $V_e = V_{D+L} + \frac{M_{pr,I} + M_{pr,J}}{L_n}$; burada $M_{pr}$ hesaplanırken $\phi = 1.0$ ve çelik gerilmesi $1.25 f_y$ alınır ($\alpha = 1.25$).  
    - Kolon: $V_e = \min(V_{e,\text{col}}, V_{e,\text{beam}}) \ge V_{u,\text{factored}}$; $V_{e,\text{col}} = \frac{M_{pr,\text{top}} + M_{pr,\text{bot}}}{H_n}$ ($1.25 f_y$ ve eksenel yüke bağlı $M_{pr}$ ile); $V_{e,\text{beam}} = \frac{\sum M_{pr,\text{beam}}}{H}$.  
    - **$V_c = 0$ Koşulu:** $P_u < \frac{A_g f'_c}{20}$ VE $V_E \ge 0.5 V_u$ VE sarılma bölgesi içinde ($l_0 = \max(h, L_n/6, 18\text{ in})$) ise $V_c = 0$ alınır.  
    - $\phi$ katsayısı: Kapasite kesmesi $V_e$ kullanılırken normal $\phi_s = 0.75$; sismik kombinasyonun salt faktörlü kesmesi kullanılıyorsa $\phi_{s,\text{seismic}} = 0.60$.
- **Durum:** **ÇIKARILDI**

---

### A.6 Burulma Tasarımı
- **Bölüm & Madde:** Chapter 3.5.3; ACI 9.6.4, 21.2.1, 22.7
- **Sayfa:** PDF 60–67 (Basılı: 3-43 – 3-50)
- **Birebir Alıntı (≤25 kelime):**  
  > *"If the factored torsion Tu is less than the threshold limit, φ Tth, torsion can be safely ignored... If Tu > φ Tth, the required longitudinal rebar area is calculated"* (PDF s. 66, Basılı 3-49)
- **Değişkenler ve Birimler:**  
  - Eşik burulma: $T_{th} = \lambda \sqrt{f'_c} \left(\frac{A_{cp}^2}{p_{cp}}\right) \sqrt{1 + \frac{P_u}{4 A_{cp} \lambda \sqrt{f'_c}}}$ [lb-in]  
  - Çatlama burulması: $T_{cr} = 4 \lambda \sqrt{f'_c} \left(\frac{A_{cp}^2}{p_{cp}}\right) \sqrt{1 + \frac{P_u}{4 A_{cp} \lambda \sqrt{f'_c}}}$ [lb-in]  
  - $T_u \le \phi T_{th}$ ise burulma ihmal edilir ($\phi = 0.75$).  
  - $T_u > \phi T_{th}$ ise gerekli kapalı etriye: $\frac{A_t}{s} = \frac{T_u}{2 \phi A_o f_{yt} \cot\theta}$ ($\theta = 45^\circ$, $A_o = 0.85 A_{oh}$)  
  - Gerekli boyuna burulma donatısı: $A_l = \frac{T_u p_h}{2 \phi A_o f_y} \cot^2\theta$  
  - Kesit ezilme sınırı: $\sqrt{\left(\frac{V_u}{b_w d}\right)^2 + \left(\frac{T_u p_h}{1.7 A_{oh}^2}\right)^2} \le \phi \left(\frac{V_c}{b_w d} + 8\sqrt{f'_c}\right)$
- **Durum:** **ÇIKARILDI**

---

### A.7 Örtü ve $d'$ Varsayımları
- **Bölüm & Madde:** Chapter 2.5, 3.1, 3.5.3.2; ACI 20.6.1.3.1
- **Sayfa:** PDF 14, 20, 61 (Basılı: 2-4, 3-3, 3-44)
- **Birebir Alıntı (≤25 kelime):**  
  > *"d' Concrete cover to center of reinforcing, in"* (PDF s. 20, Basılı 3-3, Notation tablosu)  
  > *"it is assumed that the distance between the centerline of the outermost closed stirrup and the outermost concrete surface is 1.75 inches."* (PDF s. 61, Basılı 3-44)
- **Değişkenler ve Birimler:**  
  - $d'$: Beton dış yüzeyinden boyuna donatının ağırlık merkezine olan mesafe [in veya mm] (Preferences veya section/beam designer üzerinden girilir)  
  - $d = h - d'$: Faydalı yükseklik [in veya mm]  
  - Burulma özel kesit hesabı ($A_{oh}, p_h$) için açık varsayım: Kılavuz, kapalı etriye merkez ekseni ile beton dış yüzeyi arasındaki mesafeyi $c = 1.75\text{ in}$ ($44.5\text{ mm}$) olarak kabul eder (bu değer $1.5\text{ in}$ net paspayı + #4 etriye yarıçapı varsayımına dayanır).  
  - Preferences listesi: Kılavuz metninde Concrete Design Preferences tablosunun tam ekran görüntüsü veya bağımsız tablo listesi yer almamaktadır; parametreler metin içinde (Time History, Pattern Live Load Factor, Special Seismic, $\phi$ katsayıları) tek tek anılmaktadır.
- **Durum:** **ÇIKARILDI**

---

### A.8 Sünek Çerçeve (Special Moment Frame) Kontrolleri
- **Bölüm & Madde:** Chapter 3.6, 3.6.4, 3.7; ACI 18.7.3.2, 18.8.2.1, 18.8.4.1
- **Sayfa:** PDF 68–75 (Basılı: 3-51 – 3-58)
- **Birebir Alıntı (≤25 kelime):**  
  > *"The program calculates the ratio of the sum of the beam moment capacities to the sum of the column moment capacities... If this ratio is greater than 5/6, a warning message is printed"* (PDF s. 72, 74; Basılı 3-55, 3-57)
- **Değişkenler ve Birimler:**  
  - Güçlü Kolon – Zayıf Kiriş kuralı: $\sum M_{nc} \ge \frac{6}{5} \sum M_{nb} \iff \frac{\sum M_{nb}}{\sum M_{nc}} \le \frac{5}{6} \approx 0.833$ (ACI 18.7.3.2)  
    - Moment kapasiteleri kolon ve kiriş düğüm yüzlerinde ($1.0 f_y$ ve $\phi = 1.0$) hesaplanır.  
    - Kolon moment kapasitesi, incelenen kombinasyonun eksenel kuvveti ($P_u$) altında tek eksenli etkileşim diyagramından okunur.  
    - ETABS raporda hem $\frac{6}{5}\frac{\sum M_{nb}}{\sum M_{nc}}$ hem de $\frac{\sum M_{nc}}{\sum M_{nb}}$ oranlarını basar; oran $5/6$'yı aşarsa çıktıya uyarı mesajı ekler.  
  - Birleşim Bölgesi Kesmesi (Panel Zone Shear):  
    - $V_u^h = T_L + C_R - V_u^{\text{top}} = T_L + T_R - V_u^{\text{top}}$; burada $T = 1.25 f_y A_s$ ($\alpha = 1.25, \phi = 1.0$).  
    - Kolon kesmesi: $V_u^{\text{top}} = \frac{M_{u,L} + M_{u,R}}{H}$.  
    - Dayanım kontrolü: $v_u = \frac{V_u^h}{A_j} \le \phi \gamma \lambda \sqrt{f'_c}$; $\phi = 0.85$.  
    - $\gamma$ katsayısı: 4 tarafı kuşatılmış birleşimde $20$; 3 yüzde veya karşılıklı 2 yüzde kuşatılmışta $15$; diğerlerinde $12$ (kiriş genişliği kolon yüz genişliğinin $\ge 3/4$'ünü örtüyorsa kuşatılmış sayılır).
- **Durum:** **ÇIKARILDI**

---

### A.9 Çıktı Tabloları ve OAPI Bilgisi
- **Bölüm & Madde:** Kılavuz genel taraması
- **Sayfa:** Kılavuz metninde DatabaseTables / OAPI fonksiyon listesi bulunmamaktadır.
- **Durum:** **BULUNAMADI**  
  *(Açıklama: CSI Concrete Frame Design kılavuzları algoritmik tasarım elkitabıdır; OAPI fonksiyon adları `GetSummaryResultsColumn`, `GetSummaryResultsBeam` ve DatabaseTables çıktı şemaları bu belgede yer almaz; bunlar "CSI OAPI Help / ETABS Database Documentation" belgelerinde yer almaktadır.)*

---
---

# BÖLÜM B: TS 500-2000(R2018) KILAVUZU ÇIKARIMI (`CFD-TS-500-2000(R 2018).pdf`)

### B.1 Tasarım Kombinasyonları ve Malzeme Güvenlik Katsayıları
- **Bölüm & Madde:** Chapter 2.1, 3.2, 3.4; TS 500 Madde 6.2.5, 6.2.6, 6.3, 6.5, 6.6, 6.7, 6.8
- **Sayfa:** PDF 10–11, 21–23 (Basılı: 2-1 – 2-2, 3-5 – 3-7)
- **Birebir Alıntı (≤25 kelime):**  
  > *"Partial safety factor for steel, γms = 1.15, and Partial safety factor for concrete, γmc = 1.5... These factors are already incorporated in the design equations"* (PDF s. 23, Basılı 3-7)
- **Değişkenler ve Birimler:**  
  - $G$: Zati / sabit ölü yük [kN veya N]  
  - $Q$: Hareketli yük [kN veya N]  
  - $W$: Rüzgâr yükü [kN veya N]  
  - $E$: Deprem yükü [kN veya N]  
  - $\gamma_{mc} = 1.5$: Beton malzeme katsayısı [boyutsuz] ($f_{cd} = f_{ck} / 1.5$)  
  - $\gamma_{ms} = 1.15$: Çelik malzeme katsayısı [boyutsuz] ($f_{yd} = f_{yk} / 1.15$)  
  - $\phi$ katsayısı: TS 500 tasarımında ACI tipi $\phi$ katsayısı YOKTUR ($\phi = 1.0$), güvenlik malzeme katsayıları $\gamma_m$ ile sağlanır.
- **ETABS TS 500 Varsayılan Kombinasyonları:**  
  - $1.4G + 1.6Q$ (TS 6.3)  
  - $1.0G + 1.3Q \pm 1.3W$ (TS 6.5)  
  - $0.9G \pm 1.3W$ (TS 6.6)  
  - $1.0G + 1.0Q \pm 1.0E$ (TS 6.7)  
  - $0.9G \pm 1.0E$ (TS 6.8)  
  *(Not: Spektral deprem analizinde işaret permutations olarak 8 adet alt kombinasyon otomatik türetilir.)*
- **Durum:** **ÇIKARILDI**

---

### B.2 Kolon P–M–M Etkileşimi ve D/C Oranı Tanımı
- **Bölüm & Madde:** Chapter 3.5.1, 3.5.2, 3.5.2.3; TS 500 Madde 7.1, 7.4.1
- **Sayfa:** PDF 24–27, 30–31 (Basılı: 3-8 – 3-11, 3-14 – 3-15)
- **Birebir Alıntı (≤25 kelime):**  
  > *"Point C is defined as the point where the line OL (if extended outwards) will intersect the failure surface... The capacity ratio, CR, is given by the ratio OL / OC."* (PDF s. 30, Basılı 3-14)  
  > *"The concrete compression stress block is assumed to be rectangular, with a stress value of 0.85fcd... k1 = 0.85 - 0.006(fck - 25), 0.70 <= k1 <= 0.85"* (PDF s. 26, Basılı 3-10)
- **Değişkenler ve Birimler:**  
  - $N_d$: Eksenel tasarım yükü [N veya kN] (basınç pozitif)  
  - $M_{d2}, M_{d3}$: Büyütülmüş iki eksenli tasarım eğilme momentleri [N-mm veya kN-m]  
  - Gerilme bloğu parametreleri: Gerilme değeri $0.85 f_{cd}$; derinlik $a = k_1 c$; $k_1 = 0.85 - 0.006(f_{ck} - 25)$, $0.70 \le k_1 \le 0.85$  
  - Beton sınır birim kısalması: $\varepsilon_{cu} = 0.003$  
  - Çelik gerilmesi: $\sigma_s = \varepsilon_s E_s \le f_{yd}$  
  - Maksimum eksenel yük sınırı ($N_{r,\max}$):  
    - Düşey (yerçekimi) kombinasyonlarında: $N_{r,\max} = 0.6 f_{ck} A_g$ (TS 7.4.1)  
    - Depremli (sismik) kombinasyonlarda: $N_{r,\max} = 0.4 f_{ck} A_g$ (TS 7.4.1)  
  - Donatı oranı sınırları: %1 ile %4 arası ($0.01 \le \rho \le 0.04$, TS 7.4.1)  
  - Hedef kullanım oranı (Utilization Factor Limit): Varsayılan $0.95$ (kullanıcı değiştirebilir).
- **D/C Oranı Yöntemi:** ACI ile birebir aynı Radyal Işın Yöntemi ($CR = OL / OC$). Yük noktası $L(N_d, M_{d2}, M_{d3})$ ile orijin arasındaki doğrunun $0.85f_{cd}$ ve $\gamma_m$ ile üretilen 3B etkileşim yüzeyini kestiği nokta $C$ arasındaki uzunluk oranıdır.
- **Durum:** **ÇIKARILDI**

---

### B.3 Narinlik ve Moment Büyütme
- **Bölüm & Madde:** Chapter 2.7, 3.5.2.1, 3.5.2.2, Appendix A, Appendix B; TS 500 Madde 6.3.10, 7.6.1, 7.6.2.4, 7.6.2.5
- **Sayfa:** PDF 15, 27–30, 70–73 (Basılı: 2-5, 3-11 – 3-14, A-1 – B-2)
- **Birebir Alıntı (≤25 kelime):**  
  > *"The factor βs is the moment magnification factor for moments causing sidesway. The program takes this factor to be 1 because... assumed to be obtained from a second order elastic (P-Δ) analysis"* (PDF s. 28, Basılı 3-12)  
  > *"The factored moments are further increased, if required, to obtain minimum eccentricities of (15mm + 0.03h)... applied in both directions simultaneously."* (PDF s. 27, Basılı 3-11)
- **Değişkenler ve Birimler:**  
  - Minimum dışmerkezlik: $e_{\min} = 15\text{ mm} + 0.03h$ (ACI'den farklı olarak **her iki doğrultuda aynı anda** uygulanır) (TS 6.3.10)  
  - $M_2 = M_{ns} + \beta_s M_s$ ($\beta_s = 1.0$ kabul edilir, $P-\Delta$ analizi varsayılır)  
  - Tasarım momenti: $M_d = \beta_{ns} M_2$  
  - $\beta_{ns} = \frac{C_m}{1 - 1.3 N_d / N_k} \ge 1.0$ (TS 7.6.2.5, Denk. 7.24) ($N_d \ge N_k / 1.3$ ise göçme ilan edilir)  
  - $C_m = 0.6 + 0.4 (M_1 / M_2) \ge 0.4$ (TS Denk. 7.25) ($M_1/M_2$ tek eğrilikte pozitif, çift eğrilikte negatif; enine yük varsa $C_m = 1.0$)  
  - Burkulma yükü: $N_k = \frac{\pi^2 EI}{(k l_u)^2}$ (TS Denk. 7.19); $EI = \frac{0.4 E_c I_g}{1 + R_m}$ (Denk. 7.21); $R_m = \frac{\text{Maks. kalıcı eksenel yük}}{\text{Maks. toplam eksenel yük}} \le 1.0$
- **Durum:** **ÇIKARILDI**

---

### B.4 Kiriş Eğilme Tasarımı
- **Bölüm & Madde:** Chapter 3.6.1, 3.6.1.2; TS 500 Madde 7.1, 7.3, 7.11
- **Sayfa:** PDF 40–47 (Basılı: 3-24 – 3-31)
- **Birebir Alıntı (≤25 kelime):**  
  > *"The design procedure is based on the simplified rectangular stress block, as shown in Figure 3-7 (TS 7.1)... It is assumed that the design ultimate axial force does not exceed 0.1 fck Ag"* (PDF s. 41, Basılı 3-25)
- **Değişkenler ve Birimler:**  
  - Negatif momentte Dikdörtgen ($b_w \times h$), pozitif momentte Dikdörtgen veya T-kesit ($b_f, b_w, h, d_s$)  
  - Basınç bloğu derinliği: $a = d - \sqrt{d^2 - \frac{2 |M_d|}{0.85 f_{cd} b}}$  
  - Denge derinliği: $c_b = \frac{\varepsilon_{cu}}{\varepsilon_{cu} + \varepsilon_{sy}} d = \frac{0.003}{0.003 + f_{yd}/E_s} d$; sınır derinlik: $a_{\max} = 0.85 k_1 c_b$ (TS Denk. 7.4)  
  - $a \le a_{\max}$ ise tek donatılı: $A_s = \frac{M_d}{f_{yd} (d - a/2)}$  
  - $a > a_{\max}$ ise çift donatılı: $M_{dc} = C_c (d - a_{\max}/2)$, $M_{ds} = M_d - M_{dc}$, $A'_s = \frac{M_{ds}}{(\sigma'_s - 0.85 f_{cd})(d - d')}$, $A_s = A_{s1} + A_{s2}$  
  - Donatı sınırları:  
    - Minimum çekme donatısı: $A_{s,\min} = 0.8 \frac{f_{ctd}}{f_{yd}} b_w d$ (TS Denk. 7.3)  
    - Maksimum çekme donatısı: $\rho - 0.85 \rho' \le \rho_b$ (TS 500) ve kaba sınır $\rho \le 0.02$, $\rho' \le 0.02$ (gövde brüt alanına göre en çok %2)
- **Durum:** **ÇIKARILDI**

---

### B.5 Kesme Tasarımı ve $V_c = 0$ Koşulları
- **Bölüm & Madde:** Chapter 3.5.4, 3.6.2; TS 500 Madde 8.1.3, 8.1.4, 8.1.5; TSC (Deprem Yönetmeliği) 7.3.4, 7.3.7, 7.4.5, 7.7.6
- **Sayfa:** PDF 31–39, 50–54 (Basılı: 3-15 – 3-23, 3-34 – 3-38)
- **Birebir Alıntı (≤25 kelime):**  
  > *"Vcr = 0.65 fctd bw d (1 + γ Nd / Ag)... Vc = 0.8 Vcr... if Nd < 0.05 fck Ag and VE ≥ 0.5 Vd... Vc is taken as zero"* (PDF s. 37, 52; Basılı 3-21, 3-36)
- **Değişkenler ve Birimler:**  
  - Eksenel kuvvetli eğik çekme çatlama dayanımı:  
    $$V_{cr} = 0.65 f_{ctd} b_w d \left(1 + \gamma \frac{N_d}{A_g}\right)$$  
    Burada $\gamma = 0.07$ (eksenel basınçta), $\gamma = -0.3$ (eksenel çekmede; çekme gerilmesi $< 0.5\text{ MPa}$ ise $\gamma = 0$).  
  - Betonun kesme dayanımı: $V_c = 0.8 V_{cr}$ (TS Denk. 8.4)  
  - Kesit ezilme sınırı: $V_{\max} = 0.85 f_{ck} b_w d$ (TS 8.1.5b, TSC 7.3.7.5)  
  - Gerekli etriye ($A_{sw} / s$):  
    - $V_d \le V_{cr}$ ise minimum etriye: $\frac{A_{sw}}{s} = 0.3 \frac{f_{ctd}}{f_{ywd}} b_w$ (TS Denk. 8.6)  
    - $V_{cr} < V_d \le V_{\max}$ ise: $\frac{A_{sw}}{s} = \frac{V_d - V_c}{f_{ywd} d} \ge 0.3 \frac{f_{ctd}}{f_{ywd}} b_w$ (TS Denk. 8.5)  
    - $V_d > V_{\max}$ ise kesit yetersiz hatası.  
  - **$V_c = 0$ Koşulu (Yüksek Sünek Kolon ve Kirişlerde):**  
    Eğer $N_d < 0.05 f_{ck} A_g$ VE depremden gelen kesme kuvveti $V_E \ge 0.5 V_d$ VE kontrol noktası sarılma bölgesinde ($l_0 = \max(b, h, L_n/6, 500\text{ mm})$) ise **$V_c = 0$ alınır**.
- **Durum:** **ÇIKARILDI**

---

### B.6 Sismik Kapasite Tasarımı Kesme Kuvveti ($V_e$) ve TBDY-2018 İlişkisi
- **Bölüm & Madde:** Chapter 3.1, 3.5.4.1, 3.6.2.1; TSC (TBDY-2018) 7.3.7, 7.4.5
- **Sayfa:** PDF 16, 32–37, 50–52 (Basılı: 3-1, 3-16 – 3-21, 3-34 – 3-36)
- **Birebir Alıntı (≤25 kelime):**  
  > *"The menu option 'TS 500-2000(R2018)' also covers the specification of Turkish Seismic code, 'Fundamentals for Designing Buildings under the Influence of Earthquake' (2018)."* (PDF s. 16, Basılı 3-1)  
  > *"Moment capacity at end I... using a steel yield stress fyd and a concrete stress fcd."* (PDF s. 33, 51; Basılı 3-17, 3-35)
- **Değişkenler ve Birimler:**  
  - Standart başlığı: Menüdeki **"TS 500-2000(R2018)"** seçeneği doğrudan **TBDY-2018 (TSC)** sismik tasarım kurallarını içerir (bağımsız ayrı bir TBDY menüsü yoktur; kılavuzdaki "TSC" atıfları Türkiye Bina Deprem Yönetmeliği 2018'dir).  
  - Kiriş kapasite kesmesi: $V_e = V_{G+Q} + \frac{M_{pa} + M_{p\ddot{u}}}{L_n}$; momentler $f_{cd}$ ve $f_{yd}$ kullanılarak hesaplanan tasarım taşıma gücü momentleridir ($\phi$ yoktur, $\alpha = 1.0$).  
  - Kolon kapasite kesmesi: $V_e = \min(V_{e,\text{col}}, V_{e,\text{beam}}) \ge V_{d,\text{factored}}$ (TSC Denk. 7.5); kolon momentleri eksenel yüke bağlı $f_{cd}, f_{yd}$ ile hesaplanır; kiriş tarafı ise kolon düğümüne birleşen kirişlerin taşıma gücü momentleri toplamından ($V_e = \frac{\sum M_p}{H}$) elde edilir.  
  - Ezilme üst sınırı: $V_e \le 0.85 f_{ck} A_w$.
- **Durum:** **ÇIKARILDI**

---

### B.7 Burulma Tasarımı
- **Bölüm & Madde:** Chapter 3.6.3; TS 500 Madde 8.2.2, 8.2.3, 8.2.4, 8.2.5
- **Sayfa:** PDF 54–62 (Basılı: 3-38 – 3-46)
- **Birebir Alıntı (≤25 kelime):**  
  > *"Design for torsion may be ignored if... Td <= Tcr = 0.65 fctd S... When (Vd/Vcr)^2 + (Td/Tcr)^2 <= 1, there is no need to compute torsional stirrups"* (PDF s. 60, Basılı 3-44)
- **Değişkenler ve Birimler:**  
  - $S = \sum x^2 y / 3$: Burulma kesit şekil faktörü [$mm^3$]  
  - $A_e = (b_w - 2c)(h - 2c)$: Etriye merkez ekseni alanı [$mm^2$] ($c = 30\text{ mm}$ varsayılır: $25\text{ mm}$ net paspayı + $\phi 10$ etriye yarıçapı)  
  - $u_e = 2(b_w - 2c) + 2(h - 2c)$: Çevre uzunluğu [$mm$]  
  - Çatlama burulması: $T_{cr} = 1.35 f_{ctd} S$ (Denk. 8.11); eşik sınır: $0.65 f_{ctd} S$ (Denk. 8.12)  
  - Etriye hesabı ($T_d > T_{cr}$ ise): $\frac{A_{ot}}{s} = \frac{T_d}{2 A_e f_{ywd}}$; toplam etriye: $\frac{A_o}{s} = \frac{A_{ov}}{s} + \frac{A_{ot}}{s}$  
  - Boyuna donatı: $A_{sl} = \frac{T_d u_e}{2 A_e f_{yd}}$  
  - Ezilme kontrolü: $\frac{V_d}{b_w d} + \frac{T_d}{S} \le 0.22 f_{cd}$ (TS Denk. 8.19)
- **Durum:** **ÇIKARILDI**

---

### B.8 Örtü ve $d'$ Varsayımları
- **Bölüm & Madde:** Chapter 2.4, 3.1, 3.6.3.2; TS 500
- **Sayfa:** PDF 13, 20, 55 (Basılı: 2-4, 3-4, 3-39)
- **Birebir Alıntı (≤25 kelime):**  
  > *"d' Concrete cover to center of reinforcing, mm"* (PDF s. 20, Basılı 3-4, Notation tablosu)  
  > *"it is assumed that the distance between the centerline of the outermost closed stirrup and the outermost concrete surface is 30 mm. This is equivalent to 25-mm clear cover and a 10-mm-diameter stirrup placement."* (PDF s. 55, Basılı 3-39)
- **Değişkenler ve Birimler:**  
  - $d'$: Boyuna donatı ağırlık merkezine beton örtüsü mesafesi [mm]  
  - $d = h - d'$: Faydalı yükseklik [mm]  
  - Burulma özel kesit parametrelerinde ($A_e, u_e$) varsayılan açık değer: $c = 30\text{ mm}$ ($25\text{ mm}$ net paspayı + $10\text{ mm}$ çaplı etriye).
- **Durum:** **ÇIKARILDI**

---

### B.9 Sünek Çerçeve (High Ductility) ve Güçlü Kolon – Zayıf Kiriş
- **Bölüm & Madde:** Chapter 3.7.3, 3.7.4; TSC (TBDY-2018) 7.3.5.1, 7.5.1, 7.5.2
- **Sayfa:** PDF 66–68 (Basılı: 3-50 – 3-52)
- **Birebir Alıntı (≤25 kelime):**  
  > *"For high ductility frames, at a particular joint for a particular column direction, major or minor: ∑ Mrc >= (6/5) ∑ Mrb"* (PDF s. 66, Basılı 3-50)  
  > *"The panel zone shear stress is evaluated... and comparing it with the following design shear strengths: 1.7 fck for joints confined on all four sides, 1.0 fck for all other joints"* (PDF s. 66, Basılı 3-50)
- **Değişkenler ve Birimler:**  
  - Güçlü Kolon – Zayıf Kiriş kuralı: $\sum M_{rc} \ge 1.2 \sum M_{rb}$ (TBDY 7.3.5.1)  
    - Momentler düğüm yüzünde ve tasarım dayanımları ($f_{cd}, f_{yd}$) ile hesaplanır; kolon momenti kombinasyonun $N_d$ eksenel yükü altındaki tek eksenli taşıma gücüdür.  
    - Çıktıda $\frac{\sum M_{rb}}{\sum M_{rc}}$ oranı basılır; $5/6$'yı aşarsa uyarı mesajı verilir.  
  - Birleşim Kesmesi (Panel Zone Shear):  
    - $V_e^h = T_L + T_R - V_{\text{kol}}$; burada $T = f_{yd} A_s$ alınırken $\alpha = 1.25$ overstrength çarpanı uygulanır (TSC 7.5.2.1).  
    - Dayanım sınırı: 4 tarafı kuşatılmış birleşimde $V_{\max} = 1.7 b_j h_c \sqrt{f_{ck}}$ (kılavuz formülünde $1.7 f_{ck}$ ifadesi basılmış olup değişken $v_{\text{allow}}$ birim gerilmedir); kuşatılmamışta $1.0 b_j h_c \sqrt{f_{ck}}$.
- **Durum:** **ÇIKARILDI**

---

### B.10 Çıktı Tabloları ve OAPI Bilgisi
- **Bölüm & Madde:** Kılavuz genel taraması
- **Sayfa:** Kılavuz metninde DatabaseTables / OAPI fonksiyon listesi bulunmamaktadır.
- **Durum:** **BULUNAMADI**  
  *(Açıklama: CSI TS 500 kılavuzu da salt mühendislik tasarım algoritmalarını belgeler; veritabanı tablo adları ve API çağrıları bu dokümanda yer almaz.)*

---
---

# BÖLÜM C: ACI 318-14 VE TS 500-2000(R2018) KARŞILAŞTIRMA TABLOSU

| Madde / Kriter | ETABS ACI 318-14 | ETABS TS 500-2000(R2018) |
|---|---|---|
| **Temel Güvenlik Felsefesi** | Yük katsayıları + Dayanım Azaltma Katsayısı ($\phi$) | Yük katsayıları + Malzeme Güvenlik Katsayıları ($\gamma_{mc}=1.5, \gamma_{ms}=1.15$) |
| **Beton Basınç Bloğu Gerilmesi** | $0.85 f'_c$ | $0.85 f_{cd} = 0.85 (f_{ck} / 1.5) = 0.567 f_{ck}$ |
| **Basınç Bloğu Derinlik Faktörü** | $\beta_1 = 0.85 - 0.05 \frac{f'_c - 4000}{1000}$ ($0.65 \le \beta_1 \le 0.85$) | $k_1 = 0.85 - 0.006 (f_{ck} - 25)$ ($0.70 \le k_1 \le 0.85$) |
| **Beton Sınır Kısalması ($\varepsilon_{cu}$)** | $0.003$ | $0.003$ |
| **Kolon P–M–M D/C Tanımı** | 3B Radyal Işın Oranı ($CR = OL / OC$) | 3B Radyal Işın Oranı ($CR = OL / OC$) |
| **Kolon Maksimum Eksenel Yükü** | Etriyeli: $0.80 \phi P_0$; Spiralli: $0.85 \phi P_0$ ($\phi=0.65/0.75$) | Düşey yükte: $0.6 f_{ck} A_g$; Depremli yükte: $0.4 f_{ck} A_g$ |
| **Minimum Kolon Dışmerkezliği** | $e_{\min} = 0.6\text{ in} + 0.03h$ (Tek seferde tek eksende) | $e_{\min} = 15\text{ mm} + 0.03h$ (Her iki eksende aynı anda) |
| **Narinlik Moment Büyütmesi ($\delta_s, \beta_s$)** | $P-\Delta$ analizi varsayılarak $\delta_s = 1.0$ alınır | $P-\Delta$ analizi varsayılarak $\beta_s = 1.0$ alınır |
| **Kiriş Eğilme Tasarımında $\varepsilon_{s,\min}$** | En az $\varepsilon_t = 0.005$ (çekme kontrollü) zorlanır | Dengeli durum $c_b$ derinliğinin $0.85$ katı ($a_{\max} = 0.85 k_1 c_b$) |
| **Kiriş Maksimum Çekme Donatısı** | Normal: $\rho \le 0.04$; Sismik Özel (Special): $\rho \le 0.025$ | $\rho \le 0.02$ ve $\rho - 0.85\rho' \le \rho_b$ |
| **Kiriş Kesme Kapasitesi ($V_c$)** | $V_c = 2 \lambda \sqrt{f'_c} b_w d$ | $V_c = 0.8 V_{cr} = 0.8 \times 0.65 f_{ctd} b_w d (1 + \gamma N_d/A_g)$ |
| **Sismik Kesme Dayanımı $\phi$ Katsayısı** | Kapasite kesmesinde $\phi=0.75$; Faktörlü sismik kesmede $\phi=0.60$ | $\phi$ yok (Malzeme katsayıları $f_{cd}, f_{ctd}, f_{ywd}$ içinde) |
| **$V_c = 0$ Sıfırlama Koşulu** | $P_u < \frac{A_g f'_c}{20}$ VE $V_E \ge 0.5 V_u$ VE $l_0 \le 2d$ | $N_d < 0.05 f_{ck} A_g$ VE $V_E \ge 0.5 V_d$ VE $l_0$ mesafesinde |
| **Sismik Kapasite Kesmesi ($V_e$)** | Kiriş ve kolonda $M_{pr}$ ($1.25 f_y$ ve $\phi=1.0$) ile | Kiriş ve kolonda $M_{pr}$ ($f_{yd}, f_{cd}$ taşıma gücü momenti, $\alpha=1.0$) ile |
| **Güçlü Kolon – Zayıf Kiriş Şartı** | $\sum M_{nc} \ge 1.2 \sum M_{nb}$ ($1.0 f_y, \phi=1.0$) | $\sum M_{rc} \ge 1.2 \sum M_{rb}$ ($f_{cd}, f_{yd}$) |
| **Birleşim Donatı Akma Çarpanı ($\alpha$)** | $\alpha = 1.25$ ($1.25 f_y A_s$) | $\alpha = 1.25$ ($1.25 f_{yd} A_s$) |
| **TBDY-2018 İle Bütünlük** | ACI 318-14 bağımsız Amerikan standardıdır | "TS 500-2000(R2018)" menüsü TBDY-2018 (TSC) kurallarını doğrudan uygular |

---
---

# BÖLÜM D: KILAVUZLARDA YER ALMAYAN ANALİZ BİLEŞENLERİ

Aşağıdaki modelleme parametreleri ve API fonksiyonları `CFD-ACI-318-14.pdf` ve `CFD-TS-500-2000(R 2018).pdf` kılavuzlarında aranmış ve **BULUNAMAMIŞTIR**:

1. **Rijit Uç Bölgesi Faktörü (Rigid End Zone / End Offsets):** **BULUNAMADI**  
   *(Kılavuzlar kesit tasarımında temiz açıklığı $L_n$ veya kullanıcı çarpanını kullanır; rijit bölge rijitlik matrisi formülasyonu analiz referansındadır.)*
2. **Kesit Çatlama Modifikatörleri (Stiffness Modifiers: 0.35 / 0.70):** **BULUNAMADI**  
   *(Yalnızca ACI Bölüm 3.4.2.2'de $P-\Delta$ analizinde moment of inertia reduction factor olarak ACI 6.6.3.1.1'e genel atıf yapılır, varsayılanın 1 olduğu belirtilir.)*
3. **Kütle Kaynağı ve Kat Diyaframı Ek Dışmerkezliği (%5 Accidental Eccentricity):** **BULUNAMADI**  
   *(Kılavuzlar statik ve spektral analizden elde edilen çubuk uç kuvvetlerini hazır kabul eder; kütle katılımı ve burulma diyafram formülasyonu yer almaz.)*
4. **OAPI Veritabanı Tablo Adları (DatabaseTables):** **BULUNAMADI**  
   *(Tasarım özet tabloları ve OAPI veri yapıları tasarım elkitaplarında yer almaz.)*

### 📌 Kullanıcıya Belge Talebi
Yukarıdaki analiz ve rijitlik matrisi kabullerinin doğrulanabilmesi için kullanıcıdan şu belgenin sağlanması rica edilir:
> **"CSI ETABS Analysis Reference Manual (özellikle Rigid End Zones, Frame Element Internal Forces, Diaphragm Eccentricity ve Response Spectrum Analysis bölümleri)"**
