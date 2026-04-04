using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data.Models;

namespace PizzariaFalia.Data
{
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
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories ON");
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Dishes ON");

            await SeedCategories();
            await SeedDishes();
            await SeedUsers();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories OFF");
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Dishes OFF");
        }

        private async Task SeedCategories()
        {
            if (_context.Categories.Any()) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "categories.json");
            var json = await File.ReadAllTextAsync(path);
            var categories = JsonSerializer.Deserialize<List<Category>>(json);

            using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories ON");

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Categories OFF");

            await transaction.CommitAsync();
        }

        private async Task SeedDishes()
        {
            if (_context.Dishes.Any()) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "dishes.json");
            var json = await File.ReadAllTextAsync(path);
            var dishes = JsonSerializer.Deserialize<List<Dish>>(json);

            using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Dishes ON");

            _context.Dishes.AddRange(dishes);
            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Dishes OFF");

            await transaction.CommitAsync();
        }

        private async Task SeedUsers()
        {
            if (_context.Users.Any()) return;

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "SeedData",
                "users.json"
            );

            var json = await File.ReadAllTextAsync(path);
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
