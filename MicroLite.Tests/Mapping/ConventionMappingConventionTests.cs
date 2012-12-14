namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
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

        public class WhenTheClassIdentifierIsPrefixedWithTheClassName
        {
            private readonly ObjectInfo objectInfo;

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
            public void TheInvoiceIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "InvoiceId"));
            }

            [Fact]
            public void TheInvoiceIdShouldBeIdentifier()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "InvoiceId").IsIdentifier);
            }
        }

        public class WhenUsingDefaultSettings
        {
            private readonly ObjectInfo objectInfo;

            public WhenUsingDefaultSettings()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings());

                this.objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));
            }

            [Fact]
            public void AgeInYearsShouldNotBeMapped()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Any(x => x.ColumnName == "AgeInYears"));
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
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth"));
            }

            [Fact]
            public void TheIdColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowInsert);
            }

            [Fact]
            public void TheIdColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowUpdate);
            }

            [Fact]
            public void TheIdColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Id"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").PropertyInfo);
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierColumn);
            }

            [Fact]
            public void TheIdentifierStrategyShouldBeSet()
            {
                Assert.Equal(IdentifierStrategy.DbGenerated, this.objectInfo.TableInfo.IdentifierStrategy);
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id"));
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
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name"));
            }

            [Fact]
            public void ThereShouldBe4Columns()
            {
                Assert.Equal(4, this.objectInfo.TableInfo.Columns.Count());
            }

            [Fact]
            public void TheSchemaShouldNotBeSet()
            {
                Assert.Null(this.objectInfo.TableInfo.Schema);
            }

            [Fact]
            public void TheStatusColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").AllowInsert);
            }

            [Fact]
            public void TheStatusColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").AllowUpdate);
            }

            [Fact]
            public void TheStatusColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Status"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").PropertyInfo);
            }

            [Fact]
            public void TheStatusColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").IsIdentifier);
            }

            [Fact]
            public void TheStatusPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId"));
            }

            [Fact]
            public void TheTableNameShouldBePluralized()
            {
                Assert.Equal("Customers", this.objectInfo.TableInfo.Name);
            }
        }

        public class WhenUsingIdentifierStrategyOfAssignedAndNonPluralSettings
        {
            private readonly ObjectInfo objectInfo;

            public WhenUsingIdentifierStrategyOfAssignedAndNonPluralSettings()
            {
                var mappingConvention = new ConventionMappingConvention(new ConventionMappingSettings
                {
                    IdentifierStrategy = IdentifierStrategy.Assigned,
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
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "DateOfBirth"));
            }

            [Fact]
            public void TheIdColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowInsert);
            }

            [Fact]
            public void TheIdColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").AllowUpdate);
            }

            [Fact]
            public void TheIdColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Id"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id").PropertyInfo);
            }

            [Fact]
            public void TheIdentifierColumnShouldBeSet()
            {
                Assert.Equal("Id", this.objectInfo.TableInfo.IdentifierColumn);
            }

            [Fact]
            public void TheIdentifierStrategyShouldBeSet()
            {
                Assert.Equal(IdentifierStrategy.Assigned, this.objectInfo.TableInfo.IdentifierStrategy);
            }

            [Fact]
            public void TheIdPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Id"));
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
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "Name"));
            }

            [Fact]
            public void ThereShouldBe4Columns()
            {
                Assert.Equal(4, this.objectInfo.TableInfo.Columns.Count());
            }

            [Fact]
            public void TheSchemaShouldNotBeSet()
            {
                Assert.Null(this.objectInfo.TableInfo.Schema);
            }

            [Fact]
            public void TheStatusColumnShouldAllowInsert()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").AllowInsert);
            }

            [Fact]
            public void TheStatusColumnShouldAllowUpdate()
            {
                Assert.True(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").AllowUpdate);
            }

            [Fact]
            public void TheStatusColumnShouldBeSet()
            {
                Assert.Equal(typeof(Customer).GetProperty("Status"), this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").PropertyInfo);
            }

            [Fact]
            public void TheStatusColumnShouldNotBeIdentifier()
            {
                Assert.False(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId").IsIdentifier);
            }

            [Fact]
            public void TheStatusPropertyShouldBeMapped()
            {
                Assert.NotNull(this.objectInfo.TableInfo.Columns.Single(x => x.ColumnName == "StatusId"));
            }

            [Fact]
            public void TheTableNameShouldNotBePluralized()
            {
                Assert.Equal("Customer", this.objectInfo.TableInfo.Name);
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

            public CustomerStatus Status
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