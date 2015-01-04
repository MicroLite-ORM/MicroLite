// -----------------------------------------------------------------------
// <copyright file="WriteSqlBuilderBase.cs" company="MicroLite">
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
namespace MicroLite.Builder
{
    using System;
    using MicroLite.Builder.Syntax.Write;
    using MicroLite.Characters;
    using MicroLite.FrameworkExtensions;

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

            this.Operand = " AND";
            this.WhereColumnName = this.SqlCharacters.EscapeSql(column);

            return this;
        }

        public IAndOr Between(object lower, object upper)
        {
            this.AddBetween(lower, upper, negate: false);

            return this;
        }

        public IAndOr In(params object[] args)
        {
            this.AddIn(args, negate: false);

            return this;
        }

        public IAndOr In(params SqlQuery[] subQueries)
        {
            this.AddIn(subQueries, negate: false);

            return this;
        }

        public IAndOr In(SqlQuery subQuery)
        {
            this.AddIn(subQuery, negate: false);

            return this;
        }

        public IAndOr IsEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " = ");

            return this;
        }

        public IAndOr IsGreaterThan(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " > ");

            return this;
        }

        public IAndOr IsGreaterThanOrEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " >= ");

            return this;
        }

        public IAndOr IsLessThan(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " < ");

            return this;
        }

        public IAndOr IsLessThanOrEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " <= ");

            return this;
        }

        public IAndOr IsLike(object comparisonValue)
        {
            this.AddLike(comparisonValue, negate: false);

            return this;
        }

        public IAndOr IsNotEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " <> ");

            return this;
        }

        public IAndOr IsNotLike(object comparisonValue)
        {
            this.AddLike(comparisonValue, negate: true);

            return this;
        }

        public IAndOr IsNotNull()
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.InnerSql.Append(" (").Append(this.WhereColumnName).Append(" IS NOT NULL)");

            return this;
        }

        public IAndOr IsNull()
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.InnerSql.Append(" (").Append(this.WhereColumnName).Append(" IS NULL)");

            return this;
        }

        public IAndOr NotBetween(object lower, object upper)
        {
            this.AddBetween(lower, upper, negate: true);

            return this;
        }

        public IAndOr NotIn(params object[] args)
        {
            this.AddIn(args, negate: true);

            return this;
        }

        public IAndOr NotIn(params SqlQuery[] subQueries)
        {
            this.AddIn(subQueries, negate: true);

            return this;
        }

        public IAndOr NotIn(SqlQuery subQuery)
        {
            this.AddIn(subQuery, negate: true);

            return this;
        }

        public IWhereSingleColumn OrWhere(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            this.Operand = " OR";
            this.WhereColumnName = this.SqlCharacters.EscapeSql(column);

            return this;
        }

        public IWhereSingleColumn Where(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            this.WhereColumnName = this.SqlCharacters.EscapeSql(column);

            if (!this.AddedWhere)
            {
                this.InnerSql.Append(" WHERE");
                this.AddedWhere = true;
            }

            return this;
        }
    }
}