// -----------------------------------------------------------------------
// <copyright file="MsSqlDbDriver.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Driver
{
    using System.Data;
    using MicroLite.Characters;

    /// <summary>
    /// The implementation of <see cref="IDbDriver"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDbDriver" /> class.
        /// </summary>
        internal MsSqlDbDriver()
            : base(MsSqlCharacters.Instance)
        {
        }

        public override bool SupportsBatchedQueries
        {
            get
            {
                return true;
            }
        }

        protected override void SetDbType(IDbDataParameter parameter, DbType dbType)
        {
            if (dbType != DbType.Time)
            {
                // Work around for a bug in SqlClient where it thinks DbType.Time is a MetaType.MetaDateTime.
                // Do not set the DbType, the SqlParameter will figure it out by reflecting over the value type.
                base.SetDbType(parameter, dbType);
            }
        }
    }
}