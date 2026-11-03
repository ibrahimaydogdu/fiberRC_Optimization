Imports System
Imports System.Collections.Generic
Imports RcCore

'Ex1 optimisation problem: integer design vector (44 variables, 10 groups) <-> Ex1Design; objective = penalised cost F(1+T)^2 (TL).
'Column group (4 variables): b, h (300..700 mm step 50), rho (1..4 % step 0.25), concrete class (C30, C40).
'Beam group (5 variables): bw (250..500 step 50), h (300..800 step 50), concrete class, fibre type (4D, 5D), dose index (0, 20, 25, 30, 35, 40 kg/m3; 0 = plain RC).
'Plain mode (UseFibre = False): fibre variables fixed (lb = ub = 0). Fibre doses outside the measured grid of the chosen type and class are repaired to the nearest grid dose (no extrapolation).
Public Class Ex1Problem
    Public Shared ReadOnly ColGroups As String() = {"CCorner_S12", "CCorner_S34", "CEdge_S12", "CEdge_S34", "CInter_S12", "CInter_S34"}
    Public Shared ReadOnly BeamGroups As String() = {"Bx_S12", "Bx_S34", "By_S12", "By_S34"}
    Public Shared ReadOnly Classes As Double() = {30.0, 40.0}
    Public Shared ReadOnly Doses As Double() = {0.0, 20.0, 25.0, 30.0, 35.0, 40.0}
    Public Shared ReadOnly FibreTypes As String() = {"4D", "5D"}
    Public Const ColVars As Integer = 4, BeamVars As Integer = 5

    Public ReadOnly Spec As FrameSpec, Tp As TbdyParams, Dd3 As TbdyParams, Prices As PriceSet
    Public ReadOnly UseFibre As Boolean, FRScale As Double
    Public ReadOnly Lb() As Integer, Ub() As Integer              'bounds the optimisers see (compact in the derived mode)
    Public ReadOnly FullLb() As Integer, FullUb() As Integer     'bounds of the full 44-variable vector (identical to Lb/Ub in the legacy mode)
    Public ReadOnly DeriveRho As Boolean                           'PCR-8: column rho and column concrete class are not search variables; the evaluator derives them (C30 unless the column cannot be designed in C30 at 4 % rho; smallest grid rho that satisfies the column checks and ACI 18.7.3.2)
    Private ReadOnly Active() As Integer                           'derived mode: full index of every search variable

    Public Sub New(spec As FrameSpec, tp As TbdyParams, dd3 As TbdyParams, prices As PriceSet, useFibre As Boolean, Optional fRScale As Double = 1.0, Optional deriveRho As Boolean = False)
        Me.Spec = spec : Me.Tp = tp : Me.Dd3 = dd3 : Me.Prices = prices : Me.UseFibre = useFibre : Me.FRScale = fRScale : Me.DeriveRho = deriveRho
        Dim n As Integer = ColGroups.Length * ColVars + BeamGroups.Length * BeamVars
        Dim Lb(n - 1) As Integer, Ub(n - 1) As Integer
        Dim k As Integer = 0
        For g As Integer = 0 To ColGroups.Length - 1
            Ub(k) = 8 : k += 1           'b: 300..700
            Ub(k) = 8 : k += 1           'h: 300..700
            Ub(k) = 12 : k += 1          'rho: 1..4 %
            Ub(k) = 1 : k += 1           'class
        Next
        For g As Integer = 0 To BeamGroups.Length - 1
            Ub(k) = 5 : k += 1           'bw: 250..500
            Ub(k) = 10 : k += 1          'h: 300..800
            Ub(k) = 1 : k += 1           'class
            Ub(k) = If(useFibre, 1, 0) : k += 1                  'fibre type
            Ub(k) = If(useFibre, Doses.Length - 1, 0) : k += 1   'dose index
        Next
        FullLb = Lb : FullUb = Ub
        If deriveRho Then
            Dim act As New List(Of Integer)
            For i As Integer = 0 To n - 1
                Dim isDerived As Boolean = i < ColGroups.Length * ColVars AndAlso (i Mod ColVars = 2 OrElse i Mod ColVars = 3)      'column rho and column concrete class
                If Not isDerived AndAlso Ub(i) > Lb(i) Then act.Add(i)      'derived column variables and the fixed (plain mode) fibre variables leave the search
            Next
            Active = act.ToArray()
            Dim cl(Active.Length - 1) As Integer, cu(Active.Length - 1) As Integer
            For a As Integer = 0 To Active.Length - 1 : cl(a) = Lb(Active(a)) : cu(a) = Ub(Active(a)) : Next
            Me.Lb = cl : Me.Ub = cu
        Else
            Me.Lb = Lb : Me.Ub = Ub
        End If
    End Sub

    'full 44-vector <-> the vector the optimisers use (identical in the legacy mode); rho entries of the full vector are ignored in the derived mode
    Public Function ToFull(x() As Integer) As Integer()
        If Not DeriveRho Then Return x
        Dim f(FullLb.Length - 1) As Integer
        For a As Integer = 0 To Active.Length - 1 : f(Active(a)) = x(a) : Next
        Return f
    End Function

    Public Function ToActive(f() As Integer) As Integer()
        If Not DeriveRho Then Return f
        Dim x(Active.Length - 1) As Integer
        For a As Integer = 0 To Active.Length - 1 : x(a) = f(Active(a)) : Next
        Return x
    End Function

    'PCR-6: restrict the search to a box (per-variable index range) inside the full pool. The optimisers already honour lb <> 0; Decode uses the absolute index,
    'so without a box (Lb = 0) the behaviour is identical to the frozen campaign (regression test). An empty intersection is an error (never silently widened).
    Public Sub ApplyBox(lo() As Integer, hi() As Integer)        'lo, hi: full 44-vectors
        If lo.Length <> FullLb.Length OrElse hi.Length <> FullLb.Length Then Throw New ArgumentException("box length differs from the number of variables")
        For a As Integer = 0 To Lb.Length - 1
            Dim i As Integer = If(DeriveRho, Active(a), a)
            Dim l As Integer = Math.Max(Lb(a), lo(i)), u As Integer = Math.Min(Ub(a), hi(i))
            If l > u Then Throw New ArgumentException("empty box for variable " & i & ": [" & lo(i) & "," & hi(i) & "] outside the pool [" & Lb(a) & "," & Ub(a) & "]")
            Lb(a) = l : Ub(a) = u
        Next
    End Sub

    Public ReadOnly Property NVars As Integer
        Get
            Return Lb.Length
        End Get
    End Property

    'repaired dose for a type and class (0 stays 0)
    Public Shared Function RepairDose(fibreType As String, fck As Double, dose As Double) As Double
        If dose <= 0.0 Then Return 0.0
        Dim lo As Double, hi As Double
        If Not FibreTable.DoseRange(fibreType, fck, lo, hi) Then Return 0.0
        Return Math.Max(lo, Math.Min(hi, dose))
    End Function

    Public Function Decode(x() As Integer) As Ex1Design
        Dim d As New Ex1Design
        Dim k As Integer = 0
        For g As Integer = 0 To ColGroups.Length - 1
            d.Columns(ColGroups(g)) = New ColumnDef With {.B = 300.0 + 50.0 * x(k), .H = 300.0 + 50.0 * x(k + 1), .Rho = 0.01 + 0.0025 * x(k + 2), .Fc = Classes(x(k + 3)), .RhoDerived = DeriveRho, .FcDerived = DeriveRho}
            k += ColVars
        Next
        For g As Integer = 0 To BeamGroups.Length - 1
            Dim fc As Double = Classes(x(k + 2))
            Dim bd As New BeamDef With {.B = 250.0 + 50.0 * x(k), .H = 300.0 + 50.0 * x(k + 1), .Fc = fc}
            If UseFibre AndAlso x(k + 4) > 0 Then
                Dim t As String = FibreTypes(x(k + 3))
                Dim dose As Double = RepairDose(t, fc, Doses(x(k + 4)))
                Dim ok As Boolean
                bd.Fibre = FibreTable.MakeBeamFibre(t, fc, dose, ok, FRScale)
            End If
            d.Beams(BeamGroups(g)) = bd
            k += BeamVars
        Next
        Return d
    End Function

    Public Function Evaluate(x() As Integer) As Ex1Result        'x: search vector (compact in the derived mode)
        Return Ex1Evaluator.Evaluate(Spec, Decode(ToFull(x)), Tp, Dd3, Prices)
    End Function

    Public Function EvaluateFull(f() As Integer) As Ex1Result
        Return Ex1Evaluator.Evaluate(Spec, Decode(f), Tp, Dd3, Prices)
    End Function

    Public Function Objective(x() As Integer) As Double
        Return Evaluate(x).Fpen
    End Function
End Class
