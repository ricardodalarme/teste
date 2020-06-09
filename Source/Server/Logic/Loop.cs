﻿using System;
using System.Windows.Forms;

class Loop
{
    // Usado para manter a aplicação aberta
    public static bool Working = true;

    // Contagens
    public static int Timer_500 = 0, Timer_1000 = 0, Timer_5000 = 0;
    public static int Timer_Regen = 0;
    public static int Timer_Map_Items = 0;

    public static void Init()
    {
        int CPS = 0;

        while (Working)
        {
            // Manuseia os dados recebidos
            Socket.HandleData();

            if (Environment.TickCount > Timer_500 + 500)
            {
                // Lógicas do mapa
                foreach (Objects.TMap Temp_Map in Lists.Temp_Map.Values) Temp_Map.Logic();

                // Lógica dos jogadores
                for (byte i = 0; i < Lists.Account.Count; i++)
                    if (Lists.Account[i].IsPlaying)
                        Lists.Account[i].Character.Logic();

                // Reinicia a contagem dos 500
                Timer_500 = Environment.TickCount;
            }

            // Reinicia algumas contagens
            if (Environment.TickCount > Timer_Regen + 5000) Timer_Regen = Environment.TickCount;
            if (Environment.TickCount > Timer_Map_Items + 300000) Timer_Map_Items = Environment.TickCount;

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();

            // Calcula o CPS
            if (Timer_1000 < Environment.TickCount)
            {
                Game.CPS = CPS;
                CPS = 0;
                Timer_1000 = Environment.TickCount + 1000;
            }
            else
                CPS += 1;
        }
    }

    public static void Commands()
    {
        // Laço para que seja possível a utilização de comandos pelo console
        while (Working)
        {
            Console.Write("Execute: ");
            Program.ExecuteCommand(Console.ReadLine());
        }
    }
}