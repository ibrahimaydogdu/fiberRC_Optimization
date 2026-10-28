# WP1 — ACI 318-14 kesit kapasite modülü (şartname taslağı)

*Durum (2026-10-28): AciBeam + T1 testi tamam (8/8), diğer modüller sırada; kod `program_work/RcCore`. Aşağıdaki eski not tarihseldir:* yazılım BAŞLAMADI. Neden: ACI 318-14 ve fib MC2010 metinleri `Evidence/Standards/` altında yok; `anti_hallucination_evidence.md` kuralı gereği formül ve katsayılar orijinal belgeden (madde/sayfa + birebir alıntı) alınmadan kodlanamaz.*

## Kapsam (ince host; `program_work/RcHost/`, dil VB.NET .NET 4.7.2 — FEM tabanıyla uyumlu)
| Modül | İçerik | Girdi standart |
|---|---|---|
| `AciBeam` | eğilme φMn (tek/çift donatılı), kesme Vc+Vs, ρmin/ρmax, etriye aralığı | ACI 318-14 Böl. 9, 22 |
| `AciColumn` | P-M etkileşim diyagramı, φ(εt), kesme, boyuna donatı oranı sınırları | ACI 318-14 Böl. 10, 22 |
| `SfrcBeam` | fib MC2010 σ–w: fFts, fFtu, eğilme MRd, kesme VRd,F (γc dahil), çatlak genişliği; fR1/fR3 girdisi | fib MC2010 §5.6 |
| `Cost` | ÇŞİDB 2026 birim fiyatlar (beton, donatı malzeme+işçilik, kalıp, lif) | `Evidence/BirimFiyatlar` |
| `Penalty` | Σ max(0, g_j) normalize ihlal toplamı; kısıt ve grup bazlı | tasarım kararı S-7 |
| `RcHost` | FemCore'u çağırır, kesit/donatı iç döngüsü (en ucuz yeterli düzen), ETABS final doğrulama arayüzü | – |

## Birim testler (V1)
Elle hesaplı örnekler, her biri kaynak (kitap/standart örneği, sayfa) ile; v2 hata sınıfları için regresyon testi: F-01 (PMM oranı 0 olamaz), F-02 (donatı toplamı grup içinde sıfırlanır), F-06/F-07 (γc, φ), F-11 (çatlamış atalet).

## Gerekli girdiler (kullanıcıdan)
1. ACI 318-14 PDF (veya ACI 318-19 + fark notu; hangisi esas?) → `Evidence/Standards/`.
2. fib Model Code 2010 PDF (§5.6) → aynı klasör.
3. TBDY-2018 spektrum maddeleri (Bölüm 2) → aynı klasör.
4. Çatlamış rijitlik çarpanları ACI 318-14 Tablo 6.6.3.1.1(a) ve kütüphane testi için bir ders kitabı örneği (ör. kaynağı sizin belirlemeniz).
