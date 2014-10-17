namespace MicroLite.Tests
{
    using System.Data;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlArgument"/> struct.
    /// </summary>
    public class SqlArgumentTests
    {
        public class WhenCallingEqualsAndTheDbTypeDiffers
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new SqlArgument(10, DbType.Int64);

                Assert.False(sqlArgument1 == sqlArgument2);
            }
        }

        public class WhenCallingEqualsAndTheOtherObjectIsABoxedInstanceOfPagingOptions
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = (object)new SqlArgument(10, DbType.Int32);

                Assert.True(sqlArgument1.Equals(sqlArgument2));
            }
        }

        public class WhenCallingEqualsAndTheOtherObjectIsNotAnInstanceOfPagingOptions
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new object();

                Assert.False(sqlArgument1.Equals(sqlArgument2));
            }
        }

        public class WhenCallingEqualsAndTheValueAndDbTypeMatch
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new SqlArgument(10, DbType.Int32);

                Assert.True(sqlArgument1 == sqlArgument2);
            }
        }

        public class WhenCallingEqualsAndTheValueDiffers
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new SqlArgument(20, DbType.Int32);

                Assert.False(sqlArgument1 == sqlArgument2);
            }
        }

        public class WhenCallingGetHashCode
        {
            private readonly DbType dbType = DbType.Int32;
            private readonly SqlArgument sqlArgument;
            private readonly object value = 50;

            public WhenCallingGetHashCode()
            {
                this.sqlArgument = new SqlArgument(this.value, this.dbType);
            }

            [Fact]
            public void TheHashCodeOfTheDbTypeShiftedByTheValueShouldBeReturned()
            {
                Assert.Equal(this.dbType.GetHashCode() ^ this.value.GetHashCode(), this.sqlArgument.GetHashCode());
            }
        }

        public class WhenCallingGetHashCodeAndTheValueIsNull
        {
            private readonly DbType dbType = DbType.Int32;
            private readonly SqlArgument sqlArgument;
            private readonly object value = null;

            public WhenCallingGetHashCodeAndTheValueIsNull()
            {
                this.sqlArgument = new SqlArgument(this.value, this.dbType);
            }

            [Fact]
            public void TheHashCodeOfTheDbTypeShiftedByAnEmptyStringShouldBeReturned()
            {
                Assert.Equal(this.dbType.GetHashCode() ^ string.Empty.GetHashCode(), this.sqlArgument.GetHashCode());
            }
        }

        public class WhenCallingNotEqualsAndTheDbTypeDiffers
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new SqlArgument(10, DbType.Int64);

                Assert.True(sqlArgument1 != sqlArgument2);
            }
        }

        public class WhenCallingNotEqualsAndTheValueAndDbTypeMatch
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new SqlArgument(10, DbType.Int32);

                Assert.False(sqlArgument1 != sqlArgument2);
            }
        }

        public class WhenCallingNotEqualsAndTheValueDiffers
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var sqlArgument1 = new SqlArgument(10, DbType.Int32);
                var sqlArgument2 = new SqlArgument(20, DbType.Int32);

                Assert.True(sqlArgument1 != sqlArgument2);
            }
        }

        public class WhenConstructedByDefault
        {
            private readonly SqlArgument sqlArgument;

            [Fact]
            public void TheDbTypeShouldBeTheDefault()
            {
                Assert.Equal(default(DbType), this.sqlArgument.DbType);
            }

            [Fact]
            public void TheValueShouldBeNull()
            {
                Assert.Null(this.sqlArgument.Value);
            }
        }

        public class WhenConstructedWithAValueAndDbType
        {
            private readonly SqlArgument sqlArgument = new SqlArgument(10, DbType.Int32);

            [Fact]
            public void TheDbTypeShouldBeSet()
            {
                Assert.Equal(DbType.Int32, this.sqlArgument.DbType);
            }

            [Fact]
            public void TheValueShouldBeSet()
            {
                Assert.Equal(10, this.sqlArgument.Value);
            }
        }

        public class WhenConstructedWithNullAndDbType
        {
            private readonly SqlArgument sqlArgument = new SqlArgument(null, DbType.Int32);

            [Fact]
            public void TheDbTypeShouldBeSet()
            {
                Assert.Equal(DbType.Int32, this.sqlArgument.DbType);
            }

            [Fact]
            public void TheValueShouldBeNull()
            {
                Assert.Null(this.sqlArgument.Value);
            }
        }
    }
}