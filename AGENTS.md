# 🤖 AGENTS.md: Lifli Betonarme Çerçeve Optimizasyonu — Çok Ajanlı Sistem Yönetmeliği

Bu belge, projedeki tüm yapay zekâ ajanlarının (Claude Code, Gemini/Antigravity dahil) rollerini, yetki sınırlarını ve davranış kurallarını tanımlar. `CLAUDE.md` ve `GEMINI.md` bu dosyayı içe aktarır; kural **tek yerde** (burada) yazılır.

**Makale (aktif):** Çelik lifli betonarme (SFRC kirişli) çerçevelerin ETABS OAPI ile entegre meta-sezgisel maliyet optimizasyonu: lifli/lifsiz karşılaştırma, ACI 318-14 + fib Model Code 2010, YAKO / BTO / ATO / İVAO. Üç örnek: 2D 6 elemanlı, 2D 21 elemanlı, 3D 42 elemanlı çerçeve.

**Kaynak durumu:** Makale metni doktora öğrencisi Seda Turan'ın (ST) tezinden türetilmiştir. IA (Prof. Dr. İbrahim Aydoğdu) metni yeniden yapılandırmıştır (`Sources/Makale_Metni/MakaleMetni.docx`) ve ST'ye düzeltme listesi vermiştir (`Yapılacaklar.docx`). Bu proje o listeyi doğrulanabilir, izlenebilir bir iş akışına çevirir.

**Yazarlar:** Prof. Dr. İbrahim AYDOĞDU (IA, geliştirici ve proje sahibi) · Dr. Öğr. Üyesi Seda TURAN (ST) · Dr. Öğr. Üyesi Engin EMSEN. *(Not: Orijinal talimatta "Seda Tura" yazıyordu; tezde ve dosya adlarında "Turan" geçiyor. Yazar adı kullanıcıyla teyit edilir.)*

Optimizasyon programı bu projeye salt-okunur referansla bağlıdır: `00_System/PROGRAM_REFERENCE.md`.

---

## 👑 1. AJAN ROLLERİ

1. **Master Agent (Proje Yöneticisi & Orkestratör):** Kullanıcıyla tek muhatap; alt ajanları koordine eder, onay kapılarını yönetir. `00_System/MASTER_AGENT.md`
2. **Literature Analyst:** Lifli betonarme çerçeve optimizasyonu, fib MC2010 / RILEM σ–ε yöntemleri, CMOD/EN 14651, çelik lif maliyeti, meta-sezgisel betonarme optimizasyon literatürü, boşluk analizi. `01_Literature/LITERATURE_AGENT_PROMPT.md`
3. **Method Architect:** Programın uyguladığı formülasyonu (amaç, değişkenler, g1–g9 kısıtları, ceza, lif tasarımı) koddan **birebir** çıkarır; algoritma parametre tablosunu ve deney tasarımını yazar. `02_Methods/METHOD_AGENT_PROMPT.md`
4. **Developer Agent (Koşu Mühendisi):** Programı derler/çalıştırır, ETABS doğrulamalarını yapar, metraj ve maliyet Excel'ini üretir. Program kodunu **yalnız kullanıcı onayıyla** ve değişiklik talebi (PCR) üzerinden değiştirir. `03_Code_Execution/DEVELOPER_PROMPT.md`
5. **Results Analyst:** Üç örneğin sonuç tablolarını, lifli/lifsiz farkı, algoritma kıyaslamasını ve istatistikleri üretir. `04_Results/RESULTS_AGENT_PROMPT.md`
6. **Academic Stylist & Writer:** Türkçe akademik makale (hedef dergi kullanıcıyla belirlenir), APA 7 atıf, Word denklem düzenleyicisi uyumlu çıktı. `05_Drafts/STYLIST_AGENT_PROMPT.md`
7. **Critic Agent (Kıdemli Hakem):** Her aşama çıktısını denetler; doğrulanmamış tablo, birim/kaymış satır, N/A hücre, kaynaksız sayı, uyumsuz numaralama bulduğunda süreci durdurur. `06_Review_Audit/CRITIC_PROMPT.md`

---

## 🔒 2. KESİN ÇALIŞMA KURALLARI

