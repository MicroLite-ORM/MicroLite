// -----------------------------------------------------------------------
// <copyright file="FirebirdSqlCharacters.cs" company="Project Contributors">
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
namespace MicroLite.Characters
{
    /// <summary>
    /// The implementation of <see cref="SqlCharacters"/> for Firebird.
    /// </summary>
    [System.Serializable]
    internal sealed class FirebirdSqlCharacters : SqlCharacters
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="FirebirdSqlCharacters"/> class from being created.
        /// </summary>
        private FirebirdSqlCharacters()
        {
        }

        /// <summary>
        /// Gets the left delimiter character.
        /// </summary>
        public override string LeftDelimiter => "\"";

        /// <summary>
        /// Gets the right delimiter character.
        /// </summary>
        public override string RightDelimiter => "\"";

        /// <summary>
        /// Gets the SQL parameter.
        /// </summary>
        public override string SqlParameter => "@";

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        public override bool SupportsNamedParameters => true;

        /// <summary>
        /// Gets the single instance of <see cref="SqlCharacters"/> for Firebird.
        /// </summary>
        internal static SqlCharacters Instance { get; } = new FirebirdSqlCharacters();
    }
}
