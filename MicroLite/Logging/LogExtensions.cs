namespace MicroLite.Logging
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="ILog"/> interface to simplify writing to the log since there is no
    /// guarantee that an <see cref="ILog"/> is in use.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Writes the message to the log as a debug statement.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        public static void TryLogDebug(this ILog log, string message, params string[] formatArgs)
        {
            if (log != null)
            {
                if (formatArgs != null && formatArgs.Length > 0)
                {
                    log.Debug(message, formatArgs);
                }
                else
                {
                    log.Debug(message);
                }
            }
        }

        /// <summary>
        /// Writes the message to the log as an error.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        public static void TryLogError(this ILog log, string message, params string[] formatArgs)
        {
            if (log != null)
            {
                if (formatArgs != null && formatArgs.Length > 0)
                {
                    log.Error(message, formatArgs);
                }
                else
                {
                    log.Error(message);
                }
            }
        }

        /// <summary>
        /// Writes the message to the log as an error along with the exception that occurred.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception that occurred.</param>
        public static void TryLogError(this ILog log, string message, Exception exception)
        {
            if (log != null)
            {
                log.Error(message, exception);
            }
        }

        /// <summary>
        /// Writes the message to the log as fatal.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        public static void TryLogFatal(this ILog log, string message, params string[] formatArgs)
        {
            if (log != null)
            {
                if (formatArgs != null && formatArgs.Length > 0)
                {
                    log.Fatal(message, formatArgs);
                }
                else
                {
                    log.Fatal(message);
                }
            }
        }

        /// <summary>
        /// Writes the message to the log as fatal along with the exception that occurred.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="exception">The exception that occurred.</param>
        public static void TryLogFatal(this ILog log, string message, Exception exception)
        {
            if (log != null)
            {
                log.Fatal(message, exception);
            }
        }

        /// <summary>
        /// Writes the message to the log as information.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        public static void TryLogInfo(this ILog log, string message, params string[] formatArgs)
        {
            if (log != null)
            {
                if (formatArgs != null && formatArgs.Length > 0)
                {
                    log.Info(message, formatArgs);
                }
                else
                {
                    log.Info(message);
                }
            }
        }

        /// <summary>
        /// Writes the message to the log as a warning.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="formatArgs">The format args.</param>
        public static void TryLogWarn(this ILog log, string message, params string[] formatArgs)
        {
            if (log != null)
            {
                if (formatArgs != null && formatArgs.Length > 0)
                {
                    log.Warn(message, formatArgs);
                }
                else
                {
                    log.Warn(message);
                }
            }
        }
    }
}