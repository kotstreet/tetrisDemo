using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class Exit
{
    public const string ByBy = "\tДо новых встреч.";
    public const string HaveANiceDay = "\t  Хорошего дня)";
    public const int SleepBeforeExit = 2000;

    public const string AskInputName = "Введите своё имя:";


    /// <summary>
    /// Exit the game after end of game with refresh records
    /// </summary>
    /// <param name="score">score for records</param>
    public static void ExitAfterGame(int score)
    {
        var currentPlayer = new Player(score);

        var isEntered = currentPlayer.CheckForNeedWrite(out List<Player> players);

        if(isEntered)
        {
            //get name of the player
            var name = ReadName();
            currentPlayer.Name = name;

            //select players for write
            players.Add(currentPlayer);
            var playersForWrite = players.OrderByDescending(player => player.Score).ThenBy(player => player.Name).Take(Constants.TopFifteen).ToList();

            //write players
            Player.Write(playersForWrite);
        }

        Leave();
    }

    /// <summary>
    /// Request and get name from player
    /// </summary>
    /// <returns>inputed  string</returns>
    private static string ReadName()
    {
        Console.Clear();

        //ask for enter name
        Console.SetCursorPosition(Constants.LeftShiftForRequestEnterName, Constants.TopShiftForRequestEnterName);
        Console.WriteLine(AskInputName);

        //input name
        Console.SetCursorPosition(Constants.LeftShiftForEnterName , Constants.TopShiftForEnterName);
        Console.CursorVisible = true;
        var name = Console.ReadLine();
        Console.CursorVisible = false;

        return name;
    }

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
        Console.WriteLine(ByBy);
        Console.WriteLine(HaveANiceDay);
        Thread.Sleep(SleepBeforeExit);

        //exit
        Environment.Exit(0);
    }
}
