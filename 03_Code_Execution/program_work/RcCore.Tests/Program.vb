Option Strict On
Imports System
Imports RcCore

''' <summary>V1 birim testleri. Beklenen degerler: Evidence/DesignGuides/Examples/StructurePoint_SimplySupportedBeam_ACI318-14.pdf
''' (bagimsiz yeniden hesap: 01_Literature/TEST_EXAMPLES.md T1). Donusum: 1 in=25.4 mm, 1 ksi=6.894757 MPa, 1 kip=4448.222 N.</summary>
Module Program
    Const In2mm As Double = 25.4
    Const Ksi As Double = 6.894757
    Const Kip As Double = 4448.222
    Dim fails As Integer = 0
    Dim total As Integer = 0

    Sub Check(name As String, actual As Double, expected As Double, tolRel As Double)
        total += 1
        Dim err As Double = Math.Abs(actual - expected) / Math.Abs(expected)
        Dim ok As Boolean = err <= tolRel
        If Not ok Then fails += 1
        Console.WriteLine("{0,-5} {1,-26} hesap={2,12:G6} beklenen={3,12:G6} hata={4:P3}", If(ok, "OK", "HATA"), name, actual, expected, err)
    End Sub

    Function Main() As Integer
        Dim fc As Double = 4.35 * Ksi
        Dim fy As Double = 60.0 * Ksi
        Dim b As Double = 12.0 * In2mm
        Dim d As Double = 17.56 * In2mm
        Dim kipFt As Double = 12.0 * Kip * In2mm            ' 1 kip-ft in N.mm
        Dim Mu As Double = 201.88 * kipFt
        Dim asReq As Double = AciBeam.RequiredAs(Mu, b, d, fc, fy)
        Check("T1 As gerekli (in2)", asReq / (In2mm * In2mm), 2.872, 0.005)
        Dim fr As FlexureResult = AciBeam.Flexure(b, d, asReq, fc, fy)
        Check("T1 a (in)", fr.A / In2mm, 3.88, 0.005)
        Check("T1 phiMn (kip-ft)", fr.PhiMn / kipFt, 201.88, 0.005)
        ' ACI 318M-14 (SI) katsayilari ABD metnine gore yuvarlanmistir (1.4 MPa vs 200 psi = 1.379 MPa; 0.17 vs 2 sqrt(psi) = 0.1656).
        ' Bu nedenle belge degerleriyle fark <= %3 (SI sabitlerinin yuvarlama farki) kabul edilir; SI formulun kendisi elle hesapla ayrica dogrulanir.
        Check("T1 As,min US-belge (in2)", AciBeam.MinAs(b, d, fc, fy) / (In2mm * In2mm), 0.702, 0.03)
        Check("T1 phiVc US-belge (kip)", 0.75 * AciBeam.Vc(b, d, fc) / Kip, 20.85, 0.03)
        ' SI elle hesap: 1.4*b*d/fy ; 0.75*0.17*sqrt(fc)*b*d (bagimsiz aritmetik, 2026-10-28)
        Check("T1 As,min SI elle (mm2)", AciBeam.MinAs(b, d, fc, fy), 1.4 * b * d / fy, 0.000001)
        Check("T1 phiVc SI elle (N)", 0.75 * AciBeam.Vc(b, d, fc), 0.75 * 0.17 * Math.Sqrt(fc) * b * d, 0.000001)
        total += 1
        If fr.EpsT >= 0.005 AndAlso fr.Phi = 0.9 AndAlso fr.EpsTOk Then
            Console.WriteLine("OK    T1 epsT>=0.005, phi=0.9")
        Else
            fails += 1
            Console.WriteLine("HATA  T1 epsT/phi")
        End If
        Console.WriteLine("{0}/{1} test gecti", total - fails, total)
        Return If(fails = 0, 0, 1)
    End Function
End Module
