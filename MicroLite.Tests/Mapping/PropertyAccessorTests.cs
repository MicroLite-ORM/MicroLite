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

        public class WhenCallingGetValueForAnEnum
        {
            private readonly Customer customer = new Customer
            {
                Status = CustomerStatus.Suspended
            };

            private readonly object value;

            public WhenCallingGetValueForAnEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("Status");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeAnEnum()
            {
                Assert.IsType<CustomerStatus>(this.value);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal(this.customer.Status, this.value);
            }
        }

        public class WhenCallingGetValueForANullableValueTypeWithANonNullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            private readonly object value;

            public WhenCallingGetValueForANullableValueTypeWithANonNullValue()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal(this.customer.LastInvoice, this.value);
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
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Null(this.value);
            }
        }

        public class WhenCallingGetValueForAReferenceType
        {
            private readonly Customer customer = new Customer
            {
                Name = "Fred Bloggs"
            };

            private readonly object value;

            public WhenCallingGetValueForAReferenceType()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal(this.customer.Name, this.value);
            }
        }

        public class WhenCallingGetValueGenericForANullableValueTypeWithANonNullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            private readonly int? value;

            public WhenCallingGetValueGenericForANullableValueTypeWithANonNullValue()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = (IPropertyAccessor<Customer, int?>)PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal(this.customer.LastInvoice, this.value);
            }
        }

        public class WhenCallingGetValueGenericForANullableValueTypeWithANullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = null
            };

            private readonly int? value;

            public WhenCallingGetValueGenericForANullableValueTypeWithANullValue()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = (IPropertyAccessor<Customer, int?>)PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Null(this.value);
            }
        }

        public class WhenCallingGetValueGenericForAReferenceType
        {
            private readonly Customer customer = new Customer
            {
                Name = "Fred Bloggs"
            };

            private readonly string value;

            public WhenCallingGetValueGenericForAReferenceType()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = (IPropertyAccessor<Customer, string>)PropertyAccessor.Create(propertyInfo);

                this.value = propertyAccessor.GetValue(this.customer);
            }

            [Fact]
            public void TheValueShouldBeReturned()
            {
                Assert.Equal(this.customer.Name, this.value);
            }
        }

        public class WhenCallingSetValueForAnEnum
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueForAnEnum()
            {
                var propertyInfo = typeof(Customer).GetProperty("Status");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, CustomerStatus.Suspended);
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Equal(CustomerStatus.Suspended, this.customer.Status);
            }
        }

        public class WhenCallingSetValueForANullableStructWithNonNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueForANullableStructWithNonNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, 100);
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Equal(100, this.customer.LastInvoice);
            }
        }

        public class WhenCallingSetValueForANullableStructWithNull
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            public WhenCallingSetValueForANullableStructWithNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, null);
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Null(this.customer.LastInvoice);
            }
        }

        public class WhenCallingSetValueForAReferenceType
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueForAReferenceType()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, "Fred Blogs");
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Equal("Fred Blogs", this.customer.Name);
            }
        }

        public class WhenCallingSetValueGenericForANullableStructWithNonNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueGenericForANullableStructWithNonNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = (IPropertyAccessor<Customer, int?>)PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, 100);
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Equal(100, this.customer.LastInvoice);
            }
        }

        public class WhenCallingSetValueGenericForANullableStructWithNull
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            public WhenCallingSetValueGenericForANullableStructWithNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("LastInvoice");
                var propertyAccessor = (IPropertyAccessor<Customer, int?>)PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, null);
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Null(this.customer.LastInvoice);
            }
        }

        public class WhenCallingSetValueGenericForAReferenceType
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueGenericForAReferenceType()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = (IPropertyAccessor<Customer, string>)PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, "Fred Blogs");
            }

            [Fact]
            public void ThePropertyShouldBeSet()
            {
                Assert.Equal("Fred Blogs", this.customer.Name);
            }
        }

        public class WhenCallingSetValueWithDbNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueWithDbNull()
            {
                var propertyInfo = typeof(Customer).GetProperty("Name");
                var propertyAccessor = PropertyAccessor.Create(propertyInfo);

                propertyAccessor.SetValue(this.customer, System.DBNull.Value);
            }

            [Fact]
            public void ThePropertyShouldNotBeSet()
            {
                Assert.Null(this.customer.Name);
            }
        }

        private class Customer
        {
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
                get;
                set;
            }

            public CustomerStatus Status
            {
                get;
                set;
            }
        }
    }
}