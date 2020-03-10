// -----------------------------------------------------------------------
// <copyright file="LowercaseWithUnderscoresConventionMappingSettings.cs" company="Project Contributors">
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
using System.Reflection;
using MicroLite.FrameworkExtensions;

namespace MicroLite.Mapping
{
    /// <summary>
    /// A class containing the convention mapping settings for lowercase with underscore separators (e.g. 'CreditCard' -> 'credit_card').
    /// </summary>
    public class LowercaseWithUnderscoresConventionMappingSettings : ConventionMappingSettings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="LowercaseWithUnderscoresConventionMappingSettings"/> class.
        /// </summary>
        public LowercaseWithUnderscoresConventionMappingSettings()
        {
#pragma warning disable CA1308 // Normalize strings to uppercase
            ResolveColumnName = (PropertyInfo propertyInfo) => ConventionMappingSettings.GetColumnName(propertyInfo).ToUnderscored().ToLowerInvariant();
            ResolveIdentifierColumnName = (PropertyInfo propertyInfo) => propertyInfo.Name.ToUnderscored().ToLowerInvariant();
            ResolveTableName = (Type type) =>
            {
                string tableName = UsePluralClassNameForTableName ? InflectionService.ToPlural(GetTableName(type)) : GetTableName(type);

                return tableName.ToUnderscored().ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            };
        }
    }
}
