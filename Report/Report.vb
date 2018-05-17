Public Class Report
    Public Title As String
    Public Value As String
    Public Color As ConsoleColor
    Public TurnNumber As Integer

    Private Shared CurrentTurnNumber As Integer
    Private Shared ReportList As New List(Of Report)
    Public Shared Sub Add(ByVal title As String, ByVal value As String, ByVal color As ConsoleColor)
        Dim rep As New Report
        With rep
            .Title = title
            .Value = value
            .Color = color
            .TurnNumber = CurrentTurnNumber
        End With
        ReportList.Add(rep)
    End Sub
    Public Shared Sub TurnNumberAdvance(Optional ByVal numberOfAdvances As Integer = 1)
        CurrentTurnNumber += numberOfAdvances
    End Sub
    Public Shared Sub TurnNumberReset(Optional ByVal turnNumber As Integer = 0)
        CurrentTurnNumber = turnNumber
    End Sub
    Public Shared Sub ShowReports()
        For Each rep In ReportList
            If Console.ForegroundColor <> rep.Color Then Console.ForegroundColor = rep.Color
            Console.WriteLine("[" & rep.TurnNumber.ToString("000") & "] " & rep.Value)
        Next

        ReportList.Clear()
    End Sub
End Class
