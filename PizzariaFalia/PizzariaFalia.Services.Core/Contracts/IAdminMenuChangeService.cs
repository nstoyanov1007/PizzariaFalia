using PizzariaFalia.Data.Models;
using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IAdminMenuChangeService
    {
        //Category
        Task CreateCategoryAsync(CategoryFormViewModel category);
        Task EditCategoryAsync(CategoryFormViewModel category);
        Task DeleteCategoryAsync(int categoryId);
        
        //Dish
        Task CreateDishAsync(DishFormViewModel dish);
        Task EditDishAsync(DishFormViewModel dish);
        Task DeleteDishAsync(int dishid);

    }
}
