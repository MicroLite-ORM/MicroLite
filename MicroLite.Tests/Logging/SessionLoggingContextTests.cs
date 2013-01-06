namespace MicroLite.Tests.Logging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MicroLite.Logging;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SessionLoggingContext"/> class.
    /// </summary>
    public class SessionLoggingContextTests
    {
        [Fact]
        public void ConstructorSetsCurrentSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (new SessionLoggingContext(sessionId))
            {
                Assert.Equal(sessionId, SessionLoggingContext.CurrentSessionId);
            }
        }

        [Fact]
        public void CurrentSessionIdIsUniquePerThread()
        {
            Parallel.For(1, 20, (x) =>
            {
                var sessionId = Guid.NewGuid().ToString();

                using (new SessionLoggingContext(sessionId))
                {
                    Assert.Equal(sessionId, SessionLoggingContext.CurrentSessionId);

                    Thread.Sleep(50);
                }
            });
        }

        [Fact]
        public void DisposeClearsCurrentSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (new SessionLoggingContext(sessionId))
            {
            }

            Assert.Null(SessionLoggingContext.CurrentSessionId);
        }

        [Fact]
        public void SessionLoggingContextCanBeNestedWithoutLoosingTheSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (new SessionLoggingContext(sessionId))
            {
                Assert.Equal(sessionId, SessionLoggingContext.CurrentSessionId);////, "The outer context should set the session id");

                using (new SessionLoggingContext(sessionId))
                {
                    Assert.Equal(sessionId, SessionLoggingContext.CurrentSessionId);////, "The context using should use the same session id");
                }

                Assert.Equal(sessionId, SessionLoggingContext.CurrentSessionId);////, "Disposing the inner context shouldn't clear the session id from the outer context");
            }

            Assert.Null(SessionLoggingContext.CurrentSessionId);////, "Disposing the outer context should clear the session id");
        }
    }
}