namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SQLiteDialect"/> class.
    /// </summary>
    [TestFixture]
    public class SQLiteDialectTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void DeleteQueryForInstance()
        {
            var customer = new CustomerWithIdentity
            {
                Id = 122672
            };

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Delete, customer);

            Assert.AreEqual("DELETE FROM [Customers] WHERE [CustomerId] = @p0", sqlQuery.CommandText);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[0]);
        }

        [Test]
        public void DeleteQueryForTypeByIdentifier()
        {
            object identifier = 239845763;

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Delete, typeof(CustomerWithIdentity), identifier);

            Assert.AreEqual("DELETE FROM [Customers] WHERE [CustomerId] = @p0", sqlQuery.CommandText);
            Assert.AreEqual(identifier, sqlQuery.Arguments[0]);
        }

        /// <summary>
        /// Issue #11 - Identifier property value should be included on insert for IdentifierStrategy.Assigned.
        /// </summary>
        [Test]
        public void InsertQueryForAssignedInstance()
        {
            var customer = new CustomerWithAssigned
            {
                Id = 6245543,
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO [Customers] ([CustomerId], [Name], [StatusId]) VALUES (@p0, @p1, @p2)", sqlQuery.CommandText);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[1]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[2]);
        }

        [Test]
        public void InsertQueryForIdentityInstance()
        {
            var customer = new CustomerWithIdentity
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO [Customers] ([Created], [DoB], [Name], [StatusId]) VALUES (@p0, @p1, @p2, @p3)", sqlQuery.CommandText);
            Assert.AreEqual(customer.Created, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[2]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[3]);
        }

        [Test]
        public void InsertQueryForAutoIncrementInstance()
        {
            var customer = new CustomerWithAutoIncrement
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO [Customers] ([Created], [DoB], [Name], [StatusId]) VALUES (@p0, @p1, @p2, @p3)", sqlQuery.CommandText);
            Assert.AreEqual(customer.Created, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[2]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[3]);
        }

        [Test]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT CustomerId, Name, DoB, StatusId FROM Customers LIMIT @p0,@p1", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT * FROM Customers LIMIT @p0,@p1", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithMultiWhereAndMultiOrderByMultiLine()
        {
            var sqlQuery = new SqlQuery(@"SELECT
 [Customers].[CustomerId],
 [Customers].[Name],
 [Customers].[DoB],
 [Customers].[StatusId]
 FROM
 [Customers]
 WHERE
 ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)
 ORDER BY
 [Customers].[Name] ASC,
 [Customers].[DoB] ASC", new object[] { CustomerStatus.Active, new DateTime(1980, 01, 01) });

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] WHERE ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1) ORDER BY [Customers].[Name] ASC, [Customers].[DoB] ASC LIMIT @p2,@p3", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(sqlQuery.Arguments[1], paged.Arguments[1], "The second argument should be the second argument from the original query");
            Assert.AreEqual(1, paged.Arguments[2], "The third argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[3], "The fourth argument should be the end row number");
        }

        [Test]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [Customers] ORDER BY [CustomerId] ASC");

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [Customers] ORDER BY [CustomerId] ASC LIMIT @p0,@p1", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers]");

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] LIMIT @p0,@p1", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers]");

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 2, resultsPerPage: 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] LIMIT @p0,@p1", paged.CommandText);
            Assert.AreEqual(26, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(50, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC LIMIT @p1,@p2", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(1, paged.Arguments[1], "The second argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[2], "The third argument should be the end row number");
        }

        [Test]
        public void PageWithWhereAndOrderByMultiLine()
        {
            var sqlQuery = new SqlQuery(@"SELECT
 [Customers].[CustomerId],
 [Customers].[Name],
 [Customers].[DoB],
 [Customers].[StatusId]
 FROM
 [Customers]
 WHERE
 [Customers].[StatusId] = @p0
 ORDER BY
 [Customers].[Name] ASC", new object[] { CustomerStatus.Active });

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC LIMIT @p1,@p2", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(1, paged.Arguments[1], "The second argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[2], "The third argument should be the end row number");
        }

        [Test]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] WHERE [Customers].[StatusId] = @p0", CustomerStatus.Active);

            var sqlDialect = new SQLiteDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, page: 1, resultsPerPage: 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Customers] WHERE [Customers].[StatusId] = @p0 LIMIT @p1,@p2", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(1, paged.Arguments[1], "The second argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[2], "The third argument should be the end row number");
        }

        [Test]
        public void SelectQuery()
        {
            object identifier = 12345421;

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Select, typeof(CustomerWithIdentity), identifier);

            Assert.AreEqual("SELECT [Created], [DoB], [CustomerId], [Name], [StatusId], [Updated] FROM [Customers] WHERE [CustomerId] = @p0", sqlQuery.CommandText);
            Assert.AreEqual(identifier, sqlQuery.Arguments[0]);
        }

        [Test]
        public void UpdateQuery()
        {
            var customer = new CustomerWithIdentity
            {
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Id = 134875,
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now
            };

            var sqlDialect = new SQLiteDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Update, customer);

            Assert.AreEqual("UPDATE [Customers] SET [DoB] = @p0, [Name] = @p1, [StatusId] = @p2, [Updated] = @p3 WHERE [CustomerId] = @p4", sqlQuery.CommandText);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[1]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[2]);
            Assert.AreEqual(customer.Updated, sqlQuery.Arguments[3]);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[4]);
        }

        [MicroLite.Mapping.Table(schema: "Marketing", name: "Customers")]
        private class CustomerWithAssigned
        {
            public CustomerWithAssigned()
            {
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(IdentifierStrategy.Assigned)]
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
        }

        [MicroLite.Mapping.Table(schema: "Sales", name: "Customers")]
        private class CustomerWithIdentity
        {
            public CustomerWithIdentity()
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
            [MicroLite.Mapping.Identifier(IdentifierStrategy.Identity)]
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

        [MicroLite.Mapping.Table(schema: "Sales", name: "Customers")]
        private class CustomerWithAutoIncrement
        {
            public CustomerWithAutoIncrement()
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