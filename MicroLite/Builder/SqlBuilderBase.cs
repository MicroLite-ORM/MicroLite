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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MicroLite.Builder.Syntax;
    using MicroLite.Characters;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for classes which build an <see cref="SqlQuery" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal abstract class SqlBuilderBase : IToSqlQuery
    {
        private readonly List<SqlArgument> arguments = new List<SqlArgument>();
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

        protected bool AddedWhere
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the arguments currently added to the sql builder.
        /// </summary>
        protected List<SqlArgument> Arguments
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

        protected string Operand
        {
            get;
            set;
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

        protected string WhereColumnName
        {
            get;
            set;
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

        protected void AddBetween(object lower, object upper, bool negate)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (upper == null)
            {
                throw new ArgumentNullException("upper");
            }

            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.Arguments.Add(new SqlArgument(lower));
            this.Arguments.Add(new SqlArgument(upper));

            var lowerParam = this.SqlCharacters.GetParameterName(this.Arguments.Count - 2);
            var upperParam = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" BETWEEN ")
                .Append(lowerParam)
                .Append(" AND ")
                .Append(upperParam)
                .Append(')');
        }

        protected void AddIn(object[] args, bool negate)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" IN (");

            for (int i = 0; i < args.Length; i++)
            {
                this.Arguments.Add(new SqlArgument(args[i]));
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    this.InnerSql.Append(',');
                }

                this.InnerSql.Append(this.SqlCharacters.GetParameterName((this.Arguments.Count - args.Length) + i));
            }

            this.InnerSql.Append("))");
        }

        protected void AddIn(SqlQuery subQuery, bool negate)
        {
            if (subQuery == null)
            {
                throw new ArgumentNullException("subQuery");
            }

            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.Arguments.AddRange(subQuery.Arguments);

            var renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, this.Arguments.Count);

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" IN (")
                .Append(renumberedPredicate)
                .Append("))");
        }

        protected void AddIn(SqlQuery[] subQueries, bool negate)
        {
            if (subQueries == null)
            {
                throw new ArgumentNullException("subQueries");
            }

            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" IN (");

            for (int i = 0; i < subQueries.Length; i++)
            {
                var subQuery = subQueries[i];

                this.Arguments.AddRange(subQuery.Arguments);

                var renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, this.Arguments.Count);

                this.InnerSql.Append('(')
                    .Append(renumberedPredicate)
                    .Append(i < subQueries.Length - 1 ? "), " : ")");
            }

            this.InnerSql.Append("))");
        }

        protected void AddLike(object comparisonValue, bool negate)
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.Arguments.Add(new SqlArgument(comparisonValue));

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" LIKE ")
                .Append(parameter)
                .Append(')');
        }

        protected void AddWithComparisonOperator(object comparisonValue, string comparisonOperator)
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.Arguments.Add(new SqlArgument(comparisonValue));

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(comparisonOperator)
                .Append(parameter)
                .Append(')');
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
        /// <param name="table">The name of the table.</param>
        protected void AppendTableName(string table)
        {
            if (this.sqlCharacters.IsEscaped(table))
            {
                this.innerSql.Append(table);
            }
            else
            {
                this.InnerSql.Append(this.sqlCharacters.LeftDelimiter)
                    .Append(table)
                    .Append(this.sqlCharacters.RightDelimiter);
            }
        }
    }
}