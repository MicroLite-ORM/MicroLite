// -----------------------------------------------------------------------
// <copyright file="SelectSqlBuilder.cs" company="MicroLite">
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
    using MicroLite.Builder.Syntax.Read;
    using MicroLite.Characters;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class SelectSqlBuilder : SqlBuilderBase, ISelectFrom, IFunctionOrFrom, IWhereOrOrderBy, IAndOrOrderBy, IGroupBy, IOrderBy, IWhereSingleColumn, IHavingOrOrderBy, IWhereExists
    {
        private bool addedOrder = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="SelectSqlBuilder"/> class with the starting command text 'SELECT *'.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        internal SelectSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
            this.InnerSql.Append("SELECT *");
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SelectSqlBuilder"/> class with an optional list of columns to select.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        /// <param name="column">The column.</param>
        internal SelectSqlBuilder(SqlCharacters sqlCharacters, string column)
            : base(sqlCharacters)
        {
            this.InnerSql.Append("SELECT ");

            if (column != null)
            {
                this.InnerSql.Append(this.SqlCharacters.EscapeSql(column));
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SelectSqlBuilder"/> class with an optional list of columns to select.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        /// <param name="columns">The columns.</param>
        internal SelectSqlBuilder(SqlCharacters sqlCharacters, params string[] columns)
            : base(sqlCharacters)
        {
            this.InnerSql.Append("SELECT ");

            if (columns != null)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    if (i > 0)
                    {
                        this.InnerSql.Append(',');
                    }

                    this.InnerSql.Append(this.SqlCharacters.EscapeSql(columns[i]));
                }
            }
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

        public IAndOrOrderBy AndWhere(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            for (int i = 0; i < args.Length; i++)
            {
                this.Arguments.Add(new SqlArgument(args[i]));
            }

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" AND (")
                .Append(renumberedPredicate)
                .Append(')');

            return this;
        }

        public IFunctionOrFrom Average(string columnName)
        {
            this.AddFunctionCall("AVG", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Average(string columnName, string columnAlias)
        {
            this.AddFunctionCall("AVG", columnName, columnAlias);

            return this;
        }

        public IAndOrOrderBy Between(object lower, object upper)
        {
            this.AddBetween(lower, upper, negate: false);

            return this;
        }

        public IFunctionOrFrom Count(string columnName)
        {
            this.AddFunctionCall("COUNT", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Count(string columnName, string columnAlias)
        {
            this.AddFunctionCall("COUNT", columnName, columnAlias);

            return this;
        }

        public IFunctionOrFrom Distinct(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            this.InnerSql.Append("DISTINCT ").Append(this.SqlCharacters.EscapeSql(column));

            return this;
        }

        public IFunctionOrFrom Distinct(params string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            this.InnerSql.Append("DISTINCT ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    this.InnerSql.Append(',');
                }

                this.InnerSql.Append(this.SqlCharacters.EscapeSql(columns[i]));
            }

            return this;
        }

        public IAndOrOrderBy Exists(SqlQuery subQuery)
        {
            this.AddExists(subQuery, negate: false);

            return this;
        }

        public IWhereOrOrderBy From(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("table"));
            }

            this.InnerSql.Append(" FROM ");
            this.AppendTableName(table);

            return this;
        }

        public IWhereOrOrderBy From(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            return this.From(objectInfo);
        }

        public IHavingOrOrderBy GroupBy(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            this.InnerSql.Append(" GROUP BY ");
            this.InnerSql.Append(this.SqlCharacters.EscapeSql(column));

            return this;
        }

        public IHavingOrOrderBy GroupBy(params string[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            this.InnerSql.Append(" GROUP BY ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    this.InnerSql.Append(',');
                }

                this.InnerSql.Append(this.SqlCharacters.EscapeSql(columns[i]));
            }

            return this;
        }

        public IOrderBy Having(string predicate, object value)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            this.Arguments.Add(new SqlArgument(value));

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" HAVING ").Append(renumberedPredicate);

            return this;
        }

        public IAndOrOrderBy In(params object[] args)
        {
            this.AddIn(args, negate: false);

            return this;
        }

        public IAndOrOrderBy In(params SqlQuery[] subQueries)
        {
            this.AddIn(subQueries, negate: false);

            return this;
        }

        public IAndOrOrderBy In(SqlQuery subQuery)
        {
            this.AddIn(subQuery, negate: false);

            return this;
        }

        public IAndOrOrderBy IsEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " = ");

            return this;
        }

        public IAndOrOrderBy IsEqualTo(SqlQuery subQuery)
        {
            this.AddWithComparisonOperator(subQuery, " = ");

            return this;
        }

        public IAndOrOrderBy IsGreaterThan(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " > ");

            return this;
        }

        public IAndOrOrderBy IsGreaterThanOrEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " >= ");

            return this;
        }

        public IAndOrOrderBy IsLessThan(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " < ");

            return this;
        }

        public IAndOrOrderBy IsLessThanOrEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " <= ");

            return this;
        }

        public IAndOrOrderBy IsLike(object comparisonValue)
        {
            this.AddLike(comparisonValue, negate: false);

            return this;
        }

        public IAndOrOrderBy IsNotEqualTo(object comparisonValue)
        {
            this.AddWithComparisonOperator(comparisonValue, " <> ");

            return this;
        }

        public IAndOrOrderBy IsNotEqualTo(SqlQuery subQuery)
        {
            this.AddWithComparisonOperator(subQuery, " <> ");

            return this;
        }

        public IAndOrOrderBy IsNotLike(object comparisonValue)
        {
            this.AddLike(comparisonValue, negate: true);

            return this;
        }

        public IAndOrOrderBy IsNotNull()
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.InnerSql.Append(" (").Append(this.WhereColumnName).Append(" IS NOT NULL)");

            return this;
        }

        public IAndOrOrderBy IsNull()
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.InnerSql.Append(" (").Append(this.WhereColumnName).Append(" IS NULL)");

            return this;
        }

        public IFunctionOrFrom Max(string columnName)
        {
            this.AddFunctionCall("MAX", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Max(string columnName, string columnAlias)
        {
            this.AddFunctionCall("MAX", columnName, columnAlias);

            return this;
        }

        public IFunctionOrFrom Min(string columnName)
        {
            this.AddFunctionCall("MIN", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Min(string columnName, string columnAlias)
        {
            this.AddFunctionCall("MIN", columnName, columnAlias);

            return this;
        }

        public IAndOrOrderBy NotBetween(object lower, object upper)
        {
            this.AddBetween(lower, upper, negate: true);

            return this;
        }

        public IAndOrOrderBy NotExists(SqlQuery subQuery)
        {
            this.AddExists(subQuery, negate: true);

            return this;
        }

        public IAndOrOrderBy NotIn(params object[] args)
        {
            this.AddIn(args, negate: true);

            return this;
        }

        public IAndOrOrderBy NotIn(params SqlQuery[] subQueries)
        {
            this.AddIn(subQueries, negate: true);

            return this;
        }

        public IAndOrOrderBy NotIn(SqlQuery subQuery)
        {
            this.AddIn(subQuery, negate: true);

            return this;
        }

        public IOrderBy OrderByAscending(string column)
        {
            this.AddOrder(column, " ASC");

            return this;
        }

        public IOrderBy OrderByAscending(params string[] columns)
        {
            this.AddOrder(columns, " ASC");

            return this;
        }

        public IOrderBy OrderByDescending(string column)
        {
            this.AddOrder(column, " DESC");

            return this;
        }

        public IOrderBy OrderByDescending(params string[] columns)
        {
            this.AddOrder(columns, " DESC");

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

        public IAndOrOrderBy OrWhere(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            for (int i = 0; i < args.Length; i++)
            {
                this.Arguments.Add(new SqlArgument(args[i]));
            }

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" OR (").Append(renumberedPredicate).Append(')');

            return this;
        }

        public IFunctionOrFrom Sum(string columnName)
        {
            this.AddFunctionCall("SUM", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Sum(string columnName, string columnAlias)
        {
            this.AddFunctionCall("SUM", columnName, columnAlias);

            return this;
        }

        public IWhereExists Where()
        {
            if (!this.AddedWhere)
            {
                this.InnerSql.Append(" WHERE");
                this.AddedWhere = true;
            }

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

        public IAndOrOrderBy Where(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    this.Arguments.Add(new SqlArgument(args[i]));
                }
            }

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" WHERE (").Append(renumberedPredicate).Append(')');
            this.AddedWhere = true;

            return this;
        }

        internal IWhereOrOrderBy From(IObjectInfo objectInfo)
        {
            if (this.InnerSql.Length == 8 && this.InnerSql[7].CompareTo('*') == 0)
            {
                this.InnerSql.Remove(7, 1);

                for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        this.InnerSql.Append(',');
                    }

                    this.InnerSql.Append(this.SqlCharacters.EscapeSql(objectInfo.TableInfo.Columns[i].ColumnName));
                }
            }

            this.InnerSql.Append(" FROM ");
            this.AppendTableName(objectInfo);

            return this;
        }

        private void AddBetween(object lower, object upper, bool negate)
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

        private void AddExists(SqlQuery subQuery, bool negate)
        {
            if (subQuery == null)
            {
                throw new ArgumentNullException("subQuery");
            }

            this.Arguments.AddRange(subQuery.Arguments);

            var renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, this.Arguments.Count);

            this.InnerSql.Append(negate ? " NOT" : string.Empty)
                .Append(" EXISTS (")
                .Append(renumberedPredicate)
                .Append(")");
        }

        private void AddFunctionCall(string functionName, string columnName, string columnAlias)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnName"));
            }

            if (string.IsNullOrEmpty(columnAlias))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnAlias"));
            }

            if (this.InnerSql.Length > 7)
            {
                this.InnerSql.Append(',');
            }

            this.InnerSql.Append(functionName)
                .Append('(')
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);
        }

        private void AddIn(object[] args, bool negate)
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

        private void AddIn(SqlQuery[] subQueries, bool negate)
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

        private void AddIn(SqlQuery subQuery, bool negate)
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

        private void AddLike(object comparisonValue, bool negate)
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

        private void AddOrder(string column, string direction)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            this.InnerSql.Append(!this.addedOrder ? " ORDER BY " : ",");
            this.InnerSql.Append(this.SqlCharacters.EscapeSql(column));
            this.InnerSql.Append(direction);

            this.addedOrder = true;
        }

        private void AddOrder(string[] columns, string direction)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            this.InnerSql.Append(!this.addedOrder ? " ORDER BY " : ",");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    this.InnerSql.Append(',');
                }

                this.InnerSql.Append(this.SqlCharacters.EscapeSql(columns[i]));
                this.InnerSql.Append(direction);
            }

            this.addedOrder = true;
        }

        private void AddWithComparisonOperator(SqlQuery subQuery, string comparisonOperator)
        {
            if (!string.IsNullOrEmpty(this.Operand))
            {
                this.InnerSql.Append(this.Operand);
            }

            this.Arguments.AddRange(subQuery.Arguments);

            var renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, this.Arguments.Count);

            this.InnerSql.Append(" (")
                .Append(this.WhereColumnName)
                .Append(comparisonOperator)
                .Append('(')
                .Append(renumberedPredicate)
                .Append("))");
        }
    }
}