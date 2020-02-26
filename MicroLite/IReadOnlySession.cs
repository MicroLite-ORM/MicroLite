// -----------------------------------------------------------------------
// <copyright file="IReadOnlySession.cs" company="Project Contributors">
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
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace MicroLite
{
    /// <summary>
    /// The interface which provides the read methods to map objects to database records.
    /// </summary>
    public interface IReadOnlySession : IHideObjectMethods, IDisposable
    {
        /// <summary>
        /// Gets the advanced async session operations.
        /// </summary>
        IAdvancedReadOnlySession Advanced { get; }

        /// <summary>
        /// Gets the current transaction or null if one has not been started.
        /// </summary>
        ITransaction CurrentTransaction { get; }

        /// <summary>
        /// Gets the operations which allow additional objects to be queried in a single database call.
        /// </summary>
        IIncludeSession Include { get; }

        /// <summary>
        /// Begins a transaction using <see cref="IsolationLevel"/>.ReadCommitted.
        /// </summary>
        /// <returns>An <see cref="ITransaction"/> with the default isolation level of the connection.</returns>
        /// <remarks>It is a good idea to perform all insert/update/delete actions inside a transaction.</remarks>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         // perform actions against ISession.
        ///         // ...
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        ITransaction BeginTransaction();

        /// <summary>
        /// Begins the transaction with the specified <see cref="IsolationLevel"/>.
        /// </summary>
        /// <param name="isolationLevel">The isolation level to use for the transaction.</param>
        /// <returns>An <see cref="ITransaction"/> with the specified <see cref="IsolationLevel"/>.</returns>
        /// <remarks>It is a good idea to perform all insert/update/delete actions inside a transaction.</remarks>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     // This overload allows us to specify a specific IsolationLevel.
        ///     using (var transaction = session.BeginTransaction(IsolationLevel.ReadCommitted))
        ///     {
        ///         // perform actions against ISession.
        ///         // ...
        ///
        ///         try
        ///         {
        ///             transaction.Commit();
        ///         }
        ///         catch (Exception exception)
        ///         {
        ///             transaction.Rollback();
        ///             // Log or throw the exception.
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Executes the specified SQL query and returns the matching objects in a list.
        /// </summary>
        /// <typeparam name="T">The type of object the query relates to.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("SELECT * FROM Invoices WHERE CustomerId = @p0", 1324);
        ///
        ///         var invoices = await session.FetchAsync&lt;Invoice&gt;(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes FetchAsync&lt;T&gt;(SqlQuery, CancellationToken) with CancellationToken.None.</remarks>
        Task<IList<T>> FetchAsync<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Executes the specified SQL query and returns the matching objects in a list.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <typeparam name="T">The type of object the query relates to.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("SELECT * FROM Invoices WHERE CustomerId = @p0", 1324);
        ///
        ///         var invoices = await session.FetchAsync&lt;Invoice&gt;(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<IList<T>> FetchAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken);

        /// <summary>
        /// Pages the specified SQL query and returns an <see cref="PagedResult&lt;T&gt;"/> containing the desired results.
        /// </summary>
        /// <typeparam name="T">The type of object the query relates to.</typeparam>
        /// <param name="sqlQuery">The SQL query to page before executing.</param>
        /// <param name="pagingOptions">The <see cref="PagingOptions"/>.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("SELECT * FROM Customers WHERE LastName = @p0", "Smith");
        ///
        ///         var customers = await session.PagedAsync&lt;Customer&gt;(query, PagingOptions.ForPage(page: 1, resultsPerPage: 25));
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes PagedAsync&lt;T&gt;(SqlQuery, PagingOptions, CancellationToken) with CancellationToken.None.</remarks>
        Task<PagedResult<T>> PagedAsync<T>(SqlQuery sqlQuery, PagingOptions pagingOptions);

        /// <summary>
        /// Pages the specified SQL query and returns an <see cref="PagedResult&lt;T&gt;"/> containing the desired results.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <typeparam name="T">The type of object the query relates to.</typeparam>
        /// <param name="sqlQuery">The SQL query to page before executing.</param>
        /// <param name="pagingOptions">The <see cref="PagingOptions"/>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("SELECT * FROM Customers WHERE LastName = @p0", "Smith");
        ///
        ///         var customers = await session.PagedAsync&lt;Customer&gt;(query, PagingOptions.ForPage(page: 1, resultsPerPage: 25));
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<PagedResult<T>> PagedAsync<T>(SqlQuery sqlQuery, PagingOptions pagingOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the instance of the specified type which corresponds to the row with the specified identifier
        /// in the mapped table, or null if the identifier values does not exist in the table.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="identifier">The record identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var customer = await session.SingleAsync&lt;Customer&gt;(17867);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes SingleAsync&lt;T&gt;(object, CancellationToken) with CancellationToken.None.</remarks>
        Task<T> SingleAsync<T>(object identifier)
            where T : class, new();

        /// <summary>
        /// Returns the instance of the specified type which corresponds to the row with the specified identifier
        /// in the mapped table, or null if the identifier values does not exist in the table.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="identifier">The record identifier.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var customer = await session.SingleAsync&lt;Customer&gt;(17867);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<T> SingleAsync<T>(object identifier, CancellationToken cancellationToken)
            where T : class, new();

        /// <summary>
        /// Returns a single instance based upon the specified SQL query, or null if no result is returned.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("SELECT * FROM Customers WHERE EmailAddress = @p0", "fred.flintstone@bedrock.com");
        ///
        ///         // This overload is useful to retrieve a single object based upon a unique value which isn't its identifier.
        ///         var customer = await session.SingleAsync&lt;Customer&gt;(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes SingleAsync&lt;T&gt;(SqlQuery, CancellationToken) with CancellationToken.None.</remarks>
        Task<T> SingleAsync<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Returns a single instance based upon the specified SQL query, or null if no result is returned.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncReadOnlySession()) // or sessionFactory.OpenAsyncSession()
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("SELECT * FROM Customers WHERE EmailAddress = @p0", "fred.flintstone@bedrock.com");
        ///
        ///         // This overload is useful to retrieve a single object based upon a unique value which isn't its identifier.
        ///         var customer = await session.SingleAsync&lt;Customer&gt;(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<T> SingleAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken);
    }
}
