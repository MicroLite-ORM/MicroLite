// -----------------------------------------------------------------------
// <copyright file="Listener.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    /// <summary>
    /// Empty implementation of IListener.
    /// </summary>
    /// <remarks>
    /// Provided so that implementations don't have to implement full interface.
    /// Quite possibly a violation of SRP but it does allow for simplified extensions to the framework.
    /// </remarks>
    public abstract class Listener : IListener
    {
        private static readonly ListenerCollection collection = new ListenerCollection();

        /// <summary>
        /// Gets the listener collection which contains all listeners registered with the MicroLite ORM framework.
        /// </summary>
        public static ListenerCollection Listeners
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// Invoked after the SqlQuery to delete the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been deleted.</param>
        /// <param name="rowsAffected">The number of rows affected by the delete.</param>
        public virtual void AfterDelete(object instance, int rowsAffected)
        {
        }

        /// <summary>
        /// Invoked after the SqlQuery to insert the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result.</param>
        public virtual void AfterInsert(object instance, object executeScalarResult)
        {
        }

        /// <summary>
        /// Invoked after the SqlQuery to update the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been updates.</param>
        /// <param name="rowsAffected">The number of rows affected by the update.</param>
        public virtual void AfterUpdate(object instance, int rowsAffected)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to delete the record from the database is created.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        public virtual void BeforeDelete(object instance)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to delete the record from the database is executed.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        /// <param name="sqlQuery">The SqlQuery to be executed.</param>
        public virtual void BeforeDelete(object instance, SqlQuery sqlQuery)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        public virtual void BeforeInsert(object instance)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is executed.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        /// <param name="sqlQuery">The SqlQuery to be executed.</param>
        public virtual void BeforeInsert(object instance, SqlQuery sqlQuery)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        public virtual void BeforeUpdate(object instance)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is executed.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        /// <param name="sqlQuery">The SqlQuery to be executed.</param>
        public virtual void BeforeUpdate(object instance, SqlQuery sqlQuery)
        {
        }
    }
}