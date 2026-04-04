using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface ICartService
    {
        Task CreateCartOrderAsync(string userId);
        Task PlaceCartOrderAsync(string userId);
        Task AddItemToCartAsync(DishDetailsViewModel item, string userId);
        Task AddItemToCartAsync(DishIndexViewModel item, string userId);
        Task RemoveItemFromCartAsync(OrderItemViewModel item, string userId);

        Task<IEnumerable<OrderItemViewModel>> GetCartItemsAsync(string userId);

        Task<DishDetailsViewModel> GetDishDetailsAsync(int dishId);
    }
}
