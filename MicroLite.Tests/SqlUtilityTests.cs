namespace MicroLite.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlUtility"/> class.
    /// </summary>
    public class SqlUtilityTests
    {
        [Fact]
        public void GetFirstParameterPositionThrowsArgumentNullExceptionForNullCommandText()
        {
            Assert.Throws<ArgumentNullException>(() => SqlUtility.GetFirstParameterPosition(null));
        }

        [Fact]
        public void GetFirstParameterPositionWithAtParameters()
        {
            var position = SqlUtility.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = @p0 AND Column2 = @p1");

            Assert.Equal(36, position);
        }

        [Fact]
        public void GetFirstParameterPositionWithColonParameters()
        {
            var position = SqlUtility.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = :p0 AND Column2 = :p1");

            Assert.Equal(36, position);
        }

        [Fact]
        public void GetFirstParameterPositionWithNoParameters()
        {
            var position = SqlUtility.GetFirstParameterPosition("SELECT * FROM TABLE");

            Assert.Equal(-1, position);
        }

        [Fact]
        public void GetFirstParameterPositionWithQuestionMarkParameters()
        {
            var position = SqlUtility.GetFirstParameterPosition("SELECT * FROM TABLE WHERE Column1 = ? AND Column2 = ?");

            Assert.Equal(36, position);
        }

        [Fact]
        public void GetParameterNamesThrowsArgumentNullExceptionForNullCommandText()
        {
            Assert.Throws<ArgumentNullException>(() => SqlUtility.GetParameterNames(null));
        }

        [Fact]
        public void GetParameterNamesWithAtPrefix()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = @p0 AND (Col2 = @p1 OR @p1 IS NULL)");

            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { "@p0", "@p1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithColonPrefix()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = :p0 AND (Col2 = :p1 OR :p1 IS NULL)");

            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { ":p0", ":p1" }, parameterNames);
        }

        [Fact]
        public void ReNumberParametersNoExistingArguments()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 2);

            Assert.Equal("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", commandText);
        }

        [Fact]
        public void ReNumberParametersWithExistingArguments()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 4);

            Assert.Equal("(Column1 = @p2 OR @p2 IS NULL) AND Column2 = @p3", commandText);
        }
    }
}