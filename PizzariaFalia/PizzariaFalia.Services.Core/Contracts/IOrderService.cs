using PizzariaFalia.Data.Models;
using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IOrderService
    {
        Task ChangeOrderStatusAsync(int orderId, Status status);

        Task<IEnumerable<OrderItemViewModel>> GetOrderItemsAsync(int orderId);
        Task<IEnumerable<OrderIndexViewModel>> GetAllOrdersAsync();
        Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId);
    }
}
