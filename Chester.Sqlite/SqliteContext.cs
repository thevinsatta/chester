using Chester.Interfaces;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;

namespace Chester.Sqlite
{
    public class SqliteContext : DbContext
    {
        #region Fields
        protected IDictionary<DbType, SqliteType> _dbTypeMap =
            new Dictionary<DbType, SqliteType>()
            {
                { DbType.AnsiString, SqliteType.Text },
                { DbType.AnsiStringFixedLength, SqliteType.Text },
                { DbType.Binary, SqliteType.Blob},
                { DbType.Boolean, SqliteType.Integer },
                { DbType.Byte, SqliteType.Integer },
                { DbType.Date, SqliteType.Text },
                { DbType.DateTime, SqliteType.Text },
                { DbType.DateTime2, SqliteType.Text },
                { DbType.DateTimeOffset, SqliteType.Text },
                { DbType.Decimal, SqliteType.Real },
                { DbType.Double, SqliteType.Real },
                { DbType.Guid, SqliteType.Text },
                { DbType.Int16, SqliteType.Integer},
                { DbType.Int32, SqliteType.Integer },
                { DbType.Int64, SqliteType.Integer },
                { DbType.Single, SqliteType.Real },
                { DbType.String, SqliteType.Text },
                { DbType.StringFixedLength, SqliteType.Text },
                { DbType.Time, SqliteType.Text },
                { DbType.Xml, SqliteType.Text }
            };
        #endregion

        #region Properties
        public override CommandType DefaultCommandType => CommandType.Text;
        #endregion

        #region Constructors
        public SqliteContext(string connStr) : base(connStr) { }

        public SqliteContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new SqliteTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.SetCommandTimeout(CommandTimeout.Value);

            return dbTool;
        }

        public override IDbDataParameter DbParam(string name, object value) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqliteParameter(name, value)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqliteParameter(name, GetDbType(type))
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqliteParameter(name, GetDbType(type), size)
                : throw ArgNullOrWhiteSpaceException(nameof(name));

        public override IDbDataParameter DbParam(string name, DbType type, int size, object value,
            ParameterDirection direction) =>
            !string.IsNullOrWhiteSpace(name)
                ? new SqliteParameter(name, GetDbType(type), size)
                {
                    Direction = direction,
                    Value = value
                }
                : throw ArgNullOrWhiteSpaceException(nameof(name));
        #endregion

        #region Helpers
        public SqliteType GetDbType(DbType type) =>
            _dbTypeMap.TryGetValue(type, out SqliteType nativeType)
                ? nativeType
                : throw DbTypeNotSupportedException(type, "Sqlite");
        #endregion
    }
}
