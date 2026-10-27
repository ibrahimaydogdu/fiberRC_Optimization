-- Otomatik resim yazısı ve denklem tablosu (pandoc >= 3.0)
--  * Tablo: başlık TABLONUN ÜSTÜNDE, "Tablo N." + SEQ alanı
--  * Figür: başlık FİGÜRÜN ALTINDA, "Figür N." + SEQ alanı
--  * Denklem: tek başına $$...$$ paragrafı -> 2 sütunlu tablo (denklem solda, "(N)" sağda, SEQ alanı)
-- Alanlar Word'de F9 ile güncellenir; önbellek değerleri doğru sırayla yazılır.

local tab_n, fig_n, eq_n = 0, 0, 0

local function seq_field(name, n)
  return pandoc.RawInline('openxml',
    '<w:r><w:fldChar w:fldCharType="begin"/></w:r>' ..
    '<w:r><w:instrText xml:space="preserve"> SEQ ' .. name .. ' \\* ARABIC </w:instrText></w:r>' ..
    '<w:r><w:fldChar w:fldCharType="separate"/></w:r>' ..
    '<w:r><w:t>' .. tostring(n) .. '</w:t></w:r>' ..
    '<w:r><w:fldChar w:fldCharType="end"/></w:r>')
end

local function prefix_caption(blocks, word, name, n)
  local first = blocks[1]
  if first == nil or (first.t ~= 'Plain' and first.t ~= 'Para') then return false end
  local new = pandoc.List({pandoc.Str(word), pandoc.Space(), seq_field(name, n),
                           pandoc.Str('.'), pandoc.Space()})
  new:extend(first.content)
  first.content = new
  return true
end

function Table(tbl)
  if #tbl.caption.long > 0 then
    tab_n = tab_n + 1
    prefix_caption(tbl.caption.long, 'Tablo', 'Tablo', tab_n)
    return tbl
  end
end

function Figure(fig)
  if #fig.caption.long > 0 then
    fig_n = fig_n + 1
    prefix_caption(fig.caption.long, 'Figür', 'Figur', fig_n)
    return fig
  end
end

function Para(el)
  if #el.content == 1 and el.content[1].t == 'Math'
     and el.content[1].mathtype == 'DisplayMath' then
    eq_n = eq_n + 1
    local num = pandoc.Inlines({pandoc.Str('('), seq_field('Denklem', eq_n), pandoc.Str(')')})
    local st = pandoc.SimpleTable(
      {}, {pandoc.AlignLeft, pandoc.AlignRight}, {0.92, 0.08}, {},
      {{ {pandoc.Plain({el.content[1]})}, {pandoc.Plain(num)} }})
    return pandoc.utils.from_simple_table(st)
  end
end

return {{Table = Table}, {Figure = Figure}, {Para = Para}}
