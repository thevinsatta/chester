using Chester.Interfaces;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Chester.SqlServer
{
    public class SqlContext : DbContext
    {
        #region Fields
        protected IDictionary<DbType, SqlDbType> _dbTypeMap =
            new Dictionary<DbType, SqlDbType>()
            {
                { DbType.AnsiString, SqlDbType.VarChar },
                { DbType.AnsiStringFixedLength, SqlDbType.Char },
                { DbType.Binary, SqlDbType.VarBinary },
                { DbType.Boolean, SqlDbType.Bit },
                { DbType.Byte, SqlDbType.TinyInt },
                { DbType.Date, SqlDbType.Date },
                { DbType.DateTime, SqlDbType.DateTime },
                { DbType.DateTime2, SqlDbType.DateTime2 },
                { DbType.DateTimeOffset, SqlDbType.DateTimeOffset },
                { DbType.Decimal, SqlDbType.Decimal },
                { DbType.Double, SqlDbType.Float },
                { DbType.Guid, SqlDbType.UniqueIdentifier },
                { DbType.Int16, SqlDbType.SmallInt },
                { DbType.Int32, SqlDbType.Int },
                { DbType.Int64, SqlDbType.BigInt },
                { DbType.Single, SqlDbType.Real },
                { DbType.String, SqlDbType.NVarChar },
                { DbType.StringFixedLength, SqlDbType.NChar },
                { DbType.Time, SqlDbType.Time },
                { DbType.Xml, SqlDbType.Xml }
            };
        #endregion

        #region Constructors
        public SqlContext(string connStr) : base(connStr) { }

        public SqlContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new SqlTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        protected override IDbTool CreateDbTool(IDbConnection dbConn)
        {
            var dbTool = new SqlTool(dbConn);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        public override IDbDataParameter DbParam(string name, object value) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqlParameter(name, value)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqlParameter(name, GetDbType(type))
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqlParameter(name, GetDbType(type), size)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size, object value, ParameterDirection direction) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqlParameter(name, GetDbType(type), size)
                {
                    Direction = direction,
                    Value = value
                }
                : throw ArgNullOrWhiteSpaceException(nameof(name));
        #endregion

        #region Helpers
        public SqlDbType GetDbType(DbType type) =>
            _dbTypeMap.TryGetValue(type, out SqlDbType nativeType)
                ? nativeType
                : throw DbTypeNotSupportedException(type, "SQL Server");
        #endregion
    }
}
