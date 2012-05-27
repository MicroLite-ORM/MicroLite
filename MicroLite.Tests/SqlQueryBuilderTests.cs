namespace MicroLite.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlQueryBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class SqlQueryBuilderTests
    {
        [Test]
        public void SelectFrom()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromAndWhere()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .AndWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)\r\n AND (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromComplexWhere()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0 OR @p0 IS NULL", "Foo")
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0 OR @p0 IS NULL)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByAscending()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n ORDER BY Column1 ASC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByDescending()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n ORDER BY Column1 DESC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrWhere()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .OrWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)\r\n OR (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromSimpleWhere()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)", sqlQuery.CommandText);
        }
    }
}