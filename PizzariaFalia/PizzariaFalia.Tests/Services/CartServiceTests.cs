using NUnit.Framework;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.Services.Core;
using PizzariaFalia.Tests.Helpers;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Tests.Services
{
    [TestFixture]
    public class CartServiceTests
    {
        private ApplicationDbContext _context = null!;
        private CartService _service = null!;

        private const string UserId = "user-001";

        [SetUp]
        public void SetUp()
        {
            _context = DbContextFactory.Create();
            _service = new CartService(_context);
            DbContextFactory.SeedUser(_context, UserId);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateCartOrderAsync_NoExistingPendingOrder_CreatesNewOrder()
        {
            await _service.CreateCartOrderAsync(UserId);

            var order = _context.Orders.SingleOrDefault(o => o.UserId == UserId && o.Status == Status.Pending);
            Assert.That(order, Is.Not.Null);
        }

        [Test]
        public async Task CreateCartOrderAsync_AlreadyHasPendingOrder_DoesNotCreateDuplicate()
        {
            await _service.CreateCartOrderAsync(UserId);
            await _service.CreateCartOrderAsync(UserId); // second call should be a no-op

            int count = _context.Orders.Count(o => o.UserId == UserId && o.Status == Status.Pending);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateCartOrderAsync_SetsStatusToPending()
        {
            await _service.CreateCartOrderAsync(UserId);

            var order = _context.Orders.First(o => o.UserId == UserId);
            Assert.That(order.Status, Is.EqualTo(Status.Pending));
        }

        [Test]
        public async Task PlaceCartOrderAsync_PendingOrderExists_ChangesStatusToOrdered()
        {
            await _service.CreateCartOrderAsync(UserId);

            await _service.PlaceCartOrderAsync(UserId);

            var order = _context.Orders.First(o => o.UserId == UserId);
            Assert.That(order.Status, Is.EqualTo(Status.Ordered));
        }

        [Test]
        public void PlaceCartOrderAsync_NoPendingOrder_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.PlaceCartOrderAsync(UserId));
        }

        [Test]
        public void PlaceCartOrderAsync_NullOrEmptyUserId_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.PlaceCartOrderAsync(""));

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.PlaceCartOrderAsync("   "));
        }

        [Test]
        public async Task AddItemToCartAsync_DishDetails_AddsItemToExistingCart()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id);
            await _service.CreateCartOrderAsync(UserId);

            var vm = new DishDetailsViewModel
            {
                Id = dish.Id,
                IsBig = false,
                Name = dish.Name
            };

            await _service.AddItemToCartAsync(vm, UserId);

            var order = _context.Orders.First(o => o.UserId == UserId && o.Status == Status.Pending);
            int itemCount = _context.OrderItems.Count(oi => oi.OrderId == order.Id);
            Assert.That(itemCount, Is.EqualTo(1));
        }

        [Test]
        public async Task AddItemToCartAsync_DishDetails_CreatesCartIfNoneExists()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id);

            var vm = new DishDetailsViewModel
            {
                Id = dish.Id,
                IsBig = true,
                Name = dish.Name
            };

            await _service.AddItemToCartAsync(vm, UserId);

            var order = _context.Orders.FirstOrDefault(o => o.UserId == UserId && o.Status == Status.Pending);
            Assert.That(order, Is.Not.Null);
            Assert.That(_context.OrderItems.Any(oi => oi.OrderId == order!.Id), Is.True);
        }

        [Test]
        public void AddItemToCartAsync_DishDetails_NullItem_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.AddItemToCartAsync((DishDetailsViewModel)null!, UserId));
        }

        [Test]
        public void AddItemToCartAsync_DishDetails_EmptyUserId_ThrowsArgumentException()
        {
            var vm = new DishDetailsViewModel { Id = 1 };

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.AddItemToCartAsync(vm, ""));
        }

        [Test]
        public async Task AddItemToCartAsync_DishDetails_StoresCorrectBigFlag()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id);

            var vm = new DishDetailsViewModel { Id = dish.Id, IsBig = true };
            await _service.AddItemToCartAsync(vm, UserId);

            var order = _context.Orders.First(o => o.UserId == UserId && o.Status == Status.Pending);
            var item = _context.OrderItems.First(oi => oi.OrderId == order.Id);
            Assert.That(item.IsDishBig, Is.True);
        }

        [Test]
        public async Task AddItemToCartAsync_DishIndex_AddsItemToExistingCart()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pasta");
            var dish = DbContextFactory.SeedDish(_context, category.Id, name: "Carbonara", description: "Egg and bacon pasta");
            await _service.CreateCartOrderAsync(UserId);

            var vm = new DishIndexViewModel
            {
                Id = dish.Id,
                IsBig = false,
                Name = dish.Name
            };

            await _service.AddItemToCartAsync(vm, UserId);

            var order = _context.Orders.First(o => o.UserId == UserId && o.Status == Status.Pending);
            Assert.That(_context.OrderItems.Any(oi => oi.OrderId == order.Id && oi.DishId == dish.Id), Is.True);
        }

        [Test]
        public async Task AddItemToCartAsync_DishIndex_CreatesCartIfNoneExists()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pasta");
            var dish = DbContextFactory.SeedDish(_context, category.Id, name: "Bolognese", description: "Meat sauce pasta");

            var vm = new DishIndexViewModel { Id = dish.Id, IsBig = false };

            await _service.AddItemToCartAsync(vm, UserId);

            Assert.That(_context.Orders.Any(o => o.UserId == UserId && o.Status == Status.Pending), Is.True);
        }

        [Test]
        public async Task RemoveItemFromCartAsync_ExistingItem_RemovesItFromDatabase()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id);
            await _service.CreateCartOrderAsync(UserId);
            var vm = new DishIndexViewModel { Id = dish.Id, IsBig = false };
            await _service.AddItemToCartAsync(vm, UserId);

            var order = _context.Orders.First(o => o.UserId == UserId && o.Status == Status.Pending);
            var orderItem = _context.OrderItems.First(oi => oi.OrderId == order.Id);

            var removeVm = new OrderItemViewModel { Id = orderItem.Id };
            await _service.RemoveItemFromCartAsync(removeVm, UserId);

            Assert.That(_context.OrderItems.Any(oi => oi.Id == orderItem.Id), Is.False);
        }

        [Test]
        public async Task RemoveItemFromCartAsync_NonExistentItem_DoesNotThrow()
        {
            var removeVm = new OrderItemViewModel { Id = 99999 };

            Assert.DoesNotThrowAsync(async () =>
                await _service.RemoveItemFromCartAsync(removeVm, UserId));
        }

        [Test]
        public void RemoveItemFromCartAsync_NullItem_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.RemoveItemFromCartAsync(null!, UserId));
        }

        [Test]
        public void RemoveItemFromCartAsync_EmptyUserId_ThrowsArgumentException()
        {
            var vm = new OrderItemViewModel { Id = 1 };

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.RemoveItemFromCartAsync(vm, ""));
        }


        [Test]
        public async Task GetCartItemsAsync_NoPendingOrder_ReturnsEmptyList()
        {
            var result = await _service.GetCartItemsAsync(UserId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetCartItemsAsync_WithItems_ReturnsCorrectCount()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish1 = DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");
            var dish2 = DbContextFactory.SeedDish(_context, category.Id, "Diavola", "Spicy pizza");
            await _service.CreateCartOrderAsync(UserId);

            var vmA = new DishIndexViewModel { Id = dish1.Id, IsBig = false };
            var vmB = new DishIndexViewModel { Id = dish2.Id, IsBig = true };
            await _service.AddItemToCartAsync(vmA, UserId);
            await _service.AddItemToCartAsync(vmB, UserId);

            var items = await _service.GetCartItemsAsync(UserId);
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetCartItemsAsync_WithBigItem_ReturnsCorrectPriceAndGrams()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id,
                priceSmall: 8.99m, gramSmall: 300,
                priceBig: 12.99m, gramBig: 500);
            await _service.CreateCartOrderAsync(UserId);

            var vm = new DishIndexViewModel { Id = dish.Id, IsBig = true };
            await _service.AddItemToCartAsync(vm, UserId);

            var items = (await _service.GetCartItemsAsync(UserId)).ToList();
            Assert.That(items[0].Price, Is.EqualTo(12.99m));
            Assert.That(items[0].Grams, Is.EqualTo(500));
        }

        [Test]
        public async Task GetCartItemsAsync_WithSmallItem_ReturnsCorrectPriceAndGrams()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id,
                priceSmall: 8.99m, gramSmall: 300,
                priceBig: 12.99m, gramBig: 500);
            await _service.CreateCartOrderAsync(UserId);

            var vm = new DishIndexViewModel { Id = dish.Id, IsBig = false };
            await _service.AddItemToCartAsync(vm, UserId);

            var items = (await _service.GetCartItemsAsync(UserId)).ToList();
            Assert.That(items[0].Price, Is.EqualTo(8.99m));
            Assert.That(items[0].Grams, Is.EqualTo(300));
        }


        [Test]
        public async Task GetDishDetailsAsync_ExistingDish_ReturnsCorrectViewModel()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");

            var result = await _service.GetDishDetailsAsync(dish.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(dish.Id));
            Assert.That(result.Name, Is.EqualTo("Margherita"));
            Assert.That(result.CategoryName, Is.EqualTo("Pizza"));
        }

        [Test]
        public void GetDishDetailsAsync_NonExistentDish_ThrowsException()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _service.GetDishDetailsAsync(99999));
        }
    }
}
