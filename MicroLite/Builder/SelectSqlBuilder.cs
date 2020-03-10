// -----------------------------------------------------------------------
// <copyright file="SelectSqlBuilder.cs" company="Project Contributors">
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
using MicroLite.Builder.Syntax.Read;
using MicroLite.Characters;
using MicroLite.FrameworkExtensions;
using MicroLite.Mapping;

namespace MicroLite.Builder
{
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class SelectSqlBuilder : SqlBuilderBase, IFunctionOrFrom, IWhereOrOrderBy, IAndOrOrderBy, IWhereSingleColumn, IHavingOrOrderBy, IWhereExists
    {
        private bool _addedOrder = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="SelectSqlBuilder"/> class with the starting command text 'SELECT *'.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        internal SelectSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters) => InnerSql.Append("SELECT *");

        /// <summary>
        /// Initialises a new instance of the <see cref="SelectSqlBuilder"/> class with an optional list of columns to select.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        /// <param name="column">The column.</param>
        internal SelectSqlBuilder(SqlCharacters sqlCharacters, string column)
            : base(sqlCharacters)
        {
            InnerSql.Append("SELECT ");

            if (column != null)
            {
                InnerSql.Append(SqlCharacters.EscapeSql(column));
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
            InnerSql.Append("SELECT ");

            if (columns != null)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    if (i > 0)
                    {
                        InnerSql.Append(',');
                    }

                    InnerSql.Append(SqlCharacters.EscapeSql(columns[i]));
                }
            }
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

        public IAndOrOrderBy AndWhere(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            for (int i = 0; i < args.Length; i++)
            {
                Arguments.Add(new SqlArgument(args[i]));
            }

            string renumberedPredicate = SqlUtility.RenumberParameters(predicate, Arguments.Count);

            InnerSql.Append(" AND (")
                .Append(renumberedPredicate)
                .Append(')');

            return this;
        }

        public IFunctionOrFrom Average(string columnName)
        {
            AddFunctionCall("AVG", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Average(string columnName, string columnAlias)
        {
            AddFunctionCall("AVG", columnName, columnAlias);

            return this;
        }

        public IAndOrOrderBy Between(object lower, object upper)
        {
            AddBetween(lower, upper, negate: false);

            return this;
        }

        public IFunctionOrFrom Count(string columnName)
        {
            AddFunctionCall("COUNT", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Count(string columnName, string columnAlias)
        {
            AddFunctionCall("COUNT", columnName, columnAlias);

            return this;
        }

        public IFunctionOrFrom Distinct(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            InnerSql.Append("DISTINCT ").Append(SqlCharacters.EscapeSql(column));

            return this;
        }

        public IFunctionOrFrom Distinct(params string[] columns)
        {
            if (columns is null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            InnerSql.Append("DISTINCT ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    InnerSql.Append(',');
                }

                InnerSql.Append(SqlCharacters.EscapeSql(columns[i]));
            }

            return this;
        }

        public IAndOrOrderBy Exists(SqlQuery subQuery)
        {
            AddExists(subQuery, negate: false);

            return this;
        }

        public IWhereOrOrderBy From(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("table"));
            }

            InnerSql.Append(" FROM ");
            AppendTableName(table);

            return this;
        }

        public IWhereOrOrderBy From(Type forType)
        {
            IObjectInfo objectInfo = ObjectInfo.For(forType);

            return From(objectInfo);
        }

        public IHavingOrOrderBy GroupBy(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            InnerSql.Append(" GROUP BY ");
            InnerSql.Append(SqlCharacters.EscapeSql(column));

            return this;
        }

        public IHavingOrOrderBy GroupBy(params string[] columns)
        {
            if (columns is null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            InnerSql.Append(" GROUP BY ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    InnerSql.Append(',');
                }

                InnerSql.Append(SqlCharacters.EscapeSql(columns[i]));
            }

            return this;
        }

        public IOrderBy Having(string predicate, object value)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            Arguments.Add(new SqlArgument(value));

            string renumberedPredicate = SqlUtility.RenumberParameters(predicate, Arguments.Count);

            InnerSql.Append(" HAVING ").Append(renumberedPredicate);

            return this;
        }

        public IAndOrOrderBy In(params object[] args)
        {
            AddIn(args, negate: false);

            return this;
        }

        public IAndOrOrderBy In(params SqlQuery[] subQueries)
        {
            AddIn(subQueries, negate: false);

            return this;
        }

        public IAndOrOrderBy In(SqlQuery subQuery)
        {
            AddIn(subQuery, negate: false);

            return this;
        }

        public IAndOrOrderBy IsEqualTo(object comparisonValue)
        {
            if (comparisonValue is null)
            {
                return IsNull();
            }

            AddWithComparisonOperator(comparisonValue, " = ");

            return this;
        }

        public IAndOrOrderBy IsEqualTo(SqlQuery subQuery)
        {
            AddWithComparisonOperator(subQuery, " = ");

            return this;
        }

        public IAndOrOrderBy IsGreaterThan(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " > ");

            return this;
        }

