using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.IngredientCategory;
using Server.Visitor;

namespace Server.Ingredients
{
    public class Salami : Meat
    {
        public override bool Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Salami";
        }
    }
}