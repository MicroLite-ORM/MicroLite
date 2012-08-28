namespace MicroLite.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlUtil"/> class.
    /// </summary>
    [TestFixture]
    public class SqlUtilTests
    {
        [Test]
        public void Combine()
        {
            var sqlQuery1 = new SqlQuery("SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1", "Foo", 100);
            var sqlQuery2 = new SqlQuery("SELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p0 OR @p0 IS NULL) AND [Column_2] < @p1", "Bar", -1);

            var sqlQuery = SqlUtil.Combine(new[] { sqlQuery1, sqlQuery2 });

            Assert.AreEqual(4, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual(100, sqlQuery.Arguments[1]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[2]);
            Assert.AreEqual(-1, sqlQuery.Arguments[3]);

            Assert.AreEqual(
                "SELECT [Column1], [Column2], [Column3] FROM [dbo].[Table1] WHERE [Column1] = @p0 AND [Column2] > @p1;\r\nSELECT [Column_1], [Column_2] FROM [dbo].[Table_2] WHERE ([Column_1] = @p2 OR @p2 IS NULL) AND [Column_2] < @p3",
                sqlQuery.CommandText);
        }

        [Test]
        public void CombineThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => SqlUtil.Combine(null));

            Assert.AreEqual("sqlQueries", exception.ParamName);
        }
    }
}