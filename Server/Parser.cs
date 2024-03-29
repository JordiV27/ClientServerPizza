using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Parser
    {
        public Parser() { }

        public Order parseMessage(string message) 
        {
            string[] lines = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int count = 0;
            foreach (string line in lines) 
            {
                Console.WriteLine("["+count.ToString()+"]: " + line);
                count++;
            }


            return new Order(); 
        }
    }
}
