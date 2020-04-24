using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static WorkWithFont;

public class Program
{
    // ширина, глубина, скорость
    public static int FigureFallDelay { get; set; }
    public static int RowGameFieldCount { get; set; }
    public static int ColumnGameFieldCount { get; set; }

    static void Main(string[] args)
    {
        GeneralSetting();
     
        //get patameters
        FigureFallDelay = GetDelay();
        RowGameFieldCount = GetRow();
        ColumnGameFieldCount = GetColumn();

        //set cursor in invisiable 
        Console.CursorVisible = false;

        Game.Start();
    }

    /// <summary>
    /// Input rows
    /// </summary>
    /// <returns>rows count</returns>
    private static int GetRow()
    {
        bool isDigit = false;
        int rowCount = 0;

        //input rows
        while (isDigit == false || rowCount < Constants.MinRow || rowCount > Constants.MaxRow)
        {
            //request
            Console.Clear();
            Console.SetCursorPosition(Constants.LeftShiftForInputparameteresHeader, Constants.TopShiftForInputparameteres);
            Console.WriteLine(Constants.QueryForInputRow, Constants.MinRow, Constants.MaxRow);

            //entering
            Console.SetCursorPosition(Constants.LeftShiftForInputParameteres, Console.CursorTop);
            var row = Console.ReadLine();

            isDigit = Int32.TryParse(row, out rowCount);
        }

        return rowCount;
    }

    /// <summary>
    /// Input columns
    /// </summary>
    /// <returns>columns count</returns>
    private static int GetColumn()
    {
        bool isDigit = false;
        int columnCount = 0;

        //input column
        while (isDigit == false || columnCount  < Constants.MinColumn || columnCount > Constants.MaxCoumn)
        {
            //request
            Console.Clear();
            Console.SetCursorPosition(Constants.LeftShiftForInputparameteresHeader, Constants.TopShiftForInputparameteres);
            Console.WriteLine(Constants.QueryForInputColumn, Constants.MinColumn, Constants.MaxCoumn);

            //entering
            Console.SetCursorPosition(Constants.LeftShiftForInputParameteres, Console.CursorTop);
            var column = Console.ReadLine();

            isDigit = Int32.TryParse(column, out columnCount);
        }

        return columnCount;
    }

    /// <summary>
    /// Input delay
    /// </summary>
    /// <returns>delay</returns>
    private static int GetDelay()
    {
        bool isDigit = false;
        int delayImCellPerMinutes = 0;

        //input v
        while (isDigit == false || delayImCellPerMinutes < Constants.MinDelay || delayImCellPerMinutes > Constants.MaxDelay)
        {
            //request
            Console.Clear();
            Console.SetCursorPosition(Constants.LeftShiftForInputparameteresHeader, Constants.TopShiftForInputparameteres);
            Console.WriteLine(Constants.QueryForInputDelay, Constants.MinDelay, Constants.MaxDelay);

            //entering
            Console.SetCursorPosition(Constants.LeftShiftForInputParameteres, Console.CursorTop);
            var stringDelay = Console.ReadLine();

            isDigit = Int32.TryParse(stringDelay, out delayImCellPerMinutes);
        }

        //calculate delay
        var delay = 60000 / delayImCellPerMinutes;

        return delay;
    }

    /// <summary>
    /// General setting for all game
    /// </summary>
    private static void GeneralSetting()
    {
        //set title
        Console.Title = Constants.TitleOfApp;

        //set encoding
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.Unicode;

        //set background color
        Console.BackgroundColor = Constants.BackgroundColor;
        Console.ForegroundColor = Constants.MainColor;
        
        #region work with font
        //get info aboute font
        IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
        var fontInfo = new CONSOLE_FONT_INFO_EX();

        //setting info aboute font
        fontInfo.FaceName = Constants.MainConsoleFont;
        fontInfo.dwFontSize.X = 20;
        fontInfo.dwFontSize.Y = 20;

        //save change of font
        SetCurrentConsoleFontEx(handle, false, fontInfo);

        //set console size
        Console.SetWindowSize(31, 26);
        Console.SetBufferSize(31, 26);
        #endregion

        //set cursor size
        Console.CursorSize = Constants.CurrentCursorSize;
    }
}