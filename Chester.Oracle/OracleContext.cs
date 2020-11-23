using Chester.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace Chester.Oracle
{
    public class OracleContext : DbContext
    {
        #region Fields
        protected IDictionary<DbType, OracleDbType> _dbTypeMap =
            new Dictionary<DbType, OracleDbType>()
            {
                { DbType.AnsiString, OracleDbType.Varchar2 },
                { DbType.AnsiStringFixedLength, OracleDbType.Char },
                { DbType.Binary, OracleDbType.Raw },
                { DbType.Boolean, OracleDbType.Byte }, // assuming BYTE(1)
                { DbType.Byte, OracleDbType.Byte },
                { DbType.Date, OracleDbType.Date },
                { DbType.DateTime, OracleDbType.TimeStamp },
                { DbType.DateTime2, OracleDbType.TimeStamp },
                { DbType.DateTimeOffset, OracleDbType.TimeStampTZ },
                { DbType.Decimal, OracleDbType.Decimal },
                { DbType.Double, OracleDbType.Double },
                { DbType.Guid, OracleDbType.Raw }, // assuming RAW(16)
                { DbType.Int16, OracleDbType.Int16 },
                { DbType.Int32, OracleDbType.Int32 },
                { DbType.Int64, OracleDbType.Int64 },
                { DbType.Single, OracleDbType.Single },
                { DbType.String, OracleDbType.NVarchar2 },
                { DbType.StringFixedLength, OracleDbType.NChar },
                { DbType.Time, OracleDbType.TimeStamp },
                { DbType.Xml, OracleDbType.XmlType }
            };
        #endregion

        #region Constructors
        public OracleContext(string connStr) : base(connStr) { }

        public OracleContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new OracleTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.SetCommandTimeout(CommandTimeout.Value);

            return dbTool;
        }

        public override IDbDataParameter DbParam(string name, object value) =>
            !string.IsNullOrWhiteSpace(name)
                ? new OracleParameter(name, value)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type) =>
            !string.IsNullOrWhiteSpace(name)
                ? new OracleParameter(name, GetDbType(type))
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size) =>
            !string.IsNullOrWhiteSpace(name)
                ? new OracleParameter(name, GetDbType(type), size)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size, object value, ParameterDirection direction) =>
            !string.IsNullOrWhiteSpace(name)
                ? new OracleParameter(name, GetDbType(type), size)
                {
                    Direction = direction,
                    Value = value
                }
                : throw ArgNullOrWhiteSpaceException(nameof(name));
        #endregion

        #region Helpers
        public OracleDbType GetDbType(DbType type) =>
            _dbTypeMap.TryGetValue(type, out OracleDbType nativeType)
                ? nativeType
                : throw DbTypeNotSupportedException(type, "Oracle");
        #endregion
    }
}
