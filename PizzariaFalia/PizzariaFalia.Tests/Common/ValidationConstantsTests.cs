using NUnit.Framework;
using PizzariaFalia.Common;

namespace PizzariaFalia.Tests.Common
{
    [TestFixture]
    public class ValidationConstantsTests
    {
        [Test]
        public void DishNameMaxLength_IsPositive()
        {
            Assert.That(ValidationConstants.DishNameMaxLength, Is.GreaterThan(0));
        }

        [Test]
        public void DishNameMinLength_IsLessThanMaxLength()
        {
            Assert.That(ValidationConstants.DishNameMinLength, Is.LessThan(ValidationConstants.DishNameMaxLength));
        }

        [Test]
        public void DishDescriptionMaxLength_IsGreaterThanMinLength()
        {
            Assert.That(ValidationConstants.DishDescriptionMaxLength, Is.GreaterThan(ValidationConstants.DishDescriptionMinLength));
        }

        [Test]
        public void CategoryNameMaxLength_IsPositive()
        {
            Assert.That(ValidationConstants.CategoryNameMaxLength, Is.GreaterThan(0));
        }

        [Test]
        public void CategoryNameMinLength_IsLessThanMaxLength()
        {
            Assert.That(ValidationConstants.CategoryNameMinLength, Is.LessThan(ValidationConstants.CategoryNameMaxLength));
        }

        [Test]
        public void DishNameMaxLength_HasExpectedValue()
        {
            Assert.That(ValidationConstants.DishNameMaxLength, Is.EqualTo(50));
        }

        [Test]
        public void CategoryNameMaxLength_HasExpectedValue()
        {
            Assert.That(ValidationConstants.CategoryNameMaxLength, Is.EqualTo(50));
        }
    }
}
