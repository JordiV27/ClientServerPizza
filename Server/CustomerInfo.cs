using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //Basically an immutable class
    public class CustomerInfo 
    {
        private readonly string _name;
        private readonly string _address;
        private readonly string _postal_code_city;


        public CustomerInfo(string name = "", string address = "", string postal_code_city = "")
        {
            _name = name;
            _address = address;
            _postal_code_city = postal_code_city;
        }

        public override string ToString()
        {
            return
                "----------Customer Information----------"              + Environment.NewLine +
                $"Name: \t\t\t{_name}"                                  + Environment.NewLine +
                $"Address: \t\t{_address}"                              + Environment.NewLine +
                $"Postal code and city: \t{_postal_code_city}"          + Environment.NewLine;
        }
    }
}
