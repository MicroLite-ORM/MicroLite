// -----------------------------------------------------------------------
// <copyright file="IInsertListener.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
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
    /// The interface which exposes hooks into the insertion of an object by the MicroLite ORM framework.
    /// </summary>
    public interface IInsertListener
    {
        /// <summary>
        /// Invoked after the SqlQuery to insert the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result (the identifier value returned by the database
        /// or null if the identifier is <see cref="MicroLite.Mapping.IdentifierStrategy" />.Assigned.</param>
        void AfterInsert(object instance, object executeScalarResult);

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        void BeforeInsert(object instance);
    }
}