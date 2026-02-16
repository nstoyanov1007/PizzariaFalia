using PizzariaFalia.Data.Models;
using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IMenuService
    {
        Task<IEnumerable<CategoryTreeViewModel>> GetAllCategoriesAsync();
        Task<IEnumerable<DishIndexViewModel>> GetAllDishesIndexAsync();
        Task<IEnumerable<DishIndexViewModel>> GetDishesIndexByCategoryAsync(int categoryId);
        Task<DishDetailsViewModel> GetDishDetailsAsync(int dishId);


    }
}
