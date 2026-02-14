using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Data.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; } = null!;

        public bool IsDishBig { get;set; } //Determines the size of the dish (Big, Small)

        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
    }
}
