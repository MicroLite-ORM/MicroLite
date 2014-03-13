namespace MicroLite.Tests.Builder
{
    using MicroLite;
    using MicroLite.Builder;
    using MicroLite.Dialect.MsSql;
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

            Assert.Equal("UPDATE Table SET", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .ToSqlQuery();

            Assert.Equal("UPDATE [Table] SET", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateSpecifyingType()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("UPDATE Sales.Customers SET", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("UPDATE [Sales].[Customers] SET", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void UpdateValues()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id", 100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE Table SET Column1 = ?, Column2 = ? WHERE Id = ?", sqlQuery.CommandText);
            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);
            Assert.Equal(100122, sqlQuery.Arguments[2]);
        }

        [Fact]
        public void UpdateValuesWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id", 100122)
                .ToSqlQuery();

            Assert.Equal("UPDATE [Table] SET [Column1] = @p0, [Column2] = @p1 WHERE [Id] = @p2", sqlQuery.CommandText);
            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);
            Assert.Equal(100122, sqlQuery.Arguments[2]);
        }
    }
}