// -----------------------------------------------------------------------
// <copyright file="Session.cs" company="MicroLite">
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
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="ISession"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal sealed class Session : ReadOnlySession, ISession, IAdvancedSession
    {
        private readonly IList<IDeleteListener> deleteListeners;
        private readonly IList<IInsertListener> insertListeners;
        private readonly IList<IUpdateListener> updateListeners;

        internal Session(
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

        public new IAdvancedSession Advanced
        {
            get
            {
                return this;
            }
        }

        public bool Delete(object instance)
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

            var sqlQuery = this.SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier);

            var rowsAffected = this.ExecuteQuery(sqlQuery);

            for (int i = this.deleteListeners.Count - 1; i >= 0; i--)
            {
                this.deleteListeners[i].AfterDelete(instance, rowsAffected);
            }

            return rowsAffected == 1;
        }

        public bool Delete(Type type, object identifier)
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

            var sqlQuery = this.SqlDialect.BuildDeleteSqlQuery(objectInfo, identifier);

            var rowsAffected = this.ExecuteQuery(sqlQuery);

            return rowsAffected == 1;
        }

        public int Execute(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return this.ExecuteQuery(sqlQuery);
        }

        public T ExecuteScalar<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return this.ExecuteScalarQuery<T>(sqlQuery);
        }

        public void Insert(object instance)
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

            object identifier = this.InsertReturningIdentifier(objectInfo, instance);

            for (int i = this.insertListeners.Count - 1; i >= 0; i--)
            {
                this.insertListeners[i].AfterInsert(instance, identifier);
            }
        }

        public bool Update(object instance)
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

            var rowsAffected = this.ExecuteQuery(sqlQuery);

            for (int i = this.updateListeners.Count - 1; i >= 0; i--)
            {
                this.updateListeners[i].AfterUpdate(instance, rowsAffected);
            }

            return rowsAffected == 1;
        }

        public bool Update(ObjectDelta objectDelta)
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

            var rowsAffected = this.ExecuteQuery(sqlQuery);

            return rowsAffected == 1;
        }

        private int ExecuteQuery(SqlQuery sqlQuery)
        {
            try
            {
                this.ConfigureCommand(sqlQuery);

                var result = this.Command.ExecuteNonQuery();

                this.CommandCompleted();

                return result;
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

        private T ExecuteScalarQuery<T>(SqlQuery sqlQuery)
        {
            try
            {
                this.ConfigureCommand(sqlQuery);

                var result = this.Command.ExecuteScalar();

                this.CommandCompleted();

                var resultType = typeof(T);
                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;
                var converted = (T)typeConverter.ConvertFromDbValue(result, resultType);

                return converted;
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

        private object InsertReturningIdentifier(IObjectInfo objectInfo, object instance)
        {
            object identifier = null;

            SqlQuery insertSqlQuery = this.SqlDialect.BuildInsertSqlQuery(objectInfo, instance);

            if (this.SqlDialect.SupportsSelectInsertedIdentifier
                && objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                var selectInsertIdSqlQuery = this.SqlDialect.BuildSelectInsertIdSqlQuery(objectInfo);

                if (this.DbDriver.SupportsBatchedQueries)
                {
                    var combined = this.DbDriver.Combine(insertSqlQuery, selectInsertIdSqlQuery);
                    identifier = this.ExecuteScalarQuery<object>(combined);
                }
                else
                {
                    this.ExecuteQuery(insertSqlQuery);
                    identifier = this.ExecuteScalarQuery<object>(selectInsertIdSqlQuery);
                }
            }
            else if (objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                identifier = this.ExecuteScalarQuery<object>(insertSqlQuery);
            }
            else
            {
                this.ExecuteQuery(insertSqlQuery);
            }

            return identifier;
        }
    }
}