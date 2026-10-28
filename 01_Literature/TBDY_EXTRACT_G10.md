# 📜 TBDY-2018 Çıkarım Raporu: G10 — Ötelenme Sınırı, Muafiyetler ve Çapraz Kontrol

Bu belge; `00_System/GEMINI_TASK_G9_G10.md` görev tanımı ve `.agents/rules/anti_hallucination_evidence.md` protokolü uyarınca `Evidence/DesignGuides/TDBY2018.pdf` (SHA-256 `8d3a9a463d4a534ec2c6834c557b5f706e2ad976c2d6837f6c9b6242e38a6bb2`) birincil kaynağından çıkarılmıştır.

> **Kural Notu:** Bu belgedeki tüm maddeler Gemini tarafından birincil kaynaktan çıkarılmıştır ve durum etiketi **ÇIKARILDI** olarak işaretlenmiştir. İnceleme ve kesin onay yetkisi Claude Code'a aittir.

---

## 📑 1. GÖRELİ KAT ÖTELEMESİ SINIRI VE $\lambda$ PARAMETRESİ (Madde 4.9.1)

- **Yönetmelik Maddesi:** 4.9.1 (4.9.1.1, 4.9.1.2, 4.9.1.3, 4.9.1.4)
- **PDF Sayfa No:** 76, 77 | **Basılı Sayfa No:** 55, 56
- **Birebir Alıntı (≤ 25 kelime):**  
  > *"etkin göreli kat ötelemelerinin kat içindeki en büyük değeri δi,max^(X), aşağıda (a) veya (b)’de verilen koşulları sağlayacaktır."*

### 1.1. Yerdeğiştirme Hesabı ve Yük Durumu (Madde 4.9.1.1)
- **Azaltılmış Göreli Kat Ötelemesi (Denk. 4.32):**
  $$\Delta_i^{(X)} = u_i^{(X)} - u_{i-1}^{(X)}$$
- **Hesap Koşulu (Birebir Alıntı, Madde 4.9.1.1):**
  *"tipik (X) deprem doğrultusu için binanın i’inci ve (i–1)’inci katlarında herhangi bir kolon veya perdenin uçlarında azaltılmış deprem yükleri’ne göre hesaplanan yatay yerdeğiştirmeleri göstermektedir. Ancak bu hesapta 4.7.3.2’de verilen koşul ve ayrıca Denk.(4.19)’da tanımlanan minimum eşdeğer deprem yükü koşulu gözönüne alınmayacaktır."*
  - Yükleme: Azaltılmış elastik deprem etkisi (Dayanıma Göre Tasarım — DGT kapsamında $R_a(T)$ ile azaltılmış spektral ivme $S_{aR}(T)$ yükleri altında).
  - İstisna: $T_p \le 1.4 T_{pa}$ sınırı (Madde 4.7.3.2) ve $V_{tE} \ge 0.04 m S_{DS} g I$ minimum taban kesmesi alt sınırı (Denk. 4.19) öteleme hesabında DİKKATE ALINMAZ; fiili esneklik yerdeğiştirmesi kullanılır.

### 1.2. Etkin Göreli Kat Ötelemesi (Madde 4.9.1.2, Denk. 4.33)
$$\delta_i^{(X)} = \frac{R}{I} \Delta_i^{(X)}$$
*(Burada $R$: Taşıyıcı Sistem Davranış Katsayısı, $I$: Bina Önem Katsayısıdır).*

### 1.3. Göreli Kat Ötelemesi Sınırları (Madde 4.9.1.3, Denk. 4.34a, 4.34b)
- **(a) Gevrek dolgu duvarları çerçeveye tamamen bitişik ise (Denk. 4.34a):**
  $$\frac{\delta_{i,\max}^{(X)}}{h_i} \le 0.008 \, \frac{\kappa}{\lambda}$$
- **(b) Esnek derzli veya bağımsız dolgu duvarları / esnek cephe bağlantısı ise (Denk. 4.34b):**
  $$\frac{\delta_{i,\max}^{(X)}}{h_i} \le 0.016 \, \frac{\kappa}{\lambda}$$

