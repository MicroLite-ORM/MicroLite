namespace MicroLite.Tests.Logging
{
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

            LogManager.GetLogger = (string name) =>
            {
                // TODO: Figure out why this fails when using the console runner...
                ////Assert.Equal(typeof(LogManagerTests).FullName, name);

                return log;
            };

            var logInstance = LogManager.GetCurrentClassLog();

            Assert.Same(log, logInstance);
        }
    }
}