// -----------------------------------------------------------------------
// <copyright file="SqlBuilderBase.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    using System.Collections.Generic;
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for classes which build an <see cref="SqlQuery" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal abstract class SqlBuilderBase : IToSqlQuery
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder(capacity: 128);
        private readonly SqlCharacters sqlCharacters;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlBuilderBase"/> class.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters for the builder.</param>
        protected SqlBuilderBase(SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
        }

        /// <summary>
        /// Gets the arguments currently added to the sql builder.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Allowed in this instance, we want to make use of AddRange.")]
        protected List<object> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets the inner sql the sql builder.
        /// </summary>
        protected StringBuilder InnerSql
        {
            get
            {
                return this.innerSql;
            }
        }

        /// <summary>
        /// Gets the SQL characters.
        /// </summary>
        protected SqlCharacters SqlCharacters
        {
            get
            {
                return this.sqlCharacters;
            }
        }

        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <remarks>This method is called to return an SqlQuery once query has been defined.</remarks>
        public virtual SqlQuery ToSqlQuery()
        {
            return new SqlQuery(this.innerSql.ToString(), this.arguments.ToArray());
        }

        /// <summary>
        /// Appends the table name to the inner sql.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        protected void AppendTableName(IObjectInfo objectInfo)
        {
            if (!string.IsNullOrEmpty(objectInfo.TableInfo.Schema))
            {
                this.InnerSql.Append(this.sqlCharacters.LeftDelimiter)
                    .Append(objectInfo.TableInfo.Schema)
                    .Append(this.sqlCharacters.RightDelimiter)
                    .Append('.');
            }

            this.AppendTableName(objectInfo.TableInfo.Name);
        }

        /// <summary>
        /// Appends the table name to the inner sql.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        protected void AppendTableName(string tableName)
        {
            if (this.sqlCharacters.IsEscaped(tableName))
            {
                this.innerSql.Append(tableName);
            }
            else
            {
                this.InnerSql.Append(this.sqlCharacters.LeftDelimiter)
                    .Append(tableName)
                    .Append(this.sqlCharacters.RightDelimiter);
            }
        }
    }
}