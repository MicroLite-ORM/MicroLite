// -----------------------------------------------------------------------
// <copyright file="Characters.cs" company="MicroLite">
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
namespace MicroLite
{
    /// <summary>
    /// Static character arrays for method calls (String.Split etc)
    /// </summary>
    internal static class Characters
    {
        private static readonly char[] at = new[] { '@' };
        private static readonly char[] period = new[] { '.' };

        /// <summary>
        /// Gets a character array containing the at character.
        /// </summary>
        internal static char[] At
        {
            get
            {
                return at;
            }
        }

        /// <summary>
        /// Gets a character array containing the period character.
        /// </summary>
        internal static char[] Period
        {
            get
            {
                return period;
            }
        }
    }
}