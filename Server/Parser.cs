using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Parser
    {
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

            string next_line = input_lines.Peek();
            while (!DateTime.TryParse(next_line, out dt))
            {
                List<string> pizzas = new List<string>();
                List<string> toppings = new List<string>();

                string pizza = input_lines.Dequeue();
                int num_pizzas, num_toppings;
                if (!int.TryParse(input_lines.Dequeue(), out num_pizzas)) //Next line after pizza name is not number of pizzas
                {
                    throw new FormatException("Invalid message format on number_pizzas");
                    break;
                }
                for (int np = 0; np < num_pizzas; np++)
                {
                    pizzas.Add(pizza);
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
            }
            
            CustomerInfo customer = new(name,address,postal_code_city);
            

            return new Order(); 
        }
    }
}
