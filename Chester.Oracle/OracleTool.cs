using Oracle.ManagedDataAccess.Client;
using System;
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
        protected override IDbConnection CreateConnection(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw ArgNullOrWhiteSpaceException(nameof(connStr));

            return new OracleConnection(connStr);
        }

        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new OracleCommand()
            {
                Connection = (OracleConnection)dbConn ?? throw new ArgumentNullException(nameof(dbConn))
            };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText)
        {
            if (dbConn == null)
                throw new ArgumentNullException(nameof(dbConn));

            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            return new OracleCommand(cmdText, (OracleConnection)dbConn);
        }
        #endregion
    }
}
