// -----------------------------------------------------------------------
// <copyright file="AppSettingSymmetricAlgorithmProvider.cs" company="MicroLite">
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
namespace MicroLite.Infrastructure
{
    using System.Configuration;
    using System.Text;

    /// <summary>
    /// An implementation of <see cref="ISymmetricAlgorithmProvider"/> which reads the values to use from the app.config.
    /// </summary>
    public sealed class AppSettingSymmetricAlgorithmProvider : SymmetricAlgorithmProvider
    {
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
                throw new MicroLiteException(Messages.AppSettingSymmetricAlgorithmProvider_MissingKey);
            }

            if (string.IsNullOrEmpty(algorithm))
            {
                throw new MicroLiteException(Messages.AppSettingSymmetricAlgorithmProvider_MissingAlgorithm);
            }

            this.Configure(algorithm, Encoding.ASCII.GetBytes(key));
        }
    }
}