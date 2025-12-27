using System.Collections.Generic;

namespace BakeryApp.DAL.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Weight { get; set; }
        public List<ProductIngredient> Ingredients { get; set; } = new List<ProductIngredient>();
    }

    // This class represents the junction table between Product and Ingredient
    public class ProductIngredient
    {
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public double Quantity { get; set; }
    }
}
