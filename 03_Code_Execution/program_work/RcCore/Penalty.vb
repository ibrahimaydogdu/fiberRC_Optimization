Option Strict On
Imports System
Imports System.Collections.Generic

''' <summary>Statik ceza fonksiyonu (kullanici tasarimi, 2026-10-28):
'''   F_ceza = F * (1 + T)^epsilon,   T = Toplam_{ceza turu j} max_{eleman i} max(0, g_ij)
''' g normalize edilir (ihlal yoksa &lt;= 0); negatif degerler 0 olur. epsilon varsayilan 2.
''' Carpimsal oldugu icin F'nin para birimi/olcegi degisse de optimum degismez (USD gosterimi guvenli).</summary>
Public Module Penalty
    Public Const DefaultEpsilon As Double = 2.0

    ''' <summary>Talep/kapasite oranindan normalize kisit: g = D/C - 1 (kapasite &gt; 0).</summary>
    Public Function Normalized(demand As Double, capacity As Double) As Double
        If capacity <= 0.0 Then Return Double.PositiveInfinity
        Return demand / capacity - 1.0
    End Function

    ''' <summary>Bir ceza turunun (grup) en kritik elemanindaki ihlal: max(0, max_i g_i). Bos grup = 0.</summary>
    Public Function GroupViolation(g As IEnumerable(Of Double)) As Double
        Dim m As Double = 0.0
        For Each v As Double In g
            If Double.IsNaN(v) Then Return Double.PositiveInfinity   ' hesaplanamayan kisit: uygunsuz (N/A sessizce gecmez)
            If v > m Then m = v
        Next
        Return m
    End Function

    ''' <summary>T = grup ihlallerinin toplami.</summary>
    Public Function Total(groups As IEnumerable(Of IEnumerable(Of Double))) As Double
        Dim t As Double = 0.0
        For Each grp As IEnumerable(Of Double) In groups
            t += GroupViolation(grp)
        Next
        Return t
    End Function

    ''' <summary>F_ceza = F (1 + T)^epsilon. T = 0 ise F aynen doner.</summary>
    Public Function Apply(f As Double, t As Double, Optional epsilon As Double = DefaultEpsilon) As Double
        If t <= 0.0 Then Return f
        If Double.IsInfinity(t) Then Return Double.PositiveInfinity
        Return f * Math.Pow(1.0 + t, epsilon)
    End Function
End Module
