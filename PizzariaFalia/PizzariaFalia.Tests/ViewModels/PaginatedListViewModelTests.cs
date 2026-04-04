using NUnit.Framework;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Tests.ViewModels
{
    [TestFixture]
    public class PaginatedListViewModelTests
    {
        [Test]
        public void HasPrevious_PageIndexGreaterThanOne_ReturnsTrue()
        {
            var vm = new PaginatedListViewModel<string> { PageIndex = 2, TotalPages = 5 };
            Assert.That(vm.HasPrevious, Is.True);
        }

        [Test]
        public void HasPrevious_PageIndexIsOne_ReturnsFalse()
        {
            var vm = new PaginatedListViewModel<string> { PageIndex = 1, TotalPages = 5 };
            Assert.That(vm.HasPrevious, Is.False);
        }

        [Test]
        public void HasNext_PageIndexLessThanTotalPages_ReturnsTrue()
        {
            var vm = new PaginatedListViewModel<string> { PageIndex = 2, TotalPages = 5 };
            Assert.That(vm.HasNext, Is.True);
        }

        [Test]
        public void HasNext_PageIndexEqualsTotalPages_ReturnsFalse()
        {
            var vm = new PaginatedListViewModel<string> { PageIndex = 5, TotalPages = 5 };
            Assert.That(vm.HasNext, Is.False);
        }

        [Test]
        public void HasNext_SinglePage_ReturnsFalse()
        {
            var vm = new PaginatedListViewModel<string> { PageIndex = 1, TotalPages = 1 };
            Assert.That(vm.HasNext, Is.False);
        }

        [Test]
        public void Items_DefaultsToEmptyEnumerable()
        {
            var vm = new PaginatedListViewModel<int>();
            Assert.That(vm.Items, Is.Not.Null);
            Assert.That(vm.Items, Is.Empty);
        }

        [Test]
        public void Items_CanBeSetToAList()
        {
            var vm = new PaginatedListViewModel<string>
            {
                Items = new List<string> { "a", "b", "c" }
            };
            Assert.That(vm.Items.Count(), Is.EqualTo(3));
        }

        [Test]
        public void PageIndex_And_TotalPages_DefaultToZero()
        {
            var vm = new PaginatedListViewModel<object>();
            Assert.That(vm.PageIndex, Is.EqualTo(0));
            Assert.That(vm.TotalPages, Is.EqualTo(0));
        }
    }
}
