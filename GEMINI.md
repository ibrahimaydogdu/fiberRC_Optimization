# Gemini / Antigravity giriş dosyası
Bu proje çok ajanlı bir makale platformudur. Önce şu dosyaları sırayla oku ve kurallarına uy:
1. `AGENTS.md` (tüm kurallar; Claude Code ile ortaktır)
2. `MEMORY.md` (kalıcı kararlar)
3. `00_System/HANDOFF.md` (son oturumda kim neyi bıraktı)
4. `00_System/TASKS.md` (açık görevler/kararlar)

**Git sorumlusu sensin:** `fiberRC_Optimization` deposuna commit/push/pull yalnız sen yaparsın (`Sources/` ve büyük dosyalar `.gitignore`'da). Her push öncesi `HANDOFF.md`'yi güncelle, commit mesajı Türkçe ve kısa olsun.
## Kısa komut: "commit"
Kullanıcı yalnızca **commit** yazarsa şunu uygula (dal: **master**, `main` yok):
1. `00_System/HANDOFF.md` son satırını oku; Claude'un ne bıraktığını anla.
2. `git status`. `Sources/`, `Evidence/`, `bin/`, `obj/` eklenmeyecek (`.gitignore`'a yoksa ekle). Beklenmedik büyük dosya (>20 MB) veya bu klasörlerden dosya görürsen DUR ve kullanıcıya sor.
3. Değişen dosyaları `git add`, commit mesajı Türkçe, kısa; HANDOFF son satırındaki işi özetle.
4. `git pull --rebase origin master`, sonra `git push origin master`. Çakışma olursa DUR, kullanıcıya bildir, kendin çözme.
5. `HANDOFF.md`'ye bir satır ekle (tarih, Gemini, işlem, commit hash) ve ayrı küçük commit ile push et.
6. Kod/içerik dosyalarını değiştirme; yalnız git ve HANDOFF satırı.
Sonunda kullanıcıya tek satır rapor ver: commit hash, dosya sayısı, push durumu.

İVAO = İyileştirilmiş Vahşi At Optimizasyonu (Vektör Arama değil).
`Sources/` salt-okunurdur. Dil: Türkçe. Kullanıcı onayı olmadan sonraki aşamaya geçme.
