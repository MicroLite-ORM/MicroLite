namespace MicroLite
{
    using System;

    /// <summary>
    /// The interface which exposes hooks into the processing of an object by the MicroLite ORM framework.
    /// </summary>
    internal interface IListener
    {
        /// <summary>
        /// Invoked after the record for the instance has been inserted into the database.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result.</param>
        void AfterInsert(object instance, object executeScalarResult);

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        /// <remarks>This is called before IListener.BeforeInsert(sqlQuery).</remarks>
        void BeforeInsert(object instance);

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is executed.
        /// </summary>
        /// <param name="forType">The type the query is for.</param>
        /// <param name="sqlQuery">The SqlQuery to be executed.</param>
        /// <remarks>This is called after IListener.BeforeInsert(instance).</remarks>
        void BeforeInsert(Type forType, SqlQuery sqlQuery);

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        void BeforeUpdate(object instance);
    }
}