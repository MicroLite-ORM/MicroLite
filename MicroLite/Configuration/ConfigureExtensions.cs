// -----------------------------------------------------------------------
// <copyright file="ConfigureExtensions.cs" company="Project Contributors">
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
    /// The class used to configure extensions to the MicroLite ORM framework.
    /// </summary>
    internal sealed class ConfigureExtensions : IConfigureExtensions
    {
        private ILog _log = LogManager.GetCurrentClassLog();

        public void SetLogResolver(Func<Type, ILog> logResolver)
        {
            LogManager.GetLogger = logResolver;

            _log = LogManager.GetCurrentClassLog();
        }

        public void SetMappingConvention(IMappingConvention mappingConvention)
        {
            if (mappingConvention is null)
            {
                throw new ArgumentNullException(nameof(mappingConvention));
            }

            if (_log.IsInfo)
            {
                _log.Info(LogMessages.ConfigureExtensions_UsingMappingConvention, mappingConvention.GetType().FullName);
            }

            ObjectInfo.MappingConvention = mappingConvention;
        }
    }
}
