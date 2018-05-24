Public Class Battlefield
#Region "Combatants"
    Private NPCs As New List(Of Combatant)
    Private PCs As New List(Of Combatant)
    Private Sub Add(ByVal c As Combatant)
        GetCombatantList(c).Add(c)
        c.BattlefieldSetup(Me)

        AddHandler c.WasDestroyed, AddressOf HandlerCombatantDestroyed
    End Sub
    Public Function Contains(ByVal combatant As Combatant) As Boolean
        If NPCs.Contains(combatant) Then Return True
        If PCs.Contains(combatant) Then Return True
        Return False
    End Function
    Private Function GetCombatantList(ByVal c As Combatant) As List(Of Combatant)
        If TypeOf c Is CombatantAI Then
            Return NPCs
        ElseIf TypeOf c Is CombatantPlayer Then
            Return PCs
        Else
            Throw New Exception("Unrecognised type of Combatant.")
        End If
    End Function

    Public Function GetTargets(ByVal attacker As Combatant) As List(Of Combatant)
        If TypeOf attacker Is CombatantAI Then
            Return PCs
        ElseIf TypeOf attacker Is CombatantPlayer Then
            Return NPCs
        Else
            Throw New Exception("Invalid type of attacker.")
        End If
    End Function
    Public Function GetTargetsWithinRange(ByVal attacker As Combatant, ByVal attack As Attack) As List(Of Combatant)
        Dim total As New List(Of Combatant)
        For Each target In GetTargets(attacker)
            Dim distance As Integer = target.BattlefieldPosition + attacker.BattlefieldPosition
            If distance >= attack.MinRange AndAlso distance <= attack.MaxRange Then total.Add(target)
        Next
        Return total
    End Function
    Public Function GetHighestSpeed() As Integer
        Dim highestSpeed As Integer = -1
        Dim highestSpeedCombatant As Combatant = Nothing

        For Each c In NPCs
            If c.Speed > highestSpeed Then
                highestSpeedCombatant = c
                highestSpeed = c.Speed
            End If
        Next
        For Each c In PCs
            If c.Speed > highestSpeed Then
                highestSpeedCombatant = c
                highestSpeed = c.Speed
            End If
        Next
        Return highestSpeed
    End Function
#End Region

#Region "Event Handlers"
    Private Sub HandlerCombatantDestroyed(ByVal c As Combatant)
        GetCombatantList(c).Remove(c)
    End Sub
#End Region

    Public Sub New(ByVal combatants As List(Of Combatant))
        For Each c In combatants
            Add(c)
        Next
    End Sub
    Public Function Main() As Boolean
        'setup battlefield
        Dim tickCounter As Integer = 0
        Report.TurnNumberReset(1)               'start turn number at 1

        While True
            'stat reporting
            tickCounter += 1

            'boolean controls if any actor has acted (and thus requires reporting, updating etc)
            Dim hasActed As Boolean = False

            'loop through PCs and NPCs
            For n = PCs.Count - 1 To 0 Step -1
                Dim c As CombatantPlayer = PCs(n)
                If c.Tick(Me) = True Then hasActed = True
            Next
            For n = NPCs.Count - 1 To 0 Step -1
                Dim c As CombatantAI = NPCs(n)
                If c.Tick(Me) = True Then hasActed = True
            Next

            'shortcircuit if nobody has acted
            If hasActed = False Then Continue While

            'advance turn counter and report
            Report.ShowReports()
            Report.TurnNumberAdvance()

            'check if either side has won
            If NPCs.Count = 0 OrElse PCs.Count = 0 Then Exit While
        End While


        'return true with PC victory, false if PC destroyed
        If NPCs.Count = 0 Then
            Return True
        ElseIf PCs.Count = 0 Then
            Return False
        Else
            Throw New Exception("Main battlefield loop exited without clear winner.")
        End If
    End Function
End Class
