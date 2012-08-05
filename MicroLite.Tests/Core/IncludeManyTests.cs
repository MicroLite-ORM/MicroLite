namespace MicroLite.Tests.Core
{
    using System.Collections.Generic;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Mapping;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeMany&lt;T&gt;"/> class.
    /// </summary>
    [TestFixture]
    public class IncludeManyTests
    {
        [Test]
        public void ValuesReturnsEmptyListIfNoResultsInReader()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

            var include = new IncludeMany<Customer>();

            include.BuildValue(mockReader.Object, new Mock<IObjectBuilder>().Object);

            CollectionAssert.IsEmpty(include.Values);
        }

        [Test]
        public void ValuesReturnsNullIfNuildValueNotCalled()
        {
            var include = new IncludeMany<Customer>();

            Assert.IsNull(include.Values);
        }

        [Test]
        public void ValuesReturnsResults()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

            var reader = mockReader.Object;

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var include = new IncludeMany<Customer>();

            include.BuildValue(mockReader.Object, mockObjectBuilder.Object);

            mockReader.VerifyAll();
            mockObjectBuilder.VerifyAll();

            CollectionAssert.IsNotEmpty(include.Values);
            CollectionAssert.AllItemsAreInstancesOfType(include.Values, typeof(Customer));
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