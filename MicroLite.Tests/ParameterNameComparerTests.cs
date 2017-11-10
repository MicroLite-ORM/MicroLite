namespace MicroLite.Tests
{
    using Xunit;

    public class ParameterNameComparerTests
    {
        [Fact]
        public void Parameter101SortsAfterParameter12()
        {
            Assert.Equal(1, ParameterNameComparer.Instance.Compare("@p101", "@p12"));
        }

        [Fact]
        public void Parameter1SortsBeforeParameter10()
        {
            Assert.Equal(-1, ParameterNameComparer.Instance.Compare("@p1", "@p10"));
        }

        [Fact]
        public void Parameter1SortsEqualToParameter1()
        {
            Assert.Equal(0, ParameterNameComparer.Instance.Compare("@p1", "@p1"));
        }

        [Fact]
        public void Parameter9SortsBeforeParameter10()
        {
            Assert.Equal(-1, ParameterNameComparer.Instance.Compare("@p9", "@p10"));
        }
    }
}