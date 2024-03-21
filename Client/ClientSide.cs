using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Client 
{
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
            Console.WriteLine("Would you like a pre-made pizza (0) or a customizable pizza (1)?");
            int opt1 = Convert.ToInt32(Console.ReadLine());
            if (opt1 == 0)
            {
                order = getDonePizza();
            }
            else if (opt1 == 1)
            {
                order = getCustomPizza();
            }

            return order;
        }

        public Message takeOrder() 
        {
            CustomerPersonalia cp = getCustomerData();
            string pizza_order = getPizzaOrder();
            string date_time = DateTime.Now.ToString();
            return new Message(cp, pizza_order, date_time); 
        }

        public static void StartClient() 
        {
            byte[] bytes = new byte[1024];

            try 
            {
                // Connect to a Remote server
                // Get Host IP Address that is used to establish a connection
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                // If a host has multiple addresses, you will get a list of addresses
                IPHostEntry host = Dns.GetHostEntry("127.0.0.1");
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try 
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.

                    Console.WriteLine("Enter a message here: \n");
                    
                    string testMessage = Console.ReadLine();
                    testMessage = testMessage + "<EOF>";


                    byte[] msg = Encoding.ASCII.GetBytes(testMessage);

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane) 
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se) 
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e) 
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}