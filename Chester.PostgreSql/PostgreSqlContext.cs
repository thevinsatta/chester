using Chester.Interfaces;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;

namespace Chester.PostgreSql
{
    public class PostgreSqlContext : DbContext
    {
        #region Fields
        protected IDictionary<DbType, NpgsqlDbType> _dbTypeMap =
            new Dictionary<DbType, NpgsqlDbType>()
            {
                { DbType.AnsiString, NpgsqlDbType.Varchar },
                { DbType.AnsiStringFixedLength, NpgsqlDbType.Char },
                { DbType.Binary, NpgsqlDbType.Bytea },
                { DbType.Boolean, NpgsqlDbType.Bit },
                { DbType.Byte, NpgsqlDbType.Smallint },
                { DbType.Date, NpgsqlDbType.Date },
                { DbType.DateTime, NpgsqlDbType.Timestamp },
                { DbType.DateTime2, NpgsqlDbType.Timestamp },
                { DbType.DateTimeOffset, NpgsqlDbType.TimestampTz },
                { DbType.Decimal, NpgsqlDbType.Numeric },
                { DbType.Double, NpgsqlDbType.Double },
                { DbType.Guid, NpgsqlDbType.Uuid },
                { DbType.Int16, NpgsqlDbType.Smallint },
                { DbType.Int32, NpgsqlDbType.Integer },
                { DbType.Int64, NpgsqlDbType.Bigint },
                { DbType.Single, NpgsqlDbType.Real },
                { DbType.String, NpgsqlDbType.Varchar },
                { DbType.StringFixedLength, NpgsqlDbType.Char },
                { DbType.Time, NpgsqlDbType.Time },
                { DbType.Xml, NpgsqlDbType.Xml }
            };
        #endregion

        #region Constructors
        public PostgreSqlContext(string connStr) : base(connStr) { }

        public PostgreSqlContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new PostgreSqlTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.SetCommandTimeout(CommandTimeout.Value);

            return dbTool;
        }

        public override IDbDataParameter DbParam(string name, object value) =>
            !string.IsNullOrWhiteSpace(name)
                ? new NpgsqlParameter(name, value)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type) =>
            !string.IsNullOrWhiteSpace(name)
                ? new NpgsqlParameter(name, GetDbType(type))
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size) =>
            !string.IsNullOrWhiteSpace(name)
                ? new NpgsqlParameter(name, GetDbType(type), size)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size, object value, ParameterDirection direction) =>
            !string.IsNullOrWhiteSpace(name)
                ? new NpgsqlParameter(name, GetDbType(type), size)
                {
                    Direction = direction,
                    Value = value
                }
                : throw ArgNullOrWhiteSpaceException(nameof(name));
        #endregion

        #region Helpers
        public NpgsqlDbType GetDbType(DbType type) =>
            _dbTypeMap.TryGetValue(type, out NpgsqlDbType nativeType)
                ? nativeType
                : throw DbTypeNotSupportedException(type, "PostgreSQL");
        #endregion
    }
}
