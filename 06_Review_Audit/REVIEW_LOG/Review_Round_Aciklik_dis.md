# Dış hakem notu: açıklık duyarlılığı gerekli mi?

Kapsam: EX1_SPEC.md (Ö1: 3B, 4 kat, 3×2 açıklık, 5×5 m, kat 3 m, deprem), TASKS S-1 (Ö2: 2B 3 kat × 3 açıklık, 5-6-5 m, yalnız yerçekimi), WP0 notları (Ö3: 3B, 2 kat, 2×2 açıklık, 5×6 m aks, rüzgâr; kesin değil), önceki raporum (fiyat), `FibreTable.vb`/`BeamDesign.vb`. THESIS_EXAMPLE_DATA.md dosyası bulunamadı (bu yüzden Ö2/Ö3 geometrisi ikinci elden). Koşu yok. İç Critic okunmadı.

## 1. "Lif seçilmedi, fiyat 0'da da kazanmadı" tek başına yayınlanabilir mi?

**Hayır, tek geometri/yükleme için olduğu hâliyle genellenemez; ama açıklık taraması da şart değil.**
- Ö1 geometrisi (5 m, deprem) rijitlik/geometri ile belirleniyor (Q ≤ 0,05, kolon boyutu ≥ 20 db, ötelenme); lifin rijitliği değiştirmemesi nedeniyle burada lifin kazanamaması **beklenen** sonuçtur, yani zayıf bir test. Hakem "lif nerede işe yarar?" diye sorar; sorunun cevabı koşulların dayanım-bağlayıcı olduğu bir durum göstermektir.
- Bu makalede o durum zaten var: **Ö2 (yalnız yerçekimi, 5-6-5 m açıklık) dayanım/eğilme bağlayıcı olmaya en yatkın örnek**; Ö3 ikinci geometri. Lifli/lifsiz karşılaştırma üç örnekte yapılırsa, "üç farklı geometri ve yükte (deprem, yerçekimi, rüzgâr) lif seçilmedi / seçildi" ifadesi zaten kapsamı genişletir. Açıklık, kısıt rejimini (dayanım mı rijitlik mi) değiştirdiği için Ö2 bu rolü ayrıca oynar.
- Lif sonucunun genellemesi açık yazılmalı: tek lif ürünü (65/60 BG, 4D/5D), C30/C40, doz ≥ 20 kg/m³, fR tedarikçi ortalaması.

## 2. Özgün değer açıklık taraması olmadan kısıtlı mı?

Hayır. Makalenin özgünlüğü ETABS-OAPI entegre SFRC çerçeve optimizasyonu ve doğrulanmış kapasite tasarımı zinciri (ACI 18 + fib MC2010); açıklık taraması bir parametrik çalışma olup bu özgünlüğün parçası değil. Eksik kalan şey **kapsam bildirimi** ve (Ö2 sonucuna bağlı) en az bir dayanım-bağlayıcı örnek.

Literatür beklentisi (doğrulanamadı; web kullanılmadı, hatırlamaya dayalı, DOI ile teyit gerekir): SFRC kirişlerde lifin faydası çatlama sonrası kapasitenin (fR1, fR3) eğilme/kesme talebine oranı arttıkça, yani yüksek yük/uzun açıklıkta, ve donatı alt sınırlarının bağlamadığı durumda ortaya çıkar; kısa açıklık ve minimum donatı rejiminde fayda azdır. Kaynak adayları: fib Model Code 2010 (SFRC kesit tasarımı, kesme için lif katkısı); Barros ve Figueiras (1999) ve Barros vd. (2005) SFRC eğilme/kesme; Soetens ve Matthys (2014). "Açıklık etkisi" için doğrudan çerçeve optimizasyonu atfı doğrulanamadı.

## 3. Seçenekler ve net öneri

| Seçenek | İş | CPU süresi | Şişme / post-hoc riski | Kod/PCR |
|---|---|---|---|---|
| A. Taramasız (önerilen) | Ö2 ve Ö3 lifli/lifsiz zaten yapılır; kapsam sınırı yazılır; deterministik kırılma tablosu + fiyat 0 koşusu | ek yok | düşük | yok |
| B. Ö1'de 2–3 açıklık, tek yöntem (BTO), 10 koşu, lifsiz+lifli | örn. 5/7,5/10 m: 3×2 hücre × 1,6 saat ≈ 10 saat (8 çekirdek) | orta | yüksek: kutu/havuz, kolon kesit aralığı, ötelenme, FSD ön tasarımı, yük tablosu yeniden kurulur; sonuçlara bakıldıktan sonra karar post-hoc olur | evet (havuz ve yük PCR'si, IA onayı) |
| C. Ayrı makale | parametrik SFRC çerçeve çalışması, tam ızgara, 30 koşu | ≈ 4,7 saat/hücre, onlarca hücre, günler | makale şişmez | kapsamlı |

**Öneri: A.** IA'nın "şişirme" isteğine uygun; açıklık taramasını bu makaleye eklemeyin. Gereken tek ek: (i) Ö2/Ö3 lifli/lifsiz sonuçlarını aynı (fiyat 90 ve 0, fR 1,0) protokolle verin, (ii) lifin ancak dayanımın bağlayıcı olduğu yerde yarar sağlayabileceğini deterministik kontrol (donatı/AsMin/aktif kısıt tablosu) ile gösterin. Ö2'de lif seçilirse bu zaten "lif ne zaman yararlı" cevabıdır; seçilmezse "kapsam: üç örnek, tek ürün" ile sonuç dürüstçe daraltılır. Ö2 sonucu çıktıktan sonra hâlâ soru varsa B'yi **yalnız ek materyalde, önceden kaydedilen protokolle** (açıklıklar ve yöntem sonuçtan önce donmuş) düşünün; C gelecek çalışma olarak yazılır.

Yapılamayacak iddialar: "lif kısa/uzun açıklıkta yararlı/yararsız" (taranmadı), "lif yapısal olarak ekonomik değildir" (genel).
