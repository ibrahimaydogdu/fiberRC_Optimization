Option Strict On
Imports System

Public Structure FlexureResult
    Public A As Double        ' esdeger basinc blogu derinligi (mm)
    Public C As Double        ' tarafsiz eksen derinligi (mm)
    Public EpsT As Double     ' en dis cekme donatisi birim sekil degistirmesi
    Public Phi As Double
    Public Mn As Double       ' N.mm
    Public PhiMn As Double    ' N.mm
    Public EpsTOk As Boolean  ' A10: epsT >= 0.004
    Public FibreLimited As Boolean ' SfrcBeam: alt lifte w > wu (lif etkisi kismen kesildi)
End Structure

''' <summary>ACI 318M-14 dikdortgen kiris (tek donatili) egilme, minimum donati ve kesme. Birimler N, mm, MPa.</summary>
Public Module AciBeam

    ''' <summary>A06-A07: phiMn, tek sira cekme donatisi (basinc donatisi ihmal). Donati akmamissa fs = Es*epsT ile cozer.</summary>
    Public Function Flexure(b As Double, d As Double, As_ As Double, fc As Double, fy As Double) As FlexureResult
        Dim r As FlexureResult
        Dim fs As Double = fy
        Dim cYield As Double = As_ * fy / (0.85 * fc * b * Aci.Beta1(fc))
        If Aci.EpsCu * (d - cYield) / cYield < fy / Aci.Es Then fs = SteelStress(b, d, As_, fc, fy)
        r.A = As_ * fs / (0.85 * fc * b)
        r.C = r.A / Aci.Beta1(fc)
        r.EpsT = Aci.EpsCu * (d - r.C) / r.C
        r.Phi = Aci.Phi(r.EpsT, fy)
        r.Mn = As_ * fs * (d - r.A / 2.0)
        r.PhiMn = r.Phi * r.Mn
        r.EpsTOk = r.EpsT >= 0.004
        Return r
    End Function

    Private Function SteelStress(b As Double, d As Double, As_ As Double, fc As Double, fy As Double) As Double
        Dim qa As Double = 0.85 * fc * b * Aci.Beta1(fc)
        Dim qb As Double = As_ * Aci.Es * Aci.EpsCu
        Dim c As Double = (-qb + Math.Sqrt(qb * qb + 4.0 * qa * qb * d)) / (2.0 * qa)
        Return Math.Min(fy, Aci.Es * Aci.EpsCu * (d - c) / c)
    End Function

    ''' <summary>Gerekli As; phi=0.9 ile baslar, epsT&lt;0.005 ise phi'yi iterasyonla gunceller. Kesit yetersizse +Infinity.</summary>
    Public Function RequiredAs(Mu As Double, b As Double, d As Double, fc As Double, fy As Double) As Double
        Dim phiV As Double = 0.9
        Dim asV As Double = 0.0
        For i As Integer = 0 To 49
            Dim disc As Double = d * d - 2.0 * Mu / (phiV * 0.85 * fc * b)
            If disc < 0 Then Return Double.PositiveInfinity
            Dim a As Double = d - Math.Sqrt(disc)
            asV = 0.85 * fc * b * a / fy
            Dim c As Double = a / Aci.Beta1(fc)
            Dim phiN As Double = Aci.Phi(Aci.EpsCu * (d - c) / c, fy)
            If Math.Abs(phiN - phiV) < 0.000000001 Then Exit For
            phiV = phiN
        Next
        Return asV
    End Function

    ''' <summary>A09, 9.6.1.2: max(0.25 sqrt(fc) bw d / fy ; 1.4 bw d / fy)</summary>
    Public Function MinAs(b As Double, d As Double, fc As Double, fy As Double) As Double
        Return Math.Max(0.25 * Math.Sqrt(fc) * b * d / fy, 1.4 * b * d / fy)
    End Function

    ''' <summary>A12/A15: Vc = 0.17 lambda sqrt(fc) bw d (22.5.5.1); sqrt(fc) &lt;= 8.3 MPa (22.5.3.1)</summary>
    Public Function Vc(b As Double, d As Double, fc As Double, Optional lambda As Double = 1.0) As Double
        Return 0.17 * lambda * Math.Min(Math.Sqrt(fc), 8.3) * b * d
    End Function

    ''' <summary>A16: Vs = Av fyt d / s</summary>
    Public Function Vs(Av As Double, fyt As Double, d As Double, s As Double) As Double
        Return Av * fyt * d / s
    End Function

    ''' <summary>A14: kesit boyut siniri Vs &lt;= 0.66 sqrt(fc) b d</summary>
    Public Function VsMax(b As Double, d As Double, fc As Double) As Double
        Return 0.66 * Math.Sqrt(fc) * b * d
    End Function

    ''' <summary>A18: Av,min/s = max(0.062 sqrt(fc) b/fyt ; 0.35 b/fyt)</summary>
    Public Function AvMinPerS(b As Double, fc As Double, fyt As Double) As Double
        Return Math.Max(0.062 * Math.Sqrt(fc) * b / fyt, 0.35 * b / fyt)
    End Function

    ''' <summary>A19: Tablo 9.7.6.2.2 azami etriye araligi</summary>
    Public Function MaxStirrupSpacing(VsVal As Double, b As Double, d As Double, fc As Double) As Double
        If VsVal <= 0.33 * Math.Sqrt(fc) * b * d Then Return Math.Min(d / 2.0, 600.0)
        Return Math.Min(d / 4.0, 300.0)
    End Function
End Module
