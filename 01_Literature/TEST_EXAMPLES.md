# 🧪 TEST_EXAMPLES.md (WP1 birim testleri için elle hesaplı örnekler)

Seçen/doğrulayan: Claude (kullanıcı isteği 2026-10-28; Developer'a verilmedi). Kural: `.agents/rules/anti_hallucination_evidence.md`.

## Belgeler (`Evidence/DesignGuides/Examples/`)
| Dosya | Bayt | SHA-256 | Kaynak |
|---|---|---|---|
| StructurePoint_SimplySupportedBeam_ACI318-14.pdf | 3.049.271 | 206161c26be2e3d306ac10d48b198766039353aca454c25938731040a95339ec | structurepoint.org (27–33 s.; referans: O. Chaallal, *Reinforced Concrete Structures*, 2. baskı 2018, Presses de l'Université du Québec; sürüm Nisan-08-2…) |
| StructurePoint_TiedColumnInteraction_ACI318-14.pdf | 1.922.176 | 49822393d76a9e49212aeea6d3448e3a5eb3283cce1dc4e2aaa7d1b069c76e95 | structurepoint.org (27 s.; referans: Wight & MacGregor, *Reinforced Concrete: Mechanics and Design*) |

## T1 — Basit mesnetli kiriş (eğilme, minimum donatı, kesme) — ACI 318-14
Girdi (belgeden): f'c = 4,35 ksi, fy = 60 ksi, DL 0,82 + LL 1,00 kip/ft, L = 25 ft, b = 12 in., h = 20 in., #9 boyuna, #3 etriye, örtü 1,5 in.
| Büyüklük | Belge | **Bağımsız yeniden hesap (Python, 2026-10-28)** | Durum |
|---|---|---|---|
| wu = 1,2D+1,6L | 2,58 kip/ft | 2,584 | DOĞRULANDI |
| Vu, Mu | 32,3 kip; 201,88 kip-ft | 32,30; 201,88 | DOĞRULANDI |
| d | 17,56 in. | 17,56 | DOĞRULANDI |
| As gerekli (φ=0,9, tam çözüm) | 2,872 in.² | 2,872 | DOĞRULANDI |
| a | 3,88 in. | 3,88 | DOĞRULANDI |
| As,min (9.6.1.2) | 0,702 in.² | 0,702 | DOĞRULANDI |
| Vu@d; φVc (Eq. 22.5.5.1) | 28,52; 20,85 kip | 28,52; 20,85 | DOĞRULANDI |
Tolerans önerisi: ≤ %0,5. Birim: ABD birimleri; testler SI'ya (×4,448 kN, ×0,3048 m, ×6,895 MPa) çevrilerek yazılır, dönüşüm sabitleri testte açık.
Kapsam: eğilme, minimum donatı, kesme, φ. **Kapsamda yok:** çatlamış rijitlik, deprem detayları, lifli beton.

## T2 — Kare bağlı kolon, P-M etkileşim (7 kontrol noktası) — ACI 318-14
Girdi: f'c = 5000 psi, fy = 60 ksi, 16×16 in., 4#9 üst + 4#9 alt, örtü merkezine 2,5 in. Belgede 7 nokta (saf basınç, fs=0, fs=−0,5fy, fs=−fy, εt=0,005, saf eğilme, saf çekme) elle hesaplı ve spColumn ile karşılaştırılmış. Örnek değer (nokta 2): Pn = 957 kip, φPn = 622 kip, Mn = 261 kip-ft, φMn = 170 kip-ft; Pn,max = 0,80Po = 798 kip.
Durum: **KISMEN** — belge değerleri okundu; bağımsız yeniden hesap WP1'de modül yazıldığında yapılacak (test olarak).
Kapsamda yok: narinlik, biaksiyel eğilme.

## Boşluklar
- **SFRC (fib MC2010) elle hesaplı örnek yok.** Aday: Chen vd. (2021) / Venkateshwaran vd. (2018) yayınlanmış fR değerleri ve SedaDoktora2 makalesinin Ek denklemleri (A5–A23). Bir dış doğrulama örneği aranacak; olmazsa test, standart formüllerinin elle uygulanmasıyla (iki bağımsız hesap) yapılır ve "bağımsız literatür örneği yok" diye raporlanır.
- Deprem (TBDY spektrum) için elle hesaplı örnek: TBDY-2018 içindeki örnekler araştırılacak.
