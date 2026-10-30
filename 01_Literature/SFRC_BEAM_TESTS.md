# SFRC_BEAM_TESTS.md — Donatılı çelik lifli betonarme kiriş deney verisi (Doğrulama kapısı S2)

Hazırlayan: Literature Analyst (Claude), 2026-10-30. Kural: `.agents/rules/anti_hallucination_evidence.md`. Veri dosyası: `SFRC_BEAM_TESTS.csv` (44 satır: ilk toplama 12 + 2026-10-30 eki 32; ek için §6'ya bakın).
**Sonuç (dürüst özet):** Hedef (≥3 çalışma, 15–25 kiriş, fc 25–60 MPa) **karşılanamadı.** Tam girdi veren, açık erişimli, `fR1/fR3` + taşıma yükü içeren yalnız **2 çalışma / 12 kiriş** bulundu (9 + 3). fc 25–60 MPa penceresi neredeyse boş (Nogales fc 60,6–67,9 MPa; Buttignol'da fc yok). Aşağıda erişilemeyenler ve elenenler listelidir.

## 1. Belgeler (indirilen, orijinal biçim)
| Dosya (`Papers/`) | Bayt | SHA-256 | URL / lisans |
|---|---|---|---|
| Nogales2025_EngStruct343-121237_lightly-reinforced-SFRC.pdf | 5.235.107 | c5d482d3dccbdeacd76e163bddb53f2846859807a45bb25b483c6d9d7381df0f | UPCommons bitstream `https://upcommons.upc.edu/bitstreams/0b9e5bd5-6d4b-4456-9e62-61536de7b29b/download` (yayıncı sürümü, CC BY-NC-ND, "open access article", s.1). DOI doğrulaması CrossRef ile **yapılmadı** (kimlik: makale künyesi PDF'den). |
| Buttignol2018_RIEM11-5_SFRC-RC-beams.pdf | 1.191.228 | ae020318d147fe6d242be29115b959f971b62c4642b3ce52c6566bf4b5a7c96f | `https://www.scielo.br/j/riem/a/3FH99MRnvSLHXrBY5Rg8j9p/?format=pdf&lang=en` (doi.org/10.1590/S1983-41952018000500006 çözümlemesi HTTP 200; CrossRef sorgusu başlığı verdi: "Design of reinforced concrete beams with steel fibers in the ultimate limit state") |

(Nogales PDF'i WebFetch aracının kaydettiği ikili çıktıdan Papers/'a kopyalanmıştır; bayt/SHA kopya üzerinde hesaplandı.)

## 2. Kaynakça (APA 7)
- Buttignol, T. E. T., Fernandes, J. F., Bittencourt, T. N., & Sousa, J. L. A. O. (2018). Design of reinforced concrete beams with steel fibers in the ultimate limit state. *Revista IBRACON de Estruturas e Materiais, 11*(5), 995–1010. https://doi.org/10.1590/S1983-41952018000500006 *(sayfa aralığı PDF altbilgilerinden: 999–1008 görüldü; bitiş sayfası teyit edilmedi)*
- Nogales, A., Galeote, E., Aidarov, S., Quijano, N., & de la Fuente, A. (2025). On the ductility of lightly reinforced steel fibre reinforced concrete shallow beams subjected to flexure: Experimental evidences. *Engineering Structures, 343*, Article 121237. (DOI PDF'de yazılı değil; makale numarası PDF başlığından.)

## 3. Çalışma bazlı durum
### 3.1 Nogales vd. (2025) — ERİŞİLDİ, 9 kiriş (2 RC kontrol + 7 hibrit)
- Kiriş: 400 (h) × 1000 (b) × 3000 mm; dörtlü yükleme, açıklık 2700 mm, yükler arası 900 mm (§3.2, s.4). Boyuna donatı altta Φ8: R1 As = 502 mm² (ρ=%0,15), R2 As = 402 mm² (ρ=%0,12) (Tab.6, s.5; Fig.3).
- Beton ve fR (Tab.4, s.4; EN 14651 3PBT, 150×150×600 mm, mix başına 6 numune, **ortalama**): FRC‑25A fcm,cyl 64,2; fR1m 3,3; fR3m 0,9. FRC‑30A 67,9; 3,9; 1,4. FRC‑20B 60,6 (karot); 3,3; 2,4. PC (lifsiz) 65,8.
- Donatı (Tab.5, s.4): Φ8 fym = 503 MPa, ftm = 644 (ft/fy=1,28); Φ10 fym = 529.
- Yük (Tab.7, s.7): Pmax 330 (RCS1), 252 (RCS2), 310/344/375/312 (HRCS1‑25Aa/25Ab/30Aa/30Ab), 284 (HRCS2‑25A), 333 (HRCS1‑20B), 290 (HRCS2‑20B) kN.
- Alıntı (≤25 kelime, s.4 §2.2): "Rebars of 8 and 10 mm of diameter of both types were tested according to EN 15630-1". Alıntı (s.5 §3.3): "The load bearing capacity of the SBs RCS1 and RCS2 was controlled by the strength of the rebars".
- Alıntı (s.4, §3.2): "with a span of 2700 mm and a distance between loads of 900 mm."
- Lifli‑yalnız kirişler (FRCS‑20B/25A/30A) ve PCS **dışarıda**: donatısız, gevrek; Tab.7'de Pmax/Pu yok (yalnız Pcr). Programın donatılı kesit modeli kapsamı dışı.
- **Uyarılar (önemli):** (i) Mu,exp = 0,45·Pmax, **kiriş öz ağırlığı dahil değil** (400×1000×3000 mm; öz ağırlık momenti ek olarak yaklaşık %5–7 mertebesinde olabilir – hesap yapıcıya bırakıldı, beton birim ağırlığı kaynakta yok). (ii) Pmax, donatının pekleşmesinden sonra oluşur (ft/fy = 1,28); akma‑sınırlı model göçme yükünü olduğundan düşük bulur — ya bu kapsamda karşılaştırın ya da fy yerine ft kullanın (karar kullanıcıda; kaynakta yalnız fym ve ftm var). (iii) HRCS1‑25Ab ve HRCS1‑30Aa için Pmax erken bir lif tepe noktasıdır (δ≈2,8–3,9 mm, ardından yük düşüşü; Fig.7c); çubuk kopma yükü Pu = 328 ve 322 kN. (iv) d verilmemiş (nominal örtü 35 mm, etriye Φ8); csv'de boş. (v) Beton dayanımı kirişlerle aynı seride mi ölçülmüş bilgisi yok; HRC kirişlerde fc, aynı mix'in silindir/karot değeri. (vi) fR değerleri ortalamadır; fiber tipi A: 35 mm × 0,55 mm tek kancalı, B: çift kancalı (Tab.3; Tab.3'te çekme dayanımı sütunu PDF metin katmanında bozuk: 1325/1850 MPa görünür).
- Açık erişim durumu: DOĞRULANDI (CC BY‑NC‑ND beyanı PDF s.1).

### 3.2 Buttignol vd. (2018) — ERİŞİLDİ, 3 kiriş (RC, F20, F60), KISMEN
- Kiriş 125 × 250 mm, uzunluk 2,2 m; alt 2Φ16, üst 2Φ6,3; dörtlü eğilme, destekler arası 180 cm, yükler arası 60 cm (Fig.5–6, s.1002). Fexp: 157 / 169 / 180 kN (Tab.11, s.1006). Hepsinde "ductile failure due to concrete crushing in the compressive zone and steel bars yielding" (§4.2, s.1002).
- fR (Tab.2, s.1000; **ortalama**, EN 14651 3PB, 150×150×550 mm, 6 numune): 20 kg/m³: fR1 = 2,08, fR3 = 1,14; 60 kg/m³: fR1 = 12,77, fR3 = 10,24 MPa. Karakteristik (Tab.3): 1,32/0,84 ve 5,93/2,75.
- **Eksik:** beton basınç dayanımı fc ve donatı fy metinde **yok** (BULUNAMADI; yalnız Eci = 43.985 MPa ve fctm,d = 3,15 MPa verilmiş; Fig.13b kontrol edilmedi), lif tipi ve d yok. Eci formülünden fcm türetilebilir ancak kaynakta açıkça yazılı olmadığı için **doldurulmadı**. Bu 3 satır, fc/fy elde edilmeden S2 karşılaştırmasında kullanılamaz (yazarlara başvuru önerilir).
- fR3 > 0,5·fR1 ve F60'ta pekleşmeli davranış; F60 değerleri çok yüksek (CV %24,6–33,5).

## 4. Aranıp elenen / erişilemeyen kaynaklar
| Kaynak | Durum | Gerekçe |
|---|---|---|
| Lehmann & Głodkowska (2021), *Materials* 14(11):2996 (PMC8198846) | ELENDİ | fR1 = 9,27 / fR3 = 7,87 MPa ve fy = 529 var, ancak lif 1,2 hac. (94 kg/m³) (hedef dışı), kesit/açıklık yalnız Şekil 5'te (metinde yok), yalnız BFSb flexural göçme ve moment verilmemiş (Vult = 161,2 kN). Tamamı okunmadı (metnin son ~29 bin karakteri). |
| Zhi vd. (2025), *Materials* 18(18):4322 (PMC12471304) | ELENDİ | EN 14651/fR yok; küp dayanımı; moment tablosu tutarsız. |
| Zhong vd. (2021), IJCSM 15 (10.1186/s40069-021-00461-0) | ELENDİ | Donatısız küçük kirişler; fR yok. |
| Zhao vd. (2023), *Front. Mater.* 10:1301647; Shams vd. (2024), *Front. Mater.* 11:1355351 | ELENDİ | Donatısız prizmalar. |
| Andriusis vd. (2013), J. Sustain. Archit. Civ. Eng. 2(3) | ELENDİ | fR yok; kalıntı gerilme dolaylı hesabı. |
| Hamoodi vd., IIETA (node/21056) | ELENDİ (içerik doğrulanmadı) | EN 14651 fR görünmüyor (özet). |
| Venkateshwaran vd. (2018) Struct. Concr.; Chen vd. (2021) Structures (tezdeki atıflar) | ELENDİ | Bu çalışmalar donatısız çentikli prizma fR verisidir (SedaDoktora2 makale metni, atıf [30],[31]); kiriş taşıma yükü yok. |
| Gümüş & Arslan (2019), *Structures* 20; Dancygier & Karinski; Foster & Parvez; Soetens & Matthys; Meda–Plizzari; di Prisco; Vandewalle | ERİŞİLEMEDİ | Erişim: yayıncı paywall'u (ScienceDirect/Wiley, HTTP 403) veya yalnız ikincil kopya (Scribd; kullanılmadı). Elle indirme: ilgili DOI'den kurum erişimiyle. |
| Springer Mater. Struct. 2024 (s11527-024-02336-8) bibliotecabuap yansısı | KULLANILMADI | Yetkisiz yansı sitesi (otomatik izin reddi); orijinal yayıncı sürümü açılmadı. |
| ICJ/ACI 51728248, Rjoub (2006), Nurhuda vd. (2024, FE) | İNCELENMEDİ | Özet düzeyinde; fR yok veya sayısal çalışma. |

## 5. Sonraki adım önerisi (kullanıcıya)
1. fc 25–60 MPa ve 20–60 kg/m³ aralığını kapsamak için kurum erişimiyle indirilecek adaylar: Meda, Minelli & Plizzari (2012); Tiberti vd.; Soetens & Matthys; Vandewalle (2000); Dancygier & Berkover (2016); Gümüş & Arslan (2019); Foster & Parvez (RC‑SFRC; Struct. Concr. DOI 10.1002/suco.201800090 — erişilemedi).
2. Buttignol yazarlarından (USP) fc, fy, d değerleri istenebilir; 3 ek satır tamamlanır.
3. Alternatif: fR verisi kirişle aynı seride ölçülmüş ve laboratuvar verisi paylaşılabiliyorsa (IA'nın ağı) kendi deney kirişleri.

---

## 6. Ek toplama (2026-10-30): kullanıcının `Papers/` altına koyduğu 6 PDF

Kural: `.agents/rules/anti_hallucination_evidence.md`. PDF'ler değiştirilmedi; metin `pdftotext -layout`, tablolar PyMuPDF ile görüntü olarak okundu (metin katmanı Meda Tab.3, Dancygier Tab.1/3/4, Mahmood Tab.1–3 için bozuk; hücreler sayfa görüntüsünden doğrulandı). **Yeni satır: 32** (Meda2012 5, Dancygier2016 25, Zijl2013 2). Foster-2019, Mahmood2018, Tiberti2015 için satır **yok** (aşağıda gerekçe).

### 6.1 Belgeler (orijinal biçim, `Papers/`)
| Dosya | Bayt | SHA-256 | Künye |
|---|---|---|---|
| Foster-2019.pdf | 3.117.190 | 7e512313ee07cf8c0b23fc258fc1e0c860851140f04784c4e0074ae4c852a508 | Foster & Parvez (2019), *Struct. Concr.* 20(3), 1010–1021, doi 10.1002/suco.201800090 |
| Meda2012.pdf | 2.093.665 | 76fcedafe6495592238bb3028ce6bc03561d5a5d957e6cab1565a067a6252877 | Meda, Minelli & Plizzari (2012), *Composites Part B* 43(8), 2930–2937, doi 10.1016/j.compositesb.2012.06.003 |
| Mahmood2018.pdf | 4.290.433 | a23a39cab8d5a2e316cd064caa9a0f062ade326c55631cd60085ab5cfad6a40c | Mahmood, Agarwal, Foster & Valipour (2018), *Eng. Struct.* 177, 695–706, doi 10.1016/j.engstruct.2018.10.007 |
| Dancygier2016.pdf | 5.210.372 | 3a0bb66456f27fa8a07018e330593ab83d884a6a9c751ba11940cda3dc33e08d | Dancygier & Berkover (2016), *Eng. Struct.* 111, 411–424, doi 10.1016/j.engstruct.2015.11.046 |
| Tiberti2015.pdf | 1.397.902 | a1618eb40b70416ee1ba03914e38c81b61fa1ee14a72d7b61c434df32d7a471b | Tiberti, Minelli & Plizzari (2015), *Cem. Concr. Res.* 68, 24–34, doi 10.1016/j.cemconres.2014.10.011 |
| Zijl2013.pdf | 2.121.885 | 8d42c61a64b09912fe07ace8dcd43613f2d81f23affba6ef96f127d07cff876d | van Zijl & Mbewe (2013), *Eng. Struct.* 53, 52–62 |

(DOI'ler PDF üst bilgisinden alındı; CrossRef doğrulaması yapılmadı. Zijl2013 PDF'inde DOI okunmadı.)

### 6.2 Çalışma bazlı sonuç
**Meda2012 — 5 satır (bonded; KISMEN).** 7 kirişin 5'i alındı; 2/16-UB-PC ve 2/16-UB-30 **dışlandı** (donatı bağsız/unbonded; kesit modeli kapsamı dışı). b=200 (yalnız Fig.1 kesit çiziminden; metinde yok), h=300, d=260 (alıntı: "the effective depth was 260 mm", s.2931), As Tab.2, fy=534 (s.2931), Pmax Tab.3 (s.2933), yükleme dörtlü, kayma açıklığı 1200 mm (Fig.1) → Mu = 0,6·P (öz ağırlık hariç). Dört kiriş de akma sonrası eğilme göçmesi (Tab.3). **Boş: fc** (küp 54,8 MPa; yazarların kendi dönüştürmesi 0,83 ile silindir 45,5 MPa; Tab.1'deki 49,7/45,0/43,2 MPa'nın tipi belirtilmemiş) ve **fR1/fR3** (UNI 11039 dörtlü eğilme, EN 14651 değil; yalnız feq(0–0,6)=3,08/4,75 ve feq(0,6–3)=3,45/5,65 MPa, 30/60 kg/m³). Denge oranı 1,00–1,15.

**Dancygier & Berkover (2016) — 25 satır (KISMEN).** 240×300 mm, açıklık 3200, yükler arası 1500, kayma açıklığı 850 (Fig.1); d Tab.1; Pmax (toplam yük) Tab.4 → Mu = 0,425·P (öz ağırlık hariç). 3 kirişte (40-015-1, 40-015-2, NF-1-015-1) denge oranı 1,61–1,64 → **ŞÜPHELİ** (kural gereği); donatı güçlü pekleşmeli (maks/kopma 677/676 MPa, akma 503 MPa) ve 0,15 % donatıda lif katkısı payı büyük (yazarlar da hesabın %20–50 aşırı tahmin ettiğini bildiriyor, s.423), yani veri hatası kanıtı değil. **Boş: fc** (yalnız 100 mm/150 mm küp, Tab.3) ve **fR1/fR3** (RILEM TC162 deneyleri yalnız Fig.A-1 eğrisi; tablo yok). 0,40 % grubunda (2Φ8+2Φ10) fy tek değerli değil (503/409) → fy boş. Tüm kirişlerde "concrete crushing occurred at failure" (Tab.4 dipnot b); kopan donatı sayısı Tab.4'te.

**van Zijl & Mbewe (2013) — 2 satır (KISMEN; S2 için KULLANILAMAZ).** Kendi deneyleri: R-SFRC batch B (Vf=%1,0) ve D (%1,5), 2Y10; Tab.6'da yalnız **ortalama** Mexp (18,55 ve 17,17 kN·m). Kiriş boyutu "150 × 300 × 750 mm" (b/h/L sırası açık değil), açıklık, yük aralığı, d, ölçülmüş fy (yalnız Fig.10c eğrisi) ve EN 14651 yok (yarma deneyi + feq) → b,h,d,fy,fc(küp),fR,span boş. Orijinal deney çalışması Mbewe tezine atıf olabilir; PDF'de atıf bulunamadı.

### 6.3 Satır açılmayanlar / dışlananlar (ve neden)
| Kaynak | Karar | Gerekçe |
|---|---|---|
| **Foster & Parvez (2019)** | SATIR YOK (BULUNAMADI) | Kiriş bazlı veri tablosu yok; Tab.2 yalnız çalışma/kiriş adlarını listeler (50 kirişin 39'u EN 14651/RILEM), Tab.3 model hatası istatistiği. **Yönlendirme:** asıl veri kaynakları — Dinh vd. (ASTM C1609, ACI Struct. J. 2010), Meda vd. 2012 (bu ek), de Montaignac vd. (*Mater. Struct.* 2012; EN 14651; 7 dikdörtgen kiriş; ρ=%0,33; fy 450), Conforti vd. (*Compos. B* 2013; 8 kiriş; EN 14651; fc 38; ρ=%1,0), Minelli vd. (*Mater. Struct.* 2014; 2 kiriş h=500), Ning vd. (*Constr. Build. Mater.* 2015; 4 kiriş; RILEM), Biolzi & Cattaneo (*Cem. Concr. Compos.* 2017; 18 kiriş; EN 14651). EN 14651 ile eşlenmiş fR + kiriş yükü bunlarda aranmalı (kullanıcı/kurum erişimi). Foster Tab.2 ve "50 tests" s.1014; Tab.3 s.1019; "39 tests" s.1020. |
| **Mahmood vd. (2018)** | SATIR YOK (Mu_exp BULUNAMADI) | İki açıklıklı sürekli kirişler (250×400 mm, açıklık 2×4000); deneysel kesit momenti tablolanmamış (Tab.6'daki Mr1/Mr2 AS 5100.5 **tahmini**, Pexp toplam yük). Mid-span/mesnet menteşe momenti yalnız Tab.4 mesnet reaksiyon oranları + kritik açıklığın hangisi olduğu (E/W) bilgisiyle türetilebilir; kritik açıklık ayrımı metinde yok ve toplam yük öz ağırlık/ ped ağırlığı içerir (11,7 kN + 7,1 kN + 2,3 kN/m) → varsayımsız satır çıkarılamadı. B00(+30) kesmeyle göçtü. **Kaydedilen girdi verisi (gelecek kullanım):** Tab.1: A serisi mesnet üst 1240 mm² (%1,38) / açıklık alt 620 mm² (%0,69); B serisi mesnet 620 / açıklık 930 mm² (%1,03); Φ20 fy=520, Φ28 fy=540 MPa (s.696); fc silindir (150×300) 57 / 66,1 / 57,4 MPa (0/30/60 kg/m³; s.698); lif Dramix 5D-65/60-BG, ft 2300 MPa, gerçek dozaj 27,3 ve 51,7 kg/m³ (Sonuç, s.705); **EN 14651 fR (Tab.2, s.698; beş numune, ortalama hesapla)**: 30 kg/m³ fR1 = 9,2/6,3/4,0/6,1/5,3 → ort. 6,18; fR3 = 11,1/8,4/7,9/11,1/10,0 → ort. 9,70; 60 kg/m³ fR1 = 7,5/6,5/12,6/6,5/9,6 → ort. 8,54; fR3 = 10,5/11,8/14,5/10,5/13,0 → ort. 12,06 MPa (ortalamalar bu çalışmada hesaplandı, kaynakta "Avg." yalnız f1,5 için); Pexp (Tab.3, s.701): A: 243,9/278,1/299,2; B: 258,8/289,2/289,7 kN. Kiriş boyutları Fig.1/s.696. Tek başına tek-kesit doğrulaması için uygun değil; yalnız sürekli-kiriş (plastik analiz) karşılaştırması için. |
| **Tiberti vd. (2015)** | SATIR YOK (KAPSAM DIŞI) | Eğilme kirişi değil; 97 adet eksenel çekme "tie" numunesi (b=50–200 mm, tek merkezi çubuk). EN 14651 fR (Tab.5, s.28) yalnız malzeme karakterizasyonu; kiriş taşıma momenti yok. Çatlak aralığı verisi (SLS) için başka amaçla kullanılabilir. |
| Meda2012: 2φ16-UB-PC, 2φ16-UB-30 | DIŞLANDI | Bağsız (unbonded) donatı. Pmax 87 ve 94 kN (Tab.3). |

### 6.4 Özet sayıları (bu ek)
- Meda2012: 5 satır · Dancygier2016: 25 satır · Zijl2013: 2 satır (toplam 32). Foster, Mahmood, Tiberti: 0 satır.
- **Tam girdili (d, fc, fy, fR1, fR3, Mu hepsi dolu) yeni satır: 0.** Hiçbir yeni satırda fc (silindir) ve EN 14651 fR1/fR3 birlikte yok (Meda: fc küp/belirsiz + UNI 11039; Dancygier: küp + RILEM eğrisi; Zijl: küp + yarma). Bu nedenle S2 doğrulaması için fR-tabanlı lif modelinin parametreleri bu 32 satırdan çıkarılamaz; d, fy, As, Mu dolu satırlar (Meda 5, Dancygier 24, 0,40 % grubu hariç fy) yalnız lifsiz/donatı-dominant kesit kontrolü ve lif katkısının büyüklük mertebesi için kullanılabilir.
- ŞÜPHELİ (denge oranı > 1,6): Dancygier 40-015-1 (1,64), 40-015-2 (1,61), NF-1-015-1 (1,63). Diğer tüm yeni satırlar ≤ 1,52 (Dancygier 3,27 % grubunda 0,75–0,82: iki sıralı donatı; oran < 1 çift sıra ve d değerinin 0,9d ile uyumsuzluğundan olabilir, ŞÜPHELİ değil).
- Birleşik (mevcut + yeni) CSV'de önceden bilinen: Nogales2025 d boş (9 satır), Buttignol2018 fc/fy/d boş (3 satır).

### 6.5 Kaynakça (APA 7, bu ek)
- Dancygier, A. N., & Berkover, E. (2016). Cracking localization and reduced ductility in fiber-reinforced concrete beams with low reinforcement ratios. *Engineering Structures, 111*, 411–424. https://doi.org/10.1016/j.engstruct.2015.11.046
- Foster, S. J., & Parvez, A. (2019). Assessment of model error for reinforced concrete beams with steel fibers in bending. *Structural Concrete, 20*(3), 1010–1021. https://doi.org/10.1002/suco.201800090
- Mahmood, S. M. F., Agarwal, A., Foster, S. J., & Valipour, H. (2018). Flexural performance of steel fibre reinforced concrete beams designed for moment redistribution. *Engineering Structures, 177*, 695–706. https://doi.org/10.1016/j.engstruct.2018.10.007
- Meda, A., Minelli, F., & Plizzari, G. A. (2012). Flexural behaviour of RC beams in fibre reinforced concrete. *Composites Part B: Engineering, 43*(8), 2930–2937. https://doi.org/10.1016/j.compositesb.2012.06.003
- Tiberti, G., Minelli, F., & Plizzari, G. (2015). Cracking behavior in reinforced concrete members with steel fibers: A comprehensive experimental study. *Cement and Concrete Research, 68*, 24–34. https://doi.org/10.1016/j.cemconres.2014.10.011
- van Zijl, G. P. A. G., & Mbewe, P. B. K. (2013). Flexural modelling of steel fibre-reinforced concrete beams with and without steel bars. *Engineering Structures, 53*, 52–62.

### 6.6 Yöntem notu (denge kontrolü)
Oran = Mu_exp / (As·fy·0,9·d), tüm kirişlerde aynı formül; Meda'da üst donatı (2φ10) ihmal edildi; Dancygier 0,40 % grubunda Σ(As_i·fy_i). Oran ≥ 1,0 beklenir (pekleşme, lif); > 1,6 ŞÜPHELİ. Mu değerleri öz ağırlıksız (Meda, Dancygier); öz ağırlık momenti birim ağırlık verilmediği için eklenmedi.
