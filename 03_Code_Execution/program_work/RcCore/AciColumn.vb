Option Strict On
Imports System

''' <summary>Dikdortgen kesit, donati katmanlari. Y: en dis basinc lifinden (ust) katman merkezine uzaklik (mm). Birimler N, mm, MPa.</summary>
Public Class ColumnSection
    Public B As Double          ' egilme eksenine dik genislik
    Public H As Double          ' egilme duzlemindeki yukseklik
    Public Fc As Double
    Public Fy As Double
    Public LayerY() As Double   ' ust lifden katman merkezi (mm)
    Public LayerAs() As Double  ' katman alani (mm2)
    Public Beta1Override As Double = 0.0   ' >0 ise blok derinligi katsayisi (TS 500 k1 icin); 0 = ACI Beta1(Fc)

    Public ReadOnly Property Ast As Double
        Get
            Dim s As Double = 0.0
            For Each a As Double In LayerAs
                s += a
            Next
            Return s
        End Get
    End Property
End Class

Public Structure PmPoint
    Public C As Double         ' tarafsiz eksen derinligi (mm)
    Public Pn As Double        ' N (basinc +)
    Public Mn As Double        ' N.mm (kesit agirlik merkezine gore)
    Public EpsT As Double      ' en dis cekme donatisi birim sekil degistirmesi (cekme +)
    Public Phi As Double
    Public PhiPn As Double
    Public PhiMn As Double
End Structure

''' <summary>ACI 318M-14 kolon P-M (tek eksenli) - 22.2 varsayimlari, 22.4.2 Pn,max, 21.2.2 phi.</summary>
Public Module AciColumn

    ''' <summary>A20/22.4.2.2: Po = 0.85 fc (Ag - Ast) + fy Ast</summary>
    Public Function Po(s As ColumnSection) As Double
        Return 0.85 * s.Fc * (s.B * s.H - s.Ast) + s.Fy * s.Ast
    End Function

    ''' <summary>A20, Tablo 22.4.2.1: Pn,max = 0.80 Po (etriyeli)</summary>
    Public Function PnMax(s As ColumnSection, Optional spiral As Boolean = False) As Double
        Return If(spiral, 0.85, 0.8) * Po(s)
    End Function

    ''' <summary>Verilen tarafsiz eksen derinligi c icin (22.2.2: epsCu=0.003, dogrusal birim sekil degistirme).</summary>
    Public Function AtC(s As ColumnSection, c As Double) As PmPoint
        Dim p As PmPoint
        p.C = c
        Dim a As Double = Math.Min(If(s.Beta1Override > 0.0, s.Beta1Override, Aci.Beta1(s.Fc)) * c, s.H)
        Dim cc As Double = 0.85 * s.Fc * s.B * a
        Dim pn As Double = cc
        Dim mn As Double = cc * (s.H / 2.0 - a / 2.0)
        Dim dMax As Double = 0.0
        For i As Integer = 0 To s.LayerY.Length - 1
            Dim eps As Double = Aci.EpsCu * (c - s.LayerY(i)) / c        ' basinc +
            Dim fs As Double = Math.Max(-s.Fy, Math.Min(s.Fy, Aci.Es * eps))
            ' basinc blogu icindeki betonun yerini tutan alan duzeltmesi
            Dim net As Double = fs
            If s.LayerY(i) <= a Then net = fs - 0.85 * s.Fc
            Dim f As Double = net * s.LayerAs(i)
            pn += f
            mn += f * (s.H / 2.0 - s.LayerY(i))
            If s.LayerY(i) > dMax Then
                dMax = s.LayerY(i)
                p.EpsT = -eps
            End If
        Next
        p.Pn = pn
        p.Mn = mn
        p.Phi = Aci.Phi(p.EpsT, s.Fy)
        p.PhiPn = p.Phi * pn
        p.PhiMn = p.Phi * mn
        Return p
    End Function

    ''' <summary>Saf cekme noktasi: Pn = -fy Ast, phi = 0.9</summary>
    Public Function PureTension(s As ColumnSection) As PmPoint
        Dim p As PmPoint
        p.C = 0.0
        p.Pn = -s.Fy * s.Ast
        p.Mn = 0.0
        p.EpsT = 0.005
        p.Phi = 0.9
        p.PhiPn = p.Phi * p.Pn
        p.PhiMn = 0.0
        Return p
    End Function

    ''' <summary>Pn = 0 (saf egilme) noktasi, c'yi ikiye bolme ile bulur.</summary>
    Public Function PureBending(s As ColumnSection) As PmPoint
        Dim lo As Double = 1.0
        Dim hi As Double = s.H
        For i As Integer = 0 To 59
            Dim mid As Double = 0.5 * (lo + hi)
            If AtC(s, mid).Pn > 0.0 Then hi = mid Else lo = mid
        Next
        Return AtC(s, 0.5 * (lo + hi))
    End Function

    ''' <summary>Tarafsiz eksen c, en dis cekme donatisi dMax'ta verilen epsT (cekme) icin: c = epsCu/(epsCu+epsT)*dMax</summary>
    Public Function CForEpsT(s As ColumnSection, epsT As Double) As Double
        Dim dMax As Double = 0.0
        For Each y As Double In s.LayerY
            dMax = Math.Max(dMax, y)
        Next
        Return Aci.EpsCu / (Aci.EpsCu + epsT) * dMax
    End Function

    ''' <summary>A20: tasarim eksenel ust siniri phi*Pn,max (phi=0.65 basinc kontrollu)</summary>
    Public Function DesignPnMax(s As ColumnSection) As Double
        Return 0.65 * PnMax(s)
    End Function

    ''' <summary>Sirali c degerlerinde tasarim egrisi; kapasite orani icin Pu-Mu noktasini sarar (F-01 duzeltmesi: oran 0 olamaz).</summary>
    Public Function DemandCapacityRatio(s As ColumnSection, Pu As Double, Mu As Double) As Double
        ' Ortu: ayni eksantriste (Mu/Pu) egri uzerindeki kapasiteye bolunur; Mu=0 ise eksenel orani
        If Pu >= DesignPnMax(s) Then Return Pu / DesignPnMax(s)
        If Pu <= PureTension(s).PhiPn Then Return Pu / PureTension(s).PhiPn
        Dim target As Double = Math.Abs(Mu)
        If target = 0.0 Then Return Math.Abs(Pu) / DesignPnMax(s)
        ' Pu sabit iken c'yi ara: PhiPn(c) = Pu  (PhiPn c ile artar), sonra PhiMn al
        Dim lo As Double = 0.5
        Dim hi As Double = 20.0 * s.H
        For i As Integer = 0 To 59
            Dim mid As Double = 0.5 * (lo + hi)
            Dim pt As PmPoint = AtC(s, mid)
            Dim pPhi As Double = Math.Min(pt.PhiPn, DesignPnMax(s))
            If pPhi < Pu Then lo = mid Else hi = mid
        Next
        Dim cap As PmPoint = AtC(s, 0.5 * (lo + hi))
        Dim mcap As Double = Math.Abs(cap.PhiMn)
        Return target / mcap
    End Function

    ''' <summary>Arama-ici iki eksenli kontrol (muhafazakar dogrusal toplam): Mux/phiMnx(Pu) + Muy/phiMny(Pu) = D/C.
    ''' sy: sx'in 90 derece donmus hali (B,H yer degistirir, katman Y'leri diger yone gore). Kaynak: ACI 318-14'te
    ''' iki eksenli formul YOK (CODE_PROVISIONS_EVIDENCE A20-A21, BULUNAMADI); yontem literatur atfi gerektirir.
    ''' D/C &gt; 1 ise yetersiz. Tam P-Mx-My yuzeyi ETABS final dogrulamasinda okunur.</summary>
    Public Function BiaxialDcr(sx As ColumnSection, sy As ColumnSection, Pu As Double, Mux As Double, Muy As Double) As Double
        Dim rx As Double = If(Mux = 0.0, 0.0, DemandCapacityRatio(sx, Pu, Mux))
        Dim ry As Double = If(Muy = 0.0, 0.0, DemandCapacityRatio(sy, Pu, Muy))
        If Mux = 0.0 AndAlso Muy = 0.0 Then Return DemandCapacityRatio(sx, Pu, 0.0)
        Return rx + ry
    End Function
End Module
