namespace MicroLite.Tests.TestEntities
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class MockDbDataReaderWrapper : DbDataReader
    {
        private readonly IDataReader dataReader;

        internal MockDbDataReaderWrapper(IDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        public override int Depth
        {
            get
            {
                return this.dataReader.Depth;
            }
        }

        public override int FieldCount
        {
            get
            {
                return this.dataReader.FieldCount;
            }
        }

        public override bool HasRows
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsClosed
        {
            get
            {
                return this.dataReader.IsClosed;
            }
        }

        public override int RecordsAffected
        {
            get
            {
                return this.dataReader.RecordsAffected;
            }
        }

        public override object this[string name]
        {
            get
            {
                return this.dataReader[name];
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return this.dataReader[ordinal];
            }
        }

        public override void Close()
        {
            this.dataReader.Close();
        }

        public override bool GetBoolean(int ordinal)
        {
            return this.dataReader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return this.dataReader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return this.dataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return this.dataReader.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return this.dataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return this.dataReader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return this.dataReader.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return this.dataReader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return this.dataReader.GetDouble(ordinal);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.dataReader.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return this.dataReader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return this.dataReader.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return this.dataReader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return this.dataReader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return this.dataReader.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return this.dataReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return this.dataReader.GetOrdinal(name);
        }

        public override DataTable GetSchemaTable()
        {
            return this.dataReader.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            return this.dataReader.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return this.dataReader.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return this.dataReader.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.dataReader.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return this.dataReader.NextResult();
        }

        public override bool Read()
        {
            return this.dataReader.Read();
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.dataReader.Read());
        }

        protected override void Dispose(bool disposing)
        {
            this.dataReader.Dispose();
            base.Dispose(disposing);
        }
    }
}