using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Order
    {
        public Order() { }

        public CustomerPersonalia getCustomerData()
        {
            Console.WriteLine("Please enter your first and last name: \n");
            string name = Console.ReadLine();

            Console.WriteLine("Please enter your streetname and house number: \n");
            string address = Console.ReadLine();

            Console.WriteLine("Please enter your city: \n");
            string city = Console.ReadLine();

            Console.WriteLine("Please enter your postal code: \n");
            string postal_code = Console.ReadLine();

            return new CustomerPersonalia(name, address, city, postal_code);
        }

        public string getPizzaOrder()
        {
            string order = "";
            Console.WriteLine("Welcome to Pizza! Here you can order *drum roll* PIZZA!");

            var pizza_option = new List<Tuple<int, string>>
            {
                Tuple.Create(1, "Custom Pizza"),
                Tuple.Create(2, "Pre-made Pizza")
            };

            Console.WriteLine("To order a pizza, please select one of the following options:");
            foreach (var p_opt in pizza_option)
            {
                Console.WriteLine("{0}) {1}", p_opt.Item1, p_opt.Item2);
            }
            /*Console.WriteLine("Would you like a pre-made pizza (0) or a customizable pizza (1)?");
            int opt1 = Convert.ToInt32(Console.ReadLine());
            if (opt1 == 0)
            {
                order = getDonePizza();
            }
            else if (opt1 == 1)
            {
                order = getCustomPizza();
            }*/

            return order;
        }

        public Message takeOrder()
        {
            CustomerPersonalia cp = getCustomerData();
            string pizza_order = getPizzaOrder();
            string date_time = DateTime.Now.ToString();
            return new Message(cp, pizza_order, date_time);
        }

        public class Message
        {
            private CustomerPersonalia _cp;
            private string _pizza_order;
            private string _date_time;

            public Message(CustomerPersonalia cp, string pizza_order, string date_time)
            {
                _cp = cp;
                _pizza_order = pizza_order;
                _date_time = date_time;
            }

            public override string ToString()
            {
                return _cp.ToString() + _pizza_order + _date_time;
            }

        }
    }
}
