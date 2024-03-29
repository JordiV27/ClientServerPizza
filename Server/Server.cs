using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;


namespace Server
{   
    public class Server
    {
        private static Server instance;

        private List<Order> orders;
        //private const string IPADDRESS = "192.168.68.110";  //ALERT: use HOST IPv4 address here
        private const string IPADDRESS = "192.168.68.117";  //ALERT: use HOST IPv4 address here

        private const int TCP_PORT = 12345, HTTP_PORT = 54321;

        public Server()
        {
            this.orders = new List<Order>();
        }

        public static Server getInstance()
        { 
            if (Server.instance == null)
            {
                Server.instance = new Server();
            }
            return Server.instance;
        }

        public void Start()
        {
            Thread httpServer = new Thread(() => StartHttpServer());
            httpServer.Start();

            Thread tcpServer = new Thread(() => StartTCPServer());
            tcpServer.Start();

            tcpServer.Join();
            httpServer.Join();
        }

        private void StartTCPServer()
        {
            IPAddress ipAddress = IPAddress.Parse(IPADDRESS);  
            using (TcpListener listener = new TcpListener(ipAddress, TCP_PORT))
            {
                listener.Start();
                Console.WriteLine($"TCP Server Listening... on http://{IPADDRESS}:{TCP_PORT}");

                try
                {
                    //Run server indefinately in while(true)
                    while (true)                    //TODO: listener.Stop() is NEVER called because of the loop
                    {
                        TcpClient client = listener.AcceptTcpClient();

                        Thread clientThread = new Thread(() => HandleTcpClient(client));
                        clientThread.Start();       //TODO: implement clientThread.Join() somewhere...
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("TCP error occurred: " + ex.Message);
                }

                listener.Stop();
            }

        }

        private void HandleTcpClient(TcpClient client)
        {
            try 
            {
                //Message from client
                string orderMsg;
                // Prepare data stream
                using (NetworkStream stream = client.GetStream())
                {
                    // Messages may be longer than 1024 bytes, problem solved using MemoryStream
                    // MemoryStream is a byte[] wrapper: allows dynamic growth
                    byte[] buffer = new byte[1024];
                    using (MemoryStream ms = new MemoryStream())
                    {

                        int numBytesRead;
                        while ((numBytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, numBytesRead);
                        }
                        //ALERT:  MAKE SURE THE ENCODING ON CLIENT APPLICATION IS ALSO ASCII
                        orderMsg = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                    }
                }
                
                Parser parser = new Parser();
                Order order = parser.parseMessage(orderMsg);
                this.orders.Add(order);
                DisplayOrders();

            } 
            catch (Exception ex) 
            {
                Console.WriteLine("TCP Handler threw an error: " + ex.Message);
            }

        }


        //in admin cmd execute following command                (change user params in add command to DEVICENAME\username)
        //netsh http add urlacl url=http://192.168.68.110:54321/ user=LENOVOPC\jordi listen=yes
        //netsh http add urlacl url=http://192.168.68.117:54321/ user=LAPTOP-V2BOURCE\Yvonne listen=yes
        //netsh http delete urlacl url = http://192.168.68.110:54321/
        private void StartHttpServer()
        {
            string url = "http://" + IPADDRESS + ":" + HTTP_PORT.ToString() + "/";

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Console.WriteLine($"HTTP Server Listening... on {url}");


            try 
            { 
                while (listener.IsListening) 
                {
                    HttpListenerContext context = listener.GetContext();
                    HandleHttpRequest(context);
                }
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine("HttpListener error: " + ex.Message); 
            } 
            finally 
            {
                listener.Stop();
            }
        }

        public void HandleHttpRequest(HttpListenerContext context)
        { 
            HttpListenerRequest request = context.Request;

            if (request.HttpMethod == "POST")
            {
                string orderMsg;
                using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                { 
                    orderMsg = reader.ReadToEnd();
                }

                Parser parser = new Parser();   
                Order order = parser.parseMessage(orderMsg);
                this.orders.Add(order);
                DisplayOrders();
            }

            //Finish Http Request-Response (no response sent back)
            context.Response.Close();
        }

        private void DisplayOrders()
        { 
            foreach (Order order in this.orders) 
            { 
                Console.WriteLine($"{order.ToString()}");
                Console.WriteLine();
            }
        }
    
    }
}
