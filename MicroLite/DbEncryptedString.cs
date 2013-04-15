// -----------------------------------------------------------------------
// <copyright file="DbEncryptedString.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    /// <summary>
    /// A class which represents a string which is encrypted before being written to the database
    /// and decrypted after being read from the database.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{value}")]
    public sealed class DbEncryptedString
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
            return new DbEncryptedString(value);
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