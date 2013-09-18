// -----------------------------------------------------------------------
// <copyright file="SqlQuery.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using System.Linq;

    /// <summary>
    /// A class which represents a parameterised SQL query.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{CommandText}")]
    public sealed class SqlQuery : IEquatable<SqlQuery>
    {
        private readonly List<object> arguments = new List<object>();

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlQuery"/> class with the specified command text and parameter values.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <param name="arguments">The parameter values for the query.</param>
        public SqlQuery(string commandText, params object[] arguments)
        {
            this.CommandText = commandText;
            this.arguments.AddRange(arguments ?? Enumerable.Empty<object>());
            this.Timeout = 30;
        }

        /// <summary>
        /// Gets the parameter values of the SQL query.
        /// </summary>
        public IList<object> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets or sets the SQL statement execute against the data source.
        /// </summary>
        public string CommandText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timeout in seconds for the query.
        /// </summary>
        public int Timeout
        {
            get;
            set;
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
                if (!object.Equals(other.Arguments[i], this.Arguments[i]))
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