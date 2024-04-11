using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Client 
{
    public class Client 
    {
        private string _ip_address;
        private int _port;

        private string pizza_order1 =
                                "Jansen" + Environment.NewLine +
                                "Nieuwestad 14" + Environment.NewLine +
                                "8901 PM Leeuwarden" + Environment.NewLine +
                                "Margherita" + Environment.NewLine +
                                "1" + Environment.NewLine +
                                "0" + Environment.NewLine +
                                "05/12/2022 18:30";
        private string pizza_order2 =
                                "Pieterson" + Environment.NewLine +
                                "Langestraat 6" + Environment.NewLine +
                                "2348 AB Almere" + Environment.NewLine +
                                "Tonno" + Environment.NewLine +
                                "3" + Environment.NewLine +
                                "2" + Environment.NewLine +
                                "Onion" + Environment.NewLine +
                                "Tuna" + Environment.NewLine +
                                "05/03/2024 12:00";
        private string pizza_order3 =
                                "Hein" + Environment.NewLine +
                                "Oude Kerkstraat 90" + Environment.NewLine +
                                "4867 HF Groningen" + Environment.NewLine +
                                "Calzone" + Environment.NewLine +
                                "2" + Environment.NewLine +
                                "0" + Environment.NewLine +
                                "Diavolo" + Environment.NewLine +
                                "1" + Environment.NewLine +
                                "2" + Environment.NewLine +
                                "Mozzarella" + Environment.NewLine +
                                "Salami" + Environment.NewLine +
                                "06/07/2018 16:20";


        public Client(string ip_address, int port)
        {
            _ip_address = ip_address;
            _port = port;   
        }

        public void StartTcpClient()
        {
            // IP address and port of the server to connect to
            //string serverIp = "192.168.68.110"; // Replace this with the IPv4 address of the server
            string serverIp = "192.168.68.145"; //LAPTOP IPv4
            //string serverIp = "192.168.68.117"; // Replace this with the IPv4 address of the server
            int port = 12345;

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


            try
            {
                using (TcpClient client = new TcpClient()) {
                    client.Connect(serverIp, port);
                    Console.WriteLine("Connected to server.");

                    using (NetworkStream stream = client.GetStream())
                    {

                        //byte[] data = Encoding.ASCII.GetBytes(pizza_order2);
                        byte[] data = AES_Encrypt(pizza_order2, aes.Key, aes.IV);
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