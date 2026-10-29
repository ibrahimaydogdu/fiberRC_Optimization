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
        Console.WriteLine("PASS=" & Pass & " FAIL=" & Fail)
    End Sub
End Module
