namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Data;
    using System.Xml.Linq;
    using MicroLite.TypeConverters;
    using Moq;
    using Xunit;

    public class XDocumentTypeConverterTests
    {
        public class WhenCallingCanConvert_WithTypeOfXDocument
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new XDocumentTypeConverter();

                Assert.True(typeConverter.CanConvert(typeof(XDocument)));
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new XDocumentTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue("foo", null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueIsNotNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new XDocumentTypeConverter();
            private readonly string value = "<customer><name>fred</name></customer>";

            public WhenCallingConvertFromDbValue_AndTheValueIsNotNull()
            {
                this.result = typeConverter.ConvertFromDbValue(value, typeof(XDocument));
            }

            [Fact]
            public void TheResultShouldBeAnXDocument()
            {
                Assert.IsType<XDocument>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(
                    XDocument.Parse(this.value).ToString(SaveOptions.DisableFormatting),
                    ((XDocument)this.result).ToString(SaveOptions.DisableFormatting));
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueIsNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new XDocumentTypeConverter();

            public WhenCallingConvertFromDbValue_AndTheValueIsNull()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(XDocument));
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
                var typeConverter = new XDocumentTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(null, 0, typeof(XDocument)));

                Assert.Equal("reader", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new XDocumentTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(new Mock<IDataReader>().Object, 0, null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNotNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new XDocumentTypeConverter();
            private readonly string value = "<customer><name>fred</name></customer>";

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNotNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetString(0)).Returns(this.value);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(XDocument));
            }

            [Fact]
            public void TheResultShouldBeAnXDocument()
            {
                Assert.IsType<XDocument>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(
                    XDocument.Parse(this.value).ToString(SaveOptions.DisableFormatting),
                    ((XDocument)this.result).ToString(SaveOptions.DisableFormatting));
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new XDocumentTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(true);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(XDocument));
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
            private readonly ITypeConverter typeConverter = new XDocumentTypeConverter();
            private readonly XDocument value = XDocument.Parse("<customer><name>fred</name></customer>");

            public WhenCallingConvertToDbValue_AndTheValueIsNotNull()
            {
                this.result = typeConverter.ConvertToDbValue(value, typeof(XDocument));
            }

            [Fact]
            public void TheResultShouldBeAString()
            {
                Assert.IsType<string>(this.result);
            }

            [Fact]
            public void TheResultShouldContainTheSpecifiedValue()
            {
                Assert.Equal(this.value.ToString(SaveOptions.DisableFormatting), this.result);
            }
        }

        public class WhenCallingConvertToDbValue_AndTheValueIsNull
        {
            private readonly object result;
            private readonly ITypeConverter typeConverter = new XDocumentTypeConverter();

            public WhenCallingConvertToDbValue_AndTheValueIsNull()
            {
                this.result = typeConverter.ConvertToDbValue(null, typeof(XDocument));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }
    }
}