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

    public class IncludeSingleTests
    {
        [TestFixture]
        public class WhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
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
            public void ValueShouldBeNull()
            {
                Assert.IsNull(this.include.Value);
            }
        }

        [TestFixture]
        public class WhenBuildValueHasBeenCalledAndThereIsOneResult
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenBuildValueHasBeenCalledAndThereIsOneResult()
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
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }
        }

        [TestFixture]
        public class WhenBuildValueHasNotBeenCalled
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();

            public WhenBuildValueHasNotBeenCalled()
            {
            }

            [Test]
            public void HasValueShouldBeFalse()
            {
                Assert.IsFalse(this.include.HasValue);
            }

            [Test]
            public void ValueShouldBeNull()
            {
                Assert.IsNull(this.include.Value);
            }
        }

        [TestFixture]
        public class WhenTheDataReaderContainsMultipleResults
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheDataReaderContainsMultipleResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);

                var reader = this.mockReader.Object;

                this.mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());
            }

            [Test]
            public void BuildValueShouldThrowAMicroLiteException()
            {
                var exception = Assert.Throws<MicroLiteException>(
                    () => this.include.BuildValue(this.mockReader.Object, this.mockObjectBuilder.Object));

                Assert.AreEqual(Messages.IncludeSingle_SingleResultExpected, exception.Message);
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