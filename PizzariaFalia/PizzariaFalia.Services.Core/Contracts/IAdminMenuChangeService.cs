using PizzariaFalia.Data.Models;
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
        Task CreateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
        
        //Dish
        Task CreateDishAsync(Dish dish);
        Task DeleteDishAsync(int dishid);

    }
}
