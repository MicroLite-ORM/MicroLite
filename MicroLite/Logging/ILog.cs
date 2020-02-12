// -----------------------------------------------------------------------
// <copyright file="ILog.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace MicroLite.Logging
{
    /// <summary>
    /// The interface for a framework independent logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Gets a value indicating whether the logger is logging debug statements.
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is logging error statements.
        /// </summary>
        bool IsError { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is logging fatal statements.
        /// </summary>
        bool IsFatal { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is logging info statements.
        /// </summary>
        bool IsInfo { get; }

        /// <summary>
        /// Gets a value indicating whether the logger is logging warning statements.
        /// </summary>
        bool IsWarn { get; }

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
#pragma warning disable CA1716 // Identifiers should not match keywords
        void Error(string message);
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Writes the message to the log as an error along with the exception that occurred.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception that occurred.</param>
#pragma warning disable CA1716 // Identifiers should not match keywords
        void Error(string message, Exception exception);
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Writes the message to the log as an error.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
#pragma warning disable CA1716 // Identifiers should not match keywords
        void Error(string message, params string[] formatArgs);
#pragma warning restore CA1716 // Identifiers should not match keywords

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
