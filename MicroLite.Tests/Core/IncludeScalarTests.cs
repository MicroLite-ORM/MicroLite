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
    public class IncludeScalarTests
    {
        [TestFixture]
        public class ForAReferenceTypeWhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAReferenceTypeWhenBuildValueHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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
            public void ValueShouldBeNull()
            {
                Assert.IsNull(this.include.Value);
            }
        }

        [TestFixture]
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
            public void ValueShouldBeSetToTheResult()
            {
                Assert.AreEqual("Foo", this.include.Value);
            }
        }

        [TestFixture]
        public class ForAReferenceTypeWhenBuildValueHasNotBeenCalled
        {
            private IncludeScalar<string> include = new IncludeScalar<string>();

            public ForAReferenceTypeWhenBuildValueHasNotBeenCalled()
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
        public class ForAValueTypeWhenBuildValueHasBeenCalledAndThereAreNoResults
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public ForAValueTypeWhenBuildValueHasBeenCalledAndThereAreNoResults()
            {
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { false }).Dequeue);

                this.include.BuildValue(this.mockReader.Object, new Mock<IObjectBuilder>().Object);
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
            public void ValueShouldBeDefaultValue()
            {
                Assert.AreEqual(0, this.include.Value);
            }
        }

        [TestFixture]
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
            public void ValueShouldBeSetToTheResult()
            {
                Assert.AreEqual(10, this.include.Value);
            }
        }

        [TestFixture]
        public class ForAValueTypeWhenBuildValueHasNotBeenCalled
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();

            public ForAValueTypeWhenBuildValueHasNotBeenCalled()
            {
            }

            [Test]
            public void HasValueShouldBeFalse()
            {
                Assert.IsFalse(this.include.HasValue);
            }

            [Test]
            public void ValueShouldBeDefaultValue()
            {
                Assert.AreEqual(0, this.include.Value);
            }
        }

        [TestFixture]
        public class WhenTheDataReaderContainsMultipleColumns
        {
            private IncludeScalar<int> include = new IncludeScalar<int>();
            private Mock<IDataReader> mockReader = new Mock<IDataReader>();

            public WhenTheDataReaderContainsMultipleColumns()
            {
                this.mockReader.Setup(x => x.FieldCount).Returns(2);
                this.mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, true }).Dequeue);
            }

            [Test]
            public void BuildValueShouldThrowAMicroLiteException()
            {
                var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, null));

                Assert.AreEqual(Messages.IncludeScalar_MultipleColumns, exception.Message);
            }
        }

        [TestFixture]
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

            [Test]
            public void BuildValueShouldThrowAMicroLiteException()
            {
                var exception = Assert.Throws<MicroLiteException>(() => include.BuildValue(mockReader.Object, null));

                Assert.AreEqual(Messages.IncludeSingle_SingleResultExpected, exception.Message);
            }
        }
    }
}