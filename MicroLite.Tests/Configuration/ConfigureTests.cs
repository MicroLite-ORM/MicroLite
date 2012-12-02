namespace MicroLite.Tests.Configuration
{
    using MicroLite.Configuration;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="Configure"/> class.
    /// </summary>
    public class ConfigureTests
    {
        public class WhenCallingExtensionsMultipleTimes
        {
            private readonly IConfigureExtensions extensions1;
            private readonly IConfigureExtensions extensions2;

            public WhenCallingExtensionsMultipleTimes()
            {
                this.extensions1 = Configure.Extensions();
                this.extensions2 = Configure.Extensions();
            }

            [Fact]
            public void ANewInstanceShouldBeReturnedEachTime()
            {
                Assert.NotSame(this.extensions1, this.extensions2);
            }
        }

        public class WhenCallingFluentlyMultipleTimes
        {
            private readonly IConfigureConnection configure1;
            private readonly IConfigureConnection configure2;

            public WhenCallingFluentlyMultipleTimes()
            {
                this.configure1 = Configure.Fluently();
                this.configure2 = Configure.Fluently();
            }

            [Fact]
            public void ANewInstanceShouldBeReturnedEachTime()
            {
                Assert.NotSame(this.configure1, this.configure2);
            }
        }
    }
}