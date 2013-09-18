// -----------------------------------------------------------------------
// <copyright file="IInflectionService.cs" company="MicroLite">
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
namespace MicroLite.Mapping.Inflection
{
    /// <summary>
    /// The interface for a class which is capable of modifying the grammatical category of an English word.
    /// </summary>
    public interface IInflectionService
    {
        /// <summary>
        /// Adds a word which is considered invariant, for example 'equipment' or 'species' in English.
        /// </summary>
        /// <param name="word">The invariant word.</param>
        void AddInvariantWord(string word);

        /// <summary>
        /// Adds (or replaces) the rule.
        /// </summary>
        /// <param name="searchPattern">The pattern to match upon.</param>
        /// <param name="replacementPattern">The replacement pattern.</param>
        void AddRule(string searchPattern, string replacementPattern);

        /// <summary>
        /// Returns the plural version of the specified singular word or the specified word if there is no plural version.
        /// </summary>
        /// <param name="word">The word to be pluralized.</param>
        /// <returns>The plural word, or if the word cannot be pluralized; the specified word.</returns>
        string ToPlural(string word);
    }
}