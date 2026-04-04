using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IOrderService
    {
        Task ChangeOrderStatusAsync(int orderId, Status status);

        Task<IEnumerable<OrderItemViewModel>> GetOrderItemsAsync(int orderId);
        Task<IEnumerable<OrderIndexViewModel>> GetAllOrdersAsync(string? userId = null);
        Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId);
    }
}
