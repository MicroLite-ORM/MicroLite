namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Data;
    using MicroLite.TypeConverters;
    using Moq;
    using Xunit;

    public class EnumTypeConverterTests
    {
        private enum ByteEnumStatus : byte
        {
            Default = 0,
            New = 1
        }

        private enum Int16EnumStatus : short
        {
            Default = 0,
            New = 2
        }

        private enum Int32EnumStatus : int
        {
            Default = 0,
            New = 3
        }

        private enum Int64EnumStatus : long
        {
            Default = 0,
            New = 4
        }

        private enum UIntEnumStatus : uint
        {
            Default = 0,
            New = 3
        }

        public class WhenCallingCanConvert_WithATypeWhichIsAnEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(Int32EnumStatus)));
            }
        }

        public class WhenCallingCanConvert_WithATypeWhichIsANullableEnum
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new EnumTypeConverter();
                Assert.True(typeConverter.CanConvert(typeof(Int32EnumStatus?)));
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

        public class WhenCallingConvertFromDbValue_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new EnumTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue("foo", null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableEnumWithANonNullValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_ForANullableEnumWithANonNullValue()
            {
                this.result = typeConverter.ConvertFromDbValue(3, typeof(Int32EnumStatus?));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int32EnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_ForANullableEnumWithANullableValue
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_ForANullableEnumWithANullableValue()
            {
                this.result = typeConverter.ConvertFromDbValue(DBNull.Value, typeof(Int32EnumStatus?));
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
                this.result = typeConverter.ConvertFromDbValue((byte)1, typeof(ByteEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(ByteEnumStatus.New, this.result);
            }
        }

        /// <summary>
        /// SQLite stores all numbers as int64 so we need to convert that to the underlying storage type of the enum.
        /// </summary>
        public class WhenCallingConvertFromDbValue_WithADbValueOfADifferentTypeToTheUnderlyingStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithADbValueOfADifferentTypeToTheUnderlyingStorage()
            {
                this.result = typeConverter.ConvertFromDbValue((long)1, typeof(ByteEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(ByteEnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithALong
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithALong()
            {
                this.result = typeConverter.ConvertFromDbValue((long)4, typeof(Int64EnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int64EnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithAnInt
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithAnInt()
            {
                this.result = typeConverter.ConvertFromDbValue((int)3, typeof(Int32EnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int32EnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValue_WithAShort
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValue_WithAShort()
            {
                this.result = typeConverter.ConvertFromDbValue((short)2, typeof(Int16EnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int16EnumStatus.New, this.result);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsByte
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsByte()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetByte(0)).Returns((byte)1);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(ByteEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(ByteEnumStatus.New, this.result);
            }

            [Fact]
            public void TheValueShouldBeReadFromGetByte()
            {
                this.mockReader.Verify(x => x.GetByte(0), Times.Once());
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsInt
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsInt()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetInt32(0)).Returns(3);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(Int32EnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int32EnumStatus.New, this.result);
            }

            [Fact]
            public void TheValueShouldBeReadFromGetInt32()
            {
                this.mockReader.Verify(x => x.GetInt32(0), Times.Once());
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsLong
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsLong()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetInt64(0)).Returns((long)4);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(Int64EnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int64EnumStatus.New, this.result);
            }

            [Fact]
            public void TheValueShouldBeReadFromGetInt64()
            {
                this.mockReader.Verify(x => x.GetInt64(0), Times.Once());
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsShort
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsShort()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x.GetInt16(0)).Returns((short)2);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(Int16EnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(Int16EnumStatus.New, this.result);
            }

            [Fact]
            public void TheValueShouldBeReadFromGetInt16()
            {
                this.mockReader.Verify(x => x.GetInt16(0), Times.Once());
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsUInt
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheEnumStorageIsUInt()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(false);
                this.mockReader.Setup(x => x[0]).Returns(3);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(UIntEnumStatus));
            }

            [Fact]
            public void TheCorrectEnumValueShouldBeReturned()
            {
                Assert.Equal(UIntEnumStatus.New, this.result);
            }

            [Fact]
            public void TheValueShouldBeReadFromTheIndexer()
            {
                this.mockReader.Verify(x => x[0], Times.Once());
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheReaderIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new EnumTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(null, 0, typeof(Int32EnumStatus)));

                Assert.Equal("reader", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var typeConverter = new EnumTypeConverter();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => typeConverter.ConvertFromDbValue(new Mock<IDataReader>().Object, 0, null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull
        {
            private readonly Mock<IDataReader> mockReader = new Mock<IDataReader>();
            private readonly object result;
            private readonly ITypeConverter typeConverter = new EnumTypeConverter();

            public WhenCallingConvertFromDbValueWithReader_AndTheValueIsNull()
            {
                this.mockReader.Setup(x => x.IsDBNull(0)).Returns(true);

                this.result = typeConverter.ConvertFromDbValue(this.mockReader.Object, 0, typeof(Int32EnumStatus));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
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
            public void TheResultShouldBeAByte()
            {
                Assert.IsType<byte>(this.result);
            }

            [Fact]
            public void TheResultValueShouldBeCorrect()
            {
                Assert.Equal((byte)1, result);
            }
        }

        public class WhenCallingConvertToDbValue_ForAnEnumWithIntStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private Int32EnumStatus value = Int32EnumStatus.New;

            public WhenCallingConvertToDbValue_ForAnEnumWithIntStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(Int32EnumStatus));
            }

            [Fact]
            public void TheResultShouldBeAnInteger()
            {
                Assert.IsType<int>(this.result);
            }

            [Fact]
            public void TheResultValueShouldBeCorrect()
            {
                Assert.Equal((int)3, result);
            }
        }

        public class WhenCallingConvertToDbValue_ForAnEnumWithLongStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private Int64EnumStatus value = Int64EnumStatus.New;

            public WhenCallingConvertToDbValue_ForAnEnumWithLongStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(Int64EnumStatus));
            }

            [Fact]
            public void TheResultShouldBeALong()
            {
                Assert.IsType<long>(this.result);
            }

            [Fact]
            public void TheResultValueShouldBeCorrect()
            {
                Assert.Equal((long)4, result);
            }
        }

        public class WhenCallingConvertToDbValue_ForAnEnumWithShortStorage
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private Int16EnumStatus value = Int16EnumStatus.New;

            public WhenCallingConvertToDbValue_ForAnEnumWithShortStorage()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(Int16EnumStatus));
            }

            [Fact]
            public void TheResultShouldBeAShort()
            {
                Assert.IsType<short>(this.result);
            }

            [Fact]
            public void TheResultValueShouldBeCorrect()
            {
                Assert.Equal((short)2, result);
            }
        }

        public class WhenCallingConvertToDbValue_ForANullableEnum
        {
            private object result;
            private ITypeConverter typeConverter = new EnumTypeConverter();
            private Int32EnumStatus? value = null;

            public WhenCallingConvertToDbValue_ForANullableEnum()
            {
                this.result = this.typeConverter.ConvertToDbValue(value, typeof(Int32EnumStatus?));
            }

            [Fact]
            public void TheResultShouldBeNull()
            {
                Assert.Null(this.result);
            }
        }
    }
}