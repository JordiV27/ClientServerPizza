using Server.IngredientCategory;
using Server.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Ingredient
{
    public class Tuna : Fish
    {
        public override bool Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Tuna";
        }
    }
}