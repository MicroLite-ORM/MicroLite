namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConventionMappingConvention" /> class.
    /// </summary>
    public class ConventionMappingConventionTests
    {
        public class CustomerEntity : EntityBase
        {
        }

        public class EntityBase
        {
            public int CustomerEntityId
            {
                get;
                set;
            }
        }

        public class WhenCallingCreateObjectInfoAndTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings());

                var exception = Assert.Throws<ArgumentNullException>(
                    () => mappingConvention.CreateObjectInfo(null));

                Assert.Equal("forType", exception.ParamName);
            }
        }

        public class WhenNotUsingDefaultSettings : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenNotUsingDefaultSettings()
            {
                var settings = UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned);
                settings.Ignore = (PropertyInfo propertyInfo) =>
                {
                    return propertyInfo.Name == "Website";
                };
                settings.UsePluralClassNameForTableName = false;

                var mappingConvention = new ConventionMappingConvention(settings);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void TheCreatedColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").AllowInsert);
            }

            [Fact]
            public void TheCreatedColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Created"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").PropertyInfo);
            }

            [Fact]
            public void TheCreatedColumnShouldNotAllowUpdate()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").AllowUpdate);
            }

            [Fact]
            public void TheCreatedColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").IsIdentifier);
            }

            [Fact]
            public void TheCreatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Created"));
            }

            [Fact]
            public void TheCreatedShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").SequenceName);
            }

            [Fact]
            public void TheCreditLimitColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").AllowInsert);
            }

            [Fact]
            public void TheCreditLimitColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").AllowUpdate);
            }

            [Fact]
            public void TheCreditLimitColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("CreditLimit"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").PropertyInfo);
            }

            [Fact]
            public void TheCreditLimitColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").IsIdentifier);
            }

            [Fact]
            public void TheCreditLimitPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CreditLimit"));
            }

            [Fact]
            public void TheCreditLimitShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").SequenceName);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").AllowInsert);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").AllowUpdate);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("DateOfBirth"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").PropertyInfo);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").IsIdentifier);
            }

            [Fact]
            public void TheDateOfBirthPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "DateOfBirth"));
            }

            [Fact]
            public void TheDateOfBirthShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").SequenceName);
            }

            [Fact]
            public void TheIdColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowInsert);
            }

            [Fact]
            public void TheIdColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Id"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").PropertyInfo);
            }

            [Fact]
            public void TheIdColumnShouldNotAllowUpdate()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowUpdate);
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierColumn.ColumnName);
            }

            [Fact]
            public void TheIdentifierStrategyShouldBeSet()
            {
                Assert.Equal(IdentifierStrategy.Assigned, this.objectInfo.TableInfo.IdentifierStrategy);
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Id"));
            }

            [Fact]
            public void TheIdShouldBeIdentifier()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").IsIdentifier);
            }

            [Fact]
            public void TheIdShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").SequenceName);
            }

            [Fact]
            public void TheNameColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").AllowInsert);
            }

            [Fact]
            public void TheNameColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").AllowUpdate);
            }

            [Fact]
            public void TheNameColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Name"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").PropertyInfo);
            }

            [Fact]
            public void TheNameColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").IsIdentifier);
            }

            [Fact]
            public void TheNamePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Name"));
            }

            [Fact]
            public void TheNameShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").SequenceName);
            }

            [Fact]
            public void ThereShouldBe7Columns()
            {
                Assert.Equal(7, this.objectInfo.TableInfo.Columns.Count());
            }

            [Fact]
            public void TheSchemaShouldBeSet()
            {
                Assert.Equal("Sales", this.objectInfo.TableInfo.Schema);
            }

            [Fact]
            public void TheStatusColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").AllowInsert);
            }

            [Fact]
            public void TheStatusColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").AllowUpdate);
            }

            [Fact]
            public void TheStatusColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Status"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").PropertyInfo);
            }

            [Fact]
            public void TheStatusColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").IsIdentifier);
            }

            [Fact]
            public void TheStatusPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CustomerStatusId"));
            }

            [Fact]
            public void TheStatusShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").SequenceName);
            }

            [Fact]
            public void TheTableNameShouldNotBePluralized()
            {
                Assert.Equal("Customer", this.objectInfo.TableInfo.Name);
            }

            [Fact]
            public void TheUpdatedColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").AllowUpdate);
            }

            [Fact]
            public void TheUpdatedColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Updated"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").PropertyInfo);
            }

            [Fact]
            public void TheUpdatedColumnShouldNotAllowInsert()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").AllowInsert);
            }

            [Fact]
            public void TheUpdatedColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").IsIdentifier);
            }

            [Fact]
            public void TheUpdatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Updated"));
            }

            [Fact]
            public void TheUpdatedShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").SequenceName);
            }

            [Fact]
            public void TheWebsitePropertyShouldNotBeMapped()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Website"));
            }
        }

        public class WhenTheClassIdentifierIsPrefixedWithTheClassName : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenTheClassIdentifierIsPrefixedWithTheClassName()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings
                {
                    ResolveIdentifierStrategy = (Type type) =>
                    {
                        return IdentifierStrategy.Assigned;
                    },
                    UsePluralClassNameForTableName = false
                });

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Invoice));
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("InvoiceId", this.objectInfo.TableInfo.IdentifierColumn.ColumnName);
            }

            [Fact]
            public void TheInvoiceIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "InvoiceId"));
            }

            [Fact]
            public void TheInvoiceIdShouldBeIdentifier()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "InvoiceId").IsIdentifier);
            }
        }

        /// <summary>
        /// Issue #353 - Convention based mapping fails to consider inherited property as identifier.
        /// </summary>
        public class WhenTheIdentifierPropertyIsInherited : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenTheIdentifierPropertyIsInherited()
            {
                var settings = UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated);

                var mappingConvention = new ConventionMappingConvention(settings);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(CustomerEntity));
            }

            [Fact]
            public void ItShouldStillBeAcceptedIfItMatchesConventions()
            {
                Assert.NotNull(objectInfo.TableInfo.IdentifierColumn);
                Assert.Equal("CustomerEntityId", objectInfo.TableInfo.IdentifierColumn.PropertyInfo.Name);
            }
        }

        public class WhenTheIdentifierStrategyIsSequence : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenTheIdentifierStrategyIsSequence()
            {
                var settings = UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence);

                settings.UsePluralClassNameForTableName = false;

                var mappingConvention = new ConventionMappingConvention(settings);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void TheIdentifierColumnShouldHaveTheSequenceNameSet()
            {
                Assert.Equal("Customer_Id_Sequence", this.objectInfo.TableInfo.IdentifierColumn.SequenceName);
            }
        }

        public class WhenUsingDefaultSettings : UnitTest
        {
            private readonly IObjectInfo objectInfo;

            public WhenUsingDefaultSettings()
            {
                var mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void TheCreatedColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").AllowInsert);
            }

            [Fact]
            public void TheCreatedColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").AllowUpdate);
            }

            [Fact]
            public void TheCreatedColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Created"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").PropertyInfo);
            }

            [Fact]
            public void TheCreatedColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").IsIdentifier);
            }

            [Fact]
            public void TheCreatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Created"));
            }

            [Fact]
            public void TheCreatedShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").SequenceName);
            }

            [Fact]
            public void TheCreditLimitColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").AllowInsert);
            }

            [Fact]
            public void TheCreditLimitColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").AllowUpdate);
            }

            [Fact]
            public void TheCreditLimitColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("CreditLimit"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").PropertyInfo);
            }

            [Fact]
            public void TheCreditLimitColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").IsIdentifier);
            }

            [Fact]
            public void TheCreditLimitPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CreditLimit"));
            }

            [Fact]
            public void TheCreditLimitShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CreditLimit").SequenceName);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").AllowInsert);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").AllowUpdate);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("DateOfBirth"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").PropertyInfo);
            }

            [Fact]
            public void TheDateOfBirthColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").IsIdentifier);
            }

            [Fact]
            public void TheDateOfBirthPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "DateOfBirth"));
            }

            [Fact]
            public void TheDateOfBirthShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth").SequenceName);
            }

            [Fact]
            public void TheIdColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Id"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").PropertyInfo);
            }

            [Fact]
            public void TheIdColumnShouldNotAllowInsert()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowInsert);
            }

            [Fact]
            public void TheIdColumnShouldNotAllowUpdate()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowUpdate);
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierColumn.ColumnName);
            }

            [Fact]
            public void TheIdentifierStrategyShouldBeSet()
            {
                Assert.Equal(IdentifierStrategy.DbGenerated, this.objectInfo.TableInfo.IdentifierStrategy);
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Id"));
            }

            [Fact]
            public void TheIdShouldBeIdentifier()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").IsIdentifier);
            }

            [Fact]
            public void TheIdShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").SequenceName);
            }

            [Fact]
            public void TheNameColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").AllowInsert);
            }

            [Fact]
            public void TheNameColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").AllowUpdate);
            }

            [Fact]
            public void TheNameColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Name"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").PropertyInfo);
            }

            [Fact]
            public void TheNameColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").IsIdentifier);
            }

            [Fact]
            public void TheNamePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Name"));
            }

            [Fact]
            public void TheNameShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name").SequenceName);
            }

            [Fact]
            public void ThereShouldBe8Columns()
            {
                Assert.Equal(8, this.objectInfo.TableInfo.Columns.Count());
            }

            [Fact]
            public void TheSchemaShouldNotBeSet()
            {
                Assert.Null(this.objectInfo.TableInfo.Schema);
            }

            [Fact]
            public void TheStatusColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").AllowInsert);
            }

            [Fact]
            public void TheStatusColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").AllowUpdate);
            }

            [Fact]
            public void TheStatusColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Status"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").PropertyInfo);
            }

            [Fact]
            public void TheStatusColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").IsIdentifier);
            }

            [Fact]
            public void TheStatusPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CustomerStatusId"));
            }

            [Fact]
            public void TheStatusShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerStatusId").SequenceName);
            }

            [Fact]
            public void TheTableNameShouldBePluralized()
            {
                Assert.Equal("Customers", this.objectInfo.TableInfo.Name);
            }

            [Fact]
            public void TheUpdatedColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").AllowInsert);
            }

            [Fact]
            public void TheUpdatedColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").AllowUpdate);
            }

            [Fact]
            public void TheUpdatedColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Updated"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").PropertyInfo);
            }

            [Fact]
            public void TheUpdatedColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").IsIdentifier);
            }

            [Fact]
            public void TheUpdatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Updated"));
            }

            [Fact]
            public void TheUpdatedShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").SequenceName);
            }

            [Fact]
            public void TheWebsiteColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Website").AllowInsert);
            }

            [Fact]
            public void TheWebsiteColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Website").AllowUpdate);
            }

            [Fact]
            public void TheWebsiteColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Website"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Website").PropertyInfo);
            }

            [Fact]
            public void TheWebsiteColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Website").IsIdentifier);
            }

            [Fact]
            public void TheWebsitePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Website"));
            }

            [Fact]
            public void TheWebsiteShouldNotHaveASequenceName()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Website").SequenceName);
            }
        }
    }
}