namespace MicroLite.Core
{
    using System;

    /// <summary>
    /// The interface for a class which builds an <see cref="SqlQuery"/> for a object instance.
    /// </summary>
    internal interface ISqlQueryBuilder
    {
        /// <summary>
        /// Creates a SqlQuery to perform a delete for the given instance.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="instance">The instance to build the query for.</param>
        /// <returns>A <see cref="SqlQuery"/> to delete a record.</returns>
        SqlQuery DeleteQuery<T>(T instance);

        /// <summary>
        /// Creates a SqlQuery to perform an insert for the given instance.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="instance">The instance to build the query for.</param>
        /// <returns>A <see cref="SqlQuery"/> to insert a record.</returns>
        SqlQuery InsertQuery<T>(T instance);

        /// <summary>
        /// Pages the specified SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="page">The page number to get the results for.</param>
        /// <param name="resultsPerPage">The number of results to be shown per page.</param>
        /// <returns>A <see cref="SqlQuery"/> to return the paged results of the supplied query.</returns>
        SqlQuery Page(SqlQuery sqlQuery, long page, long resultsPerPage);

        /// <summary>
        /// Creates a SqlQuery to perform a select for the given type and identifier value.
        /// </summary>
        /// <param name="forType">The type of object the query is for.</param>
        /// <param name="identifier">The identifier value for the target record.</param>
        /// <returns>A <see cref="SqlQuery"/> to select a specific record.</returns>
        SqlQuery SelectQuery(Type forType, object identifier);

        /// <summary>
        /// Creates a SqlQuery to perform an update for the given instance.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="instance">The instance to build the query for.</param>
        /// <returns>A <see cref="SqlQuery"/> to update a record.</returns>
        SqlQuery UpdateQuery<T>(T instance);
    }
}