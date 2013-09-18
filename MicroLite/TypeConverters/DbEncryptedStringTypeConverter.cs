// -----------------------------------------------------------------------
// <copyright file="DbEncryptedStringTypeConverter.cs" company="MicroLite">
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
namespace MicroLite.TypeConverters
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using MicroLite.Infrastructure;

    /// <summary>
    /// An ITypeConverter which can encrypt and decrypt the stored database value.
    /// </summary>
    public sealed class DbEncryptedStringTypeConverter : TypeConverter
    {
        private const char Separator = '@';
        private readonly ISymmetricAlgorithmProvider algorithmProvider;

        /// <summary>
        /// Initialises a new instance of the <see cref="DbEncryptedStringTypeConverter"/> class.
        /// </summary>
        /// <param name="algorithmProvider">The symmetric algorithm provider to be used.</param>
        public DbEncryptedStringTypeConverter(ISymmetricAlgorithmProvider algorithmProvider)
        {
            if (algorithmProvider == null)
            {
                throw new ArgumentNullException("algorithmProvider");
            }

            this.algorithmProvider = algorithmProvider;
        }

        /// <summary>
        /// Determines whether this type converter can convert values for the specified property type.
        /// </summary>
        /// <param name="propertyType">The type of the property value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified property type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type propertyType)
        {
            return propertyType == typeof(DbEncryptedString);
        }

        /// <summary>
        /// Converts the specified database value into an instance of the property type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <param name="propertyType">The property type to convert to.</param>
        /// <returns>
        /// An instance of the specified property type containing the specified value.
        /// </returns>
        public override object ConvertFromDbValue(object value, Type propertyType)
        {
            if (value == DBNull.Value)
            {
                return (DbEncryptedString)null;
            }

            var stringValue = (string)value;

            if (string.IsNullOrEmpty(stringValue))
            {
                return (DbEncryptedString)stringValue;
            }

            return (DbEncryptedString)this.Decrypt(stringValue);
        }

        /// <summary>
        /// Converts the specified property value into an instance of the database value.
        /// </summary>
        /// <param name="value">The property value to be converted.</param>
        /// <param name="propertyType">The property type to convert from.</param>
        /// <returns>
        /// An instance of the corresponding database type for the property type containing the property value.
        /// </returns>
        public override object ConvertToDbValue(object value, Type propertyType)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return value;
            }

            return this.Encrypt(value.ToString());
        }

        private string Decrypt(string cipherText)
        {
            var parts = cipherText.Split(Separator);

            if (parts.Length != 2)
            {
                throw new MicroLiteException(Messages.DbEncryptedStringTypeConverter_CipherTextInvalid);
            }

            byte[] cipherBytes = Convert.FromBase64String(parts[0]);
            byte[] ivBytes = Convert.FromBase64String(parts[1]);

            using (var algorithm = this.algorithmProvider.CreateAlgorithm())
            {
                algorithm.IV = ivBytes;

                var decryptor = algorithm.CreateDecryptor();

                MemoryStream memoryStream = null;
                CryptoStream cryptoStream = null;

                try
                {
                    memoryStream = new MemoryStream(cipherBytes);
                    cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                    memoryStream = null;

                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        cryptoStream = null;

                        return streamReader.ReadToEnd();
                    }
                }
                finally
                {
                    if (memoryStream != null)
                    {
                        memoryStream.Dispose();
                    }

                    if (cryptoStream != null)
                    {
                        cryptoStream.Dispose();
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Ignored for now, it shouldn't blow up and the suggested fix means we can't then access the memory stream!")]
        private string Encrypt(string clearText)
        {
            byte[] cipherBytes = null;
            byte[] ivBytes;

            using (var algorithm = this.algorithmProvider.CreateAlgorithm())
            {
                algorithm.GenerateIV();

                ivBytes = algorithm.IV; // should we use the IV bytes from the previous value if one exists?

                var encryptor = algorithm.CreateEncryptor();

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(clearText);
                        }

                        cipherBytes = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(cipherBytes) + Separator + Convert.ToBase64String(ivBytes);
        }
    }
}