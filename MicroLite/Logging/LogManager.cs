// -----------------------------------------------------------------------
// <copyright file="LogManager.cs" company="MicroLite">
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
namespace MicroLite.Logging
{
    using System;

    /// <summary>
    /// A class which the MicroLite ORM framework can call to resolve an ILog implementation.
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// Gets or sets the function which can be called by MicroLite to resolve the ILog to use.
        /// </summary>
        public static Func<string, ILog> GetLogger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the log instance with the supplied name.
        /// </summary>
        /// <param name="name">The name of the log to get.</param>
        /// <returns>
        /// The <see cref="ILog"/> for the supplied log name or null if LogManager.GetLogger nas not been set.
        /// </returns>
        public static ILog GetLogInstance(string name)
        {
            if (GetLogger != null)
            {
                return GetLogger(name);
            }

            return null;
        }
    }
}