using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class DishDetailsViewModel
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal PriceSmall { get; set; }
        public decimal GramsSmall { get; set; }

        public decimal? PriceBig { get; set; }
        public decimal? GramsBig { get; set; }

        public string CategoryName { get; set; } = null!;
    }
}
