// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="MicroLite">
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

    /// <summary>
    /// The interface for a framework independent logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Writes the message to the log as a debug statement.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        void Debug(string message);

        /// <summary>
        /// Writes the message to the log as a debug statement.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        void Debug(string message, params string[] formatArgs);

        /// <summary>
        /// Writes the message to the log as an error.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "Acceptable for logging as it is used by log4net and nlog")]
        void Error(string message);

        /// <summary>
        /// Writes the message to the log as an error along with the exception that occurred.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception that occurred.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "Acceptable for logging as it is used by log4net and nlog")]
        void Error(string message, Exception exception);

        /// <summary>
        /// Writes the message to the log as an error.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "Acceptable for logging as it is used by log4net and nlog")]
        void Error(string message, params string[] formatArgs);

        /// <summary>
        /// Writes the message to the log as fatal.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        void Fatal(string message);

        /// <summary>
        /// Writes the message to the log as fatal along with the exception that occurred.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception that occurred.</param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Writes the message to the log as fatal.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        void Fatal(string message, params string[] formatArgs);

        /// <summary>
        /// Writes the message to the log as information.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        void Info(string message);

        /// <summary>
        /// Writes the message to the log as information.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        void Info(string message, params string[] formatArgs);

        /// <summary>
        /// Writes the message to the log as a warning.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        void Warn(string message);

        /// <summary>
        /// Writes the message to the log as a warning.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        void Warn(string message, params string[] formatArgs);
    }
}