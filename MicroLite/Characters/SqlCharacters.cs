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
        private static readonly SqlCharacters empty = new SqlCharacters();
        private static readonly char[] period = new[] { '.' };
        private static SqlCharacters defaultSqlCharacters = null;

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
                var current = CallContext.LogicalGetData(LogicalGetDataName) as SqlCharacters;

                if (current != null)
                {
                    return current;
                }

                return defaultSqlCharacters ?? empty;
            }

            set
            {
                if (value is null)
                {
                    CallContext.FreeNamedDataSlot(LogicalGetDataName);
                    defaultSqlCharacters = null;
                }
                else
                {
                    CallContext.LogicalSetData(LogicalGetDataName, value);
                    defaultSqlCharacters = value;
                }
            }
        }

        /// <summary>
        /// Gets an Empty set of SqlCharacters which does not support named parameters or escaping of values.
        /// </summary>
        public static SqlCharacters Empty
        {
            get
            {
                return empty;
            }
        }

        /// <summary>
        /// Gets a string containing the delimiter used on the left hand side to escape an SQL value.
        /// </summary>
        public virtual string LeftDelimiter
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a string containing the wildcard value for use in LIKE statements.
        /// </summary>
        public virtual string LikeWildcard
        {
            get
            {
                return "%";
            }
        }

        /// <summary>
        /// Gets a string containing the delimiter used on the right hand side to escape an SQL value.
        /// </summary>
        public virtual string RightDelimiter
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a string containing the wildcard value for use in SELECT statements.
        /// </summary>
        public virtual string SelectWildcard
        {
            get
            {
                return "*";
            }
        }

        /// <summary>
        /// Gets a string containing the parameter value for use in parameterised statements.
        /// </summary>
        public virtual string SqlParameter
        {
            get
            {
                return "?";
            }
        }

        /// <summary>
        /// Gets the character used to separate SQL statements.
        /// </summary>
        public virtual string StatementSeparator
        {
            get
            {
                return ";";
            }
        }

        /// <summary>
        /// Gets the stored procedure invocation command.
        /// </summary>
        public virtual string StoredProcedureInvocationCommand
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        public virtual bool SupportsNamedParameters
        {
            get
            {
                return false;
            }
        }

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

            if (this.IsEscaped(sql))
            {
                return sql;
            }

            if (!sql.Contains('.'))
            {
                return this.LeftDelimiter + sql + this.RightDelimiter;
            }

            var sqlPieces = sql.Split(period);

            return string.Join(".", sqlPieces.Select(s => this.LeftDelimiter + s + this.RightDelimiter));
        }

        /// <summary>
        /// Gets the name of the parameter for the specified position.
        /// </summary>
        /// <param name="position">The parameter position.</param>
        /// <returns>The name of the parameter for the specified position.</returns>
        public string GetParameterName(int position)
        {
            if (this.SupportsNamedParameters)
            {
                return this.SqlParameter + "p" + position.ToString(CultureInfo.InvariantCulture);
            }

            return this.SqlParameter;
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

            return sql.StartsWith(this.LeftDelimiter, StringComparison.OrdinalIgnoreCase)
                && sql.EndsWith(this.RightDelimiter, StringComparison.OrdinalIgnoreCase);
        }
    }
}
