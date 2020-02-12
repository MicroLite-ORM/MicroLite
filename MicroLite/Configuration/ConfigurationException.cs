// -----------------------------------------------------------------------
// <copyright file="ConfigurationException.cs" company="Project Contributors">
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
namespace MicroLite.Configuration
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A <see cref="MicroLiteException"/> which is thrown for configuration exceptions.
    /// </summary>
    [Serializable]
    public class ConfigurationException : MicroLiteException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        public ConfigurationException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}