using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.IngredientCategory;
using Server.Visitor;

namespace Server.Ingredient
{
    public class Onion : Vegetable
    {
        public override bool Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Onion";
        }
    }
}