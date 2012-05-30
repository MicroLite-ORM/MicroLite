namespace MicroLite.Listeners
{
    using System;

    /// <summary>
    /// Empty implementation of IListener.
    /// </summary>
    /// <remarks>
    /// Provided so that implementations don't have to implement full interface.
    /// Quite possibly a violation of SRP but it does allow for simplified extensions to the framework.
    /// </remarks>
    internal abstract class Listener : IListener
    {
        /// <summary>
        /// Invoked after the record for the instance has been inserted into the database.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result.</param>
        public virtual void AfterInsert(object instance, object executeScalarResult)
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
        /// <param name="forType">The type the query is for.</param>
        /// <param name="sqlQuery">The SqlQuery to be executed.</param>
        public virtual void BeforeInsert(Type forType, SqlQuery sqlQuery)
        {
        }

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        public virtual void BeforeUpdate(object instance)
        {
        }
    }
}