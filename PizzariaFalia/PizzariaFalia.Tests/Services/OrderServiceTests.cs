using NUnit.Framework;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.Services.Core;
using PizzariaFalia.Tests.Helpers;

namespace PizzariaFalia.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private ApplicationDbContext _context = null!;
        private OrderService _service = null!;

        private const string UserId = "order-user-001";

        [SetUp]
        public void SetUp()
        {
            _context = DbContextFactory.Create();
            _service = new OrderService(_context);
            DbContextFactory.SeedUser(_context, UserId);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // ── ChangeOrderStatusAsync ───────────────────────────────────────────

        [Test]
        public async Task ChangeOrderStatusAsync_ExistingOrder_UpdatesStatus()
        {
            var order = SeedOrder(Status.Ordered);

            await _service.ChangeOrderStatusAsync(order.Id, Status.Delivered);

            var updated = _context.Orders.First(o => o.Id == order.Id);
            Assert.That(updated.Status, Is.EqualTo(Status.Delivered));
        }

        [Test]
        public async Task ChangeOrderStatusAsync_PendingToOrdered_UpdatesCorrectly()
        {
            var order = SeedOrder(Status.Pending);

            await _service.ChangeOrderStatusAsync(order.Id, Status.Ordered);

            var updated = _context.Orders.First(o => o.Id == order.Id);
            Assert.That(updated.Status, Is.EqualTo(Status.Ordered));
        }

        [Test]
        public async Task ChangeOrderStatusAsync_OrderedToCancelled_UpdatesCorrectly()
        {
            var order = SeedOrder(Status.Ordered);

            await _service.ChangeOrderStatusAsync(order.Id, Status.Cancelled);

            var updated = _context.Orders.First(o => o.Id == order.Id);
            Assert.That(updated.Status, Is.EqualTo(Status.Cancelled));
        }

        [Test]
        public void ChangeOrderStatusAsync_NonExistentOrder_ThrowsException()
        {
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.ChangeOrderStatusAsync(99999, Status.Delivered));
        }

        [Test]
        public async Task ChangeOrderStatusAsync_DoesNotAffectOtherOrders()
        {
            var order1 = SeedOrder(Status.Ordered);
            var order2 = SeedOrder(Status.Ordered, "order-user-002");

            await _service.ChangeOrderStatusAsync(order1.Id, Status.Delivered);

            var other = _context.Orders.First(o => o.Id == order2.Id);
            Assert.That(other.Status, Is.EqualTo(Status.Ordered));
        }

        // ── GetAllOrdersAsync ────────────────────────────────────────────────

        [Test]
        public async Task GetAllOrdersAsync_NoOrders_ReturnsEmptyList()
        {
            var result = await _service.GetAllOrdersAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllOrdersAsync_MultipleOrders_ReturnsAll()
        {
            SeedOrder(Status.Pending);
            SeedOrder(Status.Ordered, "order-user-002");
            DbContextFactory.SeedUser(_context, "order-user-002");

            var result = await _service.GetAllOrdersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllOrdersAsync_MapsFieldsCorrectly()
        {
            var order = SeedOrder(Status.Ordered);

            var result = (await _service.GetAllOrdersAsync()).ToList();

            Assert.That(result[0].Id, Is.EqualTo(order.Id));
            Assert.That(result[0].UserId, Is.EqualTo(UserId));
            Assert.That(result[0].Status, Is.EqualTo(Status.Ordered));
            Assert.That(result[0].CreatedAt, Is.EqualTo(order.CreatedAt));
        }

        // ── GetOrderDetailsAsync ─────────────────────────────────────────────

        [Test]
        public async Task GetOrderDetailsAsync_ExistingOrder_ReturnsCorrectViewModel()
        {
            var order = SeedOrder(Status.Ordered);

            var result = await _service.GetOrderDetailsAsync(order.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(UserId));
            Assert.That(result.Status, Is.EqualTo(Status.Ordered));
        }

        [Test]
        public void GetOrderDetailsAsync_NonExistentOrder_ThrowsException()
        {
            Assert.ThrowsAsync<Exception>(async () =>
                await _service.GetOrderDetailsAsync(99999));
        }

        [Test]
        public async Task GetOrderDetailsAsync_WithItems_ItemsListIsPopulated()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");
            var order = SeedOrder(Status.Ordered);

            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                IsDishBig = false
            });
            _context.SaveChanges();

            var result = await _service.GetOrderDetailsAsync(order.Id);

            Assert.That(result.Items, Is.Not.Null);
            Assert.That(result.Items.Count, Is.EqualTo(1));
        }

        // ── GetOrderItemsAsync ───────────────────────────────────────────────

        [Test]
        public async Task GetOrderItemsAsync_OrderWithNoItems_ReturnsEmptyList()
        {
            var order = SeedOrder(Status.Ordered);

            var result = await _service.GetOrderItemsAsync(order.Id);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetOrderItemsAsync_OrderWithItems_ReturnsCorrectCount()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish1 = DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");
            var dish2 = DbContextFactory.SeedDish(_context, category.Id, "Pepperoni", "Spicy pizza");
            var order = SeedOrder(Status.Ordered);

            _context.OrderItems.AddRange(
                new OrderItem { OrderId = order.Id, DishId = dish1.Id, IsDishBig = false },
                new OrderItem { OrderId = order.Id, DishId = dish2.Id, IsDishBig = true }
            );
            _context.SaveChanges();

            var result = await _service.GetOrderItemsAsync(order.Id);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrderItemsAsync_BigItem_ReturnsCorrectPriceAndGrams()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id,
                priceSmall: 8.99m, gramSmall: 300,
                priceBig: 13.99m, gramBig: 550);
            var order = SeedOrder(Status.Ordered);

            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                IsDishBig = true
            });
            _context.SaveChanges();

            var result = (await _service.GetOrderItemsAsync(order.Id)).ToList();

            Assert.That(result[0].Price, Is.EqualTo(13.99m));
            Assert.That(result[0].Grams, Is.EqualTo(550));
            Assert.That(result[0].IsBig, Is.True);
        }

        [Test]
        public async Task GetOrderItemsAsync_SmallItem_ReturnsCorrectPriceAndGrams()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id,
                priceSmall: 8.99m, gramSmall: 300,
                priceBig: 13.99m, gramBig: 550);
            var order = SeedOrder(Status.Ordered);

            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                IsDishBig = false
            });
            _context.SaveChanges();

            var result = (await _service.GetOrderItemsAsync(order.Id)).ToList();

            Assert.That(result[0].Price, Is.EqualTo(8.99m));
            Assert.That(result[0].Grams, Is.EqualTo(300));
            Assert.That(result[0].IsBig, Is.False);
        }

        [Test]
        public async Task GetOrderItemsAsync_MapsAllRequiredFields()
        {
            var category = DbContextFactory.SeedCategory(_context, "Pizza");
            var dish = DbContextFactory.SeedDish(_context, category.Id, "Margherita", "Classic pizza");
            var order = SeedOrder(Status.Ordered);

            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                IsDishBig = false
            });
            _context.SaveChanges();

            var result = (await _service.GetOrderItemsAsync(order.Id)).ToList();

            Assert.That(result[0].DishId, Is.EqualTo(dish.Id));
            Assert.That(result[0].DishName, Is.EqualTo("Margherita"));
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private Order SeedOrder(Status status, string? userId = null)
        {
            var uid = userId ?? UserId;
            if (userId != null && !_context.Users.Any(u => u.Id == uid))
                DbContextFactory.SeedUser(_context, uid, userName: uid);

            var order = new Order
            {
                UserId = uid,
                Status = status,
                CreatedAt = DateTime.Now
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            return order;
        }
    }
}
