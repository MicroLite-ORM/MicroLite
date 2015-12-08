namespace MicroLite.Tests.Mapping
{
    using System.Data;
    using MicroLite.Mapping;
    using MicroLite.Mapping.Inflection;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConventionMappingSettings" /> class.
    /// </summary>
    public class ConventionMappingSettingsTests
    {
        [Fact]
        public void Default_ReturnsNewInstanceEachTime()
        {
            var settings1 = ConventionMappingSettings.Default;
            var settings2 = ConventionMappingSettings.Default;

            Assert.NotSame(settings1, settings2);
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
                var propertyInfo = typeof(Customer).GetProperty("Status");

                Assert.Equal("CustomerStatusId", this.settings.ResolveColumnName(propertyInfo));
            }

            [Fact]
            public void ResolveColumnNameReturnsPropertyNameIfNotEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");

                Assert.Equal("Name", this.settings.ResolveColumnName(propertyInfo));
            }

            [Fact]
            public void ResolveDbTypeReturnsDbType()
            {
                var propertyInfo = typeof(Customer).GetProperty("Id");

                Assert.Equal(DbType.Int32, this.settings.ResolveDbType(propertyInfo));
            }

            [Fact]
            public void ResolveSequenceNameReturnsNull()
            {
                var properties = typeof(Customer).GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    Assert.Null(this.settings.ResolveSequenceName(properties[i]));
                }
            }

            [Fact]
            public void ResolveTableNameReturnsPluralTypeName()
            {
                var type = typeof(Customer);

                Assert.Equal("Customers", this.settings.ResolveTableName(type));
            }

            [Fact]
            public void ResolveTableNameWhenTypeIsGenericReturnsPluralTypeName()
            {
                var type = typeof(Customer<int>);

                Assert.Equal("Customers", this.settings.ResolveTableName(type));
            }

            [Fact]
            public void ResolveTableSchemaReturnsNull()
            {
                var type = typeof(Customer);

                Assert.Null(this.settings.ResolveTableSchema(type));
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
            public void TheResolveDbTypeFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveDbType);
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
            public void TheResolveSequenceNameShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveSequenceName);
            }

            [Fact]
            public void TheResolveTableNameFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveTableName);
            }

            [Fact]
            public void TheResolveTableSchemaFunctionShouldBeSet()
            {
                Assert.NotNull(this.settings.ResolveTableSchema);
            }

            [Fact]
            public void UsePluralClassNameForTableNameIsSetToTrue()
            {
                Assert.True(this.settings.UsePluralClassNameForTableName);
            }
        }
    }

    public class Customer<T>
    {
    }
}