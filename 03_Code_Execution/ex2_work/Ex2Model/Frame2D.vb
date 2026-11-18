Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports FemCore
Imports FemRcModel

'PCR-14 (EX2_SPEC v2 §1, §3): planar frame of Ex2, geometry and loading adapted from Duysak, Nigdeli and Bekdaş (2023):
'3 bays 5 - 6 - 5 m, 3 storeys of 3.3 m, fixed base, G = 30 kN/m and Q = 20 kN/m on every beam (roof Q taken as L, spec §3), member self weight added (decision Q3).
'Units: kN, m inside the FEM; section inputs mm / MPa. Out-of-plane DOF (U2, R1, R3) restrained at every joint (spec §5.1: out-of-plane braced assumption).
'No rigid end zones (spec §3 / §10 V1: self weight and volumes over the axis length, "rijit bölge yok"); see EX2_CODE_LOG.md AÇIK-4.
Public Class FrameSpec2D
    Public Bays As Double() = {5.0, 6.0, 5.0}      'm, Şekil 1 (PDF s.4): 5 - 6 - 5 m
    Public NStory As Integer = 3
    Public StoryH As Double = 3.3                  'm (PDF s.4)
    Public DeadLine As Double = 30.0               'kN/m on every beam (PDF s.4)
    Public LiveLine As Double = 20.0               'kN/m on every beam, roof included (PDF s.4; roof = L, spec §3)
    Public ConcreteUnitWeight As Double = 25.0     'kN/m3 (spec §2, project value)
    Public Groups As Integer = 9                   '9: main optimisation (Q1 B); 5: V2 force comparison with the source Table 1 sections (Q1 A)

    Public ReadOnly Property NCols As Integer
        Get
            Return Bays.Length + 1
        End Get
    End Property

    Public Function XOf(i As Integer) As Double
        Dim x As Double = 0.0
        For k As Integer = 0 To i - 1 : x += Bays(k) : Next
        Return x
    End Function
End Class

Public Class FrameModel2D
    Public Model As New FemModel
    Public Spec As FrameSpec2D
    Public ColumnGroups As New List(Of String)
    Public BeamGroups As New List(Of String)
    Public NCol As Integer                       'number of column elements (elements are NOT ordered columns first: Şekil 2 numbering)
    'Şekil 2: node number = level * 4 + i + 1 (level 0 = base); element numbers storey k: columns 7(k-1)+1..+4, beams 7(k-1)+5..+7
    Public Function NodeAt(i As Integer, level As Integer) As Integer
        Return level * Spec.NCols + i
    End Function
    Public Function IsColumn(ei As Integer) As Boolean
        Return Math.Abs(Model.Elements(ei).Axes(0, 2)) > 0.9
    End Function
End Class

