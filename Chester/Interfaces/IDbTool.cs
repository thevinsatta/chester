using Chester.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;

namespace Chester.Interfaces
{
    public interface IDbTool : IDisposable
    {
        int CommandTimeout { get; set; }

        void CloseConnection();

        IDbCommand CreateCommand(CommandType cmdType, string cmdText);
        IDbCommand CreateCommand(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        DataReaderCommand DataReader(CommandType cmdType, string cmdText);
        DataReaderCommand DataReader(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        DataReaderCommand DataReader(CommandBehavior cmdBehavior, CommandType cmdType, string cmdText);
        DataReaderCommand DataReader(CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        int ExecNonQuery(CommandType cmdType, string cmdText);
        int ExecNonQuery(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        T ExecScalar<T>(CommandType cmdType, string cmdText);
        T ExecScalar<T>(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
    }
}
