namespace MicroLite.Tests.Characters
{
    using MicroLite.Characters;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PostgreSqlCharacters"/> class.
    /// </summary>
    public class PostgreSqlCharactersTests
    {
        [Fact]
        public void InstanceReturnsTheSameInstanceEachTime()
        {
            var characters1 = PostgreSqlCharacters.Instance;
            var characters2 = PostgreSqlCharacters.Instance;

            Assert.Same(characters1, characters2);
        }

        [Fact]
        public void LeftDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", PostgreSqlCharacters.Instance.LeftDelimiter);
        }

        [Fact]
        public void RightDelimiterReturnsCorrectValue()
        {
            Assert.Equal("\"", PostgreSqlCharacters.Instance.RightDelimiter);
        }

        [Fact]
        public void SqlParameterReturnsAtSign()
        {
            Assert.Equal("@", PostgreSqlCharacters.Instance.SqlParameter);
        }

        [Fact]
        public void StoredProcedureInvocationCommandReturnsSelect()
        {
            Assert.Equal("SELECT", PostgreSqlCharacters.Instance.StoredProcedureInvocationCommand);
        }

        [Fact]
        public void SupportsNamedParametersReturnsTrue()
        {
            Assert.True(PostgreSqlCharacters.Instance.SupportsNamedParameters);
        }
    }
}