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
    using MicroLite.Characters;
    using MicroLite.Mapping;

    /// <summary>
    /// The interface for a class which builds SqlQueries for a specific database dialect.
    /// </summary>
    public interface ISqlDialect
    {
        /// <summary>
        /// Gets the SQL characters used by the SQL dialect.
        /// </summary>
        SqlCharacters SqlCharacters
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the SQL Dialect supports selecting the identifier value of an inserted column.
        /// </summary>
        bool SupportsSelectInsertedIdentifier
        {
            get;
        }

        /// <summary>
        /// Builds an SqlQuery to delete the database record with the specified identifier for the type specified by the IObjectInfo.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="identifier">The identifier of the instance to delete.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery BuildDeleteSqlQuery(IObjectInfo objectInfo, object identifier);

        /// <summary>
        /// Builds an SqlQuery to insert a database record for the specified instance with the current property values of the instance.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="instance">The instance to insert.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery BuildInsertSqlQuery(IObjectInfo objectInfo, object instance);

        /// <summary>
        /// Builds an SqlQuery to select the id of an inserted object if the database has generated the identifier.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>The created <see cref="SqlQuery" />.</returns>
        SqlQuery BuildSelectInsertIdSqlQuery(IObjectInfo objectInfo);

        /// <summary>
        /// Builds an SqlQuery to select the database record with the specified identifier for the type specified by the IObjectInfo.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="identifier">The identifier of the instance to select.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery BuildSelectSqlQuery(IObjectInfo objectInfo, object identifier);

        /// <summary>
        /// Builds an SqlQuery to update the database record for the specified instance with the current property values of the instance.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="instance">The instance to update.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery BuildUpdateSqlQuery(IObjectInfo objectInfo, object instance);

        /// <summary>
        /// Builds an SqlQuery to update the database record based upon the values in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta to create the query for.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery BuildUpdateSqlQuery(ObjectDelta objectDelta);

        /// <summary>
        /// Creates an SqlQuery to count the number of records which would be returned by the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>An <see cref="SqlQuery"/> to count the number of records which would be returned by the specified SqlQuery.</returns>
        SqlQuery CountQuery(SqlQuery sqlQuery);

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