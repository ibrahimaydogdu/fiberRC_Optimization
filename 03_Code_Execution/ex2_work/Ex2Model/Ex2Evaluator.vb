Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports FemRcModel
Imports RcCore

'PCR-17 (EX2_SPEC v2 §5, §5.4): design -> cost (TL, ÇŞİDB 2026, RcCore.PriceSet = same source as Ö1) and normalised constraints (g <= 0 feasible),
'Ö1 multiplicative penalty F (1 + T)^2 (RcCore.Penalty). No pre-design class (Q9: arm A only).
Public Class Ex2Design
    Public Columns As New Dictionary(Of String, ColumnDef)     'FemRcModel.ColumnDef: B, H (mm; H in-plane), Rho, Fc, BarDia, RhoDerived
    Public Beams As New Dictionary(Of String, BeamDef)         'FemRcModel.BeamDef: B, H, Fc, Fibre (Nothing = plain)
End Class

Public Class Ex2Result
    Public Cost As CostBreakdown
    Public CostBeams, CostColumns As Double
    Public T As Double
    Public Fpen As Double
    Public Violations As New List(Of Tuple(Of String, Double))
    Public ColRes As New Dictionary(Of String, ColumnGravityResult)
    Public BeamRes As New Dictionary(Of String, BeamGravityResult)
    Public BeamShr As New Dictionary(Of String, BeamShearGravityResult)
    Public Analysis As Analysis2D
    Public ConcreteM3, FormworkM2, LongKg, TieKg, FibreKg As Double
    Public ReadOnly Property SteelKg As Double
        Get
            Return LongKg + TieKg
        End Get
    End Property
End Class

