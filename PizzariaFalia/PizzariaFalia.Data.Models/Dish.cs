using PizzariaFalia.Common;
using System.ComponentModel.DataAnnotations;

namespace PizzariaFalia.Data.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.DishNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ValidationConstants.DishDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        public decimal PriceSmall { get; set; }
        [Required]
        public decimal GramsSmall { get; set; }

        //Not every dish can be big, so these are left as non-mandatory
        public decimal? PriceBig { get; set; }
        public decimal? GramsBig { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public bool isDeleted { get; set; }
    }
}
