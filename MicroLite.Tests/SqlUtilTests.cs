namespace MicroLite.Tests
{
    using System;
    using System.Data;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlUtil"/> class.
    /// </summary>
    [TestFixture]
    public class SqlUtilTests
    {
        [Test]
        public void Combine()
        {
            var sqlQuery1 = new SqlQuery("SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1", "Foo", 100);
            var sqlQuery2 = new SqlQuery("SELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p0 OR @p0 IS NULL) AND [Column_2] < @p1", "Bar", -1);

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.AreEqual(4, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual(100, sqlQuery.Arguments[1]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[2]);
            Assert.AreEqual(-1, sqlQuery.Arguments[3]);

            Assert.AreEqual(
                "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1;\r\nSELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p2 OR @p2 IS NULL) AND [Column_2] < @p3",
                sqlQuery.CommandText);
        }

        [Test]
        public void CombineShouldSetTimeoutToLongestTimeout()
        {
            var sqlQuery1 = new SqlQuery(string.Empty);
            var sqlQuery2 = new SqlQuery(string.Empty);

            sqlQuery1.Timeout = 42;

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.AreEqual(sqlQuery1.Timeout, sqlQuery.Timeout);
        }

        [Test]
        public void CombineThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => SqlUtil.Combine(null));

            Assert.AreEqual("sqlQueries", exception.ParamName);
        }

        [Test]
        public void GetCommandTypeForExecStatement()
        {
            var commandType = SqlUtil.GetCommandType("EXEC sp_Proc");

            Assert.AreEqual(CommandType.StoredProcedure, commandType);
        }

        [Test]
        public void GetCommandTypeForSelectStatement()
        {
            var commandType = SqlUtil.GetCommandType("SELECT *");

            Assert.AreEqual(CommandType.Text, commandType);
        }

        [Test]
        public void GetFirstParameterPositionWithMultipleParameters()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = @p0 AND Column2 = @p1");

            Assert.AreEqual(36, position);
        }

        [Test]
        public void GetFirstParameterPositionWithNoParameters()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE");

            Assert.AreEqual(-1, position);
        }

        [Test]
        public void GetFirstParameterPositionWithSingleParameter()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = @p0");

            Assert.AreEqual(36, position);
        }

        [Test]
        public void GetParameterNames()
        {
            var parameterNames = SqlUtil.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = @p0 AND (Col2 = @p1 OR @p1 IS NULL)");

            Assert.AreEqual(2, parameterNames.Count);
            CollectionAssert.AreEquivalent(new[] { "@p0", "@p1" }, parameterNames);
        }

        [Test]
        public void ReNumberParametersNoExistingArguments()
        {
            var commandText = SqlUtil.ReNumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 2);

            Assert.AreEqual("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", commandText);
        }

        [Test]
        public void ReNumberParametersWithExistingArguments()
        {
            var commandText = SqlUtil.ReNumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 4);

            Assert.AreEqual("(Column1 = @p2 OR @p2 IS NULL) AND Column2 = @p3", commandText);
        }
    }
}