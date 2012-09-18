﻿namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="TableInfo"/> class.
    /// </summary>
    [TestFixture]
    public class TableInfoTests
    {
        [Test]
        public void ConstructorSetsPropertyValues()
        {
            var columns = new[]
            {
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true),
                new ColumnInfo("Id", typeof(Customer).GetProperty("Id"), true, true, true)
            };
            var identifierStrategy = IdentifierStrategy.Guid;
            var name = "Customers";
            var schema = "Sales";

            var tableInfo = new TableInfo(columns, identifierStrategy, name, schema);

            CollectionAssert.AreEqual(columns, tableInfo.Columns);
            Assert.AreEqual(columns[1].ColumnName, tableInfo.IdentifierColumn);
            Assert.AreEqual(identifierStrategy, tableInfo.IdentifierStrategy);
            Assert.AreEqual(name, tableInfo.Name);
            Assert.AreEqual(schema, tableInfo.Schema);
        }

        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullColumns()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TableInfo(columns: null, identifierStrategy: IdentifierStrategy.Identity, name: "Customers", schema: "Sales"));

            Assert.AreEqual("columns", exception.ParamName);
        }

        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TableInfo(columns: new ColumnInfo[0], identifierStrategy: IdentifierStrategy.Identity, name: null, schema: "Sales"));

            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullSchema()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new TableInfo(columns: new ColumnInfo[0], identifierStrategy: IdentifierStrategy.Identity, name: "Customers", schema: null));

            Assert.AreEqual("schema", exception.ParamName);
        }

        [Test]
        public void ConstructorThrowsMicroLiteExceptionIfMultipleColumnsWithSameName()
        {
            var columns = new[]
            {
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true),
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true)
            };

            var exception = Assert.Throws<MicroLiteException>(
                () => new TableInfo(columns: columns, identifierStrategy: IdentifierStrategy.Identity, name: "Customers", schema: "Sales"));

            Assert.AreEqual(Messages.TableInfo_ColumnMappedMultipleTimes.FormatWith("Name"), exception.Message);
        }

        [Test]
        public void ConstructorThrowsMicroLiteExceptionIfNoColumnsAreIdentifierColumn()
        {
            var columns = new[]
            {
                new ColumnInfo("Name", typeof(Customer).GetProperty("Name"), false, true, true)
            };

            var exception = Assert.Throws<MicroLiteException>(
                () => new TableInfo(columns: columns, identifierStrategy: IdentifierStrategy.Identity, name: "Customers", schema: "Sales"));

            Assert.AreEqual(Messages.TableInfo_NoIdentifierColumn.FormatWith("Sales", "Customers"), exception.Message);
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