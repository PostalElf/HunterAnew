Public Class CombatantAI
    Inherits Combatant

#Region "Constructors"
    Public Shared Function Construct(ByVal enemyName As String) As CombatantAI
        Dim bodyparts As New List(Of Bodypart)
        Dim baseBodypart As New Bodypart

        Dim rawdata As Queue(Of String) = IO.BracketFileget("data/enemy.txt", enemyName)
        rawdata.Dequeue()             'remove name header
        While rawdata.Count > 0
            Dim ln As String() = rawdata.Dequeue.Split(":")
            Dim header As String = ln(0).Trim
            Dim entry As String = ln(1).Trim

            Select Case header
                Case "Bodypart"
                    Dim bpRaw As Queue(Of String) = IO.BracketFileget("data/enemyparts.txt", entry)
                    Dim bp As Bodypart = Bodypart.Construct(bpRaw)
                    bodyparts.Add(bp)
                Case Else
                    baseBodypart.Construct(header, entry)
            End Select
        End While

        Return Construct(enemyName, baseBodypart, bodyparts)
    End Function
    Public Shared Function Construct(ByVal enemyName As String, ByVal baseBodypart As Bodypart, ByVal bodyparts As List(Of Bodypart)) As CombatantAI
        Dim total As New CombatantAI
        With total
            ._Name = enemyName
            .BaseBodypart = baseBodypart
            For Each bp In bodyparts
                .Add(bp)
            Next
        End With
        Return total
    End Function
#End Region

#Region "Battlefield"
    Private CurrentAttack As Attack
    Private CurrentTarget As Combatant

    Public Overrides Function Tick(ByVal battlefield As battlefield) As Boolean
        Dim canAct As Boolean = MyBase.TickBase(battlefield)
        If canAct = True Then
            'set target and attack
            If CurrentAttack Is Nothing Then SetCurrentAttack()
            If CurrentTarget Is Nothing Then SetCurrentTarget(battlefield)

            'shortcircuit in the event that there's nothing left to do
            If CurrentTarget Is Nothing OrElse CurrentAttack Is Nothing Then Return False

            'check target distance
            Dim distance As Integer = CurrentTarget.BattlefieldPosition + BattlefieldPosition
            If distance >= CurrentAttack.MinRange AndAlso distance <= CurrentAttack.MaxRange Then
                'target in range; attack
                Dim targetBodypart As Bodypart = GetRandom(CurrentTarget.Bodyparts)
                targetBodypart.IsAttacked(CurrentAttack, Me)
            Else
                'target out of range; move
                Dim targetDistance As eBattlefieldPosition
                If distance < CurrentAttack.MinRange Then
                    'move further
                    targetDistance = BattlefieldPosition + 1
                ElseIf distance > CurrentAttack.MaxRange Then
                    'move nearer
                    targetDistance = BattlefieldPosition - 1
                End If
                If targetDistance < 0 Then targetDistance = 0
                If targetDistance > 2 Then targetDistance = 2
                If targetDistance = BattlefieldPosition Then Return False
                BattlefieldPosition = targetDistance
            End If

            'reset attack and target
            If CurrentAttack.Ready = False Then CurrentAttack = Nothing
            If Battlefield.Contains(CurrentTarget) = False Then CurrentTarget = Nothing

            'return true for performing an action
            Return True
        Else
            'empty tick, return false for not performing an action
            Return False
        End If
    End Function
    Private Sub SetCurrentAttack()
        CurrentAttack = GetRandom(AttacksReady)
    End Sub
    Private Sub SetCurrentTarget(ByVal battlefield As Battlefield)
        CurrentTarget = GetRandom(Battlefield.GetTargetsWithinRange(Me, CurrentAttack))
        If CurrentTarget Is Nothing Then CurrentTarget = GetRandom(Battlefield.GetTargets(Me))
    End Sub
#End Region
End Class
