using Chester.Interfaces;
using System.Data;

namespace Chester.MySql
{
    public class MySqlContext : DbContext
    {
        #region Constructors
        public MySqlContext(string connStr) : base(connStr) { }

        public MySqlContext(string connStr, int? commandTimeout) : base(connStr, commandTimeout) { }
        #endregion

        #region Methods
        protected override IDbTool CreateDbTool(string connStr)
        {
            var dbTool = new MySqlTool(connStr);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }

        protected override IDbTool CreateDbTool(IDbConnection dbConn)
        {
            var dbTool = new MySqlTool(dbConn);

            if (CommandTimeout.HasValue)
                dbTool.CommandTimeout = CommandTimeout.Value;

            return dbTool;
        }
        #endregion
    }
}
