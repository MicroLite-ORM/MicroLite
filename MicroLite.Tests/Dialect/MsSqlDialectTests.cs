namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MsSqlDialect"/> class.
    /// </summary>
    public class MsSqlDialectTests : IDisposable
    {
        public MsSqlDialectTests()
        {
            // The tests in this suite all use attribute mapping for the test.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
            SqlBuilder.SqlCharacters = null;
        }

        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        /// <summary>
        /// Issue #6 - The argument count check needs to cater for the same argument being used twice.
        /// </summary>
        [Fact]
        public void BuildCommandForSqlQueryWithSqlTextWhichUsesSameParameterTwice()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value1] = @p1 OR @p1 IS NULL",
                new object[] { 100, "hello" });

            using (var command = new System.Data.SqlClient.SqlCommand())
            {
                var sqlDialect = new MsSqlDialect();
                sqlDialect.BuildCommand(command, sqlQuery);

                Assert.Equal(sqlQuery.CommandText, command.CommandText);
                Assert.Equal(CommandType.Text, command.CommandType);
                Assert.Equal(2, command.Parameters.Count);

                var parameter1 = (IDataParameter)command.Parameters[0];
                Assert.Equal(ParameterDirection.Input, parameter1.Direction);
                Assert.Equal("@p0", parameter1.ParameterName);
                Assert.Equal(sqlQuery.Arguments[0], parameter1.Value);

                var parameter2 = (IDataParameter)command.Parameters[1];
                Assert.Equal(ParameterDirection.Input, parameter2.Direction);
                Assert.Equal("@p1", parameter2.ParameterName);
                Assert.Equal(sqlQuery.Arguments[1], parameter2.Value);
            }
        }

        [Fact]
        public void BuildCommandForSqlQueryWithStoredProcedureWithoutParameters()
        {
            var sqlQuery = new SqlQuery("EXEC GetTableContents");

            using (var command = new System.Data.SqlClient.SqlCommand())
            {
                var sqlDialect = new MsSqlDialect();
                sqlDialect.BuildCommand(command, sqlQuery);

                // The command text should only contain the stored procedure name.
                Assert.Equal("GetTableContents", command.CommandText);
                Assert.Equal(CommandType.StoredProcedure, command.CommandType);
                Assert.Equal(0, command.Parameters.Count);
            }
        }

        [Fact]
        public void BuildCommandForSqlQueryWithStoredProcedureWithParameters()
        {
            var sqlQuery = new SqlQuery(
                "EXEC GetTableContents @identifier, @Cust_Name",
                new object[] { 100, "hello" });

            using (var command = new System.Data.SqlClient.SqlCommand())
            {
                var sqlDialect = new MsSqlDialect();
                sqlDialect.BuildCommand(command, sqlQuery);

                // The command text should only contain the stored procedure name.
                Assert.Equal("GetTableContents", command.CommandText);
                Assert.Equal(CommandType.StoredProcedure, command.CommandType);
                Assert.Equal(2, command.Parameters.Count);

                var parameter1 = (IDataParameter)command.Parameters[0];
                Assert.Equal("@identifier", parameter1.ParameterName);
                Assert.Equal(sqlQuery.Arguments[0], parameter1.Value);

                var parameter2 = (IDataParameter)command.Parameters[1];
                Assert.Equal("@Cust_Name", parameter2.ParameterName);
                Assert.Equal(sqlQuery.Arguments[1], parameter2.Value);
            }
        }

        [Fact]
        public void BuildCommandThrowsMicroLiteExceptionForParameterCountMismatch()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value] = @p1",
                new object[] { 100 });

            using (var command = new System.Data.SqlClient.SqlCommand())
            {
                var sqlDialect = new MsSqlDialect();

                var exception = Assert.Throws<MicroLiteException>(
                    () => sqlDialect.BuildCommand(command, sqlQuery));

                Assert.Equal(Messages.SqlDialect_ArgumentsCountMismatch.FormatWith("2", "1"), exception.Message);
            }
        }

        public void Dispose()
        {
            // Reset the mapping convention after tests have run.
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
            SqlBuilder.SqlCharacters = null;
        }

        [Fact]
        public void InsertQueryForIdentityInstance()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.Equal("INSERT INTO [Sales].[Customers] ([Created], [DoB], [Name], [StatusId]) VALUES (@p0, @p1, @p2, @p3);SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[3]);
        }

        /// <summary>
        /// Issue #206 - Session.Paged errors if the query includes a sub query
        /// </summary>
        [Fact]
        public void PagedQueryWithoutSubQuery()
        {
            SqlBuilder.SqlCharacters = SqlCharacters.MsSql;

            var sqlQuerySingleLevel = SqlBuilder
                                        .Select("*").From(typeof(Customer))
                                        .Where("Name LIKE @p0", "Fred%")
                                        .ToSqlQuery();

            MsSqlDialect msSqlDialect = new MsSqlDialect();

            SqlQuery pageQuerySingleLevel = msSqlDialect.PageQuery(sqlQuerySingleLevel, PagingOptions.ForPage(page: 2, resultsPerPage: 10));
            Assert.Equal("SELECT [Created], [DoB], [CustomerId], [Name], [StatusId], [Updated] FROM (SELECT [Created], [DoB], [CustomerId], [Name], [StatusId], [Updated], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE (Name LIKE @p0)) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", pageQuerySingleLevel.CommandText);
            Assert.Equal("Fred%", pageQuerySingleLevel.Arguments[0]);
            Assert.Equal(11, pageQuerySingleLevel.Arguments[1]);
            Assert.Equal(20, pageQuerySingleLevel.Arguments[2]);
        }

        /// <summary>
        /// Issue #206 - Session.Paged errors if the query includes a sub query
        /// </summary>
        [Fact]
        public void PagedQueryWithSubQuery()
        {
            SqlBuilder.SqlCharacters = SqlCharacters.MsSql;

            var sqlQuerySubQuery = SqlBuilder
                                        .Select("*")
                                        .From(typeof(Customer))
                                        .Where("Name LIKE @p0", "Fred%")
                                        .AndWhere("SourceId").In(new SqlQuery("SELECT SourceId FROM Source WHERE Status = @p0", 1))
                                        .ToSqlQuery();

            MsSqlDialect msSqlDialect = new MsSqlDialect();

            SqlQuery pageQuerySubQuery = msSqlDialect.PageQuery(sqlQuerySubQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 10));
            Assert.Equal("SELECT [Created], [DoB], [CustomerId], [Name], [StatusId], [Updated] FROM (SELECT [Created], [DoB], [CustomerId], [Name], [StatusId], [Updated], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE (Name LIKE @p0) AND ([SourceId] IN (SELECT SourceId FROM Source WHERE Status = @p1))) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", pageQuerySubQuery.CommandText);
            Assert.Equal("Fred%", pageQuerySubQuery.Arguments[0]);
            Assert.Equal(1, pageQuerySubQuery.Arguments[1]);
            Assert.Equal(11, pageQuerySubQuery.Arguments[2]);
            Assert.Equal(20, pageQuerySubQuery.Arguments[3]);
        }

        [Fact]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DoB, StatusId FROM (SELECT CustomerId, Name, DoB, StatusId, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithMultiWhereAndMultiOrderByMultiLine()
        {
            var sqlQuery = new SqlQuery(@"SELECT
 [Customers].[CustomerId],
 [Customers].[Name],
 [Customers].[DoB],
 [Customers].[StatusId]
 FROM
 [Sales].[Customers]
 WHERE
 ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)
 ORDER BY
 [Customers].[Name] ASC,
 [Customers].[DoB] ASC", new object[] { CustomerStatus.Active, new DateTime(1980, 01, 01) });

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC, [Customers].[DoB] ASC) AS RowNumber FROM [Sales].[Customers] WHERE ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(sqlQuery.Arguments[1], paged.Arguments[1]);////, "The second argument should be the second argument from the original query");
            Assert.Equal(1, paged.Arguments[2]);////, "The third argument should be the start row number");
            Assert.Equal(25, paged.Arguments[3]);////, "The fourth argument should be the end row number");
        }

        [Fact]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM (SELECT [CustomerId], [Name], [DoB], [StatusId], ROW_NUMBER() OVER(ORDER BY [CustomerId] ASC) AS RowNumber FROM [dbo].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(26, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(50, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(1, paged.Arguments[1]);////, "The second argument should be the start row number");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the end row number");
        }

        [Fact]
        public void PageWithWhereAndOrderByMultiLine()
        {
            var sqlQuery = new SqlQuery(@"SELECT
 [Customers].[CustomerId],
 [Customers].[Name],
 [Customers].[DoB],
 [Customers].[StatusId]
 FROM
 [Sales].[Customers]
 WHERE
 [Customers].[StatusId] = @p0
 ORDER BY
 [Customers].[Name] ASC", new object[] { CustomerStatus.Active });

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(1, paged.Arguments[1]);////, "The second argument should be the start row number");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the end row number");
        }

        [Fact]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", CustomerStatus.Active);

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(1, paged.Arguments[1]);////, "The second argument should be the start row number");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the end row number");
        }

        public class WhenCallingCombine
        {
            private readonly SqlQuery combinedQuery;
            private readonly SqlQuery sqlQuery1;
            private readonly SqlQuery sqlQuery2;

            public WhenCallingCombine()
            {
                this.sqlQuery1 = new SqlQuery("SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1", "Foo", 100);
                this.sqlQuery1.Timeout = 38;

                this.sqlQuery2 = new SqlQuery("SELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p0 OR @p0 IS NULL) AND [Column_2] < @p1", "Bar", -1);
                this.sqlQuery2.Timeout = 42;

                var sqlDialect = new MsSqlDialect();

                this.combinedQuery = sqlDialect.Combine(new[] { this.sqlQuery1, this.sqlQuery2 });
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
                    "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1;\r\nSELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p2 OR @p2 IS NULL) AND [Column_2] < @p3",
                    this.combinedQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutShouldBeSetToTheLongestTimeoutOfTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery2.Timeout, this.combinedQuery.Timeout);
            }
        }

        /// <summary>
        /// Issue #90 - Re-Writing parameters should not happen if the query is a stored procedure.
        /// </summary>
        public class WhenCallingCombineAndAnSqlQueryIsForAStoredProcedure
        {
            private readonly SqlQuery combinedQuery;
            private readonly SqlQuery sqlQuery1;
            private readonly SqlQuery sqlQuery2;

            public WhenCallingCombineAndAnSqlQueryIsForAStoredProcedure()
            {
                this.sqlQuery1 = new SqlQuery("SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1", "Foo", 100);
                this.sqlQuery2 = new SqlQuery("EXEC CustomersByStatus @StatusId", 2);

                var sqlDialect = new MsSqlDialect();

                this.combinedQuery = sqlDialect.Combine(new[] { this.sqlQuery1, this.sqlQuery2 });
            }

            [Fact]
            public void TheParameterNamesForTheStoredProcedureShouldNotBeRenamed()
            {
                Assert.Equal(
                    "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1;\r\nEXEC CustomersByStatus @StatusId",
                    this.combinedQuery.CommandText);
            }
        }

        public class WhenCallingCombineAndTheSourceQueriesIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var sqlDialect = new MsSqlDialect();
                var exception = Assert.Throws<ArgumentNullException>(() => sqlDialect.Combine(null));

                Assert.Equal("sqlQueries", exception.ParamName);
            }
        }

        [MicroLite.Mapping.Table(schema: "Sales", name: "Customers")]
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
            [MicroLite.Mapping.Identifier(IdentifierStrategy.DbGenerated)]
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