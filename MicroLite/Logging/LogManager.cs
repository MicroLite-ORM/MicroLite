// -----------------------------------------------------------------------
// <copyright file="LogManager.cs" company="MicroLite">
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
        internal static Func<string, ILog> GetLogger
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
            var getLogger = GetLogger;

            if (getLogger != null)
            {
                var stackFrame = new StackFrame(skipFrames: 1);

                return getLogger(stackFrame.GetMethod().DeclaringType.FullName);
            }

            return null;
        }
    }
}