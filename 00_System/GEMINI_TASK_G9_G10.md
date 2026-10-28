# GEMINI_TASK_G9_G10.md — TBDY çıkarım görevleri (2026-10-28)

Kullanıcı tek komut verir: **"G9 G10"**. Önce `GEMINI.md` "commit" adımlarını uygula (bekleyen Claude dosyaları: A11_CONSTRAINTS.md, THESIS_EXAMPLE_DATA.md, HANDOFF.md), sonra aşağıdaki iki çıkarımı yap. Kurallar: `.agents/rules/anti_hallucination_evidence.md`. Kaynak: `Evidence/DesignGuides/TDBY2018.pdf` (SHA-256 8d3a9a463d4a534ec2c6834c557b5f706e2ad976c2d6837f6c9b6242e38a6bb2; basılı sayfa = PDF − 21). `Evidence/` ve `Sources/` salt-okunur, git'e girmez. Kod dosyalarını değiştirme.

## G9 — Kapasite tasarımı, sarılma ve düğüm kesmesi (Claude'un C1 kalemi)
Maddeler: 7.3.4 (kolon kesme, kapasite tasarımı Ve), 7.3.7 (kolon sarılma bölgesi etriyesi), 7.4.4 ve 7.4.5 (kiriş kesme Ve ve sarılma etriyesi), 7.5 (kolon-kiriş birleşim bölgesi kesme). Her denklem ve sayısal sınır için:
- madde no, denklem no, PDF ve basılı sayfa,
- birebir alıntı (≤25 kelime),
- değişken tanımları ve birimleri (fck, fyk, Ash, s, bk, Ve, Mra/Mrü, ln, γ katsayıları),
- bağımlılığı (DTS, BYS, sarılma bölgesi uzunluğu).
Çıktı: `01_Literature/TBDY_EXTRACT_G9.md`. Durum etiketi: DOĞRULANDI / KISMEN / BULUNAMADI. Tahmin yok.

## G10 — Ötelenme sınırı ve istisnalar (Claude'un C2 kalemi ve A5 istisnası)
- 4.9.1: göreli kat ötelemesi sınırı (Denk. 4.34a,b), κ ve λ tanımı, λ'nın DD-3 ile ilişkisi: λ = DD-3'teki ... hangi spektral ivme oranı; gerekli girdiler (SDS, SD1 DD-3 değerleri), Δ'nin hangi birleşimle hesaplandığı.
- 7.3.5.3: güçlü kolon–zayıf kiriş koşulundan muafiyetlerin tam metni (üst kat, tek kat, Nd ≤ 0,10 Ac fck vb.).
- Tablo 3.1–3.3 (zemin sınıfı ve Fs/F1) ve Denk. 2.2 değerlerini, `TBDY_EXTRACT_GEMINI.md`'deki kalemlerle çapraz karşılaştır; farklı olanı yaz.
Çıktı: `01_Literature/TBDY_EXTRACT_G10.md`, aynı biçim.

## Bitiş
`00_System/HANDOFF.md`'ye satır ekle ("Gemini, G9+G10 çıkarıldı, dosyalar"). Claude sayfa görüntüsünden doğrular; **DOĞRULANDI etiketini yalnız Claude verir.** Gemini'nin etiketi "ÇIKARILDI" olur. Kullanıcıya tek satır rapor.
