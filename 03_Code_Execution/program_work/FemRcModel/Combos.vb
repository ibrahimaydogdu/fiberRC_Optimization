Imports System
Imports System.Collections.Generic
Imports FemCore

'Load combinations and member-force envelopes of the 3D RC frame (EX1_SPEC v3).
'Gravity combinations use the gross-section model. Earthquake combinations use the cracked-section model (TBDY-2018 Table 4.2:
'beam 0.35 EI, column 0.70 EI) and response spectrum magnitudes +/- (RSResults.FaceForce + |TorsionFace| of the 5 % accidental eccentricity).
'Face forces: 12 values per member, local axes: I end (P, V2, V3, T, M2, M3), J end (same).
Public Class RcSectionDef
    Public B, H As Double       'mm
    Public Fc As Double         'MPa
End Class

Public Class ComboSet
    Public Name As String
    Public Seismic As Boolean
    Public Forces As New List(Of Double()())    'one load set (static) or two (static + magnitude, static - magnitude), each [element](12)
    Public Mid As New List(Of Double()())       'same sets at the middle of the clear span: [element](6) N, V2, V3, T, M2, M3 (FemStatic.StationForces convention)
End Class

Public Class FrameAnalysis
    Public Gross, Cracked As FrameModel
    Public Combos As New List(Of ComboSet)
    Public Modal As ModalResults
    Public BaseShearX, BaseShearY, Weight As Double
    Public RSx, RSy As RSResults
    Public Rg, Rc As StaticResults            'static results of the gross and cracked models (patterns G, Q, S)
End Class

