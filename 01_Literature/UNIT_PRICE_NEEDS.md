# 💰 UNIT_PRICE_NEEDS.md (birim fiyat için gereken kaynaklar)

Mevcut kanıt: `Steel_Composite_Hybrid_Frame_Optimization/Evidence/UnitPrices` — Türkiye ÇŞİDB birim fiyatları 2019–2026 (PDF), ÇŞİDB inşaat analizleri 2026, Bangladeş PWD SoR 2022, Maharashtra PWD SSR 2022-23, SDDOT 2024, BLS PPI. Bu set **çelik profil ve hazır beton** içindir; betonarme çerçeve için yetmez. Eksikler:

| # | Gerekli kalem | Neden | Önerilen birincil kaynak | Durum |
|---|---|---|---|---|
| P1 | Hazır beton (C30/37, C40/50) | Beton maliyeti | ÇŞİDB 2024 poz 10.130.1506/1508 (pompa hariç) | **DOĞRULANDI** (s.21) |
| P2 | Nervürlü donatı malzeme + işçilik | Donatı maliyeti | 10.130.1704/1705 (18,70 TL/kg) + 15.160.1003/1004 | **DOĞRULANDI** (s.22, s.177) |
| P3 | Kalıp (m²) | Kalıp kalemi | 15.180.1002 ahşap 524,41 TL/m² (s.177) | **DOĞRULANDI**; kodun kalıbı hesaba katıp katmadığı kontrol edilecek |
| **P4** | **Çelik lif birim fiyatı** | Ana iddia lif maliyetine bağlı | **ÇŞİDB 2024, poz 10.300.4021–4024 / 4011–4014 (kancalı çelik tel, 43–67 TL/kg)** | **DOĞRULANDI** — `UNIT_PRICE_EVIDENCE.md` §2; yalnız tezdeki lif türünün (boy/çap, tutkallı mı) teyidi kaldı |
| P5 | Lifli betonun hazır beton fiyat farkı | Karışım/işçilik farkı | ÇŞİDB'de ayrı poz yok (BULUNAMADI); hazır beton firması teklifi veya varsayımın yazılı gerekçesi | Açık (isteğe bağlı) |
| P6 | Lif tipi ve dozajı kimliği | fib MC2010 parametreleri için | Tezdeki CMOD/EN 14651 deney raporu (3 nokta eğilme, fR1–fR4); lif markası, boy/çap, çekme dayanımı, dozaj; test laboratuvarı raporu PDF'i | **ST'den istenecek; `Sources`'ta bulunamadı** |
| P7 | Fiyat yılı ve kuru | Tez 2024; ÇŞİDB 2026 | Yıl: tüm kalemler aynı yıldan; TRY/USD çevrimi gerekirse TCMB resmî kuru | Karar: D6 |
| P8 | Zaman duyarlılığı | Lif/beton oranı değişince sonuç değişir | BLS PPI veya TÜİK ÜFE (çelik, beton) | BLS mevcut; TÜİK **ERİŞİLEMEDİ** |

## Kaynak belgeler (yönetmelik/şartname; ücretli olabilir)
ACI 318-14 (PDF); fib Model Code 2010 (Bölüm 5.6 lifli beton); EN 14651; RILEM TC 162-TDF; TS 500, TBDY 2018 (referans örneklere göre); ETABS 22 OAPI yardımı. Hangileri bende yok, kullanıcı `Evidence/Standards/` altına koysun.

## Kullanıcıdan istenen (öncelik sırası)
1. (P4 çözüldü.) P5 isteğe bağlı.
2. P6: EN 14651 deney raporu.
3. Fiyat yılı ve para birimi kararı.
Kurallar: `.agents/rules/anti_hallucination_evidence.md` (orijinal PDF, SHA-256, birebir alıntı).
