using Npgsql;
using System.Data;

namespace Chester.PostgreSql
{
    public class PostgreSqlTool : DbTool
    {
        #region Constructors
        public PostgreSqlTool(string connStr) : base(connStr) { }

        public PostgreSqlTool(NpgsqlConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new NpgsqlCommand() { Connection = (NpgsqlConnection)dbConn };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText) =>
            new NpgsqlCommand(cmdText, (NpgsqlConnection)dbConn);

        protected override IDbConnection CreateConnection(string connStr) =>
            new NpgsqlConnection(connStr);
        #endregion
    }
}
