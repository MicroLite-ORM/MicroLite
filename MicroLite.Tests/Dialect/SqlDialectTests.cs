namespace MicroLite.Tests.Dialect
{
    using System;
    using MicroLite.Dialect;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlDialect"/> class.
    /// </summary>
    [TestFixture]
    public class SqlDialectTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void CountQueryNoWhereOrOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.AreEqual("SELECT COUNT(*) FROM Customers", countQuery.CommandText);
            Assert.AreEqual(0, countQuery.Arguments.Count);
        }

        [Test]
        public void CountQueryWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.AreEqual("SELECT COUNT(*) FROM [dbo].[Customers]", countQuery.CommandText);
            Assert.AreEqual(0, countQuery.Arguments.Count);
        }

        [Test]
        public void CountQueryWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.AreEqual("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", countQuery.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], countQuery.Arguments[0], "The first argument should be the first argument from the original query");
        }

        [Test]
        public void CountQueryWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", CustomerStatus.Active);

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.AreEqual("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", countQuery.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], countQuery.Arguments[0], "The first argument should be the first argument from the original query");
        }

        [Test]
        public void CreateQueryForInstanceThrowsNotSupportedExceptionForStatementTypeBatch()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Batch, new Customer()));

            Assert.AreEqual(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Test]
        public void CreateQueryForInstanceThrowsNotSupportedExceptionForStatementTypeSelect()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Select, new Customer()));

            Assert.AreEqual(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Test]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeBatch()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Batch, typeof(Customer), 1223));

            Assert.AreEqual(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Test]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeInsert()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Insert, typeof(Customer), 1223));

            Assert.AreEqual(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Test]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeUpdate()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Update, typeof(Customer), 1223));

            Assert.AreEqual(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [MicroLite.Mapping.Table(schema: "Sales", name: "Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}