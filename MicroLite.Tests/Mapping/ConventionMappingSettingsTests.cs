namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConventionMappingSettings"/> class.
    /// </summary>
    public class ConventionMappingSettingsTests
    {
        public class WhenConstructed
        {
            private readonly ConventionMappingSettings settings = new ConventionMappingSettings();

            [Fact]
            public void TheIdentifierStrategyIsSetToDbGenerated()
            {
                Assert.Equal(IdentifierStrategy.DbGenerated, this.settings.IdentifierStrategy);
            }

            [Fact]
            public void TheTableSchemIsSetToNull()
            {
                Assert.Null(this.settings.TableSchema);
            }

            [Fact]
            public void UsePluralClassNameForTableNameIsSetToTrue()
            {
                Assert.True(this.settings.UsePluralClassNameForTableName);
            }
        }
    }
}