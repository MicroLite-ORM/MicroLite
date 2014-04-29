// -----------------------------------------------------------------------
// <copyright file="ISymmetricAlgorithmProvider.cs" company="MicroLite">
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
namespace MicroLite.Infrastructure
{
    using System.Security.Cryptography;

    /// <summary>
    /// The interface for a class which can provide an <see cref="SymmetricAlgorithm"/>.
    /// </summary>
    public interface ISymmetricAlgorithmProvider
    {
        /// <summary>
        /// Creates an instance of the symmetric algorithm to be used for encryption and decryption.
        /// </summary>
        /// <returns>An instance of the required symmetric algorithm.</returns>
        SymmetricAlgorithm CreateAlgorithm();
    }
}