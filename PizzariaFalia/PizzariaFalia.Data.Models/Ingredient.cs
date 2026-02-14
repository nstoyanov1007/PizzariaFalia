using PizzariaFalia.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Data.Models
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.ToppingNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<DishIngredient> IngredientDishes { get; set; }
            = new HashSet<DishIngredient>();
    }
}
