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
        Dim ev = Ex2Evaluator.Evaluate(spec5, Ex2Problem.FromSizes(t1), pr, New AciPlainShear())
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
        Near("T8 Q17 model, plain beam: = AciBeam.Vc", q17.ConcreteShear(300, 440, 30, 900, Nothing), RcCore.AciBeam.Vc(300, 440, 30), 1.0E-15)
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
        Near("T10 O1 format = VRdF(gammaC = 1.5) for SFRC", o1.ConcreteShear(300, 440, 30, 900, fib30), RcCore.SfrcBeam.VRdF(300, 440, 900, 30, RcCore.SfrcBeam.FctkMin(30), RcCore.SfrcBeam.FFtukShear(fib30.Params), 1.5), 1.0E-15)
        Near("T10 O1 format, plain beam = AciBeam.Vc", o1.ConcreteShear(300, 440, 30, 900, Nothing), RcCore.AciBeam.Vc(300, 440, 30), 1.0E-15)
        Dim incEx As Double = FibreShearIncrementQ17.Increment(300, 440, 30, 900, fib30)
        Console.WriteLine("INFO 300x500 (d 440), As 900, 4D 30 kg/m3: Vc,ACI " & (RcCore.AciBeam.Vc(300, 440, 30) / 1000.0).ToString("F2", ci) & " kN, dVF " & (incEx / 1000.0).ToString("F2", ci) & " kN, O1 VRdF(1.5) " & (o1.ConcreteShear(300, 440, 30, 900, fib30) / 1000.0).ToString("F2", ci) & " kN")

        '--- critical section d from the node (ACI 9.4.3.2): E13 (25x30, d = 240 mm), 1.2D+1.6L, w = 1.2 (30 + 25 x 0.25 x 0.30) + 1.6 x 20 = 70.25 kN/m
        Near("T11 station shear at x = 0 equals face shear (E13)", Math.Abs(BeamShearGravity.ShearAt(a, i2, 12, 0.0)), Math.Abs(f13(1)), 1.0E-9)
        Near("T11 |V(0)| - |V(d)| = w d (E13, 1.2D+1.6L)", Math.Abs(f13(1)) - Math.Abs(BeamShearGravity.ShearAt(a, i2, 12, 0.24)), 70.25 * 0.24, 1.0E-9)
        Dim evT1 = Ex2Evaluator.Evaluate(spec5, Ex2Problem.FromSizes(t1), pr, New AciPlainShear())
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

        Console.WriteLine()
        Console.WriteLine("passed " & passes & ", failed " & fails)
        Return fails
    End Function
End Module
