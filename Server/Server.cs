using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
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

        public async void Start()
        {
            Task tcpServer = StartTCPServerAsync();
            Task httpServer = StartHttpServerAsync();
            
            
            await Task.WhenAll(httpServer, tcpServer);
        }

        private async Task StartTCPServerAsync()
        {
            IPAddress ip = IPAddress.Parse(IPADDRESS);
            TcpListener listener = new TcpListener(ip, TCP_PORT);
            listener.Start();
            Console.WriteLine("TCP server started. Listening for connections...");

            try 
            { 
                while (true) 
                { 
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine(format:"Client connected from: {0}", client.Client.RemoteEndPoint);

                    (byte[] aesKey, byte[] iv) = await TcpAsyncKeyExchange(client);

                    if (aesKey != null && iv != null)
                    {
                        await HandleTcpAsyncClient(client, aesKey, iv);
                    } 
                    else 
                    {
                        Console.WriteLine("Key exchange failed. Closing connection...");
                        client.Close();
                    }
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Error on StartTcpServer(): {0}", ex.ToString());
            } 
            finally 
            {
                listener.Stop();
                Console.WriteLine("TCP server stopped.");
            }
        }

        private async Task<(byte[], byte[])> TcpAsyncKeyExchange(TcpClient client) 
        {
            try
            {
                RSA rsa;
                // Step 0: Generate RSA key pair & export public and private keys
                using (rsa = RSA.Create())
                {
                    byte[] rsaPublicKey = rsa.ExportRSAPublicKey();
                    ExportPrivateKeyBytes(rsa.ExportRSAPrivateKey());
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(rsaPublicKey, 0, rsaPublicKey.Length); //Throws exception: connection forcibly closed by remote client
                }

                //Step 4: Receive encrypted AES key from client
                byte[] encryptedAESKeyAndIV = await ReadDataFromStreamAsync(client);

                // Step 5:
                using (rsa = RSA.Create())
                {
                    byte[] privateKey = LoadPrivateKeyBytes();
                    rsa.ImportRSAPrivateKey(privateKey, out _);

                    byte[] aesKeyAndIV = rsa.Decrypt(encryptedAESKeyAndIV, RSAEncryptionPadding.OaepSHA256);

                    byte[] aesKey = aesKeyAndIV.Take(32).ToArray(); //Assuming AES key size is 256 bits (32 bytes)
                    byte[] iv = aesKeyAndIV.Skip(32).ToArray(); //Assuming IV is 16 bytes

                    return (aesKey, iv);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("TCP KeyExchange error: {0}", ex.Message);
                return (null, null);
            }
        }

        private async Task HandleTcpAsyncClient(TcpClient client, byte[] aesKey, byte[] iv)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                while (client.Connected)
                {
                    byte[] encryptedMessage = await ReadDataFromStreamAsync(client);

                    string decryptedMessage = DecryptData(encryptedMessage, aesKey, iv);

                    /*
                        Pizza order functions go here! 
                    */
                    Console.WriteLine("Order received from customer: {0}", client.Client.RemoteEndPoint);

                    Parser pizzaParser = new Parser(client);
                    Order pizzaOrder = pizzaParser.parseMessage(decryptedMessage);
                    pizzaOrder.checkAllergens();
                    this.orders.Add(pizzaOrder);
                    DisplayOrders();

                    Console.WriteLine(pizzaOrder.ToString());

                    string response = pizzaOrder.ToString(); 
                    byte[] encryptedResponse = EncryptData(response, aesKey, iv);

                    await stream.WriteAsync(encryptedResponse, 0, encryptedResponse.Length);
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HandleTcpAsyncClient error: {0}", ex.Message);
            }
            finally
            { 
                Console.WriteLine("Client disconnected.");
            }
        }
        private string DecryptData(byte[] cipherText, byte[] aesKey, byte[] iv)
        {
            // Sanitization checks
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("Ciphertext");
            }
            if (aesKey == null || aesKey.Length <= 0)
            {
                throw new ArgumentNullException("AES Key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("IV");
            }

            string message = "";
            using (Aes aes = Aes.Create()) 
            { 
                aes.Key = aesKey;
                aes.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            message = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return message;
        }
        private byte[] EncryptData(string message, byte[] aesKey, byte[] iv)
        {
            // Sanitization checks
            if (message == null || message.Length <= 0) 
            { 
                throw new ArgumentNullException("Message"); 
            }
            if (aesKey == null || aesKey.Length <= 0) 
            { 
                throw new ArgumentNullException("AES Key");
            }
            if (iv == null || iv.Length <= 0)
            { 
                throw new ArgumentNullException("IV");
            }

            // Encryption code from: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-8.0
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            { 
                aes.Key = aesKey;
                aes.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(message);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
        private byte[] LoadPrivateKeyBytes()
        {
            string path = "C:/Users/jordi/source/repos/ClientServerPizza/Server/RSAPrivateKey/key.pem";
            byte[] rsaPrivateKey = File.ReadAllBytes(path);
            return rsaPrivateKey;
        }
        private void ExportPrivateKeyBytes(byte[] rsaPrivateKey) 
        {
            string path = "C:/Users/jordi/source/repos/ClientServerPizza/Server/RSAPrivateKey/key.pem";
            File.WriteAllBytes(path, rsaPrivateKey);
        }
        private async Task<byte[]> ReadDataFromStreamAsync(TcpClient client)
        {
            List<byte> dataList = new List<byte>();
            byte[] buffer = new byte[1024];
            int bytesRead;

            NetworkStream stream = client.GetStream();
            while (!stream.DataAvailable) { }
            bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            byte[] receivedData = new byte[bytesRead];
            Array.Copy(buffer, receivedData, bytesRead);
            dataList.AddRange(receivedData);

            return dataList.ToArray();
        }



        //in admin cmd execute following command                (change user params in add command to DEVICENAME\username)
        //in admin cmd execute following command
        //in netsh http add command change: 
        //      url parameter:      your device_IPv4_address
        //      user parameter:     your device_name\device_username 
        //netsh http add urlacl url=http://192.168.68.110:54321/ user=LENOVOPC\jordi listen=yes
        //netsh http add urlacl url=http://192.168.68.117:54321/ user=LAPTOP-V2BOURCE\Yvonne listen=yes
        //netsh http delete urlacl url = http://192.168.68.110:54321/
        private async Task StartHttpServerAsync()
        {
            string url = "http://" + IPADDRESS + ":" + HTTP_PORT.ToString() + "/";

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Console.WriteLine($"HTTP Server Listening... on {url}");


            try 
            { 
                //while (listener.IsListening)  // original code
                while(true)
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    _ = HandleHttpRequestAsync(context);
                }
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine("HTTP server error: " + ex.Message); 
            } 
            finally 
            {
                listener.Stop();
                Console.WriteLine("HTTP server stopped.");
            }
        }

        private async Task HandleHttpRequestAsync(HttpListenerContext context) 
        {
            try 
            {
                string response = "Hello from the server!";
                byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);

                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = responseBytes.Length;

                await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                context.Response.OutputStream.Close();
                context.Response.Close();
                Console.WriteLine($"Sent response to client: {response}");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error in HandleHttpRequestAsync: {0}", ex.Message);
            }
        }

        /*public void HandleHttpRequest(HttpListenerContext context)
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
        }*/

        private void DisplayOrders()
        { 
            foreach (Order order in this.orders) 
            { 
                Console.WriteLine($"{order.ToString()}");
                Console.WriteLine();
                Console.WriteLine(); 
                Console.WriteLine();
            }
        }
    
    }
}
