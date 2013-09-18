// -----------------------------------------------------------------------
// <copyright file="IObjectInfo.cs" company="MicroLite">
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

    /// <summary>
    /// The interface for a class which describes a type and the table it is mapped to.
    /// </summary>
    public interface IObjectInfo
    {
        /// <summary>
        /// Gets an object containing the default value for the type of identifier used by the type.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        object DefaultIdentifierValue
        {
            get;
        }

        /// <summary>
        /// Gets type the object info relates to.
        /// </summary>
        Type ForType
        {
            get;
        }

        /// <summary>
        /// Gets the table info for the type the object info relates to.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        TableInfo TableInfo
        {
            get;
        }

        /// <summary>
        /// Creates a new instance of the type.
        /// </summary>
        /// <returns>A new instance of the type.</returns>
        object CreateInstance();

        /// <summary>
        /// Gets the property value for the object identifier.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <returns>The value of the identifier property.</returns>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        object GetIdentifierValue(object instance);

        /// <summary>
        /// Gets the property value for the specified property on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <param name="propertyName">Name of the property to get the value for.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        object GetPropertyValue(object instance, string propertyName);

        /// <summary>
        /// Gets the property value from the specified instance and converts it to the correct type for the specified column.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <param name="columnName">Name of the column to get the value for.</param>
        /// <returns>The column value of the property.</returns>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        object GetPropertyValueForColumn(object instance, string columnName);

        /// <summary>
        /// Determines whether the specified instance has the default identifier value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if the instance has the default identifier value; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        bool HasDefaultIdentifierValue(object instance);

        /// <summary>
        /// Sets the property value for the specified property on the specified instance to the specified value.
        /// </summary>
        /// <param name="instance">The instance to set the property value on.</param>
        /// <param name="propertyName">Name of the property to set the value for.</param>
        /// <param name="value">The value to be set.</param>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        void SetPropertyValue(object instance, string propertyName, object value);

        /// <summary>
        /// Sets the property value of the property mapped to the specified column after converting it to the correct type for the property.
        /// </summary>
        /// <param name="instance">The instance to set the property value on.</param>
        /// <param name="columnName">The name of the column the property is mapped to.</param>
        /// <param name="value">The value from the database column to set the property to.</param>
        void SetPropertyValueForColumn(object instance, string columnName, object value);
    }
}