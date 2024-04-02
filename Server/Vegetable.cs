using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class Vegetable : IIngredient
    {
        public const bool child_friendly = false;
        public abstract void Accept(IVisitor visitor);
        
    }
}
