﻿namespace MicroLite.Tests.Query
{
    using System;
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlBuilder"/> class.
    /// </summary>
    public class SqlBuilderTests
    {
        [Fact]
        public void Execute()
        {
            var sqlQuery = SqlBuilder
                .Execute("GetCustomerInvoices")
                .WithParameter("@CustomerId", 7633245)
                .WithParameter("@StartDate", DateTime.Today.AddMonths(-3))
                .WithParameter("@EndDate", DateTime.Today)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(7633245, sqlQuery.Arguments[0]);
            Assert.Equal(DateTime.Today.AddMonths(-3), sqlQuery.Arguments[1]);
            Assert.Equal(DateTime.Today, sqlQuery.Arguments[2]);

            Assert.Equal("EXEC GetCustomerInvoices @CustomerId, @StartDate, @EndDate", sqlQuery.CommandText);
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullArgs()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => SqlBuilder.Select().From("").Where("").In((object[])null));

            Assert.Equal("args", exception.ParamName);
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullSqlQuery()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => SqlBuilder.Select().From("").Where("").In((SqlQuery)null));

            Assert.Equal("subQuery", exception.ParamName);
        }

        [Fact]
        public void SelectAverage()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Average("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT AVG(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectAverageWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Average("Total", columnAlias: "AverageTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT AVG(Total) AS AverageTotal FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectAverageWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Average("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .GroupBy("CustomerId")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT CustomerId, AVG(Total) AS Total FROM Invoices WHERE (CustomerId = @p0) GROUP BY CustomerId", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCount()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Count("CustomerId")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT COUNT(CustomerId) AS CustomerId FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCountWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Count("CustomerId", columnAlias: "CustomerCount")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT COUNT(CustomerId) AS CustomerCount FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectCountWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("ServiceId")
                .Count("CustomerId")
                .From(typeof(Customer))
                .GroupBy("ServiceId")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT ServiceId, COUNT(CustomerId) AS CustomerId FROM Sales.Customers GROUP BY ServiceId", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromOrderByAscending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1, Column2 FROM Table ORDER BY Column1, Column2 ASC", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Fact]
        public void SelectFromOrderByAscendingThenDescending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1")
                .OrderByDescending("Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1, Column2 FROM Table ORDER BY Column1 ASC, Column2 DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromOrderByDescending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1, Column2 FROM Table ORDER BY Column1, Column2 DESC", sqlQuery.CommandText);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Fact]
        public void SelectFromOrderByDescendingThenAscending()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1")
                .OrderByAscending("Column2")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1, Column2 FROM Table ORDER BY Column1 DESC, Column2 ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTableName()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Column1, Column2 FROM Table", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndType()
        {
            var sqlQuery = SqlBuilder
                .Select("Name", "DoB")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT Name, DoB FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingWildcardAndTableName()
        {
            var sqlQuery = SqlBuilder
                .Select("*")
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT * FROM Table", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromSpecifyingWildcardAndType()
        {
            var sqlQuery = SqlBuilder
                .Select("*")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("SELECT DoB, CustomerId, Name FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhere()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1, Column2 FROM Table WHERE (Column1 = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhereAnd()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .AndWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal("Bar", sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1, Column2 FROM Table WHERE (Column1 = @p0) AND (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhereComplex()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2", "Column3")
                .From("Table")
                .Where("Column1 = @p0 OR @p0 IS NULL", "Foo")
                .AndWhere("Column2 BETWEEN @p0 AND @p1", DateTime.Today.AddDays(-1), DateTime.Today)
                .OrWhere("Column3 IN (@p0, @p1, @p2, @p3)", 1, 2, 3, 4)
                .ToSqlQuery();

            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(DateTime.Today.AddDays(-1), sqlQuery.Arguments[1]);
            Assert.Equal(DateTime.Today, sqlQuery.Arguments[2]);
            Assert.Equal(1, sqlQuery.Arguments[3]);
            Assert.Equal(2, sqlQuery.Arguments[4]);
            Assert.Equal(3, sqlQuery.Arguments[5]);
            Assert.Equal(4, sqlQuery.Arguments[6]);

            Assert.Equal("SELECT Column1, Column2, Column3 FROM Table WHERE (Column1 = @p0 OR @p0 IS NULL) AND (Column2 BETWEEN @p1 AND @p2) OR (Column3 IN (@p3, @p4, @p5, @p6))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectFromWhereOr()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .OrWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal("Bar", sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1, Column2 FROM Table WHERE (Column1 = @p0) OR (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMax()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Max("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MAX(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMaxWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Max("Total", columnAlias: "MaxTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MAX(Total) AS MaxTotal FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMaxWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Max("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT CustomerId, MAX(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMin()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Min("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MIN(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMinWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Min("Total", columnAlias: "MinTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT MIN(Total) AS MinTotal FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectMinWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Min("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT CustomerId, MIN(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSum()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Sum("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT SUM(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSumWithAlias()
        {
            var sqlQuery = SqlBuilder
                .Select()
                .Sum("Total", columnAlias: "SumTotal")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT SUM(Total) AS SumTotal FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectSumWithOtherColumn()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Sum("Total")
                .From(typeof(Invoice))
                .Where("CustomerId = @p0", 1022)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1022, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT CustomerId, SUM(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereAndWhereInArgs()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .AndWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = @p0) AND (Column1 IN (@p1, @p2, @p3))", sqlQuery.CommandText);
        }

        [Fact]
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

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = @p0) AND (Column1 IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereBetween()
        {
            var sqlQuery = SqlBuilder
                   .Select("Column1")
                   .From("Table")
                   .Where("Column1")
                   .Between(1, 10)
                   .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(10, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 BETWEEN @p0 AND @p1)", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereGroupByHavingOrderBy()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate > @p0", new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .Having("SUM(Total) > @p0", 10000M)
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0]);
            Assert.Equal(10000M, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT CustomerId, SUM(Total) AS Total FROM Invoices WHERE (OrderDate > @p0) GROUP BY Total HAVING SUM(Total) > @p1 ORDER BY OrderDate DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereGroupByOrderBy()
        {
            var sqlQuery = SqlBuilder
                .Select("CustomerId")
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate > @p0", new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0]);

            Assert.Equal("SELECT CustomerId, SUM(Total) AS Total FROM Invoices WHERE (OrderDate > @p0) GROUP BY Total ORDER BY OrderDate DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereInArgs()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(1, sqlQuery.Arguments[0]);
            Assert.Equal(2, sqlQuery.Arguments[1]);
            Assert.Equal(3, sqlQuery.Arguments[2]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN (@p0, @p1, @p2))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(1024, sqlQuery.Arguments[0]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN (SELECT Id FROM Table WHERE Column = @p0))", sqlQuery.CommandText);
        }

        [Fact]
        public void SelectWhereOrWhereInArgs()
        {
            var sqlQuery = SqlBuilder
                .Select("Column1")
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .OrWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);
            Assert.Equal(3, sqlQuery.Arguments[3]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = @p0) OR (Column1 IN (@p1, @p2, @p3))", sqlQuery.CommandText);
        }

        [Fact]
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

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("FOO", sqlQuery.Arguments[0]);
            Assert.Equal(1024, sqlQuery.Arguments[1]);

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = @p0) OR (Column1 IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);
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
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.DbGenerated)]
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

        [MicroLite.Mapping.Table(name: "Invoices")]
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
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.DbGenerated)]
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