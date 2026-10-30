# Review Round V2e: ikinci tur (Z1–Z8 kapanışı, FemCore burulma, PCR-1/2, V2e) (Critic, 2026-10-30)

Denetlenen: `04_Results/VALIDATION_REPORT.md` (satır 82–155), `03_Code_Execution/VALIDATION_SPEC.md` (13–29), `PROGRAM_CHANGE_REQUESTS.md`, `runs/*` (29 dosya), `program_work/FemCore/FemModal.vb`, `FemRcModel/ColumnDesign.vb`, `EtabsProbe/{Program,Design,DesignBeam,SfrcX}.vb`, `tools/sfrc_independent_check.py`. Kod değiştirilmedi. Python yalnız ham logları ayrıştırmak ve S1 betiğini çalıştırmak için kullanıldı.

## KARAR
- **V2 FemCore / kolon / kiriş kısmı: [REVİZYON TALEBİ: aşağıdaki R1–R9 kapanmadan kampanyaya geçilmez].** Önceki turdan belirgin ilerleme var; FemCore↔ETABS sayısal uyumu (FE1–FE3, statik FE4) ve kiriş kapıları (B1–B3) ham logla tutuyor. Ancak CD1/CD2 "GEÇTİ" kararı mevcut kanıtla desteklenmiyor (güç yok, ayarlama kümesi = sınama kümesi), kanıt arşivi bütünlüğü (AGENTS kural 7, 11) eksik, üretim donatı düzeni hiç sınanmadı.
- **SFRC (S2): [REVİZYON TALEBİ: S2 yapılmadı].** S1 (cebirsel/sayısal bağımsız entegrasyon) yeniden hesaplandı ve KABUL; modelin kendisi (fib MC2010 σ–w + ACI bloğu) hiçbir deneyle sınanmadı. Lifli sonuçlar "model sonucu" etiketiyle kalır; lifli koşular için onay yok.

## 1. Z1–Z8 tek tek (Soru 1)

