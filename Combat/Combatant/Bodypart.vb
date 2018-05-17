Public Class Bodypart
#Region "Personal Identifiers"
    Private _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property
    Public ReadOnly Property PossessiveName As String
        Get
            Return Combatant.Name & "'s " & Name
        End Get
    End Property
    Public Combatant As Combatant

    Public Overrides Function ToString() As String
        Return Name
    End Function
#End Region

#Region "Constructors"
    Public Shared Function Construct(ByVal rawdata As Queue(Of String)) As Bodypart
        Dim bp As New Bodypart
        bp._Name = rawdata.Dequeue()         'remove header
        While rawdata.Count > 0
            Dim ln As String() = rawdata.Dequeue.Split(":")
            Dim header As String = ln(0).Trim
            Dim entry As String = ln(1).Trim
            bp.Construct(header, entry)
        End While
        Return bp
    End Function
    Public Sub Construct(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Name" : _Name = entry
            Case "IsVital" : _IsVital = CBool(entry)
            Case "Weight" : _BonusWeight = CInt(entry)
            Case "Carry" : _BonusCarry = CInt(entry)
            Case "Speed" : _BonusSpeed = CInt(entry)
            Case "Dodge" : _BonusDodge = CInt(entry)
            Case "ShockCapacity" : _BonusShockCapacity = CInt(entry)

            Case "Agility" : Agility = CInt(entry)
            Case "Armour" : Armour = CInt(entry)
            Case "Health" : Health = CInt(entry)
            Case "ShockAbsorb" : _ShockAbsorb = CDbl(entry)
            Case "ShockLoss" : _ShockLoss = CInt(entry)
            Case "Attack" : Attack = Attack.Construct(entry)
        End Select
    End Sub
    Public Function Export() As Queue(Of String)
        Dim total As New Queue(Of String)
        With total
            .Enqueue(Name)
            .Enqueue("IsVital:" & _IsVital.ToString)
            .Enqueue("Weight:" & BonusWeight)
            .Enqueue("Carry:" & BonusCarry)
            .Enqueue("Speed:" & BonusSpeed)
            .Enqueue("Dodge:" & BonusDodge)
            .Enqueue("ShockCapacity:" & BonusShockCapacity)

            .Enqueue("Agility:" & Agility)
            .Enqueue("Armour:" & Armour)
            .Enqueue("Health:" & Health)
            .Enqueue("ShockAbsorb:" & ShockAbsorb)
            .Enqueue("ShockLoss:" & ShockLoss)

            If Attack Is Nothing = False Then .Enqueue("Attack:" & Attack.Export)
        End With
        Return total
    End Function
#End Region

#Region "Combatant Bonuses"
    Private _BonusWeight As Integer
    Public ReadOnly Property BonusWeight As Integer
        Get
            Return _BonusWeight
        End Get
    End Property
    Private _BonusCarry As Integer
    Public ReadOnly Property BonusCarry As Integer
        Get
            Return _BonusCarry
        End Get
    End Property
    Private _BonusSpeed As Integer
    Public ReadOnly Property BonusSpeed As Integer
        Get
            Return _BonusSpeed
        End Get
    End Property
    Private _BonusDodge As Integer
    Public ReadOnly Property BonusDodge As Integer
        Get
            Return _BonusDodge
        End Get
    End Property
    Private _BonusShockCapacity As Integer
    Public ReadOnly Property BonusShockCapacity As Integer
        Get
            Return _BonusShockCapacity
        End Get
    End Property
#End Region

#Region "BP Specific Properties"
    Private _IsVital As Boolean
    Public ReadOnly Property IsVital As Boolean
        Get
            Return _IsVital
        End Get
    End Property
    Private Agility As Integer
    Private Armour As Integer

    Private _DamageSustained As Integer
    Private Property DamageSustained As Integer
        Get
            Return _DamageSustained
        End Get
        Set(ByVal value As Integer)
            _DamageSustained = value
            If _DamageSustained > Health Then RaiseEvent WasDestroyed(Me)
        End Set
    End Property
    Private Health As Integer
    Private _ShockAbsorb As Double
    Public ReadOnly Property ShockAbsorb As Double
        Get
            Return _ShockAbsorb
        End Get
    End Property
    Private _ShockLoss As Integer
    Public ReadOnly Property ShockLoss As Integer
        Get
            Return _ShockLoss
        End Get
    End Property

    Private _Attack As Attack
    Public Property Attack As Attack
        Get
            Return _Attack
        End Get
        Set(ByVal value As Attack)
            _Attack = value
            _Attack.Bodypart = Me
        End Set
    End Property
    Private AttackCooldown As Integer
    Public ReadOnly Property AttackReady As Boolean
        Get
            If Attack Is Nothing Then Return False
            If AttackCooldown > 0 Then Return False

            Return True
        End Get
    End Property
#End Region

#Region "Events"
    Public Event WasMissed(ByVal bodypart As Bodypart, ByVal attacker As Combatant, ByVal attack As Attack)
    Public Event WasHit(ByVal bodypart As Bodypart, ByVal attacker As Combatant, ByVal attack As Attack, ByVal isFullHit As Boolean)
    Public Event WasDestroyed(ByVal bodypart As Bodypart)

    Private Sub HandlerWasMissed(ByVal bodypart As Bodypart, ByVal attacker As Combatant, ByVal attack As Attack) Handles MyClass.WasMissed
        Report.Add("Bodypart Missed", attack.PossessiveName & " missed " & bodypart.possessivename & "!", ConsoleColor.DarkGreen)
    End Sub
    Private Sub HandlerWasHit(ByVal bodypart As Bodypart, ByVal attacker As Combatant, ByVal attack As Attack, ByVal isFullHit As Boolean) Handles MyClass.WasHit
        Dim damage As Integer
        If isFullHit = True Then damage = attack.DamageFull Else damage = attack.DamageGlancing

        Report.Add("Bodypart Hit", attack.PossessiveName & " hit " & bodypart.PossessiveName & " for " & damage & " " & attack.DamageType.ToString & "!", ConsoleColor.DarkRed)
        DamageSustained += damage
    End Sub
    Private Sub HandlerWasDestroyed(ByVal bodypart As Bodypart) Handles MyClass.WasDestroyed
        Report.Add("Bodypart Destroyed", bodypart.PossessiveName & " has been destroyed!", ConsoleColor.Red)
    End Sub
#End Region

#Region "Battlefield"
    Public Sub IsAttacked(ByVal attack As Attack, ByVal attacker As Combatant)
        Dim roll As Integer = Rng.Next(1, 101)
        If roll <= attack.Accuracy - Agility Then
            'attack hits
            roll = Rng.Next(1, 101)
            If roll <= attack.Penetration - Armour Then
                'full hit
                RaiseEvent WasHit(Me, attacker, attack, True)
            Else
                'glancing hit
                RaiseEvent WasHit(Me, attacker, attack, False)
            End If
        Else
            'attack misses
            RaiseEvent WasMissed(Me, attacker, attack)
        End If
    End Sub
#End Region
End Class
