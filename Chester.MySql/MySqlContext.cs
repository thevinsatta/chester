using Chester.Interfaces;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Chester.MySql
{
    public class MySqlContext : DbContext
    {
        #region Fields
        protected IDictionary<DbType, MySqlDbType> _dbTypeMap =
            new Dictionary<DbType, MySqlDbType>()
            {
                { DbType.AnsiString, MySqlDbType.VarString },
                { DbType.AnsiStringFixedLength, MySqlDbType.String },
                { DbType.Binary, MySqlDbType.VarBinary },
                { DbType.Boolean, MySqlDbType.Bit },
                { DbType.Byte, MySqlDbType.Byte },
                { DbType.Date, MySqlDbType.Date },
                { DbType.DateTime, MySqlDbType.DateTime },
                { DbType.DateTime2, MySqlDbType.DateTime },
                { DbType.Decimal, MySqlDbType.Decimal },
                { DbType.Double, MySqlDbType.Double },
                { DbType.Guid, MySqlDbType.Guid },
                { DbType.Int16, MySqlDbType.Int16 },
                { DbType.Int32, MySqlDbType.Int32 },
                { DbType.Int64, MySqlDbType.Int64 },
                { DbType.Single, MySqlDbType.Float },
                { DbType.String, MySqlDbType.VarString },
                { DbType.StringFixedLength, MySqlDbType.String },
                { DbType.Time, MySqlDbType.Time },
                { DbType.Xml, MySqlDbType.Text }
            };
        #endregion

        #region Constructors
        public MySqlContext(string connStr) : base(connStr) { }

        public MySqlContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new MySqlTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.SetCommandTimeout(CommandTimeout.Value);

            return dbTool;
        }

        public override IDbDataParameter DbParam(string name, object value) =>
            !string.IsNullOrWhiteSpace(name)
                ? new MySqlParameter(name, value)
                : throw ParamNameNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type) =>
            !string.IsNullOrWhiteSpace(name)
                ? new MySqlParameter(name, GetDbType(type))
                : throw ParamNameNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size) =>
            !string.IsNullOrWhiteSpace(name)
                ? new MySqlParameter(name, GetDbType(type), size)
                : throw ParamNameNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size, object value, ParameterDirection direction) =>
            !string.IsNullOrWhiteSpace(name)
                ? new MySqlParameter(name, GetDbType(type), size)
                {
                    Direction = direction,
                    Value = value
                }
                : throw ParamNameNullOrWhiteSpaceException(nameof(name));
        #endregion

        #region Helpers
        public MySqlDbType GetDbType(DbType type) =>
            _dbTypeMap.TryGetValue(type, out MySqlDbType nativeType)
                ? nativeType
                : throw DbTypeNotSupportedException(type, "MySQL");
        #endregion
    }
}
