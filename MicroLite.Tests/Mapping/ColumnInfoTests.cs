namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnInfo"/> class.
    /// </summary>
    [TestFixture]
    public class ColumnInfoTests
    {
        [Test]
        public void ConstructorSetsPropertyValues()
        {
            var columnName = "Name";
            var propertyInfo = typeof(Customer).GetProperty("Name");
            var isIdentifier = true;
            var allowUpdate = true;

            var columnInfo = new ColumnInfo(columnName, propertyInfo, isIdentifier, allowUpdate);

            Assert.AreEqual(columnName, columnInfo.ColumnName);
            Assert.AreEqual(propertyInfo, columnInfo.PropertyInfo);
            Assert.AreEqual(isIdentifier, columnInfo.IsIdentifier);
            Assert.AreEqual(allowUpdate, columnInfo.AllowUpdate);
        }

        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullColumnName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ColumnInfo(null, null, false, true));

            Assert.AreEqual("columnName", exception.ParamName);
        }

        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullPropertyInfo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ColumnInfo("Name", null, false, true));

            Assert.AreEqual("propertyInfo", exception.ParamName);
        }

        private class Customer
        {
            public Customer()
            {
            }

            public string Name
            {
                get;
                set;
            }
        }
    }
}