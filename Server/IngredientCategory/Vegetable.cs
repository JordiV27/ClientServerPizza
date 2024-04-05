using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Visitor;

namespace Server.IngredientCategory
{
    public abstract class Vegetable : IIngredient
    {
        public abstract bool Accept(IVisitor visitor);
        public abstract override string ToString();
    }
}