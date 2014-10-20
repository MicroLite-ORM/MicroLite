namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MsSql2005Dialect"/> class.
    /// </summary>
    public class MsSql2005DialectTests : UnitTest
    {
        [Fact]
        public void BuildSelectInsertIdSqlQueryForIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = sqlDialect.BuildSelectInsertIdSqlQuery(ObjectInfo.For(typeof(Customer)));

            Assert.Equal("SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void BuildSelectInsertIdSqlQueryForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = sqlDialect.BuildSelectInsertIdSqlQuery(ObjectInfo.For(typeof(Customer)));

            Assert.Equal("SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var sqlDialect = new MsSql2005Dialect();

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

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO [Sales].[Customers] ([Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Website]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6)", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(customer.Id, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[4].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[5].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[6].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6].Value);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var sqlDialect = new MsSql2005Dialect();

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO [Sales].[Customers] ([Created],[CreditLimit],[DateOfBirth],[Name],[CustomerStatusId],[Website]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5)", sqlQuery.CommandText);
            Assert.Equal(6, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[3].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[4].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5].Value);
        }

        [Fact]
        public void PageNonQualifiedQuery()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DoB, StatusId FROM (SELECT CustomerId, Name, DoB, StatusId,ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT * FROM (SELECT *,ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageQueryThrowsArgumentNullExceptionIfSqlQueryIsNull()
        {
            var sqlDialect = new MsSql2005Dialect();

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
            var sqlDialect = new MsSql2005Dialect();

            SqlCharacters.Current = MsSqlCharacters.Instance;

            var sqlQuery = SqlBuilder
                .Select("*").From(typeof(Customer))
                .Where("Name LIKE @p0", "Fred%")
                .ToSqlQuery();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 10));

            Assert.Equal("SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website] FROM (SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Customers] WHERE (Name LIKE @p0)) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);

            Assert.Equal(DbType.String, paged.Arguments[0].DbType);
            Assert.Equal("Fred%", paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(11, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(20, paged.Arguments[2].Value);
        }

        /// <summary>
        /// Issue #206 - Session.Paged errors if the query includes a sub query
        /// </summary>
        [Fact]
        public void PageQueryWithSubQuery()
        {
            var sqlDialect = new MsSql2005Dialect();

            SqlCharacters.Current = MsSqlCharacters.Instance;

            var sqlQuery = SqlBuilder
                .Select("*")
                .From(typeof(Customer))
                .Where("Name LIKE @p0", "Fred%")
                .AndWhere("SourceId").In(new SqlQuery("SELECT SourceId FROM Source WHERE Status = @p0", 1))
                .ToSqlQuery();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 10));

            Assert.Equal("SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website] FROM (SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Customers] WHERE (Name LIKE @p0) AND ([SourceId] IN (SELECT SourceId FROM Source WHERE Status = @p1))) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", paged.CommandText);

            Assert.Equal(DbType.String, paged.Arguments[0].DbType);
            Assert.Equal("Fred%", paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(11, paged.Arguments[2].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[3].DbType);
            Assert.Equal(20, paged.Arguments[3].Value);
        }

        [Fact]
        public void PageWithMultiWhereAndMultiOrderByMultiLine()
        {
            var sqlDialect = new MsSql2005Dialect();

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
[Customers].[DoB] ASC", CustomerStatus.Active, new DateTime(1980, 01, 01));

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC, [Customers].[DoB] ASC) AS RowNumber FROM [Sales].[Customers] WHERE ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", paged.CommandText);

            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);
            Assert.Equal(sqlQuery.Arguments[1], paged.Arguments[1]);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(1, paged.Arguments[2].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[3].DbType);
            Assert.Equal(25, paged.Arguments[3].Value);
        }

        [Fact]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM (SELECT [CustomerId], [Name], [DoB], [StatusId],ROW_NUMBER() OVER(ORDER BY [CustomerId] ASC) AS RowNumber FROM [dbo].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(26, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(50, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageWithWhereAndOrderBy()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(25, paged.Arguments[2].Value);
        }

        [Fact]
        public void PageWithWhereAndOrderByMultiLine()
        {
            var sqlDialect = new MsSql2005Dialect();

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
[Customers].[Name] ASC", CustomerStatus.Active);

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(25, paged.Arguments[2].Value);
        }

        [Fact]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlDialect = new MsSql2005Dialect();

            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", CustomerStatus.Active);

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId],ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(25, paged.Arguments[2].Value);
        }

        [Fact]
        public void SupportsSelectInsertedIdentifierReturnsTrue()
        {
            var sqlDialect = new MsSql2005Dialect();

            Assert.True(sqlDialect.SupportsSelectInsertedIdentifier);
        }

        [Fact]
        public void UpdateInstanceQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var sqlDialect = new MsSql2005Dialect();

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

            var sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE [Sales].[Customers] SET [CreditLimit] = @p0,[DateOfBirth] = @p1,[Name] = @p2,[CustomerStatusId] = @p3,[Updated] = @p4,[Website] = @p5 WHERE [Id] = @p6", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[4].DbType);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[6].DbType);
            Assert.Equal(customer.Id, sqlQuery.Arguments[6].Value);
        }
    }
}