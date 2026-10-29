Imports System
Imports System.Collections.Generic
Imports System.Linq

'Clean re-implementation of the thesis algorithms on integer design vectors (index into the catalogue of each variable):
'YAKO (artificial bee colony), BTO (biogeography-based optimisation) and ATO (tree-seed algorithm). Methodology from OptimizationClass.vb (Sources/Betonarme/Lifli/v2);
'deliberate corrections of the code-review findings: F-18 explicit seeded RNG per run; F-19 budget counted per candidate evaluation, identical MaxFEs for all methods;
'F-29/F-34 no duplicate-removal or reference-equality defects; roulette index bound; ATO updates every dimension (the original left unassigned dimensions at 0).
'The objective is the penalised cost F(1+T)^2; all methods use the same bounds, the same objective and the same FE counter.
Public Class OptResult
    Public Method As String
    Public BestX() As Integer
    Public BestF As Double
    Public FEs As Integer
    Public History As New List(Of Tuple(Of Integer, Double))      'FE index, best-so-far (improvements only)
End Class

Public Class FeBudget
    Public ReadOnly Max As Integer
    Public Used As Integer
    Public BestF As Double = Double.PositiveInfinity
    Public BestX() As Integer
    Public ReadOnly Hist As New List(Of Tuple(Of Integer, Double))
    Private ReadOnly Obj As Func(Of Integer(), Double)
    Public Sub New(max As Integer, obj As Func(Of Integer(), Double))
        Me.Max = max : Me.Obj = obj
    End Sub
    Public ReadOnly Property Exhausted As Boolean
        Get
            Return Used >= Max
        End Get
    End Property
    Public Function Eval(x() As Integer) As Double
        Dim f As Double = Obj(x)
        If Double.IsNaN(f) Then f = Double.PositiveInfinity          'NaN never passes as a result
        Used += 1
        If f < BestF Then
            BestF = f : BestX = CType(x.Clone(), Integer())
            Hist.Add(Tuple.Create(Used, f))
        End If
        Return f
    End Function
End Class

