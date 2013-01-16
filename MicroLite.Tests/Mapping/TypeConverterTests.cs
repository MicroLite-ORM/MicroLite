using MicroLite.Mapping;
using Xunit;

namespace MicroLite.Tests.Mapping
{
    public class TypeConverterTests
    {
        private enum Status
        {
            Default = 0,
            New = 1
        }

        public class WhenCallingForTypeWithATypeOfEnum
        {
            [Fact]
            public void TheEnumTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.ForType(typeof(Status));
                Assert.IsType<EnumTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingForTypeWithATypeOfInt
        {
            [Fact]
            public void TheObjectTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.ForType(typeof(int));
                Assert.IsType<ObjectTypeConverter>(typeConverter);
            }
        }
    }
}