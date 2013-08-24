namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Collections.ObjectModel;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="TableInfo"/> class.
    /// </summary>
    public class TableInfoTests
    {
        [Fact]
        public void ConstructorSetsPropertyValues()
        {
            var columns = new ReadOnlyCollection<ColumnInfo>(new[]
            {
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true),
                new ColumnInfo("CustomerId", typeof(Customer).GetProperty("Id"), true, true, true)
            });
            var identifierStrategy = IdentifierStrategy.Guid;
            var name = "Customers";
            var schema = "Sales";

            var tableInfo = new TableInfo(columns, identifierStrategy, name, schema);

            Assert.Equal(columns, tableInfo.Columns);
            Assert.Equal(columns[1].ColumnName, tableInfo.IdentifierColumn);
            Assert.Equal(columns[1].PropertyInfo.Name, tableInfo.IdentifierProperty);
            Assert.Equal(identifierStrategy, tableInfo.IdentifierStrategy);
            Assert.Equal(name, tableInfo.Name);
            Assert.Equal(schema, tableInfo.Schema);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullColumns()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TableInfo(columns: null, identifierStrategy: IdentifierStrategy.DbGenerated, name: "Customers", schema: "Sales"));

            Assert.Equal("columns", exception.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TableInfo(columns: new ColumnInfo[0], identifierStrategy: IdentifierStrategy.DbGenerated, name: null, schema: "Sales"));

            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void ConstructorThrowsMicroLiteExceptionIfMultipleColumnsWithSameName()
        {
            var columns = new[]
            {
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true),
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true)
            };

            var exception = Assert.Throws<MicroLiteException>(
                () => new TableInfo(columns: columns, identifierStrategy: IdentifierStrategy.DbGenerated, name: "Customers", schema: "Sales"));

            Assert.Equal(Messages.TableInfo_ColumnMappedMultipleTimes.FormatWith("Name"), exception.Message);
        }

        [Fact]
        public void ConstructorThrowsMicroLiteExceptionIfNoColumnsAreIdentifierColumn()
        {
            var columns = new[]
            {
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true)
            };

            var exception = Assert.Throws<MicroLiteException>(
                () => new TableInfo(columns: columns, identifierStrategy: IdentifierStrategy.DbGenerated, name: "Customers", schema: "Sales"));

            Assert.Equal(Messages.TableInfo_NoIdentifierColumn.FormatWith("Sales", "Customers"), exception.Message);
        }

        private class Customer
        {
            public Customer()
            {
            }

            public Guid Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }
    }
}