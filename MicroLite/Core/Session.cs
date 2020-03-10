// -----------------------------------------------------------------------
// <copyright file="Session.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using MicroLite.Dialect;
using MicroLite.Driver;
using MicroLite.Listeners;
using MicroLite.Mapping;
using MicroLite.TypeConverters;

namespace MicroLite.Core
{
    /// <summary>
    /// The default implementation of <see cref="ISession"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal sealed class Session : ReadOnlySession, ISession, IAdvancedSession
    {
        private readonly SessionListeners _sessionListeners;

        internal Session(ConnectionScope connectionScope, ISqlDialect sqlDialect, IDbDriver sqlDriver, SessionListeners sessionListeners)
            : base(connectionScope, sqlDialect, sqlDriver)
            => _sessionListeners = sessionListeners;

        public new IAdvancedSession Advanced => this;

        public Task<bool> DeleteAsync(object instance)
            => DeleteAsync(instance, CancellationToken.None);

        public Task<bool> DeleteAsync(object instance, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return DeleteAsyncInternal(instance, cancellationToken);
        }

        public Task<bool> DeleteAsync(Type type, object identifier)
            => DeleteAsync(type, identifier, CancellationToken.None);

        public Task<bool> DeleteAsync(Type type, object identifier, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (identifier is null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return DeleteAsyncInternal(type, identifier, cancellationToken);
        }

        public Task<int> ExecuteAsync(SqlQuery sqlQuery)
            => ExecuteAsync(sqlQuery, CancellationToken.None);

        public Task<int> ExecuteAsync(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            return ExecuteQueryAsync(sqlQuery, cancellationToken);
        }

        public Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery)
            => ExecuteScalarAsync<T>(sqlQuery, CancellationToken.None);

        public Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            return ExecuteScalarQueryAsync<T>(sqlQuery, cancellationToken);
        }

        public Task InsertAsync(object instance)
            => InsertAsync(instance, CancellationToken.None);

        public Task InsertAsync(object instance, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return InsertAsyncInternal(instance, cancellationToken);
        }

        public Task<bool> UpdateAsync(object instance)
            => UpdateAsync(instance, CancellationToken.None);

        public Task<bool> UpdateAsync(object instance, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return UpdateAsyncInternal(instance, cancellationToken);
        }

        public Task<bool> UpdateAsync(ObjectDelta objectDelta)
            => UpdateAsync(objectDelta, CancellationToken.None);

        public Task<bool> UpdateAsync(ObjectDelta objectDelta, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (objectDelta is null)
            {
                throw new ArgumentNullException(nameof(objectDelta));
            }

            if (objectDelta.ChangeCount == 0)
            {
                throw new MicroLiteException(ExceptionMessages.ObjectDelta_MustContainAtLeastOneChange);
            }

            return UpdateAsyncInternal(objectDelta, cancellationToken);
        }

        private async Task<bool> DeleteAsyncInternal(object instance, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _sessionListeners.DeleteListeners.Count; i++)
            {
                _sessionListeners.DeleteListeners[i].BeforeDelete(instance);
            }

            IObjectInfo objectInfo = ObjectInfo.For(instance.GetType());

            object identifier = objectInfo.GetIdentifierValue(instance);

            if (objectInfo.IsDefaultIdentifier(identifier))
            {
                throw new MicroLiteException(ExceptionMessages.Session_IdentifierNotSetForDelete);
            }

