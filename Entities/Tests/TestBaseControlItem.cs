using System;

using NUnit.Framework;

namespace Demo.Tests
{
    [TestFixture]
    public class TestBaseControlItem
    {
        [Test]
        public static void Contains_InvalidOperation_ReturnsFalse()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsFalse(item.Contains("Shake"));
        }

        [Test]
        public static void Contains_ValidOperation_ReturnsTrue()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsTrue(item.Contains("Edit"));
        }

        [Test]
        public static void Grant_NullOperationOrResource_ThrowsException()
        {
            var item = new BaseControlItem();
            Assert.Throws<ArgumentNullException>(delegate { item.Include(null, "Edit"); });
            Assert.Throws<ArgumentNullException>(delegate { item.Include("Administrators", null); });
        }

        [Test]
        public static void Grant_OneOperationWithTwoPrincipals_Success()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Read");
            item.Include("Programmers", "Read");
            item.IsIncluded(new[] {"Administrators"}, "Read");
            item.IsIncluded(new[] {"Programmers"}, "Read");
        }

        [Test]
        public static void IsGranted_CaseInsensitivity_IsTrue()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsTrue(item.IsIncluded(new[] {"administrators"}, "edit"));
        }

        [Test]
        public static void IsGranted_InvalidOperationAndResource_ReturnsFalse()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsFalse(item.IsIncluded(new[] {"Programmers"}, "Delete"));
        }

        [Test]
        public static void IsGranted_InvalidOperationOrResource_ReturnsFalse()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsFalse(item.IsIncluded(new[] {"Administrators"}, "Delete"));
            Assert.IsFalse(item.IsIncluded(new[] {"Programmers"}, "Edit"));
        }

        [Test]
        public static void IsGranted_LeftAndRightPadding_IsIgnored()
        {
            var item = new BaseControlItem();
            item.Include("Administrators  ", "Edit  ");

            Assert.IsTrue(item.IsIncluded(new[] {" administrators "}, " edit "));
        }

        [Test]
        public static void IsGranted_NullOperationOrResource_ReturnsFalse()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsFalse(item.IsIncluded(null, "Delete"));
            Assert.IsFalse(item.IsIncluded(new[] {"Programmers"}, null));
            Assert.IsFalse(item.IsIncluded(new[] {"Programmers", null}, null));
            Assert.IsTrue(item.IsIncluded(new[] {null, "Administrators"}, "Edit"));
        }

        [Test]
        public static void IsGranted_ValidOperationAndResource_ReturnsTrue()
        {
            var item = new BaseControlItem();
            item.Include("Administrators", "Edit");

            Assert.IsTrue(item.IsIncluded(new[] {"Administrators"}, "Edit"));
        }
    }
}