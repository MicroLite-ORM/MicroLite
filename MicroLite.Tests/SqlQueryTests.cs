namespace MicroLite.Tests
{
    using System.Data;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlQuery"/> class.
    /// </summary>
    public class SqlQueryTests
    {
        [Fact]
        public void ArgumentsArrayReturnsInnerArgumentArray()
        {
            var args = new[] { new SqlArgument() };

            var sqlQuery = new SqlQuery(string.Empty, args);

            Assert.ReferenceEquals(args, sqlQuery.ArgumentsArray);
        }

        [Fact]
        public void EqualsReturnsFalseIfCommandTextDiffers()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id <> @p0", 10);

            Assert.False(sqlQuery1.Equals(sqlQuery2));
        }

        [Fact]
        public void EqualsReturnsFalseIfCommandTextMatchesButArgumentsDiffer()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 35);

            Assert.False(sqlQuery1.Equals(sqlQuery2));
        }

        [Fact]
        public void EqualsReturnsFalseIfCommandTextMatchesButSqlArgumentsDiffer()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", new SqlArgument(10, DbType.Int32));
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", new SqlArgument(35, DbType.Int32));

            Assert.False(sqlQuery1.Equals(sqlQuery2));
        }

        [Fact]
        public void EqualsReturnsFalseIfComparisonObjectNotSqlQuery()
        {
            var sqlQuery = new SqlQuery("SELECT");

            Assert.False(sqlQuery.Equals("Foo"));
        }

        [Fact]
        public void EqualsReturnsFalseIfComparisonObjectNull()
        {
            var sqlQuery = new SqlQuery("SELECT");

            Assert.False(sqlQuery.Equals((SqlQuery)null));
        }

        [Fact]
        public void EqualsReturnsTrueIfCommandTextMatchesAndArgumentsMatch()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);

            Assert.True(sqlQuery1.Equals(sqlQuery2));
        }

        [Fact]
        public void EqualsReturnsTrueIfCommandTextMatchesAndSqlArgumentsMatch()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", new SqlArgument(10, DbType.Int32));
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", new SqlArgument(10, DbType.Int32));

            Assert.True(sqlQuery1.Equals(sqlQuery2));
        }

        [Fact]
        public void GetHashCodeValue()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);

            var expectedHashCode = sqlQuery.CommandText.GetHashCode() ^ sqlQuery.Arguments.GetHashCode();

            Assert.Equal(expectedHashCode, sqlQuery.GetHashCode());
        }

        [Fact]
        public void TimeoutCanBeChanged()
        {
            var timeout = 180;

            var sqlQuery = new SqlQuery(string.Empty);
            sqlQuery.Timeout = timeout;

            Assert.Equal(timeout, sqlQuery.Timeout);
        }

        [Fact]
        public void ToStringReturnsCommandText()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);

            Assert.Same(sqlQuery.CommandText, sqlQuery.ToString());
        }

        public class WhenConstructedWithCommandTextAndArguments
        {
            private readonly object[] arguments;
            private readonly string commandText;
            private readonly SqlQuery sqlQuery;

            public WhenConstructedWithCommandTextAndArguments()
            {
                this.commandText = "SELECT * FROM Table WHERE Id = @p0";
                this.arguments = new object[] { 10 };
                this.sqlQuery = new SqlQuery(this.commandText, this.arguments);
            }

            [Fact]
            public void TheArgumentsDbTypeAndValueShouldBeSet()
            {
                Assert.Equal(DbType.Int32, this.sqlQuery.Arguments[0].DbType);
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldBeSet()
            {
                Assert.NotNull(this.sqlQuery.Arguments);
                Assert.NotEmpty(this.sqlQuery.Arguments);
            }

            [Fact]
            public void TheCommandTextShouldBeSet()
            {
                Assert.Equal(this.commandText, this.sqlQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutDefaultsTo30()
            {
                Assert.Equal(30, this.sqlQuery.Timeout);
            }
        }

        public class WhenConstructedWithCommandTextAndSqlArguments
        {
            private readonly string commandText;
            private readonly SqlArgument[] sqlArguments;
            private readonly SqlQuery sqlQuery;

            public WhenConstructedWithCommandTextAndSqlArguments()
            {
                this.commandText = "SELECT * FROM Table WHERE Id = @p0";
                this.sqlArguments = new[] { new SqlArgument(10, DbType.Int32) };
                this.sqlQuery = new SqlQuery(this.commandText, this.sqlArguments);
            }

            [Fact]
            public void TheArgumentsDbTypeAndValueShouldBeSet()
            {
                Assert.Equal(DbType.Int32, this.sqlQuery.Arguments[0].DbType);
                Assert.Equal(10, this.sqlQuery.Arguments[0].Value);
            }

            [Fact]
            public void TheArgumentsShouldBeSet()
            {
                Assert.NotNull(this.sqlQuery.Arguments);
                Assert.NotEmpty(this.sqlQuery.Arguments);
            }

            [Fact]
            public void TheCommandTextShouldBeSet()
            {
                Assert.Equal(this.commandText, this.sqlQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutDefaultsTo30()
            {
                Assert.Equal(30, this.sqlQuery.Timeout);
            }
        }

        public class WhenConstructedWithCommandTextOnly
        {
            private readonly string commandText;
            private readonly SqlQuery sqlQuery;

            public WhenConstructedWithCommandTextOnly()
            {
                this.commandText = "SELECT * FROM TABLE";
                this.sqlQuery = new SqlQuery(this.commandText);
            }

            [Fact]
            public void TheArgumentsShouldBeEmpty()
            {
                Assert.Empty(this.sqlQuery.Arguments);
            }

            [Fact]
            public void TheCommandTextShouldBeSet()
            {
                Assert.Equal(this.commandText, this.sqlQuery.CommandText);
            }

            [Fact]
            public void TheSqlArgumentsShouldBeEmpty()
            {
                Assert.Empty(this.sqlQuery.Arguments);
            }

            [Fact]
            public void TheTimeoutDefaultsTo30()
            {
                Assert.Equal(30, this.sqlQuery.Timeout);
            }
        }
    }
}