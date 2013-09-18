// -----------------------------------------------------------------------
// <copyright file="MicroLiteException.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The base exception thrown by the MicroLite ORM framework.
    /// </summary>
    /// <remarks>
    /// This exception will be thrown for exceptions encountered by the MicroLite ORM framework
    /// or to wrap any exceptions thrown by .net framework classes to allow for consistent error handling.
    /// </remarks>
    [Serializable]
    public class MicroLiteException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteException"/> class.
        /// </summary>
        public MicroLiteException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MicroLiteException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MicroLiteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected MicroLiteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}