        public IAndOrOrderBy IsGreaterThanOrEqualTo(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " >= ");

            return this;
        }

        public IAndOrOrderBy IsLessThan(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " < ");

            return this;
        }

        public IAndOrOrderBy IsLessThanOrEqualTo(object comparisonValue)
        {
            AddWithComparisonOperator(comparisonValue, " <= ");

            return this;
        }

        public IAndOrOrderBy IsLike(object comparisonValue)
        {
            AddLike(comparisonValue, negate: false);

            return this;
        }

        public IAndOrOrderBy IsNotEqualTo(object comparisonValue)
        {
            if (comparisonValue is null)
            {
                return IsNotNull();
            }

            AddWithComparisonOperator(comparisonValue, " <> ");

            return this;
        }

        public IAndOrOrderBy IsNotEqualTo(SqlQuery subQuery)
        {
            AddWithComparisonOperator(subQuery, " <> ");

            return this;
        }

        public IAndOrOrderBy IsNotLike(object comparisonValue)
        {
            AddLike(comparisonValue, negate: true);

            return this;
        }

        public IAndOrOrderBy IsNotNull()
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            InnerSql.Append(" (").Append(WhereColumnName).Append(" IS NOT NULL)");

            return this;
        }

        public IAndOrOrderBy IsNull()
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            InnerSql.Append(" (").Append(WhereColumnName).Append(" IS NULL)");

            return this;
        }

        public IFunctionOrFrom Max(string columnName)
        {
            AddFunctionCall("MAX", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Max(string columnName, string columnAlias)
        {
            AddFunctionCall("MAX", columnName, columnAlias);

            return this;
        }

        public IFunctionOrFrom Min(string columnName)
        {
            AddFunctionCall("MIN", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Min(string columnName, string columnAlias)
        {
            AddFunctionCall("MIN", columnName, columnAlias);

            return this;
        }

        public IAndOrOrderBy NotBetween(object lower, object upper)
        {
            AddBetween(lower, upper, negate: true);

            return this;
        }

        public IAndOrOrderBy NotExists(SqlQuery subQuery)
        {
            AddExists(subQuery, negate: true);

            return this;
        }

        public IAndOrOrderBy NotIn(params object[] args)
        {
            AddIn(args, negate: true);

            return this;
        }

        public IAndOrOrderBy NotIn(params SqlQuery[] subQueries)
        {
            AddIn(subQueries, negate: true);

            return this;
        }

        public IAndOrOrderBy NotIn(SqlQuery subQuery)
        {
            AddIn(subQuery, negate: true);

            return this;
        }

        public IOrderBy OrderByAscending(string column)
        {
            AddOrder(column, " ASC");

            return this;
        }

        public IOrderBy OrderByAscending(params string[] columns)
        {
            AddOrder(columns, " ASC");

            return this;
        }

        public IOrderBy OrderByDescending(string column)
        {
            AddOrder(column, " DESC");

            return this;
        }

        public IOrderBy OrderByDescending(params string[] columns)
        {
            AddOrder(columns, " DESC");

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

        public IAndOrOrderBy OrWhere(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            for (int i = 0; i < args.Length; i++)
            {
                Arguments.Add(new SqlArgument(args[i]));
            }

            string renumberedPredicate = SqlUtility.RenumberParameters(predicate, Arguments.Count);

            InnerSql.Append(" OR (").Append(renumberedPredicate).Append(')');

            return this;
        }

        public IFunctionOrFrom Sum(string columnName)
        {
            AddFunctionCall("SUM", columnName, columnName);

            return this;
        }

        public IFunctionOrFrom Sum(string columnName, string columnAlias)
        {
            AddFunctionCall("SUM", columnName, columnAlias);

            return this;
        }

        public IWhereExists Where()
        {
            if (!AddedWhere)
            {
                InnerSql.Append(" WHERE");
                AddedWhere = true;
            }

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
                    Arguments.Add(new SqlArgument(args[i]));
                }
            }

            string renumberedPredicate = SqlUtility.RenumberParameters(predicate, Arguments.Count);

            InnerSql.Append(" WHERE ");

            if (!predicate.StartsWith("(", StringComparison.Ordinal) && !predicate.StartsWith(")", StringComparison.Ordinal))
            {
                InnerSql.Append('(');
            }

            InnerSql.Append(renumberedPredicate);

            if (!predicate.StartsWith("(", StringComparison.Ordinal) && !predicate.StartsWith(")", StringComparison.Ordinal))
            {
                InnerSql.Append(')');
            }

            AddedWhere = true;

            return this;
        }

        internal IWhereOrOrderBy From(IObjectInfo objectInfo)
        {
            if (InnerSql.Length == 8 && InnerSql[7].CompareTo('*') == 0)
            {
                InnerSql.Remove(7, 1);

                for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
                {
                    if (i > 0)
                    {
                        InnerSql.Append(',');
                    }

                    InnerSql.Append(SqlCharacters.EscapeSql(objectInfo.TableInfo.Columns[i].ColumnName));
                }
            }

            InnerSql.Append(" FROM ");
            AppendTableName(objectInfo);

            return this;
        }

        private void AddExists(SqlQuery subQuery, bool negate)
        {
            if (subQuery is null)
            {
                throw new ArgumentNullException(nameof(subQuery));
            }

            Arguments.AddRange(subQuery.Arguments);

            string renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, Arguments.Count);

            InnerSql.Append(negate ? " NOT" : string.Empty)
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

            if (InnerSql.Length > 7)
            {
                InnerSql.Append(',');
            }

            InnerSql.Append(functionName)
                .Append('(')
                .Append(SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);
        }

        private void AddOrder(string column, string direction)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            InnerSql.Append(!_addedOrder ? " ORDER BY " : ",");
            InnerSql.Append(SqlCharacters.EscapeSql(column));
            InnerSql.Append(direction);

            _addedOrder = true;
        }

        private void AddOrder(string[] columns, string direction)
        {
            if (columns is null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            InnerSql.Append(!_addedOrder ? " ORDER BY " : ",");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    InnerSql.Append(',');
                }

                InnerSql.Append(SqlCharacters.EscapeSql(columns[i]));
                InnerSql.Append(direction);
            }

            _addedOrder = true;
        }

        private void AddWithComparisonOperator(SqlQuery subQuery, string comparisonOperator)
        {
            if (!string.IsNullOrEmpty(Operand))
            {
                InnerSql.Append(Operand);
            }

            Arguments.AddRange(subQuery.Arguments);

            string renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, Arguments.Count);

            InnerSql.Append(" (")
                .Append(WhereColumnName)
                .Append(comparisonOperator)
                .Append('(')
                .Append(renumberedPredicate)
                .Append("))");
        }
    }
}
