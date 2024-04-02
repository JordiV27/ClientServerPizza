using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IVisitor
    {
        public void Visit(Dairy dairy);
        public void Visit(Meat meat);
        public void Visit(Vegetable vegetable);
    }
}
