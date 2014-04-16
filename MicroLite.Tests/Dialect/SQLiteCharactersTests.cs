namespace MicroLite.Tests.Dialect
{
    using MicroLite.Dialect;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SQLiteCharacters"/> class.
    /// </summary>
    public class SQLiteCharactersTests
    {
        [Fact]
        public void InstanceReturnsTheSameInstanceEachTime()
        {
            var characters1 = SQLiteCharacters.Instance;
            var characters2 = SQLiteCharacters.Instance;

            Assert.Same(characters1, characters2);
        }

        [Fact]
        public void LeftDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", SQLiteCharacters.Instance.LeftDelimiter);
        }

        [Fact]
        public void RightDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", SQLiteCharacters.Instance.RightDelimiter);
        }

        [Fact]
        public void SqlParameterReturnsAtSign()
        {
            Assert.Equal("@", SQLiteCharacters.Instance.SqlParameter);
        }

        [Fact]
        public void SupportsNamedParametersReturnsTrue()
        {
            Assert.True(SQLiteCharacters.Instance.SupportsNamedParameters);
        }
    }
}