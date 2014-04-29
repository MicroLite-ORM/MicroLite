namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Data;
    using MicroLite.TypeConverters;
    using Moq;
    using Xunit;

    public class ObjectTypeConverterTests
    {
        private enum Status
        {
            Default = 0,
            New = 1
        }

        public class WhenCallingCanConvert_WithATypeWhichIsAnEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                // Although an exlicit type converter exists for enums, ObjectTypeConverter should not discriminate against any type.
                // This is so we don't have to modify it to ignore types for which there is a specific converter.
                var typeConverter = new ObjectTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(Status)));
            }
        }

        public class WhenCallingCanConvert_WithATypeWhichIsAnInt
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new ObjectTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(int)));
            }
        }

        public class WhenCallingCanConvert_WithATypeWhichIsANullableInt
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new ObjectTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(int?)));
            }
        }

        public class WhenCallingConvertFromDbValue_AndTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new ObjectTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(1, null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableIntWithANonNullValue
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValue_ForANullableIntWithANonNullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(1, typeof(int?));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableIntWithANullValue
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValue_ForANullableIntWithANullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(int?));
            }

            [Fact]
            public void NullShouldBeReturned()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithAByteAndATypeOfInt
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValue_WithAByteAndATypeOfInt()
            {
                this.result = typeConverter.ConvertFromDbValue((byte)1, typeof(int));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }

            [Fact]
            public void TheResultShouldBeUpCast()
            {
                Assert.IsType<int>(this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithALongAndATypeOfInt
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValue_WithALongAndATypeOfInt()
            {
                this.result = typeConverter.ConvertFromDbValue((long)1, typeof(int));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }

            [Fact]
            public void TheResultShouldBeDownCast()
            {
                Assert.IsType<int>(this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndReaderIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new ObjectTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(null, 0, typeof(int)));

                Assert.Equal("reader", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new ObjectTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(new Mock<IDataReader>().Object, 0, null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_ForANullableIntWithANonNullValue
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_ForANullableIntWithANonNullValue()
            {
                this.mockReader.Setup(x => x[0]).Returns(1);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(int?));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_ForANullableIntWithANullValue
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_ForANullableIntWithANullValue()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(true);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(int?));
            }

            [Fact]
            public void NullShouldBeReturned()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_WithAByteAndATypeOfInt
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_WithAByteAndATypeOfInt()
            {
                this.mockReader.Setup(x => x[0]).Returns((byte)1);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(int));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }

            [Fact]
            public void TheResultShouldBeUpCast()
            {
                Assert.IsType<int>(this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_WithALongAndATypeOfInt
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_WithALongAndATypeOfInt()
            {
                this.mockReader.Setup(x => x[0]).Returns((long)1);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(int));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }

            [Fact]
            public void TheResultShouldBeDownCast()
            {
                Assert.IsType<int>(this.result);
            }
        }

        public class WhenCallingConvertToDbValue
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new ObjectTypeConverter();
            private readonly string value = "Foo";

            public WhenCallingConvertToDbValue()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(string));
            }

            [Fact]
            public void TheSameValueShouldBeReturned()
            {
                Assert.Same(this.value, this.result);
            }
        }
    }
}