Imports System
Imports System.Collections.Generic
Imports FemRcModel
Imports RcCore

'PCR-16 (EX2_SPEC v2 §1.2, §4, §8): integer design vector <-> Ex2Design.
'Column group: b (250..450 step 50; out-of-plane), h (300..600 step 50; in-plane). C30 fixed (Q5); rho derived by the evaluator (PCR-8, grid 1..4 %).
'Beam group: bw (250..450), h (300..600); fibre mode adds fibre type (4D, 5D) and dose index (0, 20, 25, 30, 35, 40 kg/m3; 0 = plain).
'Variables: 9 groups -> plain 18, fibre 24 (spec §4); 5 groups -> plain 10, fibre 16. Only active variables are in the vector (no fixed entries).
Public Class Ex2Problem
    Public Shared ReadOnly BPool As Double() = {250, 300, 350, 400, 450}
    Public Shared ReadOnly HPool As Double() = {300, 350, 400, 450, 500, 550, 600}
    Public Shared ReadOnly Doses As Double() = {0.0, 20.0, 25.0, 30.0, 35.0, 40.0}
    Public Shared ReadOnly FibreTypes As String() = {"4D", "5D"}
    Public Const Fc As Double = 30.0

    Public ReadOnly Spec As FrameSpec2D, Prices As PriceSet, UseFibre As Boolean, FRScale As Double
    Public ReadOnly Shear As IBeamShear
    Public ReadOnly ColGroups As String(), BeamGroups As String()
    Public ReadOnly Lb() As Integer, Ub() As Integer
    Public ReadOnly BeamVars As Integer

    Public Sub New(spec As FrameSpec2D, prices As PriceSet, useFibre As Boolean, shear As IBeamShear, Optional fRScale As Double = 1.0)
        Me.Spec = spec : Me.Prices = prices : Me.UseFibre = useFibre : Me.Shear = shear : Me.FRScale = fRScale
        If spec.Groups = 9 Then
            ColGroups = {"C_Edge_S1", "C_Edge_S2", "C_Edge_S3", "C_Inter_S1", "C_Inter_S2", "C_Inter_S3"}
        ElseIf spec.Groups = 5 Then
            ColGroups = {"C_Edge", "C_Inter"}
        Else
            Throw New ArgumentException("Groups must be 9 or 5")
        End If
        BeamGroups = {"B_S1", "B_S2", "B_S3"}
        BeamVars = If(useFibre, 4, 2)
        Dim n As Integer = ColGroups.Length * 2 + BeamGroups.Length * BeamVars
        Dim l(n - 1) As Integer, u(n - 1) As Integer
        Dim k As Integer = 0
        For g As Integer = 0 To ColGroups.Length - 1
            u(k) = BPool.Length - 1 : k += 1
            u(k) = HPool.Length - 1 : k += 1
        Next
        For g As Integer = 0 To BeamGroups.Length - 1
            u(k) = BPool.Length - 1 : k += 1
            u(k) = HPool.Length - 1 : k += 1
            If useFibre Then
                u(k) = FibreTypes.Length - 1 : k += 1
                u(k) = Doses.Length - 1 : k += 1
            End If
        Next
        Lb = l : Ub = u
    End Sub

    Public ReadOnly Property NVars As Integer
        Get
            Return Lb.Length
        End Get
    End Property

    'spec §8 (Ö1 Ex1Problem.RepairDose logic, re-written): dose <= 0 -> 0, otherwise clipped to the measured range of the type (C30: 4D 20-40, 5D 25-35)
    Public Shared Function RepairDose(fibreType As String, fck As Double, dose As Double) As Double
        If dose <= 0.0 Then Return 0.0
        Dim lo As Double, hi As Double
        If Not FibreTable.DoseRange(fibreType, fck, lo, hi) Then Return 0.0
        Return Math.Max(lo, Math.Min(hi, dose))
    End Function

    Public Function Decode(x() As Integer) As Ex2Design
        If x.Length <> NVars Then Throw New ArgumentException("design vector length " & x.Length & " <> " & NVars)
        For i As Integer = 0 To NVars - 1
            If x(i) < Lb(i) OrElse x(i) > Ub(i) Then Throw New ArgumentOutOfRangeException("x(" & i & ") = " & x(i) & " outside [" & Lb(i) & "," & Ub(i) & "]")
        Next
        Dim d As New Ex2Design
        Dim k As Integer = 0
        For Each g In ColGroups
            d.Columns(g) = New ColumnDef With {.B = BPool(x(k)), .H = HPool(x(k + 1)), .Fc = Fc, .Rho = 0.01, .BarDia = Ex2Evaluator.BarDia, .RhoDerived = True}
            k += 2
        Next
        For Each g In BeamGroups
            Dim bd As New BeamDef With {.B = BPool(x(k)), .H = HPool(x(k + 1)), .Fc = Fc}
            If UseFibre AndAlso x(k + 3) > 0 Then
                Dim t As String = FibreTypes(x(k + 2))
                Dim dose As Double = RepairDose(t, Fc, Doses(x(k + 3)))
                Dim ok As Boolean
                bd.Fibre = FibreTable.MakeBeamFibre(t, Fc, dose, ok, FRScale)
                If Not ok Then Throw New InvalidOperationException("fibre grid lookup failed: " & t & " " & dose)
            End If
            d.Beams(g) = bd
            k += BeamVars
        Next
        Return d
    End Function

    Public Function Evaluate(x() As Integer) As Ex2Result
        Return Ex2Evaluator.Evaluate(Spec, Decode(x), Prices, Shear)
    End Function

    'Fixed design from explicit sizes (V1 / V2-a / evalonly): sizes(group) = (b, h) mm; columns rho derived unless rho > 0 is given
    Public Shared Function FromSizes(sizes As Dictionary(Of String, Double()), Optional rho As Double = 0.0) As Ex2Design
        Dim d As New Ex2Design
        For Each kv In sizes
            If kv.Key.StartsWith("C") Then
                d.Columns(kv.Key) = New ColumnDef With {.B = kv.Value(0), .H = kv.Value(1), .Fc = Fc, .Rho = If(rho > 0.0, rho, 0.01), .BarDia = Ex2Evaluator.BarDia, .RhoDerived = rho <= 0.0}
            Else
                d.Beams(kv.Key) = New BeamDef With {.B = kv.Value(0), .H = kv.Value(1), .Fc = Fc}
            End If
        Next
        Return d
    End Function

    'Source Table 1 sections (PDF s.6), 5-group mode: edge columns 25x30, interior 30x30 (b x h, h in-plane), beams B1 25x35, B2 25x30, B3 25x55
    Public Shared Function SourceTable1() As Dictionary(Of String, Double())
        Return New Dictionary(Of String, Double()) From {
            {"C_Edge", {250.0, 300.0}}, {"C_Inter", {300.0, 300.0}},
            {"B_S1", {250.0, 350.0}}, {"B_S2", {250.0, 300.0}}, {"B_S3", {250.0, 550.0}}}
    End Function
End Class
