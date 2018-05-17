Module Module1

    Sub Main()
        Dim attack As Attack = attack.Construct("Claws|0|1|3|1|90|45|0.5|0|15|10")
        Dim mech As CombatantPlayer = CombatantPlayer.Construct("Fenris")

        While True
            Dim bodypart As Bodypart = GetRandom(mech.Bodyparts)
            bodypart.IsAttacked(attack, mech)
            Report.ShowReports()
            Console.ReadLine()
        End While
    End Sub

End Module
