namespace MicroLite.Tests.Logging
{
    using MicroLite.Logging;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="LogManager"/> class.
    /// </summary>
    [TestFixture]
    public class LogManagerTests
    {
        [Test]
        public void GetLogInstanceReturnsLogIfGetLoggerSet()
        {
            var log = new Mock<ILog>().Object;

            LogManager.GetLogger = (string name) =>
            {
                return log;
            };

            var logInstance = LogManager.GetLogInstance("MyLog");

            Assert.AreSame(log, logInstance);
        }

        [Test]
        public void GetLogInstanceReturnsNullIfGetLoggerNotSet()
        {
            Assert.IsNull(LogManager.GetLogInstance("MyLog"));
        }

        [SetUp]
        public void SetUp()
        {
            // Ensure that the GetLogger method is cleared before each test.
            LogManager.GetLogger = null;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            // Ensure that the GetLogger method is cleared after all tests have been run.
            LogManager.GetLogger = null;
        }
    }
}