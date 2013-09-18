// -----------------------------------------------------------------------
// <copyright file="RawWhereBuilder.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A class which can be used to build a raw SQL WHERE clause.
    /// </summary>
    /// <remarks>
    /// This class can be used to build a WHERE clause which can be applied to the SqlBuilder
    /// without having to build it up using the fluent interface.
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{builder}")]
    public sealed class RawWhereBuilder
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Appends the specified predicate (the WHERE keyword will be set when calling ApplyTo so it doesn't need specifying in the predicate).
        /// </summary>
        /// <param name="predicate">The predicate to be appended.</param>
        /// <param name="args">The argument values.</param>
        /// <returns>The predicate builder.</returns>
        public RawWhereBuilder Append(string predicate, params object[] args)
        {
            this.arguments.AddRange(args);

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.arguments.Count);

            this.builder.Append(renumberedPredicate);

            return this;
        }

        /// <summary>
        /// Applies the predicate defined in this predicate builder to the result of SqlBuilder.Select().From().
        /// </summary>
        /// <param name="selectFrom">The result of the <see cref="SqlBuilder"/> select from method call.</param>
        /// <returns>The <see cref="IAndOrOrderBy"/> which is returned by the SqlBuilder after applying the Where clause.</returns>
        public IAndOrOrderBy ApplyTo(IWhereOrOrderBy selectFrom)
        {
            if (selectFrom == null)
            {
                throw new ArgumentNullException("selectFrom");
            }

            var where = selectFrom.Where(this.builder.ToString(), this.arguments.ToArray());

            return where;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}