Imports System
Imports System.Collections.Generic
Imports System.Linq

'PCR-7 pre-design (PREDESIGN_PROTOCOL.md, frozen 2026-11-02): deterministic, scenario independent, evaluated with the PLAIN (fibre free) capacity model.
'Phase 1 (FUD-like start): generous start design; while the design is infeasible, drift / stability violations raise the columns and beams of the governing storey by one step, any other violation raises every group by one step.
'Phase 2 (FSD, until no group can be reduced (safety bound MaxRounds), beams before columns because column shear and the strong column rule depend on the beam bars): a group whose utilisation is below its trigger (column D/C < ColTrigger,
'beam max(As)/As(2 %) < BeamTrigger) is reduced by one step (columns: rho first, then the larger of b, h); the step is kept only if the whole frame stays feasible (T = 0), otherwise it is reverted and the group is frozen.
'Output: start design x0 and the search box [x0 - MarginDown, x0 + MarginUp] for b, h, rho (columns) and bw, h (beams); concrete class, fibre type and dose keep the full pool.
Public Class PreDesignResult
    Public X() As Integer
    Public Lo() As Integer, Hi() As Integer
    Public Evaluations As Integer
    Public Cost As Double, Fpen As Double, T As Double
    Public Log As New List(Of String)
End Class

