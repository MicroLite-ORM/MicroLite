namespace MicroLite.Tests.Core
{
    using System.Collections.Generic;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Mapping;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeSingle&lt;T&gt;"/> class.
    /// </summary>
    [TestFixture]
    public class IncludeSingleTests
    {
        [Test]
        public void BuildValueThrowsMicroLiteExceptionIfMoreThanOneResult()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);

            var reader = mockReader.Object;

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var include = new IncludeSingle<Customer>();

            var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, mockObjectBuilder.Object));

            Assert.AreEqual(Messages.IncludeSingle_SingleResultExpected, exception.Message);
        }

        [Test]
        public void ValueReturnsNullIfNoResultsInReader()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);

            var reader = mockReader.Object;

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader));

            var include = new IncludeSingle<Customer>();

            include.BuildValue(reader, mockObjectBuilder.Object);

            mockObjectBuilder.Verify(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader), Times.Never(), "If the first call to IDataReader.Read() returns false, we should not try and create an object.");

            Assert.IsNull(include.Value);
        }

        [Test]
        public void ValueReturnsNullIfNuildValueNotCalled()
        {
            var include = new IncludeSingle<Customer>();

            Assert.IsNull(include.Value);
        }

        [Test]
        public void ValueReturnsResults()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

            var reader = mockReader.Object;

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var include = new IncludeSingle<Customer>();

            include.BuildValue(mockReader.Object, mockObjectBuilder.Object);

            mockReader.VerifyAll();
            mockObjectBuilder.VerifyAll();

            Assert.NotNull(include.Value);
        }

        [MicroLite.Mapping.Table("dbo", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}