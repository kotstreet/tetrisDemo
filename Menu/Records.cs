using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Records
{
    public const string RecordsHeader = "Рекорды";
    public const string RecordLine = "{0}. {1} - {2}";

    public const string RecordLineFormat = "{0,2:0}.   {0,5:1} - {2}";

    public const string RecordNumberFormat = "{0,2:0}";
    public const string RecordScoreFormat = "{0,5:0}";

    public static void Show()
    {
        Console.Clear();

        //output records
        ShowRecords();

        //help for exit
        Console.ForegroundColor = Constants.ColorForHelp;
        MainMenu.HelpForExitToMainMenu();

        //reset old setting
        Console.ForegroundColor = Constants.MainColor;
        Console.SetCursorPosition(0, 0);

        MainMenu.ExitToMainMenu(0, 0);
    }

    private static void ShowRecords()
    {
        //read top 15 players from file
        var players = Player.Read();

        //write header of records
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayRecordsHeader, Constants.StartPositionTopForDisplayRecordsHeader);
        Console.Write(RecordsHeader);

        //change color
        Console.ForegroundColor = Constants.ColorForTopThreeRecords;

        //write record lines 
        var topShift = 0;
        foreach(var player in players)
        {
            //change color if it's 4'th records
            if (topShift == Constants.TopThree)
            {
                Console.ForegroundColor = Constants.ColorForRecords;
            }

            Console.SetCursorPosition(Constants.StartPositionLeftForDisplayRecords, Constants.StartPositionTopForDisplayRecords + topShift++);
            Console.Write(RecordLine, String.Format(RecordNumberFormat, topShift), String.Format(RecordScoreFormat, player.Score), player.Name);
        }
    }


}
