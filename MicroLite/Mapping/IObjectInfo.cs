// -----------------------------------------------------------------------
// <copyright file="IObjectInfo.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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

    /// <summary>
    /// The interface for a class which describes a type and the table it is mapped to.
    /// </summary>
    public interface IObjectInfo
    {
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
        /// Creates a new instance of the type populated with the values from the specified IDataReader.
        /// </summary>
        /// <param name="reader">The IDataReader containing the values to build the instance from.</param>
        /// <returns>A new instance of the type populated with the values from the specified IDataReader.</returns>
        object CreateInstance(IDataReader reader);

        /// <summary>
        /// Gets the column information for the column with the specified name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The ColumnInfo or null if no column is mapped for the object with the specified name.</returns>
        ColumnInfo GetColumnInfo(string columnName);

        /// <summary>
        /// Gets the property value for the object identifier.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <returns>The value of the identifier property.</returns>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        object GetIdentifierValue(object instance);

        /// <summary>
        /// Gets the insert values for the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An array of values to be used for the insert command.</returns>
        object[] GetInsertValues(object instance);

        /// <summary>
        /// Gets the update values for the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>An array of values to be used for the update command.</returns>
        object[] GetUpdateValues(object instance);

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
        /// Sets the property value for the object identifier to the supplied value.
        /// </summary>
        /// <param name="instance">The instance to set the value for.</param>
        /// <param name="identifier">The value to set as the identifier property.</param>
        /// <exception cref="NotSupportedException">Thrown if the object info does not support Insert, Update or Delete.</exception>
        void SetIdentifierValue(object instance, object identifier);
    }
}