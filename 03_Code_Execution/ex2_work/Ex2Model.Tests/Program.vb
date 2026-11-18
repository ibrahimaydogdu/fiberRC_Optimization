Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports FemRcModel
Imports Ex2Model

'Ex2 unit tests. Exit code = number of failed checks.
Module Program
    Dim fails As Integer = 0, passes As Integer = 0
    ReadOnly ci As CultureInfo = CultureInfo.InvariantCulture

    Sub Check(name As String, ok As Boolean, Optional detail As String = "")
        If ok Then passes += 1 Else fails += 1
        Console.WriteLine(If(ok, "PASS ", "FAIL ") & name & If(detail <> "", "  [" & detail & "]", ""))
    End Sub

    Sub Near(name As String, actual As Double, expected As Double, relTol As Double)
        Dim rel As Double = Math.Abs(actual - expected) / Math.Max(Math.Abs(expected), 1.0E-12)
        Check(name, rel <= relTol, "actual " & actual.ToString("R", ci) & ", expected " & expected.ToString("R", ci) & ", rel.err " & rel.ToString("E2", ci))
    End Sub

    Function Defs(sizes As Dictionary(Of String, Double())) As Dictionary(Of String, RcSectionDef)
        Dim d As New Dictionary(Of String, RcSectionDef)
        For Each kv In sizes : d(kv.Key) = New RcSectionDef With {.B = kv.Value(0), .H = kv.Value(1), .Fc = 30.0} : Next
        Return d
    End Function

    Function Main() As Integer
        Dim spec5 As New FrameSpec2D With {.Groups = 5}
        Dim t1 = Ex2Problem.SourceTable1()
        Dim a = RcCombos2D.Analyze(spec5, Defs(t1), True)
        Dim m = a.Frame.Model

        '--- V1 (a): numbering, lengths, supports (Şekil 1 / 2)
        Check("T1 16 nodes", m.Nodes.Count = 16, m.Nodes.Count.ToString())
        Check("T1 21 elements (12 columns + 9 beams)", m.Elements.Count = 21 AndAlso a.Frame.NCol = 12)
        Dim colNums = Enumerable.Range(0, 21).Where(Function(i) a.Frame.IsColumn(i)).Select(Function(i) CInt(m.Elements(i).Name.Substring(1))).ToArray()
        Check("T1 column numbers 1-4, 8-11, 15-18 (Şekil 2)", String.Join(",", colNums) = "1,2,3,4,8,9,10,11,15,16,17,18", String.Join(",", colNums))
        Dim beamLens = Enumerable.Range(0, 21).Where(Function(i) Not a.Frame.IsColumn(i)).Select(Function(i) m.Elements(i).Length.ToString("0.###", ci)).ToArray()
        Check("T1 beam spans 5-6-5 m per storey", String.Join(",", beamLens) = "5,6,5,5,6,5,5,6,5", String.Join(",", beamLens))
        Check("T1 column height 3.3 m", Enumerable.Range(0, 21).Where(Function(i) a.Frame.IsColumn(i)).All(Function(i) Math.Abs(m.Elements(i).Length - 3.3) < 1.0E-9))
        Check("T1 nodes N1 (0,0,0) and N16 (16,0,9.9)", m.Nodes(0).Name = "N1" AndAlso m.Nodes(15).Name = "N16" AndAlso Math.Abs(m.Nodes(15).X - 16.0) < 1.0E-9 AndAlso Math.Abs(m.Nodes(15).Z - 9.9) < 1.0E-9)
        Check("T1 fixed base N1-N4", Enumerable.Range(0, 4).All(Function(i) m.Nodes(i).Restraint.All(Function(r) r)))
        Check("T1 no rigid end zones", m.Elements.All(Function(e) e.OffsetI = 0.0 AndAlso e.OffsetJ = 0.0))

        '--- V1 (a): section orientation (spec §1.1)
        Dim e1 = m.Elements(0), e5 = m.Elements(4)
        Check("T2 column E1 local 2 = global X (in-plane)", Math.Abs(e1.Axes(1, 0) - 1.0) < 1.0E-9)
        Check("T2 column E1 depth (local 2) = h = 0.30 m, width b = 0.25 m", Math.Abs(e1.Section.Depth - 0.3) < 1.0E-9 AndAlso Math.Abs(e1.Section.Width - 0.25) < 1.0E-9)
        Check("T2 column E1 I33 = b h^3/12 (in-plane bending)", Math.Abs(e1.Section.I33 - 0.25 * 0.3 ^ 3 / 12.0) < 1.0E-12)
        Check("T2 beam E5 local 2 = global Z, depth 0.35 m", Math.Abs(e5.Axes(1, 2) - 1.0) < 1.0E-9 AndAlso Math.Abs(e5.Section.Depth - 0.35) < 1.0E-9)

        '--- V1 (b), (c): vertical reactions with the source Table 1 sections (spec §10 V1)
        Dim sumV As Double = 0.0
        For Each e In m.Elements : sumV += e.Section.A * e.Length : Next
        Near("T3 sum V = 8.067 m3 (FEM sections)", sumV, 8.067, 1.0E-9)
        Near("T3 SW reaction = 25 x 8.067 = 201.675 kN", a.VerticalReaction("SW"), 201.675, 0.001)
        Near("T3 G reaction = 1 440 kN", a.VerticalReaction("G"), 1440.0, 0.001)
        Near("T3 Q reaction = 960 kN", a.VerticalReaction("Q"), 960.0, 0.001)
        Dim u1 = a.Combos.First(Function(c) c.Name.StartsWith("U1")), u2 = a.Combos.First(Function(c) c.Name.StartsWith("U2"))
        Near("T3 1.2D+1.6L reaction = 3 506.01 kN", a.VerticalReaction(u2), 3506.01, 0.001)
        Near("T3 1.4D reaction = 2 298.345 kN", a.VerticalReaction(u1), 2298.345, 0.001)
        Check("T3 1.4D not governing (reaction)", a.VerticalReaction(u1) < a.VerticalReaction(u2))
        For Each c In a.Combos.Where(Function(x) Not x.Main)
            Console.WriteLine("INFO V2-b reaction " & c.Name & " = " & a.VerticalReaction(c).ToString("F3", ci) & " kN")
        Next

        '--- load mapping: storey-2 interior beam E13 (6 m, 25x30), 1.2D+1.6L: |V_I| + |V_J| = (1.2 (30 + 25 x 0.25 x 0.30) + 1.6 x 20) x 6 = 421.5 kN
        Dim i2 = a.Combos.IndexOf(u2)
        Dim f13 = a.Face(i2)(12)
        Near("T4 E13 end shears sum = 421.5 kN", Math.Abs(f13(1)) + Math.Abs(f13(7)), 421.5, 1.0E-6)
        Dim fE1 = a.Face(i2)(0), fE4 = a.Face(i2)(3)
        Near("T4 symmetry: axial E1 = axial E4", fE1(0), fE4(0), 1.0E-6)

        '--- cost model volume (spec §5.4: b h L, axis length) and source concrete check 8.067 x 240 = 1 936.08 TL (spec §6, §10 V1 c)
        Dim pr As New RcCore.PriceSet
        Dim ev = Ex2Evaluator.Evaluate(spec5, Ex2Problem.FromSizes(t1), pr, New AciPlainShear(), Ex2Options.Campaign1())     'T1-T22: campaign-1 rules (PCR-25/24 off); new rules T23-T25
        Near("T5 evaluator concrete volume = 8.067 m3", ev.ConcreteM3, 8.067, 1.0E-9)
        Near("T5 source price check 8.067 x 240 = 1 936.08 TL", ev.ConcreteM3 * 240.0, 1936.08, 1.0E-6)
        Near("T5 concrete cost = 8.067 x 3 649.35 TL (ÇŞİDB 15.150.1006)", ev.Cost.Concrete, 8.067 * 3649.35, 1.0E-9)
        Console.WriteLine("INFO Table 1 sections (V2-b information, own model, 1.4D / 1.2D+1.6L): cost " & ev.Cost.Total.ToString("F2", ci) & " TL, T " & ev.T.ToString("F4", ci))
        For Each v In ev.Violations.Where(Function(x) x.Item2 > 0.0) : Console.WriteLine("INFO   violated: " & v.Item1 & " = " & v.Item2.ToString("F4", ci)) : Next
        For Each kv In ev.ColRes : Console.WriteLine("INFO   " & kv.Key & " rho " & kv.Value.Rho.ToString("P2", ci) & " D/C " & kv.Value.DcrMax.ToString("F3", ci) & " Nd,max " & (kv.Value.NdMax / 1000.0).ToString("F1", ci) & " kN") : Next
        For Each kv In ev.BeamRes : Console.WriteLine("INFO   " & kv.Key & " Mu- " & (kv.Value.MuTop / 1.0E6).ToString("F1", ci) & " Mu+ " & (kv.Value.MuBot / 1.0E6).ToString("F1", ci) & " kNm, " & kv.Value.Governing & ", Vu " & (ev.BeamShr(kv.Key).Vu / 1000.0).ToString("F1", ci) & " kN, s " & ev.BeamShr(kv.Key).S.ToString("F0", ci) & " mm") : Next

        '--- Decode bounds (PCR-16)
        Dim spec9 As New FrameSpec2D With {.Groups = 9}
        Dim p9 As New Ex2Problem(spec9, pr, False, New AciPlainShear())
        Dim p9f As New Ex2Problem(spec9, pr, True, New FibreShearIncrementQ17())
        Dim p5 As New Ex2Problem(spec5, pr, False, New AciPlainShear())
        Dim p5f As New Ex2Problem(spec5, pr, True, New FibreShearIncrementQ17())
        p9.Opts = Ex2Options.Campaign1() : p9f.Opts = Ex2Options.Campaign1() : p5.Opts = Ex2Options.Campaign1() : p5f.Opts = Ex2Options.Campaign1()
        Check("T6 9 groups: 18 plain / 24 fibre variables (spec §4)", p9.NVars = 18 AndAlso p9f.NVars = 24, p9.NVars & "/" & p9f.NVars)
        Check("T6 5 groups: 10 plain / 16 fibre variables", p5.NVars = 10 AndAlso p5f.NVars = 16, p5.NVars & "/" & p5f.NVars)
        Dim space As Double = 1.0
        For i As Integer = 0 To p9.NVars - 1 : space *= p9.Ub(i) - p9.Lb(i) + 1 : Next
        Near("T6 plain space 35^9", space, Math.Pow(35, 9), 1.0E-12)
        Dim dLo = p9.Decode(p9.Lb), dHi = p9.Decode(p9.Ub)
        Check("T6 Decode(lb): 250 x 300 everywhere, C30", dLo.Columns.Values.All(Function(c) c.B = 250 AndAlso c.H = 300 AndAlso c.Fc = 30) AndAlso dLo.Beams.Values.All(Function(b) b.B = 250 AndAlso b.H = 300 AndAlso b.Fc = 30))
        Check("T6 Decode(ub): 450 x 600 everywhere", dHi.Columns.Values.All(Function(c) c.B = 450 AndAlso c.H = 600) AndAlso dHi.Beams.Values.All(Function(b) b.B = 450 AndAlso b.H = 600))
        Dim threw As Boolean = False
        Try
            Dim bad = CType(p9.Ub.Clone(), Integer()) : bad(0) += 1 : p9.Decode(bad)
        Catch ex As ArgumentOutOfRangeException
            threw = True
        End Try
        Check("T6 Decode rejects out-of-bound index", threw)

        '--- fibre dose repair (spec §8) and PLACEHOLDER Q17 block
        Check("T7 repair 5D 20 -> 25, 40 -> 35, 4D 20 -> 20, 0 -> 0", Ex2Problem.RepairDose("5D", 30, 20) = 25 AndAlso Ex2Problem.RepairDose("5D", 30, 40) = 35 AndAlso Ex2Problem.RepairDose("4D", 30, 20) = 20 AndAlso Ex2Problem.RepairDose("4D", 30, 0) = 0)
        '--- plain evaluation is deterministic
        Dim xm(p9.NVars - 1) As Integer
        For i As Integer = 0 To p9.NVars - 1 : xm(i) = (p9.Ub(i) + 1) \ 2 : Next
        Dim ra = p9.Evaluate(xm), rb = p9.Evaluate(xm)
        Check("T9 deterministic evaluation", ra.Fpen = rb.Fpen, ra.Fpen.ToString("R", ci))
        Console.WriteLine("INFO mid-pool design: cost " & ra.Cost.Total.ToString("F2", ci) & " TL, T " & ra.T.ToString("F4", ci))

        '--- Q17 fibre shear (IA 2026-11-16): dose 0 = plain, increment >= 0, monotone in dose and in fR
        'fibre vector with the same sections as xm: per beam group (b, h, type, dose)
        Dim toFibre = Function(xp As Integer(), typ As Integer, doseIdx As Integer) As Integer()
                          Dim xf2 As New List(Of Integer)
                          xf2.AddRange(xp.Take(12))
                          For g As Integer = 0 To 2
                              xf2.Add(xp(12 + 2 * g)) : xf2.Add(xp(13 + 2 * g)) : xf2.Add(typ) : xf2.Add(doseIdx)
                          Next
                          Return xf2.ToArray()
                      End Function
        Dim rf0 = p9f.Evaluate(toFibre(xm, 0, 0))
        Check("T8 dose 0 in fibre mode: Fpen = plain Fpen (same sections)", rf0.Fpen = ra.Fpen, rf0.Fpen.ToString("R", ci) & " vs " & ra.Fpen.ToString("R", ci))
        Check("T8 dose 0: Vc,f = Vc,ACI in every beam group", rf0.BeamShr.Values.All(Function(s) s.DeltaVF = 0.0 AndAlso s.Vcf = s.VcPlain))
        Dim q17 As New FibreShearIncrementQ17()
        Near("T8 Q17 model, plain beam: = AciBeam.Vc", q17.ConcreteShear(300, 440, 30, 900, 900, Nothing), RcCore.AciBeam.Vc(300, 440, 30), 1.0E-15)
        Dim okFib As Boolean = True
        Try
            Dim rf3 = p9f.Evaluate(toFibre(xm, 0, 3))
            Check("T8 fibre (4D, 30 kg/m3) evaluates; dVF > 0 in every beam group", rf3.BeamShr.Values.All(Function(s) s.DeltaVF > 0.0), String.Join(";", rf3.BeamShr.Values.Select(Function(s) (s.DeltaVF / 1000.0).ToString("F2", ci))))
            Check("T8 fibre cost item = dose x beam volume x 90", Math.Abs(rf3.Cost.Fibre - rf3.FibreKg * 90.0) < 1.0E-6 AndAlso rf3.Cost.Fibre > 0.0, rf3.Cost.Fibre.ToString("F2", ci))
        Catch ex As Exception
            okFib = False : Console.WriteLine("  " & ex.Message)
        End Try
        Check("T8 fibre evaluation not blocked", okFib)

        Dim incOk As Boolean = True, monoOk As Boolean = True, frOk As Boolean = True
        Dim worst As String = ""
        For Each bw In {250.0, 350.0, 450.0}
            For Each h In {300.0, 450.0, 600.0}
                Dim dd As Double = h - 60.0
                For Each rhoL In {0.003, 0.01, 0.02}
                    Dim asl As Double = rhoL * bw * dd
                    For Each typ In {"4D", "5D"}
                        Dim prev As Double = -1.0
                        For Each dose In {20.0, 25.0, 30.0, 35.0, 40.0}
                            Dim okL As Boolean
                            Dim fib = FibreTable.MakeBeamFibre(typ, 30.0, dose, okL)
                            If Not okL Then Continue For
                            Dim inc As Double = FibreShearIncrementQ17.Increment(bw, dd, 30.0, asl, fib)
                            If inc < 0.0 Then incOk = False : worst = typ & " " & dose & " " & bw & "x" & h
                            If inc < prev - 1.0E-9 Then monoOk = False : worst = typ & " " & dose & " " & bw & "x" & h & " rho " & rhoL
                            prev = inc
                            Dim fib08 = FibreTable.MakeBeamFibre(typ, 30.0, dose, okL, 0.8)
                            If FibreShearIncrementQ17.Increment(bw, dd, 30.0, asl, fib08) > inc + 1.0E-9 Then frOk = False
                        Next
                    Next
                Next
            Next
        Next
        Check("T10 increment dVF >= 0 (all sections, rho_l, types, doses)", incOk, worst)
        Check("T10 dVF non-decreasing in dose (4D 20-40, 5D 25-35)", monoOk, worst)
        Check("T10 fR x 0.8 gives dVF <= fR x 1.0", frOk)
        Dim ok30 As Boolean
        Dim fib30 = FibreTable.MakeBeamFibre("4D", 30.0, 30.0, ok30)
        Dim o1 As New FibreShearO1Format()
        Near("T10 O1 format = VRdF(gammaC = 1.5) for SFRC", o1.ConcreteShear(300, 440, 30, 900, 900, fib30), RcCore.SfrcBeam.VRdF(300, 440, 900, 30, RcCore.SfrcBeam.FctkMin(30), RcCore.SfrcBeam.FFtukShear(fib30.Params), 1.5), 1.0E-15)
        Near("T10 O1 format, plain beam = AciBeam.Vc", o1.ConcreteShear(300, 440, 30, 900, 900, Nothing), RcCore.AciBeam.Vc(300, 440, 30), 1.0E-15)
        Dim incEx As Double = FibreShearIncrementQ17.Increment(300, 440, 30, 900, fib30)
        Console.WriteLine("INFO 300x500 (d 440), As 900, 4D 30 kg/m3: Vc,ACI " & (RcCore.AciBeam.Vc(300, 440, 30) / 1000.0).ToString("F2", ci) & " kN, dVF " & (incEx / 1000.0).ToString("F2", ci) & " kN, O1 VRdF(1.5) " & (o1.ConcreteShear(300, 440, 30, 900, 900, fib30) / 1000.0).ToString("F2", ci) & " kN")

        '--- critical section d from the node (ACI 9.4.3.2): E13 (25x30, d = 240 mm), 1.2D+1.6L, w = 1.2 (30 + 25 x 0.25 x 0.30) + 1.6 x 20 = 70.25 kN/m
        Near("T11 station shear at x = 0 equals face shear (E13)", Math.Abs(BeamShearGravity.ShearAt(a, i2, 12, 0.0)), Math.Abs(f13(1)), 1.0E-9)
        Near("T11 |V(0)| - |V(d)| = w d (E13, 1.2D+1.6L)", Math.Abs(f13(1)) - Math.Abs(BeamShearGravity.ShearAt(a, i2, 12, 0.24)), 70.25 * 0.24, 1.0E-9)
        Dim evT1 = Ex2Evaluator.Evaluate(spec5, Ex2Problem.FromSizes(t1), pr, New AciPlainShear(), Ex2Options.Campaign1())
        Dim vuB2 As Double = evT1.BeamShr("B_S2").Vu / 1000.0
        Dim vMaxNode As Double = 0.0
        For Each ei In {11, 12, 13}
            For Each cj In Enumerable.Range(0, a.Combos.Count).Where(Function(k) a.Combos(k).Main)
                vMaxNode = Math.Max(vMaxNode, Math.Max(Math.Abs(a.Face(cj)(ei)(1)), Math.Abs(a.Face(cj)(ei)(7))))
            Next
        Next
        Near("T11 group B_S2 Vu = max node shear - w d (UDL, 1.2D+1.6L governs)", vuB2, vMaxNode - 70.25 * 0.24, 1.0E-9)

        '--- ACI 6.2.5 non-sway limit (34 + 12 M1/M2 <= 40; M1/M2 < 0 single curvature)
        Check("T12 non-sway limit: single curvature equal moments = 22", Math.Abs(ColumnDesignGravity.NonSwayLimit(1.0, 1.0) - 22.0) < 1.0E-12)
        Check("T12 non-sway limit: double curvature M1/M2 = +1 -> min(46, 40) = 40", Math.Abs(ColumnDesignGravity.NonSwayLimit(1.0, -1.0) - 40.0) < 1.0E-12)
        Check("T12 non-sway limit: double curvature M1/M2 = +0.5 -> 40", Math.Abs(ColumnDesignGravity.NonSwayLimit(2.0, -1.0) - 40.0) < 1.0E-12)
        Check("T12 non-sway limit: single curvature M1/M2 = -0.5 -> 28", Math.Abs(ColumnDesignGravity.NonSwayLimit(2.0, 1.0) - 28.0) < 1.0E-12)
        'lu/r = 3300 / 90 = 36.7: 1.0 in double curvature (limit 40), magnified (frozen) in single curvature (limit 22)
        Check("T12 delta_ns = 1 for lu/r 36.7 in double curvature", ColumnDesignGravity.DeltaNsNonSway(30, 300, 300, 3300, 500000, 0.5, 2.0E7, -1.0E7) = 1.0)
        Near("T12 delta_ns = frozen DeltaNs in single curvature", ColumnDesignGravity.DeltaNsNonSway(30, 300, 300, 3300, 500000, 0.5, 2.0E7, 1.0E7), ColumnDesign.DeltaNs(30, 300, 300, 3300, 500000, 0.5, 2.0E7, 1.0E7), 1.0E-15)

        '--- column ties: ACI 10.7.6.5.2 spacing when shear reinforcement is required, 25.7.2.1 always
        Dim tieOk As Boolean = True
        For Each rr In {ra, evT1}
            For Each c In rr.ColRes.Values
                Dim lim As Double = Math.Min(Math.Min(16.0 * c.BarDia, 48.0 * 10.0), Math.Min(c.B, c.H))
                If c.Vu > 0.5 * 0.75 * c.Vc Then lim = Math.Min(lim, (c.H - 60.0) / 2.0)
                If c.S > lim + 1.0E-9 Then tieOk = False
            Next
        Next
        Check("T13 column tie spacing <= 25.7.2.1 and 10.7.6.5.2 limits", tieOk)

        '--- second package: shear variants, rho_l, validity, fR scale on flexure and shear
        Near("T14 main uses rho_l = AsTop", q17.ConcreteShear(300, 440, 30, 1500, 600, fib30), RcCore.AciBeam.Vc(300, 440, 30) + FibreShearCommon.Increment(300, 440, 30, 1500, fib30, 1.0), 1.0E-15)
        Near("T14 O1 variant uses AsBot (Ö1 BeamDesign.VcOf one-to-one)", o1.ConcreteShear(300, 440, 30, 1500, 600, fib30), RcCore.SfrcBeam.VRdF(300, 440, 600, 30, RcCore.SfrcBeam.FctkMin(30), RcCore.SfrcBeam.FFtukShear(fib30.Params), 1.5, 0.0), 1.0E-15)
        Dim mcl As New FibreShearMc2010Local()
        Near("T14 MC2010-local = Vc + dVF(gc=1.5) / phi", mcl.ConcreteShear(300, 440, 30, 1500, 600, fib30), RcCore.AciBeam.Vc(300, 440, 30) + FibreShearCommon.Increment(300, 440, 30, 1500, fib30, 1.5) / 0.75, 1.0E-15)
        Check("T14 ordering main >= MC2010-local (fibre term)", q17.ConcreteShear(300, 440, 30, 1500, 600, fib30) >= mcl.ConcreteShear(300, 440, 30, 1500, 600, fib30))
        Dim badFib As New BeamFibre With {.Params = New RcCore.SfrcParams With {.FR1 = 4.0, .FR3 = 1.8}, .DoseKgM3 = 30}
        Check("T14 fR3/fR1 <= 0.5 -> dVF = 0 (MC2010 5.6-3)", FibreShearCommon.Increment(300, 440, 30, 1500, badFib, 1.0) = 0.0 AndAlso Not FibreShearCommon.FibreValid(badFib))
        Dim gridOk As Boolean = True
        For Each typ In {"4D", "5D"}
            For Each dose In {20.0, 25.0, 30.0, 35.0, 40.0}
                Dim okG As Boolean
                Dim fg = FibreTable.MakeBeamFibre(typ, 30.0, dose, okG)
                If okG AndAlso Not FibreShearCommon.FibreValid(fg) Then gridOk = False
            Next
        Next
        Check("T14 every C30 grid point satisfies fR3/fR1 > 0.5", gridOk)
        Dim ok06 As Boolean
        Dim fib06 = FibreTable.MakeBeamFibre("4D", 30.0, 30.0, ok06, 0.6)
        Check("T14 fR x 0.6 scales fR1 and fR3", Math.Abs(fib06.Params.FR1 - 0.6 * fib30.Params.FR1) < 1.0E-12 AndAlso Math.Abs(fib06.Params.FR3 - 0.6 * fib30.Params.FR3) < 1.0E-12)
        Check("T14 fR x 0.6: lower flexural strength and lower dVF (flexure and shear together)", BeamDesignGravity.PhiMn(300, 500, 440, 900, 30, fib06) < BeamDesignGravity.PhiMn(300, 500, 440, 900, 30, fib30) AndAlso FibreShearCommon.Increment(300, 440, 30, 900, fib06, 1.0) < FibreShearCommon.Increment(300, 440, 30, 900, fib30, 1.0))
        Dim p9f06 As New Ex2Problem(spec9, pr, True, New FibreShearIncrementQ17(), 0.6)
        p9f06.Opts = Ex2Options.Campaign1()
        Dim r06 = p9f06.Evaluate(toFibre(xm, 0, 3)), r10 = p9f.Evaluate(toFibre(xm, 0, 3))
        Check("T14 fR x 0.6 problem: dVF smaller in every beam group", p9f.BeamGroups.All(Function(g) r06.BeamShr(g).DeltaVF < r10.BeamShr(g).DeltaVF))

        '--- servis (a): g5 is no longer a constraint
        Check("T15 ACI 9.3.1.1 not in the constraint list (IA servis (a))", Not ra.Violations.Any(Function(v) v.Item1.StartsWith("g5")))

        '--- ACI 6.6.4.4.1 Q
        Check("T16 storey Q computed (3 storeys, > 0)", ra.StoreyQ IsNot Nothing AndAlso ra.StoreyQ.Length = 4 AndAlso Enumerable.Range(1, 3).All(Function(k) ra.StoreyQ(k) > 0.0), If(ra.StoreyQ Is Nothing, "", String.Join(";", ra.StoreyQ.Skip(1).Select(Function(q) q.ToString("F4", ci)))))
        Dim xSmall = CType(p9.Lb.Clone(), Integer()), xBig = CType(p9.Lb.Clone(), Integer())
        For i As Integer = 0 To 11 : xBig(i) = p9.Ub(i) : Next
        For i As Integer = 12 To 17 : xSmall(i) = 3 : xBig(i) = 3 : Next
        Dim qS = p9.Evaluate(xSmall).StoreyQ, qB = p9.Evaluate(xBig).StoreyQ
        Check("T16 larger columns -> smaller Q (every storey)", Enumerable.Range(1, 3).All(Function(k) qB(k) < qS(k)), "small " & String.Join(";", qS.Skip(1).Select(Function(q) q.ToString("F4", ci))) & " big " & String.Join(";", qB.Skip(1).Select(Function(q) q.ToString("F4", ci))))
        Console.WriteLine("INFO Q mid-pool " & String.Join(";", ra.StoreyQ.Skip(1).Select(Function(q) q.ToString("F4", ci))) & ", 250x300 columns " & String.Join(";", qS.Skip(1).Select(Function(q) q.ToString("F4", ci))))
        Dim pNoQ As New Ex2Problem(spec9, pr, False, New AciPlainShear())
        pNoQ.Opts = Ex2Options.Campaign1() : pNoQ.Opts.QCheck = False
        Check("T16 --qcheck 0: no Q, all storeys non-sway", pNoQ.Evaluate(xm).StoreyQ Is Nothing)

        '--- ACI 6.4.2 pattern analysis
        Dim ap = RcCombos2D.Analyze(spec5, Defs(t1), False, True)
        Check("T17 patterned envelope: 2 main + 4 x 6 pattern combinations", ap.Combos.Count = 26, ap.Combos.Count.ToString())
        Dim sumQe As Double = 0.0
        For ei As Integer = 0 To 20
            If Not ap.Frame.IsColumn(ei) Then sumQe += ap.VerticalReaction("Q_E" & ei)
        Next
        Near("T17 sum of per-beam live patterns = Q reaction 960 kN", sumQe, 960.0, 1.0E-9)
        Dim rpS = BeamDesignGravity.DesignGroup(a, "B_S2", 250, 300, 30, Nothing), rpP = BeamDesignGravity.DesignGroup(ap, "B_S2", 250, 300, 30, Nothing)
        Check("T17 patterned MuBot >= single-pattern MuBot (B_S2)", rpP.MuBot >= rpS.MuBot - 1.0E-6, (rpS.MuBot / 1.0E6).ToString("F2", ci) & " -> " & (rpP.MuBot / 1.0E6).ToString("F2", ci))

        '--- ACI 24.2 deflection: Hermite mid-span formula vs a simply supported FEM beam (sign convention of Ry)
        Dim fmS As New FrameModel2D With {.Spec = spec5}
        Dim secS = RcFrameBuilder.MakeSection("B", 300, 500, 30, 25.0)
        fmS.Model.Sections.Add(secS) : fmS.Model.DiaphragmCount = 0
        Dim n1 As New FemCore.FemNode With {.Name = "A", .X = 0, .Y = 0, .Z = 0}, n2 As New FemCore.FemNode With {.Name = "B", .X = 6, .Y = 0, .Z = 0}
        For Each rI In {0, 1, 2, 3, 5} : n1.Restraint(rI) = True : Next
        For Each rI In {1, 2, 3, 5} : n2.Restraint(rI) = True : Next
        fmS.Model.Nodes.Add(n1) : fmS.Model.Nodes.Add(n2)
        Dim eS As New FemCore.FemElement With {.Name = "E", .NI = 0, .NJ = 1, .Section = secS, .Group = "B", .Story = 1, .Length = 6.0}
        eS.Axes(0, 0) = 1 : eS.Axes(1, 2) = 1 : eS.Axes(2, 1) = -1
        fmS.Model.Elements.Add(eS)
        Dim pw As New FemCore.FemPattern With {.Name = "W", .SelfWeightMultiplier = 0.0}
        pw.DistLoads.Add(New FemCore.FemDistLoad With {.Element = 0, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 1.0, .V1 = 10.0, .V2 = 10.0})
        fmS.Model.Patterns.Add(pw)
        Dim rS = New FemCore.FemStatic(fmS.Model).Solve()
        Dim eiS As Double = secS.E * secS.I33
        Near("T18 Hermite mid-span deflection = 5 w L^4 / (384 EI) (simply supported FEM beam)", Deflection2D.MidspanDeflection(rS.U, fmS, 0, 10.0, eiS), 5.0 * 10.0 * 6.0 ^ 4 / (384.0 * eiS), 1.0E-6)
        Dim dRows = Deflection2D.Report(spec9, p9.Decode(xm), ra)
        Check("T18 deflection report: 9 beams, all deflections > 0, Ie/Ig in (0, 1]", dRows.Count = 9 AndAlso dRows.All(Function(rw) rw.DeltaD > 0.0 AndAlso rw.DeltaL > 0.0 AndAlso rw.IeOverIgDL > 0.0 AndAlso rw.IeOverIgDL <= 1.0))

        '--- PCR-21 (2026-11-16): column lu = clear height, Ö1 rule (half of the deepest beam at each end, 0 at the base); Q keeps lc = 3.3 m
        Dim xPilot() As Integer = {1, 0, 2, 0, 1, 1, 4, 0, 0, 0, 0, 1, 0, 5, 0, 5, 0, 5}          'Q20 pilot vector (beams 250 x 550)
        Dim rp = p9.Evaluate(xPilot)
        Dim ap9 = rp.Analysis
        Dim eS1 = Enumerable.Range(0, 21).First(Function(i) ap9.Frame.Model.Elements(i).Group = "C_Inter_S1")
        Dim eS2 = Enumerable.Range(0, 21).First(Function(i) ap9.Frame.Model.Elements(i).Group = "C_Inter_S2")
        Near("T19 lu storey 1 = 3.3 - 0.55/2 = 3.025 m (base end 0)", ColumnDesignGravity.ClearHeight(ap9.Frame, eS1), 3.025, 1.0E-12)
        Near("T19 lu storey 2 = 3.3 - 0.55 = 2.75 m (beams 550 above and below)", ColumnDesignGravity.ClearHeight(ap9.Frame, eS2), 2.75, 1.0E-12)
        Near("T19 column result reports lu = 2 750 mm (C_Inter_S2)", rp.ColRes("C_Inter_S2").LuMin, 2750.0, 1.0E-12)
        Near("T19 Critic example: 300 column, 550 beams: lu/r = 2750/90 = 30.56", 2750.0 / (0.3 * 300.0), 30.5556, 1.0E-4)
        Check("T19 lu/r 30.6 < 40: delta_ns = 1 in double curvature (non-sway limit)", ColumnDesignGravity.DeltaNsNonSway(30, 300, 300, 2750, 500000, 0.5, 2.0E7, -1.0E7) = 1.0)
        Check("T19 lu/r 30.6 > 22: magnified in single curvature equal moments", ColumnDesignGravity.DeltaNsNonSway(30, 300, 300, 2750, 500000, 0.5, 1.0E7, 1.0E7) > 1.0)
        Check("T19 net lu gives a smaller delta than the axis length 3.3 m", ColumnDesignGravity.DeltaNsNoExemption(30, 300, 250, 2750, 600000, 0.6) < ColumnDesignGravity.DeltaNsNoExemption(30, 300, 250, 3300, 600000, 0.6))

        '--- PCR-20: ACI 6.2.6 (2nd-order M <= 1.4 x 1st-order M) as constraint g6
        Near("T20 ratio = delta when the moment governs", ColumnDesignGravity.Ratio626Of(1.95, 5.0E7, 5.0E7, 1.0E7), 1.95, 1.0E-15)
        Near("T20 ratio = delta when e_min governs (M = 0)", ColumnDesignGravity.Ratio626Of(1.6, 0.0, 0.0, 1.0E7), 1.6, 1.0E-15)
        Near("T20 ratio with delta_s: delta_ns |Mns + ds Ms| / |M|", ColumnDesignGravity.Ratio626Of(1.2, 4.0E7, 5.0E7, 1.0E6), 1.5, 1.0E-15)
        Check("T20 ratio capped (buckling, delta infinite)", ColumnDesignGravity.Ratio626Of(Double.PositiveInfinity, 1.0, 1.0, 0.0) = ColumnDesignGravity.RatioCap)
        Dim g6Name As String = "g6 col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)"
        Check("T20 g6 in the constraint list", rp.Violations.Any(Function(v) v.Item1 = g6Name))
        Dim maxR = rp.ColRes.Values.Max(Function(c) c.Ratio626)
        Near("T20 g6 violation = max(0, max ratio / 1.4 - 1) (pilot)", rp.Violations.First(Function(v) v.Item1 = g6Name).Item2 + 1.0, Math.Max(0.0, maxR / 1.4 - 1.0) + 1.0, 1.0E-12)
        Dim cS2 = rp.ColRes("C_Inter_S2")
        Near("T20 pilot C_Inter_S2 (250 x 300) out-of-plane ratio = delta_2 (e_min only)", cS2.Ratio626Out, cS2.Delta2Max, 1.0E-12)
        Console.WriteLine("INFO pilot (old Q20 vector) with PCR-20/21/22: cost " & rp.Cost.Total.ToString("F2", ci) & " T " & rp.T.ToString("F4", ci) & "; C_Inter_S2 ratio in/out " & cS2.Ratio626In.ToString("F4", ci) & "/" & cS2.Ratio626Out.ToString("F4", ci) & ", D/C " & cS2.DcrMax.ToString("F4", ci))
        Dim rLow = p9.Evaluate(p9.Lb)                    'all 250 x 300: slender columns
        Dim lowMax = rLow.ColRes.Values.Max(Function(c) c.Ratio626)
        Check("T20 violation example: all 250 x 300 -> some ratio > 1.4, g6 > 0, T > 0", lowMax > 1.4 AndAlso rLow.Violations.First(Function(v) v.Item1 = g6Name).Item2 > 0.0 AndAlso rLow.T > 0.0, "max ratio " & lowMax.ToString("F4", ci))

        '--- PCR-22: Q > 0.05 storey: no slenderness exemption, delta_s = 1/(1-Q), Q <= 1/3 (g7), M2s from a braced analysis
        Near("T21 DeltaNsNoExemption = frozen DeltaNs above lu/r 22", ColumnDesignGravity.DeltaNsNoExemption(30, 300, 300, 3300, 500000, 0.5), ColumnDesign.DeltaNs(30, 300, 300, 3300, 500000, 0.5, 2.0E7, 1.0E7), 1.0E-15)
        Check("T21 no exemption: lu/r = 2750/180 = 15.3 < 22 still magnified (> 1)", ColumnDesignGravity.DeltaNsNoExemption(30, 600, 300, 2750, 2000000, 0.5) > 1.0 AndAlso ColumnDesign.DeltaNs(30, 600, 300, 2750, 2000000, 0.5, 1.0E7, 1.0E7) = 1.0)
        Dim stL = rLow.Stability
        Dim swayK = Enumerable.Range(1, 3).Where(Function(k) Not stL.NonSway(k)).ToArray()
        Check("T21 all-250 x 300 design has a sway storey (Q > 0.05)", swayK.Length > 0, String.Join(";", stL.Q.Skip(1).Select(Function(q) q.ToString("F4", ci))))
        Check("T21 delta_s = 1/(1-Q) in sway storeys, 1 in non-sway storeys", Enumerable.Range(1, 3).All(Function(k) Math.Abs(stL.DeltaS(k) - If(stL.NonSway(k), 1.0, 1.0 / (1.0 - stL.Q(k)))) < 1.0E-12))
        Check("T21 braced analysis built when a storey is sway", stL.Braced IsNot Nothing AndAlso stL.Braced.Braced)
        Check("T21 symmetric frame and load: M2s ~ 0 and gravity drift ~ 0 (reported)", Enumerable.Range(1, 3).All(Function(k) stL.MsMaxKNm(k) < 1.0E-6 AndAlso stL.DriftMm(k) < 1.0E-6), String.Join(";", stL.MsMaxKNm.Skip(1).Select(Function(v) v.ToString("E2", ci))) & " / " & String.Join(";", stL.DriftMm.Skip(1).Select(Function(v) v.ToString("E2", ci))))
        Dim swayCols = rLow.ColRes.Values.Where(Function(c) c.AnySway).ToList()
        Check("T21 sway-storey columns: delta_ns > 1 (exemption not used), delta_s recorded", swayCols.Count > 0 AndAlso swayCols.All(Function(c) c.Delta3Max > 1.0 AndAlso Math.Abs(c.DeltaSMax - swayK.Select(Function(k) stL.DeltaS(k)).Max()) < 0.5))
        Dim g7Name As String = "g7 storey Q <= 1/3 (delta_s <= 1.5; ACI 6.6.4.6.2(a))"
        Near("T21 g7 = max(0, 3 Qmax - 1)", rLow.Violations.First(Function(v) v.Item1 = g7Name).Item2 + 1.0, Math.Max(0.0, stL.Q.Skip(1).Max() * 3.0 - 1.0) + 1.0, 1.0E-12)
        'general code path: asymmetric ACI 6.4.2 live patterns give M2s <> 0; applying delta_s must not lower the column demand
        Dim sizesLow As New Dictionary(Of String, Double())
        For Each g In p9.ColGroups : sizesLow(g) = {250.0, 300.0} : Next
        For Each g In p9.BeamGroups : sizesLow(g) = {250.0, 450.0} : Next
        Dim apat = RcCombos2D.Analyze(spec9, Defs(sizesLow), False, True)
        Dim stP = Stability2D.Assess(spec9, Defs(sizesLow), apat)
        Check("T21 patterned envelope: some sway storey with M2s > 0", stP.Braced IsNot Nothing AndAlso Enumerable.Range(1, 3).Any(Function(k) Not stP.NonSway(k) AndAlso stP.MsMaxKNm(k) > 0.01), String.Join(";", stP.MsMaxKNm.Skip(1).Select(Function(v) v.ToString("F3", ci))))
        Dim stP1 As New StoreyStability With {.Q = stP.Q, .NonSway = stP.NonSway, .DeltaS = stP.DeltaS.Select(Function(v) 1.0).ToArray(), .DriftMm = stP.DriftMm, .MsMaxKNm = stP.MsMaxKNm, .Braced = stP.Braced, .SwayFace = stP.SwayFace, .StoreyH = stP.StoreyH}
        'sign and size of the net storey sway forces: floor-average lateral displacement of the unbraced frame = that of the frame under H_k alone (within 10 %)
        Dim fmH = RcFrameBuilder2D.Build(spec9, RcCombos2D.MakeSections(spec9, Defs(sizesLow)))
        fmH.Model.Patterns.Clear()
        For lev As Integer = 1 To 3
            Dim pH As New FemCore.FemPattern With {.Name = "H" & lev, .SelfWeightMultiplier = 0.0}
            For i As Integer = 0 To 3
                Dim nl As New FemCore.FemNodalLoad With {.Node = fmH.NodeAt(i, lev)}
                nl.F(0) = 0.25 : pH.NodalLoads.Add(nl)
            Next
            fmH.Model.Patterns.Add(pH)
        Next
        Dim rH = New FemCore.FemStatic(fmH.Model).Solve()
        Dim worstSway As Double = 0.0, bigU As Double = 0.0
        For ci2 As Integer = 0 To apat.Combos.Count - 1
            Dim fac = apat.Factors(apat.Combos(ci2))
            For lev As Integer = 1 To 3
                Dim uT As Double = 0.0, uH As Double = 0.0
                For i As Integer = 0 To 3
                    Dim nd As Integer = fmH.NodeAt(i, lev) * 6
                    For pz As Integer = 0 To fac.Length - 1 : uT += fac(pz) * apat.R.U(pz)(nd) / 4.0 : Next
                    For l2 As Integer = 1 To 3 : uH += stP.StoreyH(ci2)(l2) * rH.U(l2 - 1)(nd) / 4.0 : Next
                Next
                If Math.Abs(uT) > 1.0E-5 Then worstSway = Math.Max(worstSway, Math.Abs(uH - uT) / Math.Abs(uT)) : bigU = Math.Max(bigU, Math.Abs(uT))
            Next
        Next
        Check("T21 net storey forces reproduce the floor-average sway of the unbraced frame (sign; within 10 %)", bigU > 0.0 AndAlso worstSway < 0.1, "max rel diff " & worstSway.ToString("F4", ci) & ", max |u| " & (bigU * 1000.0).ToString("F3", ci) & " mm")
        'delta_s acts on M2s only: |M2ns + delta_s M2s| can rise or fall at an end (sign of M2s), so only "changes the result and raises it somewhere" is tested
        Dim anyUp As Boolean = False, anyDiff As Boolean = False
        For Each g In p9.ColGroups
            Dim cA = ColumnDesignGravity.CheckGroup(apat, g, 450, 600, 30, 0.02, 20, False, stP), cB = ColumnDesignGravity.CheckGroup(apat, g, 450, 600, 30, 0.02, 20, False, stP1)
            If Math.Abs(cA.DcrMax - cB.DcrMax) > 1.0E-9 Then anyDiff = True
            If cA.Ratio626In > cB.Ratio626In + 1.0E-9 OrElse cA.DcrMax > cB.DcrMax + 1.0E-9 Then anyUp = True
        Next
        Check("T21 delta_s on M2s is applied (patterns: D/C changes, rises in some group)", anyDiff AndAlso anyUp)
        Dim pNoQ2 As New Ex2Problem(spec9, pr, False, New AciPlainShear())
        pNoQ2.Opts = Ex2Options.Campaign1() : pNoQ2.Opts.QCheck = False
        Dim rNoQ = pNoQ2.Evaluate(p9.Lb)
        Check("T21 --qcheck 0: g7 inactive, no delta_s", rNoQ.Stability Is Nothing AndAlso rNoQ.Violations.First(Function(v) v.Item1 = g7Name).Item2 = 0.0 AndAlso rNoQ.ColRes.Values.All(Function(c) c.DeltaSMax = 1.0))

        '--- tool output: Av/s columns (strength, min, provided) and spacing label
        Dim sh = rp.BeamShr("B_S1")
        Near("T22 AvPerS = max(AvS_strength, AvS_min)", sh.AvPerS, Math.Max(sh.AvSStrength, sh.AvSMin), 1.0E-15)
        Near("T22 AvS_provided = 2 x pi 10^2/4 / s", sh.AvSProvided, 2.0 * Math.PI * 100.0 / 4.0 / sh.S, 1.0E-12)
        Check("T22 spacing label names the source of the required Av/s", sh.StirrupGov <> "smax", sh.StirrupGov)


        '=== PCR-23 / 24 / 25 (2026-11-17, post-hoc; Review_Round_Ex2_sonuc_ic A1, _dis R1-R3) ====================================================
        '--- T23 PCR-23: SFRC Required <-> PhiMn consistency (scan of the Critic: b 250, h 400-600, 5 type/dose, Mu 60-250 kN m)
        Dim fibCases As New List(Of Tuple(Of String, Double))
        For Each tdz In {Tuple.Create("4D", 20.0), Tuple.Create("4D", 30.0), Tuple.Create("4D", 40.0), Tuple.Create("5D", 25.0), Tuple.Create("5D", 35.0)} : fibCases.Add(tdz) : Next
        Dim nCase As Integer = 0, nInf As Integer = 0, worstDc As Double = 0.0, worstTight As Double = 10.0, consOk As Boolean = True, lcsOk As Boolean = True
        For Each hh In {400.0, 450.0, 500.0, 550.0, 600.0}
            For Each tdz In fibCases
                Dim okF As Boolean
                Dim fibT = FibreTable.MakeBeamFibre(tdz.Item1, 30.0, tdz.Item2, okF)
                For muK As Double = 60.0 To 250.01 Step 10.0
                    Dim dd As Double = hh - 60.0, mu As Double = muK * 1.0E6
                    Dim req As Double = BeamDesignGravity.RequiredFace(mu, 0.0, 250, hh, dd, 30, fibT)
                    If Double.IsInfinity(req) Then nInf += 1 : Continue For
                    nCase += 1
                    Dim dcv As Double = mu / BeamDesignGravity.PhiMn(250, hh, dd, req, 30, fibT)
                    worstDc = Math.Max(worstDc, dcv)
                    If dcv > 1.0 + 1.0E-9 Then consOk = False
                    worstTight = Math.Min(worstTight, mu / BeamDesignGravity.PhiMn(250, hh, dd, req * (1.0 - 0.000001), 30, fibT))
                    Dim lOut As Double
                    Dim rf = BeamDesignGravity.FlexFixed(250, hh, dd, req, 30, fibT, 0.0, lOut)
                    If Math.Abs(Math.Max(hh - rf.C, 0.2 * hh) - lOut) > 0.00001 Then lcsOk = False
                Next
            Next
        Next
        Check("T23 SFRC: Mu / phiMn(Required(Mu)) <= 1 + 1e-9 over the scan (" & nCase & " finite cases, " & nInf & " section-too-small)", consOk AndAlso nCase >= 400, "max " & worstDc.ToString("R", ci))
        Check("T23 SFRC: Required is minimal (1 ppm less steel -> D/C > 1) over the scan", worstTight > 1.0, "min " & worstTight.ToString("R", ci))
        Check("T23 SFRC: the capacity uses its own fixed-point lcs = max(h - c, 0.2 h)", lcsOk)
        Dim okF0 As Boolean
        Dim fibF0 = FibreTable.MakeBeamFibre("4D", 30.0, 40.0, okF0)
        Dim reqF0 As Double = BeamDesignGravity.RequiredFace(112.1 * 1.0E6, 0.0, 250, 500, 440, 30, fibF0)
        Check("T23 F0 B_S1 bottom (250 x 500, 4D/40, Mu 112.10 kN m): As = 694.8 +- 0.5 mm2 (stale-lcs code gave 687.5)", Math.Abs(reqF0 - 694.8) <= 0.5, reqF0.ToString("F2", ci))
        Near("T23 plain path unchanged: RequiredFace(Nt = 0) = AciBeam.RequiredAs", BeamDesignGravity.RequiredFace(150.0E6, 0.0, 250, 500, 440, 30, Nothing), RcCore.AciBeam.RequiredAs(150.0E6, 250, 440, 30, 420), 0.0)

        'PCR-23(3) plain consistency (FlexCheck): frozen AciBeam.RequiredAs in the transition zone vs the raised result (b 250-450, h 300-600, Mu 20-600 kN m)
        Dim gapOld As Double = 0.0, gapNew As Double = 0.0, nTr As Integer = 0, maxRaise As Double = 0.0, raiseWhenOk As Integer = 0
        For Each bb In {250.0, 300.0, 350.0, 400.0, 450.0}
            For hh2 As Double = 300.0 To 600.0 Step 50.0
                For muK As Double = 20.0 To 600.0 Step 2.0
                    Dim d2 As Double = hh2 - 60.0, mu2 As Double = muK * 1.0E6
                    Dim aOld As Double = BeamDesignGravity.RequiredFace(mu2, 0.0, bb, hh2, d2, 30, Nothing, False)
                    If Double.IsInfinity(aOld) Then Continue For
                    If RcCore.AciBeam.Flexure(bb, d2, aOld, 30, 420).EpsT < 0.004 Then Continue For
                    nTr += 1
                    gapOld = Math.Max(gapOld, mu2 / RcCore.AciBeam.Flexure(bb, d2, aOld, 30, 420).PhiMn)
                    Dim aNew As Double = BeamDesignGravity.RequiredFace(mu2, 0.0, bb, hh2, d2, 30, Nothing, True)
                    maxRaise = Math.Max(maxRaise, aNew / aOld - 1.0)
                    If mu2 / RcCore.AciBeam.Flexure(bb, d2, aOld, 30, 420).PhiMn <= 1.0 AndAlso aNew <> aOld Then raiseWhenOk += 1
                    If RcCore.AciBeam.Flexure(bb, d2, aNew, 30, 420).EpsT >= 0.004 Then gapNew = Math.Max(gapNew, mu2 / RcCore.AciBeam.Flexure(bb, d2, aNew, 30, 420).PhiMn)
                Next
            Next
        Next
        'transition zone: d(phi Mn)/dAs is small, so closing a 0.04 % moment gap can need ~1.7 % more steel (then often eps_t < 0.004 -> g3); RaiseUntil overshoots the
        'smallest sufficient relative raise by at most x 2 (geometric steps)
        Check("T23 plain FlexCheck: no raise when the frozen RequiredAs already holds; largest raise < 5 % (transition zone)", raiseWhenOk = 0 AndAlso maxRaise < 0.05, "max raise " & maxRaise.ToString("E3", ci))
        Check("T23 plain FlexCheck: Mu / phiMn(As) <= 1 + 1e-12 where eps_t >= 0.004 (" & nTr & " cases; frozen RequiredAs max " & gapOld.ToString("F6", ci) & ")", gapNew <= 1.0 + 1.0E-12 AndAlso nTr > 1000, gapNew.ToString("R", ci))

        '--- T24 PCR-24: beam axial tension (flexure ACI 22.2 equilibrium; shear ACI 22.5.7.1)
        Dim nt As Double = 15000.0
        Dim reqP0 As Double = BeamDesignGravity.RequiredFace(120.0E6, 0.0, 250, 500, 440, 30, Nothing), reqP1 As Double = BeamDesignGravity.RequiredFace(120.0E6, nt, 250, 500, 440, 30, Nothing)
        Dim asbP As Double = RcCore.AciBeam.RequiredAs(120.0E6 - nt * 190.0, 250, 440, 30, 420)
        Near("T24 plain: As(Mu, Nt) = As(Mu - Nt (d - h/2)) + Nt / (phi fy)", reqP1, asbP + nt / (RcCore.AciBeam.Flexure(250, 440, asbP, 30, 420).Phi * 420.0), 1.0E-12)
        Check("T24 plain: tension raises As (Nt 15 kN)", reqP1 > reqP0, reqP0.ToString("F2", ci) & " -> " & reqP1.ToString("F2", ci))
        Near("T24 plain: face D/C = 1 at the required steel (axial)", BeamDesignGravity.FaceDc(120.0E6, nt, 250, 500, 440, reqP1, 30, Nothing), 1.0, 1.0E-9)
        Dim reqS1 As Double = BeamDesignGravity.RequiredFace(112.1E6, nt, 250, 500, 440, 30, fibF0)
        Check("T24 SFRC: face D/C <= 1 + 1e-6 at the required steel (axial), As above the Nt = 0 value", BeamDesignGravity.FaceDc(112.1E6, nt, 250, 500, 440, reqS1, 30, fibF0) <= 1.0 + 0.000001 AndAlso reqS1 > reqF0, reqS1.ToString("F2", ci))
        Dim phiEq As Double = RcCore.AciBeam.Flexure(250, 440, asbP, 30, 420).Phi
        Dim aEq As Double = (reqP1 * 420.0 - nt / phiEq) / (0.85 * 30.0 * 250.0)
        Near("T24 plain: moment equilibrium about the centroid with Pn = Nt/phi (yielding steel)", phiEq * (0.85 * 30.0 * 250.0 * aEq * (250.0 - aEq / 2.0) + reqP1 * 420.0 * (440.0 - 250.0)), 120.0E6, 1.0E-9)
        Dim pAx As New Ex2Problem(spec9, pr, False, New AciPlainShear())
        pAx.Opts = New Ex2Options With {.Patterned = False, .BeamAxial = True}
        Dim rAx = pAx.Evaluate(xPilot), rNo = p9.Evaluate(xPilot)
        Check("T24 station N tension-positive: storey beams in tension, roof beams in compression (ETABS V2-a)", rAx.BeamRes("B_S1").NtMax > 5000.0 AndAlso rAx.BeamRes("B_S2").NtMax > 5000.0 AndAlso rAx.BeamRes("B_S3").NtMax = 0.0, String.Join(";", rAx.BeamRes.Values.Select(Function(b) (b.NtMax / 1000.0).ToString("F2", ci))))
        Check("T24 axial on: AsBot, AsTop >= axial off; Vc factor < 1 where in tension", p9.BeamGroups.All(Function(g) rAx.BeamRes(g).AsBot >= rNo.BeamRes(g).AsBot AndAlso rAx.BeamRes(g).AsTop >= rNo.BeamRes(g).AsTop) AndAlso rAx.BeamShr("B_S1").AxialFactor < 1.0 AndAlso rAx.BeamShr("B_S3").AxialFactor = 1.0)
        Dim bs1 = rAx.BeamRes("B_S1")
        Near("T24 ACI 22.5.7.1 factor = 1 - Nt / (3.5 b h)", rAx.BeamShr("B_S1").AxialFactor, 1.0 - bs1.NtMax / (3.5 * bs1.B * bs1.H), 1.0E-12)
        Check("T24 axial off: Vc factor exactly 1", rNo.BeamShr.Values.All(Function(s2) s2.AxialFactor = 1.0))

        '--- T25 PCR-25: patterns in the design envelope by default (beams and columns), g8 flexural D/C
        Dim pNew As New Ex2Problem(spec9, pr, False, New AciPlainShear())
        Dim pNewF As New Ex2Problem(spec9, pr, True, New FibreShearIncrementQ17())
        Check("T25 default options = campaign 2 (patterns on, beam axial on, g8/FlexCheck on, Q on); Campaign1() all off", pNew.Opts.Patterned AndAlso pNew.Opts.BeamAxial AndAlso pNew.Opts.FlexCheck AndAlso pNew.Opts.QCheck AndAlso Not Ex2Options.Campaign1().Patterned AndAlso Not Ex2Options.Campaign1().BeamAxial AndAlso Not Ex2Options.Campaign1().FlexCheck)
        Dim rNew = pNew.Evaluate(xPilot)
        Check("T25 patterned evaluation: 26 design combinations", rNew.Analysis.Combos.Where(Function(c) c.Main).Count() = 26, rNew.Analysis.Combos.Count.ToString())
        Check("T25 patterned MuBot / MuTop >= single-pattern (every beam group)", p9.BeamGroups.All(Function(g) rNew.BeamRes(g).MuBot >= rAx.BeamRes(g).MuBot - 1.0E-6 AndAlso rNew.BeamRes(g).MuTop >= rAx.BeamRes(g).MuTop - 1.0E-6), String.Join(";", p9.BeamGroups.Select(Function(g) (rAx.BeamRes(g).MuBot / 1.0E6).ToString("F1", ci) & "->" & (rNew.BeamRes(g).MuBot / 1.0E6).ToString("F1", ci))))
        Dim g8Name As String = "g8 beam flexure D/C <= 1 (ACI 9.5.1; envelope incl. ACI 6.4.2 patterns)"
        Check("T25 g8 in the constraint list and 0 for a designed (finite) section", rNew.Violations.Any(Function(v) v.Item1 = g8Name AndAlso v.Item2 = 0.0))
        Check("T25 g8 absent under the campaign-1 rules (FlexCheck off)", Not rNo.Violations.Any(Function(v) v.Item1 = g8Name))
        Dim rndT As New Random(20261117)
        Dim nRnd As Integer = 0, dcMaxRnd As Double = 0.0, g8Pos As Integer = 0, nEpsFail As Integer = 0, dcMaxEpsFail As Double = 0.0
        For k As Integer = 1 To 60
            For Each pp In {pNew, pNewF}
                Dim xr(pp.NVars - 1) As Integer
                For i As Integer = 0 To pp.NVars - 1 : xr(i) = rndT.Next(pp.Lb(i), pp.Ub(i) + 1) : Next
                Dim rr = pp.Evaluate(xr)
                For Each bRes In rr.BeamRes.Values
                    If Not bRes.SectionOk Then Continue For
                    If Not bRes.EpsTOk Then nEpsFail += 1 : dcMaxEpsFail = Math.Max(dcMaxEpsFail, bRes.DcFlex) : Continue For      'eps_t < 0.004 (g3 already violated): steel may not yield
                    nRnd += 1 : dcMaxRnd = Math.Max(dcMaxRnd, bRes.DcFlex)
                Next
                If rr.Violations.First(Function(v) v.Item1 = g8Name).Item2 > 0.0 AndAlso rr.BeamRes.Values.All(Function(b) b.EpsTOk) Then g8Pos += 1
            Next
        Next
        Check("T25 random designs (60 plain + 60 fibre, patterns + axial): flexural D/C <= 1 + 1e-6 in every beam group with eps_t >= 0.004, g8 never active without a g3 violation", dcMaxRnd <= 1.0 + 0.000001 AndAlso g8Pos = 0 AndAlso nRnd > 100, nRnd & " groups, max D/C " & dcMaxRnd.ToString("R", ci) & "; eps_t < 0.004 groups (g3 violated, steel may not yield): " & nEpsFail & ", max D/C " & dcMaxEpsFail.ToString("F4", ci))
        Dim cNo = rAx.ColRes("C_Inter_S1"), cPat = rNew.ColRes("C_Inter_S1")
        Check("T25 columns checked under the patterned envelope (C_Inter_S1 D/C or rho differs from single pattern)", Math.Abs(cNo.DcrMax - cPat.DcrMax) > 1.0E-9 OrElse cNo.Rho <> cPat.Rho, cNo.DcrMax.ToString("F4", ci) & " -> " & cPat.DcrMax.ToString("F4", ci))
        '--- T26 PCR-26 (IA 2026-11-18, option A): g5 = l/480 deflection constraint, off by default
        Dim g5Name As String = "g5 beam deflection after attachment <= l/480 (ACI 24.2, Table 24.2.2)"
        Dim pDef As New Ex2Problem(spec9, pr, False, New AciPlainShear())
        pDef.Opts.DeflCheck = True
        Dim rDef = pDef.Evaluate(xPilot)
        Dim worstRep As Double = Deflection2D.Report(spec9, p9.Decode(xPilot), rNew).Max(Function(rw) rw.DeltaAfter / rw.LimAfter480)
        Check("T26 DeflCheck default off; g5 absent by default, present when on", Not New Ex2Options().DeflCheck AndAlso Not Ex2Options.Campaign1().DeflCheck AndAlso Not rNew.Violations.Any(Function(v) v.Item1 = g5Name) AndAlso rDef.Violations.Any(Function(v) v.Item1 = g5Name))
        Near("T26 g5 violation = max(0, worst l/480 ratio - 1) of Deflection2D.Report (same quantity as the report)", rDef.Violations.First(Function(v) v.Item1 = g5Name).Item2, Math.Max(0.0, worstRep - 1.0), 1.0E-9)
        Check("T26 DeflCheck changes only the constraint list: cost identical", rDef.Cost.Total = rNew.Cost.Total, rDef.Cost.Total.ToString("F2", ci))
        Console.WriteLine("INFO pilot vector l/480 worst ratio " & worstRep.ToString("F4", ci) & ", g5 " & rDef.Violations.First(Function(v) v.Item1 = g5Name).Item2.ToString("F4", ci))
        Console.WriteLine("INFO pilot vector, campaign-2 rules: cost " & rNew.Cost.Total.ToString("F2", ci) & " TL, T " & rNew.T.ToString("F4", ci) & " (campaign-1 rules " & rNo.Cost.Total.ToString("F2", ci) & ")")

        Console.WriteLine()
        Console.WriteLine("passed " & passes & ", failed " & fails)
        Return fails
    End Function
End Module
