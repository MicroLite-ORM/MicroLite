namespace MicroLite.Tests.Builder
{
    using System;
    using MicroLite.Builder;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="DeleteSqlBuilder"/> class.
    /// </summary>
    public class DeleteSqlBuilderTests
    {
        public DeleteSqlBuilderTests()
        {
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        public void Dispose()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [Fact]
        public void DeleteFromSpecifyingTableName()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("DELETE FROM Table", sqlQuery.CommandText);
        }

        [Fact]
        public void DeleteFromSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("DELETE FROM [Table]", sqlQuery.CommandText);
        }

        [Fact]
        public void DeleteFromSpecifyingType()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("DELETE FROM Sales.Customers", sqlQuery.CommandText);
        }

        [Fact]
        public void DeleteFromSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("DELETE FROM [Sales].[Customers]", sqlQuery.CommandText);
        }

        [Fact]
        public void DeleteFromValues()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From("Table")
                .WhereEquals("Column1", "Foo")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);

            Assert.Equal("DELETE FROM Table WHERE Column1 = ?", sqlQuery.CommandText);
        }

        [Fact]
        public void DeleteFromValuesWithSqlCharacters()
        {
            var sqlBuilder = new DeleteSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .From("Table")
                .WhereEquals("Column1", "Foo")
                .ToSqlQuery();

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);

            Assert.Equal("DELETE FROM [Table] WHERE [Column1] = @p0", sqlQuery.CommandText);
        }

        [MicroLite.Mapping.Table(schema: "Sales", name: "Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.DbGenerated)]
            public int Id
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Name")]
            public string Name
            {
                get;
                set;
            }
        }
    }
}