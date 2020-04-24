using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class Exit
{
    /// <summary>
    /// Leave game
    /// </summary>
    public static void Leave()
    {
        Console.Clear();

        //separator
        Console.WriteLine(); Console.WriteLine();
        Console.WriteLine(); Console.WriteLine();
        Console.WriteLine(); Console.WriteLine();
        Console.WriteLine(); Console.WriteLine();
        Console.WriteLine(); Console.WriteLine();

        //saying by
        Console.WriteLine(Constants.ByBy);
        Console.WriteLine(Constants.HaveANiceDay);
        Thread.Sleep(Constants.SleepBeforeExit);

        //exit
        Environment.Exit(0);
    }
}
