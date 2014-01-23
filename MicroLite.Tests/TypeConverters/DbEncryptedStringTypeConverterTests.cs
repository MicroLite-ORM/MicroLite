namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using MicroLite.Infrastructure;
    using MicroLite.TypeConverters;
    using Moq;
    using Xunit;

    public class DbEncryptedStringTypeConverterTests
    {
        public class WhenCallingCanConvert_WithDbEncryptedString
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var typeConverter = new DbEncryptedStringTypeConverter(new Mock<ISymmetricAlgorithmProvider>().Object);
                Assert.True(typeConverter.CanConvert(typeof(DbEncryptedString)));
            }
        }

        public class WhenCallingConvertFromDbValue_AndTheValueDoesntHaveAnAtSign
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var typeConverter = new DbEncryptedStringTypeConverter(new Mock<ISymmetricAlgorithmProvider>().Object);
                var exception = Assert.Throws<MicroLiteException>(() => typeConverter.ConvertFromDbValue("foo", typeof(DbEncryptedString)));
                Assert.Equal(Messages.DbEncryptedStringTypeConverter_CipherTextInvalid, exception.Message);
            }
        }

        public class WhenCallingConvertFromDbValue_WithAnEmptyValue
        {
            private DbEncryptedString result;

            public WhenCallingConvertFromDbValue_WithAnEmptyValue()
            {
                var typeConverter = new DbEncryptedStringTypeConverter(new Mock<ISymmetricAlgorithmProvider>().Object);
                this.result = (DbEncryptedString)typeConverter.ConvertFromDbValue(string.Empty, typeof(DbEncryptedString));
            }

            [Fact]
            public void TheDbEncryptedStringShouldContainAnEmptyString()
            {
                Assert.Equal(string.Empty, this.result.ToString());
            }
        }

        public class WhenCallingConvertFromDbValue_WithDbNull
        {
            private DbEncryptedString result;

            public WhenCallingConvertFromDbValue_WithDbNull()
            {
                var typeConverter = new DbEncryptedStringTypeConverter(new Mock<ISymmetricAlgorithmProvider>().Object);
                this.result = (DbEncryptedString)typeConverter.ConvertFromDbValue(DBNull.Value, typeof(DbEncryptedString));
            }

            [Fact]
            public void TheDbEncryptedStringShouldBeNull()
            {
                Assert.Equal(null, this.result);
            }
        }

        public class WhenCallingConvertToDbValue
        {
            private readonly string encrypted;
            private readonly DbEncryptedString source = "7622 8765 9902 0924";
            private DbEncryptedStringTypeConverter typeConverter;

            public WhenCallingConvertToDbValue()
            {
                var mockAlgorithmProvider = new Mock<ISymmetricAlgorithmProvider>();
                mockAlgorithmProvider.Setup(x => x.CreateAlgorithm()).Returns(() =>
                {
                    var algorithm = SymmetricAlgorithm.Create("AesManaged");
                    algorithm.Key = Encoding.ASCII.GetBytes("bru$3atheM-pey+=!a5ebr7d6Tru@E?4");

                    return algorithm;
                });

                this.typeConverter = new DbEncryptedStringTypeConverter(mockAlgorithmProvider.Object);
                this.encrypted = (string)this.typeConverter.ConvertToDbValue(this.source, typeof(DbEncryptedString));
            }

            [Fact]
            public void TheResultShouldBeDecryptedBackToTheOriginalValueByConveryFromDbValue()
            {
                var actual = this.typeConverter.ConvertFromDbValue(this.encrypted, typeof(DbEncryptedString));

                Assert.Equal(source.ToString(), actual.ToString());
            }

            [Fact]
            public void TheResultShouldContainTheIVAfterAnAtSign()
            {
                Assert.Contains("@", this.encrypted);
            }

            [Fact]
            public void TheResultShouldNotMatchTheInput()
            {
                Assert.NotEqual(source.ToString(), encrypted);
            }
        }

        public class WhenCallingConvertToDbValue_WithAnEmptyValue
        {
            private string result;

            public WhenCallingConvertToDbValue_WithAnEmptyValue()
            {
                var typeConverter = new DbEncryptedStringTypeConverter(new Mock<ISymmetricAlgorithmProvider>().Object);
                this.result = (string)typeConverter.ConvertToDbValue(string.Empty, typeof(DbEncryptedString));
            }

            [Fact]
            public void TheStringShouldContainNull()
            {
                Assert.Equal(string.Empty, this.result);
            }
        }

        public class WhenCallingConvertToDbValue_WithNull
        {
            private string result;

            public WhenCallingConvertToDbValue_WithNull()
            {
                var typeConverter = new DbEncryptedStringTypeConverter(new Mock<ISymmetricAlgorithmProvider>().Object);
                this.result = (string)typeConverter.ConvertToDbValue(null, typeof(DbEncryptedString));
            }

            [Fact]
            public void TheStringShouldContainNull()
            {
                Assert.Equal((string)null, this.result);
            }
        }

        public class WhenConstructed_WithANullISymmetricAlgorithmProvider
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new DbEncryptedStringTypeConverter(null));
                Assert.Equal("algorithmProvider", exception.ParamName);
            }
        }
    }
}