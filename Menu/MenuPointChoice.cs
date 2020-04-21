using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MenuPointChoice
{
    private List<MenuPointForChoice> _menuPointForChoice;

    public MenuPointChoice()
    {
        _menuPointForChoice = new List<MenuPointForChoice>()
        {
            new MenuPointForChoice(MenuPoint.Game, Game.Start),
            new MenuPointForChoice(MenuPoint.Rules, Rules.Show),
            new MenuPointForChoice(MenuPoint.Recordes, Records.Show),
            new MenuPointForChoice(MenuPoint.Exit, Exit.Leave)
        };
    }

    public void Invoke(MenuPoint menuPoint)
    {
        _menuPointForChoice.FirstOrDefault(mpfc => mpfc.MenuPoint.Equals(menuPoint)).MenuPointOperation.Invoke();
    }
}
