using Microsoft.Data.Sqlite;
using System.Data;

namespace Chester.Sqlite
{
    public class SqliteTool : DbTool
    {
        #region Constructors
        public SqliteTool(string connStr) : base(connStr) { }

        public SqliteTool(SqliteConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new SqliteCommand() { Connection = (SqliteConnection)dbConn };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText) =>
            new SqliteCommand(cmdText, (SqliteConnection)dbConn);

        protected override IDbConnection CreateConnection(string connStr) =>
            new SqliteConnection(connStr);
        #endregion
    }
}
