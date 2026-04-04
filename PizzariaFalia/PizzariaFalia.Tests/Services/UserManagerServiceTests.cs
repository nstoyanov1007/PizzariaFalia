using NUnit.Framework;
using PizzariaFalia.Data;
using PizzariaFalia.Services.Core;
using PizzariaFalia.Tests.Helpers;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Tests.Services
{
    [TestFixture]
    public class UserManagerServiceTests
    {
        private ApplicationDbContext _context = null!;
        private UserManagerService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _context = DbContextFactory.Create();
            _service = new UserManagerService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllUsersAsync_NoUsers_ReturnsEmptyList()
        {
            var result = await _service.GetAllUsersAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllUsersAsync_MultipleUsers_ReturnsAll()
        {
            DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), "alice", "alice@test.com");
            DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), "bob", "bob@test.com");

            var result = await _service.GetAllUsersAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllUsersAsync_MapsIdAndUserNameCorrectly()
        {
            var id = Guid.NewGuid().ToString();
            DbContextFactory.SeedUser(_context, id, "alice", "alice@test.com");

            var result = (await _service.GetAllUsersAsync()).ToList();

            Assert.That(result[0].Id, Is.EqualTo(Guid.Parse(id)));
            Assert.That(result[0].UserName, Is.EqualTo("alice"));
        }

        [Test]
        public async Task GetAllUsersAsync_NullUserName_ReturnsNullLiteral()
        {
            // Identity stores empty string for username in some edge cases; test null fallback
            var id = Guid.NewGuid().ToString();
            var user = DbContextFactory.SeedUser(_context, id);

            user.UserName = null;
            _context.SaveChanges();

            var result = (await _service.GetAllUsersAsync()).ToList();

            Assert.That(result[0].UserName, Is.EqualTo("null"));
        }

        [Test]
        public async Task GetUserDetailsAsync_ExistingUser_ReturnsCorrectDetails()
        {
            var id = Guid.NewGuid();
            DbContextFactory.SeedUser(_context, id.ToString(), "charlie", "charlie@test.com", "44 Test Road");

            var result = await _service.GetUserDetailsAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.UserName, Is.EqualTo("charlie"));
            Assert.That(result.Email, Is.EqualTo("charlie@test.com"));
            Assert.That(result.Address, Is.EqualTo("44 Test Road"));
            Assert.That(result.EmailConfirmed, Is.True);
        }

        [Test]
        public void GetUserDetailsAsync_NonExistentUser_ThrowsInvalidDataException()
        {
            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.GetUserDetailsAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task GetUserDetailsAsync_EmailConfirmedStatus_IsMappedCorrectly()
        {
            var id = Guid.NewGuid();
            var user = DbContextFactory.SeedUser(_context, id.ToString());
            user.EmailConfirmed = false;
            _context.SaveChanges();

            var result = await _service.GetUserDetailsAsync(id);

            Assert.That(result.EmailConfirmed, Is.False);
        }

        [Test]
        public async Task GetUserForEditAsync_ExistingUser_ReturnsEditViewModel()
        {
            var id = Guid.NewGuid();
            DbContextFactory.SeedUser(_context, id.ToString(), "dana", "dana@test.com", "77 Edit Lane");

            var result = await _service.GetUserForEditAsync(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
            Assert.That(result.UserName, Is.EqualTo("dana"));
            Assert.That(result.Email, Is.EqualTo("dana@test.com"));
            Assert.That(result.Address, Is.EqualTo("77 Edit Lane"));
        }

        [Test]
        public void GetUserForEditAsync_NonExistentUser_ThrowsInvalidDataException()
        {
            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.GetUserForEditAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task GetUserForEditAsync_MapsEmailConfirmedFlag()
        {
            var id = Guid.NewGuid();
            var user = DbContextFactory.SeedUser(_context, id.ToString());
            user.EmailConfirmed = false;
            _context.SaveChanges();

            var result = await _service.GetUserForEditAsync(id);

            Assert.That(result.EmailConfirmed, Is.False);
        }

        [Test]
        public async Task EditUserAsync_ValidModel_UpdatesUserInDatabase()
        {
            var id = Guid.NewGuid();
            DbContextFactory.SeedUser(_context, id.ToString(), "olduser", "old@test.com", "Old Address");

            var model = new UserEditViewModel
            {
                Id = id,
                UserName = "newuser",
                Email = "new@test.com",
                Address = "New Address",
                EmailConfirmed = false
            };

            await _service.EditUserAsync(model);

            var user = _context.Users.First(u => u.Id == id.ToString());
            Assert.That(user.UserName, Is.EqualTo("newuser"));
            Assert.That(user.Email, Is.EqualTo("new@test.com"));
            Assert.That(user.Address, Is.EqualTo("New Address"));
            Assert.That(user.EmailConfirmed, Is.False);
        }

        [Test]
        public void EditUserAsync_NonExistentUser_ThrowsInvalidDataException()
        {
            var model = new UserEditViewModel
            {
                Id = Guid.NewGuid(),
                UserName = "ghost",
                Email = "ghost@test.com"
            };

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.EditUserAsync(model));
        }

        [Test]
        public void EditUserAsync_DuplicateUserName_ThrowsInvalidDataException()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            DbContextFactory.SeedUser(_context, id1.ToString(), "alice", "alice@test.com");
            DbContextFactory.SeedUser(_context, id2.ToString(), "bob", "bob@test.com");

            var model = new UserEditViewModel
            {
                Id = id2,
                UserName = "alice",     // already taken
                Email = "unique@test.com"
            };

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.EditUserAsync(model));
        }

        [Test]
        public void EditUserAsync_DuplicateEmail_ThrowsInvalidDataException()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            DbContextFactory.SeedUser(_context, id1.ToString(), "alice", "alice@test.com");
            DbContextFactory.SeedUser(_context, id2.ToString(), "bob", "bob@test.com");

            var model = new UserEditViewModel
            {
                Id = id2,
                UserName = "uniqueuser",
                Email = "alice@test.com"  // already taken
            };

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.EditUserAsync(model));
        }

        [Test]
        public async Task EditUserAsync_NullAddress_SetsDefaultAddress()
        {
            var id = Guid.NewGuid();
            DbContextFactory.SeedUser(_context, id.ToString(), "user1", "user1@test.com");

            var model = new UserEditViewModel
            {
                Id = id,
                UserName = "newuser1",
                Email = "new1@test.com",
                Address = null,
                EmailConfirmed = true
            };

            await _service.EditUserAsync(model);

            var user = _context.Users.First(u => u.Id == id.ToString());
            Assert.That(user.Address, Is.EqualTo("no address"));
        }

        [Test]
        public async Task GetUsersPagedAsync_FirstPage_ReturnsCorrectSlice()
        {
            for (int i = 1; i <= 5; i++)
                DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), $"user{i:D2}", $"user{i}@test.com");

            var result = await _service.GetUsersPagedAsync(1, 3);

            Assert.That(result.Items.Count(), Is.EqualTo(3));
            Assert.That(result.PageIndex, Is.EqualTo(1));
        }

        [Test]
        public async Task GetUsersPagedAsync_SecondPage_ReturnsRemainingUsers()
        {
            for (int i = 1; i <= 5; i++)
                DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), $"user{i:D2}", $"user{i}@test.com");

            var result = await _service.GetUsersPagedAsync(2, 3);

            Assert.That(result.Items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetUsersPagedAsync_CalculatesTotalPagesCorrectly()
        {
            for (int i = 1; i <= 7; i++)
                DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), $"user{i:D2}", $"user{i}@test.com");

            var result = await _service.GetUsersPagedAsync(1, 3);

            Assert.That(result.TotalPages, Is.EqualTo(3));
        }

        [Test]
        public async Task GetUsersPagedAsync_HasPreviousAndHasNext_AreCorrect()
        {
            for (int i = 1; i <= 6; i++)
                DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), $"user{i:D2}", $"user{i}@test.com");

            var firstPage = await _service.GetUsersPagedAsync(1, 3);
            var secondPage = await _service.GetUsersPagedAsync(2, 3);

            Assert.That(firstPage.HasPrevious, Is.False);
            Assert.That(firstPage.HasNext, Is.True);
            Assert.That(secondPage.HasPrevious, Is.True);
            Assert.That(secondPage.HasNext, Is.False);
        }

        [Test]
        public async Task GetUsersPagedAsync_EmptyDatabase_ReturnsEmptyFirstPage()
        {
            var result = await _service.GetUsersPagedAsync(1, 10);

            Assert.That(result.Items, Is.Empty);
            Assert.That(result.TotalPages, Is.EqualTo(0));
        }

        [Test]
        public async Task GetUsersPagedAsync_ResultsAreOrderedByUserName()
        {
            DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), "charlie", "charlie@test.com");
            DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), "alice", "alice@test.com");
            DbContextFactory.SeedUser(_context, Guid.NewGuid().ToString(), "bob", "bob@test.com");

            var result = await _service.GetUsersPagedAsync(1, 10);
            var userNames = result.Items.Select(u => u.UserName).ToList();

            Assert.That(userNames, Is.EqualTo(userNames.OrderBy(n => n).ToList()));
        }
    }
}
