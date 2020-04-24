using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Figure
{
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

    public abstract ConsoleColor InsertToGame(out bool canBeFilled);
}
