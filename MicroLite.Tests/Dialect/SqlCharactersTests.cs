namespace MicroLite.Tests.Dialect
{
    using MicroLite.Dialect;
    using Xunit;

    public class SqlCharactersTests
    {
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
        public void SQLiteEscapesSqlCorrectly()
        {
            Assert.Equal("\"Name\"", SqlCharacters.SQLite.EscapeSql("Name"));
        }

        [Fact]
        public void SQLiteGetParameterNameReturnsCorrectValue()
        {
            Assert.Equal("@p0", SqlCharacters.SQLite.GetParameterName(0));
        }
    }
}