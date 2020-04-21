using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WorkWithFont;

public static class MainMenu
{
    const string CaptionMenu = "Меню";
    const string GameMenuPoint = "Играть";
    const string RulesMenuPoint = "Правила";
    const string RecordMenuPoint = "Рекорды";
    const string ExitMenuPoint = "Выход";

    const char Pointer = '\u2192';  //pointer to the current point of menu

    const int LeftShiftForCursor = 10;
    const int TopShiftForCursor = 11;

    //for select next proccess after seleccted menu point
    private static MenuPoint _menuPoint = MenuPoint.Game;
    private static int  _shiftForCursor = 11;

    public static void Start()
    {
        DisplayStartScreen();

        SelectMenuPint();
    }

    private static void SelectMenuPint()
    {
        while(true)
        {
            //get input key
            var key = Console.ReadKey().Key;

            if(key == ConsoleKey.UpArrow)
            {
                if (_menuPoint > MenuPoint.Game)
                {
                    //clear current pointer
                    Console.SetCursorPosition(LeftShiftForCursor, _shiftForCursor);
                    Console.Write(Constants.Space);

                    //set cursor position
                    _shiftForCursor--;

                    //set Enum
                    _menuPoint--;

                    //draw new pointer
                    Console.SetCursorPosition(LeftShiftForCursor, _shiftForCursor);
                    Console.Write(Pointer);
                }

                Console.SetCursorPosition(0, 0);
            } 
            else if(key == ConsoleKey.DownArrow)
            {
                var cursorTop = Console.CursorTop;
                if (_menuPoint < MenuPoint.Exit)
                {
                    //clear current pointer
                    Console.SetCursorPosition(LeftShiftForCursor, _shiftForCursor);
                    Console.Write(Constants.Space);

                    //set cursor position
                    _shiftForCursor++;

                    //set Enum
                    _menuPoint++;

                    //draw new pointer
                    Console.SetCursorPosition(LeftShiftForCursor, _shiftForCursor);
                    Console.Write(Pointer);
                }

                Console.SetCursorPosition(0, 0);
            }
            else if(key == ConsoleKey.Enter)
            {
                //go to the current menu point 
                MenuPointChoice mpc = new MenuPointChoice();
                mpc.Invoke(_menuPoint);
                DisplayStartScreen();
            }

            //clear inputed character
            Console.SetCursorPosition(0,0);
            Console.Write(Constants.Space);
            Console.SetCursorPosition(0,0);
            
        }
    }

    private static void DisplayStartScreen()
    {
        Console.Clear();

        Show();

        SetPointer();

        //prepare before select menu point
        Console.SetCursorPosition(0, 0);
    }

    private static void Show()
    {
        //output header of menu
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayMainMenuHeader, Constants.StartPositionTopForDisplayMainMenuHeader);
        Console.WriteLine(CaptionMenu);


        //output points of menu
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayMainMenu, Constants.StartPositionTopForDisplayMainMenu + 0);
        Console.WriteLine(GameMenuPoint);
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayMainMenu, Constants.StartPositionTopForDisplayMainMenu + 1);
        Console.WriteLine(RulesMenuPoint);

        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayMainMenu, Constants.StartPositionTopForDisplayMainMenu + 2);
        Console.WriteLine(RecordMenuPoint);
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayMainMenu, Constants.StartPositionTopForDisplayMainMenu + 3);
        Console.WriteLine(ExitMenuPoint);

        //output help
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayHelp, Constants.StartPositionTopForDisplayHelp);
        PrintHelp();
    }

    private static void PrintHelp()
    {
        //setting foreground
        Console.ForegroundColor = Constants.ColorForHelp;
                
        //print help
        Console.Write(Constants.HelpStringForMainMenu);

        //rollback all settings
        Console.ForegroundColor = Constants.MainColor;
    }

    private static void SetPointer()
    {
        //get old cursor position
        var leftShift = Console.CursorLeft;
        var topShift = Console.CursorTop;
                        
        //set new position and write pointer
        Console.SetCursorPosition(LeftShiftForCursor, TopShiftForCursor + (int)_menuPoint);//for correct display after back from one of menu point
        Console.WriteLine(Pointer);
        
        //set old cursor position
        Console.SetCursorPosition(leftShift, topShift);
    }

    public static void ExitToMainMenu(int left, int top)
    {
        while (true)
        {
            var key = Console.ReadKey().Key;

            if (key == ConsoleKey.Escape)
            {
                return;
            }

            Console.SetCursorPosition(left, top);
            Console.Write(Constants.Space);
            Console.SetCursorPosition(left, top);
        }
    }

    public static void HelpForExitToMainMenu()
    {
        Console.SetCursorPosition(Constants.StartPositionLeftForDisplayHelp, Constants.StartPositionTopForDisplayHelp);
        Console.ForegroundColor = Constants.ColorForHelp;
        Console.Write(Constants.StringForreturnToMainMenu);
    }

}