Public Module RcFrameBuilder2D
    'Group names. Columns: edge (axes 1 and 4) / interior (axes 2 and 3); 9-group mode per storey, 5-group mode over the full height. Beams: one group per storey (both modes).
    Public Function ColumnGroupName(spec As FrameSpec2D, i As Integer, story As Integer) As String
        Dim pos As String = If(i = 0 OrElse i = spec.NCols - 1, "C_Edge", "C_Inter")
        Return If(spec.Groups = 9, pos & "_S" & story, pos)
    End Function

    Public Function BeamGroupName(spec As FrameSpec2D, story As Integer) As String
        Return "B_S" & story
    End Function

    'Local axes (same rule as the Ö1 builder / ETABS): vertical member local 2 = global X (in-plane, section depth H), local 3 = global Y;
    'horizontal member local 2 = global Z (section depth H vertical), local 3 = X x Z = -Y. Spec §1.1: column h = in-plane dimension, b = out-of-plane.
    Private Sub SetAxes(e As FemElement, dx As Double, dy As Double, dz As Double)
        Dim L As Double = Math.Sqrt(dx * dx + dy * dy + dz * dz)
        Dim ax1() As Double = {dx / L, dy / L, dz / L}
        Dim ax2() As Double
        If Math.Abs(ax1(0)) < 0.000001 AndAlso Math.Abs(ax1(1)) < 0.000001 Then ax2 = {1.0, 0.0, 0.0} Else ax2 = {0.0, 0.0, 1.0}
        Dim ax3() As Double = {ax1(1) * ax2(2) - ax1(2) * ax2(1), ax1(2) * ax2(0) - ax1(0) * ax2(2), ax1(0) * ax2(1) - ax1(1) * ax2(0)}
        ax2 = {ax3(1) * ax1(2) - ax3(2) * ax1(1), ax3(2) * ax1(0) - ax3(0) * ax1(2), ax3(0) * ax1(1) - ax3(1) * ax1(0)}
        For c As Integer = 0 To 2 : e.Axes(0, c) = ax1(c) : e.Axes(1, c) = ax2(c) : e.Axes(2, c) = ax3(c) : Next
        e.Length = L
    End Sub

    'sections: group name -> FemSection. Patterns: SW (self weight, multiplier 1; ETABS: self weight multiplier only in this pattern), G (30 kN/m), Q (20 kN/m).
    Public Function Build(spec As FrameSpec2D, sections As Dictionary(Of String, FemSection)) As FrameModel2D
        Dim fm As New FrameModel2D With {.Spec = spec}
        Dim m = fm.Model
        For level As Integer = 0 To spec.NStory
            For i As Integer = 0 To spec.NCols - 1
                Dim n As New FemNode With {.Name = "N" & (level * spec.NCols + i + 1), .X = spec.XOf(i), .Y = 0.0, .Z = level * spec.StoryH}
                If level = 0 Then
                    For r As Integer = 0 To 5 : n.Restraint(r) = True : Next
                Else
                    n.Restraint(1) = True : n.Restraint(3) = True : n.Restraint(5) = True     'planar frame: U2, R1, R3 restrained
                End If
                m.Nodes.Add(n)
            Next
        Next
        m.DiaphragmCount = 0
        For Each s In sections.Values
            If Not m.Sections.Contains(s) Then m.Sections.Add(s)
        Next
        Dim num As Integer = 0
        For k As Integer = 1 To spec.NStory
            For i As Integer = 0 To spec.NCols - 1
                num += 1
                Dim g As String = ColumnGroupName(spec, i, k)
                Dim e As New FemElement With {.Name = "E" & num, .NI = fm.NodeAt(i, k - 1), .NJ = fm.NodeAt(i, k), .Section = sections(g), .Group = g, .Story = k}
                SetAxes(e, 0, 0, spec.StoryH)
                m.Elements.Add(e)
                fm.NCol += 1
                If Not fm.ColumnGroups.Contains(g) Then fm.ColumnGroups.Add(g)
            Next
            For i As Integer = 0 To spec.Bays.Length - 1
                num += 1
                Dim g As String = BeamGroupName(spec, k)
                Dim e As New FemElement With {.Name = "E" & num, .NI = fm.NodeAt(i, k), .NJ = fm.NodeAt(i + 1, k), .Section = sections(g), .Group = g, .Story = k}
                SetAxes(e, spec.Bays(i), 0, 0)
                m.Elements.Add(e)
                If Not fm.BeamGroups.Contains(g) Then fm.BeamGroups.Add(g)
            Next
        Next
        For Each e In m.Elements
            e.OffsetI = 0.0 : e.OffsetJ = 0.0 : e.RigidZone = 0.0
        Next
        Dim pSW As New FemPattern With {.Name = "SW", .SelfWeightMultiplier = 1.0}
        Dim pG As New FemPattern With {.Name = "G", .SelfWeightMultiplier = 0.0}
        Dim pQ As New FemPattern With {.Name = "Q", .SelfWeightMultiplier = 0.0}
        m.Patterns.Add(pSW) : m.Patterns.Add(pG) : m.Patterns.Add(pQ)
        For ei As Integer = 0 To m.Elements.Count - 1
            If fm.IsColumn(ei) Then Continue For
            pG.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 1.0, .V1 = spec.DeadLine, .V2 = spec.DeadLine})
            pQ.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 1.0, .V1 = spec.LiveLine, .V2 = spec.LiveLine})
        Next
        Return fm
    End Function
End Module

'One load combination of the planar frame: factors of the patterns SW, G, Q. D = SW + G (spec §3).
Public Class Combo2D
    Public Name As String
    Public FSW, FG, FQ As Double
    Public Main As Boolean                       'True: design envelope (1.4D, 1.2D+1.6L); False: V2-b information variants
    Public QOn As List(Of Integer)               'ACI 6.4.2 live-load pattern (deterministic tool only): element indices of the loaded beams; factor FQP on their Q_E patterns
    Public FQP As Double
End Class

