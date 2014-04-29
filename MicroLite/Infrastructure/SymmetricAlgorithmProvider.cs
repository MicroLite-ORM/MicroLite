// -----------------------------------------------------------------------
// <copyright file="SymmetricAlgorithmProvider.cs" company="MicroLite">
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
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// A base class for ISymmetricAlgorithmProvider implementations.
    /// </summary>
    public abstract class SymmetricAlgorithmProvider : ISymmetricAlgorithmProvider
    {
        private string algorithm;
        private byte[] keyBytes;

        /// <summary>
        /// Creates an instance of the symmetric algorithm to be used for encryption and decryption.
        /// </summary>
        /// <returns>
        /// An instance of the required symmetric algorithm.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is a factory method, the caller is responsible for disposal of the object.")]
        public SymmetricAlgorithm CreateAlgorithm()
        {
            var symmetricAlgorithm = SymmetricAlgorithm.Create(this.algorithm);
            symmetricAlgorithm.Key = this.keyBytes;

            return symmetricAlgorithm;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SymmetricAlgorithmProvider"/> class.
        /// </summary>
        /// <param name="algorithmName">The algorithm name.</param>
        /// <param name="algorithmKey">The key bytes.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if algorithmName or algorithmKey is null.
        /// </exception>
        protected void Configure(string algorithmName, byte[] algorithmKey)
        {
            if (string.IsNullOrEmpty(algorithmName))
            {
                throw new ArgumentNullException("algorithmName");
            }

            if (algorithmKey == null)
            {
                throw new ArgumentNullException("algorithmKey");
            }

            this.algorithm = algorithmName;
            this.keyBytes = algorithmKey;
        }
    }
}