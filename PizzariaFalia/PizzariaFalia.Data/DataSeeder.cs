using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Data
{
    using System.Text.Json;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using PizzariaFalia.Data;
    using PizzariaFalia.Data.Models;

    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await SeedCategories();
            await SeedDishes();
            await SeedUsers();
        }

        private async Task SeedCategories()
        {
            if (_context.Categories.Any()) return;

            var json = await File.ReadAllTextAsync("SeedData/categories.json");
            var categories = JsonSerializer.Deserialize<List<Category>>(json);

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedDishes()
        {
            if (_context.Dishes.Any()) return;

            var json = await File.ReadAllTextAsync("SeedData/dishes.json");
            var dishes = JsonSerializer.Deserialize<List<Dish>>(json);

            _context.Dishes.AddRange(dishes);
            await _context.SaveChangesAsync();
        }

        private async Task SeedUsers()
        {
            if (_context.Users.Any()) return;

            var json = await File.ReadAllTextAsync("SeedData/users.json");
            var users = JsonSerializer.Deserialize<List<UserSeedModel>>(json);

            foreach (var userData in users)
            {
                var user = new ApplicationUser
                {
                    UserName = userData.Email,
                    Email = userData.Email,
                    Address = userData.Address
                };

                await _userManager.CreateAsync(user, userData.Password);
            }
        }
    }
}
