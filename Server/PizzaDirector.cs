using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PizzaDirector
    {
        private readonly string _pizza_name;
        private readonly int _num_pizzas;
        private readonly List<string> _toppings;
        public PizzaDirector(string pizza_name, int num_pizzas, List<string> toppings)  
        { 
            _pizza_name = pizza_name;
            _num_pizzas = num_pizzas;
            _toppings = toppings;
        }

        

    }
}
