using Microsoft.Data.Sqlite;
using System.Data;

namespace Chester.Sqlite
{
    public class SqliteTool : DbTool
    {
        #region Constructors
        public SqliteTool(string connStr) : base(connStr) { }

        public SqliteTool(IDbConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbConnection CreateConnection() =>
            new SqliteConnection();
        #endregion
    }
}
