namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
    using System.Reflection;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConventionMappingConvention"/> class.
    /// </summary>
    public class ConventionMappingConventionTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
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

        public class WhenNotUsingDefaultSettings
        {
            private readonly IObjectInfo objectInfo;

            public WhenNotUsingDefaultSettings()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings
                {
                    AllowInsert = (PropertyInfo propertyInfo) =>
                    {
                        return propertyInfo.Name != "Updated";
                    },
                    AllowUpdate = (PropertyInfo propertyInfo) =>
                    {
                        return propertyInfo.Name != "Created";
                    },
                    IdentifierStrategy = IdentifierStrategy.Assigned,
                    Ignore = (PropertyInfo propertyInfo) =>
                    {
                        return propertyInfo.Name == "NonPersistedValue";
                    },
                    TableSchema = "Sales",
                    UsePluralClassNameForTableName = false
                });

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void AgeInYearsShouldNotBeMapped()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Any(x => x.ColumnName == "AgeInYears"));
            }

            [Fact]
            public void TheCreatedColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").AllowInsert);
            }

            [Fact]
            public void TheCreatedColumnShouldNotAllowUpdate()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Created").AllowUpdate);
            }

            [Fact]
            public void TheCreatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Created"));
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
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierColumn);
            }

            [Fact]
            public void TheIdentifierPropertyShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierProperty);
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
            public void TheNonPersistedValuePropertyShouldNotBeMapped()
            {
                Assert.Null(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "NonPersistedValue"));
            }

            [Fact]
            public void ThereShouldBe6Columns()
            {
                Assert.Equal(6, this.objectInfo.TableInfo.Columns.Count());
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
            public void TheUpdatedColumnShouldNotAllowInsert()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Updated").AllowInsert);
            }

            [Fact]
            public void TheUpdatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Updated"));
            }
        }

        public class WhenTheClassIdentifierIsPrefixedWithTheClassName
        {
            private readonly IObjectInfo objectInfo;

            public WhenTheClassIdentifierIsPrefixedWithTheClassName()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings
                {
                    IdentifierStrategy = IdentifierStrategy.Assigned,
                    UsePluralClassNameForTableName = false
                });

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Invoice));
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("InvoiceId", this.objectInfo.TableInfo.IdentifierColumn);
            }

            [Fact]
            public void TheIdentifierPropertyShouldBeSet()
            {
                Assert.Equal("InvoiceId", this.objectInfo.TableInfo.IdentifierProperty);
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

        public class WhenTheResolveIdentifierColumnNameFunctionIsOverridden
        {
            private readonly IObjectInfo objectInfo;

            public WhenTheResolveIdentifierColumnNameFunctionIsOverridden()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings
                {
                    IdentifierStrategy = IdentifierStrategy.Assigned,
                    ResolveIdentifierColumnName = (PropertyInfo propertyInfo) =>
                    {
                        return propertyInfo.DeclaringType.Name + "Id";
                    },
                    UsePluralClassNameForTableName = false
                });

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("CustomerId", this.objectInfo.TableInfo.IdentifierColumn);
            }

            [Fact]
            public void TheIdentifierPropertyShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierProperty);
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "CustomerId"));
            }

            [Fact]
            public void TheIdShouldBeIdentifier()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "CustomerId").IsIdentifier);
            }
        }

        public class WhenUsingDefaultSettings
        {
            private readonly IObjectInfo objectInfo;

            public WhenUsingDefaultSettings()
            {
                var mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void AgeInYearsShouldNotBeMapped()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Any(x => x.ColumnName == "AgeInYears"));
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
            public void TheCreatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Created"));
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
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierColumn);
            }

            [Fact]
            public void TheIdentifierPropertyShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierProperty);
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
            public void TheNonPersistedValuePropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "NonPersistedValue"));
            }

            [Fact]
            public void ThereShouldBe7Columns()
            {
                Assert.Equal(7, this.objectInfo.TableInfo.Columns.Count());
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
            public void TheUpdatedPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.SingleOrDefault(x => x.ColumnName == "Updated"));
            }
        }

        private class Customer
        {
            public Customer()
            {
            }

            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            public DateTime Created
            {
                get;
                set;
            }

            public DateTime DateOfBirth
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

            public string NonPersistedValue
            {
                get;
                set;
            }

            public CustomerStatus Status
            {
                get;
                set;
            }

            public DateTime Updated
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