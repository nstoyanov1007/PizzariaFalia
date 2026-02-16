using PizzariaFalia.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class DishFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.DishNameMaxLength,
            MinimumLength = ValidationConstants.DishNameMinLength)]
        public string Name { get; set; } = null!;

        [StringLength(ValidationConstants.DishDescriptionMaxLength,
            MinimumLength = ValidationConstants.DishDescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Required]
        [DataType(DataType.Currency)]
        public decimal PriceSmall { get; set; }
        [Required]
        public decimal GramsSmall { get; set; }

        //Not every dish can be big, so these are left as non-mandatory
        public decimal? PriceBig { get; set; }
        public decimal? GramsBig { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
