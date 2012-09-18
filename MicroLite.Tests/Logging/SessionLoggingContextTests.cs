namespace MicroLite.Tests.Logging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MicroLite.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SessionLoggingContext"/> class.
    /// </summary>
    public class SessionLoggingContextTests
    {
        [Test]
        public void ConstructorSetsCurrentSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (new SessionLoggingContext(sessionId))
            {
                Assert.AreEqual(sessionId, SessionLoggingContext.CurrentSessionId, "The static CurrentSessionId should match the session id passed to the constructor.");
            }
        }

        [Test]
        public void CurrentSessionIdIsUniquePerThread()
        {
            Parallel.For(1, 20, (x) =>
            {
                var sessionId = Guid.NewGuid().ToString();

                using (new SessionLoggingContext(sessionId))
                {
                    Assert.AreEqual(sessionId, SessionLoggingContext.CurrentSessionId);

                    Thread.Sleep(50);
                }
            });
        }

        [Test]
        public void DisposeClearsCurrentSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (new SessionLoggingContext(sessionId))
            {
            }

            Assert.IsNull(SessionLoggingContext.CurrentSessionId);
        }

        [Test]
        public void SessionLoggingContextCanBeNestedWithoutLoosingTheSessionId()
        {
            var sessionId = Guid.NewGuid().ToString();

            using (new SessionLoggingContext(sessionId))
            {
                Assert.AreEqual(sessionId, SessionLoggingContext.CurrentSessionId, "The outer context should set the session id");

                using (new SessionLoggingContext(sessionId))
                {
                    Assert.AreEqual(sessionId, SessionLoggingContext.CurrentSessionId, "The context using should use the same session id");
                }

                Assert.AreEqual(sessionId, SessionLoggingContext.CurrentSessionId, "Disposing the inner context shouldn't clear the session id from the outer context");
            }

            Assert.IsNull(SessionLoggingContext.CurrentSessionId, "Disposing the outer context should clear the session id");
        }
    }
}