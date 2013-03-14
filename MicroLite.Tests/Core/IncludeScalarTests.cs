namespace MicroLite.Tests.Core
{
    using System.Collections.Generic;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="IncludeScalar&lt;T&gt;"/> class.
    /// </summary>
    public class IncludeScalarTests
    {
        private enum Status
        {
            New = 0,
            Saved = 1
        }

        /// <summary>
        /// Issue #172 - Cannot use Session.Include.Scalar to return an enum
        /// </summary>
        public class ForAnEnumTypeWhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<Status> include = new IncludeScalar<Status>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAnEnumTypeWhenBuildValueHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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
                Assert.Equal(default(Status), this.include.Value);
            }
        }

        /// <summary>
        /// Issue #172 - Cannot use Session.Include.Scalar to return an enum
        /// </summary>
        public class ForAnEnumTypeWhenBuildValueHasBeenCalledAndThereIsOneResult
        {
            private IncludeScalar<Status> include = new IncludeScalar<Status>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAnEnumTypeWhenBuildValueHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns(1);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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
                Assert.Equal(Status.Saved, this.include.Value);
            }
        }

        public class ForAReferenceTypeWhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAReferenceTypeWhenBuildValueHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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

        public class ForAReferenceTypeWhenBuildValueHasBeenCalledAndThereIsOneResult
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAReferenceTypeWhenBuildValueHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns("Foo");
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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

        public class ForAReferenceTypeWhenBuildValueHasNotBeenCalled
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();

            public ForAReferenceTypeWhenBuildValueHasNotBeenCalled()
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

        public class ForAValueTypeWhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAValueTypeWhenBuildValueHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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

        public class ForAValueTypeWhenBuildValueHasBeenCalledAndThereIsOneResult
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAValueTypeWhenBuildValueHasBeenCalledAndThereIsOneResult()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns(10);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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

        public class ForAValueTypeWhenBuildValueHasNotBeenCalled
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();

            public ForAValueTypeWhenBuildValueHasNotBeenCalled()
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

        public class WhenTheDataReaderContainsMultipleColumns
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheDataReaderContainsMultipleColumns()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(2);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);
            }

            [Fact]
            public void BuildValueShouldThrowAMicroLiteException()
            {
                var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, null));

                Assert.Equal(Messages.IncludeScalar_MultipleColumns, exception.Message);
            }
        }

        public class WhenTheDataReaderContainsMultipleResults
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheDataReaderContainsMultipleResults()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(1);
                this.mockReader.Setup(x => x[0]).Returns(10);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);
            }

            [Fact]
            public void BuildValueShouldThrowAMicroLiteException()
            {
                var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, null));

                Assert.Equal(Messages.IncludeSingle_SingleResultExpected, exception.Message);
            }
        }
    }
}