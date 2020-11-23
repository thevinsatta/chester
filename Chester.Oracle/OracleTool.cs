using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Chester.Oracle
{
    public class OracleTool : DbTool
    {
        #region Constructors
        public OracleTool(string connStr) : base(connStr) { }

        public OracleTool(OracleConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new OracleCommand() { Connection = (OracleConnection)dbConn };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText) =>
            new OracleCommand(cmdText, (OracleConnection)dbConn);

        protected override IDbConnection CreateConnection(string connStr) =>
            new OracleConnection(connStr);
        #endregion
    }
}
