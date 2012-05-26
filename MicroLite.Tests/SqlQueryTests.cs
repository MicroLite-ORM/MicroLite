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
            CollectionAssert.AreEqual(parameters, sqlQuery.Parameters);
        }
    }
}