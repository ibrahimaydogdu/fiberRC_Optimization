Imports System.Linq
Imports System
Imports System.Collections.Generic
Imports FemCore
Imports RcCore

'Ex1 (3D, 4 storeys, 116 members) design evaluation: design vector -> cost (TL, ÇŞİDB 2026) and normalised constraints g (<= 0 feasible) -> multiplicative penalty.
'Code basis (decision A, 2026-10-29): ACI 318M-14 section capacities + fib MC2010 fibre; TBDY-2018 loads, A11 constraints and capacity-design moments (fcd, fyd).
Public Class ColumnDef
    Public B, H, Rho, Fc As Double      'mm, mm, -, MPa
    Public BarDia As Double = 20.0
End Class

Public Class BeamDef
    Public B, H, Fc As Double           'mm, mm, MPa
    Public Fibre As BeamFibre           'Nothing = plain RC
End Class

Public Class Ex1Design
    Public Columns As New Dictionary(Of String, ColumnDef)
    Public Beams As New Dictionary(Of String, BeamDef)
End Class

Public Class Ex1Result
    Public Cost As CostBreakdown
    Public CostBeams, CostColumns As Double
    Public T As Double                                   'sum of the group violations
    Public Fpen As Double
    Public Violations As New List(Of Tuple(Of String, Double))      'name, worst g (> 0 = violated)
    Public ColRes As New Dictionary(Of String, ColumnGroupResult)
    Public BeamRes As New Dictionary(Of String, BeamGroupResult)
    Public Analysis As FrameAnalysis
    Public Drift As DriftResult
    Public SteelKg As Double, FibreKg As Double, ConcreteM3 As Double
End Class

