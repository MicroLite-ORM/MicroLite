﻿// -----------------------------------------------------------------------
// <copyright file="IListener.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    /// <summary>
    /// The interface which exposes hooks into the processing of an object by the MicroLite ORM framework.
    /// </summary>
    public interface IListener : IHideObjectMethods
    {
        /// <summary>
        /// Invoked after the SqlQuery to delete the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been deleted.</param>
        /// <param name="rowsAffected">The number of rows affected by the delete.</param>
        void AfterDelete(object instance, int rowsAffected);

        /// <summary>
        /// Invoked after the SqlQuery to insert the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result.</param>
        void AfterInsert(object instance, object executeScalarResult);

        /// <summary>
        /// Invoked after the SqlQuery to update the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been updates.</param>
        /// <param name="rowsAffected">The number of rows affected by the update.</param>
        void AfterUpdate(object instance, int rowsAffected);

        /// <summary>
        /// Invoked before the SqlQuery to delete the record from the database is created.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        void BeforeDelete(object instance);

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        void BeforeInsert(object instance);

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        void BeforeUpdate(object instance);
    }
}