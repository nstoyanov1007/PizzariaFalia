using PizzariaFalia.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class CategoryFormViewModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.CategoryNameMaxLength,
            MinimumLength = ValidationConstants.CategoryNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(ValidationConstants.CategoryNameMaxLength,
            MinimumLength = ValidationConstants.CategoryNameMinLength)]
        public string DisplayName { get; set; } = null!;

        public int? ParentCategoryId { get; set; }
    }
}
