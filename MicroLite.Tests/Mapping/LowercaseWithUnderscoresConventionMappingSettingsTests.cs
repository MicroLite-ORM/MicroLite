namespace MicroLite.Tests.Mapping
{
    using System.Linq;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="WhenUsingLowercaseWithUnderscores" /> class.
    /// </summary>
    public class LowercaseWithUnderscoresConventionMappingSettingsTests
    {
        public class WhenUsingLowercaseWithUnderscores : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenUsingLowercaseWithUnderscores()
            {
                var mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.LowercaseWithUnderscores);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void TheCreatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "created"));
            }

            [Fact]
            public void TheCreditLimitPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "credit_limit"));
            }

            [Fact]
            public void TheDateOfBirthPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "date_of_birth"));
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "id"));
            }

            [Fact]
            public void TheNamePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "name"));
            }

            [Fact]
            public void TheStatusPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "customer_status_id"));
            }

            [Fact]
            public void TheTableNameShouldBeLowercasedWithUnderscoresIfMultipleWords()
            {
                var mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.LowercaseWithUnderscores);

                var creditCardObjectInfo = mappingConvention.CreateObjectInfo(typeof(CreditCard));

                Assert.Equal("credit_cards", creditCardObjectInfo.TableInfo.Name);
            }

            [Fact]
            public void TheTableNameShouldBeMapped()
            {
                Assert.Equal("customers", this.objectInfo.TableInfo.Name);
            }

            [Fact]
            public void TheUpdatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "updated"));
            }

            [Fact]
            public void TheWebsitePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "website"));
            }
        }
    }
}