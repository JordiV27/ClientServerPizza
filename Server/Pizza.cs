using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Pizza
    {
        private string? type;
        private string? bottom;
        private string? sauce;
        private string? cheese;
        private List<string>? toppings;
        public Pizza(PizzaBuilder builder)
        {
            this.type = builder.type;
            this.bottom = builder.bottom;
            this.sauce = builder.sauce;
            this.cheese = builder.cheese;
            this.toppings = builder.toppings;
        }
    }

    public class PizzaBuilder 
    {
        //Still not certain, whether strings or classes should be used
        public string? type;
        public string? bottom;
        public string? sauce;
        public string? cheese;
        public List<string>? toppings;

        public PizzaBuilder()
        {
            //Cannot have implicit getters due to return type conflicts with custom setters
            this.type = null;
            this.bottom = null;
            this.sauce = null;
            this.toppings = null;
        }

        public PizzaBuilder set_type(string pizzaType)
        {
            this.type= pizzaType;
            return this;
        }

        public PizzaBuilder set_bottom(string bottom)
        { 
            this.bottom= bottom;
            return this;
        }

        public PizzaBuilder set_sauce(string sauce)
        {
            this.sauce= sauce;
            return this;
        }

        public PizzaBuilder set_toppings(List<string> toppings)
        {
            this.toppings= toppings;
            return this;
        }

        public Pizza Build()
        { 
            return new Pizza(this);
        }
    }

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
            return _builder.set_type("Margherita").set_bottom("Dough").set_sauce("Tomato Sauce").Build();
        }

    }

}
