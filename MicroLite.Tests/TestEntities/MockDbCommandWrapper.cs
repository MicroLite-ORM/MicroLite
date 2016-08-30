namespace MicroLite.Tests.TestEntities
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class MockDbCommandWrapper : DbCommand
    {
        private readonly IDbCommand command;

        internal MockDbCommandWrapper(IDbCommand command)
        {
            this.command = command;
        }

        public override string CommandText
        {
            get
            {
                return this.command.CommandText;
            }
            set
            {
                this.command.CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return this.command.CommandTimeout;
            }
            set
            {
                this.command.CommandTimeout = value;
            }
        }

        public override CommandType CommandType
        {
            get
            {
                return this.command.CommandType;
            }
            set
            {
                this.command.CommandType = value;
            }
        }

        public override bool DesignTimeVisible
        {
            get;
            set;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this.command.UpdatedRowSource;
            }

            set
            {
                this.command.UpdatedRowSource = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return new MockDbConnectionWrapper(this.command.Connection);
            }
            set
            {
                this.command.Connection = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return new MockDbTransactionWrapper(this.command.Transaction);
            }
            set
            {
                this.command.Transaction = value;
            }
        }

        public override void Cancel()
        {
            this.command.Cancel();
        }

        public override int ExecuteNonQuery()
        {
            return this.command.ExecuteNonQuery();
        }

        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.command.ExecuteNonQuery());
        }

        public override object ExecuteScalar()
        {
            return this.command.ExecuteScalar();
        }

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.command.ExecuteScalar());
        }

        public override void Prepare()
        {
            this.command.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            this.command.Dispose();
            base.Dispose(disposing);
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return new MockDbDataReaderWrapper(this.command.ExecuteReader(behavior));
        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            var reader = new MockDbDataReaderWrapper(this.command.ExecuteReader(behavior));

            return Task.FromResult((DbDataReader)reader);
        }
    }
}