using MySql.Data.MySqlClient;
using System;
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
        protected override IDbConnection CreateConnection(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw ArgNullOrWhiteSpaceException(nameof(connStr));

            return new MySqlConnection(connStr);
        }

        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new MySqlCommand()
            {
                Connection = (MySqlConnection)dbConn ?? throw new ArgumentNullException(nameof(dbConn))
            };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText)
        {
            if (dbConn == null)
                throw new ArgumentNullException(nameof(dbConn));

            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            return new MySqlCommand(cmdText, (MySqlConnection)dbConn);
        }
        #endregion
    }
}
