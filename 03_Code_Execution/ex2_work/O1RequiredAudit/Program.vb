Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports FemRcModel
Imports Ex2Model

'Read-only audit (2026-11-17): Ö1 lifli koşu dosyalarının son vektörleri, donmuş Ö1 değerlendiricisiyle (Ex1Problem.Evaluate) yeniden değerlendirilir;
'her lifli kiriş grubunda sağlanan As (Ö1: bayat-lcs Required + AsMin + alt >= 0,5 üst) ile tutarlı (PCR-23) kapasite karşılaştırılır.
'Usage: O1RequiredAudit <list file: one run csv path per line> <out csv>
Module Program
    ReadOnly ci As CultureInfo = CultureInfo.InvariantCulture

    Function F(v As Double, Optional fmt As String = "F4") As String
        Return v.ToString(fmt, ci)
    End Function

    Function Main(args() As String) As Integer
        Dim files = File.ReadAllLines(args(0)).Where(Function(s) s.Trim() <> "").ToArray()
        Dim dll As String = GetType(Ex1Problem).Assembly.Location
        Dim sha As String
        Using fs = File.OpenRead(dll), h = System.Security.Cryptography.SHA256.Create()
            sha = BitConverter.ToString(h.ComputeHash(fs)).Replace("-", "").ToLowerInvariant()
        End Using
        Using w As New StreamWriter(args(1))
            w.WriteLine("# FemRcModel.dll sha256=" & sha & " (loaded); consistent capacity = Ex2Model PCR-23 FlexFixed (Fy 420)")
            w.WriteLine("file,runFemRcModelSha16,costFile,costReeval,costDiff,T,group,b,h,d,fc,type,dose,face,Mu_kNm,As_prov,As_req_consistent,As_deficit_pct,DC_consistent,epsT_old0.7h,epsT_consistent,epsT_flip")
            For Each path In files
                Dim L = File.ReadAllLines(path)
                Dim hdr = L(1).Split(","c), cst = L(2).Split(","c)
                Dim hd As New Dictionary(Of String, String)
                For i As Integer = 0 To hdr.Length - 2 Step 2 : hd(hdr(i)) = hdr(i + 1) : Next
                Dim cd As New Dictionary(Of String, String)
                For i As Integer = 0 To cst.Length - 2 Step 2 : cd(cst(i)) = cst(i + 1) : Next
                Dim x = L(3).Substring(2).Split(","c).Select(Function(s) Integer.Parse(s)).ToArray()
                Dim fr As Double = Double.Parse(hd("fRscale"), ci), fp As Double = Double.Parse(hd("fibreprice"), ci)
                Dim derive As Boolean = hd.ContainsKey("derivedRho") AndAlso hd("derivedRho") = "1"
                Dim prob As New Ex1Problem(New FrameSpec, New TbdyParams, New TbdyParams With {.SDS = 0.294, .SD1 = 0.091}, New RcCore.PriceSet With {.Fibre = fp}, hd("fibre") = "1", fr, derive)
                Dim full As Boolean = x.Length = 44                       'run files store the full 44-vector (Ex1Runner); compact vectors are expanded
                Dim ev = If(full, prob.EvaluateFull(x), prob.Evaluate(x))
                Dim d = prob.Decode(If(full, x, prob.ToFull(x)))
                Dim costFile As Double = Double.Parse(cd("cost"), ci)
                Dim runSha As String = L(0).Substring(L(0).IndexOf("sha256=") + 7, 16)
                For Each g In Ex1Problem.BeamGroups
                    Dim bd = d.Beams(g), r = ev.BeamRes(g)
                    Dim fib = bd.Fibre
                    If fib Is Nothing OrElse fib.Params Is Nothing Then Continue For
                    Dim typ As String = If(fib.Params.FR1 > 0, "SFRC", "-")
                    For Each face In {Tuple.Create("top", r.MuTop, r.AsTop), Tuple.Create("bot", r.MuBot, r.AsBot)}
                        Dim mu As Double = face.Item2, asP As Double = face.Item3
                        Dim dc As Double = If(mu > 0.0, mu / BeamDesignGravity.PhiMn(r.B, r.H, r.D, asP, r.Fc, fib), 0.0)
                        Dim req As Double = BeamDesignGravity.RequiredFace(mu, 0.0, r.B, r.H, r.D, r.Fc, fib)
                        Dim defi As Double = If(Double.IsInfinity(req), Double.PositiveInfinity, Math.Max(0.0, req - asP) / asP * 100.0)
                        Dim eOld As Double = RcCore.SfrcBeam.Flexure(r.B, r.H, r.D, asP, r.Fc, BeamDesign.Fy, fib.Params, 0.7 * r.H).EpsT
                        Dim eNew As Double = BeamDesignGravity.FlexFixed(r.B, r.H, r.D, asP, r.Fc, fib).EpsT
                        w.WriteLine(String.Join(",", {path, runSha, F(costFile, "F2"), F(ev.Cost.Total, "F2"), F(ev.Cost.Total - costFile, "F4"), F(ev.T, "R"), g, F(r.B, "F0"), F(r.H, "F0"), F(r.D, "F0"), F(r.Fc, "F0"),
                                     F(fib.Params.FR1, "F3") & "|" & F(fib.Params.FR3, "F3"), F(fib.DoseKgM3, "F0"), face.Item1, F(mu / 1.0E6, "F3"), F(asP, "F2"), F(req, "F2"), F(defi, "F4"), F(dc, "F6"), F(eOld, "F5"), F(eNew, "F5"), If(eOld >= 0.004 AndAlso eNew < 0.004, "1", "0")}))
                    Next
                Next
                Console.WriteLine(path & " cost " & F(costFile, "F2") & " reeval " & F(ev.Cost.Total, "F2"))
            Next
        End Using
        Return 0
    End Function
End Module
