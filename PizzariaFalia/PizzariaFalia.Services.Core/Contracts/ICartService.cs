using PizzariaFalia.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface ICartService
    {
        Task CreateCartOrderAsync(string userId);
        Task CancelCartOrderAsync(int orderId);

        Task AddItemToCartAsync(int orderId, int DishId, bool isDishBig);
        Task RemoveItemFromCartAsync(int orderId, int orderItemId);

        Task<IEnumerable<OrderItem>> GetCartItemsAsync(int orderId);
        Task<IEnumerable<OrderItem>> GetCartItemsAsync(string userId);

    }
}
