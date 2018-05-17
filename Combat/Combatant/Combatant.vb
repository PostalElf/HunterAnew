Public Class Combatant

#Region "Personal Identifiers"
    Protected _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property
#End Region

#Region "Events"
    Public Event WasShocked(ByVal combatant As Combatant, ByVal shockAmount As Integer, ByVal source As String)
    Public Event WasDestroyed(ByVal combatant As Combatant)

    Private Sub HandlerBodypartHit(ByVal bodypart As Bodypart, ByVal attacker As Combatant, ByVal attack As Attack, ByVal isFullHit As Boolean)
        Dim damage As Integer
        If isFullHit = True Then damage = attack.DamageFull Else damage = attack.DamageGlancing

        Dim shock As Integer = Math.Round(damage * (bodypart.ShockAbsorb + attack.ShockModifier))
        If shock <= 0 Then shock = 1
        AddShock(shock, "Hit")
    End Sub
    Private Sub HandlerBodypartDestroyed(ByVal bodypart As Bodypart)
        bodypart.Combatant = Nothing
        If Bodyparts.Contains(bodypart) Then Bodyparts.Remove(bodypart)

        If HasVitals = False Then
            RaiseEvent WasDestroyed(Me)
        Else
            AddShock(bodypart.ShockLoss, "Bodypart Destroyed")
        End If
    End Sub
    Private Sub HandlerWasShocked(ByVal combatant As Combatant, ByVal shockAmount As Integer, ByVal source As String) Handles MyClass.WasShocked
        Report.Add("Shocked", combatant.Name & " takes " & shockAmount & " shock [" & source & "].", ConsoleColor.DarkRed)
    End Sub
    Private Sub HandlerWasDestroyed(ByVal combatant As Combatant) Handles MyClass.WasDestroyed
        Report.Add("Combatant Destroyed", combatant.Name & " has been destroyed!!!", ConsoleColor.White)
    End Sub

    Private Sub HandlerShieldTurnedOn(ByVal shield As Shield)
        'turn off all other shields
        For Each bp In Bodyparts
            If bp.Shield Is Nothing = False Then
                If bp.Shield.Equals(shield) = False Then bp.Shield.IsActive = False
            End If
        Next

        'set active shield
        _ActiveShield = shield
    End Sub
    Private Sub HandlerShieldTurnedOff(ByVal shield As Shield)
        'remove active shield
        If _ActiveShield.Equals(shield) Then _ActiveShield = Nothing
    End Sub
    Private Sub HandlerShieldOverloaded(ByVal shield As Shield, ByVal overloadShock As Integer, ByVal overloadDamage As Integer)
        AddShock(overloadShock, "Shield Overload")
    End Sub
#End Region

#Region "Bodyparts"
    Protected BaseBodypart As Bodypart
    Public Bodyparts As New List(Of Bodypart)
    Public ReadOnly Property HasVitals As Boolean
        Get
            For Each bp In Bodyparts
                If bp.IsVital = True Then Return True
            Next
            Return False
        End Get
    End Property
    Protected Sub Add(ByVal bp As Bodypart)
        bp.Combatant = Me
        Bodyparts.Add(bp)

        With bp
            AddHandler .WasHit, AddressOf HandlerBodypartHit
            AddHandler .WasDestroyed, AddressOf HandlerBodypartDestroyed

            If .Shield Is Nothing = False Then
                AddHandler .Shield.WasTurnedOn, AddressOf HandlerShieldTurnedOn
                AddHandler .Shield.WasTurnedOff, AddressOf HandlerShieldTurnedOff
                AddHandler .Shield.WasOverloaded, AddressOf HandlerShieldOverloaded
            End If
        End With
    End Sub

    Public ReadOnly Property AttacksAll As List(Of Attack)
        Get
            Dim total As New List(Of Attack)
            If BaseBodypart.Attack Is Nothing = False Then total.Add(BaseBodypart.Attack)
            For Each bp In Bodyparts
                If bp.Attack Is Nothing = False Then total.Add(bp.Attack)
            Next
            Return total
        End Get
    End Property
    Public ReadOnly Property AttacksReady As List(Of Attack)
        Get
            Dim total As New List(Of Attack)
            For Each Attack In AttacksAll
                If Attack.Ready = True Then total.Add(Attack)
            Next
            Return total
        End Get
    End Property
    Public ReadOnly Property ShieldsAll As List(Of Shield)
        Get
            Dim total As New List(Of Shield)
            For Each bp In Bodyparts
                If bp.Shield Is Nothing = False Then total.Add(bp.Shield)
            Next
            Return total
        End Get
    End Property
    Private _ActiveShield As Shield
    Public ReadOnly Property ActiveShield As Shield
        Get
            Return _ActiveShield
        End Get
    End Property

    Public ReadOnly Property Weight As Integer
        Get
            Dim total As Integer = BaseBodypart.BonusWeight
            For Each bp In Bodyparts
                total += bp.BonusWeight
            Next
            Return total
        End Get
    End Property
    Public ReadOnly Property Carry As Integer
        Get
            Dim total As Integer = BaseBodypart.BonusCarry
            For Each bp In Bodyparts
                total += bp.BonusCarry
            Next
            Return total
        End Get
    End Property
    Private ReadOnly Property Encumbrance As Double
        Get
            If Weight = 0 Then Return 1
            If Carry = 0 Then Return 0

            Dim absEncumbrance As Integer = Math.Ceiling(Weight / Carry * 100)
            Select Case absEncumbrance
                Case Is < 50 : Return 1
                Case 51 To 70 : Return 0.75
                Case 71 To 85 : Return 0.5
                Case 86 To 100 : Return 0.25
                Case Else : Return 0.1
            End Select
        End Get
    End Property
    Public ReadOnly Property Speed As Integer
        Get
            Dim total As Integer = BaseBodypart.BonusSpeed
            For Each bp In Bodyparts
                total += bp.BonusSpeed
            Next

            'apply encumbrance
            total = Math.Ceiling(total * Encumbrance)
            If total <= 0 Then total = 1

            Return total
        End Get
    End Property
    Public ReadOnly Property Dodge As Integer
        Get
            Dim total As Integer = BaseBodypart.BonusDodge
            For Each bp In Bodyparts
                total += bp.BonusDodge
            Next
            Return total
        End Get
    End Property
    Public ReadOnly Property ShockCapacity
        Get
            Dim total As Integer = BaseBodypart.BonusShockCapacity
            For Each bp In Bodyparts
                total += bp.BonusShockCapacity
            Next
            Return total
        End Get
    End Property
#End Region

#Region "Battlefield"
    Private _ShockSustained As Integer
    Private Sub AddShock(ByVal shock As Integer, ByVal source As String)
        _ShockSustained += shock
        RaiseEvent WasShocked(Me, shock, source)

        If _ShockSustained > ShockCapacity Then RaiseEvent WasDestroyed(Me)
    End Sub
#End Region
End Class
