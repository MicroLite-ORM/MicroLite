// -----------------------------------------------------------------------
// <copyright file="EnglishInflectionService.cs" company="MicroLite">
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
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A class for modifying the grammatical category of an English word.
    /// </summary>
    /// <remarks>Based upon the example here <![CDATA[http://mattgrande.wordpress.com/2009/10/28/pluralization-helper-for-c/]]></remarks>
    internal sealed class EnglishInflectionService : IInflectionService
    {
        private readonly IDictionary<string, string> rules = new Dictionary<string, string>
        {
            { "Person", "People" },
            { "Child", "Children" },
            { "(.*)fe?", "$1ves" },
            { "(.*)man$", "$1men" },
            { "(.+[aeiou]y)$", "$1s" },
            { "(.+[^aeiou])y$", "$1ies" },
            { "(.+z)$", "$1zes" },
            { "([M|L])ouse$", "$1ice" },
            { "(.+)(e|i)x$", @"$1ices" },
            { "(Octop|Vir)us$", "$1i" },
            { "(.+(s|x|sh|ch))$", @"$1es" },
            { "(.+)", @"$1s" }
        };

        private readonly HashSet<string> singularWords = new HashSet<string>
        {
            "Equipment",
            "Information",
            "Money",
            "Species",
            "Series"
        };

        /// <summary>
        /// Adds a word which is considered invariant such as equipment or species.
        /// </summary>
        /// <param name="word">The invariant word.</param>
        public void AddInvariantWord(string word)
        {
            this.singularWords.Add(word);
        }

        /// <summary>
        /// Adds (or replaces) the rule.
        /// </summary>
        /// <param name="searchPattern">The pattern to match upon.</param>
        /// <param name="replacementPattern">The replacement pattern.</param>
        public void AddRule(string searchPattern, string replacementPattern)
        {
            this.rules[searchPattern] = replacementPattern;
        }

        /// <summary>
        /// Returns the plural version of the specified singular word or the specified word if there is no plural version.
        /// </summary>
        /// <param name="word">The word to be pluralized.</param>
        /// <returns>The plural word, or if the word cannot be pluralized; the specified word.</returns>
        public string ToPlural(string word)
        {
            if (this.singularWords.Contains(word))
            {
                return word;
            }

            foreach (var pluralization in this.rules)
            {
                if (Regex.IsMatch(word, pluralization.Key))
                {
                    return Regex.Replace(word, pluralization.Key, pluralization.Value);
                }
            }

            return word;
        }
    }
}