using System;
using System.Xml.Linq;
using MicroLite.TypeConverters;
using Xunit;

namespace MicroLite.Tests.TypeConverters
{
    public class XDocumentTypeConverterTests
    {
        public class WhenCallingCanConvertTypeOfXDocument
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new XDocumentTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(XDocument)));
            }
        }

        public class WhenCallingConvertFromDbValueAndTheValueIsNotNull
        {
            private object result;
            private ITypeConverter typeConverter = new XDocumentTypeConverter();
            private string value = "<customer><name>fred</name></customer>";

            public WhenCallingConvertFromDbValueAndTheValueIsNotNull()
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

        public class WhenCallingConvertFromDbValueAndTheValueIsNull
        {
            private object result;
            private ITypeConverter typeConverter = new XDocumentTypeConverter();

            public WhenCallingConvertFromDbValueAndTheValueIsNull()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(XDocument));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }

        public class WhenCallingConvertToDbValueAndTheValueIsNotNull
        {
            private object result;
            private ITypeConverter typeConverter = new XDocumentTypeConverter();
            private XDocument value = XDocument.Parse("<customer><name>fred</name></customer>");

            public WhenCallingConvertToDbValueAndTheValueIsNotNull()
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

        public class WhenCallingConvertToDbValueAndTheValueIsNull
        {
            private object result;
            private ITypeConverter typeConverter = new XDocumentTypeConverter();

            public WhenCallingConvertToDbValueAndTheValueIsNull()
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