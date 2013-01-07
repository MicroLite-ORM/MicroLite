// -----------------------------------------------------------------------
// <copyright file="SessionLoggingContext.cs" company="MicroLite">
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
    /// A context class which captures the session id of the current session so that it can be included in the log.
    /// </summary>
    /// <remarks>
    /// ThreadStatic should be ok since MicroLite does not perform any async operations and SessionLoggingContext is internal
    /// so thread switching in ASP.NET or WCF shouldn't result in this being invalid at any point during logging.
    /// </remarks>
    internal struct SessionLoggingContext : IDisposable
    {
        [ThreadStatic]
        private static string currentSessionId;

        private readonly string previousSessionId;

        /// <summary>
        /// Initialises a new instance of the <see cref="SessionLoggingContext" /> struct.
        /// </summary>
        /// <param name="sessionId">The session id of the current session.</param>
        internal SessionLoggingContext(string sessionId)
        {
            this.previousSessionId = currentSessionId;

            currentSessionId = sessionId;
        }

        /// <summary>
        /// Gets the current session id or null if there is no active session.
        /// </summary>
        internal static string CurrentSessionId
        {
            get
            {
                return currentSessionId;
            }
        }

        /// <summary>
        /// Clears the current session id.
        /// </summary>
        public void Dispose()
        {
            currentSessionId = this.previousSessionId;
        }
    }
}