// -----------------------------------------------------------------------
// <copyright file="MsSqlCharacters.cs" company="MicroLite">
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
namespace MicroLite.Dialect.MsSql
{
    /// <summary>
    /// An implementation of SqlCharacters for MsSql.
    /// </summary>
    internal sealed class MsSqlCharacters : SqlCharacters
    {
        /// <summary>
        /// The single instance of SqlCharacters for MsSql.
        /// </summary>
        internal static readonly SqlCharacters Instance = new MsSqlCharacters();

        /// <summary>
        /// Prevents a default instance of the <see cref="MsSqlCharacters"/> class from being created.
        /// </summary>
        private MsSqlCharacters()
        {
        }

        /// <summary>
        /// Gets the left delimiter character.
        /// </summary>
        public override string LeftDelimiter
        {
            get
            {
                return "[";
            }
        }

        /// <summary>
        /// Gets the right delimiter character.
        /// </summary>
        public override string RightDelimiter
        {
            get
            {
                return "]";
            }
        }

        /// <summary>
        /// Gets the SQL parameter.
        /// </summary>
        public override string SqlParameter
        {
            get
            {
                return "@";
            }
        }

        /// <summary>
        /// Gets the stored procedure invocation command.
        /// </summary>
        public override string StoredProcedureInvocationCommand
        {
            get
            {
                return "EXEC";
            }
        }

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        public override bool SupportsNamedParameters
        {
            get
            {
                return true;
            }
        }
    }
}