using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Pizza
    {
       
        public List<IIngredient> ingredients;
        private bool lactoseFree;
        private bool vegetarian;
        private bool glutenFree;

        public Pizza() 
        { 
            ingredients = new List<IIngredient>();
        }
    }
}
