namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Characters;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlDialect"/> class.
    /// </summary>
    public class SqlDialectTests : UnitTest
    {
        [Fact]
        public void BuildDeleteSqlQueryThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.BuildDeleteSqlQuery(null, 12345));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BuildInsertSqlQueryThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.BuildInsertSqlQuery(null, new Customer()));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BuildSelectInsertIdSqlQueryReturnsSqlQueryWithEmptyCommandTextAndNoArguments()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildSelectInsertIdSqlQuery(ObjectInfo.For(typeof(Customer)));

            Assert.Equal(string.Empty, sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void BuildSelectSqlQueryThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.BuildSelectSqlQuery(null, 12345));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BuildUpdateSqlQueryForObjectDelta()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var sqlQuery = mockSqlDialect.Object.BuildUpdateSqlQuery(objectDelta);

            Assert.Equal("UPDATE Sales.Customers SET Name = ? WHERE (Id = ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Fred Flintstone", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1234, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void BuildUpdateSqlQueryThrowsArgumentNullExceptionForNullObjectDelta()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.BuildUpdateSqlQuery(null));

            Assert.Equal("objectDelta", exception.ParamName);
        }

        [Fact]
        public void BuildUpdateSqlQueryThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.BuildUpdateSqlQuery(null, new Customer()));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void CountQueryNoWhereOrOrderBy()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM Customers", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        /// <summary>
        /// Issue #400 - Parameter matching incompatible with T-SQL geography types
        /// </summary>
        /// <remarks>
        /// The actual issue description isn't quite accurate - the underlying problem was down to the fact that
        /// when we create a count query, we expect no args if there is no where clause but in this case the select
        /// contains parameters which meant that the args have a value and as a result when we combine the queries
        /// we ended up with 4 parameter names and 6 args.
        /// </remarks>
        [Fact]
        public void CountQueryNoWhereOrOrderByAndWithParameters()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, ? FROM Customers", "Test");

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM Customers", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.CountQuery(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void CountQueryWithNoWhereButOrderBy()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [dbo].[Customers]", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryWithWhereAndOrderBy()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ? ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);
        }

        [Fact]
        public void CountQueryWithWhereButNoOrderBy()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", CustomerStatus.Active);

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);
        }

        [Fact]
        public void CreateQueryThrowsArgumentNullExceptionForNullInstance()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void DeleteByIdentifierQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var identifier = 134875;

            var sqlQuery = mockSqlDialect.Object.BuildDeleteSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("DELETE FROM Sales.Customers WHERE (Id = ?)", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(identifier, sqlQuery.Arguments[0].Value);

            // Do a second query to check that the caching doesn't cause a problem.
            identifier = 998866;

            var sqlQuery2 = mockSqlDialect.Object.BuildDeleteSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("DELETE FROM Sales.Customers WHERE (Id = ?)", sqlQuery2.CommandText);
            Assert.Equal(1, sqlQuery2.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[0].DbType);
            Assert.Equal(identifier, sqlQuery2.Arguments[0].Value);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?,?)", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(customer.Id, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[4].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[5].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[6].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6].Value);

            // Do a second query to check that the caching doesn't cause a problem.
            customer = new Customer
            {
                Created = new DateTime(2012, 08, 13),
                CreditLimit = 6250.00M,
                DateOfBirth = new DateTime(1984, 3, 11),
                Id = 998866,
                Name = "John Smith",
                Status = CustomerStatus.Inactive,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com/about")
            };

            var sqlQuery2 = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?,?)", sqlQuery2.CommandText);
            Assert.Equal(7, sqlQuery2.Arguments.Count);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery2.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[3].DbType);
            Assert.Equal(customer.Id, sqlQuery2.Arguments[3].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[4].DbType);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[4].Value);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[5].DbType);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[5].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[6].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[6].Value);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?)", sqlQuery.CommandText);
            Assert.Equal(6, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[3].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[4].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5].Value);

            // Do a second query to check that the caching doesn't cause a problem.
            customer = new Customer
            {
                Created = new DateTime(2012, 08, 13),
                CreditLimit = 6250.00M,
                DateOfBirth = new DateTime(1984, 3, 11),
                Id = 998866,
                Name = "John Smith",
                Status = CustomerStatus.Inactive,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com/about")
            };

            var sqlQuery2 = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?)", sqlQuery2.CommandText);
            Assert.Equal(6, sqlQuery2.Arguments.Count);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery2.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[2].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[3].DbType);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[4].DbType);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[5].Value);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategySequence()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?)", sqlQuery.CommandText);
            Assert.Equal(6, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[3].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[4].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5].Value);

            // Do a second query to check that the caching doesn't cause a problem.
            customer = new Customer
            {
                Created = new DateTime(2012, 08, 13),
                CreditLimit = 6250.00M,
                DateOfBirth = new DateTime(1984, 3, 11),
                Id = 998866,
                Name = "John Smith",
                Status = CustomerStatus.Inactive,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com/about")
            };

            var sqlQuery2 = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?)", sqlQuery2.CommandText);
            Assert.Equal(6, sqlQuery2.Arguments.Count);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[0].DbType);
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery2.Arguments[1].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[2].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[2].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[3].DbType);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[4].DbType);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[5].Value);
        }

        [Fact]
        public void SelectByIdentifierQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var identifier = 134875;

            var sqlQuery = mockSqlDialect.Object.BuildSelectSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("SELECT Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Updated,Website FROM Sales.Customers WHERE (Id = ?)", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(identifier, sqlQuery.Arguments[0].Value);

            // Do a second query to check that the caching doesn't cause a problem.
            identifier = 998866;

            var sqlQuery2 = mockSqlDialect.Object.BuildSelectSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("SELECT Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Updated,Website FROM Sales.Customers WHERE (Id = ?)", sqlQuery2.CommandText);
            Assert.Equal(1, sqlQuery2.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[0].DbType);
            Assert.Equal(identifier, sqlQuery2.Arguments[0].Value);
        }

        [Fact]
        public void SqlCharactersPassedToConstructorAreExposed()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            Assert.Same(SqlCharacters.Empty, mockSqlDialect.Object.SqlCharacters);
        }

        [Fact]
        public void SupportsSelectInsertedIdentifierReturnsFalseByDefault()
        {
            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            Assert.False(mockSqlDialect.Object.SupportsSelectInsertedIdentifier);
        }

        [Fact]
        public void UpdateInstanceQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = mockSqlDialect.Object.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE Sales.Customers SET CreditLimit = ?,DateOfBirth = ?,Name = ?,CustomerStatusId = ?,Updated = ?,Website = ? WHERE (Id = ?)", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[0].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[1].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[2].DbType);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[4].DbType);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[6].DbType);
            Assert.Equal(customer.Id, sqlQuery.Arguments[6].Value);

            // Do a second query to check that the caching doesn't cause a problem.
            customer = new Customer
            {
                Created = new DateTime(2012, 08, 13),
                CreditLimit = 6250.00M,
                DateOfBirth = new DateTime(1984, 3, 11),
                Id = 998866,
                Name = "John Smith",
                Status = CustomerStatus.Inactive,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com/about")
            };

            var sqlQuery2 = mockSqlDialect.Object.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE Sales.Customers SET CreditLimit = ?,DateOfBirth = ?,Name = ?,CustomerStatusId = ?,Updated = ?,Website = ? WHERE (Id = ?)", sqlQuery2.CommandText);
            Assert.Equal(7, sqlQuery2.Arguments.Count);

            Assert.Equal(DbType.Decimal, sqlQuery2.Arguments[0].DbType);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[0].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[1].DbType);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[1].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[2].DbType);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[3].DbType);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[3].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery2.Arguments[4].DbType);
            Assert.Equal(customer.Updated, sqlQuery2.Arguments[4].Value);

            Assert.Equal(DbType.String, sqlQuery2.Arguments[5].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[5].Value);

            Assert.Equal(DbType.Int32, sqlQuery2.Arguments[6].DbType);
            Assert.Equal(customer.Id, sqlQuery2.Arguments[6].Value);
        }
    }
}