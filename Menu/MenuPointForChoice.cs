using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public delegate void MenuPointOperation();
public class MenuPointForChoice
{
    public const string ErrorGetValueFromDictionary = "Возникла непредвиденная ошибка, не возможно найти ключ в словаре.";

    public MenuPoint MenuPoint { get; set; }
    public MenuPointOperation MenuPointOperation { get; set; }

    public MenuPointForChoice(MenuPoint menuPoint, MenuPointOperation menuPointOperation)
    {
        MenuPoint = menuPoint;
        MenuPointOperation = menuPointOperation;
    }
}
