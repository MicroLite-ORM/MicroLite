namespace MicroLite.Tests
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlQuery"/> class.
    /// </summary>
    public class SqlQueryTests
    {
        [Fact]
        public void ConstructorSetsArgumentsToEmptyListIfNoneSpecified()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            Assert.NotNull(sqlQuery.Arguments);
        }

        [Fact]
        public void ConstructorSetsProperties()
        {
            var commandText = "SELECT * FROM Table WHERE Id = @p0";
            var parameters = new List<object> { 10 };

            var sqlQuery = new SqlQuery(commandText, parameters.ToArray());

            Assert.Equal(commandText, sqlQuery.CommandText);
            Assert.Equal(parameters, sqlQuery.Arguments);
        }

        [Fact]
        public void DefaultTimeoutIs30Seconds()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            Assert.Equal(30, sqlQuery.Timeout);
        }

        [Fact]
        public void EqualsReturnsFalseIfCommandTextMatchesButArgumentCountDiffers()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0 OR Id = @p1", 10, 35);

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
    }
}