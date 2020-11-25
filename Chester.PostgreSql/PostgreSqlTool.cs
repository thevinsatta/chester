using Npgsql;
using System;
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
        protected override IDbConnection CreateConnection(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw ArgNullOrWhiteSpaceException(nameof(connStr));

            return new NpgsqlConnection(connStr);
        }

        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new NpgsqlCommand()
            {
                Connection = (NpgsqlConnection)dbConn ?? throw new ArgumentNullException(nameof(dbConn))
            };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText)
        {
            if (dbConn == null)
                throw new ArgumentNullException(nameof(dbConn));

            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            return new NpgsqlCommand(cmdText, (NpgsqlConnection)dbConn);
        }
        #endregion
    }
}
