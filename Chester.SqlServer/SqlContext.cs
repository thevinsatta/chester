using Chester.Interfaces;
using System.Data;

namespace Chester.SqlServer
{
    public class SqlContext : DbContext
    {
        #region Constructors
        public SqlContext(string connStr) : base(connStr) { }

        public SqlContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new SqlTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        protected override IDbTool CreateDbTool(IDbConnection dbConn)
        {
            var dbTool = new SqlTool(dbConn);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }
        #endregion
    }
}
