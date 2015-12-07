﻿// -----------------------------------------------------------------------
// <copyright file="AsyncSession.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="IAsyncSession"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal sealed class AsyncSession : AsyncReadOnlySession, IAsyncSession, IAdvancedAsyncSession
    {
        private readonly IList<IDeleteListener> deleteListeners;
        private readonly IList<IInsertListener> insertListeners;
        private readonly IList<IUpdateListener> updateListeners;

        internal AsyncSession(
            ConnectionScope connectionScope,
            ISqlDialect sqlDialect,
            IDbDriver sqlDriver,
            IList<IDeleteListener> deleteListeners,
            IList<IInsertListener> insertListeners,
            IList<IUpdateListener> updateListeners)
            : base(connectionScope, sqlDialect, sqlDriver)
        {
            this.deleteListeners = deleteListeners;
            this.insertListeners = insertListeners;
            this.updateListeners = updateListeners;
        }

        public new IAdvancedAsyncSession Advanced
        {
            get
            {
                return this;
            }
        }

        public Task<bool> DeleteAsync(object instance)
        {
            return this.DeleteAsync(instance, CancellationToken.None);
        }

        public async Task<bool> DeleteAsync(object instance, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            for (int i = 0; i < this.deleteListeners.Count; i++)
            {
                this.deleteListeners[i].BeforeDelete(instance);
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            var identifier = objectInfo.GetIdentifierValue(instance);

            if (objectInfo.IsDefaultIdentifier(identifier))
            {
                throw new MicroLiteException(ExceptionMessages.Session_IdentifierNotSetForDelete);
            }

            var sqlQuery = objectInfo.TableInfo.VersionColumn == null ? this.SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier) : this.SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier, objectInfo.GetVersionValue(instance));

            var rowsAffected = await this.ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            if (rowsAffected == 0 && objectInfo.TableInfo.VersionColumn != null)
            {
                throw new DBConcurrencyException(ExceptionMessages.Session_UpdateOptimisticConcurrencyError.FormatWith(objectInfo.TableInfo.Schema, objectInfo.TableInfo.Name, objectInfo.TableInfo.VersionColumn.ColumnName));
            }

            for (int i = this.deleteListeners.Count - 1; i >= 0; i--)
            {
                this.deleteListeners[i].AfterDelete(instance, rowsAffected);
            }

            return rowsAffected == 1;
        }

        public Task<bool> DeleteAsync(Type type, object identifier)
        {
            return this.DeleteAsync(type, identifier, CancellationToken.None);
        }

        public async Task<bool> DeleteAsync(Type type, object identifier, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var objectInfo = ObjectInfo.For(type);

            if (objectInfo.TableInfo.VersionColumn != null)
            {
                throw new MicroLiteException(
                    ExceptionMessages.Session_TypeMismatchIsVersioned.FormatWith(type.FullName));
            }

            var sqlQuery = this.SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier);

            var rowsAffected = await this.ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            return rowsAffected == 1;
        }

        public Task DeleteAsync(Type type, object identifier, object version)
        {
            return this.DeleteAsync(type, identifier, version, CancellationToken.None);
        }

        public async Task DeleteAsync(Type type, object identifier, object version, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            var objectInfo = ObjectInfo.For(type);

            if (objectInfo.TableInfo.VersionColumn == null)
            {
                throw new MicroLiteException(
                    ExceptionMessages.Session_TypeMismatchNotVersioned.FormatWith(type.FullName));
            }

            var sqlQuery = this.SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier, version);

            var rowsAffected = await this.ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            if (rowsAffected == 0 && objectInfo.TableInfo.VersionColumn != null)
            {
                throw new DBConcurrencyException(ExceptionMessages.Session_UpdateOptimisticConcurrencyError.FormatWith(objectInfo.TableInfo.Schema, objectInfo.TableInfo.Name, objectInfo.TableInfo.VersionColumn.ColumnName));
            }
        }

        public Task<int> ExecuteAsync(SqlQuery sqlQuery)
        {
            return this.ExecuteAsync(sqlQuery, CancellationToken.None);
        }

        public async Task<int> ExecuteAsync(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return await this.ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);
        }

        public Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery)
        {
            return this.ExecuteScalarAsync<T>(sqlQuery, CancellationToken.None);
        }

        public async Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return await this.ExecuteScalarQueryAsync<T>(sqlQuery, cancellationToken).ConfigureAwait(false);
        }

        public Task InsertAsync(object instance)
        {
            return this.InsertAsync(instance, CancellationToken.None);
        }

        public async Task InsertAsync(object instance, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            for (int i = 0; i < this.insertListeners.Count; i++)
            {
                this.insertListeners[i].BeforeInsert(instance);
            }

            var objectInfo = ObjectInfo.For(instance.GetType());
            objectInfo.VerifyInstanceForInsert(instance);

            object identifier = await this.InsertReturningIdentifierAsync(objectInfo, instance, cancellationToken).ConfigureAwait(false);

            for (int i = this.insertListeners.Count - 1; i >= 0; i--)
            {
                this.insertListeners[i].AfterInsert(instance, identifier);
            }
        }

        public Task<bool> UpdateAsync(object instance)
        {
            return this.UpdateAsync(instance, CancellationToken.None);
        }

        public async Task<bool> UpdateAsync(object instance, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            for (int i = 0; i < this.updateListeners.Count; i++)
            {
                this.updateListeners[i].BeforeUpdate(instance);
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.HasDefaultIdentifierValue(instance))
            {
                throw new MicroLiteException(ExceptionMessages.Session_IdentifierNotSetForUpdate);
            }

            var sqlQuery = this.SqlDialect.BuildUpdateSqlQuery(objectInfo, instance);

            var rowsAffected = await this.ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            if (rowsAffected == 0 && objectInfo.TableInfo.VersionColumn != null)
            {
                throw new DBConcurrencyException(ExceptionMessages.Session_UpdateOptimisticConcurrencyError.FormatWith(objectInfo.TableInfo.Schema, objectInfo.TableInfo.Name, objectInfo.TableInfo.VersionColumn.ColumnName));
            }

            if (rowsAffected == 1 && objectInfo.TableInfo.VersionColumn != null)
            {
                var index = Array.FindIndex(objectInfo.TableInfo.Columns.Where(c => c.AllowUpdate).ToArray(), c => c.IsVersion);
                objectInfo.SetVersionValue(instance, sqlQuery.ArgumentsArray[index].Value);
            }

            for (int i = this.updateListeners.Count - 1; i >= 0; i--)
            {
                this.updateListeners[i].AfterUpdate(instance, rowsAffected);
            }

            return rowsAffected == 1;
        }

        public Task<bool> UpdateAsync(ObjectDelta objectDelta)
        {
            return this.UpdateAsync(objectDelta, CancellationToken.None);
        }

        public async Task<bool> UpdateAsync(ObjectDelta objectDelta, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (objectDelta == null)
            {
                throw new ArgumentNullException("objectDelta");
            }

            if (objectDelta.ChangeCount == 0)
            {
                throw new MicroLiteException(ExceptionMessages.ObjectDelta_MustContainAtLeastOneChange);
            }

            var sqlQuery = this.SqlDialect.BuildUpdateSqlQuery(objectDelta);

            var rowsAffected = await this.ExecuteQueryAsync(sqlQuery, cancellationToken).ConfigureAwait(false);

            return rowsAffected == 1;
        }

        private async Task<int> ExecuteQueryAsync(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            try
            {
                this.ConfigureCommand(sqlQuery);

                var command = (DbCommand)this.Command;

                var result = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                this.CommandCompleted();

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
                this.ConfigureCommand(sqlQuery);

                var command = (DbCommand)this.Command;

                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                this.CommandCompleted();

                var resultType = typeof(T);
                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;
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

        private async Task<object> InsertReturningIdentifierAsync(IObjectInfo objectInfo, object instance, CancellationToken cancellationToken)
        {
            object identifier = null;

            var insertSqlQuery = this.SqlDialect.BuildInsertSqlQuery(objectInfo, instance);

            if (this.SqlDialect.SupportsSelectInsertedIdentifier
                && objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                var selectInsertIdSqlQuery = this.SqlDialect.BuildSelectInsertIdSqlQuery(objectInfo);

                if (this.DbDriver.SupportsBatchedQueries)
                {
                    var combined = this.DbDriver.Combine(insertSqlQuery, selectInsertIdSqlQuery);
                    identifier = await this.ExecuteScalarQueryAsync<object>(combined, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await this.ExecuteQueryAsync(insertSqlQuery, cancellationToken).ConfigureAwait(false);
                    identifier = await this.ExecuteScalarQueryAsync<object>(selectInsertIdSqlQuery, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                identifier = await this.ExecuteScalarQueryAsync<object>(insertSqlQuery, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await this.ExecuteQueryAsync(insertSqlQuery, cancellationToken).ConfigureAwait(false);
            }

            return identifier;
        }
    }
}