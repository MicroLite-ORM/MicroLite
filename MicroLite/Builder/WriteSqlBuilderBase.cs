// -----------------------------------------------------------------------
// <copyright file="WriteSqlBuilderBase.cs" company="Project Contributors">
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
using MicroLite.Builder.Syntax.Write;
using MicroLite.Characters;
using MicroLite.FrameworkExtensions;

namespace MicroLite.Builder
{
    /// <summary>
    /// The base class for classes which build an <see cref="SqlQuery" /> to perform write operations.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal abstract class WriteSqlBuilderBase : SqlBuilderBase, IWhere, IWhereSingleColumn, IAndOr
    {
        protected WriteSqlBuilderBase(SqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
        }

        public IWhereSingleColumn AndWhere(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            Operand = " AND";
            WhereColumnName = SqlCharacters.EscapeSql(column);

            return this;
        }

        public IAndOr Between(object lower, object upper)
        {
            AddBetween(lower, upper, negate: false);

            return this;
        }

        public IAndOr In(params object[] args)
        {
            AddIn(args, negate: false);

            return this;
        }

        public IAndOr In(params SqlQuery[] subQueries)
        {
            AddIn(subQueries, negate: false);

            return this;
        }

        public IAndOr In(SqlQuery subQuery)
        {
            AddIn(subQuery, negate: false);

            return this;
        }

        public IAndOr IsEqualTo(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " = ");

            return this;
        }

        public IAndOr IsGreaterThan(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " > ");

            return this;
        }

        public IAndOr IsGreaterThanOrEqualTo(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " >= ");

            return this;
        }

        public IAndOr IsLessThan(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " < ");

            return this;
        }

        public IAndOr IsLessThanOrEqualTo(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " <= ");

            return this;
        }

        public IAndOr IsLike(object comparisonValue)
        {
            AddLike(comparisonValue, negate: false);

            return this;
        }

        public IAndOr IsNotEqualTo(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " <> ");

            return this;
        }

        public IAndOr IsNotLike(object comparisonValue)
        {
            AddLike(comparisonValue, negate: true);

            return this;
        }

        public IAndOr IsNotNull()
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            InnerSql.Append(" (").Append(WhereColumnName).Append(" IS NOT NULL)");

            return this;
        }

        public IAndOr IsNull()
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            InnerSql.Append(" (").Append(WhereColumnName).Append(" IS NULL)");

            return this;
        }

        public IAndOr NotBetween(object lower, object upper)
        {
            AddBetween(lower, upper, negate: true);

            return this;
        }

        public IAndOr NotIn(params object[] args)
        {
            AddIn(args, negate: true);

            return this;
        }

        public IAndOr NotIn(params SqlQuery[] subQueries)
        {
            AddIn(subQueries, negate: true);

            return this;
        }

        public IAndOr NotIn(SqlQuery subQuery)
        {
            AddIn(subQuery, negate: true);

            return this;
        }

        public IWhereSingleColumn OrWhere(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            Operand = " OR";
            WhereColumnName = SqlCharacters.EscapeSql(column);

            return this;
        }

        public IWhereSingleColumn Where(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            WhereColumnName = SqlCharacters.EscapeSql(column);

            if (!AddedWhere)
            {
                InnerSql.Append(" WHERE");
                AddedWhere = true;
            }

            return this;
        }
    }
}
