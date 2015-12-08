namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Xml.Linq;
    using MicroLite.Core;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeSingle&lt;T&gt;"/> class.
    /// </summary>
    public class IncludeSingleTests
    {
        public class WhenBuildValueAsyncHasBeenCalledAndThereAreNoResults
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenBuildValueAsyncHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None).Wait();
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
            public void ValueShouldBeNull()
            {
                Assert.Null(this.include.Value);
            }
        }

        public class WhenBuildValueAsyncHasBeenCalledAndThereIsACallbackRegistered
        {
            private bool callbackCalled = false;
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenBuildValueAsyncHasBeenCalledAndThereIsACallbackRegistered()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.OnLoad(inc => callbackCalled = object.ReferenceEquals(inc, this.include));
                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None).Wait();
            }

            [Fact]
            public void HasValueShouldBeTrue()
            {
                Assert.True(this.include.HasValue);
            }

            [Fact]
            public void TheCallbackShouldBeCalled()
            {
                Assert.True(this.callbackCalled);
            }

            [Fact]
            public void TheDataReaderShouldBeRead()
            {
                this.mockReader.VerifyAll();
            }

            [Fact]
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }
        }

        public class WhenBuildValueAsyncHasBeenCalledAndThereIsOneResult
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenBuildValueAsyncHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None).Wait();
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
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }
        }

        public class WhenBuildValueHasNotBeenCalled
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();

            public WhenBuildValueHasNotBeenCalled()
            {
            }

            [Fact]
            public void HasValueShouldBeFalse()
            {
                Assert.False(this.include.HasValue);
            }

            [Fact]
            public void ValueShouldBeNull()
            {
                Assert.Null(this.include.Value);
            }
        }

        public class WhenCallingBuildValueAsyncAndTheDataReaderContainsMultipleResults
        {
            private IncludeSingle<Customer> include = new IncludeSingle<Customer>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenCallingBuildValueAsyncAndTheDataReaderContainsMultipleResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);
            }

            [Fact]
            public void BuildValueAsyncShouldThrowAMicroLiteException()
            {
                var exception = Assert.Throws<AggregateException>(
                    () => this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None).Wait());

                Assert.IsType<MicroLiteException>(exception.InnerException);
                Assert.Equal(ExceptionMessages.Include_SingleRecordExpected, exception.InnerException.Message);
            }
        }

        public class WhenTheTypeIsAGuid
        {
            private IncludeSingle<Guid> include = new IncludeSingle<Guid>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAGuid()
            {
                this.mockReader.Setup(x => x[0]).Returns(new Guid("97FE0200-8F79-4C3B-8CD4-BE97705868EC"));
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None);
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
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal(new Guid("97FE0200-8F79-4C3B-8CD4-BE97705868EC"), this.include.Value);
            }
        }

        public class WhenTheTypeIsAnEnum
        {
            private IncludeSingle<CustomerStatus> include = new IncludeSingle<CustomerStatus>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAnEnum()
            {
                this.mockReader.Setup(x => x.GetInt32(0)).Returns(1);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None);
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
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal(CustomerStatus.Active, this.include.Value);
            }
        }

        public class WhenTheTypeIsAnXDocument
        {
            private IncludeSingle<XDocument> include = new IncludeSingle<XDocument>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAnXDocument()
            {
                this.mockReader.Setup(x => x.GetString(0)).Returns("<xml><element>text</element></xml>");
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None);
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
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal(XDocument.Parse("<xml><element>text</element></xml>").ToString(SaveOptions.DisableFormatting), this.include.Value.ToString(SaveOptions.DisableFormatting));
            }
        }

        public class WhenTheTypeIsAString
        {
            private IncludeSingle<string> include = new IncludeSingle<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheTypeIsAString()
            {
                this.mockReader.Setup(x => x[0]).Returns("Foo");
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None);
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
            public void ValueShouldNotBeNull()
            {
                Assert.NotNull(this.include.Value);
            }

            [Fact]
            public void ValuesShouldContainTheResultOfTheTypeConversion()
            {
                Assert.Equal("Foo", this.include.Value);
            }
        }
    }
}