// -----------------------------------------------------------------------
// <copyright file="ConventionMappingSettings.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    using System.Data;
    using System.Reflection;
    using MicroLite.Mapping.Inflection;
    using MicroLite.TypeConverters;

    /// <summary>
    /// A class containing the default convention mapping settings
    /// </summary>
    public class ConventionMappingSettings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ConventionMappingSettings" /> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Acceptable in this instance, it's still perfectly readable")]
        public ConventionMappingSettings()
        {
            this.AllowInsert = (PropertyInfo propertyInfo) => true;
            this.AllowUpdate = (PropertyInfo propertyInfo) => true;
            this.Ignore = (PropertyInfo propertyInfo) => false;
            this.InflectionService = Inflection.InflectionService.English;
            this.IsIdentifier = (PropertyInfo propertyInfo) =>
            {
                return propertyInfo.Name == "Id" || propertyInfo.Name == propertyInfo.ReflectedType.Name + "Id";
            };
            this.ResolveColumnName = (PropertyInfo propertyInfo) => GetColumnName(propertyInfo);
            this.ResolveDbType = (PropertyInfo propertyInfo) => TypeConverter.ResolveDbType(propertyInfo.PropertyType);
            this.ResolveIdentifierColumnName = (PropertyInfo propertyInfo) => propertyInfo.Name;
            this.ResolveIdentifierStrategy = (Type type) => IdentifierStrategy.DbGenerated;
            this.ResolveSequenceName = (PropertyInfo propertyInfo) => null;
            this.ResolveTableName = (Type type) =>
            {
                return this.UsePluralClassNameForTableName ? this.InflectionService.ToPlural(GetTableName(type)) : GetTableName(type);
            };
            this.ResolveTableSchema = (Type type) => null;
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
        /// Gets an instance of the settings with lowercase names with underscore separators (e.g. 'CreditCards' -> 'credit_cards').
        /// </summary>
        public static ConventionMappingSettings LowercaseWithUnderscores
        {
            get
            {
                return new LowercaseWithUnderscoresConventionMappingSettings();
            }
        }

        /// <summary>
        /// Gets an instance of the settings with uppercase names with underscore separators (e.g. 'CreditCards' -> 'CREDIT_CARDS').
        /// </summary>
        public static ConventionMappingSettings UppercaseWithUnderscores
        {
            get
            {
                return new UppercaseWithUnderscoresConventionMappingSettings();
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
        /// <remarks>Only called if the call to IsIdentifier returns false.</remarks>
        public Func<PropertyInfo, string> ResolveColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which resolves the DbType for the column.
        /// </summary>
        public Func<PropertyInfo, DbType> ResolveDbType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines the name of the identifier column for the table (default returns the property name).
        /// </summary>
        /// <remarks>Only called if the call to IsIdentifier returns true.</remarks>
        public Func<PropertyInfo, string> ResolveIdentifierColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which specifies the identifier strategy for a class (returns DbGenerated by default).
        /// </summary>
        public Func<Type, IdentifierStrategy> ResolveIdentifierStrategy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines the name of the sequence used to generate the identifier value (default returns null).
        /// </summary>
        /// <remarks>Only called if the call to ResolveIdentifierStrategy returns <see cref="IdentifierStrategy"/>.Sequence.</remarks>
        public Func<PropertyInfo, string> ResolveSequenceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines the name of the table the class is mapped to (default returns the plural version of the type name using the specified inflection service, or the type name if UsePluralClassNameForTableName is false).
        /// </summary>
        public Func<Type, string> ResolveTableName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the function which determines the schema of the table the class is mapped to (default returns null).
        /// </summary>
        public Func<Type, string> ResolveTableSchema
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

        /// <summary>
        /// Gets the column name for the specified <see cref="PropertyInfo"/> using the default convention settings.
        /// </summary>
        /// <param name="propertyInfo">The property info for the property.</param>
        /// <returns>The column name for the property.</returns>
        protected static string GetColumnName(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsEnum)
            {
                return propertyInfo.PropertyType.Name + "Id";
            }

            return propertyInfo.Name;
        }

        /// <summary>
        /// Gets the table name for the specified <see cref="Type"/> using the default contention settings.
        /// </summary>
        /// <param name="type">The type for the class.</param>
        /// <returns>The table name for the type.</returns>
        protected static string GetTableName(Type type)
        {
            if (type.IsGenericType)
            {
                return type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.OrdinalIgnoreCase));
            }

            return type.Name;
        }
    }
}