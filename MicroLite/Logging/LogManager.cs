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
    using System.Diagnostics;

    /// <summary>
    /// A class which the MicroLite ORM framework can call to resolve an ILog implementation.
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// Gets or sets the function which can be called by MicroLite to resolve the <see cref="ILog"/> to use.
        /// </summary>
        public static Func<string, ILog> GetLogger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the log for the current (calling) class.
        /// </summary>
        /// <returns>The <see cref="ILog"/> for the class which called the method.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The method will return a different ILog depending on the caller, it shouldn't be a property.")]
        public static ILog GetCurrentClassLog()
        {
            if (GetLogger != null)
            {
                var stackFrame = new StackFrame(skipFrames: 1);

                return GetLog(stackFrame.GetMethod().DeclaringType.FullName);
            }

            return null;
        }

        /// <summary>
        /// Gets the log instance with the supplied name.
        /// </summary>
        /// <param name="name">The name of the log to get.</param>
        /// <returns>
        /// The <see cref="ILog"/> for the supplied log name or null if LogManager.GetLogger nas not been set.
        /// </returns>
        public static ILog GetLog(string name)
        {
            if (GetLogger != null)
            {
                return GetLogger(name);
            }

            return null;
        }
    }
}