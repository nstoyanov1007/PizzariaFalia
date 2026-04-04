using NUnit.Framework;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Services.Core;
using PizzariaFalia.Tests.Helpers;

namespace PizzariaFalia.Tests.Services
{
    [TestFixture]
    public class MenuServiceTests
    {
        private ApplicationDbContext _context = null!;
        private MenuService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _context = DbContextFactory.Create();
            _service = new MenuService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllCategoriesAsync_NoCategories_ReturnsEmptyList()
        {
            var result = await _service.GetAllCategoriesAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllCategoriesAsync_OnlyRootCategories_ReturnsAll()
        {
            DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedCategory(_context, "Pasta");

            var result = await _service.GetAllCategoriesAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllCategoriesAsync_DeletedCategories_AreExcluded()
        {
            DbContextFactory.SeedCategory(_context, "Active");
            DbContextFactory.SeedCategory(_context, "Deleted", isDeleted: true);

            var result = await _service.GetAllCategoriesAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Active"));
        }

        [Test]
        public async Task GetAllCategoriesAsync_SubCategories_AreNestedUnderParent()
        {
            var parent = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedCategory(_context, "Meat Pizzas", parentCategoryId: parent.Id);
            DbContextFactory.SeedCategory(_context, "Veg Pizzas", parentCategoryId: parent.Id);

            var result = (await _service.GetAllCategoriesAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Children.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllCategoriesAsync_DeletedSubCategories_AreExcludedFromChildren()
        {
            var parent = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedCategory(_context, "Visible Sub", parentCategoryId: parent.Id);
            DbContextFactory.SeedCategory(_context, "Deleted Sub", parentCategoryId: parent.Id, isDeleted: true);

            var result = (await _service.GetAllCategoriesAsync()).ToList();

            Assert.That(result[0].Children.Count, Is.EqualTo(1));
            Assert.That(result[0].Children.First().Name, Is.EqualTo("Visible Sub"));
        }

        [Test]
        public async Task GetAllCategoriesAsync_SubCategoriesDoNotAppearAsRoots()
        {
            var parent = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedCategory(_context, "Sub", parentCategoryId: parent.Id);

            var result = await _service.GetAllCategoriesAsync();

            // Only root should appear at top level
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllDishesIndexAsync_NoDishes_ReturnsEmptyList()
        {
            var result = await _service.GetAllDishesIndexAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllDishesIndexAsync_ActiveDishes_ReturnsAll()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");
            DbContextFactory.SeedDish(_context, category.Id, "Diavola", "Spicy pizza");

            var result = await _service.GetAllDishesIndexAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllDishesIndexAsync_DeletedDishes_AreExcluded()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedDish(_context, category.Id, "Active", "Active dish");
            DbContextFactory.SeedDish(_context, category.Id, "Deleted", "Deleted dish", isDeleted: true);

            var result = await _service.GetAllDishesIndexAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Active"));
        }

        [Test]
        public async Task GetAllDishesIndexAsync_MapsFieldsCorrectly()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedDish(_context, category.Id,
                name: "Margherita",
                description: "Classic pizza",
                priceSmall: 8.99m,
                gramSmall: 300,
                priceBig: 13.99m,
                gramBig: 550);

            var result = (await _service.GetAllDishesIndexAsync()).ToList();

            Assert.That(result[0].Name, Is.EqualTo("Margherita"));
            Assert.That(result[0].PriceSmall, Is.EqualTo(8.99m));
            Assert.That(result[0].GramsSmall, Is.EqualTo(300));
            Assert.That(result[0].PriceBig, Is.EqualTo(13.99m));
            Assert.That(result[0].GramsBig, Is.EqualTo(550));
        }

        [Test]
        public async Task GetDishesIndexByCategoryAsync_CategoryWithDishes_ReturnsThem()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");

            var result = await _service.GetDishesIndexByCategoryAsync(category.Id);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetDishesIndexByCategoryAsync_IncludesSubCategoryDishes()
        {
            var parent = DbContextFactory.SeedCategory(_context, "Pizza");
            var sub = DbContextFactory.SeedCategory(_context, "Meat Pizzas", parentCategoryId: parent.Id);
            DbContextFactory.SeedDish(_context, parent.Id, "Margherita", "Classic pizza");
            DbContextFactory.SeedDish(_context, sub.Id, "Pepperoni", "Spicy meat pizza");

            var result = await _service.GetDishesIndexByCategoryAsync(parent.Id);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetDishesIndexByCategoryAsync_ExcludesDeletedDishes()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            DbContextFactory.SeedDish(_context, category.Id, "Active", "Active dish");
            DbContextFactory.SeedDish(_context, category.Id, "Deleted", "Deleted dish", isDeleted: true);

            var result = await _service.GetDishesIndexByCategoryAsync(category.Id);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetDishesIndexByCategoryAsync_NonExistentCategory_ReturnsEmptyList()
        {
            var result = await _service.GetDishesIndexByCategoryAsync(99999);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetDishesIndexByCategoryAsync_OnlyReturnsDishesForRequestedCategory()
        {
            var cat1 = DbContextFactory.SeedCategory(_context, "Pizza");
            var cat2 = DbContextFactory.SeedCategory(_context, "Pasta");
            DbContextFactory.SeedDish(_context, cat1.Id, "Margherita", "Classic pizza");
            DbContextFactory.SeedDish(_context, cat2.Id, "Carbonara", "Egg and bacon pasta");

            var result = await _service.GetDishesIndexByCategoryAsync(cat1.Id);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Margherita"));
        }
        [Test]
        public async Task GetDishDetailsAsync_ExistingDish_ReturnsCorrectDetails()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");

            var result = await _service.GetDishDetailsAsync(dish.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(dish.Id));
            Assert.That(result.Name, Is.EqualTo("Margherita"));
            Assert.That(result.CategoryName, Is.EqualTo("Pizza"));
            Assert.That(result.CategoryId, Is.EqualTo(category.Id));
        }

        [Test]
        public void GetDishDetailsAsync_NonExistentDish_ThrowsInvalidDataException()
        {
            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.GetDishDetailsAsync(99999));
        }

        [Test]
        public void GetDishDetailsAsync_DeletedDish_ThrowsInvalidDataException()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id, isDeleted: true);

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.GetDishDetailsAsync(dish.Id));
        }

        [Test]
        public async Task GetDishDetailsAsync_MapsDescriptionAndPricesCorrectly()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id,
                description: "Detailed description here",
                priceSmall: 7.50m,
                gramSmall: 280,
                priceBig: 11.00m,
                gramBig: 480);

            var result = await _service.GetDishDetailsAsync(dish.Id);

            Assert.That(result.Description, Is.EqualTo("Detailed description here"));
            Assert.That(result.PriceSmall, Is.EqualTo(7.50m));
            Assert.That(result.GramsSmall, Is.EqualTo(280));
            Assert.That(result.PriceBig, Is.EqualTo(11.00m));
            Assert.That(result.GramsBig, Is.EqualTo(480));
        }
    }
}
