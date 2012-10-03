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

            Assert.AreEqual("SELECT AVG(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectAverageWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Average("Total", columnAlias: "AverageTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT AVG(Total) AS AverageTotal FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectAverageWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Average("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .GroupBy("CustomerId")
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT CustomerId, AVG(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0) GROUP BY CustomerId", sqlQuery.CommandText);
        }

        [Test]
        public void SelectCount()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Count("CustomerId")
                .From(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT COUNT(CustomerId) AS CustomerId FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Test]
        public void SelectCountWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Count("CustomerId", columnAlias: "CustomerCount")
                .From(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT COUNT(CustomerId) AS CustomerCount FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Test]
        public void SelectCountWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("ServiceId")
                .Count("CustomerId")
                .From(typeof(Customer))
                .GroupBy("ServiceId")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT ServiceId, COUNT(CustomerId) AS CustomerId FROM Sales.Customers GROUP BY ServiceId", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByAscending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1", "Column2")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2 FROM Table ORDER BY Column1, Column2 ASC", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Test]
        public void SelectFromOrderByAscendingThenDescending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1")
                .OrderByDescending("Column2")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2 FROM Table ORDER BY Column1 ASC, Column2 DESC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByDescending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1", "Column2")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2 FROM Table ORDER BY Column1, Column2 DESC", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Test]
        public void SelectFromOrderByDescendingThenAscending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1")
                .OrderByAscending("Column2")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2 FROM Table ORDER BY Column1 DESC, Column2 ASC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingColumnsAndTableName()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2 FROM Table", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingColumnsAndType()
        {
            var sqlQuery = SqlBuilder
                .Select("Name", "DoB")
                .From(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Name, DoB FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingWildcardAndTableName()
        {
            var sqlQuery = SqlBuilder
                .Select("*")
                .From("Table")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT * FROM Table", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSpecifyingWildcardAndType()
        {
            var sqlQuery = SqlBuilder
                .Select("*")
                .From(typeof(Customer))
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT DoB, CustomerId, Name FROM Sales.Customers", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT Column1, Column2 FROM Table WHERE (Column1 = @p0)", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT Column1, Column2 FROM Table WHERE (Column1 = @p0) AND (Column2 = @p1)", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT Column1, Column2, Column3 FROM Table WHERE (Column1 = @p0 OR @p0 IS NULL) AND (Column2 BETWEEN @p1 AND @p2) OR (Column3 IN (@p3, @p4, @p5, @p6))", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT Column1, Column2 FROM Table WHERE (Column1 = @p0) OR (Column2 = @p1)", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT MAX(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectMaxWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Max("Total", columnAlias: "MaxTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT MAX(Total) AS MaxTotal FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectMaxWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Max("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT CustomerId, MAX(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT MIN(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectMinWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Min("Total", columnAlias: "MinTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT MIN(Total) AS MinTotal FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectMinWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Min("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT CustomerId, MIN(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
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

            Assert.AreEqual("SELECT SUM(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectSumWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Sum("Total", columnAlias: "SumTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT SUM(Total) AS SumTotal FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectSumWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Sum("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1022, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT CustomerId, SUM(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereAndWhereInArgs()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .AndWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.AreEqual(4, sqlQuery.Arguments.Count);
            Assert.AreEqual("FOO", sqlQuery.Arguments[0]);
            Assert.AreEqual(1, sqlQuery.Arguments[1]);
            Assert.AreEqual(2, sqlQuery.Arguments[2]);
            Assert.AreEqual(3, sqlQuery.Arguments[3]);

            Assert.AreEqual("SELECT Column1 FROM Table WHERE (Column2 = @p0) AND (Column1 IN (@p1, @p2, @p3))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereAndWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .AndWhere("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("FOO", sqlQuery.Arguments[0]);
            Assert.AreEqual(1024, sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1 FROM Table WHERE (Column2 = @p0) AND (Column1 IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereGroupByOrderBy()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate > @p0", new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(new DateTime(2000, 1, 1), sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT CustomerId, SUM(Total) AS Total FROM Invoices WHERE (OrderDate > @p0) GROUP BY Total", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereInArgs()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.AreEqual(3, sqlQuery.Arguments.Count);
            Assert.AreEqual(1, sqlQuery.Arguments[0]);
            Assert.AreEqual(2, sqlQuery.Arguments[1]);
            Assert.AreEqual(3, sqlQuery.Arguments[2]);

            Assert.AreEqual("SELECT Column1 FROM Table WHERE (Column1 IN (@p0, @p1, @p2))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual(1024, sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT Column1 FROM Table WHERE (Column1 IN (SELECT Id FROM Table WHERE Column = @p0))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereOrWhereInArgs()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .OrWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.AreEqual(4, sqlQuery.Arguments.Count);
            Assert.AreEqual("FOO", sqlQuery.Arguments[0]);
            Assert.AreEqual(1, sqlQuery.Arguments[1]);
            Assert.AreEqual(2, sqlQuery.Arguments[2]);
            Assert.AreEqual(3, sqlQuery.Arguments[3]);

            Assert.AreEqual("SELECT Column1 FROM Table WHERE (Column2 = @p0) OR (Column1 IN (@p1, @p2, @p3))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectWhereOrWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .OrWhere("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("FOO", sqlQuery.Arguments[0]);
            Assert.AreEqual(1024, sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1 FROM Table WHERE (Column2 = @p0) OR (Column1 IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);
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