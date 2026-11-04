Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports FemRcModel

'Ex1 campaign driver: independent seeded runs of YAKO / BTO / ATO on the Ex1 problem, runs in parallel, one result file per run (resumable).
'Usage: Ex1Runner --method YAKO|BTO|ATO|ALL --fibre 0|1 --runs 30 --fes 10000 --fr 1.0 --out <dir> [--seed0 1000] [--dop 8] [--fibreprice 90]
'Every run file carries the program version (SHA-256 of FemRcModel.dll, build time), seed, FE budget and the best design.

'PCR-11: per-run evaluation log (thread private: one instance per run)
Class RunLog
    Public Fe As Integer, NFeas As Integer, FirstFeas As Integer, Feas100 As Integer
    Public BestFeas As Double = Double.PositiveInfinity
    Public ReadOnly FeasHist As New List(Of Tuple(Of Integer, Double))
    Public ReadOnly Viol As New SortedDictionary(Of String, Integer)
    Public Sub Add(r As Ex1Result)
        Fe += 1
        For Each v In r.Violations
            If v.Item2 > 0.0 Then Viol(v.Item1) = If(Viol.ContainsKey(v.Item1), Viol(v.Item1) + 1, 1)
        Next
        If r.T = 0.0 Then
            NFeas += 1
            If FirstFeas = 0 Then FirstFeas = Fe
            If Fe <= 100 Then Feas100 += 1
            If r.Cost.Total < BestFeas Then BestFeas = r.Cost.Total : FeasHist.Add(Tuple.Create(Fe, BestFeas))
        End If
    End Sub
End Class

