using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Server.IngredientCategory;
using Server.Visitor;

namespace Server
{
    public class Pizza
    {
        private string _name;
        private List<IIngredient> _toppings;

        public Pizza(PizzaBuilder builder)
        {
            _name       = builder._name;
            _toppings   = builder._toppings;
        }

        
        public bool returnAllergen(IVisitor visitor) 
        {
            List<bool> free_of_allergen = new List<bool>(); //List of allergene flags, if one flag is false, then allergene of type visitor is present

            if (_toppings.Count() > 0)
            {
                foreach (IIngredient topping in _toppings)
                {
                    bool result = topping.Accept(visitor);
                    free_of_allergen.Add(result);
                }
                // If all free_of_allergen items is true, then allergen is not found -> return true
                return free_of_allergen.All(flag => flag == true);
            }

            //No toppings: free of allergens -> return true
            return true;
             
        }

        public override string ToString()
        {
            string topping_string = "";
            foreach (IIngredient topping in _toppings)
            {
                topping_string += Environment.NewLine + $" - {topping}";
            }
            string pizza_summary =
                $"-------------{_name} Pizza-------------"  + Environment.NewLine +
                 "Toppings: "                               + Environment.NewLine +
                 topping_string                             + Environment.NewLine;

            return pizza_summary;
        }
    }
}
