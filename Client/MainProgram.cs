using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class MainProgram
    {
        private const string IPADDRESS = "192.168.68.110";
        private const int TCP_PORT = 12345;
        public static int Main(String[] args)
        {

            Client client = new Client(IPADDRESS, TCP_PORT);
            client.StartTcpClient();

            Console.WriteLine("\n\n");


            return 0;
        }
    }
}
