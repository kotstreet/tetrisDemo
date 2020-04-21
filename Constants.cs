using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Constants
{
    //colors
    public const ConsoleColor ColorForHelp = ConsoleColor.DarkGray;
    public const ConsoleColor ColorForTopThreeRecords = ConsoleColor.Red;
    public const ConsoleColor ColorForRecords = ConsoleColor.Green;
    public const ConsoleColor BackgroundColor = ConsoleColor.White;
    public const ConsoleColor MainColor = ConsoleColor.Black;


    //for display records header
    public const int StartPositionLeftForDisplayRecordsHeader = 12;
    public const int StartPositionTopForDisplayRecordsHeader = 2;

    //for display records header
    public const int StartPositionLeftForDisplayRecords = 5;
    public const int StartPositionTopForDisplayRecords = 4;

    //for display help
    public const int StartPositionLeftForDisplayHelp = 0;
    public const int StartPositionTopForDisplayHelp = 25;

    //for display main menu header
    public const int StartPositionLeftForDisplayMainMenuHeader = 13;
    public const int StartPositionTopForDisplayMainMenuHeader = 9;

    //for display main menu 
    public const int StartPositionLeftForDisplayMainMenu = 12;
    public const int StartPositionTopForDisplayMainMenu = 11;

    //central of display figure in game field
    public const int CenterForStartPositionRowInConsole = 3;
    public const int CenterForStartPositionColumnInConsole = 8;

    //central of display next figure
    public const int CenterForDisplayNextFigureRow = 7;
    public const int CenterForDisplayNextFigureColumn = 22;

    //for display score
    public const int RowForDisplayScore = 3;
    public const int ColumnForDisplayScoreCaption = 19;
    public const int ColumnForDisplayScoreValue = 25;


    //for display lines
    public const int RowForDisplayLines = 15;
    public const int ColumnForDisplayLinesCaption = 19;
    public const int ColumnForDisplayLinesValue = 26;

    //central of figure in game field(in array)
    public const int CenterForStartPositionRow = 1;
    public const int CenterForStartPositionColumn = 4;

    //top left cornel of the game field in console
    public const int LeftShiftOfGameFieldStartPoint = 4;
    public const int TopShiftOfGameFieldStartPoint = 2;

    //for enter player name
    public const int LeftShiftForRequestEnterName = 7;
    public const int TopShiftForRequestEnterName = 10;

    public const int LeftShiftForEnterName = 13;
    public const int TopShiftForEnterName = 12;


    public const char Squere = '\u2588';
    public const string HelpStringForMainMenu = "Помощь: \u2191 и \u2193, а также Enter";

    //for work with file
    public const string FileName = "Records.txt";
    public const char SeparatorForFile = ',';


    public const int FigureFallDelay = 500;


    public const int CurrentCursorSize = 10;


    public const int TopFifteen = 15;
    public const int TopThree = 3;

    public const char SpaceCharacter = ' ';
    public const string Space = " ";

    //for rules and records
    public const string RulesOfGame = "  Во время игры будут появляться 7 различных фигур, каждая из которых занимает 4 клетки. Фигуры будут " +
        "появляться по середине, ваша цель заполнять ряды. После заполнения они будут исчезать. При заполнении 4 рядов одновременно вы собираете " +
        "\"Тетрис\", который приносит дополнительные очки. Во время игры скорость падения фигур изменяться не будет. Правила управления фигурами " +
        "будут отображены на экране при начале игры.";
    public const string StringForreturnToMainMenu = "Помощь: для выхода нажмите Esc";
}