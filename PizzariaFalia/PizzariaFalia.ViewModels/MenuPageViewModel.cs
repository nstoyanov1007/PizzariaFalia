using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class MenuPageViewModel
    {
        public IEnumerable<CategoryTreeViewModel> Categories { get; set; }
            = new List<CategoryTreeViewModel>();

        public IEnumerable<DishIndexViewModel> Dishes { get; set; }
            = new List<DishIndexViewModel>();

        public int? SelectedCategoryId { get; set; }
    }

}
