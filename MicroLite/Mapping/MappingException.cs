// -----------------------------------------------------------------------
// <copyright file="MappingException.cs" company="Project Contributors">
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
using System.Runtime.Serialization;

namespace MicroLite.Mapping
{
    /// <summary>
    /// A more specific MicroLiteException which is only thrown for mapping exceptions.
    /// </summary>
    [Serializable]
    public class MappingException : MicroLiteException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        public MappingException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MappingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MappingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MappingException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="SerializationException">The class name is null or HResult is zero (0). </exception>
        protected MappingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
