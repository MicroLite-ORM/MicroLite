namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
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

        [Test]
        public void DeleteQueryForInstance()
        {
            var customer = new Customer
            {
                Id = 122672
            };

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Delete, customer);

            Assert.AreEqual("DELETE FROM \"Customers\" WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[0]);
        }

        [Test]
        public void DeleteQueryForTypeByIdentifier()
        {
            object identifier = 239845763;

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Delete, typeof(Customer), identifier);

            Assert.AreEqual("DELETE FROM \"Customers\" WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.AreEqual(identifier, sqlQuery.Arguments[0]);
        }

        /// <summary>
        /// Issue #11 - Identifier property value should be included on insert for IdentifierStrategy.Assigned.
        /// </summary>
        [Test]
        public void InsertQuery()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Id = 134875,
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active,
            };

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO \"Customers\" (\"Created\", \"DoB\", \"CustomerId\", \"Name\", \"StatusId\") VALUES (?, ?, ?, ?, ?)", sqlQuery.CommandText);
            Assert.AreEqual(customer.Created, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[2]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[3]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[4]);
        }

        [Test]
        public void SelectQuery()
        {
            object identifier = 12345421;

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Select, typeof(Customer), identifier);

            Assert.AreEqual("SELECT \"Created\", \"DoB\", \"CustomerId\", \"Name\", \"StatusId\", \"Updated\" FROM \"Customers\" WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.AreEqual(identifier, sqlQuery.Arguments[0]);
        }

        [Test]
        public void UpdateQuery()
        {
            var customer = new Customer
            {
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Id = 134875,
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now
            };

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Update, customer);

            Assert.AreEqual("UPDATE \"Customers\" SET \"DoB\" = ?, \"Name\" = ?, \"StatusId\" = ?, \"Updated\" = ? WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[1]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[2]);
            Assert.AreEqual(customer.Updated, sqlQuery.Arguments[3]);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[4]);
        }

        [MicroLite.Mapping.Table("Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            [MicroLite.Mapping.Column("Created", allowInsert: true, allowUpdate: false)]
            public DateTime Created
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Assigned)]
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

            [MicroLite.Mapping.Column("StatusId")]
            public CustomerStatus Status
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Updated", allowInsert: false, allowUpdate: true)]
            public DateTime? Updated
            {
                get;
                set;
            }
        }
    }
}