// -----------------------------------------------------------------------
// <copyright file="DeleteSqlBuilder.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Builder
{
    using System;
    using MicroLite.Builder.Syntax.Write;
    using MicroLite.Characters;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class DeleteSqlBuilder : WriteSqlBuilderBase, IDeleteFrom
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteSqlBuilder"/> class with the starting command text 'DELETE FROM '.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        internal DeleteSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
            this.InnerSql.Append("DELETE");
        }

        public IWhere From(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("table"));
            }

            this.InnerSql.Append(" FROM ");
            this.AppendTableName(table);

            return this;
        }

        public IWhere From(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            return this.From(objectInfo);
        }

        internal IWhere From(IObjectInfo objectInfo)
        {
            this.InnerSql.Append(" FROM ");
            this.AppendTableName(objectInfo);

            return this;
        }
    }
}