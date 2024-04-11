using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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