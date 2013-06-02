namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using MicroLite.Mapping.Inflection;
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
            public void TheAllowInsertFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.AllowInsert);
            }

            [Fact]
            public void TheAllowUpdateFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.AllowUpdate);
            }

            [Fact]
            public void TheIdentifierStrategyIsSetToDbGenerated()
            {
                Assert.Equal(IdentifierStrategy.DbGenerated, this.settings.IdentifierStrategy);
            }

            [Fact]
            public void TheIgnoreFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.Ignore);
            }

            [Fact]
            public void TheInflectionServiceShoulBeDefaultToTheEnglishInflectionService()
            {
                Assert.IsType<EnglishInflectionService>(this.settings.InflectionService);
            }

            [Fact]
            public void TheIsIdentifierFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.IsIdentifier);
            }

            [Fact]
            public void TheResolveColumnNameFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveColumnName);
            }

            [Fact]
            public void TheResolveIdentifierColumnNameFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveIdentifierColumnName);
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