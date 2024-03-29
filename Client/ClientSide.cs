using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace Client 
{
    public class ClientSide 
    {
        public static int Main(String[] args) 
        {
            StartTcpClient();
            return 0;
        }

        static void StartTcpClient()
        {
            // IP address and port of the server to connect to
            string serverIp = "192.168.68.110"; // Replace this with the IPv4 address of the server
            //string serverIp = "192.168.68.117"; // Replace this with the IPv4 address of the server
            int port = 12345;

            try
            {
                using (TcpClient client = new TcpClient()) {
                    client.Connect(serverIp, port);
                    Console.WriteLine("Connected to server.");

                    // Get the network stream for sending data
                    using (NetworkStream stream = client.GetStream())
                    {
                        // Send a message to the server
                        // Note: a pizza order is formatted this way, becasue the parser somehow recognizes Environment.Newline as the 'true' newline character as opposed to "\n"
                        string pizzaOrder1 = 
                            "Jansen" +                  Environment.NewLine +      
                            "Nieuwestad 14" +           Environment.NewLine +
                            "8901 PM Leeuwarden" +      Environment.NewLine +
                            "Margherita" +              Environment.NewLine +
                            "1" +                       Environment.NewLine +
                            "0" +                       Environment.NewLine +
                            "05/12/2022 18:30";
                        byte[] data = Encoding.ASCII.GetBytes(pizzaOrder1);
                        stream.Write(data, 0, data.Length);
                        Console.WriteLine("Order sent to server");
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}