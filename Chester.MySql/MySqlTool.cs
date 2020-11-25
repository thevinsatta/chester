using MySql.Data.MySqlClient;
using System.Data;

namespace Chester.MySql
{
    public class MySqlTool : DbTool
    {
        #region Constructors
        public MySqlTool(string connStr) : base(connStr) { }

        public MySqlTool(IDbConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbConnection CreateConnection() =>
            new MySqlConnection();
        #endregion
    }
}
