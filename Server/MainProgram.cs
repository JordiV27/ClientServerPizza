using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MainProgram {
        public static int Main(String[] args)
        {
            Server server = Server.getInstance();
            server.Start();

            return 0;
        }
    }
}
