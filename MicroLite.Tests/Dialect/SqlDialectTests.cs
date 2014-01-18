namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlDialect"/> class.
    /// </summary>
    public class SqlDialectTests : IDisposable
    {
        public SqlDialectTests()
        {
            // The tests in this suite all use attribute mapping for the test.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

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
                var mockSqlCharacters = new Mock<SqlCharacters>();
                mockSqlCharacters.CallBase = true;

                var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
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
                var mockSqlCharacters = new Mock<SqlCharacters>();
                mockSqlCharacters.CallBase = true;

                var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
                mockSqlDialect.CallBase = true;
                mockSqlDialect.Object.BuildCommand(command, sqlQuery);

                Assert.Equal(sqlQuery.Timeout, command.CommandTimeout);
            }
        }

        [Fact]
        public void CountQueryNoWhereOrOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM Customers", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [dbo].[Customers]", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ? ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);////, "The first argument should be the first argument from the original query");
        }

        [Fact]
        public void CountQueryWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", CustomerStatus.Active);

            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);////, "The first argument should be the first argument from the original query");
        }

        [Fact]
        public void CreateQueryForInstanceThrowsNotSupportedExceptionForStatementTypeBatch()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(StatementType.Batch, new Customer()));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForInstanceThrowsNotSupportedExceptionForStatementTypeSelect()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(StatementType.Select, new Customer()));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForObjectDelta()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(objectDelta);

            Assert.Equal("UPDATE Customers SET Name = ? WHERE CustomerId = ?", sqlQuery.CommandText);
            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Fred Flintstone", sqlQuery.Arguments[0]);
            Assert.Equal(1234, sqlQuery.Arguments[1]);
        }

        [Fact]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeBatch()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(StatementType.Batch, typeof(Customer), 123));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeInsert()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(StatementType.Insert, typeof(Customer), 123));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void CreateQueryForTypeThrowsNotSupportedExceptionForStatementTypeUpdate()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<NotSupportedException>(
                () => mockSqlDialect.Object.CreateQuery(StatementType.Update, typeof(Customer), 123));

            Assert.Equal(Messages.SqlDialect_StatementTypeNotSupported, exception.Message);
        }

        [Fact]
        public void DeleteByIdentifierQuery()
        {
            var identifier = 134875;

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Delete, typeof(Customer), identifier);

            Assert.Equal("DELETE FROM Customers WHERE CustomerId = ?", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(identifier, sqlQuery.Arguments[0]);
        }

        [Fact]
        public void DeleteInstanceQuery()
        {
            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now
            };

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Delete, customer);

            Assert.Equal("DELETE FROM Customers WHERE CustomerId = ?", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Id, sqlQuery.Arguments[0]);
        }

        public void Dispose()
        {
            // Reset the mapping convention after tests have run.
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        /// <summary>
        /// Issue #11 - Identifier property value should be included on insert for IdentifierStrategy.Assigned.
        /// </summary>
        [Fact]
        public void InsertInstanceQuery()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now
            };

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            // Do a second query to check that the caching doesn't cause a problem.
            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Insert, customer);

            Assert.Equal("INSERT INTO Customers (Created, DoB, CustomerId, Name, StatusId) VALUES (?, ?, ?, ?, ?)", sqlQuery.CommandText);
            Assert.Equal(5, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4]);

            var sqlQuery2 = mockSqlDialect.Object.CreateQuery(StatementType.Insert, customer);

            Assert.Equal("INSERT INTO Customers (Created, DoB, CustomerId, Name, StatusId) VALUES (?, ?, ?, ?, ?)", sqlQuery2.CommandText);
            Assert.Equal(5, sqlQuery2.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[1]);
            Assert.Equal(customer.Id, sqlQuery2.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[4]);
        }

        [Fact]
        public void SelectByIdentifierQuery()
        {
            var identifier = 134875;

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Select, typeof(Customer), identifier);

            Assert.Equal("SELECT Created, DoB, CustomerId, Name, StatusId, Updated FROM Customers WHERE (CustomerId = ?)", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(identifier, sqlQuery.Arguments[0]);
        }

        [Fact]
        public void SqlCharactersPassedToConstructorAreExposed()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.MsSql);
            mockSqlDialect.CallBase = true;

            Assert.Same(SqlCharacters.MsSql, mockSqlDialect.Object.SqlCharacters);
        }

        [Fact]
        public void SupportsBatchedQueriesReturnsTrueByDefault()
        {
            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
            mockSqlDialect.CallBase = true;

            Assert.True(mockSqlDialect.Object.SupportsBatchedQueries);
        }

        [Fact]
        public void UpdateInstanceQuery()
        {
            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now
            };

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.CreateQuery(StatementType.Update, customer);

            Assert.Equal("UPDATE Customers SET DoB = ?, Name = ?, StatusId = ?, Updated = ? WHERE CustomerId = ?", sqlQuery.CommandText);
            Assert.Equal(5, sqlQuery.Arguments.Count);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[0]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[1]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[4]);

            // Do a second query to check that the caching doesn't cause a problem.
            var sqlQuery2 = mockSqlDialect.Object.CreateQuery(StatementType.Update, customer);

            Assert.Equal("UPDATE Customers SET DoB = ?, Name = ?, StatusId = ?, Updated = ? WHERE CustomerId = ?", sqlQuery2.CommandText);
            Assert.Equal(5, sqlQuery2.Arguments.Count);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[0]);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[1]);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[2]);
            Assert.Equal(customer.Updated, sqlQuery2.Arguments[3]);
            Assert.Equal(customer.Id, sqlQuery2.Arguments[4]);
        }

        public class WhenCallingCombine
        {
            private readonly SqlQuery combinedQuery;
            private readonly SqlQuery sqlQuery1;
            private readonly SqlQuery sqlQuery2;

            public WhenCallingCombine()
            {
                this.sqlQuery1 = new SqlQuery("SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?", "Foo", 100);
                this.sqlQuery1.Timeout = 38;

                this.sqlQuery2 = new SqlQuery("SELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?", "Bar", -1);
                this.sqlQuery2.Timeout = 42;

                var mockSqlCharacters = new Mock<SqlCharacters>();
                mockSqlCharacters.CallBase = true;

                var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
                mockSqlDialect.CallBase = true;

                this.combinedQuery = mockSqlDialect.Object.Combine(new[] { this.sqlQuery1, this.sqlQuery2 });
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[0], this.combinedQuery.Arguments[0]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[0], this.combinedQuery.Arguments[2]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheNumberOfArgumentsInTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery1.Arguments.Count + this.sqlQuery2.Arguments.Count, this.combinedQuery.Arguments.Count);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[1], this.combinedQuery.Arguments[1]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[1], this.combinedQuery.Arguments[3]);
            }

            [Fact]
            public void TheCombinedCommandTextShouldBeSeparatedUsingTheSelectSeparator()
            {
                Assert.Equal(
                    "SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?;\r\nSELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?",
                    this.combinedQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutShouldBeSetToTheLongestTimeoutOfTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery2.Timeout, this.combinedQuery.Timeout);
            }
        }

        public class WhenCallingCombineAndTheSourceQueriesIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var mockSqlCharacters = new Mock<SqlCharacters>();
                mockSqlCharacters.CallBase = true;

                var mockSqlDialect = new Mock<SqlDialect>(mockSqlCharacters.Object);
                mockSqlDialect.CallBase = true;

                var exception = Assert.Throws<ArgumentNullException>(() => mockSqlDialect.Object.Combine(null));

                Assert.Equal("sqlQueries", exception.ParamName);
            }
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