using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class Game
{
    public const char SpaceCharacter = ' ';

    public const string ScoreIs = "Счёт: {0}";
    public const string LinesIs = "Линий: {0}";

    public const int RowGameFieldCount = 20;
    public const int ColumnGameFieldCount = 10;






    //private static Mutex _mutex;

    private static List<Figure> Figures;

    private static object locker;
    //private static Mutex _mutex;

    private static int _score = 0;
    private static int _lines = 0;
    private static Figure _currentFigure = null;
    private static Figure _nextFigure = null;

    internal static TypeOfCell[,] GameField;

    public static void Start()
    {
        InitializeStaticComponents();

        Console.Clear();

        ShowStartGameField();

        //generate next figure before start game
        _nextFigure = FigureGenerator.Generate();
        Thread.Sleep(15);//because computer generate two the same figure without it

        //AutoMoveDown();
        //AutoMoveDown();
        //var cancellationTokenSource = MoveByKey();

        
        while (true)
        {
            //refresh all game field because it can be incorrect print
            // ReFillAllGameField();

            //exit if imposible insert new figure
            var isTheEnd = GenerateNewFigures();
            if (isTheEnd)
            {
                //stop task(stop read key keys for move figure)
                //cancellationTokenSource.Cancel();

                //exit from game
                Exit.ExitAfterGame(_score);
            }

            //for refresh
            var isExistsDescendingFigure = true;

            //work with the current figure
            while (true)
            {
                //read key from console and move
                var key = Console.ReadKey().Key;//
                isExistsDescendingFigure = Move(key);//


                /*
                Thread.Sleep(Constants.FigureFallDelay);
                lock (locker)
                {
                    isExistsDescendingFigure = MoveDown();
                }*/

                /*_mutex.WaitOne();
                isExistsDescendingFigure = MoveDown();
                _mutex.ReleaseMutex();*/

                if (isExistsDescendingFigure == false)
                {
                    CheckAndAddScore();
                    break;
                }
            }
        }
        
    }

    private static void ReFillAllGameField()
    {
        for (int row = 0; row < RowGameFieldCount; row++)
        {
            //shift
            for (int column = 0; column < ColumnGameFieldCount; column++)
            {
                Console.SetCursorPosition( Constants.LeftShiftOfGameFieldStartPoint + column, Constants.TopShiftOfGameFieldStartPoint + row);

                Console.ForegroundColor = GetColorGameField(Game.GameField[row, column]);

                Console.Write(Constants.Squere);
            }

            Console.Write(Constants.Space);
        }
        
    }

    private static CancellationTokenSource MoveByKey()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var move = Task.Factory.StartNew(() =>
        {
            while (token.IsCancellationRequested == false)
            {
                var key = Console.ReadKey().Key;
                if(token.IsCancellationRequested == false)
                {
                    lock (locker)
                    {
                        Move(key);
                    }
                }
            }
        });

        return cancellationTokenSource;
    }

    private static CancellationTokenSource AutoMoveDown()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var move = Task.Factory.StartNew(() =>
        {
            while (token.IsCancellationRequested == false)
            {
                var key = Console.ReadKey().Key;
                if (token.IsCancellationRequested == false)
                {
                    lock (locker)
                    {
                        while (true)
                        {
                            //exit if imposible insert new figure
                            var isTheEnd = GenerateNewFigures();
                            if (isTheEnd)
                            {
                                //stop task(stop read key keys for move figure)
                                //cancellationTokenSource.Cancel();

                                //exit from game
                                Exit.ExitAfterGame(_score);
                            }

                            //for refresh
                            var isExistsDescendingFigure = true;

                            //work with the current figure
                            while (true)
                            {
                                Thread.Sleep(Constants.FigureFallDelay);
                                lock (locker)
                                {
                                    isExistsDescendingFigure = MoveDown();
                                }


                                if (isExistsDescendingFigure == false)
                                {
                                    CheckAndAddScore();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        });

        return cancellationTokenSource;
    }

    internal static Point[] FindOldPoints()
    {
        //create new array of point and initialize it with -1, because if we shall not find any points array should not be consider any points which 
        //consists of field game indexes
        var points = new Point[4]
        {
            new Point(-1, -1),
            new Point(-1, -1),
            new Point(-1, -1),
            new Point(-1, -1)
        };

        //vars for check if we can leave the loops
        //for one index of center of figure and three index of descending of figure
        var commonBlockIndexInArray = 1;
        var basicBlockIndexInArray = 0;

        //find the center and other blocks of current Figure
        for (int row = 0; row < RowGameFieldCount; row++)
        {
            for (int column = 0; column < ColumnGameFieldCount; column++)
            {
                //fill points array
                if (GameField[row, column] == TypeOfCell.Descending)
                {
                    var point = new Point(column, row);
                    points[commonBlockIndexInArray++] = point;
                }
                else if (GameField[row, column] == TypeOfCell.CenterOfFigure)
                {
                    var point = new Point(column, row);
                    points[basicBlockIndexInArray++] = point;
                }

                //for check if we can leave the loops
                if (commonBlockIndexInArray >= 4 && basicBlockIndexInArray == 2)
                {
                    return points;
                }
            }
        }

        return points;
    }

    internal static void ResetOldDataInGameFieldInConsole(Point[] oldPoints)
    {
        //reset data
        foreach (var point in oldPoints)
        {
            Console.SetCursorPosition(point.LeftShift + Constants.LeftShiftOfGameFieldStartPoint, 
                point.TopShift + Constants.TopShiftOfGameFieldStartPoint);
            Console.Write(Constants.Squere);
        }

        //reset old parameters
        Console.SetCursorPosition(0, 0);
    }

    internal static void ResetOldDataInGameFieldInArray(Point[] oldPoints)
    {
        //reset data
        foreach (var point in oldPoints)
        {
            GameField[point.TopShift, point.LeftShift] = TypeOfCell.Empty;
        }
    }



    #region for move left or right
    private static void MoveLeft()
    {
        MoveLeftOrRight(LeftOrRightMove.Left);
    }
    private static void MoveRight()
    {
        MoveLeftOrRight(LeftOrRightMove.Right);
    }

    private static void MoveLeftOrRight(LeftOrRightMove leftOrRightMove)
    {
        var oldPoints = FindOldPoints();

        var canMoveDown = CheckForMoveLeftOrRight(oldPoints, leftOrRightMove);

        //save blocks as static one and exit the method
        if (canMoveDown == false)
        {
            return;
        }

        ResetOldDataInGameFieldInArray(oldPoints);

        ResetOldDataInGameFieldInConsole(oldPoints);

        FillArrayForMoveLeftOrRight(oldPoints, leftOrRightMove);

        FillConsoleForMoveLeftOrRight(oldPoints, leftOrRightMove);
    }

    private static void FillArrayForMoveLeftOrRight(Point[] oldPoints, LeftOrRightMove leftOrRightMove)
    {
        //reset data
        GameField[oldPoints[0].TopShift, oldPoints[0].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.CenterOfFigure;

        GameField[oldPoints[1].TopShift, oldPoints[1].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.Descending;
        GameField[oldPoints[2].TopShift, oldPoints[2].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.Descending;
        GameField[oldPoints[3].TopShift, oldPoints[3].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.Descending;

    }

    private static void FillConsoleForMoveLeftOrRight(Point[] oldPoints, LeftOrRightMove leftOrRightMove)
    {
        //set new foreground color
        Console.ForegroundColor = _currentFigure.ConsoleColor;

        //reset data
        foreach (var point in oldPoints)
        {
            Console.SetCursorPosition(point.LeftShift + Constants.LeftShiftOfGameFieldStartPoint + 1 * (int)leftOrRightMove,
                point.TopShift + Constants.TopShiftOfGameFieldStartPoint);
            Console.Write(Constants.Squere);
        }

        //reset old parameters
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = Constants.MainColor;
    }

    private static bool CheckForMoveLeftOrRight(Point[] oldPoints, LeftOrRightMove leftOrRightMove)
    {
        //for move left
        if (oldPoints.Min(point => point.LeftShift) == 0  && leftOrRightMove == LeftOrRightMove.Left)
        {
            return false;
        }

        // for move right
        if (oldPoints.Max(point => point.LeftShift) + 1 == ColumnGameFieldCount &&  leftOrRightMove == LeftOrRightMove.Right)
        {
            return false;
        }

        //for move right and left
        if ((oldPoints.Max(point => point.TopShift) == -1) || 
            (GameField[oldPoints[0].TopShift, oldPoints[1].LeftShift + 1 * (int)leftOrRightMove] <= TypeOfCell.Static) ||
            (GameField[oldPoints[1].TopShift, oldPoints[1].LeftShift + 1 * (int)leftOrRightMove] <= TypeOfCell.Static) ||
            (GameField[oldPoints[2].TopShift, oldPoints[2].LeftShift + 1 * (int)leftOrRightMove] <= TypeOfCell.Static) ||
            (GameField[oldPoints[3].TopShift, oldPoints[3].LeftShift + 1 * (int)leftOrRightMove] <= TypeOfCell.Static))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion for move left or right

    #region for  move down
    private static bool MoveDown()
    {
        var oldPoints = FindOldPoints();

        var canMoveDown = CheckForMoveDown(oldPoints);

        //save blocks as static one and exit the method
        if(canMoveDown == false)
        {
            ResetOldDataInGameFieldToStatic(oldPoints);
            return canMoveDown;
        }

        ResetOldDataInGameFieldInArray(oldPoints);

        ResetOldDataInGameFieldInConsole(oldPoints);

        FillArrayForMoveDown(oldPoints);

        FillConsoleForMoveDown(oldPoints);

        return canMoveDown;
    }

    private static void FillArrayForMoveDown(Point[] oldPoints)
    {
        //reset data
        GameField[oldPoints[0].TopShift + 1, oldPoints[0].LeftShift] = TypeOfCell.CenterOfFigure;

        GameField[oldPoints[1].TopShift + 1, oldPoints[1].LeftShift] = TypeOfCell.Descending;
        GameField[oldPoints[2].TopShift + 1, oldPoints[2].LeftShift] = TypeOfCell.Descending;
        GameField[oldPoints[3].TopShift + 1, oldPoints[3].LeftShift] = TypeOfCell.Descending;
        
    }

    private static void FillConsoleForMoveDown(Point[] oldPoints)
    {
        //set new foreground color
        Console.ForegroundColor = _currentFigure.ConsoleColor;

        //reset data
        foreach (var point in oldPoints)
        {
            Console.SetCursorPosition(point.LeftShift + Constants.LeftShiftOfGameFieldStartPoint,
                point.TopShift + Constants.TopShiftOfGameFieldStartPoint + 1);
            Console.Write(Constants.Squere);
        }

        //reset old parameters
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = Constants.MainColor;
    }

    private static void ResetOldDataInGameFieldToStatic(Point[] oldPoints)
    {
        //return if we have not got old pointes
        if (oldPoints.Max(point => point.TopShift) == -1)
        {
            return;
        }

        //reset data
        foreach (var point in oldPoints)
        {
            GameField[point.TopShift, point.LeftShift] = _currentFigure.TypeOfCell;
        }
    }

    private static bool CheckForMoveDown(Point[] oldPoints)
    {
        if ((oldPoints.Max(point => point.TopShift) + 1 == RowGameFieldCount) ||
            (oldPoints.Max(point => point.TopShift)  == -1) ||
            (GameField[oldPoints[0].TopShift + 1, oldPoints[0].LeftShift] <= TypeOfCell.Static) ||
            (GameField[oldPoints[1].TopShift + 1, oldPoints[1].LeftShift] <= TypeOfCell.Static) ||
            (GameField[oldPoints[2].TopShift + 1, oldPoints[2].LeftShift] <= TypeOfCell.Static) ||
            (GameField[oldPoints[3].TopShift + 1, oldPoints[3].LeftShift] <= TypeOfCell.Static))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion for move down

    #region for check on filled row
    private static void  CheckAndAddScore()
    {
        var filledRows = FindFilledRows();

        //find count of filled rows and exit if not any row was filled
        var countOfFilledRow = filledRows.Count(row => row != -1);
        if (countOfFilledRow == 0)
        {
            return;
        }

        RefreshGameFieldAfterFilledRows(filledRows, countOfFilledRow);

        AddScore(countOfFilledRow);
        AddLines(countOfFilledRow);

        RewriteScore();
        RewriteLines();

        ReWriteGameFieldAfterFilledRows();
    }

    private static void ReWriteGameFieldAfterFilledRows()
    {
        //rewrite console game field
        for (int row = 0; row < RowGameFieldCount; row++)
        {
            //set cursor position to the new row
            Console.SetCursorPosition(Constants.LeftShiftOfGameFieldStartPoint, Constants.TopShiftOfGameFieldStartPoint + row);

            //write squere with necessary color 
            for (int column = 0; column < ColumnGameFieldCount; column++)
            {
                Console.ForegroundColor = GetColorGameField(Game.GameField[row, column]);
                Console.Write(Constants.Squere);
            }
        }

        //reset all settings
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = Constants.MainColor;
    }

    private static void RefreshGameFieldAfterFilledRows(int[] filledRows, int countOfFilledRow)
    {
        var shift = 1;
        var index = 0;
        
        //shift all rows from the first filled to top end of fields
        for (int row = filledRows[index]; row >= countOfFilledRow; row--)
        {
            //check for skip if the next row is filled
            while(index != 3 && filledRows[index + 1] == row - shift)
            { 
                //inc shift 
                if (filledRows[index + 1] == row - shift)
                {
                    index++;
                    shift++;
                }
            }

            //shift
            for (int column = 0; column < ColumnGameFieldCount; column++)
            {
                Game.GameField[row, column] = Game.GameField[row - shift, column];
            }
        }

        //add new rows to the top
        for (int row = 0; row < countOfFilledRow; row++)
        {
            for (int column = 0; column < ColumnGameFieldCount; column++)
            {
                Game.GameField[row, column] = TypeOfCell.Empty;
            }
        }
    }

    private static int[] FindFilledRows()
    {
        var filledRow = new int[4] { -1, -1, -1, -1 };
        var index = 0;

        for (int row = RowGameFieldCount - 1; row > 0 ; row--)
        {
            var isFilled = true;
            var isEmpty = true;

            for (int column = 0; column < ColumnGameFieldCount; column++)
            {
                //check for filled row
                if (Game.GameField[row, column] == TypeOfCell.Empty)
                {
                    isFilled = false;
                }

                //check for empty row
                if (Game.GameField[row, column] < TypeOfCell.Static)
                {
                    isEmpty = false;
                }
            }

            //reset data aboute filled rows
            if(isFilled)
            {
                filledRow[index++] = row;
            }

            //exit if we will not can meet filled row
            if(isEmpty || (filledRow[0] - 3 >= row && filledRow[0] >= 0) )
            {
                return filledRow;
            }
        }

        return filledRow;
    }

    private static void RewriteScore()
    {
        //save old cursor position
        var leftShift = Console.CursorLeft;
        var topShift = Console.CursorTop;

        //set new cursor posistion
        Console.SetCursorPosition(Constants.ColumnForDisplayScoreValue, Constants.RowForDisplayScore);

        //write new score
        Console.Write(_score);

        //rollback all settings
        Console.SetCursorPosition(leftShift, topShift);
    }

    private static void RewriteLines()
    {
        //save old cursor position
        var leftShift = Console.CursorLeft;
        var topShift = Console.CursorTop;

        //set new cursor posistion
        Console.SetCursorPosition(Constants.ColumnForDisplayLinesValue, Constants.RowForDisplayLines);

        //write new score
        Console.Write(_lines);

        //rollback all settings
        Console.SetCursorPosition(leftShift, topShift);
    }

    private static void AddLines(int countOfFilledRow)
    {
        _lines += countOfFilledRow;
    }

    private static void AddScore(int countOfFilledRow)
    {
        _score += countOfFilledRow == 4 ? 100 : 10 * countOfFilledRow;
    }

    private static ConsoleColor GetColorGameField(TypeOfCell typeOfCell)
    {
        //return console color
        return typeOfCell == TypeOfCell.Empty ? Constants.MainColor : Figures.Find(figure => figure.TypeOfCell == typeOfCell).ConsoleColor;
    }

    #endregion for check on filled row


    /// <summary>
    /// Move figure left, right, down or rotate the one
    /// </summary>
    /// <param name="key">inputed key keys of keybord</param>
    /// <returns>true if the next figeru can be inserted</returns>
    private static bool Move(ConsoleKey key)
    {
        switch(key)
        {
            case ConsoleKey.UpArrow:
                _currentFigure.Rotate(_currentFigure);
                goto default;

            case ConsoleKey.DownArrow:
                var result = MoveDown();

                if(result == false)
                {
                    return false;
                }

                goto default;

            case ConsoleKey.LeftArrow:
                MoveLeft();
                goto default;

            case ConsoleKey.RightArrow:
                MoveRight();
                goto default;

            default:
                //clear inputed character
                Console.SetCursorPosition(0, 0);
                Console.Write(Constants.Space);
                Console.SetCursorPosition(0, 0);
                return true;
        }
    }

    /// <summary>
    /// clean space for display next figure for correct display next time
    /// </summary>
    private static void CleanAfterDisplayForNextFigure()
    {
        //get old cursor position
        int left = Console.CursorLeft;
        int top = Console.CursorTop;

        //create string for clean
        var stringForClean = $"{Constants.SpaceCharacter}{Constants.SpaceCharacter}{Constants.SpaceCharacter}";

        //change cursor position and rewrite the place
        Console.SetCursorPosition(Constants.CenterForDisplayNextFigureColumn - 1, Constants.CenterForDisplayNextFigureRow - 1);
        Console.Write(stringForClean);

        Console.SetCursorPosition(Constants.CenterForDisplayNextFigureColumn - 1, Constants.CenterForDisplayNextFigureRow );
        Console.Write(stringForClean);

        Console.SetCursorPosition(Constants.CenterForDisplayNextFigureColumn - 1, Constants.CenterForDisplayNextFigureRow + 1);
        Console.Write(stringForClean);

        Console.SetCursorPosition(Constants.CenterForDisplayNextFigureColumn - 1, Constants.CenterForDisplayNextFigureRow + 2);
        Console.Write(stringForClean);

        //set cursor in old position
        Console.SetCursorPosition(left, top);
    }


    /// <summary>
    /// generate new figure insert the one in game and show next figure
    /// </summary>
    /// <returns>true if the figure can not be inserted</returns>
    private static bool GenerateNewFigures()
    {
        CleanAfterDisplayForNextFigure();

        //generate and insert to game current figure
        _currentFigure = _nextFigure;
        _currentFigure.InsertToGame(out bool canBeFilled);

        if (canBeFilled == false)
        {
            return true;
        }

        //generate and show next figure
        _nextFigure = FigureGenerator.Generate();
        _nextFigure.Show();

        return false;
    }

    /// <summary>
    /// Show start game screen with help, game field, score and lines
    /// </summary>
    private static void ShowStartGameField()
    {
        //separator
        Console.WriteLine();
        Console.WriteLine();

        //string for diplay game field 
        var gameField = new string(SpaceCharacter, 4) + new string(Constants.Squere, 10);

        //display game field
        for (var i = 0; i < RowGameFieldCount; i++)
        {
            Console.WriteLine(gameField);
        }

        //separator
        Console.WriteLine();

        //output help
        PrintHelp();

        //output score
        Console.SetCursorPosition(Constants.ColumnForDisplayScoreCaption, Constants.RowForDisplayScore);
        Console.WriteLine(ScoreIs, _score);

        //output lines
        Console.SetCursorPosition(Constants.ColumnForDisplayLinesCaption, Constants.RowForDisplayLines);
        Console.WriteLine(LinesIs, _lines);
    }

    /// <summary>
    /// print help for game
    /// </summary>
    private static void PrintHelp()
    {
        //setting foreground
        Console.ForegroundColor = Constants.ColorForHelp;

        //print help
        Console.Write($"Помощь: \u2190 и \u2192: перемещение,{Environment.NewLine}\u2191: поворот по часовой стрелке,{Environment.NewLine}\u2193: быстрый спуск");

        //rollback all settings
        Console.ForegroundColor = Constants.MainColor;
    }

    /// <summary>
    /// initialize figure list with all figure types 
    /// </summary>
    private static void InitializeFigureList()
    {
        //create new figures
        var s = new FigureS();
        var z = new FigureZ();
        var i = new FigureI();
        var o = new FigureO();
        var t = new FigureT();
        var l = new FigureL();
        var j = new FigureJ();

        //create general list
        Figures = new List<Figure>() { s, z, i, o, l, j, t };
    }

    /// <summary>
    /// initialize static components for game
    /// </summary>
    private static void InitializeStaticComponents()
    {
        InitializeFigureList();

        GameField = new TypeOfCell[RowGameFieldCount, ColumnGameFieldCount];
        GameField.Initialize();

        locker = new object();
        //_mutex = new Mutex();
    }
}
