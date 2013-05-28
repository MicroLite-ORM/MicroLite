namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Xml.Linq;
    using MicroLite.Core;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeMany&lt;T&gt;"/> class.
    /// </summary>
    public class IncludeManyTests
    {
        private enum CustomerStatus
        {
            Disabled = 0,
            Active = 1
        }

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

            [Fact]
            public void HasValueShouldBeFalse()
            {
                Assert.False(this.include.HasValue);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void TheObjectBuilderShouldNotBuildAnyObjects()
            {
                this.mockObjectBuilder.Verify(
                    x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), It.IsAny<IDataReader>()),
                    Times.Never(),
                    "If the first call to IDataReader.Read() returns false, we should not try and create an object.");
            }

            [Fact]
            public void ValuesShouldBeEmpty()
            {
                Assert.Empty(this.include.Values);
            }
        }

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

            [Fact]
            public void HasValueShouldBeTrue()
            {
                Assert.True(this.include.HasValue);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void TheObjectBuilderShouldBeCalled()
            {
                this.mockObjectBuilder.VerifyAll();
            }

            [Fact]
            public void ValuesShouldNotBeEmpty()
            {
                Assert.NotEmpty(this.include.Values);
            }
        }

        public class WhenBuildValueHasNotBeenCalled
        {
            private IncludeMany<Customer> include = new IncludeMany<Customer>();

            public WhenBuildValueHasNotBeenCalled()
            {
            }

            [Fact]
            public void HasValueShouldBeFalse()
            {
                Assert.False(this.include.HasValue);
            }

            [Fact]
            public void ValuesShouldBeEmpty()
            {
                Assert.Empty(this.include.Values);
            }
        }

        public class WhenTheTypeIsAGuid
        {
            private IncludeMany<Guid> include = new IncludeMany<Guid>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAGuid()
            {
                this.mockReader.Setup(x => x[0]).Returns(new Guid("97FE0200-8F79-4C3B-8CD4-BE97705868EC"));
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var reader = this.mockReader.Object;

                this.include.BuildValue(reader, this.mockObjectBuilder.Object);
            }

            [Fact]
            public void HasValueShouldBeTrue()
            {
                Assert.True(this.include.HasValue);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void TheObjectBuilderShouldNotBeUsed()
            {
                this.mockObjectBuilder.Verify(x => x.BuildInstance<Guid>(It.IsAny<IObjectInfo>(), It.IsAny<IDataReader>()), Times.Never());
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal(Guid.Parse("97FE0200-8F79-4C3B-8CD4-BE97705868EC"), this.include.Values[0]);
            }

            [Fact]
            public void ValuesShouldNotBeEmpty()
            {
                Assert.NotEmpty(this.include.Values);
            }
        }

        public class WhenTheTypeIsAnEnum
        {
            private IncludeMany<CustomerStatus> include = new IncludeMany<CustomerStatus>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAnEnum()
            {
                this.mockReader.Setup(x => x[0]).Returns(1);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var reader = this.mockReader.Object;

                this.include.BuildValue(reader, this.mockObjectBuilder.Object);
            }

            [Fact]
            public void HasValueShouldBeTrue()
            {
                Assert.True(this.include.HasValue);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void TheObjectBuilderShouldNotBeUsed()
            {
                this.mockObjectBuilder.Verify(x => x.BuildInstance<CustomerStatus>(It.IsAny<IObjectInfo>(), It.IsAny<IDataReader>()), Times.Never());
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal(CustomerStatus.Active, this.include.Values[0]);
            }

            [Fact]
            public void ValuesShouldNotBeEmpty()
            {
                Assert.NotEmpty(this.include.Values);
            }
        }

        public class WhenTheTypeIsAnXDocument
        {
            private IncludeMany<XDocument> include = new IncludeMany<XDocument>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAnXDocument()
            {
                this.mockReader.Setup(x => x[0]).Returns("<xml><element>text</element></xml>");
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var reader = this.mockReader.Object;

                this.include.BuildValue(reader, this.mockObjectBuilder.Object);
            }

            [Fact]
            public void HasValueShouldBeTrue()
            {
                Assert.True(this.include.HasValue);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void TheObjectBuilderShouldNotBeUsed()
            {
                this.mockObjectBuilder.Verify(x => x.BuildInstance<XDocument>(It.IsAny<IObjectInfo>(), It.IsAny<IDataReader>()), Times.Never());
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal(XDocument.Parse("<xml><element>text</element></xml>").ToString(SaveOptions.DisableFormatting), this.include.Values[0].ToString(SaveOptions.DisableFormatting));
            }

            [Fact]
            public void ValuesShouldNotBeEmpty()
            {
                Assert.NotEmpty(this.include.Values);
            }
        }

        public class WhenTheTypeIsAString
        {
            private IncludeMany<string> include = new IncludeMany<string>();
            private Mock<IObjectBuilder> mockObjectBuilder = new Mock<IObjectBuilder>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAString()
            {
                this.mockReader.Setup(x => x[0]).Returns("Foo");
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var reader = this.mockReader.Object;

                this.include.BuildValue(reader, this.mockObjectBuilder.Object);
            }

            [Fact]
            public void HasValueShouldBeTrue()
            {
                Assert.True(this.include.HasValue);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void TheObjectBuilderShouldNotBeUsed()
            {
                this.mockObjectBuilder.Verify(x => x.BuildInstance<string>(It.IsAny<IObjectInfo>(), It.IsAny<IDataReader>()), Times.Never());
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal("Foo", this.include.Values[0]);
            }

            [Fact]
            public void ValuesShouldNotBeEmpty()
            {
                Assert.NotEmpty(this.include.Values);
            }
        }

        [MicroLite.Mapping.Table("Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.DbGenerated)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}