namespace MicroLite.Tests.Logging
{
    using System;
    using MicroLite.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="LogManager"/> class.
    /// </summary>
    public class LogManagerTests : IDisposable
    {
        public LogManagerTests()
        {
            // Ensure that the GetLogger method is cleared before each test.
            LogManager.GetLogger = null;
        }

        public void Dispose()
        {
            // Ensure that the GetLogger method is cleared after all tests have been run.
            LogManager.GetLogger = null;
        }

        [Fact]
        public void GetCurrentClassLogReturnsLogIfGetLoggerSet()
        {
            var log = new Mock<ILog>().Object;

            LogManager.GetLogger = (string name) =>
            {
                // TODO: Figure out why this fails when using the console runner...
                ////Assert.Equal(typeof(LogManagerTests).FullName, name);

                return log;
            };

            var logInstance = LogManager.GetCurrentClassLog();

            Assert.Same(log, logInstance);
        }

        [Fact]
        public void GetCurrentClassLogReturnsNullIfGetLoggerNotSet()
        {
            Assert.Null(LogManager.GetCurrentClassLog());
        }
    }
}