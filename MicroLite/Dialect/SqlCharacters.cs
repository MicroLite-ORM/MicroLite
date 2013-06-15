// -----------------------------------------------------------------------
// <copyright file="SqlCharacters.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Dialect
{
    using System.Globalization;

    /// <summary>
    /// A class containing the SQL characters for an SQL Dialect.
    /// </summary>
    public abstract class SqlCharacters
    {
        private static SqlCharacters msSql;
        private static SqlCharacters mySql;
        private static SqlCharacters postgreSql;
        private static SqlCharacters sqlite;

        /// <summary>
        /// Gets the SqlCharacters for MS SQL.
        /// </summary>
        public static SqlCharacters MsSql
        {
            get
            {
                return msSql ?? (msSql = new MsSqlCharacters());
            }
        }

        /// <summary>
        /// Gets the SqlCharacters for MySql.
        /// </summary>
        public static SqlCharacters MySql
        {
            get
            {
                return mySql ?? (mySql = new MySqlCharacters());
            }
        }

        /// <summary>
        /// Gets the SqlCharacters for PostgreSql.
        /// </summary>
        public static SqlCharacters PostgreSql
        {
            get
            {
                return postgreSql ?? (postgreSql = new PostgreSqlCharacters());
            }
        }

        /// <summary>
        /// Gets the SqlCharacters for SQLite.
        /// </summary>
        public static SqlCharacters SQLite
        {
            get
            {
                return sqlite ?? (sqlite = new SQLiteCharacters());
            }
        }

        /// <summary>
        /// Gets the left delimiter character.
        /// </summary>
        public virtual char LeftDelimiter
        {
            get
            {
                return '"';
            }
        }

        /// <summary>
        /// Gets the right delimiter character.
        /// </summary>
        public virtual char RightDelimiter
        {
            get
            {
                return '"';
            }
        }

        /// <summary>
        /// Gets the SQL parameter.
        /// </summary>
        public virtual char SqlParameter
        {
            get
            {
                return '?';
            }
        }

        /// <summary>
        /// Gets the character used to separate SQL statements.
        /// </summary>
        public virtual char StatementSeparator
        {
            get
            {
                return ';';
            }
        }

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        public virtual bool SupportsNamedParameters
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Escapes the specified SQL using the left and right delimiters.
        /// </summary>
        /// <param name="sql">The SQL to be escaped.</param>
        /// <returns>The escaped SQL.</returns>
        public string EscapeSql(string sql)
        {
            return this.LeftDelimiter + sql + this.RightDelimiter;
        }

        /// <summary>
        /// Gets the name of the parameter for the specified position.
        /// </summary>
        /// <param name="position">The parameter position.</param>
        /// <returns>The nape of the parameter for the specified position.</returns>
        public string GetParameterName(int position)
        {
            if (this.SupportsNamedParameters)
            {
                return this.SqlParameter + ('p' + position.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                return this.SqlParameter.ToString();
            }
        }

        private sealed class MsSqlCharacters : SqlCharacters
        {
            public override char LeftDelimiter
            {
                get
                {
                    return '[';
                }
            }

            public override char RightDelimiter
            {
                get
                {
                    return ']';
                }
            }

            public override char SqlParameter
            {
                get
                {
                    return '@';
                }
            }

            public override bool SupportsNamedParameters
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class MySqlCharacters : SqlCharacters
        {
            public override char LeftDelimiter
            {
                get
                {
                    return '`';
                }
            }

            public override char RightDelimiter
            {
                get
                {
                    return '`';
                }
            }

            public override char SqlParameter
            {
                get
                {
                    return '@';
                }
            }

            public override bool SupportsNamedParameters
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class PostgreSqlCharacters : SqlCharacters
        {
            public override char SqlParameter
            {
                get
                {
                    return ':';
                }
            }

            public override bool SupportsNamedParameters
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class SQLiteCharacters : SqlCharacters
        {
            public override char SqlParameter
            {
                get
                {
                    return '@';
                }
            }

            public override bool SupportsNamedParameters
            {
                get
                {
                    return true;
                }
            }
        }
    }
}