Public Module Ex2Evaluator
    Public Const SteelDensity As Double = 7850.0       'kg/m3
    Public Const StirrupCover As Double = 45.0         'mm to stirrup centreline (Ö1)
    Public Const HookLen As Double = 12.0              'x tie diameter, per hook (Ö1 assumption)
    Public Const BarDia As Double = 20.0               'column bar diameter (Ö1)
    Public Const RhoSteps As Integer = 12              'column rho grid 1 .. 4 % in 0.25 % steps (PCR-8, Q7)

    Public Function RhoOfIdx(i As Integer) As Double
        Return 0.01 + 0.0025 * i
    End Function

    Private Function KgPerM(areaMm2 As Double) As Double
        Return areaMm2 * 1.0E-6 * SteelDensity
    End Function

    'PCR-8 (gravity form): smallest grid rho with P-M D/C <= 1 (bisection; D/C non-increasing in rho at fixed demands; no strong-column pass: Chapter 18 not applied)
    Private Function DeriveRho(a As Analysis2D, g As String, c As ColumnDef) As Integer
        Dim ok = Function(i As Integer) ColumnDesignGravity.CheckGroup(a, g, c.B, c.H, c.Fc, RhoOfIdx(i), c.BarDia, False).DcrMax <= 1.0
        If ok(0) Then Return 0
        If Not ok(RhoSteps) Then Return RhoSteps
        Dim lo As Integer = 0, hi As Integer = RhoSteps
        While hi - lo > 1
            Dim mid As Integer = (lo + hi) \ 2
            If ok(mid) Then hi = mid Else lo = mid
        End While
        Return hi
    End Function

    Public Function Evaluate(spec As FrameSpec2D, d As Ex2Design, prices As PriceSet, shear As IBeamShear) As Ex2Result
        If shear Is Nothing Then Throw New ArgumentNullException(NameOf(shear))
        Dim r As New Ex2Result
        Dim defs As New Dictionary(Of String, RcSectionDef)
        For Each kv In d.Columns : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        For Each kv In d.Beams : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        Dim a = RcCombos2D.Analyze(spec, defs)
        r.Analysis = a
        Dim m = a.Frame.Model

        For Each g In a.Frame.BeamGroups
            Dim b = d.Beams(g)
            r.BeamRes(g) = BeamDesignGravity.DesignGroup(a, g, b.B, b.H, b.Fc, b.Fibre)
            r.BeamShr(g) = BeamShearGravity.DesignGroup(a, r.BeamRes(g), b.Fibre, shear)
        Next
        For Each g In a.Frame.ColumnGroups
            Dim c = d.Columns(g)
            If c.RhoDerived Then c.Rho = RhoOfIdx(DeriveRho(a, g, c))
            r.ColRes(g) = ColumnDesignGravity.CheckGroup(a, g, c.B, c.H, c.Fc, c.Rho, c.BarDia)
        Next

        'cost (spec §5.4): concrete b h L and longitudinal bars over the axis length (group-continuous bars, no laps); formwork beam (b + 2h) L, column 2 (b + h) L;
        'stirrups / ties at one spacing over the member (no confinement zones, no joint ties); fibre = dose x beam concrete volume x price
        Dim tieKgPerM As Double = KgPerM(Math.PI * ColumnDesignGravity.TieDia ^ 2 / 4.0)
        Dim stirKgPerM As Double = KgPerM(Math.PI * BeamShearGravity.StirrupDia ^ 2 / 4.0)
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            Dim L As Double = e.Length
            If a.Frame.IsColumn(ei) Then
                Dim c = d.Columns(e.Group), cr = r.ColRes(e.Group)
                Dim vol As Double = c.B * c.H * 1.0E-6 * L
                Dim form As Double = 2.0 * (c.B + c.H) * 0.001 * L
                Dim longKg As Double = KgPerM(cr.AsTotal) * L
                Dim tieKg As Double = (L * 1000.0 / cr.S + 1.0) * cr.TieLenPerSet * 0.001 * tieKgPerM
                Dim cb = Cost.Member(prices, c.Fc, vol, form, tieKg, longKg, 0.0, 0.0)
                r.Cost.Concrete += cb.Concrete : r.Cost.Rebar += cb.Rebar : r.Cost.Formwork += cb.Formwork
                r.CostColumns += cb.Total
                r.ConcreteM3 += vol : r.FormworkM2 += form : r.LongKg += longKg : r.TieKg += tieKg
            Else
                Dim b = d.Beams(e.Group), br = r.BeamRes(e.Group), bs = r.BeamShr(e.Group)
                Dim vol As Double = b.B * b.H * 1.0E-6 * L
                Dim form As Double = (b.B + 2.0 * b.H) * 0.001 * L
                Dim longKg As Double = KgPerM(br.AsTop + br.AsBot) * L
                Dim setLen As Double = (2.0 * (b.B - 2.0 * StirrupCover) + 2.0 * (b.H - 2.0 * StirrupCover) + 2.0 * HookLen * BeamShearGravity.StirrupDia) * 0.001
                Dim stirKg As Double = (L * 1000.0 / bs.S + 1.0) * setLen * stirKgPerM
                Dim dose As Double = If(b.Fibre IsNot Nothing AndAlso b.Fibre.Params IsNot Nothing, b.Fibre.DoseKgM3, 0.0)
                If Double.IsInfinity(longKg) Then longKg = 0.0                       'section too small: steel not priced, the design is infeasible (g3)
                Dim cb = Cost.Member(prices, b.Fc, vol, form, stirKg, longKg, dose, vol)
                r.Cost.Concrete += cb.Concrete : r.Cost.Rebar += cb.Rebar : r.Cost.Formwork += cb.Formwork : r.Cost.Fibre += cb.Fibre
                r.CostBeams += cb.Total
                r.ConcreteM3 += vol : r.FormworkM2 += form : r.LongKg += longKg : r.TieKg += stirKg : r.FibreKg += dose * vol
            End If
        Next

        'constraints: violation of a type = max over groups of max(0, g); T = sum over the types (Ö1 Penalty)
        Dim viol As New List(Of IEnumerable(Of Double)), names As New List(Of String)
        Dim add = Sub(nm As String, vals As IEnumerable(Of Double))
                      names.Add(nm) : viol.Add(vals.ToList())
                  End Sub
        Dim cl = r.ColRes.Values
        add("g1 col P-M D/C (ACI 22.4, 6.6.4.5)", cl.Select(Function(x) x.DcrMax - 1.0))
        add("g2 col rho >= 1 % (ACI 10.6.1.1)", cl.Select(Function(x) x.G_RhoLow))
        add("g2 col rho <= 4 % (grid bound)", cl.Select(Function(x) x.G_RhoHigh))
        add("col shear Vs <= 0.66 sqrt(fc) b d (ACI 22.5.1.2)", cl.Select(Function(x) x.G_Crush))
        add("col tie spacing >= 50", cl.Select(Function(x) 50.0 / Math.Max(x.S, 1.0) - 1.0))
        Dim bl = r.BeamRes.Values
        add("g3 beam section capacity (As finite)", bl.Select(Function(x) If(x.SectionOk, -1.0, 1.0)))
        add("g3 beam eps_t >= 0.004 (ACI 9.3.3.1)", bl.Select(Function(x) If(x.EpsTOk, -1.0, 1.0)))
        add("g4 beam shear Vs <= 0.66 sqrt(fc) bw d (ACI 22.5.1.2)", r.BeamShr.Values.Select(Function(x) x.G_Crush))
        add("g4 beam stirrup spacing >= 50", r.BeamShr.Values.Select(Function(x) 50.0 / Math.Max(x.S, 1.0) - 1.0))
        add("g5 beam h >= l/18.5 or l/21 (ACI Table 9.3.1.1)", bl.Select(Function(x) x.G_Depth))
        For i As Integer = 0 To viol.Count - 1
            r.Violations.Add(Tuple.Create(names(i), Penalty.GroupViolation(viol(i))))
        Next
        r.T = Penalty.Total(viol)
        r.Fpen = Penalty.Apply(r.Cost.Total, r.T)
        Return r
    End Function
End Module
