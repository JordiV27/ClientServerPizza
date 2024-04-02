using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    public class Parser
    {

        private static Dictionary<string, Pizza> menu = Menu.GetMenu();
        public Parser() { }

        public Order parseMessage(string message) 
        {
            DateTime dt;
            string[] lines = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Queue<string> input_lines = new Queue<string>(lines);

            /*foreach (string line in lines) 
            {
                Console.WriteLine("["+count.ToString()+"]: " + line);
                count++;
            }*/
            
            //Assuming first three lines ALWAYS contain customer information, TODO: implement failsafe w/ regex???
            string name = input_lines.Dequeue();
            string address = input_lines.Dequeue();
            string postal_code_city = input_lines.Dequeue();

            List<Pizza> pizzas = new List<Pizza>();
            

            string next_line = input_lines.Peek();
            while (!DateTime.TryParse(next_line, out dt))
            {
                
                List<string> toppings = new List<string>();
                string pizza_type = input_lines.Dequeue();
                

                int num_pizzas, num_toppings;
                if (!int.TryParse(input_lines.Dequeue(), out num_pizzas)) //Next line after pizza name is not number of pizzas
                {
                    throw new FormatException("Invalid message format on number_pizzas");
                    break;
                }
                
                if (!int.TryParse(input_lines.Dequeue(), out num_toppings)) //Next line is not number of toppings
                {
                    throw new FormatException("Invalid message format on number_pizzas");
                    break;
                }
                for (int nt = 0; nt < num_toppings; nt++)
                { 
                    string topping = input_lines.Dequeue();
                    toppings.Add(topping);
                }
                next_line = input_lines.Peek();
                
                PizzaBuilder pb = new PizzaBuilder();
                Pizza pizza;
                bool pre_made_pizza = false;
                foreach (string menu_item in menu.Keys)
                {
                    if (Regex.IsMatch(pizza_type, menu_item))
                    {
                        pre_made_pizza = true;
                        pizza = menu[menu_item];
                        pizzas.Add(pizza);
                    }
                }
                if (!pre_made_pizza)
                { 
                    pizza = pb.set_type(pizza_type).Build();
                    pizzas.Add(pizza);
                }

            }
            
            CustomerInfo customerInfo = new(name,address,postal_code_city);
            

            return new Order(pizzas, customerInfo); 
        }
    }
}
