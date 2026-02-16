using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data.Models;

namespace PizzariaFalia.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Pizzas", DisplayName = "Pizzas", },
                new Category() { Id = 2, Name = "Pizzas with meat", DisplayName = "With Meat", ParentCategoryId = 1},
                new Category() { Id = 3, Name = "Pizzas with no meat", DisplayName = "No Meat", ParentCategoryId = 1},
                new Category() { Id = 4, Name = "Salads", DisplayName = "Salads", },
                new Category() { Id = 5, Name = "Desserts", DisplayName = "Desserts", }     
            );

            builder.Entity<Dish>().HasData(
                new Dish()
                {
                    Id = 1,
                    Name = "Margherita",
                    Description = "Classic pizza with tomato sauce, mozzarella and fresh basil.",
                    PriceSmall = 7.50m,
                    GramsSmall = 400,
                    PriceBig = 10.90m,
                    GramsBig = 750,
                    CategoryId = 3,
                    isDeleted = false
                },
                new Dish()
                {
                    Id = 2,
                    Name = "Pepperoni",
                    Description = "Tomato sauce, mozzarella and spicy pepperoni.",
                    PriceSmall = 8.90m,
                    GramsSmall = 450,
                    PriceBig = 12.50m,
                    GramsBig = 800,
                    CategoryId = 2,
                    isDeleted = false
                },
                new Dish()
                {
                    Id = 3,
                    Name = "Prosciutto Funghi",
                    Description = "Tomato sauce, mozzarella, ham and fresh mushrooms.",
                    PriceSmall = 9.20m,
                    GramsSmall = 480,
                    PriceBig = 13.20m,
                    GramsBig = 820,
                    CategoryId = 2,
                    isDeleted = false
                },
                new Dish()
                {
                    Id = 4,
                    Name = "Vegetarian",
                    Description = "Tomato sauce, mozzarella, peppers, olives, onions and mushrooms.",
                    PriceSmall = 8.70m,
                    GramsSmall = 460,
                    PriceBig = 12.00m,
                    GramsBig = 780,
                    CategoryId = 3,
                    isDeleted = false
                },
                new Dish()
                {
                    Id = 5,
                    Name = "Caesar Salad",
                    Description = "Iceberg lettuce, grilled chicken, croutons, parmesan and Caesar dressing.",
                    PriceSmall = 7.80m,
                    GramsSmall = 350,
                    PriceBig = null,
                    GramsBig = null,
                    CategoryId = 4,
                    isDeleted = false
                },
                new Dish()
                {
                    Id = 6,
                    Name = "Greek Salad",
                    Description = "Tomatoes, cucumbers, olives, feta cheese and olive oil.",
                    PriceSmall = 6.90m,
                    GramsSmall = 330,
                    PriceBig = null,
                    GramsBig = null,
                    CategoryId = 4,
                    isDeleted = false
                },
                new Dish()
                {
                    Id = 7,
                    Name = "Tiramisu",
                    Description = "Traditional Italian dessert with mascarpone, espresso and cocoa.",
                    PriceSmall = 5.50m,
                    GramsSmall = 180,
                    PriceBig = null,
                    GramsBig = null,
                    CategoryId = 5,
                    isDeleted = false
                }
            );

        }
    }
}
