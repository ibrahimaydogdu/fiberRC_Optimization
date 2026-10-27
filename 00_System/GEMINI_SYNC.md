# 🤝 GEMINI_SYNC.md (Gemini/Antigravity ↔ Claude Code eşzamanlama)

## Mimari
Tek gerçek kaynak **bu klasördeki dosyalardır**; iki yapay zekâ aynı klasörü açar.
- `AGENTS.md` → Antigravity ve Gemini CLI okur (kurallar). `CLAUDE.md` ve `GEMINI.md` yalnız yönlendirir.
- `.agents/rules/` → Antigravity iş-alanı kuralları. `.agents/mcp_config.json` → proje araç listesi (Antigravity MCP ayarlarına elle eklenir).
- `MEMORY.md`, `PROJECT_STATE.md`, `TASKS.md`, `HANDOFF.md` → ortak hafıza.

## Kullanıcının yapması gerekenler
`TASKS.md` bölüm C (G1–G6).

## Git rolü
Tüm commit/push/pull Gemini'dedir (kullanıcı kararı). Claude dosya üretir, HANDOFF'a yazar; Gemini oturum başında `git pull`, bitirirken `git add/commit/push` yapar. Uzak: `https://github.com/ibrahimaydogdu/fiberRC_Optimization.git`.

## Çakışma kuralları
1. Aynı anda tek yapay zekâ yazar. Oturum bitince HANDOFF yazılır.
2. Google Drive eşzamanlaması gecikebilir: oturum açarken Drive'ın "senkronize" durumunu bekle. `desktop.ini` ve `*.tmp` git dışıdır.
3. Çelişen kuralda `AGENTS.md` kazanır; Gemini'ye özgü ek kural yalnız `GEMINI.md`'ye yazılır.
4. Büyük dosyalar (EDB, PDF) aynı anda iki araçta açılmaz.

## Doğrudan çağrı testi (2026-10-27)
- `gemini` CLI 0.65.0 kurulu (`%APPDATA%\npm`). `gemini -p "..."` çalıştırıldı → **IneligibleTierError (UNSUPPORTED_CLIENT)**: Google, bireysel hesapta CLI'yi desteklemiyor ve Antigravity'ye yönlendiriyor. Sonuç: Claude'dan Gemini'ye doğrudan çağrı **çalışmıyor**.
- `antigravity-ide.cmd` yalnız editör açar (dosya/klasör); ajan komutu yok. `agentapi.bat` dahili arayüz, kullanılmadı.
- **Karar:** Dosya tabanlı devir (HANDOFF/TASKS). Alternatif: CLI'yi API anahtarıyla kullanmak (Google AI Studio anahtarı, `GEMINI_API_KEY`) — kullanıcı onayı ve anahtar gerekir; doğrulanmadı.

## API anahtarıyla CLI testi (2026-10-27)
- Anahtar `GEMINI_API_KEY` ortam değişkeni olarak yalnız komut süresince verildi, hiçbir dosyaya yazılmadı. HTTP `models` çağrısı 200 → anahtar geçerli.
- CLI başlatma gereksinimleri: `--skip-trust` (veya klasörü Gemini CLI'de güvenilir yap) ve bu makinede `NODE_OPTIONS=--dns-result-order=ipv4first` (aksi halde `fetch failed`).
- İlk deneme sonucu: API'ye ulaştı, **503 "model yoğun"** döndü (geçici). Başarılı yanıt henüz alınmadı; yeniden denenecek.
- Test proje klasöründe değil boş scratch klasöründe yapıldı (proje dosyaları gönderilmedi). Ücretsiz katman veri kullanım uyarısı geçerli.
- **Başarılı test (yeni anahtar, kalıcı `GEMINI_API_KEY` kullanıcı değişkeni):** `gemini --skip-trust -p "..."` → "merhaba". 503 ve `fetch failed` nedeniyle 7 deneme gerekti (geçici yoğunluk). Çağrı biçimi (PowerShell): `$env:NODE_OPTIONS='--dns-result-order=ipv4first'; gemini --skip-trust -p "<görev>"`. Gemini'ye dosya yazdırmak için ayrı onay ve güvenilir klasör gerekir; henüz denenmedi.
