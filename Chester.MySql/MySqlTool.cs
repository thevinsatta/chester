using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Chester.MySql
{
    public class MySqlTool : DbTool
    {
        #region Constructors
        public MySqlTool(string connStr) : base(connStr) { }

        public MySqlTool(MySqlConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override DbCommand CreateCommand(DbConnection dbConn) =>
            new MySqlCommand() { Connection = (MySqlConnection)dbConn };

        protected override DbCommand CreateCommand(DbConnection dbConn, string cmdText) =>
            new MySqlCommand(cmdText, (MySqlConnection)dbConn);

        protected override DbConnection CreateConnection(string connStr) =>
            new MySqlConnection(connStr);
        #endregion
    }
}
