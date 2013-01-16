namespace MicroLite.Tests.Mapping
{
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