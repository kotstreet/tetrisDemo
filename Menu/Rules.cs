using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Rules
{

    public static void Show()
    {
        Console.Clear();

        //show rules
        Console.WriteLine(Constants.RulesOfGame);

        //help for exit
        Console.ForegroundColor = Constants.ColorForHelp;
        MainMenu.HelpForExitToMainMenu();

        //reset old setting
        Console.ForegroundColor = Constants.MainColor;
        Console.SetCursorPosition(0, 0);

        //for exit
        MainMenu.ExitToMainMenu(0, 0);
    }
}
