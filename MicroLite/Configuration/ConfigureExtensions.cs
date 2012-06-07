// -----------------------------------------------------------------------
// <copyright file="ConfigureExtensions.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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

    /// <summary>
    /// The class used to configure extensions to the MicroLite ORM framework.
    /// </summary>
    internal class ConfigureExtensions : IConfigureExtensions
    {
        public void SetLogResolver(Func<string, ILog> logResolver)
        {
            LogManager.GetLogger = logResolver;
        }
    }
}