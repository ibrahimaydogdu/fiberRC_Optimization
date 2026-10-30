Imports System
Imports System.Globalization
Imports System.Linq
Imports System.IO
Imports RcCore

'V3-prep: grid of SFRC beam sections evaluated with RcCore.SfrcBeam.Flexure, written as CSV for an independent re-implementation (Python, numerical integration of the same fib MC2010 model).
Module SfrcX
    Sub Run(path As String)
        Dim ci As CultureInfo = CultureInfo.InvariantCulture
        Using w As New StreamWriter(path)
            w.WriteLine("b,h,d,As,fc,fy,FR1,FR3,lcs,C,EpsT,Mn,FibreLimited")
            For Each fc In {30.0, 40.0}
                For Each hh In {400.0, 600.0, 800.0}
                    For Each rho In {0.004, 0.008, 0.015}
                        For Each fr In {Tuple.Create(2.0, 1.2), Tuple.Create(3.5, 2.8), Tuple.Create(5.0, 4.5), Tuple.Create(1.5, 0.6)}
                            For Each lf In {0.5, 0.7, 1.0}
                                Dim b As Double = 300.0, d As Double = hh - 60.0
                                Dim a As Double = rho * b * d
                                Dim p As New SfrcParams With {.FR1 = fr.Item1, .FR3 = fr.Item2}
                                Dim r = SfrcBeam.Flexure(b, hh, d, a, fc, 420.0, p, lf * hh)
                                w.WriteLine(String.Join(",", {b, hh, d, a, fc, 420.0, fr.Item1, fr.Item2, lf * hh, r.C, r.EpsT, r.Mn, If(r.FibreLimited, 1, 0)}.Select(Function(x) Convert.ToString(x, ci))))
                            Next
                        Next
                    Next
                Next
            Next
        End Using
    End Sub
End Module
