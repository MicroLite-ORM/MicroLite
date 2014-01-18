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

            Assert.Equal(string.Empty, sqlCharacters.LeftDelimiter);
            Assert.Equal("%", sqlCharacters.LikeWildcard);
            Assert.Equal(string.Empty, sqlCharacters.RightDelimiter);
            Assert.Equal("*", sqlCharacters.SelectWildcard);
            Assert.Equal("?", sqlCharacters.SqlParameter);
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
    }
}