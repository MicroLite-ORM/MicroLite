﻿namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="PostgreSqlDialect"/> class.
    /// </summary>
    [TestFixture]
    public class PostgreSqlDialectTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void InsertQueryForAutoIncrementInstance()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new PostgreSqlDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO \"Customers\" (\"Created\", \"DoB\", \"Name\", \"StatusId\") VALUES (:p0, :p1, :p2, :p3);SELECT lastval()", sqlQuery.CommandText);
            Assert.AreEqual(customer.Created, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[2]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[3]);
        }

        [Test]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT CustomerId, Name, DoB, StatusId FROM Customers LIMIT :p0 OFFSET :p1", paged.CommandText);
            Assert.AreEqual(25, paged.Arguments[0], "The first argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[1], "The second argument should be the number of records to skip");
        }

        [Test]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT * FROM Customers LIMIT :p0 OFFSET :p1", paged.CommandText);
            Assert.AreEqual(25, paged.Arguments[0], "The first argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[1], "The second argument should be the number of records to skip");
        }

        [Test]
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
 (""StatusId"" = :p0 AND ""DoB"" > :p1)
 ORDER BY
 ""Name"" ASC,
 ""DoB"" ASC", new object[] { CustomerStatus.Active, new DateTime(1980, 01, 01) });

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT \"CustomerId\", \"Name\", \"DoB\", \"StatusId\" FROM \"Customers\" WHERE (\"StatusId\" = :p0 AND \"DoB\" > :p1) ORDER BY \"Name\" ASC, \"DoB\" ASC LIMIT :p2 OFFSET :p3", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(sqlQuery.Arguments[1], paged.Arguments[1], "The second argument should be the second argument from the original query");
            Assert.AreEqual(25, paged.Arguments[2], "The third argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[3], "The fourth argument should be the number of records to skip");
        }

        [Test]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT \"CustomerId\", \"Name\", \"DoB\", \"StatusId\" FROM \"Customers\" ORDER BY \"CustomerId\" ASC");

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT \"CustomerId\", \"Name\", \"DoB\", \"StatusId\" FROM \"Customers\" ORDER BY \"CustomerId\" ASC LIMIT :p0 OFFSET :p1", paged.CommandText);
            Assert.AreEqual(25, paged.Arguments[0], "The first argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[1], "The second argument should be the number of records to skip");
        }

        [Test]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\"");

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" LIMIT :p0 OFFSET :p1", paged.CommandText);
            Assert.AreEqual(25, paged.Arguments[0], "The first argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[1], "The second argument should be the number of records to skip");
        }

        [Test]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\"");

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 2, resultsPerPage: 25);

            Assert.AreEqual("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" LIMIT :p0 OFFSET :p1", paged.CommandText);
            Assert.AreEqual(25, paged.Arguments[0], "The first argument should be the number of records to return");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the number of records to skip");
        }

        [Test]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = :p0 ORDER BY\"Name\" ASC", CustomerStatus.Active);

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = :p0 ORDER BY\"Name\" ASC LIMIT :p1 OFFSET :p2", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[2], "The third argument should be the number of records to skip");
        }

        [Test]
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
""StatusId"" = :p0
 ORDER BY
""Name"" ASC", new object[] { CustomerStatus.Active });

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = :p0 ORDER BY\"Name\" ASC LIMIT :p1 OFFSET :p2", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[2], "The third argument should be the number of records to skip");
        }

        [Test]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = :p0", CustomerStatus.Active);

            var sqlDialect = new PostgreSqlDialect();

            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT\"CustomerId\",\"Name\",\"DoB\",\"StatusId\" FROM \"Customers\" WHERE\"StatusId\" = :p0 LIMIT :p1 OFFSET :p2", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the number of records to return");
            Assert.AreEqual(0, paged.Arguments[2], "The third argument should be the number of records to skip");
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
            [MicroLite.Mapping.Identifier(IdentifierStrategy.AutoIncrement)]
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