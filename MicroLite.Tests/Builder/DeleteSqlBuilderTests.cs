namespace MicroLite.Tests.Builder
{
    using System;
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="DeleteSqlBuilder"/> class.
    /// </summary>
    public class DeleteSqlBuilderTests : UnitTest
    {
        public DeleteSqlBuilderTests()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.DbGenerated);
        }

        [Fact]
        public void AndWhereThrowsArgumentExceptionForEmptyColumn()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.AndWhere(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void DeleteFromInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").In(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromNotInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 NOT IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromSpecifyingTableName()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void DeleteFromSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM [Table]", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void DeleteFromSpecifyingType()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Sales.Customers", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void DeleteFromSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("DELETE FROM [Sales].[Customers]", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void DeleteFromWhereBetween()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").Between(1, 199)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 BETWEEN ? AND ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(199, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereInArgs()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(3, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void DeleteFromWhereInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").In(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").In(subQuery)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsEqualTo()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsEqualToAndWhereIsEqualTo()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .AndWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 = ?) AND (Column2 = ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereIsEqualToAndWhereIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .AndWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM [Table] WHERE ([Column1] = @p0) AND ([Column2] = @p1)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereIsEqualToOrWhereIsEqualTo()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .OrWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 = ?) OR (Column2 = ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereIsEqualToOrWhereIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .OrWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM [Table] WHERE ([Column1] = @p0) OR ([Column2] = @p1)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM [Table] WHERE ([Column1] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsGreaterThan()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsGreaterThan(10)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 > ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(10, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsGreaterThanOrEqualTo()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsGreaterThanOrEqualTo(10)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 >= ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(10, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsLessThan()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsLessThan(10)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 < ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(10, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsLessThanOrEqualTo()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsLessThanOrEqualTo(10)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 <= ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(10, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsLike()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsLike("Foo%")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 LIKE ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo%", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsNotEqualTo()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsNotEqualTo("Foo")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 <> ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsNotLike()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsNotLike("Foo%")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 NOT LIKE ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo%", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereIsNotNull()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsNotNull()
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 IS NOT NULL)", sqlQuery.CommandText);

            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void DeleteFromWhereIsNull()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsNull()
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 IS NULL)", sqlQuery.CommandText);

            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void DeleteFromWhereNotBetween()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotBetween(1, 199)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 NOT BETWEEN ? AND ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(199, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereNotInArgs()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 NOT IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(3, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void DeleteFromWhereNotInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 NOT IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void DeleteFromWhereNotInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(subQuery)
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE (Column1 NOT IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void FromThrowsArgumentExceptionForEmptyTableName()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.From(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("table"), exception.Message);
        }

        [Fact]
        public void OrWhereThrowsArgumentExceptionForEmptyColumn()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.OrWhere(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void WhereThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Where(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }
    }
}