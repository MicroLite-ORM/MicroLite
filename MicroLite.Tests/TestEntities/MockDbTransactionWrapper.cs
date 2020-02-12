namespace MicroLite.Tests.TestEntities
{


    using System.Data;
    using System.Data.Common;

    internal sealed class MockDbTransactionWrapper : DbTransaction
    {
        private readonly IDbTransaction transaction;

        internal MockDbTransactionWrapper(IDbTransaction transaction)
        {
            this.transaction = transaction;
        }

        public override IsolationLevel IsolationLevel
        {
            get
            {
                return this.transaction.IsolationLevel;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return new MockDbConnectionWrapper(this.transaction.Connection);
            }
        }

        public override void Commit()
        {
            this.transaction.Rollback();
        }

        public override void Rollback()
        {
            this.transaction.Rollback();
        }

        protected override void Dispose(bool disposing)
        {
            this.transaction.Dispose();
            base.Dispose(disposing);
        }
    }


}