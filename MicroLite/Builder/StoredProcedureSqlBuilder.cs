// -----------------------------------------------------------------------
// <copyright file="StoredProcedureSqlBuilder.cs" company="MicroLite">
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
namespace MicroLite.Builder
{
    using MicroLite.Builder.Syntax;
    using MicroLite.Characters;

    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class StoredProcedureSqlBuilder : SqlBuilderBase, IWithParameter
    {
        internal StoredProcedureSqlBuilder(SqlCharacters sqlCharacters, string procedureName)
            : base(sqlCharacters)
        {
            this.InnerSql.Append(sqlCharacters.StoredProcedureInvocationCommand)
                .Append(' ')
                .Append(procedureName)
                .Append(' ');
        }

        public IWithParameter WithParameter(string parameter, object arg)
        {
            if (this.Arguments.Count > 0)
            {
                this.InnerSql.Append(',');
            }

            this.Arguments.Add(new SqlArgument(arg));

            this.InnerSql.Append(parameter);

            return this;
        }
    }
}