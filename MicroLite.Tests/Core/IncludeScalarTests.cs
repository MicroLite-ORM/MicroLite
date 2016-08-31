namespace MicroLite.Tests.Core
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using MicroLite.Core;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeScalar&lt;T&gt;"/> class.
    /// </summary>
    public class IncludeScalarTests
    {
        /// <summary>
        /// Issue #172 - Cannot use Session.Include.Scalar to return an enum
        /// </summary>
        public class ForAnEnumTypeWhenBuildValueAsyncHasBeenCalledAndThereIsOneResult
        {
            private IncludeScalar<CustomerStatus> include = new IncludeScalar<CustomerStatus>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAnEnumTypeWhenBuildValueAsyncHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x.GetInt32(0)).Returns(1);
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
            public void ValueShouldBeSetToTheResult()
            {
                Assert.Equal(CustomerStatus.Active, this.include.Value);
            }
        }

        public class ForAReferenceTypeWhenBuildValueAsyncHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAReferenceTypeWhenBuildValueAsyncHasBeenCalledAndThereAreNoResults()
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

        public class ForAReferenceTypeWhenBuildValueAsyncHasBeenCalledAndThereIsACallbackRegistered
        {
            private bool callbackCalled = false;
            private IncludeScalar<string> include = new IncludeScalar<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAReferenceTypeWhenBuildValueAsyncHasBeenCalledAndThereIsACallbackRegistered()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns("Foo");
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
            public void ValueShouldBeSetToTheResult()
            {
                Assert.Equal("Foo", this.include.Value);
            }
        }

        public class ForAReferenceTypeWhenBuildValueAsyncHasBeenCalledAndThereIsOneResult
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAReferenceTypeWhenBuildValueAsyncHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns("Foo");
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
            public void ValueShouldBeSetToTheResult()
            {
                Assert.Equal("Foo", this.include.Value);
            }
        }

        public class ForAReferenceTypeWhenBuildValueAsyncHasNotBeenCalled
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();

            public ForAReferenceTypeWhenBuildValueAsyncHasNotBeenCalled()
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

        public class ForAValueTypeWhenBuildAsyncValueHasNotBeenCalled
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();

            public ForAValueTypeWhenBuildAsyncValueHasNotBeenCalled()
            {
            }

            [Fact]
            public void HasValueShouldBeFalse()
            {
                Assert.False(this.include.HasValue);
            }

            [Fact]
            public void ValueShouldBeDefaultValue()
            {
                Assert.Equal(0, this.include.Value);
            }
        }

        public class ForAValueTypeWhenBuildValueAsyncHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAValueTypeWhenBuildValueAsyncHasBeenCalledAndThereAreNoResults()
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
            public void ValueShouldBeDefaultValue()
            {
                Assert.Equal(0, this.include.Value);
            }
        }

        public class ForAValueTypeWhenBuildValueAsyncHasBeenCalledAndThereIsOneResult
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAValueTypeWhenBuildValueAsyncHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns(10);
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
            public void ValueShouldBeSetToTheResult()
            {
                Assert.Equal(10, this.include.Value);
            }
        }

        public class WhenCallingBuildValueAsyncAndTheDataReaderContainsMultipleColumns
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenCallingBuildValueAsyncAndTheDataReaderContainsMultipleColumns()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(2);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);
            }

            [Fact]
            public async void BuildValueAsyncShouldThrowAMicroLiteException()
            {
                var exception = await Assert.ThrowsAsync<MicroLiteException>(
                    async () => await this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None));

                Assert.Equal(ExceptionMessages.IncludeScalar_MultipleColumns, exception.Message);
            }
        }

        public class WhenCallingBuildValueAsyncAndTheDataReaderContainsMultipleResults
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenCallingBuildValueAsyncAndTheDataReaderContainsMultipleResults()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns(10);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);
            }

            [Fact]
            public async void BuildValueAsyncShouldThrowAMicroLiteException()
            {
                var exception = await Assert.ThrowsAsync<MicroLiteException>(
                    async () => await this.include.BuildValueAsync(new MockDbDataReaderWrapper(this.mockReader.Object), CancellationToken.None));

                Assert.Equal(ExceptionMessages.Include_SingleRecordExpected, exception.Message);
            }
        }
    }
}