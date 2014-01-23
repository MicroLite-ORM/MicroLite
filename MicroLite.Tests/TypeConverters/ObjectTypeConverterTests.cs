namespace MicroLite.Tests.TypeConverters
{
    using System;
    using MicroLite.TypeConverters;
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

        public class WhenCallingConvertFromDbValue_AndPropertyTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new ObjectTypeConverter();
                var exception = Assert.Throws<ArgumentNullException>(() => typeConverter.ConvertFromDbValue(1, null));

                Assert.Equal("propertyType", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableIntWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

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
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

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
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

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
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

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

        public class WhenCallingConvertToDbValue
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();
            private string value = "Foo";

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