Public Module Ex1Evaluator
    Public Const SteelDensity As Double = 7850.0      'kg/m3
    Public Const StirrupCover As Double = 45.0        'mm to stirrup centreline (ACI 20.6.1.3.1: 40 + half of 10)
    Public Const HookLen As Double = 12.0             'x tie diameter, per hook [ASSUMPTION]

    Private Function KgPerM(areaMm2 As Double) As Double
        Return areaMm2 * 1.0E-6 * SteelDensity
    End Function

    Public Function Evaluate(spec As FrameSpec, d As Ex1Design, tp As TbdyParams, dd3 As TbdyParams, prices As PriceSet) As Ex1Result
        Dim r As New Ex1Result
        Dim defs As New Dictionary(Of String, RcSectionDef)
        For Each kv In d.Columns : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        For Each kv In d.Beams : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        Dim fa = RcCombos.Analyze(spec, defs, tp)
        r.Analysis = fa
        Dim m = fa.Gross.Model

        'member designs per group
        Dim colShear As New Dictionary(Of String, ColumnShearResult)
        For Each g In fa.Gross.ColumnGroups
            Dim c = d.Columns(g)
            r.ColRes(g) = ColumnDesign.CheckGroup(fa, g, c.B, c.H, c.Fc, c.Rho, c.BarDia)
            colShear(g) = ColumnShear.Design(fa, r.ColRes(g))
        Next
        Dim beamShr As New Dictionary(Of String, BeamShearResult)
        For Each g In fa.Gross.BeamGroups
            Dim b = d.Beams(g)
            r.BeamRes(g) = BeamDesign.DesignGroup(fa, g, b.B, b.H, b.Fc, b.Fibre)
            Dim fctk As Double = 0.0, fftuk As Double = 0.0
            If b.Fibre IsNot Nothing AndAlso b.Fibre.Params IsNot Nothing Then
                fctk = SfrcBeam.FctkMin(b.Fc) : fftuk = SfrcBeam.FFtukShear(b.Fibre.Params)
            End If
            beamShr(g) = BeamShear.DesignGroup(fa, r.BeamRes(g), b.Fibre, fctk, fftuk)
        Next
        r.Drift = Checks.Drift(fa, spec, tp, dd3)
        Dim joints = Checks.StrongColumn(fa, spec, r.ColRes, r.BeamRes, Nothing)
        Dim jshear = JointShear.Check(fa, spec, r.ColRes, colShear, r.BeamRes)

        'cost: members one by one
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            Dim cl As Double = (e.Length - e.OffsetI - e.OffsetJ)          'm clear length
            If Math.Abs(e.Axes(0, 2)) > 0.9 Then
                Dim c = d.Columns(e.Group), cr = r.ColRes(e.Group), cs = colShear(e.Group)
                Dim vol As Double = c.B * c.H * 1.0E-6 * cl
                Dim form As Double = 2.0 * (c.B + c.H) * 0.001 * cl
                Dim longKg As Double = KgPerM(cr.AsTotal) * e.Length          'bars over the centre-to-centre height
                Dim nConf As Double = 2.0 * cs.Lk / cs.SConf, nMid As Double = Math.Max(cl * 1000.0 - 2.0 * cs.Lk, 0.0) / cs.SMid
                Dim tieKg As Double = (nConf + nMid + 1.0) * cs.TieLenPerSet * 0.001 * KgPerM(Math.PI * ColumnShear.TieDia ^ 2 / 4.0)
                Dim cb = Cost.Member(prices, c.Fc, vol, form, tieKg, longKg, 0.0, 0.0)
                r.Cost.Concrete += cb.Concrete : r.Cost.Rebar += cb.Rebar : r.Cost.Formwork += cb.Formwork
                r.CostColumns += cb.Total : r.SteelKg += longKg + tieKg : r.ConcreteM3 += vol
            Else
                Dim b = d.Beams(e.Group), br = r.BeamRes(e.Group), bs = beamShr(e.Group)
                Dim vol As Double = b.B * b.H * 1.0E-6 * cl
                Dim form As Double = (b.B + 2.0 * Math.Max(b.H - 1000.0 * spec.SlabThickness, 0.0)) * 0.001 * cl
                Dim longKg As Double = KgPerM(br.AsTop + br.AsBot) * e.Length
                Dim lconf As Double = Math.Min(2.0 * 2.0 * b.H, cl * 1000.0)       'two confinement zones of 2h
                Dim lmid As Double = Math.Max(cl * 1000.0 - lconf, 0.0)
                Dim setLen As Double = (2.0 * (b.B - 2.0 * StirrupCover) + 2.0 * (b.H - 2.0 * StirrupCover) + 2.0 * HookLen * BeamShear.StirrupDia) * 0.001
                Dim stirKg As Double = (lconf / bs.SConf + lmid / bs.SMid + 1.0) * setLen * KgPerM(Math.PI * BeamShear.StirrupDia ^ 2 / 4.0)
                Dim dose As Double = If(b.Fibre IsNot Nothing AndAlso b.Fibre.Params IsNot Nothing, b.Fibre.DoseKgM3, 0.0)
                Dim cb = Cost.Member(prices, b.Fc, vol, form, stirKg, longKg, dose, vol)
                r.Cost.Concrete += cb.Concrete : r.Cost.Rebar += cb.Rebar : r.Cost.Formwork += cb.Formwork : r.Cost.Fibre += cb.Fibre
                r.CostBeams += cb.Total : r.SteelKg += longKg + stirKg : r.ConcreteM3 += vol : r.FibreKg += dose * vol
            End If
        Next

        'constraints: group violation = max over members of max(0, g); T = sum over constraint types
        Dim viol As New List(Of IEnumerable(Of Double))
        Dim names As New List(Of String)
        Dim add = Sub(nm As String, vals As IEnumerable(Of Double))
                      names.Add(nm) : viol.Add(vals)
                  End Sub
        Dim cList = r.ColRes.Values
        add("A1 col min dim", cList.Select(Function(x) x.G_MinDim))
        add("A2 col axial Nd/(0.4 fck Ac)", cList.Select(Function(x) x.G_Axial))
        add("A3 col rho >= 1 %", cList.Select(Function(x) x.G_RhoLow))
        add("A3 col rho <= 4 %", cList.Select(Function(x) x.G_RhoHigh))
        add("P-M biaxial D/C", cList.Select(Function(x) x.DcrMax - 1.0))
        add("C1c col shear crush (7.7)", colShear.Values.Select(Function(x) Math.Max(x.G_Crush(0), x.G_Crush(1))))
        add("C1a col tie spacing >= 50", colShear.Values.Select(Function(x) 50.0 / Math.Max(x.SConf, 1.0) - 1.0 + If(x.SConf <= 50.0 AndAlso x.Governing <> "ok", 1.0, 0.0)))
        Dim bList = r.BeamRes.Values
        add("B1 beam bw >= 250", bList.Select(Function(x) 250.0 / x.B - 1.0))
        add("B2 beam h >= 300", bList.Select(Function(x) 300.0 / x.H - 1.0))
        add("B2 beam h <= 3.5 bw", bList.Select(Function(x) x.H / (3.5 * x.B) - 1.0))
        add("B1 beam bw <= h + bc", bList.Select(Function(x) x.B / (x.H + d.Columns.Values.Min(Function(c) Math.Min(c.B, c.H))) - 1.0))
        add("B7 beam rho <= 2 %", bList.Select(Function(x) Math.Max(x.AsTop, x.AsBot) / x.AsMaxRho - 1.0))
        add("B7 beam eps_t >= 0.004 / section capacity", bList.Select(Function(x) If(x.EpsTOk, -1.0, 1.0)))
        add("C1e beam shear crush (7.10)", beamShr.Values.Select(Function(x) x.G_Crush))
        add("C1d beam stirrup spacing >= 50", beamShr.Values.Select(Function(x) If(x.Governing = "stirrup spacing < 50 mm", 1.0, -1.0)))
        add("A5 strong column", joints.Where(Function(x) Not x.Exempt).Select(Function(x) x.G))
        add("C1f joint shear", jshear.Select(Function(x) x.G))
        add("C2 drift", New Double() {r.Drift.G})
        For i As Integer = 0 To viol.Count - 1
            r.Violations.Add(Tuple.Create(names(i), Penalty.GroupViolation(viol(i))))
        Next
        r.T = Penalty.Total(viol)
        r.Fpen = Penalty.Apply(r.Cost.Total, r.T)
        Return r
    End Function
End Module
