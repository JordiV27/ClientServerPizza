using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Client 
{
    //Test Git comment
    // Client app is the one sending messages to a Server/listener.
    // Both listener and client can send messages back and forth once a
    // communication is established.
    public class SocketClient 
    {
        public static int Main(String[] args) 
        {
            StartClient();
            return 0;
        }

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

        public string getDonePizza() 
        {
            return null;
        }

        public string getCustomPizza()
        {
            return null;
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

        static void StartClient()
        {
            // IP address and port of the server to connect to
            string serverIp = "141.252.132.23"; // Replace this with the IP address of the server
            int port = 12345;

            try
            {
                // Create a TCP client
                TcpClient client = new TcpClient();

                // Connect to the server
                client.Connect(serverIp, port);
                Console.WriteLine("Connected to server.");

                // Get the network stream for sending data
                NetworkStream stream = client.GetStream();

                // Send a message to the server
                string message = "Hello from the client!";
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Message sent to server: " + message);

                // Close the stream and client when done
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}