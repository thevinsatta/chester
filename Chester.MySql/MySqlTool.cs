using MySql.Data.MySqlClient;
using System.Data;

namespace Chester.MySql
{
    public class MySqlTool : DbTool
    {
        #region Constructors
        public MySqlTool(string connStr) : base(connStr) { }

        public MySqlTool(MySqlConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new MySqlCommand() { Connection = (MySqlConnection)dbConn };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText) =>
            new MySqlCommand(cmdText, (MySqlConnection)dbConn);

        protected override IDbConnection CreateConnection(string connStr) =>
            new MySqlConnection(connStr);
        #endregion
    }
}
