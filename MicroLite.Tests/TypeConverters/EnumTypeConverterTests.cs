using System;
using MicroLite.TypeConverters;
using Xunit;

namespace MicroLite.Tests.TypeConverters
{
    public class EnumTypeConverterTests
    {
        private enum ByteEnumStatus : byte
        {
            Default = 0,
            New = 1
        }

        private enum IntEnumStatus
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
                Assert.True(typeConverter.CanConvert(typeof(IntEnumStatus)));
            }
        }

        public class WhenCallingCanConvertWithATypeWhichIsANullableEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(IntEnumStatus?)));
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
                this.result = typeConverter.ConvertFromDbValue(1, typeof(IntEnumStatus?));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueForANullableEnumWithANullableValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueForANullableEnumWithANullableValue()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(IntEnumStatus?));
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
                this.result = typeConverter.ConvertFromDbValue((byte)1, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithALong
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithALong()
            {
                this.result = typeConverter.ConvertFromDbValue((long)1, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithAnInt
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithAnInt()
            {
                this.result = typeConverter.ConvertFromDbValue((int)1, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertToDbValueForAnEnumWithByteStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private ByteEnumStatus value = ByteEnumStatus.New;

            public WhenCallingConvertToDbValueForAnEnumWithByteStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(ByteEnumStatus));
            }

            [Fact]
            public void TheResultShouldBeAnInteger()
            {
                Assert.IsType<byte>(this.result);
            }
        }

        public class WhenCallingConvertToDbValueForAnEnumWithIntStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private IntEnumStatus value = IntEnumStatus.New;

            public WhenCallingConvertToDbValueForAnEnumWithIntStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheResultShouldBeAnInteger()
            {
                Assert.IsType<int>(this.result);
            }
        }

        public class WhenCallingConvertToDbValueForANullableEnum
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private IntEnumStatus? value = null;

            public WhenCallingConvertToDbValueForANullableEnum()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(IntEnumStatus?));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }
    }
}