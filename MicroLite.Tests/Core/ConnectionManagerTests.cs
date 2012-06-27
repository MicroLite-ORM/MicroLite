﻿namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using MicroLite.Core;
    using MicroLite.FrameworkExtensions;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ConnectionManager"/> class.
    /// </summary>
    [TestFixture]
    public class ConnectionManagerTests
    {
        [Test]
        public void BeginTransactionCallsConnectionBeginTransaction()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);
            var transaction = connectionManager.BeginTransaction();

            Assert.IsInstanceOf<Transaction>(transaction);

            mockConnection.VerifyAll();
        }

        [Test]
        public void BeginTransactionOpensConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Open());

            var connectionManager = new ConnectionManager(mockConnection.Object);
            connectionManager.BeginTransaction();

            mockConnection.VerifyAll();
        }

        [Test]
        public void BeginTransactionWithIsolationLevelCallsConnectionBeginTransactionWithIsolationLevel()
        {
            var isolationLevel = IsolationLevel.Chaos;

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction(isolationLevel)).Returns(new Mock<IDbTransaction>().Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);
            var transaction = connectionManager.BeginTransaction(isolationLevel);

            Assert.IsInstanceOf<Transaction>(transaction);

            mockConnection.VerifyAll();
        }

        [Test]
        public void BeginTransactionWithIsolationLevelOpensConnection()
        {
            var isolationLevel = IsolationLevel.Chaos;

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Open());

            var connectionManager = new ConnectionManager(mockConnection.Object);
            connectionManager.BeginTransaction(isolationLevel);

            mockConnection.VerifyAll();
        }

        [Test]
        public void BuildEnlistsInActiveTransaction()
        {
            var mockCommand = new Mock<IDbCommand>();
            mockCommand.SetupAllProperties();

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var sqlQuery = new SqlQuery("EXEC GetTableContents");

            using (var connectionManager = new ConnectionManager(mockConnection.Object))
            {
                connectionManager.BeginTransaction();

                var command = connectionManager.Build(sqlQuery);

                Assert.NotNull(command.Transaction);
            }
        }

        [Test]
        public void BuildForSqlQueryWithSqlText()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value1] = @p1 AND [Table].[Value2] = @p2",
                new object[] { 100, "hello", null });

            using (var connectionManager = new ConnectionManager(new SqlConnection()))
            {
                var command = connectionManager.Build(sqlQuery);

                Assert.AreEqual(sqlQuery.CommandText, command.CommandText);
                Assert.AreEqual(CommandType.Text, command.CommandType);
                Assert.AreEqual(3, command.Parameters.Count);

                var parameter1 = (IDataParameter)command.Parameters[0];
                Assert.AreEqual(ParameterDirection.Input, parameter1.Direction);
                Assert.AreEqual("@p0", parameter1.ParameterName);
                Assert.AreEqual(sqlQuery.Arguments[0], parameter1.Value);

                var parameter2 = (IDataParameter)command.Parameters[1];
                Assert.AreEqual(ParameterDirection.Input, parameter2.Direction);
                Assert.AreEqual("@p1", parameter2.ParameterName);
                Assert.AreEqual(sqlQuery.Arguments[1], parameter2.Value);

                var parameter3 = (IDataParameter)command.Parameters[2];
                Assert.AreEqual(ParameterDirection.Input, parameter3.Direction);
                Assert.AreEqual("@p2", parameter3.ParameterName);
                Assert.AreEqual(DBNull.Value, parameter3.Value);
            }
        }

        /// <summary>
        /// Issue #6 - The argument count check needs to cater for the same argument being used twice.
        /// </summary>
        [Test]
        public void BuildForSqlQueryWithSqlTextWhichUsesSameParameterTwice()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value1] = @p1 OR @p1 IS NULL",
                new object[] { 100, "hello" });

            using (var connectionManager = new ConnectionManager(new SqlConnection()))
            {
                var command = connectionManager.Build(sqlQuery);

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
        public void BuildForSqlQueryWithStoredProcedureWithoutParameters()
        {
            var sqlQuery = new SqlQuery("EXEC GetTableContents");

            using (var connectionManager = new ConnectionManager(new SqlConnection()))
            {
                var command = connectionManager.Build(sqlQuery);

                // The command text should only contain the stored procedure name.
                Assert.AreEqual("GetTableContents", command.CommandText);
                Assert.AreEqual(CommandType.StoredProcedure, command.CommandType);
                Assert.AreEqual(0, command.Parameters.Count);
            }
        }

        [Test]
        public void BuildForSqlQueryWithStoredProcedureWithParameters()
        {
            var sqlQuery = new SqlQuery(
                "EXEC GetTableContents @identifier, @Cust_Name",
                new object[] { 100, "hello" });

            using (var connectionManager = new ConnectionManager(new SqlConnection()))
            {
                var command = connectionManager.Build(sqlQuery);

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
        public void BuildSetsDbCommandTimeoutToSqlQueryTime()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM [Table]");
            sqlQuery.Timeout = 42; // Use an oddball time which shouldn't be a default anywhere.

            using (var connectionManager = new ConnectionManager(new SqlConnection()))
            {
                var command = connectionManager.Build(sqlQuery);

                Assert.AreEqual(sqlQuery.Timeout, command.CommandTimeout);
            }
        }

        [Test]
        public void BuildThrowsMicroLiteExceptionForParameterCountMismatch()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value] = @p1",
                new object[] { 100 });

            var connectionManager = new ConnectionManager(new Mock<IDbConnection>().Object);

            var exception = Assert.Throws<MicroLiteException>(
                () => connectionManager.Build(sqlQuery));

            Assert.AreEqual(Messages.ArgumentsCountMismatch.FormatWith("2", "1"), exception.Message);
        }

        [Test]
        public void CurrentTransactionReturnsCurrentTransactionIfActive()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);
            var transaction = connectionManager.BeginTransaction();

            Assert.AreSame(transaction, connectionManager.CurrentTransaction);
        }

        [Test]
        public void CurrentTransactionReturnsNullIfNoTransactionActive()
        {
            var connectionManager = new ConnectionManager(new Mock<IDbConnection>().Object);

            Assert.IsNull(connectionManager.CurrentTransaction);
        }

        [Test]
        public void CurrentTransactionReturnsNullOnceTransactionCommitted()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);
            var transaction = connectionManager.BeginTransaction();
            transaction.Commit();

            Assert.IsNull(connectionManager.CurrentTransaction);
        }

        [Test]
        public void CurrentTransactionReturnsNullOnceTransactionRolledback()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);
            var transaction = connectionManager.BeginTransaction();
            transaction.Rollback();

            Assert.IsNull(connectionManager.CurrentTransaction);
        }

        [Test]
        public void DisposeClosesAndDisposesConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());
            mockConnection.Setup(x => x.Dispose());

            using (new ConnectionManager(mockConnection.Object))
            {
            }

            mockConnection.VerifyAll();
        }

        [Test]
        public void DisposeDisposesCurrentTransaction()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

            using (var connectionManager = new ConnectionManager(mockConnection.Object))
            {
                connectionManager.BeginTransaction();
            }

            mockTransaction.VerifyAll();
        }
    }
}