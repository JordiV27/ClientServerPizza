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