// -----------------------------------------------------------------------
// <copyright file="SqlQuery.cs" company="Project Contributors">
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
namespace MicroLite
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents an SQL command and its argument values.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{CommandText}")]
    public sealed class SqlQuery : IEquatable<SqlQuery>
    {
        private static readonly SqlArgument[] emptyArguments = new SqlArgument[0];

        private readonly SqlArgument[] arguments;
        private readonly string commandText;
        private int timeout;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlQuery"/> class with the specified command text and no argument values.
        /// </summary>
        /// <param name="commandText">The SQL command text to be executed against the data source.</param>
        public SqlQuery(string commandText)
        {
            this.arguments = SqlQuery.emptyArguments;
            this.commandText = commandText;
            this.timeout = 30;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlQuery"/> class with the specified command text and argument values.
        /// </summary>
        /// <param name="commandText">The SQL command text to be executed against the data source.</param>
        /// <param name="arguments">The argument values for the SQL command.</param>
        public SqlQuery(string commandText, params object[] arguments)
            : this(commandText)
        {
            if (arguments != null)
            {
                this.arguments = new SqlArgument[arguments.Length];

                for (int i = 0; i < arguments.Length; i++)
                {
                    this.arguments[i] = new SqlArgument(arguments[i]);
                }
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlQuery"/> class with the specified command text and <see cref="SqlArgument"/> values.
        /// </summary>
        /// <param name="commandText">The SQL command text to be executed against the data source.</param>
        /// <param name="arguments">The <see cref="SqlArgument"/>s for the SQL command.</param>
        public SqlQuery(string commandText, params SqlArgument[] arguments)
            : this(commandText)
        {
            if (arguments != null)
            {
                this.arguments = arguments;
            }
        }

        /// <summary>
        /// Gets the <see cref="SqlArgument"/>s for the SQL command.
        /// </summary>
        public IList<SqlArgument> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets the SQL command text to be executed against the data source.
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.commandText;
            }
        }

        /// <summary>
        /// Gets or sets the timeout in seconds for the query.
        /// </summary>
        /// <remarks>Defaults to 30 seconds.</remarks>
        public int Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                this.timeout = value;
            }
        }

        /// <summary>
        /// Gets the private SqlArgument array.
        /// </summary>
        internal SqlArgument[] ArgumentsArray
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var sqlQuery = obj as SqlQuery;

            return this.Equals(sqlQuery);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SqlQuery other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Arguments.Count != this.Arguments.Count
                || other.CommandText != this.CommandText)
            {
                return false;
            }

            for (int i = 0; i < this.Arguments.Count; i++)
            {
                if (!other.Arguments[i].Equals(this.Arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.CommandText.GetHashCode() ^ this.Arguments.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.CommandText;
        }
    }
}