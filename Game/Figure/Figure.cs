using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Figure
{
    public const int RowGameFieldCount = 20;
    public const int ColumnGameFieldCount = 10;


    protected ConsoleColor _consoleColor;

    public ConsoleColor ConsoleColor 
    { 
        get
        {
            return _consoleColor;
        }
    }

    protected TypeOfCell _typeOfCell;

    public TypeOfCell TypeOfCell
    {
        get
        {
            return _typeOfCell;
        }
    }

    public abstract void Show();

    public void Rotate(Figure currentFigure)
    {
        var oldPoints = Game.FindOldPoints();

        var canRotate = CheckForRotate(oldPoints);

        //save blocks as static one and exit the method
        if (canRotate == false)
        {
            return ;
        }

        Game.ResetOldDataInGameFieldInArray(oldPoints);

        Game.ResetOldDataInGameFieldInConsole(oldPoints);

        FillArrayForRotate(oldPoints);

        FillConsoleForRotate(currentFigure, oldPoints);
    }

    private static void FillArrayForRotate(Point[] oldPoints)
    {
        //reset data for center
        Game.GameField[oldPoints[0].TopShift, oldPoints[0].LeftShift] = TypeOfCell.CenterOfFigure;

        //reset data for other blocks
        for(int index = 1; index < 4; index++)
        {
            var point = FindNewShift(oldPoints[0], oldPoints[index]);

            //replace by rule: d_left => d_top; d_top => -d_left; rule from rotate by 90 deg
            Game.GameField[point.TopShift, point.LeftShift] = TypeOfCell.Descending;
        }
    }

    private static void FillConsoleForRotate(Figure currentFigure, Point[] oldPoints)
    {
        //set new foreground color
        Console.ForegroundColor = currentFigure.ConsoleColor;

        //reset data
        for (int index = 0; index < 4; index++)
        {
            var point = FindNewShift(oldPoints[0], oldPoints[index]);

            Console.SetCursorPosition(point.LeftShift + Constants.LeftShiftOfGameFieldStartPoint, 
                point.TopShift + Constants.TopShiftOfGameFieldStartPoint);
            Console.Write(Constants.Squere);
        }

        //reset old parameters
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = Constants.MainColor;
    }

    private static Point FindNewShift(Point centralPoint, Point point)
    {
        //find delta
        var deltaLeftShift = centralPoint.LeftShift - point.LeftShift;
        var deltaTopShift = centralPoint.TopShift - point.TopShift;

        //get new left and top shifts 
        var leftShift = centralPoint.LeftShift + deltaTopShift;
        var topShift = centralPoint.TopShift - deltaLeftShift;

        return new Point(leftShift, topShift);
    }

    private static bool CheckForRotate(Point[] oldPoints)
    {
        for (int index = 1; index < 4; index++)
        {
            var point = FindNewShift(oldPoints[0], oldPoints[index]);

            //check for borders and for value of game field
            if (point.LeftShift < 0 || point.LeftShift >= ColumnGameFieldCount || point.TopShift < 0 || point.TopShift >= RowGameFieldCount ||
                Game.GameField[point.TopShift, point.LeftShift] <= TypeOfCell.Static)
            {
                return false;
            }
        }

        return true;
    }

    public abstract ConsoleColor InsertToGame(out bool canBeFilled);
}
