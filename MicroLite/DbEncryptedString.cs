// -----------------------------------------------------------------------
// <copyright file="DbEncryptedString.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite
{
    using System;

    /// <summary>
    /// A class which represents a string which is encrypted before being written to the database
    /// and decrypted after being read from the database.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{value}")]
    public sealed class DbEncryptedString : IEquatable<DbEncryptedString>, IEquatable<string>
    {
        private readonly string value;

        private DbEncryptedString(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Returns a DbEncryptedString containing the value of the specified string.
        /// </summary>
        /// <param name="value">The string to convert value.</param>
        /// <returns>A DbEncryptedString containing the value of the specified string.</returns>
        public static implicit operator DbEncryptedString(string value)
        {
            return value == null ? null : new DbEncryptedString(value);
        }

        /// <summary>
        /// Returns a string containing the value of the DbEncryptedString.
        /// </summary>
        /// <param name="dbEncryptedString">The db encrypted string.</param>
        /// <returns>A string containing the value of the DbEncryptedString.</returns>
        public static implicit operator string(DbEncryptedString dbEncryptedString)
        {
            return dbEncryptedString == null ? null : dbEncryptedString.value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var dbEncryptedString = obj as DbEncryptedString;

            if (dbEncryptedString != null)
            {
                return this.Equals(dbEncryptedString);
            }

            return this.Equals(obj as string);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DbEncryptedString other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Equals(other.value);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(string other)
        {
            if (other == null)
            {
                return false;
            }

            return this.value == other;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.value;
        }
    }
}