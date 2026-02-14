using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Data.Models
{
    public class DishIngredient
    {
        [Required]
        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;

        [Required]
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;
    }
}