Public Class Analysis2D
    Public Frame As FrameModel2D
    Public Fs As FemStatic
    Public R As StaticResults
    Public Combos As New List(Of Combo2D)
    Public Face As New List(Of Double()())            '[combo][element](12) face forces (local axes, FemStatic convention)
    Public StationX As Double()()                       '[element] station distances from the start of the clear span (m)
    Public Station As New List(Of Double()()())         '[combo][element][station](6): N, V2, V3, T, M2, M3 (M3 sagging positive for beams)
    Public SustainedAxial As New List(Of Double())      '[combo][element]: axial force of the dead part of the combination at end I (kN, compression +)
    Public WithVariants As Boolean, Patterned As Boolean, Braced As Boolean     'flags of Analyze (Braced: sway prevented, PCR-22; faces only, no stations)

    'sum of the vertical reactions (kN, upward +) of a pattern
    Public Function VerticalReaction(pattern As String) As Double
        Dim p As Integer = Frame.Model.PatternIndex(pattern)
        Dim s As Double = 0.0
        For i As Integer = 0 To Frame.Model.Nodes.Count - 1
            s += R.Reaction(p)(i * 6 + 2)
        Next
        Return s
    End Function

    'pattern factors of a combination (SW, G, Q and the per-beam live patterns Q_E<n> of the ACI 6.4.2 tool)
    Public Function Factors(c As Combo2D) As Double()
        Dim m = Frame.Model
        Dim fac(m.Patterns.Count - 1) As Double
        fac(m.PatternIndex("SW")) = c.FSW : fac(m.PatternIndex("G")) = c.FG : fac(m.PatternIndex("Q")) = c.FQ
        If c.QOn IsNot Nothing Then
            For Each ei In c.QOn : fac(m.PatternIndex("Q_E" & ei)) += c.FQP : Next
        End If
        Return fac
    End Function

    Public Function VerticalReaction(c As Combo2D) As Double
        Return c.FSW * VerticalReaction("SW") + c.FG * VerticalReaction("G") + c.FQ * VerticalReaction("Q")
    End Function
End Class

