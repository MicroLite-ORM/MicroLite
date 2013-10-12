namespace MicroLite.Tests.Query
{
    using System;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="InsertSqlBuilder"/> class.
    /// </summary>
    public class InsertSqlBuilderTests : IDisposable
    {
        public InsertSqlBuilderTests()
        {
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        public void Dispose()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [Fact]
        public void InsertInfoSpecifyingTableName()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO Table () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertInfoSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO [Table] () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertInfoSpecifyingType()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Into(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO Sales.Customers () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertInfoSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .Into(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO [Sales].[Customers] () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertInfoValues()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .Value("Column1", "Foo")
                .Value("Column2", 12)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);

            Assert.Equal("INSERT INTO Table (Column1, Column2) VALUES (?, ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertInfoValuesWithSqlCharacters()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.MsSql);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .Value("Column1", "Foo")
                .Value("Column2", 12)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);

            Assert.Equal("INSERT INTO [Table] ([Column1], [Column2]) VALUES (@p0, @p1)", sqlQuery.CommandText);
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