using System;
using System.Collections.Generic;
using System.Data;

namespace Chester.Interfaces
{
    public interface IDbTool : IDisposable
    {
        void OpenConnection();
        void CloseConnection();
        void SetCommandTimeout(int seconds);

        IDbTransaction BeginTransaction();
        IDbTransaction BeginTransaction(IsolationLevel iso);
        void CommitTransaction();
        void RollbackTransaction();

        IDataReader DataReader(CommandType cmdType, string cmdText);
        IDataReader DataReader(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        IDataReader DataReader(CommandBehavior cmdBehavior, CommandType cmdType, string cmdText);
        IDataReader DataReader(CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        int ExecNonQuery(CommandType cmdType, string cmdText);
        int ExecNonQuery(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        object ExecScalar(CommandType cmdType, string cmdText);
        object ExecScalar(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
    }
}
