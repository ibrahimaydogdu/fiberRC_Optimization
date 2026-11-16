<#
 Deterministic fibre break-even table (fibre price = 0 TL/kg), no optimisation.
 For the N lowest-cost feasible (T=0) plain designs of each method, the section vector is held fixed
 and only beam-group fibre type x dose x fR scale are changed (all 4 beam groups identical), one EvaluateFull each.
 Break-even price = -(delta non-fibre cost)/fibreKg when delta<0, else "<=0".
 Usage: powershell -NoProfile -File fibre_breakeven.ps1 [-Top 30] [-Methods BTO,ATO] [-Limit 0] [-OutDir <dir>] [-Tag ""]
 Does not modify phase3_bin (DLL copied to TEMP).
#>
param(
 [int]$Top=30,
 [string[]]$Methods=@('BTO','ATO'),
 [int]$Limit=0,
 [double[]]$FrScales=@(0.8,1.0,1.2),
 [string]$OutDir='',
 [string]$Tag=''
)
$ErrorActionPreference='Stop'
$base=(Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$bin="$base\03_Code_Execution\phase3_bin"
if($OutDir -eq ''){ $OutDir="$base\04_Results\fibre_breakeven" }
New-Item -ItemType Directory -Force $OutDir | Out-Null
$dllSha=(Get-FileHash "$bin\FemRcModel.dll" -Algorithm SHA256).Hash.ToLower()
$tmp=Join-Path $env:TEMP 'fibre_breakeven_bin'
if(Test-Path $tmp){Remove-Item $tmp -Recurse -Force}
Copy-Item $bin $tmp -Recurse
foreach($d in 'MathNet.Numerics','RcCore','FemCore','FemRcModel'){ [void][Reflection.Assembly]::LoadFrom("$tmp\$d.dll") }
$ci=[Globalization.CultureInfo]::InvariantCulture
$spec=New-Object FemRcModel.FrameSpec
$tp=New-Object FemRcModel.TbdyParams
$dd3=New-Object FemRcModel.TbdyParams; $dd3.SDS=0.294; $dd3.SD1=0.091
$types=@('4D','5D')
function Mk([double]$fr){
 $pr=New-Object RcCore.PriceSet; $pr.Fibre=0.0
 New-Object FemRcModel.Ex1Problem($spec,$tp,$dd3,$pr,$true,$fr,$true)
}
$prob=@{}; foreach($f in $FrScales){ $prob[[string]$f]=Mk $f }
$p0=Mk 1.0
$rows=New-Object System.Collections.Generic.List[object]
$srcInfo=@(); $notes=@()
foreach($m in $Methods){
 $root="$base\04_Results\phase3\$($m.ToLower())_alldim\plain_A"
 $c=@()
 foreach($f in (Get-ChildItem $root -Filter *.csv)){
  $L=Get-Content $f.FullName; $h=$L[2].Split(',')
  $T=[double]::Parse($h[5],$ci); $cost=[double]::Parse($h[3],$ci)
  if($T -eq 0){ $x=@($L[3].Split(',')|Select-Object -Skip 1|ForEach-Object{[int]$_}); $c+=[pscustomobject]@{f=$f.Name;c=$cost;x=$x} }
 }
 $total=@(Get-ChildItem $root -Filter *.csv).Count
 $sel=@($c|Sort-Object c|Select-Object -First $Top)
 if($Limit -gt 0){ $sel=@($sel|Select-Object -First $Limit) }
 $notes+="$m plain_A: $total CSV, T=0 count $($c.Count), selected $($sel.Count)"
 $srcInfo+="$m : "+(($sel|ForEach-Object{$_.f}) -join ', ')
 foreach($cd in $sel){
  $r0=$p0.EvaluateFull([int[]]$cd.x)
  $plainCost=$r0.Cost.Total; $plainSteel=$r0.SteelKg
  for($ti=0;$ti -lt 2;$ti++){ foreach($di in 1..5){ foreach($fr in $FrScales){
   $x=[int[]]$cd.x.Clone()
   foreach($g in 0..3){ $k=24+5*$g; $x[$k+3]=$ti; $x[$k+4]=$di }
   $r=$prob[[string]$fr].EvaluateFull($x)
   $nf=$r.Cost.Total-$r.Cost.Fibre
   $delta=$nf-$plainCost
   $be=$null
   if($r.FibreKg -gt 0 -and $delta -lt 0){ $be=(-$delta/$r.FibreKg) }
   $beTxt='<=0'; if($null -ne $be){ $beTxt=$be.ToString('F4',$ci) }
   $rows.Add([pscustomobject]@{
    method=$m; source=$cd.f; plainCsvCost=$cd.c; plainReevalCost=$plainCost; plainSteelKg=$plainSteel; plainT=$r0.T
    fibreType=$types[$ti]; doseIdx=$di; frScale=$fr
    T=$r.T; fibreKg=$r.FibreKg; totalCost=$r.Cost.Total; fibreCost=$r.Cost.Fibre; nonFibreCost=$nf
    deltaNonFibre=$delta; deltaTotal=($r.Cost.Total-$plainCost); steelKg=$r.SteelKg; deltaSteelKg=($r.SteelKg-$plainSteel)
    breakEven=$beTxt; beNum=$be
   })
  }}}
  Write-Host "$m $($cd.f) done"
 }
}
$csv="$OutDir\fibre_breakeven$Tag.csv"
$rows|Select-Object method,source,plainCsvCost,plainReevalCost,plainSteelKg,plainT,fibreType,doseIdx,frScale,T,fibreKg,totalCost,fibreCost,nonFibreCost,deltaNonFibre,deltaTotal,steelKg,deltaSteelKg,breakEven|
 Export-Csv $csv -NoTypeInformation -Encoding UTF8
function Stat($v){ $v=@($v|Sort-Object); if($v.Count -eq 0){return 'n=0'}; $n=$v.Count
 if($n%2){$med=$v[($n-1)/2]}else{$med=($v[$n/2-1]+$v[$n/2])/2}
 'min {0:N0} / medyan {1:N0} / maks {2:N0}' -f $v[0],$med,$v[$n-1] }
$sb=New-Object Text.StringBuilder
[void]$sb.AppendLine('# Lif fiyatı = 0 deterministik kırılma tablosu')
[void]$sb.AppendLine('')
[void]$sb.AppendLine("- DLL: ``phase3_bin/FemRcModel.dll`` SHA-256 = ``$dllSha``")
[void]$sb.AppendLine("- Tarih: $(Get-Date -Format 'yyyy-MM-dd HH:mm') ; betik: ``03_Code_Execution/tools/fibre_breakeven.ps1`` (Top=$Top, fR olcekleri $($FrScales -join '/'))")
[void]$sb.AppendLine('- Yöntem: kesit vektörü sabit; 4 kiriş grubunun lif tipi (4D/5D) ve doz indeksi (1..5 = 20, 25, 30, 35, 40 kg/m3; sınıfa göre ızgaraya onarılır) değişir; lif fiyatı 0 TL/kg; `--derive 1` modeli (Ex1Problem useFibre=true, deriveRho=true); optimizasyon yok.')
[void]$sb.AppendLine('- Δ = (toplam - lif ücreti) - lifsiz yeniden-değerlendirme maliyeti (TL). Kırılma fiyatı = -Δ/fibreKg (Δ<0), aksi halde `<=0`.')
foreach($n in $notes){ [void]$sb.AppendLine("- $n") }
[void]$sb.AppendLine('- Kaynak dosyalar (en düşük maliyetli T=0 lifsiz A tasarımları, `04_Results/phase3/<yöntem>_alldim/plain_A/`):')
foreach($s in $srcInfo){ [void]$sb.AppendLine("  - $s") }
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Özet (lif ücreti 0 TL/kg)')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| Yöntem | Konfig. sayısı | T=0 konfig. | Δ<0 konfig. | Δ lif-dışı (TL) min/med/maks | Δ toplam (TL) min/med/maks | Δ çelik (kg) min/med/maks | Kırılma fiyatı (Δ<0 olanlar) |')
[void]$sb.AppendLine('|---|---|---|---|---|---|---|---|')
foreach($m in $Methods){
 $R=@($rows|Where-Object{$_.method -eq $m}); if($R.Count -eq 0){continue}
 $neg=@($R|Where-Object{$null -ne $_.beNum})
 if($neg.Count){ $bs=(Stat ($neg|ForEach-Object{$_.beNum*1000}))+' (TL/ton)' } else { $bs='yok (hepsi <=0)' }
 [void]$sb.AppendLine("| $m | $($R.Count) | $(@($R|Where-Object{$_.T -eq 0}).Count) | $($neg.Count) | $(Stat ($R|ForEach-Object{$_.deltaNonFibre})) | $(Stat ($R|ForEach-Object{$_.deltaTotal})) | $(Stat ($R|ForEach-Object{$_.deltaSteelKg})) | $bs |")
}
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Tasarım başına en iyi konfigürasyon (Δ lif-dışı en küçük, T=0 tercihli)')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| Yöntem | Kaynak | Lifsiz maliyet (TL) | Lifsiz çelik (kg) | Tip / doz idx / fR | T | fibreKg | Δ lif-dışı (TL) | Δ toplam (TL) | Δ çelik (kg) | Kırılma fiyatı (TL/kg) |')
[void]$sb.AppendLine('|---|---|---|---|---|---|---|---|---|---|---|')
foreach($m in $Methods){ foreach($s in ($rows|Where-Object{$_.method -eq $m}|Select-Object -ExpandProperty source -Unique)){
 $R=@($rows|Where-Object{$_.method -eq $m -and $_.source -eq $s}); $F=@($R|Where-Object{$_.T -eq 0}); if($F.Count -eq 0){$F=$R}
 $b=$F|Sort-Object deltaNonFibre|Select-Object -First 1
 [void]$sb.AppendLine(("| {0} | {1} | {2:N0} | {3:N0} | {4} / {5} / {6} | {7:F3} | {8:N0} | {9:N0} | {10:N0} | {11:N0} | {12} |" -f $m,$s,$b.plainReevalCost,$b.plainSteelKg,$b.fibreType,$b.doseIdx,$b.frScale,$b.T,$b.fibreKg,$b.deltaNonFibre,$b.deltaTotal,$b.deltaSteelKg,$b.breakEven))
}}
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Ek sayımlar')
[void]$sb.AppendLine('')
foreach($m in $Methods){ $R=@($rows|Where-Object{$_.method -eq $m}); if($R.Count){
 [void]$sb.AppendLine("- $m : Δ toplam > 0 olan konfig. $(@($R|Where-Object{$_.deltaTotal -gt 0}).Count) / $($R.Count); en küçük Δ toplam $('{0:N0}' -f (($R|Measure-Object deltaTotal -Minimum).Minimum)) TL; en küçük Δ lif-dışı $('{0:N0}' -f (($R|Measure-Object deltaNonFibre -Minimum).Minimum)) TL; T>0 konfig. $(@($R|Where-Object{$_.T -gt 0}).Count)" )}}
[void]$sb.AppendLine('')
[void]$sb.AppendLine('Sınırlamalar: kesit vektörü her koşunun Fpen-en-iyi x değeri (T=0 olanlar); kesitler lifli koşullara yeniden optimize edilmez (b0 ayrı); tek fR kaynağı, fR ölçeği tablo değerlerine çarpan; lif katkısı sarılma bölgesinde 0, Mpr/VRd,F varsayımları modele aittir.')
[IO.File]::WriteAllText("$OutDir\fibre_breakeven$Tag.md",$sb.ToString(),(New-Object Text.UTF8Encoding($true)))
Write-Host "rows $($rows.Count) -> $csv"
