using Chester.Interfaces;
using System.Data;

namespace Chester.Oracle
{
    public class OracleContext : DbContext
    {
        #region Constructors
        public OracleContext(string connStr) : base(connStr) { }

        public OracleContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new OracleTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        protected override IDbTool CreateDbTool(IDbConnection dbConn)
        {
            var dbTool = new OracleTool(dbConn);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }
        #endregion
    }
}
