// -----------------------------------------------------------------------
// <copyright file="ISqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Mapping;

    /// <summary>
    /// The interface for a class which builds SqlQueries for a specific database dialect.
    /// </summary>
    public interface ISqlDialect
    {
        /// <summary>
        /// Gets the SQL characters for the SQL dialect.
        /// </summary>
        SqlCharacters SqlCharacters
        {
            get;
        }

        /// <summary>
        /// Creates an SqlQuery to count the number of records which would be returned by the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>An <see cref="SqlQuery"/> to count the number of records which would be returned by the specified SqlQuery.</returns>
        SqlQuery CountQuery(SqlQuery sqlQuery);

        /// <summary>
        /// Creates an SqlQuery to perform an update based upon the values in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta to create the query for.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery CreateQuery(ObjectDelta objectDelta);

        /// <summary>
        /// Creates an SqlQuery with the specified statement type for the specified instance.
        /// </summary>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="instance">The instance to generate the SqlQuery for.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if the statement type is not supported.</exception>
        SqlQuery CreateQuery(StatementType statementType, object instance);

        /// <summary>
        /// Creates an SqlQuery with the specified statement type for the specified type and identifier.
        /// </summary>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="forType">The type of object to create the query for.</param>
        /// <param name="identifier">The identifier of the instance to create the query for.</param>
        /// <returns>The created <see cref="SqlQuery" />.</returns>
        /// <exception cref="NotSupportedException">Thrown if the statement type is not supported.</exception>
        SqlQuery CreateQuery(StatementType statementType, Type forType, object identifier);

        /// <summary>
        /// Creates an SqlQuery to select the identity of an inserted object if the database supports Identity or AutoIncrement.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>The created <see cref="SqlQuery" />.</returns>
        SqlQuery CreateSelectIdentityQuery(IObjectInfo objectInfo);

        /// <summary>
        /// Creates an SqlQuery to page the records which would be returned by the specified SqlQuery based upon the paging options.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="pagingOptions">The paging options.</param>
        /// <returns>
        /// A <see cref="SqlQuery" /> to return the paged results of the specified query.
        /// </returns>
        SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions);
    }
}