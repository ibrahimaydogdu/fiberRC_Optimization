Imports System
Imports System.Collections.Generic
Imports FemCore

'3D reinforced-concrete frame builder for the FEM solver (EX1_SPEC v3). Units: kN, m. Section inputs in mm / MPa.
'Sections: Ec = 4700 sqrt(fc') [assumption, ACI 318M-14 19.2.2.1.b], G = E / 2.4 (nu = 0.2).
'Cracked stiffness (TBDY-2018 Table 4.2): applied through FemSection.Modifier (I22, I33) only when asked (earthquake combinations).
Public Class RcSectionDef
    Public B, H As Double      'mm; B = width (local axis 2 direction), H = depth along local axis 3 ... see Make
    Public Fc As Double        'MPa
End Class

Public Class FrameSpec
    Public NxBays As Integer = 3, NyBays As Integer = 2, NStory As Integer = 4
    Public Lx As Double = 5.0, Ly As Double = 5.0, StoryH As Double = 3.0
    Public SlabThickness As Double = 0.15
    Public DeadFloor As Double = 3.12        'kN/m2, normal floor (slab self weight excluded)
    Public DeadRoof As Double = 2.5          'kN/m2, roof (slab self weight excluded)
    Public WallLine As Double = 5.6          'kN/m on perimeter beams of storeys 1..NStory-1
    Public LiveFloor As Double = 2.0         'kN/m2
    Public Snow As Double = 0.75             'kN/m2 on the roof
    Public ConcreteUnitWeight As Double = 25.0
End Class

Public Class FrameModel
    Public Model As New FemModel
    Public Spec As FrameSpec
    Public ColumnGroups As New List(Of String)
    Public BeamGroups As New List(Of String)
    Public Const G As Double = 9.80665
    'node index of grid point (i, j, k)  i: 0..Nx, j: 0..Ny, k: 0..NStory (0 = base)
    Public Function NodeAt(i As Integer, j As Integer, k As Integer) As Integer
        Return (k * (Spec.NyBays + 1) + j) * (Spec.NxBays + 1) + i
    End Function
End Class