'PCR-14: combinations. Main envelope (decision Q2): U1 = 1.4D (ACI 5.3.1a), U2 = 1.2D + 1.6L (5.3.1b), D = SW + G. No live load patterning (decision Q16 / E3).
'Variants (V2-b information table only, decision Q2): 1.0G + 1.0Q and 1.4G + 1.6Q, each with and without self weight.
Public Module RcCombos2D
    Public Function MainCombos() As List(Of Combo2D)
        Return New List(Of Combo2D) From {
            New Combo2D With {.Name = "U1 1.4D", .FSW = 1.4, .FG = 1.4, .FQ = 0.0, .Main = True},
            New Combo2D With {.Name = "U2 1.2D+1.6L", .FSW = 1.2, .FG = 1.2, .FQ = 1.6, .Main = True}}
    End Function

    Public Function VariantCombos() As List(Of Combo2D)
        Return New List(Of Combo2D) From {
            New Combo2D With {.Name = "V 1.0G+1.0Q +SW", .FSW = 1.0, .FG = 1.0, .FQ = 1.0},
            New Combo2D With {.Name = "V 1.0G+1.0Q noSW", .FSW = 0.0, .FG = 1.0, .FQ = 1.0},
            New Combo2D With {.Name = "V 1.4G+1.6Q +SW", .FSW = 1.4, .FG = 1.4, .FQ = 1.6},
            New Combo2D With {.Name = "V 1.4G+1.6Q noSW", .FSW = 0.0, .FG = 1.4, .FQ = 1.6}}
    End Function

    Public Function MakeSections(spec As FrameSpec2D, defs As Dictionary(Of String, RcSectionDef)) As Dictionary(Of String, FemSection)
        Dim d As New Dictionary(Of String, FemSection)
        For Each kv In defs
            d(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, 1.0)      'gross section (spec §3: fa.Gross)
        Next
        Return d
    End Function

    'ACI 6.4.1 / 6.4.2 live-load patterns (deterministic tool only, NOT an optimisation constraint: decision [K b]); 1.2D + 1.6 L_pattern.
    'Per storey (6.4.1: L only on the level under consideration) and on all storeys: bay sets {1,3}, {2} (6.4.2(a) span + alternate spans), {1,2}, {2,3}
    '(6.4.2(b) adjacent spans), {1}, {3} (exterior support). Bays 0-based.
    Public Function PatternCombos(fm As FrameModel2D) As List(Of Combo2D)
        Dim sets()() As Integer = {New Integer() {0, 2}, New Integer() {1}, New Integer() {0, 1}, New Integer() {1, 2}, New Integer() {0}, New Integer() {2}}
        Dim spec = fm.Spec, m = fm.Model
        Dim res As New List(Of Combo2D)
        For lev As Integer = 0 To spec.NStory
            For Each st In sets
                Dim onB As New List(Of Integer)
                For ei As Integer = 0 To m.Elements.Count - 1
                    If fm.IsColumn(ei) Then Continue For
                    Dim e = m.Elements(ei)
                    If lev > 0 AndAlso e.Story <> lev Then Continue For
                    Dim bay As Integer = Array.IndexOf(Enumerable.Range(0, spec.Bays.Length).Select(Function(i) spec.XOf(i)).ToArray(), m.Nodes(e.NI).X)
                    If st.Contains(bay) Then onB.Add(ei)
                Next
                res.Add(New Combo2D With {.Name = "P " & If(lev = 0, "all", "S" & lev) & " bays " & String.Join("", st.Select(Function(b) (b + 1).ToString())), .FSW = 1.2, .FG = 1.2, .FQ = 0.0, .FQP = 1.6, .QOn = onB, .Main = True})
            Next
        Next
        Return res
    End Function

    'Linear static analysis (gross sections) and the combination forces. withVariants adds the V2-b combinations (not used by the optimisation);
    'patterned adds one live pattern per beam (Q_E<n>) and the ACI 6.4.2 pattern combinations to the design envelope (deterministic tool only).
    'braced (PCR-22, 2026-11-16): the same model with the lateral translation U1 restrained at every floor node (sway prevented); used only for the
    'restraint reactions -> net storey sway forces (Stability2D.Assess); face forces only (no station forces).
    Public Function Analyze(spec As FrameSpec2D, defs As Dictionary(Of String, RcSectionDef), Optional withVariants As Boolean = False, Optional patterned As Boolean = False, Optional braced As Boolean = False) As Analysis2D
        Dim a As New Analysis2D With {.WithVariants = withVariants, .Patterned = patterned, .Braced = braced}
        a.Frame = RcFrameBuilder2D.Build(spec, MakeSections(spec, defs))
        Dim m = a.Frame.Model
        If braced Then
            For lev As Integer = 1 To spec.NStory
                For i As Integer = 0 To spec.NCols - 1 : m.Nodes(a.Frame.NodeAt(i, lev)).Restraint(0) = True : Next
            Next
        End If
        If patterned Then
            For ei As Integer = 0 To m.Elements.Count - 1
                If a.Frame.IsColumn(ei) Then Continue For
                Dim pe As New FemPattern With {.Name = "Q_E" & ei, .SelfWeightMultiplier = 0.0}
                pe.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 1.0, .V1 = spec.LiveLine, .V2 = spec.LiveLine})
                m.Patterns.Add(pe)
            Next
        End If
        a.Fs = New FemStatic(m)
        a.R = a.Fs.Solve()
        If a.R.Failed Then Throw New InvalidOperationException("Ex2 frame: " & a.R.Message)
        a.Combos.AddRange(MainCombos())
        If withVariants Then a.Combos.AddRange(VariantCombos())
        If patterned Then a.Combos.AddRange(PatternCombos(a.Frame))
        Dim ne As Integer = m.Elements.Count
        Dim np As Integer = m.Patterns.Count
        Dim pIdx() As Integer = {m.PatternIndex("SW"), m.PatternIndex("G"), m.PatternIndex("Q")}
        'station forces per pattern (computed once), then combined
        ReDim a.StationX(ne - 1)
        Dim patSt(np - 1)()()() As Double
        For q As Integer = 0 To np - 1 : patSt(q) = New Double(ne - 1)()() {} : Next
        For ei As Integer = 0 To ne - 1
            If braced Then a.StationX(ei) = New Double() {} : Continue For
            a.StationX(ei) = a.Fs.StationLocations(ei)
            For q As Integer = 0 To np - 1
                patSt(q)(ei) = a.Fs.StationForces(a.R, q, ei, a.StationX(ei))
            Next
        Next
        For Each c In a.Combos
            Dim fac = a.Factors(c)
            Dim face(ne - 1)() As Double, st(ne - 1)()() As Double, sus(ne - 1) As Double
            For ei As Integer = 0 To ne - 1
                face(ei) = New Double(11) {}
                For q As Integer = 0 To np - 1
                    If fac(q) = 0.0 Then Continue For
                    Dim f = a.R.FaceForce(q)(ei)
                    For k As Integer = 0 To 11 : face(ei)(k) += fac(q) * f(k) : Next
                Next
                Dim ns As Integer = a.StationX(ei).Length
                st(ei) = New Double(ns - 1)() {}
                For j As Integer = 0 To ns - 1
                    st(ei)(j) = New Double(5) {}
                    For q As Integer = 0 To np - 1
                        If fac(q) = 0.0 Then Continue For
                        For k As Integer = 0 To 5 : st(ei)(j)(k) += fac(q) * patSt(q)(ei)(j)(k) : Next
                    Next
                Next
                sus(ei) = c.FSW * a.R.FaceForce(pIdx(0))(ei)(0) + c.FG * a.R.FaceForce(pIdx(1))(ei)(0)       'end I, compression +
            Next
            a.Face.Add(face) : a.Station.Add(st) : a.SustainedAxial.Add(sus)
        Next
        Return a
    End Function
End Module
