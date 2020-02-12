// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Project Contributors">
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
using System.Text.RegularExpressions;

namespace MicroLite.FrameworkExtensions
{
    internal static class StringExtensions
    {
        internal static string FormatWith(this string value, string arg0) => string.Format(value, arg0);

        internal static string FormatWith(this string value, string arg0, string arg1) => string.Format(value, arg0, arg1);

        internal static string ToUnderscored(this string value) => Regex.Replace(value, "(?!^)(?=[A-Z])", "_");
    }
}
