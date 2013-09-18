// -----------------------------------------------------------------------
// <copyright file="ConfigureExtensions.cs" company="MicroLite">
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
namespace MicroLite.Configuration
{
    using System;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The class used to configure extensions to the MicroLite ORM framework.
    /// </summary>
    internal sealed class ConfigureExtensions : IConfigureExtensions
    {
        private ILog log;

        public void SetLogResolver(Func<string, ILog> logResolver)
        {
            LogManager.GetLogger = logResolver;

            this.log = LogManager.GetCurrentClassLog();
            this.log.TryLogInfo(Messages.ConfigureExtensions_UsingLogger, logResolver != null && logResolver.Method != null ? logResolver.Method.ToString() : string.Empty);
        }

        public void SetMappingConvention(IMappingConvention mappingConvention)
        {
            if (mappingConvention == null)
            {
                throw new ArgumentNullException("mappingConvention");
            }

            this.log.TryLogInfo(Messages.ConfigureExtensions_UsingMappingConvention, mappingConvention.GetType().FullName);
            ObjectInfo.MappingConvention = mappingConvention;
        }
    }
}