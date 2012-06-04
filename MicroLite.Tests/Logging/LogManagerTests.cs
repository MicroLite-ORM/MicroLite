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
        public void GetCurrentClassLogReturnsNullIfGetLoggerNotSet()
        {
            Assert.IsNull(LogManager.GetCurrentClassLog());
        }

        [Test]
        public void GetCurrentClassLogReturnsLogIfGetLoggerSet()
        {
            var log = new Mock<ILog>().Object;

            LogManager.GetLogger = (string name) =>
            {
                Assert.AreEqual(typeof(LogManagerTests).FullName, name);

                return log;
            };

            var logInstance = LogManager.GetCurrentClassLog();

            Assert.AreSame(log, logInstance);
        }

        [Test]
        public void GetLogByNameReturnsLogIfGetLoggerSet()
        {
            var log = new Mock<ILog>().Object;

            LogManager.GetLogger = (string name) =>
            {
                Assert.AreEqual("LogManagerTests", name);

                return log;
            };

            var logInstance = LogManager.GetLog("LogManagerTests");

            Assert.AreSame(log, logInstance);
        }

        [Test]
        public void GetLogByNameReturnsNullIfGetLoggerNotSet()
        {
            Assert.IsNull(LogManager.GetLog("LogManagerTests"));
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