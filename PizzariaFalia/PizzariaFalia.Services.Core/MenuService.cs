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
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        public MenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryTreeViewModel>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => !c.isDeleted && c.ParentCategory == null)
                .Include(c => c.SubCategories)
                .Select(c => new CategoryTreeViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Children = c.SubCategories.Where(c2 => !c2.isDeleted)
                        .Select(c2 => new CategoryTreeViewModel
                        {
                            Id = c2.Id,
                            Name = c2.Name,
                        }).ToList()
                }).ToListAsync();
        }

        public async Task<IEnumerable<DishIndexViewModel>> GetAllDishesIndexAsync()
        {
            return await _context.Dishes
                .Where(d => !d.isDeleted)
                .Select(d => new DishIndexViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    GramsBig = d.GramsBig,
                    GramsSmall = d.GramsSmall,
                    PriceBig = d.PriceBig,
                    PriceSmall = d.PriceSmall,
                }).ToListAsync();
        }

        public async Task<DishDetailsViewModel> GetDishDetailsAsync(int dishId)
        {
            var dish = await _context.Dishes
                .Where(d => !d.isDeleted && d.Id == dishId)
                .Include(d => d.Category)
                .Select(d => new DishDetailsViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    GramsBig = d.GramsBig,
                    GramsSmall = d.GramsSmall,
                    PriceBig = d.PriceBig,
                    PriceSmall = d.PriceSmall,
                    CategoryName = d.Category.Name,
                    CategoryId = d.Category.Id,
                }).FirstOrDefaultAsync();

            if (dish != null)
                return dish;
            else
                throw new InvalidDataException("Dish does not exist");
        }

        public async Task<IEnumerable<DishIndexViewModel>> GetDishesIndexByCategoryAsync(int categoryId)
        {

            var categoryIds = await _context.Categories
                .Where(c => c.Id == categoryId || c.ParentCategoryId == categoryId)
                .Select(c => c.Id)
                .ToListAsync();

            var dishes = await _context.Dishes
                .Where(d => !d.isDeleted && categoryIds.Contains(d.CategoryId))
                .Select(d => new DishIndexViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    GramsBig = d.GramsBig,
                    GramsSmall = d.GramsSmall,
                    PriceBig = d.PriceBig,
                    PriceSmall = d.PriceSmall,
                })
                .ToListAsync();

            return dishes;

        }
    }
}
