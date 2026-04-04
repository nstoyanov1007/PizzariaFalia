using PizzariaFalia.ViewModels;

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
