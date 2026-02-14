using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PizzariaFalia.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Data.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.DishNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public decimal PriceSmall { get; set; }
        [Required]
        public decimal GramsSmall { get; set; }

        //Not every dish can be big, so these are left as non-mandatory
        public decimal PriceBig { get; set; }
        public decimal GramsBig { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<DishIngredient> DishIngredients { get; set; }
            = new HashSet<DishIngredient>();
    }
}
