// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Dialect
{
    using System;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect"/>.
    /// </summary>
    internal abstract class SqlDialect : ISqlDialect
    {
        public SqlQuery DeleteQuery(object instance)
        {
            var forType = instance.GetType();

            var objectInfo = ObjectInfo.For(forType);

            var identifierPropertyInfo =
                objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

            var identifierValue = identifierPropertyInfo.GetValue(instance);

            return this.DeleteQuery(forType, identifierValue);
        }

        public abstract SqlQuery DeleteQuery(Type forType, object identifier);

        public abstract SqlQuery InsertQuery(object instance);

        public abstract SqlQuery Page(SqlQuery sqlQuery, long page, long resultsPerPage);

        public abstract SqlQuery SelectQuery(Type forType, object identifier);

        public abstract SqlQuery UpdateQuery(object instance);
    }
}