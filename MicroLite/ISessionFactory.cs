// -----------------------------------------------------------------------
// <copyright file="ISessionFactory.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    /// <summary>
    /// The interface which specifies the factory options for creating <see cref="ISession"/>s.
    /// </summary>
    public interface ISessionFactory : IHideObjectMethods
    {
        /// <summary>
        /// Gets the name of the connection used by the session factory.
        /// </summary>
        string ConnectionName
        {
            get;
        }

        /// <summary>
        /// Opens a new session to the database.
        /// </summary>
        /// <returns>A new session instance.</returns>
        ISession OpenSession();
    }
}