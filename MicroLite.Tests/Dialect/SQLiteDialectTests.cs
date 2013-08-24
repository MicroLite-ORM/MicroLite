namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SQLiteDialect"/> class.
    /// </summary>
    public class SQLiteDialectTests : IDisposable
    {
        public SQLiteDialectTests()
        {
            // The tests in this suite all use attribute mapping for the test.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        public void Dispose()
        {
            // Reset the mapping convention after tests have run.
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [Fact]
        public void InsertQueryForDbGeneratedInstance()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.Equal("INSERT INTO \"Customers\" (\"Created\", \"DoB\", \"Name\", \"StatusId\") VALUES (@p0, @p1, @p2, @p3);SELECT last_insert_rowid()", sqlQuery.CommandText);
            Assert.Equal(customer.Created, sqlQuery.Arguments[0]);
            Assert.Equal(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.Equal(customer.Name, sqlQuery.Arguments[2]);
            Assert.Equal((int)customer.Status, sqlQuery.Arguments[3]);
        }

        [Fact]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT CustomerId, Name, DoB, StatusId FROM Customers LIMIT @p0,@p1", paged.CommandText);
            Assert.Equal(0, paged.Arguments[0]);////, "The first argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the number of records to return");
        }

        [Fact]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT * FROM Customers LIMIT @p0,@p1", paged.CommandText);
            Assert.Equal(0, paged.Arguments[0]);////, "The first argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the number of records to return");
        }

        [Fact]
        public void PageWithMultiWhereAndMultiOrderByMultiLine()
        {
            var sqlQuery = new SqlQuery(@"SELECT
 ""CustomerId"",
 ""Name"",
 ""DoB"",
 ""StatusId""
 FROM
 ""Customers""
 WHERE
 (""StatusId"" = @p0 AND ""DoB"" > @p1)
 ORDER BY
 ""Name"" ASC,
 ""DoB"" ASC", new object[] { CustomerStatus.Active, new DateTime(1980, 01, 01) });

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT \"CustomerId\", \"Name\", \"DoB\", \"StatusId\" FROM \"Customers\" WHERE (\"StatusId\" = @p0 AND \"DoB\" > @p1) ORDER BY \"Name\" ASC, \"DoB\" ASC LIMIT @p2,@p3", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(sqlQuery.Arguments[1], paged.Arguments[1]);////, "The second argument should be the second argument from the original query");
            Assert.Equal(0, paged.Arguments[2]);////, "The third argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[3]);////, "The fourth argument should be the number of records to return");
        }

        [Fact]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT \"CustomerId\", \"Name\", \"DoB\", \"StatusId\" FROM \"Customers\" ORDER BY \"CustomerId\" ASC");

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT \"CustomerId\", \"Name\", \"DoB\", \"StatusId\" FROM \"Customers\" ORDER BY \"CustomerId\" ASC LIMIT @p0,@p1", paged.CommandText);
            Assert.Equal(0, paged.Arguments[0]);////, "The first argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the number of records to return");
        }

        [Fact]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\"");

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" LIMIT @p0,@p1", paged.CommandText);
            Assert.Equal(0, paged.Arguments[0]);////, "The first argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the number of records to return");
        }

        [Fact]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\"");

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 25));

            Assert.Equal("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" LIMIT @p0,@p1", paged.CommandText);
            Assert.Equal(25, paged.Arguments[0]);////, "The first argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[1]);////, "The second argument should be the number of records to return");
        }

        [Fact]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = @p0 ORDER BY\"Name\" ASC", CustomerStatus.Active);

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = @p0 ORDER BY\"Name\" ASC LIMIT @p1,@p2", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(0, paged.Arguments[1]);////, "The second argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the number of records to return");
        }

        [Fact]
        public void PageWithWhereAndOrderByMultiLine()
        {
            var sqlQuery = new SqlQuery(@"SELECT
""CustomerId"",
""Name"",
""DoB"",
""StatusId""
 FROM
 ""Customers""
 WHERE
""StatusId"" = @p0
 ORDER BY
""Name"" ASC", new object[] { CustomerStatus.Active });

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = @p0 ORDER BY\"Name\" ASC LIMIT @p1,@p2", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(0, paged.Arguments[1]);////, "The second argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the number of records to return");
        }

        [Fact]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = @p0", CustomerStatus.Active);

            var sqlDialect = new SQLiteDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.Equal("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = @p0 LIMIT @p1,@p2", paged.CommandText);
            Assert.Equal(sqlQuery.Arguments[0], paged.Arguments[0]);////, "The first argument should be the first argument from the original query");
            Assert.Equal(0, paged.Arguments[1]);////, "The second argument should be the number of records to skip");
            Assert.Equal(25, paged.Arguments[2]);////, "The third argument should be the number of records to return");
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