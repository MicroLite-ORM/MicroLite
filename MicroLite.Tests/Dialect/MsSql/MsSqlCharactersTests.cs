namespace MicroLite.Tests.Dialect.MsSql
{
    using MicroLite.Dialect.MsSql;
    using Xunit;

    public class MsSqlCharactersTests
    {
        [Fact]
        public void InstanceReturnsTheSameInstanceEachTime()
        {
            var characters1 = MsSqlCharacters.Instance;
            var characters2 = MsSqlCharacters.Instance;

            Assert.Same(characters1, characters2);
        }

        [Fact]
        public void LeftDelimiterReturnsCorrectValue()
        {
            Assert.Equal("[", MsSqlCharacters.Instance.LeftDelimiter);
        }

        [Fact]
        public void RightDelimiterReturnsCorrectValue()
        {
            Assert.Equal("]", MsSqlCharacters.Instance.RightDelimiter);
        }

        [Fact]
        public void SqlParameterReturnsAtSign()
        {
            Assert.Equal("@", MsSqlCharacters.Instance.SqlParameter);
        }

        [Fact]
        public void StoredProcedureInvocationCommandReturnsExec()
        {
            Assert.Equal("EXEC", MsSqlCharacters.Instance.StoredProcedureInvocationCommand);
        }

        [Fact]
        public void SupportsNamedParametersReturnsTrue()
        {
            Assert.True(MsSqlCharacters.Instance.SupportsNamedParameters);
        }
    }
}