### 1.4. Katsayıların Tanımı ve Bağımlılıkları (Madde 4.9.1.4)
- **$\kappa$ Katsayısı:**
  - Betonarme binalarda: $\kappa = 1.0$
  - Çelik binalarda: $\kappa = 0.5$
- **$\lambda$ Katsayısı ve DD-3 Bağıntısı (Birebir Alıntı):**
  > *"Denk.(4.34)’te yer alan λ katsayısı, binanın gözönüne alınan deprem doğrultusundaki hakim titreşim periyodu için 2.2’de tanımlanan DD-3 deprem yer hareketinin 2.3.4.1’e göre hesaplanan elastik tasarım spektral ivmesi’nin, DD-2 deprem yer hareketinin elastik tasarım spektral ivmesi’ne oranıdır."*
  $$\lambda = \frac{S_{ae}(T, \text{DD-3})}{S_{ae}(T, \text{DD-2})}$$
- **$\lambda$ Hesabı İçin Gerekli Girdiler:**
  1. Binanın gözönüne alınan doğrultudaki hakim titreşim periyodu $T$.
  2. Standart tasarım depremi (DD-2, 475 yıl) parametreleri: $S_S^{(\text{DD-2})}, S_1^{(\text{DD-2})}$ ve zemin katsayıları $F_S^{(\text{DD-2})}, F_1^{(\text{DD-2})}$ $\rightarrow S_{DS}^{(\text{DD-2})}, S_{D1}^{(\text{DD-2})}, T_A, T_B$.
  3. Sık deprem (DD-3, 72 yıl) parametreleri: $S_S^{(\text{DD-3})}, S_1^{(\text{DD-3})}$ ve zemin katsayıları $F_S^{(\text{DD-3})}, F_1^{(\text{DD-3})}$ $\rightarrow S_{DS}^{(\text{DD-3})}, S_{D1}^{(\text{DD-3})}, T_A^{(\text{DD-3})}, T_B^{(\text{DD-3})}$.
  4. Eğer $T$ sabit ivme bölgesinde ($T_A \le T \le T_B$) kalıyorsa doğrudan:
     $$\lambda = \frac{S_{DS}^{(\text{DD-3})}}{S_{DS}^{(\text{DD-2})}}$$
- **Durum:** `ÇIKARILDI (TDBY-2018, s.55-56, Madde 4.9.1)`

---

## 📑 2. GÜÇLÜ KOLON – ZAYIF KİRİŞ KOŞULUNDAN MUAFİYETLER (Madde 7.3.5.3 ve 7.3.6)

- **Yönetmelik Maddeleri:** 7.3.5.3 (Muafiyetler) ve 7.3.6 (Kat Düzeyinde İzin Verilen Durum)
- **PDF Sayfa No:** 138, 139 | **Basılı Sayfa No:** 117, 118
- **Birebir Alıntı (Madde 7.3.5.3, ≤ 25 kelime):**  
  > *"Denk.(7.3)’ün uygulanmasına ilişkin özel durumlar aşağıda (a), (b) ve (c)’de belirtilmiştir"*

### 2.1. Muafiyet Maddelerinin Tam Metni (Madde 7.3.5.3)
1. **Düşük Eksenel Yük Muafiyeti (Madde 7.3.5.3a):**  
   *"Düğüm noktasına birleşen kolonların her ikisinde de $N_d / (A_c f_{ck}) \le 0.10$ olması durumunda, Denk.(7.3)’ün sağlanması zorunlu değildir."*
2. **Tek Kat ve En Üst Kat Muafiyeti (Madde 7.3.5.3b):**  
   *"Tek katlı binalarda ve çok katlı binaların kolonları üst kata devam etmeyen düğüm noktalarında Denk.(7.3)’ün sağlanıp sağlanmadığına bakılmayacaktır."*
3. **Perde Zayıf Doğrultu Muafiyeti (Madde 7.3.5.3c):**  
   *"Kirişlerin saplandığı perdenin zayıf doğrultuda kolon gibi çalışması durumunda, Denk.(7.3)’ün sağlanıp sağlanmadığına bakılmayacaktır."*

