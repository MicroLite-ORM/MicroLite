namespace MicroLite.Tests
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlUtil"/> class.
    /// </summary>
    public class SqlUtilTests
    {
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