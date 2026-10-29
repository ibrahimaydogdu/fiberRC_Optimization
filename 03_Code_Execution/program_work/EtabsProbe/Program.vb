Imports System
Imports ETABSv1

Module Program
    Sub Main()
        Dim helper As cHelper = New Helper()
        Dim obj As cOAPI = Nothing
        Try
            obj = helper.CreateObjectProgID("CSI.ETABS.API.ETABSObject")
            Dim ret As Integer = obj.ApplicationStart()
            Console.WriteLine("ApplicationStart ret=" & ret)
            Dim m As cSapModel = obj.SapModel
            Dim ver As String = "", vnum As Double = 0
            m.GetVersion(ver, vnum)
            Console.WriteLine("ETABS version " & ver & " (" & vnum & ")")
            ret = m.InitializeNewModel(eUnits.kN_m_C)
            ret = m.File.NewBlank()
            Console.WriteLine("NewBlank ret=" & ret)
            obj.ApplicationExit(False)
        Catch ex As Exception
            Console.WriteLine("ERR " & ex.GetType().Name & ": " & ex.Message)
            Try : If obj IsNot Nothing Then obj.ApplicationExit(False)
            Catch : End Try
        End Try
    End Sub
End Module
