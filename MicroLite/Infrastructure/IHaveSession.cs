﻿// -----------------------------------------------------------------------
// <copyright file="IHaveSession.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Infrastructure
{
    /// <summary>
    /// An interface for classes which have an <see cref="ISession"/> property.
    /// </summary>
    public interface IHaveSession
    {
        /// <summary>
        /// The MicroLite session.
        /// </summary>
        ISession Session
        {
            get;
            set;
        }
    }
}