using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Visitor;

namespace Server.IngredientCategory
{
    public interface IIngredient
    {
        public bool Accept(IVisitor visitor);
    }
}
