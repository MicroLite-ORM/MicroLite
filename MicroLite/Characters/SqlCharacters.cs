// -----------------------------------------------------------------------
// <copyright file="SqlCharacters.cs" company="Project Contributors">
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
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace MicroLite.Characters
{
    /// <summary>
    /// A class containing the SQL characters for an SQL Dialect.
    /// </summary>
    [Serializable]
    public class SqlCharacters : MarshalByRefObject
    {
        private const string LogicalGetDataName = "MicroLite.Characters.SqlCharacters_Current";
        private static readonly char[] s_period = new[] { '.' };
        private static SqlCharacters s_defaultSqlCharacters = null;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlCharacters"/> class.
        /// </summary>
        protected SqlCharacters()
        {
        }

        /// <summary>
        /// Gets or sets the current SqlCharacters or Empty if not otherwise specified.
        /// </summary>
        /// <remarks>
        /// Using CallContext internally allows us to set the correct SqlCharacters
        /// in a scenario where we have multiple session factories for different
        /// database providers active at once and SqlCharacters are used (via builder).
        /// The static defaultSqlCharacters is a fall-back so we don't end up with Empty
        /// characters.
        /// </remarks>
        public static SqlCharacters Current
        {
            get
            {
                if (CallContext.LogicalGetData(LogicalGetDataName) is SqlCharacters current)
                {
                    return current;
                }

                return s_defaultSqlCharacters ?? Empty;
            }

            set
            {
                if (value is null)
                {
                    CallContext.FreeNamedDataSlot(LogicalGetDataName);
                    s_defaultSqlCharacters = null;
                }
                else
                {
                    CallContext.LogicalSetData(LogicalGetDataName, value);
                    s_defaultSqlCharacters = value;
                }
            }
        }

        /// <summary>
        /// Gets an Empty set of SqlCharacters which does not support named parameters or escaping of values.
        /// </summary>
        public static SqlCharacters Empty { get; } = new SqlCharacters();

        /// <summary>
        /// Gets a string containing the delimiter used on the left hand side to escape an SQL value.
        /// </summary>
        public virtual string LeftDelimiter => string.Empty;

        /// <summary>
        /// Gets a string containing the wildcard value for use in LIKE statements.
        /// </summary>
        public virtual string LikeWildcard => "%";

        /// <summary>
        /// Gets a string containing the delimiter used on the right hand side to escape an SQL value.
        /// </summary>
        public virtual string RightDelimiter => string.Empty;

        /// <summary>
        /// Gets a string containing the wildcard value for use in SELECT statements.
        /// </summary>
        public virtual string SelectWildcard => "*";

        /// <summary>
        /// Gets a string containing the parameter value for use in parameterised statements.
        /// </summary>
        public virtual string SqlParameter => "?";

        /// <summary>
        /// Gets the character used to separate SQL statements.
        /// </summary>
        public virtual string StatementSeparator => ";";

        /// <summary>
        /// Gets the stored procedure invocation command.
        /// </summary>
        public virtual string StoredProcedureInvocationCommand => string.Empty;

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        public virtual bool SupportsNamedParameters => false;

        /// <summary>
        /// Escapes the specified SQL using the left and right delimiters.
        /// </summary>
        /// <param name="sql">The SQL to be escaped.</param>
        /// <returns>The escaped SQL.</returns>
        /// <exception cref="ArgumentNullException">Thrown if sql is null.</exception>
        public string EscapeSql(string sql)
        {
            if (sql is null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (IsEscaped(sql))
            {
                return sql;
            }

            if (!sql.Contains('.'))
            {
                return LeftDelimiter + sql + RightDelimiter;
            }

            string[] sqlPieces = sql.Split(s_period);

            return string.Join(".", sqlPieces.Select(s => LeftDelimiter + s + RightDelimiter));
        }

        /// <summary>
        /// Gets the name of the parameter for the specified position.
        /// </summary>
        /// <param name="position">The parameter position.</param>
        /// <returns>The name of the parameter for the specified position.</returns>
        public string GetParameterName(int position)
        {
            if (SupportsNamedParameters)
            {
                return SqlParameter + "p" + position.ToString(CultureInfo.InvariantCulture);
            }

            return SqlParameter;
        }

        /// <summary>
        /// Determines whether the specified SQL is escaped.
        /// </summary>
        /// <param name="sql">The SQL to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified SQL is escaped; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEscaped(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return false;
            }

            return sql.StartsWith(LeftDelimiter, StringComparison.OrdinalIgnoreCase)
                && sql.EndsWith(RightDelimiter, StringComparison.OrdinalIgnoreCase);
        }
    }
}
