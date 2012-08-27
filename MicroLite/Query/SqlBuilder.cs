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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using MicroLite.Mapping;

    /// <summary>
    /// A helper class for creating a dynamic <see cref="SqlQuery"/>.
    /// </summary>
    public sealed class SqlBuilder : IFrom, IFunction, IWhereOrOrderBy, IAndOrOrderBy, IOrderBy, IToSqlQuery, IWithParameter
    {
        private static readonly Regex parameterRegex = new Regex(@"(@p\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder();

        private SqlBuilder(string startingSql)
        {
            if (startingSql.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase))
            {
                this.innerSql.AppendLine(startingSql);
            }
            else
            {
                this.innerSql.Append(startingSql);
            }
        }

        /// <summary>
        /// Combines the specified SQL queries into a single SqlQuery.
        /// </summary>
        /// <param name="sqlQueries">The SQL queries to be combined.</param>
        /// <returns>The combined <see cref="SqlQuery" />.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if sqlQueries is null.</exception>
        public static SqlQuery Combine(IEnumerable<SqlQuery> sqlQueries)
        {
            if (sqlQueries == null)
            {
                throw new ArgumentNullException("sqlQueries");
            }

            int argumentsCount = 0;
            var sqlBuilder = new StringBuilder();

            foreach (var sqlQuery in sqlQueries)
            {
                argumentsCount += sqlQuery.Arguments.Count;

                var sql = ReNumberParameters(sqlQuery.CommandText, argumentsCount);

                sqlBuilder.AppendLine(sql + ";");
            }

            return new SqlQuery(sqlBuilder.ToString(0, sqlBuilder.Length - 3), sqlQueries.SelectMany(s => s.Arguments).ToArray());
        }

        /// <summary>
        /// Species the name of the specified procedure to be executed.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IWithParameter Execute(string procedure)
        {
            return new SqlBuilder("EXEC " + procedure + " ");
        }

        /// <summary>
        /// Creates a new query which calls an Sql function.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IFunction Select()
        {
            return new SqlBuilder("SELECT");
        }

        /// <summary>
        /// Creates a new query which selects the specified columns.
        /// </summary>
        /// <param name="columns">The columns to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IFrom Select(params string[] columns)
        {
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
        /// Specifies the type of object to count records for which match the specified filter.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        public IWhereOrOrderBy Count(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            this.innerSql.AppendLine(" COUNT(" + objectInfo.TableInfo.IdentifierColumn + ")");

            return this.From(objectInfo.TableInfo.Schema + "." + objectInfo.TableInfo.Name);
        }

        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IWhereOrOrderBy From(string table)
        {
            this.innerSql.AppendLine(" FROM " + table);

            return this;
        }

        /// <summary>
        /// Specifies the type to perform the query against.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
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
        /// Orders the results of the query by the specified column in ascending order.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IOrderBy OrderByAscending(string column)
        {
            this.innerSql.AppendLine(" ORDER BY " + column + " ASC");

            return this;
        }

        /// <summary>
        /// Orders the results of the query by the specified column in descending order.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IOrderBy OrderByDescending(string column)
        {
            this.innerSql.AppendLine(" ORDER BY " + column + " DESC");

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
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        public SqlQuery ToSqlQuery()
        {
            return new SqlQuery(this.innerSql.ToString(0, this.innerSql.Length - 2), this.arguments.ToArray());
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
            this.arguments.Add(arg);
            this.innerSql.Append(parameter + ", ");

            return this;
        }

        /// <summary>
        /// Re-numbers the parameters in the SQL based upon the total number of arguments.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="argumentsCount">The number of arguments.</param>
        /// <returns>The re-numbered SQL</returns>
        private static string ReNumberParameters(string sql, int argumentsCount)
        {
            var argsAdded = 0;

            var predicateReWriter = new StringBuilder(sql);

            var parameterNames = new HashSet<string>(parameterRegex.Matches(sql).Cast<Match>().Select(x => x.Value));

            if (parameterNames.Count > 0)
            {
                var parameterPrefix = parameterNames.First().Substring(0, 2);

                foreach (var parameterName in parameterNames.OrderByDescending(n => n))
                {
                    var newParameterName = parameterPrefix + (argumentsCount - ++argsAdded).ToString(CultureInfo.InvariantCulture);

                    predicateReWriter.Replace(parameterName, newParameterName);
                }
            }

            return predicateReWriter.ToString();
        }

        private void AppendPredicate(string appendFormat, string predicate, params object[] args)
        {
            this.arguments.AddRange(args);

            var renumberedPredicate = ReNumberParameters(predicate, this.arguments.Count);

            this.innerSql.AppendFormat(appendFormat, renumberedPredicate);
            this.innerSql.AppendLine();
        }
    }
}