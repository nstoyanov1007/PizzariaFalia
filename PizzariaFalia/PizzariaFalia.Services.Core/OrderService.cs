using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Services.Core
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task ChangeOrderStatusAsync(int orderId, Status status)
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Invalid Order Id");

            order.Status = status;

            await _context.SaveChangesAsync();
        }

        public async Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId)
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Invalid order Id");

            return new OrderDetailsViewModel()
            {
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Items = GetOrderItemsAsync(orderId).Result.ToList(),
            };
        }

        public async Task<IEnumerable<OrderItemViewModel>> GetOrderItemsAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.Dish)
                .Select(oi => new OrderItemViewModel
                {
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    Id = oi.Id,
                    IsBig = oi.IsDishBig,
                    Price = oi.IsDishBig
                            ? oi.Dish.PriceBig
                            : oi.Dish.PriceSmall,
                    Grams = oi.IsDishBig
                            ? oi.Dish.GramsBig
                            : oi.Dish.GramsSmall
                }).ToListAsync();
        }

        public async Task<IEnumerable<OrderIndexViewModel>> GetAllOrdersAsync(string? userId = null)
        {
            if(userId != null)
            {
                return await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Select(o => new OrderIndexViewModel()
                    {
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        Id = o.Id,
                        UserId = o.UserId,
                    }).ToListAsync();
            }
            else
            {
                return await _context.Orders
                    .Select(o => new OrderIndexViewModel()
                    {
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        Id = o.Id,
                        UserId = o.UserId,
                    }).ToListAsync();
            }
        }
    }
}
