namespace MicroLite.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlQuery"/> class.
    /// </summary>
    public class SqlQueryTests
    {
        [Test]
        public void ConstructorSetsProperties()
        {
            var commandText = "SELECT * FROM Table WHERE Id = @p0";
            var parameters = new object[] { 10 };

            var sqlQuery = new SqlQuery(commandText, parameters);

            Assert.AreEqual(commandText, sqlQuery.CommandText);
            CollectionAssert.AreEqual(parameters, sqlQuery.Arguments);
        }

        [Test]
        public void EqualsReturnsFalseIfCommandTextMatchesButArgumentCountDiffers()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0 OR Id = @p1", 10, 35);

            Assert.IsFalse(sqlQuery1.Equals(sqlQuery2));
        }

        [Test]
        public void EqualsReturnsFalseIfCommandTextMatchesButArgumentsDiffer()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 35);

            Assert.IsFalse(sqlQuery1.Equals(sqlQuery2));
        }

        [Test]
        public void EqualsReturnsFalseIfComparisonObjectNotSqlQuery()
        {
            var sqlQuery = new SqlQuery("SELECT");

            Assert.IsFalse(sqlQuery.Equals("Foo"));
        }

        [Test]
        public void EqualsReturnsFalseIfComparisonObjectNull()
        {
            var sqlQuery = new SqlQuery("SELECT");

            Assert.IsFalse(sqlQuery.Equals((SqlQuery)null));
        }

        [Test]
        public void EqualsReturnsTrueIfCommandTextMatchesAndArgumentsMatch()
        {
            var sqlQuery1 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);
            var sqlQuery2 = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);

            Assert.IsTrue(sqlQuery1.Equals(sqlQuery2));
        }

        [Test]
        public void GetHashCodeValue()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);

            var expectedHashCode = sqlQuery.CommandText.GetHashCode() ^ sqlQuery.Arguments.GetHashCode();

            Assert.AreEqual(expectedHashCode, sqlQuery.GetHashCode());
        }

        [Test]
        public void ToStringReturnsCommandText()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Table WHERE Id = @p0", 10);

            Assert.AreSame(sqlQuery.CommandText, sqlQuery.ToString());
        }
    }
}