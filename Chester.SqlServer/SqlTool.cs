using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Chester.SqlServer
{
    public class SqlTool : DbTool
    {
        #region Constructors
        public SqlTool(string connStr) : base(connStr) { }

        public SqlTool(IDbConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override IDbConnection CreateConnection(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw ArgNullOrWhiteSpaceException(nameof(connStr));

            return new SqlConnection(connStr);
        }

        protected override IDbCommand CreateCommand(IDbConnection dbConn) =>
            new SqlCommand()
            {
                Connection = (SqlConnection)dbConn ?? throw new ArgumentNullException(nameof(dbConn))
            };

        protected override IDbCommand CreateCommand(IDbConnection dbConn, string cmdText)
        {
            if (dbConn == null)
                throw new ArgumentNullException(nameof(dbConn));

            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            return new SqlCommand(cmdText, (SqlConnection)dbConn);
        }

        /// <summary>
        /// This ensures that ARITHABORT is set to ON.
        /// </summary>
        protected override void AugmentConnection()
        {
            SetCommand(CommandType.Text, "SET ARITHABORT ON;");
            _cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
