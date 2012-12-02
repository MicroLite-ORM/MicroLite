namespace MicroLite.Tests.Logging
{
    using System;
    using MicroLite.Logging;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="CurrentSessionIdResolver"/> class.
    /// </summary>

    public class CurrentSessionIdResolverTests
    {
        [Fact]
        public void ToStringReturnsCurrentSessionIdIfContextActive()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (var context = new SessionLoggingContext(sessionId))
            {
                var resolver = new CurrentSessionIdResolver();

                Assert.Equal(sessionId, resolver.ToString());
            }
        }

        [Fact]
        public void ToStringReturnsNullIfNoContextActive()
        {
            var resolver = new CurrentSessionIdResolver();

            Assert.Null(resolver.ToString());
        }
    }
}