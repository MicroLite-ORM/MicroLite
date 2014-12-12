namespace MicroLite.Tests.Builder
{
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
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
        public void UpdateTableSetColumnValueWhereEquals()
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
        public void UpdateTableSetColumnValueWhereEqualsAndWhereEquals()
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
        public void UpdateTableSetColumnValueWhereEqualsAndWhereEqualsWithSqlCharacters()
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
        public void UpdateTableSetColumnValueWhereEqualsOrWhereEquals()
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
        public void UpdateTableSetColumnValueWhereEqualsOrWhereEqualsWithSqlCharacters()
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
        public void UpdateTableSetColumnValueWhereEqualsWithSqlCharacters()
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
    }
}