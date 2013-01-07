using System;
using MicroLite.Mapping;
using Xunit;

namespace MicroLite.Tests.Mapping
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

        public class WhenCallingConvertForANullableIntWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertForANullableIntWithANonNullValue()
            {
                this.result = typeConverter.Convert(1, typeof(Status?));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }

        public class WhenCallingConvertForANullableIntWithANullableValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertForANullableIntWithANullableValue()
            {
                this.result = typeConverter.Convert(DBNull.Value, typeof(Status?));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertWithAByte
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertWithAByte()
            {
                this.result = typeConverter.Convert((byte)1, typeof(Status));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }

        public class WhenCallingConvertWithALong
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertWithALong()
            {
                this.result = typeConverter.Convert((long)1, typeof(Status));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }

        public class WhenCallingConvertWithAnInt
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertWithAnInt()
            {
                this.result = typeConverter.Convert((int)1, typeof(Status));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Status.New, this.result);
            }
        }
    }
}