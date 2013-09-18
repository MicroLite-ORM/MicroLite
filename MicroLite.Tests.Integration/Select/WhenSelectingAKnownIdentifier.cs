namespace MicroLite.Tests.Integration.Select
{
    using System;
    using Xunit;

    public class WhenSelectingAKnownIdentifier : IntegrationTest
    {
        private readonly Customer customer;

        public WhenSelectingAKnownIdentifier()
        {
            this.Session.Insert(new Customer
            {
                DateOfBirth = DateTime.Today,
                EmailAddress = "joe.bloggs@email.com",
                Forename = "Joe",
                Status = CustomerStatus.Active,
                Surname = "Bloggs"
            });

            this.customer = this.Session.Single<Customer>(1);
        }

        [Fact]
        public void TheExpectedValuesShouldBeReturned()
        {
            Assert.Equal(1, this.customer.CustomerId);
            Assert.Equal(DateTime.Today, this.customer.DateOfBirth);
            Assert.Equal("joe.bloggs@email.com", this.customer.EmailAddress);
            Assert.Equal("Joe", this.customer.Forename);
            Assert.Equal(CustomerStatus.Active, this.customer.Status);
            Assert.Equal("Bloggs", this.customer.Surname);
        }
    }
}