            SqlQuery sqlQuery = SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier);

            int rowsAffected = await ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            for (int i = _sessionListeners.DeleteListeners.Count - 1; i >= 0; i--)
            {
                _sessionListeners.DeleteListeners[i].AfterDelete(instance, rowsAffected);
            }

            return rowsAffected == 1;
        }

        private async Task<bool> DeleteAsyncInternal(Type type, object identifier, CancellationToken cancellationToken)
        {
            IObjectInfo objectInfo = ObjectInfo.For(type);

            SqlQuery sqlQuery = SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier);

            int rowsAffected = await ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            return rowsAffected == 1;
        }

        private async Task<int> ExecuteQueryAsync(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            try
            {
                ConfigureCommand(sqlQuery);

                var command = (DbCommand)Command;

                int result = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                CommandCompleted();

                return result;
            }
            catch (OperationCanceledException)
            {
                // Don't re-wrap Operation Canceled exceptions
                throw;
            }
            catch (MicroLiteException)
            {
                // Don't re-wrap MicroLite exceptions
                throw;
            }
            catch (Exception e)
            {
                throw new MicroLiteException(e.Message, e);
            }
        }

        private async Task<T> ExecuteScalarQueryAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            try
            {
                ConfigureCommand(sqlQuery);

                var command = (DbCommand)Command;

                object result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                CommandCompleted();

                Type resultType = typeof(T);
                ITypeConverter typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;
                var converted = (T)typeConverter.ConvertFromDbValue(result, resultType);

                return converted;
            }
            catch (OperationCanceledException)
            {
                // Don't re-wrap Operation Canceled exceptions
                throw;
            }
            catch (MicroLiteException)
            {
                // Don't re-wrap MicroLite exceptions
                throw;
            }
            catch (Exception e)
            {
                throw new MicroLiteException(e.Message, e);
            }
        }

        private async Task InsertAsyncInternal(object instance, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _sessionListeners.InsertListeners.Count; i++)
            {
                _sessionListeners.InsertListeners[i].BeforeInsert(instance);
            }

            IObjectInfo objectInfo = ObjectInfo.For(instance.GetType());
            objectInfo.VerifyInstanceForInsert(instance);

            object identifier = await InsertReturningIdentifierAsync(objectInfo, instance, cancellationToken).ConfigureAwait(false);

            for (int i = _sessionListeners.InsertListeners.Count - 1; i >= 0; i--)
            {
                _sessionListeners.InsertListeners[i].AfterInsert(instance, identifier);
            }
        }

        private async Task<object> InsertReturningIdentifierAsync(IObjectInfo objectInfo, object instance, CancellationToken cancellationToken)
        {
            object identifier = null;

            SqlQuery insertSqlQuery = SqlDialect.BuildInsertSqlQuery(objectInfo, instance);

            if (SqlDialect.SupportsSelectInsertedIdentifier && objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                SqlQuery selectInsertIdSqlQuery = SqlDialect.BuildSelectInsertIdSqlQuery(objectInfo);

                if (DbDriver.SupportsBatchedQueries)
                {
                    SqlQuery combined = DbDriver.Combine(insertSqlQuery, selectInsertIdSqlQuery);
                    identifier = await ExecuteScalarQueryAsync<object>(combined, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await ExecuteQueryAsync(insertSqlQuery, cancellationToken).ConfigureAwait(false);
                    identifier = await ExecuteScalarQueryAsync<object>(selectInsertIdSqlQuery, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                identifier = await ExecuteScalarQueryAsync<object>(insertSqlQuery, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await ExecuteQueryAsync(insertSqlQuery, cancellationToken).ConfigureAwait(false);
            }

            return identifier;
        }

        private async Task<bool> UpdateAsyncInternal(object instance, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _sessionListeners.UpdateListeners.Count; i++)
            {
                _sessionListeners.UpdateListeners[i].BeforeUpdate(instance);
            }

            IObjectInfo objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.HasDefaultIdentifierValue(instance))
            {
                throw new MicroLiteException(ExceptionMessages.Session_IdentifierNotSetForUpdate);
            }

            SqlQuery sqlQuery = SqlDialect.BuildUpdateSqlQuery(objectInfo, instance);

            int rowsAffected = await ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            for (int i = _sessionListeners.UpdateListeners.Count - 1; i >= 0; i--)
            {
                _sessionListeners.UpdateListeners[i].AfterUpdate(instance, rowsAffected);
            }

            return rowsAffected == 1;
        }

        private async Task<bool> UpdateAsyncInternal(ObjectDelta objectDelta, CancellationToken cancellationToken)
        {
            SqlQuery sqlQuery = SqlDialect.BuildUpdateSqlQuery(objectDelta);

            int rowsAffected = await ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            return rowsAffected == 1;
        }
    }
}
