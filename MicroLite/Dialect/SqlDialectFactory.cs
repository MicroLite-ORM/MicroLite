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

    /// <summary>
    /// The factory class for managing <see cref="ISqlDialect"/> implementations.
    /// </summary>
    internal static class SqlDialectFactory
    {
        private static readonly IDictionary<string, Type> dialects = new Dictionary<string, Type>
        {
            { "MicroLite.Dialect.MsSqlDialect", typeof(MsSqlDialect) }
        };

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

            throw new NotSupportedException(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName));
        }
    }
}