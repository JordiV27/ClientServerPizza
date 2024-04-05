using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Visitor;

namespace Server
{
    public class Order
    {
        private CustomerInfo _customer_info;
        private List<Pizza> _pizzas;
        private DateTime _dateTime;

        private IVisitor _lactoseFreeVisitor, _vegetarianVisitor, _childFriendlyVisitor;
        private bool _lactose_free, _vegetarian, _child_friendly;

        public Order(CustomerInfo customer_info, List<Pizza> pizzas, DateTime dateTime) 
        {
            _customer_info = customer_info;
            _pizzas = pizzas;
            _dateTime = dateTime;
            
            _lactoseFreeVisitor = new LactoseFreeVisitor();
            _vegetarianVisitor = new VegetarianVisitor();
            _childFriendlyVisitor = new ChildFriendlyVisitor();
            
            //Default allergene values
            _lactose_free = true;
            _vegetarian = true;
            _child_friendly = true;
        }

        public void checkAllergens()
        {
            foreach (Pizza pizza in _pizzas)
            {
                _lactose_free       = _lactose_free     && pizza.returnAllergen(_lactoseFreeVisitor);
                _vegetarian         = _vegetarian       && pizza.returnAllergen(_vegetarianVisitor);
                _child_friendly     = _child_friendly   && pizza.returnAllergen(_childFriendlyVisitor);
            }
        }


        public override string ToString()
        {
            string order = _customer_info.ToString();
            foreach (Pizza pizza in _pizzas)
            { 
                order += pizza.ToString(); 
            }
            order +=
            $"----------Allergen Information----------"     + Environment.NewLine +
            $"Lactose free:\t\t{_lactose_free}"             + Environment.NewLine +
            $"Vegetarian:\t\t{_vegetarian}"                 + Environment.NewLine +
            $"Child Friendly:\t\t{_child_friendly}"         + Environment.NewLine +
            "-----------------------------------------"     + Environment.NewLine;
            order += _dateTime.ToString();

            return order;
        }

        
    }
}
