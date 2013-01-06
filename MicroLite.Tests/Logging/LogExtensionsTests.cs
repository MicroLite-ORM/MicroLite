namespace MicroLite.Tests.Logging
{
    using System;
    using MicroLite.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="LogExtensions"/> class.
    /// </summary>
    public class LogExtensionsTests
    {
        [Fact]
        public void TryLogDebugWithArgs()
        {
            var message = "Some log message";
            var args = new[] { "foo" };

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(message, args));

            mockLog.Object.TryLogDebug(message, args);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogDebugWithoutArgs()
        {
            var message = "Some log message";

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Debug(message));

            mockLog.Object.TryLogDebug(message);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogErrorWithArgs()
        {
            var message = "Some log message";
            var args = new[] { "foo" };

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Error(message, args));

            mockLog.Object.TryLogError(message, args);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogErrorWithException()
        {
            var message = "Some log message";
            var exception = new Exception();

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Error(message, exception));

            mockLog.Object.TryLogError(message, exception);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogErrorWithoutArgs()
        {
            var message = "Some log message";

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Error(message));

            mockLog.Object.TryLogError(message);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogFatalWithArgs()
        {
            var message = "Some log message";
            var args = new[] { "foo" };

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Fatal(message, args));

            mockLog.Object.TryLogFatal(message, args);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogFatalWithException()
        {
            var message = "Some log message";
            var exception = new Exception();

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Fatal(message, exception));

            mockLog.Object.TryLogFatal(message, exception);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogFatalWithoutArgs()
        {
            var message = "Some log message";

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Fatal(message));

            mockLog.Object.TryLogFatal(message);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogInfoWithArgs()
        {
            var message = "Some log message";
            var args = new[] { "foo" };

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Info(message, args));

            mockLog.Object.TryLogInfo(message, args);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogInfoWithoutArgs()
        {
            var message = "Some log message";

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Info(message));

            mockLog.Object.TryLogInfo(message);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogWarnWithArgs()
        {
            var message = "Some log message";
            var args = new[] { "foo" };

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Warn(message, args));

            mockLog.Object.TryLogWarn(message, args);

            mockLog.VerifyAll();
        }

        [Fact]
        public void TryLogWarnWithoutArgs()
        {
            var message = "Some log message";

            var mockLog = new Mock<ILog>();
            mockLog.Setup(x => x.Warn(message));

            mockLog.Object.TryLogWarn(message);

            mockLog.VerifyAll();
        }
    }
}