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
        private enum CustomerStatus
        {
            New = 0,
            Old = 1
        }

        public class WhenConstructed
        {
            private readonly ConventionMappingSettings settings = new ConventionMappingSettings();

            [Fact]
            public void IsIdentifierReturnsTrueIfPropertyNameIsClassId()
            {
                var propertyInfo = typeof(Invoice).GetProperty("InvoiceId");

                Assert.True(this.settings.IsIdentifier(propertyInfo));
            }

            [Fact]
            public void IsIdentifierReturnsTrueIfPropertyNameIsId()
            {
                var propertyInfo = typeof(Customer).GetProperty("Id");

                Assert.True(this.settings.IsIdentifier(propertyInfo));
            }

            [Fact]
            public void ResolveColumnNameReturnsPropertyNameIdIfEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("CustomerStatus");

                Assert.Equal("CustomerStatusId", this.settings.ResolveColumnName(propertyInfo));
            }

            [Fact]
            public void ResolveColumnNameReturnsPropertyNameIfNotEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");

                Assert.Equal("Name", this.settings.ResolveColumnName(propertyInfo));
            }

            [Fact]
            public void ResolveTableNameReturnsPluralTypeName()
            {
                var type = typeof(Customer);

                Assert.Equal("Customers", this.settings.ResolveTableName(type));
            }

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
            public void TheResolveIdentifierStrategyFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveIdentifierStrategy);
            }

            [Fact]
            public void TheResolveTableNameFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveTableName);
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

        private class Customer
        {
            public CustomerStatus CustomerStatus
            {
                get;
                set;
            }

            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        private class Invoice
        {
            public int InvoiceId
            {
                get;
                set;
            }
        }
    }
}