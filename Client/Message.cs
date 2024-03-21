using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client 
{
    public class Message 
    {
        private CustomerPersonalia _cp;
        private string _pizza_order;
        private string _date_time;

        public Message(CustomerPersonalia cp, string pizza_order, string date_time) 
        {
            _cp = cp;
            _pizza_order = pizza_order;
            _date_time = date_time;
        }

        public override string ToString()
        {
            return _cp.ToString() + _pizza_order + _date_time;
        }

    }
}
