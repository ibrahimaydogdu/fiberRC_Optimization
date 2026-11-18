# Ö1 SFRC kiriş "bayat lcs" denetimi (salt-okunur; PCR-23 kalıbının Ö1'deki etkisi)

Tarih: 2026-11-17 · Hazırlayan: Claude Code (Developer Agent) · Görev: koordinatör (f) maddesi; iç Critic `Review_Round_Ex2_sonuc_ic.md` A1.1 son madde.
**Ö1 DEĞİŞTİRİLMEDİ:** `program_work/`, `phase2_bin/`, `phase3_bin/` ve Ö1 koşu çıktıları yalnız okundu; V0 SHA denetimi (35 dosya) önce/sonra aynı. Ö1'in kendi sonuçları ve tabloları bu denetimle değiştirilmez; düzeltme ayrı PCR + IA kararı gerektirir.

## 1. Kod kalıbı

`program_work/FemRcModel/BeamDesign.vb` satır 40–53 (`Required`, SFRC dalı): ikiye bölmede her deneme `SfrcBeam.Flexure(..., lcs)` ile, lcs bir **önceki** denemenin c'sinden (`lcs = max(h − c, 0,2h)`, karar sonrası güncellenir) — Ö2'deki PCR-23 öncesi kodla birebir aynı. `StrainOk` (satır 55–58) SFRC'de sabit `0,7h` kullanır; `BeamShear.MrOf` (Mpr, kapasite kesmesi) de `0,7h`. Ö1'de kiriş eğilme D/C'si kısıt değildir (Ö2 öncesi gibi); lifsiz yol `AciBeam.RequiredAs` (etkilenmez).

## 2. Yöntem

- Araç: `03_Code_Execution/ex2_work/O1RequiredAudit/` (Program.vb SHA-256 `d59d9e37…a712`; Ex2.sln dışında, ayrı derleme). Donmuş Ö1 değerlendiricisi (`Ex1Problem.Evaluate` / `EvaluateFull`) her koşu dosyasının son vektörüyle çağrılır; **Faz 2 dosyaları Faz 2 DLL'iyle** (FemRcModel `a2b418c2…`, `phase2_bin` kopyası), **Faz 3 dosyaları Faz 3 DLL'iyle** (`c1a95951…`, `ex2_work/ref` = `phase3_bin`). Kopyalar scratchpad'de; Ö1 klasörlerine yazılmadı.
- Yeniden üretim: 540 lifli koşunun tamamında yeniden değerlendirilen maliyet dosyadaki maliyetle **birebir** (en büyük fark 0,00 TL).
- Her lifli (doz > 0) kiriş grubunun iki yüzü için: sağlanan As (Ö1: bayat-lcs `Required` + AsMin + alt ≥ 0,5 üst), **tutarlı kapasite** = Ö2 PCR-23 fonksiyonu `BeamDesignGravity.FlexFixed/PhiMn` (her As için kendi sabit nokta lcs'si; fy = 420 MPa, Ö1 ile aynı `SfrcBeam`), D/C = Mu/φMn(As), tutarlı gerekli As (`RequiredFace`, eksenel yok), As eksiği (%) ve εt (eski 0,7h ↔ tutarlı lcs).
- Kapsam: `04_Results/phase2/{fibre_A,fibre_ABC,fibre_C}` (270) ve `04_Results/phase3/{20k,30k,ato_alldim,bto_alldim,fibre_price0}/fibre_*` (270) = 540 lifli koşu; **lif > 0 olan koşu 166** (Faz 2: 135; Faz 3: 31), lifli kiriş yüzü 482. Ham tablo: `04_Results/ex2/o1_required_audit.csv` (SHA-256 `1c85b846…460a`).

## 3. Sonuç

| Ölçü | Değer |
|---|---|
| Lif > 0 olan koşu / lifli kiriş yüzü | 166 / 482 |
| Tutarlı D/C > 1 olan yüz (koşu) | **14 (14 koşu)**; Faz 2 fibre_A 8, fibre_C 3; Faz 3 30k fibre_ABC 1, fibre_price0 2 |
| En büyük tutarlı D/C | **1,00243** (Faz 2 fibre_A YAKO run14, By_S34 üst, 250×400, C30, 4D/25) |
| En büyük As eksiği | **%0,25** (aynı yüz: 664,06 ↔ 665,71 mm²); 14 yüzün medyan D/C'si 1,00007 |
| εt ≥ 0,004 kararının değiştiği yüz (0,7h ↔ tutarlı lcs) | **0** |
| Muhafazakâr yön (D/C < 1) | 468 yüz; bunların çoğunda AsMin / alt ≥ 0,5 üst kuralı belirleyici (ayrıştırılmadı) |
| Her klasörün en ucuz lif > 0 koşusu | en büyük D/C ≤ 1,00001 (Faz 3 30k fibre_ABC YAKO run06: 1,000006) |
| Lifsiz yol | Etkilenmez (`fib Is Nothing` → `AciBeam.RequiredAs`; Ö2'de bit-bit regresyonla da gösterildi) |

Ö2'den farkı: Ö2'de (F0 B_S1) As eksiği %1,05 (D/C 1,0103) idi; Ö1'in lifli yüzlerinde hata daha küçük (≤ %0,25); nedeni bu denetimde ayrıştırılmadı (Ö1'de AsMin ve alt ≥ 0,5 üst kuralı birçok yüzde belirleyici olabilir).

## 4. Ö1 sonuç iddialarına etki (değerlendirme; Critic + IA kararı)

- **"Tüm-boyut lifli 180 koşuda lif 173/180 = 0"**, "lif pahalı/ekonomik iddiası yasak", fiyat = 0 ve başabaş tabloları: lifsiz yol etkilenmediği ve lifli yüzlerdeki eksik ≤ %0,25 As (tek yüzde birkaç mm²; maliyet etkisi grup başına TL mertebesi) olduğu için **yön ve büyüklük değişmez**. Hata lif lehine (güvensiz) ve aleyhine (muhafazakâr) iki yönlü ve seyrek.
- **Güvenlik:** 14 lifli yüzde D/C 1,000001–1,0024 (≤ %0,25 As eksiği). Raporlanan Ö1 en iyi lifli tasarımları (klasör en ucuzları) D/C ≤ 1,00001. Ö1 V3'te (ETABS + bağımsız lifli Mn kontrolü) lifli kirişler raporlanacaksa bu yüzler tutarlı kapasiteyle yeniden türetilmeli.
- **Denetlenmeyen:** Ö1 `BeamShear.MrOf` (Mpr, kapasite kesmesi) ve `StrainOk` 0,7h kullanır; Mpr'nin lcs'ye duyarlılığı (kesme talebi) bu denetimde ölçülmedi (εt kararı değişmedi).
- **Öneri (IA kararı):** Ö1 donmuş kalır; makalede Ö1 lifli tasarımları için "lifli eğilme kapasitesi yol bağımlı bir ikiye bölmeyle hesaplandı; tutarlı kapasiteyle yeniden değerlendirmede 482 lifli kiriş yüzünün 14'ünde D/C 1,000–1,002 (As eksiği ≤ %0,25) bulundu" sınırlama cümlesi; Ö1 kodunun düzeltilmesi ayrı PCR (ör. PCR-26) ve yeniden koşu kararı IA'da.
