namespace MicroLite.Tests
{
    using System;
    using Moq;
    using Xunit;

    public class SqlCharactersTests
    {
        [Fact]
        public void DefaultPropertyValues()
        {
            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var sqlCharacters = mockSqlCharacters.Object;

            Assert.Equal("\"", sqlCharacters.LeftDelimiter);
            Assert.Equal("%", sqlCharacters.LikeWildcard);
            Assert.Equal("\"", sqlCharacters.RightDelimiter);
            Assert.Equal("*", sqlCharacters.SelectWildcard);
            Assert.Equal("?", sqlCharacters.SqlParameter);
            Assert.Equal(";", sqlCharacters.StatementSeparator);
            Assert.Equal(false, sqlCharacters.SupportsNamedParameters);
        }

        [Fact]
        public void EmptyDoesNotExcapeValue()
        {
            Assert.Equal("Name", SqlCharacters.Empty.EscapeSql("Name"));
        }

        [Fact]
        public void EmptyEscapeSqlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SqlCharacters.Empty.EscapeSql(null));
        }

        [Fact]
        public void EmptyGetParameterNameReturnsCorrectValue()
        {
            Assert.Equal("?", SqlCharacters.Empty.GetParameterName(0));
        }

        [Fact]
        public void MsSqlDoesNotDoubleEscape()
        {
            Assert.Equal("[Name]", SqlCharacters.MsSql.EscapeSql("[Name]"));
        }

        [Fact]
        public void MsSqlEscapeSqlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SqlCharacters.MsSql.EscapeSql(null));
        }

        [Fact]
        public void MsSqlEscapesQualifiedColumnSqlCorrectly()
        {
            Assert.Equal("[Table].[Column]", SqlCharacters.MsSql.EscapeSql("Table.Column"));
        }

        [Fact]
        public void MsSqlEscapesSqlCorrectly()
        {
            Assert.Equal("[Name]", SqlCharacters.MsSql.EscapeSql("Name"));
        }

        [Fact]
        public void MsSqlGetParameterNameReturnsCorrectValue()
        {
            Assert.Equal("@p0", SqlCharacters.MsSql.GetParameterName(0));
        }

        [Fact]
        public void MsSqlIsEscapedReturnsFalseIfNotEscaped()
        {
            Assert.False(SqlCharacters.MsSql.IsEscaped("Name"));
        }

        [Fact]
        public void MsSqlIsEscapedReturnsTrueIfEscaped()
        {
            Assert.True(SqlCharacters.MsSql.IsEscaped("[Name]"));
        }

        [Fact]
        public void MySqlDoesNotDoubleEscape()
        {
            Assert.Equal("`Name`", SqlCharacters.MySql.EscapeSql("`Name`"));
        }

        [Fact]
        public void MySqlEscapeSqlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SqlCharacters.MySql.EscapeSql(null));
        }

        [Fact]
        public void MySqlEscapesQualifiedColumnSqlCorrectly()
        {
            Assert.Equal("`Table`.`Column`", SqlCharacters.MySql.EscapeSql("Table.Column"));
        }

        [Fact]
        public void MySqlEscapesSqlCorrectly()
        {
            Assert.Equal("`Name`", SqlCharacters.MySql.EscapeSql("Name"));
        }

        [Fact]
        public void MySqlGetParameterNameReturnsCorrectValue()
        {
            Assert.Equal("@p0", SqlCharacters.MySql.GetParameterName(0));
        }

        [Fact]
        public void MySqlIsEscapedReturnsFalseIfNotEscaped()
        {
            Assert.False(SqlCharacters.MySql.IsEscaped("Name"));
        }

        [Fact]
        public void MySqlIsEscapedReturnsTrueIfEscaped()
        {
            Assert.True(SqlCharacters.MySql.IsEscaped("`Name`"));
        }

        [Fact]
        public void PostgreSqlDoesNotDoubleEscape()
        {
            Assert.Equal("\"Name\"", SqlCharacters.PostgreSql.EscapeSql("\"Name\""));
        }

        [Fact]
        public void PostgreSqlEscapeSqlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SqlCharacters.PostgreSql.EscapeSql(null));
        }

        [Fact]
        public void PostgreSqlEscapesQualifiedColumnSqlCorrectly()
        {
            Assert.Equal("\"Table\".\"Column\"", SqlCharacters.PostgreSql.EscapeSql("Table.Column"));
        }

        [Fact]
        public void PostgreSqlEscapesSqlCorrectly()
        {
            Assert.Equal("\"Name\"", SqlCharacters.PostgreSql.EscapeSql("Name"));
        }

        [Fact]
        public void PostgreSqlGetParameterNameReturnsCorrectValue()
        {
            Assert.Equal(":p0", SqlCharacters.PostgreSql.GetParameterName(0));
        }

        [Fact]
        public void PostgreSqlIsEscapedReturnsFalseIfNotEscaped()
        {
            Assert.False(SqlCharacters.PostgreSql.IsEscaped("Name"));
        }

        [Fact]
        public void PostgreSqlIsEscapedReturnsTrueIfEscaped()
        {
            Assert.True(SqlCharacters.PostgreSql.IsEscaped("\"Name\""));
        }

        [Fact]
        public void SQLiteDoesNotDoubleEscape()
        {
            Assert.Equal("\"Name\"", SqlCharacters.SQLite.EscapeSql("\"Name\""));
        }

        [Fact]
        public void SQLiteEscapeSqlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SqlCharacters.SQLite.EscapeSql(null));
        }

        [Fact]
        public void SQLiteEscapesQualifiedColumnSqlCorrectly()
        {
            Assert.Equal("\"Table\".\"Column\"", SqlCharacters.SQLite.EscapeSql("Table.Column"));
        }

        [Fact]
        public void SQLiteEscapesSqlCorrectly()
        {
            Assert.Equal("\"Name\"", SqlCharacters.SQLite.EscapeSql("Name"));
        }

        [Fact]
        public void SQLiteGetParameterNameReturnsCorrectValue()
        {
            Assert.Equal("@p0", SqlCharacters.SQLite.GetParameterName(0));
        }

        [Fact]
        public void SQLiteIsEscapedReturnsFalseIfNotEscaped()
        {
            Assert.False(SqlCharacters.SQLite.IsEscaped("Name"));
        }

        [Fact]
        public void SQLiteIsEscapedReturnsTrueIfEscaped()
        {
            Assert.True(SqlCharacters.SQLite.IsEscaped("\"Name\""));
        }
    }
}