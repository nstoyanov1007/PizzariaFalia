using PizzariaFalia.Common;
using System.ComponentModel.DataAnnotations;

namespace PizzariaFalia.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        //Fully for customisation purposes and to make category visualiation easier
        //E.g. - Category (Pizza), Subcategory(Name = Pizza with meat products, DisplayName = with Meat Products)
        [MaxLength(ValidationConstants.CategoryNameMaxLength)]
        public string? DisplayName { get; set; }

        //If null - Master category
        //If not null - subcategory of another category
        public int? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }

        public bool isDeleted { get; set; }

        public virtual ICollection<Dish> Dishes { get; set; }
            = new HashSet<Dish>();
        public virtual ICollection<Category> SubCategories { get; set; }
            = new HashSet<Category>();
    }
}
