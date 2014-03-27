namespace MicroLite.Tests.TypeConverters
{
    using System.Data;
    using MicroLite.TypeConverters;
    using Moq;
    using Xunit;

    public class UriTypeConverterTests
    {
        public class WhenCallingCanConvert_WithTypeOfUri
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new UriTypeConverter();

                Assert.True(typeConverter.CanConvert(typeof(System.Uri)));
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new UriTypeConverter();

                var exception = Assert.Throws<System.ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue("foo", null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueIsNotNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new UriTypeConverter();
            private readonly string value = "http://microliteorm.wordpress.com";

            public WhenCallingConvertFromDbValue_AndTheValueIsNotNull()
            {
                this.result = typeConverter.ConvertFromDbValue(value, typeof(System.Uri));
            }

            [Fact]
            public void TheResultShouldBeAUri()
            {
                Assert.IsType<System.Uri>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(new System.Uri(this.value), this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueIsNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new UriTypeConverter();

            public WhenCallingConvertFromDbValue_AndTheValueIsNull()
            {
                this.result = typeConverter.ConvertFromDbValue(System.DBNull.Value, typeof(System.Uri));
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
                var typeConverter = new UriTypeConverter();

                var exception = Assert.Throws<System.ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(null, 0, typeof(System.Uri)));

                Assert.Equal("reader", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new UriTypeConverter();

                var exception = Assert.Throws<System.ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(new Mock<IDataReader>().Object, 0, null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNotNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new UriTypeConverter();
            private readonly string value = "http://microliteorm.wordpress.com";

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNotNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetString(0)).Returns(this.value);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(System.Uri));
            }

            [Fact]
            public void TheResultShouldBeAUri()
            {
                Assert.IsType<System.Uri>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(new System.Uri(this.value), this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new UriTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(true);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(System.Uri));
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
            private readonly ITypeConverter typeConverter = new UriTypeConverter();
            private readonly System.Uri value = new System.Uri("http://microliteorm.wordpress.com");

            public WhenCallingConvertToDbValue_AndTheValueIsNotNull()
            {
                this.result = typeConverter.ConvertToDbValue(value, typeof(System.Uri));
            }

            [Fact]
            public void TheResultShouldBeAString()
            {
                Assert.IsType<string>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(this.value.ToString(), this.result);
            }
        }

        public class WhenCallingConvertToDbValue_AndTheValueIsNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new UriTypeConverter();

            public WhenCallingConvertToDbValue_AndTheValueIsNull()
            {
                this.result = typeConverter.ConvertToDbValue(null, typeof(System.Uri));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }
    }
}