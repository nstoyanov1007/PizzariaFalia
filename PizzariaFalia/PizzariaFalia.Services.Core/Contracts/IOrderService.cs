using PizzariaFalia.Data.Models;
using PizzariaFalia.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IOrderService
    {
        Task SubmitOrderAsync(int orderId);
        Task ChangeOrderStatusAsync(int orderId, Status status);

        Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersAsync(string userId);
    }
}
