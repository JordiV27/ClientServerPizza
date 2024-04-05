using Server.Ingredients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PizzaDirector
    {
        private PizzaBuilder _builder;
        public PizzaDirector(PizzaBuilder builder)
        {
            _builder = builder;
        }

        //Could make more for each pre-made pizza
        public Pizza Margherita()
        {
            return _builder.set_name("Margherita").set_topping(new Mozzarella()).Build();
        }

        // TODO: add toppings
        public Pizza Tonno()
        {
            return _builder.set_name("Tonno").set_topping(new Salami()).Build();
        }

    }
}