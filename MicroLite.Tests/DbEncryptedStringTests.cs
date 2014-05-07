namespace MicroLite.Tests
{
    using Xunit;

    public class DbEncryptedStringTests
    {
        public class WhenCallingEqualsWithAnotherDbEncryptedStringAsAnObjectContainingTheSameValue
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (object)(DbEncryptedString)value;

                Assert.True(dbEncryptedString.Equals(other));
            }
        }

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
            public void FalseShouldBeReturned()
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
            public void FalseShouldBeReturned()
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

        public class WhenCallingEqualsWithAnotherDbEncryptedStringWhichIsNull
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (DbEncryptedString)null;

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherStringAsAnObjectContainingTheSameValue
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (object)value;

                Assert.True(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherStringContainingADifferentValue
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = "sdifjsdjfosdfj";

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherStringContainingNoValue
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = string.Empty;

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherStringContainingNull
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = (string)null;

                Assert.False(dbEncryptedString.Equals(other));
            }
        }

        public class WhenCallingEqualsWithAnotherStringContainingTheSameValue
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var value = "12334552233";
                var dbEncryptedString = (DbEncryptedString)value;
                var other = value;

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
            public void GetHashCodeShouldMatchTheHashCodeOfStringEmpty()
            {
                Assert.Equal(string.Empty.GetHashCode(), this.encryptedString.GetHashCode());
            }

            [Fact]
            public void TheValueShouldBeEmpty()
            {
                string actual = this.encryptedString;
                Assert.Equal(this.source, actual);
            }

            [Fact]
            public void ToStringShouldReturnTheOriginalValue()
            {
                Assert.Equal(this.source, this.encryptedString.ToString());
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
                Assert.Null(this.encryptedString);
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
            public void GetHashCodeShouldMatchTheHashCodeOfTheSourceString()
            {
                Assert.Equal(this.source.GetHashCode(), this.encryptedString.GetHashCode());
            }

            [Fact]
            public void TheValueShouldMatch()
            {
                string actual = this.encryptedString;
                Assert.Equal(this.source, actual);
            }

            [Fact]
            public void ToStringShouldReturnTheOriginalValue()
            {
                Assert.Equal(this.source, this.encryptedString.ToString());
            }
        }
    }
}