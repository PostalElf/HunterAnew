Public Class Battlefield
    Private NPCs As New List(Of Combatant)
    Private PCs As New List(Of Combatant)
    Public Function Contains(ByVal combatant As Combatant) As Boolean
        If NPCs.Contains(combatant) Then Return True
        If PCs.Contains(combatant) Then Return True
        Return False
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
End Class
