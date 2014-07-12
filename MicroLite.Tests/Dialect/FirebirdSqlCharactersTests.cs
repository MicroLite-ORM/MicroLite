namespace MicroLite.Tests.Dialect
{
    using MicroLite.Dialect;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FirebirdSqlCharacters"/> class.
    /// </summary>
    public class FirebirdSqlCharactersTests
    {
        [Fact]
        public void InstanceReturnsTheSameInstanceEachTime()
        {
            var characters1 = FirebirdSqlCharacters.Instance;
            var characters2 = FirebirdSqlCharacters.Instance;

            Assert.Same(characters1, characters2);
        }

        [Fact]
        public void LeftDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", FirebirdSqlCharacters.Instance.LeftDelimiter);
        }

        [Fact]
        public void RightDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", FirebirdSqlCharacters.Instance.RightDelimiter);
        }

        [Fact]
        public void SqlParameterReturnsAtSign()
        {
            Assert.Equal("@", FirebirdSqlCharacters.Instance.SqlParameter);
        }

        [Fact]
        public void SupportsNamedParametersReturnsTrue()
        {
            Assert.True(FirebirdSqlCharacters.Instance.SupportsNamedParameters);
        }
    }
}