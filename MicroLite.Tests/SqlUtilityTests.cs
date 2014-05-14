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
        public void ReadOrderByClauseRemovesLineBreaks()
        {
            var commandText = @"SELECT * FROM Table ORDER BY
Name ASC,
Age DESC";

            Assert.Equal("Name ASC, Age DESC", SqlUtility.ReadOrderByClause(commandText));
        }

        [Fact]
        public void ReadOrderByClauseReturnsAnEmptyStringIfTheCommandTextIsEmpty()
        {
            Assert.Empty(SqlUtility.ReadOrderByClause(string.Empty));
        }

        [Fact]
        public void ReadOrderByClauseReturnsAnEmptyStringIfTheCommandTextIsNull()
        {
            Assert.Empty(SqlUtility.ReadOrderByClause(null));
        }

        [Fact]
        public void ReadSelectClauseReturnsAnEmptyStringIfTheCommandTextDoesNotContainSelect()
        {
            Assert.Empty(SqlUtility.ReadSelectClause("EXEC sp_who"));
        }

        [Fact]
        public void ReadSelectClauseReturnsAnEmptyStringIfTheCommandTextIsEmpty()
        {
            Assert.Empty(SqlUtility.ReadSelectClause(string.Empty));
        }

        [Fact]
        public void ReadSelectClauseReturnsAnEmptyStringIfTheCommandTextIsNull()
        {
            Assert.Empty(SqlUtility.ReadSelectClause(null));
        }

        [Fact]
        public void ReadTableNameRemovesLineBreaks()
        {
            var commandText = @"SELECT
*
FROM
Table
tbl";

            Assert.Equal("Table tbl", SqlUtility.ReadTableName(commandText));
        }

        [Fact]
        public void ReadTableNameReturnsAnEmptyStringIfTheCommandTextDoesNotContainFrom()
        {
            Assert.Empty(SqlUtility.ReadTableName("EXEC sp_who"));
        }

        [Fact]
        public void ReadTableNameReturnsAnEmptyStringIfTheCommandTextIsEmpty()
        {
            Assert.Empty(SqlUtility.ReadTableName(string.Empty));
        }

        [Fact]
        public void ReadTableNameReturnsAnEmptyStringIfTheCommandTextIsNull()
        {
            Assert.Empty(SqlUtility.ReadTableName(null));
        }

        [Fact]
        public void ReadWhereClauseRemovesLineBreaks()
        {
            var commandText = @"SELECT * FROM Table WHERE
Name = ?
AND Age = ?";

            Assert.Equal("Name = ? AND Age = ?", SqlUtility.ReadWhereClause(commandText));
        }

        [Fact]
        public void ReadWhereClauseReturnsAnEmptyStringIfTheCommandTextIsEmpty()
        {
            Assert.Empty(SqlUtility.ReadWhereClause(string.Empty));
        }

        [Fact]
        public void ReadWhereClauseReturnsAnEmptyStringIfTheCommandTextIsNull()
        {
            Assert.Empty(SqlUtility.ReadWhereClause(null));
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

        public class WhenTheCommandTextIsOnASingleLineAndContainsOrderByButNoWhere
        {
            private readonly string commandText = @"SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] ORDER BY [Customers].[Name] ASC";

            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnASingleLineAndContainsOrderByButNoWhere()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[Name] ASC", this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnASingleLineAndContainsWhereAndOrderBy
        {
            private readonly string commandText = "SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0 ORDER BY [Customers].[Name] ASC";
            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnASingleLineAndContainsWhereAndOrderBy()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[Name] ASC", this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[StatusId] = @p0", this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnASingleLineAndContainsWhereButNoOrderBy
        {
            private readonly string commandText = "SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers] WHERE [Customers].[StatusId] = @p0";
            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnASingleLineAndContainsWhereButNoOrderBy()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[StatusId] = @p0", this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnASingleLineAndDoesNotContainWhereOrOrderBy
        {
            private readonly string commandText = "SELECT [Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId] FROM [Sales].[Customers]";
            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnASingleLineAndDoesNotContainWhereOrOrderBy()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnMultipleLinesAndContainsOrderByButNoWhere
        {
            private readonly string commandText = @"SELECT
[Customers].[CustomerId],
[Customers].[Name],
[Customers].[DoB],
[Customers].[StatusId]
FROM
[Sales].[Customers]
ORDER BY
[Customers].[Name] ASC";

            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnMultipleLinesAndContainsOrderByButNoWhere()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[Name] ASC", this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnMultipleLinesAndContainsWhereAndOrderBy
        {
            private readonly string commandText = @"SELECT
[Customers].[CustomerId],
[Customers].[Name],
[Customers].[DoB],
[Customers].[StatusId]
FROM
[Sales].[Customers]
WHERE
[Customers].[StatusId] = @p0
ORDER BY
[Customers].[Name] ASC";

            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnMultipleLinesAndContainsWhereAndOrderBy()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[Name] ASC", this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[StatusId] = @p0", this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnMultipleLinesAndContainsWhereButNoOrderBy
        {
            private readonly string commandText = @"SELECT
[Customers].[CustomerId],
[Customers].[Name],
[Customers].[DoB],
[Customers].[StatusId]
FROM
[Sales].[Customers]
WHERE
[Customers].[StatusId] = @p0";

            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnMultipleLinesAndContainsWhereButNoOrderBy()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[StatusId] = @p0", this.whereClause);
            }
        }

        public class WhenTheCommandTextIsOnMultipleLinesAndDoesNotContainWhereOrOrderBy
        {
            private readonly string commandText = @"SELECT
[Customers].[CustomerId],
[Customers].[Name],
[Customers].[DoB],
[Customers].[StatusId]
FROM
[Sales].[Customers]";

            private readonly string orderbyClause;
            private readonly string selectClause;
            private readonly string tableName;
            private readonly string whereClause;

            public WhenTheCommandTextIsOnMultipleLinesAndDoesNotContainWhereOrOrderBy()
            {
                this.selectClause = SqlUtility.ReadSelectClause(this.commandText);
                this.tableName = SqlUtility.ReadTableName(this.commandText);
                this.whereClause = SqlUtility.ReadWhereClause(this.commandText);
                this.orderbyClause = SqlUtility.ReadOrderByClause(this.commandText);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.orderbyClause);
            }

            [Fact]
            public void TheSelectClauseShouldBeReturned()
            {
                Assert.Equal("[Customers].[CustomerId], [Customers].[Name], [Customers].[DoB], [Customers].[StatusId]", this.selectClause);
            }

            [Fact]
            public void TheTableNameShouldBeReturned()
            {
                Assert.Equal("[Sales].[Customers]", this.tableName);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.whereClause);
            }
        }
    }
}