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
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddItemToCartAsync(DishDetailsViewModel item, string userId)
        {
            Order? order = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == Status.Pending).FirstOrDefaultAsync();

            if (order == null)
            {
                await CreateCartOrderAsync(userId);

                order = await _context.Orders
                    .Where(o => o.UserId == userId && o.Status == Status.Pending).FirstOrDefaultAsync();
            }
            

            await _context.OrderItems.AddAsync(new OrderItem()
            {
                DishId = item.Id,
                IsDishBig = item.IsBig,
                OrderId = order.Id
            });

            await _context.SaveChangesAsync();
        }

        public async Task AddItemToCartAsync(DishIndexViewModel item, string userId)
        {
            Order? order = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == Status.Pending).FirstOrDefaultAsync();

            if (order == null)
            {
                await CreateCartOrderAsync(userId);

                order = await _context.Orders
                    .Where(o => o.UserId == userId && o.Status == Status.Pending).FirstOrDefaultAsync();
            }


            await _context.OrderItems.AddAsync(new OrderItem()
            {
                DishId = item.Id,
                IsDishBig = item.IsBig,
                OrderId = order.Id
            });

            await _context.SaveChangesAsync();
        }

        public async Task CreateCartOrderAsync(string userId)
        {
            if (_context.Orders.Where(o => o.UserId == userId && o.Status == Status.Pending).Any())
                return;
            else
            {
                Order order = new Order()
                {
                    UserId = userId,
                    Status = Status.Pending,
                    CreatedAt = DateTime.Now,
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrderItemViewModel>> GetCartItemsAsync(string userId)
        {
            Order? order = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == Status.Pending)
                .FirstOrDefaultAsync();

            if(order != null)
            {
                return await _context.OrderItems
                    .Where(oi => oi.OrderId == order.Id)
                    .Include(oi => oi.Dish)
                    .Select(oi => new OrderItemViewModel()
                    {
                        Id = oi.OrderId,
                        DishId = oi.DishId,
                        DishName = oi.Dish.Name,
                        IsBig = oi.IsDishBig,
                        Price = oi.IsDishBig
                            ? oi.Dish.PriceBig
                            : oi.Dish.PriceSmall
                    })
                    .ToListAsync();
            }
            return new List<OrderItemViewModel>();
        }

        public async Task RemoveItemFromCartAsync(OrderItemViewModel item, string userId)
        {
            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.Id == item.Id);

            if (orderItem == null)
                return;

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}
