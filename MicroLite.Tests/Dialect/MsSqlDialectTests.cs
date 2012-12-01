namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="MsSqlDialect"/> class.
    /// </summary>
    [TestFixture]
    public class MsSqlDialectTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        /// <summary>
        /// Issue #6 - The argument count check needs to cater for the same argument being used twice.
        /// </summary>
        [Test]
        public void BuildCommandForSqlQueryWithSqlTextWhichUsesSameParameterTwice()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value1] = @p1 OR @p1 IS NULL",
                new object[] { 100, "hello" });

            using (var command = new System.Data.SqlClient.SqlCommand())
            {
                var sqlDialect = new MsSqlDialect();
                sqlDialect.BuildCommand(command, sqlQuery);

                Assert.AreEqual(sqlQuery.CommandText, command.CommandText);
                Assert.AreEqual(CommandType.Text, command.CommandType);
                Assert.AreEqual(2, command.Parameters.Count);

                var parameter1 = (IDataParameter)command.Parameters[0];
                Assert.AreEqual(ParameterDirection.Input, parameter1.Direction);
                Assert.AreEqual("@p0", parameter1.ParameterName);
                Assert.AreEqual(sqlQuery.Arguments[0], parameter1.Value);

                var parameter2 = (IDataParameter)command.Parameters[1];
                Assert.AreEqual(ParameterDirection.Input, parameter2.Direction);
                Assert.AreEqual("@p1", parameter2.ParameterName);
                Assert.AreEqual(sqlQuery.Arguments[1], parameter2.Value);
            }
        }

        [Test]
        public void BuildCommandForSqlQueryWithStoredProcedureWithoutParameters()
        {
            var sqlQuery = new SqlQuery("EXEC GetTableContents");

            using (var command = new System.Data.SqlClient.SqlCommand())
            {
                var sqlDialect = new MsSqlDialect();
                sqlDialect.BuildCommand(command, sqlQuery);

                // The command text should only contain the stored procedure name.
                Assert.AreEqual("GetTableContents", command.CommandText);
                Assert.AreEqual(CommandType.StoredProcedure, command.CommandType);
                Assert.AreEqual(0, command.Parameters.Count);
            }
        }

        [Test]
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
                Assert.AreEqual("GetTableContents", command.CommandText);
                Assert.AreEqual(CommandType.StoredProcedure, command.CommandType);
                Assert.AreEqual(2, command.Parameters.Count);

                var parameter1 = (IDataParameter)command.Parameters[0];
                Assert.AreEqual("@identifier", parameter1.ParameterName);
                Assert.AreEqual(sqlQuery.Arguments[0], parameter1.Value);

                var parameter2 = (IDataParameter)command.Parameters[1];
                Assert.AreEqual("@Cust_Name", parameter2.ParameterName);
                Assert.AreEqual(sqlQuery.Arguments[1], parameter2.Value);
            }
        }

        [Test]
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

                Assert.AreEqual(Messages.ConnectionManager_ArgumentsCountMismatch.FormatWith("2", "1"), exception.Message);
            }
        }

        [Test]
        public void InsertQueryForIdentityInstance()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new MsSqlDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO [Sales].[Customers] ([Created], [DoB], [Name], [StatusId]) VALUES (@p0, @p1, @p2, @p3);SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            Assert.AreEqual(customer.Created, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[2]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[3]);
        }

        [Test]
        public void PageNonQualifiedQuery()
        {
            var sqlQuery = new SqlQuery("SELECT CustomerId, Name, DoB, StatusId FROM Customers");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT CustomerId, Name, DoB, StatusId FROM (SELECT CustomerId, Name, DoB, StatusId, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageNonQualifiedWildcardQuery()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Customers");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
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
 [Sales].[Customers]
 WHERE
 ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)
 ORDER BY
 [Customers].[Name] ASC,
 [Customers].[DoB] ASC", new object[] { CustomerStatus.Active, new DateTime(1980, 01, 01) });

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC, [Customers].[DoB] ASC) AS RowNumber FROM [Sales].[Customers] WHERE ([Customers].[StatusId] = @p0 AND [Customers].[DoB] > @p1)) AS [Customers] WHERE (RowNumber >= @p2 AND RowNumber <= @p3)", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(sqlQuery.Arguments[1], paged.Arguments[1], "The second argument should be the second argument from the original query");
            Assert.AreEqual(1, paged.Arguments[2], "The third argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[3], "The fourth argument should be the end row number");
        }

        [Test]
        public void PageWithNoWhereButOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM [dbo].[Customers] ORDER BY [CustomerId] ASC");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT [CustomerId], [Name], [DoB], [StatusId] FROM (SELECT [CustomerId], [Name], [DoB], [StatusId], ROW_NUMBER() OVER(ORDER BY [CustomerId] ASC) AS RowNumber FROM [dbo].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithNoWhereOrOrderByFirstResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(1, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithNoWhereOrOrderBySecondResultsPage()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]");

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 2, resultsPerPage: 25));

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers]) AS [Customers] WHERE (RowNumber >= @p0 AND RowNumber <= @p1)", paged.CommandText);
            Assert.AreEqual(26, paged.Arguments[0], "The first argument should be the start row number");
            Assert.AreEqual(50, paged.Arguments[1], "The second argument should be the end row number");
        }

        [Test]
        public void PageWithWhereAndOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC", CustomerStatus.Active);

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
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
 [Sales].[Customers]
 WHERE
 [Customers].[StatusId] = @p0
 ORDER BY
 [Customers].[Name] ASC", new object[] { CustomerStatus.Active });

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY [Customers].[Name] ASC) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(1, paged.Arguments[1], "The second argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[2], "The third argument should be the end row number");
        }

        [Test]
        public void PageWithWhereButNoOrderBy()
        {
            var sqlQuery = new SqlQuery("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0", CustomerStatus.Active);

            var sqlDialect = new MsSqlDialect();
            var paged = sqlDialect.PageQuery(sqlQuery, PagingOptions.ForPage(page: 1, resultsPerPage: 25));

            Assert.AreEqual("SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM (SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId], ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0) AS [Customers] WHERE (RowNumber >= @p1 AND RowNumber <= @p2)", paged.CommandText);
            Assert.AreEqual(sqlQuery.Arguments[0], paged.Arguments[0], "The first argument should be the first argument from the original query");
            Assert.AreEqual(1, paged.Arguments[1], "The second argument should be the start row number");
            Assert.AreEqual(25, paged.Arguments[2], "The third argument should be the end row number");
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
    }
}