namespace MicroLite.Tests.Builder
{
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
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
        public void DeleteFromWhereEquals()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .WhereEquals("Column1", "Foo")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM Table WHERE Column1 = ?", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void DeleteFromWhereEqualsWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From("Table")
                .WhereEquals("Column1", "Foo")
                .ToSqlQuery();

            Assert.Equal("DELETE FROM [Table] WHERE [Column1] = @p0", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);
        }
    }
}