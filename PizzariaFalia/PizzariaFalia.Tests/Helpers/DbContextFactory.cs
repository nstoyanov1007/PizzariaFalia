using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;

namespace PizzariaFalia.Tests.Helpers
{
    /// <summary>
    /// Provides an in-memory ApplicationDbContext with an isolated database per call.
    /// Each test should call Create() to get a fresh, empty context.
    /// </summary>
    public static class DbContextFactory
    {
        public static ApplicationDbContext Create(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        // ── Seed helpers ─────────────────────────────────────────────────────

        public static ApplicationUser SeedUser(
            ApplicationDbContext context,
            string id,
            string userName = "testuser",
            string email = "test@example.com",
            string address = "123 Test St")
        {
            var user = new ApplicationUser
            {
                Id = id,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                Address = address,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public static Category SeedCategory(
            ApplicationDbContext context,
            string name = "Pizza",
            string? displayName = null,
            int? parentCategoryId = null,
            bool isDeleted = false)
        {
            var category = new Category
            {
                Name = name,
                DisplayName = displayName,
                ParentCategoryId = parentCategoryId,
                isDeleted = isDeleted
            };
            context.Categories.Add(category);
            context.SaveChanges();
            return category;
        }

        public static Dish SeedDish(
            ApplicationDbContext context,
            int categoryId,
            string name = "Margherita",
            string description = "Classic tomato and mozzarella pizza",
            decimal priceSmall = 8.99m,
            decimal gramSmall = 300,
            decimal? priceBig = 12.99m,
            decimal? gramBig = 500,
            bool isDeleted = false)
        {
            var dish = new Dish
            {
                CategoryId = categoryId,
                Name = name,
                Description = description,
                PriceSmall = priceSmall,
                GramsSmall = gramSmall,
                PriceBig = priceBig,
                GramsBig = gramBig,
                isDeleted = isDeleted
            };
            context.Dishes.Add(dish);
            context.SaveChanges();
            return dish;
        }
    }
}
