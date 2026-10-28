Option Strict On
Imports System
Imports RcCore
Imports System.Collections.Generic

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

    ''' <summary>T3: SfrcBeam. BAGIMSIZ LITERATUR ORNEGI YOK (TEST_EXAMPLES bosluk). Beklenen degerler: analitik (parcali dogrusal
    ''' alan integrali) ayri Python hesabi (kod gelistirmeden bagimsiz yazildi; 2026-10-28), sayisal toplama yerine kapali form.
    ''' Girdi (ornek, gercek veri degil): b=300 h=500 d=450 mm, As=1000 mm2, fc=30, fy=420, fR1k=3.0, fR3k=2.4 MPa, lcs=200 mm.</summary>
    Sub T3Sfrc()
        Dim b As Double = 300.0, h As Double = 500.0, d As Double = 450.0, As_ As Double = 1000.0
        Dim fc As Double = 30.0, fy As Double = 420.0
        Dim p As New SfrcParams
        p.FR1 = 3.0
        p.FR3 = 2.4
        ' elle: fFts=0.45*3=1.35 ; wu=min(200*0.02;2.5)=2.5 -> fFtu=1.35-(2.5/2.5)(1.35-1.2+0.6)=0.60 ; wu=1.5 -> 0.90
        Check("T3 fFts,k", p.FtsK(), 1.35, 0.000001)
        Check("T3 wu (lcs=200)", p.Wu(200.0), 2.5, 0.000001)
        Check("T3 fFtu,k wu=2.5", p.FtuK(2.5), 0.6, 0.000001)
        Check("T3 fFtu,k wu=1.5", p.FtuK(1.5), 0.9, 0.000001)
        Dim r As FlexureResult = SfrcBeam.Flexure(b, h, d, As_, fc, fy, p, 200.0)
        ' wu otesi (w>wu) katki vermez (Critic 2026-10-28); beklenen degerler sfrc_ref2.py (kapali form)
        Check("T3 c (mm)", r.C, 75.2592, 0.001)
        Check("T3 Mn (N.mm)", r.Mn, 186829260.3, 0.001)
        Check("T3 phiMn (N.mm)", r.PhiMn, 168146334.3, 0.001)
        total += 1
        If r.FibreLimited Then
            Console.WriteLine("OK    T3 alt lif w>wu isaretlendi (FibreLimited)")
        Else
            fails += 1
            Console.WriteLine("HATA  T3 FibreLimited")
        End If
        p.WuOverride = 1.5
        Dim r15 As FlexureResult = SfrcBeam.Flexure(b, h, d, As_, fc, fy, p, 200.0)
        Check("T3 Mn wu=1.5 (thesis)", r15.Mn, 181462435.0, 0.001)
        p.WuOverride = 0.0
        Console.WriteLine("      lcs duyarlilik (phiMn kN.m, wu mm, FibreLimited):")
        For Each l As Double In New Double() {100.0, 150.0, 200.0, 300.0, 400.0}
            Dim rs As FlexureResult = SfrcBeam.Flexure(b, h, d, As_, fc, fy, p, l)
            Console.WriteLine("      lcs={0,4:F0} wu={1:F2} phiMn={2,7:F1} limited={3}", l, p.Wu(l), rs.PhiMn / 1000000.0, rs.FibreLimited)
        Next
        ' Kesme VRd,F (fib Bull.56 Vol.2 Eq. 7.7-4/7.7-5); beklenen: ayri Python hesabi (2026-10-28). fR1=3.0, fR3=2.4 -> fFtuk(wu=1.5)=0.9
        Dim fctk As Double = SfrcBeam.FctkMin(fc)
        Check("T3 fctk,min C30 (Tablo 5.1-1: 2.0)", fctk, 2.0275, 0.002)
        Check("T3 fFtuk shear wu=1.5", SfrcBeam.FFtukShear(p), 0.9, 0.000001)
        Check("T3 VRd,F (N)", SfrcBeam.VRdF(b, d, As_, fc, fctk, SfrcBeam.FFtukShear(p), 1.5), 123716.62, 0.0001)
        Check("T3 VRd,c lifsiz (N)", SfrcBeam.VRdF(b, d, As_, fc, fctk, 0.0, 1.5), 75908.94, 0.0001)
        Check("T3 VRd,F,min (N)", SfrcBeam.VRdF(b, d, 1.0, fc, fctk, 0.0, 1.5), 55684.66, 0.0001)
        ' etriyeli: 2 kollu O8 (Av=100.5 mm2), fyt=420, s=200 -> Vs = 100.5*420*450/200 = 94 972.5 N (elle); lifsiz/lifli fark = VRd,F - VRd,c
        Dim vst As Double = SfrcBeam.VRdStirrup(b, d, As_, fc, fctk, 0.9, 1.5, 100.5, 420.0, 200.0)
        Check("T3 VRd etriyeli (N)", vst, 123716.62 + 100.5 * 420.0 * d / 200.0, 0.0001)
        Check("T3 etriye tavani", SfrcBeam.VRdStirrup(b, d, As_, fc, fctk, 0.0, 1.5, 100000.0, 420.0, 100.0) - SfrcBeam.VRdF(b, d, As_, fc, fctk, 0.0, 1.5), AciBeam.VsMax(b, d, fc), 0.000001)
        total += 1
        If SfrcBeam.MinStirrupWaived(0.9, fc) AndAlso Not SfrcBeam.MinStirrupWaived(0.2, fc) Then
            Console.WriteLine("OK    T3 7.7-8 etriye muafiyeti (0.9 evet, 0.2 hayir)")
        Else
            fails += 1
            Console.WriteLine("HATA  T3 7.7-8")
        End If
        ' lifsiz: SfrcBeam == AciBeam
        Dim p0 As New SfrcParams
        Dim r0 As FlexureResult = SfrcBeam.Flexure(b, h, d, As_, fc, fy, p0, 200.0)
        Dim ra As FlexureResult = AciBeam.Flexure(b, d, As_, fc, fy)
        Check("T3 lifsiz Mn = AciBeam", r0.Mn, ra.Mn, 0.0005)
        total += 1
        If r.Mn > r0.Mn AndAlso p.CanSubstituteRebar() Then
            Console.WriteLine("OK    T3 lif kapasiteyi artirir; 5.6-3 saglanir")
        Else
            fails += 1
            Console.WriteLine("HATA  T3 monotonluk/ikame")
        End If
    End Sub

    ''' <summary>T4: maliyet. Beklenen degerler elle (PRICES_2026.md fiyatlariyla): C30 1 m3, kalip 6 m2, Ø<=12: 20 kg, Ø>12: 80 kg, lif 40 kg/m3.</summary>
    Sub T4Cost()
        Dim p As New PriceSet
        Dim c As CostBreakdown = Cost.Member(p, 30.0, 1.0, 6.0, 20.0, 80.0, 40.0, 1.0)
        Check("T4 beton 15.150.1006", c.Concrete, 3649.35, 0.0000001)
        Check("T4 donati (24.2+46.48243)*20+(24.2+44.68118)*80", c.Rebar, 20.0 * (24.2 + 46.48243) + 80.0 * (24.2 + 44.68118), 0.0000001)
        Check("T4 kalip 6 x 873.03", c.Formwork, 5238.18, 0.0000001)
        Check("T4 lif 40 x 90", c.Fibre, 3600.0, 0.0000001)
        Check("T4 toplam (TL)", c.Total, 19411.6730, 0.000001)
        Check("T4 USD (kur 42.8843)", Cost.ToUsd(c.Total, p), 19411.673 / 42.8843, 0.000001)
        ' lif fiyati duyarlilik: +50% -> yalniz lif kalemi 1.5 kat
        Dim c15 As CostBreakdown = Cost.Member(p.WithFibreFactor(1.5), 30.0, 1.0, 6.0, 20.0, 80.0, 40.0, 1.0)
        Check("T4 lif +50%", c15.Fibre, 5400.0, 0.0000001)
        Check("T4 diger kalemler degismez", c15.Total - c15.Fibre, c.Total - c.Fibre, 0.0000001)
        Dim c0 As CostBreakdown = Cost.Member(p, 30.0, 1.0, 6.0, 20.0, 80.0, 0.0, 0.0)
        Check("T4 goreli (lifsiz=100)", Cost.Relative(c0.Total, c0.Total), 100.0, 0.0000001)
    End Sub

    ''' <summary>T5: ceza. Elle: g1={-0.1,0.2,0.05}->0.2 ; g2={-0.3,-0.2}->0 ; g3={0.1}->0.1 ; T=0.3 ; F=1000 -> 1000*1.3^2 = 1690.</summary>
    Sub T5Penalty()
        Dim g1 As Double() = New Double() {-0.1, 0.2, 0.05}
        Dim g2 As Double() = New Double() {-0.3, -0.2}
        Dim g3 As Double() = New Double() {0.1}
        Dim groups As New List(Of IEnumerable(Of Double))
        groups.Add(g1)
        groups.Add(g2)
        groups.Add(g3)
        Dim t As Double = Penalty.Total(groups)
        Check("T5 T toplam", t, 0.3, 0.0000001)
        Check("T5 F_ceza epsilon=2", Penalty.Apply(1000.0, t), 1690.0, 0.0000001)
        Check("T5 epsilon=3", Penalty.Apply(1000.0, t, 3.0), 1000.0 * 1.3 * 1.3 * 1.3, 0.0000001)
        Check("T5 uygun cozum F ayni", Penalty.Apply(1000.0, 0.0), 1000.0, 0.0000001)
        ' olcek (para birimi) degisimi: F*k -> F_ceza*k (optimum siralamasi degismez)
        Check("T5 olcek bagimsiz", Penalty.Apply(1000.0 / 42.8843, t), Penalty.Apply(1000.0, t) / 42.8843, 0.0000001)
        Check("T5 normalize D/C-1", Penalty.Normalized(1.25, 1.0), 0.25, 0.0000001)
        total += 1
        Dim nanGrp As New List(Of IEnumerable(Of Double))
        nanGrp.Add(New Double() {0.0, Double.NaN})
        If Double.IsInfinity(Penalty.Apply(1000.0, Penalty.Total(nanGrp))) Then
            Console.WriteLine("OK    T5 NaN kisit = uygunsuz (N/A gecmez)")
        Else
            fails += 1
            Console.WriteLine("HATA  T5 NaN")
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
        T3Sfrc()
        T4Cost()
        T5Penalty()
        Console.WriteLine("{0}/{1} test gecti", total - fails, total)
        Return If(fails = 0, 0, 1)
    End Function
End Module