Public Module PreDesign
    Public Const ColTrigger As Double = 0.85
    Public Const BeamTrigger As Double = 0.7
    Public Const MarginDown As Integer = 2
    Public Const MarginUp As Integer = 3
    Public Const MaxRounds As Integer = 60      'safety bound only: the loop ends when no group can be reduced (PCR-7 revision 2026-11-03, IA decision A)
    Public Const MaxRaises As Integer = 12

    Private Function ColIdx(g As Integer) As Integer
        Return g * Ex1Problem.ColVars
    End Function

    Private Function BeamIdx(g As Integer) As Integer
        Return Ex1Problem.ColGroups.Length * Ex1Problem.ColVars + g * Ex1Problem.BeamVars
    End Function

    'PCR-13: fibreType >= 0 runs the FSD/FUD with the FIBRE capacity model and all beam groups fixed to (fibreType, dose); default (-1) = plain pre-design as in PCR-7
    Public Function Run(prob As Ex1Problem, Optional fibreType As Integer = -1, Optional dose As Integer = 0) As PreDesignResult
        Dim plain As Ex1Problem = If(fibreType >= 0, prob, New Ex1Problem(prob.Spec, prob.Tp, prob.Dd3, prob.Prices, False, 1.0, prob.DeriveRho))
        Dim res As New PreDesignResult
        Dim nCol As Integer = Ex1Problem.ColGroups.Length, nBeam As Integer = Ex1Problem.BeamGroups.Length
        Dim x(plain.FullLb.Length - 1) As Integer
        For g As Integer = 0 To nCol - 1
            Dim k As Integer = ColIdx(g)
            x(k) = 6 : x(k + 1) = 6 : x(k + 2) = 4 : x(k + 3) = 0       '600 x 600, rho 2 %, C30
        Next
        For g As Integer = 0 To nBeam - 1
            Dim k As Integer = BeamIdx(g)
            x(k) = 1 : x(k + 1) = 6 : x(k + 2) = 0                      '300 x 600, C30
            If fibreType >= 0 Then x(k + 3) = fibreType : x(k + 4) = dose
        Next
        Dim ev As Func(Of Integer(), Ex1Result) = Function(v)
                                                      res.Evaluations += 1
                                                      Return plain.EvaluateFull(v)
                                                  End Function
        Dim r = ev(x)
        'phase 1: restore feasibility
        Dim raises As Integer = 0
        While r.T > 0.0 AndAlso raises < MaxRaises
            raises += 1
            Dim stiff As Boolean = r.Violations.Any(Function(t) t.Item2 > 0.0 AndAlso (t.Item1.StartsWith("C2") OrElse t.Item1.StartsWith("ACI 6.6.4.3")))
            Dim changed As Boolean = False
            For g As Integer = 0 To nCol - 1
                Dim upper As Boolean = Ex1Problem.ColGroups(g).EndsWith("S34")
                If stiff AndAlso (r.Drift.Story <= 2) = upper Then Continue For
                For q As Integer = 0 To 1 : If x(ColIdx(g) + q) < plain.FullUb(ColIdx(g) + q) Then x(ColIdx(g) + q) += 1 : changed = True
                Next
            Next
            For g As Integer = 0 To nBeam - 1
                Dim upper As Boolean = Ex1Problem.BeamGroups(g).EndsWith("S34")
                If stiff AndAlso (r.Drift.Story <= 2) = upper Then Continue For
                If x(BeamIdx(g) + 1) < plain.FullUb(BeamIdx(g) + 1) Then x(BeamIdx(g) + 1) += 1 : changed = True
            Next
            res.Log.Add("raise " & raises & (If(stiff, " (drift/Q, storey " & r.Drift.Story & ")", " (strength)")) & ": T=" & r.T.ToString("F4"))
            If Not changed Then Exit While
            r = ev(x)
        End While
        'phase 2: fully stressed reduction with a feasibility guard
        If r.T = 0.0 Then
            Dim frozenC(nCol - 1) As Boolean, frozenB(nBeam - 1) As Boolean
            For rnd As Integer = 1 To MaxRounds
                Dim anyChg As Boolean = False
                For g As Integer = 0 To nBeam - 1
                    If frozenB(g) Then Continue For
                    Dim br = r.BeamRes(Ex1Problem.BeamGroups(g))
                    Dim util As Double = Math.Max(br.AsTop, br.AsBot) / br.AsMaxRho
                    Dim k As Integer = BeamIdx(g) + 1
                    If util >= BeamTrigger OrElse x(k) <= plain.FullLb(k) Then frozenB(g) = True : Continue For
                    x(k) -= 1
                    Dim r2 = ev(x)
                    If r2.T = 0.0 Then r = r2 : anyChg = True Else x(k) += 1 : frozenB(g) = True
                Next
                For g As Integer = 0 To nCol - 1
                    If frozenC(g) Then Continue For
                    Dim cr = r.ColRes(Ex1Problem.ColGroups(g))
                    Dim k As Integer = ColIdx(g)
                    If cr.DcrMax >= ColTrigger Then frozenC(g) = True : Continue For
                    Dim kk As Integer = -1
                    If Not plain.DeriveRho AndAlso x(k + 2) > plain.FullLb(k + 2) Then
                        kk = k + 2
                    ElseIf x(k) >= x(k + 1) AndAlso x(k) > plain.FullLb(k) Then
                        kk = k
                    ElseIf x(k + 1) > plain.FullLb(k + 1) Then
                        kk = k + 1
                    End If
                    If kk < 0 Then frozenC(g) = True : Continue For
                    x(kk) -= 1
                    Dim r2 = ev(x)
                    If r2.T = 0.0 Then r = r2 : anyChg = True Else x(kk) += 1 : frozenC(g) = True
                Next
                res.Log.Add("FSD round " & rnd & ": cost " & r.Cost.Total.ToString("F0") & " evals " & res.Evaluations)
                If Not anyChg Then Exit For
            Next
        End If
        res.X = CType(x.Clone(), Integer())
        res.Cost = r.Cost.Total : res.Fpen = r.Fpen : res.T = r.T
        'box: b, h, rho (columns), bw, h (beams) narrowed; class, fibre type and dose keep the full pool
        ReDim res.Lo(x.Length - 1) : ReDim res.Hi(x.Length - 1)
        For i As Integer = 0 To x.Length - 1 : res.Lo(i) = plain.FullLb(i) : res.Hi(i) = prob.FullUb(i) : Next
        Dim narrow = Sub(i As Integer)
                         res.Lo(i) = Math.Max(plain.FullLb(i), x(i) - MarginDown)
                         res.Hi(i) = Math.Min(prob.FullUb(i), x(i) + MarginUp)
                     End Sub
        For g As Integer = 0 To nCol - 1 : For q As Integer = 0 To 2 : narrow(ColIdx(g) + q) : Next : Next
        For g As Integer = 0 To nBeam - 1 : narrow(BeamIdx(g)) : narrow(BeamIdx(g) + 1) : Next
        Return res
    End Function

    'PCR-13 (fibre scenario): scenario-specific pre-design. For every (type, dose) of the grid (all four beam groups share it) the FSD/FUD is run with the fibre capacity model; the plain pre-design is one more
    'candidate (dose 0). The candidate with the lowest penalised cost is x0 (feasible preferred); all evaluations of all candidates are counted in the returned Evaluations (part of the total budget). Box from the chosen candidate.
    Public Function RunFibre(prob As Ex1Problem) As PreDesignResult
        Dim best As PreDesignResult = Run(prob)
        Dim total As Integer = best.Evaluations
        best.Log.Add("candidate plain: cost " & best.Cost.ToString("F0") & " Fpen " & best.Fpen.ToString("F0") & " evals " & best.Evaluations)
        Dim log As New List(Of String)(best.Log)
        For t As Integer = 0 To 1
            For d As Integer = 1 To Ex1Problem.Doses.Length - 1
                Dim r = Run(prob, t, d)
                total += r.Evaluations
                log.Add("candidate type " & t & " dose idx " & d & ": cost " & r.Cost.ToString("F0") & " Fpen " & r.Fpen.ToString("F0") & " T " & r.T.ToString("F4") & " evals " & r.Evaluations)
                If r.Fpen < best.Fpen Then best = r
            Next
        Next
        best.Evaluations = total
        best.Log.Clear() : best.Log.AddRange(log)
        Return best
    End Function

    'Fibre scenario only (protocol phase 3): the pre-design x0 (plain) is completed with one common fibre type and dose for all four beam groups, chosen by scanning the type x dose grid with the fibre evaluator
    '(dose index 0 = plain is part of the scan, so the start can never be worse than the plain pre-design). Returns the evaluation count.
    Public Function DoseScan(fibreProb As Ex1Problem, x0() As Integer, ByRef evals As Integer) As Integer()
        Dim best = CType(x0.Clone(), Integer())
        Dim bestF As Double = fibreProb.EvaluateFull(best).Fpen : evals = 1
        For t As Integer = 0 To 1
            For d As Integer = 1 To Ex1Problem.Doses.Length - 1
                Dim v = CType(x0.Clone(), Integer())
                For g As Integer = 0 To Ex1Problem.BeamGroups.Length - 1 : v(BeamIdx(g) + 3) = t : v(BeamIdx(g) + 4) = d : Next
                Dim f As Double = fibreProb.EvaluateFull(v).Fpen : evals += 1
                If f < bestF Then bestF = f : best = v
            Next
        Next
        Return best
    End Function
End Module
