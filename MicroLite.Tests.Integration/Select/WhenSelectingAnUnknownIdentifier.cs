namespace MicroLite.Tests.Integration.Select
{
    using Xunit;

    public class WhenSelectingAnUnknownIdentifier : IntegrationTest
    {
        private readonly Customer customer;

        public WhenSelectingAnUnknownIdentifier()
        {
            this.customer = this.Session.Single<Customer>(12345);
        }

        [Fact]
        public void SingleShouldReturnNull()
        {
            Assert.Null(this.customer);
        }
    }
}