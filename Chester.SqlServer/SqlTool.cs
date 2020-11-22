using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Chester.SqlServer
{
    public class SqlTool : DbTool
    {
        #region Constructors
        public SqlTool(string connStr) : base(connStr) { }

        public SqlTool(SqlConnection dbConn) : base(dbConn) { }
        #endregion

        #region Methods
        protected override DbCommand CreateCommand(DbConnection dbConn) =>
            new SqlCommand() { Connection = (SqlConnection)dbConn };

        protected override DbCommand CreateCommand(DbConnection dbConn, string cmdText) =>
            new SqlCommand(cmdText, (SqlConnection)dbConn);

        protected override DbConnection CreateConnection(string connStr) =>
            new SqlConnection(connStr);

        /// <summary>
        /// This ensures that ARITHABORT is set to ON.
        /// </summary>
        protected override void AugmentConnection()
        {
            base.AugmentConnection();

            SetCommand(CommandType.Text, "SET ARITHABORT ON;");
            _cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
