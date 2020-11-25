using Chester.Interfaces;
using System.Data;

namespace Chester.PostgreSql
{
    public class PostgreSqlContext : DbContext
    {
        #region Constructors
        public PostgreSqlContext(string connStr) : base(connStr) { }

        public PostgreSqlContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new PostgreSqlTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        protected override IDbTool CreateDbTool(IDbConnection dbConn)
        {
            var dbTool = new PostgreSqlTool(dbConn);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }
        #endregion
    }
}
