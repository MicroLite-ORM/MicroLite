namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PropertyAccessor" /> class.
    /// </summary>
    public class PropertyAccessorTests
    {
        private enum CustomerStatus
        {
            Active = 0,
            Disabled = 1,
            Suspended = 2
        }

        public class WhenCallingGetValue_ForAnEnum
        {
            private readonly Customer customer = new Customer
            {
                Status = CustomerStatus.Suspended
            };

            private readonly object value;

            public WhenCallingGetValue_ForAnEnum()
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

        public class WhenCallingGetValue_ForANullableValueType_WithANonNullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            private readonly object value;

            public WhenCallingGetValue_ForANullableValueType_WithANonNullValue()
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

        public class WhenCallingGetValue_ForANullableValueType_WithANullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = null
            };

            private readonly object value;

            public WhenCallingGetValue_ForANullableValueType_WithANullValue()
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

        public class WhenCallingGetValue_ForAReferenceType
        {
            private readonly Customer customer = new Customer
            {
                Name = "Fred Bloggs"
            };

            private readonly object value;

            public WhenCallingGetValue_ForAReferenceType()
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

        public class WhenCallingGetValueGeneric_ForANullableValueType_WithANonNullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            private readonly int? value;

            public WhenCallingGetValueGeneric_ForANullableValueType_WithANonNullValue()
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

        public class WhenCallingGetValueGeneric_ForANullableValueType_WithANullValue
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = null
            };

            private readonly int? value;

            public WhenCallingGetValueGeneric_ForANullableValueType_WithANullValue()
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

        public class WhenCallingGetValueGeneric_ForAReferenceType
        {
            private readonly Customer customer = new Customer
            {
                Name = "Fred Bloggs"
            };

            private readonly string value;

            public WhenCallingGetValueGeneric_ForAReferenceType()
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

        public class WhenCallingSetValue_ForAnEnum
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValue_ForAnEnum()
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

        public class WhenCallingSetValue_ForANullableStruct_WithNonNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValue_ForANullableStruct_WithNonNull()
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

        public class WhenCallingSetValue_ForANullableStruct_WithNull
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            public WhenCallingSetValue_ForANullableStruct_WithNull()
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

        public class WhenCallingSetValue_ForAReferenceType
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValue_ForAReferenceType()
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

        public class WhenCallingSetValue_WithDbNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValue_WithDbNull()
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

        public class WhenCallingSetValueGeneric_ForANullableStruct_WithNonNull
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueGeneric_ForANullableStruct_WithNonNull()
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

        public class WhenCallingSetValueGeneric_ForANullableStruct_WithNull
        {
            private readonly Customer customer = new Customer
            {
                LastInvoice = 100
            };

            public WhenCallingSetValueGeneric_ForANullableStruct_WithNull()
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

        public class WhenCallingSetValueGeneric_ForAReferenceType
        {
            private readonly Customer customer = new Customer();

            public WhenCallingSetValueGeneric_ForAReferenceType()
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