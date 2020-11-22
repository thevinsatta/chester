using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Chester.Sqlite
{
    public class SqliteTool : DbTool
    {
        #region Constructors
        public SqliteTool(string connStr) : base(connStr) { }

        public SqliteTool(SqliteConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override DbCommand CreateCommand(DbConnection dbConn) =>
            new SqliteCommand() { Connection = (SqliteConnection)dbConn };

        protected override DbCommand CreateCommand(DbConnection dbConn, string cmdText) =>
            new SqliteCommand(cmdText, (SqliteConnection)dbConn);

        protected override DbConnection CreateConnection(string connStr) =>
            new SqliteConnection(connStr);
        #endregion
    }
}