### 2.2. Kat Bazında Kısmi Sağlanamama İzni (Madde 7.3.6.1, 7.3.6.2, 7.3.6.3)
- Gözönüne alınan deprem doğrultusunda herhangi bir $i$'inci katta kolon kesme kuvveti oranı:
  $$\alpha_i = \frac{V_{is}}{V_{ik}} \ge 0.70 \qquad \text{[Denk. 7.4]}$$
  şartı sağlanırsa, o katın alt ve/veya üst düğüm noktalarında Denk. (7.3) güçlü kolon şartının sağlanamamış olmasına izin verilebilir.
  - $V_{is}$: Güçlü kolon şartını sağlayan kolonların taban kesme kuvvetleri toplamı (ayrıca $N_d / (A_c f_{ck}) \le 0.10$ kolonları da dahil edilebilir).
  - $V_{ik}$: Katın toplam kolon kesme kuvveti.
- $\alpha_i \ge 0.70$ sağlandığında: güçlü kolon şartını sağlayan kolonların moment ve kesme kuvvetleri $1 / \alpha_i$ oranı ile çarpılarak artırılır.
- Herhangi bir katta $\alpha_i < 0.70$ kalırsa: tüm çerçeveler süneklik düzeyi sınırlı kabul edilir (Madde 7.3.6.3).
- **Durum:** `ÇIKARILDI (TDBY-2018, s.117-118, Madde 7.3.5.3, 7.3.6)`

---

## 📑 3. ÇAPRAZ KARŞILAŞTIRMA KONTROLÜ (TBDY_EXTRACT_GEMINI.md ile Karşılaştırma)

| Parametre / Tablo | `TBDY_EXTRACT_GEMINI.md` Değeri | TDBY-2018 Orijinal Değeri (PDF) | Farklılık / Not |
|---|---|---|:---:|
| **Tablo 2.1 ($F_S$ Katsayıları)** | ZA: 0.8; ZB: 0.9; ZC: 1.3..1.2; ZD: 1.6..1.0; ZE: 2.4..0.8 | PDF s.29 Tablo 2.1 ile **BİREBİR AYNI** | **FARK YOK** |
| **Tablo 2.2 ($F_1$ Katsayıları)** | ZA: 0.8; ZB: 0.8; ZC: 1.5..1.4; ZD: 2.4..1.7; ZE: 4.2..2.0 | PDF s.29 Tablo 2.2 ile **BİREBİR AYNI** | **FARK YOK** |
| **Tablo 3.1 ($I$ Katsayıları)** | BKS=1: $I=1.5$; BKS=2: $I=1.2$; BKS=3: $I=1.0$ | PDF s.36 Tablo 3.1 ile **BİREBİR AYNI** | **FARK YOK** |
| **Tablo 3.2 (DTS Sınıfları)** | $S_{DS} < 0.33$: 4/4a; $0.33 \le S_{DS} < 0.50$: 3/3a; $0.50 \le S_{DS} < 0.75$: 2/2a; $S_{DS} \ge 0.75$: 1/1a | PDF s.37 Tablo 3.2 ile **BİREBİR AYNI** | **FARK YOK** |
| **Tablo 3.3 (BYS Sınıfları)** | BYS 1-8 aralıkları; BYS $\ge 4$ için DTS=3,3a ve 4,4a birleşik sütun ($42<H_N\le 56$ vb.) | PDF s.38 Tablo 3.3 ile **BİREBİR AYNI** (sayfa görüntüsüyle teyitli) | **FARK YOK** |
| **Denklem 2.2 ($S_{ae}(T)$)** | 4 periyot bölgesi ($0-T_A, T_A-T_B, T_B-T_L, >T_L$), $T_A=0.2 S_{D1}/S_{DS}$, $T_B=S_{D1}/S_{DS}$, $T_L=6\text{ s}$ | PDF s.30 Denk. 2.2, 2.3 ile **BİREBİR AYNI** | **FARK YOK** |

### Özet Sonuç:
`TBDY_EXTRACT_GEMINI.md` dosyasındaki değerler ile TDBY-2018 birincil PDF metni arasında **hiçbir sayısal veya kavramsal farklılık bulunmamaktadır**. Tüm tablo değerleri ve katsayılar %100 örtüşmektedir.
