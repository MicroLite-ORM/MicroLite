// -----------------------------------------------------------------------
// <copyright file="IDeleteListener.cs" company="Project Contributors">
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
namespace MicroLite.Listeners
{
    /// <summary>
    /// The interface which exposes hooks into the deletion of an object by the MicroLite ORM framework.
    /// </summary>
    public interface IDeleteListener
    {
        /// <summary>
        /// Invoked after the SqlQuery to delete the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been deleted.</param>
        /// <param name="rowsAffected">The number of rows affected by the delete.</param>
        void AfterDelete(object instance, int rowsAffected);

        /// <summary>
        /// Invoked before the SqlQuery to delete the record from the database is created.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        void BeforeDelete(object instance);
    }
}