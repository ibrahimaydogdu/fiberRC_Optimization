Imports System
Imports System.Collections.Generic
Imports FemCore

'TBDY-2018 horizontal elastic design spectrum (Sec. 2.3) and the response modification (Ra) of Sec. 4.2. Units: g (acceleration ratio), s.
'Spectrum shape: TA = 0.2 SD1/SDS, TB = SD1/SDS; Sae = (0.4 + 0.6 T/TA) SDS | SDS | SD1/T | SD1 TL/T^2 (TA, TB verified against AFAD report; Ra form Eq. 4.1a/4.1b: verified from page image, PDF 52 / printed 31)
Public Class TbdyParams
    Public SDS As Double = 0.718, SD1 As Double = 0.226, TL As Double = 6.0
    Public R As Double = 8.0, D As Double = 3.0, I As Double = 1.0
    Public ReadOnly Property TA As Double
        Get
            Return 0.2 * SD1 / SDS
        End Get
    End Property
    Public ReadOnly Property TB As Double
        Get
            Return SD1 / SDS
        End Get
    End Property
End Class

Public Module Tbdy
    Public Function Sae(p As TbdyParams, T As Double) As Double
        If T < p.TA Then Return (0.4 + 0.6 * T / p.TA) * p.SDS
        If T <= p.TB Then Return p.SDS
        If T <= p.TL Then Return p.SD1 / T
        Return p.SD1 * p.TL / (T * T)
    End Function

    'Ra(T): D + (R/I - D) T/TB for T <= TB, R/I beyond
    Public Function Ra(p As TbdyParams, T As Double) As Double
        If T >= p.TB Then Return p.R / p.I
        Return p.D + (p.R / p.I - p.D) * T / p.TB
    End Function

    'Reduced design spectrum SaR = Sae / Ra (Eq. 4.8); the importance factor I enters only through Ra = R/I (no separate I on Sae).
    Public Function DesignSpectrum(p As TbdyParams, name As String, Optional elastic As Boolean = False) As FemSpectrum
        Dim ts As New List(Of Double), sa As New List(Of Double)
        Dim t As Double = 0.0
        While t <= 8.0 + 0.000001
            ts.Add(t)
            Dim s As Double = Sae(p, t)
            sa.Add(If(elastic, s, s / Ra(p, t)))
            t += If(t < 1.0, 0.01, 0.05)
        End While
        Return New FemSpectrum With {.Name = name, .T = ts.ToArray(), .Sa = sa.ToArray()}
    End Function
End Module
