namespace MicroLite.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlUtil"/> class.
    /// </summary>

    public class SqlUtilTests
    {
        /// <summary>
        /// Issue #90 - Re-Writing parameters should not happen if the query is a stored procedure.
        /// </summary>
        [Fact]
        public void CombineShouldNotReNumberParametersForStoredProcedure()
        {
            var sqlQuery1 = new SqlQuery("SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1", "Foo", 100);
            var sqlQuery2 = new SqlQuery("EXEC CustomersByStatus @StatusId", 2);

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(100, sqlQuery.Arguments[1]);
            Assert.Equal(2, sqlQuery.Arguments[2]);

            Assert.Equal(
                "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1;\r\nEXEC CustomersByStatus @StatusId",
                sqlQuery.CommandText);
        }

        [Fact]
        public void CombineShouldSetTimeoutToLongestTimeout()
        {
            var sqlQuery1 = new SqlQuery(string.Empty);
            var sqlQuery2 = new SqlQuery(string.Empty);

            sqlQuery1.Timeout = 42;

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.Equal(sqlQuery1.Timeout, sqlQuery.Timeout);
        }

        [Fact]
        public void CombineThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => SqlUtil.Combine(null));

            Assert.Equal("sqlQueries", exception.ParamName);
        }

        [Fact]
        public void CombineWithAtPrefixedParameters()
        {
            var sqlQuery1 = new SqlQuery("SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1", "Foo", 100);
            var sqlQuery2 = new SqlQuery("SELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p0 OR @p0 IS NULL) AND [Column_2] < @p1", "Bar", -1);

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(100, sqlQuery.Arguments[1]);
            Assert.Equal("Bar", sqlQuery.Arguments[2]);
            Assert.Equal(-1, sqlQuery.Arguments[3]);

            Assert.Equal(
                "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1;\r\nSELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p2 OR @p2 IS NULL) AND [Column_2] < @p3",
                sqlQuery.CommandText);
        }

        [Fact]
        public void CombineWithColonPrefixedParameters()
        {
            var sqlQuery1 = new SqlQuery("SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = :p0 AND \"Column2\" > :p1", "Foo", 100);
            var sqlQuery2 = new SqlQuery("SELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = :p0 OR :p0 IS NULL) AND \"Column2\" < :p1", "Bar", -1);

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(100, sqlQuery.Arguments[1]);
            Assert.Equal("Bar", sqlQuery.Arguments[2]);
            Assert.Equal(-1, sqlQuery.Arguments[3]);

            Assert.Equal(
                "SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = :p0 AND \"Column2\" > :p1;\r\nSELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = :p2 OR :p2 IS NULL) AND \"Column2\" < :p3",
                sqlQuery.CommandText);
        }

        [Fact]
        public void CombineWithQuestionMarkParameters()
        {
            var sqlQuery1 = new SqlQuery("SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?", "Foo", 100);
            var sqlQuery2 = new SqlQuery("SELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?", "Bar", -1);

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.Equal(4, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(100, sqlQuery.Arguments[1]);
            Assert.Equal("Bar", sqlQuery.Arguments[2]);
            Assert.Equal(-1, sqlQuery.Arguments[3]);

            Assert.Equal(
                "SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?;\r\nSELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?",
                sqlQuery.CommandText);
        }

        [Fact]
        public void GetFirstParameterPositionWithAtParameters()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = @p0 AND Column2 = @p1");

            Assert.Equal(36, position);
        }

        [Fact]
        public void GetFirstParameterPositionWithColonParameters()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = :p0 AND Column2 = :p1");

            Assert.Equal(36, position);
        }

        [Fact]
        public void GetFirstParameterPositionWithNoParameters()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE");

            Assert.Equal(-1, position);
        }

        [Fact]
        public void GetFirstParameterPositionWithQuestionMarkParameters()
        {
            var position = SqlUtil.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = ? AND Column2 = ?");

            Assert.Equal(36, position);
        }

        [Fact]
        public void GetParameterNamesWithAtPrefix()
        {
            var parameterNames = SqlUtil.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = @p0 AND (Col2 = @p1 OR @p1 IS NULL)");

            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { "@p0", "@p1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithColonPrefix()
        {
            var parameterNames = SqlUtil.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = :p0 AND (Col2 = :p1 OR :p1 IS NULL)");

            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { ":p0", ":p1" }, parameterNames);
        }

        [Fact]
        public void ReNumberParametersNoExistingArguments()
        {
            var commandText = SqlUtil.ReNumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 2);

            Assert.Equal("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", commandText);
        }

        [Fact]
        public void ReNumberParametersWithExistingArguments()
        {
            var commandText = SqlUtil.ReNumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 4);

            Assert.Equal("(Column1 = @p2 OR @p2 IS NULL) AND Column2 = @p3", commandText);
        }
    }
}