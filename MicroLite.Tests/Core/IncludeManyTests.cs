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
    public class IncludeManyTests
    {
        [TestFixture]
        public class WhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeMany<Customer> include = new IncludeMany<Customer>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenBuildValueHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                var reader = this.mockReader.Object;

                this.mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader));

                this.include.BuildValue(this.mockReader.Object, this.mockObjectBuilder.Object);
            }

            [Test]
            public void HasValueShouldBeFalse()
            {
                Assert.IsFalse(this.include.HasValue);
            }

            [Test]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Test]
            public void TheObjectBuilderShouldNotBuildAnyObjects()
            {
                this.mockObjectBuilder.Verify(
                    x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), It.IsAny<IDataReader>()),
                    Times.Never(),
                    "If the first call to IDataReader.Read() returns false, we should not try and create an object.");
            }

            [Test]
            public void ValuesShouldBeEmpty()
            {
                CollectionAssert.IsEmpty(this.include.Values);
            }
        }

        [TestFixture]
        public class WhenBuildValueHasBeenCalledAndThereAreResults
        {
            private IncludeMany<Customer> include = new IncludeMany<Customer>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenBuildValueHasBeenCalledAndThereAreResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var reader = this.mockReader.Object;

                this.mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

                this.include.BuildValue(reader, this.mockObjectBuilder.Object);
            }

            [Test]
            public void HasValueShouldBeTrue()
            {
                Assert.IsTrue(this.include.HasValue);
            }

            [Test]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Test]
            public void TheObjectBuilderShouldBeCalled()
            {
                this.mockObjectBuilder.VerifyAll();
            }

            [Test]
            public void ValuesShouldNotBeEmpty()
            {
                CollectionAssert.IsNotEmpty(this.include.Values);
            }
        }

        [TestFixture]
        public class WhenBuildValueHasNotBeenCalled
        {
            private IncludeMany<Customer> include = new IncludeMany<Customer>();

            public WhenBuildValueHasNotBeenCalled()
            {
            }

            [Test]
            public void HasValueShouldBeFalse()
            {
                Assert.IsFalse(this.include.HasValue);
            }

            [Test]
            public void ValuesShouldBeEmpty()
            {
                CollectionAssert.IsEmpty(this.include.Values);
            }
        }

        [MicroLite.Mapping.Table("Customers")]
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