namespace MicroLite.Tests.TestEntities
{


    using System;
    using System.Data;
    using System.Data.Common;

    internal sealed class MockDbConnectionWrapper : DbConnection
    {
        private readonly IDbConnection connection;

        internal MockDbConnectionWrapper(IDbConnection connection)
        {
            this.connection = connection;
        }

        public override string ConnectionString
        {
            get
            {
                return this.connection.ConnectionString;
            }
            set
            {
                this.connection.ConnectionString = value;
            }
        }

        public override string Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string DataSource
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string ServerVersion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ConnectionState State
        {
            get
            {
                return this.connection.State;
            }
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            this.connection.Close();
        }

        public override void Open()
        {
            this.connection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new MockDbTransactionWrapper(this.connection.BeginTransaction(isolationLevel));
        }

        protected override DbCommand CreateDbCommand()
        {
            return new MockDbCommandWrapper(this.connection.CreateCommand());
        }

        protected override void Dispose(bool disposing)
        {
            this.connection.Dispose();
            base.Dispose(disposing);
        }
    }


}