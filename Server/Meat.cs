using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Meat : IIngredient
    {

        public const bool vegetarian = false;
        public abstract void Accept(IVisitor visitor);
        
    }
}
