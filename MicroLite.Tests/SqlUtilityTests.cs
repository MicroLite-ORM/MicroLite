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
        public void GetParameterNamesShouldNotMatchAtAtIdentifier()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT @@IDENTITY");

            Assert.Empty(parameterNames);
        }

        [Fact]
        public void GetParameterNamesThrowsArgumentNullExceptionForNullCommandText()
        {
            Assert.Throws<ArgumentNullException>(() => SqlUtility.GetParameterNames(null));
        }

        /// <summary>
        /// #309 - Invalid SQL produced by SqlBuilder when the predicate starts with an identifier
        /// </summary>
        [Fact]
        public void GetParameterNamesWhereTextStartsWithIdentifierAndContainsFurtherText()
        {
            var parameterNames = SqlUtility.GetParameterNames("@p0 IS NOT NULL AND [FirstName] LIKE @p0");

            Assert.NotNull(parameterNames);
            Assert.Equal(1, parameterNames.Count);
            Assert.Equal(new List<string> { "@p0" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithAtPrefix()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = @p0 AND (Col2 = @p1 OR @p1 IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { "@p0", "@p1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithAtPrefixAndMoreThanPrefixCharacter()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = @param0 AND (Col2 = @param1 OR @param1 IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { "@param0", "@param1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithAtPrefixAndNoAdditionalPrefixCharacter()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = @0 AND (Col2 = @1 OR @1 IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { "@0", "@1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithAtPrefixWherePredicateOnlyContainsSingleParameter()
        {
            var parameterNames = SqlUtility.GetParameterNames("@p0");

            Assert.NotNull(parameterNames);
            Assert.Equal(1, parameterNames.Count);
            Assert.Equal(new List<string> { "@p0" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithColonPrefix()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = :p0 AND (Col2 = :p1 OR :p1 IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { ":p0", ":p1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithColonPrefixAndMoreThanPrefixCharacter()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = :param0 AND (Col2 = :param1 OR :param1 IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { ":param0", ":param1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithColonPrefixAndNoAdditionalPrefixCharacter()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = :0 AND (Col2 = :1 OR :1 IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(2, parameterNames.Count);
            Assert.Equal(new List<string> { ":0", ":1" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithColonPrefixWherePredicateOnlyContainsSingleParameter()
        {
            var parameterNames = SqlUtility.GetParameterNames(":p0");

            Assert.NotNull(parameterNames);
            Assert.Equal(1, parameterNames.Count);
            Assert.Equal(new List<string> { ":p0" }, parameterNames);
        }

        [Fact]
        public void GetParameterNamesWithNoNamedParameters()
        {
            var parameterNames = SqlUtility.GetParameterNames("SELECT * FROM TABLE WHERE Col1 = ? AND (Col2 = ? OR ? IS NULL)");

            Assert.NotNull(parameterNames);
            Assert.Equal(0, parameterNames.Count);
        }

        [Fact]
        public void ReNumberParametersNoExistingArgumentsWithAtPrefix()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 2);

            Assert.Equal("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", commandText);
        }

        [Fact]
        public void ReNumberParametersNoExistingArgumentsWithAtPrefixAndMoreThanPrefixCharacter()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = @param0 OR @param0 IS NULL) AND Column2 = @param1", totalArgumentCount: 2);

            Assert.Equal("(Column1 = @param0 OR @param0 IS NULL) AND Column2 = @param1", commandText);
        }

        [Fact]
        public void ReNumberParametersNoExistingArgumentsWithColonPrefix()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = :p0 OR :p0 IS NULL) AND Column2 = :p1", totalArgumentCount: 2);

            Assert.Equal("(Column1 = :p0 OR :p0 IS NULL) AND Column2 = :p1", commandText);
        }

        [Fact]
        public void ReNumberParametersNoExistingArgumentsWithColonPrefixAndMoreThanPrefixCharacter()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = :param0 OR :param0 IS NULL) AND Column2 = :param1", totalArgumentCount: 2);

            Assert.Equal("(Column1 = :param0 OR :param0 IS NULL) AND Column2 = :param1", commandText);
        }

        [Fact]
        public void ReNumberParametersNoParameters()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 IS NULL)", totalArgumentCount: 5);

            Assert.Equal("(Column1 IS NULL)", commandText);
        }

        [Fact]
        public void ReNumberParametersWithExistingArgumentsWithAtPrefix()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = @p0 OR @p0 IS NULL) AND Column2 = @p1", totalArgumentCount: 4);

            Assert.Equal("(Column1 = @p2 OR @p2 IS NULL) AND Column2 = @p3", commandText);
        }

        [Fact]
        public void ReNumberParametersWithExistingArgumentsWithAtPrefixAndMoreThanPrefixCharacter()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = @param0 OR @param0 IS NULL) AND Column2 = @param1", totalArgumentCount: 4);

            Assert.Equal("(Column1 = @param2 OR @param2 IS NULL) AND Column2 = @param3", commandText);
        }

        [Fact]
        public void ReNumberParametersWithExistingArgumentsWithColonPrefix()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = :p0 OR :p0 IS NULL) AND Column2 = :p1", totalArgumentCount: 4);

            Assert.Equal("(Column1 = :p2 OR :p2 IS NULL) AND Column2 = :p3", commandText);
        }

        [Fact]
        public void ReNumberParametersWithExistingArgumentsWithColonPrefixAndMoreThanPrefixCharacter()
        {
            var commandText = SqlUtility.RenumberParameters("(Column1 = :param0 OR :param0 IS NULL) AND Column2 = :param1", totalArgumentCount: 4);

            Assert.Equal("(Column1 = :param2 OR :param2 IS NULL) AND Column2 = :param3", commandText);
        }
    }
}