Public Module RcFrameBuilder

    'Rectangular RC section. B (m) is along local axis 2, H (m) along local axis 3 for columns; I33 bends about axis 3.
    'cracked: stiffness factor for I22/I33 (1 = gross). Torsion J: Roark-type approximation.
    Public Function MakeSection(name As String, bMm As Double, hMm As Double, fcMpa As Double, unitWeight As Double, Optional crackFlex As Double = 1.0) As FemSection
        Dim b As Double = bMm / 1000.0, h As Double = hMm / 1000.0
        Dim E As Double = 4700.0 * Math.Sqrt(fcMpa) * 1000.0      'kN/m2
        Dim s As New FemSection With {.Name = name, .MaterialName = "C" & fcMpa.ToString("0"), .E = E, .G = E / 2.4, .A = b * h,
                                      .UnitWeight = unitWeight, .UnitMass = unitWeight / FrameModel.G, .Depth = h, .Width = b}
        s.As2 = 5.0 / 6.0 * b * h : s.As3 = 5.0 / 6.0 * b * h
        s.I33 = b * h ^ 3 / 12.0 : s.I22 = h * b ^ 3 / 12.0
        Dim a As Double = Math.Max(b, h), c As Double = Math.Min(b, h)
        s.J = a * c ^ 3 * (1.0 / 3.0 - 0.21 * c / a * (1.0 - c ^ 4 / (12.0 * a ^ 4)))
        s.Modifier = {1.0, 1.0, 1.0, 1.0, crackFlex, crackFlex, 1.0, 1.0}
        Return s
    End Function

    'ETABS-like local axes: vertical member local 2 = global X, others local 2 = global Z projected
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

    'Column group: C-corner / C-edge / C-inter  x  storey group (S12 = storeys 1-2, S34 = storeys 3-4)
    Public Function ColumnGroupName(spec As FrameSpec, i As Integer, j As Integer, story As Integer) As String
        Dim ex As Boolean = (i = 0 OrElse i = spec.NxBays), ey As Boolean = (j = 0 OrElse j = spec.NyBays)
        Dim pos As String = If(ex AndAlso ey, "Corner", If(ex OrElse ey, "Edge", "Inter"))
        Return "C" & pos & If(story <= spec.NStory / 2, "_S12", "_S34")
    End Function

    Public Function BeamGroupName(spec As FrameSpec, dirX As Boolean, story As Integer) As String
        Return "B" & If(dirX, "x", "y") & If(story <= spec.NStory / 2, "_S12", "_S34")
    End Function

    'sections: group name -> FemSection (columns and beams). Builds nodes (fixed base), elements (columns then beams) and patterns G, Q, S.
    'G: self weight (x1) + slab dead (triangular tributary on beams) + perimeter wall. Q: live on floors 1..N-1 (roof live replaced by snow). S: snow on roof.
    Public Function Build(spec As FrameSpec, sections As Dictionary(Of String, FemSection)) As FrameModel
        Dim fm As New FrameModel With {.Spec = spec}
        Dim m = fm.Model
        For k As Integer = 0 To spec.NStory
            For j As Integer = 0 To spec.NyBays
                For i As Integer = 0 To spec.NxBays
                    Dim n As New FemNode With {.Name = "N" & i & j & k, .X = i * spec.Lx, .Y = j * spec.Ly, .Z = k * spec.StoryH}
                    If k = 0 Then For r As Integer = 0 To 5 : n.Restraint(r) = True : Next
                    If k > 0 Then n.Diaphragm = k - 1
                    m.Nodes.Add(n)
                Next
            Next
        Next
        m.DiaphragmCount = spec.NStory
        For Each s In sections.Values
            If Not m.Sections.Contains(s) Then m.Sections.Add(s)
        Next
        'columns
        For k As Integer = 1 To spec.NStory
            For j As Integer = 0 To spec.NyBays
                For i As Integer = 0 To spec.NxBays
                    Dim g As String = ColumnGroupName(spec, i, j, k)
                    Dim e As New FemElement With {.Name = "C" & i & j & k, .NI = fm.NodeAt(i, j, k - 1), .NJ = fm.NodeAt(i, j, k), .Section = sections(g), .Group = g, .Story = k}
                    SetAxes(e, 0, 0, spec.StoryH)
                    m.Elements.Add(e)
                    If Not fm.ColumnGroups.Contains(g) Then fm.ColumnGroups.Add(g)
                Next
            Next
        Next
        Dim nCol As Integer = m.Elements.Count
        'beams: x-direction then y-direction on every floor 1..NStory
        For k As Integer = 1 To spec.NStory
            For j As Integer = 0 To spec.NyBays
                For i As Integer = 0 To spec.NxBays - 1
                    AddBeam(fm, sections, "Bx" & i & j & k, fm.NodeAt(i, j, k), fm.NodeAt(i + 1, j, k), True, k)
                Next
            Next
            For i As Integer = 0 To spec.NxBays
                For j As Integer = 0 To spec.NyBays - 1
                    AddBeam(fm, sections, "By" & i & j & k, fm.NodeAt(i, j, k), fm.NodeAt(i, j + 1, k), False, k)
                Next
            Next
        Next
        'patterns
        Dim pG As New FemPattern With {.Name = "G", .SelfWeightMultiplier = 1.0}
        Dim pQ As New FemPattern With {.Name = "Q", .SelfWeightMultiplier = 0.0}
        Dim pS As New FemPattern With {.Name = "S", .SelfWeightMultiplier = 0.0}
        m.Patterns.Add(pG) : m.Patterns.Add(pQ) : m.Patterns.Add(pS)
        For ei As Integer = nCol To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            Dim nI = m.Nodes(e.NI), nJ = m.Nodes(e.NJ)
            Dim dirX As Boolean = (Math.Abs(nJ.X - nI.X) > 0.001)
            Dim k As Integer = e.Story
            Dim roof As Boolean = (k = spec.NStory)
            Dim idx As Integer = If(dirX, CInt(Math.Round(nI.Y / spec.Ly)), CInt(Math.Round(nI.X / spec.Lx)))
            Dim last As Integer = If(dirX, spec.NyBays, spec.NxBays)
            Dim sides As Integer = If(idx = 0 OrElse idx = last, 1, 2)       'adjacent slab panels
            Dim wDead As Double = (If(roof, spec.DeadRoof, spec.DeadFloor) + spec.SlabThickness * spec.ConcreteUnitWeight)
            Dim wLive As Double = If(roof, 0.0, spec.LiveFloor)
            Dim wSnow As Double = If(roof, spec.Snow, 0.0)
            AddTriangular(pG, ei, wDead * sides, e.Length, dirX, spec)
            If wLive > 0 Then AddTriangular(pQ, ei, wLive * sides, e.Length, dirX, spec)
            If wSnow > 0 Then AddTriangular(pS, ei, wSnow * sides, e.Length, dirX, spec)
            If sides = 1 AndAlso Not roof Then
                pG.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 1.0, .V1 = spec.WallLine, .V2 = spec.WallLine})
            End If
        Next
        Return fm
    End Function

    Private Sub AddBeam(fm As FrameModel, sections As Dictionary(Of String, FemSection), name As String, ni As Integer, nj As Integer, dirX As Boolean, story As Integer)
        Dim g As String = BeamGroupName(fm.Spec, dirX, story)
        Dim a = fm.Model.Nodes(ni), b = fm.Model.Nodes(nj)
        Dim e As New FemElement With {.Name = name, .NI = ni, .NJ = nj, .Section = sections(g), .Group = g, .Story = story}
        SetAxes(e, b.X - a.X, b.Y - a.Y, b.Z - a.Z)
        fm.Model.Elements.Add(e)
        If Not fm.BeamGroups.Contains(g) Then fm.BeamGroups.Add(g)
    End Sub

    'Square-panel tributary load (45 degree lines): a triangle of peak w*L/2 per adjacent panel; "w" here is the already summed intensity of the adjacent panels.
    Private Sub AddTriangular(p As FemPattern, ei As Integer, wSum As Double, L As Double, dirX As Boolean, spec As FrameSpec)
        Dim peak As Double = wSum * L / 2.0
        p.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 0.5, .V1 = 0.0, .V2 = peak})
        p.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.5, .D2 = 1.0, .V1 = peak, .V2 = 0.0})
    End Sub
End Module
