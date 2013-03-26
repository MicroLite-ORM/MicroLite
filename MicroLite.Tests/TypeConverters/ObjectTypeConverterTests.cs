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
            public void FalseShouldBeReturned()
            {
                var typeConverter = new ObjectTypeConverter();
                Assert.False(typeConverter.CanConvert(typeof(Status)));
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

        public class WhenCallingConvertForANullableIntWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertForANullableIntWithANonNullValue()
            {
                this.result = typeConverter.Convert(1, typeof(int?));
            }

            [Fact]
            public void TheCorrectValueShouldBeReturned()
            {
                Assert.Equal(1, this.result);
            }
        }

        public class WhenCallingConvertForANullableIntWithANullValue
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertForANullableIntWithANullValue()
            {
                this.result = typeConverter.Convert(DBNull.Value, typeof(int?));
            }

            [Fact]
            public void NullShouldBeReturned()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertWithAByteAndATypeOfInt
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertWithAByteAndATypeOfInt()
            {
                this.result = typeConverter.Convert((byte)1, typeof(int));
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

        public class WhenCallingConvertWithALongAndATypeOfInt
        {
            private object result;
            private ITypeConverter typeConverter = new ObjectTypeConverter();

            public WhenCallingConvertWithALongAndATypeOfInt()
            {
                this.result = typeConverter.Convert((long)1, typeof(int));
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