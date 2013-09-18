// -----------------------------------------------------------------------
// <copyright file="IIncludeSession.cs" company="MicroLite">
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

    /// <summary>
    /// The interface which provides access to include operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for batch included values and have been moved to a separate interface to avoid
    /// cluttering the ISession API.
    /// </remarks>
    public interface IIncludeSession : IHideObjectMethods
    {
        /// <summary>
        /// Includes all instances of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <returns>A pointer to the included instances of the specified type.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     // Tell the session to include all countries.
        ///     var countries = session.Include.All&lt;Country&gt;();
        ///
        ///     // At this point, countries will point to an IIncludeMany&lt;Country&gt; which will have no values.
        ///     // You can call include for multiple things, they will all be loaded in a single database call once
        ///     // either ISession.Single, ISession.Fetch or ISession.Paged is called.
        ///
        ///     // Load the customer.
        ///     var customer = session.Single&lt;Customer&gt;(1792);
        ///
        ///     // We can now acces the countries.
        ///     this.View.CountryOptions = countries.Values;
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// This will return an object for every row in the table,
        /// it should be used to retrieve un-filtered lookup lists (for example the list of countries on a shipping form).
        /// </remarks>
        IIncludeMany<T> All<T>() where T : class, new();

        /// <summary>
        /// Includes many instances based upon the specified SQL query.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A pointer to the included instances of the specified type.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     // Query to fetch the invoices for the customer.
        ///     var invoicesQuery = new SqlQuery("SELECT * FROM Invoices WHERE CustomerId = @p0", 1792);
        ///
        ///     // Tell the session to include the invoices.
        ///     var invoices = session.Include.Many&lt;Invoice&gt;(invoicesQuery);
        ///
        ///     // At this point, invoices will point to an IIncludeMany&lt;Invoice&gt; which will have no values.
        ///     // You can call include for multiple things, they will all be loaded in a single database call once
        ///     // either ISession.Single, ISession.Fetch or ISession.Paged is called.
        ///
        ///     // Load the customer.
        ///     var customer = session.Single&lt;Customer&gt;(1792);
        ///
        ///     // We can now acces the invoices for the customer
        ///     foreach (var invoice in invoices.Values)
        ///     {
        ///         // ...
        ///     }
        /// }
        /// </code>
        /// </example>
        IIncludeMany<T> Many<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Includes a single value based upon the specified SQL query.
        /// </summary>
        /// <typeparam name="T">The type of value to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A pointer to the included value of the specified type.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     // Query to count the invoices for the customer.
        ///     var invoicesCountQuery = new SqlQuery("SELECT COUNT(InvoiceId) AS InvoiceCount FROM Invoices WHERE CustomerId = @p0", 1792);
        ///
        ///     // Tell the session to include the invoices count.
        ///     var invoicesCount = session.Include.Scalar&lt;int&gt;(invoicesQuery);
        ///
        ///     // At this point, invoices will point to an IInclude&lt;int&gt; which will have it's default value of 0.
        ///     // You can call include for multiple things, they will all be loaded in a single database call once
        ///     // either ISession.Single, ISession.Fetch or ISession.Paged is called.
        ///
        ///     // Load the customer.
        ///     var customer = session.Single&lt;Customer&gt;(1792);
        ///
        ///     // We can now acces the invoices count for the customer
        ///     if (invoicesCount.Value > 0)
        ///     {
        ///         ...
        ///     }
        /// }
        /// </code>
        /// </example>
        IInclude<T> Scalar<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Includes the instance of the specified type which corresponds to the row with the specified identifier
        /// in the mapped table.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="identifier">The record identifier.</param>
        /// <returns>A pointer to the included instance of the specified type.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified identifier is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     // Tell the session to include the customer.
        ///     var includeCustomer = session.Include.Single&lt;Customer&gt;(3264);
        ///
        ///     // At this point, includeCustomer will point to an IInclude&lt;Customer&gt; which will have no value.
        ///     // You can call include for multiple things, they will all be loaded in a single database call once
        ///     // either ISession.Single, ISession.Fetch or ISession.Paged is called.
        ///
        ///     // Query to fetch the invoices for the customer.
        ///     var invoicesQuery = new SqlQuery("SELECT * FROM Invoices WHERE CustomerId = @p0", 3264);
        ///
        ///     // Load the invoices.
        ///     var invoices = session.Fetch&lt;Invoice&gt;(query);
        ///
        ///     // We can now acces the customer
        ///     Console.WriteLine(includeCustomer.Value.Name);
        /// }
        /// </code>
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Single", Justification = "It's used in loads of places by the linq extension methods as a method name.")]
        IInclude<T> Single<T>(object identifier) where T : class, new();

        /// <summary>
        /// Includes a single instance based upon the specified SQL query.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A pointer to the included instance of the specified type.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the query.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Single", Justification = "It's used in loads of places by the linq extension methods as a method name.")]
        IInclude<T> Single<T>(SqlQuery sqlQuery);
    }
}