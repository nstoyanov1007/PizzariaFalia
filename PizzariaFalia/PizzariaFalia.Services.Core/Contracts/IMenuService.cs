using PizzariaFalia.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IMenuService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Dish>> GetAllDishesAsync();
        Task<IEnumerable<Dish>> GetDishesByCategoryAsync(int categoryId);
        Task<Dish> GetDishDetailsAsync(int dishId);
    }
}
