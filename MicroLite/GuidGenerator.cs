// -----------------------------------------------------------------------
// <copyright file="GuidGenerator.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Threading;

    /// <summary>
    /// A class for generating guids using our comb implementation.
    /// </summary>
    /// <remarks>
    /// Loosely based upon Mark J. Miller's implementation <![CDATA[http://www.developmentalmadness.com/archive/2010/10/13/sequential-guid-algorithm-ndash-improving-the-algorithm.aspx]]>
    /// </remarks>
    internal static class GuidGenerator
    {
        private static readonly NetworkInterfaceType[] ignoredInterfaceTypes = new[] { NetworkInterfaceType.Loopback, NetworkInterfaceType.Tunnel };

        // This call is quite slow so we do it once.
        private static readonly byte[] nicBytes = NetworkInterface
            .GetAllNetworkInterfaces()
            .Where(ix => !ignoredInterfaceTypes.Contains(ix.NetworkInterfaceType))
            .OrderBy(ix => ix.OperationalStatus)
            .ThenByDescending(ix => ix.Speed)
            .First()
            .GetPhysicalAddress()
            .GetAddressBytes();

        private static long sequentialCounter = 0;

        /// <summary>
        /// Creates a new Guid using our comb implementation.
        /// </summary>
        /// <returns>A new Guid.</returns>
        internal static Guid CreateComb()
        {
            return CreateComb(DateTime.UtcNow);
        }

        /// <summary>
        /// Creates a new Guid using our comb implementation.
        /// </summary>
        /// <param name="dateTime">The date time to use as the seed for the guid.</param>
        /// <returns>A new Guid</returns>
        internal static Guid CreateComb(DateTime dateTime)
        {
            var increment = Interlocked.Increment(ref sequentialCounter);

            byte[] tickBytes = BitConverter.GetBytes(dateTime.Ticks + increment);

            var guidBytes = new byte[16];
            guidBytes[0] = (byte)dateTime.Month;
            guidBytes[1] = (byte)dateTime.Day;
            guidBytes[2] = (byte)dateTime.Hour;
            guidBytes[3] = (byte)dateTime.Minute;
            guidBytes[4] = (byte)dateTime.Second;
            guidBytes[5] = nicBytes[3];
            guidBytes[6] = nicBytes[4];
            guidBytes[7] = nicBytes[5];
            guidBytes[8] = tickBytes[1];
            guidBytes[9] = tickBytes[0];
            guidBytes[10] = tickBytes[7];
            guidBytes[11] = tickBytes[6];
            guidBytes[12] = tickBytes[5];
            guidBytes[13] = tickBytes[4];
            guidBytes[14] = tickBytes[3];
            guidBytes[15] = tickBytes[2];

            return new Guid(guidBytes);
        }
    }
}