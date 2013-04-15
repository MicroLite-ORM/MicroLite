// -----------------------------------------------------------------------
// <copyright file="AppSettingSymmetricAlgorithmProvider.cs" company="MicroLite">
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
namespace MicroLite.Infrastructure
{
    using System.Configuration;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// An implementation of <see cref="ISymmetricAlgorithmProvider"/> which reads the values to use from the app.config.
    /// </summary>
    public sealed class AppSettingSymmetricAlgorithmProvider : ISymmetricAlgorithmProvider
    {
        private readonly string algorithm;
        private readonly byte[] keyBytes;

        /// <summary>
        /// Initialises a new instance of the <see cref="AppSettingSymmetricAlgorithmProvider"/> class.
        /// </summary>
        /// <exception cref="MicroLiteException">Thrown if the expected configuration values are missing in the app.config.</exception>
        public AppSettingSymmetricAlgorithmProvider()
        {
            var key = ConfigurationManager.AppSettings["MicroLite.DbEncryptedString.EncryptionKey"];
            var algorithm = ConfigurationManager.AppSettings["MicroLite.DbEncryptedString.SymmetricAlgorithm"];

            if (string.IsNullOrEmpty(key))
            {
                throw new MicroLiteException(Messages.ConfigurationSymmetricAlgorithmProvider_MissingKey);
            }

            if (string.IsNullOrEmpty(algorithm))
            {
                throw new MicroLiteException(Messages.ConfigurationSymmetricAlgorithmProvider_MissingAlgorithm);
            }

            this.keyBytes = Encoding.ASCII.GetBytes(key);
            this.algorithm = algorithm;
        }

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
    }
}