using Npgsql;
using System.Data;

namespace Chester.PostgreSql
{
    public class PostgreSqlTool : DbTool
    {
        #region Constructors
        public PostgreSqlTool(string connStr) : base(connStr) { }

        public PostgreSqlTool(IDbConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbConnection CreateConnection() =>
            new NpgsqlConnection();
        #endregion
    }
}
