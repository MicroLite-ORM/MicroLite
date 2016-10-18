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
    /// Unit Tests for the <see cref="UpdateSqlBuilder"/> class.
    /// </summary>
    public class UpdateSqlBuilderTests : UnitTest
    {
        public UpdateSqlBuilderTests()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.DbGenerated);
        }

        [Fact]
        public void AndWhereThrowsArgumentExceptionForEmptyColumn()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.AndWhere(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void OrWhereThrowsArgumentExceptionForEmptyColumn()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.OrWhere(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void TableThrowsArgumentExceptionForEmptyTableName()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Table(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("tableName"), exception.Message);
        }

        [Fact]
        public void UpdateSpecifyingTableName()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET ", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .ToSqlQuery();

            Assert.Equal("UPDATE [Table] SET ", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateSpecifyingType()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("UPDATE Sales.Customers SET ", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("UPDATE [Sales].[Customers] SET ", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereBetween()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").Between(1, 199)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id BETWEEN ? AND ?)", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(199, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereInArgs()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(5, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(2, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[4].DbType);
            Assert.Equal(3, sqlQuery.Arguments[4].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").In(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").In(subQuery)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsEqualTo()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsEqualTo(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id = ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsEqualToAndWhereIsEqualTo()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsEqualTo(100122)
                .AndWhere("Column1").IsEqualTo(11)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id = ?) AND (Column1 = ?)", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(11, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsEqualToAndWhereIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsEqualTo(100122)
                .AndWhere("Column1").IsEqualTo(11)
                .ToSqlQuery();

            Assert.Equal("UPDATE [Table] SET [Column1] = @p0,[Column2] = @p1 WHERE ([Id] = @p2) AND ([Column1] = @p3)", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(11, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsEqualToOrWhereIsEqualTo()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsEqualTo(100122)
                .OrWhere("Column1").IsEqualTo(11)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id = ?) OR (Column1 = ?)", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(11, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsEqualToOrWhereIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsEqualTo(100122)
                .OrWhere("Column1").IsEqualTo(11)
                .ToSqlQuery();

            Assert.Equal("UPDATE [Table] SET [Column1] = @p0,[Column2] = @p1 WHERE ([Id] = @p2) OR ([Column1] = @p3)", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(11, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsEqualToWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsEqualTo(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE [Table] SET [Column1] = @p0,[Column2] = @p1 WHERE ([Id] = @p2)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsGreaterThan()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsGreaterThan(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id > ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsGreaterThanOrEqualTo()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsGreaterThanOrEqualTo(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id >= ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsLessThan()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsLessThan(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id < ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsLessThanOrEqualTo()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsLessThanOrEqualTo(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id <= ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsLike()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").IsLike("Foo%")
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 LIKE ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[2].DbType);
            Assert.Equal("Foo%", sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsNotEqualTo()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsNotEqualTo(100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id <> ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(100122, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsNotLike()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").IsNotLike("Foo%")
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 NOT LIKE ?)", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[2].DbType);
            Assert.Equal("Foo%", sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsNotNull()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsNotNull()
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id IS NOT NULL)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereIsNull()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").IsNull()
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id IS NULL)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereNotBetween()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id").NotBetween(1, 199)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Id NOT BETWEEN ? AND ?)", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(199, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereNotInArgs()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 NOT IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(5, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(2, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[4].DbType);
            Assert.Equal(3, sqlQuery.Arguments[4].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereNotInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").NotIn(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 NOT IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void UpdateTableSetColumnValueWhereNotInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Column3").NotIn(subQuery)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?,Column2 = ? WHERE (Column3 NOT IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(12, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void WhereThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Where(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }
    }
}