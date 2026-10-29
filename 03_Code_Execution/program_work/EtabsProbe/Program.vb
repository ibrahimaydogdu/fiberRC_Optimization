Imports System
Imports ETABSv1

Module Program
    Sub Main()
        Dim helper As cHelper = New Helper()
        Dim obj As cOAPI = helper.CreateObjectProgID("CSI.ETABS.API.ETABSObject")
        obj.ApplicationStart()
        Dim m As cSapModel = obj.SapModel
        m.InitializeNewModel(eUnits.kN_m_C)
        m.File.NewBlank()
        Dim n As Integer, keys() As String, names() As String, imp() As Integer
        m.DatabaseTables.GetAvailableTables(n, keys, names, imp)
        For i As Integer = 0 To n - 1
            If names(i).Contains("Mass Source") OrElse names(i).Contains("Response Spectrum") OrElse names(i).Contains("Functions") OrElse names(i).Contains("Modal Case") Then Console.WriteLine(keys(i) & " | " & names(i) & " | import " & imp(i))
        Next
        For Each tk In {"Mass Source Definition", "Functions - Response Spectrum - User Defined"}
            Dim ver As Integer, nf As Integer, fk() As String, fn() As String, ds() As String, un() As String, ok() As Boolean
            Dim r As Integer = m.DatabaseTables.GetAllFieldsInTable(tk, ver, nf, fk, fn, ds, un, ok)
            Console.WriteLine("== " & tk & " ret=" & r & " fields=" & nf)
            For i As Integer = 0 To nf - 1 : Console.WriteLine("   " & fk(i) & " | " & fn(i)) : Next
        Next
        obj.ApplicationExit(False)
    End Sub
End Module