Public Module Optimizers
    Private Function RandomX(lb() As Integer, ub() As Integer, rnd As Random) As Integer()
        Dim x(lb.Length - 1) As Integer
        For i As Integer = 0 To lb.Length - 1 : x(i) = rnd.Next(lb(i), ub(i) + 1) : Next
        Return x
    End Function

    Private Function Clamp(v As Double, lo As Integer, hi As Integer) As Integer
        Dim r As Integer = CInt(Math.Floor(v + 0.5))
        Return Math.Max(lo, Math.Min(hi, r))
    End Function

    Private Function Finish(method As String, b As FeBudget) As OptResult
        Return New OptResult With {.Method = method, .BestX = b.BestX, .BestF = b.BestF, .FEs = b.Used, .History = b.Hist}
    End Function

    'YAKO / ABC: sn food sources, limit trials, employed + onlooker + scout phases, one random dimension per move, greedy selection
    Public Function Abc(obj As Func(Of Integer(), Double), lb() As Integer, ub() As Integer, maxFEs As Integer, seed As Integer, Optional sn As Integer = 100, Optional limit As Integer = 50) As OptResult
        Dim rnd As New Random(seed)
        Dim b As New FeBudget(maxFEs, obj)
        Dim nd As Integer = lb.Length
        Dim x As New List(Of Integer())(), f As New List(Of Double)(), trial(sn - 1) As Integer
        For i As Integer = 0 To sn - 1
            If b.Exhausted Then Exit For
            Dim xi = RandomX(lb, ub, rnd) : x.Add(xi) : f.Add(b.Eval(xi))
        Next
        If x.Count < sn Then Return Finish("YAKO", b)
        Dim tryMove = Sub(i As Integer)
                          If b.Exhausted Then Return
                          Dim k As Integer
                          Do : k = rnd.Next(sn) : Loop While k = i
                          Dim j As Integer = rnd.Next(nd)
                          Dim v = CType(x(i).Clone(), Integer())
                          Dim step_ As Double = (rnd.NextDouble() - 0.5) * 2.0 * (x(i)(j) - x(k)(j))
                          v(j) = Clamp(x(i)(j) + step_, lb(j), ub(j))
                          If v(j) = x(i)(j) Then v(j) = Clamp(x(i)(j) + If(rnd.Next(2) = 0, -1, 1), lb(j), ub(j))      'integer grid: avoid null moves
                          Dim fv = b.Eval(v)
                          If fv < f(i) Then
                              x(i) = v : f(i) = fv : trial(i) = 0
                          Else
                              trial(i) += 1
                          End If
                      End Sub
        While Not b.Exhausted
            For i As Integer = 0 To sn - 1 : tryMove(i) : Next
            'onlooker: probability proportional to fitness 1/(1+f - fmin) mapped into (0,1]; bounded loop (no infinite-wait defect F-29)
            Dim fmin As Double = f.Min()
            Dim fit(sn - 1) As Double, sum As Double = 0.0
            For i As Integer = 0 To sn - 1
                fit(i) = If(Double.IsInfinity(f(i)), 0.0, 1.0 / (1.0 + (f(i) - fmin) / Math.Max(Math.Abs(fmin), 1.0)))
                sum += fit(i)
            Next
            For o As Integer = 0 To sn - 1
                If b.Exhausted Then Exit For
                Dim pick As Integer = rnd.Next(sn)
                If sum > 0.0 Then
                    Dim r As Double = rnd.NextDouble() * sum, acc As Double = 0.0
                    pick = sn - 1
                    For i As Integer = 0 To sn - 1
                        acc += fit(i)
                        If r <= acc Then pick = i : Exit For
                    Next
                End If
                tryMove(pick)
            Next
            'scout: the source with the largest trial counter above the limit is replaced (one per cycle)
            Dim worst As Integer = 0
            For i As Integer = 1 To sn - 1 : If trial(i) > trial(worst) Then worst = i
            Next
            If trial(worst) > limit AndAlso Not b.Exhausted Then
                Dim xs = RandomX(lb, ub, rnd) : x(worst) = xs : f(worst) = b.Eval(xs) : trial(worst) = 0
            End If
        End While
        Return Finish("YAKO", b)
    End Function

    'BTO / BBO: habitats ranked by cost each generation; mu_i = (N-i)/N, lambda_i = 1 - mu_i; immigration source by roulette on mu; mutation (uniform reset); greedy replacement
    Public Function Bbo(obj As Func(Of Integer(), Double), lb() As Integer, ub() As Integer, maxFEs As Integer, seed As Integer, Optional np As Integer = 100, Optional mutation As Double = 0.1) As OptResult
        Dim rnd As New Random(seed)
        Dim b As New FeBudget(maxFEs, obj)
        Dim nd As Integer = lb.Length
        Dim pop As New List(Of Tuple(Of Integer(), Double))()
        For i As Integer = 0 To np - 1
            If b.Exhausted Then Exit For
            Dim xi = RandomX(lb, ub, rnd) : pop.Add(Tuple.Create(xi, b.Eval(xi)))
        Next
        If pop.Count < np Then Return Finish("BTO", b)
        Dim mu(np - 1) As Double
        For i As Integer = 0 To np - 1 : mu(i) = (np - i) / CDbl(np) : Next
        Dim muSum As Double = mu.Sum()
        While Not b.Exhausted
            pop = pop.OrderBy(Function(t) t.Item2).ToList()
            Dim snapshot = pop.Select(Function(t) t.Item1).ToList()
            For i As Integer = 0 To np - 1
                If b.Exhausted Then Exit For
                Dim lam As Double = 1.0 - mu(i)
                Dim v = CType(snapshot(i).Clone(), Integer())
                For j As Integer = 0 To nd - 1
                    If rnd.NextDouble() < lam Then
                        Dim r As Double = rnd.NextDouble() * muSum, acc As Double = 0.0, src As Integer = np - 1
                        For s As Integer = 0 To np - 1
                            acc += mu(s)
                            If r <= acc Then src = s : Exit For
                        Next
                        v(j) = snapshot(src)(j)
                    End If
                    If rnd.NextDouble() < mutation Then v(j) = rnd.Next(lb(j), ub(j) + 1)
                Next
                Dim fv = b.Eval(v)
                If fv < pop(i).Item2 Then pop(i) = Tuple.Create(v, fv)
            Next
        End While
        Return Finish("BTO", b)
    End Function

    'ATO / TSA (Kiran 2015): each tree produces ns seeds (10-25 % of the population); every dimension of a seed moves towards the best tree (prob = search tendency) or a random tree; the best seed replaces the tree if better
    Public Function Tsa(obj As Func(Of Integer(), Double), lb() As Integer, ub() As Integer, maxFEs As Integer, seed As Integer, Optional np As Integer = 20, Optional st As Double = 0.5) As OptResult
        Dim rnd As New Random(seed)
        Dim b As New FeBudget(maxFEs, obj)
        Dim nd As Integer = lb.Length
        Dim x As New List(Of Integer())(), f As New List(Of Double)()
        For i As Integer = 0 To np - 1
            If b.Exhausted Then Exit For
            Dim xi = RandomX(lb, ub, rnd) : x.Add(xi) : f.Add(b.Eval(xi))
        Next
        If x.Count < np Then Return Finish("ATO", b)
        Dim nLow As Integer = CInt(Math.Round(np * 0.1)), nHigh As Integer = CInt(Math.Round(np * 0.25))
        While Not b.Exhausted
            Dim bi As Integer = 0
            For i As Integer = 1 To np - 1 : If f(i) < f(bi) Then bi = i
            Next
            Dim best = CType(x(bi).Clone(), Integer())
            For i As Integer = 0 To np - 1
                If b.Exhausted Then Exit For
                Dim ns As Integer = rnd.Next(nLow, nHigh + 1)
                Dim sBest As Integer() = Nothing, sF As Double = Double.PositiveInfinity
                For q As Integer = 1 To ns
                    If b.Exhausted Then Exit For
                    Dim r As Integer
                    Do : r = rnd.Next(np) : Loop While r = i
                    Dim v = CType(x(i).Clone(), Integer())
                    For j As Integer = 0 To nd - 1
                        Dim alpha As Double = rnd.NextDouble() * 2.0 - 1.0
                        If rnd.NextDouble() < st Then
                            v(j) = Clamp(x(i)(j) + alpha * (best(j) - x(r)(j)), lb(j), ub(j))
                        Else
                            v(j) = Clamp(x(i)(j) + alpha * (x(i)(j) - x(r)(j)), lb(j), ub(j))
                        End If
                    Next
                    Dim fv = b.Eval(v)
                    If fv < sF Then sF = fv : sBest = v
                Next
                If sBest IsNot Nothing AndAlso sF < f(i) Then x(i) = sBest : f(i) = sF
            Next
        End While
        Return Finish("ATO", b)
    End Function

    Public Function Run(method As String, obj As Func(Of Integer(), Double), lb() As Integer, ub() As Integer, maxFEs As Integer, seed As Integer) As OptResult
        Select Case method
            Case "YAKO" : Return Abc(obj, lb, ub, maxFEs, seed)
            Case "BTO" : Return Bbo(obj, lb, ub, maxFEs, seed)
            Case "ATO" : Return Tsa(obj, lb, ub, maxFEs, seed)
        End Select
        Throw New ArgumentException("unknown method " & method)
    End Function
End Module