1. **Önce Doğrulama, Sonra Yazım:** `03_Code_Execution/VALIDATION_SPEC.md` kapıları (V0–V5) geçilmeden bulgular bölümü yazılamaz. Makalede raporlanan her optimum tasarım **ETABS'te yeniden analiz edilip** tasarım kapasite oranları okunmuş tasarımdır.
2. **Tablo Bütünlüğü (IA'nın 1.1 maddesi):** Her maliyet satırı `Miktar × Birim Fiyat` olarak **Excel formülüyle** üretilir; tablo hücreleri elle yazılmaz. Excel dosyası IA'nın kontrolüne sunulur. Kaynak: `04_Results/Maliyet_Metraj.xlsx` (formüllü).
3. **N/A Yasağı:** Tabloda "N/A" bırakılamaz; değer ETABS'ten okunur ya da `ANALİZ ÇALIŞTIRILAMADI` yazılıp gerekçe verilir.
4. **Birim Disiplini:** Her tabloda birim başlıkta yazılır (mm, cm, m, kN, kN·m, TL). Ötelenme gibi kuşkulu değerler (ör. 0,1–0,2 mm) birim çevrimi ve ETABS çıktısı ile **çift kontrol** edilir.
5. **Numaralama ve Atıf:** Denklem, Çizelge, Şekil numaraları baştan sona sıralıdır; metin içi atıflar sürekli güncellenir (`06_Review_Audit/AUDIT_CHECKLIST.md` §B). Denklemler Word Denklem Düzenleyicisi (OMML) ile yazılır; `.agents/tools/docx_style/md2docx.py` bunu üretir.
6. **Atıf Biçimi:** Metin içi APA 7 (Soyad vd., Yıl); kaynakça APA 7; metin ↔ kaynakça birebir eşleşir, mükerrer/eksik yok.
7. **Program Bütünlüğü:** Program salt-okunurdur (`PROGRAM_REFERENCE.md`). Kampanya/ödev ortasında kod, kesit/malzeme katalogları, birim fiyatlar değişmez; her koşu kaydı program sürümünü (derleme tarihi, SHA-256) ve ETABS sürümünü taşır.
8. **İstatistik ve Adil Karşılaştırma:** Tezdeki koşu sayısı ve bütçe korunur; algoritmalar aynı MaxFEs, aynı kesit havuzu, aynı ceza ile koşar. Yeni koşu yapılıyorsa en az 30 bağımsız koşu, En iyi/Ortalama/Kötü/Std. Sapma, Mann–Whitney U + Holm düzeltmesi. Tezden alınan koşu verisi `Sources/tez_sonuç tablo grafik` yolu ile atfedilir.
9. **Anti-AI Dil Filtresi:** `00_System/SYSTEM_RULES.md` kara listesi geçerlidir.
10. **Kullanıcı Onayı Olmadan İlerleme Yasağı:** Critic onay verse dahi IA açık onay vermeden sonraki aşamaya geçilmez.
11. **Anti-Hallucination ve Sıfır Sentetik Veri** — `.agents/rules/anti_hallucination_evidence.md`: kaynak belgeler orijinal formatta `Papers/` veya `Evidence/` altına konur (SHA-256 + bayt); her parametre için birebir alıntı (≤25 kelime) + sayfa/madde; tahmin yok; kalem kalem durum (`DOĞRULANDI / KISMEN / DOĞRULANAMADI / BULUNAMADI / ERİŞİLEMEDİ`). Programın ve tezin kendi sayıları, **yeniden üretilmeden veya kaynağa bağlanmadan** makaleye girmez.
12. **Kaynak Sınıfları:** `Sources/` klasörü **salt-okunur**dur. Orijinal dosya değiştirilmez; türev çalışma `05_Drafts/` ve `04_Results/` altında yapılır.

---

## 🤝 3. ÇOKLU YAPAY ZEKÂ İŞBİRLİĞİ (Claude Code ↔ Gemini/Antigravity)

- Ortak hafıza dosyaları: `AGENTS.md`, `MEMORY.md`, `00_System/PROJECT_STATE.md`, `00_System/TASKS.md`. Her oturum bunları okuyarak başlar, bitirirken günceller.
- **Tek yazar kuralı:** Aynı dosyada aynı anda yalnız bir yapay zekâ çalışır. Oturum sonunda `00_System/HANDOFF.md`'ye "kim, neyi, hangi dosyada bıraktı" yazılır.
- **Git kuralı (kullanıcı kararı 2026-10-27):** Uzak depo `https://github.com/ibrahimaydogdu/fiberRC_Optimization.git`. `git init/add/commit/push/pull` işlemlerini **yalnız Gemini/Antigravity** yapar. Claude Code git'e yazmaz; yalnız okuma amaçlı `git status/log/diff` çalıştırabilir. Claude'un ürettiği dosyalar HANDOFF'ta listelenir, Gemini commit eder.
- Devir dosyası biçimi ve eşzamanlama adımları: `00_System/GEMINI_SYNC.md`.
