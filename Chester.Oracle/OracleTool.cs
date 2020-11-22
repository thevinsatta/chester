using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace Chester.Oracle
{
    public class OracleTool : DbTool
    {
        #region Constructors
        public OracleTool(string connStr) : base(connStr) { }

        public OracleTool(OracleConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override DbCommand CreateCommand(DbConnection dbConn) =>
            new OracleCommand() { Connection = (OracleConnection)dbConn };

        protected override DbCommand CreateCommand(DbConnection dbConn, string cmdText) =>
            new OracleCommand(cmdText, (OracleConnection)dbConn);

        protected override DbConnection CreateConnection(string connStr) =>
            new OracleConnection(connStr);
        #endregion
    }
}
