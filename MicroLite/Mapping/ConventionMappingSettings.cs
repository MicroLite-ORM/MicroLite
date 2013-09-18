// -----------------------------------------------------------------------
// <copyright file="ConventionMappingSettings.cs" company="MicroLite">
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
namespace MicroLite.Mapping
{
    using System;
    using System.Reflection;
    using MicroLite.Mapping.Inflection;

    /// <summary>
    /// A class containing the configurable settings.
    /// </summary>
    public sealed class ConventionMappingSettings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ConventionMappingSettings" /> class.
        /// </summary>
        public ConventionMappingSettings()
        {
            this.AllowInsert = (PropertyInfo propertyInfo) =>
            {
                return true;
            };
            this.AllowUpdate = (PropertyInfo propertyInfo) =>
            {
                return true;
            };
            this.IdentifierStrategy = IdentifierStrategy.DbGenerated;
            this.Ignore = (PropertyInfo propertyInfo) =>
            {
                return false;
            };
            this.InflectionService = Inflection.InflectionService.English;
            this.IsIdentifier = (PropertyInfo propertyInfo) =>
            {
                return propertyInfo.Name == "Id" || propertyInfo.Name == propertyInfo.DeclaringType.Name + "Id";
            };
            this.ResolveColumnName = (PropertyInfo propertyInfo) =>
            {
                if (propertyInfo.PropertyType.IsEnum)
                {
                    return propertyInfo.PropertyType.Name + "Id";
                }

                return propertyInfo.Name;
            };
            this.ResolveIdentifierColumnName = (PropertyInfo propertyInfo) =>
            {
                return propertyInfo.Name;
            };
            this.TableSchema = null;
            this.UsePluralClassNameForTableName = true;
        }

        /// <summary>
        /// Gets an instance of the settings with the default options set.
        /// </summary>
        public static ConventionMappingSettings Default
        {
            get
            {
                return new ConventionMappingSettings();
            }
        }

        /// <summary>
        /// Gets or sets the function which specifies whether a property can be inserted (returns true by default).
        /// </summary>
        public Func<PropertyInfo, bool> AllowInsert
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which specifies whether a property can be updated (returns true by default).
        /// </summary>
        public Func<PropertyInfo, bool> AllowUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier strategy (defaults to DbGenerated).
        /// </summary>
        public IdentifierStrategy IdentifierStrategy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which specifies whether a property should be ignored from the mapping (returns false by default).
        /// </summary>
        public Func<PropertyInfo, bool> Ignore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inflection service (defaults to InflectionService.English).
        /// </summary>
        public IInflectionService InflectionService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines whether a property is the identifier property (default returns true if the property name is 'Id' or {ClassName} + 'Id').
        /// </summary>
        public Func<PropertyInfo, bool> IsIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines the name of the column the property is mapped to (default returns the property name unless the property type is an enum in which case it returns {EnumName} + 'Id').
        /// </summary>
        public Func<PropertyInfo, string> ResolveColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines the name of the identifier column for the table (default returns the property name).
        /// </summary>
        public Func<PropertyInfo, string> ResolveIdentifierColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the table schema (defaults to null).
        /// </summary>
        public string TableSchema
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the plural class name for the table name (defaults to true).
        /// </summary>
        public bool UsePluralClassNameForTableName
        {
            get;
            set;
        }
    }
}