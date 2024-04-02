using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Order
    {
        private CustomerInfo _customer_info;
        private List<Pizza> _pizzas;
        public Order(List<Pizza> pizzas, CustomerInfo customer_info) 
        { 
            _pizzas = pizzas;
            _customer_info = customer_info;
        }


        public override string ToString()
        {
            return "Order acknowledged";
        }
    }
}
