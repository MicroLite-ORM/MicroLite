// -----------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// A helper class for creating a dynamic <see cref="SqlQuery" />.
    /// </summary>
    public sealed class SqlBuilder : IFrom, IFunctionOrFrom, IWhereOrOrderBy, IAndOrOrderBy, IGroupBy, IOrderBy, IToSqlQuery, IWithParameter
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder();

        private SqlBuilder(string startingSql)
        {
            this.innerSql.Append(startingSql);
        }

        /// <summary>
        /// Species the name of the procedure to be executed.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IWithParameter Execute(string procedure)
        {
            return new SqlBuilder("EXEC " + procedure);
        }

        /// <summary>
        /// Creates a new query which selects the specified columns.
        /// </summary>
        /// <param name="columns">The columns to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IFunctionOrFrom Select(params string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                return new SqlBuilder("SELECT");
            }

            return new SqlBuilder("SELECT " + string.Join(", ", columns));
        }

        /// <summary>
        /// Adds a predicate as an AND to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IAndOrOrderBy AndWhere(string predicate, params object[] args)
        {
            this.AppendPredicate(" AND ({0})", predicate, args);

            return this;
        }

        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Average(string columnName)
        {
            return this.Average(columnName, columnName);
        }

        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Average(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" AVG(" + columnName + ") AS " + columnAlias);

            return this;
        }

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Count(string columnName)
        {
            return this.Count(columnName, columnName);
        }

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Count(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" COUNT(" + columnName + ") AS " + columnAlias);

            return this;
        }

        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IWhereOrOrderBy From(string table)
        {
            this.innerSql.Append(" FROM " + table);

            return this;
        }

        /// <summary>
        /// Specifies the type to perform the query against.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IWhereOrOrderBy From(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            IFrom select = this;

            if (this.innerSql.ToString().StartsWith("SELECT *", StringComparison.Ordinal))
            {
                select = Select(objectInfo.TableInfo.Columns.Select(c => c.ColumnName).ToArray());
            }

            return select.From(objectInfo.TableInfo.Schema + "." + objectInfo.TableInfo.Name);
        }

        /// <summary>
        /// Groups the results of the query by the specified columns.
        /// </summary>
        /// <param name="columns">The columns to group by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IOrderBy GroupBy(params string[] columns)
        {
            this.innerSql.Append(" GROUP BY " + string.Join(", ", columns));

            return this;
        }

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Max(string columnName)
        {
            return this.Max(columnName, columnName);
        }

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Max(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" MAX(" + columnName + ") AS " + columnAlias);

            return this;
        }

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Min(string columnName)
        {
            return this.Min(columnName, columnName);
        }

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Min(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" MIN(" + columnName + ") AS " + columnAlias);

            return this;
        }

        /// <summary>
        /// Orders the results of the query by the specified column in ascending order.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IOrderBy OrderByAscending(string column)
        {
            this.innerSql.Append(" ORDER BY " + column + " ASC");

            return this;
        }

        /// <summary>
        /// Orders the results of the query by the specified column in descending order.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IOrderBy OrderByDescending(string column)
        {
            this.innerSql.Append(" ORDER BY " + column + " DESC");

            return this;
        }

        /// <summary>
        /// Adds a predicate as an OR to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IAndOrOrderBy OrWhere(string predicate, params object[] args)
        {
            this.AppendPredicate(" OR ({0})", predicate, args);

            return this;
        }

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Sum(string columnName)
        {
            return this.Sum(columnName, columnName);
        }

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IFrom Sum(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" SUM(" + columnName + ") AS " + columnAlias);

            return this;
        }

        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        public SqlQuery ToSqlQuery()
        {
            return new SqlQuery(this.innerSql.ToString(), this.arguments.ToArray());
        }

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IAndOrOrderBy Where(string predicate, params object[] args)
        {
            this.AppendPredicate(" WHERE ({0})", predicate, args);

            return this;
        }

        /// <summary>
        /// Specifies that the stored procedure should be executed the specified parameter and argument.
        /// </summary>
        /// <param name="parameter">The parameter to be added.</param>
        /// <param name="arg">The argument value for the parameter.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IWithParameter WithParameter(string parameter, object arg)
        {
            if (this.arguments.Count > 0)
            {
                this.innerSql.Append(",");
            }

            this.arguments.Add(arg);
            this.innerSql.Append(" " + parameter);

            return this;
        }

        private void AppendPredicate(string appendFormat, string predicate, params object[] args)
        {
            this.arguments.AddRange(args);

            var renumberedPredicate = SqlUtil.ReNumberParameters(predicate, this.arguments.Count);

            this.innerSql.AppendFormat(appendFormat, renumberedPredicate);
        }
    }
}