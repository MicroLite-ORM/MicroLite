namespace MicroLite.Core
{
    using System;
    using System.Data;

    internal interface IConnectionManager : IDisposable
    {
        ITransaction BeginTransaction();

        ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel);

        IDbCommand Build(SqlQuery sqlQuery);
    }
}