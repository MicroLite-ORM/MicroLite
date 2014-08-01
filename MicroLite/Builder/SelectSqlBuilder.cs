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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class SelectSqlBuilder : SqlBuilderBase, ISelectFrom, IFunctionOrFrom, IWhereOrOrderBy, IAndOrOrderBy, IGroupBy, IOrderBy, IWhereSingleColumn, IHavingOrOrderBy
    {
        private bool addedOrder = false;
        private bool addedWhere = false;
        private string operand;
        private string whereColumnName;

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

            this.operand = " AND";
            this.whereColumnName = this.SqlCharacters.EscapeSql(column);

            return this;
        }

        public IAndOrOrderBy AndWhere(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            this.Arguments.AddRange(args);

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" AND (")
                .Append(renumberedPredicate)
                .Append(')');

            return this;
        }

        public IFunctionOrFrom Average(string columnName)
        {
            return this.Average(columnName, columnName);
        }

        public IFunctionOrFrom Average(string columnName, string columnAlias)
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

            this.InnerSql.Append("AVG(")
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);

            return this;
        }

        public IAndOrOrderBy Between(object lower, object upper)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (upper == null)
            {
                throw new ArgumentNullException("upper");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(lower);
            this.Arguments.Add(upper);

            var lowerParam = this.SqlCharacters.GetParameterName(this.Arguments.Count - 2);
            var upperParam = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (")
                .Append(this.whereColumnName)
                .Append(" BETWEEN ")
                .Append(lowerParam)
                .Append(" AND ")
                .Append(upperParam)
                .Append(')');

            return this;
        }

        public IFunctionOrFrom Count(string columnName)
        {
            return this.Count(columnName, columnName);
        }

        public IFunctionOrFrom Count(string columnName, string columnAlias)
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

            this.InnerSql.Append("COUNT(")
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);

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

            this.Arguments.Add(value);

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" HAVING ").Append(renumberedPredicate);

            return this;
        }

        public IAndOrOrderBy In(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.AddRange(args);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" IN (");

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    this.InnerSql.Append(',');
                }

                this.InnerSql.Append(this.SqlCharacters.GetParameterName((this.Arguments.Count - args.Length) + i));
            }

            this.InnerSql.Append("))");

            return this;
        }

        public IAndOrOrderBy In(SqlQuery subQuery)
        {
            if (subQuery == null)
            {
                throw new ArgumentNullException("subQuery");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.AddRange(subQuery.Arguments);

            var renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, this.Arguments.Count);

            this.InnerSql.Append(" (")
                .Append(this.whereColumnName)
                .Append(" IN (")
                .Append(renumberedPredicate)
                .Append("))");

            return this;
        }

        public IAndOrOrderBy IsEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" = ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsGreaterThan(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" > ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsGreaterThanOrEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" >= ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsLessThan(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" < ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsLessThanOrEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" <= ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsLike(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" LIKE ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsNotEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" <> ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsNotLike(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.Add(comparisonValue);

            var parameter = this.SqlCharacters.GetParameterName(this.Arguments.Count - 1);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" NOT LIKE ").Append(parameter).Append(')');

            return this;
        }

        public IAndOrOrderBy IsNotNull()
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" IS NOT NULL)");

            return this;
        }

        public IAndOrOrderBy IsNull()
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" IS NULL)");

            return this;
        }

        public IFunctionOrFrom Max(string columnName)
        {
            return this.Max(columnName, columnName);
        }

        public IFunctionOrFrom Max(string columnName, string columnAlias)
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

            this.InnerSql.Append("MAX(")
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);

            return this;
        }

        public IFunctionOrFrom Min(string columnName)
        {
            return this.Min(columnName, columnName);
        }

        public IFunctionOrFrom Min(string columnName, string columnAlias)
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

            this.InnerSql.Append("MIN(")
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);

            return this;
        }

        public IAndOrOrderBy NotIn(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.AddRange(args);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" NOT IN (");

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                {
                    this.InnerSql.Append(',');
                }

                this.InnerSql.Append(this.SqlCharacters.GetParameterName((this.Arguments.Count - args.Length) + i));
            }

            this.InnerSql.Append("))");

            return this;
        }

        public IAndOrOrderBy NotIn(SqlQuery subQuery)
        {
            if (subQuery == null)
            {
                throw new ArgumentNullException("subQuery");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.InnerSql.Append(this.operand);
            }

            this.Arguments.AddRange(subQuery.Arguments);

            var renumberedPredicate = SqlUtility.RenumberParameters(subQuery.CommandText, this.Arguments.Count);

            this.InnerSql.Append(" (").Append(this.whereColumnName).Append(" NOT IN (").Append(renumberedPredicate).Append("))");

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

            this.operand = " OR";
            this.whereColumnName = this.SqlCharacters.EscapeSql(column);

            return this;
        }

        public IAndOrOrderBy OrWhere(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            this.Arguments.AddRange(args);

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" OR (").Append(renumberedPredicate).Append(')');

            return this;
        }

        public IFunctionOrFrom Sum(string columnName)
        {
            return this.Sum(columnName, columnName);
        }

        public IFunctionOrFrom Sum(string columnName, string columnAlias)
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

            this.InnerSql.Append("SUM(")
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(") AS ")
                .Append(columnAlias);

            return this;
        }

        public IWhereSingleColumn Where(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"));
            }

            this.whereColumnName = this.SqlCharacters.EscapeSql(column);

            if (!this.addedWhere)
            {
                this.InnerSql.Append(" WHERE");
                this.addedWhere = true;
            }

            return this;
        }

        public IAndOrOrderBy Where(string predicate, params object[] args)
        {
            if (string.IsNullOrEmpty(predicate))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"));
            }

            this.Arguments.AddRange(args);

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.Arguments.Count);

            this.InnerSql.Append(" WHERE (").Append(renumberedPredicate).Append(')');
            this.addedWhere = true;

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
    }
}