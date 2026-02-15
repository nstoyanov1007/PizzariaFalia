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
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Dish>> GetAllDishesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Dish> GetDishDetailsAsync(int dishId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Dish>> GetDishesByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
