namespace MicroLite
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A helper class for creating a dynamic <see cref="SqlQuery"/>.
    /// </summary>
    public sealed class SqlQueryBuilder : IFrom, IWhereOrOrderBy, IAndOrOrderBy, IOrderBy, IToSqlQuery
    {
        private static readonly Regex parameterRegex = new Regex(@"(@p\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder();

        private SqlQueryBuilder(string startingSql)
        {
            this.innerSql.AppendLine(startingSql);
        }

        /// <summary>
        /// Selects the specified columns.
        /// </summary>
        /// <param name="columns">The columns to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IFrom Select(params string[] columns)
        {
            return new SqlQueryBuilder("SELECT " + string.Join(", ", columns));
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

        private void AppendPredicate(string appendFormat, string predicate, params object[] args)
        {
            int argsAdded = 0;
            var predicateReWriter = new StringBuilder(predicate);

            var parameterNames = new HashSet<string>(parameterRegex.Matches(predicate).Cast<Match>().Select(x => x.Value));

            foreach (var parameterName in parameterNames)
            {
                var newParameterName = "@p" + this.arguments.Count.ToString(CultureInfo.InvariantCulture);

                predicateReWriter.Replace(parameterName, newParameterName);

                if (argsAdded < args.Length)
                {
                    this.arguments.Add(args[argsAdded]);
                    argsAdded++;
                }
            }

            this.innerSql.AppendFormat(appendFormat, predicateReWriter.ToString());
            this.innerSql.AppendLine();
        }
    }
}