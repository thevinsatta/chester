using Microsoft.Data.Sqlite;
using System;
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
        protected override IDbConnection CreateConnection(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw ArgNullOrWhiteSpaceException(nameof(connStr));

            return new SqliteConnection(connStr);
        }

        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new SqliteCommand()
            {
                Connection = (SqliteConnection)dbConn ?? throw new ArgumentNullException(nameof(dbConn))
            };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText)
        {
            if (dbConn == null)
                throw new ArgumentNullException(nameof(dbConn));

            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            return new SqliteCommand(cmdText, (SqliteConnection)dbConn);
        }
        #endregion
    }
}
