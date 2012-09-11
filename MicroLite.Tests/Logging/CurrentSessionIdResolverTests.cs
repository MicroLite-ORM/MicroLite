namespace MicroLite.Tests.Logging
{
    using System;
    using MicroLite.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="CurrentSessionIdResolver"/> class.
    /// </summary>
    [TestFixture]
    public class CurrentSessionIdResolverTests
    {
        [Test]
        public void ToStringReturnsCurrentSessionIdIfContextActive()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (var context = new SessionLoggingContext(sessionId))
            {
                var resolver = new CurrentSessionIdResolver();

                Assert.AreEqual(sessionId, resolver.ToString());
            }
        }

        [Test]
        public void ToStringReturnsNullIfNoContextActive()
        {
            var resolver = new CurrentSessionIdResolver();

            Assert.IsNull(resolver.ToString());
        }
    }
}