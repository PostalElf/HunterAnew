Public Class Shield
#Region "Personal Identifiers"
    Private _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property
    Public Bodypart As Bodypart
    Public ReadOnly Property PossessiveName As String
        Get
            Return Bodypart.Combatant.Name & "'s " & Name
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Name
    End Function
#End Region

#Region "Constructors"
    Public Shared Function Construct(ByVal entry As String) As Shield
        Dim n As New AutoIncrementer
        Dim ln As String() = entry.Split("|")

        Dim shield As New Shield
        With shield
            ._Name = ln(n.N)
            .DamageCapacity = CInt(ln(n.N))
            .OverloadShock = CInt(ln(n.N))
            .OverloadDamage = CInt(ln(n.N))
        End With
        Return shield
    End Function
    Public Function Export() As String
        Dim total As String = Name
        total &= "|" & DamageCapacity
        total &= "|" & OverloadShock
        total &= "|" & OverloadDamage
        Return total
    End Function
#End Region

#Region "Events"
    Public Event WasHit(ByVal shield As Shield, ByVal attacker As Combatant, ByVal attack As Attack)
    Public Event WasOverloaded(ByVal shield As Shield, ByVal overloadShock As Integer, ByVal overloadDamage As Integer)
    Public Event WasTurnedOn(ByVal shield As Shield)
    Public Event WasTurnedOff(ByVal shield As Shield)

    Private Sub HandlerHit(ByVal Shield As Shield, ByVal attacker As Combatant, ByVal attack As Attack) Handles MyClass.WasHit
        Report.Add("Shield Hit", PossessiveName & " has been hit for " & attack.DamageFull & " " & attack.DamageType.ToString & ".", ConsoleColor.DarkGreen)
        Shield.DamageSustained += attack.DamageFull
    End Sub
    Private Sub HandlerOverloaded(ByVal shield As Shield, ByVal overloadShock As Integer, ByVal overloadDamage As Integer) Handles MyClass.WasOverloaded
        Report.Add("Shield Overloaded", PossessiveName & " has been overloaded!", ConsoleColor.DarkGreen)
        IsActive = False
    End Sub
#End Region

#Region "Properties"
    Private DamageCapacity As Integer
    Private OverloadShock As Integer
    Private OverloadDamage As Integer
#End Region

#Region "Battlefield"
    Private _IsActive As Boolean
    Public Property IsActive As Boolean
        Get
            Return _IsActive
        End Get
        Set(ByVal value As Boolean)
            'if shield property doesn't change then no need to raise events
            If value = _IsActive Then Exit Property

            'otherwise, set and raise events
            _IsActive = value
            If value = True Then
                RaiseEvent WasTurnedOn(Me)
            ElseIf value = False Then
                RaiseEvent WasTurnedOff(Me)
            End If
        End Set
    End Property
    Private _DamageSustained As Integer
    Private Property DamageSustained As Integer
        Get
            Return _DamageSustained
        End Get
        Set(ByVal value As Integer)
            Dim difference As Integer = Math.Abs(_DamageSustained - value)
            _DamageSustained = value
            If difference = 0 OrElse value = 0 Then Exit Property 'shortcircuit for no change or healing

            If _DamageSustained >= DamageCapacity Then RaiseEvent WasOverloaded(Me, OverloadShock, OverloadDamage)
        End Set
    End Property

    Public Sub IsAttacked(ByVal attack As Attack, ByVal attacker As Combatant)
        RaiseEvent WasHit(Me, attacker, attack)
    End Sub
#End Region
End Class
