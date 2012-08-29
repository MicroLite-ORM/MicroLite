namespace MicroLite.Tests.Query
{
    using System;
    using MicroLite.Query;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class SqlBuilderTests
    {
        [Test]
        public void Execute()
        {
            var sqlQuery = SqlBuilder
                .Execute("GetCustomerInvoices")
                .WithParameter("@CustomerId", 7633245)
                .WithParameter("@StartDate", DateTime.Today.AddMonths(-3))
                .WithParameter("@EndDate", DateTime.Today)
                .ToSqlQuery();

            Assert.AreEqual(3, sqlQuery.Arguments.Count);
            Assert.AreEqual(7633245, sqlQuery.Arguments[0]);
            Assert.AreEqual(DateTime.Today.AddMonths(-3), sqlQuery.Arguments[1]);
            Assert.AreEqual(DateTime.Today, sqlQuery.Arguments[2]);

            Assert.AreEqual("EXEC GetCustomerInvoices @CustomerId, @StartDate, @EndDate", sqlQuery.CommandText);
        }

        [Test]
        public void SelectAverage()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Average("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT AVG(Total)\r\n FROM Sales.Invoices\r\n WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectCount()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Count(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT COUNT(CustomerId)\r\n FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByAscending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n ORDER BY Column1 ASC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByDescending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n ORDER BY Column1 DESC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingColumnsAndTableName()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingColumnsAndType()
        {
            var sqlQuery = SqlBuilder
                .Select("Name", "DoB")
                .From(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Name, DoB\r\n FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingWildcardAndTableName()
        {
            var sqlQuery = SqlBuilder
                .Select("*")
                .From("Table")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT *\r\n FROM Table", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingWildcardAndType()
        {
            var sqlQuery = SqlBuilder
                .Select("*")
                .From(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT DoB, CustomerId, Name\r\n FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhere()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhereAnd()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .AndWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)\r\n AND (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhereComplex()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2", "Column3")
                .From("Table")
                .Where("Column1 = @p0 OR @p0 IS NULL", "Foo")
                .AndWhere("Column2 BETWEEN @p0 AND @p1", DateTime.Today.AddDays(-1), DateTime.Today)
                .OrWhere("Column3 IN (@p0, @p1, @p2, @p3)", 1, 2, 3, 4)
                .ToSqlQuery();

            Assert.AreEqual(7, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual(DateTime.Today.AddDays(-1), sqlQuery.Arguments[1]);
            Assert.AreEqual(DateTime.Today, sqlQuery.Arguments[2]);
            Assert.AreEqual(1, sqlQuery.Arguments[3]);
            Assert.AreEqual(2, sqlQuery.Arguments[4]);
            Assert.AreEqual(3, sqlQuery.Arguments[5]);
            Assert.AreEqual(4, sqlQuery.Arguments[6]);

            Assert.AreEqual("SELECT Column1, Column2, Column3\r\n FROM Table\r\n WHERE (Column1 = @p0 OR @p0 IS NULL)\r\n AND (Column2 BETWEEN @p1 AND @p2)\r\n OR (Column3 IN (@p3, @p4, @p5, @p6))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhereOr()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .OrWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)\r\n OR (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectMax()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Max("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT MAX(Total)\r\n FROM Sales.Invoices\r\n WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectMin()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Min("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT MIN(Total)\r\n FROM Sales.Invoices\r\n WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectSum()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Sum("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT SUM(Total)\r\n FROM Sales.Invoices\r\n WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [MicroLite.Mapping.Table(schema: "Sales", name: "Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
            public int Id
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Name")]
            public string Name
            {
                get;
                set;
            }
        }

        [MicroLite.Mapping.Table(schema: "Sales", name: "Invoices")]
        private class Invoice
        {
            public Invoice()
            {
            }

            [MicroLite.Mapping.Column("CustomerId")]
            public int CustomerId
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("InvoiceId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
            public int Id
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Total")]
            public decimal Total
            {
                get;
                set;
            }
        }
    }
}