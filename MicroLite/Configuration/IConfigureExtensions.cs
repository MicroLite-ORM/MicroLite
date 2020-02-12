// -----------------------------------------------------------------------
// <copyright file="IConfigureExtensions.cs" company="Project Contributors">
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
using System;
using MicroLite.Logging;
using MicroLite.Mapping;

namespace MicroLite.Configuration
{
    /// <summary>
    /// The interface which specifies the options for configuring extensions to the MicroLite ORM framework.
    /// </summary>
    public interface IConfigureExtensions : IHideObjectMethods
    {
        /// <summary>
        /// Sets the function which can be called by MicroLite to resolve the <see cref="ILog"/> to use.
        /// </summary>
        /// <param name="logResolver">The function to resolve an ILog.</param>
        void SetLogResolver(Func<Type, ILog> logResolver);

        /// <summary>
        /// Specifies the mapping convention which should be used by MicroLite ORM to map classes to tables.
        /// </summary>
        /// <param name="mappingConvention">The mapping convention to use.</param>
        /// <exception cref="ArgumentNullException">Thrown if mappingConvention is null.</exception>
        void SetMappingConvention(IMappingConvention mappingConvention);
    }
}
