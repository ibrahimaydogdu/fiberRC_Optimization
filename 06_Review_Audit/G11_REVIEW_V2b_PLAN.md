# G11 incelemesi ve V2b karşılaştırma planı (Claude, 2026-10-30)

Kaynak: `01_Literature/ETABS_CFD_EXTRACT_G11.md` (Gemini, etiket ÇIKARILDI). Claude, PDF metnini (`pdftotext -layout`) çıkarıp aşağıdaki kalemleri **yerinde doğruladı**.

## 1. Doğrulama sonucu (Claude)
| Kalem | Sonuç |
|---|---|
| CR = OL/OC (ACI ve TS 500, radyal ışın, 3B yüzey) | **DOĞRULANDI** (ACI metin satırı 1031, TS 947) |
| ACI e_min = (0,6 + 0,03h) in | DOĞRULANDI (aci.txt 899) |
| TS e_min = 15 mm + 0,03h | DOĞRULANDI (ts.txt 824). "Her iki doğrultuda eşzamanlı" iddiası **bu çalışmada okunmadı → KISMEN** |
| TS Nr,max = 0,6 / 0,4 fck Ag | DOĞRULANDI (ts.txt 794–795) |
| ACI Vc=0: Pu < f'c Ag/20, VE ≥ 0,5 Vu, lo = max(genişlik, Ln/6, 18 in) | DOĞRULANDI (aci.txt 1390–1397) |
| TS Vc=0: Nd < 0,05 fck Ag, VE ≥ 0,5 Vd, lo = genişlik / Ln/6 / … | DOĞRULANDI; lo'nun 4. terimi ("500 mm") ve "b,h" ifadesi **okunmadı** (satır 1226 sonrası) → KISMEN |
| Vcr = 0,65 fctd bw d (1+γ Nd/Ag), Vc = 0,8 Vcr | DOĞRULANDI |
| Örtü: ACI 1,75 in, TS 30 mm (burulma kesiti için) | DOĞRULANDI |
| A.9 / B.10 OAPI tablo adları | BULUNAMADI (beklenen) |

## 2. Gemini çıkarımındaki kusurlar
- Bazı "birebir alıntılar" birden fazla yerden birleştirilmiş/özetlenmiş (A.1, A.3, A.5). Makaleye alıntı olarak girmeden önce asıl cümleye dönülmeli.
- Karşılaştırma tablosu (C), Vc=0 satırında ACI için "lo ≤ 2d" yazıyor; kılavuzda böyle bir koşul **yok** (lo = max(genişlik, Ln/6, 18 in)). Tablo hatalı, kullanılmayacak.
- Kılavuzdaki "sarılma bölgesi" tanımı ETABS'in kendi ACI 18 kuralıdır; bizim kodumuz TBDY 7.3/7.4'e göredir (uzunluklar farklıdır).

## 3. Programla fark listesi (V2b'de D/C farkının beklenen kaynakları)
1. **D/C tanımı:** ETABS = 3B radyal ışın (OL/OC). Program = sabit P'de moment oranı, çift eksenli için **doğrusal toplam** Mx/φMnx + My/φMny (muhafazakâr, `AciColumn.BiaxialDcr`). Aynı tasarıma programın radyal oranı da eklenmeli; yoksa fark ölçülemez.
2. **Minimum dışmerkezlik:** ETABS e_min uygular; programda `e_min` **yok** (grep: sonuç yok). Küçük momentli kolonlarda ETABS büyük çıkar.
3. **Narinlik:** ETABS δns ve (P-Δ varsayımı) uygular; programda moment büyütme yok, P-Δ yok. 3 m kat yüksekliğinde küçük etki beklenir; sayısal olarak ölçülecek.
4. **Pn,max:** ETABS 0,80·φ·P0 (φ=0,65); program `DesignPnMax = 0,65·PnMax(0,80 Po)` → **aynı**.
5. **φ geçişi:** ETABS ε_y ile 0,005 arası doğrusal; programın `Aci.Phi(EpsT, Fy)` bununla karşılaştırılmalı (kod okunacak).
6. **Kesme:** Programda sarılma bölgesinde Vc yalnız `Nd ≤ 0,05 Ag fck` ile sıfırlanıyor (TBDY biçimi), ETABS'te ayrıca VE ≥ 0,5 Vu koşulu var; kapasite tasarımı kesmesinde ETABS ve program aynı sonucu verir (VE tamamen depremden sayılır).
7. **Kesme D/C:** ETABS kesme/burulma için D/C değil **Av/s** verir; programın G-kısıtlarıyla doğrudan oran karşılaştırması yapılamaz, gerekli etriye alanı karşılaştırılır.
8. **TBDY ile TS 500 menüsü:** "TS 500-2000(R2018)" ETABS'te TBDY-2018 sismik kurallarını da içerir (Mr = fcd/fyd, φ yok). Programın TBDY kapasite tasarımı bununla karşılaştırılabilir; ACI menüsü yalnız ACI 318-14 sismik (Mpr, 1,25 fy).

## 4. V2b planı (ETABS OAPI ile)
1. Mevcut Ö1 deneme modeline (EtabsProbe) donatı/kesit atayıp tasarım çalıştır: `DesignConcrete.SetCode`, ardından `DesignConcrete.StartDesign` (ilgili fonksiyon adı **ETABSv1.dll yansımasıyla doğrulanacak**, tahmin edilmeyecek).
2. Okuma: `DesignConcrete.GetSummaryResultsColumn` / `...Beam` veya `DatabaseTables` ("Concrete Column PMM Envelope" vb., tablo adları DB'den listelenerek bulunur).
3. Programdan: radyal D/C (yeni yardımcı), e_min'li ve e_min'siz.
4. Çıktı: 116 elemanda (veya grup en kötüsü) kolon D/C farkları, kesme Av/s, kiriş As; `04_Results/VALIDATION_REPORT.md` V2b bölümü.
5. Sapma >%5 ise neden bölünür (ε, e_min, narinlik, kombinasyon farkı).

## 5. Ek belge isteği
`ETABS Analysis Reference Manual` (Rigid End Zones, Diaphragm eccentricity / RS accidental eccentricity bölümleri). %2–6'lık ek dış merkezlik farkı yalnız bununla açıklanabilir. Kullanıcıdan istenir.
