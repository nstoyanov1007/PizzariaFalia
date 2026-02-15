using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class CartViewModel
    {
        public ICollection<OrderItemViewModel> Items { get; set; }
           = new HashSet<OrderItemViewModel>();

        public decimal? TotalPrice => Items.Sum(x => x.Price);
    }
}
