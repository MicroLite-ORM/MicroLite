namespace MicroLite.Tests.Builder
{
    using System;
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="StoredProcedureSqlBuilder"/> class.
    /// </summary>
    public class StoredProcedureSqlBuilderTests
    {
        [Fact]
        public void Execute()
        {
            var sqlBuilder = new StoredProcedureSqlBuilder(new TestSqlCharacters(), "GetCustomerInvoices");

            var sqlQuery = sqlBuilder
                .WithParameter("@CustomerId", 7633245)
                .WithParameter("@StartDate", DateTime.Today.AddMonths(-3))
                .WithParameter("@EndDate", DateTime.Today)
                .ToSqlQuery();

            Assert.Equal("INVOKE GetCustomerInvoices @CustomerId,@StartDate,@EndDate", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(7633245, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[1].DbType);
            Assert.Equal(DateTime.Today.AddMonths(-3), sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime2, sqlQuery.Arguments[2].DbType);
            Assert.Equal(DateTime.Today, sqlQuery.Arguments[2].Value);
        }

        /// <summary>
        /// Overrides the base properties with non standard values for testing.
        /// </summary>
        private sealed class TestSqlCharacters : SqlCharacters
        {
            public override string StoredProcedureInvocationCommand
            {
                get
                {
                    return "INVOKE";
                }
            }
        }
    }
}