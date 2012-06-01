namespace MicroLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// The interface which provides methods to map objects to database queries.
    /// </summary>
    public interface ISession : IHideObjectMembers, IDisposable
    {
        /// <summary>
        /// Gets the advanced session operations.
        /// </summary>
        IAdvancedSession Advanced
        {
            get;
        }

        /// <summary>
        /// Begins the transaction with the default isolation level of of the connection.
        /// </summary>
        /// <returns>The transaction.</returns>
        ITransaction BeginTransaction();

        /// <summary>
        /// Begins the transaction with the supplied isolation level.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>The transaction.</returns>
        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Deletes the database record for the supplied instance.
        /// </summary>
        /// <param name="instance">The instance to delete from the database.</param>
        /// <returns>true if the object was deleted successfully; otherwise false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if supplied instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        bool Delete(object instance);

        /// <summary>
        /// Executes the specified SQL query and returns the matching objects in a list.
        /// </summary>
        /// <typeparam name="T">The type of object the query relates to.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The objects that match the query in a list.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if supplied instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        IList<T> Fetch<T>(SqlQuery sqlQuery) where T : class, new();

        /// <summary>
        /// Inserts a new database record for the supplied instance.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <exception cref="ObjectDisposedException">Thrown if session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if supplied instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the insert command.</exception>
        void Insert(object instance);

        /// <summary>
        /// Pages the specified SQL query and returns an <see cref="PagedResult&lt;T&gt;"/> containing the desired results.
        /// </summary>
        /// <typeparam name="T">The type of object the query relates to.</typeparam>
        /// <param name="sqlQuery">The SQL query to page before executing.</param>
        /// <param name="page">The page number (supply a 1 for first page).</param>
        /// <param name="resultsPerPage">The number of results per page.</param>
        /// <returns>An <see cref="PagedResult&lt;T&gt;"/> containing the desired results.</returns>
        PagedResult<T> Paged<T>(SqlQuery sqlQuery, long page, long resultsPerPage) where T : class, new();

        /// <summary>
        /// Returns the instance of the specified type which corresponds to the row with the supplied identifier
        /// in the mapped table or null if the identifier values does not exist in the table.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="identifier">The record identifier.</param>
        /// <returns>An instance of the specified type or null if no matching record exists.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if supplied instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Single", Justification = "It's used in loads of places by the linq extension methods as a method name.")]
        T Single<T>(ValueType identifier) where T : class, new();

        /// <summary>
        /// Updates the database record for the supplied instance with the current property values.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <exception cref="ObjectDisposedException">Thrown if session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if supplied instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the update command.</exception>
        void Update(object instance);
    }
}