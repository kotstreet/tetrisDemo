using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public static class Game
{
    private static List<Figure> Figures;
    private static object locker;

    private static Figure _currentFigure = null;
    private static Figure _nextFigure = null;

    public static TypeOfCell[,] GameField;

    /// <summary>
    /// Start game
    /// </summary>
    public static void Start()
    {
        InitializeStaticComponents();

        Console.Clear();

        ShowStartGameField();

        //generate next figure before start game
        _nextFigure = FigureGenerator.Generate();
        Thread.Sleep(15);//because computer generate two the same figure without it

        //var cancellationTokenSource = MoveByKey();
        var cancellationTokenSource = AutoMoveDown();

        var isExistsDescendingFigure = true;

        while (true)
        {
            //refresh all game field because it can be incorrect print
            // ReFillAllGameField();


            var key = Console.ReadKey().Key;
            //Thread.Sleep(Constants.FigureFallDelay);
            lock (locker)
            {
                //isExistsDescendingFigure = MoveDown();
                isExistsDescendingFigure = Move(key);

                if (isExistsDescendingFigure == false)
                {
                    ResetgameFiledAfterFilledRows();
                    //for correct outfit of game field
                    //RewriteRightBorderGameField();
                    RewriteAllGameField();

                    //exit if imposible insert new figure
                    var isTheEnd = GenerateNewFigures();
                    if (isTheEnd)
                    {
                        //stop task(stop read key keys for move figure)
                        cancellationTokenSource.Cancel();

                        //exit from game
                        Exit.Leave();
                    }

                    //for refresh
                    isExistsDescendingFigure = true;
                }
            }

        }
    }

    /// <summary>
    /// rewrite game field and one more cell after every row
    /// </summary>
    private static void RewriteAllGameField()
    {
        for (int row = 0; row < Program.RowGameFieldCount; row++)
        {
            for (int column = 0; column < Program.ColumnGameFieldCount; column++)
            {
                Console.ForegroundColor = GetColorGameField(GameField[row, column]);
                Console.SetCursorPosition(Constants.LeftShiftOfGameFieldStartPoint + column, row + Constants.TopShiftOfGameFieldStartPoint);
                Console.Write($"{Constants.Squere}{Constants.Space}");
            }
        }

        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = Constants.MainColor;
    }

    /// <summary>
    /// auto move descending block down in the other thread
    /// </summary>
    /// <returns>token source for stop move down</returns>
    private static CancellationTokenSource AutoMoveDown()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var move = Task.Factory.StartNew(() =>
        {
            while (token.IsCancellationRequested == false)
            {
                //for refresh
                var isExistsDescendingFigure = true;

                //work with the current figure

                Thread.Sleep(Program.FigureFallDelay);
                lock (locker)
                {
                    isExistsDescendingFigure = MoveDown();

                    if (isExistsDescendingFigure == false)  
                    {
                        ResetgameFiledAfterFilledRows();

                        RewriteAllGameField();

                        var isTheEnd = GenerateNewFigures();
                        if (isTheEnd)
                        {
                            //stop task(stop read key keys for move figure)
                            cancellationTokenSource.Cancel();

                            //exit from game
                            Exit.Leave();
                        }

                        //for refresh
                        isExistsDescendingFigure = true;
                
                    }
                }
            }
        });

        return cancellationTokenSource;
    }

    /// <summary>
    /// Find points of descending blocks with the center point in a 0 element
    /// </summary>
    /// <returns>finded points</returns>
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
        for (int row = 0; row < Program.RowGameFieldCount; row++)
        {
            for (int column = 0; column < Program.ColumnGameFieldCount; column++)
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

    /// <summary>
    /// reset to empty descending blocks in console
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
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

    /// <summary>
    /// reset to empty descending blocks in array
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    internal static void ResetOldDataInGameFieldInArray(Point[] oldPoints)
    {
        //reset data
        foreach (var point in oldPoints)
        {
            GameField[point.TopShift, point.LeftShift] = TypeOfCell.Empty;
        }
    }

    #region for move left or right
    /// <summary>
    /// move the current figere left
    /// </summary>
    private static void MoveLeft()
    {
        MoveLeftOrRight(LeftOrRightMove.Left);
    }

    /// <summary>
    /// move the current figere right
    /// </summary>
    private static void MoveRight()
    {
        MoveLeftOrRight(LeftOrRightMove.Right);
    }

    /// <summary>
    /// move current figure left or right
    /// </summary>
    /// <param name="leftOrRightMove">the parameter which get information aboute move(left or right)</param>
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

    /// <summary>
    /// reset array for move(left or right)
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    /// <param name="leftOrRightMove">the parameter which get information aboute move(left or right)</param>
    private static void FillArrayForMoveLeftOrRight(Point[] oldPoints, LeftOrRightMove leftOrRightMove)
    {
        //reset data
        GameField[oldPoints[0].TopShift, oldPoints[0].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.CenterOfFigure;

        GameField[oldPoints[1].TopShift, oldPoints[1].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.Descending;
        GameField[oldPoints[2].TopShift, oldPoints[2].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.Descending;
        GameField[oldPoints[3].TopShift, oldPoints[3].LeftShift + 1 * (int)leftOrRightMove] = TypeOfCell.Descending;

    }

    /// <summary>
    /// reset console for move(left or right)
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    /// <param name="leftOrRightMove">the parameter which get information aboute move(left or right)</param>
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

    /// <summary>
    /// check if we can move figure (left or right)
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    /// <param name="leftOrRightMove">the parameter which get information aboute move(left or right)</param>
    /// <returns>true if we can</returns>
    private static bool CheckForMoveLeftOrRight(Point[] oldPoints, LeftOrRightMove leftOrRightMove)
    {
        //for move left
        if (oldPoints.Min(point => point.LeftShift) == 0  && leftOrRightMove == LeftOrRightMove.Left)
        {
            return false;
        }

        // for move right
        if (oldPoints.Max(point => point.LeftShift) + 1 == Program.ColumnGameFieldCount &&  leftOrRightMove == LeftOrRightMove.Right)
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

    #region for rotate
    /// <summary>
    /// Rotate current figere
    /// </summary>
    /// <param name="currentFigure">current figure</param>
    public static void Rotate(Figure currentFigure)
    {
        var oldPoints = Game.FindOldPoints();

        var canRotate = CheckForRotate(oldPoints);

        //save blocks as static one and exit the method
        if (canRotate == false)
        {
            return;
        }

        Game.ResetOldDataInGameFieldInArray(oldPoints);

        Game.ResetOldDataInGameFieldInConsole(oldPoints);

        FillArrayForRotate(oldPoints);

        FillConsoleForRotate(currentFigure, oldPoints);
    }

    /// <summary>
    /// fill array with new points for rotate
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    private static void FillArrayForRotate(Point[] oldPoints)
    {
        //reset data for center
        Game.GameField[oldPoints[0].TopShift, oldPoints[0].LeftShift] = TypeOfCell.CenterOfFigure;

        //reset data for other blocks
        for (int index = 1; index < 4; index++)
        {
            var point = FindNewShift(oldPoints[0], oldPoints[index]);

            //replace by rule: d_left => d_top; d_top => -d_left; rule from rotate by 90 deg
            Game.GameField[point.TopShift, point.LeftShift] = TypeOfCell.Descending;
        }
    }

    /// <summary>
    /// print rotated figure in the console
    /// </summary>
    /// <param name="currentFigure">current figure</param>
    /// <param name="oldPoints">old points of descending blocks</param>
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

    /// <summary>
    /// find new point for rotate
    /// </summary>
    /// <param name="centralPoint">central point of current figure</param>
    /// <param name="point">point for rotate</param>
    /// <returns>rotated point</returns>
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

    /// <summary>
    /// check if we can rotete figure
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    /// <returns>true if we can</returns>
    private static bool CheckForRotate(Point[] oldPoints)
    {
        for (int index = 1; index < 4; index++)
        {
            var point = FindNewShift(oldPoints[0], oldPoints[index]);

            //check for borders and for value of game field
            if (point.LeftShift < 0 || point.LeftShift >= Program.ColumnGameFieldCount || point.TopShift < 0 || point.TopShift >= Program.RowGameFieldCount ||
                Game.GameField[point.TopShift, point.LeftShift] <= TypeOfCell.Static)
            {
                return false;
            }
        }

        return true;
    }
    #endregion for rotate

    #region for  move down
    /// <summary>
    /// move the current figure down
    /// </summary>
    /// <returns>true if we moved figure down</returns>
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

    /// <summary>
    /// Set in array moved figure down
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    private static void FillArrayForMoveDown(Point[] oldPoints)
    {
        //reset data
        GameField[oldPoints[0].TopShift + 1, oldPoints[0].LeftShift] = TypeOfCell.CenterOfFigure;

        GameField[oldPoints[1].TopShift + 1, oldPoints[1].LeftShift] = TypeOfCell.Descending;
        GameField[oldPoints[2].TopShift + 1, oldPoints[2].LeftShift] = TypeOfCell.Descending;
        GameField[oldPoints[3].TopShift + 1, oldPoints[3].LeftShift] = TypeOfCell.Descending;
        
    }

    /// <summary>
    /// Set in console moved figure down
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
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

    /// <summary>
    /// set descendig blocks in array as statics blocks
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
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

    /// <summary>
    /// Check if we can move figure down
    /// </summary>
    /// <param name="oldPoints">old points of descending blocks</param>
    /// <returns>true if we can</returns>
    private static bool CheckForMoveDown(Point[] oldPoints)
    {
        if ((oldPoints.Max(point => point.TopShift) + 1 == Program.RowGameFieldCount) ||
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
    /// <summary>
    /// if the any row filled then we reprint game field without the row
    /// </summary>
    private static void  ResetgameFiledAfterFilledRows()
    {
        var filledRows = FindFilledRows();

        //find count of filled rows and exit if not any row was filled
        var countOfFilledRow = filledRows.Count(row => row != -1);
        if (countOfFilledRow == 0)
        {
            return;
        }

        RefreshGameFieldAfterFilledRows(filledRows, countOfFilledRow);

        ReWriteGameFieldAfterFilledRows();
    }

    /// <summary>
    /// reprint game field
    /// </summary>
    private static void ReWriteGameFieldAfterFilledRows()
    {
        //rewrite console game field
        for (int row = 0; row < Program.RowGameFieldCount; row++)
        {
            //set cursor position to the new row
            Console.SetCursorPosition(Constants.LeftShiftOfGameFieldStartPoint, Constants.TopShiftOfGameFieldStartPoint + row);

            //write squere with necessary color 
            for (int column = 0; column < Program.ColumnGameFieldCount; column++)
            {
                Console.ForegroundColor = GetColorGameField(Game.GameField[row, column]);
                Console.Write(Constants.Squere);
            }
        }

        //reset all settings
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = Constants.MainColor;
    }

    /// <summary>
    /// Reset array of game field after filled row
    /// </summary>
    /// <param name="filledRows">numbers of fiiled rows</param>
    /// <param name="countOfFilledRow">count of filled rows</param>
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
            for (int column = 0; column < Program.ColumnGameFieldCount; column++)
            {
                Game.GameField[row, column] = Game.GameField[row - shift, column];
            }
        }

        //add new rows to the top
        for (int row = 0; row < countOfFilledRow; row++)
        {
            for (int column = 0; column < Program.ColumnGameFieldCount; column++)
            {
                Game.GameField[row, column] = TypeOfCell.Empty;
            }
        }
    }

    /// <summary>
    /// find rows which was filled
    /// </summary>
    /// <returns>filled rows</returns>
    private static int[] FindFilledRows()
    {
        var filledRow = new int[4] { -1, -1, -1, -1 };
        var index = 0;

        for (int row = Program.RowGameFieldCount - 1; row > 0 ; row--)
        {
            var isFilled = true;
            var isEmpty = true;

            for (int column = 0; column < Program.ColumnGameFieldCount; column++)
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

    /// <summary>
    /// find color of block in array for print
    /// </summary>
    /// <param name="typeOfCell">type of game field cell</param>
    /// <returns>color of block</returns>
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
                Rotate(_currentFigure);
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
        var gameField = new string(Constants.SpaceCharacter, Constants.CenterForDisplayNextFigureRow - Constants.CenterForStartPositionRowInConsole) 
            + new string(Constants.Squere, Program.ColumnGameFieldCount);

        //display game field
        for (var i = 0; i < Program.RowGameFieldCount; i++)
        {
            Console.WriteLine(gameField);
        }

        //separator
        Console.WriteLine();

        //output help
        PrintHelp();
    }

    /// <summary>
    /// print help for game
    /// </summary>
    private static void PrintHelp()
    {
        //setting foreground
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayHelp, Constants.StartPositionTopForDisplayHelp);
        Console.ForegroundColor = Constants.ColorForHelp;

        //print help
        Console.Write(Constants.HelpMessage, Environment.NewLine);

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

        GameField = new TypeOfCell[Program.RowGameFieldCount, Program.ColumnGameFieldCount];
        GameField.Initialize();

        locker = new object();
    }
}
