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

    ''' <summary>T2: StructurePoint Tied Column (Wight-MacGregor Ex. 11-1), Tablo 1 (sayfa 21). Belge degerleri tam sayiya yuvarlanmis, tol %0.7.</summary>
    Sub T2Column()
        Dim s As New ColumnSection
        s.B = 16.0 * In2mm
        s.H = 16.0 * In2mm
        s.Fc = 5.0 * Ksi
        s.Fy = 60.0 * Ksi
        Dim ab As Double = 4.0 * 1.0 * In2mm * In2mm      ' 4 #9 (1.00 in2)
        s.LayerY = New Double() {2.5 * In2mm, 13.5 * In2mm}
        s.LayerAs = New Double() {ab, ab}
        Dim kipFt As Double = 12.0 * Kip * In2mm
        ' Tolerans %1: ACI 318M-14 Tablo 22.2.2.4.3 SI ile fc'=34.47 MPa icin beta1=0.804, ABD metni (5000 psi) 0.80 verir;
        ' bu a'yi ~%0.5 buyutur ve Pn'i ~%0.4 yukari kaydirir (elle: beta1=0.80 ile fs=0 noktasi Pn=957 kip = belge). Belge degerleri ayrica tam sayiya yuvarlanmistir.
        Dim tol As Double = 0.01
        Check("T2 Po (kip)", Po(s) / Kip, 1530.0, tol)
        Check("T2 0.65Po max compr (kip)", 0.65 * Po(s) / Kip, 997.0, tol)
        Check("T2 phiPn,max (kip)", DesignPnMax(s) / Kip, 798.0, tol)
        Dim p2 As PmPoint = AtC(s, CForEpsT(s, 0.0))
        Check("T2 fs=0 phiPn", p2.PhiPn / Kip, 622.0, tol)
        Check("T2 fs=0 phiMn", p2.PhiMn / kipFt, 170.0, tol)
        Dim p3 As PmPoint = AtC(s, CForEpsT(s, 0.5 * s.Fy / Aci.Es))
        Check("T2 fs=0.5fy phiPn", p3.PhiPn / Kip, 422.0, tol)
        Check("T2 fs=0.5fy phiMn", p3.PhiMn / kipFt, 220.0, tol)
        Dim p4 As PmPoint = AtC(s, CForEpsT(s, s.Fy / Aci.Es))
        Check("T2 balanced phiPn", p4.PhiPn / Kip, 271.0, tol)
        Check("T2 balanced phiMn", p4.PhiMn / kipFt, 251.0, tol)
        Dim p5 As PmPoint = AtC(s, CForEpsT(s, 0.005))
        Check("T2 epsT=0.005 phiPn", p5.PhiPn / Kip, 175.0, tol)
        Check("T2 epsT=0.005 phiMn", p5.PhiMn / kipFt, 288.0, tol)
        Dim p6 As PmPoint = PureBending(s)
        Check("T2 pure bending phiMn", p6.PhiMn / kipFt, 214.0, tol)
        Check("T2 pure tension phiPn", -PureTension(s).PhiPn / Kip, 432.0, tol)
        ' Iki eksenli (dogrusal toplam): kare simetrik kesitte sy = sx; My=0 -> tek eksenli ile ayni; Mux=Muy -> toplam 2 kat
        Dim d1 As Double = DemandCapacityRatio(s, 400.0 * Kip, 100.0 * kipFt)
        Check("T2 biaxial My=0 = uniaxial", BiaxialDcr(s, s, 400.0 * Kip, 100.0 * kipFt, 0.0), d1, 0.000001)
        Check("T2 biaxial Mux=Muy = 2x", BiaxialDcr(s, s, 400.0 * Kip, 100.0 * kipFt, 100.0 * kipFt), 2.0 * d1, 0.000001)
        ' F-01 regresyonu: talep > 0 iken oran 0 olamaz
        Dim dcr As Double = DemandCapacityRatio(s, 400.0 * Kip, 150.0 * kipFt)
        total += 1
        If dcr > 0.0 AndAlso dcr < 1.0 Then
            Console.WriteLine("OK    T2 D/C(400k,150k-ft) = {0:F3} (0<D/C<1)", dcr)
        Else
            fails += 1
            Console.WriteLine("HATA  T2 D/C = {0}", dcr)
        End If
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
        T2Column()
        Console.WriteLine("{0}/{1} test gecti", total - fails, total)
        Return If(fails = 0, 0, 1)
    End Function
End Module
