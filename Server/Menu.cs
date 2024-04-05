using Server.IngredientCategory;
using Server.Ingredient;
using Server.Ingredients;

namespace Server
{
    public class Menu
    {
        public static Dictionary<string, IIngredient> _ingredients;


        public Menu()
        {
            _ingredients = new Dictionary<string, IIngredient>();
            FillIngredients();
        }

        //Keys are regex: ignore all spaces and newline characters, but find "mozzarella", "mushroom" etc.
        private void FillIngredients()
        {
            _ingredients.Add("[^ \n]*mozzarella", new Mozzarella());
            _ingredients.Add("[^ \n]*mushroom", new Mushroom());
            _ingredients.Add("[^ \n]*onion", new Onion());
            _ingredients.Add("[^ \n]*salami", new Salami());
            _ingredients.Add("[^ \n]*tuna", new Tuna());
        }
        public Dictionary<string, IIngredient> GetIngredients()
        {
            return _ingredients;
        }
    }
}