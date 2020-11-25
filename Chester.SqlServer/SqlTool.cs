using Microsoft.Data.SqlClient;
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
        protected override IDbConnection CreateConnection() =>
            new SqlConnection();

        /// <summary>
        /// This ensures that ARITHABORT is set to ON.
        /// </summary>
        protected override void AugmentConnection()
        {
            using var cmd = CreateCommand(CommandType.Text, "SET ARITHABORT ON;");
            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
