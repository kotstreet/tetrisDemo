using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class Constants
{
    //colors
    public const ConsoleColor ColorForHelp = ConsoleColor.DarkGray;
    public const ConsoleColor BackgroundColor = ConsoleColor.White;
    public const ConsoleColor MainColor = ConsoleColor.Black;


    //for display help
    public const int StartPositionLeftForDisplayHelp = 0;
    public const int StartPositionTopForDisplayHelp = 23;

    //central of display figure in game field
    public const int CenterForStartPositionRowInConsole = 3;
    public const int CenterForStartPositionColumnInConsole = 6;

    //central of display next figure
    public const int CenterForDisplayNextFigureRow = 7;
    public const int CenterForDisplayNextFigureColumn = 28;

    //for display score
    public const int RowForDisplayScore = 3;
    public const int ColumnForDisplayScoreValue = 25;



    //central of figure in game field(in array)
    public const int CenterForStartPositionRow = 1;
    public const int CenterForStartPositionColumn = 2;

    //top left cornel of the game field in console
    public const int LeftShiftOfGameFieldStartPoint = 4;
    public const int TopShiftOfGameFieldStartPoint = 2;

    //for exit
    public const string ByBy = "\tДо новых встреч.";
    public const string HaveANiceDay = "\t  Хорошего дня)";
    public const int SleepBeforeExit = 2000;

    //for general settings
    public const string MainConsoleFont = "Lucida Console";
    public const string TitleOfApp = "Тетрис";

    //for generate figure
    public const int GeneratedValueForS = 1;
    public const int GeneratedValueForZ = 2;
    public const int GeneratedValueForT = 3;
    public const int GeneratedValueForL = 4;
    public const int GeneratedValueForJ = 5;
    public const int GeneratedValueForO = 6;

    public const char Squere = '\u2588';


    public const string HelpMessage = "Помощь: \u2190 и \u2192: перемещение,{0}\u2191: поворот по часовой стрелке,{0}\u2193: быстрый спуск";


    public const int CurrentCursorSize = 10;


    public const char SpaceCharacter = ' ';
    public const string Space = " ";


    //for get inputed parameters
    public const string QueryForInputRow = "Введите глубину поля, не меньше {0} и не больше {1}";
    public const string QueryForInputColumn = "Введите ширину поля, не меньше {0} и не больше {1}";
    public const string QueryForInputDelay = "Введите скорость падения кубиков, не меньше {0} и не больше {1}";

    public const int MinDelay = 30;
    public const int MaxDelay = 600;

    public const int MinRow = 8;
    public const int MaxRow = 21;

    public const int MinColumn = 4;
    public const int MaxCoumn = 20;

    public const int LeftShiftForInputParameteres = 15;
    public const int LeftShiftForInputparameteresHeader = 3;
    public const int TopShiftForInputparameteres = 10;
}