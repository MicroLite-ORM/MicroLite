namespace MicroLite.Tests.Integration.Delete
{
    using Xunit;

    public class WhenDeletingAnUnknownInstance : IntegrationTest
    {
        private readonly bool deleted;

        public WhenDeletingAnUnknownInstance()
        {
            this.deleted = this.Session.Delete(new Customer
            {
                CustomerId = 1
            });
        }

        [Fact]
        public void DeleteShouldReturnFalse()
        {
            Assert.False(this.deleted);
        }
    }
}