Public Module RcCombos
    Public Const CrackBeam As Double = 0.35, CrackColumn As Double = 0.7

    Public Function MakeSections(spec As FrameSpec, defs As Dictionary(Of String, RcSectionDef), crack As Boolean) As Dictionary(Of String, FemSection)
        Dim d As New Dictionary(Of String, FemSection)
        For Each kv In defs
            Dim f As Double = 1.0
            If crack Then f = If(kv.Key.StartsWith("C"), CrackColumn, CrackBeam)
            d(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, f)
        Next
        Return d
    End Function

    Private Sub SetMass(fm As FrameModel)
        Dim m = fm.Model
        m.MassSource.SelfMass = False            'self weight is in pattern G
        m.MassSource.Patterns.Add(New FemTerm With {.Name = "G", .Factor = 1.0})
        m.MassSource.Patterns.Add(New FemTerm With {.Name = "Q", .Factor = 0.3})
        m.MassSource.Patterns.Add(New FemTerm With {.Name = "S", .Factor = 0.3})
    End Sub

    Private Function Lin(R As StaticResults, m As FemModel, terms() As Tuple(Of String, Double)) As Double()()
        Dim ne As Integer = m.Elements.Count
        Dim o(ne - 1)() As Double
        For ei As Integer = 0 To ne - 1
            o(ei) = New Double(11) {}
            For Each t In terms
                Dim p As Integer = m.PatternIndex(t.Item1)
                For c As Integer = 0 To 11 : o(ei)(c) += t.Item2 * R.FaceForce(p)(ei)(c) : Next
            Next
        Next
        Return o
    End Function

    Private Function LinMid(fs As FemStatic, R As StaticResults, m As FemModel, terms() As Tuple(Of String, Double)) As Double()()
        Dim o(m.Elements.Count - 1)() As Double
        For ei As Integer = 0 To m.Elements.Count - 1
            o(ei) = New Double(5) {}
            Dim xm() As Double = {0.5 * fs.ClearLength(m.Elements(ei))}
            For Each t In terms
                Dim sp = fs.StationForces(R, m.PatternIndex(t.Item1), ei, xm)
                For c As Integer = 0 To 5 : o(ei)(c) += t.Item2 * sp(0)(c) : Next
            Next
        Next
        Return o
    End Function

    Private Function MagMid(fs As FemStatic, rr As RSResults, m As FemModel) As Double()()
        Dim o(m.Elements.Count - 1)() As Double
        For ei As Integer = 0 To m.Elements.Count - 1
            o(ei) = fs.RSStationForces(rr, ei, {0.5 * fs.ClearLength(m.Elements(ei))})(0)
        Next
        Return o
    End Function

    Private Function ShiftMid(a As Double()(), b As Double()(), sgn As Double) As Double()()
        Dim o(a.Length - 1)() As Double
        For i As Integer = 0 To a.Length - 1
            o(i) = New Double(5) {}
            For c As Integer = 0 To 5 : o(i)(c) = a(i)(c) + sgn * b(i)(c) : Next
        Next
        Return o
    End Function

    Private Function Mag(rr As RSResults, ne As Integer) As Double()()
        Dim o(ne - 1)() As Double
        For ei As Integer = 0 To ne - 1
            o(ei) = New Double(11) {}
            For c As Integer = 0 To 11
                o(ei)(c) = Math.Abs(rr.FaceForce(ei)(c)) + If(rr.TorsionFace IsNot Nothing, Math.Abs(rr.TorsionFace(ei)(c)), 0.0)
            Next
        Next
        Return o
    End Function

    Private Function Shift(a As Double()(), b As Double()(), sgn As Double) As Double()()
        Dim o(a.Length - 1)() As Double
        For i As Integer = 0 To a.Length - 1
            o(i) = New Double(11) {}
            For c As Integer = 0 To 11 : o(i)(c) = a(i)(c) + sgn * b(i)(c) : Next
        Next
        Return o
    End Function

    'Runs both analyses and builds the combination sets.
    'Combinations: U1 1.4G+1.6(Q+S) [gravity; TS 500 form, snow treated like Q] | TBDY 4.4.4.1 Eq. 4.11 / 4.12 with Ed(H) of Eq. 4.9 and Ed(Z) of Eq. 4.10 (all verified: PDF 62 / printed 41)
    Public Function Analyze(spec As FrameSpec, defs As Dictionary(Of String, RcSectionDef), tp As TbdyParams) As FrameAnalysis
        Dim fa As New FrameAnalysis
        fa.Gross = RcFrameBuilder.Build(spec, MakeSections(spec, defs, False))
        Dim fsg As New FemStatic(fa.Gross.Model)
        Dim Rg = fsg.Solve()
        fa.Rg = Rg
        If Rg.Failed Then Throw New InvalidOperationException("gross model: " & Rg.Message)
        Dim cg As New ComboSet With {.Name = "U1 1.4G+1.6(Q+S)"}
        cg.Forces.Add(Lin(Rg, fa.Gross.Model, {Tuple.Create("G", 1.4), Tuple.Create("Q", 1.6), Tuple.Create("S", 1.6)}))
        cg.Mid.Add(LinMid(fsg, Rg, fa.Gross.Model, {Tuple.Create("G", 1.4), Tuple.Create("Q", 1.6), Tuple.Create("S", 1.6)}))
        fa.Combos.Add(cg)

        fa.Cracked = RcFrameBuilder.Build(spec, MakeSections(spec, defs, True))
        Dim m = fa.Cracked.Model
        SetMass(fa.Cracked)
        m.Spectra.Add(Tbdy.DesignSpectrum(tp, "SPD"))
        Dim fs As New FemStatic(m)
        Dim Rc = fs.Solve()
        fa.Rc = Rc
        If Rc.Failed Then Throw New InvalidOperationException("cracked model: " & Rc.Message)
        fa.Modal = fs.ModalSolve(12)
        Dim ne As Integer = m.Elements.Count
        Dim mx As Double()(), my As Double()(), mmx As Double()(), mmy As Double()()
        For d As Integer = 0 To 1
            Dim rr = fs.ResponseSpectrum(fa.Modal, New FemRSCase With {.Name = If(d = 0, "EX", "EY"), .Direction = d, .Spectrum = "SPD", .ScaleFactor = FrameModel.G, .Eccentricity = 0.05})
            If d = 0 Then fa.RSx = rr Else fa.RSy = rr
            If d = 0 Then fa.BaseShearX = rr.BaseForce(0) : mx = Mag(rr, ne) : mmx = MagMid(fs, rr, m) Else fa.BaseShearY = rr.BaseForce(1) : my = Mag(rr, ne) : mmy = MagMid(fs, rr, m)
        Next
        'TBDY 4.4.2.1 (Eq. 4.9): Ed(H) = +-Ex +- 0.3 Ey  |  +-0.3 Ex +- Ey  (response spectrum results are magnitudes)
        Dim eH As New List(Of Tuple(Of String, Double()())) From {Tuple.Create("X+0.3Y", Shift(mx, my, 0.3)), Tuple.Create("0.3X+Y", Shift(my, mx, 0.3))}
        Dim eHm As New List(Of Double()()) From {ShiftMid(mmx, mmy, 0.3), ShiftMid(mmy, mmx, 0.3)}
        'TBDY 4.4.3.2 (Eq. 4.10): Ed(Z) ~ (2/3) SDS G; Eq. 4.11: G + Q + 0.2 S + Ed(H) + 0.3 Ed(Z); Eq. 4.12: 0.9 G + H + Ed(H) - 0.3 Ed(Z)  (H = soil thrust = 0)
        Dim ez As Double = 0.3 * (2.0 / 3.0) * tp.SDS
        Dim s1 = Lin(Rc, m, {Tuple.Create("G", 1.0 + ez), Tuple.Create("Q", 1.0), Tuple.Create("S", 0.2)})
        Dim s2 = Lin(Rc, m, {Tuple.Create("G", 0.9 - ez)})
        Dim m1 = LinMid(fs, Rc, m, {Tuple.Create("G", 1.0 + ez), Tuple.Create("Q", 1.0), Tuple.Create("S", 0.2)})
        Dim m2 = LinMid(fs, Rc, m, {Tuple.Create("G", 0.9 - ez)})
        For ix As Integer = 0 To eH.Count - 1
            Dim e = eH(ix)
            Dim c1 As New ComboSet With {.Name = "Eq4.11 E(H)=" & e.Item1, .Seismic = True}
            c1.Forces.Add(Shift(s1, e.Item2, 1.0)) : c1.Forces.Add(Shift(s1, e.Item2, -1.0))
            c1.Mid.Add(ShiftMid(m1, eHm(ix), 1.0)) : c1.Mid.Add(ShiftMid(m1, eHm(ix), -1.0))
            Dim c2 As New ComboSet With {.Name = "Eq4.12 E(H)=" & e.Item1, .Seismic = True}
            c2.Forces.Add(Shift(s2, e.Item2, 1.0)) : c2.Forces.Add(Shift(s2, e.Item2, -1.0))
            c2.Mid.Add(ShiftMid(m2, eHm(ix), 1.0)) : c2.Mid.Add(ShiftMid(m2, eHm(ix), -1.0))
            fa.Combos.Add(c1) : fa.Combos.Add(c2)
        Next
        fa.Weight = fa.Modal.TotalMass(0) * FrameModel.G
        Return fa
    End Function
End Module
