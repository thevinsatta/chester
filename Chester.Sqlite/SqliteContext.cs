using Chester.Interfaces;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;

namespace Chester.Sqlite
{
    public class SqliteContext : DbContext
    {
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
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        protected override IDbTool CreateDbTool(IDbConnection dbConn)
        {
            var dbTool = new SqliteTool(dbConn);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }
        #endregion
    }
}
