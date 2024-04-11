using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;


namespace Server
{   
    public class Server
    {
        private static Server instance;

        private List<Order> orders;

        private const string IPADDRESS = "192.168.68.110";  //ALERT: use HOST IPv4 address here
        //private const string IPADDRESS = "192.168.68.117";  //ALERT: use HOST IPv4 address here


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
                byte[] aesByteKey = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD,
                              0xEE, 0xFF, 0x00, 0x11,
                              0x22, 0x33, 0x44, 0x55,
                              0x66, 0x77, 0x88, 0x99,
                              0x11, 0x22, 0x33, 0x44,
                              0x55, 0x66, 0x77, 0x88,
                              0x99, 0xAA, 0xBB, 0xCC,
                              0xDD, 0xEE, 0xFF, 0x00 }; ;
                byte[] aesByteIV = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD,
                              0xEE, 0xFF, 0x00, 0x11,
                              0x22, 0x33, 0x44, 0x55,
                              0x66, 0x77, 0x88, 0x99 };


                Aes aes = Aes.Create();
                aes.Key = aesByteKey;
                aes.IV = aesByteIV;
                //Message from client
                //string orderMsg;
                // Prepare data stream
                byte[] encoded_data;
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
                        encoded_data = ms.ToArray();
                        //orderMsg = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                    }
                }
                string orderMsg = AES_Decrypt(encoded_data, aes.Key, aes.IV);

                Parser parser = new Parser();
                Order order = parser.parseMessage(orderMsg);
                order.checkAllergens();
                this.orders.Add(order);
                DisplayOrders();

            } 
            catch (Exception ex) 
            {
                Console.WriteLine("TCP Handler threw an error: " + ex.Message);
            }

        }


        //in admin cmd execute following command
        //in netsh http add command change: 
        //      url parameter:      your device_IPv4_address
        //      user parameter:     your device_name\device_username 
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
                order.checkAllergens();
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

        static byte[] AES_Encrypt(string data, byte[] Key, byte[] IV)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
        static string AES_Decrypt(byte[] cipher_text, byte[] Key, byte[] IV)
        {
            if (cipher_text == null || cipher_text.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipher_text))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }




    }
}
