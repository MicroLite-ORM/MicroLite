// -----------------------------------------------------------------------
// <copyright file="SqlArgument.cs" company="Project Contributors">
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
using System.Data;
using MicroLite.TypeConverters;

namespace MicroLite
{
    /// <summary>
    /// A representation of an object and its DbType in a SqlQuery.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Value: {Value}, DbType: {DbType}")]
    public struct SqlArgument : IEquatable<SqlArgument>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SqlArgument"/> struct.
        /// </summary>
        /// <param name="value">The argument value.</param>
        public SqlArgument(object value)
        {
            Value = value;
            DbType = value != null ? TypeConverter.ResolveDbType(value.GetType()) : default;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlArgument"/> struct.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <param name="dbType">The DbType of the value.</param>
        public SqlArgument(object value, DbType dbType)
        {
            Value = value;
            DbType = dbType;
        }

        /// <summary>
        /// Gets the DbType of the value.
        /// </summary>
        public DbType DbType { get; }

        /// <summary>
        /// Gets the value of the argument.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Checks whether two separate SqlArgument instances are not equal.
        /// </summary>
        /// <param name="sqlArgument1">The SqlArgument to check.</param>
        /// <param name="sqlArgument2">The SqlArgument to check against.</param>
        /// <returns><c>true</c> if the instances are not considered equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(SqlArgument sqlArgument1, SqlArgument sqlArgument2) => !sqlArgument1.Equals(sqlArgument2);

        /// <summary>
        /// Checks whether two separate SqlArgument instances are equal.
        /// </summary>
        /// <param name="sqlArgument1">The SqlArgument to check.</param>
        /// <param name="sqlArgument2">The SqlArgument to check against.</param>
        /// <returns><c>true</c> if the instances are considered equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(SqlArgument sqlArgument1, SqlArgument sqlArgument2) => sqlArgument1.Equals(sqlArgument2);

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as SqlArgument?;

            if (other is null)
            {
                return false;
            }

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="SqlArgument" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SqlArgument"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="SqlArgument" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(SqlArgument other) => DbType == other.DbType && Equals(Value, other.Value);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => ((int)DbType).GetHashCode() ^ (Value ?? string.Empty).GetHashCode();
    }
}
