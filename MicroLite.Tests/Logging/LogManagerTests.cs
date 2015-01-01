namespace MicroLite.Tests.Logging
{
    using System;
    using MicroLite.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="LogManager"/> class.
    /// </summary>
    public class LogManagerTests : UnitTest
    {
        [Fact]
        public void GetCurrentClassLogReturnsEmptyLogIfGetLoggerNotSet()
        {
            Assert.IsType<EmptyLog>(LogManager.GetCurrentClassLog());
        }

        [Fact]
        public void GetCurrentClassLogReturnsLogIfGetLoggerSet()
        {
            var log = new Mock<ILog>().Object;

            LogManager.GetLogger = (Type type) =>
            {
                return log;
            };

            var logInstance = LogManager.GetCurrentClassLog();

            Assert.Same(log, logInstance);
        }
    }
}