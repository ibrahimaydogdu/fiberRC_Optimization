Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Threading.Tasks
Imports FemRcModel
Imports Ex2Model

'PCR-18 (EX2_SPEC v2 §9, §11): Ex2 campaign driver, Ö1 Ex1Runner flags and PCR-11 run log.
'Usage: Ex2Runner --method YAKO|BTO|ATO|ALL --fibre 0|1 --runs 30 --fes 10000 --fr 1.0 --fibreprice 90 --seed0 1000 --dop 8 --out <dir> [--groups 9|5] [--shearformat delta|o1]
'       Ex2Runner --evalonly <file> [--groups 9|5] [--fibre 0|1] [--fr 1.0] [--fibreprice 90] [--shearformat delta|o1]   (file: one line, comma separated design vector)
'Arm A only (Q9): random start, full pool, no pre-design, no box. BTO / ATO full-dimension moves (no --onedim).
'Fibre shear (IA 2026-11-16): main = Q17 increment rule (--shearformat delta, default); --shearformat o1 = Ö1 format (gammaC = 1.5 + phi) sensitivity.
'fR sensitivity (Q18): --fr 0.8 scales fR1 and fR3 (FibreTable.MakeBeamFibre fRScale).
Class RunLog
    Public Fe As Integer, NFeas As Integer, FirstFeas As Integer, Feas100 As Integer
    Public BestFeas As Double = Double.PositiveInfinity
    Public ReadOnly FeasHist As New List(Of Tuple(Of Integer, Double))
    Public ReadOnly Viol As New SortedDictionary(Of String, Integer)
    Public Sub Add(r As Ex2Result)
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

    Function VersionLine() As String
        Dim parts As New List(Of String)
        For Each t In {GetType(Ex2Problem), GetType(Optimizers), GetType(RcCore.PriceSet), GetType(FemCore.FemModel)}
            Dim p As String = t.Assembly.Location
            parts.Add(System.IO.Path.GetFileName(p) & " sha256=" & Sha256Of(p))
        Next
        Return String.Join("; ", parts) & "; Ex2Model built=" & File.GetLastWriteTimeUtc(GetType(Ex2Problem).Assembly.Location).ToString("s") & "Z"
    End Function

    Function Main(args() As String) As Integer
        Dim ci = CultureInfo.InvariantCulture
        Dim useFibre As Boolean = Arg(args, "fibre", "0") = "1"
        Dim fr As Double = Double.Parse(Arg(args, "fr", "1.0"), ci)
        Dim pr As New RcCore.PriceSet With {.Fibre = Double.Parse(Arg(args, "fibreprice", "90"), ci)}
        Dim spec As New FrameSpec2D With {.Groups = Integer.Parse(Arg(args, "groups", "9"))}
        'shear model: plain runs ACI 22.5.5.1; fibre runs Q17 increment rule (main) or, with --shearformat o1, the Ö1 format (sensitivity only, spec §5.2)
        Dim shearFormat As String = Arg(args, "shearformat", "delta").ToLowerInvariant()
        If shearFormat <> "delta" AndAlso shearFormat <> "o1" Then
            Console.Error.WriteLine("--shearformat must be delta or o1")
            Return 2
        End If
        Dim shear As IBeamShear
        If Not useFibre Then
            shear = New AciPlainShear()
        ElseIf shearFormat = "o1" Then
            shear = New FibreShearO1Format()
        Else
            shear = New FibreShearIncrementQ17()
        End If
        Dim prob As New Ex2Problem(spec, pr, useFibre, shear, fr)
        Dim ver As String = VersionLine()

        Dim evalFile As String = Arg(args, "evalonly", "")
        If evalFile <> "" Then
            Dim line = File.ReadAllLines(evalFile).First(Function(s) s.Trim() <> "" AndAlso Not s.StartsWith("#"))
            Dim x = line.Split(","c).Select(Function(s) Integer.Parse(s.Trim())).ToArray()
            Dim ev = prob.Evaluate(x)
            Console.WriteLine("# " & ver)
            Console.WriteLine("fibre," & If(useFibre, 1, 0) & ",groups," & spec.Groups & ",fRscale," & fr.ToString(ci) & ",fibreprice," & pr.Fibre.ToString(ci) & ",shear," & prob.Shear.Name.Replace(",", ";"))
            Console.WriteLine("x," & String.Join(",", x))
            Console.WriteLine("Fpen," & ev.Fpen.ToString("R", ci) & ",cost," & ev.Cost.Total.ToString("R", ci) & ",T," & ev.T.ToString("R", ci))
            Console.WriteLine("concrete," & ev.Cost.Concrete.ToString("F2", ci) & ",rebar," & ev.Cost.Rebar.ToString("F2", ci) & ",formwork," & ev.Cost.Formwork.ToString("F2", ci) & ",fibreCost," & ev.Cost.Fibre.ToString("F2", ci))
            Console.WriteLine("concreteM3," & ev.ConcreteM3.ToString("F4", ci) & ",formworkM2," & ev.FormworkM2.ToString("F3", ci) & ",longKg," & ev.LongKg.ToString("F2", ci) & ",tieKg," & ev.TieKg.ToString("F2", ci))
            For Each v In ev.Violations : Console.WriteLine("g," & v.Item1.Replace(",", ";") & "," & v.Item2.ToString("R", ci)) : Next
            For Each kv In ev.ColRes
                Console.WriteLine("col," & kv.Key & ",b," & kv.Value.B & ",h," & kv.Value.H & ",rho," & kv.Value.Rho.ToString("F4", ci) & ",nBars," & kv.Value.NBars & ",DC," & kv.Value.DcrMax.ToString("F4", ci) & ",s," & kv.Value.S.ToString("F1", ci))
            Next
            Dim dsg = prob.Decode(x)
            For Each kv In ev.BeamRes
                Dim s = ev.BeamShr(kv.Key)
                Dim bf = dsg.Beams(kv.Key).Fibre
                Dim fibTxt As String = If(bf IsNot Nothing AndAlso bf.Params IsNot Nothing, "dose," & bf.DoseKgM3.ToString(ci) & ",fR1," & bf.Params.FR1.ToString("F3", ci) & ",fR3," & bf.Params.FR3.ToString("F3", ci), "dose,0,fR1,0,fR3,0")
                Console.WriteLine("beam," & kv.Key & ",b," & kv.Value.B & ",h," & kv.Value.H & ",MuTop," & (kv.Value.MuTop / 1.0E6).ToString("F2", ci) & ",MuBot," & (kv.Value.MuBot / 1.0E6).ToString("F2", ci) & ",AsTop," & kv.Value.AsTop.ToString("F1", ci) & ",AsBot," & kv.Value.AsBot.ToString("F1", ci) & ",Vu_d," & (s.Vu / 1000.0).ToString("F2", ci) & ",VcACI," & (s.VcPlain / 1000.0).ToString("F2", ci) & ",Vcf," & (s.Vcf / 1000.0).ToString("F2", ci) & ",dVF," & (s.DeltaVF / 1000.0).ToString("F2", ci) & ",AvMin," & s.AvMinApplied & ",s," & s.S.ToString("F1", ci) & "," & fibTxt & ",gov," & kv.Value.Governing & "/" & s.Governing)
            Next
            Return 0
        End If

        Dim methods As String() = If(Arg(args, "method", "ALL") = "ALL", {"YAKO", "BTO", "ATO"}, {Arg(args, "method", "ATO")})
        Dim runs As Integer = Integer.Parse(Arg(args, "runs", "30"))
        Dim fes As Integer = Integer.Parse(Arg(args, "fes", "10000"))
        Dim seed0 As Integer = Integer.Parse(Arg(args, "seed0", "1000"))
        Dim dop As Integer = Integer.Parse(Arg(args, "dop", Environment.ProcessorCount.ToString()))
        Dim outDir As String = Arg(args, "out", "ex2_runs_out")
        Directory.CreateDirectory(outDir)
        Optimizers.OneDimMove = False
        Dim tasks As New List(Of Tuple(Of String, Integer))
        For Each m In methods
            For r As Integer = 1 To runs : tasks.Add(Tuple.Create(m, r)) : Next
        Next
        Dim tag As String = If(useFibre, If(shearFormat = "o1", "fibreO1", "fibre"), "plain")
        Console.WriteLine("Ex2Runner " & tag & " groups " & spec.Groups & " nvars " & prob.NVars & " fR x" & fr.ToString(ci) & " fibre price " & pr.Fibre.ToString(ci) & " TL/kg, " & tasks.Count & " runs, " & fes & " FEs, dop " & dop)
        Console.WriteLine(ver)
        Dim sw = Diagnostics.Stopwatch.StartNew()
        Dim done As Integer = 0
        Dim opts As New ParallelOptions With {.MaxDegreeOfParallelism = dop}
        Parallel.ForEach(tasks, opts, Sub(t)
                                          Dim name As String = "EX2_" & t.Item1 & "_" & tag & "_g" & spec.Groups & "_fr" & fr.ToString("0.00", ci) & "_fp" & pr.Fibre.ToString("0", ci) & "_run" & t.Item2.ToString("00")
                                          Dim path As String = System.IO.Path.Combine(outDir, name & ".csv")
                                          If File.Exists(path) Then Return
                                          Dim seed As Integer = seed0 + t.Item2
                                          Dim lg As New RunLog
                                          Dim tw = Diagnostics.Stopwatch.StartNew()
                                          Dim obj = Function(x As Integer()) As Double
                                                        Dim rr = prob.Evaluate(x)
                                                        lg.Add(rr)
                                                        Return rr.Fpen
                                                    End Function
                                          Dim res = Optimizers.Run(t.Item1, obj, prob.Lb, prob.Ub, fes, seed, Nothing)
                                          Dim secs As Double = tw.Elapsed.TotalSeconds
                                          Dim ev = prob.Evaluate(res.BestX)
                                          Dim sb As New System.Text.StringBuilder
                                          sb.AppendLine("# " & ver)
                                          sb.AppendLine("method," & t.Item1 & ",run," & t.Item2 & ",seed," & seed & ",FEs," & res.FEs & ",fibre," & If(useFibre, 1, 0) & ",groups," & spec.Groups & ",fRscale," & fr.ToString(ci) & ",fibreprice," & pr.Fibre.ToString(ci) & ",arm,A,shear," & prob.Shear.Name.Replace(",", ";") & ",wallSeconds," & secs.ToString("F2", ci))
                                          sb.AppendLine("Fpen," & res.BestF.ToString("R", ci) & ",cost," & ev.Cost.Total.ToString("R", ci) & ",T," & ev.T.ToString("R", ci) & ",concrete," & ev.Cost.Concrete.ToString("F2", ci) & ",rebar," & ev.Cost.Rebar.ToString("F2", ci) & ",formwork," & ev.Cost.Formwork.ToString("F2", ci) & ",fibreCost," & ev.Cost.Fibre.ToString("F2", ci) & ",steelKg," & ev.SteelKg.ToString("F2", ci) & ",fibreKg," & ev.FibreKg.ToString("F2", ci) & ",concreteM3," & ev.ConcreteM3.ToString("F3", ci))
                                          sb.AppendLine("x," & String.Join(",", res.BestX))
                                          sb.AppendLine("feas_summary,evals," & lg.Fe & ",feasibleEvals," & lg.NFeas & ",firstFeasibleFE," & lg.FirstFeas & ",feasibleInFirst100," & lg.Feas100 & ",bestFeasible," & If(Double.IsPositiveInfinity(lg.BestFeas), "none", lg.BestFeas.ToString("R", ci)) & ",totalFEs," & res.FEs)
                                          sb.AppendLine("violation_counts," & String.Join(",", lg.Viol.Select(Function(kv) kv.Key.Replace(",", ";") & "=" & kv.Value)))
                                          sb.AppendLine("feasible_history," & String.Join(";", lg.FeasHist.Select(Function(h) h.Item1 & ":" & h.Item2.ToString("R", ci))))
                                          sb.AppendLine("history_FE,best")
                                          For Each h In res.History : sb.AppendLine(h.Item1 & "," & h.Item2.ToString("R", ci)) : Next
                                          File.WriteAllText(path, sb.ToString())
                                          Dim n As Integer = Threading.Interlocked.Increment(done)
                                          Console.WriteLine("[" & n & "/" & tasks.Count & "] " & name & " Fpen=" & res.BestF.ToString("F0", ci) & " T=" & ev.T.ToString("F3", ci) & " " & secs.ToString("F2", ci) & " s (" & (1000.0 * secs / Math.Max(res.FEs, 1)).ToString("F2", ci) & " ms/FE)")
                                      End Sub)
        Console.WriteLine("finished " & done & " new runs in " & sw.Elapsed.TotalSeconds.ToString("F2", ci) & " s")
        Return 0
    End Function
End Module
