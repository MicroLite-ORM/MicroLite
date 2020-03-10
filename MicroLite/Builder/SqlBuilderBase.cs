// -----------------------------------------------------------------------
// <copyright file="SqlBuilderBase.cs" company="Project Contributors">
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
using System.Collections.Generic;
using System.Text;
using MicroLite.Builder.Syntax;
using MicroLite.Characters;
using MicroLite.Mapping;

namespace MicroLite.Builder
{
    /// <summary>
    /// The base class for classes which build an <see cref="SqlQuery" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal abstract class SqlBuilderBase : IToSqlQuery
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SqlBuilderBase"/> class.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters for the builder.</param>
        protected SqlBuilderBase(SqlCharacters sqlCharacters) => SqlCharacters = sqlCharacters;

        protected bool AddedWhere { get; set; }

        /// <summary>
        /// Gets the arguments currently added to the sql builder.
        /// </summary>
        protected List<SqlArgument> Arguments { get; } = new List<SqlArgument>();

        /// <summary>
        /// Gets the inner sql the sql builder.
        /// </summary>
        protected StringBuilder InnerSql { get; } = new StringBuilder(capacity: 128);

        protected string Operand { get; set; }

        /// <summary>
        /// Gets the SQL characters.
        /// </summary>
        protected SqlCharacters SqlCharacters { get; }

        protected string WhereColumnName { get; set; }

        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <remarks>This method is called to return an SqlQuery once query has been defined.</remarks>
        public virtual SqlQuery ToSqlQuery() => new SqlQuery(InnerSql.ToString(), Arguments.ToArray());

        protected void AddBetween(object lower, object upper, bool negate)
        {
            if (lower is null)
            {
                throw new ArgumentNullException(nameof(lower));
            }

            if (upper is null)
            {
                throw new ArgumentNullException(nameof(upper));
            }

            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            Arguments.Add(new SqlArgument(lower));
            Arguments.Add(new SqlArgument(upper));

            string lowerParam = SqlCharacters.GetParameterName(Arguments.Count - 2);
            string upperParam = SqlCharacters.GetParameterName(Arguments.Count - 1);

            InnerSql.Append(" (")
                .Append(WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" BETWEEN ")
                .Append(lowerParam)
                .Append(" AND ")
                .Append(upperParam)
                .Append(')');
        }

        protected void AddIn(object[] args, bool negate)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            InnerSql.Append(" (")
                .Append(WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" IN (");

            for (int i = 0; i < args.Length; i++)
            {
                Arguments.Add(new SqlArgument(args[i]));
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    InnerSql.Append(',');
                }

                InnerSql.Append(SqlCharacters.GetParameterName((Arguments.Count - args.Length) + i));
            }

            InnerSql.Append("))");
        }

        protected void AddIn(SqlQuery subQuery, bool negate)
        {
            if (subQuery is null)
            {
                throw new ArgumentNullException(nameof(subQuery));
            }

            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            Arguments.AddRange(subQuery.Arguments);

            string renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, Arguments.Count);

            InnerSql.Append(" (")
                .Append(WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" IN (")
                .Append(renumberedPredicate)
                .Append("))");
        }

        protected void AddIn(SqlQuery[] subQueries, bool negate)
        {
            if (subQueries is null)
            {
                throw new ArgumentNullException(nameof(subQueries));
            }

            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            InnerSql.Append(" (")
                .Append(WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" IN (");

            for (int i = 0; i < subQueries.Length; i++)
            {
                SqlQuery subQuery = subQueries[i];

                Arguments.AddRange(subQuery.Arguments);

                string renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, Arguments.Count);

                InnerSql.Append('(')
                    .Append(renumberedPredicate)
                    .Append(i < subQueries.Length - 1 ? "), " : ")");
            }

            InnerSql.Append("))");
        }

        protected void AddLike(object comparisonValue, bool negate)
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            Arguments.Add(new SqlArgument(comparisonValue));

            string parameter = SqlCharacters.GetParameterName(Arguments.Count - 1);

            InnerSql.Append(" (")
                .Append(WhereColumnName)
                .Append(negate ? " NOT" : string.Empty)
                .Append(" LIKE ")
                .Append(parameter)
                .Append(')');
        }

        protected void AddWithComparisonOperator(object comparisonValue, string comparisonOperator)
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            Arguments.Add(new SqlArgument(comparisonValue));

            string parameter = SqlCharacters.GetParameterName(Arguments.Count - 1);

            InnerSql.Append(" (")
                .Append(WhereColumnName)
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
                InnerSql.Append(SqlCharacters.LeftDelimiter)
                    .Append(objectInfo.TableInfo.Schema)
                    .Append(SqlCharacters.RightDelimiter)
                    .Append('.');
            }

            AppendTableName(objectInfo.TableInfo.Name);
        }

        /// <summary>
        /// Appends the table name to the inner sql.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        protected void AppendTableName(string table)
        {
            if (SqlCharacters.IsEscaped(table))
            {
                InnerSql.Append(table);
            }
            else
            {
                InnerSql.Append(SqlCharacters.LeftDelimiter)
                    .Append(table)
                    .Append(SqlCharacters.RightDelimiter);
            }
        }
    }
}
