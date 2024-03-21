using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Pizza
    {
        public Pizza() { }
        private List<IIngredient> ingredients;
        private bool lactoseFree;
        private bool vegetarian;
    }
}
