Imports System
Imports System.Collections.Generic

'Steel-fibre fR1 / fR3 (MPa) from the supplier design data (65/60 BG, hooked-end 4D and 5D; Evidence/CMOD_test_verileri.xlsx sheets 4D_*, 5D_*).
'Source type: manufacturer design data, confidential (see 01_Literature/UNIT_PRICE_EVIDENCE.md section 5); values transcribed from the sheet, checked against EX1_SPEC.
'Whether the values are characteristic (fRik) is UNVERIFIED; the program treats them as characteristic [ASSUMPTION; fR1/fR3 sensitivity required].
'No extrapolation: doses outside the measured grid are infeasible; between grid doses fR1 and fR3 are linearly interpolated.
Public Module FibreTable
    Public Structure FibreGrid
        Public Doses() As Double
        Public FR1() As Double
        Public FR3() As Double
    End Structure

    Private ReadOnly Grids As New Dictionary(Of String, FibreGrid)

    Sub New()
        Grids("4D_C30") = Make({20, 25, 30, 35, 40}, {2.7, 3.3, 3.8, 4.3, 4.7}, {3.4, 4.1, 4.8, 5.3, 5.7})
        Grids("4D_C40") = Make({20, 25, 30, 35}, {3.0, 3.7, 4.3, 4.8}, {3.6, 4.4, 5.2, 5.7})
        Grids("5D_C30") = Make({25, 30, 35}, {3.1, 3.6, 4.1}, {4.4, 5.0, 5.6})
        Grids("5D_C40") = Make({25, 30, 35}, {3.5, 4.1, 4.7}, {5.1, 5.8, 6.4})
    End Sub

    Private Function Make(d() As Double, r1() As Double, r3() As Double) As FibreGrid
        Return New FibreGrid With {.Doses = d, .FR1 = r1, .FR3 = r3}
    End Function

    'type: "4D" or "5D"; fck: 30 or 40 MPa. ok = False when the class/dose is not covered by the measured grid.
    Public Function Lookup(fibreType As String, fck As Double, dose As Double, ByRef fr1 As Double, ByRef fr3 As Double) As Boolean
        fr1 = 0.0 : fr3 = 0.0
        Dim key As String = fibreType & "_C" & CInt(Math.Round(fck)).ToString()
        If Not Grids.ContainsKey(key) Then Return False
        Dim g = Grids(key)
        Dim n As Integer = g.Doses.Length
        If dose < g.Doses(0) - 1.0E-9 OrElse dose > g.Doses(n - 1) + 1.0E-9 Then Return False
        For i As Integer = 0 To n - 2
            If dose <= g.Doses(i + 1) + 1.0E-9 Then
                Dim t As Double = (dose - g.Doses(i)) / (g.Doses(i + 1) - g.Doses(i))
                fr1 = g.FR1(i) + t * (g.FR1(i + 1) - g.FR1(i))
                fr3 = g.FR3(i) + t * (g.FR3(i + 1) - g.FR3(i))
                Return True
            End If
        Next
        Return False
    End Function

    Public Function DoseRange(fibreType As String, fck As Double, ByRef lo As Double, ByRef hi As Double) As Boolean
        Dim key As String = fibreType & "_C" & CInt(Math.Round(fck)).ToString()
        If Not Grids.ContainsKey(key) Then Return False
        lo = Grids(key).Doses(0) : hi = Grids(key).Doses(Grids(key).Doses.Length - 1)
        Return True
    End Function

    'BeamFibre for the evaluator; Nothing for dose 0 (plain RC). ok = False if outside the grid.
    'fRScale multiplies the tabulated fR1 and fR3 (the sheet values are test means, not verified characteristic values: sensitivity factor, 1.0 = as tabulated)
    Public Function MakeBeamFibre(fibreType As String, fck As Double, dose As Double, ByRef ok As Boolean, Optional fRScale As Double = 1.0) As BeamFibre
        ok = True
        If dose <= 0.0 Then Return Nothing
        Dim a As Double, b As Double
        ok = Lookup(fibreType, fck, dose, a, b)
        If Not ok Then Return Nothing
        Return New BeamFibre With {.Params = New RcCore.SfrcParams With {.FR1 = a * fRScale, .FR3 = b * fRScale}, .DoseKgM3 = dose}
    End Function
End Module
