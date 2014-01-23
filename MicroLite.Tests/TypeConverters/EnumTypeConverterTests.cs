namespace MicroLite.Tests.TypeConverters
{
    using System;
    using MicroLite.TypeConverters;
    using Xunit;

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

        public class WhenCallingCanConvert_WithATypeWhichIsAnEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(IntEnumStatus)));
            }
        }

        public class WhenCallingCanConvert_WithATypeWhichIsANullableEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(IntEnumStatus?)));
            }
        }

        public class WhenCallingCanConvert_WithATypeWhichIsNotAnEnum
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.False(typeConverter.CanConvert(typeof(int)));
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableEnumWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_ForANullableEnumWithANonNullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(1, typeof(IntEnumStatus?));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableEnumWithANullableValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_ForANullableEnumWithANullableValue()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(IntEnumStatus?));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithAByte
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithAByte()
            {
                this.result = typeConverter.ConvertFromDbValue((byte)1, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithALong
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithALong()
            {
                this.result = typeConverter.ConvertFromDbValue((long)1, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithAnInt
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithAnInt()
            {
                this.result = typeConverter.ConvertFromDbValue((int)1, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(IntEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertToDbValue_ForAnEnumWithByteStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private ByteEnumStatus value = ByteEnumStatus.New;

            public WhenCallingConvertToDbValue_ForAnEnumWithByteStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(ByteEnumStatus));
            }

            [Fact]
            public void TheResultShouldBeAnInteger()
            {
                Assert.IsType<byte>(this.result);
            }
        }

        public class WhenCallingConvertToDbValue_ForAnEnumWithIntStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private IntEnumStatus value = IntEnumStatus.New;

            public WhenCallingConvertToDbValue_ForAnEnumWithIntStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(IntEnumStatus));
            }

            [Fact]
            public void TheResultShouldBeAnInteger()
            {
                Assert.IsType<int>(this.result);
            }
        }

        public class WhenCallingConvertToDbValue_ForANullableEnum
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private IntEnumStatus? value = null;

            public WhenCallingConvertToDbValue_ForANullableEnum()
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