Module Program
    Function Arg(args() As String, name As String, def As String) As String
        For i As Integer = 0 To args.Length - 2
            If args(i) = "--" & name Then Return args(i + 1)
        Next
        Return def
    End Function

    Function Sha256Of(path As String) As String
        Using fs = File.OpenRead(path)
            Using sha = System.Security.Cryptography.SHA256.Create()
                Return BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "").ToLowerInvariant()
            End Using
        End Using
    End Function

    Sub Main(args() As String)
        Dim ci = CultureInfo.InvariantCulture
        Dim methods As String() = If(Arg(args, "method", "ALL") = "ALL", {"YAKO", "BTO", "ATO"}, {Arg(args, "method", "ATO")})
        Dim useFibre As Boolean = Arg(args, "fibre", "0") = "1"
        Dim runs As Integer = Integer.Parse(Arg(args, "runs", "30"))
        Dim fes As Integer = Integer.Parse(Arg(args, "fes", "10000"))
        Dim fr As Double = Double.Parse(Arg(args, "fr", "1.0"), ci)
        Dim seed0 As Integer = Integer.Parse(Arg(args, "seed0", "1000"))
        Optimizers.OneDimMove = (Arg(args, "onedim", "0") = "1")
        Dim dop As Integer = Integer.Parse(Arg(args, "dop", Environment.ProcessorCount.ToString()))
        Dim outDir As String = Arg(args, "out", "runs_out")
        Directory.CreateDirectory(outDir)

        Dim spec As New FrameSpec
        Dim tp As New TbdyParams
        Dim dd3 As New TbdyParams With {.SDS = 0.294, .SD1 = 0.091}
        Dim pr As New RcCore.PriceSet With {.Fibre = Double.Parse(Arg(args, "fibreprice", "90"), ci)}
        Dim derive As Boolean = Arg(args, "derive", "0") = "1"      'PCR-8: column rho derived by the evaluator, not a search variable
        Dim prob As New Ex1Problem(spec, tp, dd3, pr, useFibre, fr, derive)

        'PCR-6: optional search box, text file with two lines of comma separated indices (lower bounds, upper bounds); the box is part of the run record
        Dim boxFile As String = Arg(args, "box", "")
        Dim boxTxt As String = ""
        If boxFile <> "" Then
            Dim bl = File.ReadAllLines(boxFile).Where(Function(s) s.Trim() <> "" AndAlso Not s.StartsWith("#")).ToArray()
            Dim lo = bl(0).Split(","c).Select(Function(s) Integer.Parse(s.Trim())).ToArray()
            Dim hi = bl(1).Split(","c).Select(Function(s) Integer.Parse(s.Trim())).ToArray()
            prob.ApplyBox(lo, hi)
            boxTxt = ",box," & Sha256Of(boxFile).Substring(0, 16)
        End If
        'PCR-7: --predesign 1 computes the (plain, deterministic) FSD/FUD pre-design, restricts every method to its box and starts it from x0 (fibre: x0 completed by the dose scan)
        Dim x0 As Integer() = Nothing
        Dim preTxt As String = ""
        Dim preEvals As Integer = 0
        If Arg(args, "predesign", "0") = "1" Then
            Dim sw0 = Diagnostics.Stopwatch.StartNew()
            Dim pd = PreDesign.Run(prob)
            Dim ev As Integer = pd.Evaluations
            If Arg(args, "usebox", "1") = "1" Then prob.ApplyBox(pd.Lo, pd.Hi)      'arm without box: x0 only, full pool
            x0 = pd.X
            If useFibre Then
                Dim ne As Integer
                x0 = PreDesign.DoseScan(prob, pd.X, ne) : ev += ne
            End If
            File.WriteAllText(Path.Combine(outDir, "predesign_" & If(useFibre, "fibre", "plain") & ".txt"),
                "# pre-design evaluations " & ev & ", cost " & pd.Cost.ToString("R", ci) & ", T " & pd.T.ToString("R", ci) & vbLf &
                "# x0" & vbLf & String.Join(",", x0) & vbLf & "# box lo, hi" & vbLf & String.Join(",", pd.Lo) & vbLf & String.Join(",", pd.Hi) & vbLf & "# log" & vbLf & String.Join(vbLf, pd.Log) & vbLf)
            preEvals = ev
            preTxt = ",predesignEvals," & ev & ",x0cost," & pd.Cost.ToString("F0", ci) & ",usebox," & Arg(args, "usebox", "1")
            Console.WriteLine("pre-design: " & ev & " evaluations, " & (sw0.ElapsedMilliseconds / 60000.0).ToString("F1", ci) & " min, cost " & pd.Cost.ToString("F0", ci) & " T " & pd.T.ToString("F4", ci))
        End If
        Dim dll As String = GetType(Ex1Problem).Assembly.Location
        Dim ver As String = "FemRcModel.dll sha256=" & Sha256Of(dll) & " built=" & File.GetLastWriteTimeUtc(dll).ToString("s") & "Z"
        Dim tasks As New List(Of Tuple(Of String, Integer))
        For Each m In methods
            For r As Integer = 1 To runs : tasks.Add(Tuple.Create(m, r)) : Next
        Next
        Dim tag As String = If(useFibre, "fibre", "plain")
        Console.WriteLine("Ex1Runner " & tag & " fR x" & fr.ToString(ci) & " fibre price " & pr.Fibre.ToString(ci) & " TL/kg, " & tasks.Count & " runs, " & fes & " FEs, dop " & dop)
        Console.WriteLine(ver)
        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim done As Integer = 0
        Dim opts As New ParallelOptions With {.MaxDegreeOfParallelism = dop}
        Parallel.ForEach(tasks, opts, Sub(t)
                                          Dim name As String = t.Item1 & "_" & tag & "_fr" & fr.ToString("0.00", ci) & "_run" & t.Item2.ToString("00")
                                          Dim path As String = System.IO.Path.Combine(outDir, name & ".csv")
                                          If File.Exists(path) Then Return
                                          Dim seed As Integer = seed0 + t.Item2
                                          'PCR-11: run log (feasibility history, violation counts); the pre-design evaluations are part of the budget: search FEs = total FEs - pre-design FEs
                                          Dim lg As New RunLog
                                          Dim obj = Function(x As Integer()) As Double
                                                        Dim rr = prob.Evaluate(x)
                                                        lg.Add(rr)
                                                        Return rr.Fpen
                                                    End Function
                                          Dim res = Optimizers.Run(t.Item1, obj, prob.Lb, prob.Ub, fes - preEvals, seed, If(x0 Is Nothing, Nothing, prob.ToActive(x0)))
                                          Dim ev = prob.Evaluate(res.BestX)
                                          Dim sb As New System.Text.StringBuilder
                                          sb.AppendLine("# " & ver)
                                          sb.AppendLine("method," & t.Item1 & ",run," & t.Item2 & ",seed," & seed & ",FEs," & res.FEs & ",fibre," & If(useFibre, 1, 0) & ",fRscale," & fr.ToString(ci) & ",fibreprice," & pr.Fibre.ToString(ci) & boxTxt & preTxt & If(derive, ",derivedRho,1", "") & If(Optimizers.OneDimMove, ",oneDim,1", ""))
                                          sb.AppendLine("Fpen," & res.BestF.ToString("R", ci) & ",cost," & ev.Cost.Total.ToString("R", ci) & ",T," & ev.T.ToString("R", ci) & ",concrete," & ev.Cost.Concrete.ToString("F2", ci) & ",rebar," & ev.Cost.Rebar.ToString("F2", ci) & ",formwork," & ev.Cost.Formwork.ToString("F2", ci) & ",fibreCost," & ev.Cost.Fibre.ToString("F2", ci) & ",steelKg," & ev.SteelKg.ToString("F2", ci) & ",fibreKg," & ev.FibreKg.ToString("F2", ci) & ",concreteM3," & ev.ConcreteM3.ToString("F3", ci))
                                          sb.AppendLine("x," & String.Join(",", prob.ToFull(res.BestX)))
                                          sb.AppendLine("feas_summary,evals," & lg.Fe & ",feasibleEvals," & lg.NFeas & ",firstFeasibleFE," & lg.FirstFeas & ",feasibleInFirst100," & lg.Feas100 & ",bestFeasible," & If(Double.IsPositiveInfinity(lg.BestFeas), "none", lg.BestFeas.ToString("R", ci)) & ",totalFEs," & (res.FEs + preEvals))
                                          sb.AppendLine("violation_counts," & String.Join(",", lg.Viol.Select(Function(kv) kv.Key.Replace(",", ";") & "=" & kv.Value)))
                                          sb.AppendLine("feasible_history," & String.Join(";", lg.FeasHist.Select(Function(h) h.Item1 & ":" & h.Item2.ToString("R", ci))))
                                          sb.AppendLine("history_FE,best")
                                          For Each h In res.History : sb.AppendLine(h.Item1 & "," & h.Item2.ToString("R", ci)) : Next
                                          File.WriteAllText(path, sb.ToString())
                                          Dim n As Integer = Threading.Interlocked.Increment(done)
                                          Console.WriteLine("[" & n & "/" & tasks.Count & "] " & name & " Fpen=" & res.BestF.ToString("F0", ci) & " T=" & ev.T.ToString("F3", ci) & "  " & (sw.ElapsedMilliseconds / 60000.0).ToString("F1", ci) & " min")
                                      End Sub)
        Console.WriteLine("finished " & done & " new runs in " & (sw.ElapsedMilliseconds / 60000.0).ToString("F1", ci) & " min")
    End Sub
End Module
