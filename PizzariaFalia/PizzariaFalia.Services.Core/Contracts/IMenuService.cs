using PizzariaFalia.ViewModels;

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
