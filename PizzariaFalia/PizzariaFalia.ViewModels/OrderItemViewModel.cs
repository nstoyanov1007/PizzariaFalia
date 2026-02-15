using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class OrderItemViewModel
    {
        public int DishId { get; set; }
        public string DishName { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsBig { get; set; }

    }
}
