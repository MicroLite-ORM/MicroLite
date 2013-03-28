using System;
using MicroLite.TypeConverters;
using Xunit;

namespace MicroLite.Tests.TypeConverters
{
    public class ObjectTypeConverterTests
    {
        private enum Status
        {
            Default = 0,
            New = 1
        }

        public class WhenCallingCanConvertWithATypeWhichIsAnEnum
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

        public class WhenCallingCanConvertWithATypeWhichIsAnInt
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new ObjectTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(int)));
            }
        }

        public class WhenCallingCanConvertWithATypeWhichIsANullableInt
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new ObjectTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(int?)));
            }
        }

        public class WhenCallingConvertFromDbValueForANullableIntWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueForANullableIntWithANonNullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(1, typeof(int?));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueForANullableIntWithANullValue
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueForANullableIntWithANullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(int?));
            }

            [Fact]
            public void NullShouldBeReturned()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithAByteAndATypeOfInt
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueWithAByteAndATypeOfInt()
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

        public class WhenCallingConvertFromDbValueWithALongAndATypeOfInt
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertFromDbValueWithALongAndATypeOfInt()
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
    }
}