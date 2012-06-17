namespace MicroLite.Tests.Core
{
    using System;
    using MicroLite.Core;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlQueryBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class SqlQueryBuilderTests
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

            var queryBuilder = new SqlQueryBuilder();

            var sqlQuery = queryBuilder.DeleteQuery(customer);

            Assert.AreEqual("DELETE FROM [Sales].[Customers] WHERE [Customers].[CustomerId] = @p0", sqlQuery.CommandText);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[0]);
        }

        [Test]
        public void DeleteQueryForTypeByIdentifier()
        {
            object identifier = 239845763;

            var queryBuilder = new SqlQueryBuilder();

            var sqlQuery = queryBuilder.DeleteQuery(typeof(CustomerWithIdentity), identifier);

            Assert.AreEqual("DELETE FROM [Sales].[Customers] WHERE [Customers].[CustomerId] = @p0", sqlQuery.CommandText);
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

            var queryBuilder = new SqlQueryBuilder();

            var sqlQuery = queryBuilder.InsertQuery(customer);

            Assert.AreEqual("INSERT INTO [Marketing].[Customers] ([Customers].[CustomerId], [Customers].[Name], [Customers].[StatusId]) VALUES (@p0, @p1, @p2)", sqlQuery.CommandText);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[1]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[2]);
        }

        [Test]
        public void InsertQueryForIdentityInstance()
        {
            var customer = new CustomerWithIdentity
            {
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var queryBuilder = new SqlQueryBuilder();

            var sqlQuery = queryBuilder.InsertQuery(customer);

            Assert.AreEqual("INSERT INTO [Sales].[Customers] ([Customers].[DoB], [Customers].[Name], [Customers].[StatusId]) VALUES (@p0, @p1, @p2)", sqlQuery.CommandText);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[1]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[2]);
        }

        [Test]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT CustomerId, Name, DoB, StatusId FROM (SELECT CustomerId, Name, DoB, StatusId, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], 1);
            Assert.AreEqual(paged.Arguments[1], 25);
        }

        [Test]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], 1);
            Assert.AreEqual(paged.Arguments[1], 25);
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
 [Sales].[Customers]
 WHERE
 ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)
 ORDER BY
 [Customers].[Name] ASC,
 [Customers].[DoB] ASC", new object[] { CustomerStatus.Active, new DateTime(1980, 01, 01) });

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC, [Customers].[DoB] ASC) AS RowNumber FROM [Sales].[Customers] WHERE ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], sqlQuery.Arguments[0]);
            Assert.AreEqual(paged.Arguments[1], sqlQuery.Arguments[1]);
            Assert.AreEqual(paged.Arguments[2], 1);
            Assert.AreEqual(paged.Arguments[3], 25);
        }

        [Test]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM (SELECT [CustomerId], [Name], [DoB], [StatusId], ROW_NUMBER() OVER(ORDER BY [CustomerId] ASC) AS RowNumber FROM [dbo].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], 1);
            Assert.AreEqual(paged.Arguments[1], 25);
        }

        [Test]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], 1);
            Assert.AreEqual(paged.Arguments[1], 25);
        }

        [Test]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 2, 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], 26);
            Assert.AreEqual(paged.Arguments[1], 50);
        }

        [Test]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], sqlQuery.Arguments[0]);
            Assert.AreEqual(paged.Arguments[1], 1);
            Assert.AreEqual(paged.Arguments[2], 25);
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
 [Sales].[Customers]
 WHERE
 [Customers].[StatusId] = @p0
 ORDER BY
 [Customers].[Name] ASC", new object[] { CustomerStatus.Active });

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], sqlQuery.Arguments[0]);
            Assert.AreEqual(paged.Arguments[1], 1);
            Assert.AreEqual(paged.Arguments[2], 25);
        }

        [Test]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", CustomerStatus.Active);

            var queryBuilder = new SqlQueryBuilder();
            var paged = queryBuilder.Page(sqlQuery, 1, 25);

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.AreEqual(paged.Arguments[0], sqlQuery.Arguments[0]);
            Assert.AreEqual(paged.Arguments[1], 1);
            Assert.AreEqual(paged.Arguments[2], 25);
        }

        [Test]
        public void SelectQuery()
        {
            object identifier = 12345421;

            var queryBuilder = new SqlQueryBuilder();

            var sqlQuery = queryBuilder.SelectQuery(typeof(CustomerWithIdentity), identifier);

            Assert.AreEqual("SELECT [Customers].[DoB], [Customers].[CustomerId], [Customers].[Name], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[CustomerId] = @p0", sqlQuery.CommandText);
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
                Status = CustomerStatus.Active
            };

            var queryBuilder = new SqlQueryBuilder();

            var sqlQuery = queryBuilder.UpdateQuery(customer);

            Assert.AreEqual("UPDATE [Sales].[Customers] SET [Customers].[DoB] = @p0, [Customers].[Name] = @p1, [Customers].[StatusId] = @p2 WHERE [Customers].[CustomerId] = @p3", sqlQuery.CommandText);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[1]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[2]);
            Assert.AreEqual(customer.Id, sqlQuery.Arguments[3]);
        }

        [MicroLite.Mapping.Table(schema: "Marketing", name: "Customers")]
        private class CustomerWithAssigned
        {
            public CustomerWithAssigned()
            {
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Assigned)]
            public int Id
            {
                get;
                set;
            }

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
    }
}