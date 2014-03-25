namespace MicroLite.Tests.Dialect
{
    using MicroLite.Dialect;
    using Xunit;

    public class MySqlCharactersTests
    {
        [Fact]
        public void InstanceReturnsTheSameInstanceEachTime()
        {
            var characters1 = MySqlCharacters.Instance;
            var characters2 = MySqlCharacters.Instance;

            Assert.Same(characters1, characters2);
        }

        [Fact]
        public void LeftDelimiterReturnsCorrectValue()
        {
            Assert.Equal("`", MySqlCharacters.Instance.LeftDelimiter);
        }

        [Fact]
        public void RightDelimiterReturnsCorrectValue()
        {
            Assert.Equal("`", MySqlCharacters.Instance.RightDelimiter);
        }

        [Fact]
        public void SqlParameterReturnsAtSign()
        {
            Assert.Equal("@", MySqlCharacters.Instance.SqlParameter);
        }

        [Fact]
        public void StoredProcedureInvocationCommandReturnsCall()
        {
            Assert.Equal("CALL", MySqlCharacters.Instance.StoredProcedureInvocationCommand);
        }

        [Fact]
        public void SupportsNamedParametersReturnsTrue()
        {
            Assert.True(MySqlCharacters.Instance.SupportsNamedParameters);
        }
    }
}