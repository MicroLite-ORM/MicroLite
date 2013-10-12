namespace MicroLite.Tests.Query
{
    using System;
    using MicroLite;
    using MicroLite.Mapping;
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="UpdateSqlBuilder"/> class.
    /// </summary>
    public class UpdateSqlBuilderTests
    {
        public UpdateSqlBuilderTests()
        {
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        public void Dispose()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [Fact]
        public void UpdateSpecifyingTableName()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("UPDATE Table SET", sqlQuery.CommandText);
        }

        [Fact]
        public void UpdateSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("UPDATE [Table] SET", sqlQuery.CommandText);
        }

        [Fact]
        public void UpdateSpecifyingType()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Table(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("UPDATE Sales.Customers SET", sqlQuery.CommandText);
        }

        [Fact]
        public void UpdateSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .Table(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("UPDATE [Sales].[Customers] SET", sqlQuery.CommandText);
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

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);
            Assert.Equal(100122, sqlQuery.Arguments[2]);

            Assert.Equal("UPDATE Table SET Column1 = ?, Column2 = ? WHERE Id = ?", sqlQuery.CommandText);
        }

        [Fact]
        public void UpdateValuesWithSqlCharacters()
        {
            var sqlBuilder = new UpdateSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .Table("Table")
                .SetColumnValue("Column1", "Foo")
                .SetColumnValue("Column2", 12)
                .Where("Id", 100122)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);
            Assert.Equal(100122, sqlQuery.Arguments[2]);

            Assert.Equal("UPDATE [Table] SET [Column1] = @p0, [Column2] = @p1 WHERE [Id] = @p2", sqlQuery.CommandText);
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