namespace MicroLite.Tests
{
    using Xunit;

    public class DbEncryptedStringTests
    {
        public class WhenCallingEqualsWithAnotherDbEncryptedStringContainingADifferentValue
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (DbEncryptedString)"sdifjsdjfosdfj";

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherDbEncryptedStringContainingNoValue
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (DbEncryptedString)string.Empty;

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherDbEncryptedStringContainingNull
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (DbEncryptedString)(string)null;

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherDbEncryptedStringContainingTheSameValue
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (DbEncryptedString)value;

                Assert.True(dbEncryptedString.Equals(other));
            }
        }

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