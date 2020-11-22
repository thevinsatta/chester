using Npgsql;
using System.Data.Common;

namespace Chester.PostgreSql
{
    public class PostgreSqlTool : DbTool
    {
        #region Constructors
        public PostgreSqlTool(string connStr) : base(connStr) { }

        public PostgreSqlTool(NpgsqlConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override DbCommand CreateCommand(DbConnection dbConn) =>
            new NpgsqlCommand() { Connection = (NpgsqlConnection)dbConn };

        protected override DbCommand CreateCommand(DbConnection dbConn, string cmdText) =>
            new NpgsqlCommand(cmdText, (NpgsqlConnection)dbConn);

        protected override DbConnection CreateConnection(string connStr) =>
            new NpgsqlConnection(connStr);
        #endregion
    }
}
