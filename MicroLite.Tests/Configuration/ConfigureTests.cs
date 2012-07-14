namespace MicroLite.Tests.Configuration
{
    using MicroLite.Configuration;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Configure"/> class.
    /// </summary>
    [TestFixture]
    public class ConfigureTests
    {
        [Test]
        public void ExtensionsReturnsNewInstanceOnEachCall()
        {
            var extensions1 = Configure.Extensions();
            var extensions2 = Configure.Extensions();

            Assert.AreNotSame(extensions1, extensions2);
        }

        [Test]
        public void FluentlyReturnsNewInstanceOnEachCall()
        {
            var configure1 = Configure.Fluently();
            var configure2 = Configure.Fluently();

            Assert.AreNotSame(configure1, configure2);
        }
    }
}