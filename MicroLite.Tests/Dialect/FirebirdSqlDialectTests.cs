namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FirebirdSqlDialect"/> class.
    /// </summary>
    public class FirebirdSqlDialectTests : UnitTest
    {
        [Fact]
        public void BuildSelectInsertIdSqlQuery()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = sqlDialect.BuildSelectInsertIdSqlQuery(ObjectInfo.For(typeof(Customer)));

            Assert.Equal(string.Empty, sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var sqlDialect = new FirebirdSqlDialect();

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Version = 233,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO \"Sales\".\"Customers\" (\"Created\",\"CreditLimit\",\"DateOfBirth\",\"Id\",\"Name\",\"CustomerStatusId\",\"Version\",\"Website\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7)", sqlQuery.CommandText);
            Assert.Equal(8, sqlQuery.Arguments.Count);

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

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[6].DbType);
            Assert.Equal(customer.Version, sqlQuery.Arguments[6].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[7].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[7].Value);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var sqlDialect = new FirebirdSqlDialect();

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Version = 233,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO \"Sales\".\"Customers\" (\"Created\",\"CreditLimit\",\"DateOfBirth\",\"Name\",\"CustomerStatusId\",\"Version\",\"Website\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6) RETURNING Id", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);

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

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[5].DbType);
            Assert.Equal(customer.Version, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[6].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6].Value);
        }

        [Fact]
        public void InsertInstanceQueryForIdentifierStrategySequence()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

            var sqlDialect = new FirebirdSqlDialect();

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Version = 233,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = sqlDialect.BuildInsertSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("INSERT INTO \"Sales\".\"Customers\" (\"Id\",\"Created\",\"CreditLimit\",\"DateOfBirth\",\"Name\",\"CustomerStatusId\",\"Version\",\"Website\") VALUES (GEN_ID(Customer_Id_Sequence, 1),@p0,@p1,@p2,@p3,@p4,@p5,@p6) RETURNING Id", sqlQuery.CommandText);
            Assert.Equal(7, sqlQuery.Arguments.Count);

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

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[5].DbType);
            Assert.Equal(customer.Version, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[6].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6].Value);
        }

        [Fact]
        public void PageNonQualifiedQuery()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers ROWS @p0 TO @p1", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT * FROM Customers ROWS @p0 TO @p1", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageQueryThrowsArgumentNullExceptionForNullSqlCharacters()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlDialect.PageQuery(null, PagingOptions.None));
        }

        [Fact]
        public void PageWithMultiWhereAndMultiOrderByMultiLine()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery(@"SELECT
 CustomerId,
 Name,
 DateOfBirth,
 CustomerStatusId
 FROM
 Customers
 WHERE
 (CustomerStatusId = @p0 AND DoB > @p1)
 ORDER BY
 Name ASC,
 DoB ASC", CustomerStatus.Active, new DateTime(1980, 01, 01));

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers WHERE (CustomerStatusId = @p0 AND DoB > @p1) ORDER BY Name ASC, DoB ASC ROWS @p2 TO @p3", paged.CommandText);
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
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers ORDER BY CustomerId ASC");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers ORDER BY CustomerId ASC ROWS @p0 TO @p1", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers ROWS @p0 TO @p1", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(1, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(25, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers");

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers ROWS @p0 TO @p1", paged.CommandText);

            Assert.Equal(DbType.Int32, paged.Arguments[0].DbType);
            Assert.Equal(26, paged.Arguments[0].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(50, paged.Arguments[1].Value);
        }

        [Fact]
        public void PageWithWhereAndOrderBy()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers WHERE CustomerStatusId = @p0 ORDER BY Name ASC", CustomerStatus.Active);

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers WHERE CustomerStatusId = @p0 ORDER BY Name ASC ROWS @p1 TO @p2", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(25, paged.Arguments[2].Value);
        }

        [Fact]
        public void PageWithWhereAndOrderByMultiLine()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery(@"SELECT
 CustomerId,
 Name,
 DateOfBirth,
 CustomerStatusId
 FROM
 Customers
 WHERE
 CustomerStatusId = @p0
 ORDER BY
 Name ASC", CustomerStatus.Active);

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers WHERE CustomerStatusId = @p0 ORDER BY Name ASC ROWS @p1 TO @p2", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(25, paged.Arguments[2].Value);
        }

        [Fact]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlDialect = new FirebirdSqlDialect();

            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers WHERE CustomerStatusId = @p0", CustomerStatus.Active);

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DateOfBirth, CustomerStatusId FROM Customers WHERE CustomerStatusId = @p0 ROWS @p1 TO @p2", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);

            Assert.Equal(DbType.Int32, paged.Arguments[1].DbType);
            Assert.Equal(1, paged.Arguments[1].Value);

            Assert.Equal(DbType.Int32, paged.Arguments[2].DbType);
            Assert.Equal(25, paged.Arguments[2].Value);
        }

        [Fact]
        public void SupportsSelectInsertedIdentifierReturnsFalse()
        {
            var sqlDialect = new FirebirdSqlDialect();

            Assert.False(sqlDialect.SupportsSelectInsertedIdentifier);
        }

        [Fact]
        public void UpdateInstanceQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var sqlDialect = new FirebirdSqlDialect();

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Version = 233,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(Customer)), customer);

            Assert.Equal("UPDATE \"Sales\".\"Customers\" SET \"CreditLimit\" = @p0,\"DateOfBirth\" = @p1,\"Name\" = @p2,\"CustomerStatusId\" = @p3,\"Updated\" = @p4,\"Version\" = @p5,\"Website\" = @p6 WHERE (\"Version\" = @p7) AND (\"Id\" = @p8)", sqlQuery.CommandText);
            Assert.Equal(9, sqlQuery.Arguments.Count);

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

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[5].DbType);
            Assert.Equal(customer.Version + 1, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[6].DbType);
            Assert.Equal("http://microliteorm.wordpress.com/", sqlQuery.Arguments[6].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[7].DbType);
            Assert.Equal(customer.Version, sqlQuery.Arguments[7].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[8].DbType);
            Assert.Equal(customer.Id, sqlQuery.Arguments[8].Value);

            var customerShort = new CustomerCustomVersion<ushort>
            {
                Version = 233,
            };

            sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(CustomerCustomVersion<ushort>)), customerShort);

            Assert.Equal(DbType.UInt16, sqlQuery.Arguments[5].DbType);
            Assert.Equal((ushort)(customerShort.Version + 1), sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.UInt16, sqlQuery.Arguments[7].DbType);
            Assert.Equal(customerShort.Version, sqlQuery.Arguments[7].Value);

            var customerLong = new CustomerCustomVersion<ulong>
            {
                Version = 233,
            };

            sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(CustomerCustomVersion<ulong>)), customerLong);

            Assert.Equal(DbType.UInt64, sqlQuery.Arguments[5].DbType);
            Assert.Equal(customerLong.Version + 1, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.UInt64, sqlQuery.Arguments[7].DbType);
            Assert.Equal(customerLong.Version, sqlQuery.Arguments[7].Value);

            var customerDateTime = new CustomerCustomVersion<DateTime>
            {
                Version = new DateTime(1921, 1, 1, 1, 1, 1, DateTimeKind.Utc),
            };

            var now = DateTime.UtcNow;

            sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(CustomerCustomVersion<DateTime>)), customerDateTime);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[5].DbType);
            Assert.InRange((DateTime)sqlQuery.Arguments[5].Value, now, DateTime.UtcNow);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[7].DbType);
            Assert.Equal(customerDateTime.Version, sqlQuery.Arguments[7].Value);
        }

        [Fact]
        public void UpdateWithoutVersionInstanceQuery()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var sqlDialect = new FirebirdSqlDialect();

            var customer = new CustomerWithoutVersion
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

            var sqlQuery = sqlDialect.BuildUpdateSqlQuery(ObjectInfo.For(typeof(CustomerWithoutVersion)), customer);

            Assert.Equal("UPDATE \"Sales\".\"CustomerWithoutVersions\" SET \"CreditLimit\" = @p0,\"DateOfBirth\" = @p1,\"Name\" = @p2,\"CustomerStatusId\" = @p3,\"Updated\" = @p4,\"Website\" = @p5 WHERE (\"Id\" = @p6)", sqlQuery.CommandText);
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