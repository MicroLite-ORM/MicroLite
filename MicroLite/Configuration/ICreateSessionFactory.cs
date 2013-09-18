// -----------------------------------------------------------------------
// <copyright file="ICreateSessionFactory.cs" company="MicroLite">
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
namespace MicroLite.Configuration
{
    /// <summary>
    /// The interface which specifies the creation of the <see cref="ISessionFactory"/> in the fluent configuration
    /// of the MicroLite ORM framework.
    /// </summary>
    public interface ICreateSessionFactory : IHideObjectMethods
    {
        /// <summary>
        /// Creates the session factory for the configured connection.
        /// </summary>
        /// <returns>The session factory for the specified connection.</returns>
        ISessionFactory CreateSessionFactory();
    }
}