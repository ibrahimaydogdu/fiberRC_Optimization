Imports System
Imports FemCore

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

    'Rectangular concrete section b x h (m), E kN/m2; I22/I33: I33 = bending about local axis 3 (in-plane, global Y vertical -> see ETABS axes)
    Function Sec(b As Double, h As Double, E As Double) As FemSection
        Dim s As New FemSection With {.Name = "S", .E = E, .G = E / 2.4, .A = b * h, .As2 = 0.0, .As3 = 0.0, .UnitWeight = 25.0, .UnitMass = 25.0 / 9.80665}
        s.I33 = b * h ^ 3 / 12.0 : s.I22 = h * b ^ 3 / 12.0
        s.J = 0.0 : Dim a = Math.Max(b, h), c = Math.Min(b, h) : s.J = a * c ^ 3 * (1.0 / 3.0 - 0.21 * c / a * (1.0 - c ^ 4 / (12.0 * a ^ 4)))
        s.Depth = h : s.Width = b
        Return s
    End Function

    Sub AddNode(m As FemModel, nm As String, x As Double, y As Double, z As Double, fixedBase As Boolean)
        Dim n As New FemNode With {.Name = nm, .X = x, .Y = y, .Z = z}
        If fixedBase Then For i = 0 To 5 : n.Restraint(i) = True : Next
        m.Nodes.Add(n)
    End Sub

    'Element along global direction dv (unit vector), local 2 axis = global Y (or Z when vertical)
    Function AddEl(m As FemModel, nm As String, ni As Integer, nj As Integer, s As FemSection) As FemElement
        Dim a = m.Nodes(ni), b = m.Nodes(nj)
        Dim dx = b.X - a.X, dy = b.Y - a.Y, dz = b.Z - a.Z
        Dim L = Math.Sqrt(dx * dx + dy * dy + dz * dz)
        Dim e As New FemElement With {.Name = nm, .NI = ni, .NJ = nj, .Section = s, .Length = L}
        Dim ax1() As Double = {dx / L, dy / L, dz / L}
        'ETABS: vertical member local 2 = global X; others local 2 = global Z (up) projected
        Dim ax2() As Double, ax3() As Double
        If Math.Abs(ax1(0)) < 0.000001 AndAlso Math.Abs(ax1(1)) < 0.000001 Then
            ax2 = {1.0, 0.0, 0.0}
        Else
            ax2 = {0.0, 0.0, 1.0}
        End If
        ax3 = {ax1(1) * ax2(2) - ax1(2) * ax2(1), ax1(2) * ax2(0) - ax1(0) * ax2(2), ax1(0) * ax2(1) - ax1(1) * ax2(0)}
        ax2 = {ax3(1) * ax1(2) - ax3(2) * ax1(1), ax3(2) * ax1(0) - ax3(0) * ax1(2), ax3(0) * ax1(1) - ax3(1) * ax1(0)}
        For c = 0 To 2 : e.Axes(0, c) = ax1(c) : e.Axes(1, c) = ax2(c) : e.Axes(2, c) = ax3(c) : Next
        m.Elements.Add(e)
        Return e
    End Function

    Sub Main()
        Dim E As Double = 30000000.0           'kN/m2 (30 GPa)
        Dim b As Double = 0.4, h As Double = 0.5
        Dim s = Sec(b, h, E)
        'T1: cantilever column L = 3 m along global Z (vertical), tip load 10 kN in global X
        Dim m As New FemModel
        AddNode(m, "A", 0, 0, 0, True) : AddNode(m, "B", 0, 0, 3, False)
        m.Sections.Add(s) : AddEl(m, "C1", 0, 1, s)
        Dim p As New FemPattern With {.Name = "P", .SelfWeightMultiplier = 0.0}
        Dim nl As New FemNodalLoad With {.Node = 1} : nl.F(0) = 10.0 : p.NodalLoads.Add(nl)
        m.Patterns.Add(p)
        Dim fs As New FemStatic(m)
        Dim r = fs.Solve()
        Console.WriteLine("T1 solve failed=" & r.Failed & " " & r.Message)
        Dim uxTip = r.U(0)(6 + 0)
        'weak/strong: tip load along global X; local 2 = global X for a vertical member, so bending about local 3 (I33) with h along local 2.
        Dim Iz = b * h ^ 3 / 12.0        'section depth h along local 2 (global X) -> I33
        Dim want = 10.0 * 3.0 ^ 3 / (3.0 * E * Iz)
        Check("T1 cantilever tip deflection PL3/3EI", Math.Abs(uxTip), want, 0.0001)
        Check("T1 base reaction Fx", Math.Abs(r.Reaction(0)(0)), 10.0, 0.0001)
        Check("T1 base moment My", Math.Abs(r.Reaction(0)(4)), 30.0, 0.0001)

        'T2: fixed-fixed beam L = 5 m along X, gravity UDL w = 20 kN/m (Dir 10 gravity): end moment wL2/12, reaction wL/2, midspan deflection wL4/384EI
        Dim m2 As New FemModel
        AddNode(m2, "A", 0, 0, 3, True) : AddNode(m2, "B", 5, 0, 3, True)
        m2.Sections.Add(s) : AddEl(m2, "B1", 0, 1, s)
        Dim p2 As New FemPattern With {.Name = "W", .SelfWeightMultiplier = 0.0}
        p2.DistLoads.Add(New FemDistLoad With {.Element = 0, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0.0, .D2 = 1.0, .V1 = 20.0, .V2 = 20.0})
        m2.Patterns.Add(p2)
        Dim r2 = New FemStatic(m2).Solve()
        Console.WriteLine("T2 failed=" & r2.Failed & " " & r2.Message)
        Check("T2 reaction Fz = wL/2", Math.Abs(r2.Reaction(0)(2)), 50.0, 0.0001)
        Check("T2 end moment wL2/12", Math.Abs(r2.Reaction(0)(4)), 20.0 * 25.0 / 12.0, 0.0001)

        'T3: cantilever with tip mass m = 5 kN s2/m (lumped, X): T = 2 pi sqrt(m / (3EI/L3))
        Dim m3 As New FemModel
        AddNode(m3, "A", 0, 0, 0, True) : AddNode(m3, "B", 0, 0, 3, False)
        m3.Nodes(1).AddedMass = {5.0, 5.0, 0.0}
        m3.Sections.Add(s) : AddEl(m3, "C1", 0, 1, s)
        m3.Nodes(1).Restraint(1) = True : m3.Nodes(1).Restraint(2) = True : m3.Nodes(1).Restraint(3) = True : m3.Nodes(1).Restraint(4) = False : m3.Nodes(1).Restraint(5) = True
        m3.Patterns.Add(New FemPattern With {.Name = "D", .SelfWeightMultiplier = 0.0})
        m3.MassSource.SelfMass = False
        Dim fs3 As New FemStatic(m3)
        Dim md = fs3.ModalSolve(1)
        Dim k = 3.0 * E * Iz / 27.0
        Check("T3 period (X, tip mass)", md.Period(0), 2.0 * Math.PI * Math.Sqrt(5.0 / k), 0.0005)

        'T4: portal frame, two columns fixed at the base, very stiff beam: lateral stiffness 2 x 12EI/h3, column end moment H h / 4
        Dim sB = Sec(b, h, E) : sB.I33 = 1000000.0 * sB.I33 : sB.I22 = 1000000.0 * sB.I22 : sB.A = 1000.0 * sB.A
        Dim m4 As New FemModel
        AddNode(m4, "A", 0, 0, 0, True) : AddNode(m4, "B", 5, 0, 0, True) : AddNode(m4, "C", 0, 0, 3, False) : AddNode(m4, "D", 5, 0, 3, False)
        Dim sC = Sec(b, h, E) : sC.A = 1000.0 * sC.A      'axial deformation of the columns (rocking of the beam) excluded from the check
        m4.Sections.Add(sC) : m4.Sections.Add(sB)
        AddEl(m4, "C1", 0, 2, sC) : AddEl(m4, "C2", 1, 3, sC) : AddEl(m4, "B1", 2, 3, sB)
        Dim p4 As New FemPattern With {.Name = "H", .SelfWeightMultiplier = 0.0}
        Dim nl4 As New FemNodalLoad With {.Node = 2} : nl4.F(0) = 20.0 : p4.NodalLoads.Add(nl4) : m4.Patterns.Add(p4)
        Dim r4 = New FemStatic(m4).Solve()
        Console.WriteLine("T4 failed=" & r4.Failed & " " & r4.Message)
        Check("T4 portal sway = H h3 / (24EI)", Math.Abs(r4.U(0)(2 * 6)), 20.0 * 27.0 / (24.0 * E * Iz), 0.002)
        Check("T4 base moment = H h / 4", Math.Abs(r4.Reaction(0)(4)), 20.0 * 3.0 / 4.0, 0.002)

        'T5: one-storey 3D frame, 4 columns (fixed), rigid diaphragm, load H = 40 kN in X through the centroid: u = H h3 / (4 x 12EI/ ... ) per column 12EI/h3
        Dim m5 As New FemModel With {.DiaphragmCount = 1}
        AddNode(m5, "A", 0, 0, 0, True) : AddNode(m5, "B", 5, 0, 0, True) : AddNode(m5, "C", 5, 6, 0, True) : AddNode(m5, "D", 0, 6, 0, True)
        AddNode(m5, "a", 0, 0, 3, False) : AddNode(m5, "b", 5, 0, 3, False) : AddNode(m5, "c", 5, 6, 3, False) : AddNode(m5, "d", 0, 6, 3, False)
        For i = 4 To 7 : m5.Nodes(i).Diaphragm = 0 : Next
        m5.Sections.Add(s)
        For i = 0 To 3 : AddEl(m5, "C" & i, i, i + 4, s) : Next
        Dim p5 As New FemPattern With {.Name = "H", .SelfWeightMultiplier = 0.0}
        'load at the centroid of the diaphragm: apply as 4 equal nodal forces (statically equivalent through the centroid)
        For i = 4 To 7 : Dim q As New FemNodalLoad With {.Node = i} : q.F(0) = 10.0 : p5.NodalLoads.Add(q) : Next
        m5.Patterns.Add(p5)
        Dim r5 = New FemStatic(m5).Solve()
        Console.WriteLine("T5 failed=" & r5.Failed & " " & r5.Message)
        Check("T5 diaphragm sway = H h3 / (4 x 3EI) (columns free to rotate)", Math.Abs(r5.U(0)(4 * 6)), 40.0 * 27.0 / (4.0 * 3.0 * E * Iz), 0.0001)
        Check("T5 diaphragm same Ux at all joints", Math.Abs(r5.U(0)(7 * 6)), Math.Abs(r5.U(0)(4 * 6)), 0.000001)
        Check("T5 rotation about Z (centroid load, symmetric)", Math.Abs(r5.U(0)(4 * 6 + 5)), 0.0, 1.0E-9)

        'T6: P-Delta preset based on mass: cantilever, tip mass 5 kN s2/m, H = 10 kN: u = H / (k - m g / z), k = 3EI/L3
        Dim m6 As New FemModel
        AddNode(m6, "A", 0, 0, 0, True) : AddNode(m6, "B", 0, 0, 3, False)
        m6.Nodes(1).AddedMass = {5.0, 5.0, 0.0}
        m6.Sections.Add(s) : AddEl(m6, "C1", 0, 1, s)
        Dim p6 As New FemPattern With {.Name = "H", .SelfWeightMultiplier = 0.0}
        Dim nl6 As New FemNodalLoad With {.Node = 1} : nl6.F(0) = 10.0 : p6.NodalLoads.Add(nl6) : m6.Patterns.Add(p6)
        m6.MassSource.SelfMass = False
        Dim fs6 As New FemStatic(m6)
        fs6.UseMassPDelta()
        Dim r6 = fs6.Solve()
        Dim k6 = 3.0 * E * Iz / 27.0
        Check("T6 P-Delta (mass based) amplified tip displacement", Math.Abs(r6.U(0)(6)), 10.0 / (k6 - 5.0 * 9.80665 / 3.0), 0.0005)

        'T7: response spectrum, SDOF cantilever (tip mass 5), Sa = 0.5 g constant, scale = g: base shear = m Sa g
        Dim m7 As New FemModel
        AddNode(m7, "A", 0, 0, 0, True) : AddNode(m7, "B", 0, 0, 3, False)
        m7.Nodes(1).AddedMass = {5.0, 5.0, 0.0}
        m7.Nodes(1).Restraint(1) = True : m7.Nodes(1).Restraint(3) = True : m7.Nodes(1).Restraint(5) = True
        m7.Sections.Add(s) : AddEl(m7, "C1", 0, 1, s)
        m7.Patterns.Add(New FemPattern With {.Name = "D", .SelfWeightMultiplier = 0.0})
        m7.MassSource.SelfMass = False
        m7.Spectra.Add(New FemSpectrum With {.Name = "SP", .T = {0.0, 10.0}, .Sa = {0.5, 0.5}})
        Dim rs7 As New FemRSCase With {.Name = "EX", .Direction = 0, .Spectrum = "SP", .ScaleFactor = 9.80665}
        Dim fs7 As New FemStatic(m7)
        Dim md7 = fs7.ModalSolve(1)
        Dim rr7 = fs7.ResponseSpectrum(md7, rs7)
        Check("T7 RS base shear = m Sa g", rr7.BaseForce(0), 5.0 * 0.5 * 9.80665, 0.0005)
        Check("T7 RS tip displacement = Sa g / w2", rr7.U(6), 0.5 * 9.80665 / (3.0 * E * Iz / 27.0 / 5.0), 0.0005)

        'T8: two-storey shear building (rigid beams, 2 columns per storey, k = 2 x 12EI/h3, masses m = 20 per floor): w2 = (k/m)(3 -/+ sqrt5)/2
        Dim sC8 = Sec(b, h, E) : sC8.A = 1000.0 * sC8.A
        Dim sB8 = Sec(b, h, E) : sB8.I33 = 1000000.0 * sB8.I33 : sB8.I22 = 1000000.0 * sB8.I22 : sB8.A = 1000.0 * sB8.A
        Dim m8 As New FemModel
        AddNode(m8, "A0", 0, 0, 0, True) : AddNode(m8, "B0", 5, 0, 0, True)
        AddNode(m8, "A1", 0, 0, 3, False) : AddNode(m8, "B1", 5, 0, 3, False)
        AddNode(m8, "A2", 0, 0, 6, False) : AddNode(m8, "B2", 5, 0, 6, False)
        For i = 2 To 5 : m8.Nodes(i).Restraint(1) = True : m8.Nodes(i).Restraint(3) = True : m8.Nodes(i).Restraint(5) = True : Next
        For i = 2 To 5 : m8.Nodes(i).AddedMass = {10.0, 10.0, 0.0} : Next
        m8.Sections.Add(sC8) : m8.Sections.Add(sB8)
        AddEl(m8, "C11", 0, 2, sC8) : AddEl(m8, "C12", 1, 3, sC8) : AddEl(m8, "C21", 2, 4, sC8) : AddEl(m8, "C22", 3, 5, sC8)
        AddEl(m8, "B1", 2, 3, sB8) : AddEl(m8, "B2", 4, 5, sB8)
        m8.Patterns.Add(New FemPattern With {.Name = "D", .SelfWeightMultiplier = 0.0})
        m8.MassSource.SelfMass = False
        Dim md8 = New FemStatic(m8).ModalSolve(2)
        Dim k8 = 2.0 * 12.0 * E * Iz / 27.0
        Dim w2a = k8 / 20.0 * (3.0 - Math.Sqrt(5.0)) / 2.0, w2b = k8 / 20.0 * (3.0 + Math.Sqrt(5.0)) / 2.0
        Check("T8 shear building T1", md8.Period(0), 2.0 * Math.PI / Math.Sqrt(w2a), 0.003)
        Check("T8 shear building T2", md8.Period(1), 2.0 * Math.PI / Math.Sqrt(w2b), 0.003)
        Console.WriteLine("PASS=" & Pass & " FAIL=" & Fail)
    End Sub
End Module
