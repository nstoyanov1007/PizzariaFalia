using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core
{
    public class AdminMenuChangeService : IAdminMenuChangeService
    {
        private readonly ApplicationDbContext _context;

        public AdminMenuChangeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateCategoryAsync(CategoryFormViewModel category)
        {
            try
            {
                await _context.Categories.AddAsync(new Category()
                {
                    ParentCategoryId = category.ParentCategoryId,
                    Name = category.Name,
                    DisplayName = category.DisplayName,
                });

            }
            catch
            {
                throw new ArgumentException("Invalid View Model submitted");
            }

            await _context.SaveChangesAsync();

        }

        public async Task CreateDishAsync(DishFormViewModel dish)
        {
            try
            {
                await _context.Dishes.AddAsync(new Dish()
                {
                    CategoryId = dish.CategoryId,
                    Name = dish.Name,
                    Description = dish.Description,
                    GramsBig = dish.GramsBig,
                    GramsSmall = dish.GramsSmall,
                    PriceBig = dish.PriceBig,
                    PriceSmall = dish.PriceSmall,
                });

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new ArgumentException("Invalid View Model submitted");
            }
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            try
            {
                _context.Categories.FirstAsync(c => c.Id == categoryId).Result.isDeleted = true;

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Category does not exist");
            }
        }

        public async Task DeleteDishAsync(int dishid)
        {
            try
            {
                _context.Dishes.FirstAsync(d => d.Id == dishid).Result.isDeleted = true;

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Dish does not exist");
            }
        }

        public async Task EditCategoryAsync(CategoryFormViewModel category)
        {
            Category categoryContext;
            try
            {
               categoryContext = await _context.Categories.FirstAsync(c => c.Id == category.Id);
            }
            catch
            {
                throw new ArgumentException("No such category exists");
            }


            try
            {
                categoryContext.ParentCategoryId = category.ParentCategoryId;
                categoryContext.Name = category.Name;
                categoryContext.DisplayName = category.DisplayName;

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Invalid View Model");
            }
        }

        public async Task EditDishAsync(DishFormViewModel dish)
        {
            Dish dishContext;
            try
            {
                dishContext = await _context.Dishes.FirstAsync(d => d.Id == dish.Id);
            }
            catch
            {
                throw new ArgumentException("No such dish exists");
            }
            try
            {
                dishContext.CategoryId = dish.CategoryId;
                dishContext.Name = dish.Name;
                dishContext.Description = dish.Description;
                dishContext.GramsBig = dish.GramsBig;
                dishContext.GramsSmall = dish.GramsSmall;
                dishContext.PriceBig = dish.PriceBig;
                dishContext.PriceSmall = dish.PriceSmall;

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new InvalidOperationException("Invalid View Model");
            }
        }
    }
}
