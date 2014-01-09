// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="MicroLite">
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
namespace MicroLite.FrameworkExtensions
{
    internal static class StringExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "We're not formatting numeric values and this overload saves the cost of the params array.")]
        internal static string FormatWith(this string value, string arg0)
        {
            return string.Format(value, arg0);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", Justification = "We're not formatting numeric values and this overload saves the cost of the params array.")]
        internal static string FormatWith(this string value, string arg0, string arg1)
        {
            return string.Format(value, arg0, arg1);
        }
    }
}