using System;
using MicroLite.TypeConverters;
using Xunit;

namespace MicroLite.Tests.TypeConverters
{
    public class EnumTypeConverterTests
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
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(Status)));
            }
        }

        public class WhenCallingCanConvertWithATypeWhichIsANullableEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(Status?)));
            }
        }

        public class WhenCallingCanConvertWithATypeWhichIsNotAnEnum
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.False(typeConverter.CanConvert(typeof(int)));
            }
        }

        public class WhenCallingConvertFromDbValueForANullableEnumWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueForANullableEnumWithANonNullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(1, typeof(Status?));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueForANullableEnumWithANullableValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueForANullableEnumWithANullableValue()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(Status?));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithAByte
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithAByte()
            {
                this.result = typeConverter.ConvertFromDbValue((byte)1, typeof(Status));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithALong
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithALong()
            {
                this.result = typeConverter.ConvertFromDbValue((long)1, typeof(Status));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithAnInt
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithAnInt()
            {
                this.result = typeConverter.ConvertFromDbValue((int)1, typeof(Status));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }
    }
}