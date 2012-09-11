// -----------------------------------------------------------------------
// <copyright file="CurrentSessionIdResolver.cs" company="MicroLite">
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
    /// <summary>
    /// A class which can be used to resolve the current session id to be written to the log.
    /// </summary>
    public sealed class CurrentSessionIdResolver
    {
        /// <summary>
        /// Returns the Id of the current Session.
        /// </summary>
        /// <returns>
        /// The Id of the current Session.
        /// </returns>
        public override string ToString()
        {
            return SessionLoggingContext.CurrentSessionId;
        }
    }
}