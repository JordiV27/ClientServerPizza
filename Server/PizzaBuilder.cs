using Server.IngredientCategory;
using Server.Ingredients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PizzaBuilder
    {
        //Still not certain, whether strings or classes should be used
        public string _name;
        public List<IIngredient> _toppings;

        public PizzaBuilder()
        {
            //Cannot have implicit getters due to return type conflicts with custom setters
            _name = "Margherita";
            _toppings = new List<IIngredient>();
        }

        public PizzaBuilder set_name(string pizzaType)
        {
            _name = pizzaType;
            return this;
        }

        public PizzaBuilder set_topping(IIngredient topping)
        {
            this._toppings.Add(topping);
            return this;
        }

        public PizzaBuilder set_toppings(List<IIngredient> toppings)
        {
            _toppings = toppings;
            return this;
        }

        public Pizza Build()
        {
            return new Pizza(this);
        }
    }
}