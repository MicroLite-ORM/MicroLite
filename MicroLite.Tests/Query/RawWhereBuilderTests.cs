namespace MicroLite.Tests.Query
{
    using System;
    using MicroLite.Query;
    using Moq;
    using Xunit;

    public class RawWhereBuilderTests
    {
        [Fact]
        public void ApplyToEnsuresParametersAreRenumberedAndAllArgumentsAreAppended()
        {
            var rawWhereBuilder = new RawWhereBuilder();
            rawWhereBuilder.Append("ForeName = @p0 AND Surname = @p1", "Fred", "Flintstone");
            rawWhereBuilder.Append(" AND Created > @p0", DateTime.Today);

            var mockSqlBuilder = new Mock<IWhereOrOrderBy>();

            rawWhereBuilder.ApplyTo(mockSqlBuilder.Object);

            mockSqlBuilder.Verify(
                x => x.Where("ForeName = @p0 AND Surname = @p1 AND Created > @p2", "Fred", "Flintstone", DateTime.Today),
                Times.Once());
        }

        [Fact]
        public void ApplyToThrowsArgumentNullExceptionForNullSqlBuilder()
        {
            var rawWhereBuilder = new RawWhereBuilder();

            var exception = Assert.Throws<ArgumentNullException>(() => rawWhereBuilder.ApplyTo(null));

            Assert.Equal("selectFrom", exception.ParamName);
        }

        [Fact]
        public void ToStringReturnsInnerSql()
        {
            var rawWhereBuilder = new RawWhereBuilder();
            rawWhereBuilder.Append("Forename = @p0", "Fred");

            Assert.Equal("Forename = @p0", rawWhereBuilder.ToString());
        }
    }
}