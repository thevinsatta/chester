using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Chester.Oracle
{
    public class OracleTool : DbTool
    {
        #region Constructors
        public OracleTool(string connStr) : base(connStr) { }

        public OracleTool(IDbConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbConnection CreateConnection() =>
            new OracleConnection();
        #endregion
    }
}
