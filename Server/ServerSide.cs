using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    // Test git comment
    // Socket Listener acts as a server and listens to the incoming
    // messages on the specified port and protocol.
    public class SocketListener
    {
        public static int Main(String[] args)
        {
            StartServer();
            return 0;
        }

        public static void StartServer()
        {
            // Choose the port number for the server to listen on
            int port = 12345;

            try
            {
                // Create a TCP listener
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine("Server started. Waiting for connections...");

                // Accept incoming client connections
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                // Get the network stream for receiving data
                NetworkStream stream = client.GetStream();

                // Receive data from the client
                byte[] data = new byte[1024]; // Adjust the buffer size as needed
                int bytesRead = stream.Read(data, 0, data.Length);
                string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                Console.WriteLine("Message received from client: " + message);

                // Close the stream and listener when done
                stream.Close();
                listener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }
    }
}