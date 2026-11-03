Imports System
Imports System.Linq
Imports System.Globalization
Imports FemRcModel

'Diagnostic (read-only use of the program): at the converged FSD pre-design x0, apply every single one-step reduction of b, h, rho (columns) and bw, h (beams) and list the violated constraints.
Module Program
    Sub Main(args() As String)
        Dim ci = CultureInfo.InvariantCulture
        Dim spec As New FrameSpec, tp As New TbdyParams
        Dim dd3 As New TbdyParams With {.SDS = 0.294, .SD1 = 0.091}
        Dim pr As New RcCore.PriceSet With {.Fibre = 90.0}
        Dim prob As New Ex1Problem(spec, tp, dd3, pr, False)
        Dim pd = PreDesign.Run(prob)
        Console.WriteLine("x0 = " & String.Join(",", pd.X) & "  cost " & pd.Cost.ToString("F0", ci) & "  evals " & pd.Evaluations)
        Dim r0 = prob.Evaluate(pd.X)
        Console.WriteLine("Base: drift ratio " & r0.Drift.MaxRatio.ToString("F5", ci) & " (storey " & r0.Drift.Story & ") G=" & r0.Drift.G.ToString("F3", ci) & "  Q=" & r0.Q.ToString("F4", ci))
        For Each g In Ex1Problem.ColGroups
            Dim c = r0.ColRes(g)
            Console.WriteLine("  " & g & " b=" & c.B & " h=" & c.H & " rho=" & c.Rho.ToString("P2", ci) & " DcrMax=" & c.DcrMax.ToString("F3", ci))
        Next
        For Each v In r0.Violations.Where(Function(t) t.Item2 > -0.0000001)
            Console.WriteLine("  base violation list: " & v.Item1 & " " & v.Item2.ToString("F4", ci))
        Next
        Dim nc As Integer = Ex1Problem.ColGroups.Length
        Dim names As New System.Collections.Generic.List(Of Tuple(Of String, Integer))
        For g As Integer = 0 To nc - 1
            names.Add(Tuple.Create(Ex1Problem.ColGroups(g) & " b-1", g * 4)) : names.Add(Tuple.Create(Ex1Problem.ColGroups(g) & " h-1", g * 4 + 1)) : names.Add(Tuple.Create(Ex1Problem.ColGroups(g) & " rho-1", g * 4 + 2))
        Next
        For g As Integer = 0 To Ex1Problem.BeamGroups.Length - 1
            names.Add(Tuple.Create(Ex1Problem.BeamGroups(g) & " bw-1", nc * 4 + g * 5)) : names.Add(Tuple.Create(Ex1Problem.BeamGroups(g) & " h-1", nc * 4 + g * 5 + 1))
        Next
        For Each t In names
            If pd.X(t.Item2) <= 0 Then Console.WriteLine(t.Item1 & ": already at the lower bound") : Continue For
            Dim x = CType(pd.X.Clone(), Integer())
            x(t.Item2) -= 1
            Dim r = prob.Evaluate(x)
            Dim bad = r.Violations.Where(Function(v) v.Item2 > 0.0).Select(Function(v) v.Item1 & " (" & v.Item2.ToString("F3", ci) & ")").ToArray()
            Console.WriteLine(t.Item1 & ": cost " & r.Cost.Total.ToString("F0", ci) & " T=" & r.T.ToString("F4", ci) & If(bad.Length > 0, "  VIOLATED: " & String.Join("; ", bad), "  feasible"))
        Next
    End Sub
End Module
