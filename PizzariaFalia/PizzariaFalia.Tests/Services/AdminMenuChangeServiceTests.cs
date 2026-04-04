using NUnit.Framework;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Services.Core;
using PizzariaFalia.Tests.Helpers;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Tests.Services
{
    [TestFixture]
    public class AdminMenuChangeServiceTests
    {
        private ApplicationDbContext _context = null!;
        private AdminMenuChangeService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _context = DbContextFactory.Create();
            _service = new AdminMenuChangeService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // ── CreateCategoryAsync ──────────────────────────────────────────────

        [Test]
        public async Task CreateCategoryAsync_ValidViewModel_PersistsCategoryToDatabase()
        {
            var vm = new CategoryFormViewModel
            {
                Name = "Pizza",
                DisplayName = "Our Pizzas",
                ParentCategoryId = null
            };

            await _service.CreateCategoryAsync(vm);

            var category = _context.Categories.SingleOrDefault(c => c.Name == "Pizza");
            Assert.That(category, Is.Not.Null);
            Assert.That(category!.DisplayName, Is.EqualTo("Our Pizzas"));
        }

        [Test]
        public async Task CreateCategoryAsync_WithParentCategory_SetsParentCategoryId()
        {
            var parent = DbContextFactory.SeedCategory(_context, "Pizza");

            var vm = new CategoryFormViewModel
            {
                Name = "Meat Pizzas",
                ParentCategoryId = parent.Id
            };

            await _service.CreateCategoryAsync(vm);

            var sub = _context.Categories.SingleOrDefault(c => c.Name == "Meat Pizzas");
            Assert.That(sub, Is.Not.Null);
            Assert.That(sub!.ParentCategoryId, Is.EqualTo(parent.Id));
        }

        [Test]
        public async Task CreateCategoryAsync_MultipleCategories_AllArePersisted()
        {
            var vm1 = new CategoryFormViewModel { Name = "Pizza" };
            var vm2 = new CategoryFormViewModel { Name = "Pasta" };

            await _service.CreateCategoryAsync(vm1);
            await _service.CreateCategoryAsync(vm2);

            Assert.That(_context.Categories.Count(), Is.EqualTo(2));
        }

        // ── EditCategoryAsync ────────────────────────────────────────────────

        [Test]
        public async Task EditCategoryAsync_ExistingCategory_UpdatesAllFields()
        {
            var category = DbContextFactory.SeedCategory(_context, "OldName", "OldDisplay");

            var vm = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = "NewName",
                DisplayName = "NewDisplay",
                ParentCategoryId = null
            };

            await _service.EditCategoryAsync(vm);

            var updated = _context.Categories.First(c => c.Id == category.Id);
            Assert.That(updated.Name, Is.EqualTo("NewName"));
            Assert.That(updated.DisplayName, Is.EqualTo("NewDisplay"));
        }

        [Test]
        public void EditCategoryAsync_NonExistentCategory_ThrowsArgumentException()
        {
            var vm = new CategoryFormViewModel
            {
                Id = 99999,
                Name = "Ghost"
            };

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.EditCategoryAsync(vm));
        }

        [Test]
        public async Task EditCategoryAsync_UpdatesParentCategoryId()
        {
            var parent = DbContextFactory.SeedCategory(_context, "Parent");
            var child = DbContextFactory.SeedCategory(_context, "Child");

            var vm = new CategoryFormViewModel
            {
                Id = child.Id,
                Name = child.Name,
                ParentCategoryId = parent.Id
            };

            await _service.EditCategoryAsync(vm);

            var updated = _context.Categories.First(c => c.Id == child.Id);
            Assert.That(updated.ParentCategoryId, Is.EqualTo(parent.Id));
        }

        // ── DeleteCategoryAsync ──────────────────────────────────────────────

        [Test]
        public async Task DeleteCategoryAsync_ExistingCategory_SetsIsDeletedTrue()
        {
            var category = DbContextFactory.SeedCategory(_context, "ToDelete");

            await _service.DeleteCategoryAsync(category.Id);

            var deleted = _context.Categories.First(c => c.Id == category.Id);
            Assert.That(deleted.isDeleted, Is.True);
        }

        [Test]
        public void DeleteCategoryAsync_NonExistentCategory_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.DeleteCategoryAsync(99999));
        }

        [Test]
        public async Task DeleteCategoryAsync_DoesNotRemoveRecord_OnlySoftDeletes()
        {
            var category = DbContextFactory.SeedCategory(_context, "SoftDelete");
            int idBefore = category.Id;

            await _service.DeleteCategoryAsync(category.Id);

            Assert.That(_context.Categories.Any(c => c.Id == idBefore), Is.True);
        }

        // ── CreateDishAsync ──────────────────────────────────────────────────

        [Test]
        public async Task CreateDishAsync_ValidViewModel_PersistsDishToDatabase()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");

            var vm = new DishFormViewModel
            {
                CategoryId = category.Id,
                Name = "Margherita",
                Description = "Classic tomato and mozzarella pizza",
                PriceSmall = 8.99m,
                GramsSmall = 300,
                PriceBig = 12.99m,
                GramsBig = 500
            };

            await _service.CreateDishAsync(vm);

            var dish = _context.Dishes.SingleOrDefault(d => d.Name == "Margherita");
            Assert.That(dish, Is.Not.Null);
            Assert.That(dish!.PriceSmall, Is.EqualTo(8.99m));
            Assert.That(dish.CategoryId, Is.EqualTo(category.Id));
        }

        [Test]
        public async Task CreateDishAsync_WithoutBigSize_PersistesWithNullBigFields()
        {
            var category = DbContextFactory.SeedCategory(_context, "Salads");

            var vm = new DishFormViewModel
            {
                CategoryId = category.Id,
                Name = "Caesar Salad",
                Description = "Romaine lettuce and parmesan salad",
                PriceSmall = 6.50m,
                GramsSmall = 250,
                PriceBig = null,
                GramsBig = null
            };

            await _service.CreateDishAsync(vm);

            var dish = _context.Dishes.SingleOrDefault(d => d.Name == "Caesar Salad");
            Assert.That(dish, Is.Not.Null);
            Assert.That(dish!.PriceBig, Is.Null);
            Assert.That(dish.GramsBig, Is.Null);
        }

        // ── EditDishAsync ────────────────────────────────────────────────────

        [Test]
        public async Task EditDishAsync_ExistingDish_UpdatesAllFields()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id, "OldName", "Old desc");

            var vm = new DishFormViewModel
            {
                Id = dish.Id,
                CategoryId = category.Id,
                Name = "NewName",
                Description = "New description for this dish",
                PriceSmall = 9.99m,
                GramsSmall = 350,
                PriceBig = 14.99m,
                GramsBig = 600
            };

            await _service.EditDishAsync(vm);

            var updated = _context.Dishes.First(d => d.Id == dish.Id);
            Assert.That(updated.Name, Is.EqualTo("NewName"));
            Assert.That(updated.PriceSmall, Is.EqualTo(9.99m));
            Assert.That(updated.GramsSmall, Is.EqualTo(350));
        }

        [Test]
        public void EditDishAsync_NonExistentDish_ThrowsArgumentException()
        {
            var vm = new DishFormViewModel
            {
                Id = 99999,
                Name = "Ghost Dish",
                Description = "Does not exist"
            };

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.EditDishAsync(vm));
        }

        // ── DeleteDishAsync ──────────────────────────────────────────────────

        [Test]
        public async Task DeleteDishAsync_ExistingDish_SetsIsDeletedTrue()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id);

            await _service.DeleteDishAsync(dish.Id);

            var deleted = _context.Dishes.First(d => d.Id == dish.Id);
            Assert.That(deleted.isDeleted, Is.True);
        }

        [Test]
        public void DeleteDishAsync_NonExistentDish_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.DeleteDishAsync(99999));
        }

        [Test]
        public async Task DeleteDishAsync_DoesNotRemoveRecord_OnlySoftDeletes()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id);
            int dishId = dish.Id;

            await _service.DeleteDishAsync(dish.Id);

            Assert.That(_context.Dishes.Any(d => d.Id == dishId), Is.True);
        }
    }
}
