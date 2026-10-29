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
        Dim dop As Integer = Integer.Parse(Arg(args, "dop", Environment.ProcessorCount.ToString()))
        Dim outDir As String = Arg(args, "out", "runs_out")
        Directory.CreateDirectory(outDir)

        Dim spec As New FrameSpec
        Dim tp As New TbdyParams
        Dim dd3 As New TbdyParams With {.SDS = 0.294, .SD1 = 0.091}
        Dim pr As New RcCore.PriceSet With {.Fibre = Double.Parse(Arg(args, "fibreprice", "90"), ci)}
        Dim prob As New Ex1Problem(spec, tp, dd3, pr, useFibre, fr)

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
                                          Dim res = Optimizers.Run(t.Item1, AddressOf prob.Objective, prob.Lb, prob.Ub, fes, seed)
                                          Dim ev = prob.Evaluate(res.BestX)
                                          Dim sb As New System.Text.StringBuilder
                                          sb.AppendLine("# " & ver)
                                          sb.AppendLine("method," & t.Item1 & ",run," & t.Item2 & ",seed," & seed & ",FEs," & res.FEs & ",fibre," & If(useFibre, 1, 0) & ",fRscale," & fr.ToString(ci) & ",fibreprice," & pr.Fibre.ToString(ci))
                                          sb.AppendLine("Fpen," & res.BestF.ToString("R", ci) & ",cost," & ev.Cost.Total.ToString("R", ci) & ",T," & ev.T.ToString("R", ci) & ",concrete," & ev.Cost.Concrete.ToString("F2", ci) & ",rebar," & ev.Cost.Rebar.ToString("F2", ci) & ",formwork," & ev.Cost.Formwork.ToString("F2", ci) & ",fibreCost," & ev.Cost.Fibre.ToString("F2", ci) & ",steelKg," & ev.SteelKg.ToString("F2", ci) & ",fibreKg," & ev.FibreKg.ToString("F2", ci) & ",concreteM3," & ev.ConcreteM3.ToString("F3", ci))
                                          sb.AppendLine("x," & String.Join(",", res.BestX))
                                          sb.AppendLine("history_FE,best")
                                          For Each h In res.History : sb.AppendLine(h.Item1 & "," & h.Item2.ToString("R", ci)) : Next
                                          File.WriteAllText(path, sb.ToString())
                                          Dim n As Integer = Threading.Interlocked.Increment(done)
                                          Console.WriteLine("[" & n & "/" & tasks.Count & "] " & name & " Fpen=" & res.BestF.ToString("F0", ci) & " T=" & ev.T.ToString("F3", ci) & "  " & (sw.ElapsedMilliseconds / 60000.0).ToString("F1", ci) & " min")
                                      End Sub)
        Console.WriteLine("finished " & done & " new runs in " & (sw.ElapsedMilliseconds / 60000.0).ToString("F1", ci) & " min")
    End Sub
End Module
