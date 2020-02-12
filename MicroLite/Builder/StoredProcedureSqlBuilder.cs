// -----------------------------------------------------------------------
// <copyright file="StoredProcedureSqlBuilder.cs" company="Project Contributors">
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
using MicroLite.Builder.Syntax;
using MicroLite.Characters;

namespace MicroLite.Builder
{
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
