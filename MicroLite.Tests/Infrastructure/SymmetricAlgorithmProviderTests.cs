namespace MicroLite.Tests.Infrastructure
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using MicroLite.Infrastructure;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SymmetricAlgorithmProvider"/> class.
    /// </summary>
    public class SymmetricAlgorithmProviderTests
    {
        public class WhenCallingConfigureAndTheAlgorithmIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var algorithmProvider = new TestSymmetricAlgorithmProvider();

                var exception = Assert.Throws<ArgumentNullException>(() => algorithmProvider.CallBaseConfigure(null, new byte[0]));

                Assert.Equal("algorithmName", exception.ParamName);
            }
        }

        public class WhenCallingConfigureAndTheKeyIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var algorithmProvider = new TestSymmetricAlgorithmProvider();

#if!NET_3_5
                var exception = Assert.Throws<ArgumentNullException>(() => algorithmProvider.CallBaseConfigure("AesManaged", null));
#else
                var exception = Assert.Throws<ArgumentNullException>(() => algorithmProvider.CallBaseConfigure("Rijndael", null));
#endif

                Assert.Equal("algorithmKey", exception.ParamName);
            }
        }

        public class WhenCallingCreateAlgorithm : IDisposable
        {
            private readonly TestSymmetricAlgorithmProvider algorithmProvider = new TestSymmetricAlgorithmProvider();
            private readonly byte[] key = Encoding.ASCII.GetBytes("bru$3atheM-pey+=!a5ebr7d6Tru@E?4");
            private readonly SymmetricAlgorithm symmetricAlgorithm;

            public WhenCallingCreateAlgorithm()
            {
#if!NET_3_5
                this.algorithmProvider.CallBaseConfigure("AesManaged", this.key);
#else
                this.algorithmProvider.CallBaseConfigure("Rijndael", this.key);
#endif

                this.symmetricAlgorithm = this.algorithmProvider.CreateAlgorithm();
            }

            public void Dispose()
            {
#if!NET_3_5
                this.symmetricAlgorithm.Dispose();
#else
                ((IDisposable)this.symmetricAlgorithm).Dispose();
#endif
            }

            [Fact]
            public void TheAlgorithmShouldBeTheSpecifiedType()
            {
#if!NET_3_5
                Assert.IsType<AesManaged>(this.symmetricAlgorithm);
#else
                Assert.IsType<RijndaelManaged>(this.symmetricAlgorithm);
#endif
            }

            [Fact]
            public void TheKeyShouldBeSet()
            {
                Assert.Equal(this.key, this.symmetricAlgorithm.Key);
            }
        }

        private class TestSymmetricAlgorithmProvider : SymmetricAlgorithmProvider
        {
            public void CallBaseConfigure(string algorithmName, byte[] algorithmKey)
            {
                this.Configure(algorithmName, algorithmKey);
            }
        }
    }
}