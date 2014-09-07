namespace MicroLite.Tests.Dialect
{
    using System;
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

            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildUpdateSqlQuery(objectDelta);

            Assert.Equal("UPDATE Sales.Customers SET Name = ? WHERE Id = ?", sqlQuery.CommandText);
            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Fred Flintstone", sqlQuery.Arguments[0]);
            Assert.Equal(1234, sqlQuery.Arguments[1]);
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
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

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
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [dbo].[Customers]", countQuery.CommandText);
            Assert.Equal(0, countQuery.Arguments.Count);
        }

        [Fact]
        public void CountQueryWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ? ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);////, "The first argument should be the first argument from the original query");
        }

        [Fact]
        public void CountQueryWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", CustomerStatus.Active);

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var countQuery = mockSqlDialect.Object.CountQuery(sqlQuery);

            Assert.Equal("SELECT COUNT(*) FROM [Sales].[Customers] WHERE [Customers].[StatusId] = ?", countQuery.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], countQuery.Arguments[0]);////, "The first argument should be the first argument from the original query");
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

            var identifier = 134875;

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildDeleteSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("DELETE FROM Sales.Customers WHERE Id = ?", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(identifier, sqlQuery.Arguments[0]);

            // Do a second query to check that the caching doesn't cause a problem.
            identifier = 998866;

            var sqlQuery2 = mockSqlDialect.Object.BuildDeleteSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("DELETE FROM Sales.Customers WHERE Id = ?", sqlQuery2.CommandText);
            Assert.Equal(1, sqlQuery2.Arguments.Count);
            Assert.Equal(identifier, sqlQuery2.Arguments[0]);
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
                DateOfBirth = new DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?,?)", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[4]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[5]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6]);

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
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[2]);
            Assert.Equal(customer.Id, sqlQuery2.Arguments[3]);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[4]);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[5]);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[6]);
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
                DateOfBirth = new DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?)", sqlQuery.CommandText);
            Assert.Equal(6, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5]);

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
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[5]);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategySequence()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

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

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO Sales.Customers (Created,CreditLimit,DateOfBirth,Name,CustomerStatusId,Website) VALUES (?,?,?,?,?,?)", sqlQuery.CommandText);
            Assert.Equal(6, sqlQuery.Arguments.Count);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5]);

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
            Assert.Equal(customer.Created, sqlQuery2.Arguments[0]);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[1]);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[2]);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[3]);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[5]);
        }

        [Fact]
        public void SelectByIdentifierQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var identifier = 134875;

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildSelectSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("SELECT Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Updated,Website FROM Sales.Customers WHERE (Id = ?)", sqlQuery.CommandText);
            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(identifier, sqlQuery.Arguments[0]);

            // Do a second query to check that the caching doesn't cause a problem.
            identifier = 998866;

            var sqlQuery2 = mockSqlDialect.Object.BuildSelectSqlQuery(ObjectInfo.For(typeof(Customer)), identifier);

            Assert.Equal("SELECT Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Updated,Website FROM Sales.Customers WHERE (Id = ?)", sqlQuery2.CommandText);
            Assert.Equal(1, sqlQuery2.Arguments.Count);
            Assert.Equal(identifier, sqlQuery2.Arguments[0]);
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

            var mockSqlDialect = new Mock<SqlDialect>(SqlCharacters.Empty);
            mockSqlDialect.CallBase = true;

            var sqlQuery = mockSqlDialect.Object.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE Sales.Customers SET CreditLimit = ?,DateOfBirth = ?,Name = ?,CustomerStatusId = ?,Updated = ?,Website = ? WHERE Id = ?", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);
            Assert.Equal(customer.CreditLimit, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[3]);
            Assert.Equal(customer.Updated, sqlQuery.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[5]);
            Assert.Equal(customer.Id, sqlQuery.Arguments[6]);

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

            Assert.Equal("UPDATE Sales.Customers SET CreditLimit = ?,DateOfBirth = ?,Name = ?,CustomerStatusId = ?,Updated = ?,Website = ? WHERE Id = ?", sqlQuery2.CommandText);
            Assert.Equal(7, sqlQuery2.Arguments.Count);
            Assert.Equal(customer.CreditLimit, sqlQuery2.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery2.Arguments[1]);
            Assert.Equal(customer.Name, sqlQuery2.Arguments[2]);
            Assert.Equal((int)customer.Status, sqlQuery2.Arguments[3]);
            Assert.Equal(customer.Updated, sqlQuery2.Arguments[4]);
            Assert.Equal("http://microliteorm.wordpress.com/about", sqlQuery2.Arguments[5]);
            Assert.Equal(customer.Id, sqlQuery2.Arguments[6]);
        }
    }
}