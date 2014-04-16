namespace MicroLite.Tests.Dialect
{
    using System;
    using MicroLite.Builder;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MsSqlDialect"/> class.
    /// </summary>
    public class MsSqlDialectTests : UnitTest
    {
        [Fact]
        public void BuildSelectIdentitySqlQuery()
        {
            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.BuildSelectIdentitySqlQuery(ObjectInfo.For(typeof(Customer)));

            Assert.Equal("SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO [Sales].[Customers] ([Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Website]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[4]);
            Assert.Equal(1, sqlQuery.Arguments[5]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6]);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO [Sales].[Customers] ([Created],[CreditLimit],[DateOfBirth],[Name],[CustomerStatusId],[Website]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5)", sqlQuery.CommandText);
            Assert.Equal(6, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3]);
            Assert.Equal(1, sqlQuery.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5]);
        }

        [Fact]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DoB, StatusId FROM (SELECT CustomerId, Name, DoB, StatusId,ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageQueryThrowsArgumentNullExceptionIfSqlQueryIsNull()
        {
            var sqlDialect = new MsSqlDialect();

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlDialect.PageQuery(null, PagingOptions.ForPage(1, 10)));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        /// <summary>
        /// Issue #206 - Session.Paged errors if the query includes a sub query
        /// </summary>
        [Fact]
        public void PageQueryWithoutSubQuery()
        {
            SqlCharacters.Current = MsSqlCharacters.Instance;

            var sqlQuerySingleLevel = SqlBuilder
                                        .Select("*").From(typeof(Customer))
                                        .Where("Name LIKE @p0", "Fred%")
                                        .ToSqlQuery();

            var sqlDialect = new MsSqlDialect();

            SqlQuery pageQuerySingleLevel = sqlDialect.PageQuery(sqlQuerySingleLevel, PagingOptions.ForPage(page: 2, resultsPerPage: 10));
            Assert.Equal("SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website] FROM (SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Customers] WHERE (Name LIKE @p0)) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", pageQuerySingleLevel.CommandText);
            Assert.Equal("Fred%", pageQuerySingleLevel.Arguments[0]);
            Assert.Equal(11, pageQuerySingleLevel.Arguments[1]);
            Assert.Equal(20, pageQuerySingleLevel.Arguments[2]);
        }

        /// <summary>
        /// Issue #206 - Session.Paged errors if the query includes a sub query
        /// </summary>
        [Fact]
        public void PageQueryWithSubQuery()
        {
            SqlCharacters.Current = MsSqlCharacters.Instance;

            var sqlQuerySubQuery = SqlBuilder
                                        .Select("*")
                                        .From(typeof(Customer))
                                        .Where("Name LIKE @p0", "Fred%")
                                        .AndWhere("SourceId").In(new SqlQuery("SELECT SourceId FROM Source WHERE Status = @p0", 1))
                                        .ToSqlQuery();

            var sqlDialect = new MsSqlDialect();

            SqlQuery pageQuerySubQuery = sqlDialect.PageQuery(sqlQuerySubQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 10));
            Assert.Equal("SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website] FROM (SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Customers] WHERE (Name LIKE @p0) AND ([SourceId] IN (SELECT SourceId FROM Source WHERE Status = @p1))) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", pageQuerySubQuery.CommandText);
            Assert.Equal("Fred%", pageQuerySubQuery.Arguments[0]);
            Assert.Equal(1, pageQuerySubQuery.Arguments[1]);
            Assert.Equal(11, pageQuerySubQuery.Arguments[2]);
            Assert.Equal(20, pageQuerySubQuery.Arguments[3]);
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

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC, [Customers].[DoB] ASC) AS RowNumber FROM [Sales].[Customers] WHERE ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", paged.CommandText);
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

            Assert.Equal("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM (SELECT [CustomerId], [Name], [DoB], [StatusId],ROW_NUMBER() OVER(ORDER BY [CustomerId] ASC) AS RowNumber FROM [dbo].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(1, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.Equal(26, paged.Arguments[0]);////, "The first argument should be the start row number");
            Assert.Equal(50, paged.Arguments[1]);////, "The second argument should be the end row number");
        }

        [Fact]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
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

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
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

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(1, paged.Arguments[1]);////, "The second argument should be the start row number");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the end row number");
        }

        [Fact]
        public void SupportsIdentityReturnsTrue()
        {
            var sqlDialect = new MsSqlDialect();

            Assert.True(sqlDialect.SupportsIdentity);
        }

        [Fact]
        public void UpdateInstanceQueryForIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE [Sales].[Customers] SET [CreditLimit] = @p0,[DateOfBirth] = @p1,[Name] = @p2,[CustomerStatusId] = @p3,[Updated] = @p4,[Website] = @p5 WHERE [Id] = @p6", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2]);
            Assert.Equal(1, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[6]);
        }

        [Fact]
        public void UpdateInstanceQueryForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE [Sales].[Customers] SET [CreditLimit] = @p0,[DateOfBirth] = @p1,[Name] = @p2,[CustomerStatusId] = @p3,[Updated] = @p4,[Website] = @p5 WHERE [Id] = @p6", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2]);
            Assert.Equal(1, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[6]);
        }
    }
}