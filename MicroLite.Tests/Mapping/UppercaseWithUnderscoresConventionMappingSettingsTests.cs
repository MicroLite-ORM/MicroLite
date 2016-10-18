namespace MicroLite.Tests.Mapping
{
    using System.Linq;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="WhenUsingUppercaseWithUnderscores" /> class.
    /// </summary>
    public class UppercaseWithUnderscoresConventionMappingSettingsTests
    {
        public class WhenUsingUppercaseWithUnderscores : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenUsingUppercaseWithUnderscores()
            {
                var mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.UppercaseWithUnderscores);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void TheCreatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CREATED"));
            }

            [Fact]
            public void TheCreditLimitPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CREDIT_LIMIT"));
            }

            [Fact]
            public void TheDateOfBirthPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "DATE_OF_BIRTH"));
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "ID"));
            }

            [Fact]
            public void TheNamePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "NAME"));
            }

            [Fact]
            public void TheStatusPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CUSTOMER_STATUS_ID"));
            }

            [Fact]
            public void TheTableNameShouldBeMapped()
            {
                Assert.Equal("CUSTOMERS", this.objectInfo.TableInfo.Name);
            }

            [Fact]
            public void TheTableNameShouldBeUppercasedWithUnderscoresIfMultipleWords()
            {
                var mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.UppercaseWithUnderscores);

                var creditCardObjectInfo = mappingConvention.CreateObjectInfo(typeof(CreditCard));

                Assert.Equal("CREDIT_CARDS", creditCardObjectInfo.TableInfo.Name);
            }

            [Fact]
            public void TheUpdatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "UPDATED"));
            }

            [Fact]
            public void TheWebsitePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "WEBSITE"));
            }
        }
    }
}