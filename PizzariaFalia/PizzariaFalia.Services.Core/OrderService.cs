using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Order order = await _context.Orders.FirstAsync(o => o.Id == orderId);

            order.Status = status;

            await _context.SaveChangesAsync();
        }

        public async Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId)
        {
            Order order = await _context.Orders.FirstAsync(o => o.Id == orderId);

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

        public async Task<IEnumerable<OrderIndexViewModel>> GetAllOrdersAsync()
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
