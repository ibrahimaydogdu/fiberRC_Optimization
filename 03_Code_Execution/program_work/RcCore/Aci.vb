Option Strict On
Imports System

''' <summary>ACI 318M-14 ortak sabitler ve katsayilar. Birimler: N, mm, MPa. Madde numaralari CODE_PROVISIONS_EVIDENCE.md ID'leriyle eslesir.</summary>
Public Module Aci
    Public Const EpsCu As Double = 0.003      ' A06, 22.2.2.1
    Public Const Es As Double = 200000.0      ' A41, 20.2.2.2

    ''' <summary>A08, Tablo 22.2.2.4.3</summary>
    Public Function Beta1(fc As Double) As Double
        If fc <= 28.0 Then Return 0.85
        If fc >= 55.0 Then Return 0.65
        Return 0.85 - 0.05 * (fc - 28.0) / 7.0
    End Function

    ''' <summary>A02-A04, Tablo 21.2.2 (etriyeli eleman; spiral icin ikinci parametre).</summary>
    Public Function Phi(epsT As Double, fy As Double, Optional spiral As Boolean = False) As Double
        Dim epsTy As Double = fy / Es
        Dim phiLo As Double = If(spiral, 0.75, 0.65)
        If epsT <= epsTy Then Return phiLo
        If epsT >= 0.005 Then Return 0.9
        Return phiLo + (0.9 - phiLo) * (epsT - epsTy) / (0.005 - epsTy)
    End Function

    ''' <summary>A41, 19.2.2.1.b</summary>
    Public Function Ec(fc As Double) As Double
        Return 4700.0 * Math.Sqrt(fc)
    End Function
End Module
