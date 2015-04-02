namespace MicroLite.Tests.TypeConverters
{
    using System.Data;
    using MicroLite.TypeConverters;
    using Moq;
    using Xunit;

    public class TimeSpanTypeConverterTest
    {
        public class WhenCallingCanConvert_WithTypeOfTimeSpan
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new TimeSpanTypeConverter();

                Assert.True(typeConverter.CanConvert(typeof(System.TimeSpan)));
            }
        }

        public class WhenCallingCanConvert_WithTypeOfTimeSpanNullable
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new TimeSpanTypeConverter();

                Assert.True(typeConverter.CanConvert(typeof(System.TimeSpan?)));
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new TimeSpanTypeConverter();

                var exception = Assert.Throws<System.ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue("foo", null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueIsNotNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new TimeSpanTypeConverter();
            private readonly long value = 1234567890L;

            public WhenCallingConvertFromDbValue_AndTheValueIsNotNull()
            {
                this.result = typeConverter.ConvertFromDbValue(value, typeof(System.TimeSpan));
            }

            [Fact]
            public void TheResultShouldBeATimeSpan()
            {
                Assert.IsType<System.TimeSpan>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(new System.TimeSpan(this.value), this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueIsNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new TimeSpanTypeConverter();

            public WhenCallingConvertFromDbValue_AndTheValueIsNull()
            {
                this.result = typeConverter.ConvertFromDbValue(System.DBNull.Value, typeof(System.TimeSpan));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheReaderIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new TimeSpanTypeConverter();

                var exception = Assert.Throws<System.ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(null, 0, typeof(System.TimeSpan)));

                Assert.Equal("reader", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new TimeSpanTypeConverter();

                var exception = Assert.Throws<System.ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(new Mock<IDataReader>().Object, 0, null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNotNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new TimeSpanTypeConverter();
            private readonly long value = 1234567890L;

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNotNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetInt64(0)).Returns(this.value);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(System.TimeSpan));
            }

            [Fact]
            public void TheResultShouldBeATimeSpan()
            {
                Assert.IsType<System.TimeSpan>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(new System.TimeSpan(this.value), this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new TimeSpanTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(true);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(System.TimeSpan));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertToDbValue_AndTheValueIsNotNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new TimeSpanTypeConverter();
            private readonly System.TimeSpan value = new System.TimeSpan(1234567890L);

            public WhenCallingConvertToDbValue_AndTheValueIsNotNull()
            {
                this.result = typeConverter.ConvertToDbValue(value, typeof(System.TimeSpan));
            }

            [Fact]
            public void TheResultShouldBeAnInt64()
            {
                Assert.IsType<long>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(this.value.Ticks, this.result);
            }
        }

        public class WhenCallingConvertToDbValue_AndTheValueIsNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new TimeSpanTypeConverter();

            public WhenCallingConvertToDbValue_AndTheValueIsNull()
            {
                this.result = typeConverter.ConvertToDbValue(null, typeof(System.TimeSpan));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }
    }
}