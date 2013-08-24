namespace MicroLite.Tests.Query
{
    using System;
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="StoredProcedureSqlBuilder"/> class.
    /// </summary>
    public class StoredProcedureSqlBuilderTests
    {
        [Fact]
        public void Execute()
        {
            var sqlBuilder = new StoredProcedureSqlBuilder("GetCustomerInvoices");

            var sqlQuery = sqlBuilder
                .WithParameter("@CustomerId", 7633245)
                .WithParameter("@StartDate", DateTime.Today.AddMonths(-3))
                .WithParameter("@EndDate", DateTime.Today)
                .ToSqlQuery();

            Assert.Equal(3, sqlQuery.Arguments.Count);
            Assert.Equal(7633245, sqlQuery.Arguments[0]);
            Assert.Equal(DateTime.Today.AddMonths(-3), sqlQuery.Arguments[1]);
            Assert.Equal(DateTime.Today, sqlQuery.Arguments[2]);

            Assert.Equal("EXEC GetCustomerInvoices @CustomerId, @StartDate, @EndDate", sqlQuery.CommandText);
        }
    }
}