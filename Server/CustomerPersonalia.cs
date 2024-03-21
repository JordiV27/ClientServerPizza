using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class CustomerPersonalia
    {
            private string _name;
            private string _address;
            private string _city;
            private string _postal_code;

            //Constructor
            public CustomerPersonalia(string name, string address, string city, string postal_code)
            {
                _name = name;
                _address = address;
                _city = city;
                _postal_code = postal_code;
            }

            public override string ToString()
            {
                return "Customer Information" + "\n" +
                       "---------------------------------------" +
                       "Name        : " + _name + "\n" +
                       "Address     : " + _address + "\n" +
                       "City        : " + _city + "\n" +
                       "Postal Code : " + _postal_code + "\n" +
                       "-----------------------------------------";
            }
    }
}
