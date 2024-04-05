using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.IngredientCategory;

namespace Server.Visitor
{
    public interface IVisitor
    {
        public bool Visit(Dairy dairy);
        public bool Visit(Meat meat);
        public bool Visit(Vegetable vegetable);
        public bool Visit(Fish fish);
    }
}
