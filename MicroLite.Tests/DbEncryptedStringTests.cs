namespace MicroLite.Tests
{
    using Xunit;

    public class DbEncryptedStringTests
    {
        public class WhenCastFromAnEmptyString
        {
            private DbEncryptedString encryptedString;
            private string source = string.Empty;

            public WhenCastFromAnEmptyString()
            {
                this.encryptedString = this.source;
            }

            [Fact]
            public void TheValueShouldBeEmpty()
            {
                string actual = this.encryptedString;
                Assert.Equal(this.source, actual);
            }
        }

        public class WhenCastFromANullString
        {
            private DbEncryptedString encryptedString;
            private string source = null;

            public WhenCastFromANullString()
            {
                this.encryptedString = this.source;
            }

            [Fact]
            public void TheValueShouldBeNull()
            {
                string actual = this.encryptedString;
                Assert.Null(actual);
            }
        }

        public class WhenCastFromAString
        {
            private DbEncryptedString encryptedString;
            private string source = "foo";

            public WhenCastFromAString()
            {
                this.encryptedString = this.source;
            }

            [Fact]
            public void TheValueShouldMatch()
            {
                string actual = this.encryptedString;
                Assert.Equal(this.source, actual);
            }
        }
    }
}