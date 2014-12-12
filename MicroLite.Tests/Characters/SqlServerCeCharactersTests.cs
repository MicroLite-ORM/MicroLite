namespace MicroLite.Tests.Characters
{
    using MicroLite.Characters;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlServerCeCharacters"/> class.
    /// </summary>
    public class SqlServerCeCharactersTests
    {
        [Fact]
        public void InstanceReturnsTheSameInstanceEachTime()
        {
            var characters1 = SqlServerCeCharacters.Instance;
            var characters2 = SqlServerCeCharacters.Instance;

            Assert.Same(characters1, characters2);
        }

        [Fact]
        public void LeftDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", SqlServerCeCharacters.Instance.LeftDelimiter);
        }

        [Fact]
        public void RightDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", SqlServerCeCharacters.Instance.RightDelimiter);
        }

        [Fact]
        public void SqlParameterReturnsAtSign()
        {
            Assert.Equal("@", SqlServerCeCharacters.Instance.SqlParameter);
        }

        [Fact]
        public void SupportsNamedParametersReturnsTrue()
        {
            Assert.True(SqlServerCeCharacters.Instance.SupportsNamedParameters);
        }
    }
}