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
    const string MainConsoleFont = "Lucida Console";


    const string TitleOfApp = "Тетрис";

    static void Main(string[] args)
    {
        GeneralSetting();

        MainMenu.Start();

        Console.Read();
    }


    private static void GeneralSetting()
    {
        //set title
        Console.Title = TitleOfApp;

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
        fontInfo.FaceName = MainConsoleFont;
        fontInfo.dwFontSize.X = 20;
        fontInfo.dwFontSize.Y = 20;

        //save change of font
        SetCurrentConsoleFontEx(handle, false, fontInfo);

        //set console size
        Console.SetWindowSize(31, 26);
        Console.SetBufferSize(31, 26);
        #endregion

        //set cursor in invisiable 
        Console.CursorVisible = false;

        //set cursor size
        Console.CursorSize = Constants.CurrentCursorSize;
    }
}