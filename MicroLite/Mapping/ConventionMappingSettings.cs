// -----------------------------------------------------------------------
// <copyright file="ConventionMappingSettings.cs" company="MicroLite">
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
namespace MicroLite.Mapping
{
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
            this.IdentifierStrategy = Mapping.IdentifierStrategy.DbGenerated;
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
        /// Gets or sets the identifier strategy (defaults to DbGenerated).
        /// </summary>
        public IdentifierStrategy IdentifierStrategy
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