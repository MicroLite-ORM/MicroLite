// -----------------------------------------------------------------------
// <copyright file="SqlDialectFactory.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Collections.Generic;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The factory class for managing <see cref="ISqlDialect"/> implementations.
    /// </summary>
    public static class SqlDialectFactory
    {
        private static readonly IDictionary<string, Type> dialects = new Dictionary<string, Type>
        {
            { "MicroLite.Dialect.MsSqlDialect", typeof(MsSqlDialect) },
            { "MicroLite.Dialect.SQLiteDialect", typeof(SQLiteDialect) },
            { "MicroLite.Dialect.PostgreSqlDialect", typeof(PostgreSqlDialect) },
            { "MicroLite.Dialect.MySqlDialect", typeof(MySqlDialect) }
        };

        private static readonly ILog log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Adds the specified dialect name and type to MicroLite ORM.
        /// </summary>
        /// <param name="dialectName">Name of the dialect.</param>
        /// <param name="dialectType">Type of the dialect.</param>
        /// <exception cref="MicroLiteException">Thrown if the dialect type does not implement ISqlDialect or the dialect name is already registered.</exception>
        public static void Add(string dialectName, Type dialectType)
        {
            if (string.IsNullOrEmpty(dialectName))
            {
                throw new ArgumentNullException("dialectName");
            }

            if (dialectType == null)
            {
                throw new ArgumentNullException("dialectType");
            }

            if (!typeof(ISqlDialect).IsAssignableFrom(dialectType))
            {
                log.TryLogFatal(Messages.SqlDialectFactory_DialectMustImplementISqlDialect.FormatWith(dialectType.Name));
                throw new MicroLiteException(Messages.SqlDialectFactory_DialectMustImplementISqlDialect.FormatWith(dialectType.Name));
            }

            if (dialects.ContainsKey(dialectName))
            {
                log.TryLogFatal(Messages.SqlDialectFactory_DialectNameAlreadyUsed.FormatWith(dialectName));
                throw new MicroLiteException(Messages.SqlDialectFactory_DialectNameAlreadyUsed.FormatWith(dialectName));
            }

            dialects.Add(dialectName, dialectType);
        }

        /// <summary>
        /// Gets the dialect with the specified name.
        /// </summary>
        /// <param name="dialectName">The name of the dialect.</param>
        /// <returns>The <see cref="ISqlDialect"/> for the specified name.</returns>
        /// <exception cref="NotSupportedException">The specified dialect name is not supported.</exception>
        internal static ISqlDialect GetDialect(string dialectName)
        {
            Type dialectType;

            if (dialects.TryGetValue(dialectName, out dialectType))
            {
                return (ISqlDialect)Activator.CreateInstance(dialectType);
            }

            log.TryLogFatal(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName));
            throw new NotSupportedException(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName));
        }

        /// <summary>
        /// Verifies that the dialect name is an <see cref="ISqlDialect"/> supported by MicroLite.
        /// </summary>
        /// <param name="dialectName">Name of the dialect.</param>
        /// <exception cref="NotSupportedException">The specified dialect name is not supported.</exception>
        internal static void VerifyDialect(string dialectName)
        {
            if (!dialects.ContainsKey(dialectName))
            {
                log.TryLogFatal(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName));
                throw new NotSupportedException(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName));
            }
        }
    }
}