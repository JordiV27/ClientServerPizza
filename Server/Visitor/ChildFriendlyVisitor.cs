using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.IngredientCategory;

namespace Server.Visitor
{
    public class ChildFriendlyVisitor : IVisitor
    {
        public bool Visit(Dairy dairy)
        {
            return true;
        }

        public bool Visit(Meat meat)
        {
            return true;
        }

        public bool Visit(Vegetable vegetable)
        {
            return false;
        }

        public bool Visit(Fish fish)
        {
            return false;
        }
    }
}