namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PropertyAccessor"/> class.
    /// </summary>
    public class PropertyAccessorTests
    {
        private enum CustomerStatus
        {
            Active = 0,
            Disabled = 1,
            Suspended = 2
        }

        public class WhenCallingGetValue
        {
            private readonly Customer customer = new Customer
            {
                Name = "Fred Bloggs"
            };

            private readonly object value;

            public WhenCallingGetValue()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal(this.customer.Name, this.value);
            }
        }

        public class WhenCallingGetValueAndThePropertyIsAnEnum
        {
            private readonly Customer customer = new Customer
            {
                Status = CustomerStatus.Suspended
            };

            private readonly object value;

            public WhenCallingGetValueAndThePropertyIsAnEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("Status");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeAnInteger()
            {
                Assert.IsType<int>(this.value);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal((int)this.customer.Status, this.value);
            }
        }

        public class WhenCallingGetValueForANullableValueTypeWithANullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = null
            };

            private readonly object value;

            public WhenCallingGetValueForANullableValueTypeWithANullValue()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Null(this.customer.LastInvoice);
            }
        }

        public class WhenCallingSetValue
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValue()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                propertyAccessor.SetValue(this.customer, "Fred Blogs");
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Equal("Fred Blogs", this.customer.Name);
            }
        }

        public class WhenCallingSetValueAndThePropertyIsAnEnum
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueAndThePropertyIsAnEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("Status");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                propertyAccessor.SetValue(this.customer, 1);
            }

            [Fact]
            public void ThePropertyShouldBeSetToTheEnumWithTheIntegerValue()
            {
                Assert.Equal(CustomerStatus.Disabled, this.customer.Status);
            }
        }

        /// <summary>
        /// Issue #7 - An InvalidCastException is thrown when converting int to nullable int.
        /// </summary>
        public class WhenCallingSetValueAndThePropertyIsANullableInt
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueAndThePropertyIsANullableInt()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                propertyAccessor.SetValue(this.customer, (int?)1235);
            }

            [Fact]
            public void ThePropertyValueShouldBeSet()
            {
                Assert.Equal(1235, this.customer.LastInvoice);
            }
        }

        /// <summary>
        /// SQLite stores all integers as a long (64bit integer), enums are 32bit integers so the value should be down cast.
        /// </summary>
        public class WhenCallingSetValueForAnEnumPropertyAndTheValueIsALong
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueForAnEnumPropertyAndTheValueIsALong()
            {
                var propertyInfo = typeof(Customer).GetProperty("Status");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                propertyAccessor.SetValue(this.customer, (long)2);
            }

            [Fact]
            public void ThePropertyValueShouldBeSet()
            {
                Assert.Equal(CustomerStatus.Suspended, this.customer.Status);
            }
        }

        /// <summary>
        /// Issue #19 - Null strings in a column result in empty strings in the property
        /// </summary>
        public class WhenCallingSetValueForAPropertyWhichIsAReferenceTypeAndTheValueIsDbNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueForAPropertyWhichIsAReferenceTypeAndTheValueIsDbNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                propertyAccessor.SetValue(this.customer, DBNull.Value);
            }

            [Fact]
            public void ThePropertyShouldNotBeSet()
            {
                Assert.False(this.customer.NameSet);
            }
        }

        /// <summary>
        /// Issue #8 - An InvalidCastException is thrown when converting DBNull to nullable ValueType.
        /// </summary>
        public class WhenCallingSetValueForAPropertyWhichIsAValueTypeAndTheValueIsDbNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueForAPropertyWhichIsAValueTypeAndTheValueIsDbNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = new PropertyAccessor(propertyInfo);

                propertyAccessor.SetValue(this.customer, DBNull.Value);
            }

            [Fact]
            public void ThePropertyShouldNotBeSet()
            {
                Assert.Null(this.customer.LastInvoice);
            }
        }

        private class Customer
        {
            private string name;
            private bool nameSet;

            public int Id
            {
                get;
                set;
            }

            public int? LastInvoice
            {
                get;
                set;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.nameSet = true;
                }
            }

            public bool NameSet
            {
                get
                {
                    return this.nameSet;
                }
            }

            public CustomerStatus Status
            {
                get;
                set;
            }
        }
    }
}