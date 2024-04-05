using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Text;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.Intrinsics.X86;
using Aes = System.Security.Cryptography.Aes;

namespace Client
{
    public class Client
    {
        private string ip_address; //ALERT: use HOST IPv4 address
        private int port;

        private string pizza_order1 =
                                "Jansen"                + Environment.NewLine +
                                "Nieuwestad 14"         + Environment.NewLine +
                                "8901 PM Leeuwarden"    + Environment.NewLine +
                                "Margherita"            + Environment.NewLine +
                                "1"                     + Environment.NewLine +
                                "0"                     + Environment.NewLine +
                                "05/12/2022 18:30";
        private string pizza_order2 =
                                "Pieterson"             + Environment.NewLine +
                                "Langestraat 6"         + Environment.NewLine +
                                "2348 AB Almere"        + Environment.NewLine +
                                "Tonno"                 + Environment.NewLine +
                                "3"                     + Environment.NewLine +
                                "2"                     + Environment.NewLine +
                                "Onion"                 + Environment.NewLine +
                                "Tuna"                  + Environment.NewLine +
                                "05/03/2024 12:00";
        private string pizza_order3 =
                                "Hein"                  + Environment.NewLine +
                                "Oude Kerkstraat 90"    + Environment.NewLine +
                                "4867 HF Groningen"     + Environment.NewLine +
                                "Calzone"               + Environment.NewLine +
                                "2"                     + Environment.NewLine +
                                "0"                     + Environment.NewLine +
                                "Diavolo"               + Environment.NewLine +
                                "1"                     + Environment.NewLine +
                                "2"                     + Environment.NewLine +
                                "Mozzarella"            + Environment.NewLine +
                                "Salami"                + Environment.NewLine +
                                "06/07/2018 16:20";

        public Client(string ip_address, int port)
        {
            this.ip_address = ip_address;
            this.port = port;
        }

        public async Task StartHTTPSClient()
        {
            string url = $"http://{ip_address}:{port}/";
            try
            {
                string response = await MakeRequestAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartHTTPSClient error: {0}", ex.Message);
            }
        }

        private async Task<string> MakeRequestAsync(string url)
        { 
            using (HttpClient client = new HttpClient()) 
            { 
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                var response = client.GetStringAsync(url);

                return await response;
            }
        }

        public async Task StartTCPClient()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(this.ip_address, this.port);
                    Console.WriteLine("Connected to server");

                    (byte[] aesKey, byte[] iv) = await TcpAsyncKeyExchange(client); //closes program here

                    await HandleTcpAsyncClient(client, aesKey, iv, pizza_order2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartTCPClient error: {0}", ex.Message);
            }
        }

        private async Task<(byte[], byte[])> TcpAsyncKeyExchange(TcpClient client)
        {
            // Step 1: Client generates a unique AES key
            Aes aes = Aes.Create();
            byte[] aesKey = aes.Key;
            byte[] iv = aes.IV;

            byte[] aesKeyAndIV = aesKey.Concat(iv).ToArray();   
            // Step 2: Retrieve public RSA key and encrypt AES key 
            byte[] rsaPublicKey = await ReadDataFromStreamAsync(client);
            RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(rsaPublicKey, out _);
            byte[] encryptedAesKey = rsa.Encrypt(aesKeyAndIV, RSAEncryptionPadding.OaepSHA256);

            // Step 3: Send generated key over to server 
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(encryptedAesKey, 0, encryptedAesKey.Length);


            return (aesKey, iv);
        }

        private async Task HandleTcpAsyncClient(TcpClient client, byte[] aesKey, byte[] iv, string pizzaOrder)
        {
            try
            {
                byte[] encryptedOrder = EncryptData(pizzaOrder, aesKey, iv);
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(encryptedOrder);

                byte[] encryptedRespone = await ReadDataFromStreamAsync(client);
                string response = DecryptData(encryptedRespone, aesKey, iv);
                /* Print confirmation of pizza order: */
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HandleTcpAsyncClient error: {0}", ex.Message);
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client closed.");
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

        private async Task<byte[]> ReadDataFromStreamAsync(TcpClient client)
        {
            List<byte> dataList = new List<byte>();
            byte[] buffer = new byte[1024];  //Weird stuff going on here
            int bytesRead;

            NetworkStream stream = client.GetStream();
            while (!stream.DataAvailable && client.Connected) { }
            bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            byte[] receivedData = new byte[bytesRead];
            Array.Copy(buffer, receivedData, bytesRead);
            dataList.AddRange(receivedData);
            
            return dataList.ToArray();
        }
    }
}