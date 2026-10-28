Option Strict On
Imports System

''' <summary>Celik lifli beton cekme parametreleri (fib MC2010 5.6). Girdi fR1k, fR3k KARAKTERISTIK degerlerdir (MPa).</summary>
Public Class SfrcParams
    Public FR1 As Double
    Public FR3 As Double
    Public GammaF As Double = 1.5          ' B10, Tablo 5.6-1
    Public Linear As Boolean = True        ' True: dogrusal sigma-w (5.6-5/6); False: rijit-plastik (5.6-4)
    Public EpsFu As Double = 0.02          ' B07: degisken birim sekil degistirme dagilimi icin %2
    Public WMaxLimit As Double = 2.5       ' B07: azami carlak genisligi 2.5 mm
    Public Cmod3 As Double = 2.5           ' B02
    Public WuOverride As Double = 0.0      ' >0 ise wu bu degerdir (tez uretimi icin 1.5 mm, K3 karari)

    ''' <summary>B07: wu = min(lcs*epsFu, 2.5 mm); lineer model icin. Rijit-plastikte wu = CMOD3 (B06).</summary>
    Public Function Wu(lcs As Double) As Double
        If WuOverride > 0.0 Then Return WuOverride
        If Not Linear Then Return Cmod3
        Return Math.Min(lcs * EpsFu, WMaxLimit)
    End Function

    ''' <summary>B05 (5.6-5): fFts,k = 0.45 fR1k</summary>
    Public Function FtsK() As Double
        Return 0.45 * FR1
    End Function

    ''' <summary>B05 (5.6-6): fFtu,k = fFts - wu/CMOD3 (fFts - 0.5 fR3 + 0.2 fR1) &gt;= 0 ; rijit-plastik: fR3/3 (B04)</summary>
    Public Function FtuK(wu As Double) As Double
        If Not Linear Then Return FR3 / 3.0
        Dim fts As Double = FtsK()
        Return Math.Max(0.0, fts - wu / Cmod3 * (fts - 0.5 * FR3 + 0.2 * FR1))
    End Function

    ''' <summary>Tasarim cekme gerilmesi (B10: /gammaF) carlak genisligi w'de. wu SINIR DURUMDUR (5.6.4, 5.6-12; Critic 2026-10-28):
    ''' w &gt; wu bolgesi katki vermez (sigma=0, muhafazakar). Rijit-plastik modelde w sinirsiz kabul edilir (wu=CMOD3 icin fFtu sabit).</summary>
    Public Function SigmaD(w As Double, lcs As Double) As Double
        Dim wuV As Double = Wu(lcs)
        Dim ftu As Double = FtuK(wuV)
        If Not Linear Then Return ftu / GammaF
        If w > wuV Then Return 0.0
        Dim fts As Double = FtsK()
        Return (fts - (fts - ftu) * w / wuV) / GammaF
    End Function

    ''' <summary>B03b (5.6-3): fR3k/fR1k &gt; 0.5 ; (5.6-2): fR1k/fLk &gt; 0.4 (fLk verilirse)</summary>
    Public Function CanSubstituteRebar(Optional fLk As Double = 0.0) As Boolean
        If FR1 <= 0.0 Then Return False
        If FR3 / FR1 <= 0.5 Then Return False
        If fLk > 0.0 AndAlso FR1 / fLk <= 0.4 Then Return False
        Return True
    End Function
End Class

''' <summary>Lifli dikdortgen kiris egilme kapasitesi: ACI 318M-14 basinc blogu + celik donati + fib MC2010 lif cekme katkisi.
''' Cekme bolgesinde carlak genisligi w(y) = lcs * epsCu * y / c  (y: tarafsiz eksenden uzaklik).</summary>
Public Module SfrcBeam

    Public Function Flexure(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fy As Double,
                            p As SfrcParams, lcs As Double) As FlexureResult
        Dim b1 As Double = Aci.Beta1(fc)
        Dim lo As Double = 0.0001 * d
        Dim hi As Double = h
        For i As Integer = 0 To 199
            Dim mid As Double = 0.5 * (lo + hi)
            If Balance(b, h, d, As_, fc, fy, p, lcs, mid) > 0.0 Then hi = mid Else lo = mid
        Next
        Dim c As Double = 0.5 * (lo + hi)
        Dim r As FlexureResult
        r.C = c
        r.A = Math.Min(b1 * c, h)
        ' lif siniri: alt lifte w = lcs*epsCu*(h-c)/c > wu ise bolgenin wu otesi kismi katki vermez (EpsFuOk=False)
        r.FibreLimited = p.Linear AndAlso lcs * Aci.EpsCu * (h - c) / c > p.Wu(lcs)
        r.EpsT = Aci.EpsCu * (d - c) / c
        Dim fs As Double = Math.Min(fy, Aci.Es * r.EpsT)
        Dim fsForce As Double = As_ * fs
        Dim m As Double = fsForce * (d - r.A / 2.0)
        Dim n As Integer = 400
        Dim dy As Double = (h - c) / n
        For k As Integer = 0 To n - 1
            Dim y As Double = (k + 0.5) * dy
            Dim sg As Double = p.SigmaD(lcs * Aci.EpsCu * y / c, lcs)
            m += sg * b * dy * (c + y - r.A / 2.0)
        Next
        r.Mn = m
        r.Phi = Aci.Phi(r.EpsT, fy)
        r.PhiMn = r.Phi * r.Mn
        r.EpsTOk = r.EpsT >= 0.004
        Return r
    End Function

    ' ---- Kesme: fib Bulletin 56 (MC2010 first complete draft) Vol.2, 7.7.3.2 (PDF s.179-181, gorselden okundu). NIHAI METINLE KARSILASTIRILMADI. ----

    ''' <summary>Tablo 5.1-1 (Vol.1, PDF 157): fctk,min = 0.7 fctm, fctm = 0.30 fck^(2/3) (fck &lt;= 50 MPa). C30 icin 2.0 MPa dogrulandi.</summary>
    Public Function FctkMin(fck As Double) As Double
        Return 0.7 * 0.3 * Math.Pow(fck, 2.0 / 3.0)
    End Function

    ''' <summary>fFtuk kesme icin wu = 1.5 mm ile (Eq. 7.7-4, 7.7-8 metni); lineer model (5.6-6).</summary>
    Public Function FFtukShear(p As SfrcParams) As Double
        Dim q As New SfrcParams
        q.FR1 = p.FR1
        q.FR3 = p.FR3
        q.Linear = True
        q.Cmod3 = p.Cmod3
        Return q.FtuK(1.5)
    End Function

    ''' <summary>Eq. 7.7-4 (N): VRd,F = {0.18/gc k [100 rl (1+7.5 fFtuk/fctk) fck]^(1/3) + 0.15 scp} bw d ;
    ''' k = 1+sqrt(200/d) &lt;= 2 ; rl = Asl/(bw d) ; scp = NEd/Ac &lt; 0.2 fcd (basinc +). Alt sinir Eq. 7.7-5.</summary>
    Public Function VRdF(bw As Double, d As Double, Asl As Double, fck As Double, fctk As Double, fFtuk As Double,
                         gammaC As Double, Optional sigmaCp As Double = 0.0) As Double
        Dim k As Double = Math.Min(2.0, 1.0 + Math.Sqrt(200.0 / d))
        Dim rl As Double = Asl / (bw * d)
        Dim v1 As Double = (0.18 / gammaC) * k * Math.Pow(100.0 * rl * (1.0 + 7.5 * fFtuk / fctk) * fck, 1.0 / 3.0) + 0.15 * sigmaCp
        Dim vmin As Double = 0.035 * Math.Pow(k, 1.5) * Math.Sqrt(fck) + 0.15 * sigmaCp
        Return Math.Max(v1, vmin) * bw * d
    End Function

    ''' <summary>Etriyeli kesme: VRd = VRd,F(7.7-4: beton+lif tek terim) + Vs (ACI 22.5.10.5.3: Av fyt d / s).
    ''' YORUM (DOGRULANAMADI): Bull.56 7.7-7 etriyeli VRd,F icin sayisal denklem vermez (CODE_PROVISIONS_EVIDENCE B20). Tez kodu da ayni
    ''' yorumu kullaniyor (ETABSClass.vb:1089-1092). Lif katkisini ihmal eden muhafazakar sonuc icin fFtuk=0 verin.
    ''' Vs ust siniri 0.66 sqrt(fc) bw d (ACI 22.5.1.2, A14) uygulanir.</summary>
    Public Function VRdStirrup(bw As Double, d As Double, Asl As Double, fck As Double, fctk As Double, fFtuk As Double,
                               gammaC As Double, Av As Double, fyt As Double, s As Double, Optional sigmaCp As Double = 0.0) As Double
        Dim vsv As Double = Math.Min(Av * fyt * d / s, AciBeam.VsMax(bw, d, fck))
        Return VRdF(bw, d, Asl, fck, fctk, fFtuk, gammaC, sigmaCp) + vsv
    End Function

    ''' <summary>Eq. 7.7-8: etriyesiz minimum donati muafiyeti icin fFtuk &gt;= sqrt(fck)/20</summary>
    Public Function MinStirrupWaived(fFtuk As Double, fck As Double) As Boolean
        Return fFtuk >= Math.Sqrt(fck) / 20.0
    End Function

    ''' <summary>Denge: Cc - Fs - Ff (pozitif ise c cok buyuk)</summary>
    Private Function Balance(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fy As Double,
                             p As SfrcParams, lcs As Double, c As Double) As Double
        Dim cc As Double = 0.85 * fc * b * Math.Min(Aci.Beta1(fc) * c, h)
        Dim fs As Double = Math.Min(fy, Aci.Es * Aci.EpsCu * (d - c) / c)
        Dim ff As Double = 0.0
        Dim n As Integer = 400
        Dim dy As Double = (h - c) / n
        For k As Integer = 0 To n - 1
            Dim y As Double = (k + 0.5) * dy
            ff += p.SigmaD(lcs * Aci.EpsCu * y / c, lcs) * b * dy
        Next
        Return cc - As_ * fs - ff
    End Function
End Module
