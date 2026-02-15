using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class DishIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public decimal PriceSmall { get; set; }
        public decimal GramsSmall { get; set; }

        public decimal? PriceBig { get; set; }
        public decimal? GramsBig { get; set; }
    }
}
