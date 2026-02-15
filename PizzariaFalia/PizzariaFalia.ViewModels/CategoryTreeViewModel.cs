using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class CategoryTreeViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<CategoryTreeViewModel> Children { get; set; }
            = new HashSet<CategoryTreeViewModel>();
    }
}
