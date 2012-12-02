namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlDialect"/> class.
    /// </summary>
    public class SqlDialectTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Fact]
        public void BuildCommandForSqlQueryWithSqlText()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = ? AND [Table].[Value1] = ? AND [Table].[Value2] = ?",
                new object[] { 100, "hello", null });

            using (var command = new System.Data.OleDb.OleDbCommand())
            {
                var mockSqlDialect = new Mock<SqlDialect>();
                mockSqlDialect.CallBase = true;
                mockSqlDialect.Object.BuildCommand(command, sqlQuery);

                Assert.Equal(sqlQuery.CommandText, command.CommandText);
                Assert.Equal(CommandType.Text, command.CommandType);
                Assert.Equal(3, command.Parameters.Count);

                var parameter1 = (IDataParameter)command.Parameters[0];
                Assert.Equal(ParameterDirection.Input, parameter1.Direction);
                Assert.Equal("Parameter0", parameter1.ParameterName);
                Assert.Equal(sqlQuery.Arguments[0], parameter1.Value);

                var parameter2 = (IDataParameter)command.Parameters[1];
                Assert.Equal(ParameterDirection.Input, parameter2.Direction);
                Assert.Equal("Parameter1", parameter2.ParameterName);
                Assert.Equal(sqlQuery.Arguments[1], parameter2.Value);

                var parameter3 = (IDataParameter)command.Parameters[2];
                Assert.Equal(ParameterDirection.Input, parameter3.Direction);
                Assert.Equal("Parameter2", parameter3.ParameterName);
                Assert.Equal(DBNull.Value, parameter3.Value);
            }
        }

        [Fact]
        public void BuildCommandSetsDbCommandTimeoutToSqlQueryTime()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM [Table]");
            sqlQuery.Timeout = 42; // Use an oddball time which shouldn't be a default anywhere.

            using (var command = new System.Data.OleDb.OleDbCommand())
            {
                var mockSqlDialect = new Mock<SqlDialect>();
                mockSqlDialect.CallBase = true;
                mockSqlDialect.Object.BuildCommand(command, sqlQuery);

                Assert.Equal(sqlQuery.Timeout, command.CommandTimeout);
            }
        }

        [Fact]
        public void CountQueryNoWhereOrOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM Customers", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [dbo].[Customers]", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ? ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);////, "The first argument should be the first argument from the original query");
        }

        [Fact]
        public void CountQueryWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", CustomerStatus.Active);

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);////, "The first argument should be the first argument from the original query");
        }

        [Fact]
        public void CreateQueryForInstanceThrowsNotSupportedExceptionForStatementTypeBatch()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Batch, new Customer()));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForInstanceThrowsNotSupportedExceptionForStatementTypeSelect()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Select, new Customer()));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeBatch()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Batch, typeof(Customer), 1223));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeInsert()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Insert, typeof(Customer), 1223));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeUpdate()
        {
            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(System.Data.StatementType.Update, typeof(Customer), 1223));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void DeleteQueryForInstance()
        {
            var customer = new Customer
            {
                Id = 122672
            };

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Delete, customer);

            Assert.Equal("DELETE FROM \"Customers\" WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.Equal(customer.Id, sqlQuery.Arguments[0]);
        }

        [Fact]
        public void DeleteQueryForTypeByIdentifier()
        {
            object identifier = 239845763;

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Delete, typeof(Customer), identifier);

            Assert.Equal("DELETE FROM \"Customers\" WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.Equal(identifier, sqlQuery.Arguments[0]);
        }

        /// <summary>
        /// Issue #11 - Identifier property value should be included on insert for IdentifierStrategy.Assigned.
        /// </summary>
        [Fact]
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

            Assert.Equal("INSERT INTO \"Customers\" (\"Created\", \"DoB\", \"CustomerId\", \"Name\", \"StatusId\") VALUES (?, ?, ?, ?, ?)", sqlQuery.CommandText);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4]);
        }

        [Fact]
        public void SelectQuery()
        {
            object identifier = 12345421;

            var mockSqlDialect = new Mock<SqlDialect>();
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Select, typeof(Customer), identifier);

            Assert.Equal("SELECT \"Created\", \"DoB\", \"CustomerId\", \"Name\", \"StatusId\", \"Updated\" FROM \"Customers\" WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.Equal(identifier, sqlQuery.Arguments[0]);
        }

        [Fact]
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

            Assert.Equal("UPDATE \"Customers\" SET \"DoB\" = ?, \"Name\" = ?, \"StatusId\" = ?, \"Updated\" = ? WHERE \"CustomerId\" = ?", sqlQuery.CommandText);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[0]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[1]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[4]);
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