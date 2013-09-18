// -----------------------------------------------------------------------
// <copyright file="MemberInfoExtensions.cs" company="MicroLite">
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
namespace MicroLite.FrameworkExtensions
{
    using System;
    using System.Reflection;

    internal static class MemberInfoExtensions
    {
        internal static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit)
            where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), inherit) as T[];

            return attributes != null && attributes.Length == 1 ? attributes[0] : null;
        }
    }
}