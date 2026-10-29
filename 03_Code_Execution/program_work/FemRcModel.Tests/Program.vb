Imports System
Imports System.Collections.Generic
Imports FemCore
Imports FemRcModel

Module Program
    Dim Pass As Integer, Fail As Integer

    Sub Check(name As String, got As Double, want As Double, tol As Double)
        Dim rel As Double = If(want = 0.0, Math.Abs(got), Math.Abs(got - want) / Math.Abs(want))
        If rel <= tol Then
            Pass += 1 : Console.WriteLine("OK   " & name & "  got=" & got.ToString("G8") & " want=" & want.ToString("G8"))
        Else
            Fail += 1 : Console.WriteLine("FAIL " & name & "  got=" & got.ToString("G8") & " want=" & want.ToString("G8") & " rel=" & rel.ToString("G3"))
        End If
    End Sub

    Sub Main()
        Dim spec As New FrameSpec
        Dim secs As New Dictionary(Of String, FemSection)
        For Each pos In {"Corner", "Edge", "Inter"}
            For Each sg In {"_S12", "_S34"}
                Dim sz As Double = If(sg = "_S12", 500.0, 400.0)
                secs("C" & pos & sg) = RcFrameBuilder.MakeSection("C" & pos & sg, sz, sz, 30.0, spec.ConcreteUnitWeight)
            Next
        Next
        For Each d In {"x", "y"}
            For Each sg In {"_S12", "_S34"}
                secs("B" & d & sg) = RcFrameBuilder.MakeSection("B" & d & sg, 300.0, 600.0, 30.0, spec.ConcreteUnitWeight)
            Next
        Next
        Dim fm = RcFrameBuilder.Build(spec, secs)
        Dim m = fm.Model
        Console.WriteLine("nodes=" & m.Nodes.Count & " elements=" & m.Elements.Count & " colGroups=" & fm.ColumnGroups.Count & " beamGroups=" & fm.BeamGroups.Count)
        Check("F1 element count (48 columns + 68 beams)", m.Elements.Count, 116, 0.0)
        Check("F1 column groups", fm.ColumnGroups.Count, 6, 0.0)
        Check("F1 beam groups", fm.BeamGroups.Count, 4, 0.0)

        Dim r = New FemStatic(m).Solve()
        Console.WriteLine("solve failed=" & r.Failed & " " & r.Message)
        'total base reaction Fz per pattern must equal the applied vertical load
        Dim areaFloor As Double = spec.NxBays * spec.Lx * spec.NyBays * spec.Ly
        Dim perim As Double = 2.0 * (spec.NxBays * spec.Lx + spec.NyBays * spec.Ly)
        Dim slabSelf As Double = spec.SlabThickness * spec.ConcreteUnitWeight
        Dim wantQ As Double = areaFloor * spec.LiveFloor * (spec.NStory - 1)
        Dim wantS As Double = areaFloor * spec.Snow
        Dim wantGload As Double = areaFloor * ((spec.DeadFloor + slabSelf) * (spec.NStory - 1) + spec.DeadRoof + slabSelf) + perim * spec.WallLine * (spec.NStory - 1)
        'self weight of frame members
        Dim selfW As Double = 0.0
        For Each e In m.Elements : selfW += e.Section.UnitWeight * e.Section.A * e.Length : Next
        Dim sumQ As Double = 0.0, sumS As Double = 0.0, sumG As Double = 0.0
        For ni As Integer = 0 To m.Nodes.Count - 1
            If m.Nodes(ni).Restraint(2) Then
                sumG += r.Reaction(0)(ni * 6 + 2) : sumQ += r.Reaction(1)(ni * 6 + 2) : sumS += r.Reaction(2)(ni * 6 + 2)
            End If
        Next
        Check("F2 sum Fz live", sumQ, wantQ, 0.0001)
        Check("F2 sum Fz snow", sumS, wantS, 0.0001)
        Check("F2 sum Fz dead (slab + wall + frame self weight)", sumG, wantGload + selfW, 0.0001)
        'Seismic: mass source G + 0.30 Q + 0.30 S (TBDY Table 4.3), TBDY spectrum, modal + RS
        Dim tp As New TbdyParams
        m.MassSource.SelfMass = False
        m.MassSource.Patterns.Add(New FemTerm With {.Name = "G", .Factor = 1.0})
        m.MassSource.Patterns.Add(New FemTerm With {.Name = "Q", .Factor = 0.3})
        m.MassSource.Patterns.Add(New FemTerm With {.Name = "S", .Factor = 0.3})
        m.Spectra.Add(Tbdy.DesignSpectrum(tp, "SPD"))
        Dim fs As New FemStatic(m)
        Dim md = fs.ModalSolve(12)
        Dim wantMass As Double = (wantGload + selfW + 0.3 * (wantQ + wantS)) / 9.80665
        Console.WriteLine("mass X=" & md.TotalMass(0).ToString("F2") & " expected(self mass in G pattern counted once)=" & wantMass.ToString("F2") & " expected+selfW again=" & (wantMass + selfW / 9.80665).ToString("F2"))
        For k As Integer = 0 To 5
            Console.WriteLine("mode " & (k + 1) & " T=" & md.Period(k).ToString("F4") & " mx=" & md.MassRatio(k, 0).ToString("F3") & " my=" & md.MassRatio(k, 1).ToString("F3"))
        Next
        Dim sumX As Double = 0, sumY As Double = 0
        For k As Integer = 0 To md.Period.Length - 1 : sumX += md.MassRatio(k, 0) : sumY += md.MassRatio(k, 1) : Next
        Console.WriteLine("sum mass ratio X=" & sumX.ToString("F3") & " Y=" & sumY.ToString("F3"))
        Dim rx = fs.ResponseSpectrum(md, New FemRSCase With {.Name = "EX", .Direction = 0, .Spectrum = "SPD", .ScaleFactor = 9.80665})
        Dim ry = fs.ResponseSpectrum(md, New FemRSCase With {.Name = "EY", .Direction = 1, .Spectrum = "SPD", .ScaleFactor = 9.80665})
        Console.WriteLine("Vbase X=" & rx.BaseForce(0).ToString("F1") & " kN  Y=" & ry.BaseForce(1).ToString("F1") & " kN  W=" & (md.TotalMass(0) * 9.80665).ToString("F1"))
        Dim baseHalf As Double = 0.0
        For Each e In m.Elements
            If e.Story = 1 AndAlso Math.Abs(e.Axes(0, 2)) > 0.9 Then baseHalf += 0.5 * e.Section.UnitWeight * e.Section.A * e.Length
        Next
        Check("F3 total mass X = (G + 0.3Q + 0.3S - fixed-base half columns)/g", md.TotalMass(0), wantMass - baseHalf / 9.80665, 0.0001)
        Check("F3 total mass X = Y", md.TotalMass(1), md.TotalMass(0), 0.0001)
        Check("F3 cumulative mass ratio X (12 modes)", sumX, 1.0, 0.01)
        Dim sarMax As Double = tp.SD1 / tp.TB / tp.R * 1.0     'plateau Sar = SDS / R for T >= TB
        Check("F3 TBDY TA", tp.TA, 0.0630, 0.002)
        Check("F3 TBDY TB", tp.TB, 0.3148, 0.002)
        Check("F3 design Sar at T=0.5 s = SD1/(T R)", Tbdy.Sae(tp, 0.5) / Tbdy.Ra(tp, 0.5), 0.226 / 0.5 / 8.0, 0.0001)
        Check("F3 RS base shear X <= mass x Sar,max (CQC, 80% mass)", If(rx.BaseForce(0) <= md.TotalMass(0) * 9.80665 * 0.226 / md.Period(1) / 8.0 * 1.0001, 1.0, 0.0), 1.0, 0.0)
        'F4: combinations (ground-column axial force equilibrium for U1) + seismic envelope shape
        Dim defs As New Dictionary(Of String, RcSectionDef)
        For Each k In secs.Keys
            Dim s0 = secs(k)
            defs(k) = New RcSectionDef With {.B = s0.Width * 1000.0, .H = s0.Depth * 1000.0, .Fc = 30.0}
        Next
        Dim fa = RcCombos.Analyze(spec, defs, tp)
        Dim sumP As Double = 0.0
        For ei As Integer = 0 To fa.Gross.Model.Elements.Count - 1
            Dim e = fa.Gross.Model.Elements(ei)
            If e.Story = 1 AndAlso Math.Abs(e.Axes(0, 2)) > 0.9 Then sumP += fa.Combos(0).Forces(0)(ei)(0)
        Next
        Check("F4 U1 sum of ground column axial = 1.4G + 1.6(Q+S)", Math.Abs(sumP), 1.4 * (wantGload + selfW) + 1.6 * (wantQ + wantS), 0.0001)
        Dim sumMid As Double = 0.0
        For ei As Integer = 0 To fa.Cracked.Model.Elements.Count - 1
            Dim e = fa.Cracked.Model.Elements(ei)
            If e.Story = 1 AndAlso Math.Abs(e.Axes(0, 2)) > 0.9 Then sumMid += 0.5 * (fa.Combos(1).Forces(0)(ei)(0) + fa.Combos(1).Forces(1)(ei)(0))
        Next
        Dim ezT As Double = 0.3 * 2.0 / 3.0 * tp.SDS
        Check("F4 Eq4.11 static part ground axial = (1+0.3*2/3*SDS)G + Q + 0.2S", Math.Abs(sumMid), (1.0 + ezT) * (wantGload + selfW) + wantQ + 0.2 * wantS, 0.0001)
        'F5: U1 midspan moment of an interior beam (fixed-end-ish): span sagging + support hogging, equilibrium with face moments and load
        Dim bi As Integer = 48    'first beam
        Dim eb = fa.Gross.Model.Elements(bi)
        Dim ff = fa.Combos(0).Forces(0)(bi), mm = fa.Combos(0).Mid(0)(bi)
        Console.WriteLine("beam " & eb.Name & " face M3 I=" & ff(5).ToString("F2") & " J=" & ff(11).ToString("F2") & " mid M3=" & mm(5).ToString("F2") & " mid V2=" & mm(1).ToString("F2") & " mid V3=" & mm(2).ToString("F2"))
        Console.WriteLine("combos=" & fa.Combos.Count & "  T1(cracked)=" & fa.Modal.Period(0).ToString("F3") & " s  Vx=" & fa.BaseShearX.ToString("F1") & " Vy=" & fa.BaseShearY.ToString("F1") & " W=" & fa.Weight.ToString("F1"))
        Console.WriteLine("PASS=" & Pass & " FAIL=" & Fail)
    End Sub
End Module