| Z | Durum | Kanıt / not |
|---|---|---|
| Z1 ham log arşivi | **KISMEN** | Var: ECC=0/ECC=%5 V2a logları, V2b ECC=0 ve ECC=%5 (240 kolon satırı, 48'in hepsi), 62/16/59 test çıktıları. **Yok:** NORZ=1 koşusu (rijit bölge %18,8/%3,9/%0,6, Rapor:21; `grep "no rigid zones = True"` runs/ altında sonuç vermedi); ECCDIAG ters-çözüm çıktısı (bkz. B2); RS çubuk kuvvetinin 116 elemanda karşılaştırması. SHA-256 listesi hatalı (bkz. B1). |
| Z2 yazılı eşik, üretim kısıtı kıyası | **KISMEN** | Eşikler yazılı (Spec:13–29, post-hoc notu dürüst). Üretim kısıtı kıyası yapıldı (V2e `PRODUCTION` satırları). Ancak CD3 sonucu eşiğe göre KALDI iken "TANIM FARKI" denmiş (B4); eşiklerde IA onayı kaydı yok (⚠️ duruyor). |
| Z3 V2b ECC=0 | **KARŞILANDI** | `V2b_ecc0` ↔ `V2b_ecc5_final`: 4.11 X+0,3Y oran 0,985–1,160 (ort. 1,100) ↔ 0,994–1,167 (ort. 1,104); rapor değerleri tutuyor. "Burulma kaynaklı" yorumunun geri alınması doğru. |
| Z4 burulma kök nedeni | **KARŞILANDI (sayısal), kanıt arşivi EKSİK** | Köşe yer değiştirme farkı 6,3 % → 0,093 %; M3 uyumu 0,12 %/0,3 % ham logda (`V2a_ecc5_storyshear_torsion` s.70–87). Ters çözüm tablosu (19,6/27,6/39,3/62,3) ham logda yok. |
| Z5 yüksek D/C, narinlik, üretim düzeni | **KISMEN; sınırlama olarak kabul EDİLEMEZ** | δns ölçüldü (nomag/default logları). D/C 0,8–1,0 ve üretim düzeni açık bırakıldı; üretim düzeni ETABS'te kurulabilir (R4), "sınırlama" yerine yapılmalı. |
| Z6 kolon kesme/TS 500 menüsü | **AÇIK (dürüstçe belirtilmiş)** | Rapor:125. Kampanya kapsamına TBDY kesme zinciri girecekse şart; girmeyecekse kapsam dışı yazılmalı (R7). |
| Z7 SFRC yolu | **YOL yazılı, S2 AÇIK** | S1 bizzat yeniden çalıştırıldı (aşağıda). |
| Z8 tercih dökümü, uyarılar | **KISMEN** | Tercih/üzerine yazma dökümü logda (V2b_ecc5 s.4–21, 122–134). "Cannot open file" açıklaması log sırasıyla çelişiyor (O3). |

### Bizzat yeniden hesaplanan sayılar (16 kalem; hepsi ham logdan)
1. `SHA256SUMS`: `sha256sum -c` → 24/25 OK, 1 FAILED (listenin kendi satırı; bkz. B1).
2. V2a taban kesmesi: 305,09/304,66 − 1 = %0,141 (EX), 293,96/293,54 − 1 = %0,143 (EY). Rapor %0,141/%0,144 ✓.
3. Periyot T12: 0,06049/0,06039 − 1 = %0,166 (log −0,164; yuvarlama). FE1 ✓.
4. Köşe yer değiştirme EX kat 1: ECC=%5 ETABS 1,3213 / ECC=0 ETABS 1,2558 → +%5,2 (burulma katkısı gerçek); FemCore farkı %0,093/%0,092 ✓ (FE3).
5. C001 M3 EX: 50,42/50,36 = +%0,12; EY: 3,62/3,61 = +%0,28 ✓ (Rapor:93–94 doğru).
6. Ters çözülen tork toplamı: 19,6+27,6+39,3+62,3 = 148,8 kN·m; /(0,05×10 m) = 297,6 kN ↔ ETABS taban kesmesi 304,66 (fark %2,3; Rapor "≈" diyor, açıklanmamış).
7. V2e CD2 (satır bazında, 3×240 satır): min/maks oranlar 500/400: 0,983–1,177; 300/300: 0,999–1,237; 260/240: 1,005–1,472 ✓ (rapor tablo değerleri, SUMMARY satırlarından değil satır düzeyinden türetilmiş; SUMMARY max 1,180/1,238/1,470 farklı yuvarlama; bkz. D1).
8. U1 kritik D/C: 0,818 ↔ 0,817 (−%0,1); 1,683 ↔ 1,691 (+%0,5) ✓.
9. **Yanlış-uygun = 0 sayısı doğru, ama ETABS D/C > 1 olan satır sayısı 720 satırda yalnız 2** (260/240, U1, C111 ve C211 = aynı kolon tipi, ETABS 1,683; üretim 2,291). D/C ∈ [0,8; 1,0] satır sayısı 2 (300/300). CD1'in istatistiksel gücü fiilen tek gözlem (R2).
10. **CD3 yeniden hesap (U1 maks D/C, üretim/ETABS):** 500/400: 0,297/0,354 = **−%16,1**; 300/300: 0,655/0,818 = **−%19,9**; 260/240: 2,291/1,683 = **+%36,1**. Rapor satır 108 "%17–41 düşük" V2d (PCR öncesi) değerlerinden gelmiş (0,494/0,838 = −%41); V2e (PCR sonrası) ile tutarsız (D2).
11. Deprem birleşiminde üretim/ETABS maks D/C: 0,316/0,295 = +%7,1; 0,232/0,197 = +%17,8; 0,321/0,297 = +%8,1; 0,237/0,201 = +%17,9 ✓ (+7…+18).
12. V2c: üst As oranı 1,000 (68×5), alt 0,981–1,048, gravite Av/s 1,000, kapasite kesmesi 0,964–0,985, 6 kirişte 3,229 ✓ (`V2c_beam_rerun_prov532_storyshear`). B1–B3 sayıları doğru.
13. **S1 yeniden çalıştırıldı** (`python -I sfrc_independent_check.py SFRC_grid`): 216 kesit, maks |Δc| %0,0055, maks |ΔMn| %0,0008, >%1 olan 0 ✓. (Izgarada `FibreLimited=1` satırı 200/216: ızgara büyük oranda lif-sınırlı rejimi örnekliyor; yorum için bkz. S1 notu.)
14. Test sayıları: FemCore 16, FemRcModel **62 (PCR-1/2 öncesi, sonrası ve final aynı)**, RcCore 59 ✓ ama bkz. T1.
15. **Derleme karması:** Rapor:134'teki EtabsProbe.exe `442946f021f6…`, FemCore.dll `6f535f2bb701…`, FemRcModel.dll `9383abd8fbc2…`; diskteki güncel (15:31) ikililer: `e6fd27344df3…`, `d0aacddefc88…`, `a55b1a78f236…`. **Uyuşmuyor** (B1).
16. Z3 `RATIO` satırları ECC=0/%5 (yukarıda) ✓; `V2b` 48 satır arşivi: 240 kolon satırı ✓ (önceki B3 kapandı).

## 2. Bulgular

### Bütünlük / kanıt (AGENTS 2.7, 2.11)
- **[YÜKSEK] B1. SHA-256 listesi ve derleme sürümü hatalı.** (a) `SHA256SUMS_2026-10-30.txt` kendi satırını içeriyor (boş dosya özeti `e3b0c442…`, doğrulama FAILED). (b) Listede olmayan dosyalar: `V2_fem_etabs_log_2026-10-29.txt`, `tests_FemCore.Tests_final`, `tests_FemRcModel.Tests_final` (özet `0963ef12…` farklı bir dosya), `tests_RcCore.Tests_final`; rapor "tests_*_final" ve "25 dosya" diyor (Rapor:153), dizinde 29 dosya var. (c) Rapor:134'teki derleme özetleri güncel ikililerle uyuşmuyor; V2e koşularının hangi ikiliyle yapıldığı kayıtsız. Düzeltme: PCR-1/2 sonrası temiz derleme, `EtabsProbe.exe/FemCore.dll/FemRcModel.dll/RcCore.dll` SHA-256 + zaman + ETABS 22.6.0 sürümü, her `runs/` dosyası için `sha256sum` (listenin kendisi hariç) ve her logun hangi derlemeyle üretildiği.
- **[YÜKSEK] B2. Z4'ün merkezi kanıtı (ters çözülmüş kat torkları) ham logda yok.** `Program.vb:208–276` (ECCDIAG) tanıyı üretir, ama çıktısı `runs/` altında yok (grep "implied ETABS": sonuç yok). Betik ETABS yer değiştirmelerini koda sabit yazıyor (`Program.vb:213`, ETABS 10-29 logundan) ve genişlikleri (10/15 m) sabit (`:274`). AGENTS 11: kaynağa bağlanmadan makaleye girmez. İstenen: ECCDIAG çıktısı arşive, sabit değerlerin kaynağı belirtilmiş.
- **[ORTA] B3. NORZ=1 ve RS çubuk kuvveti kanıtları.** Rijit bölge etkisi (%18,8 / %3,9 / %0,6; Rapor:21) hâlâ ham logsuz (önceki B1'in kalan kısmı). FE4'ün RS kısmı ("RS ≤ %0,3", Rapor:105) logda yalnız 4 örnek eleman/yön ile desteklenir (C001, By314; EX/EY); 116 elemanın RS kuvvetleri karşılaştırılmadı. Spec FE4 "RS büyük bileşen ≤ %0,5" tüm eleman için okunur. İstenen: statik karşılaştırmadaki gibi `CompareStatic` benzeri RS çubuk kuvveti karşılaştırması (116 eleman, ECC=%5).
- **[ORTA] B4. Birim testi kapsamı.** FemRcModel.Tests PASS sayısı PCR-1/2 öncesi, sonrası ve final 62'de sabit; e_min, `DeltaNs`, `DcrWithEffects`, 8 işaret deseni ve `TorsionFromStoryShear`/`StoryForce` için test yok (`grep` Tests/*.vb: eşleşme yok). "62/62" bu düzeltmeleri doğruladığı izlenimi veriyor. İstenen: en az (i) elle hesaplı δns örneği (ACI 6.6.4.5.2), (ii) e_min örneği, (iii) 2 katlı basit modelde `StoryForce` toplamı = taban kesmesi ve tork = ecc·genişlik·F_k, (iv) 8 desen için basit simetrik/asimetrik moment örneği.
- **[DÜŞÜK] B5. Rapor iç tutarsızlıkları.** Rapor:3 "V2 kısmen geçti" ve §1 (satır 16–23) eski %2–6 farkı hâlâ anlatıyor (satır 84 ile geçersiz kılınmış ama §1 silinmemiş); Rapor:134 "19 dosya"; Rapor:25(c) "P-Δ ve ikinci mertebe yok" ile Rapor:122/PCR-2 "sway, P-Δ varsayımı" çelişiyor (bkz. N2).

### Burulma kök nedeni (Soru 2)
- **Ikna edicilik: YÜKSEK, ama tek modelde.** Hipotez (tork = ecc × genişlik × CQC kat kesmeleri farkı) ters çözümden bulunmuş, sonra **parametresiz ileri sınama** ile doğrulanmış: 2 yön × 4 kat köşe yer değiştirmesi %0,05–0,093, ECC=0 ve %5, ayrıca C001 ve By314 çubuk momentleri %0,1–0,3. Ters çözüm 4 denklem/4 bilinmeyen (tam belirli); doğrulayıcı olan ileri sınamadır, doğru okunmuş. ETABS Help (G12 §4b) "total lateral force applied to the diaphragm" der; bulgu bu belirsizliği "kat kuvveti = kat kesmesi farkı" olarak çözer. Rapor:87 "Help'te yazılı olmayan bir ayrıntı" ifadesi fazla güçlü; "Help'teki ifadenin RS'ye yorumu" yazılmalı.
- **Kalan açıklama/aşırı uydurma riski [ORTA]:** (a) Tek model: düzenli, simetrik plan, kat başına eşit kütle, kütle merkezi = geometrik merkez, rijit diyafram, T1 ≈ T2 yakın ve ayrı burulma modu. Kat kütlesi veya plan değişirse "kat kesmesi farkı" ile "her diyaframın modal kuvveti" arasındaki fark başka yönde kayabilir. (b) CQC kat kesmesi farkı, ETABS'in gerçekte yaptığı bir işlemin (örn. mod başına burulma uygulayıp birleştirme) bu geometriye özgü bir yaklaşığı olabilir; iki hipotez bu modelde ayrıştırılamıyor (sayısal ayrım yok). (c) Birleştirme kuralı mutlak toplam, kod yorumunda `:368–370` hâlâ eski ("F_k: CQC inertial force") açıklamayı taşıyor. (d) `FemModal.vb:345–354` diyafram indeksi = kat sırası (0 = en alt) varsayımına dayanır, doğrulama/assert yok; kat sırası ters atanırsa sessizce yanlış tork üretir [DÜŞÜK]. (e) Σ F_k ≠ ETABS taban kesmesi (%2,3). 
- **Önerilen kapanış:** ikinci bir yapılandırmayla (örn. kat başına farklı kütle veya farklı kesit seti; ideal olarak kütle merkezi kaymış bir diyafram) tek ECC=%5 koşusu; yoksa makalede "ETABS 22.6.0 davranışına, test edilen düzenli planlar için ayarlandı; nihai optimum ETABS'te yeniden analiz edilir" diye açıkça sınırlanmalı. Nihai ETABS yeniden analizi (AGENTS 2.1) riski sınırlar ama optimizasyon yönünü saptırabilir.

### Narinlik / δns hizalaması (Soru 3)
- **[YÜKSEK] N1. Bağımsız ACI savunması kısmen mümkün, kısmen ETABS kopyası.**
  - *Bağımsız savunulabilir:* EI = 0,4 EcIg/(1+βdns) (6.6.4.4.4a), Pc = π²EI/(k lu)² ile k = 1, δns = Cm/(1 − Pu/0,75Pc) ≥ 1 (6.6.4.5.2), Cm = 1,0 (muhafazakâr üst sınır; 6.6.4.5.3 ifadesi ≤ 1), lu/r ≤ 22 (6.2.5(a), sway çerçeve), r = 0,3h (6.2.5.1), βdns = 1,4G/Pu (6.6.4.4.4 tanımı), e_min = 15 mm + 0,03h (6.6.4.5.4).
  - *ETABS'e uydurulmuş (bağımsız ACI maddesi yok):* (i) `LuRatio = 0,9` (`ColumnDesign.vb:69, 132`): ACI 6.2.5.1 lu'yu kat döşemesi/kiriş arası **net yükseklik** olarak tanımlar; kiriş 600 mm ve kat 3000 mm iken net yükseklik 2400 mm = 0,8 L olur. 0,9 L ETABS'in "program determined" sonucudur ve logda yalnız **C001** için basılmış (V2b_ecc5 s.124–125); üst kat kolonlarında ETABS'in oranı muhtemelen farklıdır (iki ucunda rijit bölge). Bu, V2e'deki +%24 (300 mm) ve +%47 (260/240) üst sapmaların muhtemel nedenidir (en şişen satırlar C1xx/C2xx üst kat kolonları; **varsayım, doğrulanmadı**). (ii) δns yalnız gravite birleşiminde ve sway sınırı 22 ile uygulanıyor; sismik birleşimde G kısmı (Mns) büyütülmüyor (`ColumnDesign.vb:124`, δ = 1,0). ACI 6.6.4.6.1'e göre sway çerçevede de Mns için δns uygulanır; "ETABS yapmıyor" bağımsız bir gerekçe değildir. (iii) Cm = 1 PCR notunda açıkça "0,6+0,4M1/M2 ETABS'e göre %11 düşük kaldı" gerekçesiyle seçilmiş (`PROGRAM_CHANGE_REQUESTS.md:15`): bu seçim ETABS'e hizalama amaçlıdır (muhafazakâr olduğu için ACI ile çelişmez, ama gerekçe "ACI izin verir" olmalı).
  - **İkinci mertebe:** Ö1 analizinde (FemCore ve ETABS) P-Δ yok (`FemRcModel` içinde `UseMassPDelta` çağrısı yok; V2a statik uyumu %0,002 doğrusal). "δs = 1, P-Δ varsayımı" (Rapor:122, PCR-2) bu nedenle kanıtsızdır. Elle kaba tahmin (rapor değil, benim hesabım; arşivsiz): Q = ΣPu Δo/(Vus lc) ≈ 8500 kN × 1,32 mm / (305 kN × 3 m) ≈ 0,012 (Δo ×D=3 ile ≈ 0,04) < 0,05; yani ikinci mertebe ihmali savunulabilir ama **hesap raporda yok**. Q (veya TBDY θ) her kat için hesaplanıp kısıt/raporlama olarak eklenmeli.
  - **Tasarım uzayı ile ilişki:** alt sınır 300 mm'de lu/r = 2700/90 = 30 > 22; yani maliyet minimumunun yığıldığı bölgede δns zaten etkin (300 mm: U1 +%15). Bu nedenle δns hizalaması "uç durum" değil merkezî; doğrulama kapsamı 300/300 tek set.
- **Makale için yazım önerisi:** "Narinlik etkileri ACI 318-14 §6.6.4.5 moment büyütme yöntemiyle, Cm = 1,0 (muhafazakâr), k = 1,0, lu/r > 22 için, gravite yük birleşiminde ... ve yanal birleşimlerde yalnız Mns'e uygulanarak değerlendirildi. İkinci mertebe yanal etkiler Q ≤ 0,05 kontrolüyle (Çizelge X) ihmal edildi. lu, ACI §6.2.5.1'e göre net kat yüksekliğinden hesaplandı. ETABS 22.6.0 CFD ile aynı girdiler (K, lu, Cm, çerçeve tipi) `SetOverwrite` ile açıkça tanımlanarak karşılaştırıldı." Yani: **programı ETABS'in varsayılanına uydurmak yerine aynı girdileri iki tarafa açıkça yazıp algoritmayı doğrula** (R3). "ETABS davranışının kopyası" ifadesi makalede kullanılmamalı.

### Eşiklerin sonradan yazılması ve karar tutarlılığı (Soru 4)
- **[YÜKSEK] D1. Ayarlama kümesi = sınama kümesi.** Spec:14 notu dürüst (eşikler ilk koşular görüldükten sonra yazıldı). Ek sorun: V2b→V2e boyunca (8 işaret deseni, e_min, δns, lu = 0,9, Cm = 1, hedef tork düzeltmesi) hep aynı Ö1 deneme tasarımı, aynı kuvvetler ve aynı 3 kesit seti ile ayarlanıp aynı veriyle "GEÇTİ" ilan edildi. Bu bir **kalibrasyon**, bağımsız doğrulama değildir. Bağımsız (hold-out) bir ikinci tasarım (farklı kesit/fc/yük düzeni, tercihen optimizasyonun gerçek bir ara çözümü) olmadan CD1/CD2/FE4 "doğrulandı" denmemeli; "ETABS 22.6.0 ile hizalandı (kalibrasyon kümesi)" denmeli. (Önceki A2 hâlâ açık.)
- **[YÜKSEK] D2. CD1 vacuous.** 720 satırdan yalnız 2 satırda ETABS D/C > 1 (aynı kolon tipi, aynı birleşim), 2 satır 0,8–1,0 bandında. "Yanlış-uygun = 0" sonucu karar sınırında neredeyse hiç sınanmamış. 230/200 koşusunda (V2e ilk δns sürümü logu) bir **yanlış-uygun** var: U1 C311, ETABS 1,493, üretim 0,865; ayrıca C111/C211'de ETABS 0,917 iken üretim 10,0 (kapak): iki yönlü uyumsuzluk. Bu koşu düzeltilmiş kodla **yeniden arşivlenmemiş**; rapor "tasarım uzayı dışı (300 mm alt sınırın altı)" diyor (savunulabilir, TBDY 7.3.1 alt sınırı) ama aynı gerekçe 260/240 için de geçerli (300 mm'nin altı); yani CD1'in D/C > 1 gözlemleri tamamen tasarım uzayı dışından. Gerçek tasarım uzayında (≥ 300 mm) karar sınırı gözlemi sıfır.
- **[YÜKSEK] D3. CD3 eşiğe göre KALDI, "TANIM FARKI" etiketi eşiği geçersiz kılıyor.** Spec:24: ≤ %15 güvenli, güvensiz yönde ≤ %5. U1 üretim D/C ETABS'ten −%16,1 ve −%19,9 düşük (güvensiz yön tanımı) ve D/C > 1 bölgesinde +%36 yüksek (güvenli ama eşik dışı). Satır 108'deki durum "TANIM FARKI" eşik kararı değildir. Dürüst biçim: "CD3: KALDI (neden: sabit-P moment oranı ≠ radyal oran; uygunluk kararı için CD1 esas)" ve eşik IA onayıyla CD3'ü "bilgi amaçlı" yapacaksa bu **önceden** yazılmalı. Ayrıca satır 108'deki "%17–41" PCR sonrası değerlerle tutmuyor (−%16/−%20).
- **[ORTA] D4. CD2 tanımı Spec'ten sapıyor.** CD2 "Radyal+e_min programı / ETABS ≥ 0,95" (Spec:23); V2e ise radyal + e_min + δns kullanıyor (Design.vb:205) ve sonuç tabloda "üretim = DcrWithEffects" (Rapor:140) cümlesiyle karışıyor: tablodaki oranlar `program+emin` (radyal; Design.vb:205) sütunundan, "yanlış-uygun" `PRODUCTION` (sabit-P; `:202`) sütunundan. İki farklı büyüklük aynı tabloda; sütun adları açık yazılmalı, Spec CD2'ye "+δns" eklenmeli (IA onayı).
- **[DÜŞÜK] D5.** V2e tablosunda satır 148: "720 satır": 3 set × 5 birleşim × 48 = 720 ✓; ancak ilk δns sürümüyle 230/200 koşusu tabloya girmedi (doğru) ama arşivde duruyor (isim "firstDnsVersion"; açıklanmalı).

### Kapsam dışı ifadeler ve abartı (Soru 5)
Doğru sınırlama olarak yazılmış: S2, Z6, D/C 0,8–1,0 (Rapor:152), üretim donatı düzeni (Rapor:152), Cm = 1 / lu = 0,9 varsayımı (Rapor:152), 230/200 aralığı (Rapor:150), PCR-3/PCR-4 (Rapor:151). Bu yönüyle rapor önceki turdan çok daha dürüst.
Abartı / eksik sınırlama:
- **[YÜKSEK] A1.** Rapor:155 "FE1–FE4, CD1, CD2, B1–B3, S1 geçti" cümlesi D1–D3, B1–B4 ile desteklenmiyor. "V2" adı Spec:7'deki V2 (literatür referansı ≤ %5) ile hâlâ çakışıyor; "V2 alt kapıları (FE/CD/B/S)" olarak anılmalı; asıl V2 (Ö2/Ö3 literatür) açık.
- **[YÜKSEK] A2. Üretim donatı düzeni yalnız "sınanmadı" değil, tutarlılığı da kuşkulu.** `ColumnDesign.vb:102–103`: `sx = MakeSection(b,h,…)` ve `sy = MakeSection(h,b,…)` aynı N için **bağımsız** katman düzeni üretir. Örnek (500/400, N = 16, kendi hesabım): sx: 2 sıra × 8 çubuk (tüm çubuklar iki kısa-kenar yüzeyinde); sy: 3 katman 7+2+7. İki doğrultu fiziksel olarak **farklı kolonu** temsil eder; kare olmayan kolonda çift eksenli doğrusal toplam (BiaxialDcr) tutarsız. V2e bunu görmez (`Design.vb:169` simetrik `SymSection` kullanır). Hem düzeltilmeli hem ETABS rectangular pattern (n3 × n2) ile karşılaştırılmalı; ETABS bu düzeni kurabilir (SetRebarColumn çubuk sayıları), yani "sınırlama" mazereti geçerli değildir.
- **[ORTA] A3. B3 "GEÇTİ" yalnız probe'un ACI zincirini doğrular** (DesignBeam.vb:146–147, 1,25 fy, sağlanan donatı), üretimdeki `BeamShear` (TBDY 7.4.5, 1,4 Mr/ln) hiç sınanmadı (PCR-4 açık; Rapor:76 bunu söylüyor ama kapı tablosu satır 111 söylemiyor). Ayrıca B3 yalnız "sağlanan = 532 mm²" (zarf = sağlanan) durumunda tutuyor; 2000 mm² için 0,35–0,46. Kapı tablosuna "probe zinciri; üretim zinciri doğrulanmadı" notu.
- **[ORTA] A4. FE4 başlığı** "statik ≤ %0,002; RS ≤ %0,3" (Rapor:105) RS için örneklemeye (4 değer) dayanır (B3).
- **[ORTA] O3. Z8 açıklaması log sırasıyla çelişiyor.** Rapor:131: `Cannot open file … .Y_` "tablo uygulaması sırasında … hemen ardından tables applied: fatal=0". Logda (`V2a_ecc5` s.19–36) "tables applied" ve Mass Source dökümü **önce**, 7× "Cannot open file" **sonra** gelir (analiz öncesi/sırası). Neden bilinmiyor; rapor "bilinmiyor; etkisizliği şu kanıtla: statik %0,002, periyot %0,16" diye yazmalı. `SetDiaphragm` "şablonda zaten var" iddiası da doğrulanmamış (yine sonuçlar doğru olduğundan etkisiz).
- **[ORTA] A5. S1 notu.** ΔMn %0,0008 aynı modelin yeniden yazımıdır (betik de aynı ajan çıktısı); ortak mod hata riskini dışlamaz. Izgara: 216 kesitten 200'ü `FibreLimited=1`; ızgara genelde lif-baskın rejimi örnekler, çelik-akma-baskın rejim az. Bunu S1 sonucuna ekleyin; fib σ–w denklemleri (fFts = 0,45 fR1; fFtu Denk. 5.6-6; wu = min(0,02 lcs; 2,5)) kod ve betikte aynı: **bağımsız kaynakla sayfa/madde doğrulaması** (Literature Analyst) yapılmamış.
- **[DÜŞÜK] A6.** `DeltaNs` içinde `ratio/small/big` hesaplanıp kullanılmıyor (Cm = 1; `ColumnDesign.vb:75–76`); ölü kod ve yanıltıcı parametre (mA, mB).
- **[DÜŞÜK] A7.** "Sayı yuvarlama": Rapor:147 `1,177/1,237/1,472` satır düzeyinden; SUMMARY `max` 1,180/1,238/1,470; hangisinin kaynak olduğu açıklanmalı.

## 3. Eşiğe göre durum tablosu (benim kararım)
| Kapı | Rapor | Critic |
|---|---|---|
| FE1, FE2, FE3 | GEÇTİ | **GEÇTİ** (logdan yeniden hesaplandı); tek model/tek köşe notu |
| FE4 statik | GEÇTİ | **GEÇTİ** (116 eleman, %0,002) |
| FE4 RS | GEÇTİ | **KANITSIZ** (4 örnek) |
| CD1 | GEÇTİ | **ŞİMDİLİK GEÇERLİ SAYILMAZ** (güç yok, hold-out yok) |
| CD2 | GEÇTİ | **KISMEN** (kalibrasyon kümesinde sayısal olarak 0,983/0,999/1,005; Spec tanımı ≠ ölçülen) |
| CD3 | TANIM FARKI | **KALDI** (−%16/−%20 güvensiz yön > %5) |
| B1, B2 | GEÇTİ | **GEÇTİ** (tek kesit, tek tasarım) |
| B3 | GEÇTİ | **GEÇTİ (probe zinciri)**; üretim TBDY zinciri doğrulanmadı |
| S1 | GEÇTİ | **GEÇTİ** (bizzat çalıştırıldı) |
| S2 | AÇIK | **AÇIK** |

## 4. Kampanyadan önce zorunlu şartlar (Soru 6)
- **R1** Kanıt bütünlüğü: SHA-256 listesi yeniden (kendini içermeden, tüm 29+ dosya, final ikili özetleri, derleme zamanı, ETABS sürümü); `ECCDIAG` ve `NORZ=1` logları; RS çubuk kuvveti 116 eleman karşılaştırması (B1–B3).
- **R2** Hold-out doğrulama: Ö1 için ikinci bir tasarım (kalibrasyonda kullanılmamış sections/fc), kolon D/C'nin 0,8–1,2 bandında **en az 20 bağımsız kolon×birleşim** içeren (300–500 mm tasarım uzayında; çok küçük kesit değil) CD1/CD2 yeniden ölçümü; eşikleri koşudan önce IA onayıyla kilitle (D1, D2).
- **R3** Narinlik girdilerini eşitle: lu = net kat yüksekliği (ACI 6.2.5.1) programda hesaplanır; ETABS'te `SetOverwrite` ile aynı lu/K/Cm/çerçeve tipi yazılır; Mns için δns'in sismik birleşimlerdeki ele alınışı karara bağlanır (ACI 6.6.4.6.1 ile tutarlı ya da gerekçeli muhafazakâr sınır); Q ≤ 0,05 (veya TBDY θ) her kat için hesaplanıp kayda alınır (N1).
- **R4** Üretim donatı düzeni: `sx/sy` tutarsızlığı (A2) çözülür; ETABS'te aynı düzenle karşılaştırılır (en az 2 kesit, kare olmayan dahil).
- **R5** CD3 sonucu dürüstçe KALDI yazılır veya IA onayıyla (önceden) bilgi amaçlı sayılır; CD2 tanımı Spec ile uyumlu hâle getirilir; Rapor:3, 16–23, 25, 108, 134, 153, 155 güncellenir (B5, D3, D4, A1).
- **R6** Birim testleri: δns, e_min, 8 desen, `StoryForce` torku (B4); PASS sayısının bu testlerle arttığı gösterilir.
- **R7** Z6/PCR-4: kampanyada TBDY kapasite/kesme zinciri kullanılacaksa ETABS TS 500 menüsü karşılaştırması; kullanılmayacaksa (kolon kesme ACI) makale ve Spec kapsam dışı yazar. PCR-3 ve PCR-4 için IA kararı.
- **R8** Burulma: ikinci yapılandırma sınaması **veya** makalede açık sınırlama (bkz. "Burulma kök nedeni" bölümü).
- **R9** SFRC: S2 (Literature Analyst; yayınlanmış lifli kiriş eğilme deneyleri, γF = 1 ve ortalama fR ile; eşik Spec:29), lifsiz koşuyla başlanabilir ama lifli koşular S2 sonrası. Lif kısıtını (fR çarpanları 0,85/0,70, fFtu = 0) duyarlılık olarak planla.
- **R10** IA'nın eşik onayı (Spec ⚠️) ve kampanya onayı (AGENTS 2.10) kayıtlı olmalı.

Not: Lifsiz kampanya (yalnız R1–R8 sonrası) ile lifli kampanya (R9 sonrası) ayrılabilir; hakem görüşüm lifsiz tarafın R1–R8 ile açılabileceği yönündedir, lifli tarafın değil.

## 5. Kontrol listesi (bu tur)
| Madde | Durum |
|---|---|
| Rapor sayıları ham logla tutuyor | EVET (V2a ECC0/ECC5, V2b, V2c, V2e satır düzeyi); HAYIR (ECCDIAG tork tablosu, NORZ, RS çubuk 116) |
| SHA-256 listesi doğrulandı | HAYIR (1 FAILED, 4 dosya eksik, derleme özetleri güncel ikililerle uyuşmuyor) |
| Önceden tanımlı eşik ve eşikle tutarlı karar | KISMEN (CD3 tutarsız; eşikler post-hoc, onaysız) |
| Karşılaştırma üretim koduyla | KISMEN (üretim formülü, ama simetrik düzenle) |
| Doğrulanmamış yorumlar etiketli | KISMEN (O3, lu/Cm gerekçeleri) |
| "N/A" hücre | EVET (yok) |
| Birim disiplini | EVET |

## 6. En önemli 5 madde
1. **D2/D1:** CD1 "yanlış-uygun = 0" sonucu 720 satırın yalnız 2'sinde ETABS D/C > 1 (tek kolon tipi, tasarım uzayı dışı 260/240 kesit); ayarlama kümesi = sınama kümesi; hold-out ve 0,8–1,2 bandında yeterli satır olmadan geçti denemez.
2. **D3:** CD3 eşiğe göre KALDI (üretim D/C ETABS'ten −%16/−%20, güvensiz yönde > %5); "tanım farkı" etiketi eşiği aşmıyor; rapordaki "%17–41" PCR öncesi sayıdır.
3. **N1:** δns hizalaması kısmen bağımsız ACI (EI, δns formülü, Cm = 1 muhafazakâr, e_min) kısmen ETABS kopyası (lu = 0,9 L, ACI net yükseklik 0,8 L; sismikte Mns büyütülmüyor; P-Δ yok ama "P-Δ varsayımı" deniyor; Q hesabı yok). ETABS'e uydurma yerine aynı girdileri `SetOverwrite` ile eşitle.
4. **B1/B2/B4:** SHA-256 listesi kendi satırıyla bozuk, 4 dosya eksik, rapordaki derleme özetleri güncel ikililerle uyuşmuyor; burulma ters çözümü (19,6/27,6/39,3/62,3) ve NORZ ham logsuz; 62 test PCR-1/2/δns/8 desen/burulma düzeltmelerini hiç sınamıyor (sayı değişmedi).
5. **A2:** Üretim `MakeSection` sx/sy aynı N için farklı fiziksel donatı düzeni üretiyor (500/400, N=16: 8+8 ↔ 7+2+7); V2e simetrik düzenle yapıldığı için görmüyor; ETABS'te kurulabildiği hâlde sınanmadı. Burulma çözümü (kök neden) güçlü ve yeniden hesaplandı ama tek düzenli plan; S1 yeniden doğrulandı, S2 açık.
