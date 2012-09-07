namespace MicroLite.Tests.Core
{
    using System.Collections.Generic;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeScalar&lt;T&gt;"/> class.
    /// </summary>
    [TestFixture]
    public class IncludeScalarTests
    {
        [Test]
        public void BuildValueThrowsMicroLiteExceptionIfMoreThanOneColumn()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(2);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);

            var reader = mockReader.Object;

            var include = new IncludeScalar<int>();

            var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, null));

            Assert.AreEqual(Messages.IncludeScalar_MultipleColumns, exception.Message);
        }

        [Test]
        public void BuildValueThrowsMicroLiteExceptionIfMoreThanOneResult()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(10);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);

            var reader = mockReader.Object;

            var include = new IncludeScalar<int>();

            var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, null));

            Assert.AreEqual(Messages.IncludeSingle_SingleResultExpected, exception.Message);
        }

        [Test]
        public void ValueContainsDefaultValueForReferenceTypeIfBuildValueNotCalled()
        {
            var include = new IncludeScalar<string>();

            Assert.AreEqual(default(string), include.Value);
        }

        [Test]
        public void ValueContainsDefaultValueForReferenceTypeIfNoResultsInReader()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);

            var reader = mockReader.Object;

            var include = new IncludeScalar<string>();

            include.BuildValue(reader, null);

            Assert.AreEqual(default(string), include.Value);
        }

        [Test]
        public void ValueContainsDefaultValueForValueTypeIfBuildValueNotCalled()
        {
            var include = new IncludeScalar<int>();

            Assert.AreEqual(default(int), include.Value);
        }

        [Test]
        public void ValueContainsDefaultValueForValueTypeIfNoResultsInReader()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);

            var reader = mockReader.Object;

            var include = new IncludeScalar<int>();

            include.BuildValue(reader, null);

            Assert.AreEqual(default(int), include.Value);
        }

        [Test]
        public void ValueReturnsResults()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(10);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

            var reader = mockReader.Object;

            var include = new IncludeScalar<int>();

            include.BuildValue(reader, null);

            mockReader.VerifyAll();

            Assert.AreEqual(10, include.Value);
        }
    }
}