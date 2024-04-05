using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Server.IngredientCategory;

namespace Server
{
    public class Parser
    {
        private TcpClient? _tcpClient;

        private Menu _menu;
        private static Dictionary<string, IIngredient> _ingredients_list;

        public Parser(TcpClient client)
        {
            _tcpClient = client;

            _menu = new Menu();
            _ingredients_list = _menu.GetIngredients();
        }


        public Parser()
        {
            _menu = new Menu();
            _ingredients_list = _menu.GetIngredients();
        }

        public Order parseMessage(string message)
        {
            //Split input message on newline character
            string[] lines = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            //Save each line of input in a queue
            Queue<string> input_lines = new Queue<string>(lines);

            //Assuming first three lines ALWAYS contain customer information, TODO: implement sanity check w/ regex??? 
            string name = input_lines.Dequeue();
            string address = input_lines.Dequeue();
            string postal_code_city = input_lines.Dequeue();
            CustomerInfo customerInfo = new(name, address, postal_code_city);

            List<Pizza> pizzas = new List<Pizza>();

            DateTime dateTime;
            string next_line = input_lines.Peek(); //Peek does not remove element from queue
            while (!DateTime.TryParse(next_line, out dateTime))   //while next_line does not have DateTime format
            {
                PizzaBuilder pizzaBuilder = new PizzaBuilder();

                string pizza_name = input_lines.Dequeue();
                int num_pizzas, num_toppings;
                //NUMBER OF PIZZAS
                if (!int.TryParse(input_lines.Dequeue(), out num_pizzas)) { throw new FormatException("Invalid message format on number_pizzas"); }
                //NUMBER OF TOPPINGS
                if (!int.TryParse(input_lines.Dequeue(), out num_toppings)) { throw new FormatException("Invalid message format on number toppings"); }

                List<IIngredient> extra_toppings = new List<IIngredient>();
                for (int nt = 0; nt < num_toppings; nt++)
                {
                    string input_topping = input_lines.Dequeue();
                    bool ingredient_exists = false;
                    foreach (string menu_item in _ingredients_list.Keys)
                    {
                        if (Regex.IsMatch(input_topping.ToLower(), menu_item)) //TODO: move regex to menu?
                        {
                            ingredient_exists = true;
                            extra_toppings.Add(_ingredients_list[menu_item]);
                        }
                    }
                    if (!ingredient_exists)
                    {
                        DisconnectClient(); //TODO: close client-side program, this is not working yet
                        throw new Exception($"Parser Error! Ingredient {input_topping} does not exist! Aborting order...");
                    }
                }
                pizzaBuilder = pizzaBuilder.set_name(pizza_name).set_toppings(extra_toppings);
                for (int np = 0; np < num_pizzas; np++)
                {
                    pizzas.Add(pizzaBuilder.Build());
                }

                next_line = input_lines.Peek();
            }

            return new Order(customerInfo, pizzas, dateTime);
        }

        private void DisconnectClient()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
            }
        }
    }
}