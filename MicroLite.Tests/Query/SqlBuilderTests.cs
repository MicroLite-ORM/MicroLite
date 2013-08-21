namespace MicroLite.Tests.Query
{
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlBuilder"/> class.
    /// </summary>
    public class SqlBuilderTests
    {
        [Fact]
        public void ExecuteReuturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Execute("GetCustomerInvoices");
            var sqlBuilder2 = SqlBuilder.Execute("GetCustomerInvoices");

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }

        [Fact]
        public void SelectReuturnsNewBuilderOnEachCall()
        {
            var sqlBuilder1 = SqlBuilder.Select("*");
            var sqlBuilder2 = SqlBuilder.Select("*");

            Assert.NotSame(sqlBuilder1, sqlBuilder2);
        }
    }
}