using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Menu
    {
        private PizzaBuilder _pb;
        private PizzaDirector _pd;

        public static Dictionary<string, Pizza> menu = new Dictionary<string, Pizza>();


        public Menu() 
        { 
            _pb = new PizzaBuilder();
            _pd = new PizzaDirector(_pb);

            FillMenu();
        }

        private void FillMenu()
        {
            menu.Add("Margherita", _pd.Margherita());
            
        }

        public static Dictionary<string , Pizza> GetMenu() 
        {
            return menu;
        }
    }
}
