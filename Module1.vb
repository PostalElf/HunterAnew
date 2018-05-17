Module Module1

    Sub Main()
        Dim goblin As CombatantAI = CombatantAI.Construct("Goblin")
        Dim attack As Attack = goblin.AttacksAll(0)
        Dim mech As CombatantPlayer = CombatantPlayer.Construct("Fenris")

        Report.TurnNumberAdvance()

        While True
            Dim bodypart As Bodypart = GetRandom(mech.Bodyparts)
            bodypart.IsAttacked(attack, mech)
            Report.ShowReports()
            Report.TurnNumberAdvance()
            Console.ReadLine()
        End While
    End Sub

End Module
