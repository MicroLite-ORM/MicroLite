namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnInfo"/> class.
    /// </summary>
    public class ColumnInfoTests
    {
        [Fact]
        public void ConstructorSetsPropertyValues()
        {
            var columnName = "Name";
            var propertyInfo = typeof(Customer).GetProperty("Name");
            var isIdentifier = true;
            var allowInsert = true;
            var allowUpdate = true;

            var columnInfo = new ColumnInfo(columnName, propertyInfo, isIdentifier, allowInsert, allowUpdate);

            Assert.Equal(columnName, columnInfo.ColumnName);
            Assert.Equal(propertyInfo, columnInfo.PropertyInfo);
            Assert.Equal(isIdentifier, columnInfo.IsIdentifier);
            Assert.Equal(allowInsert, columnInfo.AllowInsert);
            Assert.Equal(allowUpdate, columnInfo.AllowUpdate);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullColumnName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ColumnInfo(null, null, false, true, true));

            Assert.Equal("columnName", exception.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullPropertyInfo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ColumnInfo("Name", null, false, true, true));

            Assert.Equal("propertyInfo", exception.ParamName);
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