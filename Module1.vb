Module Module1

    Sub Main()
        Dim goblin As CombatantAI = CombatantAI.Construct("Goblin")
        Dim attack As Attack = goblin.AttacksAll(0)
        Dim mech As CombatantPlayer = CombatantPlayer.Construct("Fenris")
        mech.ShieldsAll(0).IsActive = True

        Dim battlefield As New Battlefield(New List(Of Combatant) From {goblin, mech})
        battlefield.Main()
        Console.ReadLine()
    End Sub

End Module
