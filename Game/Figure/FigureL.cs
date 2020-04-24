using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FigureL : Figure
{
    public const ConsoleColor FigureLColor = ConsoleColor.Red;

    public FigureL()
    {
        _consoleColor = FigureLColor;
        _typeOfCell = TypeOfCell.StaticL;
    }

    public override void Show()
    {
        Print(Constants.CenterForDisplayNextFigureColumn, Constants.CenterForDisplayNextFigureRow);
    }

    public override ConsoleColor InsertToGame(out bool canBeFilled)
    {
        canBeFilled = true;

        //print figure in the game field
        Print(Constants.CenterForStartPositionColumnInConsole, Constants.CenterForStartPositionRowInConsole);

        //checks if it is possible to enter a figure
        if (CheckStateOfGameField() == false)
        {
            canBeFilled = false;
            return ConsoleColor.Black;
        }

        //Enter the figure into game field
        EnterTheFigure();

        return ConsoleColor;
    }

    private void EnterTheFigure()
    {
        Game.GameField[Constants.CenterForStartPositionRow, Constants.CenterForStartPositionColumn] = TypeOfCell.CenterOfFigure;
        Game.GameField[Constants.CenterForStartPositionRow + 1, Constants.CenterForStartPositionColumn] = TypeOfCell.Descending;
        Game.GameField[Constants.CenterForStartPositionRow + 1, Constants.CenterForStartPositionColumn + 1] = TypeOfCell.Descending;
        Game.GameField[Constants.CenterForStartPositionRow - 1, Constants.CenterForStartPositionColumn] = TypeOfCell.Descending;
    }

    private bool CheckStateOfGameField()
    {
        if ((Game.GameField[Constants.CenterForStartPositionRow, Constants.CenterForStartPositionColumn] == TypeOfCell.Empty) &&
            (Game.GameField[Constants.CenterForStartPositionRow + 1, Constants.CenterForStartPositionColumn] == TypeOfCell.Empty) &&
            (Game.GameField[Constants.CenterForStartPositionRow + 1, Constants.CenterForStartPositionColumn + 1] == TypeOfCell.Empty) &&
            (Game.GameField[Constants.CenterForStartPositionRow - 1, Constants.CenterForStartPositionColumn] == TypeOfCell.Empty))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void Print(int leftShift, int topShift)
    {
        //get old cursor position
        int left = Console.CursorLeft;
        int top = Console.CursorTop;

        //get old foreground color
        var olfForegraundColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor;

        #region set cursor in new position and write future figure
        Console.SetCursorPosition(leftShift, topShift);
        Console.Write(Constants.Squere);

        Console.SetCursorPosition(leftShift, topShift - 1);
        Console.Write(Constants.Squere);

        Console.SetCursorPosition(leftShift, topShift + 1);
        Console.Write(Constants.Squere);

        Console.SetCursorPosition(leftShift + 1, topShift + 1);
        Console.Write(Constants.Squere);
        #endregion 

        //set old foreground color
        Console.ForegroundColor = olfForegraundColor;

        //set cursor in old position
        Console.SetCursorPosition(left, top);
    }
}
