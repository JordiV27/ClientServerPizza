using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PizzaBuilder
    {
        private Pizza pizza;

        public PizzaBuilder()
        {
            this.pizza = new Pizza();
        }

        public void UsePizzaBottom() 
        {
            PizzaBottom pizzaBottom = new PizzaBottom("Regular", false, true, true); // Placeholder values, gotta change this later
            pizza.ingredients.Add(pizzaBottom);
        }

        public void UsePizzaSauce() 
        {
            PizzaSauce pizzaSauce = new PizzaSauce("Regular", true, true, true); // Placeholder values, gotta change this later
            pizza.ingredients.Add(pizzaSauce);
        }

        public void AddStandardToppings() 
        {
            Dairy dairy = new Dairy("Regular", true, true, false); // Placeholder values, gotta change this later
            pizza.ingredients.Add(dairy);
        }

        public void AddExtraToppings(List<IIngredient> toppings) 
        {
            Meat meat = new Meat("Regular", true, false, true); // Placeholder values, gotta change this later
            pizza.ingredients.Add(meat);

            Vegetable vegetable = new Vegetable("Regular", true, true, true); // Placeholder values, gotta change this later
            pizza.ingredients.Add(vegetable);
        }
    }
}
