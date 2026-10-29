Imports System.Diagnostics
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
        For Each e In m.Elements
            Dim lw As Double = e.Length
            If Math.Abs(e.Axes(0, 2)) <= 0.9 Then lw = e.Length - e.OffsetI - e.OffsetJ      'beams: clear length (as FemStatic self weight)
            selfW += e.Section.UnitWeight * e.Section.A * lw
        Next
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
        Check("F3 total mass X = (G + 0.3Q + 0.3S - fixed-base half columns)/g", md.TotalMass(0), wantMass - baseHalf / 9.80665, 0.003)   'tolerance: with rigid end zones the share of a base column's load at the (fixed) base end is not exactly one half
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
        For Each g In fa.Gross.BeamGroups
            Dim sx = secs(g)
            Dim rp = BeamDesign.DesignGroup(fa, g, sx.Width * 1000.0, sx.Depth * 1000.0, 30.0, Nothing)
            Dim sf As New BeamFibre With {.Params = New RcCore.SfrcParams With {.FR1 = 3.3, .FR3 = 3.0}, .DoseKgM3 = 25.0}
            Dim rf = BeamDesign.DesignGroup(fa, g, sx.Width * 1000.0, sx.Depth * 1000.0, 30.0, sf)
            Console.WriteLine("BEAM " & g & "  MuTop=" & (rp.MuTop / 1.0E6).ToString("F1") & " MuBot=" & (rp.MuBot / 1.0E6).ToString("F1") & " kNm | plain AsTop=" & rp.AsTop.ToString("F0") & " AsBot=" & rp.AsBot.ToString("F0") & " (" & rp.Governing & ")  | SFRC AsTop=" & rf.AsTop.ToString("F0") & " AsBot=" & rf.AsBot.ToString("F0") & " (" & rf.Governing & ")")
        Next
        Dim gS = fa.Gross.BeamGroups(0)
        For Each hh In {300.0, 350.0, 400.0}
            Dim sf2 As New BeamFibre With {.Params = New RcCore.SfrcParams With {.FR1 = 3.3, .FR3 = 3.0}, .DoseKgM3 = 25.0}
            Dim a1 = BeamDesign.DesignGroup(fa, gS, 250.0, hh, 30.0, Nothing), a2 = BeamDesign.DesignGroup(fa, gS, 250.0, hh, 30.0, sf2)
            Console.WriteLine("SMALL 250x" & hh & " plain AsTop=" & a1.AsTop.ToString("F0") & " AsBot=" & a1.AsBot.ToString("F0") & " (" & a1.Governing & ") | SFRC AsTop=" & a2.AsTop.ToString("F0") & " AsBot=" & a2.AsBot.ToString("F0") & " (" & a2.Governing & ")")
        Next
        'F5: hand calculation of the beam flexure design: b = 300, d = 540, C30, fy = 420, Mu = 250 kNm
        Dim muH As Double = 250.0E6, bH As Double = 300.0, dH As Double = 540.0
        Dim aH As Double = dH - Math.Sqrt(dH * dH - 2.0 * muH / (0.9 * 0.85 * 30.0 * bH))
        Dim asH As Double = 0.85 * 30.0 * bH * aH / 420.0
        Check("F5 hand As (phi 0.9)", asH, 1312.5, 0.002)
        Check("F5 RequiredAs equals hand As", RcCore.AciBeam.RequiredAs(muH, bH, dH, 30.0, 420.0), asH, 0.0005)
        Dim flexH = RcCore.AciBeam.Flexure(bH, dH, asH, 30.0, 420.0)
        Check("F5 phiMn at hand As = Mu", flexH.PhiMn, muH, 0.002)
        Check("F5 epsT >= 0.005 (tension-controlled)", If(flexH.EpsT >= 0.005, 1.0, 0.0), 1.0, 0.0)
        Dim p0 As New RcCore.SfrcParams With {.FR1 = 0.0, .FR3 = 0.0}
        Check("F5 SFRC with fR = 0 equals plain RC", RcCore.SfrcBeam.Flexure(bH, 600.0, dH, asH, 30.0, 420.0, p0, 600.0).PhiMn, flexH.PhiMn, 0.002)
        For Each gC In fa.Gross.ColumnGroups
            Dim sc = secs(gC)
            For Each rh In {0.01, 0.02}
                Dim cr = ColumnDesign.CheckGroup(fa, gC, sc.Width * 1000.0, sc.Depth * 1000.0, 30.0, rh, 20.0)
                Console.WriteLine("COLUMN " & gC & " " & (sc.Width * 1000).ToString("F0") & "x" & (sc.Depth * 1000).ToString("F0") & " rho=" & rh.ToString("P0") & " n=" & cr.NBars & " Ø20 Nd=" & (cr.NdMax / 1000.0).ToString("F0") & " kN  D/C=" & cr.DcrMax.ToString("F2") & "  g(A2)=" & cr.G_Axial.ToString("F2") & "  " & cr.Governing)
            Next
        Next
        'F6: column P-M hand calculation: 400 x 400, C30, fy 420, layers at y = 50 and 350, 3 Ø20 each (942.48 mm2), c = 200 mm
        Dim asL As Double = 3.0 * Math.PI * 100.0
        Dim scH As New RcCore.ColumnSection With {.B = 400.0, .H = 400.0, .Fc = 30.0, .Fy = 420.0, .LayerY = {50.0, 350.0}, .LayerAs = {asL, asL}}
        Dim b1H As Double = 0.85 - 0.05 * 2.0 / 7.0
        Dim cH As Double = 200.0, aHc As Double = b1H * cH
        Dim ccH As Double = 0.85 * 30.0 * 400.0 * aHc
        Dim ftop As Double = asL * (420.0 - 0.85 * 30.0)               'compression steel, yielded (eps 0.00225 > 0.0021)
        Dim fbot As Double = asL * 420.0                               'tension steel, yielded
        Dim pnH As Double = ccH + ftop - fbot
        Dim mnH As Double = ccH * (200.0 - aHc / 2.0) + ftop * 150.0 + fbot * 150.0
        Dim epsTH As Double = 0.003 * (350.0 - cH) / cH
        Dim phiH As Double = 0.65 + 0.25 * (epsTH - 420.0 / 200000.0) / (0.005 - 420.0 / 200000.0)
        Dim ptH = RcCore.AciColumn.AtC(scH, cH)
        Check("F6 Pn at c = 200", ptH.Pn, pnH, 0.001)
        Check("F6 Mn at c = 200", Math.Abs(ptH.Mn), mnH, 0.001)
        Check("F6 phi at c = 200 (transition zone)", ptH.Phi, phiH, 0.001)
        Check("F6 Po = 0.85 fc (Ag - Ast) + fy Ast", RcCore.AciColumn.Po(scH), 0.85 * 30.0 * (160000.0 - 2.0 * asL) + 420.0 * 2.0 * asL, 0.0001)
        Check("F6 phiPn,max = 0.65 x 0.80 Po", RcCore.AciColumn.DesignPnMax(scH), 0.65 * 0.8 * RcCore.AciColumn.Po(scH), 0.0001)
        'D/C of the hand point: load (phiPn, phiMn) at c = 200 must give D/C = 1 (same eccentricity-independent, P fixed)
        Check("F6 D/C = 1 on the interaction curve", RcCore.AciColumn.DemandCapacityRatio(scH, ptH.PhiPn, Math.Abs(ptH.PhiMn)), 1.0, 0.005)
        For Each g In fa.Gross.BeamGroups
            Dim sx = secs(g)
            Dim rp = BeamDesign.DesignGroup(fa, g, sx.Width * 1000.0, sx.Depth * 1000.0, 30.0, Nothing)
            Dim sh = BeamShear.DesignGroup(fa, rp, Nothing, 0.0, 0.0)
            Console.WriteLine("SHEAR " & g & "  Vdy=" & (sh.Vdy / 1000.0).ToString("F1") & " Vseis=" & (sh.Vseis / 1000.0).ToString("F1") & " Ve=" & (sh.Ve / 1000.0).ToString("F1") & " kN ln=" & sh.Ln.ToString("F0") & "  Vc=0:" & sh.ZeroVc & "  s_conf=" & sh.SConf.ToString("F0") & " s_mid=" & sh.SMid.ToString("F0") & " mm  g(7.10)=" & sh.G_Crush.ToString("F2") & " " & sh.Governing)
        Next
        'F7: hand calculation of the capacity-design shear for Bx_S12 (300 x 600, As = 540 / 540 mm2)
        Dim rb = BeamDesign.DesignGroup(fa, "Bx_S12", 300.0, 600.0, 30.0, Nothing)
        Dim shb = BeamShear.DesignGroup(fa, rb, Nothing, 0.0, 0.0)
        Dim mrH As Double = RcCore.AciBeam.Flexure(300.0, 540.0, 540.0, 30.0 / 1.5, 420.0 / 1.15).Mn
        Check("F7 Mr(top) = Mr(bottom) for equal steel", shb.MrTop, mrH, 0.0001)
        Check("F7 Vseis = 1.4 (Mr,top + Mr,bot)/ln", shb.Vseis, 1.4 * 2.0 * mrH / shb.Ln, 0.0001)
        Check("F7 Ve = Vdy + Vseis", shb.Ve, shb.Vdy + shb.Vseis, 0.0001)
        Dim vc7 As Double = If(shb.ZeroVc, 0.0, RcCore.AciBeam.Vc(300.0, 540.0, 30.0))
        Check("F7 Av/s in confinement zone = max((Ve/phi - Vc)/(fyt d), Av,min/s)", shb.AvConfPerS, Math.Max(Math.Max(shb.Ve / 0.75 - vc7, 0.0) / (420.0 * 540.0), RcCore.AciBeam.AvMinPerS(300.0, 30.0, 420.0)), 0.0001)
        Console.WriteLine("F7 note: Vc = 0 switch ZeroVc=" & shb.ZeroVc & " (Vseis/Ve = " & (shb.Vseis / shb.Ve).ToString("F3") & "; rule is discontinuous at 0.5)")
        Check("F8 phiMn at the axial load of a known curve point", ColumnShear.PhiMnAtP(scH, ptH.PhiPn), Math.Abs(ptH.PhiMn), 0.002)
        Dim scT As New RcCore.ColumnSection With {.B = 400.0, .H = 400.0, .Fc = 30.0 / 1.5, .Fy = 420.0 / 1.15, .LayerY = {50.0, 350.0}, .LayerAs = {asL, asL}, .Beta1Override = Ts500.K1(30.0)}
        Dim ptT = RcCore.AciColumn.AtC(scT, 200.0)
        Check("F8 TS 500 Mr (no phi) at Pn of a known point = its Mn", ColumnShear.MrAtP(scT, ptT.Pn), Math.Abs(ptT.Mn), 0.002)
        Check("F9 TS 500 k1 (C30) = 0.82", Ts500.K1(30.0), 0.82, 0.0001)
        Dim fa1 As Double, fa3 As Double
        Check("F10 4D C30 dose 22.5 in grid", If(FibreTable.Lookup("4D", 30.0, 22.5, fa1, fa3), 1.0, 0.0), 1.0, 0.0001)
        Check("F10 4D C30 22.5 fR1 = 3.0", fa1, 3.0, 0.0001)
        Check("F10 4D C30 22.5 fR3 = 3.75", fa3, 3.75, 0.0001)
        Check("F10 4D C40 40 kg/m3 not measured", If(FibreTable.Lookup("4D", 40.0, 40.0, fa1, fa3), 1.0, 0.0), 0.0, 0.0001)
        Check("F10 5D C30 20 kg/m3 not measured", If(FibreTable.Lookup("5D", 30.0, 20.0, fa1, fa3), 1.0, 0.0), 0.0, 0.0001)
        Check("F10 4D C35 not measured", If(FibreTable.Lookup("4D", 35.0, 30.0, fa1, fa3), 1.0, 0.0), 0.0, 0.0001)
        Check("F10 5D C40 35 fR3 = 6.4", If(FibreTable.Lookup("5D", 40.0, 35.0, fa1, fa3), fa3, 0.0), 6.4, 0.0001)
        'F11: optimisers on a shifted integer sphere (optimum 0 at x = 17), 12 variables in [0, 40]
        Dim lbT(11) As Integer, ubT(11) As Integer
        For i As Integer = 0 To 11 : ubT(i) = 40 : Next
        Dim sphere As Func(Of Integer(), Double) = Function(xx) xx.Sum(Function(v) CDbl((v - 17) * (v - 17)))
        For Each mth In {"YAKO", "BTO", "ATO"}
            Dim r1 = Optimizers.Run(mth, sphere, lbT, ubT, 4000, 11)
            Dim r2 = Optimizers.Run(mth, sphere, lbT, ubT, 4000, 11)
            Dim r3 = Optimizers.Run(mth, sphere, lbT, ubT, 4000, 12)
            Check("F11 " & mth & " uses exactly MaxFEs", r1.FEs, 4000.0, 0.0)
            Check("F11 " & mth & " same seed reproducible", r1.BestF, r2.BestF, 0.0)
            Check("F11 " & mth & " different seed differs", If(r1.BestF = r3.BestF AndAlso r1.History.Count = r3.History.Count, 0.0, 1.0), 1.0, 0.0)
            Check("F11 " & mth & " beats random mean (~3700) by 10x", If(r1.BestF < 370.0, 1.0, 0.0), 1.0, 0.0)
            Console.WriteLine("   " & mth & " best f=" & r1.BestF.ToString("F0"))
        Next
        Check("F9 fcd (C30) = 20", Ts500.Fcd(30.0), 20.0, 0.0001)
        Check("F9 fyd (420) = 365.22", Ts500.Fyd(420.0), 365.2174, 0.0001)
        For Each gC In fa.Gross.ColumnGroups
            Dim sc = secs(gC)
            Dim cr = ColumnDesign.CheckGroup(fa, gC, sc.Width * 1000.0, sc.Depth * 1000.0, 30.0, 0.01, 20.0)
            Dim cs2 = ColumnShear.Design(fa, cr)
            Console.WriteLine("CSHEAR " & gC & " Ve(x)=" & (cs2.Ve(0) / 1000.0).ToString("F0") & " Ve(y)=" & (cs2.Ve(1) / 1000.0).ToString("F0") & " kN  legs=" & cs2.Legs(0) & "/" & cs2.Legs(1) & "  s_conf=" & cs2.SConf.ToString("F0") & " s_mid=" & cs2.SMid.ToString("F0") & " lk=" & cs2.Lk.ToString("F0") & " mm  g(7.7)=" & Math.Max(cs2.G_Crush(0), cs2.G_Crush(1)).ToString("F2") & " " & cs2.Governing)
        Next
        Dim dd3 As New TbdyParams With {.SDS = 0.294, .SD1 = 0.091}
        Dim dr = Checks.Drift(fa, spec, tp, dd3)
        Console.WriteLine("DRIFT T1=" & dr.T1.ToString("F3") & " lambda=" & dr.Lambda.ToString("F3") & " max lambda*delta/h=" & dr.MaxRatio.ToString("F5") & " (story " & dr.Story & ", dir " & dr.Dir & ")  g=" & dr.G.ToString("F2"))
        Dim colRes As New Dictionary(Of String, ColumnGroupResult), beamRes As New Dictionary(Of String, BeamGroupResult)
        For Each gC In fa.Gross.ColumnGroups
            Dim sc = secs(gC)
            colRes(gC) = ColumnDesign.CheckGroup(fa, gC, sc.Width * 1000.0, sc.Depth * 1000.0, 30.0, 0.01, 20.0)
        Next
        For Each g In fa.Gross.BeamGroups
            Dim sx = secs(g)
            beamRes(g) = BeamDesign.DesignGroup(fa, g, sx.Width * 1000.0, sx.Depth * 1000.0, 30.0, Nothing)
        Next
        Dim jl = Checks.StrongColumn(fa, spec, colRes, beamRes, Nothing)
        Dim gMax As Double = -99.0, nEx As Integer = 0
        For Each jr In jl
            If jr.Exempt Then nEx += 1
            If Not jr.Exempt AndAlso jr.G > gMax Then gMax = jr.G
        Next
        Console.WriteLine("A5 joints=" & jl.Count & " exempt(Nd<=0.1 Ac fck)=" & nEx & " worst g (non-exempt)=" & gMax.ToString("F2") & "  (g > 0 = weak column)")
        Dim csh As New Dictionary(Of String, ColumnShearResult)
        For Each gC In fa.Gross.ColumnGroups : csh(gC) = ColumnShear.Design(fa, colRes(gC)) : Next
        Dim jsl = JointShear.Check(fa, spec, colRes, csh, beamRes)
        Dim jsMax As Double = -99.0, nConf As Integer = 0, jsName As String = ""
        For Each js In jsl
            If js.Confined Then nConf += 1
            If js.G > jsMax Then jsMax = js.G : jsName = js.Name
        Next
        Console.WriteLine("JOINT SHEAR joints=" & jsl.Count & " confined=" & nConf & " worst g=" & jsMax.ToString("F2") & " at " & jsName)
        'EVAL: Ex1 evaluator with the trial design (plain RC vs sample fibre)
        Dim dsn As New Ex1Design
        For Each pos In {"Corner", "Edge", "Inter"}
            dsn.Columns("C" & pos & "_S12") = New ColumnDef With {.B = 500.0, .H = 500.0, .Rho = 0.012, .Fc = 30.0}
            dsn.Columns("C" & pos & "_S34") = New ColumnDef With {.B = 400.0, .H = 400.0, .Rho = 0.012, .Fc = 30.0}
        Next
        For Each dd In {"x", "y"}
            For Each sg In {"_S12", "_S34"}
                dsn.Beams("B" & dd & sg) = New BeamDef With {.B = 300.0, .H = 600.0, .Fc = 30.0}
            Next
        Next
        Dim pr As New RcCore.PriceSet
        Dim ev0 = Ex1Evaluator.Evaluate(spec, dsn, tp, dd3, pr)
        Console.WriteLine("EVAL plain: cost=" & ev0.Cost.Total.ToString("F0") & " TL (concrete " & ev0.Cost.Concrete.ToString("F0") & ", rebar " & ev0.Cost.Rebar.ToString("F0") & ", formwork " & ev0.Cost.Formwork.ToString("F0") & ", fibre " & ev0.Cost.Fibre.ToString("F0") & ")  T=" & ev0.T.ToString("F3") & "  Fpen=" & ev0.Fpen.ToString("F0") & "  steel=" & ev0.SteelKg.ToString("F0") & " kg concrete=" & ev0.ConcreteM3.ToString("F1") & " m3")
        For Each v In ev0.Violations
            If v.Item2 > 0.0 Then Console.WriteLine("   VIOLATED " & v.Item1 & " g=" & v.Item2.ToString("F3"))
        Next
        For Each dd In {"x", "y"}
            For Each sg In {"_S12", "_S34"}
                dsn.Beams("B" & dd & sg).Fibre = New BeamFibre With {.Params = New RcCore.SfrcParams With {.FR1 = 3.3, .FR3 = 3.0}, .DoseKgM3 = 25.0}
            Next
        Next
        Dim ev1 = Ex1Evaluator.Evaluate(spec, dsn, tp, dd3, pr)
        Console.WriteLine("EVAL sample fibre 25 kg/m3: cost=" & ev1.Cost.Total.ToString("F0") & " TL (fibre " & ev1.Cost.Fibre.ToString("F0") & ")  T=" & ev1.T.ToString("F3") & "  steel=" & ev1.SteelKg.ToString("F0") & " kg")
        Dim swE = Stopwatch.StartNew()
        For q As Integer = 1 To 3 : Ex1Evaluator.Evaluate(spec, dsn, tp, dd3, pr) : Next
        Console.WriteLine("EVALTIME per evaluation = " & (swE.ElapsedMilliseconds / 3.0).ToString("F0") & " ms")
        'F12: Ex1Problem encoding, repair, thread safety and a short optimiser run
        Dim prob As New Ex1Problem(spec, tp, dd3, pr, True)
        Dim xT(prob.NVars - 1) As Integer
        Dim kx As Integer = 0
        For g As Integer = 0 To 5 : xT(kx) = 4 : xT(kx + 1) = 4 : xT(kx + 2) = 1 : xT(kx + 3) = 0 : kx += 4 : Next
        For g As Integer = 0 To 3 : xT(kx) = 1 : xT(kx + 1) = 6 : xT(kx + 2) = 1 : xT(kx + 3) = 0 : xT(kx + 4) = 5 : kx += 5 : Next      'beams 300x600, C40, 4D, dose 40 (repaired to 35)
        Dim dT = prob.Decode(xT)
        Check("F12 NVars = 44", prob.NVars, 44.0, 0.0)
        Check("F12 col b = 500", dT.Columns("CEdge_S12").B, 500.0, 0.0)
        Check("F12 col rho = 1.25 %", dT.Columns("CEdge_S12").Rho, 0.0125, 1.0E-9)
        Check("F12 beam h = 600", dT.Beams("By_S34").H, 600.0, 0.0)
        Check("F12 dose repaired 40 -> 35 (4D C40)", dT.Beams("Bx_S12").Fibre.DoseKgM3, 35.0, 0.0)
        Dim fT = prob.Evaluate(xT)
        Console.WriteLine("F12 fibre trial: cost=" & fT.Cost.Total.ToString("F0") & " T=" & fT.T.ToString("F3") & " Fpen=" & fT.Fpen.ToString("F0"))
        Dim par(7) As Double
        System.Threading.Tasks.Parallel.For(0, 8, Sub(q) par(q) = prob.Objective(xT))
        Check("F12 parallel evaluations identical", par.Max() - par.Min(), 0.0, 0.0)
        Check("F12 parallel = serial", par(3), fT.Fpen, 1.0E-12)
        Dim swO = Stopwatch.StartNew()
        Dim ro = Optimizers.Run("ATO", AddressOf prob.Objective, prob.Lb, prob.Ub, 200, 5)
        Console.WriteLine("F12 ATO 200 FEs: best Fpen=" & ro.BestF.ToString("F0") & " (" & swO.ElapsedMilliseconds & " ms)")
        Check("F12 ATO used 200 FEs", ro.FEs, 200.0, 0.0)
        'stage profile
        Dim defsP As New Dictionary(Of String, RcSectionDef)
        For Each kv In dsn.Columns : defsP(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        For Each kv In dsn.Beams : defsP(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        Dim swP = Stopwatch.StartNew()
        Dim faP = RcCombos.Analyze(spec, defsP, tp)
        Console.WriteLine("PROFILE analyze = " & swP.ElapsedMilliseconds & " ms") : swP.Restart()
        Dim crP As New Dictionary(Of String, ColumnGroupResult), csP As New Dictionary(Of String, ColumnShearResult)
        For Each g In faP.Gross.ColumnGroups
            Dim c = dsn.Columns(g)
            crP(g) = ColumnDesign.CheckGroup(faP, g, c.B, c.H, c.Fc, c.Rho, c.BarDia)
        Next
        Console.WriteLine("PROFILE col CheckGroup = " & swP.ElapsedMilliseconds & " ms") : swP.Restart()
        For Each g In faP.Gross.ColumnGroups : csP(g) = ColumnShear.Design(faP, crP(g)) : Next
        Console.WriteLine("PROFILE col shear = " & swP.ElapsedMilliseconds & " ms") : swP.Restart()
        Dim brP As New Dictionary(Of String, BeamGroupResult)
        For Each g In faP.Gross.BeamGroups
            Dim b = dsn.Beams(g)
            brP(g) = BeamDesign.DesignGroup(faP, g, b.B, b.H, b.Fc, b.Fibre)
        Next
        Console.WriteLine("PROFILE beam flexure = " & swP.ElapsedMilliseconds & " ms") : swP.Restart()
        Dim dP = Checks.Drift(faP, spec, tp, dd3)
        Console.WriteLine("PROFILE drift = " & swP.ElapsedMilliseconds & " ms") : swP.Restart()
        Dim jP = Checks.StrongColumn(faP, spec, crP, brP, Nothing)
        Console.WriteLine("PROFILE strong column = " & swP.ElapsedMilliseconds & " ms") : swP.Restart()
        Dim jsP = JointShear.Check(faP, spec, crP, csP, brP)
        Console.WriteLine("PROFILE joint shear = " & swP.ElapsedMilliseconds & " ms")
        Console.WriteLine("combos=" & fa.Combos.Count & "  T1(cracked)=" & fa.Modal.Period(0).ToString("F3") & " s  Vx=" & fa.BaseShearX.ToString("F1") & " Vy=" & fa.BaseShearY.ToString("F1") & " W=" & fa.Weight.ToString("F1"))
        Console.WriteLine("PASS=" & Pass & " FAIL=" & Fail)
    End Sub
End Module
