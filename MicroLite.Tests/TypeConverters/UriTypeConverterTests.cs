namespace MicroLite.Tests.TypeConverters
{
    using MicroLite.TypeConverters;
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

        public class WhenCallingConvertFromDbValue_AndTheValueIsNotNull
        {
            private object result;
            private ITypeConverter typeConverter = new UriTypeConverter();
            private string value = "http://microliteorm.wordpress.com";

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
            private object result;
            private ITypeConverter typeConverter = new UriTypeConverter();

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

        public class WhenCallingConvertToDbValue_AndTheValueIsNotNull
        {
            private object result;
            private ITypeConverter typeConverter = new UriTypeConverter();
            private System.Uri value = new System.Uri("http://microliteorm.wordpress.com");

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
            private object result;
            private ITypeConverter typeConverter = new UriTypeConverter();

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