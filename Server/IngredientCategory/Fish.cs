using Server.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.IngredientCategory
{
    public abstract class Fish : IIngredient
    {
        public abstract bool Accept(IVisitor visitor);
        public abstract override string ToString();
    }
}
