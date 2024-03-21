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
        public PizzaBuilder() { }

        public void reset() { }
        public void usePizzaBottom() { }
        public void usePizzaSauce() { }
        public void addStandardToppings() { }
        public void addExtraToppings(List<IIngredient> toppings) { }
    }
}
