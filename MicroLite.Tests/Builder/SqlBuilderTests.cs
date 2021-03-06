﻿namespace MicroLite.Tests.Builder
{
    using MicroLite.Builder;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlBuilder"/> class.
    /// </summary>
    public class SqlBuilderTests
    {
        [Fact]
        public void DeleteReturnsDeleteSqlBuilder()
        {
            Assert.IsType<DeleteSqlBuilder>(SqlBuilder.Delete());
        }

        [Fact]
        public void DeleteReturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Delete();
            var sqlBuilder2 = SqlBuilder.Delete();

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }

        [Fact]
        public void ExecuteReturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Execute("GetCustomerInvoices");
            var sqlBuilder2 = SqlBuilder.Execute("GetCustomerInvoices");

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }

        [Fact]
        public void ExecuteReturnsStoredProcedureSqlBuilder()
        {
            Assert.IsType<StoredProcedureSqlBuilder>(SqlBuilder.Execute("GetCustomerInvoices"));
        }

        [Fact]
        public void InsertReturnsInsertSqlBuilder()
        {
            Assert.IsType<InsertSqlBuilder>(SqlBuilder.Insert());
        }

        [Fact]
        public void InsertReturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Insert();
            var sqlBuilder2 = SqlBuilder.Insert();

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }

        [Fact]
        public void SelectReturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Select("*");
            var sqlBuilder2 = SqlBuilder.Select("*");

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }

        [Fact]
        public void SelectReturnsSelectSqlBuilder()
        {
            Assert.IsType<SelectSqlBuilder>(SqlBuilder.Select("*"));
        }

        [Fact]
        public void SelectReturnsSqlBuilderWithColumnSelectListIfColumnSpecified()
        {
            var sqlBuilder = SqlBuilder.Select("Id").From("Customers").ToSqlQuery();

            Assert.Equal("SELECT Id FROM Customers", sqlBuilder.ToString());
        }

        [Fact]
        public void SelectReturnsSqlBuilderWithColumnsSelectListIfColumnsSpecified()
        {
            var sqlBuilder = SqlBuilder.Select("Id", "Name").From("Customers").ToSqlQuery();

            Assert.Equal("SELECT Id,Name FROM Customers", sqlBuilder.ToString());
        }

        [Fact]
        public void SelectReturnsSqlBuilderWithNoSelectListIfNoColumnsSpecified()
        {
            var sqlBuilder = SqlBuilder.Select().From("Customers").ToSqlQuery();

            Assert.Equal("SELECT  FROM Customers", sqlBuilder.ToString());
        }

        [Fact]
        public void SelectReturnsSqlBuilderWithWildcardSelectListIfWildcardSpecified()
        {
            var sqlBuilder = SqlBuilder.Select("*").From("Customers").ToSqlQuery();

            Assert.Equal("SELECT * FROM Customers", sqlBuilder.ToString());
        }

        [Fact]
        public void UpdateReturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Update();
            var sqlBuilder2 = SqlBuilder.Update();

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }

        [Fact]
        public void UpdateReturnsUpdateSqlBuilder()
        {
            Assert.IsType<UpdateSqlBuilder>(SqlBuilder.Update());
        }
    }
}